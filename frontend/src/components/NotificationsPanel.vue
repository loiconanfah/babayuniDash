<template>
  <div
    v-if="showPanel"
    class="fixed inset-0 z-50 flex items-end sm:items-center justify-center sm:p-4 pointer-events-none"
    @click.self="closePanel"
  >
    <!-- Overlay -->
    <div class="absolute inset-0 bg-black/60 backdrop-blur-sm pointer-events-auto" @click="closePanel"></div>

    <!-- Panneau de notifications -->
    <div
      class="relative w-full max-w-md h-[80vh] sm:h-[600px] bg-zinc-900 rounded-t-2xl sm:rounded-2xl shadow-2xl border border-zinc-800 flex flex-col pointer-events-auto overflow-hidden"
    >
      <!-- En-tête -->
      <div class="flex items-center justify-between p-4 border-b border-zinc-800 bg-zinc-900/95 backdrop-blur-sm">
        <div class="flex items-center gap-3">
          <div class="h-10 w-10 rounded-xl bg-gradient-to-br from-cyan-500 via-purple-500 to-pink-500 flex items-center justify-center">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-white" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
            </svg>
          </div>
          <div>
            <h2 class="text-lg font-bold text-white">Notifications</h2>
            <p v-if="unreadCount > 0" class="text-xs text-cyan-400">{{ unreadCount }} non lue(s)</p>
          </div>
        </div>
        <div class="flex items-center gap-2">
          <button
            v-if="unreadCount > 0"
            @click="handleMarkAllAsRead"
            class="px-3 py-1.5 text-xs font-medium text-cyan-400 hover:text-cyan-300 hover:bg-cyan-500/10 rounded-lg transition-colors"
          >
            Tout marquer comme lu
          </button>
          <button
            @click="closePanel"
            class="p-2 rounded-lg hover:bg-zinc-800 text-zinc-400 hover:text-white transition-colors"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M6 18L18 6M6 6l12 12" />
            </svg>
          </button>
        </div>
      </div>

      <!-- Liste des notifications -->
      <div class="flex-1 overflow-y-auto">
        <div v-if="isLoading" class="flex items-center justify-center py-12">
          <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-cyan-500"></div>
        </div>
        <div v-else-if="notifications.length === 0" class="flex flex-col items-center justify-center py-12 px-4">
          <div class="h-16 w-16 rounded-full bg-zinc-800 flex items-center justify-center mb-4">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8 text-zinc-600" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
            </svg>
          </div>
          <p class="text-zinc-400 text-center">Aucune notification</p>
        </div>
        <div v-else class="divide-y divide-zinc-800">
          <div
            v-for="notification in notifications"
            :key="notification.id"
            @click="handleNotificationClick(notification)"
            class="p-4 hover:bg-zinc-800/50 transition-colors cursor-pointer"
            :class="{ 'bg-cyan-500/10 border-l-4 border-cyan-500': !notification.read }"
          >
            <div class="flex items-start gap-3">
              <!-- Icône selon le type -->
              <div
                class="h-10 w-10 rounded-lg flex items-center justify-center flex-shrink-0"
                :class="getNotificationIconClass(notification.type)"
              >
                <span class="text-xl">{{ getNotificationIcon(notification.type) }}</span>
              </div>

              <!-- Contenu -->
              <div class="flex-1 min-w-0">
                <div class="flex items-start justify-between gap-2">
                  <div class="flex-1">
                    <h3 class="font-semibold text-white mb-1">{{ notification.title }}</h3>
                    <p class="text-sm text-zinc-400 line-clamp-2">{{ notification.message }}</p>
                    <p class="text-xs text-zinc-500 mt-2">{{ formatTime(notification.createdAt) }}</p>
                  </div>
                  <div class="flex items-center gap-2">
                    <button
                      v-if="!notification.read"
                      @click.stop="handleMarkAsRead(notification.id)"
                      class="h-2 w-2 rounded-full bg-cyan-500 flex-shrink-0"
                      title="Marquer comme lu"
                    ></button>
                    <button
                      @click.stop="handleDelete(notification.id)"
                      class="p-1 rounded hover:bg-zinc-700 text-zinc-500 hover:text-red-400 transition-colors"
                      title="Supprimer"
                    >
                      <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
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
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useNotificationsStore } from '@/stores/notifications'
import { NotificationType } from '@/services/notificationsApi'
import type { Notification } from '@/stores/notifications'

const notificationsStore = useNotificationsStore()

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
  if (!notification.read) {
    handleMarkAsRead(notification.id)
  }
  // TODO: Naviguer vers la page appropriée selon le type de notification
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
        return 'bg-purple-500/20 text-purple-400'
      case NotificationType.FriendMessage:
      case NotificationType.FriendRequestAccepted:
        return 'bg-blue-500/20 text-blue-400'
      case NotificationType.PostComment:
      case NotificationType.PostLike:
        return 'bg-pink-500/20 text-pink-400'
      case NotificationType.TournamentStarted:
      case NotificationType.TournamentEnded:
        return 'bg-yellow-500/20 text-yellow-400'
      case NotificationType.AchievementUnlocked:
        return 'bg-cyan-500/20 text-cyan-400'
      default:
        return 'bg-zinc-800 text-zinc-400'
    }
  }
  return 'bg-zinc-800 text-zinc-400'
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

