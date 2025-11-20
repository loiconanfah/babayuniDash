// src/stores/user.ts
// Store Pinia pour gérer l'état du joueur (user connecté).

import { defineStore } from 'pinia';
import { createOrLoginUser, type UserDto } from '@/services/userApi';

const LOCAL_STORAGE_KEY = 'pb_user';

export type User = UserDto;

interface UserState {
  user: User | null;
  isInitialized: boolean;
}

export const useUserStore = defineStore('user', {
  state: (): UserState => ({
    user: null,
    isInitialized: false
  }),

  getters: {
    isLoggedIn: (state) => state.user !== null
  },

  actions: {
    /**
     * Charge le joueur depuis le localStorage au démarrage de l'appli.
     */
    loadFromLocalStorage() {
      const raw = window.localStorage.getItem(LOCAL_STORAGE_KEY);
      if (raw) {
        try {
          const parsed = JSON.parse(raw) as User;
          this.user = parsed;
        } catch {
          console.warn('Impossible de parser pb_user depuis localStorage, on réinitialise.');
          this.user = null;
        }
      }
      this.isInitialized = true;
    },

    /**
     * Sauvegarde le joueur dans le store + localStorage.
     */
    setUser(user: User) {
      this.user = user;
      window.localStorage.setItem(LOCAL_STORAGE_KEY, JSON.stringify(user));
    },

    /**
     * Déconnecte le joueur et nettoie le localStorage.
     */
    clearUser() {
      this.user = null;
      window.localStorage.removeItem(LOCAL_STORAGE_KEY);
    },

    /**
     * Action principale appelée depuis la modale :
     * - appelle l'API backend
     * - sauvegarde le user
     */
    async register(name: string, email: string) {
      const user = await createOrLoginUser(name, email);
      this.setUser(user);
    }
  }
});
