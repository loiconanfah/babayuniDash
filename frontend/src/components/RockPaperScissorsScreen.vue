<template>
  <section class="w-full h-full px-4 sm:px-6 lg:px-12 py-8 overflow-y-auto">
    <div class="max-w-4xl mx-auto">
      <!-- En-tête -->
      <header class="mb-6">
        <div class="flex items-center justify-between">
          <div>
            <h2 class="text-3xl sm:text-4xl font-bold text-slate-50 mb-2">
              ✂️ Pierre-Papier-Ciseaux
            </h2>
            <p class="text-sm text-slate-300">
              Le jeu classique ! Premier à {{ currentGame?.roundsToWin || 3 }} victoires gagne !
            </p>
          </div>
          <button
            v-if="!currentGame"
            @click="goBack"
            class="px-4 py-2 rounded-xl bg-slate-800 text-slate-300 hover:bg-slate-700 transition-colors text-sm font-medium"
          >
            Retour
          </button>
          <button
            v-else-if="!rpsStore.isGameOver"
            @click="handleAbandon"
            class="px-4 py-2 rounded-xl bg-red-600 text-white hover:bg-red-500 transition-colors text-sm font-medium"
          >
            Abandonner
          </button>
        </div>
      </header>

      <!-- Message d'erreur -->
      <div v-if="error" class="mb-4 p-4 rounded-xl bg-red-900/80 border border-red-700 text-red-100 flex items-center justify-between">
        <span>{{ error }}</span>
        <button @click="clearError" class="text-red-300 hover:text-red-100 text-xl leading-none">×</button>
      </div>

      <!-- Chargement -->
      <div v-if="isLoading" class="text-center py-12">
        <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-purple-500 mb-4"></div>
        <p class="text-slate-300">Chargement...</p>
      </div>

      <!-- Partie en cours -->
      <div v-else-if="currentGame" class="space-y-6">
        <!-- Scores -->
        <div class="grid grid-cols-2 gap-4">
          <div
            class="rounded-2xl p-4 border-2 transition-all"
            :class="currentGame.player1Score >= currentGame.roundsToWin
              ? 'bg-green-500/20 border-green-500 text-green-300'
              : 'bg-slate-800/50 border-slate-700 text-slate-300'"
          >
            <div class="text-sm font-medium mb-1">{{ currentGame.player1Name || 'Joueur 1' }}</div>
            <div class="text-3xl font-bold">{{ currentGame.player1Score }}</div>
            <div v-if="playerNumber === 1" class="text-xs mt-1 text-purple-400">(Vous)</div>
          </div>
          <div
            class="rounded-2xl p-4 border-2 transition-all"
            :class="currentGame.player2Score >= currentGame.roundsToWin
              ? 'bg-green-500/20 border-green-500 text-green-300'
              : 'bg-slate-800/50 border-slate-700 text-slate-300'"
          >
            <div class="text-sm font-medium mb-1">
              {{ currentGame.player2Name || (currentGame.gameMode === GameMode.AI ? 'IA 🤖' : 'Joueur 2') }}
            </div>
            <div class="text-3xl font-bold">{{ currentGame.player2Score }}</div>
            <div v-if="playerNumber === 2 && currentGame.gameMode === GameMode.Player" class="text-xs mt-1 text-purple-400">(Vous)</div>
          </div>
        </div>

        <!-- Round actuel -->
        <div class="text-center">
          <div class="text-lg font-semibold text-purple-300 mb-2">
            Round {{ currentGame.roundNumber }}
          </div>
          <div v-if="rpsStore.isGameOver" class="text-xl font-bold text-green-400 mb-2">
            {{ getGameOverMessage() }}
          </div>
          <div v-else-if="rpsStore.isRoundCompleted" class="text-lg font-semibold text-yellow-400 mb-2">
            {{ getRoundResultMessage() }}
          </div>
          <div v-else-if="rpsStore.canPlay" class="text-lg font-semibold text-green-400">
            ✨ Choisissez votre coup !
          </div>
          <div v-else class="text-lg font-semibold text-slate-400">
            ⏳ En attente de l'adversaire...
          </div>
        </div>

        <!-- Choix des joueurs (affichage) -->
        <div v-if="rpsStore.isRoundCompleted || rpsStore.isGameOver" class="grid grid-cols-2 gap-6">
          <!-- Choix du joueur 1 -->
          <div class="text-center">
            <div class="text-sm font-medium text-slate-400 mb-2">{{ currentGame.player1Name || 'Joueur 1' }}</div>
            <div class="text-8xl mb-2">
              {{ getChoiceEmoji(currentGame.player1Choice) }}
            </div>
            <div class="text-sm text-slate-300">{{ getChoiceName(currentGame.player1Choice) }}</div>
          </div>
          <!-- VS -->
          <div class="flex items-center justify-center">
            <div class="text-4xl font-bold text-purple-400">VS</div>
          </div>
          <!-- Choix du joueur 2 -->
          <div class="text-center">
            <div class="text-sm font-medium text-slate-400 mb-2">
              {{ currentGame.player2Name || (currentGame.gameMode === GameMode.AI ? 'IA 🤖' : 'Joueur 2') }}
            </div>
            <div class="text-8xl mb-2">
              {{ getChoiceEmoji(currentGame.player2Choice) }}
            </div>
            <div class="text-sm text-slate-300">{{ getChoiceName(currentGame.player2Choice) }}</div>
          </div>
        </div>

        <!-- Boutons de choix -->
        <div v-else-if="rpsStore.canPlay" class="grid grid-cols-1 sm:grid-cols-3 gap-4">
          <button
            @click="playChoice(RPSChoice.Rock)"
            class="group relative rounded-2xl bg-gradient-to-br from-slate-800 to-slate-900 border-2 border-slate-700 p-8 hover:border-purple-500 hover:scale-105 transition-all"
            :disabled="isLoading"
          >
            <div class="text-8xl mb-4 transform group-hover:rotate-12 transition-transform">🪨</div>
            <div class="text-xl font-bold text-slate-50">Pierre</div>
            <div class="text-sm text-slate-400 mt-1">Rock</div>
          </button>

          <button
            @click="playChoice(RPSChoice.Paper)"
            class="group relative rounded-2xl bg-gradient-to-br from-slate-800 to-slate-900 border-2 border-slate-700 p-8 hover:border-purple-500 hover:scale-105 transition-all"
            :disabled="isLoading"
          >
            <div class="text-8xl mb-4 transform group-hover:-rotate-12 transition-transform">📄</div>
            <div class="text-xl font-bold text-slate-50">Papier</div>
            <div class="text-sm text-slate-400 mt-1">Paper</div>
          </button>

          <button
            @click="playChoice(RPSChoice.Scissors)"
            class="group relative rounded-2xl bg-gradient-to-br from-slate-800 to-slate-900 border-2 border-slate-700 p-8 hover:border-purple-500 hover:scale-105 transition-all"
            :disabled="isLoading"
          >
            <div class="text-8xl mb-4 transform group-hover:rotate-12 transition-transform">✂️</div>
            <div class="text-xl font-bold text-slate-50">Ciseaux</div>
            <div class="text-sm text-slate-400 mt-1">Scissors</div>
          </button>
        </div>

        <!-- Bouton pour passer au round suivant -->
        <div v-if="rpsStore.isRoundCompleted && !rpsStore.isGameOver" class="text-center">
          <button
            @click="handleNextRound"
            class="px-8 py-4 rounded-xl bg-gradient-to-r from-purple-500 to-purple-600 text-white font-bold hover:from-purple-400 hover:to-purple-500 transition-all shadow-lg shadow-purple-500/30 text-lg"
          >
            Round suivant →
          </button>
        </div>

        <!-- Options de jeu -->
        <div v-if="rpsStore.isGameOver" class="text-center">
          <button
            @click="handleNewGame"
            class="px-6 py-3 rounded-xl bg-gradient-to-r from-purple-500 to-purple-600 text-white font-bold hover:from-purple-400 hover:to-purple-500 transition-all shadow-lg shadow-purple-500/30"
          >
            Nouvelle partie
          </button>
        </div>
      </div>

      <!-- Menu de sélection (pas de partie en cours) -->
      <div v-else class="space-y-6">
        <!-- Options de jeu -->
        <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
          <button
            @click="createGameVsAI"
            class="rounded-2xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 p-6 hover:border-purple-500/30 hover:scale-[1.02] transition-all text-left"
          >
            <div class="text-4xl mb-3">🤖</div>
            <h3 class="text-xl font-bold text-slate-50 mb-2">Jouer contre l'IA</h3>
            <p class="text-sm text-slate-300">Partie rapide contre l'intelligence artificielle</p>
          </button>

          <button
            @click="toggleOnlineUsers"
            class="rounded-2xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 p-6 hover:border-purple-500/30 hover:scale-[1.02] transition-all text-left"
          >
            <div class="text-4xl mb-3">👥</div>
            <h3 class="text-xl font-bold text-slate-50 mb-2">Jouer contre un joueur</h3>
            <p class="text-sm text-slate-300">Sélectionnez un joueur en ligne ou créez une partie publique</p>
          </button>
        </div>

        <!-- Liste des utilisateurs en ligne -->
        <div v-if="showOnlineUsers" class="space-y-4">
          <div class="flex items-center justify-between">
            <h3 class="text-lg font-bold text-slate-50">Joueurs en ligne</h3>
            <button
              @click="loadOnlineUsers"
              class="px-3 py-1 rounded-lg bg-slate-800 text-slate-300 hover:bg-slate-700 transition-colors text-sm"
              :disabled="isLoadingUsers"
            >
              {{ isLoadingUsers ? 'Chargement...' : 'Actualiser' }}
            </button>
          </div>
          
          <div v-if="isLoadingUsers" class="text-center py-8 text-slate-400">
            <p>Chargement des joueurs...</p>
          </div>
          <div v-else-if="onlineUsers.length > 0" class="space-y-2">
            <button
              v-for="session in onlineUsers"
              :key="session.id"
              @click="createGameVsPlayer(session.id)"
              class="w-full rounded-xl bg-slate-800/50 border border-slate-700 p-4 hover:border-green-500/50 hover:bg-slate-800 transition-all text-left"
            >
              <div class="flex items-center justify-between">
                <div class="flex items-center gap-3">
                  <div class="h-10 w-10 rounded-full bg-gradient-to-br from-green-500 to-green-600 flex items-center justify-center text-white font-semibold">
                    {{ session.user?.name?.charAt(0).toUpperCase() || 'U' }}
                  </div>
                  <div>
                    <div class="text-sm font-medium text-slate-200">{{ session.user?.name || 'Joueur' }}</div>
                    <div class="text-xs text-slate-400">{{ session.user?.email }}</div>
                  </div>
                </div>
                <span class="px-3 py-1 rounded-full text-xs font-medium bg-green-500/20 text-green-300 border border-green-500/30 flex items-center gap-1">
                  <span class="h-2 w-2 rounded-full bg-green-500 animate-pulse"></span>
                  Inviter
                </span>
              </div>
            </button>
          </div>
          <div v-else class="text-center py-8 text-slate-400">
            <p>Aucun autre joueur en ligne</p>
            <button
              @click="createGameVsPlayer()"
              class="mt-4 px-4 py-2 rounded-lg bg-purple-600 text-white hover:bg-purple-500 transition-colors text-sm font-medium"
            >
              Créer une partie publique
            </button>
          </div>
        </div>

        <!-- Notifications d'invitations -->
        <div v-if="invitations.length > 0 && !currentGame" class="mb-6 space-y-2">
          <h3 class="text-lg font-bold text-slate-50 mb-2">📬 Invitations reçues</h3>
          <div
            v-for="invitation in invitations"
            :key="invitation.id"
            class="rounded-xl bg-gradient-to-r from-green-900/50 to-green-800/50 border-2 border-green-500/50 p-4 hover:border-green-400/70 transition-all cursor-pointer"
            @click="joinGame(invitation.id)"
          >
            <div class="flex items-center justify-between">
              <div>
                <div class="text-sm font-medium text-green-300 mb-1">🎮 Invitation à une partie</div>
                <div class="text-xs text-green-400">{{ invitation.player1Name }} vous a invité à jouer</div>
              </div>
              <span class="px-3 py-1 rounded-full text-xs font-medium bg-green-500/30 text-green-200 border border-green-400/50">
                Accepter
              </span>
            </div>
          </div>
        </div>

        <!-- Liste des parties disponibles -->
        <div v-if="showAvailableGames">
          <h3 class="text-lg font-bold text-slate-50 mb-4">Parties disponibles</h3>
          <div v-if="availableGames.length > 0" class="space-y-2">
            <button
              v-for="game in availableGames"
              :key="game.id"
              @click="joinGame(game.id)"
              class="w-full rounded-xl bg-slate-800/50 border border-slate-700 p-4 hover:border-purple-500/50 hover:bg-slate-800 transition-all text-left"
            >
              <div class="flex items-center justify-between">
                <div>
                  <div class="text-sm font-medium text-slate-300">Partie #{{ game.id }}</div>
                  <div class="text-xs text-slate-400">Créée par {{ game.player1Name }}</div>
                </div>
                <span class="px-3 py-1 rounded-full text-xs font-medium bg-purple-500/20 text-purple-300 border border-purple-500/30">
                  Rejoindre
                </span>
              </div>
            </button>
          </div>
          <div v-else class="text-center py-8 text-slate-400">
            <p>Aucune partie disponible</p>
          </div>
        </div>

        <!-- Bouton pour voir les parties disponibles -->
        <div class="text-center">
          <button
            @click="toggleAvailableGames"
            class="px-6 py-3 rounded-xl bg-slate-800 text-slate-300 hover:bg-slate-700 transition-colors font-medium"
          >
            {{ showAvailableGames ? 'Masquer' : 'Voir' }} les parties disponibles
          </button>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, onUnmounted, watch } from 'vue';
