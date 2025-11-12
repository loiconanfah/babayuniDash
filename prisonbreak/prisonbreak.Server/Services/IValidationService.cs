using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Services;

/// <summary>
/// Interface pour le service de validation des solutions Hashi
/// Définit les méthodes pour vérifier si une solution est correcte
/// </summary>
public interface IValidationService
{
    /// <summary>
    /// Valide une solution complète d'un puzzle
    /// </summary>
    /// <param name="puzzle">Le puzzle à valider</param>
    /// <param name="playerBridges">Les ponts placés par le joueur</param>
    /// <returns>Résultat de la validation avec détails des erreurs</returns>
    ValidationResultDto ValidateSolution(Puzzle puzzle, List<BridgeDto> playerBridges);

    /// <summary>
    /// Vérifie si deux ponts se croisent
    /// </summary>
    /// <param name="bridge1">Premier pont</param>
    /// <param name="bridge2">Deuxième pont</param>
    /// <param name="islands">Liste de toutes les îles du puzzle</param>
    /// <returns>True si les ponts se croisent</returns>
    bool DoBridgesIntersect(BridgeDto bridge1, BridgeDto bridge2, List<Island> islands);

    /// <summary>
    /// Vérifie si toutes les îles sont connectées (pas de groupes isolés)
    /// Utilise un algorithme de parcours en profondeur (DFS)
    /// </summary>
    /// <param name="islands">Liste des îles</param>
    /// <param name="bridges">Liste des ponts</param>
    /// <returns>True si toutes les îles sont connectées</returns>
    bool AreAllIslandsConnected(List<Island> islands, List<BridgeDto> bridges);
}

