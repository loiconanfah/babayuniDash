<template>
  <section class="w-full h-full px-4 sm:px-6 lg:px-12 py-8 overflow-y-auto">
    <div class="max-w-6xl mx-auto">
      <!-- En-tête -->
      <header class="mb-6">
        <div class="flex items-center justify-between">
          <div>
            <h2 class="text-3xl sm:text-4xl font-bold text-slate-50 mb-2">
              🗝️ Aventure Mystérieuse
            </h2>
            <p class="text-sm text-slate-300">
              Explorez le château, collectez des objets et résolvez des énigmes !
            </p>
          </div>
          <button
            v-if="!currentGame"
            @click="goBack"
            class="px-4 py-2 rounded-xl bg-slate-800 text-slate-300 hover:bg-slate-700 transition-colors text-sm font-medium"
          >
            Retour
          </button>
          <button
            v-else-if="!adventureStore.isGameOver"
            @click="handleAbandon"
            class="px-4 py-2 rounded-xl bg-red-600 text-white hover:bg-red-500 transition-colors text-sm font-medium"
          >
            Abandonner
          </button>
        </div>
      </header>

      <!-- Message d'erreur -->
      <div v-if="error" class="mb-4 p-4 rounded-xl bg-red-900/80 border border-red-700 text-red-100 flex items-center justify-between">
        <span>{{ error }}</span>
        <button @click="clearError" class="text-red-300 hover:text-red-100 text-xl leading-none">×</button>
      </div>

      <!-- Chargement -->
      <div v-if="isLoading" class="text-center py-12">
        <div class="inline-block animate-spin rounded-full h-12 w-12 border-b-2 border-emerald-500 mb-4"></div>
        <p class="text-slate-300">Chargement...</p>
      </div>

      <!-- Partie en cours -->
      <div v-else-if="currentGame" class="space-y-6">
        <!-- Statistiques -->
        <div class="grid grid-cols-1 sm:grid-cols-3 gap-4">
          <div class="rounded-2xl bg-gradient-to-br from-emerald-900/50 to-emerald-800/50 border border-emerald-700/50 p-4">
            <div class="text-sm font-medium text-emerald-300 mb-1">Score</div>
            <div class="text-3xl font-bold text-emerald-200">{{ currentGame.score }}</div>
          </div>
          <div class="rounded-2xl bg-gradient-to-br from-blue-900/50 to-blue-800/50 border border-blue-700/50 p-4">
            <div class="text-sm font-medium text-blue-300 mb-1">Énigmes résolues</div>
            <div class="text-3xl font-bold text-blue-200">{{ currentGame.puzzlesSolved }}/15</div>
          </div>
          <div class="rounded-2xl bg-gradient-to-br from-purple-900/50 to-purple-800/50 border border-purple-700/50 p-4">
            <div class="text-sm font-medium text-purple-300 mb-1">Objets collectés</div>
            <div class="text-3xl font-bold text-purple-200">{{ currentGame.collectedItems.length }}</div>
          </div>
        </div>

        <!-- Salle actuelle avec animation -->
        <div class="relative rounded-3xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border-2 border-emerald-500/50 p-8 overflow-hidden">
          <!-- Animation de fond -->
          <div class="absolute inset-0 opacity-10">
            <div class="absolute top-0 left-0 w-64 h-64 bg-emerald-500 rounded-full blur-3xl animate-pulse"></div>
            <div class="absolute bottom-0 right-0 w-64 h-64 bg-blue-500 rounded-full blur-3xl animate-pulse" style="animation-delay: 1s"></div>
          </div>
          
          <div class="relative z-10">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-2xl font-bold text-emerald-300">{{ getCurrentRoomInfo().name }}</h3>
              <div class="px-4 py-2 rounded-xl bg-emerald-500/20 border border-emerald-500/50 text-emerald-300 font-semibold">
                Salle {{ currentGame.currentRoom }}
              </div>
            </div>
            
            <p class="text-slate-200 text-lg mb-6 leading-relaxed">
              {{ getCurrentRoomInfo().description }}
            </p>

            <!-- Objets disponibles dans la salle -->
            <div v-if="getAvailableItems().length > 0" class="mb-6">
              <h4 class="text-lg font-semibold text-slate-300 mb-3">Objets disponibles :</h4>
              <div class="flex flex-wrap gap-3">
                <button
                  v-for="item in getAvailableItems()"
                  :key="item.name"
                  @click="handleCollectItem(item.name)"
                  class="group relative px-4 py-3 rounded-xl bg-gradient-to-br from-amber-900/50 to-amber-800/50 border-2 border-amber-600/50 hover:border-amber-400 hover:scale-105 transition-all"
                  :disabled="isLoading"
                >
                  <div class="text-3xl mb-2 transform group-hover:rotate-12 transition-transform">{{ getItemEmoji(item.name) }}</div>
                  <div class="text-sm font-semibold text-amber-200">{{ item.displayName }}</div>
                  <div class="text-xs text-amber-400 mt-1">{{ item.description }}</div>
                </button>
              </div>
            </div>

            <!-- Énigmes disponibles -->
            <div v-if="getAvailablePuzzles().length > 0" class="mb-6">
              <h4 class="text-lg font-semibold text-slate-300 mb-3">Énigmes :</h4>
              <div class="space-y-3">
                <div
                  v-for="puzzle in getAvailablePuzzles()"
                  :key="puzzle.id"
                  class="rounded-xl bg-gradient-to-br from-purple-900/50 to-purple-800/50 border border-purple-700/50 p-4"
                >
                  <div class="flex items-start justify-between mb-3">
                    <div class="flex-1">
                      <div class="flex items-center gap-2 mb-2">
                        <span class="text-sm font-medium text-purple-300">Énigme #{{ puzzle.id }}</span>
                        <span v-if="puzzle.miniGameType" class="px-2 py-1 rounded-lg bg-yellow-500/20 text-yellow-300 text-xs font-semibold">
                          🎮 Mini-jeu
                        </span>
                      </div>
                      <div class="text-slate-200 mb-3">{{ puzzle.question }}</div>
                      <!-- Indices -->
                      <div v-if="puzzleHints[puzzle.id] && puzzleHints[puzzle.id].length > 0" class="mb-3">
                        <button
                          @click="showHints = showHints === puzzle.id ? null : puzzle.id"
                          class="text-xs text-purple-400 hover:text-purple-300 flex items-center gap-1"
                        >
                          💡 {{ showHints === puzzle.id ? 'Masquer' : 'Voir' }} les indices ({{ puzzleHints[puzzle.id].length }})
                        </button>
                        <div v-if="showHints === puzzle.id" class="mt-2 space-y-1">
                          <div
                            v-for="(hint, index) in puzzleHints[puzzle.id]"
                            :key="index"
                            class="text-xs text-slate-400 bg-slate-800/50 p-2 rounded"
                          >
                            💡 Indice {{ index + 1 }}: {{ hint }}
                          </div>
                        </div>
                      </div>
                    </div>
                    <div class="px-3 py-1 rounded-lg bg-purple-500/20 text-purple-300 text-xs font-semibold">
                      {{ puzzle.points }} pts
                    </div>
                  </div>
                  <div class="flex gap-2">
                    <button
                      v-if="puzzle.miniGameType"
                      @click="openMiniGame(puzzle.id, puzzle.miniGameType)"
                      class="px-4 py-2 rounded-lg bg-gradient-to-r from-yellow-500 to-yellow-600 text-white font-semibold hover:from-yellow-400 hover:to-yellow-500 transition-all"
                    >
                      🎮 Jouer au mini-jeu
                    </button>
                    <input
                      v-if="!puzzle.miniGameType"
                      v-model="puzzleAnswers[puzzle.id]"
                      type="text"
                      placeholder="Votre réponse..."
                      class="flex-1 px-4 py-2 rounded-lg bg-slate-800 border border-slate-700 text-slate-200 focus:border-purple-500 focus:outline-none"
                      @keyup.enter="handleSolvePuzzle(puzzle.id, puzzleAnswers[puzzle.id] || '')"
                    />
                    <button
                      v-if="!puzzle.miniGameType"
                      @click="handleSolvePuzzle(puzzle.id, puzzleAnswers[puzzle.id] || '')"
                      class="px-6 py-2 rounded-lg bg-gradient-to-r from-purple-500 to-purple-600 text-white font-semibold hover:from-purple-400 hover:to-purple-500 transition-all"
                      :disabled="isLoading || !puzzleAnswers[puzzle.id]"
                    >
                      Résoudre
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <!-- Navigation vers d'autres salles -->
            <div class="mt-6">
              <h4 class="text-lg font-semibold text-slate-300 mb-3">Salles disponibles :</h4>
              <div class="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-5 gap-3">
                <button
                  v-for="room in getAvailableRooms()"
                  :key="room.number"
                  @click="handleMoveToRoom(room.number)"
                  class="group relative px-4 py-3 rounded-xl bg-gradient-to-br from-slate-800 to-slate-900 border-2 border-slate-700 hover:border-emerald-500 hover:scale-105 transition-all"
                  :disabled="isLoading"
                >
                  <div class="text-4xl mb-2 transform group-hover:scale-110 transition-transform">{{ getRoomEmoji(room.number) }}</div>
                  <div class="text-sm font-semibold text-slate-200">{{ room.name }}</div>
                </button>
              </div>
            </div>
          </div>
        </div>

        <!-- Inventaire -->
        <div class="rounded-2xl bg-gradient-to-br from-amber-900/50 to-amber-800/50 border border-amber-700/50 p-6">
          <h3 class="text-xl font-bold text-amber-300 mb-4 flex items-center gap-2">
            <span>🎒</span>
            <span>Inventaire</span>
          </h3>
          <div v-if="currentGame.collectedItems.length > 0" class="flex flex-wrap gap-3">
            <div
              v-for="item in currentGame.collectedItems"
              :key="item"
              class="group relative px-4 py-3 rounded-xl bg-gradient-to-br from-amber-800/70 to-amber-900/70 border-2 border-amber-600/50 hover:border-amber-400 transition-all"
            >
              <div class="text-3xl mb-1 transform group-hover:rotate-12 transition-transform">{{ getItemEmoji(item) }}</div>
              <div class="text-xs font-semibold text-amber-200">{{ getItemDisplayName(item) }}</div>
            </div>
          </div>
          <div v-else class="text-slate-400 text-center py-4">
            Votre inventaire est vide. Explorez les salles pour trouver des objets !
          </div>
        </div>

        <!-- Message de victoire -->
        <div v-if="adventureStore.isGameOver && currentGame.status === AdventureGameStatus.Completed" class="text-center">
          <div class="inline-block p-8 rounded-3xl bg-gradient-to-br from-emerald-900/80 to-emerald-800/80 border-2 border-emerald-500/50">
            <div class="text-8xl mb-4 animate-bounce">🏆</div>
            <h3 class="text-3xl font-bold text-emerald-300 mb-2">Félicitations !</h3>
            <p class="text-xl text-emerald-200 mb-4">Vous avez résolu {{ currentGame.puzzlesSolved }} énigmes !</p>
            <p class="text-lg text-slate-300">Score final : <span class="font-bold text-emerald-300">{{ currentGame.score }}</span> points</p>
            <button
              @click="handleNewGame"
              class="mt-6 px-8 py-4 rounded-xl bg-gradient-to-r from-emerald-500 to-emerald-600 text-white font-bold hover:from-emerald-400 hover:to-emerald-500 transition-all shadow-lg shadow-emerald-500/30 text-lg"
            >
              Nouvelle aventure
            </button>
          </div>
        </div>
      </div>

      <!-- Menu de sélection (pas de partie en cours) -->
      <div v-else class="space-y-6">
        <!-- Carte d'introduction -->
        <div class="rounded-3xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border-2 border-emerald-500/50 p-8 text-center">
          <div class="text-8xl mb-6 animate-pulse">🗝️</div>
          <h3 class="text-3xl font-bold text-emerald-300 mb-4">Bienvenue dans l'Aventure !</h3>
          <p class="text-slate-300 text-lg mb-6 max-w-2xl mx-auto">
            Explorez un mystérieux château rempli d'énigmes et de secrets. 
            Collectez des objets, résolvez des puzzles et découvrez le trésor caché !
          </p>
          <button
            @click="handleStartAdventure"
            class="px-8 py-4 rounded-xl bg-gradient-to-r from-emerald-500 to-emerald-600 text-white font-bold hover:from-emerald-400 hover:to-emerald-500 transition-all shadow-lg shadow-emerald-500/30 text-lg"
            :disabled="isLoading"
          >
            🚀 Commencer l'aventure
          </button>
        </div>

        <!-- Parties précédentes -->
        <div v-if="playerGames.length > 0">
          <h3 class="text-lg font-bold text-slate-50 mb-4">Vos aventures précédentes</h3>
          <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4">
            <button
              v-for="game in playerGames"
              :key="game.id"
              @click="loadGame(game.id)"
              class="rounded-xl bg-gradient-to-br from-slate-800/50 to-slate-900/50 border border-slate-700 p-4 hover:border-emerald-500/50 hover:bg-slate-800 transition-all text-left"
            >
              <div class="flex items-center justify-between mb-2">
                <div class="text-sm font-medium text-slate-300">Aventure #{{ game.id }}</div>
                <div class="px-2 py-1 rounded-lg bg-emerald-500/20 text-emerald-300 text-xs font-semibold">
                  {{ game.status === AdventureGameStatus.Completed ? 'Terminée' : 'En cours' }}
                </div>
              </div>
              <div class="text-xs text-slate-400 space-y-1">
                <div>Score : {{ game.score }} pts</div>
                <div>Énigmes : {{ game.puzzlesSolved }}/5</div>
                <div>Objets : {{ game.collectedItems.length }}</div>
              </div>
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Modale pour les mini-jeux -->
    <div
      v-if="activeMiniGame"
      class="fixed inset-0 bg-black/80 backdrop-blur-sm z-50 flex items-center justify-center p-4"
      @click.self="closeMiniGame"
    >
      <div class="bg-gradient-to-br from-slate-900 to-slate-800 rounded-3xl border-2 border-emerald-500/50 p-6 max-w-2xl w-full max-h-[90vh] overflow-y-auto">
        <div class="flex items-center justify-between mb-4">
          <h3 class="text-2xl font-bold text-emerald-300">🎮 Mini-Jeu</h3>
          <button
            @click="closeMiniGame"
            class="text-slate-400 hover:text-slate-200 text-2xl leading-none"
          >
            ×
          </button>
        </div>
        
        <!-- Composants de mini-jeux -->
        <MemoryGame v-if="activeMiniGame === 'memory'" @win="handleMiniGameWin" />
        <SequenceGame v-if="activeMiniGame === 'sequence'" @win="handleMiniGameWin" />
        <MatchGame v-if="activeMiniGame === 'match'" @win="handleMiniGameWin" />
        <CodeGame v-if="activeMiniGame === 'code'" @win="handleMiniGameWin" />
        <ReflexGame v-if="activeMiniGame === 'reflex'" @win="handleMiniGameWin" />
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { useAdventureStore } from '@/stores/adventure';
import { useUiStore } from '@/stores/ui';
import { AdventureGameStatus, PuzzleInfoDto } from '@/types';
import { useUserStore } from '@/stores/user';
import { adventureApi } from '@/services/api';
import MemoryGame from './adventure/MemoryGame.vue';
import SequenceGame from './adventure/SequenceGame.vue';
import MatchGame from './adventure/MatchGame.vue';
import CodeGame from './adventure/CodeGame.vue';
import ReflexGame from './adventure/ReflexGame.vue';

