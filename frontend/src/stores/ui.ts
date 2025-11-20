// src/stores/ui.ts
// Store Pinia pour l'état de navigation interne (écran courant, modales, etc.).

import { defineStore } from 'pinia';

export type Screen = 'home' | 'levels' | 'game' | 'leaderboard';
export type Difficulty = 'easy' | 'medium' | 'hard' | null;

interface UiState {
  currentScreen: Screen;
  isUserModalOpen: boolean;
  isTutorialModalOpen: boolean;
  selectedDifficulty: Difficulty;
}

export const useUiStore = defineStore('ui', {
  state: (): UiState => ({
    currentScreen: 'home',
    isUserModalOpen: false,
    isTutorialModalOpen: false,
    selectedDifficulty: null
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
    }
  }
});
