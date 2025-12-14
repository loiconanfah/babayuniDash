// frontend/src/services/sessionApi.ts
// Service responsable de communiquer avec l'API backend pour la gestion des sessions.

export interface SessionDto {
  id: number;
  userId: number;
  sessionToken: string;
  createdAt: string;
  expiresAt: string;
  lastActivityAt: string;
  isActive: boolean;
  ipAddress?: string;
  userAgent?: string;
}

export interface CreateSessionRequest {
  userId: number;
  ipAddress?: string;
  userAgent?: string;
}

/**
 * URL de base de l'API backend.
 * En développement avec SPA Proxy : utilise /api (URL relative, proxyfiée)
 * IMPORTANT: Utiliser toujours des URLs relatives pour que le proxy Vite fonctionne correctement
 * Le proxy Vite redirige automatiquement /api/* vers http://localhost:5000/api/*
 */
const API_BASE_URL = '/api';

/**
 * Appelle l'API backend pour créer une nouvelle session.
 */
export async function createSession(
  request: CreateSessionRequest
): Promise<SessionDto> {
  const response = await fetch(`${API_BASE_URL}/Sessions`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify(request)
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(
      `Échec de la création de la session (code ${response.status}) : ${text}`
    );
  }

  return (await response.json()) as SessionDto;
}

/**
 * Récupère une session par son ID.
 * Lance une erreur si la session n'existe pas.
 */
export async function getSessionById(id: number): Promise<SessionDto> {
  const response = await fetch(`${API_BASE_URL}/Sessions/${id}`);

  if (response.status === 404) {
    throw new Error(`La session avec l'ID ${id} n'existe pas`);
  }

  if (!response.ok) {
    const text = await response.text();
    throw new Error(
      `Échec de la récupération de la session (code ${response.status}) : ${text}`
    );
  }

  return (await response.json()) as SessionDto;
}

/**
 * Récupère une session par son token.
 */
export async function getSessionByToken(token: string): Promise<SessionDto> {
  const response = await fetch(`${API_BASE_URL}/Sessions/token/${token}`);

  if (!response.ok) {
    const text = await response.text();
    throw new Error(
      `Échec de la récupération de la session (code ${response.status}) : ${text}`
    );
  }

  return (await response.json()) as SessionDto;
}

/**
 * Récupère la session active d'un utilisateur.
 * Retourne null si aucune session active n'existe.
 */
export async function getActiveSessionByUserId(userId: number): Promise<SessionDto | null> {
  const response = await fetch(`${API_BASE_URL}/Sessions/user/${userId}/active`);

  if (response.status === 404) {
    return null; // Aucune session active
  }

  if (!response.ok) {
    const text = await response.text();
    throw new Error(
      `Échec de la récupération de la session active (code ${response.status}) : ${text}`
    );
  }

  return (await response.json()) as SessionDto;
}

