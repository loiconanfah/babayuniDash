<template>
  <div class="games-view">
    <div class="container">
      <!-- En-tête -->
      <div class="header">
        <h1>🎮 Jeux Disponibles</h1>
        <button @click="goBack" class="btn btn-secondary">
          Retour
        </button>
      </div>

      <!-- Liste des jeux -->
      <div class="games-grid">
        <!-- Jeu Hashi -->
        <div class="game-card">
          <div class="game-icon">🌉</div>
          <h2 class="game-title">Hashi</h2>
          <p class="game-description">
            Connectez les îles avec des ponts pour résoudre des puzzles logiques.
            Plusieurs niveaux de difficulté disponibles.
          </p>
          <div class="game-features">
            <span class="feature-badge">🎯 Solo</span>
            <span class="feature-badge">📊 Scores</span>
            <span class="feature-badge">🏆 Classement</span>
          </div>
          <button @click="goToHashi" class="btn btn-primary btn-large">
            Jouer à Hashi
          </button>
        </div>

        <!-- Jeu Tic-Tac-Toe -->
        <div class="game-card">
          <div class="game-icon">⭕</div>
          <h2 class="game-title">Tic-Tac-Toe</h2>
          <p class="game-description">
            Le classique jeu de morpion. Jouez contre l'IA ou défiez d'autres joueurs en ligne.
          </p>
          <div class="game-features">
            <span class="feature-badge">🤖 IA</span>
            <span class="feature-badge">👥 Multijoueur</span>
            <span class="feature-badge">⚡ Rapide</span>
          </div>
          <button @click="goToTicTacToe" class="btn btn-primary btn-large">
            Jouer à Tic-Tac-Toe
          </button>
        </div>
      </div>

      <!-- Statistiques rapides -->
      <div class="quick-stats">
        <div class="stat-card">
          <div class="stat-icon">🎮</div>
          <div class="stat-info">
            <div class="stat-value">2</div>
            <div class="stat-label">Jeux disponibles</div>
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-icon">🌉</div>
          <div class="stat-info">
            <div class="stat-value">Hashi</div>
            <div class="stat-label">Puzzle logique</div>
          </div>
        </div>
        <div class="stat-card">
          <div class="stat-icon">⭕</div>
          <div class="stat-info">
            <div class="stat-value">Tic-Tac-Toe</div>
            <div class="stat-label">Stratégie rapide</div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRouter } from 'vue-router'

const router = useRouter()

/**
 * Retour au menu principal
 */
function goBack() {
  router.push('/')
}

/**
 * Navigation vers Hashi
 */
function goToHashi() {
  router.push('/puzzles')
}

/**
 * Navigation vers Tic-Tac-Toe
 */
function goToTicTacToe() {
  router.push('/tic-tac-toe')
}
</script>

<style scoped>
.games-view {
  min-height: 100vh;
  padding: 20px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.container {
  max-width: 1200px;
  margin: 0 auto;
  background: white;
  border-radius: 12px;
  padding: 32px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 32px;
  padding-bottom: 16px;
  border-bottom: 2px solid #e0e0e0;
}

.header h1 {
  margin: 0;
  color: #333;
  font-size: 2.5rem;
}

.games-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 24px;
  margin-bottom: 32px;
}

.game-card {
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  border-radius: 16px;
  padding: 32px;
  text-align: center;
  transition: all 0.3s ease;
  border: 2px solid transparent;
  display: flex;
  flex-direction: column;
  gap: 16px;
}

.game-card:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.15);
  border-color: #667eea;
}

.game-icon {
  font-size: 4rem;
  margin-bottom: 8px;
}

.game-title {
  margin: 0;
  color: #333;
  font-size: 1.8rem;
  font-weight: 700;
}

.game-description {
  color: #666;
  line-height: 1.6;
  flex-grow: 1;
  margin: 0;
}

.game-features {
  display: flex;
  justify-content: center;
  gap: 8px;
  flex-wrap: wrap;
  margin: 8px 0;
}

.feature-badge {
  background: rgba(102, 126, 234, 0.1);
  color: #667eea;
  padding: 4px 12px;
  border-radius: 12px;
  font-size: 0.85rem;
  font-weight: 500;
}

.btn {
  padding: 12px 24px;
  border: none;
  border-radius: 8px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 600;
  transition: all 0.2s ease;
}

.btn-primary {
  background: #667eea;
  color: white;
}

.btn-primary:hover {
  background: #5568d3;
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(102, 126, 234, 0.4);
}

.btn-secondary {
  background: #6c757d;
  color: white;
}

.btn-secondary:hover {
  background: #5a6268;
}

.btn-large {
  padding: 14px 28px;
  font-size: 1.1rem;
  width: 100%;
  margin-top: auto;
}

.quick-stats {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
  margin-top: 32px;
  padding-top: 32px;
  border-top: 2px solid #e0e0e0;
}

.stat-card {
  background: #f8f9fa;
  border-radius: 12px;
  padding: 20px;
  display: flex;
  align-items: center;
  gap: 16px;
  transition: all 0.2s ease;
}

.stat-card:hover {
  background: #e9ecef;
  transform: translateY(-2px);
}

.stat-icon {
  font-size: 2.5rem;
}

.stat-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.stat-value {
  font-size: 1.5rem;
  font-weight: 700;
  color: #333;
}

.stat-label {
  font-size: 0.9rem;
  color: #666;
}

@media (max-width: 768px) {
  .container {
    padding: 20px;
  }

  .header {
    flex-direction: column;
    gap: 16px;
    align-items: stretch;
  }

  .header h1 {
    font-size: 2rem;
    text-align: center;
  }

  .games-grid {
    grid-template-columns: 1fr;
  }

  .quick-stats {
    grid-template-columns: 1fr;
  }
}
</style>

