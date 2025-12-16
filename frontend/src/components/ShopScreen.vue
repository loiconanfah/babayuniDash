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
          <!-- Affichage des Babayuni -->
          <div v-if="userStore.isLoggedIn" class="flex items-center gap-2 sm:gap-3 px-4 sm:px-6 py-2.5 sm:py-3 rounded-xl sm:rounded-2xl bg-gradient-to-br from-cyan-500/20 to-purple-500/20 border border-cyan-500/30 backdrop-blur-sm flex-shrink-0">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-3 w-3 text-cyan-400" fill="currentColor" viewBox="0 0 24 24">
              <circle cx="12" cy="12" r="9" stroke="currentColor" stroke-width="1.5" fill="currentColor" opacity="0.2"/>
              <circle cx="12" cy="12" r="5" fill="currentColor"/>
            </svg>
            <div>
              <p class="text-[10px] sm:text-xs text-zinc-400">Tes Babayuni</p>
              <p class="text-xl sm:text-2xl font-bold text-cyan-400">{{ userStore.coins }}</p>
            </div>
          </div>
        </div>
      </header>

      <!-- Section Achat de Babayuni -->
      <div v-if="userStore.isLoggedIn" class="mb-8 rounded-3xl bg-gradient-to-br from-yellow-500/20 via-orange-500/20 to-red-500/20 border border-yellow-500/30 p-6 backdrop-blur-sm">
        <div class="flex items-center justify-between mb-6">
          <div>
            <h3 class="text-2xl font-bold text-zinc-50 mb-2">💰 Acheter des Babayuni</h3>
            <p class="text-sm text-zinc-400">Rechargez votre compte pour continuer à jouer</p>
          </div>
          <div class="text-right">
            <p class="text-xs text-zinc-400 mb-1">Solde actuel</p>
            <p class="text-3xl font-bold text-yellow-400">{{ userStore.coins }}</p>
          </div>
        </div>
        
        <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
          <!-- Pack 1 : 500 Babayuni -->
          <button
            @click="purchaseCoins(500)"
            :disabled="isPurchasingCoins"
            class="group relative rounded-2xl bg-gradient-to-br from-yellow-500/90 to-orange-500/90 border-2 border-yellow-400/50 p-6 hover:from-yellow-400 hover:to-orange-400 transition-all duration-300 hover:scale-105 hover:shadow-2xl hover:shadow-yellow-500/50 disabled:opacity-50 disabled:cursor-not-allowed overflow-hidden"
          >
            <div class="absolute top-2 right-2 px-2 py-1 rounded-lg bg-yellow-600/80 text-white text-xs font-bold">
              POPULAIRE
            </div>
            <div class="text-center">
              <div class="text-5xl mb-3">💰</div>
              <h4 class="text-2xl font-bold text-white mb-2">500</h4>
              <p class="text-sm text-yellow-100 mb-4">Babayuni</p>
              <div class="px-4 py-2 rounded-xl bg-white/20 backdrop-blur-sm">
                <p class="text-xs text-white/80 line-through mb-1">10€</p>
                <p class="text-lg font-bold text-white">Gratuit</p>
              </div>
            </div>
          </button>

          <!-- Pack 2 : 1000 Babayuni -->
          <button
            @click="purchaseCoins(1000)"
            :disabled="isPurchasingCoins || userStore.coins < 100"
            class="group relative rounded-2xl bg-gradient-to-br from-orange-500/90 to-red-500/90 border-2 border-orange-400/50 p-6 hover:from-orange-400 hover:to-red-400 transition-all duration-300 hover:scale-105 hover:shadow-2xl hover:shadow-orange-500/50 disabled:opacity-50 disabled:cursor-not-allowed overflow-hidden"
          >
            <div class="absolute top-2 right-2 px-2 py-1 rounded-lg bg-orange-600/80 text-white text-xs font-bold">
              MEILLEUR
            </div>
            <div class="text-center">
              <div class="text-5xl mb-3">💎</div>
              <h4 class="text-2xl font-bold text-white mb-2">1000</h4>
              <p class="text-sm text-orange-100 mb-4">Babayuni</p>
              <div class="px-4 py-2 rounded-xl bg-white/20 backdrop-blur-sm">
                <p class="text-xs text-white/80 mb-1">Prix</p>
                <p class="text-lg font-bold text-white">100 💰</p>
              </div>
            </div>
          </button>

          <!-- Pack 3 : 2500 Babayuni -->
          <button
            @click="purchaseCoins(2500)"
            :disabled="isPurchasingCoins || userStore.coins < 200"
            class="group relative rounded-2xl bg-gradient-to-br from-red-500/90 to-pink-500/90 border-2 border-red-400/50 p-6 hover:from-red-400 hover:to-pink-400 transition-all duration-300 hover:scale-105 hover:shadow-2xl hover:shadow-red-500/50 disabled:opacity-50 disabled:cursor-not-allowed overflow-hidden"
          >
            <div class="absolute top-2 right-2 px-2 py-1 rounded-lg bg-red-600/80 text-white text-xs font-bold">
              MAXIMUM
            </div>
            <div class="text-center">
              <div class="text-5xl mb-3">👑</div>
              <h4 class="text-2xl font-bold text-white mb-2">2500</h4>
              <p class="text-sm text-red-100 mb-4">Babayuni</p>
              <div class="px-4 py-2 rounded-xl bg-white/20 backdrop-blur-sm">
                <p class="text-xs text-white/80 mb-1">Prix</p>
                <p class="text-lg font-bold text-white">200 💰</p>
              </div>
            </div>
          </button>
        </div>
      </div>

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
          <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4" style="perspective: 1000px;">
            <div
              v-for="item in filteredItems.filter(i => selectedType === 'Tous' ? i.itemType === type : i.itemType === selectedType)"
              :key="item.id"
              class="group relative rounded-2xl bg-zinc-900 border border-zinc-800 overflow-hidden shadow-lg hover:shadow-2xl hover:border-zinc-700 transition-all duration-500 cursor-pointer transform-style-3d"
              @mouseenter="(e) => handleCardEnter(e, item.id)"
              @mouseleave="(e) => handleCardLeave(e, item.id)"
              @mousemove="(e) => handleCardMove(e, item.id)"
              :style="{ transform: `rotateX(${cardRotations[item.id]?.x || 0}deg) rotateY(${cardRotations[item.id]?.y || 0}deg) translateZ(${cardRotations[item.id]?.z || 0}px)` }"
            >
              <!-- Image avec fond beige/jaune dégradé -->
              <div class="relative h-64 bg-gradient-to-b from-amber-50/50 via-amber-100/40 to-amber-50/50">
                <img
                  v-if="item.imageUrl"
                  :src="item.imageUrl"
                  :alt="item.name"
                  class="w-full h-full object-contain p-6 transition-transform duration-500"
                />
                <div v-else class="w-full h-full flex items-center justify-center">
                  <div class="text-6xl opacity-40">{{ item.icon }}</div>
                </div>
              </div>

              <!-- Section inférieure sombre -->
              <div class="bg-zinc-900 p-5">
                <!-- Titre -->
                <h3 class="text-base font-bold text-white mb-3 truncate">{{ item.name }}</h3>
                
                <!-- Type avec icône circulaire -->
                <div class="flex items-center gap-2 mb-4">
                  <div 
                    :class="[
                      'h-4 w-4 rounded-full',
                      item.itemType === 'Avatar' ? 'bg-purple-500' :
                      item.itemType === 'Theme' ? 'bg-pink-500' :
                      item.itemType === 'PowerUp' ? 'bg-blue-500' :
                      'bg-cyan-500'
                    ]"
                  ></div>
                  <span class="text-xs text-zinc-400 uppercase tracking-wider font-medium">{{ item.itemType }}</span>
                </div>
                
                <!-- Statistiques alignées -->
                <div class="flex items-center justify-between mb-4">
                  <!-- Rating à gauche -->
                  <div class="flex items-center gap-1.5">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-yellow-400" fill="currentColor" viewBox="0 0 20 20">
                      <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                    </svg>
                    <span 
                      :class="[
                        'text-sm font-semibold',
                        item.rarity === 'Legendary' ? 'text-yellow-400' :
                        item.rarity === 'Epic' ? 'text-purple-400' :
                        item.rarity === 'Rare' ? 'text-blue-400' :
                        'text-white'
                      ]"
                    >
                      {{ item.rarity === 'Legendary' ? '96%' : item.rarity === 'Epic' ? '85%' : item.rarity === 'Rare' ? '72%' : '50%' }}
                    </span>
                  </div>
                  
                  <!-- Prix à droite avec icône très petite -->
                  <div class="flex items-center gap-1">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-2 w-2 text-cyan-400" fill="currentColor" viewBox="0 0 24 24">
                      <circle cx="12" cy="12" r="9" stroke="currentColor" stroke-width="1.5" fill="currentColor" opacity="0.2"/>
                      <circle cx="12" cy="12" r="5" fill="currentColor"/>
                    </svg>
                    <span class="text-base font-bold text-cyan-400">{{ item.price }}</span>
                  </div>
                </div>
                
                <!-- Bouton gradient -->
                <button
                  @click.stop="openItemDetails(item)"
                  class="w-full py-3 rounded-xl bg-gradient-to-r from-emerald-500 to-cyan-500 hover:from-emerald-400 hover:to-cyan-400 text-white text-sm font-semibold transition-all duration-200 flex items-center justify-center gap-2 shadow-lg hover:shadow-xl hover:shadow-emerald-500/30"
                >
                  <span>Voir détails</span>
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M9 5l7 7-7 7" />
                  </svg>
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal de détails style Magic: The Gathering -->
    <div
      v-if="selectedItem"
      @click.self="closeItemDetails"
      class="fixed inset-0 z-50 flex items-center justify-center bg-black/90 p-4 overflow-y-auto"
    >
      <div class="relative w-full max-w-6xl rounded-lg bg-zinc-900/95 shadow-2xl overflow-hidden min-h-[600px]">
        <!-- Fond avec image floutée -->
        <div 
          v-if="selectedItem.imageUrl"
          class="absolute inset-0 opacity-10 blur-3xl scale-150"
          :style="{ backgroundImage: `url(${selectedItem.imageUrl})`, backgroundSize: 'cover', backgroundPosition: 'center' }"
        ></div>
        
        <!-- Bouton fermer -->
        <button
          @click="closeItemDetails"
          class="absolute top-4 right-4 w-10 h-10 rounded-full bg-zinc-800/90 backdrop-blur-sm text-zinc-300 hover:text-white hover:bg-zinc-700 transition-all flex items-center justify-center z-30 border border-zinc-700/50"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>

        <!-- Contenu principal en deux colonnes -->
        <div class="relative z-10 flex flex-col lg:flex-row min-h-[600px]">
          <!-- Colonne gauche : Grande image de la carte -->
          <div class="lg:w-2/5 p-8 lg:p-12 flex items-center justify-center bg-zinc-950/50">
            <div class="relative w-full max-w-sm">
              <img
                v-if="selectedItem.imageUrl"
                :src="selectedItem.imageUrl"
                :alt="selectedItem.name"
                class="w-full h-auto object-contain rounded-lg shadow-2xl"
                loading="eager"
                decoding="async"
                style="filter: none;"
              />
              <div v-else class="w-full aspect-[3/4] bg-zinc-800 rounded-lg flex items-center justify-center">
                <div class="text-8xl opacity-30">{{ selectedItem.icon }}</div>
              </div>
            </div>
          </div>

          <!-- Colonne droite : Informations détaillées -->
          <div class="lg:w-3/5 p-8 lg:p-12 bg-zinc-900/80 backdrop-blur-sm">
            <!-- Titre principal -->
            <h1 class="text-4xl lg:text-5xl font-bold text-white mb-6 leading-tight">
              {{ selectedItem.name }}
            </h1>

            <!-- Badge Possédé -->
            <div v-if="selectedItem.isOwned" class="mb-6">
              <span class="inline-block px-4 py-2 rounded-lg text-sm font-bold bg-gradient-to-r from-emerald-500 to-teal-500 text-white shadow-lg shadow-emerald-500/50">
                ✓ Possédé
              </span>
            </div>

            <!-- Type et Rareté -->
            <div class="mb-8 space-y-3">
              <div>
                <p class="text-xs uppercase tracking-wider text-zinc-400 mb-1">TYPE</p>
                <p class="text-lg font-semibold text-white">{{ selectedItem.itemType }}</p>
              </div>
              <div>
                <p class="text-xs uppercase tracking-wider text-zinc-400 mb-1">RARETÉ</p>
                <p 
                  :class="[
                    'text-lg font-bold',
                    selectedItem.rarity === 'Legendary' ? 'text-yellow-400' :
                    selectedItem.rarity === 'Epic' ? 'text-purple-400' :
                    selectedItem.rarity === 'Rare' ? 'text-blue-400' :
                    'text-zinc-300'
                  ]"
                >
                  {{ selectedItem.rarity }}
                </p>
              </div>
            </div>

            <!-- Prix -->
            <div class="mb-8">
              <p class="text-xs uppercase tracking-wider text-zinc-400 mb-2">PRIX</p>
              <div class="flex items-center gap-2">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-3 w-3 text-cyan-400" fill="currentColor" viewBox="0 0 24 24">
                  <circle cx="12" cy="12" r="9" stroke="currentColor" stroke-width="1.5" fill="currentColor" opacity="0.2"/>
                  <circle cx="12" cy="12" r="5" fill="currentColor"/>
                </svg>
                <span class="text-4xl font-bold text-cyan-400">{{ selectedItem.price }}</span>
                <span class="text-zinc-400 text-sm ml-2">Babayuni</span>
              </div>
            </div>

            <!-- Description -->
            <div class="mb-8">
              <p class="text-xs uppercase tracking-wider text-zinc-400 mb-3">DESCRIPTION</p>
              <div class="bg-zinc-800/60 border border-zinc-700/50 rounded-lg p-5">
                <p class="text-base text-zinc-200 leading-relaxed">{{ selectedItem.description }}</p>
              </div>
            </div>

            <!-- Statut -->
            <div class="mb-8">
              <p class="text-xs uppercase tracking-wider text-zinc-400 mb-2">STATUT</p>
              <p :class="selectedItem.isOwned ? 'text-emerald-400 font-semibold text-lg' : 'text-zinc-300 text-lg'">
                {{ selectedItem.isOwned ? 'Possédé' : 'Non possédé' }}
              </p>
            </div>

            <!-- Actions -->
            <div class="flex flex-col sm:flex-row gap-3 mt-8 pt-8 border-t border-zinc-700/50">
              <button
                v-if="!selectedItem.isOwned"
                @click="handlePurchase(selectedItem)"
                :disabled="!userStore.isLoggedIn || userStore.coins < selectedItem.price || isPurchasing"
                class="flex-1 px-6 py-4 rounded-lg bg-gradient-to-r from-cyan-500 to-purple-500 text-white text-base font-bold hover:from-cyan-400 hover:to-purple-400 transition-all duration-300 shadow-lg shadow-cyan-500/30 hover:shadow-xl hover:shadow-cyan-500/50 disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
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
                class="flex-1 px-6 py-4 rounded-lg bg-gradient-to-r from-emerald-500 to-teal-500 text-white text-base font-bold hover:from-emerald-400 hover:to-teal-400 transition-all duration-300 flex items-center justify-center gap-2 shadow-lg shadow-emerald-500/30"
              >
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                </svg>
                Voir dans ma Collection
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
import { getAllItems, purchaseItem, getUserCoins, addCoins } from '@/services/shopApi'
import type { Item } from '@/types'
import IconShop from '@/components/icons/IconShop.vue'

