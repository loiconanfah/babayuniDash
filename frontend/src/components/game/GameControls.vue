<script setup lang="ts">
/**
 * Composant des contrôles du jeu
 * Affiche le timer, le score, et les boutons d'action
 */

import { computed } from 'vue'
import { useGameStore } from '@/stores/game'
import { useUiStore } from '@/stores/ui'

// ====================================================
// STORE
// ====================================================

const gameStore = useGameStore()
const uiStore = useUiStore()

// ====================================================
// COMPUTED
// ====================================================

/**
 * Formate le temps écoulé en MM:SS
 */
const formattedTime = computed(() => {
  const seconds = gameStore.elapsedTime
  const minutes = Math.floor(seconds / 60)
  const remainingSeconds = seconds % 60
  return `${minutes.toString().padStart(2, '0')}:${remainingSeconds.toString().padStart(2, '0')}`
})

/**
 * Compte le nombre d'îles complètes
 */
const completedIslands = computed(() => {
  if (!gameStore.currentPuzzle) return 0
  return gameStore.currentPuzzle.islands.filter((island) =>
    gameStore.isIslandComplete(island)
  ).length
})

/**
 * Nombre total d'îles
 */
const totalIslands = computed(() => gameStore.currentPuzzle?.islands.length || 0)

// ====================================================
// METHODS
// ====================================================

/**
 * Valide la solution
 */
async function handleValidate() {
  try {
    await gameStore.validateSolution()
  } catch (error) {
    console.error('Erreur lors de la validation:', error)
  }
}

/**
 * Abandonne la partie
 */
async function handleAbandon() {
  if (confirm('Êtes-vous sûr de vouloir abandonner cette partie ?')) {
    try {
      await gameStore.abandonGame()
    } catch (error) {
      console.error("Erreur lors de l'abandon:", error)
    }
  }
}

/**
 * Réinitialise la grille (supprime tous les ponts)
 */
function handleReset() {
  if (confirm('Êtes-vous sûr de vouloir effacer tous les ponts ?')) {
    gameStore.playerBridges = []
    gameStore.saveBridges()
  }
}

/**
 * Résout automatiquement le puzzle
 */
async function handleSolve() {
  if (confirm('Voulez-vous résoudre automatiquement ce puzzle ? Cette action remplacera tous vos ponts actuels.')) {
    try {
      await gameStore.solvePuzzle()
    } catch (error) {
      console.error('Erreur lors de la résolution:', error)
    }
  }
}

/**
 * Ouvre le modal d'aide/tutoriel
 */
function handleHelp() {
  uiStore.openTutorialModal()
}
</script>

<template>
  <div class="game-controls">
    <!-- Informations de la partie -->
    <div class="game-controls__info">
      <!-- Timer -->
      <div class="info-card">
        <div class="info-card__label">Temps</div>
        <div class="info-card__value">{{ formattedTime }}</div>
      </div>

      <!-- Progression -->
      <div class="info-card">
        <div class="info-card__label">Îles complètes</div>
        <div class="info-card__value">{{ completedIslands }} / {{ totalIslands }}</div>
      </div>

      <!-- Nombre de ponts -->
      <div class="info-card">
        <div class="info-card__label">Ponts placés</div>
        <div class="info-card__value">{{ gameStore.playerBridges.length }}</div>
      </div>
    </div>

    <!-- Boutons d'action -->
    <div class="game-controls__actions">
      <button class="btn btn--primary" @click="handleValidate">
        ✓ Valider la solution
      </button>
      
      <button class="btn btn--solve" @click="handleSolve" :disabled="gameStore.isLoading">
        🔓 Résoudre
      </button>
      
      <button class="btn btn--secondary" @click="handleReset">
        ↺ Réinitialiser
      </button>
      
      <button class="btn btn--help" @click="handleHelp" title="Aide et tutoriel">
        ❓ Aide
      </button>
      
      <button class="btn btn--danger" @click="handleAbandon">
        × Abandonner
      </button>
    </div>

    <!-- Message d'erreur -->
    <div v-if="gameStore.error" class="game-controls__error">
      {{ gameStore.error }}
      <button class="btn-close" @click="gameStore.clearError">×</button>
    </div>
  </div>
</template>

<style scoped>
.game-controls {
  background: white;
  border-radius: 1rem;
  padding: 1.5rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

/* Informations de la partie */
.game-controls__info {
  display: flex;
  gap: 1rem;
  margin-bottom: 1.5rem;
  flex-wrap: wrap;
}

.info-card {
  flex: 1;
  min-width: 120px;
  padding: 1rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-radius: 0.5rem;
  color: white;
  text-align: center;
}

.info-card__label {
  font-size: 0.875rem;
  opacity: 0.9;
  margin-bottom: 0.5rem;
}

.info-card__value {
  font-size: 1.5rem;
  font-weight: bold;
}

/* Boutons d'action */
.game-controls__actions {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.btn {
  width: 100%;
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 0.5rem;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn:hover {
  transform: translateY(-2px);
  box-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.btn--primary {
  background: #48bb78;
  color: white;
}

.btn--primary:hover {
  background: #38a169;
}

.btn--help {
  background: #9f7aea;
  color: white;
}

.btn--help:hover {
  background: #805ad5;
}

.btn--secondary {
  background: #4299e1;
  color: white;
}

.btn--secondary:hover {
  background: #3182ce;
}

.btn--solve {
  background: #ed8936;
  color: white;
}

.btn--solve:hover {
  background: #dd6b20;
}

.btn--solve:disabled {
  background: #a0aec0;
  cursor: not-allowed;
  opacity: 0.6;
}

.btn--danger {
  background: #f56565;
  color: white;
}

.btn--danger:hover {
  background: #e53e3e;
}

/* Message d'erreur */
.game-controls__error {
  margin-top: 1rem;
  padding: 1rem;
  background: #fed7d7;
  border: 1px solid #fc8181;
  border-radius: 0.5rem;
  color: #c53030;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.btn-close {
  background: none;
  border: none;
  font-size: 1.5rem;
  color: #c53030;
  cursor: pointer;
  padding: 0;
  width: 2rem;
  height: 2rem;
  display: flex;
  align-items: center;
  justify-content: center;
  border-radius: 0.25rem;
}

.btn-close:hover {
  background: rgba(0, 0, 0, 0.1);
}
</style>

