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
            <div v-if="currentGame.player1Wager > 0" class="flex items-center gap-1 mt-2 text-yellow-400">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="currentColor" viewBox="0 0 20 20">
                <path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 01-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 01-.567.267z" />
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-13a1 1 0 10-2 0v.092a4.535 4.535 0 00-1.676.662C6.602 6.234 6 7.009 6 8c0 .99.602 1.765 1.324 2.246.48.32 1.054.545 1.676.662v1.941c-.391-.127-.68-.317-.843-.504a1 1 0 10-1.51 1.31c.562.649 1.413 1.076 2.353 1.253V15a1 1 0 102 0v-.092a4.535 4.535 0 001.676-.662C13.398 13.766 14 12.991 14 12c0-.99-.602-1.765-1.324-2.246A4.535 4.535 0 0011 9.092V7.151c.391.127.68.317.843.504a1 1 0 101.511-1.31c-.563-.649-1.413-1.076-2.354-1.253V5z" clip-rule="evenodd" />
              </svg>
              <span class="text-sm font-bold">{{ currentGame.player1Wager }}</span>
            </div>
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
            <div v-if="currentGame.player2Wager > 0" class="flex items-center gap-1 mt-2 text-yellow-400">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="currentColor" viewBox="0 0 20 20">
                <path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 01-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 01-.567.267z" />
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-13a1 1 0 10-2 0v.092a4.535 4.535 0 00-1.676.662C6.602 6.234 6 7.009 6 8c0 .99.602 1.765 1.324 2.246.48.32 1.054.545 1.676.662v1.941c-.391-.127-.68-.317-.843-.504a1 1 0 10-1.51 1.31c.562.649 1.413 1.076 2.353 1.253V15a1 1 0 102 0v-.092a4.535 4.535 0 001.676-.662C13.398 13.766 14 12.991 14 12c0-.99-.602-1.765-1.324-2.246A4.535 4.535 0 0011 9.092V7.151c.391.127.68.317.843.504a1 1 0 101.511-1.31c-.563-.649-1.413-1.076-2.354-1.253V5z" clip-rule="evenodd" />
              </svg>
              <span class="text-sm font-bold">{{ currentGame.player2Wager }}</span>
            </div>
          </div>
        </div>

        <!-- Affichage du pot total -->
        <div v-if="currentGame.totalWager > 0" class="text-center p-4 rounded-2xl bg-gradient-to-r from-yellow-500/20 to-orange-500/20 border border-yellow-500/30">
          <div class="text-sm text-slate-300 mb-1">Pot total</div>
          <div class="flex items-center justify-center gap-2">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-yellow-400" fill="currentColor" viewBox="0 0 20 20">
              <path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 01-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 01-.567.267z" />
              <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-13a1 1 0 10-2 0v.092a4.535 4.535 0 00-1.676.662C6.602 6.234 6 7.009 6 8c0 .99.602 1.765 1.324 2.246.48.32 1.054.545 1.676.662v1.941c-.391-.127-.68-.317-.843-.504a1 1 0 10-1.51 1.31c.562.649 1.413 1.076 2.353 1.253V15a1 1 0 102 0v-.092a4.535 4.535 0 001.676-.662C13.398 13.766 14 12.991 14 12c0-.99-.602-1.765-1.324-2.246A4.535 4.535 0 0011 9.092V7.151c.391.127.68.317.843.504a1 1 0 101.511-1.31c-.563-.649-1.413-1.076-2.354-1.253V5z" clip-rule="evenodd" />
            </svg>
            <span class="text-2xl font-bold text-yellow-400">{{ currentGame.totalWager }}</span>
          </div>
          <div class="text-xs text-slate-400 mt-1">Le gagnant remporte ce total</div>
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

        <!-- Message pour les invitations -->
        <div v-if="notificationsStore.unreadInvitations.filter(i => i.invitation.gameType === 'TicTacToe').length > 0 && !currentGame" class="mb-6 p-4 rounded-xl bg-gradient-to-r from-green-900/30 to-blue-900/30 border border-green-500/30">
          <div class="flex items-center gap-3">
            <div class="text-2xl">📬</div>
            <div class="flex-1">
              <p class="text-sm font-medium text-green-300">Vous avez {{ notificationsStore.unreadInvitations.filter(i => i.invitation.gameType === 'TicTacToe').length }} invitation(s) en attente</p>
              <p class="text-xs text-green-400 mt-1">Consultez vos notifications pour les accepter</p>
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
                <div class="flex-1">
                  <div class="text-sm font-medium text-slate-300">Partie #{{ game.id }}</div>
                  <div class="text-xs text-slate-400">Créée par {{ game.player1Name }}</div>
                  <div v-if="game.player1Wager > 0" class="flex items-center gap-1 mt-1">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-3 w-3 text-yellow-400" fill="currentColor" viewBox="0 0 20 20">
                      <path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 01-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 01-.567.267z" />
                      <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-13a1 1 0 10-2 0v.092a4.535 4.535 0 00-1.676.662C6.602 6.234 6 7.009 6 8c0 .99.602 1.765 1.324 2.246.48.32 1.054.545 1.676.662v1.941c-.391-.127-.68-.317-.843-.504a1 1 0 10-1.51 1.31c.562.649 1.413 1.076 2.353 1.253V15a1 1 0 102 0v-.092a4.535 4.535 0 001.676-.662C13.398 13.766 14 12.991 14 12c0-.99-.602-1.765-1.324-2.246A4.535 4.535 0 0011 9.092V7.151c.391.127.68.317.843.504a1 1 0 101.511-1.31c-.563-.649-1.413-1.076-2.354-1.253V5z" clip-rule="evenodd" />
                    </svg>
                    <span class="text-xs text-yellow-400 font-medium">Mise: {{ game.player1Wager }} coins</span>
                  </div>
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

    <!-- Modal de pari -->
    <div
      v-if="showWagerModal"
      @click.self="closeWagerModal"
      class="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm p-4"
    >
      <div class="relative w-full max-w-md rounded-3xl bg-gradient-to-br from-slate-900 to-slate-800 border border-slate-700/50 shadow-2xl overflow-hidden max-h-[90vh] overflow-y-auto">
        <div class="p-4 sm:p-6 lg:p-8">
          <div class="flex items-center justify-between mb-6">
            <h3 class="text-2xl font-bold text-slate-50">💎 Miser des coins</h3>
            <button
              @click="closeWagerModal"
              class="w-8 h-8 rounded-full bg-slate-800 text-slate-300 hover:text-white hover:bg-slate-700 transition-all flex items-center justify-center"
            >
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12" />
              </svg>
            </button>
          </div>

          <div class="mb-6">
            <p class="text-slate-300 mb-4">
              {{ pendingAction?.type === 'join' ? 'Rejoindre cette partie avec une mise' : 'Créer une partie avec une mise' }}
            </p>
            <div v-if="pendingAction?.type === 'join' && requiredWager > 0" class="mb-4 p-3 rounded-xl bg-blue-500/20 border border-blue-500/30">
              <p class="text-sm text-blue-300">Le joueur 1 a misé <span class="font-bold">{{ requiredWager }} coins</span></p>
              <p class="text-xs text-blue-400 mt-1">Vous devez miser exactement la même somme pour rejoindre</p>
            </div>
            
            <label class="block text-sm font-medium text-slate-300 mb-2">
              Montant de la mise (coins)
            </label>
            <div class="relative">
              <input
                v-model.number="wagerAmount"
                type="number"
                min="0"
                :max="userStore.coins"
                step="10"
                class="w-full px-4 py-3 rounded-xl bg-slate-800/50 border border-slate-700/50 text-slate-100 focus:outline-none focus:ring-2 focus:ring-yellow-500/50 focus:border-yellow-500/50"
                placeholder="0"
              />
              <div class="absolute right-4 top-1/2 transform -translate-y-1/2 text-slate-400 text-sm">
                / {{ userStore.coins }} disponibles
              </div>
            </div>
            <div class="flex flex-wrap gap-2 mt-3">
              <button
                v-for="amount in [10, 50, 100, 250, 500]"
                :key="amount"
                @click="wagerAmount = Math.min(amount, userStore.coins)"
                :disabled="amount > userStore.coins"
                class="px-2 sm:px-3 py-1.5 rounded-lg text-xs sm:text-sm font-medium transition-all flex-1 min-w-[60px]"
                :class="amount > userStore.coins
                  ? 'bg-slate-800/30 text-slate-600 cursor-not-allowed'
                  : 'bg-slate-800/50 text-slate-300 hover:bg-slate-700/50 hover:text-yellow-400'"
              >
                {{ amount }}
              </button>
            </div>
          </div>

          <div class="flex flex-col sm:flex-row gap-3 sm:gap-4">
            <button
              @click="confirmWager"
              :disabled="wagerAmount < 0 || wagerAmount > userStore.coins"
              class="flex-1 px-4 sm:px-6 py-2.5 sm:py-3 rounded-xl bg-gradient-to-r from-yellow-500 to-orange-500 text-slate-900 text-sm sm:text-base font-bold hover:from-yellow-400 hover:to-orange-400 transition-all disabled:opacity-50 disabled:cursor-not-allowed flex items-center justify-center gap-2"
            >
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="currentColor" viewBox="0 0 20 20">
                <path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 01-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 01-.567.267z" />
                <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-13a1 1 0 10-2 0v.092a4.535 4.535 0 00-1.676.662C6.602 6.234 6 7.009 6 8c0 .99.602 1.765 1.324 2.246.48.32 1.054.545 1.676.662v1.941c-.391-.127-.68-.317-.843-.504a1 1 0 10-1.51 1.31c.562.649 1.413 1.076 2.353 1.253V15a1 1 0 102 0v-.092a4.535 4.535 0 001.676-.662C13.398 13.766 14 12.991 14 12c0-.99-.602-1.765-1.324-2.246A4.535 4.535 0 0011 9.092V7.151c.391.127.68.317.843.504a1 1 0 101.511-1.31c-.563-.649-1.413-1.076-2.354-1.253V5z" clip-rule="evenodd" />
              </svg>
              Confirmer
            </button>
            <button
              @click="closeWagerModal"
              class="px-6 py-3 rounded-xl bg-slate-700/50 text-slate-300 hover:bg-slate-700 text-base font-medium transition-all"
            >
              Annuler
            </button>
          </div>
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
import { useNotificationsStore } from '@/stores/notifications';
import { sessionsApi, ticTacToeApi } from '@/services/api';

