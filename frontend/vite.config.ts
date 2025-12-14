import { fileURLToPath, URL } from 'node:url'

import { defineConfig } from 'vite'
import tailwindcss from '@tailwindcss/vite'
import vue from '@vitejs/plugin-vue'
import vueDevTools from 'vite-plugin-vue-devtools'

// https://vite.dev/config/
export default defineConfig({
  plugins: [
    vue(),
    tailwindcss(),
    vueDevTools(),
  ],
  resolve: {
    alias: {
      '@': fileURLToPath(new URL('./src', import.meta.url))
    },
  },
  server: {
    strictPort: false, // Permet d'utiliser un autre port si celui spécifié est occupé
    host: true, // Écouter sur toutes les interfaces réseau (nécessaire pour ngrok)
    allowedHosts: [
      '.ngrok.io',
      '.ngrok-free.app',
      '.ngrok.app',
      'localhost',
      '127.0.0.1'
    ],
    proxy: {
      // Proxy toutes les requêtes /api/* vers le backend ASP.NET Core
      '/api': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        secure: false,
        // Ne pas réécrire l'URL, garder /api dans le chemin
        rewrite: (path) => path,
        // Gérer les erreurs de connexion silencieusement
        configure: (proxy, _options) => {
          proxy.on('error', (err: any, _req, _res) => {
            // Ignorer les erreurs ECONNREFUSED si le serveur backend n'est pas encore démarré
            // Ces erreurs sont normales au démarrage et disparaîtront une fois le serveur prêt
            if (err.code !== 'ECONNREFUSED' && err.code !== 'ETIMEDOUT') {
              console.error('Proxy error:', err)
            }
          })
        },
        // Réessayer automatiquement en cas d'échec
        timeout: 10000,
        // Ne pas échouer si le serveur n'est pas disponible
        ws: true
      },
      // Proxy pour les fichiers statiques uploadés (images)
      '/uploads': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path,
        configure: (proxy, _options) => {
          proxy.on('error', (err: any, _req, _res) => {
            if (err.code !== 'ECONNREFUSED' && err.code !== 'ETIMEDOUT') {
              console.error('Proxy error (uploads):', err)
            }
          })
        },
        timeout: 10000,
        ws: false
      },
      // Proxy pour les assets statiques (images d'items)
      '/assets': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path,
        configure: (proxy, _options) => {
          proxy.on('error', (err: any, _req, _res) => {
            if (err.code !== 'ECONNREFUSED' && err.code !== 'ETIMEDOUT') {
              console.error('Proxy error (assets):', err)
            }
          })
        },
        timeout: 10000,
        ws: false
      },
      // Proxy pour SignalR (WebSockets)
      '/hubs': {
        target: 'http://localhost:5000',
        changeOrigin: true,
        secure: false,
        rewrite: (path) => path,
        ws: true,
        configure: (proxy, _options) => {
          proxy.on('error', (err: any, _req, _res) => {
            if (err.code !== 'ECONNREFUSED' && err.code !== 'ETIMEDOUT') {
              console.error('Proxy error (SignalR):', err)
            }
          })
        },
        timeout: 10000
      }
    }
    // Note: open: true est désactivé car nous gérons l'ouverture des navigateurs via le script PowerShell
    // pour éviter les conflits lors du lancement de plusieurs instances
  }
})
