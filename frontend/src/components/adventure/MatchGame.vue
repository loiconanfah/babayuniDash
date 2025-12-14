<template>
  <div class="match-game p-6">
    <h3 class="text-2xl font-bold text-slate-50 mb-4 text-center">🔍 Jeu de Correspondance</h3>
    <p class="text-slate-300 mb-4 text-center">Trouvez toutes les paires !</p>
    
    <div class="grid grid-cols-4 gap-3 mb-4">
      <button
        v-for="(card, index) in cards"
        :key="index"
        @click="flipCard(index)"
        class="aspect-square rounded-xl border-2 transition-all duration-300 hover:scale-105"
        :class="getCardClass(card)"
        :disabled="card.matched || (flippedCards.length >= 2 && !card.flipped)"
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
        Paires trouvées : {{ matchedPairs }} / {{ totalPairs }}
      </div>
      <button
        v-if="gameWon"
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

const symbols = ['🎴', '🎯', '🎲', '🎪', '🎨', '🎭', '🎬', '🎮'];
const cards = ref<Array<{ value: string; flipped: boolean; matched: boolean }>>([]);
const flippedCards = ref<number[]>([]);
const matchedPairs = ref(0);
const totalPairs = ref(4);
const gameWon = ref(false);

function initGame() {
  const pairs = symbols.slice(0, totalPairs.value);
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

  flippedCards.value = [];
  matchedPairs.value = 0;
  gameWon.value = false;
}

function flipCard(index: number) {
  const card = cards.value[index];
  if (!card || card.flipped || card.matched || flippedCards.value.length >= 2) return;

  card.flipped = true;
  flippedCards.value.push(index);

  if (flippedCards.value.length === 2) {
    const [first, second] = flippedCards.value;
    const firstCard = cards.value[first];
    const secondCard = cards.value[second];
    if (firstCard && secondCard && firstCard.value === secondCard.value) {
      // Paire trouvée !
      firstCard.matched = true;
      secondCard.matched = true;
      matchedPairs.value++;
      flippedCards.value = [];

      if (matchedPairs.value === totalPairs.value) {
        setTimeout(() => {
          gameWon.value = true;
        }, 500);
      }
    } else {
      // Pas une paire, retourner les cartes
      setTimeout(() => {
        if (firstCard) firstCard.flipped = false;
        if (secondCard) secondCard.flipped = false;
        flippedCards.value = [];
      }, 1000);
    }
  }
}

function getCardClass(card: { value: string; flipped: boolean; matched: boolean }) {
  if (card.matched) {
    return 'bg-green-500/50 border-green-500';
  }
  if (card.flipped) {
    return 'bg-blue-500/50 border-blue-500';
  }
  return 'bg-slate-700 border-slate-600 hover:border-slate-500';
}

onMounted(() => {
  initGame();
});
</script>

