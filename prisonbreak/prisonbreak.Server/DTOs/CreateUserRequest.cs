using System.ComponentModel.DataAnnotations;

namespace prisonbreak.Server.DTOs
{
    /// <summary>
    /// Données envoyées par le frontend pour créer/identifier un joueur.
    /// </summary>
    public class CreateUserRequest
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}
