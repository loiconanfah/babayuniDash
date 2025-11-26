<script setup lang="ts">
/**
 * Écran des statistiques et du classement
 */

import { onMounted } from 'vue'
import { useStatsStore } from '@/stores/stats'
import { useUserStore } from '@/stores/user'
import UserStatsPanel from '@/components/UserStatsPanel.vue'
import LeaderboardPanel from '@/components/LeaderboardPanel.vue'

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
      <h1 class="page-title">📊 Statistiques & Classement</h1>

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
  padding: 2rem;
  background: #0f172a;
}

.stats-container {
  max-width: 1200px;
  margin: 0 auto;
}

.page-title {
  font-size: 2.5rem;
  font-weight: bold;
  color: white;
  text-align: center;
  margin-bottom: 2rem;
  text-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(400px, 1fr));
  gap: 2rem;
}

.stats-section {
  min-width: 0; /* Permet au contenu de se rétrécir */
}

@media (max-width: 768px) {
  .stats-grid {
    grid-template-columns: 1fr;
  }

  .page-title {
    font-size: 2rem;
  }
}
</style>

