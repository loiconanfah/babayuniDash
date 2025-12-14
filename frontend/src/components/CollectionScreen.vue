<template>
  <section class="w-full h-full px-4 sm:px-6 lg:px-12 py-8 overflow-y-auto">
    <div class="max-w-7xl mx-auto">
      <!-- En-tête -->
      <header class="mb-8">
        <div class="flex items-center gap-3 mb-3">
          <div class="h-1 w-12 bg-gradient-to-r from-purple-500 to-pink-500 rounded-full"></div>
          <p class="text-sm uppercase tracking-[0.3em] text-purple-400 font-semibold">
            Ma Collection
          </p>
        </div>
        <h2 class="text-4xl sm:text-5xl font-extrabold text-slate-50 mb-3 bg-gradient-to-r from-slate-50 via-slate-100 to-slate-200 bg-clip-text text-transparent">
          📦 Ma Collection
        </h2>
        <p class="text-base text-slate-300 max-w-2xl">
          Gérez vos items possédés. Équipez-les pour personnaliser votre expérience !
        </p>
      </header>

      <!-- Filtres -->
      <div class="mb-8 flex flex-wrap gap-3">
        <button
          v-for="type in itemTypes"
          :key="type"
          @click="selectedType = type"
          class="px-5 py-2.5 rounded-xl text-sm font-semibold transition-all duration-300 hover:scale-105"
          :class="selectedType === type
            ? 'bg-gradient-to-r from-purple-500 to-pink-500 text-white shadow-lg shadow-purple-500/30'
            : 'bg-slate-800/60 text-slate-300 hover:bg-slate-700/60 border border-slate-700/50'"
        >
          {{ type }}
        </button>
      </div>

      <!-- Collection -->
      <div v-if="isLoading" class="flex items-center justify-center py-20">
        <div class="text-slate-400">Chargement...</div>
      </div>

      <div v-else-if="error" class="text-red-400 text-center py-20">
        {{ error }}
      </div>

      <div v-else-if="filteredUserItems.length === 0" class="text-center py-20">
        <div class="text-6xl mb-4">📭</div>
        <p class="text-xl text-slate-400 mb-2">Votre collection est vide</p>
        <p class="text-sm text-slate-500">Visitez la boutique pour acheter des items !</p>
      </div>

      <div v-else class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6">
        <div
          v-for="userItem in filteredUserItems"
          :key="userItem.id"
          class="group relative rounded-2xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border overflow-hidden shadow-xl transition-all duration-300 hover:scale-[1.02]"
          :class="userItem.isEquipped
            ? 'border-purple-500/50 shadow-purple-500/20'
            : 'border-slate-700/50 hover:border-purple-500/30'"
        >
          <!-- Image de l'item -->
          <div class="relative h-48 bg-gradient-to-br from-purple-600/20 via-slate-900 to-slate-800 flex items-center justify-center overflow-hidden">
            <img
              v-if="userItem.item.imageUrl"
              :src="userItem.item.imageUrl"
              :alt="userItem.item.name"
              class="w-full h-full object-cover group-hover:scale-110 transition-transform duration-500"
            />
            <div v-else class="text-8xl opacity-40 group-hover:scale-110 transition-transform duration-500">{{ userItem.item.icon }}</div>
            <div v-if="userItem.isEquipped" class="absolute top-3 right-3">
              <span class="px-3 py-1.5 rounded-full text-xs font-bold bg-gradient-to-r from-purple-500 to-pink-500 text-white shadow-lg">
                Équipé
              </span>
            </div>
            <div class="absolute top-3 left-3">
              <span class="px-3 py-1.5 rounded-full text-xs font-bold bg-slate-900/80 backdrop-blur-sm text-slate-100">
                {{ userItem.item.rarity }}
              </span>
            </div>
          </div>

          <!-- Contenu -->
          <div class="p-5">
            <h3 class="text-xl font-bold text-slate-50 mb-2">{{ userItem.item.name }}</h3>
            <p class="text-sm text-slate-300 leading-relaxed mb-4 line-clamp-2">{{ userItem.item.description }}</p>
            
            <div class="flex items-center justify-between mb-4">
              <span class="px-3 py-1 rounded-full text-xs font-medium bg-slate-700/50 text-slate-300">
                {{ userItem.item.itemType }}
              </span>
              <span class="text-xs text-slate-400">
                Acheté le {{ new Date(userItem.purchasedAt).toLocaleDateString('fr-FR') }}
              </span>
            </div>

            <button
              @click="handleEquip(userItem)"
              :disabled="isEquipping"
              class="w-full px-4 py-3 rounded-xl text-sm font-bold tracking-wide transition-all duration-300 shadow-lg hover:shadow-xl hover:scale-105 active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
              :class="userItem.isEquipped
                ? 'bg-gradient-to-r from-slate-700 to-slate-800 text-slate-200 hover:from-slate-600 hover:to-slate-700'
                : 'bg-gradient-to-r from-purple-500 to-pink-500 text-white hover:from-purple-400 hover:to-pink-400'"
            >
              <span v-if="!isEquipping">
                {{ userItem.isEquipped ? 'Déséquiper' : 'Équiper' }}
              </span>
              <span v-else class="animate-spin">⏳</span>
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
import { getUserItems, equipItem } from '@/services/shopApi'
import type { UserItem } from '@/types'

const userStore = useUserStore()

const userItems = ref<UserItem[]>([])
const isLoading = ref(false)
const error = ref<string | null>(null)
const isEquipping = ref(false)
const selectedType = ref<string>('Tous')

const itemTypes = ['Tous', 'Avatar', 'Theme', 'PowerUp', 'Decoration']

const filteredUserItems = computed(() => {
  if (selectedType.value === 'Tous') {
    return userItems.value
  }
  return userItems.value.filter(ui => ui.item.itemType === selectedType.value)
})

async function loadUserItems() {
  if (!userStore.userId) {
    error.value = 'Vous devez être connecté'
    return
  }

  isLoading.value = true
  error.value = null
  try {
    userItems.value = await getUserItems(userStore.userId)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Erreur lors du chargement de la collection'
  } finally {
    isLoading.value = false
  }
}

async function handleEquip(userItem: UserItem) {
  if (isEquipping.value) return // Éviter les clics multiples
  
  isEquipping.value = true
  error.value = null
  
  try {
    // Si on équipe un item, le backend devrait automatiquement déséquiper les autres du même type
    const newEquippedState = !userItem.isEquipped
    
    await equipItem(userItem.id, {
      userItemId: userItem.id,
      isEquipped: newEquippedState
    })
    
    // Recharger la collection pour avoir l'état à jour
    await loadUserItems()
    
    // Si c'est un avatar, recharger l'avatar équipé dans le store
    if (userItem.item.itemType === 'Avatar') {
      await userStore.loadEquippedAvatar()
    }
    
    // Recharger aussi les coins au cas où
    await userStore.loadCoins()
  } catch (err) {
    console.error('Erreur lors de l\'équipement:', err)
    error.value = err instanceof Error ? err.message : 'Erreur lors de l\'équipement'
    // Recharger quand même pour avoir l'état actuel
    await loadUserItems()
  } finally {
    isEquipping.value = false
  }
}

onMounted(() => {
  if (userStore.isLoggedIn) {
    loadUserItems()
  } else {
    error.value = 'Vous devez être connecté pour voir votre collection'
  }
})
</script>

