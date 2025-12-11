<template>
  <section class="w-full h-full px-4 sm:px-6 lg:px-12 py-8 overflow-y-auto">
    <div class="max-w-4xl mx-auto">
      <!-- En-tête -->
      <header class="mb-6">
        <div class="flex items-center justify-between">
          <div>
            <h2 class="text-3xl sm:text-4xl font-bold text-slate-50 mb-2">
              ⭕ Tic-Tac-Toe
            </h2>
            <p class="text-sm text-slate-300">
              Jouez contre l'IA ou défiez d'autres joueurs en ligne
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
        <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-orange-500 mb-4"></div>
        <p class="text-slate-300">Chargement...</p>
      </div>

      <!-- Partie en cours -->
      <div v-else-if="currentGame" class="space-y-6">
        <!-- Informations des joueurs -->
        <div class="grid grid-cols-2 gap-4">
          <div
            class="rounded-2xl p-4 border-2 transition-all"
            :class="currentGame.currentPlayer === 1
              ? 'bg-orange-500/20 border-orange-500 text-orange-300'
              : 'bg-slate-800/50 border-slate-700 text-slate-300'"
          >
            <div class="text-sm font-medium mb-1">Joueur 1 (X)</div>
            <div class="text-lg font-bold">{{ currentGame.player1Name || 'Joueur 1' }}</div>
            <div v-if="playerNumber === 1" class="text-xs mt-1 text-orange-400">(Vous)</div>
          </div>
          <div
            class="rounded-2xl p-4 border-2 transition-all"
            :class="currentGame.currentPlayer === 2
              ? 'bg-blue-500/20 border-blue-500 text-blue-300'
              : 'bg-slate-800/50 border-slate-700 text-slate-300'"
          >
            <div class="text-sm font-medium mb-1">Joueur 2 (O)</div>
            <div class="text-lg font-bold">
              {{ currentGame.player2Name || (currentGame.gameMode === GameMode.AI ? 'IA 🤖' : 'Joueur 2') }}
            </div>
            <div v-if="playerNumber === 2 && currentGame.gameMode === GameMode.Player" class="text-xs mt-1 text-blue-400">(Vous)</div>
          </div>
        </div>

        <!-- Statut de la partie -->
        <div class="text-center">
          <div v-if="ticTacToeStore.isGameOver" class="text-xl font-bold text-green-400 mb-2">
            {{ getGameOverMessage() }}
          </div>
          <div v-else-if="isMyTurn" class="text-lg font-semibold text-orange-400">
            ✨ C'est votre tour !
          </div>
          <div v-else-if="currentGame.gameMode === GameMode.AI" class="text-lg font-semibold text-blue-400">
            🤖 L'IA réfléchit...
          </div>
          <div v-else class="text-lg font-semibold text-slate-400">
            ⏳ En attente de l'adversaire...
          </div>
        </div>

        <!-- Grille de jeu -->
        <TicTacToeBoard :game="currentGame" @cell-click="handleCellClick" />

        <!-- Options de jeu -->
        <div v-if="ticTacToeStore.isGameOver" class="text-center">
          <button
            @click="handleNewGame"
            class="px-6 py-3 rounded-xl bg-gradient-to-r from-orange-500 to-orange-600 text-white font-bold hover:from-orange-400 hover:to-orange-500 transition-all shadow-lg shadow-orange-500/30"
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
            class="rounded-2xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 p-6 hover:border-orange-500/30 hover:scale-[1.02] transition-all text-left"
          >
            <div class="text-4xl mb-3">🤖</div>
            <h3 class="text-xl font-bold text-slate-50 mb-2">Jouer contre l'IA</h3>
            <p class="text-sm text-slate-300">Partie rapide contre l'intelligence artificielle</p>
          </button>

          <button
            @click="toggleOnlineUsers"
            class="rounded-2xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 p-6 hover:border-blue-500/30 hover:scale-[1.02] transition-all text-left"
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
              class="mt-4 px-4 py-2 rounded-lg bg-blue-600 text-white hover:bg-blue-500 transition-colors text-sm font-medium"
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
              class="w-full rounded-xl bg-slate-800/50 border border-slate-700 p-4 hover:border-blue-500/50 hover:bg-slate-800 transition-all text-left"
            >
              <div class="flex items-center justify-between">
                <div>
                  <div class="text-sm font-medium text-slate-300">Partie #{{ game.id }}</div>
                  <div class="text-xs text-slate-400">Créée par {{ game.player1Name }}</div>
                </div>
                <span class="px-3 py-1 rounded-full text-xs font-medium bg-blue-500/20 text-blue-300 border border-blue-500/30">
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
import { useTicTacToeStore } from '@/stores/ticTacToe';
import { useUiStore } from '@/stores/ui';
import { TicTacToeGameMode, TicTacToeGameStatus, SessionDto } from '@/types';
import TicTacToeBoard from '@/components/tic-tac-toe/TicTacToeBoard.vue';
import { useUserStore } from '@/stores/user';
import { sessionsApi } from '@/services/api';

