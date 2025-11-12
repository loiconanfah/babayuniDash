# 🏛️ Architecture du Projet Hashi

Ce document décrit l'architecture technique complète du projet.

## 📊 Vue d'Ensemble

```
┌─────────────────────────────────────────────────────────┐
│                     NAVIGATEUR                          │
│  ┌─────────────────────────────────────────────────┐   │
│  │           Frontend Vue.js 3 + TypeScript        │   │
│  │  ┌───────────┐  ┌───────────┐  ┌─────────────┐ │   │
│  │  │   Views   │  │Components │  │   Stores    │ │   │
│  │  │   (UI)    │◄─┤ (UI Logic)│◄─┤   (State)   │ │   │
│  │  └─────┬─────┘  └───────────┘  └──────┬──────┘ │   │
│  │        │                               │        │   │
│  │        └───────────┬───────────────────┘        │   │
│  │                    ▼                             │   │
│  │            ┌───────────────┐                     │   │
│  │            │  API Service  │                     │   │
│  │            └───────┬───────┘                     │   │
│  └────────────────────┼─────────────────────────────┘   │
└────────────────────────┼─────────────────────────────────┘
                         │ HTTP/REST
                         │ JSON
┌────────────────────────┼─────────────────────────────────┐
│                        ▼                                 │
│  ┌─────────────────────────────────────────────────┐   │
│  │      Backend ASP.NET Core 8.0 (.NET)           │   │
│  │  ┌───────────┐  ┌───────────┐  ┌───────────┐  │   │
│  │  │Controllers│──►│ Services  │──►│   Models  │  │   │
│  │  │  (API)    │  │ (Business)│  │  (Entities)  │   │
│  │  └───────────┘  └───────┬───┘  └─────┬─────┘  │   │
│  │                          │             │        │   │
│  │                          ▼             ▼        │   │
│  │                  ┌──────────────────────────┐  │   │
│  │                  │  Entity Framework Core   │  │   │
│  │                  └───────────┬──────────────┘  │   │
│  └────────────────────────────┼─────────────────────┘   │
└────────────────────────────────┼─────────────────────────┘
                                 │ SQL
                                 ▼
                         ┌───────────────┐
                         │ SQLite Database│
                         │   (hashi.db)   │
                         └───────────────┘
```

---

## 🎨 Frontend - Vue.js 3

### Stack Technologique

- **Vue 3.5** avec Composition API
- **TypeScript 5.9** pour le typage
- **Pinia** pour la gestion d'état
- **Vue Router** pour le routing
- **Vite** comme build tool
- **Tailwind CSS** (optionnel) pour le styling

### Architecture Frontend

#### 1. **Views (Vues)** - `/src/views/`

Les vues représentent les pages principales de l'application.

```typescript
MenuView.vue              // Menu principal
├── PuzzleSelectionView   // Sélection de puzzles
├── GeneratePuzzleView    // Génération de puzzle
└── GameView              // Vue de jeu principale
```

**Responsabilités :**
- Composition de composants
- Récupération des données via stores
- Gestion du layout de la page

#### 2. **Components (Composants)** - `/src/components/`

Composants réutilisables et spécialisés.

```typescript
game/
├── IslandComponent.vue      // Affichage d'une île
├── BridgeComponent.vue      // Affichage d'un pont
├── GameGrid.vue             // Grille de jeu complète
└── GameControls.vue         // Contrôles du jeu
```

**Responsabilités :**
- Logique UI spécifique
- Émission d'événements
- Affichage réactif

#### 3. **Stores (Pinia)** - `/src/stores/`

Gestion centralisée de l'état de l'application.

```typescript
game.ts
├── State
│   ├── currentGame          // Partie en cours
│   ├── currentPuzzle        // Puzzle joué
│   ├── playerBridges        // Ponts placés
│   ├── selectedIsland       // Île sélectionnée
│   └── elapsedTime          // Temps écoulé
├── Getters
│   ├── hasActiveGame
│   ├── getBridgeCountForIsland
│   └── isIslandComplete
└── Actions
    ├── startGame()
    ├── selectIsland()
    ├── validateSolution()
    └── abandonGame()

puzzle.ts
├── State
│   ├── puzzles              // Liste des puzzles
│   ├── selectedPuzzle       // Puzzle sélectionné
│   └── isLoading
└── Actions
    ├── fetchAllPuzzles()
    ├── fetchPuzzlesByDifficulty()
    └── generatePuzzle()
```

