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
  SessionDto
} from '@/types'

// URL de base de l'API
// En développement avec SPA Proxy : utilise /api (URL relative, proxyfiée automatiquement)
// En développement sans proxy : utilise http://localhost:5000/api
// En production : utilise la variable d'environnement VITE_API_URL ou https://localhost:5001/api
const API_BASE_URL = import.meta.env.VITE_API_URL || 
  (import.meta.env.DEV ? '/api' : 'https://localhost:5001/api')

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
 * Fonction helper pour gérer les réponses HTTP
 */
async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    let errorData = null
    try {
      const text = await response.text()
      errorData = text ? JSON.parse(text) : null
    } catch {
      // Si la réponse n'est pas du JSON, on ignore
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
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json'
        }
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
    const response = await fetch(`${API_BASE_URL}/Puzzles/${id}`)
    return handleResponse<Puzzle>(response)
  },

  /**
   * Récupère les puzzles d'un niveau de difficulté
   */
  async getByDifficulty(difficulty: DifficultyLevel): Promise<Puzzle[]> {
    const response = await fetch(`${API_BASE_URL}/Puzzles/difficulty/${difficulty}`)
    return handleResponse<Puzzle[]>(response)
  },

  /**
   * Génère un nouveau puzzle
   */
  async generate(request: GeneratePuzzleRequest): Promise<Puzzle> {
    const response = await fetch(`${API_BASE_URL}/Puzzles/generate`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(request)
    })
    return handleResponse<Puzzle>(response)
  },

  /**
   * Récupère la solution d'un puzzle (les ponts de la solution)
   */
  async getSolution(puzzleId: number): Promise<Bridge[]> {
    const response = await fetch(`${API_BASE_URL}/Puzzles/${puzzleId}/solution`)
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
      headers: {
        'Content-Type': 'application/json'
      },
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
    const response = await fetch(`${API_BASE_URL}/Games/${id}`)
    return handleResponse<Game>(response)
  },

  /**
   * Met à jour les ponts placés par le joueur
   */
  async updateBridges(gameId: number, bridges: Bridge[]): Promise<Game> {
    const response = await fetch(`${API_BASE_URL}/Games/${gameId}/bridges`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
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
    const response = await fetch(`${API_BASE_URL}/Stats/user/${userId}`)
    return handleResponse<UserStats>(response)
  },

  /**
   * Récupère les statistiques d'un utilisateur par son email
   */
  async getUserStatsByEmail(email: string): Promise<UserStats> {
    const response = await fetch(`${API_BASE_URL}/Stats/user/email/${encodeURIComponent(email)}`)
    return handleResponse<UserStats>(response)
  },

  /**
   * Récupère le classement des meilleurs joueurs
   */
  async getLeaderboard(limit: number = 10): Promise<LeaderboardEntry[]> {
    const response = await fetch(`${API_BASE_URL}/Stats/leaderboard?limit=${limit}`)
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
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(request)
    })
    return handleResponse<TicTacToeGame>(response)
  },

  /**
   * Récupère une partie par son ID
   */
  async getById(id: number): Promise<TicTacToeGame> {
    const response = await fetch(`${API_BASE_URL}/TicTacToe/${id}`)
    return handleResponse<TicTacToeGame>(response)
  },

  /**
   * Récupère les parties en attente d'un deuxième joueur
   */
  async getAvailableGames(): Promise<TicTacToeGame[]> {
    const response = await fetch(`${API_BASE_URL}/TicTacToe/available`)
    return handleResponse<TicTacToeGame[]>(response)
  },

  /**
   * Récupère les parties où le joueur est invité
   */
  async getInvitations(sessionId: number): Promise<TicTacToeGame[]> {
    const response = await fetch(`${API_BASE_URL}/TicTacToe/invitations/${sessionId}`)
    return handleResponse<TicTacToeGame[]>(response)
  },

  /**
   * Rejoint une partie existante
   */
  async joinGame(gameId: number, sessionId: number): Promise<TicTacToeGame> {
    const response = await fetch(`${API_BASE_URL}/TicTacToe/${gameId}/join`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ gameId, sessionId })
    })
    return handleResponse<TicTacToeGame>(response)
  },

  /**
   * Joue un coup dans la partie
   */
  async playMove(gameId: number, request: PlayMoveRequest): Promise<TicTacToeGame> {
    const response = await fetch(`${API_BASE_URL}/TicTacToe/${gameId}/move`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
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
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify({ gameId, sessionId })
    })
    return handleResponse<TicTacToeGame>(response)
  }
}

