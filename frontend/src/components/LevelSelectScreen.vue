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

      <!-- Chargement -->
      <div v-if="isLoading" class="text-center py-8">
        <p class="text-slate-300">Chargement des puzzles...</p>
      </div>

      <!-- Erreur -->
      <div v-else-if="error" class="p-4 rounded-lg bg-red-900/80 border border-red-700 text-red-100">
        <p>{{ error }}</p>
        <button
          class="mt-2 px-4 py-2 rounded-lg bg-red-600 hover:bg-red-500 text-sm"
          @click="loadPuzzles"
        >
          Réessayer
        </button>
      </div>

      <!-- Sélection de difficulté si aucune difficulté n'est sélectionnée -->
      <div v-else-if="!uiStore.selectedDifficulty" class="grid gap-4 sm:gap-6 md:grid-cols-3">
        <button
          class="relative rounded-2xl bg-amber-900/70 border border-amber-700 p-4 text-left flex flex-col justify-between hover:border-amber-400 hover:shadow-lg transition-shadow"
          @click="selectLevel('easy')"
        >
          <div>
            <p class="text-xs uppercase tracking-[0.2em] text-amber-200 mb-1">
              Facile
            </p>
            <h3 class="text-lg font-semibold text-amber-50">
              Cellule d'Isolement
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
              Plus de prisonniers, plus de chemins possibles. La marge d'erreur
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
              tentative d'évasion.
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

      <!-- Liste des puzzles disponibles pour la difficulté sélectionnée -->
      <div v-else-if="availablePuzzles.length > 0" class="grid gap-4 sm:gap-6 md:grid-cols-3">
        <button
          v-for="puzzle in availablePuzzles"
          :key="puzzle.id"
          class="relative rounded-2xl border p-4 text-left flex flex-col justify-between hover:shadow-lg transition-shadow"
          :class="getDifficultyClasses(puzzle.difficulty)"
          @click="startPuzzle(puzzle)"
        >
          <div>
            <p class="text-xs uppercase tracking-[0.2em] mb-1" :class="getDifficultyTextColor(puzzle.difficulty)">
              {{ getDifficultyLabel(puzzle.difficulty) }}
            </p>
            <h3 class="text-lg font-semibold" :class="getDifficultyTitleColor(puzzle.difficulty)">
              {{ puzzle.name || `Puzzle ${puzzle.id}` }}
            </h3>
            <p class="text-xs mt-1" :class="getDifficultyDescriptionColor(puzzle.difficulty)">
              {{ getThemeName(puzzle.theme) }}
            </p>
            <p class="text-xs mt-1" :class="getDifficultyDescriptionColor(puzzle.difficulty)">
              {{ puzzle.width }}x{{ puzzle.height }} - {{ puzzle.islandCount }} îles
            </p>
          </div>
          <div class="mt-4 flex items-center justify-between">
            <span
              class="text-[11px] px-2 py-1 rounded-full bg-black/30"
              :class="getDifficultyBadgeColor(puzzle.difficulty)"
            >
              {{ getEstimatedTime(puzzle.difficulty) }}
            </span>
            <span
              class="inline-flex items-center justify-center h-8 w-8 rounded-full text-black text-xs font-bold"
              :class="getDifficultyButtonColor(puzzle.difficulty)"
            >
              ▶
            </span>
          </div>
        </button>
      </div>

      <!-- Aucun puzzle disponible après sélection -->
      <div v-else class="text-center py-8">
        <p class="text-slate-300 mb-4">Aucun puzzle disponible pour cette difficulté.</p>
        <div class="flex gap-3 justify-center">
          <button
            class="px-4 py-2 rounded-lg bg-slate-700 hover:bg-slate-600 text-sm"
            @click="goBackToLevelSelection"
          >
            Choisir un autre niveau
          </button>
          <button
            class="px-4 py-2 rounded-lg bg-slate-700 hover:bg-slate-600 text-sm"
            @click="uiStore.goToHome()"
          >
            Retour à l'accueil
          </button>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import { useUiStore } from '@/stores/ui';
import { useUserStore } from '@/stores/user';
import { useGameStore } from '@/stores/game';
import { puzzleApi } from '@/services/api';
import type { Puzzle } from '@/types';
import { DifficultyLevel } from '@/types';
import { getThemeName } from '@/utils/themeConfig';

const uiStore = useUiStore();
const userStore = useUserStore();
const gameStore = useGameStore();

const isLoading = ref(false);
const error = ref<string | null>(null);
const puzzles = ref<Puzzle[]>([]);

/**
 * Filtre les puzzles selon la difficulté sélectionnée
 * Si une difficulté est sélectionnée, retourne les puzzles de cette difficulté
 */
const availablePuzzles = computed(() => {
  if (!uiStore.selectedDifficulty) {
    return [];
  }

  const difficultyMap: Record<string, DifficultyLevel> = {
    'easy': DifficultyLevel.Easy,
    'medium': DifficultyLevel.Medium,
    'hard': DifficultyLevel.Hard
  };

  const selectedDifficulty = difficultyMap[uiStore.selectedDifficulty];
  return puzzles.value.filter(p => p.difficulty === selectedDifficulty);
});

/**
 * Charge les puzzles depuis l'API
 */
async function loadPuzzles() {
  try {
    isLoading.value = true;
    error.value = null;

    if (uiStore.selectedDifficulty) {
      const difficultyMap: Record<string, DifficultyLevel> = {
        'easy': DifficultyLevel.Easy,
        'medium': DifficultyLevel.Medium,
        'hard': DifficultyLevel.Hard
      };
      const selectedDifficulty = difficultyMap[uiStore.selectedDifficulty];
      if (selectedDifficulty) {
        // Le backend génère automatiquement un puzzle s'il n'y en a pas
        puzzles.value = await puzzleApi.getByDifficulty(selectedDifficulty);
      }
    } else {
      // Si aucune difficulté n'est sélectionnée, ne pas charger de puzzles
      // On affichera les boutons de sélection de niveau
      puzzles.value = [];
    }
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Erreur lors du chargement des puzzles';
    console.error('Erreur lors du chargement des puzzles:', err);
  } finally {
    isLoading.value = false;
  }
}

