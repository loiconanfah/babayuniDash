<template>
  <section class="w-full h-full flex items-center justify-center px-4 lg:px-10 py-8 relative overflow-hidden">
    <!-- Effet de barreaux en arrière-plan -->
    <div class="absolute inset-0 overflow-hidden pointer-events-none opacity-5">
      <div class="absolute inset-0" style="background-image: repeating-linear-gradient(90deg, transparent, transparent 60px, rgba(251, 146, 60, 0.2) 60px, rgba(251, 146, 60, 0.2) 62px);"></div>
      <div class="absolute inset-0" style="background-image: repeating-linear-gradient(0deg, transparent, transparent 60px, rgba(251, 146, 60, 0.2) 60px, rgba(251, 146, 60, 0.2) 62px);"></div>
    </div>

    <!-- Effet de particules en arrière-plan -->
    <div class="absolute inset-0 overflow-hidden pointer-events-none">
      <div 
        v-for="i in 15" 
        :key="i" 
        class="particle"
        :style="{
          left: `${Math.random() * 100}%`,
          animationDelay: `${Math.random() * 5}s`,
          animationDuration: `${3 + Math.random() * 4}s`
        }"
      ></div>
    </div>

    <!-- Icônes de barreaux décoratives -->
    <div class="absolute top-10 right-10 opacity-10 pointer-events-none">
      <IconBars class="w-32 h-32 text-orange-500" />
    </div>
    <div class="absolute bottom-10 left-10 opacity-10 pointer-events-none">
      <IconBars class="w-24 h-24 text-orange-500" />
    </div>

    <div class="w-full max-w-5xl relative z-10">
      <!-- Titre + intro -->
      <header class="mb-10 animate-fade-in">
        <div class="flex items-center gap-3 mb-3">
          <div class="h-1 w-12 bg-gradient-to-r from-orange-500 to-orange-300 rounded-full"></div>
          <p class="text-sm uppercase tracking-[0.3em] text-orange-300 font-semibold">
            Prison Break
          </p>
        </div>
        <h1 class="text-4xl sm:text-5xl lg:text-6xl font-extrabold mb-4 bg-gradient-to-r from-slate-50 via-slate-100 to-slate-200 bg-clip-text text-transparent drop-shadow-lg">
          {{ greeting }}
        </h1>
        <p class="text-base sm:text-lg text-slate-300 max-w-2xl leading-relaxed">
          Tu es enfermé dans une prison haute sécurité. Résous les puzzles des verrous pour
          connecter toutes les clés et t'échapper avant la prochaine inspection&nbsp;!
        </p>
      </header>

      <!-- Statut du prisonnier -->
      <section class="mb-8 animate-slide-up">
        <div class="flex items-center gap-4 p-4 rounded-2xl bg-slate-900/60 backdrop-blur-sm border border-slate-800/50 shadow-xl relative overflow-hidden">
          <!-- Effet de barreaux subtil -->
          <div class="absolute right-0 top-0 bottom-0 w-px bg-gradient-to-b from-transparent via-orange-500/20 to-transparent"></div>
          
          <div class="flex items-center gap-3 flex-1">
            <div class="relative">
              <div 
                class="h-3 w-3 rounded-full animate-pulse"
                :class="isLoggedIn ? 'bg-green-500 shadow-lg shadow-green-500/50' : 'bg-orange-500 shadow-lg shadow-orange-500/50'"
              ></div>
              <div 
                class="absolute inset-0 rounded-full animate-ping"
                :class="isLoggedIn ? 'bg-green-500' : 'bg-orange-500'"
                style="animation-duration: 2s; opacity: 0.3;"
              ></div>
            </div>
            <span class="text-sm font-medium text-slate-200">
              {{ isLoggedIn ? 'Identité confirmée' : 'Non enregistré' }}
            </span>
          </div>
          <span
            class="px-4 py-2 rounded-full text-xs font-bold tracking-wider transition-all duration-300 transform hover:scale-105"
            :class="
              isLoggedIn
                ? 'bg-gradient-to-r from-green-500 to-green-400 text-slate-950 shadow-lg shadow-green-500/30'
                : 'bg-gradient-to-r from-orange-600 to-orange-500 text-orange-100 shadow-lg shadow-orange-500/30'
            "
          >
            {{ isLoggedIn ? 'SESSION ACTIVE' : 'SESSION INVITÉE' }}
          </span>
        </div>
      </section>

      <!-- Cartes visuelles -->
      <section
        class="grid grid-cols-1 md:grid-cols-2 gap-6 md:gap-8 mb-10"
      >
        <!-- Carte puzzle / verrous -->
        <div
          class="group rounded-3xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 shadow-2xl p-6 flex flex-col justify-between backdrop-blur-sm transition-all duration-500 hover:shadow-orange-500/20 hover:border-orange-500/30 hover:scale-[1.02] animate-slide-up"
          style="animation-delay: 0.1s"
        >
          <div class="mb-4">
            <div class="flex items-center gap-2 mb-2">
              <div class="h-1 w-8 bg-gradient-to-r from-orange-500 to-orange-300 rounded-full"></div>
              <p class="text-xs uppercase tracking-[0.2em] text-orange-300 font-semibold">
                Plan de la cellule
              </p>
            </div>
            <p class="text-sm text-slate-200 leading-relaxed">
              Vue schématique des verrous à relier pour préparer ton évasion.
            </p>
          </div>

          <!-- Faux visuel de puzzle/verrous amélioré avec barreaux -->
          <div
            class="mt-4 flex-1 rounded-2xl bg-gradient-to-br from-slate-800/80 to-slate-900/80 border border-slate-700/50 grid grid-cols-3 grid-rows-3 place-items-center gap-3 px-5 py-4 shadow-inner group-hover:border-orange-500/30 transition-colors duration-300 relative overflow-hidden"
          >
            <!-- Effet de barreaux en arrière-plan -->
            <div class="absolute inset-0 opacity-5 pointer-events-none">
              <div class="absolute inset-0" style="background-image: repeating-linear-gradient(0deg, transparent, transparent 20px, rgba(251, 146, 60, 0.3) 20px, rgba(251, 146, 60, 0.3) 22px), repeating-linear-gradient(90deg, transparent, transparent 20px, rgba(251, 146, 60, 0.3) 20px, rgba(251, 146, 60, 0.3) 22px);"></div>
            </div>
            
            <!-- Verrous/îles -->
            <div class="relative z-10 h-10 w-10 rounded-full bg-gradient-to-br from-slate-700 to-slate-800 border-2 border-orange-500/50 flex items-center justify-center text-xs font-bold text-orange-300 shadow-lg shadow-orange-500/20 group-hover:scale-110 transition-transform">
              1
            </div>
            <div class="relative z-10 h-10 w-10 rounded-full bg-gradient-to-br from-slate-700 to-slate-800 border-2 border-orange-500/50 flex items-center justify-center text-xs font-bold text-orange-300 shadow-lg shadow-orange-500/20 group-hover:scale-110 transition-transform">
              2
            </div>
            <div class="relative z-10 h-10 w-10 rounded-full bg-gradient-to-br from-slate-700 to-slate-800 border-2 border-orange-500/50 flex items-center justify-center text-xs font-bold text-orange-300 shadow-lg shadow-orange-500/20 group-hover:scale-110 transition-transform">
              2
            </div>
            <div class="relative z-10 col-span-3 h-1.5 w-4/5 bg-gradient-to-r from-transparent via-orange-500/60 to-transparent rounded-full shadow-lg shadow-orange-500/30"></div>
            <div class="relative z-10 h-10 w-10 rounded-full bg-gradient-to-br from-slate-700 to-slate-800 border-2 border-orange-500/50 flex items-center justify-center text-xs font-bold text-orange-300 shadow-lg shadow-orange-500/20 group-hover:scale-110 transition-transform">
              3
            </div>
            <div class="relative z-10 h-10 w-10 rounded-full bg-gradient-to-br from-slate-700 to-slate-800 border-2 border-orange-500/50 flex items-center justify-center text-xs font-bold text-orange-300 shadow-lg shadow-orange-500/20 group-hover:scale-110 transition-transform">
              4
            </div>
            <div class="relative z-10 h-10 w-10 rounded-full bg-gradient-to-br from-slate-700 to-slate-800 border-2 border-orange-500/50 flex items-center justify-center text-xs font-bold text-orange-300 shadow-lg shadow-orange-500/20 group-hover:scale-110 transition-transform">
              1
            </div>
          </div>
        </div>

        <!-- Carte prisonnier / mugshot -->
        <div
          class="group rounded-3xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 shadow-2xl p-6 flex flex-col justify-between backdrop-blur-sm transition-all duration-500 hover:shadow-orange-500/20 hover:border-orange-500/30 hover:scale-[1.02] animate-slide-up"
          style="animation-delay: 0.2s"
        >
          <div class="mb-4">
            <div class="flex items-center gap-2 mb-2">
              <div class="h-1 w-8 bg-gradient-to-r from-orange-500 to-orange-300 rounded-full"></div>
              <p class="text-xs uppercase tracking-[0.2em] text-orange-300 font-semibold">
                Dossier du prisonnier
              </p>
            </div>
            <p class="text-sm text-slate-200 leading-relaxed">
              Numéro d'identification et identité utilisée pour le classement et les statistiques.
            </p>
          </div>

          <!-- Contenu différent selon l'état connexion -->
          <div
            class="mt-4 flex-1 rounded-2xl bg-gradient-to-br from-slate-800/80 to-slate-900/80 border border-slate-700/50 flex items-center justify-center px-5 py-4 shadow-inner group-hover:border-orange-500/30 transition-colors duration-300"
          >
            <!-- AVANT enregistrement : prisonnier générique fâché -->
            <div
              v-if="!isLoggedIn"
              class="flex flex-col items-center gap-3 text-xs text-slate-200"
            >
              <div class="relative">
                <IconPrisoner class="h-20 w-20 drop-shadow-xl animate-pulse" />
                <div class="absolute -top-1 -right-1">
                  <IconBars class="w-6 h-6 text-orange-500/50" />
                </div>
              </div>
              <p class="font-bold text-sm">Prisonnier #000</p>
              <p class="text-[11px] text-slate-400 text-center max-w-[200px] leading-relaxed">
                Identité non enregistrée. Utilise "Jouer / S'inscrire" pour créer ton dossier.
              </p>
            </div>

            <!-- APRÈS enregistrement : mugshot -->
            <div
              v-else
              class="flex items-center gap-5 w-full"
            >
              <!-- Silhouette du prisonnier -->
              <div class="relative">
                <IconPrisoner class="h-24 w-24 drop-shadow-xl transition-transform group-hover:scale-110" />
                <div class="absolute -top-1 -right-1 opacity-60">
                  <IconBars class="w-6 h-6 text-orange-500" />
                </div>
              </div>
              <!-- Ardoise + infos -->
              <div class="flex-1">
                <div
                  class="w-full rounded-xl bg-gradient-to-br from-slate-950 to-slate-900 border-2 border-orange-500/30 px-4 py-3 mb-3 flex flex-col items-center shadow-lg"
                >
                  <span class="text-xs font-semibold text-orange-300 uppercase tracking-wider mb-1">
                    {{ userNameLabel }}
                  </span>
                  <span class="text-lg font-mono font-bold text-slate-50 bg-slate-800 px-3 py-1 rounded-lg">
                    #{{ userIdLabel }}
                  </span>
                </div>
                <p class="text-[11px] text-slate-400 leading-relaxed text-center">
                  Identité utilisée pour suivre tes cellules résolues, tes temps d'évasion et ton classement.
                </p>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Boutons principaux -->
      <section class="flex flex-wrap gap-4 animate-slide-up" style="animation-delay: 0.3s">
        <button
          v-if="!isLoggedIn"
          class="group relative px-8 py-4 rounded-full bg-gradient-to-r from-orange-500 to-orange-600 text-slate-900 text-sm font-bold tracking-wide hover:from-orange-400 hover:to-orange-500 transition-all duration-300 shadow-lg shadow-orange-500/30 hover:shadow-xl hover:shadow-orange-500/40 hover:scale-105 active:scale-95 overflow-hidden"
          @click="onPlayOrRegister"
        >
          <span class="relative z-10 flex items-center gap-2">
            <span>🎮</span>
            <span>Jouer / S'inscrire</span>
            <span class="opacity-0 group-hover:opacity-100 transition-opacity">→</span>
          </span>
          <div class="absolute inset-0 bg-gradient-to-r from-orange-400 to-orange-500 opacity-0 group-hover:opacity-100 transition-opacity"></div>
        </button>

        <button
          v-else
          class="group relative px-8 py-4 rounded-full bg-gradient-to-r from-green-500 to-green-600 text-slate-900 text-sm font-bold tracking-wide hover:from-green-400 hover:to-green-500 transition-all duration-300 shadow-lg shadow-green-500/30 hover:shadow-xl hover:shadow-green-500/40 hover:scale-105 active:scale-95 overflow-hidden"
          @click="onPlay"
        >
          <span class="relative z-10 flex items-center gap-2">
            <span>🎮</span>
            <span>Jouer</span>
            <span class="opacity-0 group-hover:opacity-100 transition-opacity">→</span>
          </span>
          <div class="absolute inset-0 bg-gradient-to-r from-green-400 to-green-500 opacity-0 group-hover:opacity-100 transition-opacity"></div>
        </button>

        <button
          class="group relative px-8 py-4 rounded-full bg-gradient-to-r from-slate-800 to-slate-900 text-slate-100 text-sm font-bold tracking-wide hover:from-slate-700 hover:to-slate-800 transition-all duration-300 shadow-lg shadow-slate-900/30 hover:shadow-xl hover:shadow-slate-900/40 hover:scale-105 active:scale-95 border border-slate-700 hover:border-slate-600 overflow-hidden"
          @click="onTutorial"
        >
          <span class="relative z-10 flex items-center gap-2">
            <span>📖</span>
            <span>Tutoriel</span>
            <span class="opacity-0 group-hover:opacity-100 transition-opacity">→</span>
          </span>
          <div class="absolute inset-0 bg-gradient-to-r from-slate-700 to-slate-800 opacity-0 group-hover:opacity-100 transition-opacity"></div>
        </button>
      </section>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useUserStore } from '@/stores/user';
