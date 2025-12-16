/**
 * Configuration des thèmes pour les puzzles Hashi
 * Chaque thème a des couleurs, gradients et animations uniques
 */

import { PuzzleTheme } from '@/types'

export interface ThemeConfig {
  name: string
  colors: {
    background: string
    gridBackground: string
    gridLines: string
    islandGradient: string[]
    islandSelected: string[]
    islandComplete: string[]
    bridgeColor: string
    bridgeGradient: string[]
    textColor: string
    shadowColor: string
  }
  animations: {
    islandFloat?: string
    bridgePulse?: string
    particleEffect?: string
  }
}

/**
 * Configuration complète de tous les thèmes
 */
export const themeConfigs: Record<PuzzleTheme, ThemeConfig> = {
  [PuzzleTheme.Classic]: {
    name: 'Prison Classique',
    colors: {
      background: 'linear-gradient(135deg, #667eea 0%, #764ba2 100%)',
      gridBackground: 'linear-gradient(135deg, #f8fafc 0%, #f1f5f9 100%)',
      gridLines: '#cbd5e1',
      islandGradient: ['#9ca3af', '#6b7280', '#4b5563', '#374151'],
      islandSelected: ['#60a5fa', '#3b82f6', '#2563eb', '#1e40af'],
      islandComplete: ['#34d399', '#10b981', '#059669', '#047857'],
      bridgeColor: '#000000',
      bridgeGradient: ['#2d2d2d', '#1a1a1a', '#000000', '#1a1a1a', '#2d2d2d'],
      textColor: '#ffffff',
      shadowColor: 'rgba(0, 0, 0, 0.8)'
    },
    animations: {
      islandFloat: 'islandFloat 3s ease-in-out infinite'
    }
  },
  [PuzzleTheme.Medieval]: {
    name: 'Château Fort',
    colors: {
      background: 'linear-gradient(135deg, #78350f 0%, #451a03 100%)',
      gridBackground: 'linear-gradient(135deg, #fef3c7 0%, #fde68a 100%)',
      gridLines: '#d97706',
      islandGradient: ['#92400e', '#78350f', '#451a03', '#1c1917'],
      islandSelected: ['#f59e0b', '#d97706', '#b45309', '#92400e'],
      islandComplete: ['#10b981', '#059669', '#047857', '#065f46'],
      bridgeColor: '#451a03',
      bridgeGradient: ['#92400e', '#78350f', '#451a03', '#78350f', '#92400e'],
      textColor: '#ffffff',
      shadowColor: 'rgba(69, 26, 3, 0.9)'
    },
    animations: {
      islandFloat: 'islandFloat 2.5s ease-in-out infinite'
    }
  },
  [PuzzleTheme.Futuristic]: {
    name: 'Prison Spatiale',
    colors: {
      background: 'linear-gradient(135deg, #0f172a 0%, #1e293b 50%, #0f172a 100%)',
      gridBackground: 'linear-gradient(135deg, #1e293b 0%, #334155 100%)',
      gridLines: '#475569',
      islandGradient: ['#64748b', '#475569', '#334155', '#1e293b'],
      islandSelected: ['#3b82f6', '#2563eb', '#1d4ed8', '#1e40af'],
      islandComplete: ['#06b6d4', '#0891b2', '#0e7490', '#155e75'],
      bridgeColor: '#3b82f6',
      bridgeGradient: ['#60a5fa', '#3b82f6', '#2563eb', '#3b82f6', '#60a5fa'],
      textColor: '#ffffff',
      shadowColor: 'rgba(59, 130, 246, 0.8)'
    },
    animations: {
      islandFloat: 'islandFloat 4s ease-in-out infinite',
      bridgePulse: 'bridgePulse 2s ease-in-out infinite'
    }
  },
  [PuzzleTheme.Underwater]: {
    name: 'Prison Aquatique',
    colors: {
      background: 'linear-gradient(135deg, #0c4a6e 0%, #075985 50%, #0c4a6e 100%)',
      gridBackground: 'linear-gradient(135deg, #e0f2fe 0%, #bae6fd 100%)',
      gridLines: '#7dd3fc',
      islandGradient: ['#0ea5e9', '#0284c7', '#0369a1', '#075985'],
      islandSelected: ['#38bdf8', '#0ea5e9', '#0284c7', '#0369a1'],
      islandComplete: ['#22d3ee', '#06b6d4', '#0891b2', '#0e7490'],
      bridgeColor: '#0369a1',
      bridgeGradient: ['#0ea5e9', '#0284c7', '#0369a1', '#0284c7', '#0ea5e9'],
      textColor: '#ffffff',
      shadowColor: 'rgba(3, 105, 161, 0.8)'
    },
    animations: {
      islandFloat: 'islandFloat 3.5s ease-in-out infinite',
      particleEffect: 'bubbles 4s ease-in-out infinite'
    }
  },
  [PuzzleTheme.Desert]: {
    name: 'Désert Aride',
    colors: {
      background: 'linear-gradient(135deg, #d97706 0%, #92400e 100%)',
      gridBackground: 'linear-gradient(135deg, #fef3c7 0%, #fde68a 100%)',
      gridLines: '#f59e0b',
      islandGradient: ['#fbbf24', '#f59e0b', '#d97706', '#b45309'],
      islandSelected: ['#fcd34d', '#fbbf24', '#f59e0b', '#d97706'],
      islandComplete: ['#10b981', '#059669', '#047857', '#065f46'],
      bridgeColor: '#92400e',
      bridgeGradient: ['#d97706', '#b45309', '#92400e', '#b45309', '#d97706'],
      textColor: '#ffffff',
      shadowColor: 'rgba(146, 64, 14, 0.9)'
    },
    animations: {
      islandFloat: 'islandFloat 2.8s ease-in-out infinite'
    }
  },
  [PuzzleTheme.Forest]: {
    name: 'Jungle Perdue',
    colors: {
      background: 'linear-gradient(135deg, #14532d 0%, #052e16 100%)',
      gridBackground: 'linear-gradient(135deg, #dcfce7 0%, #bbf7d0 100%)',
      gridLines: '#86efac',
      islandGradient: ['#22c55e', '#16a34a', '#15803d', '#166534'],
      islandSelected: ['#4ade80', '#22c55e', '#16a34a', '#15803d'],
      islandComplete: ['#10b981', '#059669', '#047857', '#065f46'],
      bridgeColor: '#166534',
      bridgeGradient: ['#22c55e', '#16a34a', '#15803d', '#16a34a', '#22c55e'],
      textColor: '#ffffff',
      shadowColor: 'rgba(22, 101, 52, 0.9)'
    },
    animations: {
      islandFloat: 'islandFloat 3.2s ease-in-out infinite'
    }
  },
  [PuzzleTheme.Ice]: {
    name: 'Glacier Arctique',
    colors: {
      background: 'linear-gradient(135deg, #0c4a6e 0%, #075985 50%, #0e7490 100%)',
      gridBackground: 'linear-gradient(135deg, #f0f9ff 0%, #e0f2fe 100%)',
      gridLines: '#bae6fd',
      islandGradient: ['#e0f2fe', '#bae6fd', '#7dd3fc', '#38bdf8'],
      islandSelected: ['#0ea5e9', '#0284c7', '#0369a1', '#075985'],
      islandComplete: ['#06b6d4', '#0891b2', '#0e7490', '#155e75'],
      bridgeColor: '#0e7490',
      bridgeGradient: ['#38bdf8', '#0ea5e9', '#0284c7', '#0ea5e9', '#38bdf8'],
      textColor: '#0c4a6e',
      shadowColor: 'rgba(14, 116, 144, 0.8)'
    },
    animations: {
      islandFloat: 'islandFloat 4s ease-in-out infinite',
      particleEffect: 'snowflakes 5s ease-in-out infinite'
    }
  },
  [PuzzleTheme.Volcano]: {
    name: 'Volcan Brûlant',
    colors: {
      background: 'linear-gradient(135deg, #7f1d1d 0%, #991b1b 50%, #dc2626 100%)',
      gridBackground: 'linear-gradient(135deg, #fef2f2 0%, #fee2e2 100%)',
      gridLines: '#fca5a5',
      islandGradient: ['#ef4444', '#dc2626', '#b91c1c', '#991b1b'],
      islandSelected: ['#f87171', '#ef4444', '#dc2626', '#b91c1c'],
      islandComplete: ['#10b981', '#059669', '#047857', '#065f46'],
      bridgeColor: '#991b1b',
      bridgeGradient: ['#ef4444', '#dc2626', '#b91c1c', '#dc2626', '#ef4444'],
      textColor: '#ffffff',
      shadowColor: 'rgba(153, 27, 27, 0.9)'
    },
    animations: {
      islandFloat: 'islandFloat 2s ease-in-out infinite',
      bridgePulse: 'bridgePulse 1.5s ease-in-out infinite',
      particleEffect: 'lava 3s ease-in-out infinite'
    }
  },
  [PuzzleTheme.Neon]: {
    name: 'Cyberpunk',
    colors: {
      background: 'linear-gradient(135deg, #1a0033 0%, #330066 50%, #1a0033 100%)',
      gridBackground: 'linear-gradient(135deg, #1e1b4b 0%, #312e81 100%)',
      gridLines: '#6366f1',
      islandGradient: ['#a855f7', '#9333ea', '#7e22ce', '#6b21a8'],
      islandSelected: ['#ec4899', '#db2777', '#be185d', '#9f1239'],
      islandComplete: ['#06b6d4', '#0891b2', '#0e7490', '#155e75'],
      bridgeColor: '#ec4899',
      bridgeGradient: ['#f472b6', '#ec4899', '#db2777', '#ec4899', '#f472b6'],
      textColor: '#ffffff',
      shadowColor: 'rgba(236, 72, 153, 0.9)'
    },
    animations: {
      islandFloat: 'islandFloat 2.5s ease-in-out infinite',
      bridgePulse: 'bridgePulse 1s ease-in-out infinite',
      particleEffect: 'neonGlow 2s ease-in-out infinite'
    }
  },
  [PuzzleTheme.Steampunk]: {
    name: 'Steampunk',
    colors: {
      background: 'linear-gradient(135deg, #451a03 0%, #78350f 50%, #92400e 100%)',
      gridBackground: 'linear-gradient(135deg, #fef3c7 0%, #fde68a 100%)',
      gridLines: '#d97706',
      islandGradient: ['#ca8a04', '#a16207', '#854d0e', '#713f12'],
      islandSelected: ['#fbbf24', '#f59e0b', '#d97706', '#b45309'],
      islandComplete: ['#10b981', '#059669', '#047857', '#065f46'],
      bridgeColor: '#713f12',
      bridgeGradient: ['#ca8a04', '#a16207', '#854d0e', '#a16207', '#ca8a04'],
      textColor: '#ffffff',
      shadowColor: 'rgba(113, 63, 18, 0.9)'
    },
    animations: {
      islandFloat: 'islandFloat 3s ease-in-out infinite'
    }
  },
  [PuzzleTheme.Pirate]: {
    name: 'Pirate',
    colors: {
      background: 'linear-gradient(135deg, #1e3a8a 0%, #1e40af 50%, #1e3a8a 100%)',
      gridBackground: 'linear-gradient(135deg, #fef3c7 0%, #fde68a 100%)',
      gridLines: '#f59e0b',
      islandGradient: ['#fbbf24', '#f59e0b', '#d97706', '#b45309'],
      islandSelected: ['#fcd34d', '#fbbf24', '#f59e0b', '#d97706'],
      islandComplete: ['#10b981', '#059669', '#047857', '#065f46'],
      bridgeColor: '#1e3a8a',
      bridgeGradient: ['#3b82f6', '#2563eb', '#1e40af', '#2563eb', '#3b82f6'],
      textColor: '#ffffff',
      shadowColor: 'rgba(30, 58, 138, 0.9)'
    },
    animations: {
      islandFloat: 'islandFloat 3.5s ease-in-out infinite',
      particleEffect: 'waves 4s ease-in-out infinite'
    }
  },
  [PuzzleTheme.Zombie]: {
    name: 'Apocalypse',
    colors: {
      background: 'linear-gradient(135deg, #1c1917 0%, #292524 50%, #1c1917 100%)',
      gridBackground: 'linear-gradient(135deg, #f5f5f4 0%, #e7e5e4 100%)',
      gridLines: '#78716c',
      islandGradient: ['#78716c', '#57534e', '#44403c', '#292524'],
      islandSelected: ['#ef4444', '#dc2626', '#b91c1c', '#991b1b'],
      islandComplete: ['#22c55e', '#16a34a', '#15803d', '#166534'],
      bridgeColor: '#44403c',
      bridgeGradient: ['#78716c', '#57534e', '#44403c', '#57534e', '#78716c'],
      textColor: '#ffffff',
      shadowColor: 'rgba(68, 64, 60, 0.9)'
    },
    animations: {
      islandFloat: 'islandFloat 2.2s ease-in-out infinite',
      particleEffect: 'zombieGlow 3s ease-in-out infinite'
    }
  },
  [PuzzleTheme.Ninja]: {
    name: 'Ninja Secret',
    colors: {
      background: 'linear-gradient(135deg, #0f172a 0%, #1e293b 50%, #0f172a 100%)',
      gridBackground: 'linear-gradient(135deg, #f1f5f9 0%, #e2e8f0 100%)',
      gridLines: '#64748b',
      islandGradient: ['#475569', '#334155', '#1e293b', '#0f172a'],
      islandSelected: ['#6366f1', '#4f46e5', '#4338ca', '#3730a3'],
      islandComplete: ['#10b981', '#059669', '#047857', '#065f46'],
      bridgeColor: '#1e293b',
      bridgeGradient: ['#475569', '#334155', '#1e293b', '#334155', '#475569'],
      textColor: '#ffffff',
      shadowColor: 'rgba(30, 41, 59, 0.9)'
    },
    animations: {
      islandFloat: 'islandFloat 2.8s ease-in-out infinite',
      particleEffect: 'smoke 4s ease-in-out infinite'
    }
  },
  [PuzzleTheme.Magic]: {
    name: 'Magie Enchantée',
    colors: {
      background: 'linear-gradient(135deg, #581c87 0%, #7c3aed 50%, #a855f7 100%)',
      gridBackground: 'linear-gradient(135deg, #faf5ff 0%, #f3e8ff 100%)',
      gridLines: '#c084fc',
      islandGradient: ['#a855f7', '#9333ea', '#7e22ce', '#6b21a8'],
      islandSelected: ['#c084fc', '#a855f7', '#9333ea', '#7e22ce'],
      islandComplete: ['#06b6d4', '#0891b2', '#0e7490', '#155e75'],
      bridgeColor: '#7e22ce',
      bridgeGradient: ['#c084fc', '#a855f7', '#9333ea', '#a855f7', '#c084fc'],
      textColor: '#ffffff',
      shadowColor: 'rgba(126, 34, 206, 0.9)'
    },
    animations: {
      islandFloat: 'islandFloat 3.5s ease-in-out infinite',
      bridgePulse: 'bridgePulse 2s ease-in-out infinite',
      particleEffect: 'magicSparkles 3s ease-in-out infinite'
    }
  },
  [PuzzleTheme.Western]: {
    name: 'Far West',
    colors: {
      background: 'linear-gradient(135deg, #78350f 0%, #92400e 50%, #b45309 100%)',
      gridBackground: 'linear-gradient(135deg, #fef3c7 0%, #fde68a 100%)',
      gridLines: '#d97706',
      islandGradient: ['#fbbf24', '#f59e0b', '#d97706', '#b45309'],
      islandSelected: ['#fcd34d', '#fbbf24', '#f59e0b', '#d97706'],
      islandComplete: ['#10b981', '#059669', '#047857', '#065f46'],
      bridgeColor: '#92400e',
      bridgeGradient: ['#f59e0b', '#d97706', '#b45309', '#d97706', '#f59e0b'],
      textColor: '#ffffff',
      shadowColor: 'rgba(146, 64, 14, 0.9)'
    },
    animations: {
      islandFloat: 'islandFloat 3s ease-in-out infinite'
    }
  }
}

/**
 * Récupère la configuration d'un thème
 */
export function getThemeConfig(theme: PuzzleTheme): ThemeConfig {
  return themeConfigs[theme] || themeConfigs[PuzzleTheme.Classic]
}

/**
 * Récupère le nom d'un thème
 */
export function getThemeName(theme: PuzzleTheme): string {
  return getThemeConfig(theme).name
}

