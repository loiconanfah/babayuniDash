using Microsoft.EntityFrameworkCore;
using prisonbreak.Server.Data;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Services;

/// <summary>
/// Service pour la gestion des tournois
/// </summary>
public class TournamentService : ITournamentService
{
    private readonly HashiDbContext _context;
    private readonly ILogger<TournamentService>? _logger;
    private readonly INotificationService? _notificationService;

    public TournamentService(
        HashiDbContext context,
        ILogger<TournamentService>? logger = null,
        INotificationService? notificationService = null)
    {
        _context = context;
        _logger = logger;
        _notificationService = notificationService;
    }

    public async Task<List<TournamentDto>> GetAllTournamentsAsync(int? userId = null)
    {
        var tournaments = await _context.Tournaments
            .Include(t => t.Participants)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();

        var result = new List<TournamentDto>();

        foreach (var tournament in tournaments)
        {
            var dto = await ConvertToDtoAsync(tournament, userId);
            result.Add(dto);
        }

        return result;
    }

    public async Task<TournamentDto?> GetTournamentByIdAsync(int tournamentId, int? userId = null)
    {
        var tournament = await _context.Tournaments
            .Include(t => t.Participants)
                .ThenInclude(p => p.User)
            .Include(t => t.Matches)
                .ThenInclude(m => m.Participant1)
                    .ThenInclude(p => p.User)
            .Include(t => t.Matches)
                .ThenInclude(m => m.Participant2)
                    .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(t => t.Id == tournamentId);

        if (tournament == null)
            return null;

        return await ConvertToDtoAsync(tournament, userId);
    }

    public async Task<TournamentDto> CreateTournamentAsync(CreateTournamentRequest request)
    {
        // Calculer les récompenses basées sur la mise d'entrée
        var totalPrize = request.MaxParticipants * request.EntryFee;
        var firstPlacePrize = (int)(totalPrize * 0.5); // 50%
        var secondPlacePrize = (int)(totalPrize * 0.3); // 30%
        var thirdPlacePrize = (int)(totalPrize * 0.2); // 20%

        var tournament = new Tournament
        {
            Name = request.Name,
            Description = request.Description,
            GameType = request.GameType,
            MaxParticipants = request.MaxParticipants,
            EntryFee = request.EntryFee,
            TotalPrize = totalPrize,
            FirstPlacePrize = firstPlacePrize,
            SecondPlacePrize = secondPlacePrize,
            ThirdPlacePrize = thirdPlacePrize,
            StartDate = request.StartDate,
            Status = TournamentStatus.Registration,
            CreatedAt = DateTime.UtcNow,
            ImageUrl = request.ImageUrl
        };

        _context.Tournaments.Add(tournament);
        await _context.SaveChangesAsync();

        _logger?.LogInformation("Tournoi créé: {TournamentId} - {Name}", tournament.Id, tournament.Name);

        return await ConvertToDtoAsync(tournament, null);
    }

    public async Task<TournamentParticipantDto> RegisterUserAsync(int tournamentId, int userId)
    {
        var tournament = await _context.Tournaments
            .Include(t => t.Participants)
            .FirstOrDefaultAsync(t => t.Id == tournamentId);

        if (tournament == null)
            throw new ArgumentException("Tournoi introuvable");

        if (tournament.Status != TournamentStatus.Registration)
            throw new InvalidOperationException("Les inscriptions sont fermées pour ce tournoi");

        if (tournament.CurrentParticipants >= tournament.MaxParticipants)
            throw new InvalidOperationException("Le tournoi est complet");

        // Vérifier si l'utilisateur est déjà inscrit
        if (tournament.Participants.Any(p => p.UserId == userId))
            throw new InvalidOperationException("Vous êtes déjà inscrit à ce tournoi");

        // Vérifier et déduire la mise d'entrée
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new ArgumentException("Utilisateur introuvable");

        if (tournament.EntryFee > 0)
        {
            if (user.Coins < tournament.EntryFee)
                throw new InvalidOperationException("Vous n'avez pas assez de Babayuni pour vous inscrire");

            user.Coins -= tournament.EntryFee;
            await _context.SaveChangesAsync();
        }

        var participant = new TournamentParticipant
        {
            TournamentId = tournamentId,
            UserId = userId,
            JoinedAt = DateTime.UtcNow
        };

        _context.TournamentParticipants.Add(participant);
        tournament.CurrentParticipants++;
        await _context.SaveChangesAsync();

        _logger?.LogInformation("Utilisateur {UserId} inscrit au tournoi {TournamentId}", userId, tournamentId);

        var participantDto = await ConvertParticipantToDtoAsync(participant);
        return participantDto;
    }

