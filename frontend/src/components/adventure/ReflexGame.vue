<template>
  <div class="reflex-game p-6">
    <h3 class="text-2xl font-bold text-slate-50 mb-4 text-center">⚡ Jeu de Réflexe</h3>
    <p class="text-slate-300 mb-4 text-center">Cliquez quand la barre est dans la zone verte !</p>
    
    <div class="mb-6">
      <div class="relative w-full h-8 bg-slate-800 rounded-full border-2 border-slate-700 overflow-hidden">
        <div
          class="absolute top-0 left-0 h-full bg-gradient-to-r from-red-500 via-yellow-500 to-green-500 transition-all duration-75"
          :style="{ width: `${barPosition}%` }"
        ></div>
        <div
          class="absolute top-0 w-2 h-full bg-white border border-slate-900 z-10 transition-all duration-75"
          :style="{ left: `${targetZone}%` }"
        ></div>
        <div
          class="absolute top-0 w-2 h-full bg-white border border-slate-900 z-10 transition-all duration-75"
          :style="{ left: `${targetZone + 20}%` }"
        ></div>
      </div>
      <div class="text-center mt-2 text-sm text-slate-400">
        Zone cible (entre les lignes blanches)
      </div>
    </div>

    <div class="text-center mb-4">
      <div class="text-lg font-semibold text-slate-300 mb-2">
        Score : {{ score }} | Meilleur : {{ bestScore }}
      </div>
      <div v-if="!gameStarted" class="text-yellow-400 font-semibold mb-4">
        Prêt ? Cliquez pour commencer !
      </div>
      <div v-else-if="waitingForClick" class="text-green-400 font-semibold mb-4">
        👆 Cliquez maintenant !
      </div>
      <div v-else class="text-slate-400 font-semibold mb-4">
        Attendez...
      </div>
    </div>

    <div class="text-center">
      <button
        v-if="!gameStarted"
        @click="startGame"
        class="px-8 py-4 rounded-xl bg-gradient-to-r from-green-500 to-green-600 text-white font-bold hover:from-green-400 hover:to-green-500 transition-all text-lg"
      >
        🚀 Commencer
      </button>
      <button
        v-else
        @click="handleClick"
        class="px-8 py-4 rounded-xl bg-gradient-to-r from-blue-500 to-blue-600 text-white font-bold hover:from-blue-400 hover:to-blue-500 transition-all text-lg"
        :disabled="!waitingForClick"
      >
        Cliquez !
      </button>
    </div>

    <div v-if="gameWon" class="text-center mt-6">
      <div class="text-4xl mb-4">🎉</div>
      <button
        @click="$emit('win')"
        class="px-6 py-3 rounded-xl bg-gradient-to-r from-green-500 to-green-600 text-white font-bold hover:from-green-400 hover:to-green-500 transition-all"
      >
        ✅ Réussi ! Continuer
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';

const emit = defineEmits<{
  win: [];
}>();

const barPosition = ref(0);
const targetZone = ref(0);
const score = ref(0);
const bestScore = ref(0);
const gameStarted = ref(false);
const waitingForClick = ref(false);
const gameWon = ref(false);
let animationInterval: number | null = null;
let clickTimeout: number | null = null;

function startGame() {
  gameStarted.value = true;
  score.value = 0;
  gameWon.value = false;
  startRound();
}

function startRound() {
  barPosition.value = 0;
  targetZone.value = Math.random() * 60; // Zone entre 0% et 60%
  waitingForClick.value = false;

  // Attendre un délai aléatoire avant d'activer le clic
  const delay = 1000 + Math.random() * 2000;
  clickTimeout = window.setTimeout(() => {
    waitingForClick.value = true;
  }, delay);

  // Animer la barre
  animationInterval = window.setInterval(() => {
    barPosition.value += 2;
    if (barPosition.value >= 100) {
      // Trop tard !
      endRound(false);
    }
  }, 50);
}

function handleClick() {
  if (!waitingForClick.value) {
    // Trop tôt !
    endRound(false);
    return;
  }

  // Vérifier si dans la zone
  const inZone = barPosition.value >= targetZone.value && barPosition.value <= targetZone.value + 20;
  endRound(inZone);
}

function endRound(success: boolean) {
  if (animationInterval) {
    clearInterval(animationInterval);
    animationInterval = null;
  }
  if (clickTimeout) {
    clearTimeout(clickTimeout);
    clickTimeout = null;
  }

  if (success) {
    score.value += 10;
    if (score.value > bestScore.value) {
      bestScore.value = score.value;
    }

    if (score.value >= 50) {
      // Gagné !
      gameWon.value = true;
    } else {
      setTimeout(() => {
        startRound();
      }, 1000);
    }
  } else {
    // Perdu, réessayer
    setTimeout(() => {
      startRound();
    }, 1000);
  }
}

onMounted(() => {
  // Initialisation
});

onUnmounted(() => {
  if (animationInterval) clearInterval(animationInterval);
  if (clickTimeout) clearTimeout(clickTimeout);
});
</script>

