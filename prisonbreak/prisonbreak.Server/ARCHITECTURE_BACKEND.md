# 🏗️ Architecture Backend Professionnelle - Hashi

## 📋 Vue d'Ensemble

Cette architecture suit les meilleures pratiques de développement .NET avec une séparation claire des responsabilités et une structure modulaire.

---

## 🎯 Principes Architecturaux

### 1. **Séparation des Responsabilités (SoC)**
- **Models** : Entités de domaine (User, Session, Puzzle, Island, Bridge, Game)
- **DTOs** : Objets de transfert de données (sans logique métier)
- **Repositories** : Accès aux données (abstraction de la base de données)
- **Services** : Logique métier (règles du jeu, validations)
- **Controllers** : Points d'entrée API (validation HTTP, délégation aux services)

### 2. **Pattern Repository**
Séparation de l'accès aux données de la logique métier pour faciliter les tests et la maintenance.

### 3. **Dependency Injection**
Tous les services et repositories sont injectés via le conteneur DI d'ASP.NET Core.

### 4. **Documentation XML Complète**
Toutes les classes, méthodes et propriétés publiques sont documentées avec des commentaires XML.

---

## 📁 Structure des Dossiers

```
prisonbreak.Server/
│
├── Controllers/          # Points d'entrée API REST
│   ├── UsersController.cs
│   ├── SessionsController.cs
│   ├── GamesController.cs
│   └── PuzzlesController.cs
│
├── Data/                 # Accès aux données
│   └── HashiDbContext.cs    # Contexte Entity Framework
│
├── DTOs/                 # Objets de transfert de données
│   ├── UserDto.cs
│   ├── SessionDto.cs
│   ├── GameDto.cs
│   ├── PuzzleDto.cs
│   ├── IslandDto.cs
│   ├── BridgeDto.cs
│   ├── ValidationResultDto.cs
│   ├── CreateUserRequest.cs
│   └── CreateSessionRequest.cs
│
├── Models/               # Entités de domaine
│   ├── User.cs              # Utilisateur avec nom et email
│   ├── Session.cs           # Session de jeu (une par compte)
│   ├── Puzzle.cs            # Puzzle Hashi
│   ├── Island.cs            # Île dans un puzzle
│   ├── Bridge.cs            # Pont entre deux îles
│   └── Game.cs              # Partie de jeu
│
├── Repositories/         # Pattern Repository
│   ├── IUserRepository.cs
│   ├── UserRepository.cs
│   ├── ISessionRepository.cs
│   └── SessionRepository.cs
│
└── Services/             # Logique métier
    ├── IUserService.cs
    ├── UserService.cs
    ├── ISessionService.cs
    ├── SessionService.cs
    ├── IPuzzleService.cs
    ├── PuzzleService.cs
    ├── IGameService.cs
    ├── GameService.cs
    ├── IValidationService.cs
    └── ValidationService.cs
```

---

## 🔄 Flux de Données

### Création d'un Utilisateur et d'une Session

```
1. Client → POST /api/users
   ↓
2. UsersController.CreateUser()
   ↓
3. UserService.CreateUserAsync()
   ↓
4. UserRepository.CreateAsync()
   ↓
5. HashiDbContext.SaveChangesAsync()
   ↓
6. Retour UserDto
```

### Création d'une Partie

```
1. Client → POST /api/sessions (créer session)
   ↓
2. SessionsController.CreateSession()
   ↓
3. SessionService.CreateSessionAsync()
   ↓
4. SessionRepository.CreateAsync()
   ↓
5. Client → POST /api/games (créer partie)
   ↓
6. GamesController.CreateGame()
   ↓
7. GameService.CreateGameAsync()
   ↓
8. Vérification Session valide
   ↓
9. Création Game avec SessionId
```

---

## 📊 Modèle de Données

### Relations

```
User (1) ──< (N) Session
  │
  └── Nom, Email, CreatedAt, LastLoginAt, IsActive

Session (1) ──< (N) Game
  │
  └── SessionToken, ExpiresAt, IsActive, UserId

Game (N) ──> (1) Puzzle
  │
  └── SessionId, Status, Score, PlayerBridgesJson

Puzzle (1) ──< (N) Island
Puzzle (1) ──< (N) Bridge (solution)
```