const adventureStore = useAdventureStore();
const uiStore = useUiStore();
const userStore = useUserStore();

const puzzleAnswers = ref<Record<number, string>>({});
const puzzleHints = ref<Record<number, string[]>>({});
const showHints = ref<number | null>(null);
const activeMiniGame = ref<string | null>(null);
const activePuzzleId = ref<number | null>(null);

const currentGame = computed(() => adventureStore.currentGame);
const playerGames = computed(() => adventureStore.playerGames);
const isLoading = computed(() => adventureStore.isLoading);
const error = computed(() => adventureStore.error);

// Configuration des salles (doit correspondre au backend)
const rooms = [
  { number: 1, name: "Entrée du Château", description: "Vous êtes devant l'entrée d'un mystérieux château. La porte est verrouillée. Des énigmes vous attendent...", emoji: "🏰" },
  { number: 2, name: "Hall Principal", description: "Un grand hall avec des portraits anciens. Une clé brille sur une table. Explorez avec prudence...", emoji: "🕋" },
  { number: 3, name: "Bibliothèque", description: "Des milliers de livres remplissent cette bibliothèque. Un livre particulier attire votre attention...", emoji: "📚" },
  { number: 4, name: "Laboratoire", description: "Un laboratoire rempli d'équipements étranges. Une formule est écrite au tableau...", emoji: "⚗️" },
  { number: 5, name: "Salle du Trésor", description: "Vous avez trouvé la salle du trésor ! Mais elle est protégée par des énigmes complexes...", emoji: "💎" },
  { number: 6, name: "Salle Secrète", description: "Une salle cachée derrière un mur de livres. Des énigmes complexes vous attendent...", emoji: "🔮" },
  { number: 7, name: "Tour du Mages", description: "Une tour mystérieuse remplie de magie. Les énigmes ici sont les plus difficiles...", emoji: "🗼" }
];

