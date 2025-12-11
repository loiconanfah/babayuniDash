using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Services;

/// <summary>
/// Interface pour le service de gestion des parties de Connect Four
/// </summary>
public interface IConnectFourService
{
    /// <summary>
    /// Crée une nouvelle partie de Connect Four
    /// </summary>
    /// <param name="sessionId">ID de la session du joueur qui crée la partie</param>
    /// <param name="gameMode">Mode de jeu : contre un joueur ou contre l'IA</param>
    /// <param name="player2SessionId">ID de la session du joueur 2 (optionnel, pour inviter un joueur spécifique)</param>
    /// <returns>La partie créée</returns>
    Task<ConnectFourGame> CreateGameAsync(int sessionId, ConnectFourGameMode gameMode = ConnectFourGameMode.Player, int? player2SessionId = null);

    /// <summary>
    /// Récupère une partie par son ID
    /// </summary>
    /// <param name="gameId">ID de la partie</param>
    /// <returns>La partie ou null</returns>
    Task<ConnectFourGame?> GetGameByIdAsync(int gameId);

    /// <summary>
    /// Récupère les parties en attente d'un deuxième joueur
    /// </summary>
    /// <returns>Liste des parties disponibles</returns>
    Task<IEnumerable<ConnectFourGame>> GetAvailableGamesAsync();

    /// <summary>
    /// Récupère les parties où le joueur est invité
    /// </summary>
    /// <param name="sessionId">ID de la session du joueur</param>
    /// <returns>Liste des parties où le joueur est invité</returns>
    Task<IEnumerable<ConnectFourGame>> GetInvitationsAsync(int sessionId);

    /// <summary>
    /// Rejoint une partie existante
    /// </summary>
    /// <param name="gameId">ID de la partie</param>
    /// <param name="sessionId">ID de la session du joueur qui rejoint</param>
    /// <returns>La partie mise à jour</returns>
    Task<ConnectFourGame> JoinGameAsync(int gameId, int sessionId);

    /// <summary>
    /// Joue un coup dans la partie (laisse tomber une pièce dans une colonne)
    /// </summary>
    /// <param name="gameId">ID de la partie</param>
    /// <param name="column">Numéro de la colonne (0-6)</param>
    /// <param name="sessionId">ID de la session du joueur qui joue</param>
    /// <returns>La partie mise à jour</returns>
    Task<ConnectFourGame> PlayMoveAsync(int gameId, int column, int sessionId);

    /// <summary>
    /// Abandonne une partie
    /// </summary>
    /// <param name="gameId">ID de la partie</param>
    /// <param name="sessionId">ID de la session du joueur qui abandonne</param>
    /// <returns>La partie mise à jour</returns>
    Task<ConnectFourGame> AbandonGameAsync(int gameId, int sessionId);

    /// <summary>
    /// Convertit une ConnectFourGame en ConnectFourGameDto
    /// </summary>
    /// <param name="game">La partie à convertir</param>
    /// <returns>Le DTO de la partie</returns>
    ConnectFourGameDto ConvertToDto(ConnectFourGame game);
}