import { useRockPaperScissorsStore } from '@/stores/rockPaperScissors';
import { useUiStore } from '@/stores/ui';
import { RPSGameMode, RPSGameStatus, RPSChoice, SessionDto } from '@/types';
import { useUserStore } from '@/stores/user';
import { sessionsApi } from '@/services/api';

const rpsStore = useRockPaperScissorsStore();
const uiStore = useUiStore();
const userStore = useUserStore();

const GameMode = RPSGameMode;
const showAvailableGames = ref(false);
const showOnlineUsers = ref(false);
const onlineUsers = ref<SessionDto[]>([]);
const isLoadingUsers = ref(false);
let refreshInterval: number | null = null;
let invitationCheckInterval: number | null = null;

const currentGame = computed(() => rpsStore.currentGame);
const availableGames = computed(() => rpsStore.availableGames);
const invitations = computed(() => rpsStore.invitations);
const isLoading = computed(() => rpsStore.isLoading);
const error = computed(() => rpsStore.error);
const playerNumber = computed(() => rpsStore.playerNumber);

// Obtenir l'ID de session
const sessionId = computed(() => {
  if (userStore.user?.id) {
    return userStore.user.id;
  }
  let tempId = localStorage.getItem('tempSessionId');
  if (!tempId) {
    tempId = Date.now().toString();
    localStorage.setItem('tempSessionId', tempId);
  }
  return parseInt(tempId);
});

