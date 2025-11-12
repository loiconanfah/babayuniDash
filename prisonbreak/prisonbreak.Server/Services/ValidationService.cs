using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Services;

/// <summary>
/// Service de validation des solutions Hashi
/// Implémente toutes les règles du jeu pour vérifier si une solution est correcte
/// </summary>
public class ValidationService : IValidationService
{
    /// <summary>
    /// Valide une solution complète d'un puzzle
    /// Vérifie toutes les règles du jeu Hashi
    /// </summary>
    public ValidationResultDto ValidateSolution(Puzzle puzzle, List<BridgeDto> playerBridges)
    {
        var result = new ValidationResultDto();

        // Règle 1: Vérifier que chaque île a le bon nombre de ponts
        foreach (var island in puzzle.Islands)
        {
            int bridgeCount = CountBridgesForIsland(island.Id, playerBridges);
            
            if (bridgeCount != island.RequiredBridges)
            {
                result.IncompleteIslandIds.Add(island.Id);
                result.Errors.Add($"L'île à ({island.X}, {island.Y}) nécessite {island.RequiredBridges} ponts mais en a {bridgeCount}");
            }
        }

        // Règle 2: Vérifier qu'il n'y a pas plus de 2 ponts entre deux îles
        var bridgeGroups = playerBridges.GroupBy(b => 
            new { 
                From = Math.Min(b.FromIslandId, b.ToIslandId), 
                To = Math.Max(b.FromIslandId, b.ToIslandId) 
            });

        foreach (var group in bridgeGroups)
        {
            if (group.Count() > 1)
            {
                result.Errors.Add($"Il y a plus de 2 ponts entre les îles {group.Key.From} et {group.Key.To}");
            }
        }

        // Règle 3: Vérifier qu'aucun pont ne se croise
        for (int i = 0; i < playerBridges.Count; i++)
        {
            for (int j = i + 1; j < playerBridges.Count; j++)
            {
                if (DoBridgesIntersect(playerBridges[i], playerBridges[j], puzzle.Islands.ToList()))
                {
                    result.Errors.Add($"Des ponts se croisent illégalement");
                    break;
                }
            }
        }

        // Règle 4: Vérifier que toutes les îles sont connectées
        result.IsFullyConnected = AreAllIslandsConnected(puzzle.Islands.ToList(), playerBridges);
        if (!result.IsFullyConnected)
        {
            result.Errors.Add("Toutes les îles doivent être connectées en un seul réseau");
        }

        // Déterminer si la solution est complète et valide
        result.IsComplete = result.IncompleteIslandIds.Count == 0;
        result.IsValid = result.Errors.Count == 0 && result.IsComplete && result.IsFullyConnected;

        if (result.IsValid)
        {
            result.Message = "Félicitations ! Vous avez résolu le puzzle ! 🎉";
        }

        return result;
    }

    /// <summary>
    /// Compte le nombre de ponts connectés à une île
    /// Un pont double compte pour 2
    /// </summary>
    private int CountBridgesForIsland(int islandId, List<BridgeDto> bridges)
    {
        int count = 0;
        foreach (var bridge in bridges)
        {
            if (bridge.FromIslandId == islandId || bridge.ToIslandId == islandId)
            {
                count += bridge.IsDouble ? 2 : 1;
            }
        }
        return count;
    }

    /// <summary>
    /// Vérifie si deux ponts se croisent
    /// Les ponts ne peuvent se croiser que s'ils sont perpendiculaires
    /// </summary>
    public bool DoBridgesIntersect(BridgeDto bridge1, BridgeDto bridge2, List<Island> islands)
    {
        var island1From = islands.FirstOrDefault(i => i.Id == bridge1.FromIslandId);
        var island1To = islands.FirstOrDefault(i => i.Id == bridge1.ToIslandId);
        var island2From = islands.FirstOrDefault(i => i.Id == bridge2.FromIslandId);
        var island2To = islands.FirstOrDefault(i => i.Id == bridge2.ToIslandId);

        if (island1From == null || island1To == null || island2From == null || island2To == null)
            return false;

        // Déterminer la direction de chaque pont
        bool bridge1IsHorizontal = island1From.Y == island1To.Y;
        bool bridge2IsHorizontal = island2From.Y == island2To.Y;

        // Deux ponts parallèles ne peuvent pas se croiser
        if (bridge1IsHorizontal == bridge2IsHorizontal)
            return false;

        // Si le premier pont est horizontal et le second vertical
        if (bridge1IsHorizontal)
        {
            int bridge1MinX = Math.Min(island1From.X, island1To.X);
            int bridge1MaxX = Math.Max(island1From.X, island1To.X);
            int bridge1Y = island1From.Y;

            int bridge2MinY = Math.Min(island2From.Y, island2To.Y);
            int bridge2MaxY = Math.Max(island2From.Y, island2To.Y);
            int bridge2X = island2From.X;

            // Vérifier si le pont vertical passe à travers le pont horizontal
            return bridge2X > bridge1MinX && bridge2X < bridge1MaxX && 
                   bridge1Y > bridge2MinY && bridge1Y < bridge2MaxY;
        }
        else // Le premier pont est vertical et le second horizontal
        {
            return DoBridgesIntersect(bridge2, bridge1, islands);
        }
    }

    /// <summary>
    /// Vérifie si toutes les îles sont connectées
    /// Utilise un algorithme DFS (Depth-First Search)
    /// </summary>
    public bool AreAllIslandsConnected(List<Island> islands, List<BridgeDto> bridges)
    {
        if (islands.Count == 0)
            return true;

        // Créer un graphe d'adjacence
        var adjacency = new Dictionary<int, List<int>>();
        foreach (var island in islands)
        {
            adjacency[island.Id] = new List<int>();
        }

        // Ajouter les connexions des ponts
        foreach (var bridge in bridges)
        {
            if (!adjacency.ContainsKey(bridge.FromIslandId) || !adjacency.ContainsKey(bridge.ToIslandId))
                continue;

            adjacency[bridge.FromIslandId].Add(bridge.ToIslandId);
            adjacency[bridge.ToIslandId].Add(bridge.FromIslandId);
        }

        // Effectuer un DFS à partir de la première île
        var visited = new HashSet<int>();
        DFS(islands[0].Id, adjacency, visited);

        // Vérifier si toutes les îles ont été visitées
        return visited.Count == islands.Count;
    }

    /// <summary>
    /// Algorithme de parcours en profondeur (DFS - Depth-First Search)
    /// Parcourt récursivement toutes les îles connectées
    /// </summary>
    private void DFS(int islandId, Dictionary<int, List<int>> adjacency, HashSet<int> visited)
    {
        visited.Add(islandId);

        if (!adjacency.ContainsKey(islandId))
            return;

        foreach (var neighborId in adjacency[islandId])
        {
            if (!visited.Contains(neighborId))
            {
                DFS(neighborId, adjacency, visited);
            }
        }
    }
}

