using Microsoft.AspNetCore.Mvc;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Services;

namespace prisonbreak.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdventureController : ControllerBase
{
    private readonly IAdventureService _adventureService;
    private readonly ILogger<AdventureController> _logger;

    public AdventureController(
        IAdventureService adventureService,
        ILogger<AdventureController> logger)
    {
        _adventureService = adventureService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<ActionResult<AdventureGameDto>> CreateGame([FromBody] CreateAdventureGameRequest request)
    {
        try
        {
            if (request == null) return BadRequest("La requête ne peut pas être nulle");
            _logger.LogInformation("Création d'une partie d'aventure : SessionId={SessionId}", request.SessionId);
            var game = await _adventureService.CreateGameAsync(request.SessionId);
            var gameDto = _adventureService.ConvertToDto(game);
            return CreatedAtAction(nameof(GetGameById), new { id = game.Id }, gameDto);
        }
        catch (ArgumentException ex) { _logger.LogWarning(ex, "Erreur ArgumentException : {Message}", ex.Message); return BadRequest(new { message = ex.Message }); }
        catch (Exception ex) { _logger.LogError(ex, "Erreur inattendue : {Message}", ex.Message); return StatusCode(500, new { message = "Erreur interne du serveur" }); }
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AdventureGameDto>> GetGameById(int id)
    {
        try
        {
            var game = await _adventureService.GetGameByIdAsync(id);
            if (game == null) return NotFound($"La partie avec l'ID {id} n'existe pas");
            var gameDto = _adventureService.ConvertToDto(game);
            return Ok(gameDto);
        }
        catch (Exception ex) { _logger.LogError(ex, "Erreur lors de la récupération de la partie {GameId}", id); return StatusCode(500, "Erreur interne du serveur"); }
    }

    [HttpGet("session/{sessionId}")]
    public async Task<ActionResult<IEnumerable<AdventureGameDto>>> GetGamesBySession(int sessionId)
    {
        try
        {
            var games = await _adventureService.GetGamesBySessionAsync(sessionId);
            var gameDtos = games.Select(g => _adventureService.ConvertToDto(g));
            return Ok(gameDtos);
        }
        catch (Exception ex) { _logger.LogError(ex, "Erreur lors de la récupération des parties pour la session {SessionId}", sessionId); return StatusCode(500, "Erreur interne du serveur"); }
    }

    [HttpPost("{id}/move")]
    public async Task<ActionResult<AdventureGameDto>> MoveToRoom(int id, [FromBody] MoveToRoomRequest request)
    {
        try
        {
            if (request == null) return BadRequest("La requête ne peut pas être nulle");
            var game = await _adventureService.MoveToRoomAsync(id, request.SessionId, request.RoomNumber);
            var gameDto = _adventureService.ConvertToDto(game);
            return Ok(gameDto);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
        catch (Exception ex) { _logger.LogError(ex, "Erreur lors du déplacement pour la partie {GameId}", id); return StatusCode(500, "Erreur interne du serveur"); }
    }

    [HttpPost("{id}/collect")]
    public async Task<ActionResult<AdventureGameDto>> CollectItem(int id, [FromBody] CollectItemRequest request)
    {
        try
        {
            if (request == null) return BadRequest("La requête ne peut pas être nulle");
            var game = await _adventureService.CollectItemAsync(id, request.SessionId, request.ItemName);
            var gameDto = _adventureService.ConvertToDto(game);
            return Ok(gameDto);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
        catch (Exception ex) { _logger.LogError(ex, "Erreur lors de la collecte pour la partie {GameId}", id); return StatusCode(500, "Erreur interne du serveur"); }
    }

    [HttpPost("{id}/solve")]
    public async Task<ActionResult<AdventureGameDto>> SolvePuzzle(int id, [FromBody] SolvePuzzleRequest request)
    {
        try
        {
            if (request == null) return BadRequest("La requête ne peut pas être nulle");
            var game = await _adventureService.SolvePuzzleAsync(id, request.SessionId, request.PuzzleId, request.Answer);
            var gameDto = _adventureService.ConvertToDto(game);
            return Ok(gameDto);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (InvalidOperationException ex) { return BadRequest(ex.Message); }
        catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
        catch (Exception ex) { _logger.LogError(ex, "Erreur lors de la résolution pour la partie {GameId}", id); return StatusCode(500, "Erreur interne du serveur"); }
    }

    [HttpGet("puzzle/{puzzleId}")]
    public async Task<ActionResult<PuzzleInfoDto>> GetPuzzleInfo(int puzzleId)
    {
        try
        {
            var puzzleInfo = await _adventureService.GetPuzzleInfoAsync(puzzleId);
            return Ok(puzzleInfo);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
        catch (Exception ex) { _logger.LogError(ex, "Erreur lors de la récupération de l'énigme {PuzzleId}", puzzleId); return StatusCode(500, "Erreur interne du serveur"); }
    }

    [HttpPost("{id}/abandon")]
    public async Task<ActionResult<AdventureGameDto>> AbandonGame(int id, [FromBody] CreateAdventureGameRequest request)
    {
        try
        {
            if (request == null) return BadRequest("La requête ne peut pas être nulle");
            var game = await _adventureService.AbandonGameAsync(id, request.SessionId);
            var gameDto = _adventureService.ConvertToDto(game);
            _logger.LogInformation("Partie abandonnée : GameId={GameId}, SessionId={SessionId}", id, request.SessionId);
            return Ok(gameDto);
        }
        catch (ArgumentException ex) { return NotFound(ex.Message); }
        catch (UnauthorizedAccessException ex) { return Unauthorized(ex.Message); }
        catch (Exception ex) { _logger.LogError(ex, "Erreur lors de l'abandon de la partie {GameId}", id); return StatusCode(500, "Erreur interne du serveur"); }
    }
}

