using prisonbreak.Server.Models;

namespace prisonbreak.Server.Repositories
{
    /// <summary>
    /// Interface d'accès aux données pour les joueurs.
    /// </summary>
    public interface IUserRepository
    {
        /// <summary>
        /// Retourne un utilisateur par son email, ou null s'il n'existe pas.
        /// </summary>
        Task<User?> GetByEmailAsync(string email);

        /// <summary>
        /// Retourne un utilisateur par son identifiant unique.
        /// </summary>
        Task<User?> GetByIdAsync(int id);

        /// <summary>
        /// Ajoute un nouvel utilisateur et sauvegarde immédiatement.
        /// </summary>
        Task<User> AddAsync(User user);

        /// <summary>
        /// Met à jour un utilisateur existant et sauvegarde immédiatement.
        /// </summary>
        Task UpdateAsync(User user);

        /// <summary>
        /// Sauvegarde les modifications en attente.
        /// </summary>
        Task SaveChangesAsync();

        /// <summary>
        /// Recherche des utilisateurs par email (pour autocomplétion).
        /// </summary>
        Task<List<User>> SearchByEmailAsync(string emailQuery, int limit = 10);
    }
}
