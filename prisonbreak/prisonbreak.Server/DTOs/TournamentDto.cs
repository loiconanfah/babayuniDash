using prisonbreak.Server.Models;

namespace prisonbreak.Server.DTOs;

/// <summary>
/// DTO pour un tournoi
/// </summary>
public class TournamentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TournamentGameType GameType { get; set; }
    public TournamentStatus Status { get; set; }
    public int MaxParticipants { get; set; }
    public int CurrentParticipants { get; set; }
    public int EntryFee { get; set; }
    public int TotalPrize { get; set; }
    public int FirstPlacePrize { get; set; }
    public int SecondPlacePrize { get; set; }
    public int ThirdPlacePrize { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public string? ImageUrl { get; set; }
    public List<TournamentParticipantDto> Participants { get; set; } = new();
    public List<TournamentMatchDto> Matches { get; set; } = new();
    public bool IsUserRegistered { get; set; }
    public int? UserPosition { get; set; } // Position de l'utilisateur dans le tournoi
    public TournamentParticipantDto? UserParticipant { get; set; } // Informations du participant de l'utilisateur
}

/// <summary>
/// DTO pour un participant à un tournoi
/// </summary>
public class TournamentParticipantDto
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserEmail { get; set; }
    public int? FinalPosition { get; set; }
    public int PrizeWon { get; set; }
    public DateTime JoinedAt { get; set; }
    public bool IsEliminated { get; set; }
}

/// <summary>
/// DTO pour un match de tournoi
/// </summary>
public class TournamentMatchDto
{
    public int Id { get; set; }
    public int TournamentId { get; set; }
    public int Participant1Id { get; set; }
    public string Participant1Name { get; set; } = string.Empty;
    public int Participant2Id { get; set; }
    public string Participant2Name { get; set; } = string.Empty;
    public int Round { get; set; }
    public int? WinnerId { get; set; }
    public TournamentMatchStatus Status { get; set; }
    public int? GameId { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// DTO pour créer un tournoi
/// </summary>
public class CreateTournamentRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public TournamentGameType GameType { get; set; }
    public int MaxParticipants { get; set; } = 16;
    public int EntryFee { get; set; } = 0;
    public DateTime StartDate { get; set; }
    public string? ImageUrl { get; set; }
}

