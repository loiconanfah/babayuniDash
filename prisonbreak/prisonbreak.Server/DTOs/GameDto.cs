namespace prisonbreak.Server.DTOs;

/// <summary>
/// DTO pour une partie de jeu
/// Contient l'état actuel de la partie et le puzzle associé
/// </summary>
public class GameDto
{
    /// <summary>
    /// Identifiant de la partie
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant du puzzle
    /// </summary>
    public int PuzzleId { get; set; }

    /// <summary>
    /// Le puzzle associé à cette partie
    /// </summary>
    public PuzzleDto? Puzzle { get; set; }

    /// <summary>
    /// Identifiant de la session de jeu
    /// Chaque partie appartient à une session utilisateur
    /// </summary>
    public int SessionId { get; set; }

    /// <summary>
    /// Date de début
    /// </summary>
    public DateTime StartedAt { get; set; }

    /// <summary>
    /// Date de fin (null si en cours)
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Temps écoulé en secondes
    /// </summary>
    public int ElapsedSeconds { get; set; }

    /// <summary>
    /// Statut (1=InProgress, 2=Completed, 3=Abandoned, 4=Paused)
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Ponts placés par le joueur
    /// </summary>
    public List<BridgeDto> PlayerBridges { get; set; } = new();

    /// <summary>
    /// Score de la partie
    /// </summary>
    public int Score { get; set; }

    /// <summary>
    /// Nombre d'indices utilisés
    /// </summary>
    public int HintsUsed { get; set; }
}