const ticTacToeStore = useTicTacToeStore();
const uiStore = useUiStore();
const userStore = useUserStore();
const notificationsStore = useNotificationsStore();

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
  
  // Vérifier s'il y a une invitation en attente depuis les notifications
  const pendingInvitation = notificationsStore.unreadInvitations.find(
    i => i.invitation.gameType === 'TicTacToe'
  );
  if (pendingInvitation && !currentGame.value) {
    // Ouvrir automatiquement le modal de mise pour cette invitation
    openWagerModal(
      { type: 'join', gameId: pendingInvitation.invitation.gameId },
      pendingInvitation.invitation.wager
    );
    notificationsStore.markAsRead(pendingInvitation.id);
  }
  
  // Vérifier s'il y a une invitation en attente depuis les notifications
  const pendingInvitation = notificationsStore.unreadInvitations.find(
    i => i.invitation.gameType === 'TicTacToe'
  );
  if (pendingInvitation && !currentGame.value) {
    // Ouvrir automatiquement le modal de mise pour cette invitation
    openWagerModal(
      { type: 'join', gameId: pendingInvitation.invitation.gameId },
      pendingInvitation.invitation.wager
    );
    notificationsStore.markAsRead(pendingInvitation.id);
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
      
      // Ajouter les nouvelles invitations aux notifications
      invitations.value.forEach(invitation => {
        // Vérifier si cette invitation n'est pas déjà dans les notifications
        const existingNotification = notificationsStore.notifications.find(
          n => n.invitation?.gameId === invitation.id && n.invitation?.gameType === 'TicTacToe'
        );
        
        if (!existingNotification) {
          notificationsStore.addInvitation({
            id: invitation.id,
            gameType: 'TicTacToe',
            gameId: invitation.id,
            fromPlayerName: invitation.player1Name || 'Joueur',
            fromPlayerId: invitation.player1SessionId,
            wager: invitation.player1Wager || 0,
            game: invitation,
            createdAt: new Date(invitation.createdAt)
          });
        }
      });
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

const showWagerModal = ref(false)
const wagerAmount = ref(0)
const requiredWager = ref(0) // Mise requise si on rejoint une partie avec mise
const pendingAction = ref<{ type: 'create' | 'join', gameId?: number, player2SessionId?: number } | null>(null)

function openWagerModal(action: { type: 'create' | 'join', gameId?: number, player2SessionId?: number }, requiredWagerAmount: number = 0) {
  pendingAction.value = action
  requiredWager.value = requiredWagerAmount
  wagerAmount.value = requiredWagerAmount > 0 ? requiredWagerAmount : 0
  showWagerModal.value = true
}

function closeWagerModal() {
  showWagerModal.value = false
  pendingAction.value = null
  wagerAmount.value = 0
  requiredWager.value = 0
}

async function confirmWager() {
  if (!pendingAction.value) return
  
  if (wagerAmount.value < 0) {
    ticTacToeStore.error = 'La mise doit être positive'
    return
  }

  if (wagerAmount.value > userStore.coins) {
    ticTacToeStore.error = 'Vous n\'avez pas assez de coins'
    return
  }

  // Vérifier si on rejoint une partie avec une mise requise
  if (pendingAction.value.type === 'join' && requiredWager.value > 0) {
    if (wagerAmount.value !== requiredWager.value) {
      ticTacToeStore.error = `Vous devez miser exactement ${requiredWager.value} coins pour rejoindre cette partie`
      return
    }
  }

  try {
    if (pendingAction.value.type === 'create') {
      // Créer une nouvelle partie
      await ticTacToeStore.createGame(
        sessionId.value, 
        TicTacToeGameMode.Player,
        pendingAction.value.player2SessionId,
        wagerAmount.value
      )
    } else if (pendingAction.value.type === 'join' && pendingAction.value.gameId) {
      await ticTacToeStore.joinGame(pendingAction.value.gameId, sessionId.value, wagerAmount.value)
    }
    
    closeWagerModal()
    
    // Démarrer le refresh pour les parties multijoueur
    if (currentGame.value && currentGame.value.gameMode === GameMode.Player) {
      startRefreshInterval()
    }
    
    // Recharger les coins
    await userStore.loadCoins()
  } catch (err) {
    console.error('Erreur:', err)
    ticTacToeStore.error = err instanceof Error ? err.message : 'Erreur lors de l\'action'
  }
}

async function createGameVsAI() {
  await ticTacToeStore.createGame(sessionId.value, TicTacToeGameMode.AI, undefined, 0);
  
  // Si c'est contre l'IA, rafraîchir immédiatement pour voir le coup de l'IA
  if (currentGame.value?.gameMode === GameMode.AI && !ticTacToeStore.isGameOver) {
    setTimeout(async () => {
      await ticTacToeStore.refreshGame();
    }, 500);
  }
}

async function createGameVsPlayer(player2SessionId?: number) {
  openWagerModal({ type: 'create', player2SessionId })
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
  // Charger d'abord le jeu pour voir s'il y a déjà une mise
  try {
    // Utiliser l'API directement pour ne pas remplacer currentGame
    const game = await ticTacToeApi.getById(gameId)
    const requiredWagerAmount = game?.player1Wager || 0
    openWagerModal({ type: 'join', gameId }, requiredWagerAmount)
  } catch (err) {
    console.error('Erreur lors du chargement du jeu:', err)
    ticTacToeStore.error = 'Impossible de charger la partie'
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

