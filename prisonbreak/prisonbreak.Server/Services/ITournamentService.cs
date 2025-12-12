using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Services;

/// <summary>
/// Interface pour le service de gestion des tournois
/// </summary>
public interface ITournamentService
{
    /// <summary>
    /// Récupère tous les tournois disponibles
    /// </summary>
    Task<List<TournamentDto>> GetAllTournamentsAsync(int? userId = null);

    /// <summary>
    /// Récupère un tournoi par son ID
    /// </summary>
    Task<TournamentDto?> GetTournamentByIdAsync(int tournamentId, int? userId = null);

    /// <summary>
    /// Crée un nouveau tournoi
    /// </summary>
    Task<TournamentDto> CreateTournamentAsync(CreateTournamentRequest request);

    /// <summary>
    /// Inscrit un utilisateur à un tournoi
    /// </summary>
    Task<TournamentParticipantDto> RegisterUserAsync(int tournamentId, int userId);

    /// <summary>
    /// Désinscrit un utilisateur d'un tournoi
    /// </summary>
    Task<bool> UnregisterUserAsync(int tournamentId, int userId);

    /// <summary>
    /// Démarre un tournoi (génère les matchs)
    /// </summary>
    Task<TournamentDto> StartTournamentAsync(int tournamentId);

    /// <summary>
    /// Enregistre le résultat d'un match
    /// </summary>
    Task<TournamentMatchDto> RecordMatchResultAsync(int matchId, int winnerId);

    /// <summary>
    /// Récupère les matchs d'un tournoi
    /// </summary>
    Task<List<TournamentMatchDto>> GetTournamentMatchesAsync(int tournamentId);

    /// <summary>
    /// Récupère les matchs d'un utilisateur dans un tournoi
    /// </summary>
    Task<List<TournamentMatchDto>> GetUserMatchesAsync(int tournamentId, int userId);
}

