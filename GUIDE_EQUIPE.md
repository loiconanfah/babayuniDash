# 🚀 Guide pour l'Équipe de Développement - Hashi

## 📋 Bienvenue dans le Projet Hashi !

Ce document vous guidera pour démarrer le développement sur le projet Hashi, un jeu de puzzle japonais implémenté avec une architecture moderne.

---

## 🎯 Vue d'Ensemble du Projet

### Stack Technologique

**Backend :**
- ASP.NET Core 8.0
- Entity Framework Core 8.0
- SQLite (développement)
- Swagger (documentation API)

**Frontend :**
- Vue.js 3.5 avec TypeScript
- Pinia (gestion d'état)
- Vue Router
- Vite (build tool)
- Tailwind CSS

---

## 📁 Structure du Projet

```
projet-de-session-hashi-prisonbreak2/
│
├── frontend/                    # Application Vue.js (CLIENT UNIQUE)
│   ├── src/
│   │   ├── components/         # Composants réutilisables
│   │   ├── views/              # Pages de l'application
│   │   ├── stores/             # Stores Pinia (état global)
│   │   ├── services/           # Services API
│   │   ├── types/              # Types TypeScript
│   │   └── router/             # Configuration des routes
│   └── package.json
│
└── prisonbreak/
    └── prisonbreak.Server/      # Backend ASP.NET Core
        ├── Controllers/         # Endpoints API REST
        ├── Services/            # Logique métier
        ├── Repositories/        # Accès aux données
        ├── Models/              # Entités de domaine
        ├── DTOs/                # Objets de transfert
        ├── Data/                # Contexte EF Core
        └── Program.cs           # Point d'entrée
```

---

## ⚙️ Prérequis

### Obligatoires

- **Node.js** 20.19+ ou 22.12+
- **.NET SDK 8.0**
- **Git**
- **Visual Studio 2022** (recommandé) ou **VS Code**

### Optionnels

- **Vue DevTools** (extension navigateur)
- **Postman** ou **Thunder Client** (test API)

---

## 🚀 Démarrage Rapide

### 1. Cloner le Repository

```bash
git clone [URL_DU_REPO]
cd projet-de-session-hashi-prisonbreak2
```

### 2. Installer les Dépendances Frontend

```powershell
cd frontend
npm install
cd ..
```

### 3. Restaurer les Packages Backend

```powershell
cd prisonbreak/prisonbreak.Server
dotnet restore
cd ../..
```

### 4. Lancer l'Application

**Option A : Script PowerShell (Recommandé)**

```powershell
cd prisonbreak
.\start-dev.ps1
```

**Option B : Manuellement**

**Terminal 1 - Backend :**
```powershell
cd prisonbreak/prisonbreak.Server
dotnet run
```

**Terminal 2 - Frontend :**
```powershell
cd frontend
npm run dev
```

### 5. Accéder à l'Application

- **Frontend** : http://localhost:5173
- **Backend API** : https://localhost:5001/api
- **Swagger** : https://localhost:5001/swagger

---

## 🏗️ Architecture Backend

### Structure en Couches

```
┌─────────────────────────────────┐
│      Controllers (API)          │  ← Points d'entrée HTTP
├─────────────────────────────────┤
│      Services (Métier)           │  ← Logique métier
├─────────────────────────────────┤
│      Repositories (Données)      │  ← Accès aux données
├─────────────────────────────────┤
│      Models (Domain)             │  ← Entités de domaine
└─────────────────────────────────┘
```

### Modèles Principaux

- **User** : Utilisateur avec nom et email
- **Session** : Session de jeu (une par compte actif)
- **Puzzle** : Puzzle Hashi avec îles et solution
- **Game** : Partie en cours ou terminée
- **Island** : Île dans un puzzle
- **Bridge** : Pont entre deux îles

### Flux de Données

1. **Création Utilisateur** → `POST /api/users`
2. **Création Session** → `POST /api/sessions`
3. **Création Partie** → `POST /api/games` (nécessite SessionId)
4. **Jouer** → `PUT /api/games/{id}/bridges`
5. **Valider** → `POST /api/games/{id}/validate`

---

## 📝 Standards de Code

### Backend (C#)

- ✅ Commentaires XML sur toutes les méthodes publiques
- ✅ Utilisation d'interfaces pour les services
- ✅ Pattern Repository pour l'accès aux données
- ✅ Gestion d'erreurs avec try-catch et logging
- ✅ Validation des entrées avec Data Annotations

**Exemple :**
```csharp
/// <summary>
/// Crée un nouvel utilisateur
/// </summary>
/// <param name="request">Requête contenant les informations</param>
/// <returns>L'utilisateur créé</returns>
public async Task<UserDto> CreateUserAsync(CreateUserRequest request)
{
    // Logique métier...
}
```

### Frontend (TypeScript/Vue)

- ✅ TypeScript strict mode
- ✅ Composition API (Vue 3)
- ✅ Stores Pinia pour l'état global
- ✅ Commentaires JSDoc sur les fonctions complexes

**Exemple :**
```typescript
/**
 * Démarre une nouvelle partie avec un puzzle
 * @param puzzle - Le puzzle à jouer
 */
async function startGame(puzzle: Puzzle): Promise<void> {
    // Logique...
}
```

---

## 🔌 API REST

### Endpoints Principaux

#### Utilisateurs
- `GET /api/users` - Liste tous les utilisateurs
- `GET /api/users/{id}` - Récupère un utilisateur
- `POST /api/users` - Crée un utilisateur
- `PUT /api/users/{id}` - Met à jour un utilisateur
- `DELETE /api/users/{id}` - Désactive un utilisateur

#### Sessions
- `GET /api/sessions/{id}` - Récupère une session
- `GET /api/sessions/token/{token}` - Récupère par token
- `POST /api/sessions` - Crée une session
- `POST /api/sessions/{token}/deactivate` - Désactive une session
- `GET /api/sessions/{token}/validate` - Valide une session

#### Puzzles
- `GET /api/puzzles` - Liste tous les puzzles
- `GET /api/puzzles/{id}` - Récupère un puzzle
- `GET /api/puzzles/difficulty/{level}` - Filtre par difficulté
- `POST /api/puzzles/generate` - Génère un puzzle

#### Parties
- `POST /api/games` - Crée une partie (nécessite SessionId)
- `GET /api/games/{id}` - Récupère une partie
- `PUT /api/games/{id}/bridges` - Met à jour les ponts
- `POST /api/games/{id}/validate` - Valide la solution
- `POST /api/games/{id}/abandon` - Abandonne la partie

**Documentation complète** : https://localhost:5001/swagger

---

## 🧪 Tests

### Backend

```powershell
cd prisonbreak/prisonbreak.Server
dotnet test
```

### Frontend

```powershell
cd frontend
npm run test
```

### Tests Manuels

1. **Swagger** : Tester les endpoints directement
2. **Frontend** : Tester le jeu dans le navigateur
3. **Console** : Vérifier les logs

---

## 🐛 Débogage

### Backend

- **Visual Studio** : Points d'arrêt (F9), Debug (F5)
- **Logs** : Console de Visual Studio
- **Swagger** : Tester les endpoints

### Frontend

- **Vue DevTools** : Inspecter l'état Pinia
- **Chrome DevTools** : Console, Network, Sources
- **Hot Reload** : Modifications automatiques

---

## 📚 Documentation

### Fichiers Importants

- **README.md** : Vue d'ensemble du projet
- **ARCHITECTURE.md** : Architecture complète
- **ARCHITECTURE_BACKEND.md** : Architecture backend détaillée
- **CONTRIBUTING.md** : Guide de contribution
- **START.md** : Guide de démarrage rapide
- **GUIDE_EQUIPE.md** : Ce fichier

### Documentation API

- **Swagger** : https://localhost:5001/swagger
- Commentaires XML dans le code backend
- JSDoc dans le code frontend

---

## 🔄 Workflow Git

### Branches

- `main` : Branche principale (production)
- `develop` : Branche de développement
- `feature/*` : Nouvelles fonctionnalités
- `fix/*` : Corrections de bugs

### Commits

Format : `type(scope): description`

**Types :**
- `feat` : Nouvelle fonctionnalité
- `fix` : Correction de bug
- `docs` : Documentation
- `refactor` : Refactoring
- `test` : Tests

**Exemples :**
```bash
git commit -m "feat(backend): ajouter système de sessions"
git commit -m "fix(frontend): corriger bug de sauvegarde"
git commit -m "docs: mettre à jour README"
```

---

## ✅ Checklist Démarrage

Avant de commencer à développer :

- [ ] Node.js installé et vérifié (`node --version`)
- [ ] .NET SDK 8.0 installé (`dotnet --version`)
- [ ] Repository cloné
- [ ] Dépendances frontend installées (`npm install` dans `frontend/`)
- [ ] Packages backend restaurés (`dotnet restore` dans `prisonbreak.Server/`)
- [ ] Backend démarre sans erreur (`dotnet run`)
- [ ] Frontend démarre sans erreur (`npm run dev`)
- [ ] Swagger accessible (https://localhost:5001/swagger)
- [ ] Application accessible (http://localhost:5173)
- [ ] Aucune erreur dans les consoles

---

## 🎯 Prochaines Étapes de Développement

### Priorité Haute

1. **Tests Unitaires** : Tests pour les services et repositories
2. **Authentification** : Système d'authentification complet
3. **Génération de Puzzles** : Améliorer l'algorithme de génération
4. **Validation Côté Client** : Valider avant d'envoyer au serveur

### Priorité Moyenne

5. **Système d'Indices** : Aide pour les joueurs bloqués
6. **Statistiques** : Suivi des performances des joueurs
7. **Classement** : Leaderboard par difficulté
8. **Mode Sombre** : Thème sombre pour l'interface

### Nice to Have

9. **Animations** : Transitions fluides
10. **Sons** : Feedback audio
11. **Partage** : Partager des puzzles
12. **Mobile** : Version mobile responsive

---

## 🆘 Support

### Problèmes Courants

**Port déjà utilisé :**
```powershell
# Trouver le processus
netstat -ano | findstr :5001
# Tuer le processus
taskkill /PID <PID> /F
```

**Erreur de certificat SSL :**
```powershell
dotnet dev-certs https --trust
```

**Base de données corrompue :**
```powershell
cd prisonbreak/prisonbreak.Server
del hashi.db
dotnet run  # Recréera la DB
```

### Besoin d'Aide ?

1. Consulter la documentation
2. Vérifier les logs dans les consoles
3. Tester avec Swagger
4. Contacter l'équipe

---

## 📞 Contacts

- **Chef de Projet** : [Nom]
- **Tech Lead** : [Nom]
- **Repository** : [URL]

---

## 🎉 Bon Développement !

L'architecture est prête, la documentation est complète, et le projet est configuré pour une collaboration efficace.

**N'oubliez pas :**
- ✅ Commiter régulièrement
- ✅ Écrire des tests
- ✅ Documenter votre code
- ✅ Suivre les standards de code
- ✅ Communiquer avec l'équipe

**Happy coding! 🚀**

