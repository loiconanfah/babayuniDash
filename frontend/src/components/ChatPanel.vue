<template>
  <div
    v-if="isOpen"
    class="fixed inset-y-0 right-0 w-full sm:w-96 bg-zinc-900 border-l border-zinc-800 z-50 flex flex-col shadow-2xl"
  >
    <!-- Header -->
    <div class="h-16 border-b border-zinc-800 px-4 flex items-center justify-between bg-gradient-to-r from-zinc-900 to-zinc-800">
      <div class="flex items-center gap-3">
        <div
          v-if="currentOtherUser"
          class="h-10 w-10 rounded-full bg-gradient-to-br from-cyan-500 to-purple-500 flex items-center justify-center text-white font-bold text-sm"
        >
          {{ currentOtherUser?.name?.charAt(0).toUpperCase() || '?' }}
        </div>
        <div>
          <h3 class="text-sm font-bold text-zinc-50">
            {{ currentOtherUser?.name || 'Chat' }}
          </h3>
          <div class="flex items-center gap-2">
            <div 
              :class="[
                'h-2 w-2 rounded-full',
                currentOtherUser?.isOnline ? 'bg-green-500 animate-pulse' : 'bg-zinc-500'
              ]"
            ></div>
            <p class="text-xs text-zinc-400">
              {{ currentOtherUser?.isOnline ? 'En ligne' : 'Hors ligne' }}
            </p>
          </div>
        </div>
      </div>
      <button
        @click="closeChat"
        class="p-2 rounded-lg hover:bg-zinc-800 transition-colors"
      >
        <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-zinc-400" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
          <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
        </svg>
      </button>
    </div>

    <!-- Messages -->
    <div ref="messagesContainer" class="flex-1 overflow-y-auto p-4 space-y-4 bg-zinc-950">
      <div v-if="chatStore.currentConversation.length === 0" class="flex items-center justify-center h-full">
        <div class="text-center">
          <div class="text-4xl mb-4 opacity-50">💬</div>
          <p class="text-zinc-400 text-sm">Aucun message</p>
          <p class="text-zinc-500 text-xs mt-1">Commencez la conversation !</p>
        </div>
      </div>
      <div
        v-for="message in chatStore.currentConversation"
        :key="message.id"
        :class="[
          'flex',
          message.senderId === userStore.user?.id ? 'justify-end' : 'justify-start'
        ]"
      >
        <div
          :class="[
            'max-w-[80%] rounded-2xl px-4 py-2 transition-all duration-200',
            message.senderId === userStore.user?.id
              ? 'bg-gradient-to-r from-cyan-500 to-purple-500 text-white shadow-lg'
              : 'bg-zinc-800 text-zinc-50 shadow-md'
          ]"
        >
          <p class="text-sm break-words">{{ message.content }}</p>
          <div class="flex items-center gap-2 mt-1">
            <p
              :class="[
                'text-xs',
                message.senderId === userStore.user?.id ? 'text-white/70' : 'text-zinc-400'
              ]"
            >
              {{ formatTime(message.sentAt) }}
            </p>
            <svg
              v-if="message.senderId === userStore.user?.id"
              xmlns="http://www.w3.org/2000/svg"
              class="h-3 w-3"
              :class="message.isRead ? 'text-blue-300' : 'text-white/50'"
              fill="currentColor"
              viewBox="0 0 20 20"
            >
              <path d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z" />
            </svg>
          </div>
        </div>
      </div>
    </div>

    <!-- Input -->
    <div class="border-t border-zinc-800 p-4 bg-zinc-900">
      <div class="flex gap-2">
        <input
          v-model="messageContent"
          @keyup.enter="sendMessage"
          type="text"
          placeholder="Tape ton message..."
          class="flex-1 px-4 py-2.5 rounded-xl bg-zinc-800 border border-zinc-700 text-zinc-50 placeholder-zinc-500 focus:outline-none focus:ring-2 focus:ring-cyan-500/50"
        />
        <button
          @click="sendMessage"
          :disabled="!messageContent.trim()"
          class="px-4 py-2.5 rounded-xl bg-gradient-to-r from-cyan-500 to-purple-500 text-white font-semibold hover:from-cyan-400 hover:to-purple-400 transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round" d="M12 19l9 2-9-18-9 18 9-2zm0 0v-8" />
          </svg>
        </button>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { ref, computed, watch, onMounted } from 'vue';
import { useChatStore } from '@/stores/chat';
import { useUserStore } from '@/stores/user';
import { useUiStore } from '@/stores/ui';

const chatStore = useChatStore();
const userStore = useUserStore();
const uiStore = useUiStore();

const messageContent = ref('');
const isOpen = computed(() => uiStore.isChatOpen);
const messagesContainer = ref<HTMLElement | null>(null);

const currentOtherUser = computed(() => {
  if (!chatStore.currentOtherUserId || !chatStore.conversations.length) return null;
  return chatStore.conversations.find(c => c.otherUserId === chatStore.currentOtherUserId);
});

// Scroll automatique vers le bas quand de nouveaux messages arrivent
watch(() => chatStore.currentConversation, () => {
  scrollToBottom();
}, { deep: true });

// Scroll quand le chat s'ouvre
watch(isOpen, async (newValue) => {
  if (newValue) {
    setTimeout(() => scrollToBottom(), 100);
    // Initialiser SignalR si pas déjà connecté
    if (userStore.user?.id && !chatStore.isConnected) {
      await chatStore.initializeSignalR(userStore.user.id);
    }
  }
});

// Charger la conversation quand on change d'utilisateur
watch(() => chatStore.currentOtherUserId, async (newUserId) => {
  if (newUserId && userStore.user?.id) {
    await chatStore.fetchConversation(userStore.user.id, newUserId);
    scrollToBottom();
  }
});

function scrollToBottom() {
  setTimeout(() => {
    if (messagesContainer.value) {
      messagesContainer.value.scrollTop = messagesContainer.value.scrollHeight;
    }
  }, 100);
}

function formatTime(dateString: string) {
  const date = new Date(dateString);
  const now = new Date();
  const diff = now.getTime() - date.getTime();
  const minutes = Math.floor(diff / 60000);
  
  if (minutes < 1) return 'À l\'instant';
  if (minutes < 60) return `Il y a ${minutes} min`;
  if (minutes < 1440) return `Il y a ${Math.floor(minutes / 60)} h`;
  return date.toLocaleDateString('fr-FR');
}

async function sendMessage() {
  if (!messageContent.value.trim() || !userStore.user || !chatStore.currentOtherUserId) return;

  const content = messageContent.value.trim();
  messageContent.value = ''; // Vider immédiatement pour une meilleure UX

  try {
    await chatStore.sendMessage(
      userStore.user.id,
      chatStore.currentOtherUserId,
      content
    );
    // Le message sera ajouté automatiquement via SignalR
    scrollToBottom();
  } catch (error) {
    console.error('Erreur lors de l\'envoi du message:', error);
    // Remettre le message en cas d'erreur
    messageContent.value = content;
  }
}

function closeChat() {
  uiStore.closeChat();
}
</script>

