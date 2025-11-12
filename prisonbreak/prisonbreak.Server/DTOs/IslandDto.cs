namespace prisonbreak.Server.DTOs;

/// <summary>
/// DTO (Data Transfer Object) pour une île
/// Utilisé pour la communication entre le frontend et le backend
/// Version simplifiée sans références circulaires
/// </summary>
public class IslandDto
{
    /// <summary>
    /// Identifiant de l'île
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Position X (colonne)
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Position Y (ligne)
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// Nombre de ponts requis
    /// </summary>
    public int RequiredBridges { get; set; }

    /// <summary>
    /// Nombre actuel de ponts connectés (calculé côté serveur)
    /// </summary>
    public int CurrentBridges { get; set; }
}

