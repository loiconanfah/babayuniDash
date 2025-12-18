/**
 * Service API pour communiquer avec le backend Hashi
 * Centralise tous les appels HTTP vers l'API REST
 */

import type {
  Puzzle,
  Game,
  Bridge,
  ValidationResult,
  GeneratePuzzleRequest,
  CreateGameRequest,
  DifficultyLevel,
  UserStats,
  LeaderboardEntry,
  TicTacToeGame,
  CreateTicTacToeGameRequest,
  JoinTicTacToeGameRequest,
  PlayMoveRequest,
  ConnectFourGame,
  CreateConnectFourGameRequest,
  JoinConnectFourGameRequest,
  PlayConnectFourMoveRequest,
  RockPaperScissorsGame,
  CreateRPSGameRequest,
  JoinRPSGameRequest,
  PlayRPSChoiceRequest,
  AdventureGame,
  CreateAdventureGameRequest,
  MoveToRoomRequest,
  CollectItemRequest,
  SolvePuzzleRequest,
  PuzzleInfoDto,
  SessionDto
} from '@/types'

// URL de base de l'API
// Si VITE_API_URL est défini (ngrok backend), l'utiliser
// Sinon, utiliser /api (proxy Vite vers localhost:5000)
const API_BASE_URL = import.meta.env.VITE_API_URL || '/api'

/**
 * Classe de gestion des erreurs API
 */
export class ApiError extends Error {
  constructor(
    message: string,
    public status?: number,
    public data?: unknown
  ) {
    super(message)
    this.name = 'ApiError'
  }
}

/**
 * Fonction helper pour obtenir les headers par défaut
 * Inclut le header pour contourner la page d'interception ngrok
 */
function getDefaultHeaders(): HeadersInit {
  return {
    'Accept': 'application/json',
    'Content-Type': 'application/json',
    'ngrok-skip-browser-warning': 'true'
  }
}

/**
 * Fonction utilitaire pour vérifier si une erreur 404 doit déclencher l'ouverture du modal de création de compte
 */
function shouldTrigger404Modal(url: string): boolean {
  // Ne pas déclencher pour les routes qui peuvent légitimement retourner 404
  // (par exemple, les puzzles qui n'existent pas, les jeux spécifiques)
  return !url.includes('/Puzzles/') && 
         !url.includes('/Games/') &&
         !url.includes('/TicTacToe/') &&
         !url.includes('/ConnectFour/') &&
         !url.includes('/RockPaperScissors/') &&
         !url.includes('/Adventure/')
}

/**
 * Fonction helper pour gérer les réponses HTTP
 * Détecte les erreurs 404 et déclenche un événement pour ouvrir le modal de création de compte
 */
async function handleResponse<T>(response: Response, requestUrl?: string): Promise<T> {
  if (!response.ok) {
    let errorData = null
    try {
      const text = await response.text()
      errorData = text ? JSON.parse(text) : null
    } catch {
      // Si la réponse n'est pas du JSON, on ignore
    }
    
    // Si c'est une erreur 404, déclencher un événement pour ouvrir le modal de création de compte
    if (response.status === 404) {
      const url = requestUrl || response.url || ''
      if (shouldTrigger404Modal(url)) {
        // Émettre un événement personnalisé pour ouvrir le modal
        window.dispatchEvent(new CustomEvent('api-404-error', { 
          detail: { url, message: errorData?.message || 'Ressource non trouvée' }
        }))
      }
    }
    
    throw new ApiError(
      errorData?.message || `Erreur HTTP ${response.status}`,
      response.status,
      errorData
    )
  }
  return response.json()
}

/**
 * Service de gestion des puzzles
 */