const ticTacToeStore = useTicTacToeStore();
const uiStore = useUiStore();
const userStore = useUserStore();

const GameMode = TicTacToeGameMode;
const showAvailableGames = ref(false);
const showOnlineUsers = ref(false);
const onlineUsers = ref<SessionDto[]>([]);
const isLoadingUsers = ref(false);
let refreshInterval: number | null = null;
let invitationCheckInterval: number | null = null;

const currentGame = computed(() => ticTacToeStore.currentGame);
const availableGames = computed(() => ticTacToeStore.availableGames);
const invitations = computed(() => ticTacToeStore.invitations);
const isLoading = computed(() => ticTacToeStore.isLoading);
const error = computed(() => ticTacToeStore.error);
const isMyTurn = computed(() => ticTacToeStore.isMyTurn);
const playerNumber = computed(() => ticTacToeStore.playerNumber);

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
  ticTacToeStore.setSessionId(sessionId.value);
  
  // Si une partie est en cours, la charger
  if (currentGame.value) {
    await ticTacToeStore.loadGame(currentGame.value.id);
    // Démarrer le refresh si c'est une partie multijoueur
    if (currentGame.value.gameMode === GameMode.Player) {
      startRefreshInterval();
    }
  }
  
  // Vérifier les invitations périodiquement
  startInvitationCheck();
  
  // Surveiller les changements de partie pour démarrer/arrêter le refresh
  watch(
    () => currentGame.value?.gameMode === GameMode.Player && currentGame.value && !ticTacToeStore.isGameOver,
    (shouldRefresh) => {
      if (shouldRefresh) {
        // Démarrer le refresh si une partie multijoueur est active
        if (!refreshInterval) {
          startRefreshInterval();
        }
      } else {
        // Arrêter le refresh si plus de partie multijoueur ou partie terminée
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
  // Arrêter l'interval existant s'il y en a un
  if (refreshInterval) {
    window.clearInterval(refreshInterval);
    refreshInterval = null;
  }
  
  // Démarrer un nouvel interval
  refreshInterval = window.setInterval(async () => {
    if (currentGame.value && !ticTacToeStore.isGameOver && currentGame.value.gameMode === GameMode.Player) {
      try {
        await ticTacToeStore.refreshGame();
        if (ticTacToeStore.isGameOver) {
          stopRefreshInterval();
        }
      } catch (err) {
        console.error('Erreur lors du rafraîchissement:', err);
      }
    } else if (!currentGame.value || currentGame.value.gameMode !== GameMode.Player) {
      // Si plus de partie en cours ou pas de partie multijoueur, arrêter le refresh
      stopRefreshInterval();
    }
  }, 1000); // Rafraîchir toutes les 1 seconde pour une meilleure synchronisation
}

function stopRefreshInterval() {
  if (refreshInterval) {
    window.clearInterval(refreshInterval);
    refreshInterval = null;
  }
}

function startInvitationCheck() {
  if (invitationCheckInterval) return;
  
  // Vérifier les invitations toutes les 3 secondes
  invitationCheckInterval = window.setInterval(async () => {
    if (!currentGame.value && sessionId.value) {
      await ticTacToeStore.loadInvitations(sessionId.value);
      
      // Si une invitation est trouvée et qu'aucune partie n'est en cours, charger automatiquement
      if (invitations.value.length > 0 && !currentGame.value) {
        const invitation = invitations.value[0];
        // InProgress = 2
        if (invitation.status === 2) {
          await ticTacToeStore.loadGame(invitation.id);
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
  await ticTacToeStore.loadAvailableGames();
}

async function createGameVsAI() {
  await ticTacToeStore.createGame(sessionId.value, TicTacToeGameMode.AI);
  
  // Si c'est contre l'IA, rafraîchir immédiatement pour voir le coup de l'IA
  if (currentGame.value?.gameMode === GameMode.AI && !ticTacToeStore.isGameOver) {
    setTimeout(async () => {
      await ticTacToeStore.refreshGame();
    }, 500);
  }
}

async function createGameVsPlayer(player2SessionId?: number) {
  await ticTacToeStore.createGame(sessionId.value, TicTacToeGameMode.Player, player2SessionId);
  // Démarrer le refresh pour toutes les parties multijoueur
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
  await ticTacToeStore.joinGame(gameId, sessionId.value);
  // Démarrer le refresh pour la synchronisation en temps réel
  if (currentGame.value && currentGame.value.gameMode === GameMode.Player) {
    startRefreshInterval();
    // Rafraîchir immédiatement pour charger l'état actuel
    await ticTacToeStore.refreshGame();
  }
}

async function handleCellClick(position: number) {
  if (!currentGame.value || !isMyTurn.value || ticTacToeStore.isGameOver) {
    return;
  }
  
  await ticTacToeStore.playMove(position);
  
  // Si c'est contre l'IA, rafraîchir immédiatement pour voir le coup de l'IA
  if (currentGame.value?.gameMode === GameMode.AI && !ticTacToeStore.isGameOver) {
    setTimeout(async () => {
      await ticTacToeStore.refreshGame();
    }, 500);
  }
  
  // Pour le multijoueur, rafraîchir immédiatement après le coup pour que l'autre joueur voie le changement
  if (currentGame.value?.gameMode === GameMode.Player && !ticTacToeStore.isGameOver) {
    // Rafraîchir immédiatement pour voir le changement de tour
    setTimeout(async () => {
      await ticTacToeStore.refreshGame();
    }, 200);
    // S'assurer que le refresh interval tourne toujours
    if (!refreshInterval) {
      startRefreshInterval();
    }
  }
  
  if (ticTacToeStore.isGameOver) {
    stopRefreshInterval();
  }
}

async function handleAbandon() {
  if (currentGame.value) {
    await ticTacToeStore.abandonGame();
    stopRefreshInterval();
  }
}

function handleNewGame() {
  ticTacToeStore.resetGame();
  stopRefreshInterval();
}

function getGameOverMessage(): string {
  if (!currentGame.value) return '';
  
  // Vérifier d'abord si c'est un match nul
  if (currentGame.value.status === TicTacToeGameStatus.Draw) {
    return 'Match nul !';
  }
  
  // Vérifier si le joueur a gagné
  if (currentGame.value.status === TicTacToeGameStatus.Completed && 
      currentGame.value.winnerPlayerId === playerNumber.value) {
    return '🎉 Vous avez gagné !';
  }
  
  // Sinon, le joueur a perdu
  if (currentGame.value.status === TicTacToeGameStatus.Completed) {
    return '😔 Vous avez perdu...';
  }
  
  return '';
}

function clearError() {
  ticTacToeStore.clearError();
}
</script>

<style scoped>
section {
  scrollbar-width: thin;
  scrollbar-color: rgba(148, 163, 184, 0.3) transparent;
}

section::-webkit-scrollbar {
  width: 8px;
}

section::-webkit-scrollbar-track {
  background: transparent;
}

section::-webkit-scrollbar-thumb {
  background-color: rgba(148, 163, 184, 0.3);
  border-radius: 4px;
}

section::-webkit-scrollbar-thumb:hover {
  background-color: rgba(148, 163, 184, 0.5);
}
</style>

