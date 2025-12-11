using prisonbreak.Server.Models;

namespace prisonbreak.Server.Repositories;

/// <summary>
/// Interface du repository pour la gestion des sessions
/// Définit les opérations CRUD et de recherche sur les sessions de jeu
/// </summary>
public interface ISessionRepository
{
    /// <summary>
    /// Récupère une session par son identifiant
    /// </summary>
    /// <param name="id">Identifiant unique de la session</param>
    /// <param name="includeUser">Si true, inclut les informations de l'utilisateur</param>
    /// <param name="includeGames">Si true, inclut les parties associées</param>
    /// <returns>La session trouvée, ou null si aucune session n'existe avec cet ID</returns>
    Task<Session?> GetByIdAsync(int id, bool includeUser = false, bool includeGames = false);

    /// <summary>
    /// Récupère une session par son token
    /// </summary>
    /// <param name="sessionToken">Token unique de la session</param>
    /// <param name="includeUser">Si true, inclut les informations de l'utilisateur</param>
    /// <param name="includeGames">Si true, inclut les parties associées</param>
    /// <returns>La session trouvée, ou null si aucune session n'existe avec ce token</returns>
    Task<Session?> GetByTokenAsync(string sessionToken, bool includeUser = false, bool includeGames = false);

    /// <summary>
    /// Récupère toutes les sessions d'un utilisateur
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur</param>
    /// <param name="includeActiveOnly">Si true, retourne uniquement les sessions actives</param>
    /// <returns>Liste des sessions de l'utilisateur</returns>
    Task<IEnumerable<Session>> GetByUserIdAsync(int userId, bool includeActiveOnly = false);

    /// <summary>
    /// Récupère la session active d'un utilisateur
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur</param>
    /// <returns>La session active trouvée, ou null si aucune session active</returns>
    Task<Session?> GetActiveSessionByUserIdAsync(int userId);

    /// <summary>
    /// Vérifie si une session est valide (active et non expirée)
    /// </summary>
    /// <param name="sessionToken">Token de la session à vérifier</param>
    /// <returns>True si la session est valide, sinon False</returns>
    Task<bool> IsValidSessionAsync(string sessionToken);

    /// <summary>
    /// Crée une nouvelle session dans la base de données
    /// </summary>
    /// <param name="session">La session à créer</param>
    /// <returns>La session créée avec son ID généré</returns>
    Task<Session> CreateAsync(Session session);

    /// <summary>
    /// Met à jour une session existante dans la base de données
    /// </summary>
    /// <param name="session">La session à mettre à jour</param>
    /// <returns>La session mise à jour</returns>
    Task<Session> UpdateAsync(Session session);

    /// <summary>
    /// Désactive toutes les sessions expirées
    /// </summary>
    /// <returns>Le nombre de sessions désactivées</returns>
    Task<int> DeactivateExpiredSessionsAsync();

    /// <summary>
    /// Supprime une session de la base de données
    /// </summary>
    /// <param name="id">Identifiant de la session à supprimer</param>
    /// <returns>True si la session a été supprimée, False si la session n'existait pas</returns>
    Task<bool> DeleteAsync(int id);

    /// <summary>
    /// Récupère toutes les sessions actives (utilisateurs en ligne)
    /// </summary>
    /// <param name="excludeSessionId">ID de session à exclure (pour ne pas s'inclure soi-même)</param>
    /// <returns>Liste des sessions actives</returns>
    Task<IEnumerable<Session>> GetActiveSessionsAsync(int? excludeSessionId = null);

    /// <summary>
    /// Sauvegarde les changements dans la base de données
    /// </summary>
    /// <returns>Le nombre d'entités modifiées</returns>
    Task<int> SaveChangesAsync();
}

