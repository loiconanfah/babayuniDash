using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using prisonbreak.Server.Data;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;
using prisonbreak.Server.Repositories;

namespace prisonbreak.Server.Services;

/// <summary>
/// Service de gestion des parties de Tic-Tac-Toe
/// Gère la création, la mise à jour et la fin des parties multijoueurs
/// </summary>
public class TicTacToeService : ITicTacToeService
{
    private readonly HashiDbContext _context;
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<TicTacToeService>? _logger;

    public TicTacToeService(
        HashiDbContext context, 
        ISessionRepository sessionRepository,
        ILogger<TicTacToeService>? logger = null)
    {
        _context = context;
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    /// <summary>
    /// Crée une nouvelle partie de Tic-Tac-Toe
    /// </summary>
    public async Task<TicTacToeGame> CreateGameAsync(int sessionId, TicTacToeGameMode gameMode = TicTacToeGameMode.Player, int? player2SessionId = null, int wager = 0)
    {
        // Vérifier que la session existe et est valide
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null || !session.IsValid())
        {
            throw new ArgumentException($"La session avec l'ID {sessionId} n'existe pas ou n'est pas valide", nameof(sessionId));
        }

        // Si une mise est spécifiée, vérifier et débiter les coins
        if (wager > 0)
        {
            var user = await _context.Users.FindAsync(session.UserId);
            if (user == null)
            {
                throw new ArgumentException("L'utilisateur n'existe pas");
            }
            if (user.Coins < wager)
            {
                throw new InvalidOperationException("Vous n'avez pas assez de coins pour cette mise");
            }
            user.RemoveCoins(wager);
            await _context.SaveChangesAsync();
        }

        // Si un joueur 2 est spécifié, vérifier qu'il existe et est valide
        if (player2SessionId.HasValue && gameMode == TicTacToeGameMode.Player)
        {
            var player2Session = await _sessionRepository.GetByIdAsync(player2SessionId.Value);
            if (player2Session == null || !player2Session.IsValid())
            {
                throw new ArgumentException($"La session du joueur 2 avec l'ID {player2SessionId.Value} n'existe pas ou n'est pas valide", nameof(player2SessionId));
            }

            if (player2SessionId.Value == sessionId)
            {
                throw new ArgumentException("Vous ne pouvez pas jouer contre vous-même", nameof(player2SessionId));
            }
        }

        var game = new TicTacToeGame
        {
            Player1SessionId = sessionId,
            Player2SessionId = player2SessionId,
            BoardJson = "[\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"]",
            CurrentPlayer = 1,
            Status = gameMode == TicTacToeGameMode.AI 
                ? TicTacToeGameStatus.InProgress 
                : TicTacToeGameStatus.WaitingForPlayer, // Toujours en attente pour les parties multijoueurs, même avec player2SessionId
            GameMode = gameMode,
            CreatedAt = DateTime.UtcNow,
            Player1Wager = wager
        };

        // Si c'est contre l'IA, démarrer immédiatement
        // Pour les parties multijoueurs, on attend que le joueur 2 accepte et entre sa mise
        if (gameMode == TicTacToeGameMode.AI)
        {
            game.StartedAt = DateTime.UtcNow;
        }

        _context.TicTacToeGames.Add(game);
        await _context.SaveChangesAsync();

        _logger?.LogInformation("Partie Tic-Tac-Toe créée : GameId={GameId}, Player1SessionId={SessionId}, Player2SessionId={Player2SessionId}, GameMode={GameMode}", 
            game.Id, sessionId, player2SessionId, gameMode);

