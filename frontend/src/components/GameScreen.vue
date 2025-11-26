<!-- src/components/GameScreen.vue -->
<template>
  <section class="w-full h-full px-2 sm:px-4 lg:px-8 py-4 relative">
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
          class="lg:col-span-6 flex items-center justify-center rounded-2xl bg-slate-900/70 border border-slate-700 p-2 sm:p-4 overflow-hidden relative"
        >
          <!-- Grille Hashi intégrée -->
          <GameGrid
            v-if="gameStore.currentPuzzle"
            :width="gameStore.currentPuzzle.width"
            :height="gameStore.currentPuzzle.height"
            class="w-full h-full max-w-full max-h-full"
            :class="{ 'opacity-50 pointer-events-none': gameStore.isPaused }"
          />
          <div
            v-else
            class="w-full h-64 sm:h-80 lg:h-[26rem] flex items-center justify-center border border-dashed border-slate-600 rounded-xl text-slate-400 text-sm"
          >
            Chargement du puzzle...
          </div>
          
          <!-- Overlay de pause -->
          <div
            v-if="gameStore.isPaused"
            class="absolute inset-0 bg-slate-900/80 backdrop-blur-sm flex items-center justify-center rounded-2xl z-10"
          >
            <div class="text-center">
              <div class="text-6xl mb-4">⏸️</div>
              <p class="text-2xl font-bold text-amber-300 mb-2">Jeu en Pause</p>
              <p class="text-sm text-slate-400">Cliquez sur "Reprendre" pour continuer</p>
            </div>
          </div>
        </div>

        <!-- Colonne droite : timer + contrôles -->
        <div
          class="lg:col-span-3 flex flex-col justify-between items-center gap-4"
        >
          <!-- Timer + boutons -->
          <div class="w-full flex flex-col items-center gap-3">
            <div
              class="w-full rounded-2xl bg-slate-900/80 border border-slate-600 px-4 py-3 flex flex-col gap-3"
            >
              <div class="flex items-center justify-between">
                <div>
                  <p class="text-[10px] uppercase tracking-[0.2em] text-slate-400">
                    Temps écoulé
                  </p>
                  <p class="text-lg font-mono text-orange-300">
                    {{ formattedTime }}
                  </p>
                </div>
              </div>
              <div class="flex flex-col gap-2">
                <button
                  class="px-3 py-2 rounded-lg bg-green-600 text-sm font-semibold hover:bg-green-500 transition-colors"
                  @click="handleValidate"
                  :disabled="gameStore.isLoading"
                >
                  Valider
                </button>
                <button
                  class="px-3 py-2 rounded-lg bg-slate-700 text-sm hover:bg-slate-600 transition-colors"
                  @click="handleReset"
                >
                  Réinitialiser
                </button>
                <button
                  class="px-3 py-2 rounded-lg bg-purple-600 text-sm hover:bg-purple-500 transition-colors"
                  @click="handleHelp"
                >
                  ❓ Aide
                </button>
                <button
                  class="px-3 py-2 rounded-lg bg-red-600 text-sm hover:bg-red-500 transition-colors"
                  @click="handleAbandon"
                >
                  Abandonner
                </button>
              </div>
            </div>
          </div>

          <!-- Bouton Pause -->
          <button
            class="w-full flex flex-col items-center gap-3 cursor-pointer hover:opacity-80 transition-opacity"
            @click="handlePause"
            :title="gameStore.isPaused ? 'Reprendre le jeu' : 'Mettre le jeu en pause'"
          >
            <div
              class="w-24 h-32 sm:w-28 sm:h-36 rounded-2xl bg-slate-950 border-4 flex items-center justify-center transition-colors"
              :class="gameStore.isPaused 
                ? 'border-green-500 bg-green-950/30' 
                : 'border-slate-800 hover:border-amber-500'"
            >
              <div v-if="gameStore.isPaused" class="flex items-center gap-1.5">
                <div class="w-2 h-6 bg-green-400 rounded"></div>
                <div class="w-2 h-6 bg-green-400 rounded"></div>
              </div>
              <div v-else class="flex items-center justify-center">
                <div class="w-0 h-0 border-l-[12px] border-l-amber-400 border-y-[10px] border-y-transparent ml-1"></div>
                <div class="w-0 h-0 border-l-[12px] border-l-amber-400 border-y-[10px] border-y-transparent"></div>
              </div>
            </div>
            <p
              class="text-xs uppercase tracking-[0.25em]"
              :class="gameStore.isPaused ? 'text-green-300' : 'text-amber-300'"
            >
              {{ gameStore.isPaused ? 'Reprendre' : 'Pause' }}
            </p>
          </button>
        </div>
      </div>

      <!-- Message d'erreur -->
      <div
        v-if="gameStore.error"
        class="mt-4 p-3 rounded-lg bg-red-900/80 border border-red-700 text-red-100 text-sm"
      >
        {{ gameStore.error }}
      </div>
    </div>

    <!-- Bouton pour changer de niveau (en haut à droite) -->
    <button
      class="absolute top-4 right-4 px-4 py-2 rounded-lg bg-slate-800/80 border border-slate-700 text-slate-200 text-sm font-medium hover:bg-slate-700 transition-colors z-10"
      @click="handleChangeLevel"
    >
      ← Changer de Niveau
    </button>

    <!-- Modale de victoire -->
    <VictoryModal v-if="uiStore.isVictoryModalOpen" />
  </section>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useGameStore } from '@/stores/game';
