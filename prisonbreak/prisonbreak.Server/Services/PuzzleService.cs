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
    private readonly ILogger<PuzzleService>? _logger;

    public PuzzleService(HashiDbContext context, ILogger<PuzzleService>? logger = null)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Génère un nouveau puzzle Hashi avec une solution valide
    /// Place les îles de manière structurée et génère une solution cohérente
    /// </summary>
    public async Task<Puzzle> GeneratePuzzleAsync(int width, int height, DifficultyLevel difficulty, PuzzleTheme theme = PuzzleTheme.Classic)
    {
        const int maxRetries = 20; // Nombre maximum de tentatives (augmenté pour éviter les croisements)
        int attempt = 0;
        
        while (attempt < maxRetries)
        {
            try
            {
                // Supprimer le puzzle précédent si c'est un retry
                if (attempt > 0)
                {
                    // Nettoyer les puzzles précédents qui ont échoué
                    var failedPuzzles = await _context.Puzzles
                        .Where(p => p.Width == width && p.Height == height && 
                                   p.Difficulty == difficulty && p.Theme == theme &&
                                   !p.Islands.Any(i => i.RequiredBridges > 0))
                        .ToListAsync();
                    
                    foreach (var failedPuzzle in failedPuzzles)
                    {
                        _context.Puzzles.Remove(failedPuzzle);
                    }
                    await _context.SaveChangesAsync();
                }

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
                EnsureAllIslandsConnected(savedIslands, bridgesToAdd, puzzle.Id);

                // Calculer les RequiredBridges pour chaque île en fonction de la solution
                // IMPORTANT : Cette méthode doit être appelée APRÈS tous les ponts sont créés
                CalculateRequiredBridges(savedIslands, bridgesToAdd);
                
                // Recalculer une fois de plus pour s'assurer que tout est cohérent
                // (au cas où des ponts ont été ajoutés dans CalculateRequiredBridges)
                CalculateRequiredBridges(savedIslands, bridgesToAdd);

                // VALIDATION : Vérifier que toutes les îles sont connectées
                if (!ValidatePuzzleConnectivity(savedIslands, bridgesToAdd))
                {
                    throw new InvalidOperationException("Le puzzle généré n'est pas valide : toutes les îles ne sont pas connectées");
                }

                // VALIDATION : Vérifier qu'il n'y a pas de croisements illégaux
                ValidateNoIllegalCrossings(savedIslands, bridgesToAdd);

                // VALIDATION : Vérifier que les RequiredBridges correspondent aux ponts
                ValidateRequiredBridges(savedIslands, bridgesToAdd);

                // Mettre à jour les RequiredBridges dans la base de données
                foreach (var island in savedIslands)
                {
                    _context.Islands.Update(island);
                }

                // Ajouter les ponts de la solution
                _context.Bridges.AddRange(bridgesToAdd);

                // Sauvegarder les modifications
                await _context.SaveChangesAsync();

                // Recharger le puzzle avec toutes ses relations
                return await _context.Puzzles
                    .Include(p => p.Islands)
                    .FirstOrDefaultAsync(p => p.Id == puzzle.Id) ?? puzzle;
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("Croisement illégal") || ex.Message.Contains("Impossible de créer un pont diagonal"))
            {
                attempt++;
                // Supprimer le puzzle qui a échoué
                var failedPuzzle = await _context.Puzzles
                    .FirstOrDefaultAsync(p => p.Width == width && p.Height == height && 
                                             p.Difficulty == difficulty && p.Theme == theme &&
                                             p.CreatedAt >= DateTime.UtcNow.AddSeconds(-5));
                
                if (failedPuzzle != null)
                {
                    _context.Puzzles.Remove(failedPuzzle);
                    await _context.SaveChangesAsync();
                }
                
                // Si c'est le dernier essai, relancer l'exception
                if (attempt >= maxRetries)
                {
                    throw new InvalidOperationException($"Impossible de générer un puzzle sans croisements après {maxRetries} tentatives. Dernière erreur : {ex.Message}", ex);
                }
                // Sinon, réessayer avec une nouvelle génération
                continue;
            }
            catch (Exception ex) when (!ex.Message.Contains("Croisement illégal") && !ex.Message.Contains("Impossible de créer un pont diagonal"))
            {
                attempt++;
                // Supprimer le puzzle qui a échoué
                var failedPuzzle = await _context.Puzzles
                    .FirstOrDefaultAsync(p => p.Width == width && p.Height == height && 
                                             p.Difficulty == difficulty && p.Theme == theme &&
                                             p.CreatedAt >= DateTime.UtcNow.AddSeconds(-5));
                
                if (failedPuzzle != null)
                {
                    _context.Puzzles.Remove(failedPuzzle);
                    await _context.SaveChangesAsync();
                }
                
                // Si c'est le dernier essai, relancer l'exception
                if (attempt >= maxRetries)
                {
                    throw;
                }
                // Sinon, réessayer avec une nouvelle génération
                continue;
            }
        }
        
        // Ne devrait jamais arriver ici, mais au cas où
        throw new InvalidOperationException($"Impossible de générer un puzzle valide après {maxRetries} tentatives");
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

        // Patterns améliorés et variés selon le thème - designs plus intéressants
        List<(int x, int y)> positions;
        
        // Utiliser le thème pour générer des patterns variés et esthétiques
        var themeIndex = (int)theme;
        
        // 15 patterns différents pour plus de variété
        var patternIndex = themeIndex % 15;
        
        switch (patternIndex)
        {
            case 0: // Colonnes symétriques
                positions = new List<(int x, int y)> { (1, 0), (1, 2), (1, 4), (3, 0), (3, 2), (3, 4) };
                break;
            case 1: // Lignes symétriques
                positions = new List<(int x, int y)> { (0, 1), (2, 1), (4, 1), (0, 3), (2, 3), (4, 3) };
                break;
            case 2: // Croix centrale
                positions = new List<(int x, int y)> { (2, 0), (0, 2), (2, 2), (4, 2), (2, 4) };
                break;
            case 3: // Forme en L
                positions = new List<(int x, int y)> { (0, 0), (0, 2), (2, 2), (2, 4), (4, 4) };
                break;
            case 4: // Hub central avec branches
                positions = new List<(int x, int y)> { (2, 1), (1, 2), (2, 2), (3, 2), (2, 3) };
                break;
            case 5: // Diagonale symétrique
                positions = new List<(int x, int y)> { (0, 0), (1, 1), (2, 2), (3, 3), (4, 4) };
                break;
            case 6: // Rectangle
                positions = new List<(int x, int y)> { (1, 1), (3, 1), (1, 3), (3, 3) };
                break;
            case 7: // Étoile à 5 branches
                positions = new List<(int x, int y)> { (2, 0), (0, 2), (2, 2), (4, 2), (2, 4) };
                break;
            case 8: // Double colonne avec connexion
                positions = new List<(int x, int y)> { (1, 0), (1, 2), (3, 1), (3, 3), (1, 4), (3, 4) };
                break;
            case 9: // Grille 2x3
                positions = new List<(int x, int y)> { (1, 0), (3, 0), (1, 2), (3, 2), (1, 4), (3, 4) };
                break;
            case 10: // Triangle
                positions = new List<(int x, int y)> { (2, 0), (1, 2), (3, 2), (0, 4), (2, 4), (4, 4) };
                break;
            case 11: // Zigzag
                positions = new List<(int x, int y)> { (0, 0), (2, 1), (0, 2), (2, 3), (4, 4) };
                break;
            case 12: // Forme en T
                positions = new List<(int x, int y)> { (2, 0), (0, 2), (2, 2), (4, 2), (2, 4) };
                break;
            case 13: // Double ligne avec hub
                positions = new List<(int x, int y)> { (1, 1), (3, 1), (2, 2), (1, 3), (3, 3) };
                break;
            default: // Pattern aléatoire équilibré
                positions = new List<(int x, int y)> { (1, 1), (3, 1), (0, 2), (2, 2), (4, 2), (1, 3), (3, 3) };
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
                        // Vérifier que les îles sont alignées avant de créer le pont
                        if (nearest.X == island.X || nearest.Y == island.Y)
                        {
                            var direction = nearest.X == island.X ? BridgeDirection.Vertical : BridgeDirection.Horizontal;
                            bridges.Add(CreateBridge(nearest, island, false, direction));
                            connected.Add(island);
                        }
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
                        // Vérifier que les îles sont alignées avant de créer le pont
                        if (nearest.X == island.X || nearest.Y == island.Y)
                        {
                            var direction = nearest.X == island.X ? BridgeDirection.Vertical : BridgeDirection.Horizontal;
                            bridges.Add(CreateBridge(nearest, island, false, direction));
                            connected.Add(island);
                        }
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

        // Patterns améliorés pour niveau moyen - designs plus complexes et variés
        var themeIndex = (int)theme;
        var patternIndex = themeIndex % 12;
        List<(int x, int y)> positions;
        
        switch (patternIndex)
        {
            case 0: // Colonnes doubles avec hubs
                positions = new List<(int x, int y)>
                {
                    (2, 1), (2, 3), (2, 5), (6, 1), (6, 3), (6, 5), (4, 3), (4, 6)
                };
                break;
            case 1: // Grille 3x3 avec connexions
                positions = new List<(int x, int y)>
                {
                    (1, 2), (3, 2), (5, 2), (2, 4), (4, 4), (6, 4), (3, 6), (5, 6)
                };
                break;
            case 2: // Croix avec branches
                positions = new List<(int x, int y)>
                {
                    (3, 1), (1, 3), (3, 3), (5, 3), (7, 3), (3, 5), (3, 7)
                };
                break;
            case 3: // Double ligne avec hubs centraux
                positions = new List<(int x, int y)>
                {
                    (1, 1), (3, 1), (5, 1), (4, 3), (1, 5), (3, 5), (5, 5), (7, 5)
                };
                break;
            case 4: // Forme en étoile
                positions = new List<(int x, int y)>
                {
                    (3, 2), (1, 4), (3, 4), (5, 4), (3, 6), (6, 3), (6, 5)
                };
                break;
            case 5: // Rectangle avec diagonales
                positions = new List<(int x, int y)>
                {
                    (2, 2), (5, 2), (2, 5), (5, 5), (3, 3), (4, 3), (3, 4), (4, 4)
                };
                break;
            case 6: // Lignes horizontales connectées
                positions = new List<(int x, int y)>
                {
                    (1, 1), (3, 1), (5, 1), (2, 3), (4, 3), (6, 3), (3, 5), (5, 5)
                };
                break;
            case 7: // Hub central avec 4 branches
                positions = new List<(int x, int y)>
                {
                    (3, 2), (1, 4), (3, 4), (5, 4), (3, 6), (6, 2), (6, 6)
                };
                break;
            case 8: // Zigzag vertical
                positions = new List<(int x, int y)>
                {
                    (2, 1), (4, 2), (2, 3), (4, 4), (2, 5), (4, 6), (6, 7)
                };
                break;
            case 9: // Double colonne asymétrique
                positions = new List<(int x, int y)>
                {
                    (1, 1), (1, 3), (1, 5), (5, 2), (5, 4), (5, 6), (3, 4), (7, 4)
                };
                break;
            case 10: // Forme en H
                positions = new List<(int x, int y)>
                {
                    (2, 1), (2, 3), (2, 5), (4, 3), (6, 1), (6, 3), (6, 5)
                };
                break;
            default: // Pattern complexe avec multiple hubs
                positions = new List<(int x, int y)>
                {
                    (1, 2), (3, 2), (5, 2), (2, 4), (4, 4), (6, 4), (3, 6), (5, 6)
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
            // Connecter les colonnes - vérifier les croisements avant d'ajouter
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
                        var newBridge = CreateBridge(left, right, false, BridgeDirection.Horizontal);
                        
                        // Vérifier qu'il n'y a pas de croisement avec les ponts existants
                        bool wouldIntersect = false;
                        foreach (var existingBridge in bridges)
                        {
                            if (DoBridgesIntersect(newBridge, existingBridge, islands))
                            {
                                wouldIntersect = true;
                                break;
                            }
                        }
                        
                        if (!wouldIntersect)
                        {
                            bridges.Add(newBridge);
                        }
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
            // Connecter les lignes - vérifier les croisements avant d'ajouter
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
                        var newBridge = CreateBridge(top, bottom, false, BridgeDirection.Vertical);
                        
                        // Vérifier qu'il n'y a pas de croisement avec les ponts existants
                        bool wouldIntersect = false;
                        foreach (var existingBridge in bridges)
                        {
                            if (DoBridgesIntersect(newBridge, existingBridge, islands))
                            {
                                wouldIntersect = true;
                                break;
                            }
                        }
                        
                        if (!wouldIntersect)
                        {
                            bridges.Add(newBridge);
                        }
                    }
                }
            }
        }
        else
        {
            // Solution en étoile (hub central) - améliorée pour éviter les croisements
            var center = islands.OrderBy(i => Math.Abs(i.X - 4) + Math.Abs(i.Y - 4)).First();
            
            // Connecter d'abord les îles alignées verticalement avec le centre
            var verticalIslands = islands.Where(i => i != center && i.X == center.X)
                .OrderBy(i => Math.Abs(i.Y - center.Y))
                .ToList();
            for (int i = 0; i < verticalIslands.Count; i++)
            {
                var island = verticalIslands[i];
                var newBridge = CreateBridge(center, island, false, BridgeDirection.Vertical);
                
                // Vérifier les croisements avec les ponts existants
                bool wouldIntersect = false;
                foreach (var existingBridge in bridges)
                {
                    if (DoBridgesIntersect(newBridge, existingBridge, islands))
                    {
                        wouldIntersect = true;
                        break;
                    }
                }
                
                if (!wouldIntersect)
                {
                    bridges.Add(newBridge);
                }
            }
            
            // Ensuite connecter les îles alignées horizontalement avec le centre
            var horizontalIslands = islands.Where(i => i != center && i.Y == center.Y)
                .OrderBy(i => Math.Abs(i.X - center.X))
                .ToList();
            for (int i = 0; i < horizontalIslands.Count; i++)
            {
                var island = horizontalIslands[i];
                var newBridge = CreateBridge(center, island, false, BridgeDirection.Horizontal);
                
                // Vérifier les croisements avec les ponts existants
                bool wouldIntersect = false;
                foreach (var existingBridge in bridges)
                {
                    if (DoBridgesIntersect(newBridge, existingBridge, islands))
                    {
                        wouldIntersect = true;
                        break;
                    }
                }
                
                if (!wouldIntersect)
                {
                    bridges.Add(newBridge);
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

        // Patterns améliorés pour niveau difficile - designs complexes et variés
        var themeIndex = (int)theme;
        var patternIndex = themeIndex % 10;
        List<(int x, int y)> positions;
        
        switch (patternIndex)
        {
            case 0: // Double colonne avec hubs multiples
                positions = new List<(int x, int y)>
                {
                    (3, 2), (9, 2), (3, 4), (6, 4), (9, 4), (3, 6), (9, 6),
                    (3, 8), (6, 8), (9, 8), (3, 10), (9, 10), (6, 11)
                };
                break;
            case 1: // Grille avec connexions croisées
                positions = new List<(int x, int y)>
                {
                    (2, 2), (5, 2), (8, 2), (2, 5), (5, 5), (8, 5),
                    (2, 8), (5, 8), (8, 8), (5, 10), (2, 11), (8, 11)
                };
                break;
            case 2: // Forme en étoile avec branches
                positions = new List<(int x, int y)>
                {
                    (5, 1), (2, 3), (5, 3), (8, 3), (1, 5), (5, 5), (9, 5),
                    (2, 7), (5, 7), (8, 7), (5, 9), (3, 11), (7, 11)
                };
                break;
            case 3: // Triple colonne
                positions = new List<(int x, int y)>
                {
                    (2, 2), (6, 2), (10, 2), (2, 4), (6, 4), (10, 4),
                    (2, 6), (6, 6), (10, 6), (2, 8), (6, 8), (10, 8)
                };
                break;
            case 4: // Hub central avec réseau
                positions = new List<(int x, int y)>
                {
                    (5, 2), (2, 4), (5, 4), (8, 4), (1, 6), (5, 6), (9, 6),
                    (2, 8), (5, 8), (8, 8), (5, 10), (3, 11), (7, 11)
                };
                break;
            case 5: // Lignes horizontales avec connexions verticales
                positions = new List<(int x, int y)>
                {
                    (1, 2), (4, 2), (7, 2), (10, 2), (3, 4), (6, 4), (9, 4),
                    (2, 6), (5, 6), (8, 6), (4, 8), (7, 8), (5, 10)
                };
                break;
            case 6: // Rectangle avec diagonales
                positions = new List<(int x, int y)>
                {
                    (2, 2), (6, 2), (10, 2), (2, 5), (6, 5), (10, 5),
                    (2, 8), (6, 8), (10, 8), (4, 10), (8, 10)
                };
                break;
            case 7: // Zigzag complexe
                positions = new List<(int x, int y)>
                {
                    (3, 1), (7, 2), (2, 4), (6, 4), (10, 4), (4, 6), (8, 6),
                    (1, 8), (5, 8), (9, 8), (3, 10), (7, 10), (5, 11)
                };
                break;
            case 8: // Forme en H avec extensions
                positions = new List<(int x, int y)>
                {
                    (2, 2), (2, 4), (2, 6), (5, 4), (8, 2), (8, 4), (8, 6),
                    (2, 8), (5, 8), (8, 8), (3, 10), (7, 10), (5, 11)
                };
                break;
            default: // Pattern complexe équilibré
                positions = new List<(int x, int y)>
                {
                    (3, 2), (9, 2), (3, 4), (6, 4), (9, 4), (3, 6), (9, 6),
                    (3, 8), (6, 8), (9, 8), (3, 10), (9, 10), (6, 11)
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

        // Générer une solution dynamique basée sur les positions des îles
        // Connecter les îles de manière intelligente pour éviter les croisements
        var usedPositions = new HashSet<(int, int)>();
        foreach (var island in islands)
        {
            usedPositions.Add((island.X, island.Y));
        }

        // Grouper les îles par colonne et ligne
        var byColumn = islands.GroupBy(i => i.X).Where(g => g.Count() > 1).ToList();
        var byRow = islands.GroupBy(i => i.Y).Where(g => g.Count() > 1).ToList();

        // Connecter les îles dans chaque colonne verticalement
        foreach (var col in byColumn)
        {
            var colIslands = col.OrderBy(i => i.Y).ToList();
            for (int i = 0; i < colIslands.Count - 1; i++)
            {
                var newBridge = CreateBridge(colIslands[i], colIslands[i + 1], false, BridgeDirection.Vertical);
                
                // Vérifier les croisements
                bool wouldIntersect = false;
                foreach (var existingBridge in bridges)
                {
                    if (DoBridgesIntersect(newBridge, existingBridge, islands))
                    {
                        wouldIntersect = true;
                        break;
                    }
                }
                
                if (!wouldIntersect)
                {
                    bridges.Add(newBridge);
                }
            }
        }

        // Connecter les îles dans chaque ligne horizontalement
        foreach (var row in byRow)
        {
            var rowIslands = row.OrderBy(i => i.X).ToList();
            for (int i = 0; i < rowIslands.Count - 1; i++)
            {
                var newBridge = CreateBridge(rowIslands[i], rowIslands[i + 1], false, BridgeDirection.Horizontal);
                
                // Vérifier les croisements
                bool wouldIntersect = false;
                foreach (var existingBridge in bridges)
                {
                    if (DoBridgesIntersect(newBridge, existingBridge, islands))
                    {
                        wouldIntersect = true;
                        break;
                    }
                }
                
                if (!wouldIntersect)
                {
                    bridges.Add(newBridge);
                }
            }
        }

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

        // Patterns améliorés pour niveau expert - designs très complexes et variés
        var themeIndex = (int)theme;
        var patternIndex = themeIndex % 12;
        List<(int x, int y)> positions;
        
        switch (patternIndex)
        {
            case 0: // Double colonne avec hubs multiples
                positions = new List<(int x, int y)>
                {
                    (3, 2), (11, 2), (3, 4), (7, 4), (11, 4), (3, 6), (11, 6),
                    (3, 8), (5, 8), (9, 8), (11, 8), (3, 10), (11, 10),
                    (3, 12), (7, 12), (11, 12), (3, 14), (11, 14)
                };
                break;
            case 1: // Grille 3x5 avec connexions
                positions = new List<(int x, int y)>
                {
                    (2, 2), (7, 2), (12, 2), (2, 5), (7, 5), (12, 5),
                    (2, 8), (7, 8), (12, 8), (2, 11), (7, 11), (12, 11),
                    (4, 13), (9, 13), (7, 14)
                };
                break;
            case 2: // Étoile complexe avec réseau
                positions = new List<(int x, int y)>
                {
                    (7, 1), (3, 3), (7, 3), (11, 3), (1, 5), (5, 5), (7, 5), (9, 5), (13, 5),
                    (3, 7), (7, 7), (11, 7), (2, 9), (7, 9), (12, 9),
                    (5, 11), (7, 11), (9, 11), (7, 13), (4, 14), (10, 14)
                };
                break;
            case 3: // Triple colonne avec hubs
                positions = new List<(int x, int y)>
                {
                    (2, 2), (7, 2), (12, 2), (2, 4), (7, 4), (12, 4),
                    (2, 6), (7, 6), (12, 6), (2, 8), (7, 8), (12, 8),
                    (2, 10), (7, 10), (12, 10), (2, 12), (7, 12), (12, 12)
                };
                break;
            case 4: // Hub central avec branches multiples
                positions = new List<(int x, int y)>
                {
                    (7, 2), (3, 4), (7, 4), (11, 4), (1, 6), (5, 6), (7, 6), (9, 6), (13, 6),
                    (3, 8), (7, 8), (11, 8), (2, 10), (7, 10), (12, 10),
                    (5, 12), (7, 12), (9, 12), (7, 14), (4, 14), (10, 14)
                };
                break;
            case 5: // Lignes horizontales avec connexions
                positions = new List<(int x, int y)>
                {
                    (1, 2), (5, 2), (9, 2), (13, 2), (3, 4), (7, 4), (11, 4),
                    (2, 6), (6, 6), (10, 6), (14, 6), (4, 8), (8, 8), (12, 8),
                    (1, 10), (7, 10), (13, 10), (5, 12), (9, 12), (7, 14)
                };
                break;
            case 6: // Rectangle avec diagonales
                positions = new List<(int x, int y)>
                {
                    (2, 2), (7, 2), (12, 2), (2, 5), (7, 5), (12, 5),
                    (2, 8), (7, 8), (12, 8), (2, 11), (7, 11), (12, 11),
                    (4, 13), (9, 13), (7, 14)
                };
                break;
            case 7: // Zigzag complexe
                positions = new List<(int x, int y)>
                {
                    (3, 1), (9, 2), (2, 4), (7, 4), (12, 4), (4, 6), (9, 6),
                    (1, 8), (6, 8), (11, 8), (3, 10), (8, 10), (13, 10),
                    (5, 12), (10, 12), (7, 14)
                };
                break;
            case 8: // Forme en H avec extensions
                positions = new List<(int x, int y)>
                {
                    (2, 2), (2, 4), (2, 6), (7, 4), (12, 2), (12, 4), (12, 6),
                    (2, 8), (7, 8), (12, 8), (2, 10), (7, 10), (12, 10),
                    (4, 12), (10, 12), (7, 14)
                };
                break;
            case 9: // Réseau complexe
                positions = new List<(int x, int y)>
                {
                    (4, 2), (8, 2), (2, 4), (6, 4), (10, 4), (4, 6), (8, 6), (12, 6),
                    (3, 8), (7, 8), (11, 8), (5, 10), (9, 10), (2, 12), (7, 12), (12, 12),
                    (7, 14)
                };
                break;
            case 10: // Symétrie parfaite
                positions = new List<(int x, int y)>
                {
                    (3, 2), (11, 2), (1, 4), (7, 4), (13, 4), (3, 6), (7, 6), (11, 6),
                    (1, 8), (7, 8), (13, 8), (3, 10), (7, 10), (11, 10),
                    (1, 12), (7, 12), (13, 12), (3, 14), (11, 14)
                };
                break;
            default: // Pattern complexe équilibré
                positions = new List<(int x, int y)>
                {
                    (3, 2), (11, 2), (3, 4), (7, 4), (11, 4), (3, 6), (11, 6),
                    (3, 8), (5, 8), (9, 8), (11, 8), (3, 10), (11, 10),
                    (3, 12), (7, 12), (11, 12), (3, 14), (11, 14)
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

        // Générer une solution dynamique basée sur les positions des îles
        // Connecter les îles de manière intelligente pour éviter les croisements
        var usedPositions = new HashSet<(int, int)>();
        foreach (var island in islands)
        {
            usedPositions.Add((island.X, island.Y));
        }

        // Grouper les îles par colonne et ligne
        var byColumn = islands.GroupBy(i => i.X).Where(g => g.Count() > 1).ToList();
        var byRow = islands.GroupBy(i => i.Y).Where(g => g.Count() > 1).ToList();

        // Connecter les îles dans chaque colonne verticalement
        foreach (var col in byColumn)
        {
            var colIslands = col.OrderBy(i => i.Y).ToList();
            for (int i = 0; i < colIslands.Count - 1; i++)
            {
                var newBridge = CreateBridge(colIslands[i], colIslands[i + 1], false, BridgeDirection.Vertical);
                
                // Vérifier les croisements
                bool wouldIntersect = false;
                foreach (var existingBridge in bridges)
                {
                    if (DoBridgesIntersect(newBridge, existingBridge, islands))
                    {
                        wouldIntersect = true;
                        break;
                    }
                }
                
                if (!wouldIntersect)
                {
                    bridges.Add(newBridge);
                }
            }
        }

        // Connecter les îles dans chaque ligne horizontalement
        foreach (var row in byRow)
        {
            var rowIslands = row.OrderBy(i => i.X).ToList();
            for (int i = 0; i < rowIslands.Count - 1; i++)
            {
                // Parfois utiliser des ponts doubles pour plus de complexité
                bool isDouble = random.Next(100) < 20; // 20% de chance de pont double
                var newBridge = CreateBridge(rowIslands[i], rowIslands[i + 1], isDouble, BridgeDirection.Horizontal);
                
                // Vérifier les croisements
                bool wouldIntersect = false;
                foreach (var existingBridge in bridges)
                {
                    if (DoBridgesIntersect(newBridge, existingBridge, islands))
                    {
                        wouldIntersect = true;
                        break;
                    }
                }
                
                if (!wouldIntersect)
                {
                    bridges.Add(newBridge);
                }
            }
        }

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
            DifficultyLevel.Hard => Math.Max(14, Math.Min(baseArea / 4, 22)), // Beaucoup plus d'îles pour niveau difficile
            DifficultyLevel.Expert => Math.Max(12, Math.Min(baseArea / 4, 25)),
            _ => 5
        };
        
        // Pour les grandes grilles difficiles, augmenter significativement le nombre d'îles
        if (baseArea > 150 && difficulty == DifficultyLevel.Hard)
        {
            islandCount = Math.Max(islandCount, 16); // Minimum 16 îles pour grandes grilles difficiles
        }
        
        // Pour les très grandes grilles (18x13+), encore plus d'îles
        if (baseArea > 200 && difficulty == DifficultyLevel.Hard)
        {
            islandCount = Math.Max(islandCount, 18); // Minimum 18 îles pour très grandes grilles
        }

        // Placer les îles de manière variée, en s'assurant qu'elles sont alignées
        var positions = new List<(int x, int y)>();
            int attempts = 0;
        int maxAttempts = 1000;

        // Pour les grandes grilles, augmenter la distance minimale entre îles
        // Pour niveau difficile, réduire légèrement la distance pour plus de complexité
        int minDistance = difficulty == DifficultyLevel.Hard 
            ? ((width > 15 || height > 15) ? 2 : 2)  // Distance réduite pour plus de densité
            : ((width > 15 || height > 15) ? 3 : 2);
        
        // Placer la première île aléatoirement
        if (islandCount > 0)
        {
            int firstX = _random.Next(1, width - 1);
            int firstY = _random.Next(1, height - 1);
            positions.Add((firstX, firstY));
            usedPositions.Add((firstX, firstY));
        }
        
        // Placer les autres îles en s'assurant qu'elles sont alignées avec au moins une île existante
        while (positions.Count < islandCount && attempts < maxAttempts)
        {
            int x, y;
            
            // Essayer de placer une île alignée avec une île existante
            if (positions.Count > 0)
            {
                var existingIsland = positions[_random.Next(positions.Count)];
                
                // 50% de chance d'être sur la même ligne, 50% sur la même colonne
                if (_random.Next(2) == 0)
                {
                    // Même ligne (horizontal)
                    x = _random.Next(1, width - 1);
                    y = existingIsland.y;
                }
                else
                {
                    // Même colonne (vertical)
                    x = existingIsland.x;
                    y = _random.Next(1, height - 1);
                }
            }
            else
            {
                // Si aucune île n'existe encore, placer aléatoirement
                x = _random.Next(1, width - 1);
                y = _random.Next(1, height - 1);
            }

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
        
        // Si on n'a pas assez d'îles, réduire la distance minimale mais toujours s'assurer de l'alignement
        if (positions.Count < Math.Max(8, islandCount / 2) && minDistance > 1)
        {
            minDistance = 1;
            while (positions.Count < islandCount && attempts < maxAttempts * 2)
            {
                int x, y;
                
                // Toujours s'assurer de l'alignement avec une île existante
                if (positions.Count > 0)
                {
                    var existingIsland = positions[_random.Next(positions.Count)];
                    
                    if (_random.Next(2) == 0)
                    {
                        x = _random.Next(1, width - 1);
                        y = existingIsland.y;
                    }
                    else
                    {
                        x = existingIsland.x;
                        y = _random.Next(1, height - 1);
                    }
                }
                else
                {
                    x = _random.Next(1, width - 1);
                    y = _random.Next(1, height - 1);
                }

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

        // Limiter le nombre de tentatives pour éviter les boucles infinies
        int maxConnectionAttempts = islands.Count * 10;
        int connectionAttempts = 0;
        
        while (unconnectedIslands.Count > 0 && connectionAttempts < maxConnectionAttempts)
        {
            connectionAttempts++;
            
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
                            // Augmenter la probabilité de ponts doubles pour niveau difficile
                            int doubleBridgeChance = difficulty == DifficultyLevel.Hard ? 25 : 
                                                     difficulty == DifficultyLevel.Expert ? 30 : 10;
                            bool isDouble = _random.Next(100) < doubleBridgeChance;
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
                var newBridge = CreateBridge(from, to, isDouble, direction);
                
                // Vérifier qu'il n'y a pas de croisement avec les ponts existants
                bool wouldIntersect = false;
                foreach (var existingBridge in bridges)
                {
                    if (DoBridgesIntersect(newBridge, existingBridge, islands))
                    {
                        wouldIntersect = true;
                        break;
                    }
                }
                
                if (!wouldIntersect)
                {
                    bridges.Add(newBridge);
                    unconnectedIslands.Remove(toIdx);
                    connectedIslands.Add(toIdx);
                }
                else
                {
                    // Essayer de trouver une autre connexion qui ne crée pas de croisement
                    bool foundAlternative = false;
                    foreach (var connectedIdx in connectedIslands)
                    {
                        if (connectedIdx == fromIdx) continue;
                        foreach (var unconnectedIdx in unconnectedIslands)
                        {
                            if (unconnectedIdx == toIdx) continue;
                            var altFrom = islands[connectedIdx];
                            var altTo = islands[unconnectedIdx];
                            
                            if (CanConnectIslands(altFrom, altTo, usedPositions))
                            {
                                BridgeDirection altDirection = altFrom.X == altTo.X ? BridgeDirection.Vertical : BridgeDirection.Horizontal;
                                var altBridge = CreateBridge(altFrom, altTo, isDouble, altDirection);
                                
                                bool altWouldIntersect = false;
                                foreach (var existingBridge in bridges)
                                {
                                    if (DoBridgesIntersect(altBridge, existingBridge, islands))
                                    {
                                        altWouldIntersect = true;
                                        break;
                                    }
                                }
                                
                                if (!altWouldIntersect)
                                {
                                    bridges.Add(altBridge);
                                    unconnectedIslands.Remove(unconnectedIdx);
                                    connectedIslands.Add(unconnectedIdx);
                                    foundAlternative = true;
                                    break;
                                }
                            }
                        }
                        if (foundAlternative) break;
                    }
                    
                    // Si aucune alternative n'a été trouvée, forcer la connexion quand même
                    // (le croisement sera géré par EnsureAllIslandsConnected)
                    if (!foundAlternative)
                    {
                        bridges.Add(newBridge);
                        unconnectedIslands.Remove(toIdx);
                        connectedIslands.Add(toIdx);
                    }
                }
            }
            else
            {
                // Fallback : connecter n'importe quelle île alignée
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
                            // Augmenter la probabilité de ponts doubles pour niveau difficile
                            int doubleBridgeChance = difficulty == DifficultyLevel.Hard ? 30 : 
                                                     difficulty == DifficultyLevel.Expert ? 35 : 15;
                            bool isDouble = _random.Next(100) < doubleBridgeChance;
                            var newBridge = CreateBridge(from, to, isDouble, direction);
                            
                            bridges.Add(newBridge);
                            unconnectedIslands.Remove(unconnectedIdx);
                            connectedIslands.Add(unconnectedIdx);
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                }
                if (!found) break; // Sortir si aucune connexion n'est possible
            }
        }

        // Ajouter quelques ponts supplémentaires selon la difficulté
        // Pour le niveau difficile, ajouter beaucoup plus de ponts pour créer de la complexité
        int extraBridges = difficulty switch
        {
            DifficultyLevel.Easy => 0,
            DifficultyLevel.Medium => Math.Min(1, islands.Count / 4),
            DifficultyLevel.Hard => Math.Min(5, islands.Count / 2), // Beaucoup plus de ponts supplémentaires
            DifficultyLevel.Expert => Math.Min(3, islands.Count / 2),
            _ => 0
        };

        // Ajouter des ponts supplémentaires en vérifiant les croisements
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
                // Essayer plusieurs candidats jusqu'à trouver un qui ne crée pas de croisement
                var shuffledCandidates = candidates.OrderBy(x => _random.Next()).ToList();
                
                foreach (var (fromIdx, toIdx) in shuffledCandidates)
                {
                    var from = islands[fromIdx];
                    var to = islands[toIdx];
                    BridgeDirection direction = from.X == to.X ? BridgeDirection.Vertical : BridgeDirection.Horizontal;
                    // Augmenter significativement la probabilité de ponts doubles pour niveau difficile
                    int doubleBridgeChance = difficulty == DifficultyLevel.Hard ? 40 : 
                                             difficulty == DifficultyLevel.Expert ? 45 : 25;
                    bool isDouble = _random.Next(100) < doubleBridgeChance;
                    var newBridge = CreateBridge(from, to, isDouble, direction);
                    
                    // Vérifier qu'il n'y a pas de croisement
                    bool wouldIntersect = false;
                    foreach (var existingBridge in bridges)
                    {
                        if (DoBridgesIntersect(newBridge, existingBridge, islands))
                        {
                            wouldIntersect = true;
                            break;
                        }
                    }
                    
                    if (!wouldIntersect)
                    {
                        bridges.Add(newBridge);
                        break;
                    }
                }
                
                // Si aucun pont valide n'a été trouvé, on passe au suivant
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
    private void EnsureAllIslandsConnected(List<Island> islands, List<Bridge> bridges, int? puzzleId = null)
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
                            var newBridge = CreateBridge(nearest, island, false, direction);
                            
                            // Ajouter le pont sans vérifier les croisements pour accélérer
                            // Les croisements seront gérés plus tard si nécessaire
                            if (puzzleId.HasValue)
                            {
                                newBridge.PuzzleId = puzzleId.Value;
                            }
                            bridges.Add(newBridge);
                            connectedIslands.Add(island);
                            adjacency[nearest].Add(island);
                            adjacency[island].Add(nearest);
                            addedConnection = true;
                            break; // Sortir de la boucle pour recommencer le parcours
                        }
                    }
                }
                
                // Si aucune connexion n'a été ajoutée, forcer la connexion de la première île isolée
                // en trouvant la connexion la plus proche possible (simplifié pour éviter les blocages)
                if (!addedConnection)
                {
                    var unconnected = islands.Where(i => !connectedIslands.Contains(i)).ToList();
                    if (unconnected.Count > 0)
                    {
                        var isolated = unconnected[0];
                        
                        // Trouver la première île connectée qui peut être reliée (simplifié)
                        var target = connectedIslands
                            .Where(i => CanConnectIslands(i, isolated, usedPositions))
                            .OrderBy(i => Math.Abs(i.X - isolated.X) + Math.Abs(i.Y - isolated.Y))
                            .FirstOrDefault();
                        
                        // Si toujours aucune île alignée, prendre la plus proche île connectée
                        if (target == null && connectedIslands.Count > 0)
                        {
                            target = connectedIslands
                                .OrderBy(i => Math.Abs(i.X - isolated.X) + Math.Abs(i.Y - isolated.Y))
                                .FirstOrDefault();
                        }
                        
                        if (target != null)
                        {
                            // Vérifier que les îles sont alignées avant de créer le pont
                            if (target.X == isolated.X || target.Y == isolated.Y)
                            {
                                var direction = target.X == isolated.X ? BridgeDirection.Vertical : BridgeDirection.Horizontal;
                                var newBridge = CreateBridge(target, isolated, false, direction);
                                
                                // Ajouter le pont sans vérifier les croisements pour accélérer
                                if (puzzleId.HasValue)
                                {
                                    newBridge.PuzzleId = puzzleId.Value;
                                }
                                bridges.Add(newBridge);
                                connectedIslands.Add(isolated);
                                adjacency[target].Add(isolated);
                                adjacency[isolated].Add(target);
                                addedConnection = true;
                            }
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
        // VALIDATION : S'assurer que les îles sont alignées (même ligne ou même colonne)
        if (from.X != to.X && from.Y != to.Y)
        {
            throw new InvalidOperationException($"Impossible de créer un pont diagonal entre ({from.X}, {from.Y}) et ({to.X}, {to.Y}). Les ponts doivent être horizontaux ou verticaux.");
        }
        
        // Corriger la direction si nécessaire
        BridgeDirection correctDirection;
        if (from.X == to.X)
        {
            correctDirection = BridgeDirection.Vertical;
        }
        else if (from.Y == to.Y)
        {
            correctDirection = BridgeDirection.Horizontal;
        }
        else
        {
            throw new InvalidOperationException($"Impossible de déterminer la direction du pont entre ({from.X}, {from.Y}) et ({to.X}, {to.Y})");
        }
        
        return new Bridge
        {
            FromIslandId = from.Id > 0 ? from.Id : 0,
            ToIslandId = to.Id > 0 ? to.Id : 0,
            FromIsland = from,
            ToIsland = to,
            IsDouble = isDouble,
            Direction = correctDirection
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

        // Créer un dictionnaire pour accéder rapidement aux îles par ID
        var islandsById = islands.ToDictionary(i => i.Id, i => i);

        // Compter les ponts pour chaque île en utilisant les IDs
        foreach (var bridge in bridges)
        {
            // Utiliser FromIslandId et ToIslandId pour trouver les îles dans la liste
            if (bridge.FromIslandId > 0 && islandsById.TryGetValue(bridge.FromIslandId, out var fromIsland))
            {
                fromIsland.RequiredBridges += bridge.IsDouble ? 2 : 1;
            }
            else if (bridge.FromIsland != null)
            {
                // Fallback : utiliser la référence d'objet si l'ID n'est pas disponible
                var fromIslandByRef = islands.FirstOrDefault(i => i.X == bridge.FromIsland.X && i.Y == bridge.FromIsland.Y);
                if (fromIslandByRef != null)
                {
                    fromIslandByRef.RequiredBridges += bridge.IsDouble ? 2 : 1;
                }
            }

            if (bridge.ToIslandId > 0 && islandsById.TryGetValue(bridge.ToIslandId, out var toIsland))
            {
                toIsland.RequiredBridges += bridge.IsDouble ? 2 : 1;
            }
            else if (bridge.ToIsland != null)
            {
                // Fallback : utiliser la référence d'objet si l'ID n'est pas disponible
                var toIslandByRef = islands.FirstOrDefault(i => i.X == bridge.ToIsland.X && i.Y == bridge.ToIsland.Y);
                if (toIslandByRef != null)
                {
                    toIslandByRef.RequiredBridges += bridge.IsDouble ? 2 : 1;
                }
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
                        // Vérifier que les îles sont alignées avant de créer le pont
                        if (nearest.X == island.X || nearest.Y == island.Y)
                        {
                            var direction = nearest.X == island.X ? BridgeDirection.Vertical : BridgeDirection.Horizontal;
                            var newBridge = CreateBridge(nearest, island, false, direction);
                            
                            // Vérifier que ce pont ne croise pas un pont existant
                            bool wouldIntersect = false;
                            foreach (var existingBridge in bridges)
                            {
                                if (DoBridgesIntersect(newBridge, existingBridge, islands))
                                {
                                    wouldIntersect = true;
                                    break;
                                }
                            }
                            
                            // Si pas de croisement, ajouter le pont
                            if (!wouldIntersect)
                            {
                                // Note: PuzzleId sera défini par l'appelant si nécessaire
                                bridges.Add(newBridge);
                                // Recalculer pour cette île (le pont compte pour 1)
                                island.RequiredBridges += 1;
                                nearest.RequiredBridges += 1;
                                continue; // Passer à l'île suivante
                            }
                        }
                        else
                        {
                            // Si les îles ne sont pas alignées, chercher une île intermédiaire alignée
                            var intermediate = connectedIslands
                                .Where(i => (i.X == island.X || i.Y == island.Y) && 
                                           (i.X == nearest.X || i.Y == nearest.Y))
                                .FirstOrDefault();
                            
                            if (intermediate != null)
                            {
                                // Connecter l'île isolée à l'intermédiaire
                                var dir1 = intermediate.X == island.X ? BridgeDirection.Vertical : BridgeDirection.Horizontal;
                                var newBridge1 = CreateBridge(intermediate, island, false, dir1);
                                
                                // Vérifier que ce pont ne croise pas un pont existant
                                bool wouldIntersect1 = false;
                                foreach (var existingBridge in bridges)
                                {
                                    if (DoBridgesIntersect(newBridge1, existingBridge, islands))
                                    {
                                        wouldIntersect1 = true;
                                        break;
                                    }
                                }
                                
                                // Si pas de croisement, ajouter le pont
                                if (!wouldIntersect1)
                                {
                                    bridges.Add(newBridge1);
                                    island.RequiredBridges += 1;
                                    intermediate.RequiredBridges += 1;
                                    continue; // Passer à l'île suivante
                                }
                            }
                        }
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
    /// <summary>
    /// Valide qu'il n'y a pas de croisements illégaux entre les ponts
    /// Les ponts ne peuvent se croiser que s'ils sont perpendiculaires
    /// </summary>
    private void ValidateNoIllegalCrossings(List<Island> islands, List<Bridge> bridges)
    {
        for (int i = 0; i < bridges.Count; i++)
        {
            for (int j = i + 1; j < bridges.Count; j++)
            {
                if (DoBridgesIntersect(bridges[i], bridges[j], islands))
                {
                    throw new InvalidOperationException(
                        $"Croisement illégal détecté entre les ponts ({bridges[i].FromIsland?.X}, {bridges[i].FromIsland?.Y})-({bridges[i].ToIsland?.X}, {bridges[i].ToIsland?.Y}) " +
                        $"et ({bridges[j].FromIsland?.X}, {bridges[j].FromIsland?.Y})-({bridges[j].ToIsland?.X}, {bridges[j].ToIsland?.Y})");
                }
            }
        }
    }

    /// <summary>
    /// Vérifie si deux ponts se croisent (version pour objets Bridge)
    /// </summary>
    private bool DoBridgesIntersect(Bridge bridge1, Bridge bridge2, List<Island> islands)
    {
        if (bridge1.FromIsland == null || bridge1.ToIsland == null || 
            bridge2.FromIsland == null || bridge2.ToIsland == null)
            return false;

        // Déterminer la direction de chaque pont
        bool bridge1IsHorizontal = bridge1.FromIsland.Y == bridge1.ToIsland.Y;
        bool bridge2IsHorizontal = bridge2.FromIsland.Y == bridge2.ToIsland.Y;

        // Deux ponts parallèles ne peuvent pas se croiser
        if (bridge1IsHorizontal == bridge2IsHorizontal)
            return false;

        // Si le premier pont est horizontal et le second vertical
        if (bridge1IsHorizontal)
        {
            int bridge1MinX = Math.Min(bridge1.FromIsland.X, bridge1.ToIsland.X);
            int bridge1MaxX = Math.Max(bridge1.FromIsland.X, bridge1.ToIsland.X);
            int bridge1Y = bridge1.FromIsland.Y;

            int bridge2MinY = Math.Min(bridge2.FromIsland.Y, bridge2.ToIsland.Y);
            int bridge2MaxY = Math.Max(bridge2.FromIsland.Y, bridge2.ToIsland.Y);
            int bridge2X = bridge2.FromIsland.X;

            // Vérifier si le pont vertical passe à travers le pont horizontal
            // Exclure les cas où ils se rencontrent à une île (c'est autorisé)
            return bridge2X > bridge1MinX && bridge2X < bridge1MaxX && 
                   bridge1Y > bridge2MinY && bridge1Y < bridge2MaxY;
        }
        else // Le premier pont est vertical et le second horizontal
        {
            return DoBridgesIntersect(bridge2, bridge1, islands);
        }
    }

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
                try
                {
                    // Chaque thème a une taille unique mais cohérente avec la difficulté
                    var (width, height) = GetDimensionsForTheme(difficulty, theme);
                    
                    // Générer un nouveau puzzle unique pour ce thème avec sa taille spécifique
                    var newPuzzle = await GeneratePuzzleAsync(width, height, difficulty, theme);
                    puzzles.Add(newPuzzle);
                }
                catch (Exception ex)
                {
                    // Si la génération échoue pour un thème, continuer avec les autres thèmes
                    // plutôt que de faire échouer toute la requête
                    _logger?.LogWarning(ex, "Impossible de générer un puzzle pour le thème {Theme} et la difficulté {Difficulty}. Continuation avec les autres thèmes.", theme, difficulty);
                }
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