const puzzles = [
  { id: 1, question: "Mini-jeu : Mémoire", points: 15, room: 1, miniGameType: "memory" },
  { id: 2, question: "Quel est le résultat de 2 + 2 ?", points: 10, room: 1 },
  { id: 3, question: "Mini-jeu : Séquence", points: 20, room: 2, miniGameType: "sequence" },
  { id: 4, question: "Quelle est la capitale de la France ?", points: 15, room: 2 },
  { id: 5, question: "Mini-jeu : Puzzle", points: 25, room: 3, miniGameType: "puzzle" },
  { id: 6, question: "Combien de lettres dans le mot 'AVENTURE' ?", points: 20, room: 3 },
  { id: 7, question: "Mini-jeu : Correspondance", points: 30, room: 3, miniGameType: "match" },
  { id: 8, question: "Quel est le symbole chimique de l'eau ?", points: 25, room: 4 },
  { id: 9, question: "Mini-jeu : Logique", points: 35, room: 4, miniGameType: "logic" },
  { id: 10, question: "Quel est le nombre premier après 7 ?", points: 50, room: 5 },
  { id: 11, question: "Mini-jeu : Code", points: 60, room: 5, miniGameType: "code" },
  { id: 12, question: "Mini-jeu : Réflexe", points: 55, room: 5, miniGameType: "reflex" },
  { id: 13, question: "Mini-jeu : Mastermind", points: 70, room: 6, miniGameType: "mastermind" },
  { id: 14, question: "Quelle est la racine carrée de 144 ?", points: 40, room: 6 },
  { id: 15, question: "Mini-jeu : Final Boss", points: 100, room: 7, miniGameType: "boss" }
];

