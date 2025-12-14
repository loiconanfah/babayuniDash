namespace prisonbreak.Server.DTOs
{
    /// <summary>
    /// DTO pour représenter un commentaire sur un post de la communauté.
    /// </summary>
    public class CommunityPostCommentDto
    {
        public int Id { get; set; }
        public int PostId { get; set; }
        public int AuthorId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}






