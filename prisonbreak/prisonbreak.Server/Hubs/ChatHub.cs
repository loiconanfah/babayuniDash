using Microsoft.AspNetCore.SignalR;
using prisonbreak.Server.DTOs;
using System.Collections.Concurrent;

namespace prisonbreak.Server.Hubs;

/// <summary>
/// Hub SignalR pour le chat en temps réel
/// </summary>
public class ChatHub : Hub
{
    // Dictionnaire pour stocker les connexions utilisateur (userId -> connectionId)
    private static readonly ConcurrentDictionary<int, string> UserConnections = new();

    /// <summary>
    /// Appelé quand un utilisateur se connecte
    /// </summary>
    public async Task JoinChat(int userId)
    {
        // Associer l'userId à la connectionId
        UserConnections[userId] = Context.ConnectionId;

        // Ajouter l'utilisateur à un groupe pour les notifications
        await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");

        // Notifier les autres utilisateurs que cet utilisateur est en ligne
        await Clients.Others.SendAsync("UserOnline", userId);
    }

    /// <summary>
    /// Appelé quand un utilisateur se déconnecte
    /// </summary>
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Trouver l'userId associé à cette connexion
        var userId = UserConnections.FirstOrDefault(x => x.Value == Context.ConnectionId).Key;
        
        if (userId != 0)
        {
            UserConnections.TryRemove(userId, out _);
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            
            // Notifier les autres utilisateurs que cet utilisateur est hors ligne
            await Clients.Others.SendAsync("UserOffline", userId);
        }

        await base.OnDisconnectedAsync(exception);
    }

    /// <summary>
    /// Envoie un message à un utilisateur spécifique
    /// </summary>
    public async Task SendMessageToUser(int senderId, int receiverId, string content, object messageDto)
    {
        // Envoyer le message au destinataire s'il est connecté
        if (UserConnections.TryGetValue(receiverId, out var receiverConnectionId))
        {
            await Clients.Client(receiverConnectionId).SendAsync("ReceiveMessage", messageDto);
        }

        // Envoyer aussi au sender pour confirmation
        if (UserConnections.TryGetValue(senderId, out var senderConnectionId))
        {
            await Clients.Client(senderConnectionId).SendAsync("MessageSent", messageDto);
        }
    }

    /// <summary>
    /// Marque un message comme lu
    /// </summary>
    public async Task MarkMessageAsRead(int messageId, int userId, int senderId)
    {
        // Notifier l'expéditeur que son message a été lu
        if (UserConnections.TryGetValue(senderId, out var senderConnectionId))
        {
            await Clients.Client(senderConnectionId).SendAsync("MessageRead", messageId, userId);
        }
    }

    /// <summary>
    /// Vérifie si un utilisateur est en ligne
    /// </summary>
    public bool IsUserOnline(int userId)
    {
        return UserConnections.ContainsKey(userId);
    }

    /// <summary>
    /// Récupère tous les utilisateurs en ligne
    /// </summary>
    public List<int> GetOnlineUsers()
    {
        return UserConnections.Keys.ToList();
    }
}

