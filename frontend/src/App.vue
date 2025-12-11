<template>
  <div class="min-h-screen w-full bg-slate-950 text-slate-50 flex">
    <!-- Sidebar verticale -->
    <aside
      class="w-56 lg:w-64 bg-slate-900 border-r border-slate-800 flex flex-col py-6 px-4 lg:px-6"
    >
      <!-- Logo / icône prisonnier -->
      <div class="flex items-center gap-3 mb-8 group cursor-pointer" @click="ui.goToHome()">
        <div
          class="h-12 w-12 rounded-2xl bg-gradient-to-br from-orange-500 to-orange-600 flex items-center justify-center text-white font-bold text-lg shadow-lg shadow-orange-500/30 transition-all duration-300 group-hover:scale-110 group-hover:rotate-3 group-hover:shadow-xl group-hover:shadow-orange-500/40"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
            <path stroke-linecap="round" stroke-linejoin="round" d="M13.828 10.172a4 4 0 00-5.656 0l-4 4a4 4 0 105.656 5.656l1.102-1.101m-.758-4.899a4 4 0 005.656 0l4-4a4 4 0 00-5.656-5.656l-1.1 1.1" />
          </svg>
        </div>
        <div class="flex flex-col">
          <span class="text-sm font-semibold text-slate-50 group-hover:text-orange-400 transition-colors duration-300">Prison Break</span>
          <span class="text-[11px] text-slate-400 group-hover:text-slate-300 transition-colors duration-300">Évasion de cellule</span>
        </div>
      </div>

      <!-- ========================
           MENU
      ============================ -->
      <nav class="flex flex-col gap-1.5 text-sm flex-1">

        <!-- Accueil -->
        <button
          class="nav-item group relative flex items-center gap-3 px-3 py-2.5 rounded-xl text-left transition-all duration-300 ease-out"
          :class="ui.currentScreen === 'home'
            ? 'bg-gradient-to-r from-orange-500 to-orange-600 text-white shadow-lg shadow-orange-500/30 scale-[1.02]'
            : 'text-slate-300 hover:bg-slate-800/80 hover:text-slate-50 hover:translate-x-1'"
          @click="ui.goToHome()"
        >
          <div class="nav-icon-wrapper" :class="ui.currentScreen === 'home' ? 'nav-icon-active' : ''">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
            </svg>
          </div>
          <span class="font-medium">Accueil</span>
          <div v-if="ui.currentScreen === 'home'" class="absolute right-2 w-1.5 h-1.5 rounded-full bg-white animate-pulse"></div>
        </button>

        <!-- Niveaux -->
        <button
          class="nav-item group relative flex items-center gap-3 px-3 py-2.5 rounded-xl text-left transition-all duration-300 ease-out"
          :class="ui.currentScreen === 'levels'
            ? 'bg-gradient-to-r from-blue-500 to-blue-600 text-white shadow-lg shadow-blue-500/30 scale-[1.02]'
            : 'text-slate-300 hover:bg-slate-800/80 hover:text-slate-50 hover:translate-x-1'"
          @click="ui.goToLevels()"
        >
          <div class="nav-icon-wrapper" :class="ui.currentScreen === 'levels' ? 'nav-icon-active' : ''">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01" />
            </svg>
          </div>
          <span class="font-medium">Niveaux</span>
          <div v-if="ui.currentScreen === 'levels'" class="absolute right-2 w-1.5 h-1.5 rounded-full bg-white animate-pulse"></div>
        </button>

        <!-- Classement -->
        <button
          class="nav-item group relative flex items-center gap-3 px-3 py-2.5 rounded-xl text-left transition-all duration-300 ease-out"
          :class="ui.currentScreen === 'leaderboard'
            ? 'bg-gradient-to-r from-purple-500 to-purple-600 text-white shadow-lg shadow-purple-500/30 scale-[1.02]'
            : 'text-slate-300 hover:bg-slate-800/80 hover:text-slate-50 hover:translate-x-1'"
          @click="ui.goToStats()"
        >
          <div class="nav-icon-wrapper" :class="ui.currentScreen === 'leaderboard' ? 'nav-icon-active' : ''">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z" />
            </svg>
          </div>
          <span class="font-medium">Classement</span>
          <div v-if="ui.currentScreen === 'leaderboard'" class="absolute right-2 w-1.5 h-1.5 rounded-full bg-white animate-pulse"></div>
        </button>

        <!-- Statistiques -->
        <button
          class="nav-item group relative flex items-center gap-3 px-3 py-2.5 rounded-xl text-left transition-all duration-300 ease-out"
          :class="ui.currentScreen === 'stats'
            ? 'bg-gradient-to-r from-emerald-500 to-emerald-600 text-white shadow-lg shadow-emerald-500/30 scale-[1.02]'
            : 'text-slate-300 hover:bg-slate-800/80 hover:text-slate-50 hover:translate-x-1'"
          @click="ui.goToStats()"
        >
          <div class="nav-icon-wrapper" :class="ui.currentScreen === 'stats' ? 'nav-icon-active' : ''">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
            </svg>
          </div>
          <span class="font-medium">Statistiques</span>
          <div v-if="ui.currentScreen === 'stats'" class="absolute right-2 w-1.5 h-1.5 rounded-full bg-white animate-pulse"></div>
        </button>

        <!-- Jeux -->
        <button
          class="nav-item group relative flex items-center gap-3 px-3 py-2.5 rounded-xl text-left transition-all duration-300 ease-out"
          :class="ui.currentScreen === 'games'
            ? 'bg-gradient-to-r from-cyan-500 to-cyan-600 text-white shadow-lg shadow-cyan-500/30 scale-[1.02]'
            : 'text-slate-300 hover:bg-slate-800/80 hover:text-slate-50 hover:translate-x-1'"
          @click="ui.goToGames()"
        >
          <div class="nav-icon-wrapper" :class="ui.currentScreen === 'games' ? 'nav-icon-active' : ''">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M14.751 9.75l3.501 3.75m0 0l3.499-3.75M18.252 13.5H21m-2.25 0v6.75m-9-9.75H5.25A2.25 2.25 0 003 12.75v6.75A2.25 2.25 0 005.25 22h13.5A2.25 2.25 0 0021 19.5v-6.75a2.25 2.25 0 00-2.25-2.25h-4.752m-9 0H3m2.25 0h4.752M9.75 3v3m0 0v3m0-3h3m-3 0H6.75" />
            </svg>
          </div>
          <span class="font-medium">Jeux</span>
          <div v-if="ui.currentScreen === 'games'" class="absolute right-2 w-1.5 h-1.5 rounded-full bg-white animate-pulse"></div>
        </button>

      </nav>

      <!-- Pied de page -->
      <div class="mt-6 text-[11px] text-slate-500">
        <p>Session Hashi Prison Break</p>
      </div>
    </aside>

    <!-- ========================
         CONTENU PRINCIPAL
    ============================ -->
    <div class="flex-1 flex flex-col">

      <header class="h-14 border-b border-slate-800/50 bg-slate-950/50 backdrop-blur-sm px-4 lg:px-8 flex items-center justify-between shadow-sm">
        <div class="flex items-center gap-2">
          <div class="h-2 w-2 rounded-full bg-emerald-500 animate-pulse"></div>
          <span class="text-xs text-slate-400 font-medium">En ligne</span>
        </div>
        <div class="flex items-center gap-3">
          <!-- Menu utilisateur si connecté -->
          <div v-if="userStore.isLoggedIn" class="relative">
            <div class="flex items-center gap-2 px-3 py-1.5 rounded-lg hover:bg-slate-800/60 transition-colors duration-200 cursor-pointer" @click="toggleUserMenu">
              <div class="h-8 w-8 rounded-full bg-gradient-to-br from-orange-500 to-orange-600 flex items-center justify-center text-white font-semibold text-sm">
                {{ userStore.user?.name?.charAt(0).toUpperCase() || 'U' }}
              </div>
              <span class="text-sm text-slate-300 font-medium hidden lg:inline">{{ userStore.user?.name }}</span>
              <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-slate-400 transition-transform duration-200" :class="{ 'rotate-180': isUserMenuOpen }" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M19 9l-7 7-7-7" />
              </svg>
            </div>
            
            <!-- Menu déroulant -->
            <div v-if="isUserMenuOpen" class="absolute right-0 mt-2 w-56 bg-slate-800 rounded-xl shadow-2xl border border-slate-700 overflow-hidden z-50 animate-fadeIn">
              <div class="px-4 py-3 border-b border-slate-700">
                <p class="text-sm font-semibold text-slate-50">{{ userStore.user?.name }}</p>
                <p class="text-xs text-slate-400 mt-0.5">{{ userStore.user?.email }}</p>
              </div>
              
              <div class="py-1">
                <button
                  @click="handleLogout"
                  class="w-full flex items-center gap-3 px-4 py-2.5 text-sm text-red-400 hover:bg-red-500/10 hover:text-red-300 transition-colors duration-200 group"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 group-hover:rotate-12 transition-transform duration-200" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                  </svg>
                  <span class="font-medium">Déconnexion</span>
                </button>
              </div>
            </div>
          </div>
          
          <!-- Bouton paramètres si non connecté -->
          <button v-else class="p-2 rounded-lg hover:bg-slate-800/60 transition-colors duration-200 group" @click="ui.openUserModal">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-slate-400 group-hover:text-slate-200 transition-colors" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
              <path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
            </svg>
          </button>
        </div>
      </header>

      <main class="flex-1 flex items-stretch justify-center">
        <transition name="fade" mode="out-in">
          <component :is="currentComponent" :key="ui.currentScreen" />
        </transition>
      </main>

      <!-- Modales globales -->
      <UserRegisterModal v-if="ui.isUserModalOpen" />
      <TutorialModal v-if="ui.isTutorialModalOpen" />

    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, onUnmounted } from 'vue';