export const puzzleApi = {
  /**
   * Récupère tous les puzzles disponibles
   */
  async getAll(): Promise<Puzzle[]> {
    try {
      const response = await fetch(`${API_BASE_URL}/Puzzles`, {
        method: 'GET',
        headers: getDefaultHeaders()
      })
      return handleResponse<Puzzle[]>(response)
    } catch (error) {
      console.error('Erreur lors de l\'appel à getAll:', error)
      throw new ApiError(
        error instanceof Error ? error.message : 'Erreur réseau lors de la récupération des puzzles',
        undefined,
        error
      )
    }
  },

  /**
   * Récupère un puzzle par son ID
   */
  async getById(id: number): Promise<Puzzle> {
    const response = await fetch(`${API_BASE_URL}/Puzzles/${id}`, {
      headers: getDefaultHeaders()
    })
    return handleResponse<Puzzle>(response)
  },

  /**
   * Récupère les puzzles d'un niveau de difficulté
   */
  async getByDifficulty(difficulty: DifficultyLevel): Promise<Puzzle[]> {
    const response = await fetch(`${API_BASE_URL}/Puzzles/difficulty/${difficulty}`, {
      headers: getDefaultHeaders()
    })
    return handleResponse<Puzzle[]>(response)
  },

  /**
   * Génère un nouveau puzzle
   */
  async generate(request: GeneratePuzzleRequest): Promise<Puzzle> {
    const response = await fetch(`${API_BASE_URL}/Puzzles/generate`, {
      method: 'POST',
      headers: getDefaultHeaders(),
      body: JSON.stringify(request)
    })
    return handleResponse<Puzzle>(response)
  },

  /**
   * Récupère la solution d'un puzzle (les ponts de la solution)
   */
  async getSolution(puzzleId: number): Promise<Bridge[]> {
    const response = await fetch(`${API_BASE_URL}/Puzzles/${puzzleId}/solution`, {
      headers: getDefaultHeaders()
    })
    return handleResponse<Bridge[]>(response)
  }
}

/**
 * Service de gestion des parties
 */
export const gameApi = {
  /**
   * Crée une nouvelle partie
   */
  async create(request: CreateGameRequest): Promise<Game> {
    const response = await fetch(`${API_BASE_URL}/Games`, {
      method: 'POST',
      headers: getDefaultHeaders(),
      body: JSON.stringify({
        puzzleId: request.puzzleId,
        sessionId: request.sessionId
      })
    })
    return handleResponse<Game>(response)
  },

  /**
   * Récupère une partie par son ID
   */
  async getById(id: number): Promise<Game> {
    const response = await fetch(`${API_BASE_URL}/Games/${id}`, {
      headers: getDefaultHeaders()
    })
    return handleResponse<Game>(response)
  },

  /**
   * Met à jour les ponts placés par le joueur
   */
  async updateBridges(gameId: number, bridges: Bridge[]): Promise<Game> {
    const response = await fetch(`${API_BASE_URL}/Games/${gameId}/bridges`, {
      method: 'PUT',
      headers: getDefaultHeaders(),
      body: JSON.stringify(bridges)
    })
    return handleResponse<Game>(response)
  },

  /**
   * Valide la solution actuelle
   */
  async validate(gameId: number): Promise<ValidationResult> {
    const response = await fetch(`${API_BASE_URL}/Games/${gameId}/validate`, {
      method: 'POST'
    })
    return handleResponse<ValidationResult>(response)
  },

  /**
   * Abandonne une partie
   */
  async abandon(gameId: number): Promise<Game> {
    const response = await fetch(`${API_BASE_URL}/Games/${gameId}/abandon`, {
      method: 'POST'
    })
    return handleResponse<Game>(response)
  }
}

/**
 * Service de gestion des statistiques et du classement
 */
export const statsApi = {
  /**
   * Récupère les statistiques d'un utilisateur par son ID
   */
  async getUserStats(userId: number): Promise<UserStats> {
    const response = await fetch(`${API_BASE_URL}/Stats/user/${userId}`, {
      headers: getDefaultHeaders()
    })
    return handleResponse<UserStats>(response)
  },

  /**
   * Récupère les statistiques d'un utilisateur par son email
   */
  async getUserStatsByEmail(email: string): Promise<UserStats> {
    const response = await fetch(`${API_BASE_URL}/Stats/user/email/${encodeURIComponent(email)}`, {
      headers: getDefaultHeaders()
    })
    return handleResponse<UserStats>(response)
  },

  /**
   * Récupère le classement des meilleurs joueurs
   */
  async getLeaderboard(limit: number = 10): Promise<LeaderboardEntry[]> {
    const response = await fetch(`${API_BASE_URL}/Stats/leaderboard?limit=${limit}`, {
      headers: getDefaultHeaders()
    })
    return handleResponse<LeaderboardEntry[]>(response)
  }
}

/**
 * Service de gestion des sessions
 */
