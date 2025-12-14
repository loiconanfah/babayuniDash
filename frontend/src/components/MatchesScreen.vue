<template>
  <section class="w-full h-full px-4 sm:px-6 lg:px-12 py-8 overflow-y-auto">
    <div class="max-w-7xl mx-auto">
      <!-- En-tête -->
      <header class="mb-8">
        <div class="flex items-center gap-3 mb-3">
          <div class="h-1 w-12 bg-gradient-to-r from-blue-500 to-purple-500 rounded-full"></div>
          <p class="text-sm uppercase tracking-[0.3em] text-blue-400 font-semibold">
            Matchs en Direct
          </p>
        </div>
        <h2 class="text-4xl sm:text-5xl font-extrabold text-slate-50 mb-3 bg-gradient-to-r from-slate-50 via-slate-100 to-slate-200 bg-clip-text text-transparent">
          ⚔️ Matchs VS en Cours
        </h2>
        <p class="text-base text-slate-300 max-w-2xl">
          Observez les parties multijoueurs en cours avec paris. Rejoignez ou créez votre propre match !
        </p>
        <div class="mt-4">
          <button
            @click="uiStore.goToTournaments()"
            class="px-6 py-3 rounded-xl bg-gradient-to-r from-yellow-500 to-orange-500 text-white font-semibold hover:from-yellow-400 hover:to-orange-400 transition-all duration-300 shadow-lg hover:shadow-xl"
          >
            🏆 Voir les Tournois
          </button>
        </div>
      </header>

      <!-- Filtres par type de jeu -->
      <div class="mb-8 flex flex-wrap gap-3">
        <button
          v-for="filter in gameFilters"
          :key="filter.type"
          @click="selectedFilter = filter.type"
          class="px-5 py-2.5 rounded-xl text-sm font-semibold transition-all duration-300 hover:scale-105"
          :class="selectedFilter === filter.type
            ? 'bg-gradient-to-r from-blue-500 to-purple-500 text-white shadow-lg shadow-blue-500/30'
            : 'bg-slate-800/60 text-slate-300 hover:bg-slate-700/60 border border-slate-700/50'"
        >
          {{ filter.icon }} {{ filter.label }}
        </button>
      </div>

      <!-- Liste des matchs -->
      <div v-if="isLoading" class="flex items-center justify-center py-20">
        <div class="text-slate-400 animate-pulse">Chargement des matchs...</div>
      </div>

      <div v-else-if="error" class="text-red-400 text-center py-20">
        {{ error }}
      </div>

      <div v-else-if="filteredMatches.length === 0" class="text-center py-20">
        <div class="text-6xl mb-4">🎮</div>
        <p class="text-xl text-slate-400 mb-2">Aucun match en cours</p>
        <p class="text-sm text-slate-500">Créez une partie pour commencer !</p>
      </div>

      <div v-else class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
        <div
          v-for="match in filteredMatches"
          :key="`${match.gameType}-${match.gameId}`"
          class="group relative rounded-2xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border overflow-hidden shadow-xl transition-all duration-300 hover:scale-[1.02]"
          :class="match.status === 'En attente'
            ? 'border-orange-500/50 shadow-orange-500/20'
            : 'border-blue-500/50 shadow-blue-500/20'"
        >
          <!-- Header avec type de jeu -->
          <div class="px-6 py-4 bg-gradient-to-r from-slate-800/80 to-slate-900/80 border-b border-slate-700/50 flex items-center justify-between">
            <div class="flex items-center gap-3">
              <div class="text-3xl">{{ match.gameTypeIcon }}</div>
              <div>
                <h3 class="text-lg font-bold text-slate-50">{{ match.gameType }}</h3>
                <p class="text-xs text-slate-400">{{ match.status }}</p>
              </div>
            </div>
            <div v-if="match.totalWager > 0" class="flex items-center gap-1 px-3 py-1.5 rounded-lg bg-yellow-500/20 border border-yellow-500/30">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-yellow-400" fill="currentColor" viewBox="0 0 20 20">
                <path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 01-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 01-.567.267z" />
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-13a1 1 0 10-2 0v.092a4.535 4.535 0 00-1.676.662C6.602 6.234 6 7.009 6 8c0 .99.602 1.765 1.324 2.246.48.32 1.054.545 1.676.662v1.941c-.391-.127-.68-.317-.843-.504a1 1 0 10-1.51 1.31c.562.649 1.413 1.076 2.353 1.253V15a1 1 0 102 0v-.092a4.535 4.535 0 001.676-.662C13.398 13.766 14 12.991 14 12c0-.99-.602-1.765-1.324-2.246A4.535 4.535 0 0011 9.092V7.151c.391.127.68.317.843.504a1 1 0 101.511-1.31c-.563-.649-1.413-1.076-2.354-1.253V5z" clip-rule="evenodd" />
              </svg>
              <span class="text-sm font-bold text-yellow-400">{{ match.totalWager }}</span>
            </div>
          </div>

          <!-- Contenu VS -->
          <div class="p-6">
            <!-- Joueur 1 -->
            <div class="flex items-center justify-between mb-4">
              <div class="flex items-center gap-3 flex-1">
                <div class="h-12 w-12 rounded-full bg-gradient-to-br from-blue-500 to-blue-600 flex items-center justify-center text-2xl shadow-lg">
                  {{ match.player1Avatar }}
                </div>
                <div class="flex-1 min-w-0">
                  <p class="text-sm font-bold text-slate-50 truncate">{{ match.player1Name }}</p>
                  <div v-if="match.player1Wager > 0" class="flex items-center gap-1 mt-1">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-3 w-3 text-yellow-400" fill="currentColor" viewBox="0 0 20 20">
                      <path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 01-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 01-.567.267z" />
                      <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-13a1 1 0 10-2 0v.092a4.535 4.535 0 00-1.676.662C6.602 6.234 6 7.009 6 8c0 .99.602 1.765 1.324 2.246.48.32 1.054.545 1.676.662v1.941c-.391-.127-.68-.317-.843-.504a1 1 0 10-1.51 1.31c.562.649 1.413 1.076 2.353 1.253V15a1 1 0 102 0v-.092a4.535 4.535 0 001.676-.662C13.398 13.766 14 12.991 14 12c0-.99-.602-1.765-1.324-2.246A4.535 4.535 0 0011 9.092V7.151c.391.127.68.317.843.504a1 1 0 101.511-1.31c-.563-.649-1.413-1.076-2.354-1.253V5z" clip-rule="evenodd" />
                    </svg>
                    <span class="text-xs text-yellow-400 font-medium">{{ match.player1Wager }}</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- VS -->
            <div class="flex items-center justify-center my-4">
              <div class="px-4 py-2 rounded-full bg-gradient-to-r from-blue-500/20 to-purple-500/20 border border-blue-500/30">
                <span class="text-lg font-bold text-slate-50">VS</span>
              </div>
            </div>

            <!-- Joueur 2 -->
            <div class="flex items-center justify-between mb-4">
              <div class="flex items-center gap-3 flex-1">
                <div class="h-12 w-12 rounded-full bg-gradient-to-br from-orange-500 to-orange-600 flex items-center justify-center text-2xl shadow-lg">
                  {{ match.player2Avatar }}
                </div>
                <div class="flex-1 min-w-0">
                  <p v-if="match.player2Name" class="text-sm font-bold text-slate-50 truncate">{{ match.player2Name }}</p>
                  <p v-else class="text-sm font-bold text-slate-400 italic">En attente...</p>
                  <div v-if="match.player2Wager > 0" class="flex items-center gap-1 mt-1">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-3 w-3 text-yellow-400" fill="currentColor" viewBox="0 0 20 20">
                      <path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 01-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 01-.567.267z" />
                      <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-13a1 1 0 10-2 0v.092a4.535 4.535 0 00-1.676.662C6.602 6.234 6 7.009 6 8c0 .99.602 1.765 1.324 2.246.48.32 1.054.545 1.676.662v1.941c-.391-.127-.68-.317-.843-.504a1 1 0 10-1.51 1.31c.562.649 1.413 1.076 2.353 1.253V15a1 1 0 102 0v-.092a4.535 4.535 0 001.676-.662C13.398 13.766 14 12.991 14 12c0-.99-.602-1.765-1.324-2.246A4.535 4.535 0 0011 9.092V7.151c.391.127.68.317.843.504a1 1 0 101.511-1.31c-.563-.649-1.413-1.076-2.354-1.253V5z" clip-rule="evenodd" />
                    </svg>
                    <span class="text-xs text-yellow-400 font-medium">{{ match.player2Wager }}</span>
                  </div>
                </div>
              </div>
            </div>

            <!-- Bouton d'action -->
            <button
              v-if="match.status === 'En attente' && userStore.isLoggedIn"
              @click="handleJoinMatch(match)"
              class="w-full px-4 py-3 rounded-xl bg-gradient-to-r from-orange-500 to-orange-600 text-white text-sm font-bold tracking-wide hover:from-orange-400 hover:to-orange-500 transition-all duration-300 shadow-lg shadow-orange-500/30 hover:shadow-xl hover:shadow-orange-500/40 hover:scale-105 active:scale-95"
            >
              Rejoindre
            </button>
            <button
              v-else-if="match.status !== 'En attente'"
              @click="handleViewMatch(match)"
              class="w-full px-4 py-3 rounded-xl bg-gradient-to-r from-blue-500 to-purple-500 text-white text-sm font-bold tracking-wide hover:from-blue-400 hover:to-purple-400 transition-all duration-300 shadow-lg shadow-blue-500/30 hover:shadow-xl hover:shadow-blue-500/40 hover:scale-105 active:scale-95"
            >
              Voir le match
            </button>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
