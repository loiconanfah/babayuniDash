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
            <!-- Header avec gradient -->
            <div class="relative h-48 bg-gradient-to-br from-purple-600/30 via-pink-600/30 to-slate-900 flex items-center justify-center overflow-hidden">
              <div class="absolute inset-0 opacity-10" style="background: repeating-linear-gradient(45deg, transparent, transparent 10px, rgba(156, 146, 172, 0.1) 10px, rgba(156, 146, 172, 0.1) 20px);"></div>
              
              <!-- Avatar équipé -->
              <div class="relative z-10">
                <div class="h-32 w-32 rounded-full bg-gradient-to-br from-purple-500 to-pink-500 flex items-center justify-center text-6xl shadow-2xl border-4 border-white/20">
                  {{ equippedAvatar?.icon || '👤' }}
                </div>
                <div v-if="equippedAvatar" class="absolute -bottom-2 left-1/2 transform -translate-x-1/2 px-3 py-1 rounded-full bg-slate-900/90 backdrop-blur-sm text-xs text-slate-200 border border-purple-500/30">
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
            </div>
          </div>
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
import type { UserItem } from '@/types'

const userStore = useUserStore()
const uiStore = useUiStore()

const userItems = ref<UserItem[]>([])
const isLoading = ref(false)

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

onMounted(() => {
  if (userStore.isLoggedIn) {
    loadUserItems()
  }
})
</script>

