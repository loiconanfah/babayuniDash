<template>
  <div class="friends-list-container">
    <div class="friends-header">
      <h3 class="text-lg font-bold text-zinc-50 mb-2">Mes Amis</h3>
      <button
        @click="showAddFriendModal = true"
        class="px-3 py-1.5 rounded-lg bg-gradient-to-r from-cyan-500 to-purple-500 text-white text-xs font-semibold hover:from-cyan-400 hover:to-purple-400 transition-all duration-200"
      >
        + Ajouter
      </button>
    </div>

    <!-- Demandes reçues en attente -->
    <div v-if="friendsStore.pendingRequests.length > 0" class="mb-4">
      <h4 class="text-sm font-semibold text-cyan-400 mb-2">Demandes reçues</h4>
      <div class="space-y-2">
        <div
          v-for="request in friendsStore.pendingRequests"
          :key="request.id"
          class="p-3 rounded-xl bg-zinc-800/60 border border-zinc-700/50 flex items-center justify-between"
        >
          <div class="flex items-center gap-3">
            <div class="h-10 w-10 rounded-full bg-gradient-to-br from-cyan-500 to-purple-500 flex items-center justify-center text-white font-bold text-sm">
              {{ request.requesterName.charAt(0).toUpperCase() }}
            </div>
            <div>
              <p class="text-sm font-medium text-zinc-50">{{ request.requesterName }}</p>
              <p class="text-xs text-zinc-400">Veut être ton ami</p>
            </div>
          </div>
          <div class="flex gap-2">
            <button
              @click="acceptRequest(request.id)"
              class="px-3 py-1.5 rounded-lg bg-emerald-500/20 text-emerald-400 text-xs font-semibold hover:bg-emerald-500/30 transition-colors"
            >
              Accepter
            </button>
            <button
              @click="rejectRequest(request.id)"
              class="px-3 py-1.5 rounded-lg bg-red-500/20 text-red-400 text-xs font-semibold hover:bg-red-500/30 transition-colors"
            >
              Refuser
            </button>
          </div>
        </div>
      </div>
    </div>

    <!-- Demandes envoyées en attente -->
    <div v-if="friendsStore.sentRequests.length > 0" class="mb-4">
      <h4 class="text-sm font-semibold text-purple-400 mb-2">Demandes envoyées</h4>
      <div class="space-y-2">
        <div
          v-for="request in friendsStore.sentRequests"
          :key="request.id"
          class="p-3 rounded-xl bg-zinc-800/60 border border-zinc-700/50 flex items-center justify-between"
        >
          <div class="flex items-center gap-3">
            <div class="h-10 w-10 rounded-full bg-gradient-to-br from-purple-500 to-pink-500 flex items-center justify-center text-white font-bold text-sm">
              {{ request.receiverName.charAt(0).toUpperCase() }}
            </div>
            <div>
              <p class="text-sm font-medium text-zinc-50">{{ request.receiverName }}</p>
              <p class="text-xs text-zinc-400">En attente de réponse</p>
            </div>
          </div>
          <span class="px-3 py-1.5 rounded-lg bg-yellow-500/20 text-yellow-400 text-xs font-semibold">
            En attente
          </span>
        </div>
      </div>
    </div>

    <!-- Liste des amis -->
    <div v-if="friendsStore.friends.length > 0" class="space-y-2 max-h-96 overflow-y-auto">
      <div
        v-for="friend in friendsStore.friends"
        :key="friend.id"
        @click="openChat(friend.id)"
        class="p-3 rounded-xl bg-zinc-800/60 border border-zinc-700/50 hover:border-cyan-500/50 cursor-pointer transition-all duration-200 flex items-center gap-3 group"
      >
        <div class="relative">
          <div class="h-10 w-10 rounded-full bg-gradient-to-br from-cyan-500 to-purple-500 flex items-center justify-center text-white font-bold text-sm">
            {{ friend.name.charAt(0).toUpperCase() }}
          </div>
          <div
            v-if="friend.isOnline"
            class="absolute -bottom-0.5 -right-0.5 h-3 w-3 rounded-full bg-emerald-500 border-2 border-zinc-900"
          ></div>
        </div>
        <div class="flex-1 min-w-0">
          <p class="text-sm font-medium text-zinc-50 truncate">{{ friend.name }}</p>
          <p class="text-xs text-zinc-400">
            {{ friend.isOnline ? 'En ligne' : 'Hors ligne' }}
          </p>
        </div>
        <svg
          xmlns="http://www.w3.org/2000/svg"
          class="h-5 w-5 text-zinc-400 group-hover:text-cyan-400 transition-colors"
          fill="none"
          viewBox="0 0 24 24"
          stroke="currentColor"
          stroke-width="2"
        >
          <path stroke-linecap="round" stroke-linejoin="round" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
        </svg>
      </div>
    </div>

    <div v-else class="text-center py-8 text-zinc-400 text-sm">
      <p>Aucun ami pour le moment</p>
      <p class="text-xs mt-1">Ajoute des amis pour commencer à jouer ensemble !</p>
    </div>

    <!-- Modal pour ajouter un ami -->
    <div
      v-if="showAddFriendModal"
      class="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 flex items-center justify-center p-4"
      @click.self="showAddFriendModal = false"
    >
      <div class="bg-zinc-900 rounded-2xl p-6 max-w-md w-full border border-zinc-700/50">
        <h3 class="text-xl font-bold text-zinc-50 mb-4">Ajouter un ami</h3>
        <div class="space-y-4">
          <div class="relative">
            <label class="block text-sm font-medium text-zinc-300 mb-2">Email de l'utilisateur</label>
            <input
              v-model="friendEmail"
              @input="searchUsers"
              type="email"
              placeholder="email@example.com"
              class="w-full px-4 py-2.5 rounded-xl bg-zinc-800 border border-zinc-700 text-zinc-50 placeholder-zinc-500 focus:outline-none focus:ring-2 focus:ring-cyan-500/50"
            />
            <!-- Suggestions -->
            <div
              v-if="userSuggestions.length > 0 && friendEmail.length > 0"
              class="absolute z-50 w-full mt-2 bg-zinc-800 border border-zinc-700 rounded-xl shadow-2xl max-h-60 overflow-y-auto"
            >
              <div
                v-for="user in userSuggestions"
                :key="user.id"
                @click="selectUser(user)"
                class="px-4 py-3 hover:bg-zinc-700 cursor-pointer transition-colors flex items-center gap-3"
              >
                <div class="h-8 w-8 rounded-full bg-gradient-to-br from-cyan-500 to-purple-500 flex items-center justify-center text-white font-bold text-sm">
                  {{ user.name.charAt(0).toUpperCase() }}
                </div>
                <div class="flex-1 min-w-0">
                  <p class="text-sm font-medium text-zinc-50 truncate">{{ user.name }}</p>
                  <p class="text-xs text-zinc-400 truncate">{{ user.email }}</p>
                </div>
              </div>
            </div>
          </div>
          <div class="flex gap-3">
            <button
              @click="sendFriendRequest"
              class="flex-1 px-4 py-2.5 rounded-xl bg-gradient-to-r from-cyan-500 to-purple-500 text-white font-semibold hover:from-cyan-400 hover:to-purple-400 transition-all duration-200"
            >
              Envoyer
            </button>
            <button
              @click="showAddFriendModal = false"
              class="px-4 py-2.5 rounded-xl bg-zinc-800 text-zinc-300 font-semibold hover:bg-zinc-700 transition-colors"
            >
              Annuler
            </button>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { useFriendsStore } from '@/stores/friends';
