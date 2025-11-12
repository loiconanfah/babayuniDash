namespace prisonbreak.Server.DTOs;

/// <summary>
/// DTO pour un pont entre deux îles
/// </summary>
public class BridgeDto
{
    /// <summary>
    /// Identifiant de l'île de départ
    /// </summary>
    public int FromIslandId { get; set; }

    /// <summary>
    /// Identifiant de l'île d'arrivée
    /// </summary>
    public int ToIslandId { get; set; }

    /// <summary>
    /// Indique si le pont est double
    /// </summary>
    public bool IsDouble { get; set; }
}

