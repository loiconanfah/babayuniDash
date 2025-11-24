// frontend/src/services/userApi.ts
// Service responsable de communiquer avec l'API backend pour tout ce qui concerne les joueurs.

export interface UserDto {
  id: number;
  name: string;
  email: string;
  createdAt: string;
  lastLoginAt: string | null;
  isActive: boolean;
  activeSessionCount: number;
}

/**
 * URL de base de l'API backend.
 *
 * ⚠ IMPORTANT :
 * En développement avec SPA Proxy (Visual Studio) : utilise /api (URL relative)
 * Le SPA Proxy redirige automatiquement /api/* vers le backend ASP.NET Core
 * 
 * En développement sans proxy : utilise http://localhost:5000/api
 * En production : utilise la variable d'environnement VITE_API_URL ou https://localhost:5001/api
 */
const API_BASE_URL = import.meta.env.VITE_API_URL || 
  (import.meta.env.DEV ? '/api' : 'https://localhost:5001/api');

/**
 * Appelle l'API backend pour créer ou connecter un joueur.
 * Côté backend, cela appelle POST http://localhost:5000/api/Users avec { name, email }.
 */
export async function createOrLoginUser(
  name: string,
  email: string
): Promise<UserDto> {
  const response = await fetch(`${API_BASE_URL}/Users`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json"
    },
    body: JSON.stringify({ name, email })
  });

  if (!response.ok) {
    const text = await response.text();
    throw new Error(
      `Échec de la création/connexion du joueur (code ${response.status}) : ${text}`
    );
  }

  return (await response.json()) as UserDto;
}
