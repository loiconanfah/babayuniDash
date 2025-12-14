namespace prisonbreak.Server.Models;

/// <summary>
/// Représente une notification pour un utilisateur
/// </summary>
public class Notification
{
    /// <summary>
    /// Identifiant unique de la notification
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID de l'utilisateur destinataire
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Utilisateur destinataire
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Type de notification
    /// </summary>
    public NotificationType Type { get; set; }

    /// <summary>
    /// Titre de la notification
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Message de la notification
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Données supplémentaires en JSON (pour les invitations, etc.)
    /// </summary>
    public string? DataJson { get; set; }

    /// <summary>
    /// Indique si la notification a été lue
    /// </summary>
    public bool IsRead { get; set; }

    /// <summary>
    /// Date de création de la notification
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date de lecture de la notification
    /// </summary>
    public DateTime? ReadAt { get; set; }
}

/// <summary>
/// Types de notifications disponibles
/// </summary>
public enum NotificationType
{
    /// <summary>
    /// Invitation à jouer
    /// </summary>
    GameInvitation = 1,

    /// <summary>
    /// Partie commencée
    /// </summary>
    GameStarted = 2,

    /// <summary>
    /// Partie terminée
    /// </summary>
    GameEnded = 3,

    /// <summary>
    /// Message d'un ami
    /// </summary>
    FriendMessage = 4,

    /// <summary>
    /// Demande d'amitié acceptée
    /// </summary>
    FriendRequestAccepted = 5,

    /// <summary>
    /// Nouveau commentaire sur un post
    /// </summary>
    PostComment = 6,

    /// <summary>
    /// Nouveau like sur un post
    /// </summary>
    PostLike = 7,

    /// <summary>
    /// Tournoi commencé
    /// </summary>
    TournamentStarted = 8,

    /// <summary>
    /// Tournoi terminé
    /// </summary>
    TournamentEnded = 9,

    /// <summary>
    /// Achievement débloqué
    /// </summary>
    AchievementUnlocked = 10
}