export const sessionsApi = {
  /**
   * Récupère toutes les sessions actives (utilisateurs en ligne)
   */
  async getActiveSessions(excludeSessionId?: number): Promise<SessionDto[]> {
    const url = excludeSessionId 
      ? `${API_BASE_URL}/Sessions/active?excludeSessionId=${excludeSessionId}`
      : `${API_BASE_URL}/Sessions/active`
    const response = await fetch(url)
    return handleResponse<SessionDto[]>(response)
  }
}

/**
 * Service de gestion des parties de Tic-Tac-Toe
 */
export const ticTacToeApi = {
  /**
   * Crée une nouvelle partie de Tic-Tac-Toe
   */
  async create(request: CreateTicTacToeGameRequest): Promise<TicTacToeGame> {
    const response = await fetch(`${API_BASE_URL}/TicTacToe`, {
      method: 'POST',
      headers: getDefaultHeaders(),
      body: JSON.stringify(request)
    })
    return handleResponse<TicTacToeGame>(response)
  },

  /**
   * Récupère une partie par son ID
   */
  async getById(id: number): Promise<TicTacToeGame> {
    const response = await fetch(`${API_BASE_URL}/TicTacToe/${id}`, {
      headers: getDefaultHeaders()
    })
    return handleResponse<TicTacToeGame>(response)
  },

  /**
   * Récupère les parties en attente d'un deuxième joueur
   */
  async getAvailableGames(): Promise<TicTacToeGame[]> {
    const response = await fetch(`${API_BASE_URL}/TicTacToe/available`, {
      headers: getDefaultHeaders()
    })
    return handleResponse<TicTacToeGame[]>(response)
  },

  /**
   * Récupère les parties où le joueur est invité
   */
  async getInvitations(sessionId: number): Promise<TicTacToeGame[]> {
    const response = await fetch(`${API_BASE_URL}/TicTacToe/invitations/${sessionId}`, {
      headers: getDefaultHeaders()
    })
    return handleResponse<TicTacToeGame[]>(response)
  },

  /**
   * Rejoint une partie existante
   */
  async joinGame(gameId: number, sessionId: number, wager: number = 0): Promise<TicTacToeGame> {
    const response = await fetch(`${API_BASE_URL}/TicTacToe/${gameId}/join`, {
      method: 'POST',
      headers: getDefaultHeaders(),
      body: JSON.stringify({ gameId, sessionId, wager })
    })
    return handleResponse<TicTacToeGame>(response)
  },

  /**
   * Joue un coup dans la partie
   */
  async playMove(gameId: number, request: PlayMoveRequest): Promise<TicTacToeGame> {
    const response = await fetch(`${API_BASE_URL}/TicTacToe/${gameId}/move`, {
      method: 'POST',
      headers: getDefaultHeaders(),
      body: JSON.stringify(request)
    })
    return handleResponse<TicTacToeGame>(response)
  },

  /**
   * Abandonne une partie
   */
  async abandon(gameId: number, sessionId: number): Promise<TicTacToeGame> {
    const response = await fetch(`${API_BASE_URL}/TicTacToe/${gameId}/abandon`, {
      method: 'POST',
      headers: getDefaultHeaders(),
      body: JSON.stringify({ gameId, sessionId })
    })
    return handleResponse<TicTacToeGame>(response)
  }
}

/**
 * Service de gestion des parties de Connect Four
 */
