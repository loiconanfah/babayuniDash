<template>
  <div class="user-stats-panel">
    <div v-if="isLoading" class="loading">
      <p>Chargement des statistiques...</p>
    </div>

    <div v-else-if="error" class="error">
      <p class="error-message">{{ error }}</p>
    </div>

    <div v-else-if="stats" class="stats-content">
      <h3 class="stats-title">📊 Mes Statistiques</h3>

      <!-- Statistiques générales -->
      <div class="stats-section">
        <h4 class="section-title">Général</h4>
        <div class="stats-grid">
          <div class="stat-item">
            <div class="stat-label">Score Total</div>
            <div class="stat-value">{{ stats.totalScore.toLocaleString() }}</div>
          </div>
          <div class="stat-item">
            <div class="stat-label">Score Moyen</div>
            <div class="stat-value">{{ stats.averageScore.toFixed(0) }}</div>
          </div>
          <div class="stat-item">
            <div class="stat-label">Meilleur Score</div>
            <div class="stat-value">{{ stats.bestScore.toLocaleString() }}</div>
          </div>
          <div class="stat-item">
            <div class="stat-label">Parties Jouées</div>
            <div class="stat-value">{{ stats.totalGamesPlayed }}</div>
          </div>
          <div class="stat-item">
            <div class="stat-label">Parties Complétées</div>
            <div class="stat-value">{{ stats.gamesCompleted }}</div>
          </div>
          <div class="stat-item">
            <div class="stat-label">Temps Total</div>
            <div class="stat-value">{{ formatTime(stats.totalPlayTime) }}</div>
          </div>
        </div>
      </div>

      <!-- Statistiques par niveau -->
      <div v-if="Object.keys(stats.statsByLevel).length > 0" class="stats-section">
        <h4 class="section-title">Par Niveau</h4>
        <div class="level-stats">
          <div
            v-for="(levelStats, level) in stats.statsByLevel"
            :key="level"
            class="level-stat-item"
          >
            <div class="level-header">
              <span class="level-name">{{ getDifficultyName(Number(level)) }}</span>
            </div>
            <div class="level-details">
              <div class="level-detail">
                <span class="detail-label">Parties:</span>
                <span class="detail-value">{{ levelStats.gamesPlayed }} ({{ levelStats.gamesCompleted }} complétées)</span>
              </div>
              <div class="level-detail">
                <span class="detail-label">Meilleur Score:</span>
                <span class="detail-value">{{ levelStats.bestScore.toLocaleString() }}</span>
              </div>
              <div class="level-detail">
                <span class="detail-label">Score Moyen:</span>
                <span class="detail-value">{{ levelStats.averageScore.toFixed(0) }}</span>
              </div>
              <div class="level-detail">
                <span class="detail-label">Temps Moyen:</span>
                <span class="detail-value">{{ formatTime(Math.round(levelStats.averageTime)) }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <div v-else class="no-stats">
        <p>Aucune statistique disponible. Commencez à jouer pour voir vos scores !</p>
      </div>
    </div>

    <div v-else class="no-stats">
      <p>Aucune statistique disponible.</p>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useStatsStore } from '@/stores/stats'
import { DifficultyLevel } from '@/types'

const statsStore = useStatsStore()

const stats = computed(() => statsStore.userStats)
const isLoading = computed(() => statsStore.isLoadingStats)
const error = computed(() => statsStore.error)

function formatTime(seconds: number): string {
  return statsStore.formatTime(seconds)
}

function getDifficultyName(level: number): string {
  switch (level) {
    case DifficultyLevel.Easy:
      return '🟢 Facile'
    case DifficultyLevel.Medium:
      return '🟡 Moyen'
    case DifficultyLevel.Hard:
      return '🔴 Difficile'
    default:
      return `Niveau ${level}`
  }
}
</script>

<style scoped>
.user-stats-panel {
  background: white;
  border-radius: 1rem;
  padding: 1.5rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.loading,
.error,
.no-stats {
  text-align: center;
  padding: 2rem;
  color: #666;
}

.error-message {
  color: #dc2626;
  font-weight: 500;
}

.stats-title {
  font-size: 1.5rem;
  font-weight: bold;
  margin-bottom: 1.5rem;
  color: #1f2937;
}

.stats-section {
  margin-bottom: 2rem;
}

.section-title {
  font-size: 1.1rem;
  font-weight: 600;
  margin-bottom: 1rem;
  color: #374151;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(150px, 1fr));
  gap: 1rem;
}

.stat-item {
  background: #f9fafb;
  border-radius: 0.5rem;
  padding: 1rem;
  text-align: center;
}

.stat-label {
  font-size: 0.875rem;
  color: #6b7280;
  margin-bottom: 0.5rem;
}

.stat-value {
  font-size: 1.5rem;
  font-weight: bold;
  color: #1f2937;
}

.level-stats {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.level-stat-item {
  background: #f9fafb;
  border-radius: 0.5rem;
  padding: 1rem;
  border-left: 4px solid #667eea;
}

.level-header {
  margin-bottom: 0.75rem;
}

.level-name {
  font-size: 1rem;
  font-weight: 600;
  color: #1f2937;
}

.level-details {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 0.5rem;
}

.level-detail {
  display: flex;
  justify-content: space-between;
  font-size: 0.875rem;
}

.detail-label {
  color: #6b7280;
}

.detail-value {
  font-weight: 500;
  color: #1f2937;
}
</style>

