<script setup lang="ts">
/**
 * Composant pour afficher une île dans le jeu Hashi
 * Une île est un cercle avec un nombre indiquant combien de ponts sont requis
 */

import { computed } from 'vue'
import type { Island } from '@/types'

// Props du composant
interface Props {
  /** L'île à afficher */
  island: Island
  /** Taille de la cellule dans la grille (en pixels) */
  cellSize: number
  /** Indique si l'île est sélectionnée */
  isSelected?: boolean
  /** Indique si l'île est complète (a le bon nombre de ponts) */
  isComplete?: boolean
  /** Nombre actuel de ponts connectés */
  currentBridges: number
}

const props = withDefaults(defineProps<Props>(), {
  isSelected: false,
  isComplete: false
})

// Événements émis par le composant
const emit = defineEmits<{
  click: [island: Island]
}>()

// ====================================================
// COMPUTED - Propriétés calculées
// ====================================================

/**
 * Position X de l'île en pixels
 */
const xPosition = computed(() => props.island.x * props.cellSize)

/**
 * Position Y de l'île en pixels
 */
const yPosition = computed(() => props.island.y * props.cellSize)

/**
 * Rayon du cercle de l'île
 */
const radius = computed(() => Math.min(props.cellSize * 0.35, 25))

/**
 * Classes CSS dynamiques pour l'île
 */
const islandClasses = computed(() => ({
  'island--selected': props.isSelected,
  'island--complete': props.isComplete,
  'island--incomplete': props.currentBridges > 0 && !props.isComplete,
  'island--error': props.currentBridges > props.island.requiredBridges
}))

/**
 * Gère le clic sur l'île
 */
function handleClick() {
  emit('click', props.island)
}
</script>

<template>
  <g class="island" :class="islandClasses" @click="handleClick">
    <!-- Cercle de l'île -->
    <circle
      :cx="xPosition"
      :cy="yPosition"
      :r="radius"
      class="island__circle"
    />
    
    <!-- Nombre de ponts requis -->
    <text
      :x="xPosition"
      :y="yPosition"
      class="island__text"
      text-anchor="middle"
      dominant-baseline="middle"
    >
      {{ island.requiredBridges }}
    </text>
  </g>
</template>

<style scoped>
.island {
  cursor: pointer;
  transition: all 0.2s ease;
}

.island:hover .island__circle {
  filter: brightness(1.2);
}

/* Cercle de l'île */
.island__circle {
  fill: #4a5568;
  stroke: #2d3748;
  stroke-width: 2;
  transition: all 0.2s ease;
}

/* Île sélectionnée */
.island--selected .island__circle {
  fill: #4299e1;
  stroke: #2b6cb0;
  stroke-width: 3;
}

/* Île complète (bon nombre de ponts) */
.island--complete .island__circle {
  fill: #48bb78;
  stroke: #2f855a;
}

/* Île incomplète mais avec des ponts */
.island--incomplete .island__circle {
  fill: #ed8936;
  stroke: #c05621;
}

/* Île en erreur (trop de ponts) */
.island--error .island__circle {
  fill: #f56565;
  stroke: #c53030;
}

/* Texte du nombre */
.island__text {
  fill: white;
  font-size: 18px;
  font-weight: bold;
  font-family: 'Arial', sans-serif;
  pointer-events: none;
  user-select: none;
}
</style>

