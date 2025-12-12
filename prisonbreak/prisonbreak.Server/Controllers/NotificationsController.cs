using Microsoft.AspNetCore.Mvc;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Services;

namespace prisonbreak.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;
    private readonly ILogger<NotificationsController> _logger;

    public NotificationsController(INotificationService notificationService, ILogger<NotificationsController> logger)
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    /// <summary>
    /// GET api/notifications/users/{userId}
    /// Récupère toutes les notifications d'un utilisateur
    /// </summary>
    [HttpGet("users/{userId}")]
    public async Task<ActionResult<List<NotificationDto>>> GetUserNotifications(int userId, [FromQuery] bool unreadOnly = false)
    {
        try
        {
            var notifications = await _notificationService.GetUserNotificationsAsync(userId, unreadOnly);
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des notifications pour l'utilisateur {UserId}", userId);
            return StatusCode(500, "Erreur serveur");
        }
    }

    /// <summary>
    /// GET api/notifications/{id}
    /// Récupère une notification par son ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<NotificationDto>> GetNotification(int id, [FromQuery] int userId)
    {
        try
        {
            var notification = await _notificationService.GetNotificationByIdAsync(id, userId);
            if (notification == null)
                return NotFound("Notification introuvable");

            return Ok(notification);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de la notification {NotificationId}", id);
            return StatusCode(500, "Erreur serveur");
        }
    }

    /// <summary>
    /// GET api/notifications/users/{userId}/unread-count
    /// Récupère le nombre de notifications non lues
    /// </summary>
    [HttpGet("users/{userId}/unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount(int userId)
    {
        try
        {
            var count = await _notificationService.GetUnreadCountAsync(userId);
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du nombre de notifications non lues pour l'utilisateur {UserId}", userId);
            return StatusCode(500, "Erreur serveur");
        }
    }

    /// <summary>
    /// PUT api/notifications/{id}/read
    /// Marque une notification comme lue
    /// </summary>
    [HttpPut("{id}/read")]
    public async Task<ActionResult> MarkAsRead(int id, [FromBody] MarkNotificationReadRequest request)
    {
        try
        {
            var result = await _notificationService.MarkAsReadAsync(id, request.UserId);
            if (!result)
                return NotFound("Notification introuvable");

            return Ok(new { message = "Notification marquée comme lue" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du marquage de la notification {NotificationId} comme lue", id);
            return StatusCode(500, "Erreur serveur");
        }
    }

    /// <summary>
    /// PUT api/notifications/users/{userId}/read-all
    /// Marque toutes les notifications d'un utilisateur comme lues
    /// </summary>
    [HttpPut("users/{userId}/read-all")]
    public async Task<ActionResult> MarkAllAsRead(int userId)
    {
        try
        {
            var count = await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(new { message = $"{count} notification(s) marquée(s) comme lue(s)" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du marquage de toutes les notifications comme lues pour l'utilisateur {UserId}", userId);
            return StatusCode(500, "Erreur serveur");
        }
    }

    /// <summary>
    /// DELETE api/notifications/{id}
    /// Supprime une notification
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteNotification(int id, [FromQuery] int userId)
    {
        try
        {
            var result = await _notificationService.DeleteNotificationAsync(id, userId);
            if (!result)
                return NotFound("Notification introuvable");

            return Ok(new { message = "Notification supprimée" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression de la notification {NotificationId}", id);
            return StatusCode(500, "Erreur serveur");
        }
    }
}

/// <summary>
/// Requête pour marquer une notification comme lue
/// </summary>
public class MarkNotificationReadRequest
{
    public int UserId { get; set; }
}

