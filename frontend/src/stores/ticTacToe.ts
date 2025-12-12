/**
 * Store Pinia pour la gestion des parties de Tic-Tac-Toe
 * Centralise l'état des parties multijoueurs
 */

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { TicTacToeGame } from '@/types'
import { TicTacToeGameStatus } from '@/types'
import { ticTacToeApi } from '@/services/api'

// Valeurs numériques pour les statuts
const GameStatus = {
  WaitingForPlayer: 1,
  InProgress: 2,
  Completed: 3,
  Draw: 4,
  Abandoned: 5
} as const

export const useTicTacToeStore = defineStore('ticTacToe', () => {
  // ====================================================
  // STATE - État réactif
  // ====================================================

  /** Partie actuellement en cours */
  const currentGame = ref<TicTacToeGame | null>(null)

  /** Parties disponibles (en attente d'un joueur) */
  const availableGames = ref<TicTacToeGame[]>([])

  /** Parties où le joueur est invité */
  const invitations = ref<TicTacToeGame[]>([])

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
    return currentGame.value.currentPlayer === playerNumber.value
  })

  /** Indique si la partie est terminée */
  const isGameOver = computed(() => {
    if (!currentGame.value) return false
    return currentGame.value.status === TicTacToeGameStatus.Completed ||
           currentGame.value.status === TicTacToeGameStatus.Draw ||
           currentGame.value.status === TicTacToeGameStatus.Abandoned
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
  async function createGame(sessionId: number, gameMode: number = 1, player2SessionId?: number, wager: number = 0): Promise<void> {
    try {
      isLoading.value = true
      error.value = null

      const game = await ticTacToeApi.create({ sessionId, gameMode, player2SessionId, wager })
      currentGame.value = game
      currentSessionId.value = sessionId
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors de la création de la partie'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Charge une partie existante
   */
  async function loadGame(gameId: number): Promise<void> {
    try {
      isLoading.value = true
      error.value = null

      const game = await ticTacToeApi.getById(gameId)
      currentGame.value = game
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors du chargement de la partie'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Charge les parties disponibles
   */
  async function loadAvailableGames(): Promise<void> {
    try {
      isLoading.value = true
      error.value = null

      const games = await ticTacToeApi.getAvailableGames()
      availableGames.value = games
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors du chargement des parties disponibles'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Charge les invitations pour le joueur actuel
   */
  async function loadInvitations(sessionId: number): Promise<void> {
    try {
      const games = await ticTacToeApi.getInvitations(sessionId)
      invitations.value = games
      
      // Si une invitation existe et qu'aucune partie n'est en cours, charger automatiquement la première invitation
      if (games.length > 0 && !currentGame.value) {
        const invitation = games[0]
        // Charger la partie si elle est en cours (InProgress = 2)
        if (invitation.status === 2) {
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
  async function joinGame(gameId: number, sessionId: number, wager: number = 0): Promise<void> {
    try {
      isLoading.value = true
      error.value = null

      const game = await ticTacToeApi.joinGame(gameId, sessionId, wager)
      currentGame.value = game
      currentSessionId.value = sessionId
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors de la jonction à la partie'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Joue un coup
   */
  async function playMove(position: number): Promise<void> {
    if (!currentGame.value || !currentSessionId.value) {
      throw new Error('Aucune partie en cours')
    }

    if (!isMyTurn.value) {
      throw new Error("Ce n'est pas votre tour")
    }

    try {
      isLoading.value = true
      error.value = null

      const game = await ticTacToeApi.playMove(currentGame.value.id, {
        position,
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
   * Abandonne la partie
   */
  async function abandonGame(): Promise<void> {
    if (!currentGame.value || !currentSessionId.value) return

    try {
      await ticTacToeApi.abandon(currentGame.value.id, currentSessionId.value)
      await loadGame(currentGame.value.id) // Recharger pour avoir l'état final
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Erreur lors de l'abandon de la partie"
      throw err
    }
  }

  /**
   * Rafraîchit l'état de la partie actuelle
   */
  async function refreshGame(): Promise<void> {
    if (!currentGame.value) return

    try {
      const game = await ticTacToeApi.getById(currentGame.value.id)
      currentGame.value = game
    } catch (err) {
      console.error('Erreur lors du rafraîchissement de la partie:', err)
    }
  }

  /**
   * Réinitialise l'état
   */
  function resetGame(): void {
    currentGame.value = null
    error.value = null
  }

  /**
   * Efface le message d'erreur
   */
  function clearError(): void {
    error.value = null
  }

  // ====================================================
  // RETOUR DES PROPRIÉTÉS ET MÉTHODES PUBLIQUES
  // ====================================================

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
    loadAvailableGames,
    loadInvitations,
    joinGame,
    playMove,
    abandonGame,
    refreshGame,
    resetGame,
    clearError
  }
})

