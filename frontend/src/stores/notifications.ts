import { defineStore } from 'pinia'
import { ref, computed, onMounted } from 'vue'
import * as notificationsApi from '@/services/notificationsApi'
import type { NotificationDto, NotificationType as ApiNotificationType } from '@/services/notificationsApi'
import type { TicTacToeGame, ConnectFourGame, RockPaperScissorsGame } from '@/types'
import { useUserStore } from './user'

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
  id: string | number
  type: NotificationType | ApiNotificationType
  title: string
  message: string
  invitation?: GameInvitation
  read: boolean
  isRead?: boolean // Pour compatibilité avec l'API (NotificationDto utilise isRead)
  createdAt: Date | string
  dataJson?: string | null
}

export const useNotificationsStore = defineStore('notifications', () => {
  const userStore = useUserStore()
  const notifications = ref<Notification[]>([])
  const showNotificationsPanel = ref(false)
  const isLoading = ref(false)
  const lastFetchTime = ref<Date | null>(null)

  const unreadCount = computed(() => {
    return notifications.value.filter(n => !(n.read || n.isRead)).length
  })

  const unreadInvitations = computed(() => {
    return notifications.value.filter(
      n => (n.type === 'invitation' || n.type === notificationsApi.NotificationType.GameInvitation) && !(n.read || n.isRead) && n.invitation
    ) as Array<Notification & { invitation: GameInvitation }>
  })

  /**
   * Charge les notifications depuis l'API
   */
  async function loadNotifications(forceRefresh: boolean = false) {
    if (!userStore.userId) return

    // Ne pas recharger si déjà chargé récemment (moins de 30 secondes)
    if (!forceRefresh && lastFetchTime.value) {
      const timeSinceLastFetch = Date.now() - lastFetchTime.value.getTime()
      if (timeSinceLastFetch < 30000) return
    }

    isLoading.value = true
    try {
      const apiNotifications = await notificationsApi.getUserNotifications(userStore.userId, false)
      notifications.value = apiNotifications.map(convertApiNotification)
      lastFetchTime.value = new Date()
    } catch (error) {
      console.error('Erreur lors du chargement des notifications:', error)
    } finally {
      isLoading.value = false
    }
  }

  /**
   * Charge le nombre de notifications non lues
   */
  async function loadUnreadCount(): Promise<number> {
    if (!userStore.userId) return 0
    try {
      return await notificationsApi.getUnreadCount(userStore.userId)
    } catch (error) {
      console.error('Erreur lors du chargement du nombre de notifications non lues:', error)
      return 0
    }
  }

  /**
   * Convertit une notification API en notification locale
   */
  function convertApiNotification(apiNotif: NotificationDto): Notification {
    let invitation: GameInvitation | undefined
    if (apiNotif.dataJson) {
      try {
        const data = JSON.parse(apiNotif.dataJson)
        if (data.gameId && data.gameType) {
          invitation = {
            id: data.gameId,
            gameType: data.gameType,
            gameId: data.gameId,
            fromPlayerName: data.fromPlayerName || 'Joueur',
            fromPlayerId: data.fromPlayerId || 0,
            wager: data.wager || 0,
            game: data.game,
            createdAt: new Date(apiNotif.createdAt)
          }
        }
      } catch {
        // Ignore JSON parse errors
      }
    }

    return {
      id: apiNotif.id,
      type: apiNotif.type,
      title: apiNotif.title,
      message: apiNotif.message,
      invitation,
      read: apiNotif.isRead,
      isRead: apiNotif.isRead, // Pour compatibilité
      createdAt: apiNotif.createdAt,
      dataJson: apiNotif.dataJson
    }
  }

  function addNotification(notification: Omit<Notification, 'id' | 'read' | 'isRead' | 'createdAt'>) {
    const newNotification: Notification = {
      ...notification,
      id: `notif-${Date.now()}-${Math.random()}`,
      read: false,
      isRead: false,
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

  async function markAsRead(notificationId: string | number) {
    if (!userStore.userId) return

    const notification = notifications.value.find(n => n.id === notificationId)
    if (notification) {
      notification.read = true
      notification.isRead = true
    }

    // Si c'est une notification API, mettre à jour côté serveur
    if (typeof notificationId === 'number') {
      try {
        await notificationsApi.markNotificationAsRead(notificationId, userStore.userId)
      } catch (error) {
        console.error('Erreur lors du marquage de la notification comme lue:', error)
        // Revert local change
        if (notification) {
          notification.read = false
          notification.isRead = false
        }
      }
    }
  }

  async function markAllAsRead() {
    if (!userStore.userId) return

    notifications.value.forEach(n => {
      n.read = true
      n.isRead = true
    })

    try {
      await notificationsApi.markAllNotificationsAsRead(userStore.userId)
      await loadNotifications(true) // Recharger pour avoir les données à jour
    } catch (error) {
      console.error('Erreur lors du marquage de toutes les notifications comme lues:', error)
    }
  }

  async function removeNotification(notificationId: string | number) {
    if (!userStore.userId) return

    const index = notifications.value.findIndex(n => n.id === notificationId)
    if (index !== -1) {
      notifications.value.splice(index, 1)
    }

    // Si c'est une notification API, supprimer côté serveur
    if (typeof notificationId === 'number') {
      try {
        await notificationsApi.deleteNotification(notificationId, userStore.userId)
      } catch (error) {
        console.error('Erreur lors de la suppression de la notification:', error)
      }
    }
  }

  function removeInvitation(gameId: number, gameType: string) {
    notifications.value = notifications.value.filter(
      n => !(n.invitation && n.invitation.gameId === gameId && n.invitation.gameType === gameType)
    )
  }

  function toggleNotificationsPanel() {
    showNotificationsPanel.value = !showNotificationsPanel.value
    if (showNotificationsPanel.value) {
      loadNotifications(true)
    }
  }

  function closeNotificationsPanel() {
    showNotificationsPanel.value = false
  }

  // Charger les notifications au montage si l'utilisateur est connecté
  if (userStore.userId) {
    loadNotifications()
  }

  return {
    notifications,
    showNotificationsPanel,
    isLoading,
    unreadCount,
    unreadInvitations,
    loadNotifications,
    loadUnreadCount,
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