const userStore = useUserStore()
const uiStore = useUiStore()

const items = ref<Item[]>([])
const isLoading = ref(false)
const error = ref<string | null>(null)
const isPurchasing = ref(false)
const isPurchasingCoins = ref(false)
const selectedType = ref<string>('Tous')
const selectedItem = ref<Item | null>(null)

// Effets 3D pour les cartes
const cardRotations = ref<Record<number, { x: number; y: number; z: number }>>({})
const cardHovered = ref<Record<number, boolean>>({})

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
    error.value = 'Tu n\'as pas assez de Babayuni pour cet item'
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

async function purchaseCoins(amount: number) {
  if (!userStore.userId) {
    error.value = 'Connecte-toi pour acheter des Babayuni'
    return
  }

  // Déterminer le prix selon le pack
  let price = 0
  if (amount === 500) {
    price = 0 // Gratuit
  } else if (amount === 1000) {
    price = 100
  } else if (amount === 2500) {
    price = 200
  }

  // Vérifier si l'utilisateur a assez de coins (sauf pour le pack gratuit)
  if (price > 0 && userStore.coins < price) {
    error.value = `Vous n'avez pas assez de Babayuni. Il vous faut ${price} Babayuni.`
    setTimeout(() => { error.value = null }, 5000)
    return
  }

  isPurchasingCoins.value = true
  error.value = null
  
  try {
    // Si c'est payant, déduire le prix d'abord
    if (price > 0) {
      // Déduire le prix des coins de l'utilisateur
      const deductResult = await addCoins(userStore.userId, -price)
      userStore.coins = deductResult.coins
    }
    
    // Ajouter les coins achetés
    const result = await addCoins(userStore.userId, amount)
    userStore.coins = result.coins
    
    // Message de succès temporaire
    const successMessage = `✅ ${amount} Babayuni ${price === 0 ? 'ajoutés' : 'achetés'} avec succès !`
    error.value = successMessage
    setTimeout(() => {
      if (error.value === successMessage) {
        error.value = null
      }
    }, 3000)
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Erreur lors de l\'achat de Babayuni'
    setTimeout(() => { error.value = null }, 5000)
  } finally {
    isPurchasingCoins.value = false
  }
}

