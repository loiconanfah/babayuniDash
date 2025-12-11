namespace prisonbreak.Server.DTOs;

/// <summary>
/// DTO pour une partie de Connect Four (Puissance 4)
/// Utilisé pour la communication entre le frontend et le backend
/// </summary>
public class ConnectFourGameDto
{
    /// <summary>
    /// Identifiant de la partie
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant de la session du joueur 1 (Rouge)
    /// </summary>
    public int Player1SessionId { get; set; }

    /// <summary>
    /// Nom du joueur 1
    /// </summary>
    public string? Player1Name { get; set; }

    /// <summary>
    /// Identifiant de la session du joueur 2 (Jaune)
    /// Null si la partie attend un deuxième joueur
    /// </summary>
    public int? Player2SessionId { get; set; }

    /// <summary>
    /// Nom du joueur 2
    /// </summary>
    public string? Player2Name { get; set; }

    /// <summary>
    /// État de la grille 7x6
    /// Tableau de 7 colonnes, chaque colonne contient 6 lignes
    /// Format: [[0,0,0,0,0,0], [0,0,0,0,0,0], ...]
    /// 0 = case vide, 1 = joueur 1 (Rouge), 2 = joueur 2 (Jaune)
    /// </summary>
    public List<List<int>> Board { get; set; } = new();

    /// <summary>
    /// Tour actuel : 1 = joueur 1 (Rouge), 2 = joueur 2 (Jaune)
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
}

/// <summary>
/// Requête pour créer une nouvelle partie de Connect Four
/// </summary>
public class CreateConnectFourGameRequest
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
}

/// <summary>
/// Requête pour rejoindre une partie existante
/// </summary>
public class JoinConnectFourGameRequest
{
    /// <summary>
    /// Identifiant de la partie à rejoindre
    /// </summary>
    public int GameId { get; set; }

    /// <summary>
    /// Identifiant de la session du joueur qui rejoint
    /// </summary>
    public int SessionId { get; set; }
}

/// <summary>
/// Requête pour jouer un coup dans Connect Four
/// </summary>
public class PlayConnectFourMoveRequest
{
    /// <summary>
    /// Numéro de la colonne (0-6)
    /// 0 = colonne de gauche, 6 = colonne de droite
    /// </summary>
    public int Column { get; set; }

    /// <summary>
    /// Identifiant de la session du joueur qui joue
    /// </summary>
    public int SessionId { get; set; }
}

