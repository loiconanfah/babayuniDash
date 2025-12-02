/**
 * Store Pinia pour la gestion de l'état du jeu Hashi
 * Centralise l'état de la partie en cours, les ponts placés, etc.
 */

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { Game, Puzzle, Bridge, Island, ValidationResult } from '@/types'
import { gameApi, puzzleApi } from '@/services/api'

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

  /** Indique si le jeu est en pause */
  const isPaused = ref(false)

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
  function getIslandById(id: number): Island | undefined {
    return currentPuzzle.value?.islands.find((island) => island.id === id)
  }

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
   * Nécessite qu'un utilisateur soit connecté avec une session active
   */
  async function startGame(puzzle: Puzzle, sessionId: number): Promise<void> {
    try {
      isLoading.value = true
      error.value = null

      // Créer une nouvelle partie via l'API
      const game = await gameApi.create({ puzzleId: puzzle.id, sessionId })

      currentGame.value = game
      currentPuzzle.value = puzzle
      playerBridges.value = game.playerBridges || []
      selectedIsland.value = null
      elapsedTime.value = game.elapsedSeconds || 0

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
    // Ne rien faire si le jeu est en pause
    if (isPaused.value) {
      return
    }

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
   * Vérifie si deux ponts se croisent illégalement
   */
  function doBridgesIntersect(bridge1: Bridge, bridge2: Bridge): boolean {
    if (!currentPuzzle.value) return false

    const island1From = getIslandById(bridge1.fromIslandId)
    const island1To = getIslandById(bridge1.toIslandId)
    const island2From = getIslandById(bridge2.fromIslandId)
    const island2To = getIslandById(bridge2.toIslandId)

    if (!island1From || !island1To || !island2From || !island2To) {
      return false
    }

    // Déterminer la direction de chaque pont
    const bridge1IsHorizontal = island1From.y === island1To.y
    const bridge2IsHorizontal = island2From.y === island2To.y

    // Deux ponts parallèles ne peuvent pas se croiser
    if (bridge1IsHorizontal === bridge2IsHorizontal) {
      return false
    }

    // Si le premier pont est horizontal et le second vertical
    if (bridge1IsHorizontal) {
      const bridge1MinX = Math.min(island1From.x, island1To.x)
      const bridge1MaxX = Math.max(island1From.x, island1To.x)
      const bridge1Y = island1From.y

      const bridge2MinY = Math.min(island2From.y, island2To.y)
      const bridge2MaxY = Math.max(island2From.y, island2To.y)
      const bridge2X = island2From.x

      // Vérifier si le pont vertical passe à travers le pont horizontal
      // Exclure les cas où ils se rencontrent à une île (c'est autorisé)
      return (
        bridge2X > bridge1MinX &&
        bridge2X < bridge1MaxX &&
        bridge1Y > bridge2MinY &&
        bridge1Y < bridge2MaxY
      )
    } else {
      // Le premier pont est vertical et le second horizontal - inverser l'appel
      return doBridgesIntersect(bridge2, bridge1)
    }
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

      // Vérifier si ce pont croise un pont existant
      const wouldIntersect = playerBridges.value.some((bridge) =>
        doBridgesIntersect(newBridge, bridge)
      )

      if (wouldIntersect) {
        error.value = 'Ce pont croiserait un pont existant. Les croisements ne sont pas autorisés.'
        return
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
  async function validateSolution(): Promise<ValidationResult> {
    if (!currentGame.value) {
      throw new Error('Aucune partie en cours')
    }

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
    isPaused.value = false
    stopTimer()
  }

  /**
   * Met le jeu en pause
   */
  function pauseGame(): void {
    if (!hasActiveGame.value) return
    isPaused.value = true
    selectedIsland.value = null // Désélectionner l'île en cours
    stopTimer() // Arrêter le timer
  }

  /**
   * Reprend le jeu
   */
  function resumeGame(): void {
    if (!hasActiveGame.value) return
    isPaused.value = false
    startTimer() // Redémarrer le timer
  }

  /**
   * Alterne entre pause et reprise
   */
  function togglePause(): void {
    if (isPaused.value) {
      resumeGame()
    } else {
      pauseGame()
    }
  }

  /**
   * Démarre le timer
   */
  function startTimer(): void {
    stopTimer() // Arrêter le timer existant s'il y en a un
    if (!isPaused.value) {
      timerInterval = window.setInterval(() => {
        elapsedTime.value++
      }, 1000)
    }
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

  /**
   * Résout automatiquement le puzzle en utilisant la solution stockée
   */
  async function solvePuzzle(): Promise<void> {
    if (!currentPuzzle.value) {
      error.value = 'Aucun puzzle en cours'
      return
    }

    try {
      isLoading.value = true
      error.value = null

      // Récupérer la solution depuis l'API
      const solutionBridges = await puzzleApi.getSolution(currentPuzzle.value.id)

      // Appliquer la solution
      playerBridges.value = solutionBridges

      // Sauvegarder les ponts sur le serveur
      await saveBridges()

      // Valider automatiquement la solution
      await validateSolution()
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors de la résolution du puzzle'
      console.error('Erreur lors de la résolution:', err)
    } finally {
      isLoading.value = false
    }
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
    isPaused,

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
    solvePuzzle,
    clearError,
    pauseGame,
    resumeGame,
    togglePause
  }
})

