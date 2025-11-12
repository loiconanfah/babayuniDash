import { fileURLToPath, URL } from 'node:url';
import { defineConfig } from 'vite';
import plugin from '@vitejs/plugin-vue';
import vueDevTools from 'vite-plugin-vue-devtools';
import { env } from 'process';

// Port du serveur Vite (peut être configuré via env)
const port = parseInt(env.DEV_SERVER_PORT || '5173');

// URL du backend ASP.NET Core
const backendUrl = env.ASPNETCORE_HTTPS_PORT 
    ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
    : env.ASPNETCORE_URLS 
    ? env.ASPNETCORE_URLS.split(';')[0] 
    : 'https://localhost:5001';

// https://vitejs.dev/config/
export default defineConfig({
    plugins: [
        plugin(),
        vueDevTools(),
    ],
    resolve: {
        alias: {
            '@': fileURLToPath(new URL('./src', import.meta.url))
        }
    },
    server: {
        port: port,
        strictPort: true,
        // Proxy vers le backend pour les appels API
        proxy: {
            '/api': {
                target: backendUrl,
                changeOrigin: true,
                secure: false,
                rewrite: (path) => path
            }
        },
        // Configurer pour accepter les connexions depuis le backend
        cors: true,
        hmr: {
            clientPort: port
        }
    },
    build: {
        outDir: 'dist',
        emptyOutDir: true
    }
})