export const connectFourApi = {
  /**
   * Crée une nouvelle partie de Connect Four
   */
  async create(request: CreateConnectFourGameRequest): Promise<ConnectFourGame> {
    const response = await fetch(`${API_BASE_URL}/ConnectFour`, {
      method: 'POST',
      headers: getDefaultHeaders(),
      body: JSON.stringify(request)
    })
    return handleResponse<ConnectFourGame>(response)
  },

  /**
   * Récupère une partie par son ID
   */
  async getById(id: number): Promise<ConnectFourGame> {
    const response = await fetch(`${API_BASE_URL}/ConnectFour/${id}`, {
      headers: getDefaultHeaders()
    })
    return handleResponse<ConnectFourGame>(response)
  },

  /**
   * Récupère les parties en attente d'un deuxième joueur
   */
  async getAvailableGames(): Promise<ConnectFourGame[]> {
    const response = await fetch(`${API_BASE_URL}/ConnectFour/available`, {
      headers: getDefaultHeaders()
    })
    return handleResponse<ConnectFourGame[]>(response)
  },

  /**
   * Récupère les parties où le joueur est invité
   */
  async getInvitations(sessionId: number): Promise<ConnectFourGame[]> {
    const response = await fetch(`${API_BASE_URL}/ConnectFour/invitations/${sessionId}`, {
      headers: getDefaultHeaders()
    })
    return handleResponse<ConnectFourGame[]>(response)
  },

  /**
   * Rejoint une partie existante
   */
  async joinGame(gameId: number, sessionId: number, wager: number = 0): Promise<ConnectFourGame> {
    const response = await fetch(`${API_BASE_URL}/ConnectFour/${gameId}/join`, {
      method: 'POST',
      headers: getDefaultHeaders(),
      body: JSON.stringify({ gameId, sessionId, wager })
    })
    return handleResponse<ConnectFourGame>(response)
  },

  /**
   * Joue un coup dans la partie (laisse tomber une pièce dans une colonne)
   */
  async playMove(gameId: number, request: PlayConnectFourMoveRequest): Promise<ConnectFourGame> {
    const response = await fetch(`${API_BASE_URL}/ConnectFour/${gameId}/move`, {
      method: 'POST',
      headers: getDefaultHeaders(),
      body: JSON.stringify(request)
    })
    return handleResponse<ConnectFourGame>(response)
  },

  /**
   * Abandonne une partie
   */
  async abandon(gameId: number, sessionId: number): Promise<ConnectFourGame> {
    const response = await fetch(`${API_BASE_URL}/ConnectFour/${gameId}/abandon`, {
      method: 'POST',
      headers: getDefaultHeaders(),
      body: JSON.stringify({ gameId, sessionId })
    })
    return handleResponse<ConnectFourGame>(response)
  }
}

/**
 * Service de gestion des parties de Rock Paper Scissors (Pierre-Papier-Ciseaux)
 */
export const rpsApi = {
  /**
   * Crée une nouvelle partie de Rock Paper Scissors
   */
  async create(request: CreateRPSGameRequest): Promise<RockPaperScissorsGame> {
    const response = await fetch(`${API_BASE_URL}/RockPaperScissors`, {
      method: 'POST',
      headers: getDefaultHeaders(),
      body: JSON.stringify(request)
    })
    return handleResponse<RockPaperScissorsGame>(response)
  },

  /**
   * Récupère une partie par son ID
   */
  async getById(id: number, sessionId?: number): Promise<RockPaperScissorsGame> {
    const url = sessionId 
      ? `${API_BASE_URL}/RockPaperScissors/${id}?sessionId=${sessionId}`
      : `${API_BASE_URL}/RockPaperScissors/${id}`
    const response = await fetch(url)
    return handleResponse<RockPaperScissorsGame>(response)
  },

  /**
   * Récupère les parties en attente d'un deuxième joueur
   */
  async getAvailableGames(): Promise<RockPaperScissorsGame[]> {
    const response = await fetch(`${API_BASE_URL}/RockPaperScissors/available`, {
      headers: getDefaultHeaders()
    })
    return handleResponse<RockPaperScissorsGame[]>(response)
  },

  /**
   * Récupère les parties où le joueur est invité
   */
  async getInvitations(sessionId: number): Promise<RockPaperScissorsGame[]> {
    const response = await fetch(`${API_BASE_URL}/RockPaperScissors/invitations/${sessionId}`, {
      headers: getDefaultHeaders()
    })
    return handleResponse<RockPaperScissorsGame[]>(response)
  },

  /**
   * Rejoint une partie existante
   */
  async joinGame(gameId: number, sessionId: number, wager: number = 0): Promise<RockPaperScissorsGame> {
    const response = await fetch(`${API_BASE_URL}/RockPaperScissors/${gameId}/join`, {
      method: 'POST',
      headers: getDefaultHeaders(),
      body: JSON.stringify({ gameId, sessionId, wager })
    })
    return handleResponse<RockPaperScissorsGame>(response)
  },

  /**
   * Joue un choix (Rock, Paper, ou Scissors)
   */
  async playChoice(gameId: number, request: PlayRPSChoiceRequest): Promise<RockPaperScissorsGame> {
    const response = await fetch(`${API_BASE_URL}/RockPaperScissors/${gameId}/choice`, {
      method: 'POST',
      headers: getDefaultHeaders(),
      body: JSON.stringify(request)
    })
    return handleResponse<RockPaperScissorsGame>(response)
  },

  /**
   * Passe au round suivant
   */
  async nextRound(gameId: number, sessionId: number): Promise<RockPaperScissorsGame> {
    const response = await fetch(`${API_BASE_URL}/RockPaperScissors/${gameId}/next-round`, {
      method: 'POST',
      headers: getDefaultHeaders(),
      body: JSON.stringify({ gameId, sessionId })
    })
    return handleResponse<RockPaperScissorsGame>(response)
  },

  /**
   * Abandonne une partie
   */
  async abandon(gameId: number, sessionId: number): Promise<RockPaperScissorsGame> {
    const response = await fetch(`${API_BASE_URL}/RockPaperScissors/${gameId}/abandon`, {
      method: 'POST',
      headers: getDefaultHeaders(),
      body: JSON.stringify({ gameId, sessionId })
    })
    return handleResponse<RockPaperScissorsGame>(response)
  }
}

