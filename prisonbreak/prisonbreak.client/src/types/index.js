/**
 * Types et constantes pour le jeu Hashi (Version JavaScript)
 * Pour TypeScript, voir le dossier frontend/
 */

/**
 * Niveaux de difficulté
 */
export const DifficultyLevel = {
  Easy: 1,
  Medium: 2,
  Hard: 3,
  Expert: 4
}

/**
 * Statuts de partie
 */
export const GameStatus = {
  InProgress: 1,
  Completed: 2,
  Abandoned: 3,
  Paused: 4
}

/**
 * Obtient le label d'une difficulté
 */
export function getDifficultyLabel(difficulty) {
  switch (difficulty) {
    case DifficultyLevel.Easy:
      return 'Facile'
    case DifficultyLevel.Medium:
      return 'Moyen'
    case DifficultyLevel.Hard:
      return 'Difficile'
    case DifficultyLevel.Expert:
      return 'Expert'
    default:
      return 'Inconnu'
  }
}

