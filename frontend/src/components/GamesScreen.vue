<template>
  <section class="w-full h-full px-4 sm:px-6 lg:px-12 py-8 overflow-y-auto">
    <div class="max-w-6xl mx-auto">
      <!-- En-tête -->
      <header class="mb-8">
        <div class="flex items-center gap-3 mb-3">
          <div class="h-1 w-12 bg-gradient-to-r from-orange-500 to-orange-300 rounded-full"></div>
          <p class="text-sm uppercase tracking-[0.3em] text-orange-300 font-semibold">
            Bibliothèque de Jeux
          </p>
        </div>
        <h2 class="text-4xl sm:text-5xl font-extrabold text-slate-50 mb-3 bg-gradient-to-r from-slate-50 via-slate-100 to-slate-200 bg-clip-text text-transparent">
          🎮 Jeux Disponibles
        </h2>
        <p class="text-base text-slate-300 max-w-2xl">
          Explorez notre collection complète de jeux. Choisissez votre aventure et commencez à jouer !
        </p>
      </header>

      <!-- Grille des jeux -->
      <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-8">
        <!-- Jeu Hashi -->
        <div
          class="group relative rounded-3xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 shadow-2xl overflow-hidden transition-all duration-500 hover:shadow-orange-500/20 hover:border-orange-500/30 hover:scale-[1.02] cursor-pointer"
          @click="goToHashi"
        >
          <!-- Image/Header du jeu -->
          <div class="relative h-48 bg-gradient-to-br from-orange-600/30 via-slate-900 to-slate-800 flex items-center justify-center overflow-hidden">
            <div class="text-8xl opacity-30 group-hover:scale-110 transition-transform duration-500">🌉</div>
            <div class="absolute top-4 left-4">
              <span class="px-3 py-1.5 rounded-full text-xs font-bold bg-gradient-to-r from-yellow-500 to-orange-500 text-slate-900 shadow-lg">
                POPULAIRE
              </span>
            </div>
            <div class="absolute top-4 right-4">
              <div class="flex items-center gap-1 bg-slate-900/80 backdrop-blur-sm px-2.5 py-1.5 rounded-lg">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-yellow-400" viewBox="0 0 20 20" fill="currentColor">
                  <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                </svg>
                <span class="text-sm font-bold text-slate-50">5.0</span>
              </div>
            </div>
          </div>

          <!-- Contenu -->
          <div class="p-6 flex flex-col flex-1">
            <h3 class="text-2xl font-bold text-slate-50 mb-2">Hashi</h3>
            <p class="text-sm text-slate-300 leading-relaxed mb-4 line-clamp-2">
              Connectez les îles avec des ponts pour résoudre des puzzles logiques.
              Plusieurs niveaux de difficulté disponibles.
            </p>
            <div class="flex flex-wrap gap-2 mb-6">
              <span class="px-3 py-1 rounded-full text-xs font-medium bg-orange-500/20 text-orange-300 border border-orange-500/30">
                🎯 Solo
              </span>
              <span class="px-3 py-1 rounded-full text-xs font-medium bg-orange-500/20 text-orange-300 border border-orange-500/30">
                📊 Scores
              </span>
              <span class="px-3 py-1 rounded-full text-xs font-medium bg-orange-500/20 text-orange-300 border border-orange-500/30">
                🏆 Classement
              </span>
            </div>
            <button
              class="mt-auto px-6 py-3 rounded-xl bg-gradient-to-r from-orange-500 to-orange-600 text-slate-900 text-sm font-bold tracking-wide hover:from-orange-400 hover:to-orange-500 transition-all duration-300 shadow-lg shadow-orange-500/30 hover:shadow-xl hover:shadow-orange-500/40 hover:scale-105 active:scale-95 flex items-center justify-center gap-2"
            >
              <span>Jouer maintenant</span>
              <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M9 5l7 7-7 7" />
              </svg>
            </button>
          </div>
        </div>

        <!-- Jeu Tic-Tac-Toe -->
        <div
          class="group relative rounded-3xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 shadow-2xl overflow-hidden transition-all duration-500 hover:shadow-blue-500/20 hover:border-blue-500/30 hover:scale-[1.02] cursor-pointer"
          @click="goToTicTacToe"
        >
          <div class="relative h-48 bg-gradient-to-br from-blue-600/30 via-slate-900 to-slate-800 flex items-center justify-center overflow-hidden">
            <div class="text-8xl opacity-30 group-hover:scale-110 transition-transform duration-500">⭕</div>
            <div class="absolute top-4 right-4">
              <div class="flex items-center gap-1 bg-slate-900/80 backdrop-blur-sm px-2.5 py-1.5 rounded-lg">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-yellow-400" viewBox="0 0 20 20" fill="currentColor">
                  <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                </svg>
                <span class="text-sm font-bold text-slate-50">5.0</span>
              </div>
            </div>
          </div>
          <div class="p-6 flex flex-col flex-1">
            <h3 class="text-2xl font-bold text-slate-50 mb-2">Tic-Tac-Toe</h3>
            <p class="text-sm text-slate-300 leading-relaxed mb-4 line-clamp-2">
              Le classique jeu de morpion. Jouez contre l'IA ou défiez d'autres joueurs en ligne.
            </p>
            <div class="flex flex-wrap gap-2 mb-6">
              <span class="px-3 py-1 rounded-full text-xs font-medium bg-blue-500/20 text-blue-300 border border-blue-500/30">
                🤖 IA
              </span>
              <span class="px-3 py-1 rounded-full text-xs font-medium bg-blue-500/20 text-blue-300 border border-blue-500/30">
                👥 Multijoueur
              </span>
              <span class="px-3 py-1 rounded-full text-xs font-medium bg-blue-500/20 text-blue-300 border border-blue-500/30">
                ⚡ Rapide
              </span>
            </div>
            <button
              class="mt-auto px-6 py-3 rounded-xl bg-gradient-to-r from-blue-500 to-blue-600 text-slate-50 text-sm font-bold tracking-wide hover:from-blue-400 hover:to-blue-500 transition-all duration-300 shadow-lg shadow-blue-500/30 hover:shadow-xl hover:shadow-blue-500/40 hover:scale-105 active:scale-95 flex items-center justify-center gap-2"
            >
              <span>Jouer maintenant</span>
              <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M9 5l7 7-7 7" />
              </svg>
            </button>
          </div>
        </div>

        <!-- Jeu Connect Four -->
        <div
          class="group relative rounded-3xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 shadow-2xl p-8 flex flex-col transition-all duration-500 hover:shadow-red-500/20 hover:border-red-500/30 hover:scale-[1.02] cursor-pointer"
          @click="goToConnectFour"
        >
          <div class="mb-6">
            <div class="text-6xl mb-4">🔴</div>
            <h3 class="text-2xl font-bold text-slate-50 mb-3">Connect Four</h3>
            <p class="text-sm text-slate-300 leading-relaxed mb-4">
              Alignez 4 pièces pour gagner ! Le classique jeu de Puissance 4.
            </p>
            <div class="flex flex-wrap gap-2 mb-4">
              <span class="px-3 py-1 rounded-full text-xs font-medium bg-red-500/20 text-red-300 border border-red-500/30">
                🤖 IA
              </span>
              <span class="px-3 py-1 rounded-full text-xs font-medium bg-red-500/20 text-red-300 border border-red-500/30">
                👥 Multijoueur
              </span>
              <span class="px-3 py-1 rounded-full text-xs font-medium bg-red-500/20 text-red-300 border border-red-500/30">
                ⚡ Rapide
              </span>
            </div>
          </div>
          <button
            class="mt-auto px-6 py-3 rounded-xl bg-gradient-to-r from-red-500 to-red-600 text-slate-50 text-sm font-bold tracking-wide hover:from-red-400 hover:to-red-500 transition-all duration-300 shadow-lg shadow-red-500/30 hover:shadow-xl hover:shadow-red-500/40 hover:scale-105 active:scale-95"
          >
            Jouer à Connect Four →
          </button>
        </div>

        <!-- Jeu Rock Paper Scissors -->
        <div
          class="group relative rounded-3xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 shadow-2xl p-8 flex flex-col transition-all duration-500 hover:shadow-purple-500/20 hover:border-purple-500/30 hover:scale-[1.02] cursor-pointer"
          @click="goToRockPaperScissors"
        >
          <div class="mb-6">
            <div class="text-6xl mb-4">✂️</div>
            <h3 class="text-2xl font-bold text-slate-50 mb-3">Pierre-Papier-Ciseaux</h3>
            <p class="text-sm text-slate-300 leading-relaxed mb-4">
              Le jeu classique ! Choisissez votre coup et défiez vos amis en ligne. Rapide et amusant !
            </p>
            <div class="flex flex-wrap gap-2 mb-4">
              <span class="px-3 py-1 rounded-full text-xs font-medium bg-purple-500/20 text-purple-300 border border-purple-500/30">
                🤖 IA
              </span>
              <span class="px-3 py-1 rounded-full text-xs font-medium bg-purple-500/20 text-purple-300 border border-purple-500/30">
                👥 Multijoueur
              </span>
              <span class="px-3 py-1 rounded-full text-xs font-medium bg-purple-500/20 text-purple-300 border border-purple-500/30">
                ⚡ Rapide
              </span>
            </div>
          </div>
          <button
            class="mt-auto px-6 py-3 rounded-xl bg-gradient-to-r from-purple-500 to-purple-600 text-slate-50 text-sm font-bold tracking-wide hover:from-purple-400 hover:to-purple-500 transition-all duration-300 shadow-lg shadow-purple-500/30 hover:shadow-xl hover:shadow-purple-500/40 hover:scale-105 active:scale-95"
          >
            Jouer à RPS →
          </button>
        </div>

        <!-- Jeu d'Aventure -->
        <div
          class="group relative rounded-3xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 shadow-2xl p-8 flex flex-col transition-all duration-500 hover:shadow-emerald-500/20 hover:border-emerald-500/30 hover:scale-[1.02] cursor-pointer"
          @click="goToAdventure"
        >
          <div class="mb-6">
            <div class="text-6xl mb-4">🗝️</div>
            <h3 class="text-2xl font-bold text-slate-50 mb-3">Aventure Mystérieuse</h3>
            <p class="text-sm text-slate-300 leading-relaxed mb-4">
              Explorez un château, résolvez des énigmes et collectez des objets dans cette aventure interactive !
            </p>
            <div class="flex flex-wrap gap-2 mb-4">
              <span class="px-3 py-1 rounded-full text-xs font-medium bg-emerald-500/20 text-emerald-300 border border-emerald-500/30">
                🎮 Solo
              </span>
              <span class="px-3 py-1 rounded-full text-xs font-medium bg-emerald-500/20 text-emerald-300 border border-emerald-500/30">
                🧩 Énigmes
              </span>
              <span class="px-3 py-1 rounded-full text-xs font-medium bg-emerald-500/20 text-emerald-300 border border-emerald-500/30">
                🎨 Animations
              </span>
            </div>
          </div>
          <button
            class="mt-auto px-6 py-3 rounded-xl bg-gradient-to-r from-emerald-500 to-emerald-600 text-slate-50 text-sm font-bold tracking-wide hover:from-emerald-400 hover:to-emerald-500 transition-all duration-300 shadow-lg shadow-emerald-500/30 hover:shadow-xl hover:shadow-emerald-500/40 hover:scale-105 active:scale-95"
          >
            Commencer l'aventure →
          </button>
        </div>
      </div>

      <!-- Statistiques rapides -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-6 gap-4">
        <div class="rounded-2xl bg-gradient-to-br from-slate-800/80 to-slate-900/80 border border-slate-700/50 p-6 flex items-center gap-4">
          <div class="text-4xl">🎮</div>
          <div>
            <div class="text-2xl font-bold text-slate-50">5</div>
            <div class="text-xs text-slate-400">Jeux disponibles</div>
          </div>
        </div>
        <div class="rounded-2xl bg-gradient-to-br from-slate-800/80 to-slate-900/80 border border-slate-700/50 p-6 flex items-center gap-4">
          <div class="text-4xl">🌉</div>
          <div>
            <div class="text-lg font-bold text-slate-50">Hashi</div>
            <div class="text-xs text-slate-400">Puzzle logique</div>
          </div>
        </div>
        <div class="rounded-2xl bg-gradient-to-br from-slate-800/80 to-slate-900/80 border border-slate-700/50 p-6 flex items-center gap-4">
          <div class="text-4xl">⭕</div>
          <div>
            <div class="text-lg font-bold text-slate-50">Tic-Tac-Toe</div>
            <div class="text-xs text-slate-400">Stratégie rapide</div>
          </div>
        </div>
        <div class="rounded-2xl bg-gradient-to-br from-slate-800/80 to-slate-900/80 border border-slate-700/50 p-6 flex items-center gap-4">
          <div class="text-4xl">🔴</div>
          <div>
            <div class="text-lg font-bold text-slate-50">Connect Four</div>
            <div class="text-xs text-slate-400">Alignement 4</div>
          </div>
        </div>
        <div class="rounded-2xl bg-gradient-to-br from-slate-800/80 to-slate-900/80 border border-slate-700/50 p-6 flex items-center gap-4">
          <div class="text-4xl">✂️</div>
          <div>
            <div class="text-lg font-bold text-slate-50">RPS</div>
            <div class="text-xs text-slate-400">Pierre-Papier-Ciseaux</div>
          </div>
        </div>
        <div class="rounded-2xl bg-gradient-to-br from-slate-800/80 to-slate-900/80 border border-slate-700/50 p-6 flex items-center gap-4">
          <div class="text-4xl">🗝️</div>
          <div>
            <div class="text-lg font-bold text-slate-50">Aventure</div>
            <div class="text-xs text-slate-400">Énigmes & Exploration</div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { useUiStore } from '@/stores/ui';

const uiStore = useUiStore();

/**
 * Navigation vers Hashi (sélection de niveaux)
 */
function goToHashi() {
  uiStore.goToLevels();
}

/**
 * Navigation vers Tic-Tac-Toe
 */
function goToTicTacToe() {
  uiStore.goToTicTacToe();
}

/**
 * Navigation vers Connect Four
 */
function goToConnectFour() {
  uiStore.goToConnectFour();
}

/**
 * Navigation vers Rock Paper Scissors
 */
function goToRockPaperScissors() {
  uiStore.goToRockPaperScissors();
}

/**
 * Navigation vers Aventure
 */
function goToAdventure() {
  uiStore.goToAdventure();
}
</script>

<style scoped>
section {
  scrollbar-width: thin;
  scrollbar-color: rgba(148, 163, 184, 0.3) transparent;
}

section::-webkit-scrollbar {
  width: 8px;
}

section::-webkit-scrollbar-track {
  background: transparent;
}

section::-webkit-scrollbar-thumb {
  background-color: rgba(148, 163, 184, 0.3);
  border-radius: 4px;
}

section::-webkit-scrollbar-thumb:hover {
  background-color: rgba(148, 163, 184, 0.5);
}
</style>