// Fonctions pour les effets 3D
function handleCardEnter(event: MouseEvent, itemId: number) {
  cardHovered.value[itemId] = true
  if (!cardRotations.value[itemId]) {
    cardRotations.value[itemId] = { x: 0, y: 0, z: 0 }
  }
  cardRotations.value[itemId].z = 20
}

function handleCardLeave(event: MouseEvent, itemId: number) {
  cardHovered.value[itemId] = false
  if (cardRotations.value[itemId]) {
    cardRotations.value[itemId] = { x: 0, y: 0, z: 0 }
  }
}

function handleCardMove(event: MouseEvent, itemId: number) {
  if (!cardHovered.value[itemId]) return
  
  const card = event.currentTarget as HTMLElement
  const rect = card.getBoundingClientRect()
  const centerX = rect.left + rect.width / 2
  const centerY = rect.top + rect.height / 2
  
  const mouseX = event.clientX - centerX
  const mouseY = event.clientY - centerY
  
  const rotateX = (mouseY / rect.height) * -15 // Rotation verticale limitée à 15 degrés
  const rotateY = (mouseX / rect.width) * 15 // Rotation horizontale limitée à 15 degrés
  
  if (!cardRotations.value[itemId]) {
    cardRotations.value[itemId] = { x: 0, y: 0, z: 0 }
  }
  cardRotations.value[itemId].x = rotateX
  cardRotations.value[itemId].y = rotateY
  cardRotations.value[itemId].z = 20
}

