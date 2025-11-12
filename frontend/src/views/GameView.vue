<script setup lang="ts">
/**
 * Vue principale du jeu
 * Affiche la grille de jeu et les contrôles
 */

import { ref, onMounted, onUnmounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useGameStore } from '@/stores/game'
import { usePuzzleStore } from '@/stores/puzzle'
import GameGrid from '@/components/game/GameGrid.vue'
import GameControls from '@/components/game/GameControls.vue'

const route = useRoute()
const router = useRouter()
const gameStore = useGameStore()
const puzzleStore = usePuzzleStore()

const isLoading = ref(true)
const error = ref<string | null>(null)

/**
 * Initialise la partie au montage du composant
 */
onMounted(async () => {
  try {
    const puzzleId = parseInt(route.params.id as string)
    
    if (isNaN(puzzleId)) {
      error.value = 'ID de puzzle invalide'
      return
    }

    // Charger le puzzle
    const puzzle = await puzzleStore.fetchPuzzleById(puzzleId)
    
    // Démarrer une nouvelle partie
    await gameStore.startGame(puzzle)
    
    isLoading.value = false
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Erreur lors du chargement du jeu'
    isLoading.value = false
  }
})

/**
 * Nettoie le store au démontage
 */
onUnmounted(() => {
  gameStore.stopTimer()
})

/**
 * Retour à la sélection de puzzles
 */
function goBack() {
  router.push('/puzzles')
}
</script>

<template>
  <div class="game-view">
    <!-- Chargement -->
    <div v-if="isLoading" class="game-view__loading">
      <div class="loading-spinner"></div>
      <p>Chargement du puzzle...</p>
    </div>

    <!-- Erreur -->
    <div v-else-if="error" class="game-view__error">
      <h2>Erreur</h2>
      <p>{{ error }}</p>
      <button class="btn btn--primary" @click="goBack">
        Retour à la sélection
      </button>
    </div>

    <!-- Jeu -->
    <div v-else class="game-view__content">
      <!-- Header -->
      <div class="game-header">
        <button class="btn-back" @click="goBack">← Retour</button>
        <h1 class="game-title">
          {{ gameStore.currentPuzzle?.name || 'Puzzle Hashi' }}
        </h1>
        <div class="game-difficulty">
          {{ gameStore.currentPuzzle?.difficulty === 1 ? 'Facile' :
             gameStore.currentPuzzle?.difficulty === 2 ? 'Moyen' :
             gameStore.currentPuzzle?.difficulty === 3 ? 'Difficile' :
             'Expert' }}
        </div>
      </div>

      <!-- Contrôles du jeu -->
      <div class="game-controls-section">
        <GameControls />
      </div>

      <!-- Grille de jeu -->
      <div class="game-grid-section">
        <GameGrid
          v-if="gameStore.currentPuzzle"
          :width="gameStore.currentPuzzle.width"
          :height="gameStore.currentPuzzle.height"
        />
      </div>

      <!-- Instructions rapides -->
      <div class="game-instructions">
        <p><strong>Comment jouer :</strong></p>
        <ul>
          <li>Cliquez sur une île, puis sur une autre pour créer un pont</li>
          <li>Cliquez à nouveau pour créer un pont double</li>
          <li>Cliquez une 3ème fois pour supprimer le pont</li>
          <li>Les îles en vert sont complètes ✓</li>
        </ul>
      </div>
    </div>
  </div>
</template>

<style scoped>
.game-view {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 2rem;
}

/* Loading et Error */
.game-view__loading,
.game-view__error {
  max-width: 600px;
  margin: 0 auto;
  text-align: center;
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 1rem;
  padding: 4rem 2rem;
  color: white;
}

.loading-spinner {
  width: 50px;
  height: 50px;
  border: 4px solid rgba(255, 255, 255, 0.3);
  border-top-color: white;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin: 0 auto 1rem;
}

@keyframes spin {
  to {
    transform: rotate(360deg);
  }
}

.game-view__error h2 {
  margin-top: 0;
}

.btn {
  margin-top: 1rem;
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 0.5rem;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn--primary {
  background: white;
  color: #667eea;
}

.btn--primary:hover {
  transform: translateY(-2px);
}

/* Contenu du jeu */
.game-view__content {
  max-width: 1400px;
  margin: 0 auto;
}

/* Header */
.game-header {
  display: flex;
  align-items: center;
  gap: 2rem;
  margin-bottom: 2rem;
  flex-wrap: wrap;
}

.btn-back {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: none;
  padding: 0.75rem 1.5rem;
  border-radius: 0.5rem;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.2s ease;
}

.btn-back:hover {
  background: rgba(255, 255, 255, 0.3);
}

.game-title {
  flex: 1;
  color: white;
  font-size: 2rem;
  margin: 0;
}

.game-difficulty {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  padding: 0.5rem 1rem;
  border-radius: 0.5rem;
  font-weight: 600;
}

/* Sections */
.game-controls-section {
  margin-bottom: 2rem;
}

.game-grid-section {
  margin-bottom: 2rem;
}

/* Instructions */
.game-instructions {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 1rem;
  padding: 1.5rem;
  color: white;
}

.game-instructions p {
  margin: 0 0 0.5rem 0;
}

.game-instructions ul {
  margin: 0;
  padding-left: 1.5rem;
}

.game-instructions li {
  margin-bottom: 0.5rem;
}

/* Responsive */
@media (max-width: 768px) {
  .game-view {
    padding: 1rem;
  }

  .game-header {
    flex-direction: column;
    align-items: stretch;
    gap: 1rem;
  }

  .game-title {
    font-size: 1.5rem;
  }
}
</style>

