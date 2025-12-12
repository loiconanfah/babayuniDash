namespace prisonbreak.Server.Models;

/// <summary>
/// Représente une partie de Pierre-Papier-Ciseaux (Rock Paper Scissors)
/// Jeu simple et rapide où les joueurs choisissent simultanément leur coup
/// </summary>
public class RockPaperScissorsGame
{
    /// <summary>
    /// Identifiant unique de la partie
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant de la session du joueur 1
    /// </summary>
    public int Player1SessionId { get; set; }

    /// <summary>
    /// Navigation vers la session du joueur 1
    /// </summary>
    public Session? Player1Session { get; set; }

    /// <summary>
    /// Identifiant de la session du joueur 2
    /// Null si la partie attend un deuxième joueur
    /// </summary>
    public int? Player2SessionId { get; set; }

    /// <summary>
    /// Navigation vers la session du joueur 2
    /// </summary>
    public Session? Player2Session { get; set; }

    /// <summary>
    /// Mode de jeu : contre un joueur (Player) ou contre l'ordinateur (AI)
    /// </summary>
    public RPSGameMode GameMode { get; set; } = RPSGameMode.Player;

    /// <summary>
    /// Coup du joueur 1 : Rock (1), Paper (2), Scissors (3), null si pas encore joué
    /// </summary>
    public int? Player1Choice { get; set; }

    /// <summary>
    /// Coup du joueur 2 : Rock (1), Paper (2), Scissors (3), null si pas encore joué
    /// </summary>
    public int? Player2Choice { get; set; }

    /// <summary>
    /// Nombre de rounds joués
    /// </summary>
    public int RoundNumber { get; set; } = 1;

    /// <summary>
    /// Score du joueur 1 (nombre de rounds gagnés)
    /// </summary>
    public int Player1Score { get; set; } = 0;

    /// <summary>
    /// Score du joueur 2 (nombre de rounds gagnés)
    /// </summary>
    public int Player2Score { get; set; } = 0;

    /// <summary>
    /// Nombre de rounds requis pour gagner (par défaut 3)
    /// </summary>
    public int RoundsToWin { get; set; } = 3;

    /// <summary>
    /// Statut de la partie
    /// </summary>
    public RPSGameStatus Status { get; set; } = RPSGameStatus.WaitingForPlayer;

    /// <summary>
    /// Identifiant du joueur gagnant du round actuel (1 ou 2), null si égalité ou pas encore déterminé
    /// </summary>
    public int? RoundWinner { get; set; }

    /// <summary>
    /// Identifiant du joueur gagnant de la partie (1 ou 2), null si en cours
    /// </summary>
    public int? WinnerPlayerId { get; set; }

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
    /// Temps écoulé en secondes depuis le début
    /// </summary>
    public int ElapsedSeconds { get; set; }

    /// <summary>
    /// Mise du joueur 1 en coins
    /// </summary>
    public int Player1Wager { get; set; } = 0;

    /// <summary>
    /// Mise du joueur 2 en coins
    /// </summary>
    public int Player2Wager { get; set; } = 0;

    /// <summary>
    /// Total de la mise (Player1Wager + Player2Wager)
    /// Le gagnant remporte ce total
    /// </summary>
    public int TotalWager => Player1Wager + Player2Wager;

    /// <summary>
    /// Vérifie si la partie est terminée
    /// </summary>
    public bool IsCompleted() => Status == RPSGameStatus.Completed || 
                                Status == RPSGameStatus.Abandoned;

    /// <summary>
    /// Vérifie si la partie attend un deuxième joueur
    /// </summary>
    public bool IsWaitingForPlayer() => Status == RPSGameStatus.WaitingForPlayer && GameMode == RPSGameMode.Player;

    /// <summary>
    /// Vérifie si c'est une partie contre l'IA
    /// </summary>
    public bool IsAgainstAI() => GameMode == RPSGameMode.AI;
}

/// <summary>
/// Mode de jeu pour Rock Paper Scissors
/// </summary>
public enum RPSGameMode
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
/// Statut d'une partie de Rock Paper Scissors
/// </summary>
public enum RPSGameStatus
{
    /// <summary>
    /// En attente d'un deuxième joueur
    /// </summary>
    WaitingForPlayer = 1,

    /// <summary>
    /// En attente des choix des joueurs
    /// </summary>
    WaitingForChoices = 2,

    /// <summary>
    /// Round terminé, affichage du résultat
    /// </summary>
    RoundCompleted = 3,

    /// <summary>
    /// Partie terminée avec un gagnant
    /// </summary>
    Completed = 4,

    /// <summary>
    /// Partie abandonnée
    /// </summary>
    Abandoned = 5
}

/// <summary>
/// Choix possibles dans Rock Paper Scissors
/// </summary>
public enum RPSChoice
{
    /// <summary>
    /// Pierre (Rock)
    /// </summary>
    Rock = 1,

    /// <summary>
    /// Papier (Paper)
    /// </summary>
    Paper = 2,

    /// <summary>
    /// Ciseaux (Scissors)
    /// </summary>
    Scissors = 3
}