    public async Task<bool> UnregisterUserAsync(int tournamentId, int userId)
    {
        var participant = await _context.TournamentParticipants
            .Include(p => p.Tournament)
            .FirstOrDefaultAsync(p => p.TournamentId == tournamentId && p.UserId == userId);

        if (participant == null)
            return false;

        if (participant.Tournament.Status != TournamentStatus.Registration)
            throw new InvalidOperationException("Impossible de se désinscrire, le tournoi a déjà commencé");

        // Rembourser la mise d'entrée
        if (participant.Tournament.EntryFee > 0)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                user.Coins += participant.Tournament.EntryFee;
            }
        }

        participant.Tournament.CurrentParticipants--;
        _context.TournamentParticipants.Remove(participant);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<TournamentDto> StartTournamentAsync(int tournamentId)
    {
        var tournament = await _context.Tournaments
            .Include(t => t.Participants)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(t => t.Id == tournamentId);

        if (tournament == null)
            throw new ArgumentException("Tournoi introuvable");

        if (tournament.Status != TournamentStatus.Registration)
            throw new InvalidOperationException("Le tournoi ne peut pas être démarré dans son état actuel");

        if (tournament.CurrentParticipants < 2)
            throw new InvalidOperationException("Il faut au moins 2 participants pour démarrer un tournoi");

        tournament.Status = TournamentStatus.InProgress;
        tournament.StartDate = DateTime.UtcNow;

        // Générer les matchs du premier round (format élimination directe)
        var participants = tournament.Participants.ToList();
        var shuffled = participants.OrderBy(x => Guid.NewGuid()).ToList();

        int round = 1;
        for (int i = 0; i < shuffled.Count; i += 2)
        {
            if (i + 1 < shuffled.Count)
            {
                var match = new TournamentMatch
                {
                    TournamentId = tournamentId,
                    Participant1Id = shuffled[i].Id,
                    Participant2Id = shuffled[i + 1].Id,
                    Round = round,
                    Status = TournamentMatchStatus.Pending
                };
                _context.TournamentMatches.Add(match);
            }
            else
            {
                // Participant avec bye (passe automatiquement au round suivant)
                // Pour l'instant, on ne gère pas les byes, donc on nécessite un nombre pair
            }
        }

        await _context.SaveChangesAsync();

        // Envoyer des notifications à tous les participants
        if (_notificationService != null)
        {
            foreach (var participant in participants)
            {
                await _notificationService.CreateNotificationAsync(
                    participant.UserId,
                    NotificationType.TournamentStarted,
                    "🏆 Tournoi commencé !",
                    $"Le tournoi '{tournament.Name}' a commencé. Bonne chance !",
                    System.Text.Json.JsonSerializer.Serialize(new { tournamentId = tournamentId })
                );
            }
        }

        _logger?.LogInformation("Tournoi {TournamentId} démarré avec {Count} participants", tournamentId, participants.Count);

        return await ConvertToDtoAsync(tournament, null);
    }

    public async Task<TournamentMatchDto> RecordMatchResultAsync(int matchId, int winnerId)
    {
        var match = await _context.TournamentMatches
            .Include(m => m.Participant1)
                .ThenInclude(p => p.User)
            .Include(m => m.Participant2)
                .ThenInclude(p => p.User)
            .Include(m => m.Tournament)
            .FirstOrDefaultAsync(m => m.Id == matchId);

        if (match == null)
            throw new ArgumentException("Match introuvable");

        if (match.Status == TournamentMatchStatus.Completed)
            throw new InvalidOperationException("Ce match est déjà terminé");

        if (winnerId != match.Participant1Id && winnerId != match.Participant2Id)
            throw new ArgumentException("Le gagnant doit être l'un des participants");

        match.WinnerId = winnerId;
        match.Status = TournamentMatchStatus.Completed;
        match.CompletedAt = DateTime.UtcNow;

        // Marquer le perdant comme éliminé
        var loserId = winnerId == match.Participant1Id ? match.Participant2Id : match.Participant1Id;
        var loser = await _context.TournamentParticipants.FindAsync(loserId);
        if (loser != null)
        {
            loser.IsEliminated = true;
        }

        await _context.SaveChangesAsync();

        // Vérifier si c'est le dernier match du round et générer le round suivant
        await GenerateNextRoundAsync(match.TournamentId, match.Round);

        return await ConvertMatchToDtoAsync(match);
    }

    private async Task GenerateNextRoundAsync(int tournamentId, int currentRound)
    {
        var tournament = await _context.Tournaments
            .Include(t => t.Matches)
            .Include(t => t.Participants)
            .FirstOrDefaultAsync(t => t.Id == tournamentId);

        if (tournament == null) return;

        // Vérifier si tous les matchs du round actuel sont terminés
        var currentRoundMatches = tournament.Matches.Where(m => m.Round == currentRound).ToList();
        if (currentRoundMatches.Any(m => m.Status != TournamentMatchStatus.Completed))
            return; // Pas encore prêt pour le round suivant

        // Récupérer les gagnants du round actuel
        var winners = currentRoundMatches
            .Where(m => m.WinnerId.HasValue)
            .Select(m => m.WinnerId!.Value)
            .ToList();

        if (winners.Count < 2)
        {
            // Tournoi terminé, attribuer les récompenses
            await FinalizeTournamentAsync(tournamentId, winners.FirstOrDefault());
            return;
        }

        // Générer les matchs du round suivant
        int nextRound = currentRound + 1;
        for (int i = 0; i < winners.Count; i += 2)
        {
            if (i + 1 < winners.Count)
            {
                var participant1 = await _context.TournamentParticipants
                    .FirstOrDefaultAsync(p => p.Id == winners[i]);
                var participant2 = await _context.TournamentParticipants
                    .FirstOrDefaultAsync(p => p.Id == winners[i + 1]);

                if (participant1 != null && participant2 != null)
                {
                    var match = new TournamentMatch
                    {
                        TournamentId = tournamentId,
                        Participant1Id = participant1.Id,
                        Participant2Id = participant2.Id,
                        Round = nextRound,
                        Status = TournamentMatchStatus.Pending
                    };
                    _context.TournamentMatches.Add(match);
                }
            }
        }

        await _context.SaveChangesAsync();
    }

    private async Task FinalizeTournamentAsync(int tournamentId, int? winnerId)
    {
        var tournament = await _context.Tournaments
            .Include(t => t.Participants)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(t => t.Id == tournamentId);

        if (tournament == null) return;

        tournament.Status = TournamentStatus.Completed;
        tournament.EndDate = DateTime.UtcNow;

        // Attribuer les récompenses
        var participants = tournament.Participants
            .OrderByDescending(p => p.IsEliminated ? 0 : 1)
            .ThenBy(p => p.FinalPosition ?? int.MaxValue)
            .ToList();

        if (participants.Count > 0 && winnerId.HasValue)
        {
            var winner = participants.FirstOrDefault(p => p.Id == winnerId.Value);
            if (winner != null)
            {
                winner.FinalPosition = 1;
                winner.PrizeWon = tournament.FirstPlacePrize;
                winner.User.Coins += tournament.FirstPlacePrize;
            }
        }

        if (participants.Count > 1)
        {
            var second = participants.Skip(1).FirstOrDefault();
            if (second != null && !second.IsEliminated)
            {
                second.FinalPosition = 2;
                second.PrizeWon = tournament.SecondPlacePrize;
                second.User.Coins += tournament.SecondPlacePrize;
            }
        }

        if (participants.Count > 2)
        {
            var third = participants.Skip(2).FirstOrDefault();
            if (third != null && !third.IsEliminated)
            {
                third.FinalPosition = 3;
                third.PrizeWon = tournament.ThirdPlacePrize;
                third.User.Coins += tournament.ThirdPlacePrize;
            }
        }

        await _context.SaveChangesAsync();

        // Envoyer des notifications
        if (_notificationService != null)
        {
            foreach (var participant in participants.Where(p => p.PrizeWon > 0))
            {
                await _notificationService.CreateNotificationAsync(
                    participant.UserId,
                    NotificationType.TournamentEnded,
                    "🏆 Tournoi terminé !",
                    $"Vous avez terminé {GetPositionText(participant.FinalPosition ?? 0)} dans le tournoi '{tournament.Name}' et avez gagné {participant.PrizeWon} Babayuni !",
                    System.Text.Json.JsonSerializer.Serialize(new { tournamentId = tournamentId, position = participant.FinalPosition, prize = participant.PrizeWon })
                );
            }
        }

        _logger?.LogInformation("Tournoi {TournamentId} terminé", tournamentId);
    }

    private static string GetPositionText(int position)
    {
        return position switch
        {
            1 => "1er",
            2 => "2ème",
            3 => "3ème",
            _ => $"{position}ème"
        };
    }

    public async Task<List<TournamentMatchDto>> GetTournamentMatchesAsync(int tournamentId)
    {
        var matches = await _context.TournamentMatches
            .Include(m => m.Participant1)
                .ThenInclude(p => p.User)
            .Include(m => m.Participant2)
                .ThenInclude(p => p.User)
            .Where(m => m.TournamentId == tournamentId)
            .OrderBy(m => m.Round)
            .ThenBy(m => m.Id)
            .ToListAsync();

        var result = new List<TournamentMatchDto>();
        foreach (var match in matches)
        {
            result.Add(await ConvertMatchToDtoAsync(match));
        }

        return result;
    }

    public async Task<List<TournamentMatchDto>> GetUserMatchesAsync(int tournamentId, int userId)
    {
        var participant = await _context.TournamentParticipants
            .FirstOrDefaultAsync(p => p.TournamentId == tournamentId && p.UserId == userId);

        if (participant == null)
            return new List<TournamentMatchDto>();

        var matches = await _context.TournamentMatches
            .Include(m => m.Participant1)
                .ThenInclude(p => p.User)
            .Include(m => m.Participant2)
                .ThenInclude(p => p.User)
            .Where(m => m.TournamentId == tournamentId && 
                       (m.Participant1Id == participant.Id || m.Participant2Id == participant.Id))
            .OrderBy(m => m.Round)
            .ToListAsync();

        var result = new List<TournamentMatchDto>();
        foreach (var match in matches)
        {
            result.Add(await ConvertMatchToDtoAsync(match));
        }

        return result;
    }

    private async Task<TournamentDto> ConvertToDtoAsync(Tournament tournament, int? userId)
    {
        await _context.Entry(tournament).Collection(t => t.Participants).LoadAsync();
        await _context.Entry(tournament).Collection(t => t.Matches).LoadAsync();

        var userParticipant = userId.HasValue 
            ? tournament.Participants.FirstOrDefault(p => p.UserId == userId.Value)
            : null;

        var dto = new TournamentDto
        {
            Id = tournament.Id,
            Name = tournament.Name,
            Description = tournament.Description,
            GameType = tournament.GameType,
            Status = tournament.Status,
            MaxParticipants = tournament.MaxParticipants,
            CurrentParticipants = tournament.CurrentParticipants,
            EntryFee = tournament.EntryFee,
            TotalPrize = tournament.TotalPrize,
            FirstPlacePrize = tournament.FirstPlacePrize,
            SecondPlacePrize = tournament.SecondPlacePrize,
            ThirdPlacePrize = tournament.ThirdPlacePrize,
            StartDate = tournament.StartDate,
            EndDate = tournament.EndDate,
            CreatedAt = tournament.CreatedAt,
            ImageUrl = tournament.ImageUrl,
            IsUserRegistered = userParticipant != null,
            UserPosition = userParticipant?.FinalPosition,
            UserParticipant = userParticipant != null ? await ConvertParticipantToDtoAsync(userParticipant) : null
        };

        foreach (var participant in tournament.Participants)
        {
            dto.Participants.Add(await ConvertParticipantToDtoAsync(participant));
        }

        foreach (var match in tournament.Matches)
        {
            dto.Matches.Add(await ConvertMatchToDtoAsync(match));
        }

        return dto;
    }

    private async Task<TournamentParticipantDto> ConvertParticipantToDtoAsync(TournamentParticipant participant)
    {
        await _context.Entry(participant).Reference(p => p.User).LoadAsync();

        return new TournamentParticipantDto
        {
            Id = participant.Id,
            TournamentId = participant.TournamentId,
            UserId = participant.UserId,
            UserName = participant.User.Name,
            UserEmail = participant.User.Email,
            FinalPosition = participant.FinalPosition,
            PrizeWon = participant.PrizeWon,
            JoinedAt = participant.JoinedAt,
            IsEliminated = participant.IsEliminated
        };
    }

    private async Task<TournamentMatchDto> ConvertMatchToDtoAsync(TournamentMatch match)
    {
        await _context.Entry(match).Reference(m => m.Participant1).LoadAsync();
        await _context.Entry(match).Reference(m => m.Participant2).LoadAsync();
        await _context.Entry(match.Participant1).Reference(p => p.User).LoadAsync();
        await _context.Entry(match.Participant2).Reference(p => p.User).LoadAsync();

        return new TournamentMatchDto
        {
            Id = match.Id,
            TournamentId = match.TournamentId,
            Participant1Id = match.Participant1Id,
            Participant1Name = match.Participant1.User.Name,
            Participant2Id = match.Participant2Id,
            Participant2Name = match.Participant2.User.Name,
            Round = match.Round,
            WinnerId = match.WinnerId,
            Status = match.Status,
            GameId = match.GameId,
            StartedAt = match.StartedAt,
            CompletedAt = match.CompletedAt
        };
    }
}

