<template>
  <div class="fixed inset-0 bg-black/70 backdrop-blur-sm flex items-center justify-center z-50 animate-fade-in">
    <div
      class="bg-gradient-to-br from-slate-900 via-slate-800 to-slate-900 rounded-3xl shadow-2xl w-full max-w-md border-2 border-amber-500/50 overflow-hidden animate-scale-in"
    >
      <!-- En-tête avec effet de lumière -->
      <div class="relative bg-gradient-to-r from-amber-600 via-orange-500 to-amber-600 p-6 text-center overflow-hidden">
        <div class="absolute inset-0 bg-[radial-gradient(circle_at_center,_rgba(255,255,255,0.3)_0%,_transparent_70%)]"></div>
        <div class="relative z-10">
          <!-- Icône de victoire -->
          <div class="mb-4 flex justify-center">
            <div class="relative">
              <div class="absolute inset-0 bg-amber-400 rounded-full blur-xl opacity-50 animate-pulse"></div>
              <div class="relative bg-amber-500 rounded-full w-20 h-20 flex items-center justify-center text-4xl shadow-lg">
                🎉
              </div>
            </div>
          </div>
          <h2 class="text-3xl font-bold text-white mb-2 drop-shadow-lg">
            Évasion Réussie !
          </h2>
          <p class="text-amber-100 text-sm">
            Tous les verrous ont été déverrouillés
          </p>
        </div>
      </div>

      <!-- Contenu -->
      <div class="p-6 space-y-4">
        <!-- Statistiques -->
        <div class="grid grid-cols-2 gap-4">
          <div class="bg-slate-800/50 rounded-xl p-4 border border-slate-700">
            <p class="text-xs uppercase tracking-wider text-slate-400 mb-1">
              Temps
            </p>
            <p class="text-2xl font-bold text-amber-400 font-mono">
              {{ formattedTime }}
            </p>
          </div>
          <div class="bg-slate-800/50 rounded-xl p-4 border border-slate-700">
            <p class="text-xs uppercase tracking-wider text-slate-400 mb-1">
              Score
            </p>
            <p class="text-2xl font-bold text-green-400">
              {{ score }}
            </p>
          </div>
        </div>

        <!-- Message de félicitations -->
        <div class="bg-slate-800/30 rounded-xl p-4 border border-slate-700/50">
          <p class="text-slate-200 text-center text-sm leading-relaxed">
            {{ message }}
          </p>
        </div>

        <!-- Boutons d'action -->
        <div class="flex flex-col gap-3 pt-2">
          <button
            class="w-full px-6 py-3 rounded-xl bg-gradient-to-r from-amber-600 to-orange-600 text-white font-semibold hover:from-amber-500 hover:to-orange-500 transition-all transform hover:scale-105 shadow-lg"
            @click="handleNextLevel"
          >
            Niveau Suivant
          </button>
          <button
            class="w-full px-6 py-3 rounded-xl bg-slate-700 text-slate-200 font-semibold hover:bg-slate-600 transition-colors"
            @click="handleChangeLevel"
          >
            Changer de Niveau
          </button>
          <button
            class="w-full px-6 py-3 rounded-xl bg-slate-800/50 text-slate-400 font-medium hover:bg-slate-700/50 hover:text-slate-300 transition-colors"
            @click="handleGoHome"
          >
            Retour à l'Accueil
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useGameStore } from '@/stores/game';
import { useUiStore } from '@/stores/ui';
import { useUserStore } from '@/stores/user';
import { useStatsStore } from '@/stores/stats';
import { puzzleApi } from '@/services/api';
import { DifficultyLevel } from '@/types';
import { formatTime } from '@/utils/helpers';

const gameStore = useGameStore();
const uiStore = useUiStore();
const userStore = useUserStore();
const statsStore = useStatsStore();

/**
 * Temps formaté
 */
const formattedTime = computed(() => {
  return formatTime(gameStore.elapsedTime);
});

/**
 * Score calculé (basé sur le temps et les erreurs)
 */
