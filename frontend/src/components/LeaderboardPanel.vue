<template>
  <div class="leaderboard-panel">
    <div class="leaderboard-title-wrapper">
      <div class="title-icon">
        <IconLeaderboard />
      </div>
      <h3 class="leaderboard-title">Classement</h3>
    </div>

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
          <div class="rank" :class="getRankClass(entry.rank)">
            <span v-if="entry.rank <= 3" class="rank-medal">{{ getRankIcon(entry.rank) }}</span>
            <span v-else class="rank-number">#{{ entry.rank }}</span>
          </div>
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
import IconLeaderboard from '@/components/icons/IconLeaderboard.vue'

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
      return '1'
    case 2:
      return '2'
    case 3:
      return '3'
    default:
      return `${rank}`
  }
}

function getRankClass(rank: number): string {
  switch (rank) {
    case 1:
      return 'rank-gold'
    case 2:
      return 'rank-silver'
    case 3:
      return 'rank-bronze'
    default:
      return 'rank-default'
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
  background: linear-gradient(135deg, #1e293b 0%, #0f172a 100%);
  border-radius: 1.5rem;
  padding: 2rem;
  box-shadow: 0 10px 40px rgba(0, 0, 0, 0.3);
  border: 1px solid rgba(148, 163, 184, 0.1);
}

.leaderboard-title-wrapper {
  display: flex;
  align-items: center;
  justify-content: center;
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
  background: linear-gradient(135deg, #f59e0b 0%, #ef4444 100%);
  border-radius: 12px;
  color: white;
}

.title-icon svg {
  width: 24px;
  height: 24px;
}

.leaderboard-title {
  font-size: 1.75rem;
  font-weight: 700;
  margin: 0;
  color: white;
}

.loading,
.error,
.no-leaderboard {
  text-align: center;
  padding: 2rem;
  color: #94a3b8;
}

.error-message {
  color: #ef4444;
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
  padding: 1.25rem;
  background: rgba(30, 41, 59, 0.6);
  border-radius: 1rem;
  border: 1px solid rgba(148, 163, 184, 0.1);
  transition: all 0.3s ease;
}

.leaderboard-entry:hover {
  background: rgba(30, 41, 59, 0.8);
  transform: translateX(4px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.2);
  border-color: rgba(148, 163, 184, 0.3);
}

.leaderboard-entry.is-current-user {
  background: linear-gradient(135deg, rgba(6, 182, 212, 0.2) 0%, rgba(139, 92, 246, 0.2) 100%);
  border: 2px solid #06b6d4;
  font-weight: 600;
  box-shadow: 0 4px 16px rgba(6, 182, 212, 0.3);
}

.rank {
  font-size: 1.5rem;
  font-weight: 700;
  min-width: 3.5rem;
  text-align: center;
  display: flex;
  align-items: center;
  justify-content: center;
}

.rank-medal {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 40px;
  height: 40px;
  border-radius: 50%;
  font-weight: 700;
  font-size: 1.25rem;
}

.rank-gold .rank-medal {
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  color: white;
  box-shadow: 0 4px 12px rgba(251, 191, 36, 0.4);
}

.rank-silver .rank-medal {
  background: linear-gradient(135deg, #e5e7eb 0%, #9ca3af 100%);
  color: #1f2937;
  box-shadow: 0 4px 12px rgba(156, 163, 175, 0.4);
}

.rank-bronze .rank-medal {
  background: linear-gradient(135deg, #d97706 0%, #b45309 100%);
  color: white;
  box-shadow: 0 4px 12px rgba(217, 119, 6, 0.4);
}

.rank-number {
  color: #94a3b8;
  font-size: 1.125rem;
}

.user-info {
  flex: 1;
}

.user-name {
  font-size: 1.125rem;
  font-weight: 600;
  color: white;
  margin-bottom: 0.5rem;
}

.leaderboard-entry.is-current-user .user-name {
  color: #06b6d4;
}

.user-details {
  font-size: 0.875rem;
  color: #94a3b8;
  display: flex;
  gap: 0.5rem;
  align-items: center;
}

.score {
  font-size: 1.5rem;
  font-weight: 700;
  color: white;
  min-width: 7rem;
  text-align: right;
  background: linear-gradient(135deg, #06b6d4 0%, #8b5cf6 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

.leaderboard-entry.is-current-user .score {
  background: linear-gradient(135deg, #fbbf24 0%, #f59e0b 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
}

@media (max-width: 768px) {
  .leaderboard-panel {
    padding: 1.5rem 1rem;
  }

  .leaderboard-title-wrapper {
    flex-direction: column;
    gap: 0.75rem;
  }

  .title-icon {
    width: 40px;
    height: 40px;
  }

  .title-icon svg {
    width: 20px;
    height: 20px;
  }

  .leaderboard-title {
    font-size: 1.5rem;
  }

  .leaderboard-entry {
    padding: 1rem;
    gap: 0.75rem;
  }

  .rank {
    min-width: 3rem;
    font-size: 1.25rem;
  }

  .rank-medal {
    width: 36px;
    height: 36px;
    font-size: 1.125rem;
  }

  .user-name {
    font-size: 1rem;
  }

  .user-details {
    font-size: 0.75rem;
    flex-wrap: wrap;
  }

  .score {
    font-size: 1.25rem;
    min-width: 5rem;
  }
}
</style>

