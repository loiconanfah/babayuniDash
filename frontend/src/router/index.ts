import { createRouter, createWebHistory } from 'vue-router'
import MenuView from '@/views/MenuView.vue'
import PuzzleSelectionView from '@/views/PuzzleSelectionView.vue'
import GameView from '@/views/GameView.vue'
import GeneratePuzzleView from '@/views/GeneratePuzzleView.vue'
import StatsView from '@/views/StatsView.vue'
import TicTacToeView from '@/views/TicTacToeView.vue'
import GamesView from '@/views/GamesView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'menu',
      component: MenuView
    },
    {
      path: '/puzzles',
      name: 'puzzles',
      component: PuzzleSelectionView
    },
    {
      path: '/game/:id',
      name: 'game',
      component: GameView
    },
    {
      path: '/generate',
      name: 'generate',
      component: GeneratePuzzleView
    },
    {
      path: '/stats',
      name: 'stats',
      component: StatsView
    },
    {
      path: '/tic-tac-toe',
      name: 'tic-tac-toe',
      component: TicTacToeView
    },
    {
      path: '/games',
      name: 'games',
      component: GamesView
    }
  ]
})

export default router
