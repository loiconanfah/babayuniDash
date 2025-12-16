using Microsoft.AspNetCore.Mvc;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;
using prisonbreak.Server.Services;

namespace prisonbreak.Server.Controllers;

/// <summary>
/// Contrôleur API pour la gestion des parties de Tic-Tac-Toe
/// Gère le cycle de vie d'une partie multijoueur : création, rejoindre, jouer, abandon
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TicTacToeController : ControllerBase
{
    private readonly ITicTacToeService _ticTacToeService;
    private readonly ILogger<TicTacToeController> _logger;

    public TicTacToeController(
        ITicTacToeService ticTacToeService,
        ILogger<TicTacToeController> logger)
    {
        _ticTacToeService = ticTacToeService;
        _logger = logger;
    }

    /// <summary>
    /// POST api/tic-tac-toe
    /// Crée une nouvelle partie de Tic-Tac-Toe
    /// </summary>
    /// <param name="request">Requête contenant l'ID de la session</param>
    /// <returns>La partie créée</returns>
    [HttpPost]
    public async Task<ActionResult<TicTacToeGameDto>> CreateGame([FromBody] CreateTicTacToeGameRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest("La requête ne peut pas être nulle");
            }

            _logger.LogInformation("Création d'une partie Tic-Tac-Toe : SessionId={SessionId}, GameMode={GameMode}", 
                request.SessionId, request.GameMode);

            var gameMode = (TicTacToeGameMode)request.GameMode;
            var game = await _ticTacToeService.CreateGameAsync(request.SessionId, gameMode, request.Player2SessionId, request.Wager);
            var gameDto = _ticTacToeService.ConvertToDto(game);

            return CreatedAtAction(nameof(GetGameById), new { id = game.Id }, gameDto);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erreur ArgumentException lors de la création de la partie : {Message}", ex.Message);
            return BadRequest(new { message = ex.Message, error = "ArgumentException" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur inattendue lors de la création de la partie : {Message}", ex.Message);
            return StatusCode(500, new { message = "Erreur interne du serveur", error = ex.GetType().Name, details = ex.Message });
        }
    }

    /// <summary>
    /// GET api/tic-tac-toe/{id}
    /// Récupère une partie par son ID
    /// </summary>
    /// <param name="id">ID de la partie</param>
    /// <returns>La partie demandée</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<TicTacToeGameDto>> GetGameById(int id)
    {
        try
        {
            var game = await _ticTacToeService.GetGameByIdAsync(id);

            if (game == null)
            {
                return NotFound($"La partie avec l'ID {id} n'existe pas");
            }

            var gameDto = _ticTacToeService.ConvertToDto(game);
            return Ok(gameDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de la partie {GameId}", id);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// GET api/tic-tac-toe/available
    /// Récupère les parties en attente d'un deuxième joueur
    /// </summary>
    /// <returns>Liste des parties disponibles</returns>
    [HttpGet("available")]
    public async Task<ActionResult<IEnumerable<TicTacToeGameDto>>> GetAvailableGames()
    {
        try
        {
            var games = await _ticTacToeService.GetAvailableGamesAsync();
            var gameDtos = games.Select(g => _ticTacToeService.ConvertToDto(g));
            return Ok(gameDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des parties disponibles");
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// GET api/tic-tac-toe/invitations/{sessionId}
    /// Récupère les parties où le joueur est invité
    /// </summary>
    /// <param name="sessionId">ID de la session du joueur</param>
    /// <returns>Liste des parties où le joueur est invité</returns>
    [HttpGet("invitations/{sessionId}")]
    public async Task<ActionResult<IEnumerable<TicTacToeGameDto>>> GetInvitations(int sessionId)
    {
        try
        {
            var games = await _ticTacToeService.GetInvitationsAsync(sessionId);
            var gameDtos = games.Select(g => _ticTacToeService.ConvertToDto(g));
            return Ok(gameDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des invitations pour la session {SessionId}", sessionId);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// POST api/tic-tac-toe/{id}/join
    /// Rejoint une partie existante
    /// </summary>
    /// <param name="id">ID de la partie</param>
    /// <param name="request">Requête contenant l'ID de la session</param>
    /// <returns>La partie mise à jour</returns>
    [HttpPost("{id}/join")]
    public async Task<ActionResult<TicTacToeGameDto>> JoinGame(int id, [FromBody] JoinTicTacToeGameRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest("La requête ne peut pas être nulle");
            }

            var game = await _ticTacToeService.JoinGameAsync(id, request.SessionId, request.Wager);
            var gameDto = _ticTacToeService.ConvertToDto(game);

            _logger.LogInformation("Joueur a rejoint la partie : GameId={GameId}, SessionId={SessionId}", id, request.SessionId);

            return Ok(gameDto);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la jonction à la partie {GameId}", id);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// POST api/tic-tac-toe/{id}/move
    /// Joue un coup dans la partie
    /// </summary>
    /// <param name="id">ID de la partie</param>
    /// <param name="request">Requête contenant la position et l'ID de la session</param>
    /// <returns>La partie mise à jour</returns>
    [HttpPost("{id}/move")]
    public async Task<ActionResult<TicTacToeGameDto>> PlayMove(int id, [FromBody] PlayMoveRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest("La requête ne peut pas être nulle");
            }

            var game = await _ticTacToeService.PlayMoveAsync(id, request.Position, request.SessionId);
            var gameDto = _ticTacToeService.ConvertToDto(game);

            return Ok(gameDto);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du coup joué pour la partie {GameId}", id);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// POST api/tic-tac-toe/{id}/abandon
    /// Abandonne une partie
    /// </summary>
    /// <param name="id">ID de la partie</param>
    /// <param name="request">Requête contenant l'ID de la session</param>
    /// <returns>La partie mise à jour</returns>
    [HttpPost("{id}/abandon")]
    public async Task<ActionResult<TicTacToeGameDto>> AbandonGame(int id, [FromBody] JoinTicTacToeGameRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest("La requête ne peut pas être nulle");
            }

            var game = await _ticTacToeService.AbandonGameAsync(id, request.SessionId);
            var gameDto = _ticTacToeService.ConvertToDto(game);

            _logger.LogInformation("Partie abandonnée : GameId={GameId}, SessionId={SessionId}", id, request.SessionId);

            return Ok(gameDto);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'abandon de la partie {GameId}", id);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }
}