import { useUiStore } from '@/stores/ui';
import { useUserStore } from '@/stores/user';
import { useStatsStore } from '@/stores/stats';

import HomeScreen from '@/components/HomeScreen.vue';
import LevelSelectScreen from '@/components/LevelSelectScreen.vue';
import GameScreen from '@/components/GameScreen.vue';
import StatsScreen from '@/components/StatsScreen.vue';
import GamesScreen from '@/components/GamesScreen.vue';
import TicTacToeScreen from '@/components/TicTacToeScreen.vue';
import UserRegisterModal from '@/components/UserRegisterModal.vue';
import TutorialModal from '@/components/TutorialModal.vue';

const ui = useUiStore();
const userStore = useUserStore();
const statsStore = useStatsStore();

// État du menu utilisateur
const isUserMenuOpen = ref(false);

// Toggle du menu utilisateur
function toggleUserMenu() {
  isUserMenuOpen.value = !isUserMenuOpen.value;
}

// Fermer le menu si on clique en dehors
function handleClickOutside(event: MouseEvent) {
  const target = event.target as HTMLElement;
  if (!target.closest('.relative')) {
    isUserMenuOpen.value = false;
  }
}

// Gérer la déconnexion
function handleLogout() {
  userStore.clearUser();
  statsStore.resetStats();
  isUserMenuOpen.value = false;
  ui.goToHome();
}

