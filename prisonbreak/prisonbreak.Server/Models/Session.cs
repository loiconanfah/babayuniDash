namespace prisonbreak.Server.Models;

/// <summary>
/// Représente une session de jeu Hashi
/// Une session est liée à un utilisateur et peut contenir plusieurs parties
/// Chaque compte utilisateur correspond à une session active
/// </summary>
public class Session
{
    /// <summary>
    /// Identifiant unique de la session
    /// Clé primaire auto-générée
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant de l'utilisateur propriétaire de la session
    /// Clé étrangère vers la table Users
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Navigation vers l'utilisateur propriétaire
    /// Relation plusieurs-à-un : plusieurs sessions peuvent appartenir à un utilisateur
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Token unique de la session
    /// Généré automatiquement à la création de la session
    /// Utilisé pour l'authentification et l'identification de la session
    /// </summary>
    public string SessionToken { get; set; } = string.Empty;

    /// <summary>
    /// Date et heure de création de la session
    /// Initialisé automatiquement à la création
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date et heure d'expiration de la session
    /// La session devient inactive après cette date
    /// Par défaut, expire après 30 jours
    /// </summary>
    public DateTime ExpiresAt { get; set; }

    /// <summary>
    /// Date et heure de la dernière activité sur la session
    /// Mis à jour à chaque interaction avec la session
    /// </summary>
    public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indique si la session est actuellement active
    /// Une session inactive ne peut plus être utilisée
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Adresse IP de l'utilisateur lors de la création de la session
    /// Utilisé pour la sécurité et le suivi
    /// </summary>
    public string? IpAddress { get; set; }

    /// <summary>
    /// User-Agent du navigateur lors de la création de la session
    /// Utilisé pour la sécurité et le suivi
    /// </summary>
    public string? UserAgent { get; set; }

    /// <summary>
    /// Collection des parties de jeu associées à cette session
    /// Relation un-à-plusieurs : une session peut contenir plusieurs parties
    /// </summary>
    public ICollection<Game> Games { get; set; } = new List<Game>();

    /// <summary>
    /// Vérifie si la session est expirée
    /// </summary>
    /// <returns>True si la session est expirée, sinon False</returns>
    public bool IsExpired()
    {
        return DateTime.UtcNow > ExpiresAt;
    }

    /// <summary>
    /// Vérifie si la session est valide (active et non expirée)
    /// </summary>
    /// <returns>True si la session est valide, sinon False</returns>
    public bool IsValid()
    {
        return IsActive && !IsExpired();
    }

    /// <summary>
    /// Met à jour la date de dernière activité
    /// Appelé à chaque interaction avec la session
    /// </summary>
    public void UpdateActivity()
    {
        LastActivityAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Désactive la session
    /// Utilisé lors de la déconnexion ou de l'invalidation de la session
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Génère un nouveau token de session unique
    /// Utilisé lors de la création ou du renouvellement de session
    /// </summary>
    /// <returns>Un token de session unique</returns>
    public static string GenerateSessionToken()
    {
        return Guid.NewGuid().ToString("N") + "-" + DateTime.UtcNow.Ticks.ToString("X");
    }
}

