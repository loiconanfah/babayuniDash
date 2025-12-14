<template>
  <div class="code-game p-6">
    <h3 class="text-2xl font-bold text-slate-50 mb-4 text-center">🔐 Déchiffrez le Code</h3>
    <p class="text-slate-300 mb-4 text-center">Trouvez la combinaison de 4 couleurs !</p>
    
    <div class="mb-6">
      <div class="text-center mb-4">
        <div class="inline-flex gap-2">
          <div
            v-for="(color, index) in secretCode"
            :key="index"
            class="w-12 h-12 rounded-full border-2"
            :class="getColorClass(color)"
          ></div>
        </div>
      </div>
      <div class="text-center text-sm text-slate-400 mb-4">
        Indices : {{ hints.join(' | ') }}
      </div>
    </div>

    <div class="flex justify-center gap-3 mb-6">
      <button
        v-for="(color, index) in availableColors"
        :key="index"
        @click="selectColor(color)"
        class="w-16 h-16 rounded-full border-4 transition-all hover:scale-110"
        :class="getColorClass(color)"
        :disabled="currentGuess.length >= 4 || gameWon"
      ></button>
    </div>

    <div class="mb-4">
      <div class="text-center mb-2 text-slate-300 font-semibold">Votre tentative :</div>
      <div class="flex justify-center gap-2 mb-4">
        <div
          v-for="(color, index) in currentGuess"
          :key="index"
          class="w-12 h-12 rounded-full border-2"
          :class="getColorClass(color)"
        ></div>
        <div
          v-for="i in (4 - currentGuess.length)"
          :key="`empty-${i}`"
          class="w-12 h-12 rounded-full border-2 border-slate-600 bg-slate-800"
        ></div>
      </div>
      <div class="flex justify-center gap-2">
        <button
          @click="clearGuess"
          class="px-4 py-2 rounded-lg bg-slate-700 text-slate-300 hover:bg-slate-600 transition-colors"
          :disabled="currentGuess.length === 0 || gameWon"
        >
          Effacer
        </button>
        <button
          @click="submitGuess"
          class="px-4 py-2 rounded-lg bg-gradient-to-r from-blue-500 to-blue-600 text-white font-semibold hover:from-blue-400 hover:to-blue-500 transition-all"
          :disabled="currentGuess.length !== 4 || gameWon"
        >
          Vérifier
        </button>
      </div>
    </div>

    <div v-if="gameWon" class="text-center">
      <div class="text-4xl mb-4">🎉</div>
      <button
        @click="$emit('win')"
        class="px-6 py-3 rounded-xl bg-gradient-to-r from-green-500 to-green-600 text-white font-bold hover:from-green-400 hover:to-green-500 transition-all"
      >
        ✅ Code déchiffré ! Continuer
      </button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';

const emit = defineEmits<{
  win: [];
}>();

const availableColors = ['red', 'blue', 'green', 'yellow', 'purple', 'orange'];
const secretCode = ref<string[]>([]);
const currentGuess = ref<string[]>([]);
const hints = ref<string[]>([]);
const gameWon = ref(false);

function initGame() {
  // Générer un code secret de 4 couleurs
  secretCode.value = [];
  for (let i = 0; i < 4; i++) {
    const color = availableColors[Math.floor(Math.random() * availableColors.length)];
    if (color) {
      secretCode.value.push(color);
    }
  }

  currentGuess.value = [];
  hints.value = [
    `Le code contient ${secretCode.value.filter(c => c === 'red').length} rouge(s)`,
    `Le code contient ${secretCode.value.filter(c => c === 'blue').length} bleu(s)`,
    'La première couleur est ' + secretCode.value[0]
  ];
  gameWon.value = false;
}

function selectColor(color: string) {
  if (currentGuess.value.length < 4 && !gameWon.value) {
    currentGuess.value.push(color);
  }
}

function clearGuess() {
  currentGuess.value = [];
}

function submitGuess() {
  if (currentGuess.value.length !== 4) return;

  // Vérifier si le code est correct
  let correct = true;
  for (let i = 0; i < 4; i++) {
    if (currentGuess.value[i] !== secretCode.value[i]) {
      correct = false;
      break;
    }
  }

  if (correct) {
    gameWon.value = true;
  } else {
    // Donner un indice
    const correctPositions = currentGuess.value.filter((c, i) => c === secretCode.value[i]).length;
    alert(`Vous avez ${correctPositions} couleur(s) à la bonne position. Réessayez !`);
    currentGuess.value = [];
  }
}

function getColorClass(color: string) {
  const colorMap: Record<string, string> = {
    red: 'bg-red-500 border-red-600',
    blue: 'bg-blue-500 border-blue-600',
    green: 'bg-green-500 border-green-600',
    yellow: 'bg-yellow-500 border-yellow-600',
    purple: 'bg-purple-500 border-purple-600',
    orange: 'bg-orange-500 border-orange-600'
  };
  return colorMap[color] || 'bg-slate-500 border-slate-600';
}

onMounted(() => {
  initGame();
});
</script>

