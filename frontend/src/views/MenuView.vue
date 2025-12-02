<script setup lang="ts">
/**
 * Vue du menu principal
 * Point d'entrée de l'application
 */

import { useRouter } from 'vue-router'
import { onMounted, ref } from 'vue'

const router = useRouter()
const isLoaded = ref(false)

onMounted(() => {
  // Animation d'entrée
  setTimeout(() => {
    isLoaded.value = true
  }, 100)
})

/**
 * Navigation vers la sélection de puzzles
 */
function goToSelectPuzzle() {
  router.push('/puzzles')
}

/**
 * Navigation vers la génération d'un nouveau puzzle
 */
function goToGeneratePuzzle() {
  router.push('/generate')
}

/**
 * Navigation vers les statistiques
 */
function goToStats() {
  router.push('/stats')
}
</script>

<template>
  <div class="menu-view">
    <!-- Effet de particules animées en arrière-plan -->
    <div class="background-particles">
      <div class="particle" v-for="i in 20" :key="i" :style="{ 
        left: `${Math.random() * 100}%`,
        animationDelay: `${Math.random() * 5}s`,
        animationDuration: `${3 + Math.random() * 4}s`
      }"></div>
    </div>

    <div class="menu-container" :class="{ 'loaded': isLoaded }">
      <!-- Logo et titre -->
      <div class="menu-header">
        <div class="logo-wrapper">
          <div class="logo-icon">🌉</div>
        </div>
        <h1 class="menu-title">Hashi Prison Break</h1>
        <p class="menu-subtitle">Le jeu de puzzle des ponts</p>
        <div class="title-underline"></div>
      </div>

      <!-- Boutons du menu -->
      <div class="menu-actions">
        <button class="menu-btn menu-btn--primary" @click="goToSelectPuzzle">
          <span class="menu-btn__icon">🎮</span>
          <span class="menu-btn__text">Jouer à un puzzle</span>
          <span class="menu-btn__arrow">→</span>
        </button>

        <button class="menu-btn menu-btn--secondary" @click="goToGeneratePuzzle">
          <span class="menu-btn__icon">✨</span>
          <span class="menu-btn__text">Générer un nouveau puzzle</span>
          <span class="menu-btn__arrow">→</span>
        </button>

        <button class="menu-btn menu-btn--secondary" @click="goToStats">
          <span class="menu-btn__icon">📊</span>
          <span class="menu-btn__text">Statistiques & Classement</span>
          <span class="menu-btn__arrow">→</span>
        </button>
      </div>

      <!-- Instructions -->
      <div class="menu-instructions">
        <div class="instructions-header">
          <h3 class="instructions-title">📖 Comment jouer ?</h3>
        </div>
        <ul class="instructions-list">
          <li>
            <span class="instruction-icon">🔗</span>
            <span>Reliez toutes les îles avec des ponts</span>
          </li>
          <li>
            <span class="instruction-icon">🔢</span>
            <span>Le nombre sur chaque île indique combien de ponts doivent y être connectés</span>
          </li>
          <li>
            <span class="instruction-icon">🌉</span>
            <span>Les ponts peuvent être simples ou doubles</span>
          </li>
          <li>
            <span class="instruction-icon">❌</span>
            <span>Les ponts ne peuvent pas se croiser</span>
          </li>
          <li>
            <span class="instruction-icon">🌐</span>
            <span>Toutes les îles doivent être connectées en un seul réseau</span>
          </li>
        </ul>
      </div>
    </div>
  </div>
</template>

