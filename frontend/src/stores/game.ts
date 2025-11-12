/**
 * Store Pinia pour la gestion de l'état du jeu Hashi
 * Centralise l'état de la partie en cours, les ponts placés, etc.
 */

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Game, Puzzle, Bridge, Island } from '@/types'
import { gameApi } from '@/services/api'

export const useGameStore = defineStore('game', () => {
  // ====================================================
  // STATE - État réactif du jeu
  // ====================================================

  /** Partie actuellement en cours */
  const currentGame = ref<Game | null>(null)

  /** Puzzle actuellement joué */
  const currentPuzzle = ref<Puzzle | null>(null)

  /** Ponts placés par le joueur */
  const playerBridges = ref<Bridge[]>([])

  /** Île actuellement sélectionnée (pour placer des ponts) */
  const selectedIsland = ref<Island | null>(null)

  /** Indique si le jeu est en chargement */
  const isLoading = ref(false)

  /** Message d'erreur éventuel */
  const error = ref<string | null>(null)

  /** Timer pour le temps écoulé */
  const elapsedTime = ref(0)
  let timerInterval: number | null = null

  // ====================================================
  // GETTERS - Propriétés calculées
  // ====================================================

  /**
   * Vérifie si une partie est en cours
   */
  const hasActiveGame = computed(() => currentGame.value !== null)

  /**
   * Récupère une île par son ID
   */
  const getIslandById = computed(() => {
    return (id: number): Island | undefined => {
      return currentPuzzle.value?.islands.find((island) => island.id === id)
    }
  })

  /**
   * Compte le nombre de ponts connectés à une île
   */
  const getBridgeCountForIsland = computed(() => {
    return (islandId: number): number => {
      return playerBridges.value.reduce((count, bridge) => {
        if (bridge.fromIslandId === islandId || bridge.toIslandId === islandId) {
          return count + (bridge.isDouble ? 2 : 1)
        }
        return count
      }, 0)
    }
  })

  /**
   * Vérifie si une île est complète (a le bon nombre de ponts)
   */
  const isIslandComplete = computed(() => {
    return (island: Island): boolean => {
      const currentCount = getBridgeCountForIsland.value(island.id)
      return currentCount === island.requiredBridges
    }
  })

  /**
   * Vérifie si une île peut encore recevoir des ponts
   */
  const canIslandReceiveBridge = computed(() => {
    return (island: Island): boolean => {
      const currentCount = getBridgeCountForIsland.value(island.id)
      return currentCount < island.requiredBridges
    }
  })

  /**
   * Trouve un pont existant entre deux îles
   */
  const findBridgeBetween = computed(() => {
    return (island1Id: number, island2Id: number): Bridge | undefined => {
      return playerBridges.value.find(
        (bridge) =>
          (bridge.fromIslandId === island1Id && bridge.toIslandId === island2Id) ||
          (bridge.fromIslandId === island2Id && bridge.toIslandId === island1Id)
      )
    }
  })

  // ====================================================
  // ACTIONS - Méthodes pour modifier l'état
  // ====================================================

  /**
   * Démarre une nouvelle partie avec un puzzle
   */
  async function startGame(puzzle: Puzzle): Promise<void> {
    try {
      isLoading.value = true
      error.value = null

      // Créer une nouvelle partie via l'API
      const game = await gameApi.create({ puzzleId: puzzle.id })

      currentGame.value = game
      currentPuzzle.value = puzzle
      playerBridges.value = []
      selectedIsland.value = null
      elapsedTime.value = 0

      // Démarrer le timer
      startTimer()
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors du démarrage du jeu'
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

      const game = await gameApi.getById(gameId)

      currentGame.value = game
      currentPuzzle.value = game.puzzle || null
      playerBridges.value = game.playerBridges
      elapsedTime.value = game.elapsedSeconds

      startTimer()
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors du chargement de la partie'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Sélectionne une île (pour commencer à placer un pont)
   */
  function selectIsland(island: Island): void {
    // Si aucune île n'est sélectionnée, sélectionner celle-ci
    if (!selectedIsland.value) {
      selectedIsland.value = island
      return
    }

    // Si on clique sur la même île, la désélectionner
    if (selectedIsland.value.id === island.id) {
      selectedIsland.value = null
      return
    }

    // Sinon, essayer de créer un pont entre les deux îles
    tryCreateBridge(selectedIsland.value, island)
    selectedIsland.value = null
  }

  /**
   * Tente de créer un pont entre deux îles
   */
  function tryCreateBridge(from: Island, to: Island): void {
    // Vérifier si les îles sont alignées (même ligne ou même colonne)
    if (from.x !== to.x && from.y !== to.y) {
      error.value = 'Les ponts doivent être horizontaux ou verticaux'
      return
    }

    // Vérifier s'il existe déjà un pont
    const existingBridge = findBridgeBetween.value(from.id, to.id)

    if (existingBridge) {
      // Si c'est un pont simple, le transformer en pont double
      if (!existingBridge.isDouble) {
        existingBridge.isDouble = true
      } else {
        // Si c'est déjà un pont double, le supprimer
        removeBridge(existingBridge)
      }
    } else {
      // Créer un nouveau pont simple
      const newBridge: Bridge = {
        fromIslandId: from.id,
        toIslandId: to.id,
        isDouble: false
      }
      playerBridges.value.push(newBridge)
    }

    // Sauvegarder l'état sur le serveur
    saveBridges()
  }

  /**
   * Supprime un pont
   */
  function removeBridge(bridge: Bridge): void {
    const index = playerBridges.value.indexOf(bridge)
    if (index > -1) {
      playerBridges.value.splice(index, 1)
      saveBridges()
    }
  }

  /**
   * Sauvegarde les ponts sur le serveur
   */
  async function saveBridges(): Promise<void> {
    if (!currentGame.value) return

    try {
      await gameApi.updateBridges(currentGame.value.id, playerBridges.value)
    } catch (err) {
      console.error('Erreur lors de la sauvegarde des ponts:', err)
    }
  }

  /**
   * Valide la solution actuelle
   */
  async function validateSolution(): Promise<void> {
    if (!currentGame.value) return

    try {
      isLoading.value = true
      const result = await gameApi.validate(currentGame.value.id)

      if (result.isValid) {
        stopTimer()
        // La partie est terminée avec succès
      }

      return result
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors de la validation'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Abandonne la partie en cours
   */
  async function abandonGame(): Promise<void> {
    if (!currentGame.value) return

    try {
      await gameApi.abandon(currentGame.value.id)
      stopTimer()
      resetGame()
    } catch (err) {
      error.value = err instanceof Error ? err.message : "Erreur lors de l'abandon de la partie"
      throw err
    }
  }

  /**
   * Réinitialise l'état du jeu
   */
  function resetGame(): void {
    currentGame.value = null
    currentPuzzle.value = null
    playerBridges.value = []
    selectedIsland.value = null
    elapsedTime.value = 0
    error.value = null
    stopTimer()
  }

  /**
   * Démarre le timer
   */
  function startTimer(): void {
    stopTimer() // Arrêter le timer existant s'il y en a un
    timerInterval = window.setInterval(() => {
      elapsedTime.value++
    }, 1000)
  }

  /**
   * Arrête le timer
   */
  function stopTimer(): void {
    if (timerInterval !== null) {
      clearInterval(timerInterval)
      timerInterval = null
    }
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
    currentPuzzle,
    playerBridges,
    selectedIsland,
    isLoading,
    error,
    elapsedTime,

    // Getters
    hasActiveGame,
    getIslandById,
    getBridgeCountForIsland,
    isIslandComplete,
    canIslandReceiveBridge,
    findBridgeBetween,

    // Actions
    startGame,
    loadGame,
    selectIsland,
    tryCreateBridge,
    removeBridge,
    saveBridges,
    validateSolution,
    abandonGame,
    resetGame,
    clearError
  }
})

