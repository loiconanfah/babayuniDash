<script setup lang="ts">
/**
 * Composant principal de la grille de jeu Hashi
 * Affiche les îles, les ponts et gère les interactions
 */

import { ref, computed, watch } from 'vue'
import { useGameStore } from '@/stores/game'
import type { Island, Bridge } from '@/types'
import IslandComponent from './IslandComponent.vue'
import BridgeComponent from './BridgeComponent.vue'

// Props
interface Props {
  /** Largeur de la grille en nombre de cellules */
  width: number
  /** Hauteur de la grille en nombre de cellules */
  height: number
}

const props = defineProps<Props>()

// ====================================================
// STORE & STATE
// ====================================================

const gameStore = useGameStore()

/** Taille d'une cellule en pixels */
const cellSize = ref(60)

/** Dimensions du SVG */
const svgWidth = computed(() => props.width * cellSize.value)
const svgHeight = computed(() => props.height * cellSize.value)

// ====================================================
// COMPUTED - Propriétés calculées
// ====================================================

/**
 * Liste des îles à afficher
 */
const islands = computed(() => gameStore.currentPuzzle?.islands || [])

/**
 * Liste des ponts placés par le joueur
 */
const bridges = computed(() => gameStore.playerBridges)

/**
 * Île actuellement sélectionnée
 */
const selectedIsland = computed(() => gameStore.selectedIsland)

/**
 * Vérifie si une île est sélectionnée
 */
function isIslandSelected(island: Island): boolean {
  return selectedIsland.value?.id === island.id
}

/**
 * Récupère le nombre actuel de ponts pour une île
 */
function getCurrentBridgeCount(island: Island): number {
  return gameStore.getBridgeCountForIsland(island.id)
}

/**
 * Vérifie si une île est complète
 */
function isIslandComplete(island: Island): boolean {
  return gameStore.isIslandComplete(island)
}

/**
 * Trouve l'île par son ID
 */
function findIslandById(id: number): Island | undefined {
  return gameStore.getIslandById(id)
}

// ====================================================
// METHODS - Gestion des événements
// ====================================================

/**
 * Gère le clic sur une île
 */
function handleIslandClick(island: Island) {
  gameStore.selectIsland(island)
}

/**
 * Gère le clic sur un pont
 */
function handleBridgeClick(bridge: Bridge) {
  // Cliquer sur un pont le supprime
  gameStore.removeBridge(bridge)
}

/**
 * Ajuste la taille des cellules selon la taille de la fenêtre
 */
function adjustCellSize() {
  const container = document.querySelector('.game-grid')
  if (!container) return

  const containerWidth = container.clientWidth
  const containerHeight = container.clientHeight

  const cellWidth = Math.floor(containerWidth / props.width)
  const cellHeight = Math.floor(containerHeight / props.height)

  cellSize.value = Math.min(cellWidth, cellHeight, 80) // Max 80px par cellule
}

// Ajuster la taille au montage et lors du redimensionnement
watch([() => props.width, () => props.height], adjustCellSize, { immediate: true })
</script>

<template>
  <div class="game-grid">
    <svg
      :width="svgWidth"
      :height="svgHeight"
      class="game-grid__svg"
      xmlns="http://www.w3.org/2000/svg"
    >
      <!-- Grille de fond (optionnel, pour le debug) -->
      <defs>
        <pattern
          id="grid"
          :width="cellSize"
          :height="cellSize"
          patternUnits="userSpaceOnUse"
        >
          <path
            :d="`M ${cellSize} 0 L 0 0 0 ${cellSize}`"
            fill="none"
            stroke="#e2e8f0"
            stroke-width="0.5"
          />
        </pattern>
      </defs>
      
      <!-- Fond avec grille -->
      <rect width="100%" height="100%" fill="url(#grid)" />
      
      <!-- Ponts (dessinés en premier pour être derrière les îles) -->
      <g class="bridges-layer">
        <BridgeComponent
          v-for="(bridge, index) in bridges"
          :key="`bridge-${index}`"
          :bridge="bridge"
          :from-island="findIslandById(bridge.fromIslandId)!"
          :to-island="findIslandById(bridge.toIslandId)!"
          :cell-size="cellSize"
          @click="handleBridgeClick(bridge)"
        />
      </g>
      
      <!-- Îles (dessinées au-dessus des ponts) -->
      <g class="islands-layer">
        <IslandComponent
          v-for="island in islands"
          :key="`island-${island.id}`"
          :island="island"
          :cell-size="cellSize"
          :is-selected="isIslandSelected(island)"
          :is-complete="isIslandComplete(island)"
          :current-bridges="getCurrentBridgeCount(island)"
          @click="handleIslandClick(island)"
        />
      </g>
    </svg>
  </div>
</template>

<style scoped>
.game-grid {
  width: 100%;
  height: 100%;
  display: flex;
  justify-content: center;
  align-items: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 2rem;
  border-radius: 1rem;
}

.game-grid__svg {
  background: white;
  border-radius: 0.5rem;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.2);
}
</style>

