namespace prisonbreak.Server.Models;

/// <summary>
/// Représente un tournoi
/// </summary>
public class Tournament
{
    /// <summary>
    /// Identifiant unique du tournoi
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nom du tournoi
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description du tournoi
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Type de jeu du tournoi
    /// </summary>
    public TournamentGameType GameType { get; set; }

    /// <summary>
    /// Statut du tournoi
    /// </summary>
    public TournamentStatus Status { get; set; } = TournamentStatus.Registration;

    /// <summary>
    /// Nombre maximum de participants
    /// </summary>
    public int MaxParticipants { get; set; } = 16;

    /// <summary>
    /// Nombre actuel de participants
    /// </summary>
    public int CurrentParticipants { get; set; } = 0;

    /// <summary>
    /// Mise d'entrée en Babayuni (0 = gratuit)
    /// </summary>
    public int EntryFee { get; set; } = 0;

    /// <summary>
    /// Récompense totale du tournoi (en Babayuni)
    /// </summary>
    public int TotalPrize { get; set; } = 0;

    /// <summary>
    /// Récompense pour le 1er place
    /// </summary>
    public int FirstPlacePrize { get; set; } = 0;

    /// <summary>
    /// Récompense pour le 2ème place
    /// </summary>
    public int SecondPlacePrize { get; set; } = 0;

    /// <summary>
    /// Récompense pour le 3ème place
    /// </summary>
    public int ThirdPlacePrize { get; set; } = 0;

    /// <summary>
    /// Date de début du tournoi
    /// </summary>
    public DateTime StartDate { get; set; }

    /// <summary>
    /// Date de fin du tournoi
    /// </summary>
    public DateTime? EndDate { get; set; }

    /// <summary>
    /// Date de création du tournoi
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Participants du tournoi
    /// </summary>
    public ICollection<TournamentParticipant> Participants { get; set; } = new List<TournamentParticipant>();

    /// <summary>
    /// Matchs du tournoi
    /// </summary>
    public ICollection<TournamentMatch> Matches { get; set; } = new List<TournamentMatch>();
}

/// <summary>
/// Type de jeu pour un tournoi
/// </summary>
public enum TournamentGameType
{
    /// <summary>
    /// Pierre-Papier-Ciseaux
    /// </summary>
    RockPaperScissors = 1,

    /// <summary>
    /// Tic-Tac-Toe
    /// </summary>
    TicTacToe = 2,

    /// <summary>
    /// Connect Four
    /// </summary>
    ConnectFour = 3
}

/// <summary>
/// Statut d'un tournoi
/// </summary>
public enum TournamentStatus
{
    /// <summary>
    /// Inscriptions ouvertes
    /// </summary>
    Registration = 1,

    /// <summary>
    /// Tournoi en cours
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Tournoi terminé
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Tournoi annulé
    /// </summary>
    Cancelled = 4
}

/// <summary>
/// Participant à un tournoi
/// </summary>
public class TournamentParticipant
{
    /// <summary>
    /// Identifiant unique
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID du tournoi
    /// </summary>
    public int TournamentId { get; set; }

    /// <summary>
    /// Tournoi
    /// </summary>
    public Tournament Tournament { get; set; } = null!;

    /// <summary>
    /// ID de l'utilisateur
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Utilisateur participant
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Position finale dans le tournoi (null si pas encore déterminé)
    /// </summary>
    public int? FinalPosition { get; set; }

    /// <summary>
    /// Récompense gagnée (en Babayuni)
    /// </summary>
    public int PrizeWon { get; set; } = 0;

    /// <summary>
    /// Date d'inscription
    /// </summary>
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Indique si le participant est éliminé
    /// </summary>
    public bool IsEliminated { get; set; } = false;
}

/// <summary>
/// Match dans un tournoi
/// </summary>
public class TournamentMatch
{
    /// <summary>
    /// Identifiant unique
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// ID du tournoi
    /// </summary>
    public int TournamentId { get; set; }

    /// <summary>
    /// Tournoi
    /// </summary>
    public Tournament Tournament { get; set; } = null!;

    /// <summary>
    /// ID du participant 1
    /// </summary>
    public int Participant1Id { get; set; }

    /// <summary>
    /// Participant 1
    /// </summary>
    public TournamentParticipant Participant1 { get; set; } = null!;

    /// <summary>
    /// ID du participant 2
    /// </summary>
    public int Participant2Id { get; set; }

    /// <summary>
    /// Participant 2
    /// </summary>
    public TournamentParticipant Participant2 { get; set; } = null!;

    /// <summary>
    /// Round du tournoi (1 = premier round, 2 = quart de finale, etc.)
    /// </summary>
    public int Round { get; set; }

    /// <summary>
    /// ID du gagnant (null si pas encore déterminé)
    /// </summary>
    public int? WinnerId { get; set; }

    /// <summary>
    /// Statut du match
    /// </summary>
    public TournamentMatchStatus Status { get; set; } = TournamentMatchStatus.Pending;

    /// <summary>
    /// ID de la partie de jeu associée (pour Rock Paper Scissors, etc.)
    /// </summary>
    public int? GameId { get; set; }

    /// <summary>
    /// Date de début du match
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Date de fin du match
    /// </summary>
    public DateTime? CompletedAt { get; set; }
}

/// <summary>
/// Statut d'un match de tournoi
/// </summary>
public enum TournamentMatchStatus
{
    /// <summary>
    /// En attente
    /// </summary>
    Pending = 1,

    /// <summary>
    /// En cours
    /// </summary>
    InProgress = 2,

    /// <summary>
    /// Terminé
    /// </summary>
    Completed = 3,

    /// <summary>
    /// Forfait (un participant n'a pas joué)
    /// </summary>
    Forfeit = 4
}

