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
            :class="currentGame.player2Score >= currentGame.roundsToWin
              ? 'bg-green-500/20 border-green-500 text-green-300'
              : 'bg-slate-800/50 border-slate-700 text-slate-300'"
          >
            <div class="text-sm font-medium mb-1">
              {{ currentGame.player2Name || (currentGame.gameMode === GameMode.AI ? 'IA 🤖' : 'Joueur 2') }}
            </div>
            <div class="text-3xl font-bold">{{ currentGame.player2Score }}</div>
            <div v-if="playerNumber === 2 && currentGame.gameMode === GameMode.Player" class="text-xs mt-1 text-purple-400">(Vous)</div>
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

        <!-- Message pour les invitations -->
        <div v-if="notificationsStore.unreadInvitations.filter(i => i.invitation.gameType === 'RockPaperScissors').length > 0 && !currentGame" class="mb-6 p-4 rounded-xl bg-gradient-to-r from-green-900/30 to-blue-900/30 border border-green-500/30">
          <div class="flex items-center gap-3">
            <div class="text-2xl">📬</div>
            <div class="flex-1">
              <p class="text-sm font-medium text-green-300">Vous avez {{ notificationsStore.unreadInvitations.filter(i => i.invitation.gameType === 'RockPaperScissors').length }} invitation(s) en attente</p>
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
              class="w-full rounded-xl bg-slate-800/50 border border-slate-700 p-4 hover:border-purple-500/50 hover:bg-slate-800 transition-all text-left"
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

    <!-- Modal de pari -->
    <div
      v-if="showWagerModal"
      @click.self="closeWagerModal"
      class="fixed inset-0 z-50 flex items-center justify-center bg-black/60 backdrop-blur-sm p-3 sm:p-4"
    >
      <div class="relative w-full max-w-md rounded-2xl sm:rounded-3xl bg-gradient-to-br from-slate-900 to-slate-800 border border-slate-700/50 shadow-2xl overflow-hidden max-h-[90vh] overflow-y-auto">
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
import { useRockPaperScissorsStore } from '@/stores/rockPaperScissors';
import { useUiStore } from '@/stores/ui';
import { RPSGameMode, RPSGameStatus, RPSChoice, SessionDto } from '@/types';
import { useUserStore } from '@/stores/user';
import { useNotificationsStore } from '@/stores/notifications';
import { sessionsApi, rpsApi } from '@/services/api';

const rpsStore = useRockPaperScissorsStore();
const uiStore = useUiStore();
const userStore = useUserStore();
const notificationsStore = useNotificationsStore();

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
  
  // Vérifier s'il y a une invitation en attente depuis les notifications
  const pendingInvitation = notificationsStore.unreadInvitations.find(
    i => i.invitation.gameType === 'RockPaperScissors'
  );
  if (pendingInvitation && !currentGame.value) {
    // Ouvrir automatiquement le modal de mise pour cette invitation
    openWagerModal(
      { type: 'join', gameId: pendingInvitation.invitation.gameId },
      pendingInvitation.invitation.wager
    );
    notificationsStore.markAsRead(pendingInvitation.id);
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
      
      // Ajouter les nouvelles invitations aux notifications
      invitations.value.forEach(invitation => {
        const existingNotification = notificationsStore.notifications.find(
          n => n.invitation?.gameId === invitation.id && n.invitation?.gameType === 'RockPaperScissors'
        );
        
        if (!existingNotification) {
          notificationsStore.addInvitation({
            id: invitation.id,
            gameType: 'RockPaperScissors',
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
  await rpsStore.loadAvailableGames();
}

const showWagerModal = ref(false)
const wagerAmount = ref(0)
const requiredWager = ref(0)
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
    rpsStore.error = 'La mise doit être positive'
    return
  }

  if (wagerAmount.value > userStore.coins) {
    rpsStore.error = 'Vous n\'avez pas assez de coins'
    return
  }

  if (pendingAction.value.type === 'join' && requiredWager.value > 0) {
    if (wagerAmount.value !== requiredWager.value) {
      rpsStore.error = `Vous devez miser exactement ${requiredWager.value} coins pour rejoindre cette partie`
      return
    }
  }

  try {
    if (pendingAction.value.type === 'create') {
      await rpsStore.createGame(RPSGameMode.Player, pendingAction.value.player2SessionId, wagerAmount.value)
    } else if (pendingAction.value.type === 'join' && pendingAction.value.gameId) {
      await rpsStore.joinGame(pendingAction.value.gameId, wagerAmount.value)
    }
    
    closeWagerModal()
    
    if (currentGame.value && currentGame.value.gameMode === GameMode.Player) {
      startRefreshInterval()
    }
    
    await userStore.loadCoins()
  } catch (err) {
    console.error('Erreur:', err)
    rpsStore.error = err instanceof Error ? err.message : 'Erreur lors de l\'action'
  }
}

async function createGameVsAI() {
  await rpsStore.createGame(RPSGameMode.AI, undefined, 0);
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
  try {
    const game = await rpsApi.getById(gameId)
    const requiredWagerAmount = game?.player1Wager || 0
    openWagerModal({ type: 'join', gameId }, requiredWagerAmount)
  } catch (err) {
    console.error('Erreur lors du chargement du jeu:', err)
    rpsStore.error = 'Impossible de charger la partie'
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

