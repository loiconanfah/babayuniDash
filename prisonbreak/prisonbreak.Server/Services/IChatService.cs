using prisonbreak.Server.DTOs;

namespace prisonbreak.Server.Services
{
    /// <summary>
    /// Interface pour le service de chat.
    /// </summary>
    public interface IChatService
    {
        Task<ChatMessageDto> SendMessageAsync(int senderId, int receiverId, string content);
        Task<List<ChatMessageDto>> GetConversationAsync(int userId1, int userId2);
        Task<List<ChatConversationDto>> GetConversationsAsync(int userId);
        Task<bool> MarkAsReadAsync(int messageId, int userId);
        Task<int> GetUnreadCountAsync(int userId);
    }
}




