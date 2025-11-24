using Microsoft.EntityFrameworkCore;
using prisonbreak.Server.Data;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;
using System.Linq;

namespace prisonbreak.Server.Services;

/// <summary>
/// Service de gestion des puzzles Hashi
/// Gère la génération, la récupération et la conversion des puzzles
/// </summary>
public class PuzzleService : IPuzzleService
{
    private readonly HashiDbContext _context;
    private readonly Random _random = new();

    public PuzzleService(HashiDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Génère un nouveau puzzle Hashi avec une solution valide
    /// Place les îles de manière structurée et génère une solution cohérente
    /// </summary>
    public async Task<Puzzle> GeneratePuzzleAsync(int width, int height, DifficultyLevel difficulty)
    {
        // Créer le puzzle
        var puzzle = new Puzzle
        {
            Name = $"Puzzle {difficulty} ({width}x{height})",
            Width = width,
            Height = height,
            Difficulty = difficulty,
            CreatedAt = DateTime.UtcNow
        };

        // Générer les îles et la solution selon la difficulté
        var (islands, solutionBridges) = GenerateValidPuzzle(width, height, difficulty);

        // Assigner les îles au puzzle
        foreach (var island in islands)
        {
            island.Puzzle = puzzle;
        }
        puzzle.Islands = islands;

        // Sauvegarder d'abord le puzzle et les îles pour obtenir les IDs
        _context.Puzzles.Add(puzzle);
        await _context.SaveChangesAsync();

        // Recharger les îles depuis la base de données pour obtenir leurs IDs
        var savedIslands = await _context.Islands
            .Where(i => i.PuzzleId == puzzle.Id)
            .ToListAsync();

        // Maintenant que les îles ont des IDs, créer les ponts avec les IDs corrects
        var bridgesToAdd = new List<Bridge>();
        foreach (var bridgeTemplate in solutionBridges)
        {
            // Trouver les îles correspondantes par position (X, Y) dans les îles sauvegardées
            var fromIsland = savedIslands.FirstOrDefault(i => 
                i.X == bridgeTemplate.FromIsland!.X && 
                i.Y == bridgeTemplate.FromIsland.Y);
            var toIsland = savedIslands.FirstOrDefault(i => 
                i.X == bridgeTemplate.ToIsland!.X && 
                i.Y == bridgeTemplate.ToIsland.Y);

            if (fromIsland != null && toIsland != null)
            {
                bridgesToAdd.Add(new Bridge
                {
                    FromIslandId = fromIsland.Id,
                    ToIslandId = toIsland.Id,
                    FromIsland = fromIsland,
                    ToIsland = toIsland,
                    IsDouble = bridgeTemplate.IsDouble,
                    Direction = bridgeTemplate.Direction,
                    PuzzleId = puzzle.Id,
                    Puzzle = puzzle
                });
            }
        }

        // Calculer les RequiredBridges pour chaque île en fonction de la solution
        CalculateRequiredBridges(savedIslands, bridgesToAdd);

        // Mettre à jour les RequiredBridges dans la base de données
        foreach (var island in savedIslands)
        {
            _context.Islands.Update(island);
        }

        // Ajouter les ponts de la solution
        _context.Bridges.AddRange(bridgesToAdd);
        puzzle.SolutionBridges = bridgesToAdd;

        // Sauvegarder les modifications
        await _context.SaveChangesAsync();

        return puzzle;
    }

    /// <summary>
    /// Génère un puzzle valide avec des îles alignées et une solution
    /// </summary>
    private (List<Island> islands, List<Bridge> bridges) GenerateValidPuzzle(int width, int height, DifficultyLevel difficulty)
    {
        var islands = new List<Island>();
        var bridges = new List<Bridge>();
        var usedPositions = new HashSet<(int, int)>();

        // Pour le niveau facile (5x5), créer un puzzle simple et structuré
        if (difficulty == DifficultyLevel.Easy && width == 5 && height == 5)
        {
            // Créer un puzzle simple avec 6 îles bien alignées
            // Positions choisies pour permettre des connexions simples
            var positions = new List<(int x, int y)>
            {
                (0, 0), (2, 0), (4, 0),  // Ligne du haut
                (2, 2),                   // Centre
                (0, 4), (4, 4)            // Ligne du bas
            };

            foreach (var (x, y) in positions)
            {
                islands.Add(new Island
                {
                    Id = 0, // Sera assigné par EF Core
                    X = x,
                    Y = y,
                    RequiredBridges = 0 // Sera calculé après
                });
                usedPositions.Add((x, y));
            }

            // Créer une solution simple : connecter les îles en formant un réseau connecté
            // Toutes les îles doivent être connectées directement ou indirectement
            
            // Ligne du haut : (0,0) -> (2,0) -> (4,0)
            bridges.Add(CreateBridge(islands[0], islands[1], false, BridgeDirection.Horizontal));
            bridges.Add(CreateBridge(islands[1], islands[2], false, BridgeDirection.Horizontal));
            
            // Centre : (2,0) -> (2,2) verticalement
            bridges.Add(CreateBridge(islands[1], islands[3], false, BridgeDirection.Vertical));
            
            // Connecter le centre aux îles du bas
            // (2,2) -> (0,4) : on ne peut pas car pas alignés
            // (2,2) -> (4,4) : on ne peut pas car pas alignés
            // Solution : connecter (0,0) -> (0,4) et (4,0) -> (4,4) pour créer un réseau complet
            bridges.Add(CreateBridge(islands[0], islands[4], false, BridgeDirection.Vertical));
            bridges.Add(CreateBridge(islands[2], islands[5], false, BridgeDirection.Vertical));
            
            // Maintenant toutes les îles sont connectées :
            // (0,0) <-> (2,0) <-> (4,0)
            //   |         |         |
            //   |      (2,2)         |
            //   |                    |
            // (0,4)                (4,4)
        }
        else
        {
            // Pour les autres difficultés, générer un puzzle plus complexe
            int islandCount = difficulty switch
            {
                DifficultyLevel.Easy => Math.Min(width * height / 4, 8),
                DifficultyLevel.Medium => Math.Min(width * height / 3, 12),
                DifficultyLevel.Hard => Math.Min(width * height / 2, 16),
                DifficultyLevel.Expert => Math.Min(width * height / 2, 20),
                _ => 8
            };

            // Placer les îles de manière structurée (sur des lignes/colonnes)
            int placed = 0;
            for (int y = 1; y < height - 1 && placed < islandCount; y += 2)
            {
                for (int x = 1; x < width - 1 && placed < islandCount; x += 2)
                {
                    if (!usedPositions.Contains((x, y)))
                    {
                        islands.Add(new Island
                        {
                            Id = 0,
                            X = x,
                            Y = y,
                            RequiredBridges = 0
                        });
                        usedPositions.Add((x, y));
                        placed++;
                    }
                }
            }

            // Générer une solution simple : connecter les îles en chaîne
            for (int i = 0; i < islands.Count - 1; i++)
            {
                var from = islands[i];
                var to = islands[i + 1];

                // Déterminer la direction et vérifier l'alignement
                BridgeDirection direction;
                if (from.X == to.X)
                {
                    direction = BridgeDirection.Vertical;
                    // Vérifier qu'il n'y a pas d'île entre les deux
                    bool canConnect = true;
                    for (int y = Math.Min(from.Y, to.Y) + 1; y < Math.Max(from.Y, to.Y); y++)
                    {
                        if (usedPositions.Contains((from.X, y)))
                        {
                            canConnect = false;
                            break;
                        }
                    }
                    if (canConnect)
                    {
                        bridges.Add(CreateBridge(from, to, false, direction));
                    }
                }
                else if (from.Y == to.Y)
                {
                    direction = BridgeDirection.Horizontal;
                    // Vérifier qu'il n'y a pas d'île entre les deux
                    bool canConnect = true;
                    for (int x = Math.Min(from.X, to.X) + 1; x < Math.Max(from.X, to.X); x++)
                    {
                        if (usedPositions.Contains((x, from.Y)))
                        {
                            canConnect = false;
                            break;
                        }
                    }
                    if (canConnect)
                    {
                        bridges.Add(CreateBridge(from, to, false, direction));
                    }
                }
            }
        }

        return (islands, bridges);
    }

    /// <summary>
    /// Crée un pont entre deux îles
    /// </summary>
    private Bridge CreateBridge(Island from, Island to, bool isDouble, BridgeDirection direction)
    {
        return new Bridge
        {
            FromIsland = from,
            ToIsland = to,
            IsDouble = isDouble,
            Direction = direction
        };
    }

    /// <summary>
    /// Calcule les RequiredBridges pour chaque île en fonction de la solution
    /// </summary>
    private void CalculateRequiredBridges(List<Island> islands, List<Bridge> bridges)
    {
        // Initialiser tous les RequiredBridges à 0
        foreach (var island in islands)
        {
            island.RequiredBridges = 0;
        }

        // Compter les ponts pour chaque île en utilisant les références d'objets
        foreach (var bridge in bridges)
        {
            if (bridge.FromIsland != null)
            {
                bridge.FromIsland.RequiredBridges += bridge.IsDouble ? 2 : 1;
            }

            if (bridge.ToIsland != null)
            {
                bridge.ToIsland.RequiredBridges += bridge.IsDouble ? 2 : 1;
            }
        }

        // S'assurer qu'aucune île n'a 0 ponts requis (minimum 1)
        foreach (var island in islands)
        {
            if (island.RequiredBridges == 0)
            {
                island.RequiredBridges = 1; // Au minimum 1 pont
            }
        }
    }

    /// <summary>
    /// Récupère un puzzle par son ID avec toutes ses îles
    /// </summary>
    public async Task<Puzzle?> GetPuzzleByIdAsync(int puzzleId)
    {
        return await _context.Puzzles
            .Include(p => p.Islands)
            .Include(p => p.SolutionBridges)
            .FirstOrDefaultAsync(p => p.Id == puzzleId);
    }

    /// <summary>
    /// Récupère tous les puzzles
    /// </summary>
    public async Task<List<Puzzle>> GetAllPuzzlesAsync()
    {
        return await _context.Puzzles
            .Include(p => p.Islands)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère les puzzles par difficulté
    /// Si aucun puzzle n'existe pour cette difficulté, en génère un avec les dimensions par défaut
    /// </summary>
    public async Task<List<Puzzle>> GetPuzzlesByDifficultyAsync(DifficultyLevel difficulty)
    {
        var puzzles = await _context.Puzzles
            .Include(p => p.Islands)
            .Where(p => p.Difficulty == difficulty)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        // Si aucun puzzle n'existe pour cette difficulté, en générer un avec les dimensions par défaut
        if (puzzles.Count == 0)
        {
            var (width, height) = GetDefaultDimensionsForDifficulty(difficulty);
            var newPuzzle = await GeneratePuzzleAsync(width, height, difficulty);
            puzzles.Add(newPuzzle);
        }

        return puzzles;
    }

    /// <summary>
    /// Retourne les dimensions par défaut selon la difficulté
    /// Facile : 5x5, Moyen : 8x8, Difficile : 12x12, Expert : 15x15
    /// </summary>
    private (int width, int height) GetDefaultDimensionsForDifficulty(DifficultyLevel difficulty)
    {
        return difficulty switch
        {
            DifficultyLevel.Easy => (5, 5),
            DifficultyLevel.Medium => (8, 8),
            DifficultyLevel.Hard => (12, 12),
            DifficultyLevel.Expert => (15, 15),
            _ => (5, 5)
        };
    }

    /// <summary>
    /// Convertit un Puzzle en PuzzleDto pour l'envoyer au frontend
    /// </summary>
    public PuzzleDto ConvertToDto(Puzzle puzzle)
    {
        if (puzzle == null)
        {
            throw new ArgumentNullException(nameof(puzzle));
        }

        var islands = puzzle.Islands ?? new List<Island>();

        return new PuzzleDto
        {
            Id = puzzle.Id,
            Name = puzzle.Name,
            Width = puzzle.Width,
            Height = puzzle.Height,
            Difficulty = (int)puzzle.Difficulty,
            IslandCount = islands.Count,
            Islands = islands.Select(i => new IslandDto
            {
                Id = i.Id,
                X = i.X,
                Y = i.Y,
                RequiredBridges = i.RequiredBridges,
                CurrentBridges = 0 // Le joueur commence avec 0 ponts placés
            }).ToList()
        };
    }
}

