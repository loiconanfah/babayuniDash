/**
 * Fonctions utilitaires et helpers
 * Regroupement de fonctions réutilisables dans toute l'application
 */

/**
 * Formate un temps en secondes vers le format MM:SS
 * @param seconds - Nombre de secondes
 * @returns Temps formaté (ex: "05:42")
 */
export function formatTime(seconds: number): string {
  const minutes = Math.floor(seconds / 60)
  const remainingSeconds = seconds % 60
  return `${minutes.toString().padStart(2, '0')}:${remainingSeconds.toString().padStart(2, '0')}`
}

/**
 * Calcule la distance Manhattan entre deux points
 * Utilisé pour déterminer si deux îles peuvent être connectées
 * @param x1 - Position X du premier point
 * @param y1 - Position Y du premier point
 * @param x2 - Position X du deuxième point
 * @param y2 - Position Y du deuxième point
 * @returns Distance Manhattan
 */
export function manhattanDistance(x1: number, y1: number, x2: number, y2: number): number {
  return Math.abs(x2 - x1) + Math.abs(y2 - y1)
}

/**
 * Vérifie si deux îles sont alignées (même ligne ou même colonne)
 * @param x1 - Position X de la première île
 * @param y1 - Position Y de la première île
 * @param x2 - Position X de la deuxième île
 * @param y2 - Position Y de la deuxième île
 * @returns True si les îles sont alignées
 */
export function areIslandsAligned(x1: number, y1: number, x2: number, y2: number): boolean {
  return x1 === x2 || y1 === y2
}

/**
 * Détermine la direction entre deux îles alignées
 * @param x1 - Position X de la première île
 * @param y1 - Position Y de la première île
 * @param x2 - Position X de la deuxième île
 * @param y2 - Position Y de la deuxième île
 * @returns 'horizontal' | 'vertical' | null
 */
export function getBridgeDirection(
  x1: number,
  y1: number,
  x2: number,
  y2: number
): 'horizontal' | 'vertical' | null {
  if (y1 === y2) return 'horizontal'
  if (x1 === x2) return 'vertical'
  return null
}

/**
 * Génère un ID unique basé sur le timestamp et un nombre aléatoire
 * @returns ID unique
 */
export function generateUniqueId(): string {
  return `${Date.now()}-${Math.random().toString(36).substr(2, 9)}`
}

/**
 * Débounce une fonction pour limiter le nombre d'appels
 * @param func - Fonction à débouncer
 * @param wait - Temps d'attente en millisecondes
 * @returns Fonction debouncée
 */
export function debounce<T extends (...args: any[]) => any>(
  func: T,
  wait: number
): (...args: Parameters<T>) => void {
  let timeout: NodeJS.Timeout | null = null

  return function executedFunction(...args: Parameters<T>) {
    const later = () => {
      timeout = null
      func(...args)
    }

    if (timeout) {
      clearTimeout(timeout)
    }
    timeout = setTimeout(later, wait)
  }
}

/**
 * Throttle une fonction pour limiter sa fréquence d'exécution
 * @param func - Fonction à throttler
 * @param limit - Limite en millisecondes
 * @returns Fonction throttlée
 */
export function throttle<T extends (...args: any[]) => any>(
  func: T,
  limit: number
): (...args: Parameters<T>) => void {
  let inThrottle: boolean
  
  return function executedFunction(...args: Parameters<T>) {
    if (!inThrottle) {
      func(...args)
      inThrottle = true
      setTimeout(() => {
        inThrottle = false
      }, limit)
    }
  }
}

/**
 * Clone profond d'un objet
 * @param obj - Objet à cloner
 * @returns Clone de l'objet
 */
export function deepClone<T>(obj: T): T {
  return JSON.parse(JSON.stringify(obj))
}

/**
 * Vérifie si une valeur est vide (null, undefined, '', [], {})
 * @param value - Valeur à vérifier
 * @returns True si la valeur est vide
 */
export function isEmpty(value: any): boolean {
  if (value === null || value === undefined) return true
  if (typeof value === 'string') return value.trim().length === 0
  if (Array.isArray(value)) return value.length === 0
  if (typeof value === 'object') return Object.keys(value).length === 0
  return false
}

/**
 * Obtient un label lisible pour un niveau de difficulté
 * @param difficulty - Niveau de difficulté (1-4)
 * @returns Label en français
 */
export function getDifficultyLabel(difficulty: number): string {
  switch (difficulty) {
    case 1:
      return 'Facile'
    case 2:
      return 'Moyen'
    case 3:
      return 'Difficile'
    case 4:
      return 'Expert'
    default:
      return 'Inconnu'
  }
}

/**
 * Obtient une couleur pour un niveau de difficulté
 * @param difficulty - Niveau de difficulté (1-4)
 * @returns Code couleur hexadécimal
 */
export function getDifficultyColor(difficulty: number): string {
  switch (difficulty) {
    case 1:
      return '#48bb78' // Vert
    case 2:
      return '#4299e1' // Bleu
    case 3:
      return '#ed8936' // Orange
    case 4:
      return '#9f7aea' // Violet
    default:
      return '#718096' // Gris
  }
}

