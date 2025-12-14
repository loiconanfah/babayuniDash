/**
 * Service API pour la boutique et les items
 */

import type { Item, UserItem, UserCoins, PurchaseItemRequest, EquipItemRequest } from '@/types'

// Utiliser VITE_API_URL si disponible (pour Render), sinon utiliser /api (pour le proxy Vite en développement)
const API_BASE_URL = import.meta.env.VITE_API_URL || '/api'

function getDefaultHeaders(): HeadersInit {
  return {
    'Accept': 'application/json',
    'Content-Type': 'application/json',
    'ngrok-skip-browser-warning': 'true'
  }
}

async function handleResponse<T>(response: Response): Promise<T> {
  if (!response.ok) {
    const error = await response.text()
    throw new Error(error || `HTTP error! status: ${response.status}`)
  }
  return response.json()
}

/**
 * Récupère tous les items disponibles
 */
export async function getAllItems(userId?: number): Promise<Item[]> {
  const url = userId 
    ? `${API_BASE_URL}/Shop/items?userId=${userId}`
    : `${API_BASE_URL}/Shop/items`
  const response = await fetch(url)
  return handleResponse<Item[]>(response)
}

/**
 * Récupère un item par son ID
 */
export async function getItemById(itemId: number, userId?: number): Promise<Item> {
  const url = userId
    ? `${API_BASE_URL}/Shop/items/${itemId}?userId=${userId}`
    : `${API_BASE_URL}/Shop/items/${itemId}`
  const response = await fetch(url)
  return handleResponse<Item>(response)
}

/**
 * Récupère les items possédés par un utilisateur
 */
export async function getUserItems(userId: number): Promise<UserItem[]> {
  const response = await fetch(`${API_BASE_URL}/Shop/users/${userId}/items`, {
    headers: getDefaultHeaders()
  })
  return handleResponse<UserItem[]>(response)
}

/**
 * Achète un item
 */
export async function purchaseItem(request: PurchaseItemRequest): Promise<UserItem> {
  const response = await fetch(`${API_BASE_URL}/Shop/purchase`, {
    method: 'POST',
      headers: getDefaultHeaders(),
    body: JSON.stringify(request)
  })
  return handleResponse<UserItem>(response)
}

/**
 * Équipe ou déséquipe un item
 */
export async function equipItem(userItemId: number, request: EquipItemRequest): Promise<UserItem> {
  const response = await fetch(`${API_BASE_URL}/Shop/items/${userItemId}/equip`, {
    method: 'PUT',
      headers: getDefaultHeaders(),
    body: JSON.stringify(request)
  })
  return handleResponse<UserItem>(response)
}

/**
 * Récupère le nombre de coins d'un utilisateur
 */
export async function getUserCoins(userId: number): Promise<UserCoins> {
  const response = await fetch(`${API_BASE_URL}/Shop/users/${userId}/coins`, {
    headers: getDefaultHeaders()
  })
  return handleResponse<UserCoins>(response)
}

/**
 * Ajoute des coins à un utilisateur
 */
export async function addCoins(userId: number, amount: number): Promise<UserCoins> {
  const response = await fetch(`${API_BASE_URL}/Shop/users/${userId}/coins`, {
    method: 'POST',
      headers: getDefaultHeaders(),
    body: JSON.stringify(amount)
  })
  return handleResponse<UserCoins>(response)
}

