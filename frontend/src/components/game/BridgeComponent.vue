<script setup lang="ts">
/**
 * Composant pour afficher un pont entre deux îles
 * Un pont peut être simple (une ligne) ou double (deux lignes parallèles)
 */

import { computed, ref, watch } from 'vue'
import { useGameStore } from '@/stores/game'
import type { Bridge, Island } from '@/types'
import { PuzzleTheme } from '@/types'
import { getThemeConfig } from '@/utils/themeConfig'
import PrisonnierAnimation from './PrisonnierAnimation.vue'

// Props du composant
interface Props {
  /** Le pont à afficher */
  bridge: Bridge
  /** L'île de départ */
  fromIsland: Island
  /** L'île d'arrivée */
  toIsland: Island
  /** Taille de la cellule dans la grille (en pixels) */
  cellSize: number
  /** Indique si l'île de départ est complète */
  isFromIslandComplete?: boolean
  /** Indique si l'île d'arrivée est complète */
  isToIslandComplete?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  isFromIslandComplete: false,
  isToIslandComplete: false
})

// Événements
const emit = defineEmits<{
  click: [bridge: Bridge]
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

// État de l'animation du prisonnier
const showPrisonnier = ref(false)
const hasAnimated = ref(false)

/**
 * Vérifie si la connexion est correcte (les deux îles sont complètes)
 */
const isConnectionCorrect = computed(() => {
  return props.isFromIslandComplete && props.isToIslandComplete
})

/**
 * Surveille les changements de complétion pour déclencher l'animation
 */
watch(
  () => isConnectionCorrect.value,
  (newValue, oldValue) => {
    // Déclencher l'animation seulement quand les deux îles deviennent complètes
    if (newValue && !oldValue && !hasAnimated.value) {
      showPrisonnier.value = true
      hasAnimated.value = true
    }
  }
)

// ====================================================
// COMPUTED - Propriétés calculées
// ====================================================

/**
 * Position de départ du pont (X) - centré sur l'île
 */
const x1 = computed(() => props.fromIsland.x * props.cellSize + props.cellSize / 2)

/**
 * Position de départ du pont (Y) - centré sur l'île
 */
const y1 = computed(() => props.fromIsland.y * props.cellSize + props.cellSize / 2)

/**
 * Position d'arrivée du pont (X) - centré sur l'île
 */
const x2 = computed(() => props.toIsland.x * props.cellSize + props.cellSize / 2)

/**
 * Position d'arrivée du pont (Y) - centré sur l'île
 */
const y2 = computed(() => props.toIsland.y * props.cellSize + props.cellSize / 2)

/**
 * Vérifie si le pont est horizontal
 */
const isHorizontal = computed(() => y1.value === y2.value)

/**
 * Calcule le décalage pour les ponts doubles
 * Les deux lignes sont parallèles avec un petit espace entre elles
 */
const offset = computed(() => 5)

/**
 * Coordonnées de la première ligne (pour pont double)
 * Les ponts partent de la surface des îles
 */
const line1 = computed(() => {
  if (!props.bridge.isDouble) {
    return { 
      x1: startPoint.value.x, 
      y1: startPoint.value.y, 
      x2: endPoint.value.x, 
      y2: endPoint.value.y 
    }
  }

  if (isHorizontal.value) {
    // Pont horizontal : décaler verticalement
    return {
      x1: startPoint.value.x,
      y1: startPoint.value.y - offset.value,
      x2: endPoint.value.x,
      y2: endPoint.value.y - offset.value
    }
  } else {
    // Pont vertical : décaler horizontalement
    return {
      x1: startPoint.value.x - offset.value,
      y1: startPoint.value.y,
      x2: endPoint.value.x - offset.value,
      y2: endPoint.value.y
    }
  }
})

/**
 * Coordonnées de la deuxième ligne (pour pont double)
 */
const line2 = computed(() => {
  if (!props.bridge.isDouble) return null

  if (isHorizontal.value) {
    return {
      x1: startPoint.value.x,
      y1: startPoint.value.y + offset.value,
      x2: endPoint.value.x,
      y2: endPoint.value.y + offset.value
    }
  } else {
    return {
      x1: startPoint.value.x + offset.value,
      y1: startPoint.value.y,
      x2: endPoint.value.x + offset.value,
      y2: endPoint.value.y
    }
  }
})

/**
 * Rayon des îles pour calculer où les ponts doivent commencer/finir
 */
const islandRadius = computed(() => Math.min(props.cellSize * 0.35, 25))

/**
 * Calcule le point de départ du pont (à la surface de l'île)
 */
const startPoint = computed(() => {
  const dx = x2.value - x1.value
  const dy = y2.value - y1.value
  const distance = Math.sqrt(dx * dx + dy * dy)
  
  if (distance === 0) return { x: x1.value, y: y1.value }
  
  const ratio = islandRadius.value / distance
  return {
    x: x1.value + dx * ratio,
    y: y1.value + dy * ratio
  }
})

/**
 * Calcule le point d'arrivée du pont (à la surface de l'île)
 */
const endPoint = computed(() => {
  const dx = x2.value - x1.value
  const dy = y2.value - y1.value
  const distance = Math.sqrt(dx * dx + dy * dy)
  
  if (distance === 0) return { x: x2.value, y: y2.value }
  
  const ratio = islandRadius.value / distance
  return {
    x: x2.value - dx * ratio,
    y: y2.value - dy * ratio
  }
})

/**
 * Zone cliquable invisible autour du pont
 * Plus large que le pont lui-même pour faciliter le clic
 */
const clickArea = computed(() => {
  const padding = 10
  return {
    x1: startPoint.value.x,
    y1: startPoint.value.y,
    x2: endPoint.value.x,
    y2: endPoint.value.y,
    strokeWidth: padding * 2
  }
})

/**
 * Gère le clic sur le pont
 */
function handleClick() {
  emit('click', props.bridge)
}
</script>

<template>
  <g 
    class="bridge" 
    :data-direction="isHorizontal ? 'horizontal' : 'vertical'"
    @click="handleClick"
  >
    <!-- Zone cliquable invisible -->
    <line
      :x1="clickArea.x1"
      :y1="clickArea.y1"
      :x2="clickArea.x2"
      :y2="clickArea.y2"
      :stroke-width="clickArea.strokeWidth"
      class="bridge__click-area"
    />
    
    <!-- Première ligne (ou ligne unique si pont simple) - Couleur selon le thème -->
    <line
      :x1="line1.x1"
      :y1="line1.y1"
      :x2="line1.x2"
      :y2="line1.y2"
      :stroke="themeConfig.colors.bridgeColor"
      :stroke-width="15"
      fill="none"
      stroke-linecap="round"
      stroke-linejoin="round"
      vector-effect="non-scaling-stroke"
      :class="{ 'bridge__line--pulse': themeConfig.animations.bridgePulse }"
    />
    
    <!-- Deuxième ligne (si pont double) - Couleur selon le thème -->
    <line
      v-if="line2"
      :x1="line2.x1"
      :y1="line2.y1"
      :x2="line2.x2"
      :y2="line2.y2"
      :stroke="themeConfig.colors.bridgeColor"
      :stroke-width="15"
      fill="none"
      stroke-linecap="round"
      stroke-linejoin="round"
      vector-effect="non-scaling-stroke"
      :class="{ 'bridge__line--pulse': themeConfig.animations.bridgePulse }"
    />
    
    <!-- Animation du prisonnier quand la connexion est correcte -->
    <PrisonnierAnimation
      v-if="showPrisonnier && isConnectionCorrect"
      :from-island="fromIsland"
      :to-island="toIsland"
      :cell-size="cellSize"
      @complete="showPrisonnier = false"
    />
  </g>
</template>

<style scoped>
.bridge {
  cursor: pointer;
}

/* Zone cliquable invisible */
.bridge__click-area {
  stroke: transparent;
  fill: none;
}

.bridge:hover .bridge__click-area {
  stroke: rgba(66, 153, 225, 0.2);
}
</style>

<style>
/* Style global pour forcer la visibilité des lignes */
.bridge line {
  stroke-width: 15px !important;
  opacity: 1 !important;
  fill: none !important;
  visibility: visible !important;
  transition: all 0.3s ease;
}

/* Effet au survol */
.bridge:hover line {
  stroke-width: 17px !important;
  filter: brightness(1.2);
}

/* Animation de pulsation pour certains thèmes */
.bridge__line--pulse {
  animation: bridgePulse 2s ease-in-out infinite;
}

@keyframes bridgePulse {
  0%, 100% {
    opacity: 1;
    filter: brightness(1);
  }
  50% {
    opacity: 0.8;
    filter: brightness(1.3);
  }
}
</style>

