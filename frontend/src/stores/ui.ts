// src/stores/ui.ts
// Store Pinia pour l'état de navigation interne (écran courant, modales, etc.).

import { defineStore } from 'pinia';

export type Screen = 'home' | 'levels' | 'game' | 'leaderboard' | 'stats' | 'games' | 'ticTacToe' | 'connectFour' | 'rockPaperScissors' | 'adventure' | 'shop' | 'collection' | 'matches' | 'profile' | 'community' | 'tournaments';
export type Difficulty = 'easy' | 'medium' | 'hard' | null;

interface UiState {
  currentScreen: Screen;
  isUserModalOpen: boolean;
  isTutorialModalOpen: boolean;
  isVictoryModalOpen: boolean;
  selectedDifficulty: Difficulty;
  isMobileMenuOpen: boolean;
  isChatOpen: boolean;
  showCommunity: boolean;
}

export const useUiStore = defineStore('ui', {
  state: (): UiState => ({
    currentScreen: 'home',
    isUserModalOpen: false,
    isTutorialModalOpen: false,
    isVictoryModalOpen: false,
    selectedDifficulty: null,
    isMobileMenuOpen: false,
    isChatOpen: false,
    showCommunity: false
  }),

  actions: {
    // Navigation principale
    goToHome() {
      this.currentScreen = 'home';
    },
    goToLevels() {
      this.currentScreen = 'levels';
    },
    goToGame() {
      this.currentScreen = 'game';
    },
    goToLeaderboard() {
      this.currentScreen = 'leaderboard';
    },
    goToStats() {
      this.currentScreen = 'stats';
    },
    goToGames() {
      this.currentScreen = 'games';
    },
    goToTicTacToe() {
      this.currentScreen = 'ticTacToe';
    },
    goToConnectFour() {
      this.currentScreen = 'connectFour';
    },
    goToRockPaperScissors() {
      this.currentScreen = 'rockPaperScissors';
    },
    goToAdventure() {
      this.currentScreen = 'adventure';
    },
    goToShop() {
      this.currentScreen = 'shop';
    },
    goToCollection() {
      this.currentScreen = 'collection';
    },
    goToMatches() {
      this.currentScreen = 'matches';
    },
    goToProfile() {
      this.currentScreen = 'profile';
    },
    goToCommunity() {
      this.currentScreen = 'community';
    },
    goToTournaments() {
      this.currentScreen = 'tournaments';
    },

    // Modale utilisateur
    openUserModal() {
      this.isUserModalOpen = true;
    },
    closeUserModal() {
      this.isUserModalOpen = false;
    },

    // Modale tutoriel
    openTutorialModal() {
      this.isTutorialModalOpen = true;
    },
    closeTutorialModal() {
      this.isTutorialModalOpen = false;
    },

          // Sélection de difficulté
          selectDifficulty(diff: Difficulty) {
            this.selectedDifficulty = diff;
          },

          // Modale de victoire
          openVictoryModal() {
            this.isVictoryModalOpen = true;
          },
          closeVictoryModal() {
            this.isVictoryModalOpen = false;
          },

          // Menu mobile
          toggleMobileMenu() {
            this.isMobileMenuOpen = !this.isMobileMenuOpen;
          },
          closeMobileMenu() {
            this.isMobileMenuOpen = false;
          },

          // Chat
          openChat() {
            this.isChatOpen = true;
          },
          closeChat() {
            this.isChatOpen = false;
          },

          // Communauté
          toggleCommunity() {
            this.showCommunity = !this.showCommunity;
          },
          closeCommunity() {
            this.showCommunity = false;
          }
        }
      });
