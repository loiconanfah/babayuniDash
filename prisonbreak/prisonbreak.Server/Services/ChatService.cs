using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using prisonbreak.Server.Data;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Hubs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Services
{
    /// <summary>
    /// Service pour gérer le chat entre utilisateurs.
    /// </summary>
    public class ChatService : IChatService
    {
        private readonly HashiDbContext _context;
        private readonly IFriendshipService _friendshipService;
        private readonly IHubContext<ChatHub>? _hubContext;

        public ChatService(
            HashiDbContext context, 
            IFriendshipService friendshipService,
            IHubContext<ChatHub>? hubContext = null)
        {
            _context = context;
            _friendshipService = friendshipService;
            _hubContext = hubContext;
        }

        public async Task<ChatMessageDto> SendMessageAsync(int senderId, int receiverId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Le contenu du message ne peut pas être vide.");

            // Vérifier que les utilisateurs sont amis
            var areFriends = await _friendshipService.AreFriendsAsync(senderId, receiverId);
            if (!areFriends)
                throw new UnauthorizedAccessException("Vous ne pouvez envoyer des messages qu'à vos amis.");

            var sender = await _context.Users.FindAsync(senderId);
            var receiver = await _context.Users.FindAsync(receiverId);

            if (sender == null || receiver == null)
                throw new ArgumentException("Utilisateur introuvable.");

            var message = new ChatMessage
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                SentAt = DateTime.UtcNow,
                IsRead = false
            };

            _context.ChatMessages.Add(message);
            await _context.SaveChangesAsync();

            var messageDto = new ChatMessageDto
            {
                Id = message.Id,
                SenderId = senderId,
                SenderName = sender.Name,
                ReceiverId = receiverId,
                ReceiverName = receiver.Name,
                Content = content,
                SentAt = message.SentAt,
                IsRead = false
            };

            // Envoyer le message via SignalR en temps réel
            if (_hubContext != null)
            {
                // Envoyer au destinataire s'il est connecté
                await _hubContext.Clients.Group($"user_{receiverId}").SendAsync("ReceiveMessage", messageDto);
                // Envoyer aussi à l'expéditeur pour confirmation
                await _hubContext.Clients.Group($"user_{senderId}").SendAsync("MessageSent", messageDto);
            }

            return messageDto;
        }

        public async Task<List<ChatMessageDto>> GetConversationAsync(int userId1, int userId2)
        {
            var messages = await _context.ChatMessages
                .Include(cm => cm.Sender)
                .Include(cm => cm.Receiver)
                .Where(cm => (cm.SenderId == userId1 && cm.ReceiverId == userId2) ||
                            (cm.SenderId == userId2 && cm.ReceiverId == userId1))
                .OrderBy(cm => cm.SentAt)
                .ToListAsync();

            return messages.Select(m => new ChatMessageDto
            {
                Id = m.Id,
                SenderId = m.SenderId,
                SenderName = m.Sender.Name,
                ReceiverId = m.ReceiverId,
                ReceiverName = m.Receiver.Name,
                Content = m.Content,
                SentAt = m.SentAt,
                IsRead = m.IsRead,
                ReadAt = m.ReadAt
            }).ToList();
        }

        public async Task<List<ChatConversationDto>> GetConversationsAsync(int userId)
        {
            var conversations = await _context.ChatMessages
                .Include(cm => cm.Sender)
                .Include(cm => cm.Receiver)
                .Where(cm => cm.SenderId == userId || cm.ReceiverId == userId)
                .GroupBy(cm => cm.SenderId == userId ? cm.ReceiverId : cm.SenderId)
                .Select(g => new
                {
                    OtherUserId = g.Key,
                    LastMessage = g.OrderByDescending(m => m.SentAt).First(),
                    UnreadCount = g.Count(m => m.ReceiverId == userId && !m.IsRead)
                })
                .ToListAsync();

            var otherUserIds = conversations.Select(c => c.OtherUserId).ToList();
            var otherUsers = await _context.Users
                .Where(u => otherUserIds.Contains(u.Id))
                .ToDictionaryAsync(u => u.Id);

            var activeSessions = await _context.Sessions
                .Where(s => otherUserIds.Contains(s.UserId) && s.IsActive && s.ExpiresAt > DateTime.UtcNow)
                .Select(s => s.UserId)
                .Distinct()
                .ToListAsync();

            return conversations.Select(c =>
            {
                var otherUser = otherUsers[c.OtherUserId];
                return new ChatConversationDto
                {
                    OtherUserId = c.OtherUserId,
                    OtherUserName = otherUser.Name,
                    LastMessage = c.LastMessage.Content,
                    LastMessageAt = c.LastMessage.SentAt,
                    UnreadCount = c.UnreadCount,
                    IsOnline = activeSessions.Contains(c.OtherUserId)
                };
            }).OrderByDescending(c => c.LastMessageAt).ToList();
        }

        public async Task<bool> MarkAsReadAsync(int messageId, int userId)
        {
            var message = await _context.ChatMessages.FindAsync(messageId);
            if (message == null || message.ReceiverId != userId)
                return false;

            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Notifier via SignalR que le message a été lu
            if (_hubContext != null)
            {
                await _hubContext.Clients.All.SendAsync("MessageRead", messageId, userId);
            }

            return true;
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            return await _context.ChatMessages
                .CountAsync(cm => cm.ReceiverId == userId && !cm.IsRead);
        }
    }
}



