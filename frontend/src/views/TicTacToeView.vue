<template>
  <div class="tic-tac-toe-view">
    <div class="container">
      <!-- En-tête -->
      <div class="header">
        <h1>Tic-Tac-Toe</h1>
        <button v-if="currentGame" @click="handleAbandon" class="btn btn-danger">
          Abandonner
        </button>
        <button v-else @click="goBack" class="btn btn-secondary">
          Retour
        </button>
      </div>

      <!-- Message d'erreur -->
      <div v-if="error" class="alert alert-error">
        {{ error }}
        <button @click="clearError" class="close-btn">×</button>
      </div>

      <!-- Chargement -->
      <div v-if="isLoading" class="loading">
        <p>Chargement...</p>
      </div>

      <!-- Partie en cours -->
      <div v-else-if="currentGame" class="game-container">
        <!-- Informations des joueurs -->
        <div class="players-info">
          <div class="player" :class="{ active: currentGame.currentPlayer === 1 }">
            <div class="player-symbol">X</div>
            <div class="player-name">
              {{ currentGame.player1Name || 'Joueur 1' }}
              <span v-if="playerNumber === 1" class="you-badge">(Vous)</span>
            </div>
            <div v-if="currentGame.winnerPlayerId === 1" class="winner-badge">🏆 Gagnant</div>
          </div>

          <div class="vs">VS</div>

          <div class="player" :class="{ active: currentGame.currentPlayer === 2 }">
            <div class="player-symbol">O</div>
            <div class="player-name">
              {{ currentGame.player2Name || (currentGame.gameMode === GameMode.AI ? 'IA 🤖' : 'Joueur 2') }}
              <span v-if="playerNumber === 2 && currentGame.gameMode === GameMode.Player" class="you-badge">(Vous)</span>
            </div>
            <div v-if="currentGame.winnerPlayerId === 2" class="winner-badge">🏆 Gagnant</div>
          </div>
        </div>

        <!-- Statut de la partie -->
        <div class="game-status">
          <div v-if="currentGame.status === 1" class="status-waiting">
            ⏳ En attente d'un deuxième joueur...
          </div>
          <div v-else-if="currentGame.status === 2" class="status-in-progress">
            <span v-if="isMyTurn">✅ C'est votre tour !</span>
            <span v-else-if="currentGame.gameMode === GameMode.AI">🤖 L'IA réfléchit...</span>
            <span v-else>⏸️ En attente de l'adversaire...</span>
          </div>
          <div v-else-if="currentGame.status === 3" class="status-completed">
            🎉 Partie terminée !
          </div>
          <div v-else-if="currentGame.status === 4" class="status-draw">
            🤝 Match nul !
          </div>
          <div v-else-if="currentGame.status === 5" class="status-abandoned">
            ❌ Partie abandonnée
          </div>
        </div>

        <!-- Grille de jeu -->
        <TicTacToeBoard
          :game="currentGame"
          @cell-click="handleCellClick"
        />

        <!-- Statistiques -->
        <div class="game-stats">
          <div class="stat">
            <span class="stat-label">Coups joués:</span>
            <span class="stat-value">{{ currentGame.moveCount }}</span>
          </div>
          <div class="stat">
            <span class="stat-label">Temps:</span>
            <span class="stat-value">{{ formatTime(currentGame.elapsedSeconds) }}</span>
          </div>
        </div>
      </div>

      <!-- Menu principal (pas de partie) -->
      <div v-else class="menu-container">
        <div class="game-mode-selection">
          <h2>Choisissez un mode de jeu</h2>
          <div class="mode-options">
            <button @click="handleCreateGameAI" class="btn btn-primary btn-large mode-btn" :disabled="!sessionId">
              <span class="mode-icon">🤖</span>
              <span class="mode-title">Jouer contre l'IA</span>
              <span class="mode-description">Partie solo contre l'ordinateur</span>
            </button>
            <button @click="handleCreateGamePlayer" class="btn btn-primary btn-large mode-btn" :disabled="!sessionId">
              <span class="mode-icon">👥</span>
              <span class="mode-title">Jouer contre un joueur</span>
              <span class="mode-description">Partie multijoueur en ligne</span>
            </button>
          </div>
        </div>

        <div class="menu-options">
          <button @click="handleShowAvailableGames" class="btn btn-secondary btn-large">
            Rejoindre une partie
          </button>
        </div>

        <!-- Liste des parties disponibles -->
        <div v-if="showAvailableGames && availableGames.length > 0" class="available-games">
          <h3>Parties disponibles</h3>
          <div class="games-list">
            <div
              v-for="game in availableGames"
              :key="game.id"
              class="game-item"
            >
              <div class="game-info">
                <span class="game-player">{{ game.player1Name || 'Joueur 1' }}</span>
                <span class="game-time">{{ formatTime(game.elapsedSeconds) }}</span>
              </div>
              <button
                @click="handleJoinGame(game.id)"
                class="btn btn-primary btn-small"
                :disabled="!sessionId"
              >
                Rejoindre
              </button>
            </div>
          </div>
        </div>

        <div v-else-if="showAvailableGames && availableGames.length === 0" class="no-games">
          <p>Aucune partie disponible. Créez-en une !</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted } from 'vue'
