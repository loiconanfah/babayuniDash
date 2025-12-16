<template>
  <div
    v-if="showPanel"
    class="absolute right-0 mt-2 w-[calc(100vw-4rem)] sm:w-96 max-w-[calc(100vw-4rem)] sm:max-w-none bg-gradient-to-br from-zinc-800 to-zinc-900 rounded-2xl shadow-2xl border border-zinc-700/50 overflow-hidden z-[70] max-h-[calc(100vh-6rem)] flex flex-col backdrop-blur-xl animate-fadeIn"
    @click.stop
  >
    <!-- En-tête -->
    <div class="px-5 py-4 border-b border-zinc-700/50 bg-gradient-to-r from-cyan-500/10 to-purple-500/10">
      <div class="flex items-center justify-between">
        <div class="flex items-center gap-3">
          <div class="h-10 w-10 rounded-xl bg-gradient-to-br from-cyan-500 via-purple-500 to-pink-500 flex items-center justify-center shadow-lg shadow-purple-500/30">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
            </svg>
          </div>
          <div>
            <h3 class="text-sm font-bold text-zinc-50">Notifications</h3>
            <p v-if="unreadCount > 0" class="text-xs text-cyan-400 mt-0.5">
              {{ unreadCount }} non lue{{ unreadCount > 1 ? 's' : '' }}
            </p>
            <p v-else class="text-xs text-zinc-400 mt-0.5">Tout est à jour</p>
          </div>
        </div>
        <div class="flex items-center gap-2">
          <button
            v-if="unreadCount > 0"
            @click="handleMarkAllAsRead"
            class="px-3 py-1.5 text-xs font-medium text-cyan-400 hover:text-cyan-300 hover:bg-cyan-500/10 rounded-lg transition-all duration-200"
          >
            Tout marquer
          </button>
          <button
            @click="closePanel"
            class="p-1.5 rounded-lg hover:bg-zinc-700/50 text-zinc-400 hover:text-white transition-all duration-200"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
      </div>
    </div>

    <!-- Liste des notifications -->
    <div class="flex-1 overflow-y-auto">
      <div v-if="isLoading" class="flex items-center justify-center py-12">
        <div class="animate-spin rounded-full h-8 w-8 border-2 border-cyan-500 border-t-transparent"></div>
      </div>
      <div v-else-if="notifications.length === 0" class="flex flex-col items-center justify-center py-12 px-4">
        <div class="h-16 w-16 rounded-xl bg-gradient-to-br from-cyan-500/20 via-purple-500/20 to-pink-500/20 flex items-center justify-center mb-3 shadow-lg">
          <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8 text-zinc-500" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
          </svg>
        </div>
        <p class="text-zinc-400 text-center text-sm font-medium">Aucune notification</p>
        <p class="text-xs text-zinc-500 mt-1 text-center">Vous serez notifié des nouveaux événements</p>
      </div>
      <div v-else class="divide-y divide-zinc-700/50">
        <div
          v-for="notification in notifications"
          :key="notification.id"
          @click="handleNotificationClick(notification)"
          class="p-4 hover:bg-zinc-800/60 transition-all duration-200 cursor-pointer group"
          :class="{ 
            'bg-cyan-500/10 border-l-4 border-cyan-500': !notification.read && !notification.isRead,
            'bg-zinc-800/30': notification.read || notification.isRead
          }"
        >
          <div class="flex items-start gap-3">
            <!-- Icône selon le type -->
            <div
              class="h-10 w-10 rounded-lg flex items-center justify-center flex-shrink-0 shadow-md transition-transform duration-200 group-hover:scale-110"
              :class="getNotificationIconClass(notification.type)"
            >
              <span class="text-xl">{{ getNotificationIcon(notification.type) }}</span>
            </div>

            <!-- Contenu -->
            <div class="flex-1 min-w-0">
              <div class="flex items-start justify-between gap-2">
                <div class="flex-1">
                  <h3 class="font-semibold text-zinc-50 mb-1 text-sm">{{ notification.title }}</h3>
                  <p class="text-xs text-zinc-300 line-clamp-2 leading-relaxed mb-1">{{ notification.message }}</p>
                  <p class="text-[10px] text-zinc-500">{{ formatTime(notification.createdAt) }}</p>
                </div>
                <div class="flex items-center gap-1.5 flex-shrink-0">
                  <button
                    v-if="!notification.read && !notification.isRead"
                    @click.stop="handleMarkAsRead(notification.id)"
                    class="h-2 w-2 rounded-full bg-cyan-500 flex-shrink-0 shadow-lg shadow-cyan-500/50 animate-pulse"
                    title="Marquer comme lu"
                  ></button>
                  <button
                    @click.stop="handleDelete(notification.id)"
                    class="p-1 rounded hover:bg-red-500/20 text-zinc-400 hover:text-red-400 transition-all duration-200"
                    title="Supprimer"
                  >
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-3.5 w-3.5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
                      <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
                    </svg>
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useNotificationsStore } from '@/stores/notifications'
import { useUiStore } from '@/stores/ui'
import { NotificationType } from '@/services/notificationsApi'
import type { Notification } from '@/stores/notifications'

