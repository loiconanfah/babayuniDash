/**
 * Store Pinia pour la gestion des parties de Connect Four
 * Centralise l'état des parties multijoueurs
 */

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { ConnectFourGame } from '@/types'
import { ConnectFourGameStatus } from '@/types'
import { connectFourApi } from '@/services/api'

// Valeurs numériques pour les statuts
const GameStatus = {
  WaitingForPlayer: 1,
  InProgress: 2,
  Completed: 3,
  Draw: 4,
  Abandoned: 5
} as const

export const useConnectFourStore = defineStore('connectFour', () => {
  // ====================================================
  // STATE - État réactif
  // ====================================================

  /** Partie actuellement en cours */
  const currentGame = ref<ConnectFourGame | null>(null)

  /** Parties disponibles (en attente d'un joueur) */
  const availableGames = ref<ConnectFourGame[]>([])

  /** Parties où le joueur est invité */
  const invitations = ref<ConnectFourGame[]>([])

  /** Indique si le jeu est en chargement */
  const isLoading = ref(false)

  /** Message d'erreur éventuel */
  const error = ref<string | null>(null)

  /** ID de la session actuelle */
  const currentSessionId = ref<number | null>(null)

  /** Indique si on est le joueur 1 ou 2 */
  const playerNumber = computed(() => {
    if (!currentGame.value || !currentSessionId.value) return null
    if (currentGame.value.player1SessionId === currentSessionId.value) return 1
    if (currentGame.value.player2SessionId === currentSessionId.value) return 2
    return null
  })

  /** Indique si c'est notre tour */
  const isMyTurn = computed(() => {
    if (!currentGame.value || !playerNumber.value) return false
    return currentGame.value.currentPlayer === playerNumber.value &&
           currentGame.value.status === ConnectFourGameStatus.InProgress
  })

  /** Indique si la partie est terminée */
  const isGameOver = computed(() => {
    if (!currentGame.value) return false
    return currentGame.value.status === ConnectFourGameStatus.Completed ||
           currentGame.value.status === ConnectFourGameStatus.Draw ||
           currentGame.value.status === ConnectFourGameStatus.Abandoned
  })

  // ====================================================
  // ACTIONS - Méthodes pour modifier l'état
  // ====================================================

  /**
   * Définit l'ID de la session actuelle
   */
  function setSessionId(sessionId: number) {
    currentSessionId.value = sessionId
  }

  /**
   * Crée une nouvelle partie
   */
  async function createGame(gameMode: number, player2SessionId?: number, wager: number = 0): Promise<void> {
    if (!currentSessionId.value) {
      throw new Error('Session ID non défini')
    }

    try {
      isLoading.value = true
      error.value = null

      const game = await connectFourApi.create({
        sessionId: currentSessionId.value,
        gameMode,
        player2SessionId,
        wager
      })
      currentGame.value = game
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors de la création de la partie'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Charge une partie par son ID
   */
  async function loadGame(gameId: number): Promise<void> {
    try {
      isLoading.value = true
      error.value = null

      const game = await connectFourApi.getById(gameId)
      currentGame.value = game
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors du chargement de la partie'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Rafraîchit l'état de la partie actuelle
   */
  async function refreshGame(): Promise<void> {
    if (!currentGame.value) return

    try {
      const game = await connectFourApi.getById(currentGame.value.id)
      currentGame.value = game
    } catch (err) {
      console.error('Erreur lors du rafraîchissement de la partie:', err)
    }
  }

  /**
   * Charge les parties disponibles
   */
  async function loadAvailableGames(): Promise<void> {
    try {
      isLoading.value = true
      error.value = null

      const games = await connectFourApi.getAvailableGames()
      availableGames.value = games
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors du chargement des parties disponibles'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Charge les invitations pour la session actuelle
   */
  async function loadInvitations(): Promise<void> {
    if (!currentSessionId.value) return

    try {
      const games = await connectFourApi.getInvitations(currentSessionId.value)
      invitations.value = games
      
      // Si une invitation existe et qu'aucune partie n'est en cours, charger automatiquement la première invitation
      if (games.length > 0 && !currentGame.value) {
        const invitation = games[0]
        // Charger la partie si elle est en cours (InProgress = 2)
        if (invitation && invitation.status === 2) {
          await loadGame(invitation.id)
        }
      }
    } catch (err) {
      console.error('Erreur lors du chargement des invitations:', err)
    }
  }

  /**
   * Rejoint une partie existante
   */
  async function joinGame(gameId: number, wager: number = 0): Promise<void> {
    if (!currentSessionId.value) {
      throw new Error('Session ID non défini')
    }

    try {
      isLoading.value = true
      error.value = null

      const game = await connectFourApi.joinGame(gameId, currentSessionId.value, wager)
      currentGame.value = game
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors de la jonction à la partie'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Joue un coup (laisse tomber une pièce dans une colonne)
   */
  async function playMove(column: number): Promise<void> {
    if (!currentGame.value || !currentSessionId.value) {
      throw new Error('Aucune partie en cours')
    }

    if (!isMyTurn.value) {
      throw new Error("Ce n'est pas votre tour")
    }

    try {
      isLoading.value = true
      error.value = null

      const game = await connectFourApi.playMove(currentGame.value.id, {
        column,
        sessionId: currentSessionId.value
      })
      currentGame.value = game
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors du coup joué'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Abandonne la partie actuelle
   */
  async function abandonGame(): Promise<void> {
    if (!currentGame.value || !currentSessionId.value) return

    try {
      await connectFourApi.abandon(currentGame.value.id, currentSessionId.value)
      await loadGame(currentGame.value.id) // Recharger pour avoir l'état final
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Erreur lors de l'abandon de la partie"
      throw err
    }
  }

  /**
   * Réinitialise l'état du store
   */
  function resetGame(): void {
    currentGame.value = null
    error.value = null
  }

  /**
   * Efface l'erreur actuelle
   */
  function clearError() {
    error.value = null
  }

  return {
    // State
    currentGame,
    availableGames,
    invitations,
    isLoading,
    error,
    currentSessionId,
    
    // Computed
    playerNumber,
    isMyTurn,
    isGameOver,
    
    // Actions
    setSessionId,
    createGame,
    loadGame,
    refreshGame,
    loadAvailableGames,
    loadInvitations,
    joinGame,
    playMove,
    abandonGame,
    resetGame,
    clearError
  }
})

