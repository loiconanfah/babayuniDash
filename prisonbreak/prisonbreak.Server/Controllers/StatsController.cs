using Microsoft.AspNetCore.Mvc;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Services;

namespace prisonbreak.Server.Controllers;

/// <summary>
/// Contrôleur API pour les statistiques et le classement
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class StatsController : ControllerBase
{
    private readonly IStatsService _statsService;
    private readonly ILogger<StatsController> _logger;

    public StatsController(IStatsService statsService, ILogger<StatsController> logger)
    {
        _statsService = statsService;
        _logger = logger;
    }

    /// <summary>
    /// GET api/stats/user/{userId}
    /// Récupère les statistiques d'un utilisateur par son ID
    /// </summary>
    /// <param name="userId">ID de l'utilisateur</param>
    /// <returns>Statistiques de l'utilisateur</returns>
    [HttpGet("user/{userId}")]
    public async Task<ActionResult<UserStatsDto>> GetUserStats(int userId)
    {
        try
        {
            var stats = await _statsService.GetUserStatsAsync(userId);
            return Ok(stats);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des statistiques pour l'utilisateur {UserId}", userId);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// GET api/stats/user/email/{email}
    /// Récupère les statistiques d'un utilisateur par son email
    /// </summary>
    /// <param name="email">Email de l'utilisateur</param>
    /// <returns>Statistiques de l'utilisateur</returns>
    [HttpGet("user/email/{email}")]
    public async Task<ActionResult<UserStatsDto>> GetUserStatsByEmail(string email)
    {
        try
        {
            var stats = await _statsService.GetUserStatsByEmailAsync(email);
            if (stats == null)
            {
                return NotFound($"Aucun utilisateur trouvé avec l'email {email}");
            }
            return Ok(stats);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des statistiques pour l'email {Email}", email);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// GET api/stats/leaderboard
    /// Récupère le classement des meilleurs joueurs
    /// </summary>
    /// <param name="limit">Nombre maximum de joueurs à retourner (défaut: 10)</param>
    /// <returns>Liste des meilleurs joueurs</returns>
    [HttpGet("leaderboard")]
    public async Task<ActionResult<List<LeaderboardEntryDto>>> GetLeaderboard([FromQuery] int limit = 10)
    {
        try
        {
            if (limit < 1 || limit > 100)
            {
                return BadRequest("Le paramètre limit doit être entre 1 et 100");
            }

            var leaderboard = await _statsService.GetLeaderboardAsync(limit);
            return Ok(leaderboard);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du classement");
            return StatusCode(500, "Erreur interne du serveur");
        }
    }
}

