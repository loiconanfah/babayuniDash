using System.ComponentModel.DataAnnotations;

namespace prisonbreak.Server.DTOs;

/// <summary>
/// Requête pour créer une nouvelle session
/// Utilisé dans les endpoints POST pour la création de session
/// </summary>
public class CreateSessionRequest
{
    /// <summary>
    /// Identifiant de l'utilisateur pour lequel créer la session
    /// Requis
    /// </summary>
    [Required(ErrorMessage = "L'identifiant utilisateur est requis")]
    public int UserId { get; set; }

    /// <summary>
    /// Durée de validité de la session en jours
    /// Par défaut : 30 jours
    /// </summary>
    [Range(1, 365, ErrorMessage = "La durée doit être entre 1 et 365 jours")]
    public int DurationDays { get; set; } = 30;

    /// <summary>
    /// Adresse IP de l'utilisateur (optionnel)
    /// Utilisé pour la sécurité et le suivi
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User-Agent du navigateur (optionnel)
    /// Utilisé pour la sécurité et le suivi
    /// </summary>
    public string? UserAgent { get; set; }
}

