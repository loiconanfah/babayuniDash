<script setup lang="ts">
/**
 * Composant racine de l'application Hashi
 * Gère le layout global et le routing
 */

import { RouterView } from 'vue-router'
</script>

<template>
  <div id="app">
    <!-- RouterView affiche le composant correspondant à la route active -->
    <RouterView />
  </div>
</template>

<style>
/**
 * Styles globaux de l'application
 * Reset CSS et styles de base
 */

/* Reset et styles de base */
* {
  margin: 0;
  padding: 0;
  box-sizing: border-box;
}

html,
body {
  height: 100%;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen',
    'Ubuntu', 'Cantarell', 'Fira Sans', 'Droid Sans', 'Helvetica Neue',
    sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
}

#app {
  min-height: 100vh;
}

/* Désactiver la sélection de texte sur les éléments interactifs */
button,
.island,
.bridge {
  user-select: none;
  -webkit-user-select: none;
  -moz-user-select: none;
  -ms-user-select: none;
}

/* Transitions globales */
* {
  transition-property: background-color, border-color, color, fill, stroke;
  transition-duration: 200ms;
  transition-timing-function: ease;
}

/* Scrollbar personnalisée */
::-webkit-scrollbar {
  width: 8px;
  height: 8px;
}

::-webkit-scrollbar-track {
  background: #f1f1f1;
}

::-webkit-scrollbar-thumb {
  background: #888;
  border-radius: 4px;
}

::-webkit-scrollbar-thumb:hover {
  background: #555;
}
</style>
