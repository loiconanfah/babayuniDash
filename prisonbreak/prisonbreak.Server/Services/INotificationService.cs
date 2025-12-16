using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Services;

/// <summary>
/// Interface pour le service de gestion des notifications
/// </summary>
public interface INotificationService
{
    /// <summary>
    /// Récupère toutes les notifications d'un utilisateur
    /// </summary>
    Task<List<NotificationDto>> GetUserNotificationsAsync(int userId, bool unreadOnly = false);

    /// <summary>
    /// Récupère une notification par son ID
    /// </summary>
    Task<NotificationDto?> GetNotificationByIdAsync(int notificationId, int userId);

    /// <summary>
    /// Crée une nouvelle notification
    /// </summary>
    Task<NotificationDto> CreateNotificationAsync(int userId, NotificationType type, string title, string message, string? dataJson = null);

    /// <summary>
    /// Marque une notification comme lue
    /// </summary>
    Task<bool> MarkAsReadAsync(int notificationId, int userId);

    /// <summary>
    /// Marque toutes les notifications d'un utilisateur comme lues
    /// </summary>
    Task<int> MarkAllAsReadAsync(int userId);

    /// <summary>
    /// Supprime une notification
    /// </summary>
    Task<bool> DeleteNotificationAsync(int notificationId, int userId);

    /// <summary>
    /// Récupère le nombre de notifications non lues
    /// </summary>
    Task<int> GetUnreadCountAsync(int userId);
}




