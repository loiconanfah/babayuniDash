<template>
  <section class="w-full h-full px-4 sm:px-6 lg:px-12 py-8 overflow-y-auto">
    <div class="max-w-7xl mx-auto">
      <!-- En-tête -->
      <header class="mb-8">
        <div class="flex items-center gap-3 mb-3">
          <div class="h-1 w-12 bg-gradient-to-r from-cyan-400 via-purple-500 to-pink-500 rounded-full"></div>
          <p class="text-sm uppercase tracking-[0.3em] text-cyan-300 font-semibold">
            Boutique
          </p>
        </div>
        <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
          <div class="flex-1">
            <div class="flex items-center gap-3 mb-3">
              <IconShop class="h-8 w-8 sm:h-10 sm:w-10 text-cyan-400" />
              <h2 class="text-3xl sm:text-4xl lg:text-5xl font-extrabold text-zinc-50 bg-gradient-to-r from-cyan-400 via-purple-400 to-pink-400 bg-clip-text text-transparent">
                Boutique
              </h2>
            </div>
            <p class="text-sm sm:text-base text-zinc-300 max-w-2xl">
              Découvre des items uniques pour personnaliser ton expérience. 
              Chaque achat te rapproche d'un style unique.
            </p>
          </div>
          <!-- Affichage des coins -->
          <div v-if="userStore.isLoggedIn" class="flex items-center gap-2 sm:gap-3 px-4 sm:px-6 py-2.5 sm:py-3 rounded-xl sm:rounded-2xl bg-gradient-to-br from-cyan-500/20 to-purple-500/20 border border-cyan-500/30 backdrop-blur-sm flex-shrink-0">
            <IconDiamond class="h-6 w-6 sm:h-8 sm:w-8 text-cyan-400" />
            <div>
              <p class="text-[10px] sm:text-xs text-zinc-400">Tes coins</p>
              <p class="text-xl sm:text-2xl font-bold text-cyan-400">{{ userStore.coins }}</p>
            </div>
          </div>
        </div>
      </header>

      <!-- Filtres par type -->
      <div class="mb-8 flex flex-wrap gap-3">
        <button
          v-for="type in itemTypes"
          :key="type"
          @click="selectedType = type"
          class="px-5 py-2.5 rounded-xl text-sm font-semibold transition-all duration-300 hover:scale-105"
          :class="selectedType === type
            ? 'bg-gradient-to-r from-cyan-500 to-purple-500 text-white shadow-lg shadow-cyan-500/40'
            : 'bg-zinc-800/60 text-zinc-300 hover:bg-zinc-700/60 border border-zinc-700/50'"
        >
          {{ type }}
        </button>
      </div>

      <!-- Grille d'items -->
      <div v-if="isLoading" class="flex items-center justify-center py-20">
        <div class="text-zinc-400">Chargement...</div>
      </div>

      <div v-else-if="error" class="text-red-400 text-center py-20">
        {{ error }}
      </div>

      <!-- Grille horizontale scrollable style CS:GO -->
      <div v-else class="space-y-6">
        <!-- Section par type -->
        <div v-for="type in itemTypes.filter(t => t !== 'Tous')" :key="type">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-xl font-bold text-zinc-50">{{ type }}</h3>
            <span class="text-sm text-zinc-400">{{ filteredItems.filter(i => i.itemType === type).length }} items</span>
          </div>
          <div class="flex gap-4 overflow-x-auto pb-4 scrollbar-thin scrollbar-thumb-zinc-700 scrollbar-track-zinc-900">
            <div
              v-for="item in filteredItems.filter(i => selectedType === 'Tous' ? i.itemType === type : i.itemType === selectedType)"
              :key="item.id"
              @click="openItemDetails(item)"
              class="group relative flex-shrink-0 w-64 rounded-xl bg-gradient-to-br from-zinc-900/95 to-zinc-800/95 border border-zinc-700/50 overflow-hidden shadow-xl hover:shadow-cyan-500/20 hover:border-cyan-500/30 transition-all duration-300 hover:scale-[1.02] cursor-pointer"
            >
              <!-- Image/Icone de l'item avec effet glow -->
              <div class="relative h-40 bg-gradient-to-br from-cyan-600/20 via-zinc-900 to-zinc-800 flex items-center justify-center overflow-hidden">
                <div class="absolute inset-0 bg-gradient-to-t from-zinc-900 via-transparent to-transparent z-10"></div>
                <div class="text-5xl opacity-50 group-hover:scale-110 transition-transform duration-500 z-0">{{ item.icon }}</div>
                <div v-if="item.isOwned" class="absolute top-2 right-2 z-20">
                  <span class="px-2 py-1 rounded-lg text-[10px] font-bold bg-gradient-to-r from-emerald-500 to-teal-500 text-white shadow-lg shadow-emerald-500/50">
                    Possédé
                  </span>
                </div>
                <div class="absolute top-2 left-2 z-20">
                  <span class="px-2 py-1 rounded-lg text-[10px] font-bold bg-zinc-900/90 backdrop-blur-sm text-zinc-100 border border-zinc-700/50">
                    {{ item.rarity }}
                  </span>
                </div>
                <!-- Badge de type en bas -->
                <div class="absolute bottom-2 left-2 z-20">
                  <span class="px-2 py-1 rounded-lg text-[10px] font-semibold bg-cyan-500/20 text-cyan-300 border border-cyan-500/30">
                    {{ item.itemType }}
                  </span>
                </div>
              </div>

              <!-- Contenu compact -->
              <div class="p-4 bg-zinc-900/50">
                <h3 class="text-base font-bold text-zinc-50 mb-1 truncate">{{ item.name }}</h3>
                <p class="text-xs text-zinc-400 mb-3 line-clamp-1">{{ item.description }}</p>
                
                <div class="flex items-center justify-between">
                  <div class="flex items-center gap-1.5">
                    <IconDiamond class="h-4 w-4 text-cyan-400" />
                    <span class="text-lg font-bold text-cyan-400">{{ item.price }}</span>
                  </div>
                  <button
                    v-if="!item.isOwned"
                    @click.stop="handlePurchase(item)"
                    :disabled="!userStore.isLoggedIn || userStore.coins < item.price || isPurchasing"
                    class="px-3 py-1.5 rounded-lg bg-gradient-to-r from-cyan-500 to-purple-500 text-white text-xs font-bold hover:from-cyan-400 hover:to-purple-400 transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed flex items-center gap-1"
                  >
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-3.5 w-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                      <path stroke-linecap="round" stroke-linejoin="round" d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
                    </svg>
                    <span v-if="!isPurchasing">Acheter</span>
                    <span v-else class="animate-spin">⏳</span>
                  </button>
                  <button
                    v-else
                    @click.stop="uiStore.goToCollection()"
                    class="px-3 py-1.5 rounded-lg bg-gradient-to-r from-emerald-500 to-teal-500 text-white text-xs font-bold hover:from-emerald-400 hover:to-teal-400 transition-all duration-200"
                  >
                    Possédé
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal de détails d'item -->
    <div
      v-if="selectedItem"
      @click.self="closeItemDetails"
      class="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm p-4"
    >
      <div class="relative w-full max-w-2xl rounded-3xl bg-gradient-to-br from-slate-900 to-slate-800 border border-slate-700/50 shadow-2xl overflow-hidden">
        <!-- Header avec gradient -->
        <div class="relative h-64 bg-gradient-to-br from-indigo-600/30 via-slate-900 to-slate-800 flex items-center justify-center overflow-hidden">
          <div class="text-6xl opacity-30">{{ selectedItem.icon }}</div>
          <button
            @click="closeItemDetails"
            class="absolute top-4 right-4 w-10 h-10 rounded-full bg-slate-900/80 backdrop-blur-sm text-slate-300 hover:text-white hover:bg-slate-800 transition-all flex items-center justify-center"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
          <div v-if="selectedItem.isOwned" class="absolute top-4 left-4">
            <span class="px-4 py-2 rounded-full text-sm font-bold bg-gradient-to-r from-emerald-500 to-teal-500 text-white shadow-lg shadow-emerald-500/30">
              Possédé
            </span>
          </div>
          <div class="absolute bottom-4 left-4">
            <span class="px-4 py-2 rounded-full text-sm font-bold bg-slate-900/80 backdrop-blur-sm text-slate-100">
              {{ selectedItem.rarity }}
            </span>
          </div>
        </div>

        <!-- Contenu -->
        <div class="p-8">
          <div class="flex items-start justify-between mb-6">
            <div>
              <h2 class="text-3xl font-bold text-slate-50 mb-2">{{ selectedItem.name }}</h2>
              <span class="px-4 py-1.5 rounded-full text-sm font-medium bg-slate-700/50 text-slate-300">
                {{ selectedItem.itemType }}
              </span>
            </div>
            <div class="text-right">
              <p class="text-sm text-slate-400 mb-1">Prix</p>
              <div class="flex items-center gap-2">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-yellow-400" fill="currentColor" viewBox="0 0 20 20">
                  <path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 01-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 01-.567.267z" />
                  <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-13a1 1 0 10-2 0v.092a4.535 4.535 0 00-1.676.662C6.602 6.234 6 7.009 6 8c0 .99.602 1.765 1.324 2.246.48.32 1.054.545 1.676.662v1.941c-.391-.127-.68-.317-.843-.504a1 1 0 10-1.51 1.31c.562.649 1.413 1.076 2.353 1.253V15a1 1 0 102 0v-.092a4.535 4.535 0 001.676-.662C13.398 13.766 14 12.991 14 12c0-.99-.602-1.765-1.324-2.246A4.535 4.535 0 0011 9.092V7.151c.391.127.68.317.843.504a1 1 0 101.511-1.31c-.563-.649-1.413-1.076-2.354-1.253V5z" clip-rule="evenodd" />
                </svg>
                <span class="text-3xl font-bold text-yellow-400">{{ selectedItem.price }}</span>
              </div>
            </div>
          </div>

          <div class="mb-6">
            <h3 class="text-lg font-semibold text-slate-300 mb-3">Description</h3>
            <p class="text-slate-200 leading-relaxed">{{ selectedItem.description }}</p>
          </div>

          <div class="mb-6 p-4 rounded-xl bg-slate-800/50 border border-slate-700/50">
            <h3 class="text-sm font-semibold text-slate-400 mb-2">Informations</h3>
            <div class="space-y-2 text-sm">
              <div class="flex justify-between">
                <span class="text-slate-400">Type:</span>
                <span class="text-slate-200 font-medium">{{ selectedItem.itemType }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-slate-400">Rareté:</span>
                <span class="text-slate-200 font-medium">{{ selectedItem.rarity }}</span>
              </div>
              <div class="flex justify-between">
                <span class="text-slate-400">Statut:</span>
                <span class="text-slate-200 font-medium">{{ selectedItem.isOwned ? 'Possédé' : 'Non possédé' }}</span>
              </div>
            </div>
          </div>

          <!-- Actions -->
          <div class="flex gap-4">
            <button
              v-if="!selectedItem.isOwned"
              @click="handlePurchase(selectedItem)"
              :disabled="!userStore.isLoggedIn || userStore.coins < selectedItem.price || isPurchasing"
              class="flex-1 px-6 py-4 rounded-xl bg-gradient-to-r from-yellow-500 to-orange-500 text-slate-900 text-base font-bold tracking-wide hover:from-yellow-400 hover:to-orange-400 transition-all duration-300 shadow-lg shadow-yellow-500/30 hover:shadow-xl hover:shadow-yellow-500/40 disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
            >
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 3h2l.4 2M7 13h10l4-8H5.4M7 13L5.4 5M7 13l-2.293 2.293c-.63.63-.184 1.707.707 1.707H17m0 0a2 2 0 100 4 2 2 0 000-4zm-8 2a2 2 0 11-4 0 2 2 0 014 0z" />
              </svg>
              <span v-if="!isPurchasing">Acheter maintenant</span>
              <span v-else class="animate-spin">⏳</span>
            </button>
            <button
              v-else
              @click="uiStore.goToCollection()"
              class="flex-1 px-6 py-4 rounded-xl bg-gradient-to-r from-green-500 to-emerald-500 text-white text-base font-bold hover:from-green-400 hover:to-emerald-400 transition-all duration-300 flex items-center justify-center gap-2"
            >
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
              </svg>
              Voir dans ma Collection
            </button>
            <button
              @click="closeItemDetails"
              class="px-6 py-4 rounded-xl bg-slate-700/50 text-slate-300 hover:bg-slate-700 text-base font-medium transition-all"
            >
              Fermer
            </button>
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
import { getAllItems, purchaseItem, getUserCoins } from '@/services/shopApi'
import type { Item } from '@/types'
import IconShop from '@/components/icons/IconShop.vue'
import IconDiamond from '@/components/icons/IconDiamond.vue'

const userStore = useUserStore()
const uiStore = useUiStore()

const items = ref<Item[]>([])
const isLoading = ref(false)
const error = ref<string | null>(null)
const isPurchasing = ref(false)
const selectedType = ref<string>('Tous')
const selectedItem = ref<Item | null>(null)

const itemTypes = ['Tous', 'Avatar', 'Theme', 'PowerUp', 'Decoration']

const filteredItems = computed(() => {
  if (selectedType.value === 'Tous') {
    return items.value
  }
  return items.value.filter(item => item.itemType === selectedType.value)
})

async function loadItems() {
  isLoading.value = true
  error.value = null
  try {
    const userId = userStore.userId
    items.value = await getAllItems(userId || undefined)
    if (userId) {
      const coinsData = await getUserCoins(userId)
      userStore.coins = coinsData.coins
    }
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Erreur lors du chargement des items'
  } finally {
    isLoading.value = false
  }
}

function openItemDetails(item: Item) {
  selectedItem.value = item
}

function closeItemDetails() {
  selectedItem.value = null
}

async function handlePurchase(item: Item) {
  if (!userStore.userId) {
    error.value = 'Connecte-toi pour acheter des items'
    return
  }

  if (userStore.coins < item.price) {
    error.value = 'Tu n\'as pas assez de coins pour cet item'
    return
  }

  isPurchasing.value = true
  try {
    await purchaseItem({
      itemId: item.id,
      userId: userStore.userId
    })
    
    // Recharger les items et les coins
    await loadItems()
    // Fermer le modal après achat réussi
    closeItemDetails()
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Erreur lors de l\'achat'
  } finally {
    isPurchasing.value = false
  }
}

onMounted(() => {
  loadItems()
})
</script>

