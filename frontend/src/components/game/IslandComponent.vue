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
 * Position X de l'île en pixels (centrée dans la cellule)
 */
const xPosition = computed(() => props.island.x * props.cellSize + props.cellSize / 2)

/**
 * Position Y de l'île en pixels (centrée dans la cellule)
 */
const yPosition = computed(() => props.island.y * props.cellSize + props.cellSize / 2)

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
    <!-- Cercle extérieur de la serrure (bordure métallique) -->
    <circle
      :cx="xPosition"
      :cy="yPosition"
      :r="radius * 0.9"
      class="island__outer-ring"
    />
    
    <!-- Cercle intérieur de la serrure (corps principal) -->
    <circle
      :cx="xPosition"
      :cy="yPosition"
      :r="radius * 0.75"
      class="island__circle"
    />
    
    <!-- Trou de serrure (rectangle avec barre horizontale) -->
    <rect
      :x="xPosition - radius * 0.3"
      :y="yPosition - radius * 0.5"
      :width="radius * 0.6"
      :height="radius * 0.6"
      rx="2"
      class="island__keyhole"
    />
    <line
      :x1="xPosition - radius * 0.15"
      :y1="yPosition - radius * 0.3"
      :x2="xPosition + radius * 0.15"
      :y2="yPosition - radius * 0.3"
      class="island__keyhole-bar"
    />
    
    <!-- Nombre de ponts requis - CENTRÉ sur l'île -->
    <text
      :x="xPosition"
      :y="yPosition"
      class="island__text"
      text-anchor="middle"
      dominant-baseline="central"
    >
      {{ island.requiredBridges }}
    </text>
  </g>
</template>

<style scoped>
.island {
  cursor: pointer;
  transition: all 0.2s ease;
  /* Animation désactivée pour faciliter la sélection */
  /* animation: islandFloat 3s ease-in-out infinite; */
}

/* Animation de flottement pour les verrous - désactivée pour faciliter la sélection */
@keyframes islandFloat {
  0%, 100% {
    transform: translateY(0);
  }
  50% {
    transform: translateY(-2px);
  }
}

.island:hover {
  transform: scale(1.05);
  /* Désactiver l'animation au survol pour faciliter le clic */
}

.island:hover .island__circle {
  filter: brightness(1.3) drop-shadow(0 4px 8px rgba(255, 255, 255, 0.2));
}

.island:hover .island__outer-ring {
  filter: drop-shadow(0 4px 8px rgba(255, 255, 255, 0.3));
}

/* Corps principal de la serrure (forme rectangulaire) */
.island__lock-body {
  fill: #3a3a3a;
  stroke: #1a1a1a;
  stroke-width: 2;
  transition: all 0.3s ease;
  filter: drop-shadow(0 3px 6px rgba(0, 0, 0, 0.7));
}

/* Bordure extérieure de la serrure (métallique) */
.island__outer-ring {
  fill: #2a2a2a;
  stroke: #1a1a1a;
  stroke-width: 2.5;
  transition: all 0.3s ease;
  filter: drop-shadow(0 3px 6px rgba(0, 0, 0, 0.6));
}

/* Cercle intérieur de la serrure (corps principal) */
.island__circle {
  fill: url(#verrou-gradient);
  stroke: #2a2a2a;
  stroke-width: 3;
  transition: all 0.3s ease;
  filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.5));
}

/* Trou de serrure (forme de trou de clé) */
.island__keyhole {
  fill: #0a0a0a;
  stroke: #1a1a1a;
  stroke-width: 1.5;
  transition: all 0.3s ease;
  filter: drop-shadow(0 1px 2px rgba(0, 0, 0, 0.8));
}

/* Barre horizontale dans le trou de serrure */
.island__keyhole-bar {
  stroke: #0a0a0a;
  stroke-width: 2;
  transition: all 0.3s ease;
}

/* Île sélectionnée */
.island--selected {
  animation: islandSelected 1s ease-in-out infinite;
}

@keyframes islandSelected {
  0%, 100% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.05);
  }
}

