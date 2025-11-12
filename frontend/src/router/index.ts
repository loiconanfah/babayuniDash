/**
 * Configuration du routeur Vue Router
 * Définit toutes les routes de l'application Hashi
 */

import { createRouter, createWebHistory } from 'vue-router'
import MenuView from '../views/MenuView.vue'

const router = createRouter({
  history: createWebHistory(import.meta.env.BASE_URL),
  routes: [
    {
      path: '/',
      name: 'menu',
      component: MenuView,
      meta: {
        title: 'Menu Principal - Hashi'
      }
    },
    {
      path: '/puzzles',
      name: 'puzzle-selection',
      // Lazy loading pour optimiser les performances
      component: () => import('../views/PuzzleSelectionView.vue'),
      meta: {
        title: 'Sélectionner un Puzzle - Hashi'
      }
    },
    {
      path: '/generate',
      name: 'generate-puzzle',
      component: () => import('../views/GeneratePuzzleView.vue'),
      meta: {
        title: 'Générer un Puzzle - Hashi'
      }
    },
    {
      path: '/play/:id',
      name: 'play-game',
      component: () => import('../views/GameView.vue'),
      meta: {
        title: 'Jouer - Hashi'
      },
      props: true
    },
    {
      // Route 404 - Redirige vers le menu
      path: '/:pathMatch(.*)*',
      redirect: '/'
    }
  ],
})

// Guard de navigation pour mettre à jour le titre de la page
router.beforeEach((to, from, next) => {
  const title = to.meta.title as string || 'Hashi'
  document.title = title
  next()
})

export default router
