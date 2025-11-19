using prisonbreak.Server.Models;

namespace prisonbreak.Server.Repositories;

/// <summary>
/// Interface du repository pour la gestion des utilisateurs
/// Définit les opérations CRUD et de recherche sur les utilisateurs
/// Pattern Repository : sépare la logique d'accès aux données de la logique métier
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Récupère un utilisateur par son identifiant
    /// </summary>
    /// <param name="id">Identifiant unique de l'utilisateur</param>
    /// <returns>L'utilisateur trouvé, ou null si aucun utilisateur n'existe avec cet ID</returns>
    Task<User?> GetByIdAsync(int id);

    /// <summary>
    /// Récupère un utilisateur par son adresse email
    /// </summary>
    /// <param name="email">Adresse email de l'utilisateur</param>
    /// <returns>L'utilisateur trouvé, ou null si aucun utilisateur n'existe avec cet email</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Récupère tous les utilisateurs
    /// </summary>
    /// <param name="includeInactive">Si true, inclut les utilisateurs inactifs</param>
    /// <returns>Liste de tous les utilisateurs</returns>
    Task<IEnumerable<User>> GetAllAsync(bool includeInactive = false);

    /// <summary>
    /// Vérifie si un utilisateur existe avec l'adresse email donnée
    /// </summary>
    /// <param name="email">Adresse email à vérifier</param>
    /// <returns>True si un utilisateur existe avec cet email, sinon False</returns>
    Task<bool> ExistsByEmailAsync(string email);

    /// <summary>
    /// Crée un nouvel utilisateur dans la base de données
    /// </summary>
    /// <param name="user">L'utilisateur à créer</param>
    /// <returns>L'utilisateur créé avec son ID généré</returns>
    Task<User> CreateAsync(User user);

    /// <summary>
    /// Met à jour un utilisateur existant dans la base de données
    /// </summary>
    /// <param name="user">L'utilisateur à mettre à jour</param>
    /// <returns>L'utilisateur mis à jour</returns>
    Task<User> UpdateAsync(User user);

    /// <summary>
    /// Supprime un utilisateur de la base de données
    /// </summary>
    /// <param name="id">Identifiant de l'utilisateur à supprimer</param>
    /// <returns>True si l'utilisateur a été supprimé, False si l'utilisateur n'existait pas</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Sauvegarde les changements dans la base de données
    /// </summary>
    /// <returns>Le nombre d'entités modifiées</returns>
    Task<int> SaveChangesAsync();
}

