<script setup lang="ts">
import { RouterLink, RouterView } from 'vue-router'
import HelloWorld from './components/HelloWorld.vue'
</script>

<template>
  <header class="flex flex-col items-center justify-center min-h-screen bg-gradient-to-br from-blue-100 via-indigo-100 to-purple-200 p-8">
    <img
      alt="Vue logo"
      class="w-32 h-32 mb-6 animate-bounce"
      src="@/assets/logo.svg"
    />

    <div class="text-center">
      <HelloWorld msg="Tailwind is working 🎉" />

      <nav class="mt-6 space-x-4 text-lg font-semibold">
        <RouterLink
          to="/"
          class="text-indigo-600 hover:text-indigo-800 transition-colors"
        >
          Home
        </RouterLink>
        <RouterLink
          to="/about"
          class="text-indigo-600 hover:text-indigo-800 transition-colors"
        >
          About
        </RouterLink>
      </nav>
    </div>
  </header>

  <main class="p-8">
    <RouterView />
  </main>
</template>

<style scoped>
/* aucun style manuel ici : tout est géré par Tailwind */
</style>
