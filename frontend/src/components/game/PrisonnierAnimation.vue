<script setup lang="ts">
/**
 * Composant pour animer un prisonnier qui se déplace le long d'un pont
 * S'affiche quand une connexion correcte est établie entre deux serrures
 */

import { ref, onMounted, onUnmounted, computed } from 'vue'
import { useGameStore } from '@/stores/game'
import type { Island } from '@/types'
import { PuzzleTheme } from '@/types'
import { getThemeConfig } from '@/utils/themeConfig'

interface Props {
  /** Île de départ */
  fromIsland: Island
  /** Île d'arrivée */
  toIsland: Island
  /** Taille de la cellule dans la grille (en pixels) */
  cellSize: number
  /** Durée de l'animation en millisecondes */
  duration?: number
}

const props = withDefaults(defineProps<Props>(), {
  duration: 2000
})

const emit = defineEmits<{
  complete: []
}>()

// Store pour récupérer le thème
const gameStore = useGameStore()

/**
 * Thème actuel du puzzle
 */
const currentTheme = computed(() => gameStore.currentPuzzle?.theme || PuzzleTheme.Classic)

/**
 * Configuration du thème actuel
 */
const themeConfig = computed(() => getThemeConfig(currentTheme.value))

// Position actuelle du prisonnier
const currentX = ref(0)
const currentY = ref(0)
const isAnimating = ref(false)
let animationFrameId: number | null = null
let startTime: number = 0

/**
 * Calcule les positions des îles en pixels
 */
const fromX = props.fromIsland.x * props.cellSize + props.cellSize / 2
const fromY = props.fromIsland.y * props.cellSize + props.cellSize / 2
const toX = props.toIsland.x * props.cellSize + props.cellSize / 2
const toY = props.toIsland.y * props.cellSize + props.cellSize / 2

/**
 * Rayon des îles pour calculer où le prisonnier doit commencer/finir
 */
const islandRadius = Math.min(props.cellSize * 0.35, 25)

/**
 * Calcule le point de départ (à la surface de l'île)
 */
const dx = toX - fromX
const dy = toY - fromY
const distance = Math.sqrt(dx * dx + dy * dy)
const ratio = islandRadius / distance
const startX = fromX + dx * ratio
const startY = fromY + dy * ratio
const endX = toX - dx * ratio
const endY = toY - dy * ratio

/**
 * Taille du prisonnier
 */
const prisonnierSize = 12

/**
 * Animation du prisonnier
 */
function animate(timestamp: number) {
  if (!startTime) {
    startTime = timestamp
  }

  const elapsed = timestamp - startTime
  const progress = Math.min(elapsed / props.duration, 1)

  // Utiliser une fonction d'easing pour un mouvement plus naturel
  const easeInOut = (t: number) => {
    return t < 0.5 ? 2 * t * t : -1 + (4 - 2 * t) * t
  }
  const easedProgress = easeInOut(progress)

  // Calculer la position actuelle
  currentX.value = startX + (endX - startX) * easedProgress
  currentY.value = startY + (endY - startY) * easedProgress

  if (progress < 1) {
    animationFrameId = requestAnimationFrame(animate)
  } else {
    isAnimating.value = false
    emit('complete')
  }
}

/**
 * Démarre l'animation
 */
function startAnimation() {
  isAnimating.value = true
  startTime = 0
  currentX.value = startX
  currentY.value = startY
  animationFrameId = requestAnimationFrame(animate)
}

onMounted(() => {
  startAnimation()
})

onUnmounted(() => {
  if (animationFrameId !== null) {
    cancelAnimationFrame(animationFrameId)
  }
})
</script>

<template>
  <g v-if="isAnimating" class="prisonnier-animation">
    <!-- Corps du prisonnier (cercle) - Couleur selon le thème -->
    <circle
      :cx="currentX"
      :cy="currentY"
      :r="prisonnierSize"
      class="prisonnier__body"
      :fill="themeConfig.colors.islandGradient[2]"
      :stroke="themeConfig.colors.islandGradient[3]"
    />
    
    <!-- Tête du prisonnier (cercle plus petit) - Couleur selon le thème -->
    <circle
      :cx="currentX"
      :cy="currentY - prisonnierSize * 0.6"
      :r="prisonnierSize * 0.5"
      class="prisonnier__head"
      :fill="themeConfig.colors.islandSelected[0]"
      :stroke="themeConfig.colors.islandSelected[1]"
    />
    
    <!-- Rayures de prisonnier (lignes horizontales) -->
    <line
      :x1="currentX - prisonnierSize * 0.7"
      :y1="currentY - prisonnierSize * 0.2"
      :x2="currentX + prisonnierSize * 0.7"
      :y2="currentY - prisonnierSize * 0.2"
      class="prisonnier__stripe"
    />
    <line
      :x1="currentX - prisonnierSize * 0.7"
      :y1="currentY + prisonnierSize * 0.2"
      :x2="currentX + prisonnierSize * 0.7"
      :y2="currentY + prisonnierSize * 0.2"
      class="prisonnier__stripe"
    />
    
    <!-- Bras du prisonnier (lignes) -->
    <line
      :x1="currentX - prisonnierSize * 0.8"
      :y1="currentY - prisonnierSize * 0.3"
      :x2="currentX - prisonnierSize * 1.2"
      :y2="currentY - prisonnierSize * 0.1"
      class="prisonnier__arm"
    />
    <line
      :x1="currentX + prisonnierSize * 0.8"
      :y1="currentY - prisonnierSize * 0.3"
      :x2="currentX + prisonnierSize * 1.2"
      :y2="currentY - prisonnierSize * 0.1"
      class="prisonnier__arm"
    />
    
    <!-- Jambes du prisonnier (lignes) -->
    <line
      :x1="currentX - prisonnierSize * 0.3"
      :y1="currentY + prisonnierSize * 0.8"
      :x2="currentX - prisonnierSize * 0.5"
      :y2="currentY + prisonnierSize * 1.3"
      class="prisonnier__leg"
    />
    <line
      :x1="currentX + prisonnierSize * 0.3"
      :y1="currentY + prisonnierSize * 0.8"
      :x2="currentX + prisonnierSize * 0.5"
      :y2="currentY + prisonnierSize * 1.3"
      class="prisonnier__leg"
    />
  </g>
</template>

<style scoped>
.prisonnier-animation {
  pointer-events: none;
  z-index: 1000;
}

.prisonnier__body {
  stroke-width: 2;
  filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.8));
  transition: fill 0.3s ease, stroke 0.3s ease;
}

.prisonnier__head {
  stroke-width: 1.5;
  filter: drop-shadow(0 1px 2px rgba(0, 0, 0, 0.6));
  transition: fill 0.3s ease, stroke 0.3s ease;
}

.prisonnier__stripe {
  stroke: #ffffff;
  stroke-width: 1.5;
  opacity: 0.8;
}

.prisonnier__arm,
.prisonnier__leg {
  stroke: #2d3748;
  stroke-width: 2.5;
  stroke-linecap: round;
}
</style>

