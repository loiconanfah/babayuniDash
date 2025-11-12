<script setup lang="ts">
/**
 * Vue de sélection de puzzles
 * Permet de choisir un puzzle existant parmi différentes difficultés
 */

import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'
import { usePuzzleStore } from '@/stores/puzzle'
import { DifficultyLevel } from '@/types'

const router = useRouter()
const puzzleStore = usePuzzleStore()

/** Difficulté sélectionnée pour le filtre */
const selectedDifficulty = ref<DifficultyLevel | null>(null)

/**
 * Charge les puzzles au montage du composant
 */
onMounted(async () => {
  await puzzleStore.fetchAllPuzzles()
})

/**
 * Filtre les puzzles par difficulté
 */
async function filterByDifficulty(difficulty: DifficultyLevel | null) {
  selectedDifficulty.value = difficulty
  
  if (difficulty === null) {
    await puzzleStore.fetchAllPuzzles()
  } else {
    await puzzleStore.fetchPuzzlesByDifficulty(difficulty)
  }
}

/**
 * Commence une partie avec le puzzle sélectionné
 */
function playPuzzle(puzzleId: number) {
  router.push(`/play/${puzzleId}`)
}

/**
 * Retour au menu
 */
function goBack() {
  router.push('/')
}

/**
 * Obtient le label de difficulté
 */
function getDifficultyLabel(difficulty: number): string {
  switch (difficulty) {
    case DifficultyLevel.Easy:
      return 'Facile'
    case DifficultyLevel.Medium:
      return 'Moyen'
    case DifficultyLevel.Hard:
      return 'Difficile'
    case DifficultyLevel.Expert:
      return 'Expert'
    default:
      return 'Inconnu'
  }
}

/**
 * Obtient la classe CSS de difficulté
 */
function getDifficultyClass(difficulty: number): string {
  switch (difficulty) {
    case DifficultyLevel.Easy:
      return 'difficulty--easy'
    case DifficultyLevel.Medium:
      return 'difficulty--medium'
    case DifficultyLevel.Hard:
      return 'difficulty--hard'
    case DifficultyLevel.Expert:
      return 'difficulty--expert'
    default:
      return ''
  }
}
</script>

<template>
  <div class="puzzle-selection">
    <div class="container">
      <!-- Header -->
      <div class="header">
        <button class="btn-back" @click="goBack">← Retour</button>
        <h1 class="title">Sélectionner un puzzle</h1>
      </div>

      <!-- Filtres de difficulté -->
      <div class="filters">
        <button
          class="filter-btn"
          :class="{ 'filter-btn--active': selectedDifficulty === null }"
          @click="filterByDifficulty(null)"
        >
          Tous
        </button>
        <button
          class="filter-btn"
          :class="{ 'filter-btn--active': selectedDifficulty === DifficultyLevel.Easy }"
          @click="filterByDifficulty(DifficultyLevel.Easy)"
        >
          Facile
        </button>
        <button
          class="filter-btn"
          :class="{ 'filter-btn--active': selectedDifficulty === DifficultyLevel.Medium }"
          @click="filterByDifficulty(DifficultyLevel.Medium)"
        >
          Moyen
        </button>
        <button
          class="filter-btn"
          :class="{ 'filter-btn--active': selectedDifficulty === DifficultyLevel.Hard }"
          @click="filterByDifficulty(DifficultyLevel.Hard)"
        >
          Difficile
        </button>
        <button
          class="filter-btn"
          :class="{ 'filter-btn--active': selectedDifficulty === DifficultyLevel.Expert }"
          @click="filterByDifficulty(DifficultyLevel.Expert)"
        >
          Expert
        </button>
      </div>

      <!-- Liste des puzzles -->
      <div v-if="puzzleStore.isLoading" class="loading">
        Chargement des puzzles...
      </div>

      <div v-else-if="puzzleStore.puzzles.length === 0" class="empty-state">
        <p>Aucun puzzle disponible pour le moment.</p>
        <button class="btn btn--primary" @click="router.push('/generate')">
          Générer un nouveau puzzle
        </button>
      </div>

      <div v-else class="puzzles-grid">
        <div
          v-for="puzzle in puzzleStore.puzzles"
          :key="puzzle.id"
          class="puzzle-card"
          @click="playPuzzle(puzzle.id)"
        >
          <div class="puzzle-card__header">
            <h3 class="puzzle-card__title">{{ puzzle.name || `Puzzle #${puzzle.id}` }}</h3>
            <span
              class="puzzle-card__difficulty"
              :class="getDifficultyClass(puzzle.difficulty)"
            >
              {{ getDifficultyLabel(puzzle.difficulty) }}
            </span>
          </div>
          
          <div class="puzzle-card__info">
            <div class="info-item">
              <span class="info-label">Grille:</span>
              <span class="info-value">{{ puzzle.width }}×{{ puzzle.height }}</span>
            </div>
            <div class="info-item">
              <span class="info-label">Îles:</span>
              <span class="info-value">{{ puzzle.islandCount }}</span>
            </div>
          </div>
          
          <button class="puzzle-card__play">Jouer →</button>
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.puzzle-selection {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 2rem;
}

.container {
  max-width: 1200px;
  margin: 0 auto;
}

/* Header */
.header {
  display: flex;
  align-items: center;
  gap: 2rem;
  margin-bottom: 2rem;
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

.title {
  color: white;
  font-size: 2.5rem;
  margin: 0;
}

/* Filtres */
.filters {
  display: flex;
  gap: 1rem;
  margin-bottom: 2rem;
  flex-wrap: wrap;
}

.filter-btn {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: 2px solid transparent;
  padding: 0.75rem 1.5rem;
  border-radius: 0.5rem;
  font-size: 1rem;
  cursor: pointer;
  transition: all 0.2s ease;
}

.filter-btn:hover {
  background: rgba(255, 255, 255, 0.3);
}

.filter-btn--active {
  background: white;
  color: #667eea;
  border-color: white;
}

/* Grille de puzzles */
.puzzles-grid {
  display: grid;
  grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
  gap: 1.5rem;
}

.puzzle-card {
  background: white;
  border-radius: 1rem;
  padding: 1.5rem;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.puzzle-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
}

.puzzle-card__header {
  display: flex;
  justify-content: space-between;
  align-items: start;
  margin-bottom: 1rem;
}

.puzzle-card__title {
  margin: 0;
  font-size: 1.25rem;
  color: #2d3748;
}

.puzzle-card__difficulty {
  padding: 0.25rem 0.75rem;
  border-radius: 0.25rem;
  font-size: 0.875rem;
  font-weight: 600;
}

.difficulty--easy {
  background: #c6f6d5;
  color: #22543d;
}

.difficulty--medium {
  background: #bee3f8;
  color: #1a365d;
}

.difficulty--hard {
  background: #fed7d7;
  color: #742a2a;
}

.difficulty--expert {
  background: #e9d8fd;
  color: #44337a;
}

.puzzle-card__info {
  display: flex;
  gap: 1.5rem;
  margin-bottom: 1rem;
}

.info-item {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.info-label {
  font-size: 0.875rem;
  color: #718096;
}

.info-value {
  font-size: 1.25rem;
  font-weight: 600;
  color: #2d3748;
}

.puzzle-card__play {
  width: 100%;
  background: #667eea;
  color: white;
  border: none;
  padding: 0.75rem;
  border-radius: 0.5rem;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
}

.puzzle-card__play:hover {
  background: #5568d3;
}

/* États */
.loading,
.empty-state {
  text-align: center;
  padding: 4rem;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 1rem;
  color: white;
  font-size: 1.25rem;
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
</style>

