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
          <button
            @click="openCreateTournamentModal"
            class="px-6 py-3 rounded-xl bg-gradient-to-r from-emerald-500 to-teal-500 text-white text-base font-bold hover:from-emerald-400 hover:to-teal-400 transition-all duration-300 shadow-lg shadow-emerald-500/30 flex items-center justify-center gap-2"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M12 4v16m8-8H4" />
            </svg>
            Créer un tournoi
          </button>
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

      <!-- Erreur globale -->
      <div v-if="error" class="mb-6 p-4 rounded-xl bg-red-500/20 border border-red-500/30 text-red-400 text-center">
        <div class="flex items-center justify-center gap-2">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          <span class="font-semibold">{{ error }}</span>
        </div>
      </div>

      <!-- Liste des tournois -->
      <div v-else class="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <div
          v-for="tournament in filteredTournaments"
          :key="tournament.id"
          class="rounded-2xl bg-zinc-900 border border-zinc-800 overflow-hidden shadow-xl hover:shadow-2xl transition-all duration-300"
        >
          <!-- Image du tournoi -->
          <div v-if="tournament.imageUrl" class="relative h-48 overflow-hidden">
            <img 
              :src="tournament.imageUrl" 
              :alt="tournament.name"
              class="w-full h-full object-cover"
            />
            <div class="absolute inset-0 bg-gradient-to-t from-zinc-900/80 via-transparent to-transparent"></div>
          </div>
          <div v-else class="relative h-48 bg-gradient-to-br from-cyan-600/20 via-purple-600/20 to-pink-600/20 overflow-hidden flex items-center justify-center">
            <div class="text-6xl opacity-30">🏆</div>
            <div class="absolute inset-0 bg-gradient-to-t from-zinc-900/80 via-transparent to-transparent"></div>
          </div>
          
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
                :disabled="isRegistering || (tournament.entryFee > 0 && userStore.coins < tournament.entryFee)"
                class="flex-1 px-4 py-3 rounded-xl bg-gradient-to-r from-cyan-500 to-purple-500 text-white font-semibold hover:from-cyan-400 hover:to-purple-400 transition-all duration-300 disabled:opacity-50 disabled:cursor-not-allowed relative"
                :title="tournament.entryFee > 0 && userStore.coins < tournament.entryFee ? `Il vous faut ${tournament.entryFee} Babayuni, vous avez ${userStore.coins}` : ''"
              >
                <span v-if="isRegistering">Inscription...</span>
                <span v-else>S'inscrire</span>
              </button>
              <div 
                v-if="tournament.status === 1 && !tournament.isUserRegistered && tournament.entryFee > 0 && userStore.coins < tournament.entryFee"
                class="mt-2 text-xs text-red-400 text-center"
              >
                💰 Il vous faut {{ tournament.entryFee }} Babayuni, vous avez {{ userStore.coins }}
              </div>
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

    <!-- Modal de création de tournoi -->
    <div
      v-if="showCreateModal"
      class="fixed inset-0 z-50 flex items-center justify-center p-4 bg-black/50 backdrop-blur-sm"
      @click.self="closeCreateTournamentModal"
    >
      <div class="bg-zinc-900 rounded-2xl border border-zinc-800 shadow-2xl max-w-md w-full p-6">
        <div class="flex items-center justify-between mb-6">
          <h3 class="text-xl font-bold text-white">Créer un tournoi</h3>
          <button
            @click="closeCreateTournamentModal"
            class="p-2 rounded-lg hover:bg-zinc-800 text-zinc-400 hover:text-white transition-colors"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>

        <form @submit.prevent="createTournament" class="space-y-4">
          <div>
            <label class="block text-sm font-semibold text-zinc-300 mb-2">Nom du tournoi</label>
            <input
              v-model="createForm.name"
              type="text"
              required
              placeholder="Ex: Championnat de Pierre-Papier-Ciseaux"
              class="w-full px-4 py-2.5 rounded-xl bg-zinc-800 border border-zinc-700 text-white placeholder-zinc-500 focus:outline-none focus:ring-2 focus:ring-cyan-500/50"
            />
          </div>

          <div>
            <label class="block text-sm font-semibold text-zinc-300 mb-2">Description</label>
            <textarea
              v-model="createForm.description"
              required
              rows="3"
              placeholder="Décrivez votre tournoi..."
              class="w-full px-4 py-2.5 rounded-xl bg-zinc-800 border border-zinc-700 text-white placeholder-zinc-500 focus:outline-none focus:ring-2 focus:ring-cyan-500/50 resize-none"
            ></textarea>
          </div>

          <div class="grid grid-cols-2 gap-4">
            <div>
              <label class="block text-sm font-semibold text-zinc-300 mb-2">Participants max</label>
              <input
                v-model.number="createForm.maxParticipants"
                type="number"
                min="2"
                max="32"
                required
                class="w-full px-4 py-2.5 rounded-xl bg-zinc-800 border border-zinc-700 text-white focus:outline-none focus:ring-2 focus:ring-cyan-500/50"
              />
            </div>

            <div>
              <label class="block text-sm font-semibold text-zinc-300 mb-2">Mise d'entrée</label>
              <input
                v-model.number="createForm.entryFee"
                type="number"
                min="0"
                required
                class="w-full px-4 py-2.5 rounded-xl bg-zinc-800 border border-zinc-700 text-white focus:outline-none focus:ring-2 focus:ring-cyan-500/50"
              />
            </div>
          </div>

          <div>
            <label class="block text-sm font-semibold text-zinc-300 mb-2">Date de début</label>
            <input
              v-model="createForm.startDate"
              type="datetime-local"
              required
              class="w-full px-4 py-2.5 rounded-xl bg-zinc-800 border border-zinc-700 text-white focus:outline-none focus:ring-2 focus:ring-cyan-500/50"
            />
          </div>

          <div>
            <label class="block text-sm font-semibold text-zinc-300 mb-2">URL de l'image (optionnel)</label>
            <input
              v-model="createForm.imageUrl"
              type="url"
              placeholder="https://exemple.com/image.jpg"
              class="w-full px-4 py-2.5 rounded-xl bg-zinc-800 border border-zinc-700 text-white placeholder-zinc-500 focus:outline-none focus:ring-2 focus:ring-cyan-500/50"
            />
            <p class="text-xs text-zinc-500 mt-1">L'image sera affichée sur la carte du tournoi</p>
          </div>

          <!-- Aperçu de l'image -->
          <div v-if="createForm.imageUrl" class="mt-2">
            <p class="text-xs text-zinc-400 mb-2">Aperçu :</p>
            <div class="relative h-32 rounded-xl overflow-hidden border border-zinc-700">
              <img 
                :src="createForm.imageUrl" 
                alt="Aperçu"
                class="w-full h-full object-cover"
                @error="handleImageError"
              />
            </div>
          </div>

          <div v-if="error" class="text-red-400 text-sm">{{ error }}</div>

          <div class="flex gap-3 pt-4">
            <button
              type="button"
              @click="closeCreateTournamentModal"
              class="flex-1 px-4 py-3 rounded-xl bg-zinc-800 text-zinc-300 font-semibold hover:bg-zinc-700 transition-all duration-300"
            >
              Annuler
            </button>
            <button
              type="submit"
              :disabled="isCreating"
              class="flex-1 px-4 py-3 rounded-xl bg-gradient-to-r from-emerald-500 to-teal-500 text-white font-semibold hover:from-emerald-400 hover:to-teal-400 transition-all duration-300 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {{ isCreating ? 'Création...' : 'Créer' }}
            </button>
          </div>
        </form>
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
const showCreateModal = ref(false)
const isCreating = ref(false)
const createForm = ref({
  name: '',
  description: '',
  maxParticipants: 8,
  entryFee: 0,
  startDate: new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString().slice(0, 16), // Demain par défaut
  imageUrl: ''
})

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
    setTimeout(() => { error.value = null }, 5000)
    return
  }

  // Vérifier si l'utilisateur a assez de coins
  const tournament = tournaments.value.find(t => t.id === tournamentId)
  if (tournament && tournament.entryFee > 0 && userStore.coins < tournament.entryFee) {
    error.value = `Vous n'avez pas assez de Babayuni. Il vous faut ${tournament.entryFee} Babayuni, vous avez ${userStore.coins}.`
    setTimeout(() => { error.value = null }, 5000)
    return
  }

  isRegistering.value = true
  error.value = null // Réinitialiser l'erreur avant la tentative
  
  try {
    await tournamentsApi.registerForTournament(tournamentId, userStore.userId)
    await loadTournaments()
    await userStore.loadCoins() // Recharger les Babayuni
    // Succès - pas besoin d'afficher d'erreur
  } catch (err) {
    console.error('Erreur lors de l\'inscription:', err)
    const errorMessage = err instanceof Error ? err.message : 'Erreur lors de l\'inscription'
    error.value = errorMessage
    // Afficher l'erreur pendant 5 secondes
    setTimeout(() => { error.value = null }, 5000)
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

function openCreateTournamentModal() {
  showCreateModal.value = true
  createForm.value = {
    name: '',
    description: '',
    maxParticipants: 8,
    entryFee: 0,
    startDate: new Date(Date.now() + 24 * 60 * 60 * 1000).toISOString().slice(0, 16),
    imageUrl: ''
  }
}

function handleImageError(event: Event) {
  const img = event.target as HTMLImageElement
  img.style.display = 'none'
}

function closeCreateTournamentModal() {
  showCreateModal.value = false
}

async function createTournament() {
  if (!createForm.value.name.trim() || !createForm.value.description.trim()) {
    error.value = 'Veuillez remplir tous les champs'
    return
  }

  if (createForm.value.maxParticipants < 2 || createForm.value.maxParticipants > 32) {
    error.value = 'Le nombre de participants doit être entre 2 et 32'
    return
  }

  if (createForm.value.entryFee < 0) {
    error.value = 'La mise d\'entrée ne peut pas être négative'
    return
  }

  isCreating.value = true
  error.value = null

  try {
    await tournamentsApi.createTournament({
      name: createForm.value.name.trim(),
      description: createForm.value.description.trim(),
      gameType: tournamentsApi.TournamentGameType.RockPaperScissors,
      maxParticipants: createForm.value.maxParticipants,
      entryFee: createForm.value.entryFee,
      startDate: new Date(createForm.value.startDate).toISOString(),
      imageUrl: createForm.value.imageUrl?.trim() || null
    })
    
    await loadTournaments()
    closeCreateTournamentModal()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Erreur lors de la création du tournoi'
  } finally {
    isCreating.value = false
  }
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

