<template>
  <div class="leaderboard-panel">
    <h3 class="leaderboard-title">🏆 Classement</h3>

    <div v-if="isLoading" class="loading">
      <p>Chargement du classement...</p>
    </div>

    <div v-else-if="error" class="error">
      <p class="error-message">{{ error }}</p>
    </div>

    <div v-else-if="leaderboard.length === 0" class="no-leaderboard">
      <p>Aucun joueur dans le classement pour le moment.</p>
    </div>

    <div v-else class="leaderboard-content">
      <div class="leaderboard-list">
        <div
          v-for="entry in leaderboard"
          :key="entry.userId"
          class="leaderboard-entry"
          :class="{ 'is-current-user': isCurrentUser(entry.userId) }"
        >
          <div class="rank">{{ getRankIcon(entry.rank) }}</div>
          <div class="user-info">
            <div class="user-name">{{ entry.userName }}</div>
            <div class="user-details">
              <span>{{ entry.gamesCompleted }} parties</span>
              <span>•</span>
              <span>Moyenne: {{ entry.averageScore.toFixed(0) }}</span>
            </div>
          </div>
          <div class="score">{{ entry.totalScore.toLocaleString() }}</div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted } from 'vue'
import { useStatsStore } from '@/stores/stats'
import { useUserStore } from '@/stores/user'

const statsStore = useStatsStore()
const userStore = useUserStore()

const leaderboard = computed(() => statsStore.leaderboard)
const isLoading = computed(() => statsStore.isLoadingLeaderboard)
const error = computed(() => statsStore.error)

function isCurrentUser(userId: number): boolean {
  return userStore.user?.id === userId
}

function getRankIcon(rank: number): string {
  switch (rank) {
    case 1:
      return '🥇'
    case 2:
      return '🥈'
    case 3:
      return '🥉'
    default:
      return `#${rank}`
  }
}

onMounted(async () => {
  try {
    await statsStore.loadLeaderboard(10)
  } catch (err) {
    console.error('Erreur lors du chargement du classement:', err)
  }
})
</script>

<style scoped>
.leaderboard-panel {
  background: white;
  border-radius: 1rem;
  padding: 1.5rem;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
}

.leaderboard-title {
  font-size: 1.5rem;
  font-weight: bold;
  margin-bottom: 1.5rem;
  color: #1f2937;
  text-align: center;
}

.loading,
.error,
.no-leaderboard {
  text-align: center;
  padding: 2rem;
  color: #666;
}

.error-message {
  color: #dc2626;
  font-weight: 500;
}

.leaderboard-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
}

.leaderboard-entry {
  display: flex;
  align-items: center;
  gap: 1rem;
  padding: 1rem;
  background: #f9fafb;
  border-radius: 0.5rem;
  transition: all 0.2s;
}

.leaderboard-entry:hover {
  background: #f3f4f6;
  transform: translateX(4px);
}

.leaderboard-entry.is-current-user {
  background: #dbeafe;
  border: 2px solid #3b82f6;
  font-weight: 500;
}

.rank {
  font-size: 1.25rem;
  font-weight: bold;
  min-width: 3rem;
  text-align: center;
  color: #667eea;
}

.user-info {
  flex: 1;
}

.user-name {
  font-size: 1rem;
  font-weight: 600;
  color: #1f2937;
  margin-bottom: 0.25rem;
}

.user-details {
  font-size: 0.875rem;
  color: #6b7280;
  display: flex;
  gap: 0.5rem;
}

.score {
  font-size: 1.25rem;
  font-weight: bold;
  color: #1f2937;
  min-width: 6rem;
  text-align: right;
}
</style>