import { useRouter } from 'vue-router'
import { useTicTacToeStore } from '@/stores/ticTacToe'
import { useUserStore } from '@/stores/user'
import { TicTacToeGameMode } from '@/types'
import TicTacToeBoard from '@/components/tic-tac-toe/TicTacToeBoard.vue'

const router = useRouter()
const ticTacToeStore = useTicTacToeStore()
const userStore = useUserStore()

const showAvailableGames = ref(false)
let refreshInterval: number | null = null

// Exposer l'enum pour le template
const GameMode = TicTacToeGameMode

const currentGame = computed(() => ticTacToeStore.currentGame)
const availableGames = computed(() => ticTacToeStore.availableGames)
const isLoading = computed(() => ticTacToeStore.isLoading)
const error = computed(() => ticTacToeStore.error)
const isMyTurn = computed(() => ticTacToeStore.isMyTurn)
const playerNumber = computed(() => ticTacToeStore.playerNumber)
const sessionId = computed(() => userStore.sessionId)

/**
 * Initialise le store avec la session
 */
onMounted(async () => {
  if (sessionId.value) {
    ticTacToeStore.setSessionId(sessionId.value)
  }

  // Si on a une partie en cours, la recharger
  if (currentGame.value) {
    await ticTacToeStore.loadGame(currentGame.value.id)
  }

  // Rafraîchir la partie toutes les 2 secondes si elle est en cours (uniquement pour le mode multijoueur)
  if (currentGame.value && currentGame.value.status === 2 && currentGame.value.gameMode === GameMode.Player) {
    startRefreshInterval()
  }
})

onUnmounted(() => {
  stopRefreshInterval()
})

/**
 * Démarre l'intervalle de rafraîchissement
 */
function startRefreshInterval() {
  stopRefreshInterval()
  refreshInterval = window.setInterval(async () => {
    if (currentGame.value && !ticTacToeStore.isGameOver && currentGame.value.gameMode === GameMode.Player) {
      await ticTacToeStore.refreshGame()
      // Arrêter si la partie est terminée
      if (ticTacToeStore.isGameOver) {
        stopRefreshInterval()
      }
    }
  }, 2000)
}

/**
 * Arrête l'intervalle de rafraîchissement
 */
function stopRefreshInterval() {
  if (refreshInterval !== null) {
    clearInterval(refreshInterval)
    refreshInterval = null
  }
}

/**
 * Crée une nouvelle partie contre l'IA
 */
async function handleCreateGameAI() {
  if (!sessionId.value) {
    alert('Vous devez être connecté pour créer une partie')
    return
  }

  try {
    await ticTacToeStore.createGame(sessionId.value, TicTacToeGameMode.AI)
    startRefreshInterval()
  } catch (err) {
    console.error('Erreur lors de la création de la partie:', err)
  }
}

