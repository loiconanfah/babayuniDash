import { defineStore } from 'pinia';
import { ref } from 'vue';

export interface ChatMessage {
  id: number;
  senderId: number;
  senderName: string;
  receiverId: number;
  receiverName: string;
  content: string;
  sentAt: string;
  isRead: boolean;
  readAt: string | null;
}

export interface ChatConversation {
  otherUserId: number;
  otherUserName: string;
  lastMessage: string | null;
  lastMessageAt: string | null;
  unreadCount: number;
  isOnline: boolean;
}

export const useChatStore = defineStore('chat', () => {
  const conversations = ref<ChatConversation[]>([]);
  const currentConversation = ref<ChatMessage[]>([]);
  const currentUserId = ref<number | null>(null);
  const currentOtherUserId = ref<number | null>(null);
  const unreadCount = ref(0);
  const isLoading = ref(false);

  async function fetchConversations(userId: number) {
    isLoading.value = true;
    try {
      const response = await fetch(`/api/chat/conversations/${userId}`);
      if (response.ok) {
        conversations.value = await response.json();
      }
    } catch (error) {
      console.error('Erreur lors de la récupération des conversations:', error);
    } finally {
      isLoading.value = false;
    }
  }

  async function fetchConversation(userId1: number, userId2: number) {
    isLoading.value = true;
    try {
      const response = await fetch(`/api/chat/conversation/${userId1}/${userId2}`);
      if (response.ok) {
        currentConversation.value = await response.json();
        currentUserId.value = userId1;
        currentOtherUserId.value = userId2;
      }
    } catch (error) {
      console.error('Erreur lors de la récupération de la conversation:', error);
    } finally {
      isLoading.value = false;
    }
  }

  async function sendMessage(senderId: number, receiverId: number, content: string) {
    try {
      const response = await fetch('/api/chat/send', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ senderId, receiverId, content }),
      });
      if (response.ok) {
        const message = await response.json();
        if (currentUserId.value === senderId && currentOtherUserId.value === receiverId) {
          currentConversation.value.push(message);
        }
        return message;
      }
      throw new Error('Erreur lors de l\'envoi du message');
    } catch (error) {
      console.error('Erreur lors de l\'envoi du message:', error);
      throw error;
    }
  }

  async function markAsRead(messageId: number, userId: number) {
    try {
      await fetch(`/api/chat/message/${messageId}/read`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ userId }),
      });
    } catch (error) {
      console.error('Erreur lors du marquage comme lu:', error);
    }
  }

  async function fetchUnreadCount(userId: number) {
    try {
      const response = await fetch(`/api/chat/unread/${userId}`);
      if (response.ok) {
        const data = await response.json();
        unreadCount.value = data.count;
      }
    } catch (error) {
      console.error('Erreur lors de la récupération du nombre de messages non lus:', error);
    }
  }

  return {
    conversations,
    currentConversation,
    currentUserId,
    currentOtherUserId,
    unreadCount,
    isLoading,
    fetchConversations,
    fetchConversation,
    sendMessage,
    markAsRead,
    fetchUnreadCount,
  };
});