// Initialiser le store
onMounted(async () => {
  rpsStore.setSessionId(sessionId.value);
  
  if (currentGame.value) {
    await rpsStore.loadGame(currentGame.value.id);
    if (currentGame.value.gameMode === GameMode.Player) {
      startRefreshInterval();
    }
  }
  
  startInvitationCheck();
  
  watch(
    () => currentGame.value?.gameMode === GameMode.Player && currentGame.value && !rpsStore.isGameOver,
    (shouldRefresh) => {
      if (shouldRefresh) {
        if (!refreshInterval) {
          startRefreshInterval();
        }
      } else {
        stopRefreshInterval();
      }
    },
    { immediate: true }
  );
});

onUnmounted(() => {
  stopRefreshInterval();
  stopInvitationCheck();
});

function startRefreshInterval() {
  if (refreshInterval) {
    window.clearInterval(refreshInterval);
    refreshInterval = null;
  }
  
  refreshInterval = window.setInterval(async () => {
    if (currentGame.value && !rpsStore.isGameOver && currentGame.value.gameMode === GameMode.Player) {
      try {
        await rpsStore.refreshGame();
        if (rpsStore.isGameOver) {
          stopRefreshInterval();
        }
      } catch (err) {
        console.error('Erreur lors du rafraîchissement:', err);
      }
    } else if (!currentGame.value || currentGame.value.gameMode !== GameMode.Player) {
      stopRefreshInterval();
    }
  }, 2000);
}

