<script setup lang="ts">
/**
 * Vue du menu principal
 * Point d'entrée de l'application
 */

import { useRouter } from 'vue-router'

const router = useRouter()

/**
 * Navigation vers la sélection de puzzles
 */
function goToSelectPuzzle() {
  router.push('/puzzles')
}

/**
 * Navigation vers la génération d'un nouveau puzzle
 */
function goToGeneratePuzzle() {
  router.push('/generate')
}

/**
 * Navigation vers les statistiques
 */
function goToStats() {
  router.push('/stats')
}
</script>

<template>
  <div class="menu-view">
    <div class="menu-container">
      <!-- Logo et titre -->
      <div class="menu-header">
        <h1 class="menu-title">🌉 Hashi</h1>
        <p class="menu-subtitle">Le jeu de puzzle des ponts</p>
      </div>

      <!-- Boutons du menu -->
      <div class="menu-actions">
        <button class="menu-btn menu-btn--primary" @click="goToSelectPuzzle">
          <span class="menu-btn__icon">🎮</span>
          <span class="menu-btn__text">Jouer à un puzzle</span>
        </button>

        <button class="menu-btn menu-btn--secondary" @click="goToGeneratePuzzle">
          <span class="menu-btn__icon">✨</span>
          <span class="menu-btn__text">Générer un nouveau puzzle</span>
        </button>

        <button class="menu-btn menu-btn--secondary" @click="goToStats">
          <span class="menu-btn__icon">📊</span>
          <span class="menu-btn__text">Statistiques & Classement</span>
        </button>
      </div>

      <!-- Instructions -->
      <div class="menu-instructions">
        <h3 class="instructions-title">Comment jouer ?</h3>
        <ul class="instructions-list">
          <li>Reliez toutes les îles avec des ponts</li>
          <li>Le nombre sur chaque île indique combien de ponts doivent y être connectés</li>
          <li>Les ponts peuvent être simples ou doubles</li>
          <li>Les ponts ne peuvent pas se croiser</li>
          <li>Toutes les îles doivent être connectées en un seul réseau</li>
        </ul>
      </div>
    </div>
  </div>
</template>

<style scoped>
.menu-view {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 2rem;
}

.menu-container {
  max-width: 600px;
  width: 100%;
}

/* Header */
.menu-header {
  text-align: center;
  margin-bottom: 3rem;
  color: white;
}

.menu-title {
  font-size: 4rem;
  font-weight: bold;
  margin: 0 0 1rem 0;
  text-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.menu-subtitle {
  font-size: 1.5rem;
  opacity: 0.9;
  margin: 0;
}

/* Actions */
.menu-actions {
  display: flex;
  flex-direction: column;
  gap: 1rem;
  margin-bottom: 3rem;
}

.menu-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 1rem;
  padding: 1.5rem 2rem;
  border: none;
  border-radius: 1rem;
  font-size: 1.25rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.3s ease;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.menu-btn:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
}

.menu-btn--primary {
  background: white;
  color: #667eea;
}

.menu-btn--secondary {
  background: rgba(255, 255, 255, 0.2);
  color: white;
  border: 2px solid white;
}

.menu-btn__icon {
  font-size: 2rem;
}

/* Instructions */
.menu-instructions {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(10px);
  border-radius: 1rem;
  padding: 2rem;
  color: white;
}

.instructions-title {
  margin: 0 0 1rem 0;
  font-size: 1.5rem;
}

.instructions-list {
  margin: 0;
  padding-left: 1.5rem;
  line-height: 1.8;
}

.instructions-list li {
  margin-bottom: 0.5rem;
}

/* Responsive */
@media (max-width: 768px) {
  .menu-title {
    font-size: 3rem;
  }

  .menu-subtitle {
    font-size: 1.25rem;
  }

  .menu-btn {
    font-size: 1rem;
  }
}
</style>

