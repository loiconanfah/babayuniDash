namespace prisonbreak.Server.Models;

/// <summary>
/// Représente une partie de Hashi en cours ou terminée
/// Stocke l'état actuel de la partie et les ponts placés par le joueur
/// </summary>
public class Game
{
    /// <summary>
    /// Identifiant unique de la partie
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant du puzzle joué
    /// </summary>
    public int PuzzleId { get; set; }

    /// <summary>
    /// Navigation vers le puzzle
    /// </summary>
    public Puzzle? Puzzle { get; set; }

    /// <summary>
    /// Identifiant de la session de jeu
    /// Clé étrangère vers la table Sessions
    /// Chaque partie appartient à une session utilisateur
    /// </summary>
    public int SessionId { get; set; }

    /// <summary>
    /// Navigation vers la session propriétaire
    /// Relation plusieurs-à-un : plusieurs parties peuvent appartenir à une session
    /// </summary>
    public Session? Session { get; set; }

    /// <summary>
    /// Date et heure de début de la partie
    /// </summary>
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date et heure de fin de la partie (null si en cours)
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Temps écoulé en secondes
    /// </summary>
    public int ElapsedSeconds { get; set; }

    /// <summary>
    /// Statut actuel de la partie
    /// </summary>
    public GameStatus Status { get; set; } = GameStatus.InProgress;

    /// <summary>
    /// Les ponts actuellement placés par le joueur
    /// Sérialisé en JSON pour simplifier le stockage
    /// Format: [{fromIslandId: 1, toIslandId: 2, isDouble: false}, ...]
    /// </summary>
    public string PlayerBridgesJson { get; set; } = "[]";

    /// <summary>
    /// Score de la partie (calculé selon le temps et les erreurs)
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Nombre d'indices utilisés
    /// </summary>
    public int HintsUsed { get; set; }

    /// <summary>
    /// Calcule si la partie est terminée
    /// </summary>
    public bool IsCompleted() => Status == GameStatus.Completed;

    /// <summary>
    /// Calcule le temps total écoulé
    /// </summary>
    public TimeSpan GetElapsedTime()
    {
        if (CompletedAt.HasValue)
        {
            return CompletedAt.Value - StartedAt;
        }
        return DateTime.UtcNow - StartedAt;
    }
}

/// <summary>
/// Statut d'une partie de Hashi
/// </summary>
public enum GameStatus
{
    /// <summary>
    /// Partie en cours
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Partie terminée avec succès
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Partie abandonnée par le joueur
    /// </summary>
    Abandoned = 3,

    /// <summary>
    /// Partie en pause
    /// </summary>
    Paused = 4
}