import { useUiStore } from '@/stores/ui';
import GameGrid from './game/GameGrid.vue';
import VictoryModal from './VictoryModal.vue';

const gameStore = useGameStore();
const uiStore = useUiStore();

/**
 * Formate le temps écoulé en MM:SS
 */
const formattedTime = computed(() => {
  const seconds = gameStore.elapsedTime;
  const minutes = Math.floor(seconds / 60);
  const remainingSeconds = seconds % 60;
  return `${minutes.toString().padStart(2, '0')}:${remainingSeconds.toString().padStart(2, '0')}`;
});

/**
 * Valide la solution actuelle
 */
async function handleValidate() {
  try {
    const result = await gameStore.validateSolution();
    if (result.isValid) {
      // Ouvrir la modale de victoire
      uiStore.openVictoryModal();
    } else {
      // Afficher les erreurs
      const errorMessage = result.errors.length > 0 
        ? result.errors.join('\n')
        : 'Solution incorrecte. Vérifiez que tous les verrous sont correctement connectés.';
      alert(errorMessage);
    }
  } catch (error) {
    console.error('Erreur lors de la validation:', error);
  }
}

/**
 * Change de niveau
 */
function handleChangeLevel() {
  if (confirm('Êtes-vous sûr de vouloir changer de niveau ? Votre progression actuelle sera perdue.')) {
    gameStore.resetGame();
    uiStore.goToLevels();
  }
}

/**
 * Réinitialise la grille (supprime tous les ponts)
 */
function handleReset() {
  if (confirm('Êtes-vous sûr de vouloir effacer tous les ponts ?')) {
    gameStore.playerBridges = [];
    gameStore.saveBridges();
  }
}

/**
 * Abandonne la partie
 */
async function handleAbandon() {
  if (confirm('Êtes-vous sûr de vouloir abandonner cette partie ?')) {
    try {
      await gameStore.abandonGame();
      uiStore.goToHome();
    } catch (error) {
      console.error("Erreur lors de l'abandon:", error);
    }
  }
}

/**
 * Résout automatiquement le puzzle (bouton d'aide)
 */
async function handleHelp() {
  if (confirm('Voulez-vous résoudre automatiquement ce puzzle ? Cette action remplacera tous vos ponts actuels.')) {
    try {
      await gameStore.solvePuzzle();
    } catch (error) {
      console.error('Erreur lors de la résolution:', error);
    }
  }
}

/**
 * Met le jeu en pause ou le reprend
 */
function handlePause() {
  gameStore.togglePause();
}
</script>
