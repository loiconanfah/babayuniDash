using Microsoft.AspNetCore.Mvc;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Services;

namespace prisonbreak.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TournamentsController : ControllerBase
{
    private readonly ITournamentService _tournamentService;
    private readonly ILogger<TournamentsController> _logger;

    public TournamentsController(ITournamentService tournamentService, ILogger<TournamentsController> logger)
    {
        _tournamentService = tournamentService;
        _logger = logger;
    }

    /// <summary>
    /// GET api/tournaments
    /// Récupère tous les tournois
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<TournamentDto>>> GetAllTournaments([FromQuery] int? userId = null)
    {
        try
        {
            var tournaments = await _tournamentService.GetAllTournamentsAsync(userId);
            return Ok(tournaments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des tournois");
            return StatusCode(500, "Erreur serveur");
        }
    }

    /// <summary>
    /// GET api/tournaments/{id}
    /// Récupère un tournoi par son ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<TournamentDto>> GetTournament(int id, [FromQuery] int? userId = null)
    {
        try
        {
            var tournament = await _tournamentService.GetTournamentByIdAsync(id, userId);
            if (tournament == null)
                return NotFound("Tournoi introuvable");

            return Ok(tournament);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du tournoi {TournamentId}", id);
            return StatusCode(500, "Erreur serveur");
        }
    }

    /// <summary>
    /// POST api/tournaments
    /// Crée un nouveau tournoi
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<TournamentDto>> CreateTournament([FromBody] CreateTournamentRequest request)
    {
        try
        {
            var tournament = await _tournamentService.CreateTournamentAsync(request);
            return CreatedAtAction(nameof(GetTournament), new { id = tournament.Id }, tournament);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création du tournoi");
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// POST api/tournaments/{id}/register
    /// Inscrit un utilisateur à un tournoi
    /// </summary>
    [HttpPost("{id}/register")]
    public async Task<ActionResult<TournamentParticipantDto>> Register(int id, [FromBody] RegisterTournamentRequest request)
    {
        try
        {
            var participant = await _tournamentService.RegisterUserAsync(id, request.UserId);
            return Ok(participant);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'inscription au tournoi {TournamentId}", id);
            return StatusCode(500, "Erreur serveur");
        }
    }

    /// <summary>
    /// POST api/tournaments/{id}/unregister
    /// Désinscrit un utilisateur d'un tournoi
    /// </summary>
    [HttpPost("{id}/unregister")]
    public async Task<ActionResult> Unregister(int id, [FromBody] UnregisterTournamentRequest request)
    {
        try
        {
            var result = await _tournamentService.UnregisterUserAsync(id, request.UserId);
            if (!result)
                return NotFound("Inscription introuvable");

            return Ok(new { message = "Désinscription réussie" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la désinscription du tournoi {TournamentId}", id);
            return StatusCode(500, "Erreur serveur");
        }
    }

    /// <summary>
    /// POST api/tournaments/{id}/start
    /// Démarre un tournoi
    /// </summary>
    [HttpPost("{id}/start")]
    public async Task<ActionResult<TournamentDto>> StartTournament(int id)
    {
        try
        {
            var tournament = await _tournamentService.StartTournamentAsync(id);
            return Ok(tournament);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du démarrage du tournoi {TournamentId}", id);
            return StatusCode(500, "Erreur serveur");
        }
    }

    /// <summary>
    /// GET api/tournaments/{id}/matches
    /// Récupère les matchs d'un tournoi
    /// </summary>
    [HttpGet("{id}/matches")]
    public async Task<ActionResult<List<TournamentMatchDto>>> GetMatches(int id)
    {
        try
        {
            var matches = await _tournamentService.GetTournamentMatchesAsync(id);
            return Ok(matches);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des matchs du tournoi {TournamentId}", id);
            return StatusCode(500, "Erreur serveur");
        }
    }

    /// <summary>
    /// GET api/tournaments/{id}/matches/user/{userId}
    /// Récupère les matchs d'un utilisateur dans un tournoi
    /// </summary>
    [HttpGet("{id}/matches/user/{userId}")]
    public async Task<ActionResult<List<TournamentMatchDto>>> GetUserMatches(int id, int userId)
    {
        try
        {
            var matches = await _tournamentService.GetUserMatchesAsync(id, userId);
            return Ok(matches);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des matchs de l'utilisateur {UserId} dans le tournoi {TournamentId}", userId, id);
            return StatusCode(500, "Erreur serveur");
        }
    }

    /// <summary>
    /// POST api/tournaments/matches/{matchId}/result
    /// Enregistre le résultat d'un match
    /// </summary>
    [HttpPost("matches/{matchId}/result")]
    public async Task<ActionResult<TournamentMatchDto>> RecordMatchResult(int matchId, [FromBody] RecordMatchResultRequest request)
    {
        try
        {
            var match = await _tournamentService.RecordMatchResultAsync(matchId, request.WinnerId);
            return Ok(match);
        }
        catch (ArgumentException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'enregistrement du résultat du match {MatchId}", matchId);
            return StatusCode(500, "Erreur serveur");
        }
    }
}

/// <summary>
/// Requête pour s'inscrire à un tournoi
/// </summary>
public class RegisterTournamentRequest
{
    public int UserId { get; set; }
}

/// <summary>
/// Requête pour se désinscrire d'un tournoi
/// </summary>
public class UnregisterTournamentRequest
{
    public int UserId { get; set; }
}

/// <summary>
/// Requête pour enregistrer le résultat d'un match
/// </summary>
public class RecordMatchResultRequest
{
    public int WinnerId { get; set; }
}


