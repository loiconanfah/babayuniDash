using System.ComponentModel.DataAnnotations;

namespace prisonbreak.Server.Models
{
    /// <summary>
    /// Représente un commentaire sur un post de la communauté.
    /// </summary>
    public class CommunityPostComment
    {
        /// <summary>
        /// Identifiant unique du commentaire.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifiant du post commenté.
        /// </summary>
        [Required]
        public int PostId { get; set; }

        /// <summary>
        /// Post commenté.
        /// </summary>
        public CommunityPost Post { get; set; } = null!;

        /// <summary>
        /// Identifiant de l'utilisateur qui a commenté.
        /// </summary>
        [Required]
        public int AuthorId { get; set; }

        /// <summary>
        /// Auteur du commentaire.
        /// </summary>
        public User Author { get; set; } = null!;

        /// <summary>
        /// Contenu du commentaire.
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Date de création du commentaire.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date de dernière modification.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Le commentaire est-il actif (non supprimé) ?
        /// </summary>
        public bool IsActive { get; set; } = true;
    }
}




