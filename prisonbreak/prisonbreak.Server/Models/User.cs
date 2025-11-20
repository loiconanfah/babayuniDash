using System.ComponentModel.DataAnnotations;

namespace prisonbreak.Server.Models
{
    /// <summary>
    /// Représente un joueur de Prison Break.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Identifiant unique du joueur (clé primaire).
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nom affiché dans l’interface (ex : "Kevin").
        /// Obligatoire, max 50 caractères.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Adresse courriel, utilisée pour identifier le joueur.
        /// Obligatoire, max 255 caractères.
        /// </summary>
        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Date de création du compte.
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Dernière connexion du joueur.
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Le compte est-il actif ?
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Met à jour la date de dernière connexion du joueur.
        /// Cette méthode est utilisée par SessionService (user.UpdateLastLogin()).
        /// </summary>
        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            // On peut aussi forcer le compte comme actif au passage.
            IsActive = true;
        }
    }
}
