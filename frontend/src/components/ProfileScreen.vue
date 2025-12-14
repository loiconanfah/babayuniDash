<template>
  <section class="w-full h-full px-4 sm:px-6 lg:px-12 py-8 overflow-y-auto">
    <div class="max-w-7xl mx-auto">
      <!-- En-tête -->
      <header class="mb-8">
        <div class="flex items-center gap-3 mb-3">
          <div class="h-1 w-12 bg-gradient-to-r from-purple-500 to-pink-500 rounded-full"></div>
          <p class="text-sm uppercase tracking-[0.3em] text-purple-400 font-semibold">
            Profil
          </p>
        </div>
        <h2 class="text-4xl sm:text-5xl font-extrabold text-slate-50 mb-3 bg-gradient-to-r from-slate-50 via-slate-100 to-slate-200 bg-clip-text text-transparent">
          👤 Mon Profil
        </h2>
        <p class="text-base text-slate-300 max-w-2xl">
          Personnalisez votre profil avec vos items équipés. Montrez votre style unique !
        </p>
      </header>

      <div v-if="!userStore.isLoggedIn" class="text-center py-20">
        <div class="text-6xl mb-4">🔒</div>
        <p class="text-xl text-slate-400 mb-2">Connectez-vous pour voir votre profil</p>
        <button
          @click="uiStore.openUserModal()"
          class="mt-4 px-6 py-3 rounded-xl bg-gradient-to-r from-purple-500 to-pink-500 text-white font-bold hover:from-purple-400 hover:to-pink-400 transition-all"
        >
          Se connecter
        </button>
      </div>

      <div v-else class="grid grid-cols-1 lg:grid-cols-3 gap-8">
        <!-- Carte de profil principale -->
        <div class="lg:col-span-2">
          <div class="rounded-3xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 overflow-hidden shadow-2xl">
            <!-- Header avec gradient - applique l'effet du thème équipé -->
            <div 
              class="relative h-48 flex items-center justify-center overflow-hidden transition-all duration-500"
              :style="themeHeaderStyle"
            >
              <div class="absolute inset-0 opacity-10" :style="themePatternStyle"></div>
              
              <!-- Avatar équipé -->
              <div class="relative z-10">
                <div 
                  class="h-32 w-32 rounded-full flex items-center justify-center text-6xl shadow-2xl border-4 transition-all duration-500"
                  :style="avatarStyle"
                >
                  {{ equippedAvatar?.icon || '👤' }}
                </div>
                <div 
                  v-if="equippedAvatar" 
                  class="absolute -bottom-2 left-1/2 transform -translate-x-1/2 px-3 py-1 rounded-full backdrop-blur-sm text-xs text-slate-200 transition-all duration-500"
                  :style="avatarBadgeStyle"
                >
                  {{ equippedAvatar.name }}
                </div>
              </div>
            </div>

            <!-- Informations utilisateur -->
            <div class="p-8">
              <div class="flex items-center justify-between mb-6">
                <div>
                  <h3 class="text-3xl font-bold text-slate-50 mb-2">{{ userStore.user?.name || 'Joueur' }}</h3>
                  <p class="text-slate-400">{{ userStore.user?.email }}</p>
                </div>
                <div class="text-right">
                  <p class="text-sm text-slate-400 mb-1">Coins</p>
                  <div class="flex items-center gap-2">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-yellow-400" fill="currentColor" viewBox="0 0 20 20">
                      <path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 01-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 01-.567.267z" />
                      <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-13a1 1 0 10-2 0v.092a4.535 4.535 0 00-1.676.662C6.602 6.234 6 7.009 6 8c0 .99.602 1.765 1.324 2.246.48.32 1.054.545 1.676.662v1.941c-.391-.127-.68-.317-.843-.504a1 1 0 10-1.51 1.31c.562.649 1.413 1.076 2.353 1.253V15a1 1 0 102 0v-.092a4.535 4.535 0 001.676-.662C13.398 13.766 14 12.991 14 12c0-.99-.602-1.765-1.324-2.246A4.535 4.535 0 0011 9.092V7.151c.391.127.68.317.843.504a1 1 0 101.511-1.31c-.563-.649-1.413-1.076-2.354-1.253V5z" clip-rule="evenodd" />
                    </svg>
                    <span class="text-3xl font-bold text-yellow-400">{{ userStore.coins }}</span>
                  </div>
                </div>
              </div>

              <!-- Items équipés -->
              <div class="space-y-4">
                <div>
                  <h4 class="text-lg font-semibold text-slate-300 mb-3">Items équipés</h4>
                  <div class="grid grid-cols-2 gap-4">
                    <!-- Thème équipé -->
                    <div v-if="equippedTheme" class="p-4 rounded-xl bg-slate-800/50 border border-slate-700/50">
                      <p class="text-xs text-slate-400 mb-1">Thème</p>
                      <div class="flex items-center gap-2">
                        <span class="text-2xl">{{ equippedTheme.icon }}</span>
                        <span class="text-sm font-medium text-slate-200">{{ equippedTheme.name }}</span>
                      </div>
                    </div>
                    <div v-else class="p-4 rounded-xl bg-slate-800/30 border border-slate-700/30 border-dashed">
                      <p class="text-xs text-slate-500">Aucun thème équipé</p>
                    </div>

                    <!-- Décoration équipée -->
                    <div v-if="equippedDecoration" class="p-4 rounded-xl bg-slate-800/50 border border-slate-700/50">
                      <p class="text-xs text-slate-400 mb-1">Décoration</p>
                      <div class="flex items-center gap-2">
                        <span class="text-2xl">{{ equippedDecoration.icon }}</span>
                        <span class="text-sm font-medium text-slate-200">{{ equippedDecoration.name }}</span>
                      </div>
                    </div>
                    <div v-else class="p-4 rounded-xl bg-slate-800/30 border border-slate-700/30 border-dashed">
                      <p class="text-xs text-slate-500">Aucune décoration équipée</p>
                    </div>
                  </div>
                </div>

                <button
                  @click="uiStore.goToCollection()"
                  class="w-full px-6 py-3 rounded-xl bg-gradient-to-r from-purple-500 to-pink-500 text-white font-bold hover:from-purple-400 hover:to-pink-400 transition-all flex items-center justify-center gap-2"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                  </svg>
                  Gérer mes items
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Sidebar avec statistiques -->
        <div class="space-y-6">
          <!-- Statistiques -->
          <div class="rounded-2xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 p-6">
            <h4 class="text-lg font-semibold text-slate-300 mb-4">Statistiques</h4>
            <div class="space-y-4">
              <div class="flex justify-between items-center">
                <span class="text-slate-400">Items possédés</span>
                <span class="text-slate-200 font-bold">{{ userItems.length }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-slate-400">Items équipés</span>
                <span class="text-slate-200 font-bold">{{ equippedItemsCount }}</span>
              </div>
              <div class="flex justify-between items-center">
                <span class="text-slate-400">Total dépensé</span>
                <span class="text-slate-200 font-bold">{{ totalSpent }} coins</span>
              </div>
            </div>
          </div>

          <!-- Actions rapides -->
          <div class="rounded-2xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 p-6">
            <h4 class="text-lg font-semibold text-slate-300 mb-4">Actions rapides</h4>
            <div class="space-y-3">
              <button
                @click="uiStore.goToShop()"
                class="w-full px-4 py-3 rounded-xl bg-gradient-to-r from-yellow-500 to-orange-500 text-slate-900 font-bold hover:from-yellow-400 hover:to-orange-400 transition-all text-sm"
              >
                🛒 Visiter la boutique
              </button>
              <button
                @click="uiStore.goToCollection()"
                class="w-full px-4 py-3 rounded-xl bg-slate-800/50 text-slate-300 hover:bg-slate-700/50 transition-all text-sm font-medium"
              >
                📦 Ma collection
              </button>
              <button
                @click="uiStore.goToTournaments()"
                class="w-full px-4 py-3 rounded-xl bg-gradient-to-r from-cyan-500 to-purple-500 text-white font-bold hover:from-cyan-400 hover:to-purple-400 transition-all text-sm"
              >
                🏆 Voir les tournois
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- Section Tournois -->
      <div v-if="userStore.isLoggedIn" class="mt-8">
        <div class="flex items-center justify-between mb-6">
          <div>
            <h3 class="text-2xl font-bold text-slate-50 mb-2">🏆 Mes Tournois</h3>
            <p class="text-sm text-slate-400">Tournois auxquels vous êtes inscrit</p>
          </div>
        </div>

        <!-- Chargement -->
        <div v-if="isLoadingTournaments" class="flex items-center justify-center py-12">
          <div class="animate-spin rounded-full h-10 w-10 border-b-2 border-cyan-500"></div>
        </div>

        <!-- Liste des tournois -->
        <div v-else-if="userTournaments.length > 0" class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <div
            v-for="tournament in userTournaments"
            :key="tournament.id"
            class="rounded-2xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 overflow-hidden shadow-xl hover:shadow-2xl transition-all duration-300"
          >
            <!-- Image du tournoi -->
            <div v-if="tournament.imageUrl" class="relative h-40 overflow-hidden">
              <img 
                :src="tournament.imageUrl" 
                :alt="tournament.name"
                class="w-full h-full object-cover"
              />
              <div class="absolute inset-0 bg-gradient-to-t from-slate-900/90 via-transparent to-transparent"></div>
            </div>
            <div v-else class="relative h-40 bg-gradient-to-br from-cyan-600/20 via-purple-600/20 to-pink-600/20 overflow-hidden flex items-center justify-center">
              <div class="text-5xl opacity-30">🏆</div>
              <div class="absolute inset-0 bg-gradient-to-t from-slate-900/90 via-transparent to-transparent"></div>
            </div>

            <!-- Contenu -->
            <div class="p-6">
              <div class="flex items-start justify-between mb-4">
                <div class="flex-1">
                  <h4 class="text-xl font-bold text-slate-50 mb-2">{{ tournament.name }}</h4>
                  <p class="text-sm text-slate-400 line-clamp-2">{{ tournament.description }}</p>
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

              <!-- Classement de l'utilisateur -->
              <div v-if="tournament.userParticipant" class="mb-4 p-4 rounded-xl bg-gradient-to-br from-cyan-500/10 via-purple-500/10 to-pink-500/10 border border-cyan-500/20">
                <div class="flex items-center justify-between">
                  <div>
                    <p class="text-xs text-slate-400 mb-1">Votre classement</p>
                    <div class="flex items-center gap-2">
                      <span v-if="tournament.userPosition" class="text-2xl font-bold text-cyan-400">
                        {{ getPositionEmoji(tournament.userPosition) }} {{ tournament.userPosition }}<span class="text-sm">ème</span>
                      </span>
                      <span v-else class="text-lg font-semibold text-slate-300">
                        En attente
                      </span>
                    </div>
                  </div>
                  <div v-if="tournament.userParticipant.prizeWon > 0" class="text-right">
                    <p class="text-xs text-slate-400 mb-1">Récompense</p>
                    <p class="text-lg font-bold text-yellow-400">
                      {{ tournament.userParticipant.prizeWon }} 💰
                    </p>
                  </div>
                </div>
                <div v-if="tournament.userParticipant.isEliminated" class="mt-2 text-xs text-red-400">
                  ❌ Éliminé
                </div>
              </div>

              <!-- Informations du tournoi -->
              <div class="grid grid-cols-2 gap-3 text-sm mb-4">
                <div>
                  <p class="text-slate-500 mb-1">Participants</p>
                  <p class="text-slate-200 font-semibold">{{ tournament.currentParticipants }} / {{ tournament.maxParticipants }}</p>
                </div>
                <div>
                  <p class="text-slate-500 mb-1">Récompense totale</p>
                  <p class="text-yellow-400 font-bold">{{ tournament.totalPrize }} 💰</p>
                </div>
                <div>
                  <p class="text-slate-500 mb-1">Début</p>
                  <p class="text-slate-200 font-semibold text-xs">{{ formatDate(tournament.startDate) }}</p>
                </div>
                <div v-if="tournament.endDate">
                  <p class="text-slate-500 mb-1">Fin</p>
                  <p class="text-slate-200 font-semibold text-xs">{{ formatDate(tournament.endDate) }}</p>
                </div>
              </div>

              <!-- Récompenses -->
              <div class="mb-4 p-3 rounded-xl bg-slate-800/50 border border-slate-700/30">
                <p class="text-xs text-slate-400 mb-2">Récompenses</p>
                <div class="space-y-1 text-xs">
                  <div class="flex justify-between">
                    <span class="text-slate-300">🥇 1er</span>
                    <span class="text-yellow-400 font-bold">{{ tournament.firstPlacePrize }} 💰</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-slate-300">🥈 2ème</span>
                    <span class="text-slate-400 font-bold">{{ tournament.secondPlacePrize }} 💰</span>
                  </div>
                  <div class="flex justify-between">
                    <span class="text-slate-300">🥉 3ème</span>
                    <span class="text-slate-400 font-bold">{{ tournament.thirdPlacePrize }} 💰</span>
                  </div>
                </div>
              </div>

              <!-- Actions -->
              <button
                @click="uiStore.goToTournaments()"
                class="w-full px-4 py-2.5 rounded-xl bg-gradient-to-r from-cyan-500 to-purple-500 text-white font-semibold hover:from-cyan-400 hover:to-purple-400 transition-all duration-300 text-sm"
              >
                Voir le tournoi
              </button>
            </div>
          </div>
        </div>

        <!-- Aucun tournoi -->
        <div v-else class="text-center py-12 rounded-2xl bg-slate-900/50 border border-slate-700/30">
          <div class="text-5xl mb-4 opacity-50">🏆</div>
          <p class="text-slate-400 mb-2">Vous n'êtes inscrit à aucun tournoi</p>
          <button
            @click="uiStore.goToTournaments()"
            class="mt-4 px-6 py-3 rounded-xl bg-gradient-to-r from-cyan-500 to-purple-500 text-white font-bold hover:from-cyan-400 hover:to-purple-400 transition-all"
          >
            Voir les tournois disponibles
          </button>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, computed, onMounted } from 'vue'
