<template>
  <div class="fixed inset-0 bg-black/60 backdrop-blur-sm flex items-center justify-center z-50">
    <div
      class="bg-white rounded-2xl shadow-2xl w-full max-w-2xl max-h-[80vh] flex flex-col animate-fade-in"
    >
      <!-- En-tête -->
      <div class="px-6 py-4 border-b border-gray-200 flex items-center justify-between">
        <h2 class="text-xl font-bold text-gray-900">
          Règles du jeu – Prison Break Hashi
        </h2>
        <button
          class="text-gray-500 hover:text-gray-800 text-xl leading-none"
          @click="onClose"
          aria-label="Fermer le tutoriel"
        >
          ×
        </button>
      </div>

      <!-- Contenu scrollable -->
      <div class="px-6 py-4 overflow-y-auto text-sm leading-relaxed text-gray-800 space-y-4">
        <p>
          Tu es un prisonnier enfermé dans une prison haute sécurité. Chaque puzzle représente
          le plan de ta cellule vue du dessus. Les ronds numérotés sont des
          <strong>verrous</strong> et ton objectif est de placer des
          <strong>passerelles</strong> entre eux pour créer un chemin d’évasion cohérent.
        </p>

        <h3 class="font-semibold text-base mt-2">Objectif du jeu</h3>
        <p>
          Connecter tous les <strong>verrous</strong> de la cellule en traçant des
          <strong>passerelles</strong> horizontales ou verticales, de façon à :
        </p>
        <ul class="list-disc list-inside space-y-1">
          <li>respecter le nombre indiqué sur chaque verrou ;</li>
          <li>obtenir un seul réseau connecté (tous les verrous doivent être reliés) ;</li>
          <li>ne jamais faire croiser deux passerelles ;</li>
          <li>ne tracer que des passerelles horizontales ou verticales (jamais en diagonale).</li>
        </ul>

        <h3 class="font-semibold text-base mt-2">Les éléments principaux</h3>
        <ul class="list-disc list-inside space-y-1">
          <li>
            <strong>Verrou :</strong> cercle contenant un nombre. Ce nombre indique combien de
            passerelles doivent partir de ce verrou (en tout).
          </li>
          <li>
            <strong>Passerelles :</strong> segments qui relient deux verrous alignés. Elles
            peuvent être <strong>simples</strong> (1 trait) ou <strong>doubles</strong> (2 traits
            parallèles). Une passerelle double compte pour 2.
          </li>
          <li>
            <strong>Cellule :</strong> la grille complète du puzzle. Quand tous les verrous sont
            correctement reliés, la cellule est “déverrouillée” et tu peux t’évader.
          </li>
        </ul>

        <h3 class="font-semibold text-base mt-2">Règles détaillées</h3>
        <ol class="list-decimal list-inside space-y-2">
          <li>
            <span class="font-semibold">Respect des nombres :</span>
            le nombre inscrit dans un verrou est le nombre total de passerelles qui doivent
            partir de ce verrou. Exemple : un verrou “3” doit être relié par 3 passerelles au
            total (1+2, 1+1+1, 3 simples, etc.).
          </li>
          <li>
            <span class="font-semibold">Alignement :</span>
            deux verrous peuvent être reliés seulement s’ils sont exactement sur la même ligne
            ou la même colonne, sans autre verrou entre les deux.
          </li>
          <li>
            <span class="font-semibold">Pas de croisements :</span>
            aucune passerelle ne peut croiser une autre. Si deux segments se coupent,
            l’évasion est impossible et la configuration est invalide.
          </li>
          <li>
            <span class="font-semibold">Réseau unique :</span>
            à la fin, tous les verrous doivent être connectés entre eux, directement ou
            indirectement. Il ne doit pas rester de groupe isolé de verrous.
          </li>
          <li>
            <span class="font-semibold">Passerelles simples ou doubles :</span>
            entre deux verrous, tu peux placer 0, 1 ou 2 passerelles. Jamais plus de deux,
            jamais en diagonale.
          </li>
        </ol>

        <h3 class="font-semibold text-base mt-2">Contrôles (version jeu)</h3>
        <ul class="list-disc list-inside space-y-1">
          <li>
            Clique sur un verrou puis sur un autre verrou aligné pour créer une passerelle entre
            eux.
          </li>
          <li>
            Reclique sur la même connexion pour la transformer en passerelle double.
          </li>
          <li>
            Reclique encore une fois pour supprimer la passerelle.
          </li>
          <li>
            Tu peux modifier tes passerelles tant que la cellule n’est pas validée.
          </li>
        </ul>

        <h3 class="font-semibold text-base mt-2">Conditions de victoire</h3>
        <p>Tu réussis ton évasion lorsque :</p>
        <ul class="list-disc list-inside space-y-1">
          <li>
            chaque verrou a exactement le nombre de passerelles indiqué ;
          </li>
          <li>
            aucune passerelle ne se croise ;
          </li>
          <li>
            tous les verrous forment un seul réseau connecté ;
          </li>
          <li>
            tu as réussi avant la fin du temps et sans perdre toutes tes vies.
          </li>
        </ul>

        <h3 class="font-semibold text-base mt-2">Niveaux de difficulté</h3>
        <ul class="list-disc list-inside space-y-1">
          <li>
            <strong>Cellule d’isolement :</strong> petite grille, peu de verrous. Idéal pour
            apprendre les règles.
          </li>
          <li>
            <strong>Aile de détention B :</strong> plus de verrous, plus de passerelles, plus
            de pièges possibles.
          </li>
          <li>
            <strong>Mirador – Dernière barrière :</strong> grande grille, beaucoup de verrous.
            C’est ta dernière chance de t’évader.
          </li>
        </ul>

        <p class="text-gray-700">
          Quand tu te sens prêt, ferme ce tutoriel et clique sur
          <span class="font-semibold">Jouer</span> pour tenter ta prochaine évasion !
        </p>
      </div>

      <!-- Pied -->
      <div class="px-6 py-3 border-t border-gray-200 flex justify-end">
        <button
          class="px-4 py-2 rounded-lg bg-blue-600 hover:bg-blue-700 text-white font-medium transition"
          @click="onClose"
        >
          J’ai compris, fermer
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useUiStore } from '@/stores/ui';

const uiStore = useUiStore();

function onClose() {
  uiStore.closeTutorialModal();
}
</script>

<style scoped>
@keyframes fade-in {
  from {
    opacity: 0;
    transform: translateY(8px) scale(0.98);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

.animate-fade-in {
  animation: fade-in 0.18s ease-out;
}
</style>
