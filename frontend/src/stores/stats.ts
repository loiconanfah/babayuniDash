/**
 * Store Pinia pour la gestion des statistiques et du classement
 */

import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { UserStats, LeaderboardEntry } from '@/types'
import { statsApi } from '@/services/api'

export const useStatsStore = defineStore('stats', () => {
  // ====================================================
  // STATE - État réactif
  // ====================================================

  /** Statistiques de l'utilisateur actuel */
  const userStats = ref<UserStats | null>(null)

  /** Classement des meilleurs joueurs */
  const leaderboard = ref<LeaderboardEntry[]>([])

  /** Indique si les statistiques sont en chargement */
  const isLoadingStats = ref(false)

  /** Indique si le classement est en chargement */
  const isLoadingLeaderboard = ref(false)

  /** Message d'erreur éventuel */
  const error = ref<string | null>(null)

  // ====================================================
  // GETTERS - Propriétés calculées
  // ====================================================

  /**
   * Vérifie si l'utilisateur a des statistiques
   */
  const hasStats = computed(() => userStats.value !== null)

  /**
   * Formate le temps en minutes et secondes
   */
  const formatTime = (seconds: number): string => {
    const mins = Math.floor(seconds / 60)
    const secs = seconds % 60
    return `${mins}m ${secs}s`
  }

  // ====================================================
  // ACTIONS - Méthodes
  // ====================================================

  /**
   * Charge les statistiques d'un utilisateur par son ID
   */
  async function loadUserStats(userId: number): Promise<void> {
    try {
      isLoadingStats.value = true
      error.value = null
      userStats.value = await statsApi.getUserStats(userId)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors du chargement des statistiques'
      console.error('Erreur lors du chargement des statistiques:', err)
      throw err
    } finally {
      isLoadingStats.value = false
    }
  }

  /**
   * Charge les statistiques d'un utilisateur par son email
   */
  async function loadUserStatsByEmail(email: string): Promise<void> {
    try {
      isLoadingStats.value = true
      error.value = null
      userStats.value = await statsApi.getUserStatsByEmail(email)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors du chargement des statistiques'
      console.error('Erreur lors du chargement des statistiques:', err)
      // Ne pas throw si l'utilisateur n'existe pas encore (première connexion)
      if (err instanceof Error && err.message.includes('404')) {
        userStats.value = null
        return
      }
      throw err
    } finally {
      isLoadingStats.value = false
    }
  }

  /**
   * Charge le classement des meilleurs joueurs
   */
  async function loadLeaderboard(limit: number = 10): Promise<void> {
    try {
      isLoadingLeaderboard.value = true
      error.value = null
      leaderboard.value = await statsApi.getLeaderboard(limit)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors du chargement du classement'
      console.error('Erreur lors du chargement du classement:', err)
      throw err
    } finally {
      isLoadingLeaderboard.value = false
    }
  }

  /**
   * Réinitialise les statistiques
   */
  function resetStats(): void {
    userStats.value = null
    leaderboard.value = []
    error.value = null
  }

  return {
    // State
    userStats,
    leaderboard,
    isLoadingStats,
    isLoadingLeaderboard,
    error,
    // Getters
    hasStats,
    formatTime,
    // Actions
    loadUserStats,
    loadUserStatsByEmail,
    loadLeaderboard,
    resetStats
  }
})

