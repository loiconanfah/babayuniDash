using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Services;

public interface IRockPaperScissorsService
{
    Task<RockPaperScissorsGame> CreateGameAsync(int sessionId, int wager, RPSGameMode gameMode = RPSGameMode.Player, int? player2SessionId = null);
    Task<RockPaperScissorsGame?> GetGameByIdAsync(int gameId);
    Task<IEnumerable<RockPaperScissorsGame>> GetAvailableGamesAsync();
    Task<IEnumerable<RockPaperScissorsGame>> GetInvitationsAsync(int sessionId);
    Task<RockPaperScissorsGame> JoinGameAsync(int gameId, int sessionId, int wager = 0);
    Task<RockPaperScissorsGame> PlayChoiceAsync(int gameId, int sessionId, RPSChoice choice);
    Task<RockPaperScissorsGame> NextRoundAsync(int gameId, int sessionId);
    Task<RockPaperScissorsGame> AbandonGameAsync(int gameId, int sessionId);
    RockPaperScissorsGameDto ConvertToDto(RockPaperScissorsGame game, int? viewingPlayerSessionId = null);
}