// Utiliser VITE_API_URL si disponible (pour Render), sinon utiliser /api (pour le proxy Vite en développement)
const API_BASE_URL = import.meta.env.VITE_API_URL || '/api';
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useUserStore } from '@/stores/user'
import { useUiStore } from '@/stores/ui'

const userStore = useUserStore()
const uiStore = useUiStore()

interface ActiveMatch {
  gameId: number
  gameType: string
  gameTypeIcon: string
  player1Name: string
  player1Avatar: string
  player1Wager: number
  player2Name: string | null
  player2Avatar: string
  player2Wager: number
  totalWager: number
  status: string
  createdAt: string
}

const matches = ref<ActiveMatch[]>([])
const isLoading = ref(false)
const error = ref<string | null>(null)
const selectedFilter = ref<string>('Tous')

const gameFilters = [
  { type: 'Tous', label: 'Tous', icon: '🎮' },
  { type: 'TicTacToe', label: 'Tic-Tac-Toe', icon: '⭕' },
  { type: 'ConnectFour', label: 'Connect Four', icon: '🔴' },
  { type: 'RockPaperScissors', label: 'RPS', icon: '✂️' }
]

const filteredMatches = computed(() => {
  if (selectedFilter.value === 'Tous') {
    return matches.value
  }
  return matches.value.filter(m => m.gameType === selectedFilter.value)
})

async function loadMatches() {
  isLoading.value = true
  error.value = null
  try {
    const response = await fetch(`${API_BASE_URL}/Matches/active`)
    if (!response.ok) throw new Error('Erreur lors du chargement')
    matches.value = await response.json()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Erreur lors du chargement des matchs'
  } finally {
    isLoading.value = false
  }
}

function handleJoinMatch(match: ActiveMatch) {
  // Navigation vers le jeu approprié
  switch (match.gameType) {
    case 'TicTacToe':
      uiStore.goToTicTacToe()
      break
    case 'ConnectFour':
      uiStore.goToConnectFour()
      break
    case 'RockPaperScissors':
      uiStore.goToRockPaperScissors()
      break
  }
}

function handleViewMatch(match: ActiveMatch) {
  handleJoinMatch(match)
}

let refreshInterval: number | null = null

onMounted(() => {
  loadMatches()
  // Rafraîchir toutes les 5 secondes
  refreshInterval = window.setInterval(loadMatches, 5000)
})

onUnmounted(() => {
  if (refreshInterval) {
    clearInterval(refreshInterval)
  }
})
</script>

