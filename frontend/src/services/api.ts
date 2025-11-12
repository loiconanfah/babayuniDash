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
  DifficultyLevel
} from '@/types'

// URL de base de l'API
// En développement, le backend tourne sur le port 5001 (ou celui configuré dans launchSettings.json)
const API_BASE_URL = import.meta.env.VITE_API_URL || 'https://localhost:5001/api'

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
    const errorData = await response.json().catch(() => null)
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
    const response = await fetch(`${API_BASE_URL}/puzzles`)
    return handleResponse<Puzzle[]>(response)
  },

  /**
   * Récupère un puzzle par son ID
   */
  async getById(id: number): Promise<Puzzle> {
    const response = await fetch(`${API_BASE_URL}/puzzles/${id}`)
    return handleResponse<Puzzle>(response)
  },

  /**
   * Récupère les puzzles d'un niveau de difficulté
   */
  async getByDifficulty(difficulty: DifficultyLevel): Promise<Puzzle[]> {
    const response = await fetch(`${API_BASE_URL}/puzzles/difficulty/${difficulty}`)
    return handleResponse<Puzzle[]>(response)
  },

  /**
   * Génère un nouveau puzzle
   */
  async generate(request: GeneratePuzzleRequest): Promise<Puzzle> {
    const response = await fetch(`${API_BASE_URL}/puzzles/generate`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(request)
    })
    return handleResponse<Puzzle>(response)
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
    const response = await fetch(`${API_BASE_URL}/games`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(request)
    })
    return handleResponse<Game>(response)
  },

  /**
   * Récupère une partie par son ID
   */
  async getById(id: number): Promise<Game> {
    const response = await fetch(`${API_BASE_URL}/games/${id}`)
    return handleResponse<Game>(response)
  },

  /**
   * Met à jour les ponts placés par le joueur
   */
  async updateBridges(gameId: number, bridges: Bridge[]): Promise<Game> {
    const response = await fetch(`${API_BASE_URL}/games/${gameId}/bridges`, {
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
    const response = await fetch(`${API_BASE_URL}/games/${gameId}/validate`, {
      method: 'POST'
    })
    return handleResponse<ValidationResult>(response)
  },

  /**
   * Abandonne une partie
   */
  async abandon(gameId: number): Promise<Game> {
    const response = await fetch(`${API_BASE_URL}/games/${gameId}/abandon`, {
      method: 'POST'
    })
    return handleResponse<Game>(response)
  }
}