onMounted(() => {
  loadItems()
})
</script>

<style scoped>
.transform-style-3d {
  transform-style: preserve-3d;
  transition: transform 0.3s cubic-bezier(0.23, 1, 0.32, 1);
  will-change: transform;
}

.group:hover {
  animation: cardFloat 2s ease-in-out infinite;
}

@keyframes cardFloat {
  0%, 100% {
    transform: translateY(0px);
  }
  50% {
    transform: translateY(-5px);
  }
}

/* Amélioration de l'image avec effet 3D */
.group:hover img {
  transform: scale(1.1) translateZ(10px);
  transition: transform 0.5s cubic-bezier(0.23, 1, 0.32, 1);
}

/* Effet de brillance sur hover */
.group::before {
  content: '';
  position: absolute;
  top: 0;
  left: -100%;
  width: 100%;
  height: 100%;
  background: linear-gradient(
    90deg,
    transparent,
    rgba(255, 255, 255, 0.1),
    transparent
  );
  transition: left 0.5s;
  z-index: 1;
  pointer-events: none;
}

.group:hover::before {
  left: 100%;
}

/* Animation d'entrée pour les cartes */
@keyframes cardEntrance {
  from {
    opacity: 0;
    transform: translateY(20px) rotateX(-10deg);
  }
  to {
    opacity: 1;
    transform: translateY(0) rotateX(0deg);
  }
}

.grid > div {
  animation: cardEntrance 0.6s ease-out backwards;
}

.grid > div:nth-child(1) { animation-delay: 0.1s; }
.grid > div:nth-child(2) { animation-delay: 0.2s; }
.grid > div:nth-child(3) { animation-delay: 0.3s; }
.grid > div:nth-child(4) { animation-delay: 0.4s; }
.grid > div:nth-child(5) { animation-delay: 0.5s; }
.grid > div:nth-child(6) { animation-delay: 0.6s; }
.grid > div:nth-child(7) { animation-delay: 0.7s; }
.grid > div:nth-child(8) { animation-delay: 0.8s; }
</style>

