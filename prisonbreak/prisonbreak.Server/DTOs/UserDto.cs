namespace prisonbreak.Server.DTOs
{
    /// <summary>
    /// Données renvoyées au frontend pour représenter un joueur.
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Date de création du compte.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Dernière connexion du joueur.
        /// </summary>
        public DateTime? LastLoginAt { get; set; }

        /// <summary>
        /// Le compte est-il actif ?
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Nombre de sessions actives pour ce joueur.
        /// Utilisé par certains services (ex: SessionService) pour les statistiques.
        /// </summary>
        public int ActiveSessionCount { get; set; }
    }
}
