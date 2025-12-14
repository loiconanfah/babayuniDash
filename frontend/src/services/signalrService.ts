/**
 * Service SignalR pour le chat en temps réel
 */

import * as signalR from '@microsoft/signalr'
import type { ChatMessage } from '@/stores/chat'

// Utiliser VITE_API_URL si disponible (pour Render), sinon utiliser /hubs/chat (pour le proxy Vite en développement)
const API_BASE_URL = import.meta.env.VITE_API_URL || ''

class SignalRService {
  private connection: signalR.HubConnection | null = null
  private isConnecting = false
  private reconnectAttempts = 0
  private maxReconnectAttempts = 5

  /**
   * Démarre la connexion SignalR
   */
  async startConnection(userId: number): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      return
    }

    if (this.isConnecting) {
      return
    }

    this.isConnecting = true

    try {
      // Construire l'URL du hub SignalR
      // Si VITE_API_URL est défini (Render), utiliser cette URL, sinon utiliser /hubs/chat (proxy Vite)
      const hubUrl = API_BASE_URL ? `${API_BASE_URL.replace('/api', '')}/hubs/chat` : '/hubs/chat'
      
      this.connection = new signalR.HubConnectionBuilder()
        .withUrl(hubUrl, {
          withCredentials: true,
          skipNegotiation: false,
          transport: signalR.HttpTransportType.WebSockets | signalR.HttpTransportType.LongPolling
        })
        .withAutomaticReconnect({
          nextRetryDelayInMilliseconds: (retryContext) => {
            if (retryContext.previousRetryCount < this.maxReconnectAttempts) {
              return Math.min(1000 * Math.pow(2, retryContext.previousRetryCount), 30000)
            }
            return null
          }
        })
        .configureLogging(signalR.LogLevel.Information)
        .build()

      // Rejoindre le chat avec l'userId
      this.connection.onclose(async () => {
        console.log('Connexion SignalR fermée')
        this.reconnectAttempts++
        if (this.reconnectAttempts < this.maxReconnectAttempts) {
          await this.startConnection(userId)
        }
      })

      this.connection.onreconnecting(() => {
        console.log('Reconnexion SignalR en cours...')
      })

      this.connection.onreconnected(() => {
        console.log('SignalR reconnecté')
        this.reconnectAttempts = 0
        this.joinChat(userId)
      })

      await this.connection.start()
      await this.joinChat(userId)
      this.reconnectAttempts = 0
      console.log('Connexion SignalR établie')
    } catch (error) {
      console.error('Erreur lors de la connexion SignalR:', error)
      throw error
    } finally {
      this.isConnecting = false
    }
  }

  /**
   * Rejoint le chat
   */
  private async joinChat(userId: number): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      await this.connection.invoke('JoinChat', userId)
    }
  }

  /**
   * Arrête la connexion
   */
  async stopConnection(): Promise<void> {
    if (this.connection) {
      await this.connection.stop()
      this.connection = null
      this.reconnectAttempts = 0
    }
  }

  /**
   * Envoie un message via SignalR
   */
  async sendMessage(senderId: number, receiverId: number, content: string, messageDto: ChatMessage): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      await this.connection.invoke('SendMessageToUser', senderId, receiverId, content, messageDto)
    } else {
      throw new Error('Connexion SignalR non établie')
    }
  }

  /**
   * Marque un message comme lu via SignalR
   */
  async markMessageAsRead(messageId: number, userId: number, senderId: number): Promise<void> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      await this.connection.invoke('MarkMessageAsRead', messageId, userId, senderId)
    }
  }

  /**
   * Vérifie si un utilisateur est en ligne
   */
  async isUserOnline(userId: number): Promise<boolean> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      return await this.connection.invoke<boolean>('IsUserOnline', userId)
    }
    return false
  }

  /**
   * Récupère la liste des utilisateurs en ligne
   */
  async getOnlineUsers(): Promise<number[]> {
    if (this.connection?.state === signalR.HubConnectionState.Connected) {
      return await this.connection.invoke<number[]>('GetOnlineUsers')
    }
    return []
  }

  /**
   * Écoute les nouveaux messages
   */
  onReceiveMessage(callback: (message: ChatMessage) => void): void {
    if (this.connection) {
      this.connection.on('ReceiveMessage', callback)
    }
  }

  /**
   * Écoute la confirmation d'envoi de message
   */
  onMessageSent(callback: (message: ChatMessage) => void): void {
    if (this.connection) {
      this.connection.on('MessageSent', callback)
    }
  }

  /**
   * Écoute les notifications de message lu
   */
  onMessageRead(callback: (messageId: number, userId: number) => void): void {
    if (this.connection) {
      this.connection.on('MessageRead', callback)
    }
  }

  /**
   * Écoute les notifications d'utilisateur en ligne
   */
  onUserOnline(callback: (userId: number) => void): void {
    if (this.connection) {
      this.connection.on('UserOnline', callback)
    }
  }

  /**
   * Écoute les notifications d'utilisateur hors ligne
   */
  onUserOffline(callback: (userId: number) => void): void {
    if (this.connection) {
      this.connection.on('UserOffline', callback)
    }
  }

  /**
   * Supprime tous les listeners
   */
  removeAllListeners(): void {
    if (this.connection) {
      this.connection.off('ReceiveMessage')
      this.connection.off('MessageSent')
      this.connection.off('MessageRead')
      this.connection.off('UserOnline')
      this.connection.off('UserOffline')
    }
  }

  /**
   * Vérifie si la connexion est active
   */
  isConnected(): boolean {
    return this.connection?.state === signalR.HubConnectionState.Connected
  }
}

export const signalRService = new SignalRService()

