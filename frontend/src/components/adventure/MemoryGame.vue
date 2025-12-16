<template>
  <div class="memory-game p-6">
    <h3 class="text-2xl font-bold text-slate-50 mb-4 text-center">🎴 Jeu de Mémoire</h3>
    <p class="text-slate-300 mb-4 text-center">Mémorisez l'ordre des cartes et cliquez-les dans le bon ordre !</p>
    
    <div class="grid grid-cols-4 gap-3 mb-4">
      <button
        v-for="(card, index) in cards"
        :key="index"
        @click="flipCard(index)"
        class="aspect-square rounded-xl border-2 transition-all duration-300"
        :class="getCardClass(card, index)"
        :disabled="isRevealing || card.flipped || card.matched"
      >
        <div v-if="card.flipped || card.matched" class="w-full h-full flex items-center justify-center text-4xl">
          {{ card.value }}
        </div>
        <div v-else class="w-full h-full flex items-center justify-center text-4xl">
          ❓
        </div>
      </button>
    </div>

    <div class="text-center">
      <div class="text-lg font-semibold text-slate-300 mb-2">
        Clics : {{ clicks }} / {{ sequence.length }}
      </div>
      <button
        v-if="gameWon"
        @click="$emit('win')"
        class="px-6 py-3 rounded-xl bg-gradient-to-r from-green-500 to-green-600 text-white font-bold hover:from-green-400 hover:to-green-500 transition-all"
      >
        ✅ Réussi ! Continuer
      </button>
      <button
        v-else-if="gameLost"
        @click="restart"
        class="px-6 py-3 rounded-xl bg-gradient-to-r from-red-500 to-red-600 text-white font-bold hover:from-red-400 hover:to-red-500 transition-all"
      >
        🔄 Réessayer
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';

const emit = defineEmits<{
  win: [];
}>();

const cards = ref<Array<{ value: string; flipped: boolean; matched: boolean }>>([]);
const sequence = ref<number[]>([]);
const clicks = ref(0);
const isRevealing = ref(false);
const gameWon = ref(false);
const gameLost = ref(false);

const symbols = ['🎴', '🎯', '🎲', '🎪', '🎨', '🎭', '🎬', '🎮'];

function initGame() {
  // Créer 8 cartes avec 4 paires
  const pairs = symbols.slice(0, 4);
  const allCards = [...pairs, ...pairs];
  
  // Mélanger
  for (let i = allCards.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1));
    const temp = allCards[i];
    allCards[i] = allCards[j]!;
    allCards[j] = temp!;
  }

  cards.value = allCards.map(value => ({
    value,
    flipped: false,
    matched: false
  }));

  // Créer la séquence à mémoriser (ordre des indices)
  sequence.value = [];
  const indices = Array.from({ length: 8 }, (_, i) => i);
  for (let i = indices.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1));
    const temp = indices[i];
    indices[i] = indices[j]!;
    indices[j] = temp!;
  }
  sequence.value = indices.slice(0, 4);

  clicks.value = 0;
  gameWon.value = false;
  gameLost.value = false;

  // Révéler la séquence
  revealSequence();
}

function revealSequence() {
  isRevealing.value = true;
  sequence.value.forEach((index, i) => {
    setTimeout(() => {
      const card = cards.value[index];
      if (card) {
        card.flipped = true;
        setTimeout(() => {
          if (card) card.flipped = false;
          if (i === sequence.value.length - 1) {
            isRevealing.value = false;
          }
        }, 800);
      }
    }, i * 1000);
  });
}

function flipCard(index: number) {
  const card = cards.value[index];
  if (isRevealing.value || !card || card.flipped || card.matched) return;

  card.flipped = true;
  clicks.value++;

  // Vérifier si c'est le bon ordre
  const expectedIndex = sequence.value[clicks.value - 1];
  if (index === expectedIndex) {
    card.matched = true;
    if (clicks.value === sequence.value.length) {
      // Gagné !
      setTimeout(() => {
        gameWon.value = true;
      }, 500);
    }
  } else {
    // Perdu
    setTimeout(() => {
      if (card) card.flipped = false;
      gameLost.value = true;
    }, 1000);
  }
}

function getCardClass(card: { value: string; flipped: boolean; matched: boolean }, index: number) {
  if (card.matched) {
    return 'bg-green-500/50 border-green-500';
  }
  if (card.flipped) {
    return 'bg-blue-500/50 border-blue-500';
  }
  return 'bg-slate-700 border-slate-600 hover:border-slate-500';
}

function restart() {
  initGame();
}

onMounted(() => {
  initGame();
});
</script>

