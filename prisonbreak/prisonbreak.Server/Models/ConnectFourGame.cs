namespace prisonbreak.Server.Models;

/// <summary>
/// Représente une partie de Connect Four (Puissance 4) multijoueur
/// Stocke l'état de la grille 7x6 et les informations des joueurs
/// </summary>
public class ConnectFourGame
{
    /// <summary>
    /// Identifiant unique de la partie
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant de la session du joueur 1 (Rouge)
    /// </summary>
    public int Player1SessionId { get; set; }

    /// <summary>
    /// Navigation vers la session du joueur 1
    /// </summary>
    public Session? Player1Session { get; set; }

    /// <summary>
    /// Identifiant de la session du joueur 2 (Jaune)
    /// Null si la partie attend un deuxième joueur ou si c'est une partie contre l'IA
    /// </summary>
    public int? Player2SessionId { get; set; }

    /// <summary>
    /// Navigation vers la session du joueur 2
    /// </summary>
    public Session? Player2Session { get; set; }

    /// <summary>
    /// Mode de jeu : contre un joueur (Player) ou contre l'ordinateur (AI)
    /// </summary>
    public ConnectFourGameMode GameMode { get; set; } = ConnectFourGameMode.Player;

    /// <summary>
    /// État de la grille 7x6 sérialisé en JSON
    /// Format: [[0,0,0,0,0,0], [0,0,0,0,0,0], ...] (7 colonnes, 6 lignes)
    /// 0 = case vide, 1 = joueur 1 (Rouge), 2 = joueur 2 (Jaune)
    /// </summary>
    public string BoardJson { get; set; } = "[[0,0,0,0,0,0],[0,0,0,0,0,0],[0,0,0,0,0,0],[0,0,0,0,0,0],[0,0,0,0,0,0],[0,0,0,0,0,0],[0,0,0,0,0,0]]";

    /// <summary>
    /// Tour actuel : 1 = joueur 1 (Rouge), 2 = joueur 2 (Jaune)
    /// </summary>
    public int CurrentPlayer { get; set; } = 1;

    /// <summary>
    /// Statut de la partie
    /// </summary>
    public ConnectFourGameStatus Status { get; set; } = ConnectFourGameStatus.WaitingForPlayer;

    /// <summary>
    /// Identifiant du joueur gagnant (1 ou 2), null si match nul ou en cours
    /// </summary>
    public int? WinnerPlayerId { get; set; }

    /// <summary>
    /// Date et heure de création de la partie
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date et heure de début de la partie (quand le 2e joueur rejoint)
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Date et heure de fin de la partie
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Temps écoulé en secondes depuis le début
    /// </summary>
    public int ElapsedSeconds { get; set; }

    /// <summary>
    /// Nombre de coups joués
    /// </summary>
    public int MoveCount { get; set; }

    /// <summary>
    /// Vérifie si la partie est terminée
    /// </summary>
    public bool IsCompleted() => Status == ConnectFourGameStatus.Completed || 
                                 Status == ConnectFourGameStatus.Draw || 
                                 Status == ConnectFourGameStatus.Abandoned;

    /// <summary>
    /// Vérifie si la partie attend un deuxième joueur
    /// </summary>
    public bool IsWaitingForPlayer() => Status == ConnectFourGameStatus.WaitingForPlayer && GameMode == ConnectFourGameMode.Player;

    /// <summary>
    /// Vérifie si c'est une partie contre l'IA
    /// </summary>
    public bool IsAgainstAI() => GameMode == ConnectFourGameMode.AI;
}

/// <summary>
/// Mode de jeu pour Connect Four
/// </summary>
public enum ConnectFourGameMode
{
    /// <summary>
    /// Partie contre un autre joueur (multijoueur)
    /// </summary>
    Player = 1,

    /// <summary>
    /// Partie contre l'ordinateur (IA)
    /// </summary>
    AI = 2
}

/// <summary>
/// Statut d'une partie de Connect Four
/// </summary>
public enum ConnectFourGameStatus
{
    /// <summary>
    /// En attente d'un deuxième joueur
    /// </summary>
    WaitingForPlayer = 1,

    /// <summary>
    /// Partie en cours
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Partie terminée avec un gagnant
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Match nul (grille pleine sans gagnant)
    /// </summary>
    Draw = 4,

    /// <summary>
    /// Partie abandonnée
    /// </summary>
    Abandoned = 5
}