**Responsabilités :**
- État réactif centralisé
- Logique métier frontend
- Communication avec l'API

#### 4. **Services** - `/src/services/`

Couche d'abstraction pour les appels API.

```typescript
api.ts
├── puzzleApi
│   ├── getAll()
│   ├── getById()
│   ├── getByDifficulty()
│   └── generate()
└── gameApi
    ├── create()
    ├── updateBridges()
    ├── validate()
    └── abandon()
```

**Responsabilités :**
- Appels HTTP vers le backend
- Gestion des erreurs
- Transformation des données

#### 5. **Types** - `/src/types/`

Définitions TypeScript pour la sécurité des types.

```typescript
index.ts
├── Island              // Interface d'île
├── Bridge              // Interface de pont
├── Puzzle              // Interface de puzzle
├── Game                // Interface de partie
├── DifficultyLevel     // Enum de difficulté
├── GameStatus          // Enum de statut
└── ValidationResult    // Résultat de validation
```

#### 6. **Router** - `/src/router/`

Configuration des routes de l'application.

```typescript
/                  → MenuView
/puzzles           → PuzzleSelectionView
/generate          → GeneratePuzzleView
/play/:id          → GameView
```

---

## 🔧 Backend - ASP.NET Core

### Stack Technologique

- **ASP.NET Core 8.0**
- **Entity Framework Core 8.0**
- **SQLite** (base de données)
- **Swagger** (documentation API)

### Architecture Backend

#### 1. **Controllers** - `/Controllers/`

Points d'entrée de l'API REST.

```csharp
PuzzlesController
├── GET    /api/puzzles                    // Liste tous les puzzles
├── GET    /api/puzzles/{id}               // Récupère un puzzle
├── GET    /api/puzzles/difficulty/{level} // Filtre par difficulté
└── POST   /api/puzzles/generate           // Génère un puzzle

GamesController
├── POST   /api/games                      // Crée une partie
├── GET    /api/games/{id}                 // Récupère une partie
├── PUT    /api/games/{id}/bridges         // Met à jour les ponts
├── POST   /api/games/{id}/validate        // Valide la solution
└── POST   /api/games/{id}/abandon         // Abandonne la partie
```

**Responsabilités :**
- Validation des requêtes
- Gestion des réponses HTTP
- Délégation aux services

#### 2. **Services** - `/Services/`

Logique métier de l'application.

```csharp
PuzzleService
├── GeneratePuzzleAsync()      // Génère un puzzle aléatoire
├── GetPuzzleByIdAsync()        // Récupère un puzzle
├── GetAllPuzzlesAsync()        // Liste les puzzles
├── GetPuzzlesByDifficultyAsync() // Filtre par difficulté
└── ConvertToDto()              // Conversion vers DTO

GameService
├── CreateGameAsync()           // Crée une partie
├── GetGameByIdAsync()          // Récupère une partie
├── UpdateGameBridgesAsync()    // Met à jour les ponts
├── CompleteGameAsync()         // Termine une partie
└── ConvertToDto()              // Conversion vers DTO

ValidationService
├── ValidateSolution()          // Valide une solution complète
├── DoBridgesIntersect()        // Vérifie les croisements
└── AreAllIslandsConnected()    // Vérifie la connectivité (DFS)
```

**Responsabilités :**
- Logique métier complexe
- Validation des règles du jeu
- Algorithmes (DFS, génération, etc.)

#### 3. **Models** - `/Models/`

Entités de la base de données.