import { useUserStore } from '@/stores/user';
import { useChatStore } from '@/stores/chat';
import { useUiStore } from '@/stores/ui';

const friendsStore = useFriendsStore();
const userStore = useUserStore();
const chatStore = useChatStore();
const uiStore = useUiStore();

const showAddFriendModal = ref(false);
const friendEmail = ref('');
const userSuggestions = ref<Array<{ id: number; name: string; email: string }>>([]);
const searchTimeout = ref<number | null>(null);

onMounted(async () => {
  if (userStore.user) {
    await friendsStore.fetchFriends(userStore.user.id);
    await friendsStore.fetchPendingRequests(userStore.user.id);
    await friendsStore.fetchSentRequests(userStore.user.id);
  }
});

async function acceptRequest(requestId: number) {
  if (userStore.user) {
    await friendsStore.acceptFriendRequest(requestId, userStore.user.id);
    await friendsStore.fetchFriends(userStore.user.id);
  }
}

async function rejectRequest(requestId: number) {
  if (userStore.user) {
    await friendsStore.rejectFriendRequest(requestId, userStore.user.id);
  }
}

async function searchUsers() {
  if (searchTimeout.value) {
    clearTimeout(searchTimeout.value);
  }

  if (!friendEmail.value || friendEmail.value.length < 2) {
    userSuggestions.value = [];
    return;
  }

  searchTimeout.value = window.setTimeout(async () => {
    try {
      const response = await fetch(`/api/users/search?email=${encodeURIComponent(friendEmail.value)}&limit=5`);
      if (response.ok) {
        const users = await response.json();
        // Filtrer l'utilisateur actuel
        userSuggestions.value = users.filter((u: any) => u.id !== userStore.user?.id);
      }
    } catch (error) {
      console.error('Erreur lors de la recherche:', error);
      userSuggestions.value = [];
    }
  }, 300);
}

function selectUser(user: { id: number; name: string; email: string }) {
  friendEmail.value = user.email;
  userSuggestions.value = [];
}

async function sendFriendRequest() {
  if (!userStore.user || !friendEmail.value) return;

  try {
    // Chercher l'utilisateur par email
    const response = await fetch(`/api/users/search?email=${encodeURIComponent(friendEmail.value)}&limit=1`);
    if (!response.ok) {
      throw new Error('Erreur lors de la recherche');
    }

    const users = await response.json();
    if (users.length === 0) {
      alert('Aucun utilisateur trouvé avec cet email');
      return;
    }

    const receiverId = users[0].id;
    if (receiverId === userStore.user.id) {
      alert('Tu ne peux pas t\'envoyer une demande d\'amitié à toi-même');
      return;
    }
    
    await friendsStore.sendFriendRequest(userStore.user.id, receiverId);
    showAddFriendModal.value = false;
    friendEmail.value = '';
    userSuggestions.value = [];
    await friendsStore.fetchPendingRequests(userStore.user.id);
    await friendsStore.fetchSentRequests(userStore.user.id);
  } catch (error) {
    console.error('Erreur lors de l\'envoi de la demande:', error);
    const errorMessage = error instanceof Error ? error.message : 'Erreur lors de l\'envoi de la demande d\'amitié';
    alert(errorMessage);
  }
}

function openChat(friendId: number) {
  if (userStore.user) {
    chatStore.fetchConversation(userStore.user.id, friendId);
    uiStore.openChat();
  }
}
</script>

<style scoped>
.friends-list-container {
  @apply w-full;
}
</style>