const items = [
  { name: "key", displayName: "Clé", description: "Une clé ancienne en or", emoji: "🗝️", room: 2 },
  { name: "map", displayName: "Carte", description: "Une carte du château", emoji: "🗺️", room: 3 },
  { name: "torch", displayName: "Torche", description: "Une torche pour éclairer", emoji: "🔥", room: 4 },
  { name: "book", displayName: "Livre", description: "Un livre mystérieux", emoji: "📖", room: 3 },
  { name: "crystal", displayName: "Cristal", description: "Un cristal magique brillant", emoji: "💎", room: 6 }
];

// Obtenir l'ID de session
const sessionId = computed(() => {
  if (userStore.user?.id) {
    return userStore.user.id;
  }
  let tempId = localStorage.getItem('tempSessionId');
  if (!tempId) {
    tempId = Date.now().toString();
    localStorage.setItem('tempSessionId', tempId);
  }
  return parseInt(tempId);
});

// Initialiser le store
onMounted(async () => {
  adventureStore.setSessionId(sessionId.value);
  await adventureStore.loadPlayerGames();
  
  // Charger les indices pour les énigmes disponibles
  if (currentGame.value) {
    await loadPuzzleHints();
  }
});

// Recharger les indices quand on change de salle
watch(
  () => currentGame.value?.currentRoom,
  async () => {
    if (currentGame.value) {
      await loadPuzzleHints();
    }
  }
);

