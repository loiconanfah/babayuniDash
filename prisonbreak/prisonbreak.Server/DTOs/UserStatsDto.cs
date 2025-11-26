namespace prisonbreak.Server.DTOs;

/// <summary>
/// Statistiques d'un utilisateur
/// </summary>
public class UserStatsDto
{
    /// <summary>
    /// ID de l'utilisateur
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Nom de l'utilisateur
    /// </summary>
    public string UserName { get; set; } = string.Empty;

    /// <summary>
    /// Email de l'utilisateur
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Score total (somme de tous les scores)
    /// </summary>
    public int TotalScore { get; set; }

    /// <summary>
    /// Score moyen par partie
    /// </summary>
    public double AverageScore { get; set; }

    /// <summary>
    /// Nombre total de parties jouées
    /// </summary>
    public int TotalGamesPlayed { get; set; }

    /// <summary>
    /// Nombre de parties complétées
    /// </summary>
    public int GamesCompleted { get; set; }

    /// <summary>
    /// Nombre de parties abandonnées
    /// </summary>
    public int GamesAbandoned { get; set; }

    /// <summary>
    /// Meilleur score obtenu
    /// </summary>
    public int BestScore { get; set; }

    /// <summary>
    /// Temps total de jeu (en secondes)
    /// </summary>
    public int TotalPlayTime { get; set; }

    /// <summary>
    /// Temps moyen par partie (en secondes)
    /// </summary>
    public double AveragePlayTime { get; set; }

    /// <summary>
    /// Statistiques par niveau de difficulté
    /// </summary>
    public Dictionary<int, LevelStatsDto> StatsByLevel { get; set; } = new();
}

/// <summary>
/// Statistiques pour un niveau de difficulté spécifique
/// </summary>
public class LevelStatsDto
{
    /// <summary>
    /// Niveau de difficulté (1 = Facile, 2 = Moyen, 3 = Difficile)
    /// </summary>
    public int DifficultyLevel { get; set; }

    /// <summary>
    /// Nombre de parties jouées à ce niveau
    /// </summary>
    public int GamesPlayed { get; set; }

    /// <summary>
    /// Nombre de parties complétées à ce niveau
    /// </summary>
    public int GamesCompleted { get; set; }

    /// <summary>
    /// Meilleur score à ce niveau
    /// </summary>
    public int BestScore { get; set; }

    /// <summary>
    /// Score moyen à ce niveau
    /// </summary>
    public double AverageScore { get; set; }

    /// <summary>
    /// Temps moyen à ce niveau (en secondes)
    /// </summary>
    public double AverageTime { get; set; }
}

