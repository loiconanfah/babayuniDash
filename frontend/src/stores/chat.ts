import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import { signalRService } from '@/services/signalrService';

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
  const isConnected = ref(false);
  const onlineUsers = ref<Set<number>>(new Set());

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
      // Envoyer via l'API REST (qui enverra aussi via SignalR)
      const response = await fetch('/api/chat/send', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ senderId, receiverId, content }),
      });
      
      if (response.ok) {
        const message = await response.json();
        // Le message sera ajouté automatiquement via SignalR
        // Mais on l'ajoute aussi localement pour une réponse immédiate
        if (currentUserId.value === senderId && currentOtherUserId.value === receiverId) {
          // Vérifier qu'il n'est pas déjà dans la liste (éviter les doublons)
          if (!currentConversation.value.find(m => m.id === message.id)) {
            currentConversation.value.push(message);
          }
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
      const message = currentConversation.value.find(m => m.id === messageId);
      const senderId = message?.senderId;
      
      // Marquer via l'API REST
      await fetch(`/api/chat/message/${messageId}/read`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ userId }),
      });

      // Notifier via SignalR aussi
      if (senderId && signalRService.isConnected()) {
        await signalRService.markMessageAsRead(messageId, userId, senderId);
      }

      // Mettre à jour localement
      const msg = currentConversation.value.find(m => m.id === messageId);
      if (msg) {
        msg.isRead = true;
        msg.readAt = new Date().toISOString();
      }
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

  /**
   * Initialise la connexion SignalR
   */
  async function initializeSignalR(userId: number) {
    if (isConnected.value) {
      return
    }

    try {
      await signalRService.startConnection(userId)
      isConnected.value = true

      // Écouter les nouveaux messages
      signalRService.onReceiveMessage((message: ChatMessage) => {
        // Si c'est la conversation actuelle, ajouter le message
        if (
          (currentUserId.value === message.receiverId && currentOtherUserId.value === message.senderId) ||
          (currentUserId.value === message.senderId && currentOtherUserId.value === message.receiverId)
        ) {
          // Vérifier qu'il n'est pas déjà dans la liste
          if (!currentConversation.value.find(m => m.id === message.id)) {
            currentConversation.value.push(message)
            // Marquer comme lu si c'est notre conversation
            if (currentUserId.value === message.receiverId) {
              markAsRead(message.id, currentUserId.value)
            }
          }
        }

        // Mettre à jour la conversation dans la liste
        updateConversationFromMessage(message)
        
        // Mettre à jour le compteur de non lus
        if (currentUserId.value === message.receiverId) {
          unreadCount.value++
        }
      })

      // Écouter les confirmations d'envoi
      signalRService.onMessageSent((message: ChatMessage) => {
        // Le message a été envoyé avec succès
        if (!currentConversation.value.find(m => m.id === message.id)) {
          currentConversation.value.push(message)
        }
      })

      // Écouter les notifications de message lu
      signalRService.onMessageRead((messageId: number, userId: number) => {
        const msg = currentConversation.value.find(m => m.id === messageId)
        if (msg && msg.senderId === currentUserId.value) {
          msg.isRead = true
          msg.readAt = new Date().toISOString()
        }
      })

      // Écouter les changements de statut en ligne
      signalRService.onUserOnline((userId: number) => {
        onlineUsers.value.add(userId)
        updateConversationOnlineStatus(userId, true)
      })

      signalRService.onUserOffline((userId: number) => {
        onlineUsers.value.delete(userId)
        updateConversationOnlineStatus(userId, false)
      })

      // Charger la liste des utilisateurs en ligne
      const online = await signalRService.getOnlineUsers()
      online.forEach(id => onlineUsers.value.add(id))
    } catch (error) {
      console.error('Erreur lors de l\'initialisation de SignalR:', error)
      isConnected.value = false
    }
  }

  /**
   * Déconnecte SignalR
   */
  async function disconnectSignalR() {
    if (isConnected.value) {
      signalRService.removeAllListeners()
      await signalRService.stopConnection()
      isConnected.value = false
      onlineUsers.value.clear()
    }
  }

  /**
   * Met à jour une conversation à partir d'un message
   */
  function updateConversationFromMessage(message: ChatMessage) {
    const otherUserId = currentUserId.value === message.senderId 
      ? message.receiverId 
      : message.senderId
    
    const conversation = conversations.value.find(c => c.otherUserId === otherUserId)
    if (conversation) {
      conversation.lastMessage = message.content
      conversation.lastMessageAt = message.sentAt
      if (currentUserId.value === message.receiverId) {
        conversation.unreadCount++
      }
    } else {
      // Créer une nouvelle conversation
      conversations.value.push({
        otherUserId,
        otherUserName: currentUserId.value === message.senderId 
          ? message.receiverName 
          : message.senderName,
        lastMessage: message.content,
        lastMessageAt: message.sentAt,
        unreadCount: currentUserId.value === message.receiverId ? 1 : 0,
        isOnline: onlineUsers.value.has(otherUserId)
      })
    }

    // Trier par date de dernier message
    conversations.value.sort((a, b) => {
      if (!a.lastMessageAt) return 1
      if (!b.lastMessageAt) return -1
      return new Date(b.lastMessageAt).getTime() - new Date(a.lastMessageAt).getTime()
    })
  }

  /**
   * Met à jour le statut en ligne d'une conversation
   */
  function updateConversationOnlineStatus(userId: number, isOnline: boolean) {
    const conversation = conversations.value.find(c => c.otherUserId === userId)
    if (conversation) {
      conversation.isOnline = isOnline
    }
  }

  return {
    conversations,
    currentConversation,
    currentUserId,
    currentOtherUserId,
    unreadCount,
    isLoading,
    isConnected,
    onlineUsers: computed(() => Array.from(onlineUsers.value)),
    fetchConversations,
    fetchConversation,
    sendMessage,
    markAsRead,
    fetchUnreadCount,
    initializeSignalR,
    disconnectSignalR,
  };
});



