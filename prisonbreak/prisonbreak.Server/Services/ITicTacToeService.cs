using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Services;

/// <summary>
/// Interface pour le service de gestion des parties de Tic-Tac-Toe
/// </summary>
public interface ITicTacToeService
{
    /// <summary>
    /// Crée une nouvelle partie de Tic-Tac-Toe
    /// </summary>
    /// <param name="sessionId">ID de la session du joueur qui crée la partie</param>
    /// <param name="gameMode">Mode de jeu : contre un joueur ou contre l'IA</param>
    /// <param name="player2SessionId">ID de la session du joueur 2 (optionnel, pour inviter un joueur spécifique)</param>
    /// <returns>La partie créée</returns>
    Task<TicTacToeGame> CreateGameAsync(int sessionId, TicTacToeGameMode gameMode = TicTacToeGameMode.Player, int? player2SessionId = null);

    /// <summary>
    /// Récupère une partie par son ID
    /// </summary>
    /// <param name="gameId">ID de la partie</param>
    /// <returns>La partie ou null</returns>
    Task<TicTacToeGame?> GetGameByIdAsync(int gameId);

    /// <summary>
    /// Récupère les parties en attente d'un deuxième joueur
    /// </summary>
    /// <returns>Liste des parties disponibles</returns>
    Task<IEnumerable<TicTacToeGame>> GetAvailableGamesAsync();

    /// <summary>
    /// Récupère les parties où le joueur est invité (parties créées avec player2SessionId correspondant à la session)
    /// </summary>
    /// <param name="sessionId">ID de la session du joueur</param>
    /// <returns>Liste des parties où le joueur est invité</returns>
    Task<IEnumerable<TicTacToeGame>> GetInvitationsAsync(int sessionId);

    /// <summary>
    /// Rejoint une partie existante
    /// </summary>
    /// <param name="gameId">ID de la partie</param>
    /// <param name="sessionId">ID de la session du joueur qui rejoint</param>
    /// <returns>La partie mise à jour</returns>
    Task<TicTacToeGame> JoinGameAsync(int gameId, int sessionId);

    /// <summary>
    /// Joue un coup dans la partie
    /// </summary>
    /// <param name="gameId">ID de la partie</param>
    /// <param name="position">Position dans la grille (0-8)</param>
    /// <param name="sessionId">ID de la session du joueur qui joue</param>
    /// <returns>La partie mise à jour</returns>
    Task<TicTacToeGame> PlayMoveAsync(int gameId, int position, int sessionId);

    /// <summary>
    /// Abandonne une partie
    /// </summary>
    /// <param name="gameId">ID de la partie</param>
    /// <param name="sessionId">ID de la session du joueur qui abandonne</param>
    /// <returns>La partie mise à jour</returns>
    Task<TicTacToeGame> AbandonGameAsync(int gameId, int sessionId);

    /// <summary>
    /// Convertit une TicTacToeGame en TicTacToeGameDto
    /// </summary>
    /// <param name="game">La partie à convertir</param>
    /// <returns>Le DTO de la partie</returns>
    TicTacToeGameDto ConvertToDto(TicTacToeGame game);
}