import { useUserStore } from '@/stores/user'
import { useUiStore } from '@/stores/ui'
import { getUserItems } from '@/services/shopApi'
import * as tournamentsApi from '@/services/tournamentsApi'
import type { UserItem } from '@/types'
import type { TournamentDto } from '@/services/tournamentsApi'
import { PuzzleTheme } from '@/types'
import { getThemeConfig } from '@/utils/themeConfig'

const userStore = useUserStore()
const uiStore = useUiStore()

const userItems = ref<UserItem[]>([])
const isLoading = ref(false)
const userTournaments = ref<TournamentDto[]>([])
const isLoadingTournaments = ref(false)

const equippedAvatar = computed(() => {
  return userItems.value.find(ui => ui.isEquipped && ui.item.itemType === 'Avatar')?.item
})

const equippedTheme = computed(() => {
  return userItems.value.find(ui => ui.isEquipped && ui.item.itemType === 'Theme')?.item
})

const equippedDecoration = computed(() => {
  return userItems.value.find(ui => ui.isEquipped && ui.item.itemType === 'Decoration')?.item
})

const equippedItemsCount = computed(() => {
  return userItems.value.filter(ui => ui.isEquipped).length
})

const totalSpent = computed(() => {
  return userItems.value.reduce((sum, ui) => sum + ui.item.price, 0)
})

