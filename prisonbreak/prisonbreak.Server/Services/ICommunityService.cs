using prisonbreak.Server.DTOs;

namespace prisonbreak.Server.Services
{
    /// <summary>
    /// Interface pour le service de communauté.
    /// </summary>
    public interface ICommunityService
    {
        Task<CommunityPostDto> CreatePostAsync(int authorId, string title, string content, string? imageUrl, CommunityPostTypeDto postType);
        Task<List<CommunityPostDto>> GetPostsAsync(int? limit = null, CommunityPostTypeDto? postType = null);
        Task<CommunityPostDto?> GetPostByIdAsync(int postId);
        Task<bool> LikePostAsync(int postId, int userId);
        Task<bool> UnlikePostAsync(int postId, int userId);
        Task<CommunityPostCommentDto> AddCommentAsync(int postId, int authorId, string content);
        Task<List<CommunityPostCommentDto>> GetCommentsAsync(int postId);
        Task<bool> DeletePostAsync(int postId, int userId);
    }
}

