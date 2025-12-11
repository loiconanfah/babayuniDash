<template>
  <section class="w-full h-full px-4 sm:px-6 lg:px-12 py-8 overflow-y-auto">
    <div class="max-w-6xl mx-auto">
      <!-- En-tête -->
      <header class="mb-8">
        <h2 class="text-3xl sm:text-4xl font-bold text-slate-50 mb-2">
          🎮 Jeux Disponibles
        </h2>
        <p class="text-sm text-slate-300">
          Choisissez un jeu pour commencer à jouer
        </p>
      </header>

      <!-- Grille des jeux -->
      <div class="grid grid-cols-1 md:grid-cols-2 gap-6 mb-8">
        <!-- Jeu Hashi -->
        <div
          class="group relative rounded-3xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 shadow-2xl p-8 flex flex-col transition-all duration-500 hover:shadow-orange-500/20 hover:border-orange-500/30 hover:scale-[1.02] cursor-pointer"
          @click="goToHashi"
        >
          <div class="mb-6">
            <div class="text-6xl mb-4">🌉</div>
            <h3 class="text-2xl font-bold text-slate-50 mb-3">Hashi</h3>
            <p class="text-sm text-slate-300 leading-relaxed mb-4">
              Connectez les îles avec des ponts pour résoudre des puzzles logiques.
              Plusieurs niveaux de difficulté disponibles.
            </p>
            <div class="flex flex-wrap gap-2 mb-4">
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
          </div>
          <button
            class="mt-auto px-6 py-3 rounded-xl bg-gradient-to-r from-orange-500 to-orange-600 text-slate-900 text-sm font-bold tracking-wide hover:from-orange-400 hover:to-orange-500 transition-all duration-300 shadow-lg shadow-orange-500/30 hover:shadow-xl hover:shadow-orange-500/40 hover:scale-105 active:scale-95"
          >
            Jouer à Hashi →
          </button>
        </div>

        <!-- Jeu Tic-Tac-Toe -->
        <div
          class="group relative rounded-3xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 shadow-2xl p-8 flex flex-col transition-all duration-500 hover:shadow-blue-500/20 hover:border-blue-500/30 hover:scale-[1.02] cursor-pointer"
          @click="goToTicTacToe"
        >
          <div class="mb-6">
            <div class="text-6xl mb-4">⭕</div>
            <h3 class="text-2xl font-bold text-slate-50 mb-3">Tic-Tac-Toe</h3>
            <p class="text-sm text-slate-300 leading-relaxed mb-4">
              Le classique jeu de morpion. Jouez contre l'IA ou défiez d'autres joueurs en ligne.
            </p>
            <div class="flex flex-wrap gap-2 mb-4">
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
          </div>
          <button
            class="mt-auto px-6 py-3 rounded-xl bg-gradient-to-r from-blue-500 to-blue-600 text-slate-50 text-sm font-bold tracking-wide hover:from-blue-400 hover:to-blue-500 transition-all duration-300 shadow-lg shadow-blue-500/30 hover:shadow-xl hover:shadow-blue-500/40 hover:scale-105 active:scale-95"
          >
            Jouer à Tic-Tac-Toe →
          </button>
        </div>
      </div>

      <!-- Statistiques rapides -->
      <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
        <div class="rounded-2xl bg-gradient-to-br from-slate-800/80 to-slate-900/80 border border-slate-700/50 p-6 flex items-center gap-4">
          <div class="text-4xl">🎮</div>
          <div>
            <div class="text-2xl font-bold text-slate-50">2</div>
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