.island--selected .island__circle {
  fill: url(#verrou-selected-gradient);
  stroke: #3b82f6;
  stroke-width: 4;
  filter: drop-shadow(0 4px 12px rgba(59, 130, 246, 0.7));
  animation: selectedPulse 1.5s ease-in-out infinite;
}

@keyframes selectedPulse {
  0%, 100% {
    filter: drop-shadow(0 4px 12px rgba(59, 130, 246, 0.7));
  }
  50% {
    filter: drop-shadow(0 6px 16px rgba(59, 130, 246, 1));
  }
}

.island--selected .island__outer-ring {
  fill: #3b82f6;
  stroke: #2563eb;
  stroke-width: 3;
  filter: drop-shadow(0 4px 8px rgba(59, 130, 246, 0.6));
}

/* Île complète (bon nombre de ponts) */
.island--complete {
  animation: islandComplete 2s ease-in-out infinite;
}

@keyframes islandComplete {
  0%, 100% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.03);
  }
}

.island--complete .island__circle {
  fill: url(#verrou-complete-gradient);
  stroke: #10b981;
  stroke-width: 3;
  filter: drop-shadow(0 4px 12px rgba(16, 185, 129, 0.7));
  animation: completeGlow 2s ease-in-out infinite;
}

@keyframes completeGlow {
  0%, 100% {
    filter: drop-shadow(0 4px 12px rgba(16, 185, 129, 0.7));
  }
  50% {
    filter: drop-shadow(0 6px 16px rgba(16, 185, 129, 1));
  }
}

.island--complete .island__outer-ring {
  fill: #10b981;
  stroke: #059669;
  stroke-width: 3;
  filter: drop-shadow(0 4px 8px rgba(16, 185, 129, 0.6));
}

/* Île incomplète mais avec des ponts */
.island--incomplete .island__circle {
  fill: #f59e0b;
  stroke: #f97316;
  stroke-width: 3;
  filter: drop-shadow(0 3px 6px rgba(245, 158, 11, 0.6));
  animation: incompletePulse 2s ease-in-out infinite;
}

@keyframes incompletePulse {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.8;
  }
}

.island--incomplete .island__outer-ring {
  fill: #f97316;
  stroke: #ea580c;
  stroke-width: 2;
  filter: drop-shadow(0 3px 6px rgba(245, 158, 11, 0.5));
}

/* Île en erreur (trop de ponts) */
.island--error {
  animation: islandError 0.5s ease-in-out infinite;
}

@keyframes islandError {
  0%, 100% {
    transform: translateX(0);
  }
  25% {
    transform: translateX(-2px);
  }
  75% {
    transform: translateX(2px);
  }
}

.island--error .island__circle {
  fill: #ef4444;
  stroke: #f87171;
  stroke-width: 3;
  filter: drop-shadow(0 3px 6px rgba(239, 68, 68, 0.7));
  animation: errorFlash 0.5s ease-in-out infinite;
}

@keyframes errorFlash {
  0%, 100% {
    opacity: 1;
  }
  50% {
    opacity: 0.7;
  }
}

.island--error .island__outer-ring {
  fill: #f87171;
  stroke: #dc2626;
  stroke-width: 2;
  filter: drop-shadow(0 3px 6px rgba(239, 68, 68, 0.6));
}

/* Texte du nombre */
.island__text {
  fill: #ffffff;
  font-size: 20px;
  font-weight: 900;
  font-family: 'Arial', 'Helvetica', sans-serif;
  pointer-events: none;
  user-select: none;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.8);
  filter: drop-shadow(0 1px 2px rgba(0, 0, 0, 0.9));
  transition: all 0.3s ease;
}

.island:hover .island__text {
  font-size: 22px;
  filter: drop-shadow(0 2px 4px rgba(255, 255, 255, 0.5));
}

.island--selected .island__text {
  fill: #ffffff;
  text-shadow: 0 2px 6px rgba(59, 130, 246, 0.8);
}

.island--complete .island__text {
  fill: #ffffff;
  text-shadow: 0 2px 6px rgba(16, 185, 129, 0.8);
}

.island--error .island__text {
  fill: #ffffff;
  text-shadow: 0 2px 6px rgba(239, 68, 68, 0.8);
}
</style>

