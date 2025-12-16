using Microsoft.AspNetCore.Mvc;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;
using prisonbreak.Server.Services;

namespace prisonbreak.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RockPaperScissorsController : ControllerBase
{
    private readonly IRockPaperScissorsService _rpsService;
    private readonly ILogger<RockPaperScissorsController> _logger;

    public RockPaperScissorsController(
        IRockPaperScissorsService rpsService,
        ILogger<RockPaperScissorsController> logger)
    {
        _rpsService = rpsService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<RockPaperScissorsGameDto>> CreateGame([FromBody] CreateRPSGameRequest request)
    {
        try
        {
            if (request == null) return BadRequest("La requête ne peut pas être nulle");
            _logger.LogInformation("Création d'une partie RPS : SessionId={SessionId}, GameMode={GameMode}", request.SessionId, request.GameMode);
            var gameMode = (RPSGameMode)request.GameMode;
            var game = await _rpsService.CreateGameAsync(request.SessionId, request.Wager, gameMode, request.Player2SessionId);
            var gameDto = _rpsService.ConvertToDto(game, request.SessionId);
            return CreatedAtAction(nameof(GetGameById), new { id = game.Id }, gameDto);
        }
        catch (ArgumentException ex) { _logger.LogWarning(ex, "Erreur ArgumentException : {Message}", ex.Message); return BadRequest(new { message = ex.Message }); }
        catch (Exception ex) { _logger.LogError(ex, "Erreur inattendue : {Message}", ex.Message); return StatusCode(500, new { message = "Erreur interne du serveur" }); }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<RockPaperScissorsGameDto>> GetGameById(int id, [FromQuery] int? sessionId = null)
    {
        try
        {
            var game = await _rpsService.GetGameByIdAsync(id);
            if (game == null) return NotFound($"La partie avec l'ID {id} n'existe pas");
            var gameDto = _rpsService.ConvertToDto(game, sessionId);
            return Ok(gameDto);
        }
        catch (Exception ex) { _logger.LogError(ex, "Erreur lors de la récupération de la partie {GameId}", id); return StatusCode(500, "Erreur interne du serveur"); }
    }

    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<RockPaperScissorsGameDto>>> GetAvailableGames()
    {
        try
        {
            var games = await _rpsService.GetAvailableGamesAsync();
            var gameDtos = games.Select(g => _rpsService.ConvertToDto(g));
            return Ok(gameDtos);
        }
        catch (Exception ex) { _logger.LogError(ex, "Erreur lors de la récupération des parties disponibles"); return StatusCode(500, "Erreur interne du serveur"); }
    }

    [HttpGet("invitations/{sessionId}")]
    public async Task<ActionResult<IEnumerable<RockPaperScissorsGameDto>>> GetInvitations(int sessionId)
    {
        try
        {
            var games = await _rpsService.GetInvitationsAsync(sessionId);
            var gameDtos = games.Select(g => _rpsService.ConvertToDto(g, sessionId));
            return Ok(gameDtos);
        }
        catch (Exception ex) { _logger.LogError(ex, "Erreur lors de la récupération des invitations pour la session {SessionId}", sessionId); return StatusCode(500, "Erreur interne du serveur"); }
    }

    [HttpPost("{id}/join")]
    public async Task<ActionResult<RockPaperScissorsGameDto>> JoinGame(int id, [FromBody] JoinRPSGameRequest request)
    {
        try
        {
            if (request == null) return BadRequest("La requête ne peut pas être nulle");
            var game = await _rpsService.JoinGameAsync(id, request.SessionId, request.Wager);
            var gameDto = _rpsService.ConvertToDto(game, request.SessionId);
            _logger.LogInformation("Joueur a rejoint la partie : GameId={GameId}, SessionId={SessionId}", id, request.SessionId);
            return Ok(gameDto);
        }
        catch (ArgumentException ex) { return NotFound(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { _logger.LogError(ex, "Erreur lors de la jonction à la partie {GameId}", id); return StatusCode(500, "Erreur interne du serveur"); }
    }

    [HttpPost("{id}/choice")]
    public async Task<ActionResult<RockPaperScissorsGameDto>> PlayChoice(int id, [FromBody] PlayRPSChoiceRequest request)
    {
        try
        {
            if (request == null) return BadRequest("La requête ne peut pas être nulle");
            var choice = (RPSChoice)request.Choice;
            var game = await _rpsService.PlayChoiceAsync(id, request.SessionId, choice);
            var gameDto = _rpsService.ConvertToDto(game, request.SessionId);
            return Ok(gameDto);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
        catch (Exception ex) { _logger.LogError(ex, "Erreur lors du choix pour la partie {GameId}", id); return StatusCode(500, "Erreur interne du serveur"); }
    }

    [HttpPost("{id}/next-round")]
    public async Task<ActionResult<RockPaperScissorsGameDto>> NextRound(int id, [FromBody] JoinRPSGameRequest request)
    {
        try
        {
            if (request == null) return BadRequest("La requête ne peut pas être nulle");
            var game = await _rpsService.NextRoundAsync(id, request.SessionId);
            var gameDto = _rpsService.ConvertToDto(game, request.SessionId);
            return Ok(gameDto);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
        catch (Exception ex) { _logger.LogError(ex, "Erreur lors du passage au round suivant pour la partie {GameId}", id); return StatusCode(500, "Erreur interne du serveur"); }
    }

    [HttpPost("{id}/abandon")]
    public async Task<ActionResult<RockPaperScissorsGameDto>> AbandonGame(int id, [FromBody] JoinRPSGameRequest request)
    {
        try
        {
            if (request == null) return BadRequest("La requête ne peut pas être nulle");
            var game = await _rpsService.AbandonGameAsync(id, request.SessionId);
            var gameDto = _rpsService.ConvertToDto(game, request.SessionId);
            _logger.LogInformation("Partie abandonnée : GameId={GameId}, SessionId={SessionId}", id, request.SessionId);
            return Ok(gameDto);
        }
        catch (ArgumentException ex) { return NotFound(ex.Message); }
        catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
        catch (Exception ex) { _logger.LogError(ex, "Erreur lors de l'abandon de la partie {GameId}", id); return StatusCode(500, "Erreur interne du serveur"); }
    }
}

