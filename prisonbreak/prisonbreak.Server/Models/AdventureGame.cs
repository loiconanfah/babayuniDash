namespace prisonbreak.Server.Models;

/// <summary>
/// Représente une partie d'aventure (jeu d'énigmes et d'exploration)
/// Le joueur explore des salles, collecte des objets et résout des énigmes
/// </summary>
public class AdventureGame
{
    /// <summary>
    /// Identifiant unique de la partie
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant de la session du joueur
    /// </summary>
    public int PlayerSessionId { get; set; }

    /// <summary>
    /// Navigation vers la session du joueur
    /// </summary>
    public Session? PlayerSession { get; set; }

    /// <summary>
    /// Salle actuelle où se trouve le joueur (1-10)
    /// </summary>
    public int CurrentRoom { get; set; } = 1;

    /// <summary>
    /// Objets collectés sérialisés en JSON
    /// Format: ["key", "map", "torch", ...]
    /// </summary>
    public string CollectedItemsJson { get; set; } = "[]";

    /// <summary>
    /// Énigmes résolues sérialisées en JSON
    /// Format: [1, 3, 5, ...] (IDs des énigmes résolues)
    /// </summary>
    public string SolvedPuzzlesJson { get; set; } = "[]";

    /// <summary>
    /// Score du joueur (points gagnés en résolvant des énigmes)
    /// </summary>
    public int Score { get; set; } = 0;

    /// <summary>
    /// Temps écoulé en secondes
    /// </summary>
    public int ElapsedSeconds { get; set; } = 0;

    /// <summary>
    /// Nombre d'énigmes résolues
    /// </summary>
    public int PuzzlesSolved { get; set; } = 0;

    /// <summary>
    /// Statut de la partie
    /// </summary>
    public AdventureGameStatus Status { get; set; } = AdventureGameStatus.InProgress;

    /// <summary>
    /// Date et heure de création de la partie
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date et heure de début de la partie
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Date et heure de fin de la partie
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Vérifie si la partie est terminée
    /// </summary>
    public bool IsCompleted() => Status == AdventureGameStatus.Completed || 
                                Status == AdventureGameStatus.Abandoned;
}

/// <summary>
/// Statut d'une partie d'aventure
/// </summary>
public enum AdventureGameStatus
{
    /// <summary>
    /// Partie en cours
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Partie terminée (toutes les énigmes résolues)
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Partie abandonnée
    /// </summary>
    Abandoned = 3
}