/**
 * Crée une nouvelle partie contre un joueur
 */
async function handleCreateGamePlayer() {
  if (!sessionId.value) {
    alert('Vous devez être connecté pour créer une partie')
    return
  }

  try {
    await ticTacToeStore.createGame(sessionId.value, TicTacToeGameMode.Player)
    startRefreshInterval()
  } catch (err) {
    console.error('Erreur lors de la création de la partie:', err)
  }
}

/**
 * Affiche les parties disponibles
 */
async function handleShowAvailableGames() {
  showAvailableGames.value = true
  await ticTacToeStore.loadAvailableGames()
}

/**
 * Rejoint une partie
 */
async function handleJoinGame(gameId: number) {
  if (!sessionId.value) {
    alert('Vous devez être connecté pour rejoindre une partie')
    return
  }

  try {
    await ticTacToeStore.joinGame(gameId, sessionId.value)
    showAvailableGames.value = false
    startRefreshInterval()
  } catch (err) {
    console.error('Erreur lors de la jonction à la partie:', err)
  }
}

/**
 * Joue un coup
 */
async function handleCellClick(position: number) {
  try {
    await ticTacToeStore.playMove(position)
    
    // Si c'est contre l'IA, rafraîchir immédiatement pour voir le coup de l'IA
    if (currentGame.value?.gameMode === GameMode.AI && !ticTacToeStore.isGameOver) {
      // Attendre un peu pour que l'IA joue
      setTimeout(async () => {
        await ticTacToeStore.refreshGame()
      }, 500)
    }
    
    // Si la partie est terminée, arrêter le rafraîchissement
    if (ticTacToeStore.isGameOver) {
      stopRefreshInterval()
    }
  } catch (err) {
    console.error('Erreur lors du coup joué:', err)
  }
}

/**
 * Abandonne la partie
 */
async function handleAbandon() {
  if (!confirm('Êtes-vous sûr de vouloir abandonner cette partie ?')) {
    return
  }

  try {
    await ticTacToeStore.abandonGame()
    stopRefreshInterval()
  } catch (err) {
    console.error('Erreur lors de l\'abandon:', err)
  }
}

/**
 * Retour au menu
 */
function goBack() {
  ticTacToeStore.resetGame()
  router.push('/')
}

/**
 * Efface l'erreur
 */
function clearError() {
  ticTacToeStore.clearError()
}

/**
 * Formate le temps en secondes en format MM:SS
 */
function formatTime(seconds: number): string {
  const mins = Math.floor(seconds / 60)
  const secs = seconds % 60
  return `${mins.toString().padStart(2, '0')}:${secs.toString().padStart(2, '0')}`
}
</script>

<style scoped>
.tic-tac-toe-view {
  min-height: 100vh;
  padding: 20px;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
}

.container {
  max-width: 800px;
  margin: 0 auto;
  background: white;
  border-radius: 12px;
  padding: 24px;
  box-shadow: 0 10px 30px rgba(0, 0, 0, 0.3);
}

.header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 24px;
}

.header h1 {
  margin: 0;
  color: #333;
  font-size: 2rem;
}

.alert {
  padding: 12px;
  border-radius: 6px;
  margin-bottom: 16px;
  display: flex;
  justify-content: space-between;
  align-items: center;
}

.alert-error {
  background-color: #fee;
  color: #c33;
  border: 1px solid #fcc;
}

.close-btn {
  background: none;
  border: none;
  font-size: 1.5rem;
  cursor: pointer;
  color: #c33;
}

.loading {
  text-align: center;
  padding: 40px;
}

