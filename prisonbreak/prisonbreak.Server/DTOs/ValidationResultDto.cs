namespace prisonbreak.Server.DTOs;

/// <summary>
/// Résultat de validation d'une solution de puzzle
/// Utilisé pour informer le joueur si sa solution est correcte
/// </summary>
public class ValidationResultDto
{
    /// <summary>
    /// Indique si la solution est valide et complète
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// Indique si le puzzle est terminé (toutes les îles ont le bon nombre de ponts)
    /// </summary>
    public bool IsComplete { get; set; }

    /// <summary>
    /// Liste des erreurs trouvées dans la solution
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Liste des îles qui n'ont pas le bon nombre de ponts
    /// </summary>
    public List<int> IncompleteIslandIds { get; set; } = new();

    /// <summary>
    /// Indique si tous les îles sont connectées (pas de groupes isolés)
    /// </summary>
    public bool IsFullyConnected { get; set; }

    /// <summary>
    /// Message de félicitations si le puzzle est résolu
    /// </summary>
    public string? Message { get; set; }
}

