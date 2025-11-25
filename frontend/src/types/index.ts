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

