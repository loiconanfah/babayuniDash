<template>
  <section class="w-full h-full px-4 sm:px-6 lg:px-12 py-8 overflow-y-auto">
    <div class="max-w-7xl mx-auto">
      <!-- En-tête -->
      <header class="mb-8">
        <div class="flex items-center gap-3 mb-3">
          <div class="h-1 w-12 bg-gradient-to-r from-cyan-400 via-purple-500 to-pink-500 rounded-full"></div>
          <p class="text-sm uppercase tracking-[0.3em] text-cyan-300 font-semibold">
            Tournois
          </p>
        </div>
        <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
          <div class="flex-1">
            <div class="flex items-center gap-3 mb-3">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8 sm:h-10 sm:w-10 text-cyan-400" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z" />
              </svg>
              <h2 class="text-3xl sm:text-4xl lg:text-5xl font-extrabold text-zinc-50 bg-gradient-to-r from-cyan-400 via-purple-400 to-pink-400 bg-clip-text text-transparent">
                Tournois
              </h2>
            </div>
            <p class="text-sm sm:text-base text-zinc-300 max-w-2xl">
              Participe à des tournois de Pierre-Papier-Ciseaux et gagne des récompenses en Babayuni !
            </p>
          </div>
        </div>
      </header>

      <!-- Filtres -->
      <div class="mb-6 flex flex-wrap gap-3">
        <button
          @click="selectedFilter = 'all'"
          :class="selectedFilter === 'all'
            ? 'bg-gradient-to-r from-cyan-500 to-purple-500 text-white'
            : 'bg-zinc-800/60 text-zinc-300 hover:bg-zinc-700/60'"
          class="px-5 py-2.5 rounded-xl text-sm font-semibold transition-all duration-300"
        >
          Tous
        </button>
        <button
          @click="selectedFilter = 'registration'"
          :class="selectedFilter === 'registration'
            ? 'bg-gradient-to-r from-cyan-500 to-purple-500 text-white'
            : 'bg-zinc-800/60 text-zinc-300 hover:bg-zinc-700/60'"
          class="px-5 py-2.5 rounded-xl text-sm font-semibold transition-all duration-300"
        >
          Inscriptions ouvertes
        </button>
        <button
          @click="selectedFilter = 'in_progress'"
          :class="selectedFilter === 'in_progress'
            ? 'bg-gradient-to-r from-cyan-500 to-purple-500 text-white'
            : 'bg-zinc-800/60 text-zinc-300 hover:bg-zinc-700/60'"
          class="px-5 py-2.5 rounded-xl text-sm font-semibold transition-all duration-300"
        >
          En cours
        </button>
        <button
          @click="selectedFilter = 'completed'"
          :class="selectedFilter === 'completed'
            ? 'bg-gradient-to-r from-cyan-500 to-purple-500 text-white'
            : 'bg-zinc-800/60 text-zinc-300 hover:bg-zinc-700/60'"
          class="px-5 py-2.5 rounded-xl text-sm font-semibold transition-all duration-300"
        >
          Terminés
        </button>
      </div>

      <!-- Chargement -->
      <div v-if="isLoading" class="flex items-center justify-center py-20">
        <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-cyan-500"></div>
      </div>

      <!-- Erreur -->
      <div v-else-if="error" class="text-red-400 text-center py-20">
        {{ error }}
      </div>

      <!-- Liste des tournois -->
      <div v-else class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div
          v-for="tournament in filteredTournaments"
          :key="tournament.id"
          class="rounded-2xl bg-zinc-900 border border-zinc-800 overflow-hidden shadow-xl hover:shadow-2xl transition-all duration-300"
        >
          <!-- En-tête du tournoi -->
          <div class="p-6 bg-gradient-to-br from-zinc-800/50 to-zinc-900 border-b border-zinc-800">
            <div class="flex items-start justify-between mb-4">
              <div class="flex-1">
                <h3 class="text-xl font-bold text-white mb-2">{{ tournament.name }}</h3>
                <p class="text-sm text-zinc-400">{{ tournament.description }}</p>
              </div>
              <span
                :class="[
                  'px-3 py-1 rounded-full text-xs font-bold',
                  tournament.status === 1 ? 'bg-green-500/20 text-green-400' :
                  tournament.status === 2 ? 'bg-blue-500/20 text-blue-400' :
                  tournament.status === 3 ? 'bg-purple-500/20 text-purple-400' :
                  'bg-red-500/20 text-red-400'
                ]"
              >
                {{ getStatusText(tournament.status) }}
              </span>
            </div>

            <!-- Informations du tournoi -->
            <div class="grid grid-cols-2 gap-4 text-sm">
              <div>
                <p class="text-zinc-500 mb-1">Participants</p>
                <p class="text-white font-semibold">{{ tournament.currentParticipants }} / {{ tournament.maxParticipants }}</p>
              </div>
              <div>
                <p class="text-zinc-500 mb-1">Mise d'entrée</p>
                <p class="text-white font-semibold">
                  {{ tournament.entryFee > 0 ? `${tournament.entryFee} Babayuni` : 'Gratuit' }}
                </p>
              </div>
              <div>
                <p class="text-zinc-500 mb-1">Récompense totale</p>
                <p class="text-yellow-400 font-bold">{{ tournament.totalPrize }} Babayuni</p>
              </div>
              <div>
                <p class="text-zinc-500 mb-1">Début</p>
                <p class="text-white font-semibold">{{ formatDate(tournament.startDate) }}</p>
              </div>
            </div>
          </div>

          <!-- Récompenses -->
          <div class="p-6 bg-zinc-900">
            <h4 class="text-sm font-semibold text-zinc-400 mb-3">Récompenses</h4>
            <div class="space-y-2">
              <div class="flex items-center justify-between">
                <span class="text-sm text-zinc-300">🥇 1er place</span>
                <span class="text-yellow-400 font-bold">{{ tournament.firstPlacePrize }} Babayuni</span>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-sm text-zinc-300">🥈 2ème place</span>
                <span class="text-zinc-400 font-bold">{{ tournament.secondPlacePrize }} Babayuni</span>
              </div>
              <div class="flex items-center justify-between">
                <span class="text-sm text-zinc-300">🥉 3ème place</span>
                <span class="text-zinc-400 font-bold">{{ tournament.thirdPlacePrize }} Babayuni</span>
              </div>
            </div>

            <!-- Actions -->
            <div class="mt-6 flex gap-3">
              <button
                v-if="tournament.status === 1 && !tournament.isUserRegistered"
                @click="handleRegister(tournament.id)"
                :disabled="isRegistering || userStore.coins < tournament.entryFee"
                class="flex-1 px-4 py-3 rounded-xl bg-gradient-to-r from-cyan-500 to-purple-500 text-white font-semibold hover:from-cyan-400 hover:to-purple-400 transition-all duration-300 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                S'inscrire
              </button>
              <button
                v-else-if="tournament.status === 1 && tournament.isUserRegistered"
                @click="handleUnregister(tournament.id)"
                :disabled="isUnregistering"
                class="flex-1 px-4 py-3 rounded-xl bg-zinc-800 text-zinc-300 font-semibold hover:bg-zinc-700 transition-all duration-300 disabled:opacity-50"
              >
                Se désinscrire
              </button>
              <button
                v-else-if="tournament.status === 2 && tournament.isUserRegistered"
                @click="viewTournament(tournament.id)"
                class="flex-1 px-4 py-3 rounded-xl bg-gradient-to-r from-blue-500 to-cyan-500 text-white font-semibold hover:from-blue-400 hover:to-cyan-400 transition-all duration-300"
              >
                Voir le tournoi
              </button>
              <button
                v-else-if="tournament.status === 3"
                @click="viewTournament(tournament.id)"
                class="flex-1 px-4 py-3 rounded-xl bg-zinc-800 text-zinc-300 font-semibold hover:bg-zinc-700 transition-all duration-300"
              >
                Voir les résultats
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Message si aucun tournoi -->
      <div v-if="!isLoading && !error && filteredTournaments.length === 0" class="text-center py-20">
        <p class="text-zinc-400">Aucun tournoi disponible</p>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useUserStore } from '@/stores/user'
