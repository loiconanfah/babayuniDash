using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using prisonbreak.Server.Data;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;
using prisonbreak.Server.Repositories;

namespace prisonbreak.Server.Services;

public class RockPaperScissorsService : IRockPaperScissorsService
{
    private readonly HashiDbContext _context;
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<RockPaperScissorsService>? _logger;

    public RockPaperScissorsService(
        HashiDbContext context,
        ISessionRepository sessionRepository,
        ILogger<RockPaperScissorsService>? logger = null)
    {
        _context = context;
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    public async Task<RockPaperScissorsGame> CreateGameAsync(int sessionId, int wager, RPSGameMode gameMode = RPSGameMode.Player, int? player2SessionId = null)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null || !session.IsValid())
        {
            throw new ArgumentException($"La session avec l'ID {sessionId} n'existe pas ou n'est pas valide", nameof(sessionId));
        }

        // Gérer la mise si c'est une partie multijoueur
        if (wager > 0 && gameMode == RPSGameMode.Player)
        {
            var user = await _context.Users.FindAsync(session.UserId);
            if (user == null)
            {
                throw new ArgumentException("L'utilisateur associé à la session n'existe pas", nameof(sessionId));
            }

            if (user.Coins < wager)
            {
                throw new InvalidOperationException($"Vous n'avez pas assez de coins. Vous avez {user.Coins} coins, mais vous essayez de miser {wager} coins.");
            }

            // Déduire les coins du joueur 1
            user.Coins -= wager;
            _context.Users.Update(user);
        }

        if (player2SessionId.HasValue && gameMode == RPSGameMode.Player)
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

        var game = new RockPaperScissorsGame
        {
            Player1SessionId = sessionId,
            Player2SessionId = player2SessionId,
            GameMode = gameMode,
            Status = gameMode == RPSGameMode.AI 
                ? RPSGameStatus.WaitingForChoices 
                : RPSGameStatus.WaitingForPlayer, // Toujours en attente pour les parties multijoueurs, même avec player2SessionId
            CreatedAt = DateTime.UtcNow,
            RoundsToWin = 3,
            Player1Wager = wager
        };

        // Si c'est contre l'IA, démarrer immédiatement
        // Pour les parties multijoueurs, on attend que le joueur 2 accepte et entre sa mise
        if (gameMode == RPSGameMode.AI)
        {
            game.StartedAt = DateTime.UtcNow;
        }

        _context.RockPaperScissorsGames.Add(game);
        await _context.SaveChangesAsync();

        _logger?.LogInformation("Partie RPS créée : GameId={GameId}, Player1SessionId={SessionId}, GameMode={GameMode}", 
            game.Id, sessionId, gameMode);