/**
 * Sélectionne un niveau de difficulté et charge les puzzles
 */
async function selectLevel(diff: 'easy' | 'medium' | 'hard') {
  uiStore.selectDifficulty(diff);
  await loadPuzzles();
}

/**
 * Retourne à la sélection de niveau (réinitialise la difficulté)
 */
function goBackToLevelSelection() {
  uiStore.selectDifficulty(null);
  puzzles.value = [];
}

/**
 * Démarre un puzzle sélectionné
 */
async function startPuzzle(puzzle: Puzzle) {
  try {
    // Vérifier que l'utilisateur est connecté
    if (!userStore.isLoggedIn) {
      uiStore.openUserModal();
      return;
    }

    // S'assurer qu'une session active existe
    const sessionId = await userStore.ensureActiveSession();

    // Démarrer le jeu
    await gameStore.startGame(puzzle, sessionId);
    uiStore.goToGame();
  } catch (err) {
    error.value = err instanceof Error ? err.message : 'Erreur lors du démarrage du puzzle';
    console.error('Erreur lors du démarrage du puzzle:', err);
  }
}

/**
 * Retourne les classes CSS selon la difficulté
 */
function getDifficultyClasses(difficulty: DifficultyLevel) {
  const classes: Record<DifficultyLevel, string> = {
    [DifficultyLevel.Easy]: 'bg-amber-900/70 border-amber-700 hover:border-amber-400',
    [DifficultyLevel.Medium]: 'bg-orange-900/70 border-orange-700 hover:border-orange-400',
    [DifficultyLevel.Hard]: 'bg-red-900/80 border-red-700 hover:border-red-400',
    [DifficultyLevel.Expert]: 'bg-purple-900/80 border-purple-700 hover:border-purple-400'
  };
  return classes[difficulty] || classes[DifficultyLevel.Easy];
}

function getDifficultyTextColor(difficulty: DifficultyLevel) {
  const colors: Record<DifficultyLevel, string> = {
    [DifficultyLevel.Easy]: 'text-amber-200',
    [DifficultyLevel.Medium]: 'text-orange-200',
    [DifficultyLevel.Hard]: 'text-red-200',
    [DifficultyLevel.Expert]: 'text-purple-200'
  };
  return colors[difficulty] || colors[DifficultyLevel.Easy];
}

function getDifficultyTitleColor(difficulty: DifficultyLevel) {
  const colors: Record<DifficultyLevel, string> = {
    [DifficultyLevel.Easy]: 'text-amber-50',
    [DifficultyLevel.Medium]: 'text-orange-50',
    [DifficultyLevel.Hard]: 'text-red-50',
    [DifficultyLevel.Expert]: 'text-purple-50'
  };
  return colors[difficulty] || colors[DifficultyLevel.Easy];
}

function getDifficultyDescriptionColor(difficulty: DifficultyLevel) {
  const colors: Record<DifficultyLevel, string> = {
    [DifficultyLevel.Easy]: 'text-amber-100',
    [DifficultyLevel.Medium]: 'text-orange-100',
    [DifficultyLevel.Hard]: 'text-red-100',
    [DifficultyLevel.Expert]: 'text-purple-100'
  };
  return colors[difficulty] || colors[DifficultyLevel.Easy];
}

function getDifficultyBadgeColor(difficulty: DifficultyLevel) {
  const colors: Record<DifficultyLevel, string> = {
    [DifficultyLevel.Easy]: 'text-amber-100',
    [DifficultyLevel.Medium]: 'text-orange-100',
    [DifficultyLevel.Hard]: 'text-red-100',
    [DifficultyLevel.Expert]: 'text-purple-100'
  };
  return colors[difficulty] || colors[DifficultyLevel.Easy];
}

function getDifficultyButtonColor(difficulty: DifficultyLevel) {
  const colors: Record<DifficultyLevel, string> = {
    [DifficultyLevel.Easy]: 'bg-amber-500',
    [DifficultyLevel.Medium]: 'bg-orange-500',
    [DifficultyLevel.Hard]: 'bg-red-500',
    [DifficultyLevel.Expert]: 'bg-purple-500'
  };
  return colors[difficulty] || colors[DifficultyLevel.Easy];
}

function getDifficultyLabel(difficulty: DifficultyLevel): string {
  const labels: Record<DifficultyLevel, string> = {
    [DifficultyLevel.Easy]: 'Facile',
    [DifficultyLevel.Medium]: 'Moyen',
    [DifficultyLevel.Hard]: 'Difficile',
    [DifficultyLevel.Expert]: 'Expert'
  };
  return labels[difficulty] || 'Inconnu';
}

function getEstimatedTime(difficulty: DifficultyLevel): string {
  const times: Record<DifficultyLevel, string> = {
    [DifficultyLevel.Easy]: '~ 5-10 min',
    [DifficultyLevel.Medium]: '~ 10-15 min',
    [DifficultyLevel.Hard]: '~ 15-20 min',
    [DifficultyLevel.Expert]: '~ 20+ min'
  };
  return times[difficulty] || '~ ? min';
}

// Ne pas charger les puzzles au montage si aucune difficulté n'est sélectionnée
// On affichera les boutons de sélection de niveau
onMounted(() => {
  if (uiStore.selectedDifficulty) {
    loadPuzzles();
  }
});
</script>
