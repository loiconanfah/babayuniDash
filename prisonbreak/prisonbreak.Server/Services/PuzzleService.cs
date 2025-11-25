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

        // VALIDATION : Vérifier que toutes les îles sont connectées
        if (!ValidatePuzzleConnectivity(savedIslands, bridgesToAdd))
        {
            throw new InvalidOperationException("Le puzzle généré n'est pas valide : toutes les îles ne sont pas connectées");
        }

        // VALIDATION : Vérifier que les RequiredBridges correspondent aux ponts
        ValidateRequiredBridges(savedIslands, bridgesToAdd);

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
    /// Génère un puzzle valide avec des îles alignées et une solution connectée
    /// Crée des puzzles logiques, variés et résolvables pour chaque niveau
    /// </summary>
    private (List<Island> islands, List<Bridge> bridges) GenerateValidPuzzle(int width, int height, DifficultyLevel difficulty)
    {
        var islands = new List<Island>();
        var bridges = new List<Bridge>();
        var usedPositions = new HashSet<(int, int)>();

        // Patterns prédéfinis pour chaque difficulté et taille
        if (width == 5 && height == 5)
        {
            return Generate5x5Puzzle(difficulty);
        }
        else if (width == 8 && height == 8)
        {
            return Generate8x8Puzzle(difficulty);
        }
        else if (width == 12 && height == 12)
        {
            return Generate12x12Puzzle(difficulty);
        }
        else if (width == 15 && height == 15)
        {
            return Generate15x15Puzzle(difficulty);
        }
        else
        {
            // Génération générique pour autres tailles
            return GenerateGenericPuzzle(width, height, difficulty);
        }
    }

    /// <summary>
    /// Génère un puzzle 5x5 pour niveau Facile
    /// </summary>
    private (List<Island> islands, List<Bridge> bridges) Generate5x5Puzzle(DifficultyLevel difficulty)
    {
        var islands = new List<Island>();
        var bridges = new List<Bridge>();

        // Pattern 5x5 avec îles alignées correctement (même colonne ou même ligne)
        // IMPORTANT : RequiredBridges sera calculé après
        var positions = new List<(int x, int y)>
        {
            (1, 0),  // Colonne x=1
            (3, 0),  // Colonne x=3
            (1, 2),  // Colonne x=1, ligne y=2
            (3, 2),  // Hub central - colonne x=3, ligne y=2
            (1, 4),  // Colonne x=1
            (3, 4)   // Colonne x=3
        };

        foreach (var (x, y) in positions)
        {
            islands.Add(new Island
            {
                Id = 0,
                X = x,
                Y = y,
                RequiredBridges = 0 // Sera calculé après
            });
        }

        // Solution : TOUTES les îles connectées en un seul réseau
        // Colonne x=1 : connexions verticales
        bridges.Add(CreateBridge(islands[2], islands[0], false, BridgeDirection.Vertical)); // (1,2) -> (1,0) vertical ✓
        bridges.Add(CreateBridge(islands[2], islands[4], false, BridgeDirection.Vertical)); // (1,2) -> (1,4) vertical ✓

        // Colonne x=3 : connexions verticales
        bridges.Add(CreateBridge(islands[3], islands[1], false, BridgeDirection.Vertical)); // Hub (3,2) -> (3,0) vertical ✓
        bridges.Add(CreateBridge(islands[3], islands[5], false, BridgeDirection.Vertical)); // Hub (3,2) -> (3,4) vertical ✓

        // Connexion horizontale entre les colonnes à y=2
        bridges.Add(CreateBridge(islands[2], islands[3], false, BridgeDirection.Horizontal)); // (1,2) -> Hub (3,2) horizontal ✓

        // Connexion horizontale en haut à y=0
        bridges.Add(CreateBridge(islands[0], islands[1], false, BridgeDirection.Horizontal)); // (1,0) -> (3,0) horizontal ✓

        // Résultat : Toutes les 6 îles sont connectées en un seul réseau
        // Colonne x=1 : (1,0) <-> (1,2) <-> (1,4)
        // Colonne x=3 : (3,0) <-> Hub (3,2) <-> (3,4)
        // Connexions : (1,0) <-> (3,0) horizontal, (1,2) <-> Hub (3,2) horizontal
        // TOUTES CONNECTÉES EN UN SEUL RÉSEAU ✓

        return (islands, bridges);
    }

    /// <summary>
    /// Génère un puzzle 8x8 pour niveau Moyen
    /// </summary>
    private (List<Island> islands, List<Bridge> bridges) Generate8x8Puzzle(DifficultyLevel difficulty)
    {
        var islands = new List<Island>();
        var bridges = new List<Bridge>();

        // Pattern 8x8 - TOUTES les îles alignées correctement pour respecter les règles Hashi
        // IMPORTANT : RequiredBridges sera calculé après
        var positions = new List<(int x, int y)>
        {
            (2, 1),  // Colonne x=2
            (2, 3),  // Hub - colonne x=2
            (2, 5),  // Colonne x=2
            (6, 1),  // Colonne x=6
            (6, 3),  // Colonne x=6
            (6, 5),  // Colonne x=6
            (4, 5),  // Ligne y=5 (connecte les deux colonnes)
            (4, 7)   // Ligne y=7
        };

        foreach (var (x, y) in positions)
        {
            islands.Add(new Island
            {
                Id = 0,
                X = x,
                Y = y,
                RequiredBridges = 0 // Sera calculé après
            });
        }

        // Solution : TOUTES les îles connectées en un seul réseau
        // Colonne x=2 : connexions verticales
        bridges.Add(CreateBridge(islands[1], islands[0], false, BridgeDirection.Vertical)); // Hub (2,3) -> (2,1)
        bridges.Add(CreateBridge(islands[1], islands[2], false, BridgeDirection.Vertical)); // Hub (2,3) -> (2,5)

        // Colonne x=6 : connexions verticales
        bridges.Add(CreateBridge(islands[4], islands[3], false, BridgeDirection.Vertical)); // (6,3) -> (6,1)
        bridges.Add(CreateBridge(islands[4], islands[5], false, BridgeDirection.Vertical)); // (6,3) -> (6,5)

        // Connexion horizontale entre les colonnes à y=3
        bridges.Add(CreateBridge(islands[1], islands[4], false, BridgeDirection.Horizontal)); // Hub (2,3) -> (6,3)

        // Connexion horizontale à y=5 pour relier (2,5) et (6,5) via (4,5)
        bridges.Add(CreateBridge(islands[2], islands[6], false, BridgeDirection.Horizontal)); // (2,5) -> (4,5)
        bridges.Add(CreateBridge(islands[6], islands[5], false, BridgeDirection.Horizontal)); // (4,5) -> (6,5)

        // Connexion verticale de (4,5) à (4,7)
        bridges.Add(CreateBridge(islands[6], islands[7], false, BridgeDirection.Vertical)); // (4,5) -> (4,7)

        // Vérification : Toutes les 8 îles sont connectées
        // Colonne x=2 : (2,1) <-> Hub (2,3) <-> (2,5) <-> (4,5) <-> (4,7)
        // Colonne x=6 : (6,1) <-> (6,3) <-> (6,5) <-> (4,5)
        // Connexion : Hub (2,3) <-> (6,3)
        // TOUTES CONNECTÉES ✓

        return (islands, bridges);
    }

    /// <summary>
    /// Génère un puzzle 12x12 pour niveau Difficile
    /// </summary>
    private (List<Island> islands, List<Bridge> bridges) Generate12x12Puzzle(DifficultyLevel difficulty)
    {
        var islands = new List<Island>();
        var bridges = new List<Bridge>();

        // Pattern 12x12 avec îles alignées correctement - TOUTES les connexions respectent l'alignement
        // IMPORTANT : RequiredBridges sera calculé après
        var positions = new List<(int x, int y)>
        {
            (3, 2),   // Colonne x=3
            (9, 2),   // Colonne x=9
            (3, 4),   // Colonne x=3, ligne y=4
            (9, 4),   // Hub - colonne x=9, ligne y=4
            (3, 6),   // Colonne x=3
            (9, 6),   // Colonne x=9
            (3, 8),   // Colonne x=3, ligne y=8
            (9, 8),   // Colonne x=9, ligne y=8
            (6, 8),   // Hub secondaire - ligne y=8
            (3, 10),  // Colonne x=3
            (9, 10),  // Colonne x=9
            (6, 11)   // Ligne y=11
        };

        foreach (var (x, y) in positions)
        {
            islands.Add(new Island
            {
                Id = 0,
                X = x,
                Y = y,
                RequiredBridges = 0 // Sera calculé après
            });
        }

        // Solution : TOUTES les îles connectées en un seul réseau
        // Colonne x=3 : connexions verticales
        bridges.Add(CreateBridge(islands[2], islands[0], false, BridgeDirection.Vertical)); // (3,4) -> (3,2) vertical ✓
        bridges.Add(CreateBridge(islands[2], islands[4], false, BridgeDirection.Vertical)); // (3,4) -> (3,6) vertical ✓
        bridges.Add(CreateBridge(islands[6], islands[4], false, BridgeDirection.Vertical)); // (3,8) -> (3,6) vertical ✓
        bridges.Add(CreateBridge(islands[6], islands[9], false, BridgeDirection.Vertical)); // (3,8) -> (3,10) vertical ✓

        // Colonne x=9 : connexions verticales
        bridges.Add(CreateBridge(islands[3], islands[1], false, BridgeDirection.Vertical)); // Hub (9,4) -> (9,2) vertical ✓
        bridges.Add(CreateBridge(islands[3], islands[5], false, BridgeDirection.Vertical)); // Hub (9,4) -> (9,6) vertical ✓
        bridges.Add(CreateBridge(islands[7], islands[5], false, BridgeDirection.Vertical)); // (9,8) -> (9,6) vertical ✓
        bridges.Add(CreateBridge(islands[7], islands[10], false, BridgeDirection.Vertical)); // (9,8) -> (9,10) vertical ✓

        // Connexions horizontales à y=4
        bridges.Add(CreateBridge(islands[2], islands[3], false, BridgeDirection.Horizontal)); // (3,4) -> Hub (9,4) horizontal ✓

        // Connexions horizontales à y=8
        bridges.Add(CreateBridge(islands[6], islands[8], false, BridgeDirection.Horizontal)); // (3,8) -> Hub2 (6,8) horizontal ✓
        bridges.Add(CreateBridge(islands[8], islands[7], false, BridgeDirection.Horizontal)); // Hub2 (6,8) -> (9,8) horizontal ✓

        // Connexion verticale de Hub2
        bridges.Add(CreateBridge(islands[8], islands[11], false, BridgeDirection.Vertical)); // Hub2 (6,8) -> (6,11) vertical ✓

        // Vérification : Toutes les 12 îles sont connectées
        // Colonne x=3 : (3,2) <-> (3,4) <-> (3,6) <-> (3,8) <-> (3,10)
        // Colonne x=9 : (9,2) <-> Hub (9,4) <-> (9,6) <-> (9,8) <-> (9,10)
        // Ligne y=4 : (3,4) <-> Hub (9,4)
        // Ligne y=8 : (3,8) <-> Hub2 (6,8) <-> (9,8)
        // Hub2 (6,8) <-> (6,11)
        // TOUTES CONNECTÉES EN UN SEUL RÉSEAU ✓

        return (islands, bridges);
    }

    /// <summary>
    /// Génère un puzzle 15x15 pour niveau Expert
    /// </summary>
    private (List<Island> islands, List<Bridge> bridges) Generate15x15Puzzle(DifficultyLevel difficulty)
    {
        var islands = new List<Island>();
        var bridges = new List<Bridge>();

        // Pattern 15x15 avec îles alignées correctement - TOUTES les connexions respectent l'alignement
        // IMPORTANT : RequiredBridges sera calculé après
        var positions = new List<(int x, int y)>
        {
            (3, 2),   // Colonne x=3
            (11, 2),  // Colonne x=11
            (3, 4),   // Colonne x=3, ligne y=4
            (7, 4),   // Hub principal - ligne y=4
            (11, 4),  // Colonne x=11, ligne y=4
            (3, 6),   // Colonne x=3
            (11, 6),  // Colonne x=11
            (3, 8),   // Colonne x=3, ligne y=8
            (5, 8),   // Hub secondaire - ligne y=8
            (9, 8),   // Hub secondaire - ligne y=8
            (11, 8),  // Colonne x=11, ligne y=8
            (3, 10),  // Colonne x=3
            (11, 10), // Colonne x=11
            (3, 12),  // Colonne x=3, ligne y=12
            (7, 12),  // Hub - ligne y=12
            (11, 12), // Colonne x=11, ligne y=12
            (3, 14),  // Colonne x=3
            (11, 14)  // Colonne x=11
        };

        foreach (var (x, y) in positions)
        {
            islands.Add(new Island
            {
                Id = 0,
                X = x,
                Y = y,
                RequiredBridges = 0 // Sera calculé après
            });
        }

        // Solution : TOUTES les îles connectées en un seul réseau
        // Colonne x=3 : connexions verticales
        bridges.Add(CreateBridge(islands[2], islands[0], false, BridgeDirection.Vertical)); // (3,4) -> (3,2) vertical ✓
        bridges.Add(CreateBridge(islands[2], islands[5], false, BridgeDirection.Vertical)); // (3,4) -> (3,6) vertical ✓
        bridges.Add(CreateBridge(islands[7], islands[5], false, BridgeDirection.Vertical)); // (3,8) -> (3,6) vertical ✓
        bridges.Add(CreateBridge(islands[7], islands[11], false, BridgeDirection.Vertical)); // (3,8) -> (3,10) vertical ✓
        bridges.Add(CreateBridge(islands[12], islands[11], false, BridgeDirection.Vertical)); // (3,12) -> (3,10) vertical ✓
        bridges.Add(CreateBridge(islands[12], islands[15], false, BridgeDirection.Vertical)); // (3,12) -> (3,14) vertical ✓

        // Colonne x=11 : connexions verticales
        bridges.Add(CreateBridge(islands[4], islands[1], false, BridgeDirection.Vertical)); // (11,4) -> (11,2) vertical ✓
        bridges.Add(CreateBridge(islands[4], islands[6], false, BridgeDirection.Vertical)); // (11,4) -> (11,6) vertical ✓
        bridges.Add(CreateBridge(islands[10], islands[6], false, BridgeDirection.Vertical)); // (11,8) -> (11,6) vertical ✓
        bridges.Add(CreateBridge(islands[10], islands[12], false, BridgeDirection.Vertical)); // (11,8) -> (11,10) vertical ✓
        bridges.Add(CreateBridge(islands[14], islands[12], false, BridgeDirection.Vertical)); // (11,12) -> (11,10) vertical ✓
        bridges.Add(CreateBridge(islands[14], islands[16], false, BridgeDirection.Vertical)); // (11,12) -> (11,14) vertical ✓

        // Hub principal (7,4) - connexions horizontales à y=4
        bridges.Add(CreateBridge(islands[2], islands[3], false, BridgeDirection.Horizontal)); // (3,4) -> Hub (7,4) horizontal ✓
        bridges.Add(CreateBridge(islands[3], islands[4], false, BridgeDirection.Horizontal)); // Hub (7,4) -> (11,4) horizontal ✓

        // Connexions horizontales à y=8
        bridges.Add(CreateBridge(islands[7], islands[8], false, BridgeDirection.Horizontal)); // (3,8) -> Hub2 (5,8) horizontal ✓
        bridges.Add(CreateBridge(islands[8], islands[9], true, BridgeDirection.Horizontal)); // Hub2 (5,8) -> Hub3 (9,8) pont double ✓
        bridges.Add(CreateBridge(islands[9], islands[10], false, BridgeDirection.Horizontal)); // Hub3 (9,8) -> (11,8) horizontal ✓

        // Connexions horizontales à y=12
        bridges.Add(CreateBridge(islands[12], islands[13], false, BridgeDirection.Horizontal)); // (3,12) -> Hub (7,12) horizontal ✓
        bridges.Add(CreateBridge(islands[13], islands[14], false, BridgeDirection.Horizontal)); // Hub (7,12) -> (11,12) horizontal ✓

        // Vérification : Toutes les 17 îles sont connectées
        // Colonne x=3 : (3,2) <-> (3,4) <-> (3,6) <-> (3,8) <-> (3,10) <-> (3,12) <-> (3,14)
        // Colonne x=11 : (11,2) <-> (11,4) <-> (11,6) <-> (11,8) <-> (11,10) <-> (11,12) <-> (11,14)
        // Ligne y=4 : (3,4) <-> Hub (7,4) <-> (11,4)
        // Ligne y=8 : (3,8) <-> Hub2 (5,8) <-> Hub3 (9,8) <-> (11,8)
        // Ligne y=12 : (3,12) <-> Hub (7,12) <-> (11,12)
        // TOUTES CONNECTÉES EN UN SEUL RÉSEAU ✓

        return (islands, bridges);
    }

    /// <summary>
    /// Génère un puzzle générique pour autres tailles
    /// </summary>
    private (List<Island> islands, List<Bridge> bridges) GenerateGenericPuzzle(int width, int height, DifficultyLevel difficulty)
    {
        var islands = new List<Island>();
        var bridges = new List<Bridge>();
        var usedPositions = new HashSet<(int, int)>();

        int islandCount = difficulty switch
        {
            DifficultyLevel.Easy => Math.Max(4, Math.Min(width * height / 8, 6)),
            DifficultyLevel.Medium => Math.Max(6, Math.Min(width * height / 6, 10)),
            DifficultyLevel.Hard => Math.Max(8, Math.Min(width * height / 4, 15)),
            DifficultyLevel.Expert => Math.Max(10, Math.Min(width * height / 3, 20)),
            _ => 5
        };

        // Placer les îles de manière variée
        var positions = new List<(int x, int y)>();
        int attempts = 0;
        int maxAttempts = 1000;

        while (positions.Count < islandCount && attempts < maxAttempts)
        {
            int x = _random.Next(1, width - 1);
            int y = _random.Next(1, height - 1);

            // Éviter les positions trop proches (minimum 2 cases d'écart)
            bool tooClose = false;
            foreach (var (px, py) in positions)
            {
                int distance = Math.Abs(x - px) + Math.Abs(y - py);
                if (distance < 2)
                {
                    tooClose = true;
                    break;
                }
            }

            if (!tooClose && !usedPositions.Contains((x, y)))
            {
                positions.Add((x, y));
                usedPositions.Add((x, y));
            }
            attempts++;
        }

        // Créer les îles
        foreach (var (x, y) in positions)
        {
            islands.Add(new Island
            {
                Id = 0,
                X = x,
                Y = y,
                RequiredBridges = 0
            });
        }

        // Générer une solution connectée
        var connectedIslands = new HashSet<int> { 0 };
        var unconnectedIslands = new HashSet<int>(Enumerable.Range(1, islands.Count - 1));

        while (unconnectedIslands.Count > 0)
        {
            var bestConnection = FindBestConnection(islands, connectedIslands, unconnectedIslands, usedPositions);
            
            if (bestConnection.HasValue)
            {
                var (fromIdx, toIdx, isDouble) = bestConnection.Value;
                var from = islands[fromIdx];
                var to = islands[toIdx];
                BridgeDirection direction = from.X == to.X ? BridgeDirection.Vertical : BridgeDirection.Horizontal;
                bridges.Add(CreateBridge(from, to, isDouble, direction));
                
                if (connectedIslands.Contains(fromIdx))
                {
                    unconnectedIslands.Remove(toIdx);
                    connectedIslands.Add(toIdx);
                }
                else
                {
                    unconnectedIslands.Remove(fromIdx);
                    connectedIslands.Add(fromIdx);
                }
            }
            else
            {
                // Fallback : connecter n'importe quelle île
                bool found = false;
                foreach (var connectedIdx in connectedIslands.ToList())
                {
                    foreach (var unconnectedIdx in unconnectedIslands.ToList())
                    {
                        var from = islands[connectedIdx];
                        var to = islands[unconnectedIdx];
                        
                        if (CanConnectIslands(from, to, usedPositions))
                        {
                            BridgeDirection direction = from.X == to.X ? BridgeDirection.Vertical : BridgeDirection.Horizontal;
                            bool isDouble = _random.Next(100) < 15;
                            bridges.Add(CreateBridge(from, to, isDouble, direction));
                            unconnectedIslands.Remove(unconnectedIdx);
                            connectedIslands.Add(unconnectedIdx);
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                }
                if (!found) break;
            }
        }

        // Ajouter quelques ponts supplémentaires selon la difficulté
        int extraBridges = difficulty switch
        {
            DifficultyLevel.Easy => 0,
            DifficultyLevel.Medium => Math.Min(1, islands.Count / 4),
            DifficultyLevel.Hard => Math.Min(2, islands.Count / 3),
            DifficultyLevel.Expert => Math.Min(3, islands.Count / 2),
            _ => 0
        };

        for (int i = 0; i < extraBridges; i++)
        {
            var candidates = new List<(int from, int to)>();
            for (int j = 0; j < islands.Count; j++)
            {
                for (int k = j + 1; k < islands.Count; k++)
                {
                    if (CanConnectIslands(islands[j], islands[k], usedPositions))
                    {
                        bool alreadyConnected = bridges.Any(b => 
                            (b.FromIsland == islands[j] && b.ToIsland == islands[k]) ||
                            (b.FromIsland == islands[k] && b.ToIsland == islands[j]));
                        
                        if (!alreadyConnected)
                        {
                            candidates.Add((j, k));
                        }
                    }
                }
            }

            if (candidates.Count > 0)
            {
                var (fromIdx, toIdx) = candidates[_random.Next(candidates.Count)];
                var from = islands[fromIdx];
                var to = islands[toIdx];
                BridgeDirection direction = from.X == to.X ? BridgeDirection.Vertical : BridgeDirection.Horizontal;
                bool isDouble = _random.Next(100) < 25;
                bridges.Add(CreateBridge(from, to, isDouble, direction));
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

        // Compter les ponts pour chaque île
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
                // Si une île n'a aucun pont, c'est une erreur - toutes les îles doivent être connectées
                throw new InvalidOperationException($"L'île à ({island.X}, {island.Y}) n'a aucun pont dans la solution. Toutes les îles doivent être connectées.");
            }
        }
    }

    /// <summary>
    /// Valide que toutes les îles sont connectées en un seul réseau
    /// </summary>
    private bool ValidatePuzzleConnectivity(List<Island> islands, List<Bridge> bridges)
    {
        if (islands.Count == 0) return true;
        if (islands.Count == 1) return true;

        // Créer un graphe d'adjacence
        var adjacency = new Dictionary<int, List<int>>();
        foreach (var island in islands)
        {
            adjacency[island.Id] = new List<int>();
        }

        // Ajouter les connexions des ponts
        foreach (var bridge in bridges)
        {
            if (adjacency.ContainsKey(bridge.FromIslandId) && adjacency.ContainsKey(bridge.ToIslandId))
            {
                adjacency[bridge.FromIslandId].Add(bridge.ToIslandId);
                adjacency[bridge.ToIslandId].Add(bridge.FromIslandId);
            }
        }

        // Effectuer un DFS à partir de la première île
        var visited = new HashSet<int>();
        DFSConnectivity(islands[0].Id, adjacency, visited);

        // Vérifier si toutes les îles ont été visitées
        return visited.Count == islands.Count;
    }

    /// <summary>
    /// Algorithme DFS pour vérifier la connectivité
    /// </summary>
    private void DFSConnectivity(int islandId, Dictionary<int, List<int>> adjacency, HashSet<int> visited)
    {
        visited.Add(islandId);

        if (!adjacency.ContainsKey(islandId))
            return;

        foreach (var neighborId in adjacency[islandId])
        {
            if (!visited.Contains(neighborId))
            {
                DFSConnectivity(neighborId, adjacency, visited);
            }
        }
    }

    /// <summary>
    /// Valide que les RequiredBridges correspondent exactement aux ponts créés
    /// </summary>
    private void ValidateRequiredBridges(List<Island> islands, List<Bridge> bridges)
    {
        foreach (var island in islands)
        {
            int actualCount = 0;
            foreach (var bridge in bridges)
            {
                if (bridge.FromIslandId == island.Id || bridge.ToIslandId == island.Id)
                {
                    actualCount += bridge.IsDouble ? 2 : 1;
                }
            }

            if (island.RequiredBridges != actualCount)
            {
                throw new InvalidOperationException(
                    $"Incohérence : L'île à ({island.X}, {island.Y}) a RequiredBridges={island.RequiredBridges} mais {actualCount} ponts dans la solution");
            }
        }
    }

    /// <summary>
    /// Trouve la meilleure connexion entre une île connectée et une île non connectée
    /// </summary>
    private (int fromIdx, int toIdx, bool isDouble)? FindBestConnection(
        List<Island> islands,
        HashSet<int> connectedIslands,
        HashSet<int> unconnectedIslands,
        HashSet<(int, int)> usedPositions)
    {
        int bestDistance = int.MaxValue;
        (int fromIdx, int toIdx, bool isDouble)? bestConnection = null;

        foreach (var connectedIdx in connectedIslands)
        {
            foreach (var unconnectedIdx in unconnectedIslands)
            {
                var from = islands[connectedIdx];
                var to = islands[unconnectedIdx];

                if (CanConnectIslands(from, to, usedPositions))
                {
                    int distance = Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
                    
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        bool isDouble = _random.Next(100) < 10; // 10% de chance d'un pont double
                        bestConnection = (connectedIdx, unconnectedIdx, isDouble);
                    }
                }
            }
        }

        return bestConnection;
    }

    /// <summary>
    /// Vérifie si deux îles peuvent être connectées (alignées et pas d'obstacle)
    /// </summary>
    private bool CanConnectIslands(Island from, Island to, HashSet<(int, int)> usedPositions)
    {
        // Les îles doivent être alignées (même ligne ou même colonne)
        if (from.X != to.X && from.Y != to.Y)
            return false;

        // Vérifier qu'il n'y a pas d'île entre les deux
        if (from.X == to.X) // Vertical
        {
            for (int y = Math.Min(from.Y, to.Y) + 1; y < Math.Max(from.Y, to.Y); y++)
            {
                if (usedPositions.Contains((from.X, y)))
                    return false;
            }
        }
        else // Horizontal
        {
            for (int x = Math.Min(from.X, to.X) + 1; x < Math.Max(from.X, to.X); x++)
            {
                if (usedPositions.Contains((x, from.Y)))
                    return false;
            }
        }

        return true;
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
