using System.ComponentModel.DataAnnotations;

namespace prisonbreak.Server.Models
{
    /// <summary>
    /// Représente une relation d'amitié entre deux utilisateurs.
    /// </summary>
    public class Friendship
    {
        /// <summary>
        /// Identifiant unique de l'amitié.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur qui a envoyé la demande d'amitié.
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// Utilisateur qui a envoyé la demande d'amitié.
        /// </summary>
        public User User { get; set; } = null!;

        /// <summary>
        /// Identifiant de l'utilisateur qui a accepté la demande d'amitié.
        /// </summary>
        [Required]
        public int FriendId { get; set; }

        /// <summary>
        /// L'ami (utilisateur qui a accepté).
        /// </summary>
        public User Friend { get; set; } = null!;

        /// <summary>
        /// Date de création de l'amitié (quand la demande a été acceptée).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Statut de l'amitié (Active, Blocked, etc.).
        /// </summary>
        public FriendshipStatus Status { get; set; } = FriendshipStatus.Active;
    }

    /// <summary>
    /// Statut d'une amitié.
    /// </summary>
    public enum FriendshipStatus
    {
        Active = 0,
        Blocked = 1
    }
}




