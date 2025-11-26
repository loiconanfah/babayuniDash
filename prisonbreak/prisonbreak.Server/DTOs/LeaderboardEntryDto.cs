namespace prisonbreak.Server.DTOs;

/// <summary>
/// Entrée du classement (leaderboard)
/// </summary>
public class LeaderboardEntryDto
{
    /// <summary>
    /// Position dans le classement
    /// </summary>
    public int Rank { get; set; }

    /// <summary>
    /// ID de l'utilisateur
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Nom de l'utilisateur
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Score total
    /// </summary>
    public int TotalScore { get; set; }

    /// <summary>
    /// Nombre de parties complétées
    /// </summary>
    public int GamesCompleted { get; set; }

    /// <summary>
    /// Score moyen
    /// </summary>
    public double AverageScore { get; set; }

    /// <summary>
    /// Meilleur score
    /// </summary>
    public int BestScore { get; set; }
}

