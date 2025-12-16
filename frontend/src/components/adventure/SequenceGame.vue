<template>
  <div class="sequence-game p-6">
    <h3 class="text-2xl font-bold text-slate-50 mb-4 text-center">🎨 Jeu de Séquence</h3>
    <p class="text-slate-300 mb-4 text-center">Mémorisez et répétez la séquence de couleurs !</p>
    
    <div class="flex justify-center gap-4 mb-6">
      <button
        v-for="(color, index) in colors"
        :key="index"
        @click="selectColor(index)"
        class="w-20 h-20 rounded-full border-4 transition-all hover:scale-110"
        :class="getColorClass(color)"
        :disabled="isShowingSequence || gameWon"
      >
      </button>
    </div>

    <div class="text-center mb-4">
      <div class="text-lg font-semibold text-slate-300 mb-2">
        Niveau : {{ level }} | Score : {{ score }}
      </div>
      <div v-if="isShowingSequence" class="text-yellow-400 font-semibold">
        👀 Regardez attentivement...
      </div>
      <div v-else-if="!gameWon" class="text-green-400 font-semibold">
        ✨ Votre tour ! Répétez la séquence
      </div>
    </div>

    <div v-if="gameWon" class="text-center">
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
import { ref, onMounted } from 'vue';

const emit = defineEmits<{
  win: [];
}>();

const colors = ['red', 'blue', 'green', 'yellow'];
const sequence = ref<number[]>([]);
const playerSequence = ref<number[]>([]);
const level = ref(1);
const score = ref(0);
const isShowingSequence = ref(false);
const gameWon = ref(false);

function initGame() {
  level.value = 1;
  score.value = 0;
  sequence.value = [];
  playerSequence.value = [];
  gameWon.value = false;
  nextLevel();
}

function nextLevel() {
  // Ajouter une nouvelle couleur à la séquence
  sequence.value.push(Math.floor(Math.random() * colors.length));
  playerSequence.value = [];
  isShowingSequence.value = true;

  // Afficher la séquence
  sequence.value.forEach((colorIndex, i) => {
    setTimeout(() => {
      highlightColor(colorIndex);
      if (i === sequence.value.length - 1) {
        setTimeout(() => {
          isShowingSequence.value = false;
        }, 600);
      }
    }, i * 800);
  });
}

function highlightColor(index: number) {
  // Animation visuelle (gérée par CSS)
  const button = document.querySelector(`button:nth-child(${index + 1})`) as HTMLElement;
  if (button) {
    button.style.transform = 'scale(1.3)';
    setTimeout(() => {
      button.style.transform = '';
    }, 300);
  }
}

function selectColor(index: number) {
  if (isShowingSequence.value || gameWon.value) return;

  playerSequence.value.push(index);
  highlightColor(index);

  // Vérifier si c'est correct
  const expectedIndex = sequence.value[playerSequence.value.length - 1];
  if (index !== expectedIndex) {
    // Perdu
    setTimeout(() => {
      alert('Mauvaise séquence ! Réessayez.');
      initGame();
    }, 500);
    return;
  }

  // Si la séquence est complète
  if (playerSequence.value.length === sequence.value.length) {
    score.value += level.value * 10;
    level.value++;

    if (level.value > 5) {
      // Gagné !
      gameWon.value = true;
    } else {
      setTimeout(() => {
        nextLevel();
      }, 1000);
    }
  }
}

function getColorClass(color: string) {
  const colorMap: Record<string, string> = {
    red: 'bg-red-500 border-red-600 hover:bg-red-400',
    blue: 'bg-blue-500 border-blue-600 hover:bg-blue-400',
    green: 'bg-green-500 border-green-600 hover:bg-green-400',
    yellow: 'bg-yellow-500 border-yellow-600 hover:bg-yellow-400'
  };
  return colorMap[color] || 'bg-slate-500 border-slate-600';
}

onMounted(() => {
  initGame();
});
</script>