// Mapper le nom du thème équipé vers un PuzzleTheme pour obtenir les couleurs
function getThemeFromItem(themeItem: any): PuzzleTheme | null {
  if (!themeItem) return null
  
  const themeName = themeItem.name?.toLowerCase() || ''
  const themeMap: Record<string, PuzzleTheme> = {
    'classic': PuzzleTheme.Classic,
    'classique': PuzzleTheme.Classic,
    'medieval': PuzzleTheme.Medieval,
    'futuristic': PuzzleTheme.Futuristic,
    'futuriste': PuzzleTheme.Futuristic,
    'underwater': PuzzleTheme.Underwater,
    'aquatique': PuzzleTheme.Underwater,
    'desert': PuzzleTheme.Desert,
    'forest': PuzzleTheme.Forest,
    'jungle': PuzzleTheme.Forest,
    'ice': PuzzleTheme.Ice,
    'glacier': PuzzleTheme.Ice,
    'volcano': PuzzleTheme.Volcano,
    'volcan': PuzzleTheme.Volcano,
    'neon': PuzzleTheme.Neon,
    'cyberpunk': PuzzleTheme.Neon,
    'steampunk': PuzzleTheme.Steampunk,
    'pirate': PuzzleTheme.Pirate,
    'zombie': PuzzleTheme.Zombie,
    'apocalypse': PuzzleTheme.Zombie,
    'ninja': PuzzleTheme.Ninja,
    'magic': PuzzleTheme.Magic,
    'magie': PuzzleTheme.Magic,
    'western': PuzzleTheme.Western,
    'far west': PuzzleTheme.Western,
  }
  
  for (const [key, theme] of Object.entries(themeMap)) {
    if (themeName.includes(key)) {
      return theme
    }
  }
  
  return PuzzleTheme.Classic // Par défaut
}

