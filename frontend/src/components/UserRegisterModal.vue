<template>
  <div class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-[100]" @click.self="onCancel">
    <div class="bg-white rounded-2xl shadow-2xl p-8 w-full max-w-md animate-fade-in mx-4">
      <h2 class="text-2xl font-bold text-center mb-6 text-gray-800">
        Jouer / S’inscrire
      </h2>

      <form @submit.prevent="onSubmit" class="space-y-5">

        <!-- Champ Nom -->
        <div>
          <label class="block font-semibold text-gray-700 mb-1">Nom</label>
          <input
            v-model="name"
            type="text"
            class="w-full px-4 py-2 rounded-lg bg-gray-100 text-gray-900 border border-gray-300 focus:outline-none
                   focus:ring-2 focus:ring-blue-500 focus:bg-white transition"
            placeholder="Votre nom"
          />
        </div>

        <!-- Champ Email -->
        <div>
          <label class="block font-semibold text-gray-700 mb-1">Email</label>
          <input
            v-model="email"
            type="email"
            class="w-full px-4 py-2 rounded-lg bg-gray-100 text-gray-900 border border-gray-300 focus:outline-none
                   focus:ring-2 focus:ring-blue-500 focus:bg-white transition"
            placeholder="nom@example.com"
          />
        </div>

        <!-- Message d’erreur -->
        <p
          v-if="errorMessage"
          class="text-red-600 text-sm font-medium mt-2"
        >
          {{ errorMessage }}
        </p>

        <!-- Boutons -->
        <div class="flex justify-end gap-3 pt-4">
          <button
            type="button"
            class="px-4 py-2 rounded-lg bg-gray-300 hover:bg-gray-400 text-gray-800 transition"
            @click="onCancel"
          >
            Annuler
          </button>

          <button
            type="submit"
            :disabled="isSubmitting"
            class="px-5 py-2 rounded-lg bg-blue-600 hover:bg-blue-700 text-white font-medium
                   disabled:opacity-50 transition"
          >
            {{ isSubmitting ? "Envoi..." : "Continuer" }}
          </button>
        </div>

      </form>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref } from 'vue'
import { useUserStore } from '@/stores/user'
import { useUiStore } from '@/stores/ui'
import { useStatsStore } from '@/stores/stats'

const userStore = useUserStore()
const uiStore = useUiStore()
const statsStore = useStatsStore()

const name = ref('')
const email = ref('')
const isSubmitting = ref(false)
const errorMessage = ref<string | null>(null)

/** Validation simple */
function validate(): boolean {
  if (!name.value.trim() || !email.value.trim()) {
    errorMessage.value = 'Le nom et l’email sont obligatoires.'
    return false
  }
  if (!email.value.includes('@')) {
    errorMessage.value = 'Adresse email invalide.'
    return false
  }
  errorMessage.value = null
  return true
}

/** Soumission du formulaire */
async function onSubmit() {
  if (!validate()) return

  try {
    isSubmitting.value = true
    await userStore.register(name.value.trim(), email.value.trim())
    
    // Charger les statistiques de l'utilisateur après connexion
    if (userStore.user?.email) {
      try {
        await statsStore.loadUserStatsByEmail(userStore.user.email)
      } catch (err) {
        // Ignorer les erreurs si l'utilisateur n'a pas encore de statistiques
        console.log('Aucune statistique disponible pour cet utilisateur')
      }
    }
    
    uiStore.closeUserModal()
  } catch (err: unknown) {
    if (err instanceof Error) {
      errorMessage.value = err.message
    } else {
      errorMessage.value = 'Une erreur inconnue est survenue.'
    }
  } finally {
    isSubmitting.value = false
  }
}

/** Bouton Annuler */
function onCancel() {
  // Ne pas permettre de fermer le modal si l'utilisateur n'est pas connecté
  // L'utilisateur doit créer un compte pour continuer
  if (!userStore.isLoggedIn) {
    return;
  }
  uiStore.closeUserModal()
}
</script>

<style scoped>
/* Animation légère d’apparition */
@keyframes fade-in {
  from {
    opacity: 0;
    transform: scale(0.95);
  }
  to {
    opacity: 1;
    transform: scale(1);
  }
}

.animate-fade-in {
  animation: fade-in 0.2s ease-out;
}
</style>
