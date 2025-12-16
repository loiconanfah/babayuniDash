using prisonbreak.Server.DTOs;

namespace prisonbreak.Server.Services
{
    /// <summary>
    /// Interface pour le service de gestion des amitiés.
    /// </summary>
    public interface IFriendshipService
    {
        Task<FriendRequestDto> SendFriendRequestAsync(int requesterId, int receiverId);
        Task<bool> AcceptFriendRequestAsync(int requestId, int userId);
        Task<bool> RejectFriendRequestAsync(int requestId, int userId);
        Task<List<FriendDto>> GetFriendsAsync(int userId);
        Task<List<FriendRequestDto>> GetPendingRequestsAsync(int userId);
        Task<List<FriendRequestDto>> GetSentRequestsAsync(int userId);
        Task<bool> RemoveFriendAsync(int userId, int friendId);
        Task<bool> AreFriendsAsync(int userId1, int userId2);
    }
}

