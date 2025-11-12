<script setup lang="ts">
/**
 * Vue de génération de puzzle
 * Permet de créer un nouveau puzzle personnalisé
 */

import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { usePuzzleStore } from '@/stores/puzzle'
import { DifficultyLevel } from '@/types'

const router = useRouter()
const puzzleStore = usePuzzleStore()

// Formulaire
const width = ref(10)
const height = ref(10)
const difficulty = ref(DifficultyLevel.Medium)

const isGenerating = ref(false)
const error = ref<string | null>(null)

/**
 * Génère le puzzle et commence une partie
 */
async function generateAndPlay() {
  try {
    error.value = null
    isGenerating.value = true

    // Validation
    if (width.value < 5 || width.value > 20) {
      error.value = 'La largeur doit être entre 5 et 20'
      return
    }

    if (height.value < 5 || height.value > 20) {
      error.value = 'La hauteur doit être entre 5 et 20'
      return
    }

    // Générer le puzzle
    const puzzle = await puzzleStore.generatePuzzle({
      width: width.value,
      height: height.value,
      difficulty: difficulty.value
    })

    // Rediriger vers la vue de jeu
    router.push(`/play/${puzzle.id}`)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Erreur lors de la génération'
  } finally {
    isGenerating.value = false
  }
}

/**
 * Retour au menu
 */
function goBack() {
  router.push('/')
}
</script>

<template>
  <div class="generate-puzzle">
    <div class="container">
      <!-- Header -->
      <div class="header">
        <button class="btn-back" @click="goBack">← Retour</button>
        <h1 class="title">Générer un nouveau puzzle</h1>
      </div>

      <!-- Formulaire -->
      <div class="form-card">
        <div class="form-group">
          <label for="width" class="form-label">
            Largeur de la grille (5-20)
          </label>
          <input
            id="width"
            v-model.number="width"
            type="number"
            min="5"
            max="20"
            class="form-input"
          />
          <div class="form-hint">
            Valeur actuelle: {{ width }} colonnes
          </div>
        </div>

        <div class="form-group">
          <label for="height" class="form-label">
            Hauteur de la grille (5-20)
          </label>
          <input
            id="height"
            v-model.number="height"
            type="number"
            min="5"
            max="20"
            class="form-input"
          />
          <div class="form-hint">
            Valeur actuelle: {{ height }} lignes
          </div>
        </div>

        <div class="form-group">
          <label class="form-label">Difficulté</label>
          <div class="difficulty-buttons">
            <button
              class="difficulty-btn"
              :class="{ 'difficulty-btn--active': difficulty === DifficultyLevel.Easy }"
              @click="difficulty = DifficultyLevel.Easy"
            >
              Facile
            </button>
            <button
              class="difficulty-btn"
              :class="{ 'difficulty-btn--active': difficulty === DifficultyLevel.Medium }"
              @click="difficulty = DifficultyLevel.Medium"
            >
              Moyen
            </button>
            <button
              class="difficulty-btn"
              :class="{ 'difficulty-btn--active': difficulty === DifficultyLevel.Hard }"
              @click="difficulty = DifficultyLevel.Hard"
            >
              Difficile
            </button>
            <button
              class="difficulty-btn"
              :class="{ 'difficulty-btn--active': difficulty === DifficultyLevel.Expert }"
              @click="difficulty = DifficultyLevel.Expert"
            >
              Expert
            </button>
          </div>
        </div>

        <!-- Erreur -->
        <div v-if="error" class="error-message">
          {{ error }}
        </div>

        <!-- Bouton de génération -->
        <button
          class="btn-generate"
          :disabled="isGenerating"
          @click="generateAndPlay"
        >
          {{ isGenerating ? 'Génération en cours...' : '✨ Générer et jouer' }}
        </button>

        <!-- Note -->
        <div class="note">
          <strong>Note :</strong> La génération de puzzles est actuellement simplifiée.
          Les puzzles générés peuvent ne pas avoir une solution unique garantie.
          Pour une expérience optimale, jouez aux puzzles prédéfinis.
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.generate-puzzle {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 2rem;
}

.container {
  max-width: 800px;
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

/* Formulaire */
.form-card {
  background: white;
  border-radius: 1rem;
  padding: 2rem;
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
}

.form-group {
  margin-bottom: 2rem;
}

.form-label {
  display: block;
  font-weight: 600;
  font-size: 1.125rem;
  color: #2d3748;
  margin-bottom: 0.5rem;
}

.form-input {
  width: 100%;
  padding: 0.75rem;
  border: 2px solid #e2e8f0;
  border-radius: 0.5rem;
  font-size: 1rem;
  transition: border-color 0.2s ease;
}

.form-input:focus {
  outline: none;
  border-color: #667eea;
}

.form-hint {
  margin-top: 0.5rem;
  font-size: 0.875rem;
  color: #718096;
}

/* Boutons de difficulté */
.difficulty-buttons {
  display: grid;
  grid-template-columns: repeat(2, 1fr);
  gap: 1rem;
}

.difficulty-btn {
  padding: 1rem;
  border: 2px solid #e2e8f0;
  border-radius: 0.5rem;
  background: white;
  font-size: 1rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
}

.difficulty-btn:hover {
  border-color: #667eea;
  background: #f7fafc;
}

.difficulty-btn--active {
  border-color: #667eea;
  background: #667eea;
  color: white;
}

/* Erreur */
.error-message {
  padding: 1rem;
  background: #fed7d7;
  border: 1px solid #fc8181;
  border-radius: 0.5rem;
  color: #c53030;
  margin-bottom: 1rem;
}

/* Bouton de génération */
.btn-generate {
  width: 100%;
  padding: 1rem 2rem;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  border: none;
  border-radius: 0.5rem;
  font-size: 1.25rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
}

.btn-generate:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
}

.btn-generate:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

/* Note */
.note {
  margin-top: 1.5rem;
  padding: 1rem;
  background: #fef5e7;
  border: 1px solid #f9e79f;
  border-radius: 0.5rem;
  font-size: 0.875rem;
  color: #7d6608;
}

/* Responsive */
@media (max-width: 768px) {
  .title {
    font-size: 1.75rem;
  }

  .difficulty-buttons {
    grid-template-columns: 1fr;
  }
}
</style>

