using System.ComponentModel.DataAnnotations;

namespace prisonbreak.Server.Models
{
    /// <summary>
    /// Représente un message de chat entre deux utilisateurs.
    /// </summary>
    public class ChatMessage
    {
        /// <summary>
        /// Identifiant unique du message.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identifiant de l'utilisateur qui envoie le message.
        /// </summary>
        [Required]
        public int SenderId { get; set; }

        /// <summary>
        /// Utilisateur qui envoie le message.
        /// </summary>
        public User Sender { get; set; } = null!;

        /// <summary>
        /// Identifiant de l'utilisateur qui reçoit le message.
        /// </summary>
        [Required]
        public int ReceiverId { get; set; }

        /// <summary>
        /// Utilisateur qui reçoit le message.
        /// </summary>
        public User Receiver { get; set; } = null!;

        /// <summary>
        /// Contenu du message.
        /// </summary>
        [Required]
        [MaxLength(1000)]
        public string Content { get; set; } = string.Empty;

        /// <summary>
        /// Date d'envoi du message.
        /// </summary>
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Le message a-t-il été lu ?
        /// </summary>
        public bool IsRead { get; set; } = false;

        /// <summary>
        /// Date de lecture du message.
        /// </summary>
        public DateTime? ReadAt { get; set; }
    }
}