.game-container {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.players-info {
  display: flex;
  justify-content: space-around;
  align-items: center;
  padding: 16px;
  background: #f5f5f5;
  border-radius: 8px;
}

.player {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  padding: 12px;
  border-radius: 8px;
  transition: all 0.3s ease;
}

.player.active {
  background: #e3f2fd;
  box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
}

.player-symbol {
  font-size: 2rem;
  font-weight: bold;
}

.player-name {
  font-weight: 500;
  color: #333;
}

.you-badge {
  font-size: 0.8rem;
  color: #666;
  font-style: italic;
}

.winner-badge {
  font-size: 0.9rem;
  color: #ff6b35;
  font-weight: bold;
}

.vs {
  font-size: 1.5rem;
  font-weight: bold;
  color: #666;
}

.game-status {
  text-align: center;
  padding: 12px;
  border-radius: 6px;
  font-weight: 500;
}

.status-waiting {
  background: #fff3cd;
  color: #856404;
}

.status-in-progress {
  background: #d1ecf1;
  color: #0c5460;
}

.status-completed {
  background: #d4edda;
  color: #155724;
}

.status-draw {
  background: #e2e3e5;
  color: #383d41;
}

.status-abandoned {
  background: #f8d7da;
  color: #721c24;
}

.game-stats {
  display: flex;
  justify-content: center;
  gap: 24px;
  padding: 12px;
  background: #f9f9f9;
  border-radius: 6px;
}

.stat {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 4px;
}

.stat-label {
  font-size: 0.9rem;
  color: #666;
}

.stat-value {
  font-size: 1.2rem;
  font-weight: bold;
  color: #333;
}

.menu-container {
  display: flex;
  flex-direction: column;
  gap: 24px;
}

.menu-options {
  display: flex;
  flex-direction: column;
  gap: 12px;
}

.available-games {
  margin-top: 24px;
}

.available-games h3 {
  margin-bottom: 16px;
  color: #333;
}

.games-list {
  display: flex;
  flex-direction: column;
  gap: 8px;
}

.game-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 12px;
  background: #f5f5f5;
  border-radius: 6px;
}

.game-info {
  display: flex;
  flex-direction: column;
  gap: 4px;
}

.game-player {
  font-weight: 500;
  color: #333;
}

.game-time {
  font-size: 0.9rem;
  color: #666;
}

.no-games {
  text-align: center;
  padding: 24px;
  color: #666;
}

.game-mode-selection {
  margin-bottom: 32px;
}

.game-mode-selection h2 {
  text-align: center;
  margin-bottom: 24px;
  color: #333;
  font-size: 1.5rem;
}

.mode-options {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
  margin-bottom: 24px;
}

.mode-btn {
  display: flex;
  flex-direction: column;
  align-items: center;
  gap: 8px;
  padding: 24px;
  border-radius: 12px;
  transition: all 0.3s ease;
}

.mode-btn:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
}

.mode-icon {
  font-size: 3rem;
}

.mode-title {
  font-size: 1.2rem;
  font-weight: 600;
}

.mode-description {
  font-size: 0.9rem;
  opacity: 0.8;
  text-align: center;
}

.btn {
  padding: 10px 20px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 1rem;
  font-weight: 500;
  transition: all 0.2s ease;
}

.btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.btn-primary {
  background: #667eea;
  color: white;
}

.btn-primary:hover:not(:disabled) {
  background: #5568d3;
}

.btn-secondary {
  background: #6c757d;
  color: white;
}

.btn-secondary:hover:not(:disabled) {
  background: #5a6268;
}

.btn-danger {
  background: #dc3545;
  color: white;
}

.btn-danger:hover:not(:disabled) {
  background: #c82333;
}

.btn-large {
  padding: 14px 28px;
  font-size: 1.1rem;
}

.btn-small {
  padding: 6px 12px;
  font-size: 0.9rem;
}

@media (max-width: 600px) {
  .container {
    padding: 16px;
  }

  .header h1 {
    font-size: 1.5rem;
  }

  .players-info {
    flex-direction: column;
    gap: 12px;
  }

  .vs {
    display: none;
  }
}
</style>

