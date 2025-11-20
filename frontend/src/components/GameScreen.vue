<!-- src/components/GameScreen.vue -->
<template>
  <section class="w-full h-full px-2 sm:px-4 lg:px-8 py-4">
    <div
      class="w-full h-full max-w-6xl mx-auto rounded-3xl border border-slate-700 bg-[radial-gradient(circle_at_top,_#7f1d1d_0,_#020617_55%,_#000000_100%)] text-slate-100 shadow-2xl p-3 sm:p-4 lg:p-6"
    >
      <div
        class="grid grid-cols-1 lg:grid-cols-12 gap-4 sm:gap-6 h-full items-stretch"
      >
        <!-- Colonne gauche : personnage + vies -->
        <div
          class="lg:col-span-3 flex flex-col items-center justify-between gap-4"
        >
          <div class="w-full flex flex-col items-center gap-3">
            <p class="text-xs uppercase tracking-[0.2em] text-orange-200">
              Prisonnier
            </p>
            <div
              class="h-24 w-20 rounded-2xl bg-orange-500 border-4 border-orange-200 flex items-center justify-center text-slate-900 font-bold"
            >
              YOU
            </div>
            <div class="flex items-center gap-1">
              <span class="text-xs text-slate-300 mr-1">Vies :</span>
              <span
                v-for="life in 3"
                :key="life"
                class="h-4 w-4 rounded-full bg-red-500 shadow-inner"
              ></span>
            </div>
          </div>
        </div>

        <!-- Colonne centrale : grille de jeu -->
        <div
          class="lg:col-span-6 flex items-center justify-center rounded-2xl bg-slate-900/70 border border-slate-700 p-2 sm:p-4"
        >
          <!-- Ici on intégrera plus tard ton composant GameGrid -->
          <div
            class="w-full h-64 sm:h-80 lg:h-[26rem] flex items-center justify-center border border-dashed border-slate-600 rounded-xl text-slate-400 text-sm"
          >
            Grille Hashi (GameGrid) à intégrer ici
          </div>
        </div>

        <!-- Colonne droite : timer + porte -->
        <div
          class="lg:col-span-3 flex flex-col justify-between items-center gap-4"
        >
          <!-- Timer + boutons -->
          <div class="w-full flex flex-col items-center gap-3">
            <div
              class="w-full rounded-2xl bg-slate-900/80 border border-slate-600 px-4 py-3 flex items-center justify-between"
            >
              <div>
                <p class="text-[10px] uppercase tracking-[0.2em] text-slate-400">
                  Temps restant
                </p>
                <p class="text-lg font-mono text-orange-300">
                  15:00
                </p>
              </div>
              <div class="flex flex-col gap-2">
                <button
                  class="px-2 py-1 rounded-lg bg-slate-700 text-xs hover:bg-slate-600"
                >
                  Pause
                </button>
                <button
                  class="px-2 py-1 rounded-lg bg-red-600 text-xs hover:bg-red-500"
                >
                  Quitter
                </button>
              </div>
            </div>
          </div>

          <!-- Porte Escape -->
          <div class="w-full flex flex-col items-center gap-3">
            <div
              class="w-24 h-32 sm:w-28 sm:h-36 rounded-2xl bg-slate-950 border-4 border-slate-800 flex items-center justify-center"
            >
              <div class="space-y-2 w-14">
                <div class="h-1.5 bg-slate-500 rounded"></div>
                <div class="h-1.5 bg-slate-500 rounded"></div>
                <div class="h-1.5 bg-slate-500 rounded"></div>
                <div class="h-1.5 bg-slate-500 rounded"></div>
                <div class="h-1.5 bg-slate-500 rounded"></div>
              </div>
            </div>
            <p
              class="text-xs uppercase tracking-[0.25em] text-amber-300"
            >
              Escape
            </p>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
// Plus tard, on pourra importer useGame() et le store de jeu ici.
</script>
