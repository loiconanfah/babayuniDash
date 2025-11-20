<template>
  <section class="w-full h-full flex items-center justify-center px-4 lg:px-10 py-8">
    <div class="w-full max-w-5xl">
      <!-- Titre + intro -->
      <header class="mb-8">
        <p class="text-sm uppercase tracking-[0.3em] text-orange-300 mb-2">
          Prison Break
        </p>
        <h1 class="text-3xl sm:text-4xl lg:text-5xl font-bold text-slate-50 mb-3">
          {{ greeting }}
        </h1>
        <p class="text-sm sm:text-base text-slate-300 max-w-2xl">
          Tu es enfermé dans une prison haute sécurité. Résous les puzzles des verrous pour
          connecter toutes les clés et t’échapper avant la prochaine inspection&nbsp;!
        </p>
      </header>

      <!-- Statut du prisonnier -->
      <section class="mb-6">
        <h2 class="text-sm font-semibold text-slate-100 mb-1">
          Statut du prisonnier
        </h2>
        <div class="flex items-center gap-3 text-sm">
          <span class="text-slate-300">
            {{ isLoggedIn ? 'Identité confirmée' : 'Non enregistré' }}
          </span>
          <span
            class="px-3 py-1 rounded-full text-xs font-semibold"
            :class="
              isLoggedIn
                ? 'bg-green-500/90 text-slate-950'
                : 'bg-orange-800/90 text-orange-100'
            "
          >
            {{ isLoggedIn ? 'SESSION ACTIVE' : 'SESSION INVITÉE' }}
          </span>
        </div>
      </section>

      <!-- Cartes visuelles -->
      <section
        class="grid grid-cols-1 md:grid-cols-2 gap-4 md:gap-6 mb-8"
      >
        <!-- Carte puzzle / verrous -->
        <div
          class="rounded-3xl bg-slate-900 border border-slate-800 shadow-xl p-4 flex flex-col justify-between"
        >
          <div class="mb-3">
            <p class="text-xs uppercase tracking-[0.2em] text-slate-400 mb-1">
              Plan de la cellule
            </p>
            <p class="text-sm text-slate-200">
              Vue schématique des verrous à relier pour préparer ton évasion.
            </p>
          </div>

          <!-- Faux visuel de puzzle/verrous (à remplacer par une vraie image plus tard si tu veux) -->
          <div
            class="mt-2 flex-1 rounded-2xl bg-slate-800/70 border border-slate-700 grid grid-cols-3 grid-rows-3 place-items-center gap-2 px-4 py-3"
          >
            <div class="h-9 w-9 rounded-full bg-slate-900 border border-slate-500 flex items-center justify-center text-xs">
              1
            </div>
            <div class="h-9 w-9 rounded-full bg-slate-900 border border-slate-500 flex items-center justify-center text-xs">
              2
            </div>
            <div class="h-9 w-9 rounded-full bg-slate-900 border border-slate-500 flex items-center justify-center text-xs">
              2
            </div>
            <div class="col-span-3 h-1 w-3/4 bg-slate-600 rounded-full"></div>
            <div class="h-9 w-9 rounded-full bg-slate-900 border border-slate-500 flex items-center justify-center text-xs">
              3
            </div>
            <div class="h-9 w-9 rounded-full bg-slate-900 border border-slate-500 flex items-center justify-center text-xs">
              4
            </div>
            <div class="h-9 w-9 rounded-full bg-slate-900 border border-slate-500 flex items-center justify-center text-xs">
              1
            </div>
          </div>
        </div>

        <!-- Carte prisonnier / mugshot -->
        <div
          class="rounded-3xl bg-slate-900 border border-slate-800 shadow-xl p-4 flex flex-col justify-between"
        >
          <div class="mb-3">
            <p class="text-xs uppercase tracking-[0.2em] text-slate-400 mb-1">
              Dossier du prisonnier
            </p>
            <p class="text-sm text-slate-200">
              Numéro d’identification et identité utilisée pour le classement et les statistiques.
            </p>
          </div>

          <!-- Contenu différent selon l'état connexion -->
          <div
            class="mt-2 flex-1 rounded-2xl bg-slate-800/70 border border-slate-700 flex items-center justify-center px-4 py-3"
          >
            <!-- AVANT enregistrement : prisonnier générique fâché -->
            <div
              v-if="!isLoggedIn"
              class="flex flex-col items-center gap-2 text-xs text-slate-200"
            >
              <div
                class="h-16 w-16 rounded-full bg-orange-500 flex items-center justify-center text-3xl"
              >
                😠
              </div>
              <p class="font-semibold">Prisonnier #000</p>
              <p class="text-[11px] text-slate-400 text-center">
                Identité non enregistrée. Utilise “Jouer / S’inscrire” pour créer ton dossier.
              </p>
            </div>

            <!-- APRÈS enregistrement : mugshot -->
            <div
              v-else
              class="flex items-center gap-4 w-full"
            >
              <!-- Silhouette du prisonnier -->
              <div
                class="h-20 w-20 rounded-2xl bg-orange-500 flex items-center justify-center text-3xl text-slate-900"
              >
                😠
              </div>
              <!-- Ardoise + infos -->
              <div class="flex-1">
                <div
                  class="w-full rounded-lg bg-slate-950 border border-slate-600 px-3 py-2 mb-2 flex flex-col items-center"
                >
                  <span class="text-xs text-slate-300">
                    {{ userNameLabel }}
                  </span>
                  <span class="text-sm font-mono text-slate-50">
                    #{{ userIdLabel }}
                  </span>
                </div>
                <p class="text-[11px] text-slate-400">
                  Identité utilisée pour suivre tes cellules résolues, tes temps d’évasion et ton classement.
                </p>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Boutons principaux -->
      <section class="flex flex-wrap gap-3">
        <button
          v-if="!isLoggedIn"
          class="px-6 py-3 rounded-full bg-orange-500 text-slate-900 text-sm font-semibold hover:bg-orange-400 transition-colors"
          @click="onPlayOrRegister"
        >
          Jouer / S'inscrire
        </button>

        <button
          v-else
          class="px-6 py-3 rounded-full bg-green-500 text-slate-900 text-sm font-semibold hover:bg-green-400 transition-colors"
          @click="onPlay"
        >
          Jouer
        </button>

        <button
          class="px-6 py-3 rounded-full bg-slate-800 text-slate-100 text-sm font-semibold hover:bg-slate-700 transition-colors"
          @click="onTutorial"
        >
          Tutoriel
        </button>
      </section>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed } from 'vue';
import { useUserStore } from '@/stores/user';
import { useUiStore } from '@/stores/ui';

const userStore = useUserStore();
const uiStore = useUiStore();

const isLoggedIn = computed(() => userStore.isLoggedIn);

// Titre principal
const greeting = computed(() =>
  userStore.user ? `Bonsoir, ${userStore.user.name}` : 'Bonsoir, Joueur'
);

// Nom affiché sur l'ardoise après enregistrement
const userNameLabel = computed(() =>
  userStore.user ? userStore.user.name.toUpperCase() : 'PRISONNIER'
);

// Numéro de prisonnier (#000 avant enregistrement, #XYZ après)
const userIdLabel = computed(() => {
  if (!userStore.user) {
    return '000';
  }
  return userStore.user.id.toString().padStart(3, '0');
});

function onPlayOrRegister() {
  uiStore.openUserModal();
}

function onPlay() {
  uiStore.goToLevels();
}

function onTutorial() {
  uiStore.openTutorialModal();
}
</script>