function stopRefreshInterval() {
  if (refreshInterval) {
    window.clearInterval(refreshInterval);
    refreshInterval = null;
  }
}

function startInvitationCheck() {
  if (invitationCheckInterval) return;
  
  invitationCheckInterval = window.setInterval(async () => {
    if (!currentGame.value && sessionId.value) {
      await rpsStore.loadInvitations();
      
      if (invitations.value.length > 0 && !currentGame.value) {
        const invitation = invitations.value[0];
        if (invitation.status === RPSGameStatus.WaitingForChoices || 
            invitation.status === RPSGameStatus.RoundCompleted) {
          await rpsStore.loadGame(invitation.id);
          startRefreshInterval();
        }
      }
    }
  }, 3000);
}

function stopInvitationCheck() {
  if (invitationCheckInterval) {
    window.clearInterval(invitationCheckInterval);
    invitationCheckInterval = null;
  }
}

function goBack() {
  uiStore.goToGames();
}

function toggleAvailableGames() {
  showAvailableGames.value = !showAvailableGames.value;
  if (showAvailableGames.value) {
    loadAvailableGames();
  }
}

async function loadAvailableGames() {
  await rpsStore.loadAvailableGames();
}

async function createGameVsAI() {
  await rpsStore.createGame(RPSGameMode.AI);
}

