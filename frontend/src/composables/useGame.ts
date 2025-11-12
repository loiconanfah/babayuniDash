/**
 * Composable pour la logique du jeu
 * Encapsule la logique réutilisable liée au gameplay
 */

import { computed } from 'vue'
import { useGameStore } from '@/stores/game'
import type { Island, Bridge } from '@/types'

/**
 * Hook composable pour gérer la logique du jeu
 * Fournit des fonctions et computed properties réutilisables
 */
export function useGame() {
  const gameStore = useGameStore()

  /**
   * Vérifie si une île peut recevoir plus de ponts
   */
  const canIslandReceiveBridge = (island: Island): boolean => {
    const currentCount = gameStore.getBridgeCountForIsland(island.id)
    return currentCount < island.requiredBridges
  }

  /**
   * Vérifie si une île est complète
   */
  const isIslandComplete = (island: Island): boolean => {
    return gameStore.isIslandComplete(island)
  }

  /**
   * Vérifie si deux îles peuvent être connectées
   * Elles doivent être alignées horizontalement ou verticalement
   */
  const canConnectIslands = (island1: Island, island2: Island): boolean => {
    // Les îles doivent être différentes
    if (island1.id === island2.id) return false

    // Elles doivent être alignées
    const isHorizontallyAligned = island1.y === island2.y
    const isVerticallyAligned = island1.x === island2.x

    return isHorizontallyAligned || isVerticallyAligned
  }

  /**
   * Trouve toutes les îles connectées à une île donnée
   */
  const getConnectedIslands = (islandId: number): Island[] => {
    if (!gameStore.currentPuzzle) return []

    const connectedIds = new Set<number>()

    gameStore.playerBridges.forEach((bridge) => {
      if (bridge.fromIslandId === islandId) {
        connectedIds.add(bridge.toIslandId)
      } else if (bridge.toIslandId === islandId) {
        connectedIds.add(bridge.fromIslandId)
      }
    })

    return gameStore.currentPuzzle.islands.filter((island) =>
      connectedIds.has(island.id)
    )
  }

  /**
   * Calcule le pourcentage de complétion du puzzle
   */
  const completionPercentage = computed((): number => {
    if (!gameStore.currentPuzzle) return 0

    const totalIslands = gameStore.currentPuzzle.islands.length
    const completeIslands = gameStore.currentPuzzle.islands.filter((island) =>
      isIslandComplete(island)
    ).length

    return Math.round((completeIslands / totalIslands) * 100)
  })

  /**
   * Vérifie si le puzzle semble résolu (toutes les îles complètes)
   * Ne garantit pas que c'est valide (pas de croisements, connectivité)
   */
  const isPuzzleSolved = computed((): boolean => {
    if (!gameStore.currentPuzzle) return false

    return gameStore.currentPuzzle.islands.every((island) =>
      isIslandComplete(island)
    )
  })

  /**
   * Calcule les statistiques de la partie
   */
  const gameStats = computed(() => {
    if (!gameStore.currentPuzzle) {
      return {
        totalIslands: 0,
        completeIslands: 0,
        incompleteIslands: 0,
        totalBridges: 0,
        completionPercentage: 0
      }
    }

    const islands = gameStore.currentPuzzle.islands
    const completeIslands = islands.filter((island) => isIslandComplete(island))

    return {
      totalIslands: islands.length,
      completeIslands: completeIslands.length,
      incompleteIslands: islands.length - completeIslands.length,
      totalBridges: gameStore.playerBridges.length,
      completionPercentage: completionPercentage.value
    }
  })

  /**
   * Trouve le pont le plus proche d'une position (pour le clic)
   */
  const findNearestBridge = (x: number, y: number): Bridge | null => {
    let nearestBridge: Bridge | null = null
    let minDistance = Infinity

    gameStore.playerBridges.forEach((bridge) => {
      const fromIsland = gameStore.getIslandById(bridge.fromIslandId)
      const toIsland = gameStore.getIslandById(bridge.toIslandId)

      if (!fromIsland || !toIsland) return

      // Calcul de distance simplifiée (à améliorer si nécessaire)
      const centerX = (fromIsland.x + toIsland.x) / 2
      const centerY = (fromIsland.y + toIsland.y) / 2
      const distance = Math.sqrt(Math.pow(x - centerX, 2) + Math.pow(y - centerY, 2))

      if (distance < minDistance) {
        minDistance = distance
        nearestBridge = bridge
      }
    })

    return nearestBridge
  }

  /**
   * Réinitialise tous les ponts du puzzle
   */
  const resetAllBridges = () => {
    gameStore.playerBridges = []
    gameStore.saveBridges()
  }

  return {
    // State du store
    currentGame: computed(() => gameStore.currentGame),
    currentPuzzle: computed(() => gameStore.currentPuzzle),
    playerBridges: computed(() => gameStore.playerBridges),
    selectedIsland: computed(() => gameStore.selectedIsland),
    isLoading: computed(() => gameStore.isLoading),
    error: computed(() => gameStore.error),
    elapsedTime: computed(() => gameStore.elapsedTime),

    // Computed properties
    completionPercentage,
    isPuzzleSolved,
    gameStats,

    // Methods
    canIslandReceiveBridge,
    isIslandComplete,
    canConnectIslands,
    getConnectedIslands,
    findNearestBridge,
    resetAllBridges,

    // Actions du store
    startGame: gameStore.startGame,
    selectIsland: gameStore.selectIsland,
    validateSolution: gameStore.validateSolution,
    abandonGame: gameStore.abandonGame,
    clearError: gameStore.clearError
  }
}

