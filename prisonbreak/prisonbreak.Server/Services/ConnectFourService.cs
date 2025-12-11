using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using prisonbreak.Server.Data;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;
using prisonbreak.Server.Repositories;

namespace prisonbreak.Server.Services;

/// <summary>
/// Service de gestion des parties de Connect Four (Puissance 4)
/// Gère la création, la mise à jour et la fin des parties multijoueurs
/// </summary>
public class ConnectFourService : IConnectFourService
{
    private readonly HashiDbContext _context;
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<ConnectFourService>? _logger;

    private const int COLUMNS = 7;
    private const int ROWS = 6;

    public ConnectFourService(
        HashiDbContext context, 
        ISessionRepository sessionRepository,
        ILogger<ConnectFourService>? logger = null)
    {
        _context = context;
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    /// <summary>
    /// Crée une nouvelle partie de Connect Four
    /// </summary>
    public async Task<ConnectFourGame> CreateGameAsync(int sessionId, ConnectFourGameMode gameMode = ConnectFourGameMode.Player, int? player2SessionId = null)
    {
        // Vérifier que la session existe et est valide
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null || !session.IsValid())
        {
            throw new ArgumentException($"La session avec l'ID {sessionId} n'existe pas ou n'est pas valide", nameof(sessionId));
        }

        // Si un joueur 2 est spécifié, vérifier qu'il existe et est valide
        if (player2SessionId.HasValue && gameMode == ConnectFourGameMode.Player)
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

        // Créer une grille vide (7 colonnes x 6 lignes)
        var emptyBoard = new List<List<int>>();
        for (int col = 0; col < COLUMNS; col++)
        {
            emptyBoard.Add(new List<int>(new int[ROWS]));
        }

        var game = new ConnectFourGame
        {
            Player1SessionId = sessionId,
            Player2SessionId = player2SessionId,
            BoardJson = JsonSerializer.Serialize(emptyBoard),
            CurrentPlayer = 1,
            Status = gameMode == ConnectFourGameMode.AI 
                ? ConnectFourGameStatus.InProgress 
                : (player2SessionId.HasValue ? ConnectFourGameStatus.InProgress : ConnectFourGameStatus.WaitingForPlayer),
            GameMode = gameMode,
            CreatedAt = DateTime.UtcNow
        };

        // Si c'est contre l'IA ou si un joueur 2 est déjà assigné, démarrer immédiatement
        if (gameMode == ConnectFourGameMode.AI || player2SessionId.HasValue)
        {
            game.StartedAt = DateTime.UtcNow;
        }

        _context.ConnectFourGames.Add(game);
        await _context.SaveChangesAsync();

        _logger?.LogInformation("Partie Connect Four créée : GameId={GameId}, Player1SessionId={SessionId}, Player2SessionId={Player2SessionId}, GameMode={GameMode}", 
            game.Id, sessionId, player2SessionId, gameMode);

        return game;
    }

    /// <summary>
    /// Récupère une partie par son ID
    /// </summary>
    public async Task<ConnectFourGame?> GetGameByIdAsync(int gameId)
    {
        return await _context.ConnectFourGames
            .Include(g => g.Player1Session)
                .ThenInclude(s => s!.User)
            .Include(g => g.Player2Session)
                .ThenInclude(s => s!.User)
            .FirstOrDefaultAsync(g => g.Id == gameId);
    }

