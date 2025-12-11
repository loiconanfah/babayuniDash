/**
 * Types TypeScript pour le jeu Hashi
 * Ces types correspondent aux DTOs du backend pour assurer la cohérence des données
 */

/**
 * Représente une île dans le jeu
 */
export interface Island {
  /** Identifiant unique de l'île */
  id: number
  /** Position X (colonne) dans la grille */
  x: number
  /** Position Y (ligne) dans la grille */
  y: number
  /** Nombre de ponts requis pour cette île */
  requiredBridges: number
  /** Nombre actuel de ponts connectés */
  currentBridges: number
}

/**
 * Représente un pont entre deux îles
 */
export interface Bridge {
  /** ID de l'île de départ */
  fromIslandId: number
  /** ID de l'île d'arrivée */
  toIslandId: number
  /** Indique si le pont est double (compte pour 2 connexions) */
  isDouble: boolean
}

/**
 * Niveau de difficulté du puzzle
 */
export enum DifficultyLevel {
  /** Facile - Pour les débutants */
  Easy = 1,
  /** Moyen - Difficulté intermédiaire */
  Medium = 2,
  /** Difficile - Pour les joueurs expérimentés */
  Hard = 3,
  /** Expert - Très difficile */
  Expert = 4
}

/**
 * Thèmes visuels pour les puzzles
 */
export enum PuzzleTheme {
  Classic = 1,
  Medieval = 2,
  Futuristic = 3,
  Underwater = 4,
  Desert = 5,
  Forest = 6,
  Ice = 7,
  Volcano = 8,
  Neon = 9,
  Steampunk = 10,
  Pirate = 11,
  Zombie = 12,
  Ninja = 13,
  Magic = 14,
  Western = 15
}

/**
 * Représente un puzzle complet
 */
export interface Puzzle {
  /** Identifiant unique du puzzle */
  id: number
  /** Nom du puzzle */
  name?: string
  /** Largeur de la grille */
  width: number
  /** Hauteur de la grille */
  height: number
  /** Niveau de difficulté */
  difficulty: DifficultyLevel
  /** Thème visuel du puzzle */
  theme: PuzzleTheme
  /** Liste des îles */
  islands: Island[]
  /** Nombre total d'îles */
  islandCount: number
}

/**
 * Statut d'une partie
 */
export enum GameStatus {
  /** Partie en cours */
  InProgress = 1,
  /** Partie terminée avec succès */
  Completed = 2,
  /** Partie abandonnée */
  Abandoned = 3,
  /** Partie en pause */
  Paused = 4
}

/**
 * Représente une partie de jeu
 */
export interface Game {
  /** Identifiant unique de la partie */
  id: number
  /** ID du puzzle joué */
  puzzleId: number
  /** Le puzzle associé */
  puzzle?: Puzzle
  /** Date de début */
  startedAt: Date
  /** Date de fin (null si en cours) */
  completedAt?: Date
  /** Temps écoulé en secondes */
  elapsedSeconds: number
  /** Statut de la partie */
  status: GameStatus
  /** Ponts placés par le joueur */
  playerBridges: Bridge[]
  /** Score de la partie */
  score: number
  /** Nombre d'indices utilisés */
  hintsUsed: number
}

/**
 * Résultat de validation d'une solution
 */
export interface ValidationResult {
  /** La solution est-elle valide ? */
  isValid: boolean
  /** Le puzzle est-il complet ? */
  isComplete: boolean
  /** Liste des erreurs trouvées */
  errors: string[]
  /** IDs des îles incomplètes */
  incompleteIslandIds: number[]
  /** Toutes les îles sont-elles connectées ? */
  isFullyConnected: boolean
  /** Message de félicitations ou d'information */
  message?: string
}

/**
 * Requête pour générer un nouveau puzzle
 */
export interface GeneratePuzzleRequest {
  /** Largeur de la grille (5-20) */
  width: number
  /** Hauteur de la grille (5-20) */
  height: number
  /** Niveau de difficulté */
  difficulty: DifficultyLevel
}

/**
 * Requête pour créer une nouvelle partie
 */
export interface CreateGameRequest {
  /** ID du puzzle à jouer */
  puzzleId: number
  /** ID de la session de jeu */
  sessionId: number
}

/**
 * Statistiques d'un utilisateur
 */
