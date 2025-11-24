// src/stores/user.ts
// Store Pinia pour gérer l'état du joueur (user connecté) et sa session.

import { defineStore } from 'pinia';
import { createOrLoginUser, type UserDto } from '@/services/userApi';
import { createSession, getActiveSessionByUserId, getSessionById, type SessionDto } from '@/services/sessionApi';

const LOCAL_STORAGE_KEY = 'pb_user';
const LOCAL_STORAGE_SESSION_KEY = 'pb_session';

export type User = UserDto;

interface UserState {
  user: User | null;
  session: SessionDto | null;
  isInitialized: boolean;
}

export const useUserStore = defineStore('user', {
  state: (): UserState => ({
    user: null,
    session: null,
    isInitialized: false
  }),

  getters: {
    isLoggedIn: (state) => state.user !== null,
    hasActiveSession: (state) => state.session !== null && state.session.isActive,
    sessionId: (state) => state.session?.id ?? null
  },

  actions: {
    /**
     * Charge le joueur et la session depuis le localStorage au démarrage de l'appli.
     */
    loadFromLocalStorage() {
      // Charger l'utilisateur
      const rawUser = window.localStorage.getItem(LOCAL_STORAGE_KEY);
      if (rawUser) {
        try {
          const parsed = JSON.parse(rawUser) as User;
          this.user = parsed;
        } catch {
          console.warn('Impossible de parser pb_user depuis localStorage, on réinitialise.');
          this.user = null;
        }
      }

      // Charger la session
      const rawSession = window.localStorage.getItem(LOCAL_STORAGE_SESSION_KEY);
      if (rawSession) {
        try {
          const parsed = JSON.parse(rawSession) as SessionDto;
          this.session = parsed;
        } catch {
          console.warn('Impossible de parser pb_session depuis localStorage, on réinitialise.');
          this.session = null;
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
     * Sauvegarde la session dans le store + localStorage.
     */
    setSession(session: SessionDto) {
      this.session = session;
      window.localStorage.setItem(LOCAL_STORAGE_SESSION_KEY, JSON.stringify(session));
    },

    /**
     * Déconnecte le joueur et nettoie le localStorage.
     */
    clearUser() {
      this.user = null;
      this.session = null;
      window.localStorage.removeItem(LOCAL_STORAGE_KEY);
      window.localStorage.removeItem(LOCAL_STORAGE_SESSION_KEY);
    },

    /**
     * Action principale appelée depuis la modale :
     * - appelle l'API backend pour créer/connecter l'utilisateur
     * - crée une session pour cet utilisateur
     * - sauvegarde le user et la session
     */
    async register(name: string, email: string) {
      // Créer ou connecter l'utilisateur
      const user = await createOrLoginUser(name, email);
      this.setUser(user);

      // Vérifier s'il existe déjà une session active
      try {
        const activeSession = await getActiveSessionByUserId(user.id);
        if (activeSession && activeSession.isActive) {
          this.setSession(activeSession);
          return;
        }
      } catch {
        // Pas de session active, on en crée une nouvelle
      }

      // Créer une nouvelle session
      const session = await createSession({
        userId: user.id,
        ipAddress: undefined, // Le backend le récupérera automatiquement
        userAgent: navigator.userAgent
      });
      this.setSession(session);
    },

    /**
     * Récupère ou crée une session active pour l'utilisateur actuel.
     * Utilisé avant de créer une partie de jeu.
     */
    async ensureActiveSession(): Promise<number> {
      if (!this.user) {
        throw new Error('Aucun utilisateur connecté');
      }

      // Si on a une session stockée, vérifier qu'elle existe réellement côté serveur
      if (this.session && this.session.id) {
        try {
          const serverSession = await getSessionById(this.session.id);
          if (serverSession && serverSession.isActive) {
            // La session existe et est active, on peut l'utiliser
            this.setSession(serverSession);
            return serverSession.id;
          }
        } catch {
          // La session n'existe pas côté serveur (peut-être que la BD a été recréée)
          // On va créer une nouvelle session
          this.session = null;
        }
      }

      // Vérifier s'il existe une session active côté serveur pour cet utilisateur
      try {
        const activeSession = await getActiveSessionByUserId(this.user.id);
        if (activeSession && activeSession.isActive) {
          this.setSession(activeSession);
          return activeSession.id;
        }
      } catch {
        // Pas de session active, on en crée une nouvelle
      }

      // Créer une nouvelle session
      try {
        const session = await createSession({
          userId: this.user.id,
          ipAddress: undefined,
          userAgent: navigator.userAgent
        });
        this.setSession(session);
        return session.id;
      } catch (error) {
        // Si la création de session échoue (peut-être que l'utilisateur n'existe plus),
        // nettoyer le localStorage et demander à l'utilisateur de se réinscrire
        console.error('Erreur lors de la création de la session:', error);
        this.clearUser();
        throw new Error('Votre session a expiré. Veuillez vous réinscrire.');
      }
    }
  }
});
