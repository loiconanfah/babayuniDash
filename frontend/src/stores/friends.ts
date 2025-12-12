import { defineStore } from 'pinia';
import { ref } from 'vue';

export interface Friend {
  id: number;
  name: string;
  email: string;
  lastLoginAt: string | null;
  isOnline: boolean;
  friendsSince: string;
}

export interface FriendRequest {
  id: number;
  requesterId: number;
  requesterName: string;
  requesterEmail: string;
  receiverId: number;
  receiverName: string;
  receiverEmail: string;
  createdAt: string;
  status: string;
}

export const useFriendsStore = defineStore('friends', () => {
  const friends = ref<Friend[]>([]);
  const pendingRequests = ref<FriendRequest[]>([]);
  const sentRequests = ref<FriendRequest[]>([]);
  const isLoading = ref(false);

  async function fetchFriends(userId: number) {
    isLoading.value = true;
    try {
      const response = await fetch(`/api/friends/${userId}`);
      if (response.ok) {
        friends.value = await response.json();
      }
    } catch (error) {
      console.error('Erreur lors de la récupération des amis:', error);
    } finally {
      isLoading.value = false;
    }
  }

  async function fetchPendingRequests(userId: number) {
    try {
      const response = await fetch(`/api/friends/${userId}/requests/pending`);
      if (response.ok) {
        pendingRequests.value = await response.json();
      }
    } catch (error) {
      console.error('Erreur lors de la récupération des demandes:', error);
    }
  }

  async function fetchSentRequests(userId: number) {
    try {
      const response = await fetch(`/api/friends/${userId}/requests/sent`);
      if (response.ok) {
        sentRequests.value = await response.json();
      }
    } catch (error) {
      console.error('Erreur lors de la récupération des demandes envoyées:', error);
    }
  }

  async function sendFriendRequest(requesterId: number, receiverId: number) {
    try {
      const response = await fetch('/api/friends/request', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ requesterId, receiverId }),
      });
      if (response.ok) {
        return await response.json();
      }
      const errorData = await response.json().catch(() => ({ message: 'Erreur lors de l\'envoi de la demande' }));
      throw new Error(errorData.message || 'Erreur lors de l\'envoi de la demande');
    } catch (error) {
      console.error('Erreur lors de l\'envoi de la demande:', error);
      throw error;
    }
  }

  async function acceptFriendRequest(requestId: number, userId: number) {
    try {
      const response = await fetch(`/api/friends/request/${requestId}/accept`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ userId }),
      });
      if (response.ok) {
        pendingRequests.value = pendingRequests.value.filter(r => r.id !== requestId);
        return true;
      }
      return false;
    } catch (error) {
      console.error('Erreur lors de l\'acceptation:', error);
      return false;
    }
  }

  async function rejectFriendRequest(requestId: number, userId: number) {
    try {
      const response = await fetch(`/api/friends/request/${requestId}/reject`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ userId }),
      });
      if (response.ok) {
        pendingRequests.value = pendingRequests.value.filter(r => r.id !== requestId);
        return true;
      }
      return false;
    } catch (error) {
      console.error('Erreur lors du refus:', error);
      return false;
    }
  }

  return {
    friends,
    pendingRequests,
    sentRequests,
    isLoading,
    fetchFriends,
    fetchPendingRequests,
    fetchSentRequests,
    sendFriendRequest,
    acceptFriendRequest,
    rejectFriendRequest,
  };
});