/**
 * Service de gestion des parties d'aventure
 */
export const adventureApi = {
  /**
   * Crée une nouvelle partie d'aventure
   */
  async create(request: CreateAdventureGameRequest): Promise<AdventureGame> {
    const response = await fetch(`${API_BASE_URL}/Adventure`, {
      method: 'POST',
      headers: getDefaultHeaders(),
      body: JSON.stringify(request)
    })
    return handleResponse<AdventureGame>(response)
  },

  /**
   * Récupère une partie par son ID
   */
  async getById(id: number): Promise<AdventureGame> {
    const response = await fetch(`${API_BASE_URL}/Adventure/${id}`, {
      headers: getDefaultHeaders()
    })
    return handleResponse<AdventureGame>(response)
  },

  /**
   * Récupère les parties d'un joueur
   */
  async getGamesBySession(sessionId: number): Promise<AdventureGame[]> {
    const response = await fetch(`${API_BASE_URL}/Adventure/session/${sessionId}`, {
      headers: getDefaultHeaders()
    })
    return handleResponse<AdventureGame[]>(response)
  },

  /**
   * Se déplace vers une salle
   */
  async moveToRoom(gameId: number, request: MoveToRoomRequest): Promise<AdventureGame> {
    const response = await fetch(`${API_BASE_URL}/Adventure/${gameId}/move`, {
      method: 'POST',
      headers: getDefaultHeaders(),
      body: JSON.stringify(request)
    })
    return handleResponse<AdventureGame>(response)
  },

  /**
   * Collecte un objet
   */
  async collectItem(gameId: number, request: CollectItemRequest): Promise<AdventureGame> {
    const response = await fetch(`${API_BASE_URL}/Adventure/${gameId}/collect`, {
      method: 'POST',
      headers: getDefaultHeaders(),
      body: JSON.stringify(request)
    })
    return handleResponse<AdventureGame>(response)
  },

  /**
   * Résout une énigme
   */
  async solvePuzzle(gameId: number, request: SolvePuzzleRequest): Promise<AdventureGame> {
    const response = await fetch(`${API_BASE_URL}/Adventure/${gameId}/solve`, {
      method: 'POST',
      headers: getDefaultHeaders(),
      body: JSON.stringify(request)
    })
    return handleResponse<AdventureGame>(response)
  },

  /**
   * Récupère les informations d'une énigme
   */
  async getPuzzleInfo(puzzleId: number): Promise<PuzzleInfoDto> {
    const response = await fetch(`${API_BASE_URL}/Adventure/puzzle/${puzzleId}`, {
      headers: getDefaultHeaders()
    })
    return handleResponse<PuzzleInfoDto>(response)
  },

  /**
   * Abandonne une partie
   */
  async abandon(gameId: number, sessionId: number): Promise<AdventureGame> {
    const response = await fetch(`${API_BASE_URL}/Adventure/${gameId}/abandon`, {
      method: 'POST',
      headers: getDefaultHeaders(),
      body: JSON.stringify({ sessionId })
    })
    return handleResponse<AdventureGame>(response)
  }
}

