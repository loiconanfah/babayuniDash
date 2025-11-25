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

// Vérification que les îles sont bien chargées
watch(() => islands.value, (newIslands) => {
  if (newIslands.length > 0) {
    console.log('Îles chargées:', newIslands.length, newIslands)
  }
}, { immediate: true })

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
 * Référence au conteneur de la grille
 */
const gridContainer = ref<HTMLElement | null>(null)

/**
 * Ajuste la taille des cellules selon la taille du conteneur
 * Prend en compte le padding pour éviter que le puzzle dépasse
 */
function adjustCellSize() {
  if (!gridContainer.value) {
    // Réessayer après un court délai si le conteneur n'est pas encore monté
    setTimeout(adjustCellSize, 100)
    return
  }

  // Obtenir les dimensions réelles du conteneur (sans padding)
  const containerRect = gridContainer.value.getBoundingClientRect()
  const padding = 32 // 2rem = 32px de padding de chaque côté
  const availableWidth = containerRect.width - (padding * 2)
  const availableHeight = containerRect.height - (padding * 2)

  // Calculer la taille de cellule maximale pour tenir dans l'espace disponible
  const cellWidth = Math.floor(availableWidth / props.width)
  const cellHeight = Math.floor(availableHeight / props.height)

  // Utiliser la plus petite dimension pour garder les proportions
  // Limiter à 80px max par cellule pour les grands puzzles
  cellSize.value = Math.max(20, Math.min(cellWidth, cellHeight, 80))
}

// Ajuster la taille au montage, lors du redimensionnement et quand les dimensions changent
watch([() => props.width, () => props.height], adjustCellSize, { immediate: true })

// Ajuster aussi lors du redimensionnement de la fenêtre
if (typeof window !== 'undefined') {
  window.addEventListener('resize', adjustCellSize)
}
</script>

<template>
  <div ref="gridContainer" class="game-grid">
    <svg
      :width="svgWidth"
      :height="svgHeight"
      :viewBox="`0 0 ${svgWidth} ${svgHeight}`"
      class="game-grid__svg"
      xmlns="http://www.w3.org/2000/svg"
      preserveAspectRatio="xMidYMid meet"
    >
      <!-- Définitions SVG (gradients, patterns, etc.) -->
      <defs>
        <!-- Pattern de grille -->
        <pattern
          id="grid"
          :width="cellSize"
          :height="cellSize"
          patternUnits="userSpaceOnUse"
        >
          <path
            :d="`M ${cellSize} 0 L 0 0 0 ${cellSize}`"
            fill="none"
            stroke="#cbd5e1"
            stroke-width="1"
            opacity="0.6"
          />
        </pattern>
        
        <!-- Gradient pour les barreaux de prison (horizontal) - NOIR TRÈS VISIBLE -->
        <linearGradient id="barreau-gradient-h" x1="0%" y1="0%" x2="100%" y2="0%">
          <stop offset="0%" style="stop-color:#2d2d2d;stop-opacity:1" />
          <stop offset="10%" style="stop-color:#1a1a1a;stop-opacity:1" />
          <stop offset="50%" style="stop-color:#000000;stop-opacity:1" />
          <stop offset="90%" style="stop-color:#1a1a1a;stop-opacity:1" />
          <stop offset="100%" style="stop-color:#2d2d2d;stop-opacity:1" />
        </linearGradient>
        
        <!-- Gradient pour les barreaux de prison (vertical) - NOIR TRÈS VISIBLE -->
        <linearGradient id="barreau-gradient-v" x1="0%" y1="0%" x2="0%" y2="100%">
          <stop offset="0%" style="stop-color:#2d2d2d;stop-opacity:1" />
          <stop offset="10%" style="stop-color:#1a1a1a;stop-opacity:1" />
          <stop offset="50%" style="stop-color:#000000;stop-opacity:1" />
          <stop offset="90%" style="stop-color:#1a1a1a;stop-opacity:1" />
          <stop offset="100%" style="stop-color:#2d2d2d;stop-opacity:1" />
        </linearGradient>
        
        <!-- Gradients pour les verrous (plus visibles) -->
        <radialGradient id="verrou-gradient" cx="50%" cy="30%">
          <stop offset="0%" style="stop-color:#9ca3af;stop-opacity:1" />
          <stop offset="40%" style="stop-color:#6b7280;stop-opacity:1" />
          <stop offset="80%" style="stop-color:#4b5563;stop-opacity:1" />
          <stop offset="100%" style="stop-color:#374151;stop-opacity:1" />
        </radialGradient>
        <radialGradient id="verrou-selected-gradient" cx="50%" cy="30%">
          <stop offset="0%" style="stop-color:#60a5fa;stop-opacity:1" />
          <stop offset="40%" style="stop-color:#3b82f6;stop-opacity:1" />
          <stop offset="80%" style="stop-color:#2563eb;stop-opacity:1" />
          <stop offset="100%" style="stop-color:#1e40af;stop-opacity:1" />
        </radialGradient>
        <radialGradient id="verrou-complete-gradient" cx="50%" cy="30%">
          <stop offset="0%" style="stop-color:#34d399;stop-opacity:1" />
          <stop offset="40%" style="stop-color:#10b981;stop-opacity:1" />
          <stop offset="80%" style="stop-color:#059669;stop-opacity:1" />
          <stop offset="100%" style="stop-color:#047857;stop-opacity:1" />
        </radialGradient>
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
          :is-from-island-complete="isIslandComplete(findIslandById(bridge.fromIslandId)!)"
          :is-to-island-complete="isIslandComplete(findIslandById(bridge.toIslandId)!)"
          @click="handleBridgeClick(bridge)"
        />
        <!-- Debug : afficher le nombre de ponts -->
        <!-- <text x="10" y="20" fill="red" font-size="12">Ponts: {{ bridges.length }}</text> -->
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
  padding: 1rem;
  border-radius: 1rem;
  overflow: hidden; /* Empêche le contenu de dépasser */
  box-sizing: border-box; /* Inclut le padding dans les dimensions */
  min-width: 0; /* Permet au flex de rétrécir si nécessaire */
  min-height: 0; /* Permet au flex de rétrécir si nécessaire */
}

.game-grid__svg {
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%);
  border-radius: 0.5rem;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  border: 2px solid #e2e8f0;
  max-width: 100%; /* Ne dépasse pas le conteneur */
  max-height: 100%; /* Ne dépasse pas le conteneur */
  width: auto; /* S'adapte au contenu */
  height: auto; /* S'adapte au contenu */
  display: block; /* Évite les espaces inline */
}
</style>

