<template>
  <Teleport to="body">
    <div
      v-if="isOpen"
      class="fixed inset-0 z-[100] flex items-center justify-center p-4 bg-black/60 backdrop-blur-sm animate-fade-in"
      @click.self="close"
    >
      <div class="relative bg-gradient-to-br from-yellow-500/95 via-orange-500/95 to-red-500/95 rounded-3xl shadow-2xl border-2 border-yellow-400/50 max-w-md w-full p-8 animate-scale-in">
        <!-- Confettis animés -->
        <div class="absolute inset-0 overflow-hidden pointer-events-none rounded-3xl">
          <div
            v-for="i in 20"
            :key="i"
            class="confetti"
            :style="{
              left: `${Math.random() * 100}%`,
              animationDelay: `${Math.random() * 2}s`,
              animationDuration: `${2 + Math.random() * 2}s`
            }"
          ></div>
        </div>

        <!-- Contenu -->
        <div class="relative z-10 text-center">
          <!-- Icône de victoire -->
          <div class="mb-6 animate-bounce">
            <div class="inline-block text-8xl">🎉</div>
          </div>

          <!-- Titre -->
          <h2 class="text-4xl font-extrabold text-white mb-4 drop-shadow-lg">
            {{ title }}
          </h2>

          <!-- Message -->
          <p class="text-lg text-white/90 mb-6 font-semibold">
            {{ message }}
          </p>

          <!-- Récompense si applicable -->
          <div v-if="reward > 0" class="mb-6 p-4 rounded-2xl bg-white/20 backdrop-blur-sm border border-white/30">
            <p class="text-sm text-white/80 mb-2">Récompense gagnée</p>
            <div class="flex items-center justify-center gap-2">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8 text-yellow-300" fill="currentColor" viewBox="0 0 20 20">
                <path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 01-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 01-.567.267z" />
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-13a1 1 0 10-2 0v.092a4.535 4.535 0 00-1.676.662C6.602 6.234 6 7.009 6 8c0 .99.602 1.765 1.324 2.246.48.32 1.054.545 1.676.662v1.941c-.391-.127-.68-.317-.843-.504a1 1 0 10-1.51 1.31c.562.649 1.413 1.076 2.353 1.253V15a1 1 0 102 0v-.092a4.535 4.535 0 001.676-.662C13.398 13.766 14 12.991 14 12c0-.99-.602-1.765-1.324-2.246A4.535 4.535 0 0011 9.092V7.151c.391.127.68.317.843.504a1 1 0 101.511-1.31c-.563-.649-1.413-1.076-2.354-1.253V5z" clip-rule="evenodd" />
              </svg>
              <span class="text-3xl font-bold text-white">{{ reward }}</span>
              <span class="text-lg text-white/80">Babayuni</span>
            </div>
          </div>

          <!-- Boutons d'action -->
          <div class="flex gap-3">
            <button
              v-if="showNewGame"
              @click="handleNewGame"
              class="flex-1 px-6 py-3 rounded-xl bg-white text-orange-600 font-bold hover:bg-orange-50 transition-all duration-300 shadow-lg"
            >
              Nouvelle partie
            </button>
            <button
              @click="close"
              class="flex-1 px-6 py-3 rounded-xl bg-white/20 backdrop-blur-sm text-white font-bold hover:bg-white/30 transition-all duration-300 border border-white/30"
            >
              Fermer
            </button>
          </div>
        </div>
      </div>
    </div>
  </Teleport>
</template>

<script setup lang="ts">
import { Teleport } from 'vue'

interface Props {
  isOpen: boolean
  title?: string
  message?: string
  reward?: number
  showNewGame?: boolean
}

const props = withDefaults(defineProps<Props>(), {
  title: '🎉 Victoire !',
  message: 'Félicitations, vous avez gagné !',
  reward: 0,
  showNewGame: false
})

const emit = defineEmits<{
  close: []
  newGame: []
}>()

function close() {
  emit('close')
}

function handleNewGame() {
  emit('newGame')
  close()
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
    transform: scale(0.8);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

@keyframes confetti-fall {
  0% {
    transform: translateY(-100vh) rotate(0deg);
    opacity: 1;
  }
  100% {
    transform: translateY(100vh) rotate(720deg);
    opacity: 0;
  }
}

.animate-fade-in {
  animation: fade-in 0.3s ease-out;
}

.animate-scale-in {
  animation: scale-in 0.4s cubic-bezier(0.34, 1.56, 0.64, 1);
}

.confetti {
  position: absolute;
  width: 10px;
  height: 10px;
  background: linear-gradient(45deg, #ffd700, #ff6b6b, #4ecdc4, #ffe66d);
  border-radius: 2px;
  animation: confetti-fall linear infinite;
}
</style>
