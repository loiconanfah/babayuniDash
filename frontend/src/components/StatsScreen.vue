<script setup lang="ts">
/**
 * Écran des statistiques et du classement
 */

import { onMounted } from 'vue'
import { useStatsStore } from '@/stores/stats'
import { useUserStore } from '@/stores/user'
import UserStatsPanel from '@/components/UserStatsPanel.vue'
import LeaderboardPanel from '@/components/LeaderboardPanel.vue'
import IconStats from '@/components/icons/IconStats.vue'

const statsStore = useStatsStore()
const userStore = useUserStore()

/**
 * Charge les statistiques de l'utilisateur connecté
 */
onMounted(async () => {
  // Charger le classement
  try {
    await statsStore.loadLeaderboard(10)
  } catch (err) {
    console.error('Erreur lors du chargement du classement:', err)
  }

  // Charger les statistiques de l'utilisateur si connecté
  if (userStore.user?.email) {
    try {
      await statsStore.loadUserStatsByEmail(userStore.user.email)
    } catch (err) {
      console.log('Aucune statistique disponible pour cet utilisateur')
    }
  }
})
</script>

<template>
  <div class="stats-screen">
    <div class="stats-container">
      <div class="page-header">
        <div class="header-icon">
          <IconStats />
        </div>
        <h1 class="page-title">Statistiques & Classement</h1>
        <p class="page-subtitle">Consultez vos performances et comparez-vous aux autres joueurs</p>
      </div>

      <div class="stats-grid">
        <!-- Statistiques utilisateur -->
        <div class="stats-section">
          <UserStatsPanel />
        </div>

        <!-- Classement -->
        <div class="stats-section">
          <LeaderboardPanel />
        </div>
      </div>
    </div>
  </div>
</template>

<style scoped>
.stats-screen {
  min-height: 100%;
  width: 100%;
  padding: 1rem;
  background: linear-gradient(135deg, #0f172a 0%, #1e293b 100%);
}

.stats-container {
  max-width: 1400px;
  margin: 0 auto;
}

.page-header {
  text-align: center;
  margin-bottom: 2.5rem;
  padding: 2rem 1rem;
}

.header-icon {
  display: inline-flex;
  align-items: center;
  justify-content: center;
  width: 64px;
  height: 64px;
  margin: 0 auto 1rem;
  background: linear-gradient(135deg, #06b6d4 0%, #8b5cf6 50%, #ec4899 100%);
  border-radius: 16px;
  color: white;
  box-shadow: 0 8px 24px rgba(6, 182, 212, 0.3);
}

.header-icon svg {
  width: 32px;
  height: 32px;
}

.page-title {
  font-size: 2.5rem;
  font-weight: 700;
  color: white;
  margin-bottom: 0.5rem;
  text-shadow: 0 2px 8px rgba(0, 0, 0, 0.3);
}

.page-subtitle {
  font-size: 1rem;
  color: #94a3b8;
  margin: 0;
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 2rem;
}

.stats-section {
  min-width: 0;
}

@media (max-width: 768px) {
  .stats-screen {
    padding: 0.5rem;
  }

  .page-header {
    padding: 1.5rem 0.5rem;
    margin-bottom: 1.5rem;
  }

  .header-icon {
    width: 56px;
    height: 56px;
  }

  .header-icon svg {
    width: 28px;
    height: 28px;
  }

  .page-title {
    font-size: 1.75rem;
  }

  .page-subtitle {
    font-size: 0.875rem;
  }

  .stats-grid {
    grid-template-columns: 1fr;
    gap: 1.5rem;
  }
}
</style>

