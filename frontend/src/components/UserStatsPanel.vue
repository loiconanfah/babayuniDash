<template>
  <div class="user-stats-panel">
    <div v-if="isLoading" class="loading">
      <p>Chargement des statistiques...</p>
    </div>

    <div v-else-if="error" class="error">
      <p class="error-message">{{ error }}</p>
    </div>

    <div v-else-if="stats" class="stats-content">
      <div class="stats-title-wrapper">
        <div class="title-icon">
          <IconStats />
        </div>
        <h3 class="stats-title">Mes Statistiques</h3>
      </div>

      <!-- Statistiques générales -->
      <div class="stats-section">
        <h4 class="section-title">
          <svg xmlns="http://www.w3.org/2000/svg" class="section-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
          </svg>
          Général
        </h4>
        <div class="stats-grid">
          <div class="stat-item stat-item-primary">
            <div class="stat-icon-wrapper">
              <svg xmlns="http://www.w3.org/2000/svg" class="stat-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M12 6v12m-3-3h6m-9 0V9a2 2 0 012-2h4a2 2 0 012 2v6a2 2 0 01-2 2H6a2 2 0 01-2-2z" />
              </svg>
            </div>
            <div class="stat-label">Score Total</div>
            <div class="stat-value">{{ stats.totalScore.toLocaleString() }}</div>
          </div>
          <div class="stat-item stat-item-secondary">
            <div class="stat-icon-wrapper">
              <svg xmlns="http://www.w3.org/2000/svg" class="stat-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
              </svg>
            </div>
            <div class="stat-label">Score Moyen</div>
            <div class="stat-value">{{ stats.averageScore.toFixed(0) }}</div>
          </div>
          <div class="stat-item stat-item-success">
            <div class="stat-icon-wrapper">
              <svg xmlns="http://www.w3.org/2000/svg" class="stat-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M5 3v4M3 5h4M6 17v4m-2-2h4m5-16l2.286 6.857L21 12l-5.714 2.143L13 21l-2.286-6.857L5 12l5.714-2.143L13 3z" />
              </svg>
            </div>
            <div class="stat-label">Meilleur Score</div>
            <div class="stat-value">{{ stats.bestScore.toLocaleString() }}</div>
          </div>
          <div class="stat-item stat-item-info">
            <div class="stat-icon-wrapper">
              <svg xmlns="http://www.w3.org/2000/svg" class="stat-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M14.751 9.75l3.501 3.75m0 0l3.499-3.75M18.252 13.5H21m-2.25 0v6.75m-9-9.75H5.25A2.25 2.25 0 003 12.75v6.75A2.25 2.25 0 005.25 22h13.5A2.25 2.25 0 0021 19.5v-6.75a2.25 2.25 0 00-2.25-2.25h-4.752m-9 0H3m2.25 0h4.752M9.75 3v3m0 0v3m0-3h3m-3 0H6.75" />
              </svg>
            </div>
            <div class="stat-label">Parties Jouées</div>
            <div class="stat-value">{{ stats.totalGamesPlayed }}</div>
          </div>
          <div class="stat-item stat-item-warning">
            <div class="stat-icon-wrapper">
              <svg xmlns="http://www.w3.org/2000/svg" class="stat-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <div class="stat-label">Parties Complétées</div>
            <div class="stat-value">{{ stats.gamesCompleted }}</div>
          </div>
          <div class="stat-item stat-item-time">
            <div class="stat-icon-wrapper">
              <svg xmlns="http://www.w3.org/2000/svg" class="stat-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
              </svg>
            </div>
            <div class="stat-label">Temps Total</div>
            <div class="stat-value">{{ formatTime(stats.totalPlayTime) }}</div>
          </div>
        </div>
      </div>

      <!-- Statistiques par niveau -->
      <div v-if="Object.keys(stats.statsByLevel).length > 0" class="stats-section">
        <h4 class="section-title">
          <svg xmlns="http://www.w3.org/2000/svg" class="section-icon" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round" d="M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z" />
          </svg>
          Par Niveau
        </h4>
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
import IconStats from '@/components/icons/IconStats.vue'

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
      return 'Facile'
    case DifficultyLevel.Medium:
      return 'Moyen'
    case DifficultyLevel.Hard:
      return 'Difficile'
    default:
      return `Niveau ${level}`
  }
}

function getDifficultyColor(level: number): string {
  switch (level) {
    case DifficultyLevel.Easy:
      return '#10b981' // green-500
    case DifficultyLevel.Medium:
      return '#f59e0b' // amber-500
    case DifficultyLevel.Hard:
      return '#ef4444' // red-500
    default:
      return '#6b7280' // gray-500
  }
}
</script>

<style scoped>
.user-stats-panel {
  background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
  border-radius: 1.5rem;
  padding: 2rem;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(148, 163, 184, 0.1);
}

.loading,
.error,
.no-stats {
  text-align: center;
  padding: 2rem;
  color: #94a3b8;
}

.error-message {
  color: #ef4444;
  font-weight: 500;
}

