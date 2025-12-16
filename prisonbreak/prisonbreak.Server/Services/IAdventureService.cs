using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Services;

public interface IAdventureService
{
    Task<AdventureGame> CreateGameAsync(int sessionId);
    Task<AdventureGame?> GetGameByIdAsync(int gameId);
    Task<IEnumerable<AdventureGame>> GetGamesBySessionAsync(int sessionId);
    Task<AdventureGame> MoveToRoomAsync(int gameId, int sessionId, int roomNumber);
    Task<AdventureGame> CollectItemAsync(int gameId, int sessionId, string itemName);
    Task<AdventureGame> SolvePuzzleAsync(int gameId, int sessionId, int puzzleId, string answer);
    Task<AdventureGame> AbandonGameAsync(int gameId, int sessionId);
    Task<PuzzleInfoDto> GetPuzzleInfoAsync(int puzzleId);
    AdventureGameDto ConvertToDto(AdventureGame game);
}