    /// <summary>
    /// Récupère les parties en attente d'un deuxième joueur
    /// </summary>
    public async Task<IEnumerable<ConnectFourGame>> GetAvailableGamesAsync()
    {
        return await _context.ConnectFourGames
            .Include(g => g.Player1Session)
                .ThenInclude(s => s!.User)
            .Where(g => g.Status == ConnectFourGameStatus.WaitingForPlayer)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère les parties où le joueur est invité
    /// </summary>
    public async Task<IEnumerable<ConnectFourGame>> GetInvitationsAsync(int sessionId)
    {
        return await _context.ConnectFourGames
            .Include(g => g.Player1Session)
                .ThenInclude(s => s!.User)
            .Include(g => g.Player2Session)
                .ThenInclude(s => s!.User)
            .Where(g => g.Player2SessionId == sessionId && 
                       (g.Status == ConnectFourGameStatus.InProgress || g.Status == ConnectFourGameStatus.WaitingForPlayer))
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Rejoint une partie existante
    /// </summary>
    public async Task<ConnectFourGame> JoinGameAsync(int gameId, int sessionId)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null)
        {
            throw new ArgumentException($"La partie avec l'ID {gameId} n'existe pas", nameof(gameId));
        }

        if (game.Status != ConnectFourGameStatus.WaitingForPlayer)
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

        game.Player2SessionId = sessionId;
        game.Status = ConnectFourGameStatus.InProgress;
        game.StartedAt = DateTime.UtcNow;
        game.CurrentPlayer = 1; // Le joueur 1 commence

        await _context.SaveChangesAsync();

        _logger?.LogInformation("Joueur 2 a rejoint la partie : GameId={GameId}, Player2SessionId={SessionId}", gameId, sessionId);

        return game;
    }

