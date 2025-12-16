namespace prisonbreak.Server.DTOs;

/// <summary>
/// DTO pour une partie de Rock Paper Scissors
/// </summary>
public class RockPaperScissorsGameDto
{
    public int Id { get; set; }
    public int Player1SessionId { get; set; }
    public string? Player1Name { get; set; }
    public int? Player2SessionId { get; set; }
    public string? Player2Name { get; set; }
    public int? Player1Choice { get; set; } // 1=Rock, 2=Paper, 3=Scissors
    public int? Player2Choice { get; set; }
    public int RoundNumber { get; set; }
    public int Player1Score { get; set; }
    public int Player2Score { get; set; }
    public int RoundsToWin { get; set; }
    public int Status { get; set; }
    public int? RoundWinner { get; set; }
    public int? WinnerPlayerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int ElapsedSeconds { get; set; }
    public int GameMode { get; set; }
    public int Player1Wager { get; set; }
    public int Player2Wager { get; set; }
    public int TotalWager => Player1Wager + Player2Wager;
}

public class CreateRPSGameRequest
{
    public int SessionId { get; set; }
    public int GameMode { get; set; } = 1;
    public int? Player2SessionId { get; set; }
    public int Wager { get; set; } = 0;
}

public class JoinRPSGameRequest
{
    public int GameId { get; set; }
    public int SessionId { get; set; }
    public int Wager { get; set; } = 0;
}

public class PlayRPSChoiceRequest
{
    public int SessionId { get; set; }
    public int Choice { get; set; } // 1=Rock, 2=Paper, 3=Scissors
}

