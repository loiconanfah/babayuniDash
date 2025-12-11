namespace prisonbreak.Server.DTOs;

/// <summary>
/// DTO pour une partie d'aventure
/// </summary>
public class AdventureGameDto
{
    public int Id { get; set; }
    public int PlayerSessionId { get; set; }
    public string? PlayerName { get; set; }
    public int CurrentRoom { get; set; }
    public List<string> CollectedItems { get; set; } = new();
    public List<int> SolvedPuzzles { get; set; } = new();
    public int Score { get; set; }
    public int ElapsedSeconds { get; set; }
    public int PuzzlesSolved { get; set; }
    public int Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class CreateAdventureGameRequest
{
    public int SessionId { get; set; }
}

public class MoveToRoomRequest
{
    public int SessionId { get; set; }
    public int RoomNumber { get; set; }
}

public class CollectItemRequest
{
    public int SessionId { get; set; }
    public string ItemName { get; set; } = string.Empty;
}

public class SolvePuzzleRequest
{
    public int SessionId { get; set; }
    public int PuzzleId { get; set; }
    public string Answer { get; set; } = string.Empty;
}

public class GetPuzzleHintsRequest
{
    public int PuzzleId { get; set; }
}

public class PuzzleInfoDto
{
    public int Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public string? MiniGameType { get; set; }
    public List<string> Hints { get; set; } = new();
    public int Points { get; set; }
}