    /// <summary>
    /// Joue un coup dans la partie (laisse tomber une pièce dans une colonne)
    /// </summary>
    public async Task<ConnectFourGame> PlayMoveAsync(int gameId, int column, int sessionId)
    {
        if (column < 0 || column >= COLUMNS)
        {
            throw new ArgumentException($"La colonne doit être entre 0 et {COLUMNS - 1}", nameof(column));
        }

        var game = await GetGameByIdAsync(gameId);
        if (game == null)
        {
            throw new ArgumentException($"La partie avec l'ID {gameId} n'existe pas", nameof(gameId));
        }

        if (game.Status != ConnectFourGameStatus.InProgress && game.Status != ConnectFourGameStatus.WaitingForPlayer)
        {
            throw new InvalidOperationException("Cette partie n'est plus en cours");
        }

        // Si la partie est en attente d'un joueur, la démarrer
        if (game.Status == ConnectFourGameStatus.WaitingForPlayer)
        {
            if (game.Player2SessionId == null && game.GameMode == ConnectFourGameMode.Player)
            {
                throw new InvalidOperationException("La partie attend encore un deuxième joueur");
            }
            
            game.Status = ConnectFourGameStatus.InProgress;
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
        var board = JsonSerializer.Deserialize<List<List<int>>>(game.BoardJson) ?? new List<List<int>>();

        // Trouver la première case vide dans la colonne (la pièce tombe vers le bas)
        int row = -1;
        for (int r = ROWS - 1; r >= 0; r--)
        {
            if (board[column][r] == 0)
            {
                row = r;
                break;
            }
        }

        if (row == -1)
        {
            throw new InvalidOperationException("Cette colonne est pleine");
        }

        // Placer la pièce
        board[column][row] = playerNumber;
        game.BoardJson = JsonSerializer.Serialize(board);
        game.MoveCount++;
        game.ElapsedSeconds = (int)(DateTime.UtcNow - (game.StartedAt ?? game.CreatedAt)).TotalSeconds;

        // Vérifier s'il y a un gagnant
        var winner = CheckWinner(board, column, row);
        if (winner != null)
        {
            game.Status = ConnectFourGameStatus.Completed;
            game.WinnerPlayerId = winner;
            game.CompletedAt = DateTime.UtcNow;
        }
        else if (IsBoardFull(board))
        {
            // Match nul
            game.Status = ConnectFourGameStatus.Draw;
            game.CompletedAt = DateTime.UtcNow;
        }
        else
        {
            // Si c'est contre l'IA et que le joueur 1 vient de jouer, l'IA joue automatiquement
            if (game.GameMode == ConnectFourGameMode.AI && playerNumber == 1)
            {
                // L'IA joue (joueur 2)
                var aiColumn = GetBestMove(board);
                if (aiColumn.HasValue)
                {
                    int aiRow = -1;
                    for (int r = ROWS - 1; r >= 0; r--)
                    {
                        if (board[aiColumn.Value][r] == 0)
                        {
                            aiRow = r;
                            break;
                        }
                    }

                    if (aiRow != -1)
                    {
                        board[aiColumn.Value][aiRow] = 2;
                        game.BoardJson = JsonSerializer.Serialize(board);
                        game.MoveCount++;
                        game.ElapsedSeconds = (int)(DateTime.UtcNow - (game.StartedAt ?? game.CreatedAt)).TotalSeconds;

                        // Vérifier si l'IA a gagné
                        winner = CheckWinner(board, aiColumn.Value, aiRow);
                        if (winner != null)
                        {
                            game.Status = ConnectFourGameStatus.Completed;
                            game.WinnerPlayerId = winner;
                            game.CompletedAt = DateTime.UtcNow;
                        }
                        else if (IsBoardFull(board))
                        {
                            game.Status = ConnectFourGameStatus.Draw;
                            game.CompletedAt = DateTime.UtcNow;
                        }
                        else
                        {
                            // Revenir au joueur 1
                            game.CurrentPlayer = 1;
                        }
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

        _logger?.LogInformation("Coup joué : GameId={GameId}, Player={Player}, Column={Column}", gameId, playerNumber, column);

        return game;
    }

    /// <summary>
    /// Abandonne une partie
    /// </summary>
    public async Task<ConnectFourGame> AbandonGameAsync(int gameId, int sessionId)
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

        game.Status = ConnectFourGameStatus.Abandoned;
        game.WinnerPlayerId = playerNumber == 1 ? 2 : 1; // L'autre joueur gagne
        game.CompletedAt = DateTime.UtcNow;
        game.ElapsedSeconds = (int)(DateTime.UtcNow - (game.StartedAt ?? game.CreatedAt)).TotalSeconds;

        await _context.SaveChangesAsync();

        _logger?.LogInformation("Partie abandonnée : GameId={GameId}, Player={Player}", gameId, playerNumber);

        return game;
    }

    /// <summary>
    /// Convertit une ConnectFourGame en ConnectFourGameDto
    /// </summary>
    public ConnectFourGameDto ConvertToDto(ConnectFourGame game)
    {
        var board = JsonSerializer.Deserialize<List<List<int>>>(game.BoardJson) ?? new List<List<int>>();

        return new ConnectFourGameDto
        {
            Id = game.Id,
            Player1SessionId = game.Player1SessionId,
            Player1Name = game.Player1Session?.User?.Name,
            Player2SessionId = game.Player2SessionId,
            Player2Name = game.GameMode == ConnectFourGameMode.AI ? "IA" : game.Player2Session?.User?.Name,
            Board = board,
            CurrentPlayer = game.CurrentPlayer,
            Status = (int)game.Status,
            WinnerPlayerId = game.WinnerPlayerId,
            CreatedAt = game.CreatedAt,
            StartedAt = game.StartedAt,
            CompletedAt = game.CompletedAt,
            ElapsedSeconds = game.ElapsedSeconds,
            MoveCount = game.MoveCount,
            GameMode = (int)game.GameMode
        };
    }

    /// <summary>
    /// Obtient le numéro du joueur (1 ou 2) selon sa session, ou 0 si ce n'est pas un joueur
    /// </summary>
    private int GetPlayerNumber(ConnectFourGame game, int sessionId)
    {
        if (game.Player1SessionId == sessionId)
            return 1;
        if (game.Player2SessionId == sessionId)
            return 2;
        return 0;
    }

    /// <summary>
    /// Vérifie s'il y a un gagnant après un coup joué à la position (column, row)
    /// Retourne 1 si le joueur 1 gagne, 2 si le joueur 2 gagne, null sinon
    /// </summary>
    private int? CheckWinner(List<List<int>> board, int column, int row)
    {
        int player = board[column][row];
        if (player == 0) return null;

        // Vérifier horizontalement (gauche-droite)
        int count = 1;
        // Vers la gauche
        for (int c = column - 1; c >= 0 && board[c][row] == player; c--) count++;
        // Vers la droite
        for (int c = column + 1; c < COLUMNS && board[c][row] == player; c++) count++;
        if (count >= 4) return player;

        // Vérifier verticalement (haut-bas)
        count = 1;
        // Vers le bas
        for (int r = row + 1; r < ROWS && board[column][r] == player; r++) count++;
        // Vers le haut
        for (int r = row - 1; r >= 0 && board[column][r] == player; r--) count++;
        if (count >= 4) return player;

        // Vérifier diagonale principale (haut-gauche vers bas-droite)
        count = 1;
        // Vers haut-gauche
        for (int c = column - 1, r = row - 1; c >= 0 && r >= 0 && board[c][r] == player; c--, r--) count++;
        // Vers bas-droite
        for (int c = column + 1, r = row + 1; c < COLUMNS && r < ROWS && board[c][r] == player; c++, r++) count++;
        if (count >= 4) return player;

        // Vérifier diagonale secondaire (haut-droite vers bas-gauche)
        count = 1;
        // Vers haut-droite
        for (int c = column + 1, r = row - 1; c < COLUMNS && r >= 0 && board[c][r] == player; c++, r--) count++;
        // Vers bas-gauche
        for (int c = column - 1, r = row + 1; c >= 0 && r < ROWS && board[c][r] == player; c--, r++) count++;
        if (count >= 4) return player;

        return null;
    }

    /// <summary>
    /// Vérifie si la grille est pleine (match nul)
    /// </summary>
    private bool IsBoardFull(List<List<int>> board)
    {
        for (int c = 0; c < COLUMNS; c++)
        {
            if (board[c][0] == 0) // Si la case du haut est vide, la colonne n'est pas pleine
                return false;
        }
        return true;
    }

    /// <summary>
    /// Obtient le meilleur coup pour l'IA
    /// Utilise une stratégie simple : gagner si possible, bloquer le joueur sinon, sinon jouer au centre
    /// </summary>
    private int? GetBestMove(List<List<int>> board)
    {
        // 1. Vérifier si l'IA peut gagner
        for (int c = 0; c < COLUMNS; c++)
        {
            int row = GetNextEmptyRow(board, c);
            if (row != -1)
            {
                board[c][row] = 2;
                if (CheckWinner(board, c, row) == 2)
                {
                    board[c][row] = 0; // Restaurer
                    return c;
                }
                board[c][row] = 0; // Restaurer
            }
        }

        // 2. Bloquer le joueur s'il peut gagner
        for (int c = 0; c < COLUMNS; c++)
        {
            int row = GetNextEmptyRow(board, c);
            if (row != -1)
            {
                board[c][row] = 1;
                if (CheckWinner(board, c, row) == 1)
                {
                    board[c][row] = 0; // Restaurer
                    return c;
                }
                board[c][row] = 0; // Restaurer
            }
        }

        // 3. Jouer au centre si disponible
        int center = COLUMNS / 2;
        if (GetNextEmptyRow(board, center) != -1)
        {
            return center;
        }

        // 4. Jouer dans n'importe quelle colonne disponible
        for (int c = 0; c < COLUMNS; c++)
        {
            if (GetNextEmptyRow(board, c) != -1)
            {
                return c;
            }
        }

        return null; // Aucun coup disponible
    }

    /// <summary>
    /// Obtient la prochaine ligne vide dans une colonne (la pièce tombe vers le bas)
    /// Retourne -1 si la colonne est pleine
    /// </summary>
    private int GetNextEmptyRow(List<List<int>> board, int column)
    {
        for (int r = ROWS - 1; r >= 0; r--)
        {
            if (board[column][r] == 0)
            {
                return r;
            }
        }
        return -1; // Colonne pleine
    }
}

