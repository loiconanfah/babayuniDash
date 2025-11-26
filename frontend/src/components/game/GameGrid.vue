<script setup lang="ts">
/**
 * Composant principal de la grille de jeu Hashi
 * Affiche les îles, les ponts et gère les interactions
 */

import { ref, computed, watch } from 'vue'
import { useGameStore } from '@/stores/game'
import type { Island, Bridge } from '@/types'
import { PuzzleTheme } from '@/types'
import { getThemeConfig } from '@/utils/themeConfig'
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
 * Thème actuel du puzzle
 */
const currentTheme = computed(() => gameStore.currentPuzzle?.theme || PuzzleTheme.Classic)

/**
 * Configuration du thème actuel
 */
const themeConfig = computed(() => getThemeConfig(currentTheme.value))

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
  // Ne rien faire si le jeu est en pause
  if (gameStore.isPaused) {
    return
  }
  gameStore.selectIsland(island)
}

/**
 * Gère le clic sur un pont
 */
function handleBridgeClick(bridge: Bridge) {
  // Ne rien faire si le jeu est en pause
  if (gameStore.isPaused) {
    return
  }
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
        <!-- Pattern de grille avec couleur du thème -->
        <pattern
          id="grid"
          :width="cellSize"
          :height="cellSize"
          patternUnits="userSpaceOnUse"
        >
          <path
            :d="`M ${cellSize} 0 L 0 0 0 ${cellSize}`"
            fill="none"
            :stroke="themeConfig.colors.gridLines"
            stroke-width="1"
            opacity="0.6"
          />
        </pattern>
        
        <!-- Gradient pour les barreaux de prison (horizontal) selon le thème -->
        <linearGradient id="barreau-gradient-h" x1="0%" y1="0%" x2="100%" y2="0%">
          <stop offset="0%" :style="`stop-color:${themeConfig.colors.bridgeGradient[0]};stop-opacity:1`" />
          <stop offset="10%" :style="`stop-color:${themeConfig.colors.bridgeGradient[1]};stop-opacity:1`" />
          <stop offset="50%" :style="`stop-color:${themeConfig.colors.bridgeGradient[2]};stop-opacity:1`" />
          <stop offset="90%" :style="`stop-color:${themeConfig.colors.bridgeGradient[3]};stop-opacity:1`" />
          <stop offset="100%" :style="`stop-color:${themeConfig.colors.bridgeGradient[4]};stop-opacity:1`" />
        </linearGradient>
        
        <!-- Gradient pour les barreaux de prison (vertical) selon le thème -->
        <linearGradient id="barreau-gradient-v" x1="0%" y1="0%" x2="0%" y2="100%">
          <stop offset="0%" :style="`stop-color:${themeConfig.colors.bridgeGradient[0]};stop-opacity:1`" />
          <stop offset="10%" :style="`stop-color:${themeConfig.colors.bridgeGradient[1]};stop-opacity:1`" />
          <stop offset="50%" :style="`stop-color:${themeConfig.colors.bridgeGradient[2]};stop-opacity:1`" />
          <stop offset="90%" :style="`stop-color:${themeConfig.colors.bridgeGradient[3]};stop-opacity:1`" />
          <stop offset="100%" :style="`stop-color:${themeConfig.colors.bridgeGradient[4]};stop-opacity:1`" />
        </linearGradient>
        
        <!-- Gradients pour les verrous selon le thème -->
        <radialGradient id="verrou-gradient" cx="50%" cy="30%">
          <stop offset="0%" :style="`stop-color:${themeConfig.colors.islandGradient[0]};stop-opacity:1`" />
          <stop offset="40%" :style="`stop-color:${themeConfig.colors.islandGradient[1]};stop-opacity:1`" />
          <stop offset="80%" :style="`stop-color:${themeConfig.colors.islandGradient[2]};stop-opacity:1`" />
          <stop offset="100%" :style="`stop-color:${themeConfig.colors.islandGradient[3]};stop-opacity:1`" />
        </radialGradient>
        <radialGradient id="verrou-selected-gradient" cx="50%" cy="30%">
          <stop offset="0%" :style="`stop-color:${themeConfig.colors.islandSelected[0]};stop-opacity:1`" />
          <stop offset="40%" :style="`stop-color:${themeConfig.colors.islandSelected[1]};stop-opacity:1`" />
          <stop offset="80%" :style="`stop-color:${themeConfig.colors.islandSelected[2]};stop-opacity:1`" />
          <stop offset="100%" :style="`stop-color:${themeConfig.colors.islandSelected[3]};stop-opacity:1`" />
        </radialGradient>
        <radialGradient id="verrou-complete-gradient" cx="50%" cy="30%">
          <stop offset="0%" :style="`stop-color:${themeConfig.colors.islandComplete[0]};stop-opacity:1`" />
          <stop offset="40%" :style="`stop-color:${themeConfig.colors.islandComplete[1]};stop-opacity:1`" />
          <stop offset="80%" :style="`stop-color:${themeConfig.colors.islandComplete[2]};stop-opacity:1`" />
          <stop offset="100%" :style="`stop-color:${themeConfig.colors.islandComplete[3]};stop-opacity:1`" />
        </radialGradient>
      </defs>
      
      <!-- Fond avec grille selon le thème -->
      <rect width="100%" height="100%" fill="url(#grid)" :style="{ background: themeConfig.colors.gridBackground }" />
      
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
  padding: 1rem;
  border-radius: 1rem;
  overflow: hidden; /* Empêche le contenu de dépasser */
  box-sizing: border-box; /* Inclut le padding dans les dimensions */
  min-width: 0; /* Permet au flex de rétrécir si nécessaire */
  min-height: 0; /* Permet au flex de rétrécir si nécessaire */
  transition: background 0.5s ease;
  background: v-bind('themeConfig.colors.background');
}

.game-grid__svg {
  border-radius: 0.5rem;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
  border: 2px solid rgba(255, 255, 255, 0.2);
  max-width: 100%; /* Ne dépasse pas le conteneur */
  max-height: 100%; /* Ne dépasse pas le conteneur */
  width: auto; /* S'adapte au contenu */
  height: auto; /* S'adapte au contenu */
  display: block; /* Évite les espaces inline */
  transition: all 0.5s ease;
  background: v-bind('themeConfig.colors.gridBackground');
}
</style>