@media (max-width: 768px) {
  .user-stats-panel {
    padding: 1.5rem 1rem;
  }

  .stats-title-wrapper {
    flex-direction: column;
    gap: 0.75rem;
    margin-bottom: 1.5rem;
    padding-bottom: 1rem;
  }

  .title-icon {
    width: 40px;
    height: 40px;
  }

  .title-icon svg {
    width: 20px;
    height: 20px;
  }

  .stats-title {
    font-size: 1.5rem;
  }

  .section-title {
    font-size: 1.125rem;
    margin-bottom: 1.25rem;
  }

  .section-icon {
    width: 20px;
    height: 20px;
  }

  .stats-grid {
    grid-template-columns: repeat(2, 1fr);
    gap: 1rem;
  }

  .stat-item {
    padding: 1.25rem 0.75rem;
  }

  .stat-icon-wrapper {
    width: 40px;
    height: 40px;
    margin-bottom: 0.5rem;
  }

  .stat-icon {
    width: 20px;
    height: 20px;
  }

  .stat-value {
    font-size: 1.5rem;
  }

  .stat-label {
    font-size: 0.75rem;
  }

  .level-stat-item {
    padding: 1.25rem;
  }

  .level-details {
    grid-template-columns: 1fr;
    gap: 0.75rem;
  }
}

.stats-title-wrapper {
  display: flex;
  align-items: center;
  gap: 1rem;
  margin-bottom: 2rem;
  padding-bottom: 1.5rem;
  border-bottom: 2px solid rgba(148, 163, 184, 0.1);
}

.title-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 48px;
  height: 48px;
  background: linear-gradient(135deg, #06b6d4 0%, #8b5cf6 50%, #ec4899 100%);
  border-radius: 12px;
  color: white;
}

.title-icon svg {
  width: 24px;
  height: 24px;
}

.stats-title {
  font-size: 1.75rem;
  font-weight: 700;
  margin: 0;
  color: white;
}

.stats-section {
  margin-bottom: 2rem;
}

.section-title {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 1.5rem;
  color: white;
}

.section-icon {
  width: 24px;
  height: 24px;
  color: #06b6d4;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
  gap: 1.25rem;
}

.stat-item {
  background: rgba(30, 41, 59, 0.6);
  border-radius: 1rem;
  padding: 1.5rem 1rem;
  text-align: center;
  border: 1px solid rgba(148, 163, 184, 0.1);
  transition: all 0.3s ease;
  position: relative;
  overflow: hidden;
}

.stat-item::before {
  content: '';
  position: absolute;
  top: 0;
  left: 0;
  right: 0;
  height: 3px;
  background: linear-gradient(90deg, transparent, currentColor, transparent);
  opacity: 0;
  transition: opacity 0.3s ease;
}

.stat-item:hover {
  transform: translateY(-4px);
  box-shadow: 0 8px 24px rgba(0, 0, 0, 0.4);
  border-color: rgba(148, 163, 184, 0.3);
}

.stat-item:hover::before {
  opacity: 1;
}

.stat-item-primary::before {
  color: #3b82f6;
}

.stat-item-secondary::before {
  color: #8b5cf6;
}

.stat-item-success::before {
  color: #10b981;
}

.stat-item-info::before {
  color: #06b6d4;
}

.stat-item-warning::before {
  color: #f59e0b;
}

.stat-item-time::before {
  color: #ec4899;
}

.stat-icon-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 48px;
  height: 48px;
  margin: 0 auto 0.75rem;
  border-radius: 12px;
  background: rgba(148, 163, 184, 0.1);
}

.stat-icon {
  width: 24px;
  height: 24px;
  color: #06b6d4;
}

.stat-item-primary .stat-icon {
  color: #3b82f6;
}

.stat-item-secondary .stat-icon {
  color: #8b5cf6;
}

.stat-item-success .stat-icon {
  color: #10b981;
}

.stat-item-info .stat-icon {
  color: #06b6d4;
}

.stat-item-warning .stat-icon {
  color: #f59e0b;
}

.stat-item-time .stat-icon {
  color: #ec4899;
}

.stat-label {
  font-size: 0.875rem;
  color: #94a3b8;
  margin-bottom: 0.5rem;
  font-weight: 500;
}

.stat-value {
  font-size: 1.75rem;
  font-weight: 700;
  color: white;
}

.level-stats {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.level-stat-item {
  background: rgba(30, 41, 59, 0.6);
  border-radius: 1rem;
  padding: 1.5rem;
  border-left: 4px solid #06b6d4;
  border: 1px solid rgba(148, 163, 184, 0.1);
  transition: all 0.3s ease;
}

.level-stat-item:hover {
  transform: translateX(4px);
  border-color: rgba(6, 182, 212, 0.3);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
}

.level-header {
  margin-bottom: 0.75rem;
}

.level-name {
  font-size: 1.125rem;
  font-weight: 600;
  color: white;
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
  color: #94a3b8;
}

.detail-value {
  font-weight: 600;
  color: white;
}
</style>

