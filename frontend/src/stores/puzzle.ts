/**
 * Store Pinia pour la gestion des puzzles
 * Gère la liste des puzzles disponibles et la génération de nouveaux puzzles
 */

import { defineStore } from 'pinia'
import { ref } from 'vue'
import type { Puzzle, GeneratePuzzleRequest, DifficultyLevel } from '@/types'
import { puzzleApi } from '@/services/api'

export const usePuzzleStore = defineStore('puzzle', () => {
  // ====================================================
  // STATE
  // ====================================================

  /** Liste de tous les puzzles disponibles */
  const puzzles = ref<Puzzle[]>([])

  /** Puzzle actuellement sélectionné pour prévisualisation */
  const selectedPuzzle = ref<Puzzle | null>(null)

  /** Indique si les puzzles sont en cours de chargement */
  const isLoading = ref(false)

  /** Message d'erreur éventuel */
  const error = ref<string | null>(null)

  // ====================================================
  // ACTIONS
  // ====================================================

  /**
   * Charge tous les puzzles disponibles
   */
  async function fetchAllPuzzles(): Promise<void> {
    try {
      isLoading.value = true
      error.value = null
      puzzles.value = await puzzleApi.getAll()
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors du chargement des puzzles'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Charge les puzzles d'une difficulté spécifique
   */
  async function fetchPuzzlesByDifficulty(difficulty: DifficultyLevel): Promise<void> {
    try {
      isLoading.value = true
      error.value = null
      puzzles.value = await puzzleApi.getByDifficulty(difficulty)
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors du chargement des puzzles'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Charge un puzzle spécifique par son ID
   */
  async function fetchPuzzleById(id: number): Promise<Puzzle> {
    try {
      isLoading.value = true
      error.value = null
      const puzzle = await puzzleApi.getById(id)
      selectedPuzzle.value = puzzle
      return puzzle
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors du chargement du puzzle'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Génère un nouveau puzzle
   */
  async function generatePuzzle(request: GeneratePuzzleRequest): Promise<Puzzle> {
    try {
      isLoading.value = true
      error.value = null
      const puzzle = await puzzleApi.generate(request)
      
      // Ajouter le nouveau puzzle à la liste
      puzzles.value.unshift(puzzle)
      selectedPuzzle.value = puzzle
      
      return puzzle
    } catch (err) {
      error.value = err instanceof Error ? err.message : 'Erreur lors de la génération du puzzle'
      throw err
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Sélectionne un puzzle
   */
  function selectPuzzle(puzzle: Puzzle): void {
    selectedPuzzle.value = puzzle
  }

  /**
   * Désélectionne le puzzle actuel
   */
  function clearSelection(): void {
    selectedPuzzle.value = null
  }

  /**
   * Efface le message d'erreur
   */
  function clearError(): void {
    error.value = null
  }

  // ====================================================
  // RETOUR
  // ====================================================

  return {
    // State
    puzzles,
    selectedPuzzle,
    isLoading,
    error,

    // Actions
    fetchAllPuzzles,
    fetchPuzzlesByDifficulty,
    fetchPuzzleById,
    generatePuzzle,
    selectPuzzle,
    clearSelection,
    clearError
  }
})