import { useUiStore } from '@/stores/ui';
import IconPrisoner from '@/components/icons/IconPrisoner.vue';
import IconBars from '@/components/icons/IconBars.vue';

const userStore = useUserStore();
const uiStore = useUiStore();

const isLoggedIn = computed(() => userStore.isLoggedIn);

// Titre principal
const greeting = computed(() =>
  userStore.user ? `Bonsoir, ${userStore.user.name}` : 'Bonsoir, Joueur'
);

// Nom affiché sur l'ardoise après enregistrement
const userNameLabel = computed(() =>
  userStore.user ? userStore.user.name.toUpperCase() : 'PRISONNIER'
);

// Numéro de prisonnier (#000 avant enregistrement, #XYZ après)
const userIdLabel = computed(() => {
  if (!userStore.user) {
    return '000';
  }
  return userStore.user.id.toString().padStart(3, '0');
});

function onPlayOrRegister() {
  uiStore.openUserModal();
}

function onPlay() {
  uiStore.goToLevels();
}

function onTutorial() {
  uiStore.openTutorialModal();
}
</script>

<style scoped>
/* Particules animées */
.particle {
  position: absolute;
  width: 4px;
  height: 4px;
  background: rgba(251, 146, 60, 0.4);
  border-radius: 50%;
  animation: float linear infinite;
  box-shadow: 0 0 10px rgba(251, 146, 60, 0.5);
}

@keyframes float {
  0% {
    transform: translateY(100vh) translateX(0);
    opacity: 0;
  }
  10% {
    opacity: 1;
  }
  90% {
    opacity: 1;
  }
  100% {
    transform: translateY(-100px) translateX(100px);
    opacity: 0;
  }
}

/* Animations d'entrée */
@keyframes fade-in {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes slide-up {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fade-in {
  animation: fade-in 0.8s ease-out;
}

.animate-slide-up {
  animation: slide-up 0.8s ease-out both;
}
</style>
