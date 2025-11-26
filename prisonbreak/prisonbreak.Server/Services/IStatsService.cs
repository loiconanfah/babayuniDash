using prisonbreak.Server.DTOs;

namespace prisonbreak.Server.Services;

/// <summary>
/// Interface pour le service de statistiques et de classement
/// </summary>
public interface IStatsService
{
    /// <summary>
    /// Récupère les statistiques d'un utilisateur
    /// </summary>
    /// <param name="userId">ID de l'utilisateur</param>
    /// <returns>Statistiques de l'utilisateur</returns>
    Task<UserStatsDto> GetUserStatsAsync(int userId);

    /// <summary>
    /// Récupère les statistiques d'un utilisateur par son email
    /// </summary>
    /// <param name="email">Email de l'utilisateur</param>
    /// <returns>Statistiques de l'utilisateur</returns>
    Task<UserStatsDto?> GetUserStatsByEmailAsync(string email);

    /// <summary>
    /// Récupère le classement (leaderboard) des meilleurs joueurs
    /// </summary>
    /// <param name="limit">Nombre maximum de joueurs à retourner (défaut: 10)</param>
    /// <returns>Liste des meilleurs joueurs</returns>
    Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(int limit = 10);
}

