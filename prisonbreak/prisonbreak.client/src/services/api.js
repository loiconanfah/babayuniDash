/**
 * Service API pour communiquer avec le backend Hashi
 * Version JavaScript (pas TypeScript)
 */

// URL de base de l'API
// En développement avec le proxy SPA, on utilise des URLs relatives
const API_BASE_URL = '/api'

/**
 * Classe d'erreur API
 */
export class ApiError extends Error {
  constructor(message, status, data) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.data = data
  }
}

/**
 * Fonction helper pour gérer les réponses HTTP
 */
async function handleResponse(response) {
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
   * Récupère tous les puzzles
   */
  async getAll() {
    const response = await fetch(`${API_BASE_URL}/puzzles`)
    return handleResponse(response)
  },

  /**
   * Récupère un puzzle par ID
   */
  async getById(id) {
    const response = await fetch(`${API_BASE_URL}/puzzles/${id}`)
    return handleResponse(response)
  },

  /**
   * Récupère les puzzles par difficulté
   */
  async getByDifficulty(difficulty) {
    const response = await fetch(`${API_BASE_URL}/puzzles/difficulty/${difficulty}`)
    return handleResponse(response)
  },

  /**
   * Génère un nouveau puzzle
   */
  async generate(request) {
    const response = await fetch(`${API_BASE_URL}/puzzles/generate`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(request)
    })
    return handleResponse(response)
  }
}

/**
 * Service de gestion des parties
 */
export const gameApi = {
  /**
   * Crée une nouvelle partie
   */
  async create(request) {
    const response = await fetch(`${API_BASE_URL}/games`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(request)
    })
    return handleResponse(response)
  },

  /**
   * Récupère une partie par ID
   */
  async getById(id) {
    const response = await fetch(`${API_BASE_URL}/games/${id}`)
    return handleResponse(response)
  },

  /**
   * Met à jour les ponts
   */
  async updateBridges(gameId, bridges) {
    const response = await fetch(`${API_BASE_URL}/games/${gameId}/bridges`, {
      method: 'PUT',
      headers: {
        'Content-Type': 'application/json'
      },
      body: JSON.stringify(bridges)
    })
    return handleResponse(response)
  },

  /**
   * Valide la solution
   */
  async validate(gameId) {
    const response = await fetch(`${API_BASE_URL}/games/${gameId}/validate`, {
      method: 'POST'
    })
    return handleResponse(response)
  },

  /**
   * Abandonne une partie
   */
  async abandon(gameId) {
    const response = await fetch(`${API_BASE_URL}/games/${gameId}/abandon`, {
      method: 'POST'
    })
    return handleResponse(response)
  }
}