// Écouter les clics pour fermer le menu
onMounted(() => {
  document.addEventListener('click', handleClickOutside);
});

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside);
});

// Charger l'utilisateur si stocké en local et récupérer ses statistiques
onMounted(async () => {
  userStore.loadFromLocalStorage();
  
  // Si un utilisateur est déjà connecté, charger ses statistiques automatiquement
  if (userStore.user?.email) {
    try {
      await statsStore.loadUserStatsByEmail(userStore.user.email);
    } catch (err) {
      // Ignorer les erreurs si l'utilisateur n'a pas encore de statistiques
      console.log('Aucune statistique disponible pour cet utilisateur');
    }
  }
});

// Sélection dynamique de l'écran actif
const currentComponent = computed(() => {
  switch (ui.currentScreen) {
    case 'home': return HomeScreen;
    case 'levels': return LevelSelectScreen;
    case 'game': return GameScreen;
    case 'stats': return StatsScreen;
    case 'games': return GamesScreen;
    case 'ticTacToe': return TicTacToeScreen;
    case 'leaderboard': return HomeScreen; // temporaire
    default: return HomeScreen;
  }
});
</script>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 180ms ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

/* Navigation item animations */
.nav-item {
  position: relative;
  overflow: hidden;
}

.nav-item::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  height: 100%;
  width: 3px;
  background: linear-gradient(to bottom, transparent, currentColor, transparent);
  transform: translateX(-100%);
  transition: transform 0.3s ease;
}

.nav-item:hover::before,
.nav-item[class*='bg-gradient']::before {
  transform: translateX(0);
}

.nav-icon-wrapper {
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.nav-item:hover .nav-icon-wrapper {
  transform: scale(1.1) rotate(5deg);
}

.nav-icon-active {
  transform: scale(1.15);
  animation: iconPulse 2s ease-in-out infinite;
}

@keyframes iconPulse {
  0%, 100% {
    transform: scale(1.15);
  }
  50% {
    transform: scale(1.2);
  }
}

/* Smooth transitions for active states */
.nav-item[class*='bg-gradient'] {
  animation: slideIn 0.3s ease-out;
}

@keyframes slideIn {
  from {
    opacity: 0.8;
    transform: translateX(-5px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

/* Menu utilisateur animation */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(-10px) scale(0.95);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

.animate-fadeIn {
  animation: fadeIn 0.2s ease-out;
}
</style>
