namespace prisonbreak.Server.DTOs;

/// <summary>
/// Objet de transfert de données pour un utilisateur
/// Utilisé pour exposer les informations utilisateur via l'API REST
/// Ne contient pas d'informations sensibles ni de relations complexes
/// </summary>
public class UserDto
{
    /// <summary>
    /// Identifiant unique de l'utilisateur
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nom complet ou pseudonyme de l'utilisateur
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Adresse email de l'utilisateur
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Date et heure de création du compte
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date et heure de la dernière connexion
    /// Null si l'utilisateur ne s'est jamais connecté
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Indique si le compte utilisateur est actif
    /// </summary>
    public bool IsActive { get; set; }

    /// <summary>
    /// Nombre de sessions actives de l'utilisateur
    /// </summary>
    public int ActiveSessionCount { get; set; }
}

