<!-- src/components/LevelSelectScreen.vue -->
<template>
  <section class="w-full h-full px-4 sm:px-6 lg:px-12 py-8">
    <div class="max-w-4xl mx-auto">
      <h2 class="text-2xl sm:text-3xl font-bold text-slate-50 mb-6">
        Choix de niveaux
      </h2>
      <p class="text-sm text-slate-300 mb-6">
        Chaque niveau correspond à une étape de ton évasion. Plus la difficulté
        augmente, plus la grille Hashi est grande et complexe.
      </p>

      <div class="grid gap-4 sm:gap-6 md:grid-cols-3">
        <button
          class="relative rounded-2xl bg-amber-900/70 border border-amber-700 p-4 text-left flex flex-col justify-between hover:border-amber-400 hover:shadow-lg transition-shadow"
          @click="selectLevel('easy')"
        >
          <div>
            <p class="text-xs uppercase tracking-[0.2em] text-amber-200 mb-1">
              Facile
            </p>
            <h3 class="text-lg font-semibold text-amber-50">
              Cellule d’Isolement
            </h3>
            <p class="text-xs text-amber-100 mt-2">
              Une petite cellule, peu de gardiens. Idéal pour apprendre les
              règles du puzzle.
            </p>
          </div>
          <div class="mt-4 flex items-center justify-between">
            <span
              class="text-[11px] px-2 py-1 rounded-full bg-black/30 text-amber-100"
            >
              ~ 5-10 min
            </span>
            <span
              class="inline-flex items-center justify-center h-8 w-8 rounded-full bg-amber-500 text-black text-xs font-bold"
            >
              ▶
            </span>
          </div>
        </button>

        <button
          class="relative rounded-2xl bg-orange-900/70 border border-orange-700 p-4 text-left flex flex-col justify-between hover:border-orange-400 hover:shadow-lg transition-shadow"
          @click="selectLevel('medium')"
        >
          <div>
            <p class="text-xs uppercase tracking-[0.2em] text-orange-200 mb-1">
              Moyen
            </p>
            <h3 class="text-lg font-semibold text-orange-50">
              Aile de Détention B
            </h3>
            <p class="text-xs text-orange-100 mt-2">
              Plus de prisonniers, plus de chemins possibles. La marge d’erreur
              se réduit.
            </p>
          </div>
          <div class="mt-4 flex items-center justify-between">
            <span
              class="text-[11px] px-2 py-1 rounded-full bg-black/30 text-orange-100"
            >
              ~ 10-15 min
            </span>
            <span
              class="inline-flex items-center justify-center h-8 w-8 rounded-full bg-orange-500 text-black text-xs font-bold"
            >
              ▶
            </span>
          </div>
        </button>

        <button
          class="relative rounded-2xl bg-red-900/80 border border-red-700 p-4 text-left flex flex-col justify-between hover:border-red-400 hover:shadow-lg transition-shadow"
          @click="selectLevel('hard')"
        >
          <div>
            <p class="text-xs uppercase tracking-[0.2em] text-red-200 mb-1">
              Difficile
            </p>
            <h3 class="text-lg font-semibold text-red-50">
              Mirador – Dernière Barrière
            </h3>
            <p class="text-xs text-red-100 mt-2">
              Les gardes surveillent tout. Une seule erreur peut ruiner ta
              tentative d’évasion.
            </p>
          </div>
          <div class="mt-4 flex items-center justify-between">
            <span
              class="text-[11px] px-2 py-1 rounded-full bg-black/30 text-red-100"
            >
              ~ 15-20 min
            </span>
            <span
              class="inline-flex items-center justify-center h-8 w-8 rounded-full bg-red-500 text-black text-xs font-bold"
            >
              ▶
            </span>
          </div>
        </button>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { useUiStore } from '@/stores/ui';

const uiStore = useUiStore();

function selectLevel(diff: 'easy' | 'medium' | 'hard') {
  uiStore.selectDifficulty(diff);
  uiStore.goToGame();
}
</script>
