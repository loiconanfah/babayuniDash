<script setup lang="ts">
/**
 * Composant pour afficher un pont entre deux îles
 * Un pont peut être simple (une ligne) ou double (deux lignes parallèles)
 */

import { computed } from 'vue'
import type { Bridge, Island } from '@/types'

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
}

const props = defineProps<Props>()

// Événements
const emit = defineEmits<{
  click: [bridge: Bridge]
}>()

// ====================================================
// COMPUTED - Propriétés calculées
// ====================================================

/**
 * Position de départ du pont (X)
 */
const x1 = computed(() => props.fromIsland.x * props.cellSize)

/**
 * Position de départ du pont (Y)
 */
const y1 = computed(() => props.fromIsland.y * props.cellSize)

/**
 * Position d'arrivée du pont (X)
 */
const x2 = computed(() => props.toIsland.x * props.cellSize)

/**
 * Position d'arrivée du pont (Y)
 */
const y2 = computed(() => props.toIsland.y * props.cellSize)

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
 */
const line1 = computed(() => {
  if (!props.bridge.isDouble) {
    return { x1: x1.value, y1: y1.value, x2: x2.value, y2: y2.value }
  }

  if (isHorizontal.value) {
    // Pont horizontal : décaler verticalement
    return {
      x1: x1.value,
      y1: y1.value - offset.value,
      x2: x2.value,
      y2: y2.value - offset.value
    }
  } else {
    // Pont vertical : décaler horizontalement
    return {
      x1: x1.value - offset.value,
      y1: y1.value,
      x2: x2.value - offset.value,
      y2: y2.value
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
      x1: x1.value,
      y1: y1.value + offset.value,
      x2: x2.value,
      y2: y2.value + offset.value
    }
  } else {
    return {
      x1: x1.value + offset.value,
      y1: y1.value,
      x2: x2.value + offset.value,
      y2: y2.value
    }
  }
})

/**
 * Zone cliquable invisible autour du pont
 * Plus large que le pont lui-même pour faciliter le clic
 */
const clickArea = computed(() => {
  const padding = 10
  return {
    x1: x1.value,
    y1: y1.value,
    x2: x2.value,
    y2: y2.value,
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
  <g class="bridge" @click="handleClick">
    <!-- Zone cliquable invisible -->
    <line
      :x1="clickArea.x1"
      :y1="clickArea.y1"
      :x2="clickArea.x2"
      :y2="clickArea.y2"
      :stroke-width="clickArea.strokeWidth"
      class="bridge__click-area"
    />
    
    <!-- Première ligne (ou ligne unique si pont simple) -->
    <line
      :x1="line1.x1"
      :y1="line1.y1"
      :x2="line1.x2"
      :y2="line1.y2"
      class="bridge__line"
    />
    
    <!-- Deuxième ligne (si pont double) -->
    <line
      v-if="line2"
      :x1="line2.x1"
      :y1="line2.y1"
      :x2="line2.x2"
      :y2="line2.y2"
      class="bridge__line"
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

/* Ligne du pont */
.bridge__line {
  stroke: #2d3748;
  stroke-width: 4;
  stroke-linecap: round;
  transition: stroke 0.2s ease;
  pointer-events: none;
}

.bridge:hover .bridge__line {
  stroke: #4299e1;
}
</style>

