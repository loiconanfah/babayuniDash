namespace prisonbreak.Server.DTOs;

/// <summary>
/// Objet de transfert de données pour une session
/// Utilisé pour exposer les informations de session via l'API REST
/// </summary>
public class SessionDto
{
    /// <summary>
    /// Identifiant unique de la session
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant de l'utilisateur propriétaire de la session
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Informations de l'utilisateur (optionnel, peut être null si non chargé)
    /// </summary>
    public UserDto? User { get; set; }

    /// <summary>
    /// Token unique de la session
    /// Utilisé pour l'authentification et l'identification
    /// </summary>
    public string SessionToken { get; set; } = string.Empty;

    /// <summary>
    /// Date et heure de création de la session
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date et heure d'expiration de la session
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Date et heure de la dernière activité sur la session
    /// </summary>
    public DateTime LastActivityAt { get; set; }

    /// <summary>
    /// Indique si la session est actuellement active
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Indique si la session est expirée
    /// </summary>
    public bool IsExpired { get; set; }

    /// <summary>
    /// Nombre de parties associées à cette session
    /// </summary>
    public int GameCount { get; set; }
}

