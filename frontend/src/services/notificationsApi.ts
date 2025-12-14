/**
 * Service API pour les notifications
 */

const API_BASE_URL = import.meta.env.VITE_API_URL || 
  (import.meta.env.DEV ? '/api' : 'https://localhost:5001/api')

export interface NotificationDto {
  id: number
  userId: number
  type: NotificationType
  title: string
  message: string
  dataJson?: string | null
  isRead: boolean
  createdAt: string
  readAt?: string | null
}

export enum NotificationType {
  GameInvitation = 1,
  GameStarted = 2,
  GameEnded = 3,
  FriendMessage = 4,
  FriendRequestAccepted = 5,
  PostComment = 6,
  PostLike = 7,
  TournamentStarted = 8,
  TournamentEnded = 9,
  AchievementUnlocked = 10
}

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const error = await response.text()
    throw new Error(error || `HTTP error! status: ${response.status}`)
  }
  return response.json()
}

/**
 * Récupère toutes les notifications d'un utilisateur
 */
export async function getUserNotifications(userId: number, unreadOnly: boolean = false): Promise<NotificationDto[]> {
  const response = await fetch(`${API_BASE_URL}/Notifications/users/${userId}?unreadOnly=${unreadOnly}`)
  return handleResponse<NotificationDto[]>(response)
}

/**
 * Récupère une notification par son ID
 */
export async function getNotificationById(notificationId: number, userId: number): Promise<NotificationDto> {
  const response = await fetch(`${API_BASE_URL}/Notifications/${notificationId}?userId=${userId}`)
  return handleResponse<NotificationDto>(response)
}

/**
 * Récupère le nombre de notifications non lues
 */
export async function getUnreadCount(userId: number): Promise<number> {
  const response = await fetch(`${API_BASE_URL}/Notifications/users/${userId}/unread-count`)
  const data = await handleResponse<{ count: number }>(response)
  return data.count
}

/**
 * Marque une notification comme lue
 */
export async function markNotificationAsRead(notificationId: number, userId: number): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/Notifications/${notificationId}/read`, {
    method: 'PUT',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({ userId })
  })
  await handleResponse(response)
}

/**
 * Marque toutes les notifications comme lues
 */
export async function markAllNotificationsAsRead(userId: number): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/Notifications/users/${userId}/read-all`, {
    method: 'PUT'
  })
  await handleResponse(response)
}

/**
 * Supprime une notification
 */
export async function deleteNotification(notificationId: number, userId: number): Promise<void> {
  const response = await fetch(`${API_BASE_URL}/Notifications/${notificationId}?userId=${userId}`, {
    method: 'DELETE'
  })
  await handleResponse(response)
}