### Contraintes

- **User.Email** : Unique, requis
- **Session.SessionToken** : Unique, requis
- **Game.SessionId** : Requis (chaque partie appartient à une session)
- **Cascade Delete** : Suppression d'un User supprime ses Sessions et Games

---

## 🔐 Gestion des Utilisateurs et Sessions

### Utilisateur (User)

- **Création** : Nom + Email (unique)
- **Authentification** : Via email
- **Sessions** : Un utilisateur peut avoir plusieurs sessions (historique)
- **Session Active** : Une seule session active à la fois par utilisateur

### Session

- **Création** : Automatique lors de la connexion
- **Token** : Généré automatiquement (GUID + timestamp)
- **Expiration** : 30 jours par défaut (configurable)
- **Validation** : Vérifie IsActive et ExpiresAt
- **Désactivation** : Lors de la déconnexion ou expiration

---

## 🎮 Logique Métier Hashi

### Règles du Jeu Implémentées

1. **Nombre de ponts** : Chaque île doit avoir le nombre exact de ponts requis
2. **Direction** : Ponts horizontaux ou verticaux uniquement
3. **Ponts doubles** : Maximum 2 ponts entre deux îles
4. **Pas de croisements** : Les ponts ne peuvent pas se croiser
5. **Connectivité** : Toutes les îles doivent être connectées (algorithme DFS)

### Validation

- **ValidationService** : Valide les solutions complètes
- **Vérifications** :
  - Nombre de ponts par île
  - Absence de croisements
  - Connectivité complète du réseau

---

## 📝 Commentaires XML

Tous les éléments publics sont documentés avec des commentaires XML :

```csharp
/// <summary>
/// Description de la classe/méthode/propriété
/// </summary>
/// <param name="param">Description du paramètre</param>
/// <returns>Description de la valeur de retour</returns>
/// <exception cref="ExceptionType">Quand cette exception est levée</exception>
```

---

## 🚀 Configuration (Program.cs)

### Enregistrement des Services

```csharp
// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IPuzzleService, PuzzleService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IValidationService, ValidationService>();
```

### Cycle de Vie

- **Scoped** : Une instance par requête HTTP (recommandé pour la plupart des services)
- **Singleton** : Une instance pour toute l'application (à éviter sauf cas spécifiques)
- **Transient** : Une nouvelle instance à chaque injection (rare)

---

## ✅ Bonnes Pratiques Implémentées

1. ✅ **Séparation des responsabilités** (Models, DTOs, Repositories, Services, Controllers)
2. ✅ **Pattern Repository** pour l'abstraction des données
3. ✅ **Documentation XML complète** sur tous les éléments publics
4. ✅ **Gestion d'erreurs** avec try-catch et logging
5. ✅ **Validation des entrées** avec Data Annotations
6. ✅ **Index de base de données** pour les performances
7. ✅ **Relations EF Core** correctement configurées
8. ✅ **DTOs** pour éviter l'exposition directe des modèles
9. ✅ **Interfaces** pour faciliter les tests et la maintenance
10. ✅ **Logging** structuré avec ILogger

---

## 🔄 Prochaines Étapes Recommandées

1. **Tests Unitaires** : Tests pour les services et repositories
2. **Tests d'Intégration** : Tests pour les controllers
3. **Authentification JWT** : Pour sécuriser les endpoints
4. **Rate Limiting** : Pour protéger l'API
5. **Migrations EF Core** : Pour gérer les changements de schéma
6. **Caching** : Pour améliorer les performances
7. **Validation avancée** : FluentValidation pour des règles complexes

---

## 📚 Documentation API

L'API est documentée avec Swagger :
- **URL** : `https://localhost:5001/swagger`
- **Endpoints** : Tous documentés avec descriptions et exemples

---

**Architecture créée avec les meilleures pratiques de développement .NET professionnel** 🎯