const notificationsStore = useNotificationsStore()
const uiStore = useUiStore()

const showPanel = computed(() => notificationsStore.showNotificationsPanel)
const notifications = computed(() => notificationsStore.notifications)
const unreadCount = computed(() => notificationsStore.unreadCount)
const isLoading = computed(() => notificationsStore.isLoading)

function closePanel() {
  notificationsStore.closeNotificationsPanel()
}

function handleMarkAsRead(notificationId: string | number) {
  notificationsStore.markAsRead(notificationId)
}

async function handleMarkAllAsRead() {
  await notificationsStore.markAllAsRead()
}

function handleDelete(notificationId: string | number) {
  notificationsStore.removeNotification(notificationId)
}

function handleNotificationClick(notification: Notification) {
  const isRead = notification.read || (notification as any).isRead;
  if (!isRead) {
    handleMarkAsRead(notification.id)
  }
  
  // Naviguer vers la page appropriée selon le type de notification
  if (typeof notification.type === 'number') {
    if (notification.type === NotificationType.GameInvitation && notification.dataJson) {
      try {
        const data = JSON.parse(notification.dataJson);
        if (data.gameType) {
          switch (data.gameType) {
            case 'TicTacToe':
              uiStore.goToTicTacToe();
              break;
            case 'ConnectFour':
              uiStore.goToConnectFour();
              break;
            case 'RockPaperScissors':
              uiStore.goToRockPaperScissors();
              break;
          }
        }
      } catch (e) {
        console.error('Erreur lors du parsing de la notification:', e);
      }
    } else if (notification.type === NotificationType.FriendRequestAccepted) {
      uiStore.goToProfile();
    } else if (notification.type === NotificationType.PostComment || 
               notification.type === NotificationType.PostLike) {
      uiStore.goToCommunity();
    } else if (notification.type === NotificationType.TournamentStarted || 
               notification.type === NotificationType.TournamentEnded) {
      uiStore.goToTournaments();
    }
  }
  
  notificationsStore.closeNotificationsPanel();
}

function getNotificationIcon(type: string | number): string {
  if (typeof type === 'number') {
    switch (type) {
      case NotificationType.GameInvitation:
        return '🎮'
      case NotificationType.GameStarted:
        return '▶️'
      case NotificationType.GameEnded:
        return '🏁'
      case NotificationType.FriendMessage:
        return '💬'
      case NotificationType.FriendRequestAccepted:
        return '✅'
      case NotificationType.PostComment:
        return '💭'
      case NotificationType.PostLike:
        return '❤️'
      case NotificationType.TournamentStarted:
        return '🏆'
      case NotificationType.TournamentEnded:
        return '🎉'
      case NotificationType.AchievementUnlocked:
        return '🏅'
      default:
        return '🔔'
    }
  }
  return '🔔'
}

function getNotificationIconClass(type: string | number): string {
  if (typeof type === 'number') {
    switch (type) {
      case NotificationType.GameInvitation:
      case NotificationType.GameStarted:
      case NotificationType.GameEnded:
        return 'bg-gradient-to-br from-purple-500 to-purple-600 text-white shadow-lg shadow-purple-500/40'
      case NotificationType.FriendMessage:
      case NotificationType.FriendRequestAccepted:
        return 'bg-gradient-to-br from-blue-500 to-blue-600 text-white shadow-lg shadow-blue-500/40'
      case NotificationType.PostComment:
      case NotificationType.PostLike:
        return 'bg-gradient-to-br from-pink-500 to-pink-600 text-white shadow-lg shadow-pink-500/40'
      case NotificationType.TournamentStarted:
      case NotificationType.TournamentEnded:
        return 'bg-gradient-to-br from-yellow-500 to-yellow-600 text-white shadow-lg shadow-yellow-500/40'
      case NotificationType.AchievementUnlocked:
        return 'bg-gradient-to-br from-cyan-500 to-cyan-600 text-white shadow-lg shadow-cyan-500/40'
      default:
        return 'bg-gradient-to-br from-zinc-500 to-zinc-600 text-white shadow-lg'
    }
  }
  return 'bg-gradient-to-br from-zinc-500 to-zinc-600 text-white shadow-lg'
}

function formatTime(date: Date | string): string {
  const d = typeof date === 'string' ? new Date(date) : date
  const now = new Date()
  const diff = now.getTime() - d.getTime()
  const seconds = Math.floor(diff / 1000)
  const minutes = Math.floor(seconds / 60)
  const hours = Math.floor(minutes / 60)
  const days = Math.floor(hours / 24)

  if (seconds < 60) return 'À l\'instant'
  if (minutes < 60) return `Il y a ${minutes} min`
  if (hours < 24) return `Il y a ${hours} h`
  if (days < 7) return `Il y a ${days} j`
  
  return d.toLocaleDateString('fr-FR', {
    day: 'numeric',
    month: 'short',
    hour: '2-digit',
    minute: '2-digit'
  })
}
</script>

<style scoped>
.line-clamp-2 {
  display: -webkit-box;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
}
</style>