import * as tournamentsApi from '@/services/tournamentsApi'
import type { TournamentDto } from '@/services/tournamentsApi'

const userStore = useUserStore()

const tournaments = ref<TournamentDto[]>([])
const isLoading = ref(false)
const error = ref<string | null>(null)
const selectedFilter = ref<'all' | 'registration' | 'in_progress' | 'completed'>('all')
const isRegistering = ref(false)
const isUnregistering = ref(false)

const filteredTournaments = computed(() => {
  let filtered = tournaments.value.filter(t => t.gameType === tournamentsApi.TournamentGameType.RockPaperScissors)
  
  if (selectedFilter.value === 'registration') {
    filtered = filtered.filter(t => t.status === tournamentsApi.TournamentStatus.Registration)
  } else if (selectedFilter.value === 'in_progress') {
    filtered = filtered.filter(t => t.status === tournamentsApi.TournamentStatus.InProgress)
  } else if (selectedFilter.value === 'completed') {
    filtered = filtered.filter(t => t.status === tournamentsApi.TournamentStatus.Completed)
  }
  
  return filtered
})

async function loadTournaments() {
  isLoading.value = true
  error.value = null
  try {
    tournaments.value = await tournamentsApi.getAllTournaments(userStore.userId || undefined)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Erreur lors du chargement des tournois'
  } finally {
    isLoading.value = false
  }
}

async function handleRegister(tournamentId: number) {
  if (!userStore.userId) {
    error.value = 'Vous devez être connecté pour vous inscrire'
    return
  }

  isRegistering.value = true
  try {
    await tournamentsApi.registerForTournament(tournamentId, userStore.userId)
    await loadTournaments()
    await userStore.loadCoins() // Recharger les Babayuni
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Erreur lors de l\'inscription'
  } finally {
    isRegistering.value = false
  }
}

async function handleUnregister(tournamentId: number) {
  if (!userStore.userId) {
    error.value = 'Vous devez être connecté'
    return
  }

  isUnregistering.value = true
  try {
    await tournamentsApi.unregisterFromTournament(tournamentId, userStore.userId)
    await loadTournaments()
    await userStore.loadCoins() // Recharger les Babayuni
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Erreur lors de la désinscription'
  } finally {
    isUnregistering.value = false
  }
}

function viewTournament(tournamentId: number) {
  // TODO: Naviguer vers la page de détail du tournoi
  console.log('Voir le tournoi', tournamentId)
}

function getStatusText(status: number): string {
  switch (status) {
    case tournamentsApi.TournamentStatus.Registration:
      return 'Inscriptions ouvertes'
    case tournamentsApi.TournamentStatus.InProgress:
      return 'En cours'
    case tournamentsApi.TournamentStatus.Completed:
      return 'Terminé'
    case tournamentsApi.TournamentStatus.Cancelled:
      return 'Annulé'
    default:
      return 'Inconnu'
  }
}

function formatDate(dateString: string): string {
  const date = new Date(dateString)
  return date.toLocaleDateString('fr-FR', {
    day: 'numeric',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit'
  })
}

onMounted(() => {
  loadTournaments()
})
</script>