```csharp
Puzzle
├── Id                    // Clé primaire
├── Name                  // Nom du puzzle
├── Width/Height          // Dimensions
├── Difficulty            // Niveau
├── CreatedAt             // Date de création
├── Islands               // Collection d'îles
├── SolutionBridges       // Ponts de la solution
└── Games                 // Parties associées

Island
├── Id
├── X, Y                  // Position
├── RequiredBridges       // Nombre de ponts requis
├── PuzzleId              // FK vers Puzzle
├── BridgesFrom           // Ponts sortants
└── BridgesTo             // Ponts entrants

Bridge
├── Id
├── FromIslandId          // FK vers Island
├── ToIslandId            // FK vers Island
├── IsDouble              // Pont double ?
├── Direction             // Horizontal/Vertical
└── PuzzleId              // FK vers Puzzle

Game
├── Id
├── PuzzleId              // FK vers Puzzle
├── PlayerId              // Identifiant du joueur
├── StartedAt/CompletedAt // Dates
├── ElapsedSeconds        // Durée
├── Status                // En cours/Terminé/Abandonné
├── PlayerBridgesJson     // Ponts du joueur (JSON)
├── Score                 // Score final
└── HintsUsed             // Nombre d'indices utilisés
```

#### 4. **DTOs** - `/DTOs/`

Objets de transfert pour l'API (sans références circulaires).

```csharp
PuzzleDto                 // Version simplifiée de Puzzle
IslandDto                 // Version simplifiée de Island
BridgeDto                 // Version simplifiée de Bridge
GameDto                   // Version simplifiée de Game
ValidationResultDto       // Résultat de validation
```

**Responsabilités :**
- Sérialisation JSON propre
- Éviter les références circulaires
- Exposer uniquement les données nécessaires

#### 5. **Data** - `/Data/`

Contexte de base de données et configuration.

```csharp
HashiDbContext
├── DbSet<Puzzle>         // Table des puzzles
├── DbSet<Island>         // Table des îles
├── DbSet<Bridge>         // Table des ponts
├── DbSet<Game>           // Table des parties
└── OnModelCreating()     // Configuration des relations
```

**Relations :**
- `Puzzle` → `Islands` (1:N)
- `Puzzle` → `SolutionBridges` (1:N)
- `Puzzle` → `Games` (1:N)
- `Island` → `BridgesFrom` (1:N)
- `Island` → `BridgesTo` (1:N)

---

## 🔄 Flux de Données

### Création d'une Partie

```
1. Utilisateur clique "Jouer" sur un puzzle
   ↓
2. Frontend : Vue envoie puzzleId au store
   ↓
3. Store appelle gameApi.create()
   ↓
4. API Service envoie POST /api/games
   ↓
5. Backend : GamesController reçoit la requête
   ↓
6. Controller valide et délègue à GameService
   ↓
7. GameService crée une entité Game
   ↓
8. Entity Framework sauvegarde en base de données
   ↓
9. GameService retourne GameDto
   ↓
10. Controller retourne 201 Created avec GameDto
    ↓
11. API Service reçoit la réponse
    ↓
12. Store met à jour currentGame
    ↓
13. Vue réagit et affiche la grille de jeu
```

### Placement d'un Pont

```
1. Utilisateur clique sur deux îles
   ↓
2. GameGrid émet événement @click
   ↓
3. Store gère la logique (tryCreateBridge)
   ↓
4. Store ajoute/modifie le pont dans playerBridges
   ↓
5. Store appelle saveBridges()
   ↓
6. API Service envoie PUT /api/games/{id}/bridges
   ↓
7. Backend : GamesController reçoit la liste de ponts
   ↓
8. Controller délègue à GameService
   ↓
9. GameService sérialise en JSON et sauvegarde
   ↓
10. Entity Framework met à jour la base
    ↓
11. Backend retourne 200 OK
    ↓
12. Frontend : La vue se met à jour automatiquement (réactivité)
```

### Validation de la Solution

```
1. Utilisateur clique "Valider"
   ↓
2. GameControls appelle handleValidate()
   ↓
3. Store appelle validateSolution()
   ↓
4. API Service envoie POST /api/games/{id}/validate
   ↓
5. Backend : GamesController reçoit la requête
   ↓
6. Controller délègue à ValidationService
   ↓
7. ValidationService applique toutes les règles :
   - Vérifie le nombre de ponts par île
   - Détecte les croisements
   - Vérifie la connectivité (algorithme DFS)
   ↓
8. ValidationService retourne ValidationResultDto
   ↓
9. Si valide : GameService termine la partie avec un score
   ↓
10. Backend retourne ValidationResultDto
    ↓
11. Frontend affiche le résultat
    ↓
12. Si succès : Affiche message de félicitations
```