// Styles dynamiques basés sur le thème équipé
const themeConfig = computed(() => {
  if (!equippedTheme.value) {
    // Thème par défaut (purple/pink)
    return {
      background: 'linear-gradient(135deg, rgba(147, 51, 234, 0.3) 0%, rgba(219, 39, 119, 0.3) 50%, rgba(15, 23, 42, 1) 100%)',
      pattern: 'repeating-linear-gradient(45deg, transparent, transparent 10px, rgba(156, 146, 172, 0.1) 10px, rgba(156, 146, 172, 0.1) 20px)',
      avatarGradient: 'linear-gradient(135deg, rgb(168, 85, 247) 0%, rgb(236, 72, 153) 100%)',
      avatarBorder: 'rgba(255, 255, 255, 0.2)',
      badgeBg: 'rgba(15, 23, 42, 0.9)',
      badgeBorder: 'rgba(168, 85, 247, 0.3)'
    }
  }
  
  const puzzleTheme = getThemeFromItem(equippedTheme.value)
  if (!puzzleTheme) {
    return {
      background: 'linear-gradient(135deg, rgba(147, 51, 234, 0.3) 0%, rgba(219, 39, 119, 0.3) 50%, rgba(15, 23, 42, 1) 100%)',
      pattern: 'repeating-linear-gradient(45deg, transparent, transparent 10px, rgba(156, 146, 172, 0.1) 10px, rgba(156, 146, 172, 0.1) 20px)',
      avatarGradient: 'linear-gradient(135deg, rgb(168, 85, 247) 0%, rgb(236, 72, 153) 100%)',
      avatarBorder: 'rgba(255, 255, 255, 0.2)',
      badgeBg: 'rgba(15, 23, 42, 0.9)',
      badgeBorder: 'rgba(168, 85, 247, 0.3)'
    }
  }
  
  const config = getThemeConfig(puzzleTheme)
  
  // Extraire les couleurs principales du thème
  const primaryColor = config.colors.islandGradient[1] || config.colors.islandSelected[1] || '#667eea'
  const secondaryColor = config.colors.islandGradient[2] || config.colors.islandSelected[2] || '#764ba2'
  
  return {
    background: `linear-gradient(135deg, ${primaryColor}40 0%, ${secondaryColor}40 50%, rgba(15, 23, 42, 1) 100%)`,
    pattern: `repeating-linear-gradient(45deg, transparent, transparent 10px, ${primaryColor}20 10px, ${primaryColor}20 20px)`,
    avatarGradient: `linear-gradient(135deg, ${primaryColor} 0%, ${secondaryColor} 100%)`,
    avatarBorder: `${primaryColor}80`,
    badgeBg: 'rgba(15, 23, 42, 0.9)',
    badgeBorder: `${primaryColor}50`
  }
})