<style scoped>
.menu-view {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(135deg, #1e1b4b 0%, #312e81 25%, #4c1d95 50%, #5b21b6 75%, #6d28d9 100%);
  background-size: 400% 400%;
  animation: gradientShift 15s ease infinite;
  padding: 2rem;
  position: relative;
  overflow: hidden;
}

@keyframes gradientShift {
  0% {
    background-position: 0% 50%;
  }
  50% {
    background-position: 100% 50%;
  }
  100% {
    background-position: 0% 50%;
  }
}

/* Particules animées en arrière-plan */
.background-particles {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  overflow: hidden;
  z-index: 0;
}

.particle {
  position: absolute;
  width: 4px;
  height: 4px;
  background: rgba(255, 255, 255, 0.5);
  border-radius: 50%;
  animation: float linear infinite;
  box-shadow: 0 0 10px rgba(255, 255, 255, 0.5);
}

@keyframes float {
  0% {
    transform: translateY(100vh) translateX(0);
    opacity: 0;
  }
  10% {
    opacity: 1;
  }
  90% {
    opacity: 1;
  }
  100% {
    transform: translateY(-100px) translateX(100px);
    opacity: 0;
  }
}

.menu-container {
  max-width: 700px;
  width: 100%;
  position: relative;
  z-index: 1;
  opacity: 0;
  transform: translateY(30px);
  transition: all 0.8s cubic-bezier(0.4, 0, 0.2, 1);
}

.menu-container.loaded {
  opacity: 1;
  transform: translateY(0);
}

/* Header */
.menu-header {
  text-align: center;
  margin-bottom: 3.5rem;
  color: white;
  animation: fadeInDown 0.8s ease-out;
}

@keyframes fadeInDown {
  from {
    opacity: 0;
    transform: translateY(-30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.logo-wrapper {
  margin-bottom: 1.5rem;
  display: inline-block;
  animation: pulse 2s ease-in-out infinite;
}

@keyframes pulse {
  0%, 100% {
    transform: scale(1);
  }
  50% {
    transform: scale(1.05);
  }
}

.logo-icon {
  font-size: 5rem;
  filter: drop-shadow(0 0 20px rgba(255, 255, 255, 0.3));
  display: inline-block;
  animation: rotate 20s linear infinite;
}

@keyframes rotate {
  from {
    transform: rotate(0deg);
  }
  to {
    transform: rotate(360deg);
  }
}

.menu-title {
  font-size: 3.5rem;
  font-weight: 800;
  margin: 0 0 1rem 0;
  text-shadow: 
    0 0 20px rgba(255, 255, 255, 0.3),
    0 4px 8px rgba(0, 0, 0, 0.3),
    0 8px 16px rgba(0, 0, 0, 0.2);
  background: linear-gradient(135deg, #ffffff 0%, #e0e7ff 100%);
  -webkit-background-clip: text;
  -webkit-text-fill-color: transparent;
  background-clip: text;
  letter-spacing: -0.02em;
}

.menu-subtitle {
  font-size: 1.5rem;
  opacity: 0.95;
  margin: 0 0 1rem 0;
  font-weight: 300;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.title-underline {
  width: 100px;
  height: 4px;
  background: linear-gradient(90deg, transparent, #ffffff, transparent);
  margin: 1rem auto 0;
  border-radius: 2px;
  box-shadow: 0 0 10px rgba(255, 255, 255, 0.5);
}

/* Actions */
.menu-actions {
  display: flex;
  flex-direction: column;
  gap: 1.25rem;
  margin-bottom: 3rem;
}

.menu-btn {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 1rem;
  padding: 1.5rem 2rem;
  border: none;
  border-radius: 1.25rem;
  font-size: 1.2rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.4s cubic-bezier(0.4, 0, 0.2, 1);
  box-shadow: 
    0 4px 6px rgba(0, 0, 0, 0.1),
    0 0 0 0 rgba(255, 255, 255, 0);
  position: relative;
  overflow: hidden;
  backdrop-filter: blur(10px);
}

.menu-btn::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 50%;
  width: 0;
  height: 0;
  border-radius: 50%;
  background: rgba(255, 255, 255, 0.2);
  transform: translate(-50%, -50%);
  transition: width 0.6s, height 0.6s;
}

.menu-btn:hover::before {
  width: 300px;
  height: 300px;
}

.menu-btn:hover {
  transform: translateY(-6px) scale(1.02);
  box-shadow: 
    0 12px 24px rgba(0, 0, 0, 0.2),
    0 0 0 4px rgba(255, 255, 255, 0.1);
}

.menu-btn:active {
  transform: translateY(-2px) scale(0.98);
}

.menu-btn--primary {
  background: linear-gradient(135deg, #ffffff 0%, #f8fafc 100%);
  color: #4c1d95;
  border: 2px solid rgba(255, 255, 255, 0.3);
}

.menu-btn--primary:hover {
  background: linear-gradient(135deg, #ffffff 0%, #e0e7ff 100%);
  color: #5b21b6;
}

.menu-btn--secondary {
  background: rgba(255, 255, 255, 0.15);
  color: white;
  border: 2px solid rgba(255, 255, 255, 0.3);
  backdrop-filter: blur(20px);
}

.menu-btn--secondary:hover {
  background: rgba(255, 255, 255, 0.25);
  border-color: rgba(255, 255, 255, 0.5);
}

.menu-btn__icon {
  font-size: 2rem;
  filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.2));
  transition: transform 0.3s ease;
}

.menu-btn:hover .menu-btn__icon {
  transform: scale(1.2) rotate(10deg);
}

.menu-btn__text {
  flex: 1;
  text-align: left;
  text-shadow: 0 1px 2px rgba(0, 0, 0, 0.1);
}

.menu-btn__arrow {
  font-size: 1.5rem;
  opacity: 0;
  transform: translateX(-10px);
  transition: all 0.3s ease;
}

.menu-btn:hover .menu-btn__arrow {
  opacity: 1;
  transform: translateX(0);
}

/* Instructions */
.menu-instructions {
  background: rgba(255, 255, 255, 0.1);
  backdrop-filter: blur(20px);
  border-radius: 1.5rem;
  padding: 2.5rem;
  color: white;
  border: 1px solid rgba(255, 255, 255, 0.2);
  box-shadow: 
    0 8px 32px rgba(0, 0, 0, 0.1),
    inset 0 1px 0 rgba(255, 255, 255, 0.2);
  animation: slideInUp 0.8s ease-out 0.2s both;
}

@keyframes slideInUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.instructions-header {
  margin-bottom: 1.5rem;
  padding-bottom: 1rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.2);
}

.instructions-title {
  margin: 0;
  font-size: 1.75rem;
  font-weight: 700;
  text-shadow: 0 2px 4px rgba(0, 0, 0, 0.2);
}

.instructions-list {
  margin: 0;
  padding: 0;
  list-style: none;
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.instructions-list li {
  display: flex;
  align-items: flex-start;
  gap: 1rem;
  padding: 0.75rem;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 0.75rem;
  transition: all 0.3s ease;
  border-left: 3px solid transparent;
}

.instructions-list li:hover {
  background: rgba(255, 255, 255, 0.1);
  border-left-color: rgba(255, 255, 255, 0.5);
  transform: translateX(5px);
}

.instruction-icon {
  font-size: 1.5rem;
  flex-shrink: 0;
  filter: drop-shadow(0 2px 4px rgba(0, 0, 0, 0.2));
}

.instructions-list li span:last-child {
  flex: 1;
  line-height: 1.6;
  font-size: 1rem;
}

/* Responsive */
@media (max-width: 768px) {
  .menu-view {
    padding: 1.5rem;
  }

  .menu-title {
    font-size: 2.5rem;
  }

  .menu-subtitle {
    font-size: 1.25rem;
  }

  .menu-btn {
    font-size: 1rem;
    padding: 1.25rem 1.5rem;
  }

  .menu-btn__icon {
    font-size: 1.5rem;
  }

  .menu-instructions {
    padding: 1.5rem;
  }

  .logo-icon {
    font-size: 4rem;
  }
}

@media (max-width: 480px) {
  .menu-title {
    font-size: 2rem;
  }

  .menu-subtitle {
    font-size: 1rem;
  }

  .menu-btn__text {
    font-size: 0.9rem;
  }
}
</style>