// Charger les indices des énigmes
async function loadPuzzleHints() {
  if (!currentGame.value) return;
  
  const availablePuzzles = getAvailablePuzzles();
  for (const puzzle of availablePuzzles) {
    try {
      const puzzleInfo = await adventureApi.getPuzzleInfo(puzzle.id);
      puzzleHints.value[puzzle.id] = puzzleInfo.hints || [];
    } catch (err) {
      console.error(`Erreur lors du chargement des indices pour l'énigme ${puzzle.id}:`, err);
    }
  }
}

// Ouvrir un mini-jeu
async function openMiniGame(puzzleId: number, miniGameType: string) {
  activePuzzleId.value = puzzleId;
  activeMiniGame.value = miniGameType;
  
  // Charger les indices si pas déjà chargés
  if (!puzzleHints.value[puzzleId]) {
    try {
      const puzzleInfo = await adventureApi.getPuzzleInfo(puzzleId);
      puzzleHints.value[puzzleId] = puzzleInfo.hints || [];
    } catch (err) {
      console.error('Erreur lors du chargement des indices:', err);
    }
  }
}

// Fermer le mini-jeu
function closeMiniGame() {
  activeMiniGame.value = null;
  activePuzzleId.value = null;
}

// Gérer la victoire d'un mini-jeu
async function handleMiniGameWin() {
  if (activePuzzleId.value) {
    const puzzle = puzzles.find(p => p.id === activePuzzleId.value);
    if (puzzle && puzzle.miniGameType) {
      // Résoudre l'énigme avec la réponse du mini-jeu
      await handleSolvePuzzle(activePuzzleId.value, puzzle.miniGameType);
    }
  }
  closeMiniGame();
}