        return game;
    }

    /// <summary>
    /// Récupère une partie par son ID
    /// </summary>
    public async Task<TicTacToeGame?> GetGameByIdAsync(int gameId)
    {
        return await _context.TicTacToeGames
            .Include(g => g.Player1Session)
                .ThenInclude(s => s!.User)
            .Include(g => g.Player2Session)
                .ThenInclude(s => s!.User)
            .FirstOrDefaultAsync(g => g.Id == gameId);
    }

    /// <summary>
    /// Récupère les parties en attente d'un deuxième joueur
    /// </summary>
    public async Task<IEnumerable<TicTacToeGame>> GetAvailableGamesAsync()
    {
        return await _context.TicTacToeGames
            .Include(g => g.Player1Session)
                .ThenInclude(s => s!.User)
            .Where(g => g.Status == TicTacToeGameStatus.WaitingForPlayer)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère les parties où le joueur est invité (parties créées avec player2SessionId correspondant à la session)
    /// </summary>
    public async Task<IEnumerable<TicTacToeGame>> GetInvitationsAsync(int sessionId)
    {
        return await _context.TicTacToeGames
            .Include(g => g.Player1Session)
                .ThenInclude(s => s!.User)
            .Include(g => g.Player2Session)
                .ThenInclude(s => s!.User)
            .Where(g => g.Player2SessionId == sessionId && 
                       (g.Status == TicTacToeGameStatus.InProgress || g.Status == TicTacToeGameStatus.WaitingForPlayer))
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Rejoint une partie existante
    /// </summary>
    public async Task<TicTacToeGame> JoinGameAsync(int gameId, int sessionId, int wager = 0)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null)
        {
            throw new ArgumentException($"La partie avec l'ID {gameId} n'existe pas", nameof(gameId));
        }

        if (game.Status != TicTacToeGameStatus.WaitingForPlayer)
        {
            throw new InvalidOperationException("Cette partie n'est plus disponible");
        }

        if (game.Player1SessionId == sessionId)
        {
            throw new InvalidOperationException("Vous ne pouvez pas rejoindre votre propre partie");
        }

        // Vérifier que la session existe et est valide
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null || !session.IsValid())
        {
            throw new ArgumentException($"La session avec l'ID {sessionId} n'existe pas ou n'est pas valide", nameof(sessionId));
        }

        // Vérifier et gérer la mise
        if (game.Player1Wager > 0)
        {
            if (wager != game.Player1Wager)
            {
                throw new InvalidOperationException($"La mise doit être de {game.Player1Wager} coins pour rejoindre cette partie");
            }
            
            var user2 = await _context.Users.FindAsync(session.UserId);
            if (user2 == null)
            {
                throw new ArgumentException("L'utilisateur n'existe pas");
            }
            if (user2.Coins < wager)
            {
                throw new InvalidOperationException("Vous n'avez pas assez de coins pour cette mise");
            }
            user2.RemoveCoins(wager);
            game.Player2Wager = wager;
        }
        else if (wager > 0)
        {
            var user2 = await _context.Users.FindAsync(session.UserId);
            if (user2 == null)
            {
                throw new ArgumentException("L'utilisateur n'existe pas");
            }
            if (user2.Coins < wager)
            {
                throw new InvalidOperationException("Vous n'avez pas assez de coins pour cette mise");
            }
            user2.RemoveCoins(wager);
            game.Player2Wager = wager;
        }

        game.Player2SessionId = sessionId;
        game.Status = TicTacToeGameStatus.InProgress;
        game.StartedAt = DateTime.UtcNow;
        game.CurrentPlayer = 1; // Le joueur 1 commence

        await _context.SaveChangesAsync();

        _logger?.LogInformation("Joueur 2 a rejoint la partie : GameId={GameId}, Player2SessionId={SessionId}", gameId, sessionId);

        return game;
    }

    /// <summary>
    /// Joue un coup dans la partie
    /// </summary>
    public async Task<TicTacToeGame> PlayMoveAsync(int gameId, int position, int sessionId)
    {
        if (position < 0 || position > 8)
        {
            throw new ArgumentException("La position doit être entre 0 et 8", nameof(position));
        }

        var game = await GetGameByIdAsync(gameId);
        if (game == null)
        {
            throw new ArgumentException($"La partie avec l'ID {gameId} n'existe pas", nameof(gameId));
        }

        if (game.Status != TicTacToeGameStatus.InProgress && game.Status != TicTacToeGameStatus.WaitingForPlayer)
        {
            throw new InvalidOperationException("Cette partie n'est plus en cours");
        }

        // Si la partie est en attente d'un joueur, la démarrer
        if (game.Status == TicTacToeGameStatus.WaitingForPlayer)
        {
            // Vérifier qu'un deuxième joueur a rejoint
            if (game.Player2SessionId == null && game.GameMode == TicTacToeGameMode.Player)
            {
                throw new InvalidOperationException("La partie attend encore un deuxième joueur");
            }
            
            // Démarrer la partie
            game.Status = TicTacToeGameStatus.InProgress;
            if (game.StartedAt == null)
            {
                game.StartedAt = DateTime.UtcNow;
            }
        }

        // Vérifier que c'est le tour du joueur
        int playerNumber = GetPlayerNumber(game, sessionId);
        if (playerNumber == 0)
        {
            throw new UnauthorizedAccessException("Vous n'êtes pas un joueur de cette partie");
        }

        if (game.CurrentPlayer != playerNumber)
        {
            throw new InvalidOperationException("Ce n'est pas votre tour");
        }

        // Désérialiser la grille
        var board = JsonSerializer.Deserialize<List<string>>(game.BoardJson) ?? new List<string>(new string[9]);

        // Vérifier que la case est vide
        if (board[position] != "")
        {
            throw new InvalidOperationException("Cette case est déjà occupée");
        }

        // Jouer le coup du joueur
        board[position] = playerNumber == 1 ? "X" : "O";
        game.BoardJson = JsonSerializer.Serialize(board);
        game.MoveCount++;
        game.ElapsedSeconds = (int)(DateTime.UtcNow - (game.StartedAt ?? game.CreatedAt)).TotalSeconds;

        // Vérifier s'il y a un gagnant après le coup du joueur
        var winner = CheckWinner(board);
        if (winner != null)
        {
            game.Status = TicTacToeGameStatus.Completed;
            game.WinnerPlayerId = winner;
            game.CompletedAt = DateTime.UtcNow;

            // Distribuer les gains si il y a des paris
            if (game.TotalWager > 0)
            {
                var winnerSession = winner == 1 ? game.Player1Session : game.Player2Session;
                if (winnerSession != null)
                {
                    var winnerUser = await _context.Users.FindAsync(winnerSession.UserId);
                    if (winnerUser != null)
                    {
                        winnerUser.AddCoins(game.TotalWager);
                        await _context.SaveChangesAsync();
                    }
                }
            }
        }
        else if (game.MoveCount >= 9)
        {
            // Match nul - rembourser les mises
            game.Status = TicTacToeGameStatus.Draw;
            game.CompletedAt = DateTime.UtcNow;

            if (game.TotalWager > 0)
            {
                // Rembourser les deux joueurs
                if (game.Player1Session != null)
                {
                    var user1 = await _context.Users.FindAsync(game.Player1Session.UserId);
                    if (user1 != null && game.Player1Wager > 0)
                    {
                        user1.AddCoins(game.Player1Wager);
                    }
                }
                if (game.Player2Session != null)
                {
                    var user2 = await _context.Users.FindAsync(game.Player2Session.UserId);
                    if (user2 != null && game.Player2Wager > 0)
                    {
                        user2.AddCoins(game.Player2Wager);
                    }
                }
                await _context.SaveChangesAsync();
            }
        }
        else
        {
            // Si c'est contre l'IA et que le joueur 1 vient de jouer, l'IA joue automatiquement
            if (game.GameMode == TicTacToeGameMode.AI && playerNumber == 1)
            {
                // L'IA joue (joueur 2 = O)
                var aiMove = GetBestMove(board);
                if (aiMove.HasValue)
                {
                    board[aiMove.Value] = "O";
                    game.BoardJson = JsonSerializer.Serialize(board);
                    game.MoveCount++;
                    game.ElapsedSeconds = (int)(DateTime.UtcNow - (game.StartedAt ?? game.CreatedAt)).TotalSeconds;

                    // Vérifier si l'IA a gagné
                    winner = CheckWinner(board);
                    if (winner != null)
                    {
                        game.Status = TicTacToeGameStatus.Completed;
                        game.WinnerPlayerId = winner;
                        game.CompletedAt = DateTime.UtcNow;
                    }
                    else if (game.MoveCount >= 9)
                    {
                        game.Status = TicTacToeGameStatus.Draw;
                        game.CompletedAt = DateTime.UtcNow;
                    }
                    else
                    {
                        // Revenir au joueur 1
                        game.CurrentPlayer = 1;
                    }
                }
            }
            else
            {
                // Passer au joueur suivant (mode multijoueur)
                game.CurrentPlayer = game.CurrentPlayer == 1 ? 2 : 1;
            }
        }

        await _context.SaveChangesAsync();

        _logger?.LogInformation("Coup joué : GameId={GameId}, Player={Player}, Position={Position}", gameId, playerNumber, position);

        return game;
    }

    /// <summary>
    /// Abandonne une partie
    /// </summary>
    public async Task<TicTacToeGame> AbandonGameAsync(int gameId, int sessionId)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null)
        {
            throw new ArgumentException($"La partie avec l'ID {gameId} n'existe pas", nameof(gameId));
        }

        int playerNumber = GetPlayerNumber(game, sessionId);
        if (playerNumber == 0)
        {
            throw new UnauthorizedAccessException("Vous n'êtes pas un joueur de cette partie");
        }

        game.Status = TicTacToeGameStatus.Abandoned;
        game.WinnerPlayerId = playerNumber == 1 ? 2 : 1; // L'autre joueur gagne
        game.CompletedAt = DateTime.UtcNow;
        game.ElapsedSeconds = (int)(DateTime.UtcNow - (game.StartedAt ?? game.CreatedAt)).TotalSeconds;

        await _context.SaveChangesAsync();

        _logger?.LogInformation("Partie abandonnée : GameId={GameId}, Player={Player}", gameId, playerNumber);

        return game;
    }

    /// <summary>
    /// Convertit une TicTacToeGame en TicTacToeGameDto
    /// </summary>
    public TicTacToeGameDto ConvertToDto(TicTacToeGame game)
    {
        var board = JsonSerializer.Deserialize<List<string>>(game.BoardJson) ?? new List<string>(new string[9]);

        return new TicTacToeGameDto
        {
            Id = game.Id,
            Player1SessionId = game.Player1SessionId,
            Player1Name = game.Player1Session?.User?.Name,
            Player2SessionId = game.Player2SessionId,
            Player2Name = game.GameMode == TicTacToeGameMode.AI ? "IA" : game.Player2Session?.User?.Name,
            Board = board,
            CurrentPlayer = game.CurrentPlayer,
            Status = (int)game.Status,
            WinnerPlayerId = game.WinnerPlayerId,
            CreatedAt = game.CreatedAt,
            StartedAt = game.StartedAt,
            CompletedAt = game.CompletedAt,
            ElapsedSeconds = game.ElapsedSeconds,
            MoveCount = game.MoveCount,
            GameMode = (int)game.GameMode,
            Player1Wager = game.Player1Wager,
            Player2Wager = game.Player2Wager
        };
    }

    /// <summary>
    /// Obtient le numéro du joueur (1 ou 2) selon sa session, ou 0 si ce n'est pas un joueur
    /// </summary>
    private int GetPlayerNumber(TicTacToeGame game, int sessionId)
    {
        if (game.Player1SessionId == sessionId)
            return 1;
        if (game.Player2SessionId == sessionId)
            return 2;
        return 0;
    }

    /// <summary>
    /// Vérifie s'il y a un gagnant dans la grille
    /// Retourne 1 si X gagne, 2 si O gagne, null sinon
    /// </summary>
    private int? CheckWinner(List<string> board)
    {
        // Lignes
        for (int i = 0; i < 3; i++)
        {
            int start = i * 3;
            if (board[start] != "" && board[start] == board[start + 1] && board[start] == board[start + 2])
            {
                return board[start] == "X" ? 1 : 2;
            }
        }

        // Colonnes
        for (int i = 0; i < 3; i++)
        {
            if (board[i] != "" && board[i] == board[i + 3] && board[i] == board[i + 6])
            {
                return board[i] == "X" ? 1 : 2;
            }
        }

        // Diagonale principale (haut-gauche vers bas-droite)
        if (board[0] != "" && board[0] == board[4] && board[0] == board[8])
        {
            return board[0] == "X" ? 1 : 2;
        }

        // Diagonale secondaire (haut-droite vers bas-gauche)
        if (board[2] != "" && board[2] == board[4] && board[2] == board[6])
        {
            return board[2] == "X" ? 1 : 2;
        }

        return null;
    }

    /// <summary>
    /// Obtient le meilleur coup pour l'IA
    /// Utilise une stratégie simple : bloquer le joueur si nécessaire, sinon jouer au centre ou dans un coin
    /// </summary>
    private int? GetBestMove(List<string> board)
    {
        // 1. Vérifier si l'IA peut gagner
        for (int i = 0; i < 9; i++)
        {
            if (board[i] == "")
            {
                board[i] = "O";
                if (CheckWinner(board) == 2)
                {
                    board[i] = ""; // Restaurer
                    return i;
                }
                board[i] = ""; // Restaurer
            }
        }

        // 2. Bloquer le joueur s'il peut gagner
        for (int i = 0; i < 9; i++)
        {
            if (board[i] == "")
            {
                board[i] = "X";
                if (CheckWinner(board) == 1)
                {
                    board[i] = ""; // Restaurer
                    return i;
                }
                board[i] = ""; // Restaurer
            }
        }

        // 3. Jouer au centre si disponible
        if (board[4] == "")
        {
            return 4;
        }

        // 4. Jouer dans un coin disponible
        int[] corners = { 0, 2, 6, 8 };
        foreach (int corner in corners)
        {
            if (board[corner] == "")
            {
                return corner;
            }
        }

        // 5. Jouer dans n'importe quelle case disponible
        for (int i = 0; i < 9; i++)
        {
            if (board[i] == "")
            {
                return i;
            }
        }

        return null; // Aucun coup disponible
    }
}

