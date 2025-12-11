/**
 * Store Pinia pour la gestion des parties d'aventure
 * Centralise l'état des parties d'exploration et d'énigmes
 */

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { AdventureGame } from '@/types'
import { AdventureGameStatus } from '@/types'
import { adventureApi } from '@/services/api'

export const useAdventureStore = defineStore('adventure', () => {
  // ====================================================
  // STATE - État réactif
  // ====================================================

  /** Partie actuellement en cours */
  const currentGame = ref<AdventureGame | null>(null)

  /** Parties du joueur */
  const playerGames = ref<AdventureGame[]>([])

  /** Indique si le jeu est en chargement */
  const isLoading = ref(false)

  /** Message d'erreur éventuel */
  const error = ref<string | null>(null)

  /** ID de la session actuelle */
  const currentSessionId = ref<number | null>(null)

  /** Indique si la partie est terminée */
  const isGameOver = computed(() => {
    if (!currentGame.value) return false
    return currentGame.value.status === AdventureGameStatus.Completed ||
           currentGame.value.status === AdventureGameStatus.Abandoned
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
  async function createGame(): Promise<void> {
    if (!currentSessionId.value) {
      throw new Error('Session ID non défini')
    }

    try {
      isLoading.value = true
      error.value = null

      const game = await adventureApi.create({
        sessionId: currentSessionId.value
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

      const game = await adventureApi.getById(gameId)
      currentGame.value = game
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors du chargement de la partie'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Charge les parties du joueur
   */
  async function loadPlayerGames(): Promise<void> {
    if (!currentSessionId.value) return

    try {
      isLoading.value = true
      error.value = null

      const games = await adventureApi.getGamesBySession(currentSessionId.value)
      playerGames.value = games
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors du chargement des parties'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Se déplace vers une salle
   */
  async function moveToRoom(roomNumber: number): Promise<void> {
    if (!currentGame.value || !currentSessionId.value) {
      throw new Error('Aucune partie en cours')
    }

    try {
      isLoading.value = true
      error.value = null

      const game = await adventureApi.moveToRoom(currentGame.value.id, {
        sessionId: currentSessionId.value,
        roomNumber
      })
      currentGame.value = game
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors du déplacement'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Collecte un objet
   */
  async function collectItem(itemName: string): Promise<void> {
    if (!currentGame.value || !currentSessionId.value) {
      throw new Error('Aucune partie en cours')
    }

    try {
      isLoading.value = true
      error.value = null

      const game = await adventureApi.collectItem(currentGame.value.id, {
        sessionId: currentSessionId.value,
        itemName
      })
      currentGame.value = game
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors de la collecte'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Résout une énigme
   */
  async function solvePuzzle(puzzleId: number, answer: string): Promise<void> {
    if (!currentGame.value || !currentSessionId.value) {
      throw new Error('Aucune partie en cours')
    }

    try {
      isLoading.value = true
      error.value = null

      const game = await adventureApi.solvePuzzle(currentGame.value.id, {
        sessionId: currentSessionId.value,
        puzzleId,
        answer
      })
      currentGame.value = game
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors de la résolution'
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
      await adventureApi.abandon(currentGame.value.id, currentSessionId.value)
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
    playerGames,
    isLoading,
    error,
    currentSessionId,
    
    // Computed
    isGameOver,
    
    // Actions
    setSessionId,
    createGame,
    loadGame,
    loadPlayerGames,
    moveToRoom,
    collectItem,
    solvePuzzle,
    abandonGame,
    resetGame,
    clearError
  }
})