const themeHeaderStyle = computed(() => ({
  background: themeConfig.value.background
}))

const themePatternStyle = computed(() => ({
  background: themeConfig.value.pattern
}))

const avatarStyle = computed(() => ({
  background: themeConfig.value.avatarGradient,
  borderColor: themeConfig.value.avatarBorder
}))

const avatarBadgeStyle = computed(() => ({
  background: themeConfig.value.badgeBg,
  border: `1px solid ${themeConfig.value.badgeBorder}`
}))

async function loadUserItems() {
  if (!userStore.userId) return
  
  isLoading.value = true
  try {
    userItems.value = await getUserItems(userStore.userId)
  } catch (err) {
    console.error('Erreur lors du chargement des items:', err)
  } finally {
    isLoading.value = false
  }
}

async function loadUserTournaments() {
  if (!userStore.userId) return
  
  isLoadingTournaments.value = true
  try {
    const allTournaments = await tournamentsApi.getAllTournaments(userStore.userId)
    // Filtrer seulement les tournois auxquels l'utilisateur est inscrit
    userTournaments.value = allTournaments.filter(t => t.isUserRegistered)
  } catch (err) {
    console.error('Erreur lors du chargement des tournois:', err)
  } finally {
    isLoadingTournaments.value = false
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

function getPositionEmoji(position: number): string {
  if (position === 1) return '🥇'
  if (position === 2) return '🥈'
  if (position === 3) return '🥉'
  return '📍'
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
  if (userStore.isLoggedIn) {
    loadUserItems()
    loadUserTournaments()
  }
})
</script>