function goBack() {
  uiStore.goToGames();
}

async function handleStartAdventure() {
  await adventureStore.createGame();
}

async function loadGame(gameId: number) {
  await adventureStore.loadGame(gameId);
}

function getCurrentRoomInfo() {
  const room = rooms.find(r => r.number === currentGame.value?.currentRoom);
  return room || { name: "Salle inconnue", description: "..." };
}

function getAvailableItems() {
  if (!currentGame.value) return [];
  const roomItems = items.filter(item => item.room === currentGame.value!.currentRoom);
  return roomItems.filter(item => !currentGame.value!.collectedItems.includes(item.name));
}

function getAvailablePuzzles() {
  if (!currentGame.value) return [];
  return puzzles.filter(p => 
    p.room === currentGame.value!.currentRoom && 
    !currentGame.value!.solvedPuzzles.includes(p.id)
  );
}

function getAvailableRooms() {
  if (!currentGame.value) return rooms;
  
  // Filtrer les salles accessibles selon les objets collectés
  return rooms.filter(room => {
    // Salle 1 toujours accessible
    if (room.number === 1) return true;
    
    // Salle 3 nécessite la clé
    if (room.number === 3 && !currentGame.value!.collectedItems.includes("key")) return false;
    
    // Salle 5 nécessite la carte et la torche
    if (room.number === 5 && 
        (!currentGame.value!.collectedItems.includes("map") || 
         !currentGame.value!.collectedItems.includes("torch"))) return false;
    
    // Salle 6 nécessite le livre
    if (room.number === 6 && !currentGame.value!.collectedItems.includes("book")) return false;
    
    // Salle 7 nécessite le cristal
    if (room.number === 7 && !currentGame.value!.collectedItems.includes("crystal")) return false;
    
    return true;
  });
}

function getRoomEmoji(roomNumber: number): string {
  const room = rooms.find(r => r.number === roomNumber);
  return room?.emoji || "🚪";
}

function getItemEmoji(itemName: string): string {
  const item = items.find(i => i.name === itemName);
  return item?.emoji || "📦";
}

function getItemDisplayName(itemName: string): string {
  const item = items.find(i => i.name === itemName);
  return item?.displayName || itemName;
}

async function handleMoveToRoom(roomNumber: number) {
  try {
    await adventureStore.moveToRoom(roomNumber);
  } catch (err) {
    // L'erreur est déjà gérée par le store
  }
}

async function handleCollectItem(itemName: string) {
  try {
    await adventureStore.collectItem(itemName);
  } catch (err) {
    // L'erreur est déjà gérée par le store
  }
}

async function handleSolvePuzzle(puzzleId: number, answer: string) {
  if (!answer.trim()) return;
  
  try {
    await adventureStore.solvePuzzle(puzzleId, answer.trim());
    puzzleAnswers.value[puzzleId] = ''; // Réinitialiser le champ
  } catch (err) {
    // L'erreur est déjà gérée par le store
  }
}

async function handleAbandon() {
  if (currentGame.value) {
    await adventureStore.abandonGame();
  }
}

function handleNewGame() {
  adventureStore.resetGame();
}

function clearError() {
  adventureStore.clearError();
}
</script>

<style scoped>
@keyframes float {
  0%, 100% {
    transform: translateY(0px);
  }
  50% {
    transform: translateY(-10px);
  }
}

.animate-float {
  animation: float 3s ease-in-out infinite;
}
</style>

