<template>
  <div class="min-h-screen w-full bg-slate-950 text-slate-50 flex">
    <!-- Sidebar verticale -->
    <aside
      class="w-56 lg:w-64 bg-slate-900 border-r border-slate-800 flex flex-col py-6 px-4 lg:px-6"
    >
      <!-- Logo / icône prisonnier -->
      <div class="flex items-center gap-3 mb-8">
        <div
          class="h-10 w-10 rounded-2xl bg-orange-500 flex items-center justify-center text-slate-900 font-bold text-lg"
        >
          PB
        </div>
        <div class="flex flex-col">
          <span class="text-sm font-semibold">Prison Break</span>
          <span class="text-[11px] text-slate-400">Évasion de cellule</span>
        </div>
      </div>

      <!-- ========================
           MENU
      ============================ -->
      <nav class="flex flex-col gap-1 text-sm flex-1">

        <!-- Accueil -->
        <button
          class="flex items-center gap-2 px-3 py-2 rounded-lg text-left transition-colors"
          :class="ui.currentScreen === 'home'
            ? 'bg-slate-800 text-slate-50'
            : 'text-slate-300 hover:bg-slate-800/60 hover:text-slate-50'"
          @click="ui.goToHome()"
        >
          <span class="inline-flex h-5 w-5 rounded-full bg-slate-800"></span>
          <span>Accueil</span>
        </button>

        <!-- Niveaux -->
        <button
          class="flex items-center gap-2 px-3 py-2 rounded-lg text-left transition-colors"
          :class="ui.currentScreen === 'levels'
            ? 'bg-slate-800 text-slate-50'
            : 'text-slate-300 hover:bg-slate-800/60 hover:text-slate-50'"
          @click="ui.goToLevels()"
        >
          <span class="inline-flex h-5 w-5 rounded-full bg-slate-800"></span>
          <span>Niveaux</span>
        </button>

        <!-- Classement (désactivé) -->
        <button
          class="flex items-center gap-2 px-3 py-2 rounded-lg text-left
                 text-slate-600 cursor-not-allowed opacity-50"
          @click="() => {}"
          disabled
        >
          <span class="inline-flex h-5 w-5 rounded-full bg-slate-800/40"></span>
          <span>Classement</span>
        </button>

        <!-- Statistiques (désactivé) -->
        <button
          class="flex items-center gap-2 px-3 py-2 rounded-lg text-left
                 text-slate-600 cursor-not-allowed opacity-50"
          @click="() => {}"
          disabled
        >
          <span class="inline-flex h-5 w-5 rounded-full bg-slate-800/40"></span>
          <span>Statistiques</span>
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

      <header class="h-12 border-b border-slate-800 px-4 lg:px-8 flex items-center justify-end">
        <!-- zone pour futur bouton profil -->
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
import { computed, onMounted } from 'vue';
import { useUiStore } from '@/stores/ui';
import { useUserStore } from '@/stores/user';

import HomeScreen from '@/components/HomeScreen.vue';
import LevelSelectScreen from '@/components/LevelSelectScreen.vue';
import GameScreen from '@/components/GameScreen.vue';
import UserRegisterModal from '@/components/UserRegisterModal.vue';
import TutorialModal from '@/components/TutorialModal.vue';

const ui = useUiStore();
const userStore = useUserStore();

// Charger l'utilisateur si stocké en local
onMounted(() => {
  userStore.loadFromLocalStorage();
});

// Sélection dynamique de l'écran actif
const currentComponent = computed(() => {
  switch (ui.currentScreen) {
    case 'home': return HomeScreen;
    case 'levels': return LevelSelectScreen;
    case 'game': return GameScreen;
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
</style>
