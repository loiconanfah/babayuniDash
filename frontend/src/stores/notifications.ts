import { defineStore } from 'pinia'
import { ref, computed } from 'vue'
import type { TicTacToeGame, ConnectFourGame, RockPaperScissorsGame } from '@/types'

export type NotificationType = 'invitation' | 'game_started' | 'game_ended' | 'message'

export interface GameInvitation {
  id: number
  gameType: 'TicTacToe' | 'ConnectFour' | 'RockPaperScissors'
  gameId: number
  fromPlayerName: string
  fromPlayerId: number
  wager: number
  game: TicTacToeGame | ConnectFourGame | RockPaperScissorsGame
  createdAt: Date
}

export interface Notification {
  id: string
  type: NotificationType
  title: string
  message: string
  invitation?: GameInvitation
  read: boolean
  createdAt: Date
}

export const useNotificationsStore = defineStore('notifications', () => {
  const notifications = ref<Notification[]>([])
  const showNotificationsPanel = ref(false)

  const unreadCount = computed(() => {
    return notifications.value.filter(n => !n.read).length
  })

  const unreadInvitations = computed(() => {
    return notifications.value.filter(
      n => n.type === 'invitation' && !n.read && n.invitation
    ) as Array<Notification & { invitation: GameInvitation }>
  })

  function addNotification(notification: Omit<Notification, 'id' | 'read' | 'createdAt'>) {
    const newNotification: Notification = {
      ...notification,
      id: `notif-${Date.now()}-${Math.random()}`,
      read: false,
      createdAt: new Date()
    }
    notifications.value.unshift(newNotification)
  }

  function addInvitation(invitation: GameInvitation) {
    const gameTypeNames = {
      TicTacToe: 'Morpion',
      ConnectFour: 'Puissance 4',
      RockPaperScissors: 'Pierre-Papier-Ciseaux'
    }

    addNotification({
      type: 'invitation',
      title: `🎮 Invitation à jouer`,
      message: `${invitation.fromPlayerName} vous invite à jouer à ${gameTypeNames[invitation.gameType]}${invitation.wager > 0 ? ` (mise: ${invitation.wager} coins)` : ''}`,
      invitation
    })
  }

  function markAsRead(notificationId: string) {
    const notification = notifications.value.find(n => n.id === notificationId)
    if (notification) {
      notification.read = true
    }
  }

  function markAllAsRead() {
    notifications.value.forEach(n => (n.read = true))
  }

  function removeNotification(notificationId: string) {
    const index = notifications.value.findIndex(n => n.id === notificationId)
    if (index !== -1) {
      notifications.value.splice(index, 1)
    }
  }

  function removeInvitation(gameId: number, gameType: string) {
    notifications.value = notifications.value.filter(
      n => !(n.invitation && n.invitation.gameId === gameId && n.invitation.gameType === gameType)
    )
  }

  function toggleNotificationsPanel() {
    showNotificationsPanel.value = !showNotificationsPanel.value
  }

  function closeNotificationsPanel() {
    showNotificationsPanel.value = false
  }

  return {
    notifications,
    showNotificationsPanel,
    unreadCount,
    unreadInvitations,
    addNotification,
    addInvitation,
    markAsRead,
    markAllAsRead,
    removeNotification,
    removeInvitation,
    toggleNotificationsPanel,
    closeNotificationsPanel
  }
})