export interface UserStats {
  /** ID de l'utilisateur */
  userId: number
  /** Nom de l'utilisateur */
  userName: string
  /** Email de l'utilisateur */
  email: string
  /** Score total */
  totalScore: number
  /** Score moyen par partie */
  averageScore: number
  /** Nombre total de parties jouées */
  totalGamesPlayed: number
  /** Nombre de parties complétées */
  gamesCompleted: number
  /** Nombre de parties abandonnées */
  gamesAbandoned: number
  /** Meilleur score obtenu */
  bestScore: number
  /** Temps total de jeu (en secondes) */
  totalPlayTime: number
  /** Temps moyen par partie (en secondes) */
  averagePlayTime: number
  /** Statistiques par niveau de difficulté */
  statsByLevel: Record<number, LevelStats>
}

/**
 * Statistiques pour un niveau de difficulté spécifique
 */
export interface LevelStats {
  /** Niveau de difficulté (1 = Facile, 2 = Moyen, 3 = Difficile) */
  difficultyLevel: number
  /** Nombre de parties jouées à ce niveau */
  gamesPlayed: number
  /** Nombre de parties complétées à ce niveau */
  gamesCompleted: number
  /** Meilleur score à ce niveau */
  bestScore: number
  /** Score moyen à ce niveau */
  averageScore: number
  /** Temps moyen à ce niveau (en secondes) */
  averageTime: number
}

/**
 * Entrée du classement (leaderboard)
 */
export interface LeaderboardEntry {
  /** Position dans le classement */
  rank: number
  /** ID de l'utilisateur */
  userId: number
  /** Nom de l'utilisateur */
  userName: string
  /** Score total */
  totalScore: number
  /** Nombre de parties complétées */
  gamesCompleted: number
  /** Score moyen */
  averageScore: number
  /** Meilleur score */
  bestScore: number
}

/**
 * Statut d'une partie de Tic-Tac-Toe
 */
export enum TicTacToeGameStatus {
  /** En attente d'un deuxième joueur */
  WaitingForPlayer = 1,
  /** Partie en cours */
  InProgress = 2,
  /** Partie terminée avec un gagnant */
  Completed = 3,
  /** Match nul */
  Draw = 4,
  /** Partie abandonnée */
  Abandoned = 5
}

/**
 * Mode de jeu pour Tic-Tac-Toe
 */
export enum TicTacToeGameMode {
  /** Partie contre un autre joueur (multijoueur) */
  Player = 1,
  /** Partie contre l'ordinateur (IA) */
  AI = 2
}

/**
 * Représente une partie de Tic-Tac-Toe
 */
export interface TicTacToeGame {
  /** Identifiant unique de la partie */
  id: number
  /** ID de la session du joueur 1 (X) */
  player1SessionId: number
  /** Nom du joueur 1 */
  player1Name?: string
  /** ID de la session du joueur 2 (O) */
  player2SessionId?: number
  /** Nom du joueur 2 */
  player2Name?: string
  /** État de la grille 3x3 (9 éléments) */
  board: string[]
  /** Tour actuel : 1 = joueur 1 (X), 2 = joueur 2 (O) */
  currentPlayer: number
  /** Statut de la partie */
  status: TicTacToeGameStatus
  /** ID du joueur gagnant (1 ou 2), null si match nul ou en cours */
  winnerPlayerId?: number
  /** Date de création */
  createdAt: Date
  /** Date de début */
  startedAt?: Date
  /** Date de fin */
  completedAt?: Date
  /** Temps écoulé en secondes */
  elapsedSeconds: number
  /** Nombre de coups joués */
  moveCount: number
  /** Mode de jeu : 1 = contre un joueur, 2 = contre l'IA */
  gameMode: TicTacToeGameMode
}

/**
 * DTO pour une session
 */
export interface SessionDto {
  id: number
  userId: number
  user?: {
    id: number
    name: string
    email: string
  }
  sessionToken: string
  createdAt: string
  expiresAt: string
  lastActivityAt: string
  isActive: boolean
  isExpired: boolean
  gameCount: number
}

/**
 * Requête pour créer une nouvelle partie de Tic-Tac-Toe
 */
export interface CreateTicTacToeGameRequest {
  /** ID de la session du joueur */
  sessionId: number
  /** Mode de jeu : 1 = contre un joueur, 2 = contre l'IA */
  gameMode: TicTacToeGameMode
  /** ID de la session du joueur 2 (optionnel, pour inviter un joueur spécifique) */
  player2SessionId?: number
}

/**
 * Requête pour rejoindre une partie
 */
export interface JoinTicTacToeGameRequest {
  /** ID de la partie */
  gameId: number
  /** ID de la session du joueur */
  sessionId: number
}

/**
 * Requête pour jouer un coup
 */
export interface PlayMoveRequest {
  /** Position dans la grille (0-8) */
  position: number
  /** ID de la session du joueur */
  sessionId: number
}

