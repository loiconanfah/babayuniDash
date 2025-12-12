using System.ComponentModel.DataAnnotations;

namespace prisonbreak.Server.Models
{
    /// <summary>
    /// Représente une demande d'amitié entre deux utilisateurs.
    /// </summary>
    public class FriendRequest
    {
        /// <summary>
        /// Identifiant unique de la demande d'amitié.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur qui envoie la demande.
        /// </summary>
        [Required]
        public int RequesterId { get; set; }

        /// <summary>
        /// Utilisateur qui envoie la demande.
        /// </summary>
        public User Requester { get; set; } = null!;

        /// <summary>
        /// Identifiant de l'utilisateur qui reçoit la demande.
        /// </summary>
        [Required]
        public int ReceiverId { get; set; }

        /// <summary>
        /// Utilisateur qui reçoit la demande.
        /// </summary>
        public User Receiver { get; set; } = null!;

        /// <summary>
        /// Date de création de la demande.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Statut de la demande (Pending, Accepted, Rejected).
        /// </summary>
        public FriendRequestStatus Status { get; set; } = FriendRequestStatus.Pending;

        /// <summary>
        /// Date de réponse (acceptation ou refus).
        /// </summary>
        public DateTime? RespondedAt { get; set; }
    }

    /// <summary>
    /// Statut d'une demande d'amitié.
    /// </summary>
    public enum FriendRequestStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2
    }
}



