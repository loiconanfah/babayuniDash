<template>
  <section class="w-full h-full px-4 sm:px-6 lg:px-12 py-8 overflow-y-auto">
    <div class="max-w-4xl mx-auto">
      <!-- En-tête -->
      <header class="mb-6">
        <div class="flex items-center justify-between">
          <div>
            <h2 class="text-3xl sm:text-4xl font-bold text-slate-50 mb-2">
              🔴 Connect Four
            </h2>
            <p class="text-sm text-slate-300">
              Alignez 4 pièces pour gagner !
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
            v-else
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
        <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-red-500 mb-4"></div>
        <p class="text-slate-300">Chargement...</p>
      </div>

      <!-- Partie en cours -->
      <div v-else-if="currentGame" class="space-y-6">
        <!-- Informations des joueurs -->
        <div class="grid grid-cols-2 gap-4">
          <div
            class="rounded-2xl p-4 border-2 transition-all"
            :class="currentGame.currentPlayer === 1
              ? 'bg-red-500/20 border-red-500 text-red-300'
              : 'bg-slate-800/50 border-slate-700 text-slate-300'"
          >
            <div class="text-sm font-medium mb-1">Joueur 1 (Rouge)</div>
            <div class="text-lg font-bold">{{ currentGame.player1Name || 'Joueur 1' }}</div>
            <div v-if="playerNumber === 1" class="text-xs mt-1 text-red-400">(Vous)</div>
          </div>
          <div
            class="rounded-2xl p-4 border-2 transition-all"
            :class="currentGame.currentPlayer === 2
              ? 'bg-yellow-500/20 border-yellow-500 text-yellow-300'
              : 'bg-slate-800/50 border-slate-700 text-slate-300'"
          >
            <div class="text-sm font-medium mb-1">Joueur 2 (Jaune)</div>
            <div class="text-lg font-bold">
              {{ currentGame.player2Name || (currentGame.gameMode === GameMode.AI ? 'IA 🤖' : 'Joueur 2') }}
            </div>
            <div v-if="playerNumber === 2 && currentGame.gameMode === GameMode.Player" class="text-xs mt-1 text-yellow-400">(Vous)</div>
          </div>
        </div>

        <!-- Statut de la partie -->
        <div class="text-center">
          <div v-if="connectFourStore.isGameOver" class="text-xl font-bold text-green-400 mb-2">
            {{ getGameOverMessage() }}
          </div>
          <div v-else-if="isMyTurn" class="text-lg font-semibold text-red-400">
            ✨ C'est votre tour ! Cliquez sur une colonne
          </div>
          <div v-else-if="currentGame.gameMode === GameMode.AI" class="text-lg font-semibold text-yellow-400">
            🤖 L'IA réfléchit...
          </div>
          <div v-else class="text-lg font-semibold text-slate-400">
            ⏳ En attente de l'adversaire...
          </div>
        </div>

        <!-- Grille de jeu -->
        <ConnectFourBoard :game="currentGame" @column-click="handleColumnClick" />

        <!-- Options de jeu -->
        <div v-if="connectFourStore.isGameOver" class="text-center">
          <button
            @click="handleNewGame"
            class="px-6 py-3 rounded-xl bg-gradient-to-r from-red-500 to-red-600 text-white font-bold hover:from-red-400 hover:to-red-500 transition-all shadow-lg shadow-red-500/30"
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
            class="rounded-2xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 p-6 hover:border-red-500/30 hover:scale-[1.02] transition-all text-left"
          >
            <div class="text-4xl mb-3">🤖</div>
            <h3 class="text-xl font-bold text-slate-50 mb-2">Jouer contre l'IA</h3>
            <p class="text-sm text-slate-300">Partie rapide contre l'intelligence artificielle</p>
          </button>

          <button
            @click="toggleOnlineUsers"
            class="rounded-2xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 p-6 hover:border-yellow-500/30 hover:scale-[1.02] transition-all text-left"
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
              class="mt-4 px-4 py-2 rounded-lg bg-yellow-600 text-white hover:bg-yellow-500 transition-colors text-sm font-medium"
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
              class="w-full rounded-xl bg-slate-800/50 border border-slate-700 p-4 hover:border-yellow-500/50 hover:bg-slate-800 transition-all text-left"
            >
              <div class="flex items-center justify-between">
                <div>
                  <div class="text-sm font-medium text-slate-300">Partie #{{ game.id }}</div>
                  <div class="text-xs text-slate-400">Créée par {{ game.player1Name }}</div>
                </div>
                <span class="px-3 py-1 rounded-full text-xs font-medium bg-yellow-500/20 text-yellow-300 border border-yellow-500/30">
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
import { useConnectFourStore } from '@/stores/connectFour';
import { useUiStore } from '@/stores/ui';
import { ConnectFourGameMode, ConnectFourGameStatus, SessionDto } from '@/types';
import ConnectFourBoard from '@/components/connect-four/ConnectFourBoard.vue';
import { useUserStore } from '@/stores/user';
import { sessionsApi } from '@/services/api';