const score = computed(() => {
  // Utiliser le score de la partie si disponible
  if (gameStore.currentGame?.score) {
    return gameStore.currentGame.score;
  }
  // Fallback : calcul basé sur le temps (plus rapide = meilleur score)
  const timeBonus = Math.max(0, 10000 - gameStore.elapsedTime * 10);
  return Math.floor(timeBonus);
});

/**
 * Recharger les statistiques après une victoire
 */
async function reloadStats() {
  if (userStore.user?.email) {
    try {
      await statsStore.loadUserStatsByEmail(userStore.user.email)
      await statsStore.loadLeaderboard(10)
    } catch (err) {
      console.log('Erreur lors du rechargement des statistiques:', err)
    }
  }
}

/**
 * Message de félicitations personnalisé
 */
const message = computed(() => {
  const time = gameStore.elapsedTime;
  if (time < 60) {
    return 'Évasion éclair ! Tu es un véritable maître de l\'évasion !';
  } else if (time < 180) {
    return 'Excellente performance ! Tu as réussi à t\'échapper avec brio.';
  } else {
    return 'Bien joué ! Tu as réussi à déverrouiller tous les verrous.';
  }
});

/**
 * Passe au niveau suivant (difficulté supérieure)
 */
async function handleNextLevel() {
  try {
    // Recharger les statistiques avant de continuer
    await reloadStats()
    
    uiStore.closeVictoryModal();
    
    // Récupérer la difficulté actuelle
    const currentDifficulty = gameStore.currentPuzzle?.difficulty;
    if (!currentDifficulty) {
      uiStore.goToLevels();
      return;
    }

    // Calculer la difficulté supérieure
    let nextDifficulty: DifficultyLevel;
    switch (currentDifficulty) {
      case DifficultyLevel.Easy:
        nextDifficulty = DifficultyLevel.Medium;
        break;
      case DifficultyLevel.Medium:
        nextDifficulty = DifficultyLevel.Hard;
        break;
      case DifficultyLevel.Hard:
        nextDifficulty = DifficultyLevel.Expert;
        break;
      case DifficultyLevel.Expert:
        // Si on est déjà à Expert, on reste à Expert
        nextDifficulty = DifficultyLevel.Expert;
        break;
      default:
        nextDifficulty = DifficultyLevel.Medium;
    }

    // Réinitialiser le jeu actuel
    gameStore.resetGame();

    // Charger les puzzles de la difficulté supérieure
    const puzzles = await puzzleApi.getByDifficulty(nextDifficulty);
    
    if (puzzles.length === 0) {
      alert('Aucun puzzle disponible pour le niveau supérieur. Retour à la sélection de niveaux.');
      uiStore.goToLevels();
      return;
    }

    // Prendre le premier puzzle disponible
    const nextPuzzle = puzzles[0];
    
    if (!nextPuzzle) {
      throw new Error('Aucun puzzle disponible pour le niveau suivant');
    }

    // S'assurer qu'une session active existe
    const sessionId = await userStore.ensureActiveSession();

    // Démarrer le nouveau jeu
    await gameStore.startGame(nextPuzzle, sessionId);
    uiStore.goToGame();
  } catch (error) {
    console.error('Erreur lors du passage au niveau suivant:', error);
    alert('Erreur lors du chargement du niveau suivant. Retour à la sélection de niveaux.');
    uiStore.goToLevels();
  }
}

/**
 * Change de niveau
 */
async function handleChangeLevel() {
  await reloadStats()
  uiStore.closeVictoryModal();
  uiStore.goToLevels();
}

/**
 * Retourne à l'accueil
 */
async function handleGoHome() {
  await reloadStats()
  uiStore.closeVictoryModal();
  gameStore.resetGame();
  uiStore.goToHome();
}
</script>

<style scoped>
@keyframes fade-in {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes scale-in {
  from {
    opacity: 0;
    transform: scale(0.9);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

.animate-fade-in {
  animation: fade-in 0.3s ease-out;
}

.animate-scale-in {
  animation: scale-in 0.3s ease-out;
}
</style>

