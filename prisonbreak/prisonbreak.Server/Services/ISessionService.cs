using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Services;

/// <summary>
/// Interface du service de gestion des sessions
/// Définit les opérations métier pour la gestion des sessions de jeu
/// </summary>
public interface ISessionService
{
    /// <summary>
    /// Récupère une session par son identifiant
    /// </summary>
    /// <param name="id">Identifiant unique de la session</param>
    /// <returns>Le DTO de la session trouvée, ou null si non trouvée</returns>
    Task<SessionDto?> GetSessionByIdAsync(int id);

    /// <summary>
    /// Récupère une session par son token
    /// </summary>
    /// <param name="sessionToken">Token unique de la session</param>
    /// <returns>Le DTO de la session trouvée, ou null si non trouvée</returns>
    Task<SessionDto?> GetSessionByTokenAsync(string sessionToken);

    /// <summary>
    /// Récupère toutes les sessions d'un utilisateur
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur</param>
    /// <param name="includeActiveOnly">Si true, retourne uniquement les sessions actives</param>
    /// <returns>Liste des DTOs des sessions</returns>
    Task<IEnumerable<SessionDto>> GetSessionsByUserIdAsync(int userId, bool includeActiveOnly = false);

    /// <summary>
    /// Récupère la session active d'un utilisateur
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur</param>
    /// <returns>Le DTO de la session active trouvée, ou null si aucune session active</returns>
    Task<SessionDto?> GetActiveSessionByUserIdAsync(int userId);

    /// <summary>
    /// Crée une nouvelle session pour un utilisateur
    /// </summary>
    /// <param name="request">Requête contenant les informations de la session à créer</param>
    /// <returns>Le DTO de la session créée</returns>
    /// <exception cref="ArgumentException">Si l'utilisateur n'existe pas</exception>
    Task<SessionDto> CreateSessionAsync(CreateSessionRequest request);

    /// <summary>
    /// Met à jour la dernière activité d'une session
    /// </summary>
    /// <param name="sessionToken">Token de la session à mettre à jour</param>
    /// <returns>True si la mise à jour a réussi, False si la session n'existe pas ou est invalide</returns>
    Task<bool> UpdateSessionActivityAsync(string sessionToken);

    /// <summary>
    /// Désactive une session (déconnexion)
    /// </summary>
    /// <param name="sessionToken">Token de la session à désactiver</param>
    /// <returns>True si la session a été désactivée, False si non trouvée</returns>
    Task<bool> DeactivateSessionAsync(string sessionToken);

    /// <summary>
    /// Vérifie si une session est valide
    /// </summary>
    /// <param name="sessionToken">Token de la session à vérifier</param>
    /// <returns>True si la session est valide, sinon False</returns>
    Task<bool> IsValidSessionAsync(string sessionToken);

    /// <summary>
    /// Convertit un modèle Session en SessionDto
    /// </summary>
    /// <param name="session">Le modèle Session à convertir</param>
    /// <returns>Le DTO correspondant</returns>
    SessionDto ConvertToDto(Session session);
}

