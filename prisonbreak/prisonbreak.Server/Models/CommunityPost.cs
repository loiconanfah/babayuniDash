using System.ComponentModel.DataAnnotations;

namespace prisonbreak.Server.Models
{
    /// <summary>
    /// Représente un post dans la communauté.
    /// </summary>
    public class CommunityPost
    {
        /// <summary>
        /// Identifiant unique du post.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur qui a créé le post.
        /// </summary>
        [Required]
        public int AuthorId { get; set; }

        /// <summary>
        /// Auteur du post.
        /// </summary>
        public User Author { get; set; } = null!;

        /// <summary>
        /// Titre du post.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Contenu du post.
        /// </summary>
        [Required]
        [MaxLength(5000)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// URL de l'image associée (optionnel).
        /// </summary>
        [MaxLength(500)]
        public string? ImageUrl { get; set; }

        /// <summary>
        /// Type de post (Discussion, Achievement, Question, etc.).
        /// </summary>
        public CommunityPostType PostType { get; set; } = CommunityPostType.Discussion;

        /// <summary>
        /// Date de création du post.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date de dernière modification.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Nombre de likes.
        /// </summary>
        public int LikesCount { get; set; } = 0;

        /// <summary>
        /// Nombre de commentaires.
        /// </summary>
        public int CommentsCount { get; set; } = 0;

        /// <summary>
        /// Le post est-il actif (non supprimé) ?
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Collection des likes sur ce post.
        /// </summary>
        public ICollection<CommunityPostLike> Likes { get; set; } = new List<CommunityPostLike>();

        /// <summary>
        /// Collection des commentaires sur ce post.
        /// </summary>
        public ICollection<CommunityPostComment> Comments { get; set; } = new List<CommunityPostComment>();
    }

    /// <summary>
    /// Type de post dans la communauté.
    /// </summary>
    public enum CommunityPostType
    {
        Discussion = 0,
        Achievement = 1,
        Question = 2,
        Tip = 3,
        Share = 4
    }
}



