using System.ComponentModel.DataAnnotations;

namespace prisonbreak.Server.Models
{
    /// <summary>
    /// Représente un like sur un post de la communauté.
    /// </summary>
    public class CommunityPostLike
    {
        /// <summary>
        /// Identifiant unique du like.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifiant du post liké.
        /// </summary>
        [Required]
        public int PostId { get; set; }

        /// <summary>
        /// Post liké.
        /// </summary>
        public CommunityPost Post { get; set; } = null!;

        /// <summary>
        /// Identifiant de l'utilisateur qui a liké.
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Utilisateur qui a liké.
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// Date du like.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}



