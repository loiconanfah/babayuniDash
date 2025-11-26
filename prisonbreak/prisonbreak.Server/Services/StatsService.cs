using Microsoft.EntityFrameworkCore;
using prisonbreak.Server.Data;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Services;

/// <summary>
/// Service de gestion des statistiques et du classement
/// </summary>
public class StatsService : IStatsService
{
    private readonly HashiDbContext _context;

    public StatsService(HashiDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Récupère les statistiques d'un utilisateur
    /// </summary>
    public async Task<UserStatsDto> GetUserStatsAsync(int userId)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null)
        {
            throw new ArgumentException($"L'utilisateur avec l'ID {userId} n'existe pas");
        }

        // Récupérer toutes les sessions de l'utilisateur avec leurs jeux
        var sessions = await _context.Sessions
            .Include(s => s.Games)
                .ThenInclude(g => g.Puzzle)
            .Where(s => s.UserId == userId)
            .ToListAsync();

        return CalculateUserStats(user, sessions);
    }

    /// <summary>
    /// Récupère les statistiques d'un utilisateur par son email
    /// </summary>
    public async Task<UserStatsDto?> GetUserStatsByEmailAsync(string email)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);

        if (user == null)
        {
            return null;
        }

        // Récupérer toutes les sessions de l'utilisateur avec leurs jeux
        var sessions = await _context.Sessions
            .Include(s => s.Games)
                .ThenInclude(g => g.Puzzle)
            .Where(s => s.UserId == user.Id)
            .ToListAsync();

        return CalculateUserStats(user, sessions);
    }

    /// <summary>
    /// Récupère le classement (leaderboard) des meilleurs joueurs
    /// </summary>
    public async Task<List<LeaderboardEntryDto>> GetLeaderboardAsync(int limit = 10)
    {
        var users = await _context.Users
            .Where(u => u.IsActive)
            .ToListAsync();

        var leaderboard = new List<LeaderboardEntryDto>();

        foreach (var user in users)
        {
            // Récupérer toutes les sessions de l'utilisateur avec leurs jeux
            var sessions = await _context.Sessions
                .Include(s => s.Games)
                .Where(s => s.UserId == user.Id)
                .ToListAsync();

            var allGames = sessions
                .SelectMany(s => s.Games)
                .Where(g => g.Status == GameStatus.Completed)
                .ToList();

            if (allGames.Any())
            {
                var totalScore = allGames.Sum(g => g.Score);
                var gamesCompleted = allGames.Count;
                var averageScore = allGames.Average(g => g.Score);
                var bestScore = allGames.Max(g => g.Score);

                leaderboard.Add(new LeaderboardEntryDto
                {
                    UserId = user.Id,
                    UserName = user.Name,
                    TotalScore = totalScore,
                    GamesCompleted = gamesCompleted,
                    AverageScore = Math.Round(averageScore, 2),
                    BestScore = bestScore
                });
            }
        }

        // Trier par score total décroissant, puis par nombre de parties complétées
        leaderboard = leaderboard
            .OrderByDescending(l => l.TotalScore)
            .ThenByDescending(l => l.GamesCompleted)
            .Take(limit)
            .ToList();

        // Ajouter le rang
        for (int i = 0; i < leaderboard.Count; i++)
        {
            leaderboard[i].Rank = i + 1;
        }

        return leaderboard;
    }

    /// <summary>
    /// Calcule les statistiques d'un utilisateur
    /// </summary>
    private UserStatsDto CalculateUserStats(User user, List<Session> sessions)
    {
        var allGames = sessions
            .SelectMany(s => s.Games)
            .ToList();

        var completedGames = allGames
            .Where(g => g.Status == GameStatus.Completed)
            .ToList();

        var abandonedGames = allGames
            .Where(g => g.Status == GameStatus.Abandoned)
            .ToList();

        var totalScore = completedGames.Sum(g => g.Score);
        var averageScore = completedGames.Any() ? completedGames.Average(g => g.Score) : 0;
        var bestScore = completedGames.Any() ? completedGames.Max(g => g.Score) : 0;
        var totalPlayTime = allGames.Sum(g => g.ElapsedSeconds);
        var averagePlayTime = allGames.Any() ? allGames.Average(g => g.ElapsedSeconds) : 0;

        // Statistiques par niveau
        var statsByLevel = new Dictionary<int, LevelStatsDto>();
        for (int level = 1; level <= 3; level++)
        {
            var levelGames = allGames
                .Where(g => g.Puzzle != null && (int)g.Puzzle.Difficulty == level)
                .ToList();

            var levelCompleted = levelGames
                .Where(g => g.Status == GameStatus.Completed)
                .ToList();

            if (levelGames.Any())
            {
                statsByLevel[level] = new LevelStatsDto
                {
                    DifficultyLevel = level,
                    GamesPlayed = levelGames.Count,
                    GamesCompleted = levelCompleted.Count,
                    BestScore = levelCompleted.Any() ? levelCompleted.Max(g => g.Score) : 0,
                    AverageScore = levelCompleted.Any() ? Math.Round(levelCompleted.Average(g => g.Score), 2) : 0,
                    AverageTime = levelGames.Any() ? Math.Round(levelGames.Average(g => g.ElapsedSeconds), 2) : 0
                };
            }
        }

        return new UserStatsDto
        {
            UserId = user.Id,
            UserName = user.Name,
            Email = user.Email,
            TotalScore = totalScore,
            AverageScore = Math.Round(averageScore, 2),
            TotalGamesPlayed = allGames.Count,
            GamesCompleted = completedGames.Count,
            GamesAbandoned = abandonedGames.Count,
            BestScore = bestScore,
            TotalPlayTime = totalPlayTime,
            AveragePlayTime = Math.Round(averagePlayTime, 2),
            StatsByLevel = statsByLevel
        };
    }
}

