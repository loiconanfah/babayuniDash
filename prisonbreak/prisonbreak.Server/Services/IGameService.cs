using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Services;

/// <summary>
/// Interface pour le service de gestion des parties
/// Gère le cycle de vie d'une partie de jeu
/// </summary>
public interface IGameService
{
    /// <summary>
    /// Crée une nouvelle partie pour un puzzle donné
    /// </summary>
    /// <param name="puzzleId">ID du puzzle à jouer</param>
    /// <param name="playerId">ID du joueur (optionnel)</param>
    /// <returns>La partie créée</returns>
    Task<Game> CreateGameAsync(int puzzleId, string? playerId = null);

    /// <summary>
    /// Récupère une partie par son ID
    /// </summary>
    /// <param name="gameId">ID de la partie</param>
    /// <returns>La partie ou null</returns>
    Task<Game?> GetGameByIdAsync(int gameId);

    /// <summary>
    /// Met à jour les ponts placés par le joueur
    /// </summary>
    /// <param name="gameId">ID de la partie</param>
    /// <param name="bridges">Liste des ponts placés</param>
    /// <returns>La partie mise à jour</returns>
    Task<Game> UpdateGameBridgesAsync(int gameId, List<BridgeDto> bridges);

    /// <summary>
    /// Termine une partie (succès ou abandon)
    /// </summary>
    /// <param name="gameId">ID de la partie</param>
    /// <param name="status">Nouveau statut</param>
    /// <param name="score">Score final</param>
    /// <returns>La partie terminée</returns>
    Task<Game> CompleteGameAsync(int gameId, GameStatus status, int score);

    /// <summary>
    /// Convertit une Game en GameDto
    /// </summary>
    /// <param name="game">La partie à convertir</param>
    /// <returns>Le DTO de la partie</returns>
    GameDto ConvertToDto(Game game);
}