const connectFourStore = useConnectFourStore();
const uiStore = useUiStore();
const userStore = useUserStore();

const GameMode = ConnectFourGameMode;
const showAvailableGames = ref(false);
const showOnlineUsers = ref(false);
const onlineUsers = ref<SessionDto[]>([]);
const isLoadingUsers = ref(false);
let refreshInterval: number | null = null;
let invitationCheckInterval: number | null = null;

const currentGame = computed(() => connectFourStore.currentGame);
const availableGames = computed(() => connectFourStore.availableGames);
const invitations = computed(() => connectFourStore.invitations);
const isLoading = computed(() => connectFourStore.isLoading);
const error = computed(() => connectFourStore.error);
const isMyTurn = computed(() => connectFourStore.isMyTurn);
const playerNumber = computed(() => connectFourStore.playerNumber);

// Obtenir l'ID de session
const sessionId = computed(() => {
  if (userStore.user?.id) {
    return userStore.user.id;
  }
  // Générer un ID temporaire si pas d'utilisateur
  let tempId = localStorage.getItem('tempSessionId');
  if (!tempId) {
    tempId = Date.now().toString();
    localStorage.setItem('tempSessionId', tempId);
  }
  return parseInt(tempId);
});

// Initialiser le store
onMounted(async () => {
  connectFourStore.setSessionId(sessionId.value);
  
  // Si une partie est en cours, la charger
  if (currentGame.value) {
    await connectFourStore.loadGame(currentGame.value.id);
    // Démarrer le refresh si c'est une partie multijoueur
    if (currentGame.value.gameMode === GameMode.Player) {
      startRefreshInterval();
    }
  }
  
  // Vérifier les invitations périodiquement
  startInvitationCheck();
  
  // Surveiller les changements de partie pour démarrer/arrêter le refresh
  watch(
    () => currentGame.value?.gameMode === GameMode.Player && currentGame.value && !connectFourStore.isGameOver,
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
    if (currentGame.value && !connectFourStore.isGameOver && currentGame.value.gameMode === GameMode.Player) {
      try {
        await connectFourStore.refreshGame();
        if (connectFourStore.isGameOver) {
          stopRefreshInterval();
        }
      } catch (err) {
        console.error('Erreur lors du rafraîchissement:', err);
      }
    } else if (!currentGame.value || currentGame.value.gameMode !== GameMode.Player) {
      stopRefreshInterval();
    }
  }, 1000);
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
      await connectFourStore.loadInvitations();
      
      if (invitations.value.length > 0 && !currentGame.value) {
        const invitation = invitations.value[0];
        if (invitation.status === ConnectFourGameStatus.InProgress) {
          await connectFourStore.loadGame(invitation.id);
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
  await connectFourStore.loadAvailableGames();
}

async function createGameVsAI() {
  await connectFourStore.createGame(ConnectFourGameMode.AI);
  
  if (currentGame.value?.gameMode === GameMode.AI && !connectFourStore.isGameOver) {
    setTimeout(async () => {
      await connectFourStore.refreshGame();
    }, 500);
  }
}

async function createGameVsPlayer(player2SessionId?: number) {
  await connectFourStore.createGame(ConnectFourGameMode.Player, player2SessionId);
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
  await connectFourStore.joinGame(gameId);
  if (currentGame.value && currentGame.value.gameMode === GameMode.Player) {
    startRefreshInterval();
    await connectFourStore.refreshGame();
  }
}

async function handleColumnClick(column: number) {
  if (!currentGame.value || !isMyTurn.value || connectFourStore.isGameOver) {
    return;
  }
  
  await connectFourStore.playMove(column);
  
  if (currentGame.value?.gameMode === GameMode.AI && !connectFourStore.isGameOver) {
    setTimeout(async () => {
      await connectFourStore.refreshGame();
    }, 500);
  }
  
  if (connectFourStore.isGameOver) {
    stopRefreshInterval();
  }
}

async function handleAbandon() {
  if (currentGame.value) {
    await connectFourStore.abandonGame();
    stopRefreshInterval();
  }
}

function handleNewGame() {
  connectFourStore.resetGame();
  stopRefreshInterval();
}

function getGameOverMessage(): string {
  if (!currentGame.value) return '';
  
  if (currentGame.value.status === ConnectFourGameStatus.Draw) {
    return 'Match nul !';
  }
  
  if (currentGame.value.status === ConnectFourGameStatus.Completed && 
      currentGame.value.winnerPlayerId === playerNumber.value) {
    return '🎉 Vous avez gagné !';
  }
  
  if (currentGame.value.status === ConnectFourGameStatus.Completed) {
    return '😔 Vous avez perdu...';
  }
  
  return '';
}

function clearError() {
  connectFourStore.clearError();
}
</script>