---

## 🗄️ Modèle de Base de Données

```sql
-- Table Puzzles
CREATE TABLE Puzzles (
    Id INTEGER PRIMARY KEY,
    Name TEXT,
    Width INTEGER NOT NULL,
    Height INTEGER NOT NULL,
    Difficulty INTEGER NOT NULL,
    CreatedAt DATETIME NOT NULL
);

-- Table Islands
CREATE TABLE Islands (
    Id INTEGER PRIMARY KEY,
    PuzzleId INTEGER NOT NULL,
    X INTEGER NOT NULL,
    Y INTEGER NOT NULL,
    RequiredBridges INTEGER NOT NULL,
    FOREIGN KEY (PuzzleId) REFERENCES Puzzles(Id) ON DELETE CASCADE
);

-- Table Bridges
CREATE TABLE Bridges (
    Id INTEGER PRIMARY KEY,
    PuzzleId INTEGER NOT NULL,
    FromIslandId INTEGER NOT NULL,
    ToIslandId INTEGER NOT NULL,
    IsDouble INTEGER NOT NULL,
    Direction INTEGER NOT NULL,
    FOREIGN KEY (PuzzleId) REFERENCES Puzzles(Id) ON DELETE CASCADE,
    FOREIGN KEY (FromIslandId) REFERENCES Islands(Id) ON DELETE RESTRICT,
    FOREIGN KEY (ToIslandId) REFERENCES Islands(Id) ON DELETE RESTRICT
);

-- Table Games
CREATE TABLE Games (
    Id INTEGER PRIMARY KEY,
    PuzzleId INTEGER NOT NULL,
    PlayerId TEXT,
    StartedAt DATETIME NOT NULL,
    CompletedAt DATETIME,
    ElapsedSeconds INTEGER NOT NULL,
    Status INTEGER NOT NULL,
    PlayerBridgesJson TEXT NOT NULL DEFAULT '[]',
    Score INTEGER NOT NULL DEFAULT 0,
    HintsUsed INTEGER NOT NULL DEFAULT 0,
    FOREIGN KEY (PuzzleId) REFERENCES Puzzles(Id) ON DELETE CASCADE
);
```

---

## 🔐 Sécurité et Considérations

### Actuellement Implémenté

✅ Validation des entrées utilisateur
✅ Gestion des erreurs HTTP
✅ CORS configuré pour le développement
✅ HTTPS activé
✅ Logging des erreurs

### À Implémenter (Production)

🔄 Authentification JWT/OAuth
🔄 Rate limiting sur l'API
🔄 Validation côté serveur renforcée
🔄 Chiffrement des données sensibles
🔄 Audit logging

---

## 📈 Performance et Optimisation

### Frontend

- **Lazy loading** des routes
- **Code splitting** automatique par Vite
- **Réactivité fine** avec Vue 3
- **Memoization** dans les computed

### Backend

- **Index de base de données** sur les clés fréquemment recherchées
- **Async/await** pour toutes les opérations I/O
- **Include explicite** pour éviter le N+1
- **DTOs** pour limiter les données transférées

---

## 🧪 Tests (À Implémenter)

### Frontend
- Tests unitaires (Vitest)
- Tests de composants (Vue Test Utils)
- Tests E2E (Playwright)

### Backend
- Tests unitaires (xUnit)
- Tests d'intégration
- Tests de l'API (Swagger)

---

## 📝 Documentation

- **README.md** : Vue d'ensemble et installation
- **ARCHITECTURE.md** : Architecture technique (ce fichier)
- **CONTRIBUTING.md** : Guide de contribution
- **START.md** : Guide de démarrage rapide
- **Swagger** : Documentation API interactive

---

**Cette architecture est évolutive et peut être adaptée selon les besoins futurs du projet.**

