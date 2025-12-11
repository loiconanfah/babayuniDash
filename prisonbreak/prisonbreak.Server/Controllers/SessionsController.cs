using Microsoft.AspNetCore.Mvc;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Services;

namespace prisonbreak.Server.Controllers;

/// <summary>
/// Contrôleur API pour la gestion des sessions
/// Gère la création, la récupération et la gestion des sessions de jeu
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly ISessionService _sessionService;
    private readonly ILogger<SessionsController> _logger;

    /// <summary>
    /// Initialise une nouvelle instance du contrôleur sessions
    /// </summary>
    /// <param name="sessionService">Service de gestion des sessions</param>
    /// <param name="logger">Logger pour l'enregistrement des événements</param>
    public SessionsController(ISessionService sessionService, ILogger<SessionsController> logger)
    {
        _sessionService = sessionService ?? throw new ArgumentNullException(nameof(sessionService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// GET api/sessions/{id}
    /// Récupère une session par son ID
    /// </summary>
    /// <param name="id">Identifiant unique de la session</param>
    /// <returns>La session demandée</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<SessionDto>> GetSessionById(int id)
    {
        try
        {
            var session = await _sessionService.GetSessionByIdAsync(id);
            
            if (session == null)
            {
                return NotFound($"La session avec l'ID {id} n'existe pas");
            }

            return Ok(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de la session {SessionId}", id);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// GET api/sessions/token/{token}
    /// Récupère une session par son token
    /// </summary>
    /// <param name="token">Token unique de la session</param>
    /// <returns>La session demandée</returns>
    [HttpGet("token/{token}")]
    public async Task<ActionResult<SessionDto>> GetSessionByToken(string token)
    {
        try
        {
            var session = await _sessionService.GetSessionByTokenAsync(token);
            
            if (session == null)
            {
                return NotFound($"Aucune session trouvée avec le token fourni");
            }

            return Ok(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de la session par token");
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// GET api/sessions/user/{userId}
    /// Récupère toutes les sessions d'un utilisateur
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur</param>
    /// <param name="activeOnly">Si true, retourne uniquement les sessions actives</param>
    /// <returns>Liste des sessions de l'utilisateur</returns>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<IEnumerable<SessionDto>>> GetSessionsByUserId(int userId, [FromQuery] bool activeOnly = false)
    {
        try
        {
            var sessions = await _sessionService.GetSessionsByUserIdAsync(userId, activeOnly);
            return Ok(sessions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des sessions de l'utilisateur {UserId}", userId);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// GET api/sessions/user/{userId}/active
    /// Récupère la session active d'un utilisateur
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur</param>
    /// <returns>La session active trouvée, ou NotFound si aucune session active</returns>
    [HttpGet("user/{userId}/active")]
    public async Task<ActionResult<SessionDto>> GetActiveSessionByUserId(int userId)
    {
        try
        {
            var session = await _sessionService.GetActiveSessionByUserIdAsync(userId);
            
            if (session == null)
            {
                return NotFound($"Aucune session active trouvée pour l'utilisateur {userId}");
            }

            return Ok(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de la session active de l'utilisateur {UserId}", userId);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// POST api/sessions
    /// Crée une nouvelle session pour un utilisateur
    /// </summary>
    /// <param name="request">Requête contenant les informations de la session à créer</param>
    /// <returns>La session créée</returns>
    [HttpPost]
    public async Task<ActionResult<SessionDto>> CreateSession([FromBody] CreateSessionRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Récupérer l'adresse IP et le User-Agent depuis la requête HTTP
            if (string.IsNullOrWhiteSpace(request.IpAddress))
            {
                request.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
            }

            if (string.IsNullOrWhiteSpace(request.UserAgent))
            {
                request.UserAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            }

            var session = await _sessionService.CreateSessionAsync(request);
            
            _logger.LogInformation("Nouvelle session créée : {SessionId} pour l'utilisateur {UserId}", session.Id, session.UserId);
            
            return CreatedAtAction(nameof(GetSessionById), new { id = session.Id }, session);
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
            _logger.LogError(ex, "Erreur lors de la création de la session");
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// POST api/sessions/{token}/activity
    /// Met à jour la dernière activité d'une session
    /// </summary>
    /// <param name="token">Token de la session à mettre à jour</param>
    /// <returns>Résultat de l'opération</returns>
    [HttpPost("{token}/activity")]
    public async Task<ActionResult> UpdateSessionActivity(string token)
    {
        try
        {
            var result = await _sessionService.UpdateSessionActivityAsync(token);
            
            if (!result)
            {
                return NotFound("Session non trouvée ou invalide");
            }

            return Ok(new { message = "Activité mise à jour avec succès" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour de l'activité de la session");
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// POST api/sessions/{token}/deactivate
    /// Désactive une session (déconnexion)
    /// </summary>
    /// <param name="token">Token de la session à désactiver</param>
    /// <returns>Résultat de l'opération</returns>
    [HttpPost("{token}/deactivate")]
    public async Task<ActionResult> DeactivateSession(string token)
    {
        try
        {
            var result = await _sessionService.DeactivateSessionAsync(token);
            
            if (!result)
            {
                return NotFound("Session non trouvée");
            }

            _logger.LogInformation("Session désactivée : {Token}", token);
            
            return Ok(new { message = "Session désactivée avec succès" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la désactivation de la session");
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// GET api/sessions/{token}/validate
    /// Vérifie si une session est valide
    /// </summary>
    /// <param name="token">Token de la session à vérifier</param>
    /// <returns>True si la session est valide, sinon False</returns>
    [HttpGet("{token}/validate")]
    public async Task<ActionResult<bool>> ValidateSession(string token)
    {
        try
        {
            var isValid = await _sessionService.IsValidSessionAsync(token);
            return Ok(new { isValid });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la validation de la session");
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// GET api/sessions/active
    /// Récupère toutes les sessions actives (utilisateurs en ligne)
    /// </summary>
    /// <param name="excludeSessionId">ID de session à exclure (optionnel)</param>
    /// <returns>Liste des sessions actives</returns>
    [HttpGet("active")]
    public async Task<ActionResult<IEnumerable<SessionDto>>> GetActiveSessions([FromQuery] int? excludeSessionId = null)
    {
        try
        {
            var sessions = await _sessionService.GetActiveSessionsAsync(excludeSessionId);
            return Ok(sessions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des sessions actives");
            return StatusCode(500, "Erreur interne du serveur");
        }
    }
}

