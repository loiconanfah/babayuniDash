/**
 * Service API pour les tournois
 */

const API_BASE_URL = import.meta.env.VITE_API_URL || 
  (import.meta.env.DEV ? '/api' : 'https://localhost:5001/api')

export enum TournamentGameType {
  RockPaperScissors = 1,
  TicTacToe = 2,
  ConnectFour = 3
}

export enum TournamentStatus {
  Registration = 1,
  InProgress = 2,
  Completed = 3,
  Cancelled = 4
}

export enum TournamentMatchStatus {
  Pending = 1,
  InProgress = 2,
  Completed = 3,
  Forfeit = 4
}

export interface TournamentDto {
  id: number
  name: string
  description: string
  gameType: TournamentGameType
  status: TournamentStatus
  maxParticipants: number
  currentParticipants: number
  entryFee: number
  totalPrize: number
  firstPlacePrize: number
  secondPlacePrize: number
  thirdPlacePrize: number
  startDate: string
  endDate?: string | null
  createdAt: string
  imageUrl?: string | null
  participants: TournamentParticipantDto[]
  matches: TournamentMatchDto[]
  isUserRegistered: boolean
  userPosition?: number | null
  userParticipant?: TournamentParticipantDto | null
}

export interface TournamentParticipantDto {
  id: number
  tournamentId: number
  userId: number
  userName: string
  userEmail?: string | null
  finalPosition?: number | null
  prizeWon: number
  joinedAt: string
  isEliminated: boolean
}

export interface TournamentMatchDto {
  id: number
  tournamentId: number
  participant1Id: number
  participant1Name: string
  participant2Id: number
  participant2Name: string
  round: number
  winnerId?: number | null
  status: TournamentMatchStatus
  gameId?: number | null
  startedAt?: string | null
  completedAt?: string | null
}

export interface CreateTournamentRequest {
  name: string
  description: string
  gameType: TournamentGameType
  maxParticipants: number
  entryFee: number
  startDate: string
  imageUrl?: string | null
}

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    let errorMessage = `HTTP error! status: ${response.status}`
    try {
      const errorText = await response.text()
      if (errorText) {
        try {
          const errorData = JSON.parse(errorText)
          errorMessage = errorData.message || errorData.title || errorText
        } catch {
          errorMessage = errorText
        }
      }
    } catch {
      // Si on ne peut pas lire l'erreur, utiliser le message par défaut
    }
    throw new Error(errorMessage)
  }
  return response.json()
}

/**
 * Récupère tous les tournois
 */
export async function getAllTournaments(userId?: number): Promise<TournamentDto[]> {
  const url = userId 
    ? `${API_BASE_URL}/Tournaments?userId=${userId}`
    : `${API_BASE_URL}/Tournaments`
  const response = await fetch(url)
  return handleResponse<TournamentDto[]>(response)
}

/**
 * Récupère un tournoi par son ID
 */
export async function getTournamentById(tournamentId: number, userId?: number): Promise<TournamentDto> {
  const url = userId
    ? `${API_BASE_URL}/Tournaments/${tournamentId}?userId=${userId}`
    : `${API_BASE_URL}/Tournaments/${tournamentId}`
  const response = await fetch(url)
  return handleResponse<TournamentDto>(response)
}

/**
 * Crée un nouveau tournoi
 */
export async function createTournament(request: CreateTournamentRequest): Promise<TournamentDto> {
  const response = await fetch(`${API_BASE_URL}/Tournaments`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify(request)
  })
  return handleResponse<TournamentDto>(response)
}

/**
 * Inscrit un utilisateur à un tournoi
 */
export async function registerForTournament(tournamentId: number, userId: number): Promise<TournamentParticipantDto> {
  const response = await fetch(`${API_BASE_URL}/Tournaments/${tournamentId}/register`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ userId })
  })
  return handleResponse<TournamentParticipantDto>(response)
}

/**
 * Désinscrit un utilisateur d'un tournoi
 */
export async function unregisterFromTournament(tournamentId: number, userId: number): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/Tournaments/${tournamentId}/unregister`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ userId })
  })
  await handleResponse(response)
}

/**
 * Démarre un tournoi
 */
export async function startTournament(tournamentId: number): Promise<TournamentDto> {
  const response = await fetch(`${API_BASE_URL}/Tournaments/${tournamentId}/start`, {
    method: 'POST'
  })
  return handleResponse<TournamentDto>(response)
}

/**
 * Récupère les matchs d'un tournoi
 */
export async function getTournamentMatches(tournamentId: number): Promise<TournamentMatchDto[]> {
  const response = await fetch(`${API_BASE_URL}/Tournaments/${tournamentId}/matches`)
  return handleResponse<TournamentMatchDto[]>(response)
}

/**
 * Récupère les matchs d'un utilisateur dans un tournoi
 */
export async function getUserMatches(tournamentId: number, userId: number): Promise<TournamentMatchDto[]> {
  const response = await fetch(`${API_BASE_URL}/Tournaments/${tournamentId}/matches/user/${userId}`)
  return handleResponse<TournamentMatchDto[]>(response)
}

/**
 * Enregistre le résultat d'un match
 */
export async function recordMatchResult(matchId: number, winnerId: number): Promise<TournamentMatchDto> {
  const response = await fetch(`${API_BASE_URL}/Tournaments/matches/${matchId}/result`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ winnerId })
  })
  return handleResponse<TournamentMatchDto>(response)
}

