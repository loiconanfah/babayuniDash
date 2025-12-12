namespace prisonbreak.Server.DTOs
{
    /// <summary>
    /// DTO pour représenter un post de la communauté.
    /// </summary>
    public class CommunityPostDto
    {
        public int Id { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public CommunityPostTypeDto PostType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int LikesCount { get; set; }
        public int CommentsCount { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
    }

    /// <summary>
    /// Type de post dans la communauté.
    /// </summary>
    public enum CommunityPostTypeDto
    {
        Discussion = 0,
        Achievement = 1,
        Question = 2,
        Tip = 3,
        Share = 4
    }
}

