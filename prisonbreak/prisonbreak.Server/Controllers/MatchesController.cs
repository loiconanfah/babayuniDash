using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using prisonbreak.Server.Data;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Controllers;

/// <summary>
/// Contrôleur API pour afficher les matchs en cours (VS)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class MatchesController : ControllerBase
{
    private readonly HashiDbContext _context;
    private readonly ILogger<MatchesController> _logger;

    public MatchesController(HashiDbContext context, ILogger<MatchesController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Récupère tous les matchs en cours (tous les jeux multijoueurs)
    /// </summary>
    [HttpGet("active")]
    public async Task<ActionResult<List<ActiveMatchDto>>> GetActiveMatches()
    {
        try
        {
            var matches = new List<ActiveMatchDto>();

            // TicTacToe games
            var ticTacToeGames = await _context.TicTacToeGames
                .Include(g => g.Player1Session)
                    .ThenInclude(s => s!.User)
                .Include(g => g.Player2Session)
                    .ThenInclude(s => s!.User)
                .Where(g => g.Status == TicTacToeGameStatus.InProgress || g.Status == TicTacToeGameStatus.WaitingForPlayer)
                .ToListAsync();

            foreach (var game in ticTacToeGames)
            {
                matches.Add(new ActiveMatchDto
                {
                    GameId = game.Id,
                    GameType = "TicTacToe",
                    GameTypeIcon = "⭕",
                    Player1Name = game.Player1Session?.User?.Name ?? "Joueur 1",
                    Player1Avatar = "👤",
                    Player1Wager = game.Player1Wager,
                    Player2Name = game.Player2Session?.User?.Name ?? (game.Status == TicTacToeGameStatus.WaitingForPlayer ? null : "IA"),
                    Player2Avatar = game.GameMode == TicTacToeGameMode.AI ? "🤖" : "👤",
                    Player2Wager = game.Player2Wager,
                    TotalWager = game.TotalWager,
                    Status = game.Status == TicTacToeGameStatus.WaitingForPlayer ? "En attente" : "En cours",
                    CreatedAt = game.CreatedAt
                });
            }

            // ConnectFour games
            var connectFourGames = await _context.ConnectFourGames
                .Include(g => g.Player1Session)
                    .ThenInclude(s => s!.User)
                .Include(g => g.Player2Session)
                    .ThenInclude(s => s!.User)
                .Where(g => g.Status == ConnectFourGameStatus.InProgress || g.Status == ConnectFourGameStatus.WaitingForPlayer)
                .ToListAsync();

            foreach (var game in connectFourGames)
            {
                matches.Add(new ActiveMatchDto
                {
                    GameId = game.Id,
                    GameType = "ConnectFour",
                    GameTypeIcon = "🔴",
                    Player1Name = game.Player1Session?.User?.Name ?? "Joueur 1",
                    Player1Avatar = "👤",
                    Player1Wager = game.Player1Wager,
                    Player2Name = game.Player2Session?.User?.Name ?? (game.Status == ConnectFourGameStatus.WaitingForPlayer ? null : "IA"),
                    Player2Avatar = game.GameMode == ConnectFourGameMode.AI ? "🤖" : "👤",
                    Player2Wager = game.Player2Wager,
                    TotalWager = game.TotalWager,
                    Status = game.Status == ConnectFourGameStatus.WaitingForPlayer ? "En attente" : "En cours",
                    CreatedAt = game.CreatedAt
                });
            }

            // RockPaperScissors games
            var rpsGames = await _context.RockPaperScissorsGames
                .Include(g => g.Player1Session)
                    .ThenInclude(s => s!.User)
                .Include(g => g.Player2Session)
                    .ThenInclude(s => s!.User)
                .Where(g => g.Status == RPSGameStatus.WaitingForPlayer || 
                           g.Status == RPSGameStatus.WaitingForChoices || 
                           g.Status == RPSGameStatus.RoundCompleted)
                .ToListAsync();

            foreach (var game in rpsGames)
            {
                matches.Add(new ActiveMatchDto
                {
                    GameId = game.Id,
                    GameType = "RockPaperScissors",
                    GameTypeIcon = "✂️",
                    Player1Name = game.Player1Session?.User?.Name ?? "Joueur 1",
                    Player1Avatar = "👤",
                    Player1Wager = game.Player1Wager,
                    Player2Name = game.Player2Session?.User?.Name ?? (game.Status == RPSGameStatus.WaitingForPlayer ? null : "IA"),
                    Player2Avatar = game.GameMode == RPSGameMode.AI ? "🤖" : "👤",
                    Player2Wager = game.Player2Wager,
                    TotalWager = game.TotalWager,
                    Status = game.Status == RPSGameStatus.WaitingForPlayer ? "En attente" : 
                            game.Status == RPSGameStatus.WaitingForChoices ? "En attente des choix" :
                            game.Status == RPSGameStatus.RoundCompleted ? "Round terminé" : "En cours",
                    CreatedAt = game.CreatedAt
                });
            }

            return Ok(matches.OrderByDescending(m => m.CreatedAt));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des matchs actifs");
            return StatusCode(500, "Erreur serveur");
        }
    }
}

/// <summary>
/// DTO pour un match actif
/// </summary>
public class ActiveMatchDto
{
    public int GameId { get; set; }
    public string GameType { get; set; } = string.Empty;
    public string GameTypeIcon { get; set; } = "🎮";
    public string Player1Name { get; set; } = string.Empty;
    public string Player1Avatar { get; set; } = "👤";
    public int Player1Wager { get; set; }
    public string? Player2Name { get; set; }
    public string Player2Avatar { get; set; } = "👤";
    public int Player2Wager { get; set; }
    public int TotalWager { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