        return game;
    }

    public async Task<RockPaperScissorsGame?> GetGameByIdAsync(int gameId)
    {
        return await _context.RockPaperScissorsGames
            .Include(g => g.Player1Session)
                .ThenInclude(s => s!.User)
            .Include(g => g.Player2Session)
                .ThenInclude(s => s!.User)
            .FirstOrDefaultAsync(g => g.Id == gameId);
    }

    public async Task<IEnumerable<RockPaperScissorsGame>> GetAvailableGamesAsync()
    {
        return await _context.RockPaperScissorsGames
            .Include(g => g.Player1Session)
                .ThenInclude(s => s!.User)
            .Where(g => g.Status == RPSGameStatus.WaitingForPlayer)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<RockPaperScissorsGame>> GetInvitationsAsync(int sessionId)
    {
        return await _context.RockPaperScissorsGames
            .Include(g => g.Player1Session)
                .ThenInclude(s => s!.User)
            .Include(g => g.Player2Session)
                .ThenInclude(s => s!.User)
            .Where(g => g.Player2SessionId == sessionId && 
                       (g.Status == RPSGameStatus.WaitingForChoices || 
                        g.Status == RPSGameStatus.RoundCompleted ||
                        g.Status == RPSGameStatus.WaitingForPlayer))
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    public async Task<RockPaperScissorsGame> JoinGameAsync(int gameId, int sessionId, int wager = 0)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null)
        {
            throw new ArgumentException($"La partie avec l'ID {gameId} n'existe pas", nameof(gameId));
        }

        if (game.Status != RPSGameStatus.WaitingForPlayer)
        {
            throw new InvalidOperationException("Cette partie n'est plus disponible");
        }

        if (game.Player1SessionId == sessionId)
        {
            throw new InvalidOperationException("Vous ne pouvez pas rejoindre votre propre partie");
        }

        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null || !session.IsValid())
        {
            throw new ArgumentException($"La session avec l'ID {sessionId} n'existe pas ou n'est pas valide", nameof(sessionId));
        }

        // Gérer la mise si nécessaire
        if (game.Player1Wager > 0)
        {
            if (wager != game.Player1Wager)
            {
                throw new InvalidOperationException($"Vous devez miser exactement {game.Player1Wager} coins pour rejoindre cette partie");
            }

            var user = await _context.Users.FindAsync(session.UserId);
            if (user == null)
            {
                throw new ArgumentException("L'utilisateur associé à la session n'existe pas", nameof(sessionId));
            }

            if (user.Coins < wager)
            {
                throw new InvalidOperationException($"Vous n'avez pas assez de coins. Vous avez {user.Coins} coins, mais vous essayez de miser {wager} coins.");
            }

            // Déduire les coins du joueur 2
            user.Coins -= wager;
            _context.Users.Update(user);
        }
        else if (wager > 0)
        {
            // Si le joueur 1 n'a pas misé mais le joueur 2 veut miser, ce n'est pas autorisé
            throw new InvalidOperationException("Le joueur 1 n'a pas misé de coins, vous ne pouvez pas miser");
        }

        game.Player2SessionId = sessionId;
        game.Player2Wager = wager;
        game.Status = RPSGameStatus.WaitingForChoices;
        game.StartedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger?.LogInformation("Joueur 2 a rejoint la partie : GameId={GameId}, Player2SessionId={SessionId}", gameId, sessionId);

        return game;
    }

    public async Task<RockPaperScissorsGame> PlayChoiceAsync(int gameId, int sessionId, RPSChoice choice)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null)
        {
            throw new ArgumentException($"La partie avec l'ID {gameId} n'existe pas", nameof(gameId));
        }

        if (game.Status != RPSGameStatus.WaitingForChoices)
        {
            throw new InvalidOperationException("La partie n'est pas en attente de choix");
        }

        int playerNumber = GetPlayerNumber(game, sessionId);
        if (playerNumber == 0)
        {
            throw new UnauthorizedAccessException("Vous n'êtes pas un joueur de cette partie");
        }

        // Enregistrer le choix
        if (playerNumber == 1)
        {
            if (game.Player1Choice.HasValue)
            {
                throw new InvalidOperationException("Vous avez déjà fait votre choix pour ce round");
            }
            game.Player1Choice = (int)choice;
        }
        else
        {
            if (game.Player2Choice.HasValue)
            {
                throw new InvalidOperationException("Vous avez déjà fait votre choix pour ce round");
            }
            game.Player2Choice = (int)choice;
        }

        // Si c'est contre l'IA et que le joueur vient de jouer, l'IA joue automatiquement
        if (game.GameMode == RPSGameMode.AI && playerNumber == 1 && !game.Player2Choice.HasValue)
        {
            var random = new Random();
            game.Player2Choice = random.Next(1, 4); // 1=Rock, 2=Paper, 3=Scissors
        }

        // Vérifier si les deux joueurs ont fait leur choix
        if (game.Player1Choice.HasValue && game.Player2Choice.HasValue)
        {
            // Déterminer le gagnant du round
            game.RoundWinner = DetermineWinner((RPSChoice)game.Player1Choice.Value, (RPSChoice)game.Player2Choice.Value);
            
            if (game.RoundWinner == 1)
            {
                game.Player1Score++;
            }
            else if (game.RoundWinner == 2)
            {
                game.Player2Score++;
            }

            game.Status = RPSGameStatus.RoundCompleted;
            game.ElapsedSeconds = (int)(DateTime.UtcNow - (game.StartedAt ?? game.CreatedAt)).TotalSeconds;

            // Vérifier si un joueur a gagné la partie
            if (game.Player1Score >= game.RoundsToWin)
            {
                game.WinnerPlayerId = 1;
                game.Status = RPSGameStatus.Completed;
                game.CompletedAt = DateTime.UtcNow;

                // Distribuer les gains si il y a des paris
                if (game.Player1Wager + game.Player2Wager > 0)
                {
                    if (game.Player1Session != null)
                    {
                        var winnerUser = await _context.Users.FindAsync(game.Player1Session.UserId);
                        if (winnerUser != null)
                        {
                            var totalWager = game.Player1Wager + game.Player2Wager;
                            winnerUser.Coins += totalWager;
                            _context.Users.Update(winnerUser);
                        }
                    }
                }
            }
            else if (game.Player2Score >= game.RoundsToWin)
            {
                game.WinnerPlayerId = 2;
                game.Status = RPSGameStatus.Completed;
                game.CompletedAt = DateTime.UtcNow;

                // Distribuer les gains si il y a des paris
                if (game.Player1Wager + game.Player2Wager > 0)
                {
                    if (game.Player2Session != null)
                    {
                        var winnerUser = await _context.Users.FindAsync(game.Player2Session.UserId);
                        if (winnerUser != null)
                        {
                            var totalWager = game.Player1Wager + game.Player2Wager;
                            winnerUser.Coins += totalWager;
                            _context.Users.Update(winnerUser);
                        }
                    }
                }
            }
        }

        await _context.SaveChangesAsync();

        _logger?.LogInformation("Choix joué : GameId={GameId}, Player={Player}, Choice={Choice}", gameId, playerNumber, choice);

        return game;
    }

    public async Task<RockPaperScissorsGame> NextRoundAsync(int gameId, int sessionId)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null)
        {
            throw new ArgumentException($"La partie avec l'ID {gameId} n'existe pas", nameof(gameId));
        }

        if (game.Status != RPSGameStatus.RoundCompleted)
        {
            throw new InvalidOperationException("Le round n'est pas terminé");
        }

        if (game.Status == RPSGameStatus.Completed)
        {
            throw new InvalidOperationException("La partie est terminée");
        }

        int playerNumber = GetPlayerNumber(game, sessionId);
        if (playerNumber == 0)
        {
            throw new UnauthorizedAccessException("Vous n'êtes pas un joueur de cette partie");
        }

        // Passer au round suivant
        game.RoundNumber++;
        game.Player1Choice = null;
        game.Player2Choice = null;
        game.RoundWinner = null;
        game.Status = RPSGameStatus.WaitingForChoices;

        await _context.SaveChangesAsync();

        _logger?.LogInformation("Round suivant : GameId={GameId}, Round={Round}", gameId, game.RoundNumber);

        return game;
    }

    public async Task<RockPaperScissorsGame> AbandonGameAsync(int gameId, int sessionId)
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

        game.Status = RPSGameStatus.Abandoned;
        game.WinnerPlayerId = playerNumber == 1 ? 2 : 1;
        game.CompletedAt = DateTime.UtcNow;
        game.ElapsedSeconds = (int)(DateTime.UtcNow - (game.StartedAt ?? game.CreatedAt)).TotalSeconds;

        // Distribuer les gains si il y a des paris
        if (game.Player1Wager + game.Player2Wager > 0)
        {
            var winnerSession = game.WinnerPlayerId == 1 ? game.Player1Session : game.Player2Session;
            if (winnerSession != null)
            {
                var winnerUser = await _context.Users.FindAsync(winnerSession.UserId);
                if (winnerUser != null)
                {
                    var totalWager = game.Player1Wager + game.Player2Wager;
                    winnerUser.Coins += totalWager;
                    _context.Users.Update(winnerUser);
                }
            }
        }

        await _context.SaveChangesAsync();

        _logger?.LogInformation("Partie abandonnée : GameId={GameId}, Player={Player}", gameId, playerNumber);

        return game;
    }

    public RockPaperScissorsGameDto ConvertToDto(RockPaperScissorsGame game, int? viewingPlayerSessionId = null)
    {
        int? viewingPlayer = viewingPlayerSessionId.HasValue ? GetPlayerNumber(game, viewingPlayerSessionId.Value) : null;

        var dto = new RockPaperScissorsGameDto
        {
            Id = game.Id,
            Player1SessionId = game.Player1SessionId,
            Player1Name = game.Player1Session?.User?.Name,
            Player2SessionId = game.Player2SessionId,
            Player2Name = game.GameMode == RPSGameMode.AI ? "IA 🤖" : game.Player2Session?.User?.Name,
            // Ne montrer les choix que si les deux joueurs ont joué ou si c'est le joueur qui regarde
            Player1Choice = (viewingPlayer == 1 || (game.Player1Choice.HasValue && game.Player2Choice.HasValue)) 
                ? game.Player1Choice 
                : null,
            Player2Choice = (viewingPlayer == 2 || (game.Player1Choice.HasValue && game.Player2Choice.HasValue)) 
                ? game.Player2Choice 
                : null,
            RoundNumber = game.RoundNumber,
            Player1Score = game.Player1Score,
            Player2Score = game.Player2Score,
            RoundsToWin = game.RoundsToWin,
            Status = (int)game.Status,
            RoundWinner = game.RoundWinner,
            WinnerPlayerId = game.WinnerPlayerId,
            CreatedAt = game.CreatedAt,
            StartedAt = game.StartedAt,
            CompletedAt = game.CompletedAt,
            ElapsedSeconds = game.ElapsedSeconds,
            GameMode = (int)game.GameMode,
            Player1Wager = game.Player1Wager,
            Player2Wager = game.Player2Wager
        };

        return dto;
    }

    private int GetPlayerNumber(RockPaperScissorsGame game, int sessionId)
    {
        if (game.Player1SessionId == sessionId)
            return 1;
        if (game.Player2SessionId == sessionId)
            return 2;
        return 0;
    }

    private int? DetermineWinner(RPSChoice player1Choice, RPSChoice player2Choice)
    {
        // Égalité
        if (player1Choice == player2Choice)
            return null;

        // Rock bat Scissors, Scissors bat Paper, Paper bat Rock
        if ((player1Choice == RPSChoice.Rock && player2Choice == RPSChoice.Scissors) ||
            (player1Choice == RPSChoice.Scissors && player2Choice == RPSChoice.Paper) ||
            (player1Choice == RPSChoice.Paper && player2Choice == RPSChoice.Rock))
        {
            return 1; // Joueur 1 gagne
        }

        return 2; // Joueur 2 gagne
    }
}