async function createGameVsPlayer(player2SessionId?: number) {
  await rpsStore.createGame(RPSGameMode.Player, player2SessionId);
  if (currentGame.value && currentGame.value.gameMode === GameMode.Player) {
    startRefreshInterval();
  }
}

async function loadOnlineUsers() {
  try {
    isLoadingUsers.value = true;
    const sessions = await sessionsApi.getActiveSessions(sessionId.value);
    onlineUsers.value = sessions;
  } catch (err) {
    console.error('Erreur lors du chargement des utilisateurs en ligne:', err);
  } finally {
    isLoadingUsers.value = false;
  }
}

function toggleOnlineUsers() {
  showOnlineUsers.value = !showOnlineUsers.value;
  if (showOnlineUsers.value) {
    loadOnlineUsers();
  }
}

async function joinGame(gameId: number) {
  await rpsStore.joinGame(gameId);
  if (currentGame.value && currentGame.value.gameMode === GameMode.Player) {
    startRefreshInterval();
    await rpsStore.refreshGame();
  }
}

async function playChoice(choice: RPSChoice) {
  if (!currentGame.value || !rpsStore.canPlay) {
    return;
  }
  
  await rpsStore.playChoice(choice);
  
  // Si c'est contre l'IA, rafraîchir immédiatement pour voir le résultat
  if (currentGame.value?.gameMode === GameMode.AI && !rpsStore.isGameOver) {
    setTimeout(async () => {
      await rpsStore.refreshGame();
    }, 500);
  }
  
  if (rpsStore.isGameOver) {
    stopRefreshInterval();
  }
}

async function handleNextRound() {
  if (rpsStore.isRoundCompleted && !rpsStore.isGameOver) {
    await rpsStore.nextRound();
  }
}

async function handleAbandon() {
  if (currentGame.value) {
    await rpsStore.abandonGame();
    stopRefreshInterval();
  }
}

function handleNewGame() {
  rpsStore.resetGame();
  stopRefreshInterval();
}

function getChoiceEmoji(choice?: number): string {
  if (!choice) return '❓';
  switch (choice) {
    case RPSChoice.Rock: return '🪨';
    case RPSChoice.Paper: return '📄';
    case RPSChoice.Scissors: return '✂️';
    default: return '❓';
  }
}

function getChoiceName(choice?: number): string {
  if (!choice) return 'En attente...';
  switch (choice) {
    case RPSChoice.Rock: return 'Pierre';
    case RPSChoice.Paper: return 'Papier';
    case RPSChoice.Scissors: return 'Ciseaux';
    default: return 'Inconnu';
  }
}

function getRoundResultMessage(): string {
  if (!currentGame.value) return '';
  
  if (currentGame.value.roundWinner === null) {
    return '🤝 Égalité !';
  }
  
  if (currentGame.value.roundWinner === playerNumber.value) {
    return '🎉 Vous avez gagné ce round !';
  }
  
  return '😔 Vous avez perdu ce round...';
}

function getGameOverMessage(): string {
  if (!currentGame.value) return '';
  
  if (currentGame.value.winnerPlayerId === playerNumber.value) {
    return '🏆 Vous avez gagné la partie !';
  }
  
  return '😔 Vous avez perdu la partie...';
}

function clearError() {
  rpsStore.clearError();
}
</script>

