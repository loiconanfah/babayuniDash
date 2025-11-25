namespace prisonbreak.Server.Models;

/// <summary>
/// Représente un puzzle Hashi complet
/// Un puzzle contient une grille avec des îles et les ponts (solution)
/// </summary>
public class Puzzle
{
    /// <summary>
    /// Identifiant unique du puzzle
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nom ou titre du puzzle (optionnel)
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Largeur de la grille (nombre de colonnes)
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Hauteur de la grille (nombre de lignes)
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Niveau de difficulté du puzzle
    /// </summary>
    public DifficultyLevel Difficulty { get; set; }

    /// <summary>
    /// Thème visuel du puzzle
    /// </summary>
    public PuzzleTheme Theme { get; set; } = PuzzleTheme.Classic;

    /// <summary>
    /// Date de création du puzzle
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Collection des îles du puzzle
    /// </summary>
    public ICollection<Island> Islands { get; set; } = new List<Island>();

    /// <summary>
    /// Collection des ponts de la solution
    /// Ces ponts représentent la solution correcte du puzzle
    /// </summary>
    public ICollection<Bridge> SolutionBridges { get; set; } = new List<Bridge>();

    /// <summary>
    /// Parties jouées avec ce puzzle
    /// </summary>
    public ICollection<Game> Games { get; set; } = new List<Game>();

    /// <summary>
    /// Calcule le nombre total d'îles dans le puzzle
    /// </summary>
    public int GetIslandCount() => Islands.Count;

    /// <summary>
    /// Calcule le nombre total de ponts dans la solution
    /// </summary>
    public int GetBridgeCount() => SolutionBridges.Count;
}

/// <summary>
/// Niveaux de difficulté pour les puzzles Hashi
/// </summary>
public enum DifficultyLevel
{
    /// <summary>
    /// Facile - Petite grille, peu d'îles, solution évidente
    /// Recommandé pour les débutants
    /// </summary>
    Easy = 1,

    /// <summary>
    /// Moyen - Grille moyenne, nécessite quelques déductions
    /// </summary>
    Medium = 2,

    /// <summary>
    /// Difficile - Grande grille, nécessite des stratégies avancées
    /// </summary>
    Hard = 3,

    /// <summary>
    /// Expert - Très grande grille, nécessite beaucoup de déductions
    /// Pour les joueurs expérimentés
    /// </summary>
    Expert = 4
}

/// <summary>
/// Thèmes visuels pour les puzzles Hashi
/// Chaque thème a un design unique avec couleurs, animations et styles différents
/// </summary>
public enum PuzzleTheme
{
    /// <summary>
    /// Thème classique - Prison traditionnelle
    /// </summary>
    Classic = 1,

    /// <summary>
    /// Thème médiéval - Château fort
    /// </summary>
    Medieval = 2,

    /// <summary>
    /// Thème futuriste - Prison spatiale
    /// </summary>
    Futuristic = 3,

    /// <summary>
    /// Thème sous-marin - Prison aquatique
    /// </summary>
    Underwater = 4,

    /// <summary>
    /// Thème désert - Prison dans le désert
    /// </summary>
    Desert = 5,

    /// <summary>
    /// Thème forêt - Prison dans la jungle
    /// </summary>
    Forest = 6,

    /// <summary>
    /// Thème glace - Prison arctique
    /// </summary>
    Ice = 7,

    /// <summary>
    /// Thème volcan - Prison volcanique
    /// </summary>
    Volcano = 8,

    /// <summary>
    /// Thème néon - Prison cyberpunk
    /// </summary>
    Neon = 9,

    /// <summary>
    /// Thème steampunk - Prison à vapeur
    /// </summary>
    Steampunk = 10,

    /// <summary>
    /// Thème pirate - Prison de pirates
    /// </summary>
    Pirate = 11,

    /// <summary>
    /// Thème zombie - Prison apocalyptique
    /// </summary>
    Zombie = 12,

    /// <summary>
    /// Thème ninja - Prison secrète
    /// </summary>
    Ninja = 13,

    /// <summary>
    /// Thème magie - Prison enchantée
    /// </summary>
    Magic = 14,

    /// <summary>
    /// Thème western - Prison du Far West
    /// </summary>
    Western = 15
}

