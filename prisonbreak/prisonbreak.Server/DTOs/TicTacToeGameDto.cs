namespace prisonbreak.Server.DTOs;

/// <summary>
/// DTO pour une partie de Tic-Tac-Toe
/// Utilisé pour la communication entre le frontend et le backend
/// </summary>
public class TicTacToeGameDto
{
    /// <summary>
    /// Identifiant de la partie
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant de la session du joueur 1 (X)
    /// </summary>
    public int Player1SessionId { get; set; }

    /// <summary>
    /// Nom du joueur 1
    /// </summary>
    public string? Player1Name { get; set; }

    /// <summary>
    /// Identifiant de la session du joueur 2 (O)
    /// Null si la partie attend un deuxième joueur
    /// </summary>
    public int? Player2SessionId { get; set; }

    /// <summary>
    /// Nom du joueur 2
    /// </summary>
    public string? Player2Name { get; set; }

    /// <summary>
    /// État de la grille 3x3
    /// Tableau de 9 éléments : ["X","O","", "","X","", "","","O"]
    /// </summary>
    public List<string> Board { get; set; } = new();

    /// <summary>
    /// Tour actuel : 1 = joueur 1 (X), 2 = joueur 2 (O)
    /// </summary>
    public int CurrentPlayer { get; set; }

    /// <summary>
    /// Statut de la partie (1=WaitingForPlayer, 2=InProgress, 3=Completed, 4=Draw, 5=Abandoned)
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// Identifiant du joueur gagnant (1 ou 2), null si match nul ou en cours
    /// </summary>
    public int? WinnerPlayerId { get; set; }

    /// <summary>
    /// Date de création
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Date de début (quand le 2e joueur rejoint)
    /// </summary>
    public DateTime? StartedAt { get; set; }

    /// <summary>
    /// Date de fin
    /// </summary>
    public DateTime? CompletedAt { get; set; }

    /// <summary>
    /// Temps écoulé en secondes
    /// </summary>
    public int ElapsedSeconds { get; set; }

    /// <summary>
    /// Nombre de coups joués
    /// </summary>
    public int MoveCount { get; set; }

    /// <summary>
    /// Mode de jeu : 1 = contre un joueur, 2 = contre l'IA
    /// </summary>
    public int GameMode { get; set; }

    /// <summary>
    /// Mise du joueur 1 en coins
    /// </summary>
    public int Player1Wager { get; set; }

    /// <summary>
    /// Mise du joueur 2 en coins
    /// </summary>
    public int Player2Wager { get; set; }

    /// <summary>
    /// Total de la mise (Player1Wager + Player2Wager)
    /// </summary>
    public int TotalWager => Player1Wager + Player2Wager;
}

/// <summary>
/// Requête pour créer une nouvelle partie de Tic-Tac-Toe
/// </summary>
public class CreateTicTacToeGameRequest
{
    /// <summary>
    /// Identifiant de la session du joueur qui crée la partie
    /// </summary>
    public int SessionId { get; set; }

    /// <summary>
    /// Mode de jeu : 1 = contre un joueur, 2 = contre l'IA
    /// </summary>
    public int GameMode { get; set; } = 1;

    /// <summary>
    /// Identifiant de la session du joueur 2 (optionnel, pour inviter un joueur spécifique)
    /// </summary>
    public int? Player2SessionId { get; set; }

    /// <summary>
    /// Mise du joueur 1 en coins (optionnel, 0 par défaut)
    /// </summary>
    public int Wager { get; set; } = 0;
}

/// <summary>
/// Requête pour rejoindre une partie existante
/// </summary>
public class JoinTicTacToeGameRequest
{
    /// <summary>
    /// Identifiant de la partie à rejoindre
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// Identifiant de la session du joueur qui rejoint
    /// </summary>
    public int SessionId { get; set; }

    /// <summary>
    /// Mise du joueur 2 en coins (doit correspondre à la mise du joueur 1)
    /// </summary>
    public int Wager { get; set; } = 0;
}

/// <summary>
/// Requête pour jouer un coup
/// </summary>
public class PlayMoveRequest
{
    /// <summary>
    /// Position dans la grille (0-8)
    /// 0 = haut-gauche, 1 = haut-milieu, 2 = haut-droite
    /// 3 = milieu-gauche, 4 = centre, 5 = milieu-droite
    /// 6 = bas-gauche, 7 = bas-milieu, 8 = bas-droite
    /// </summary>
    public int Position { get; set; }

    /// <summary>
    /// Identifiant de la session du joueur qui joue
    /// </summary>
    public int SessionId { get; set; }
}

