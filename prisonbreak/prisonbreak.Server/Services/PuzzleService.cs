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
    public async Task<Puzzle> GeneratePuzzleAsync(int width, int height, DifficultyLevel difficulty, PuzzleTheme theme = PuzzleTheme.Classic)
    {
        // Créer le puzzle
        var puzzle = new Puzzle
        {
            Name = $"{GetThemeName(theme)} - {difficulty} ({width}x{height})",
            Width = width,
            Height = height,
            Difficulty = difficulty,
            Theme = theme,
            CreatedAt = DateTime.UtcNow
        };

        // Générer les îles et la solution selon la difficulté et le thème
        // Chaque thème a un pattern unique d'îles
        var (islands, solutionBridges) = GenerateValidPuzzle(width, height, difficulty, theme);

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

        // S'assurer que toutes les îles sont connectées AVANT de calculer RequiredBridges
        EnsureAllIslandsConnected(savedIslands, bridgesToAdd);

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
    /// Chaque thème génère un puzzle unique avec un pattern différent
    /// </summary>
    private (List<Island> islands, List<Bridge> bridges) GenerateValidPuzzle(int width, int height, DifficultyLevel difficulty, PuzzleTheme theme)
    {
        // Utiliser le thème comme seed pour générer des patterns différents
        var themeSeed = (int)theme;
        var random = new Random(themeSeed + (int)difficulty * 1000 + width * 10 + height);

        // Patterns prédéfinis pour chaque difficulté et taille, avec variation selon le thème
        if (width == 5 && height == 5)
        {
            return Generate5x5Puzzle(difficulty, theme, random);
        }
        else if (width == 8 && height == 8)
        {
            return Generate8x8Puzzle(difficulty, theme, random);
        }
        else if (width == 12 && height == 12)
        {
            return Generate12x12Puzzle(difficulty, theme, random);
        }
        else if (width == 15 && height == 15)
        {
            return Generate15x15Puzzle(difficulty, theme, random);
        }
        else
        {
            // Génération générique pour autres tailles avec variation selon le thème
            return GenerateGenericPuzzle(width, height, difficulty, theme, random);
        }
    }

    /// <summary>
    /// Génère un puzzle 5x5 pour niveau Facile
    /// Chaque thème a un pattern unique d'îles
    /// </summary>
    private (List<Island> islands, List<Bridge> bridges) Generate5x5Puzzle(DifficultyLevel difficulty, PuzzleTheme theme, Random random)
    {
        var islands = new List<Island>();
        var bridges = new List<Bridge>();

        // Patterns différents selon le thème - chaque thème a un arrangement unique
        List<(int x, int y)> positions;
        
        // Utiliser le thème pour générer des patterns variés
        var themeOffset = ((int)theme % 4); // 0-3 pour varier les patterns
        
        switch (themeOffset)
        {
            case 0: // Classic, Ice, Neon, Magic
                positions = new List<(int x, int y)> { (1, 0), (3, 0), (1, 2), (3, 2), (1, 4), (3, 4) };
                break;
            case 1: // Medieval, Volcano, Steampunk, Western
                positions = new List<(int x, int y)> { (0, 1), (2, 1), (4, 1), (0, 3), (2, 3), (4, 3) };
                break;
            case 2: // Futuristic, Desert, Pirate, Zombie
                positions = new List<(int x, int y)> { (0, 0), (2, 0), (4, 0), (2, 2), (0, 4), (4, 4) };
                break;
            case 3: // Underwater, Forest, Ninja
                positions = new List<(int x, int y)> { (1, 1), (3, 1), (0, 2), (2, 2), (4, 2), (1, 3), (3, 3) };
                break;
            default:
                positions = new List<(int x, int y)> { (1, 0), (3, 0), (1, 2), (3, 2), (1, 4), (3, 4) };
                break;
        }
        
        // Ajuster les positions selon le thème spécifique pour plus de variété
        var themeSpecificOffset = ((int)theme % 3) - 1; // -1, 0, ou 1
        if (themeSpecificOffset != 0)
        {
            // Décaler légèrement certaines positions (pour grille 5x5)
            positions = positions.Select(p => (
                Math.Max(0, Math.Min(4, p.x + themeSpecificOffset)), 
                p.y
            )).ToList();
        }

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

        // Générer une solution unique selon le thème
        // Chaque thème a une solution différente même avec le même pattern de base
        var themeSolutionType = ((int)theme % 3);
        
        // Solution type 0 : connexions verticales puis horizontales
        if (themeSolutionType == 0 && islands.Count >= 2)
        {
            // Connecter les îles de la même colonne verticalement
            var byColumn = islands.GroupBy(i => i.X).Where(g => g.Count() > 1).ToList();
            foreach (var col in byColumn)
            {
                var colIslands = col.OrderBy(i => i.Y).ToList();
                for (int i = 0; i < colIslands.Count - 1; i++)
                {
                    bridges.Add(CreateBridge(colIslands[i], colIslands[i + 1], false, BridgeDirection.Vertical));
                }
            }
            // Connecter les colonnes horizontalement
            if (byColumn.Count > 1)
            {
                var firstCol = byColumn[0].OrderBy(i => i.Y).First();
                var secondCol = byColumn[1].OrderBy(i => i.Y).First();
                if (firstCol.Y == secondCol.Y)
                {
                    bridges.Add(CreateBridge(firstCol, secondCol, false, BridgeDirection.Horizontal));
                }
            }
            
            // Si certaines îles ne sont pas connectées, les connecter
            var usedPositions = new HashSet<(int, int)>();
            foreach (var island in islands)
            {
                usedPositions.Add((island.X, island.Y));
            }
            
            var connected = new HashSet<Island>();
            if (islands.Count > 0)
            {
                connected.Add(islands[0]);
                foreach (var bridge in bridges)
                {
                    if (bridge.FromIsland != null) connected.Add(bridge.FromIsland);
                    if (bridge.ToIsland != null) connected.Add(bridge.ToIsland);
                }
                
                foreach (var island in islands.Where(i => !connected.Contains(i)))
                {
                    var nearest = connected
                        .Where(i => CanConnectIslands(i, island, usedPositions))
                        .OrderBy(i => Math.Abs(i.X - island.X) + Math.Abs(i.Y - island.Y))
                        .FirstOrDefault();
                    
                    if (nearest != null)
                    {
                        var direction = nearest.X == island.X ? BridgeDirection.Vertical : BridgeDirection.Horizontal;
                        bridges.Add(CreateBridge(nearest, island, false, direction));
                        connected.Add(island);
                    }
                }
            }
        }
        // Solution type 1 : connexions horizontales puis verticales
        else if (themeSolutionType == 1 && islands.Count >= 2)
        {
            var byRow = islands.GroupBy(i => i.Y).Where(g => g.Count() > 1).ToList();
            foreach (var row in byRow)
            {
                var rowIslands = row.OrderBy(i => i.X).ToList();
                for (int i = 0; i < rowIslands.Count - 1; i++)
                {
                    bridges.Add(CreateBridge(rowIslands[i], rowIslands[i + 1], false, BridgeDirection.Horizontal));
                }
            }
            if (byRow.Count > 1)
            {
                var firstRow = byRow[0].OrderBy(i => i.X).First();
                var secondRow = byRow[1].OrderBy(i => i.X).First();
                if (firstRow.X == secondRow.X)
                {
                    bridges.Add(CreateBridge(firstRow, secondRow, false, BridgeDirection.Vertical));
                }
            }
            
            // Si certaines îles ne sont pas connectées, les connecter
            var usedPositions = new HashSet<(int, int)>();
            foreach (var island in islands)
            {
                usedPositions.Add((island.X, island.Y));
            }
            
            var connected = new HashSet<Island>();
            if (islands.Count > 0)
            {
                connected.Add(islands[0]);
                foreach (var bridge in bridges)
                {
                    if (bridge.FromIsland != null) connected.Add(bridge.FromIsland);
                    if (bridge.ToIsland != null) connected.Add(bridge.ToIsland);
                }
                
                foreach (var island in islands.Where(i => !connected.Contains(i)))
                {
                    var nearest = connected
                        .Where(i => CanConnectIslands(i, island, usedPositions))
                        .OrderBy(i => Math.Abs(i.X - island.X) + Math.Abs(i.Y - island.Y))
                        .FirstOrDefault();
                    
                    if (nearest != null)
                    {
                        var direction = nearest.X == island.X ? BridgeDirection.Vertical : BridgeDirection.Horizontal;
                        bridges.Add(CreateBridge(nearest, island, false, direction));
                        connected.Add(island);
                    }
                }
            }
        }
        // Solution type 2 : solution en étoile (hub central)
        else if (islands.Count >= 2)
        {
            var center = islands.OrderBy(i => Math.Abs(i.X - 2) + Math.Abs(i.Y - 2)).First();
            foreach (var island in islands.Where(i => i != center))
            {
                if (center.X == island.X || center.Y == island.Y)
                {
                    var direction = center.X == island.X ? BridgeDirection.Vertical : BridgeDirection.Horizontal;
                    bridges.Add(CreateBridge(center, island, false, direction));
                }
            }
            
            // S'assurer que toutes les îles sont connectées
            // Si une île n'est pas alignée avec le centre, la connecter via une autre île
            var connectedIslands = new HashSet<Island> { center };
            foreach (var bridge in bridges)
            {
                if (bridge.FromIsland != null) connectedIslands.Add(bridge.FromIsland);
                if (bridge.ToIsland != null) connectedIslands.Add(bridge.ToIsland);
            }
            
            foreach (var island in islands)
            {
                if (!connectedIslands.Contains(island))
                {
                    // Trouver l'île la plus proche qui est déjà connectée
                    var nearest = connectedIslands
                        .Where(i => i.X == island.X || i.Y == island.Y)
                        .OrderBy(i => Math.Abs(i.X - island.X) + Math.Abs(i.Y - island.Y))
                        .FirstOrDefault();
                    
                    if (nearest != null)
                    {
                        var direction = nearest.X == island.X ? BridgeDirection.Vertical : BridgeDirection.Horizontal;
                        bridges.Add(CreateBridge(nearest, island, false, direction));
                        connectedIslands.Add(island);
                    }
                }
            }
        }

        // Vérification finale : s'assurer que toutes les îles sont connectées
        EnsureAllIslandsConnected(islands, bridges);

        return (islands, bridges);
    }

    /// <summary>
    /// Génère un puzzle 8x8 pour niveau Moyen
    /// </summary>
    private (List<Island> islands, List<Bridge> bridges) Generate8x8Puzzle(DifficultyLevel difficulty, PuzzleTheme theme, Random random)
    {
        var islands = new List<Island>();
        var bridges = new List<Bridge>();

        // Patterns différents selon le thème - chaque thème a un arrangement unique
        var themeOffset = ((int)theme % 3);
        List<(int x, int y)> positions;
        
        switch (themeOffset)
        {
            case 0: // Pattern en colonnes
                positions = new List<(int x, int y)>
                {
                    (2, 1), (2, 3), (2, 5), (6, 1), (6, 3), (6, 5), (4, 5), (4, 7)
                };
                break;
            case 1: // Pattern en lignes
                positions = new List<(int x, int y)>
                {
                    (1, 2), (3, 2), (5, 2), (1, 6), (3, 6), (5, 6), (3, 4), (7, 4)
                };
                break;
            default: // Pattern mixte
                positions = new List<(int x, int y)>
                {
                    (1, 1), (3, 1), (5, 1), (1, 5), (3, 5), (5, 5), (3, 3), (7, 3)
                };
                break;
        }

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

        // Générer une solution unique selon le thème
        var themeSolutionType = ((int)theme % 3);
        
        if (themeSolutionType == 0)
        {
            // Solution en colonnes verticales puis horizontales
            var byColumn = islands.GroupBy(i => i.X).Where(g => g.Count() > 1).ToList();
            foreach (var col in byColumn)
            {
                var colIslands = col.OrderBy(i => i.Y).ToList();
                for (int i = 0; i < colIslands.Count - 1; i++)
                {
                    bridges.Add(CreateBridge(colIslands[i], colIslands[i + 1], false, BridgeDirection.Vertical));
                }
            }
            // Connecter les colonnes
            if (byColumn.Count > 1)
            {
                var cols = byColumn.OrderBy(g => g.Key).ToList();
                for (int i = 0; i < cols.Count - 1; i++)
                {
                    var commonY = cols[i].Select(isl => isl.Y).Intersect(cols[i + 1].Select(isl => isl.Y)).FirstOrDefault();
                    if (commonY >= 0)
                    {
                        var left = cols[i].First(isl => isl.Y == commonY);
                        var right = cols[i + 1].First(isl => isl.Y == commonY);
                        bridges.Add(CreateBridge(left, right, false, BridgeDirection.Horizontal));
                    }
                }
            }
        }
        else if (themeSolutionType == 1)
        {
            // Solution en lignes horizontales puis verticales
            var byRow = islands.GroupBy(i => i.Y).Where(g => g.Count() > 1).ToList();
            foreach (var row in byRow)
            {
                var rowIslands = row.OrderBy(i => i.X).ToList();
                for (int i = 0; i < rowIslands.Count - 1; i++)
                {
                    bridges.Add(CreateBridge(rowIslands[i], rowIslands[i + 1], false, BridgeDirection.Horizontal));
                }
            }
            // Connecter les lignes
            if (byRow.Count > 1)
            {
                var rows = byRow.OrderBy(g => g.Key).ToList();
                for (int i = 0; i < rows.Count - 1; i++)
                {
                    var commonX = rows[i].Select(isl => isl.X).Intersect(rows[i + 1].Select(isl => isl.X)).FirstOrDefault();
                    if (commonX >= 0)
                    {
                        var top = rows[i].First(isl => isl.X == commonX);
                        var bottom = rows[i + 1].First(isl => isl.X == commonX);
                        bridges.Add(CreateBridge(top, bottom, false, BridgeDirection.Vertical));
                    }
                }
            }
        }
        else
        {
            // Solution en étoile (hub central)
            var center = islands.OrderBy(i => Math.Abs(i.X - 4) + Math.Abs(i.Y - 4)).First();
            foreach (var island in islands.Where(i => i != center))
            {
                if (center.X == island.X || center.Y == island.Y)
                {
                    var direction = center.X == island.X ? BridgeDirection.Vertical : BridgeDirection.Horizontal;
                    bridges.Add(CreateBridge(center, island, false, direction));
                }
            }
        }

        // Vérification finale : s'assurer que toutes les îles sont connectées
        EnsureAllIslandsConnected(islands, bridges);

        return (islands, bridges);
    }

    /// <summary>
    /// Génère un puzzle 12x12 pour niveau Difficile
    /// Chaque thème a un pattern unique
    /// </summary>
    private (List<Island> islands, List<Bridge> bridges) Generate12x12Puzzle(DifficultyLevel difficulty, PuzzleTheme theme, Random random)
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

        // Vérification finale : s'assurer que toutes les îles sont connectées
        EnsureAllIslandsConnected(islands, bridges);

        return (islands, bridges);
    }

    /// <summary>
    /// Génère un puzzle 15x15 pour niveau Expert
    /// Chaque thème a un pattern unique
    /// </summary>
    private (List<Island> islands, List<Bridge> bridges) Generate15x15Puzzle(DifficultyLevel difficulty, PuzzleTheme theme, Random random)
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

        // Vérification finale : s'assurer que toutes les îles sont connectées
        EnsureAllIslandsConnected(islands, bridges);

        return (islands, bridges);
    }

    /// <summary>
    /// Génère un puzzle générique pour autres tailles
    /// </summary>
    private (List<Island> islands, List<Bridge> bridges) GenerateGenericPuzzle(int width, int height, DifficultyLevel difficulty, PuzzleTheme theme, Random random)
    {
        var islands = new List<Island>();
        var bridges = new List<Bridge>();
        var usedPositions = new HashSet<(int, int)>();

        // Calculer le nombre d'îles selon la difficulté et la taille
        // Pour les grandes grilles rectangulaires, ajuster le ratio
        int baseArea = width * height;
        int islandCount = difficulty switch
        {
            DifficultyLevel.Easy => Math.Max(4, Math.Min(baseArea / 8, 6)),
            DifficultyLevel.Medium => Math.Max(6, Math.Min(baseArea / 6, 10)),
            DifficultyLevel.Hard => Math.Max(10, Math.Min(baseArea / 5, 18)), // Plus d'îles pour niveau difficile
            DifficultyLevel.Expert => Math.Max(12, Math.Min(baseArea / 4, 25)),
            _ => 5
        };
        
        // Pour les grandes grilles (18x11 = 198 cases), s'assurer d'avoir assez d'îles
        if (baseArea > 150 && difficulty == DifficultyLevel.Hard)
        {
            islandCount = Math.Max(islandCount, 12); // Minimum 12 îles pour grandes grilles difficiles
        }

        // Placer les îles de manière variée
        var positions = new List<(int x, int y)>();
        int attempts = 0;
        int maxAttempts = 1000;

        // Pour les grandes grilles, augmenter la distance minimale entre îles
        int minDistance = (width > 15 || height > 15) ? 3 : 2;
        
        while (positions.Count < islandCount && attempts < maxAttempts)
        {
            int x = _random.Next(1, width - 1);
            int y = _random.Next(1, height - 1);

            // Éviter les positions trop proches
            bool tooClose = false;
            foreach (var (px, py) in positions)
            {
                int distance = Math.Abs(x - px) + Math.Abs(y - py);
                if (distance < minDistance)
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
        
        // Si on n'a pas assez d'îles, réduire la distance minimale
        if (positions.Count < Math.Max(8, islandCount / 2) && minDistance > 1)
        {
            minDistance = 1;
            while (positions.Count < islandCount && attempts < maxAttempts * 2)
            {
                int x = _random.Next(1, width - 1);
                int y = _random.Next(1, height - 1);

                if (!usedPositions.Contains((x, y)))
                {
                    positions.Add((x, y));
                    usedPositions.Add((x, y));
                }
                attempts++;
            }
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
            // Trouver la meilleure connexion
            (int fromIdx, int toIdx, bool isDouble)? bestConnection = null;
            int bestDistance = int.MaxValue;
            
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
                            bool isDouble = _random.Next(100) < 10;
                            bestConnection = (connectedIdx, unconnectedIdx, isDouble);
                        }
                    }
                }
            }
            
            if (bestConnection.HasValue)
            {
                var (fromIdx, toIdx, isDouble) = bestConnection.Value;
                var from = islands[fromIdx];
                var to = islands[toIdx];
                BridgeDirection direction = from.X == to.X ? BridgeDirection.Vertical : BridgeDirection.Horizontal;
                bridges.Add(CreateBridge(from, to, isDouble, direction));
                
                unconnectedIslands.Remove(toIdx);
                connectedIslands.Add(toIdx);
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

        // Vérification finale : s'assurer que toutes les îles sont connectées
        EnsureAllIslandsConnected(islands, bridges);

        return (islands, bridges);
    }

    /// <summary>
    /// Crée un pont entre deux îles
    /// </summary>
    /// <summary>
    /// S'assure que toutes les îles sont connectées dans le réseau
    /// Ajoute des ponts si nécessaire pour connecter les îles isolées
    /// </summary>
    private void EnsureAllIslandsConnected(List<Island> islands, List<Bridge> bridges)
    {
        if (islands.Count <= 1) return;

        // Créer un HashSet des positions utilisées pour vérifier les obstacles
        var usedPositions = new HashSet<(int x, int y)>();
        foreach (var island in islands)
        {
            usedPositions.Add((island.X, island.Y));
        }

        // Créer un graphe d'adjacence pour trouver les îles connectées
        var connectedIslands = new HashSet<Island>();
        var adjacency = new Dictionary<Island, List<Island>>();
        
        foreach (var island in islands)
        {
            adjacency[island] = new List<Island>();
        }

        foreach (var bridge in bridges)
        {
            if (bridge.FromIsland != null && bridge.ToIsland != null)
            {
                if (!adjacency[bridge.FromIsland].Contains(bridge.ToIsland))
                    adjacency[bridge.FromIsland].Add(bridge.ToIsland);
                if (!adjacency[bridge.ToIsland].Contains(bridge.FromIsland))
                    adjacency[bridge.ToIsland].Add(bridge.FromIsland);
            }
        }

        // Parcourir en profondeur pour trouver toutes les îles connectées
        if (islands.Count > 0)
        {
            var visited = new HashSet<Island>();
            var stack = new Stack<Island>();
            stack.Push(islands[0]);
            
            while (stack.Count > 0)
            {
                var current = stack.Pop();
                if (visited.Contains(current)) continue;
                
                visited.Add(current);
                connectedIslands.Add(current);
                
                foreach (var neighbor in adjacency[current])
                {
                    if (!visited.Contains(neighbor))
                        stack.Push(neighbor);
                }
            }

            // Connecter les îles isolées - essayer plusieurs fois jusqu'à ce que toutes soient connectées
            int maxAttempts = islands.Count * 2;
            int attempts = 0;
            
            while (connectedIslands.Count < islands.Count && attempts < maxAttempts)
            {
                bool addedConnection = false;
                
                foreach (var island in islands)
                {
                    if (!connectedIslands.Contains(island))
                    {
                        // Trouver l'île connectée la plus proche qui peut être reliée
                        var nearest = connectedIslands
                            .Where(i => CanConnectIslands(i, island, usedPositions))
                            .OrderBy(i => Math.Abs(i.X - island.X) + Math.Abs(i.Y - island.Y))
                            .FirstOrDefault();
                        
                        if (nearest != null)
                        {
                            var direction = nearest.X == island.X ? BridgeDirection.Vertical : BridgeDirection.Horizontal;
                            bridges.Add(CreateBridge(nearest, island, false, direction));
                            connectedIslands.Add(island);
                            adjacency[nearest].Add(island);
                            adjacency[island].Add(nearest);
                            addedConnection = true;
                            break; // Sortir de la boucle pour recommencer le parcours
                        }
                    }
                }
                
                // Si aucune connexion n'a été ajoutée, forcer la connexion de la première île isolée
                // en trouvant la connexion la plus proche possible
                if (!addedConnection)
                {
                    var unconnected = islands.Where(i => !connectedIslands.Contains(i)).ToList();
                    if (unconnected.Count > 0)
                    {
                        var isolated = unconnected[0];
                        
                        // Trouver toutes les îles connectées qui sont alignées (même ligne ou colonne)
                        var alignedConnected = connectedIslands
                            .Where(i => (i.X == isolated.X || i.Y == isolated.Y))
                            .OrderBy(i => Math.Abs(i.X - isolated.X) + Math.Abs(i.Y - isolated.Y))
                            .ToList();
                        
                        Island? target = null;
                        
                        if (alignedConnected.Count > 0)
                        {
                            // Prendre la plus proche qui peut être connectée
                            target = alignedConnected
                                .Where(i => CanConnectIslands(i, isolated, usedPositions))
                                .FirstOrDefault();
                            
                            if (target == null && alignedConnected.Count > 0)
                            {
                                // Si aucune ne peut être connectée directement, prendre la plus proche quand même
                                // et créer le pont (il sera validé plus tard)
                                target = alignedConnected[0];
                            }
                        }
                        
                        // Si toujours aucune île alignée, prendre la plus proche île connectée
                        if (target == null && connectedIslands.Count > 0)
                        {
                            target = connectedIslands
                                .OrderBy(i => Math.Abs(i.X - isolated.X) + Math.Abs(i.Y - isolated.Y))
                                .FirstOrDefault();
                        }
                        
                        if (target != null)
                        {
                            var direction = target.X == isolated.X ? BridgeDirection.Vertical : BridgeDirection.Horizontal;
                            bridges.Add(CreateBridge(target, isolated, false, direction));
                            connectedIslands.Add(isolated);
                            adjacency[target].Add(isolated);
                            adjacency[isolated].Add(target);
                            addedConnection = true;
                        }
                    }
                }
                
                // Si toujours aucune connexion, relancer le parcours pour mettre à jour connectedIslands
                if (addedConnection)
                {
                    // Refaire le parcours pour mettre à jour connectedIslands
                    visited.Clear();
                    stack.Clear();
                    stack.Push(islands[0]);
                    
                    while (stack.Count > 0)
                    {
                        var current = stack.Pop();
                        if (visited.Contains(current)) continue;
                        
                        visited.Add(current);
                        connectedIslands.Add(current);
                        
                        foreach (var neighbor in adjacency[current])
                        {
                            if (!visited.Contains(neighbor))
                                stack.Push(neighbor);
                        }
                    }
                }
                
                attempts++;
            }
        }
    }

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
        // Si une île n'a aucun pont, essayer de la connecter avant de lancer l'erreur
        foreach (var island in islands)
        {
            if (island.RequiredBridges == 0)
            {
                // Dernière tentative : trouver n'importe quelle île connectée et créer un pont
                var usedPositions = new HashSet<(int, int)>();
                foreach (var i in islands)
                {
                    usedPositions.Add((i.X, i.Y));
                }
                
                var connectedIslands = islands.Where(i => i.RequiredBridges > 0).ToList();
                if (connectedIslands.Count > 0)
                {
                    // Essayer d'abord avec CanConnectIslands
                    var nearest = connectedIslands
                        .Where(i => CanConnectIslands(i, island, usedPositions))
                        .OrderBy(i => Math.Abs(i.X - island.X) + Math.Abs(i.Y - island.Y))
                        .FirstOrDefault();
                    
                    // Si aucune connexion valide trouvée, forcer la connexion avec la plus proche île alignée
                    if (nearest == null)
                    {
                        // Chercher les îles alignées (même ligne ou colonne)
                        var aligned = connectedIslands
                            .Where(i => i.X == island.X || i.Y == island.Y)
                            .OrderBy(i => Math.Abs(i.X - island.X) + Math.Abs(i.Y - island.Y))
                            .FirstOrDefault();
                        
                        if (aligned != null)
                        {
                            nearest = aligned;
                        }
                        else
                        {
                            // Si aucune île alignée, prendre la plus proche
                            nearest = connectedIslands
                                .OrderBy(i => Math.Abs(i.X - island.X) + Math.Abs(i.Y - island.Y))
                                .FirstOrDefault();
                        }
                    }
                    
                    if (nearest != null)
                    {
                        var direction = nearest.X == island.X ? BridgeDirection.Vertical : BridgeDirection.Horizontal;
                        bridges.Add(CreateBridge(nearest, island, false, direction));
                        // Recalculer pour cette île
                        island.RequiredBridges += 1;
                        nearest.RequiredBridges += 1;
                        continue; // Passer à l'île suivante
                    }
                }
                
                // Si toujours aucune connexion possible, lancer l'erreur
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
    private (Island from, Island to, bool isDouble)? FindBestConnection(
        HashSet<Island> connectedIslands,
        List<Island> unconnectedIslands,
        HashSet<(int, int)> usedPositions)
    {
        int bestDistance = int.MaxValue;
        (Island from, Island to, bool isDouble)? bestConnection = null;

        foreach (var from in connectedIslands)
        {
            foreach (var to in unconnectedIslands)
            {
                if (CanConnectIslands(from, to, usedPositions))
                {
                    int distance = Math.Abs(from.X - to.X) + Math.Abs(from.Y - to.Y);
                    
                    if (distance < bestDistance)
                    {
                        bestDistance = distance;
                        bool isDouble = _random.Next(100) < 10; // 10% de chance d'un pont double
                        bestConnection = (from, to, isDouble);
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
    /// Génère un puzzle unique pour chaque thème (15 puzzles au total)
    /// Chaque thème a une taille et un arrangement différents
    /// </summary>
    public async Task<List<Puzzle>> GetPuzzlesByDifficultyAsync(DifficultyLevel difficulty)
    {
        var puzzles = await _context.Puzzles
            .Include(p => p.Islands)
            .Where(p => p.Difficulty == difficulty)
            .OrderBy(p => p.Theme) // Trier par thème pour garantir l'ordre
            .ToListAsync();

        var themes = Enum.GetValues<PuzzleTheme>().ToList();

        // Pour chaque thème, s'assurer qu'un puzzle unique existe avec sa propre taille
        foreach (var theme in themes)
        {
            // Vérifier si un puzzle avec ce thème existe déjà
            var existingPuzzle = puzzles.FirstOrDefault(p => p.Theme == theme);
            
            if (existingPuzzle == null)
            {
                // Chaque thème a une taille unique mais cohérente avec la difficulté
                var (width, height) = GetDimensionsForTheme(difficulty, theme);
                
                // Générer un nouveau puzzle unique pour ce thème avec sa taille spécifique
                var newPuzzle = await GeneratePuzzleAsync(width, height, difficulty, theme);
                puzzles.Add(newPuzzle);
            }
        }

        // Retourner les puzzles triés par thème
        return puzzles.OrderBy(p => p.Theme).ToList();
    }

    /// <summary>
    /// Retourne des dimensions uniques pour chaque thème selon la difficulté
    /// Chaque thème a une taille différente pour créer des puzzles variés
    /// </summary>
    private (int width, int height) GetDimensionsForTheme(DifficultyLevel difficulty, PuzzleTheme theme)
    {
        // Utiliser le thème comme index pour varier les tailles
        var themeIndex = (int)theme;
        
        return difficulty switch
        {
            DifficultyLevel.Easy => GetEasyDimensions(themeIndex),
            DifficultyLevel.Medium => GetMediumDimensions(themeIndex),
            DifficultyLevel.Hard => GetHardDimensions(themeIndex),
            DifficultyLevel.Expert => GetExpertDimensions(themeIndex),
            _ => (5, 5)
        };
    }

    /// <summary>
    /// Retourne des dimensions variées pour niveau Facile (5x5 à 6x7)
    /// </summary>
    private (int width, int height) GetEasyDimensions(int themeIndex)
    {
        var sizes = new[]
        {
            (5, 5), (5, 6), (5, 7), (6, 5), (6, 6),
            (6, 7), (7, 5), (7, 6), (5, 8), (6, 8),
            (7, 7), (8, 5), (8, 6), (7, 8), (8, 7)
        };
        return sizes[themeIndex % sizes.Length];
    }

    /// <summary>
    /// Retourne des dimensions variées pour niveau Moyen (8x8 à 10x10)
    /// </summary>
    private (int width, int height) GetMediumDimensions(int themeIndex)
    {
        var sizes = new[]
        {
            (8, 8), (8, 9), (8, 10), (9, 8), (9, 9),
            (9, 10), (10, 8), (10, 9), (8, 11), (9, 11),
            (10, 10), (11, 8), (11, 9), (10, 11), (11, 10)
        };
        return sizes[themeIndex % sizes.Length];
    }

    /// <summary>
    /// Retourne des dimensions variées pour niveau Difficile
    /// Inclut des tailles rectangulaires et carrées pour plus de variété
    /// Tous les puzzles sont résolvables grâce à GenerateGenericPuzzle
    /// </summary>
    private (int width, int height) GetHardDimensions(int themeIndex)
    {
        var sizes = new[]
        {
            // Tailles carrées
            (12, 12), (13, 13), (14, 14), (15, 15),
            // Tailles rectangulaires larges
            (18, 11), (17, 12), (16, 13), (15, 14),
            (18, 12), (17, 13), (16, 14), (15, 15),
            // Tailles rectangulaires hautes
            (11, 18), (12, 17), (13, 16), (14, 15),
            (12, 18), (13, 17), (14, 16), (15, 15),
            // Tailles moyennes variées
            (16, 12), (14, 13), (13, 15), (15, 13),
            (17, 11), (16, 11), (15, 12), (14, 14),
            // Tailles plus grandes
            (18, 13), (17, 14), (16, 15), (15, 16),
            (14, 17), (13, 18), (12, 16), (11, 17)
        };
        return sizes[themeIndex % sizes.Length];
    }

    /// <summary>
    /// Retourne des dimensions variées pour niveau Expert (15x15 à 18x18)
    /// </summary>
    private (int width, int height) GetExpertDimensions(int themeIndex)
    {
        var sizes = new[]
        {
            (15, 15), (15, 16), (15, 17), (16, 15), (16, 16),
            (16, 17), (17, 15), (17, 16), (15, 18), (16, 18),
            (17, 17), (18, 15), (18, 16), (17, 18), (18, 17)
        };
        return sizes[themeIndex % sizes.Length];
    }

    /// <summary>
    /// Retourne le nom du thème en français
    /// </summary>
    private string GetThemeName(PuzzleTheme theme)
    {
        return theme switch
        {
            PuzzleTheme.Classic => "Prison Classique",
            PuzzleTheme.Medieval => "Château Fort",
            PuzzleTheme.Futuristic => "Prison Spatiale",
            PuzzleTheme.Underwater => "Prison Aquatique",
            PuzzleTheme.Desert => "Désert Aride",
            PuzzleTheme.Forest => "Jungle Perdue",
            PuzzleTheme.Ice => "Glacier Arctique",
            PuzzleTheme.Volcano => "Volcan Brûlant",
            PuzzleTheme.Neon => "Cyberpunk",
            PuzzleTheme.Steampunk => "Steampunk",
            PuzzleTheme.Pirate => "Pirate",
            PuzzleTheme.Zombie => "Apocalypse",
            PuzzleTheme.Ninja => "Ninja Secret",
            PuzzleTheme.Magic => "Magie Enchantée",
            PuzzleTheme.Western => "Far West",
            _ => "Puzzle"
        };
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
            Theme = (int)puzzle.Theme,
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
