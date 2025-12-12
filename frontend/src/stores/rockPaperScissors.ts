/**
 * Store Pinia pour la gestion des parties de Rock Paper Scissors
 * Centralise l'état des parties multijoueurs
 */

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { RockPaperScissorsGame } from '@/types'
import { RPSGameStatus } from '@/types'
import { rpsApi } from '@/services/api'

export const useRockPaperScissorsStore = defineStore('rockPaperScissors', () => {
  // ====================================================
  // STATE - État réactif
  // ====================================================

  /** Partie actuellement en cours */
  const currentGame = ref<RockPaperScissorsGame | null>(null)

  /** Parties disponibles (en attente d'un joueur) */
  const availableGames = ref<RockPaperScissorsGame[]>([])

  /** Parties où le joueur est invité */
  const invitations = ref<RockPaperScissorsGame[]>([])

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

  /** Indique si on peut jouer (en attente de notre choix) */
  const canPlay = computed(() => {
    if (!currentGame.value || !playerNumber.value) return false
    if (currentGame.value.status !== RPSGameStatus.WaitingForChoices) return false
    
    // Vérifier si on a déjà joué
    if (playerNumber.value === 1) {
      return !currentGame.value.player1Choice
    } else {
      return !currentGame.value.player2Choice
    }
  })

  /** Indique si la partie est terminée */
  const isGameOver = computed(() => {
    if (!currentGame.value) return false
    return currentGame.value.status === RPSGameStatus.Completed ||
           currentGame.value.status === RPSGameStatus.Abandoned
  })

  /** Indique si le round est terminé */
  const isRoundCompleted = computed(() => {
    if (!currentGame.value) return false
    return currentGame.value.status === RPSGameStatus.RoundCompleted
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

      const game = await rpsApi.create({
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

      const game = await rpsApi.getById(gameId, currentSessionId.value || undefined)
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
      const game = await rpsApi.getById(currentGame.value.id, currentSessionId.value || undefined)
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

      const games = await rpsApi.getAvailableGames()
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
      const games = await rpsApi.getInvitations(currentSessionId.value)
      invitations.value = games
      
      // Si une invitation existe et qu'aucune partie n'est en cours, charger automatiquement la première invitation
      if (games.length > 0 && !currentGame.value) {
        const invitation = games[0]
        if (invitation.status === RPSGameStatus.WaitingForChoices || 
            invitation.status === RPSGameStatus.RoundCompleted) {
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

      const game = await rpsApi.joinGame(gameId, currentSessionId.value, wager)
      currentGame.value = game
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors de la jonction à la partie'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Joue un choix (Rock, Paper, ou Scissors)
   */
  async function playChoice(choice: number): Promise<void> {
    if (!currentGame.value || !currentSessionId.value) {
      throw new Error('Aucune partie en cours')
    }

    if (!canPlay.value) {
      throw new Error("Vous ne pouvez pas jouer maintenant")
    }

    try {
      isLoading.value = true
      error.value = null

      const game = await rpsApi.playChoice(currentGame.value.id, {
        sessionId: currentSessionId.value,
        choice
      })
      currentGame.value = game
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors du choix'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Passe au round suivant
   */
  async function nextRound(): Promise<void> {
    if (!currentGame.value || !currentSessionId.value) {
      throw new Error('Aucune partie en cours')
    }

    try {
      isLoading.value = true
      error.value = null

      const game = await rpsApi.nextRound(currentGame.value.id, currentSessionId.value)
      currentGame.value = game
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors du passage au round suivant'
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
      await rpsApi.abandon(currentGame.value.id, currentSessionId.value)
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
  function clearError(): void {
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
    canPlay,
    isGameOver,
    isRoundCompleted,
    
    // Actions
    setSessionId,
    createGame,
    loadGame,
    refreshGame,
    loadAvailableGames,
    loadInvitations,
    joinGame,
    playChoice,
    nextRound,
    abandonGame,
    resetGame,
    clearError
  }
})

