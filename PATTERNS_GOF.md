# 🎯 Design Patterns GOF Utilisés dans le Projet Hashi

Ce document répertorie les design patterns du "Gang of Four" (GOF) identifiés dans le projet Hashi (prisonbreak).

---

## 📋 Réponse Rapide : Patterns Présents

### ✅ Patterns Présents (7)
1. **Singleton** - Créational
2. **Adapter** - Structurel
3. **Facade** - Structurel
4. **Observer** - Comportemental
5. **Template Method** - Comportemental
6. **Dependency Injection** - Structurel (IoC)
7. **Repository** - Structurel (pattern architectural)

### ⚠️ Patterns Partiels/Implicites (4)
1. **Builder** - Créational (via framework ASP.NET Core)
2. **Composite** - Structurel (structure hiérarchique implicite)
3. **Proxy** - Structurel (SPA Proxy, mais pas pattern Proxy GOF)
4. **State** - Comportemental (enum, mais pas pattern State complet)

### ❌ Patterns Absents (6)
1. **Abstract Factory** - Créational
2. **Factory Method** - Créational
3. **Prototype** - Créational
4. **Decorator** - Structurel
5. **Strategy** - Comportemental
6. **Visitor** - Comportemental

---

## 📋 Détails des Patterns Identifiés

### 1. **Repository Pattern** 
**Catégorie :** Pattern de conception structurel/comportemental

**Localisation :** Backend C# (.NET)

**Description :** Abstraction de l'accès aux données, séparant la logique métier de la persistance.

**Exemples dans le code :**
- `IUserRepository` / `UserRepository`
- `ISessionRepository` / `SessionRepository`

**Fichiers :**
```23:24:prisonbreak/prisonbreak.Server/Program.cs
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
```

```1:35:prisonbreak/prisonbreak.Server/Repositories/ISessionRepository.cs
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Repositories;

/// <summary>
/// Interface du repository pour la gestion des sessions
/// Définit les opérations CRUD et de recherche sur les sessions de jeu
/// </summary>
public interface ISessionRepository
{
    /// <summary>
    /// Récupère une session par son identifiant
    /// </summary>
    /// <param name="id">Identifiant unique de la session</param>
    /// <param name="includeUser">Si true, inclut les informations de l'utilisateur</param>
    /// <param name="includeGames">Si true, inclut les parties associées</param>
    /// <returns>La session trouvée, ou null si aucune session n'existe avec cet ID</returns>
    Task<Session?> GetByIdAsync(int id, bool includeUser = false, bool includeGames = false);

    /// <summary>
    /// Récupère une session par son token
    /// </summary>
    /// <param name="sessionToken">Token unique de la session</param>
    /// <param name="includeUser">Si true, inclut les informations de l'utilisateur</param>
    /// <param name="includeGames">Si true, inclut les parties associées</param>
    /// <returns>La session trouvée, ou null si aucune session n'existe avec ce token</returns>
    Task<Session?> GetByTokenAsync(string sessionToken, bool includeUser = false, bool includeGames = false);

    /// <summary>
    /// Récupère toutes les sessions d'un utilisateur
    /// </summary>
    /// <param name="userId">Identifiant de l'utilisateur</param>
    /// <param name="includeActiveOnly">Si true, retourne uniquement les sessions actives</param>
    /// <returns>Liste des sessions de l'utilisateur</returns>
    Task<IEnumerable<Session>> GetByUserIdAsync(int userId, bool includeActiveOnly = false);
```

**Avantages :**
- Séparation des responsabilités
- Facilité de test (mock des repositories)
- Indépendance vis-à-vis de la base de données

---

### 2. **Dependency Injection (Inversion of Control)** ✅
**Catégorie :** Pattern de conception structurel

**Localisation :** Backend C# (.NET) - ASP.NET Core DI Container

**Description :** Injection des dépendances via le conteneur DI d'ASP.NET Core, permettant la découplage et la testabilité.

**Exemples dans le code :**
```19:36:prisonbreak/prisonbreak.Server/Program.cs
// ====================================================
// ENREGISTREMENT DES REPOSITORIES
// Pattern Repository : sépare l'accès aux données de la logique métier
// ====================================================
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();

// ====================================================
// ENREGISTREMENT DES SERVICES MÉTIER
// Ces services implémentent la logique du jeu Hashi
// Utilisent les repositories pour l'accès aux données
// ====================================================
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ISessionService, SessionService>();
builder.Services.AddScoped<IPuzzleService, PuzzleService>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<IValidationService, ValidationService>();
builder.Services.AddScoped<IStatsService, StatsService>();
```

**Injection dans les contrôleurs :**
```21:31:prisonbreak/prisonbreak.Server/Controllers/GamesController.cs
    public GamesController(
        IGameService gameService, 
        IValidationService validationService,
        IPuzzleService puzzleService,
        ILogger<GamesController> logger)
    {
        _gameService = gameService;
        _validationService = validationService;
        _puzzleService = puzzleService;
        _logger = logger;
    }
```

**Avantages :**
- Découplage des composants
- Facilité de test (injection de mocks)
- Gestion du cycle de vie (Scoped, Singleton, Transient)

---

### 3. **Adapter Pattern** ✅
**Catégorie :** Pattern de conception structurel

**Localisation :** Backend C# - Conversion Models ↔ DTOs

**Description :** Les DTOs (Data Transfer Objects) servent d'adaptateurs entre les modèles de domaine et les réponses API.

**Exemples dans le code :**
```147:173:prisonbreak/prisonbreak.Server/Services/GameService.cs
    /// <summary>
    /// Convertit une Game en GameDto pour l'envoyer au frontend
    /// </summary>
    public GameDto ConvertToDto(Game game)
    {
        List<BridgeDto> playerBridges;
        try
        {
            playerBridges = JsonSerializer.Deserialize<List<BridgeDto>>(game.PlayerBridgesJson) ?? new();
        }
        catch
        {
            playerBridges = new();
        }

        return new GameDto
        {
            Id = game.Id,
            PuzzleId = game.PuzzleId,
            Puzzle = game.Puzzle != null ? _puzzleService.ConvertToDto(game.Puzzle) : null,
            SessionId = game.SessionId,
            StartedAt = game.StartedAt,
            CompletedAt = game.CompletedAt,
            ElapsedSeconds = game.ElapsedSeconds,
            Status = (int)game.Status,
            PlayerBridges = playerBridges,
            Score = game.Score,
            HintsUsed = game.HintsUsed
        };
    }
```

**Fichiers DTOs :**
- `UserDto.cs`
- `SessionDto.cs`
- `GameDto.cs`
- `PuzzleDto.cs`
- `IslandDto.cs`
- `BridgeDto.cs`

**Avantages :**
- Évite l'exposition directe des modèles internes
- Évite les références circulaires en JSON
- Contrôle fin des données exposées

---

### 4. **Facade Pattern** ✅
**Catégorie :** Pattern de conception structurel

**Localisation :** Backend C# - Services Layer

**Description :** Les services agissent comme des façades simplifiant l'accès aux repositories et encapsulant la logique métier complexe.

**Exemples dans le code :**
```11:25:prisonbreak/prisonbreak.Server/Services/SessionService.cs
/// <summary>
/// Service de gestion des sessions
/// Implémente la logique métier pour la gestion des sessions de jeu
/// </summary>
public class SessionService : ISessionService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initialise une nouvelle instance du service session
    /// </summary>
    /// <param name="sessionRepository">Repository pour l'accès aux données session</param>
    /// <param name="userRepository">Repository pour l'accès aux données utilisateur</param>
    public SessionService(ISessionRepository sessionRepository, IUserRepository userRepository)
    {
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }
```

**Avantages :**
- Interface simplifiée pour les contrôleurs
- Encapsulation de la logique métier complexe
- Réduction du couplage

---

### 5. **Observer Pattern** ✅
**Catégorie :** Pattern de conception comportemental

**Localisation :** Frontend Vue.js - Système de réactivité

**Description :** Vue.js utilise un système de réactivité basé sur l'Observer pattern pour mettre à jour automatiquement l'UI lors des changements d'état.

**Exemples dans le code :**
```11:36:frontend/src/stores/game.ts
export const useGameStore = defineStore('game', () => {
  // ====================================================
  // STATE - État réactif du jeu
  // ====================================================

  /** Partie actuellement en cours */
  const currentGame = ref<Game | null>(null)

  /** Puzzle actuellement joué */
  const currentPuzzle = ref<Puzzle | null>(null)

  /** Ponts placés par le joueur */
  const playerBridges = ref<Bridge[]>([])

  /** Île actuellement sélectionnée (pour placer des ponts) */
  const selectedIsland = ref<Island | null>(null)

  /** Indique si le jeu est en chargement */
  const isLoading = ref(false)

  /** Message d'erreur éventuel */
  const error = ref<string | null>(null)

  /** Timer pour le temps écoulé */
  const elapsedTime = ref(0)
  let timerInterval: number | null = null

  /** Indique si le jeu est en pause */
  const isPaused = ref(false)
```

**Computed properties (observers) :**
```44:69:frontend/src/stores/game.ts
  // ====================================================
  // GETTERS - Propriétés calculées
  // ====================================================

  /**
   * Vérifie si une partie est en cours
   */
  const hasActiveGame = computed(() => currentGame.value !== null)

  /**
   * Récupère une île par son ID
   */
  function getIslandById(id: number): Island | undefined {
    return currentPuzzle.value?.islands.find((island) => island.id === id)
  }

  /**
   * Compte le nombre de ponts connectés à une île
   */
  const getBridgeCountForIsland = computed(() => {
    return (islandId: number): number => {
      return playerBridges.value.reduce((count, bridge) => {
        if (bridge.fromIslandId === islandId || bridge.toIslandId === islandId) {
          return count + (bridge.isDouble ? 2 : 1)
        }
        return count
      }, 0)
    }
  })
```

**Avantages :**
- Mise à jour automatique de l'UI
- Découplage entre l'état et la présentation
- Réactivité déclarative

---

### 6. **Singleton Pattern** ✅
**Catégorie :** Pattern de conception créational

**Localisation :** Backend C# - DI Container (AddSingleton), Frontend - Pinia Stores

**Description :** Gestion d'instances uniques via le conteneur DI (pour les services) et les stores Pinia (pour l'état global).

**Exemples dans le code :**
- Services enregistrés avec `AddScoped` (une instance par requête HTTP)
- Stores Pinia sont des singletons par défaut

**Avantages :**
- Garantit une seule instance
- Accès global contrôlé
- Économie de ressources

---

### 7. **Template Method Pattern** (Partiel) ✅
**Catégorie :** Pattern de conception comportemental

**Localisation :** Frontend Vue.js - Composables

**Description :** Les composables Vue.js encapsulent une logique réutilisable avec un template de méthodes standardisées.

**Exemples dans le code :**
```14:185:frontend/src/composables/useGame.ts
/**
 * Hook composable pour gérer la logique du jeu
 * Fournit des fonctions et computed properties réutilisables
 */
export function useGame() {
  const gameStore = useGameStore()

  /**
   * Vérifie si une île peut recevoir plus de ponts
   */
  const canIslandReceiveBridge = (island: Island): boolean => {
    const currentCount = gameStore.getBridgeCountForIsland(island.id)
    return currentCount < island.requiredBridges
  }

  /**
   * Vérifie si une île est complète
   */
  const isIslandComplete = (island: Island): boolean => {
    return gameStore.isIslandComplete(island)
  }

  /**
   * Vérifie si deux îles peuvent être connectées
   * Elles doivent être alignées horizontalement ou verticalement
   */
  const canConnectIslands = (island1: Island, island2: Island): boolean => {
    // Les îles doivent être différentes
    if (island1.id === island2.id) return false

    // Elles doivent être alignées
    const isHorizontallyAligned = island1.y === island2.y
    const isVerticallyAligned = island1.x === island2.x

    return isHorizontallyAligned || isVerticallyAligned
  }
```

**Avantages :**
- Réutilisabilité du code
- Encapsulation de la logique métier
- Cohérence dans l'utilisation

---

---

## 📊 Analyse Complète des Patterns GOF

### 🔨 Patterns de Création (Creational Patterns)

| Pattern | Présent | Localisation | Détails |
|---------|---------|--------------|---------|
| **Abstract Factory** | ❌ Non | - | Aucune factory abstraite pour créer des familles d'objets |
| **Builder** | ⚠️ Partiel | Backend C# | `WebApplication.CreateBuilder(args)` utilise le pattern Builder du framework ASP.NET Core |
| **Factory Method** | ❌ Non | - | Pas de méthode factory explicite pour créer des objets |
| **Prototype** | ❌ Non | - | Pas d'implémentation de clonage d'objets |
| **Singleton** | ✅ Oui | Backend/Frontend | DI Container (AddScoped/AddSingleton), Stores Pinia |

**Détails Builder (Partiel) :**
```6:17:prisonbreak/prisonbreak.Server/Program.cs
var builder = WebApplication.CreateBuilder(args);

// ====================================================
// CONFIGURATION DES SERVICES
// ====================================================

// Configuration de la base de données SQLite
// SQLite est utilisé pour sa simplicité et portabilité
// Pour la production, envisagez PostgreSQL ou SQL Server
builder.Services.AddDbContext<HashiDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=hashi.db"));
```

---

### 🏗️ Patterns Structurels (Structural Patterns)

| Pattern | Présent | Localisation | Détails |
|---------|---------|--------------|---------|
| **Adapter** | ✅ Oui | Backend C# | Conversion Models ↔ DTOs via `ConvertToDto()` |
| **Composite** | ⚠️ Implicite | Backend C# | Structure hiérarchique : Puzzle → Islands → Bridges |
| **Decorator** | ❌ Non | - | Pas de décoration d'objets pour ajouter des fonctionnalités |
| **Facade** | ✅ Oui | Backend C# | Services comme façades simplifiant l'accès aux repositories |
| **Proxy** | ⚠️ Partiel | Frontend | SPA Proxy pour redirection, mais pas le pattern Proxy GOF classique |

**Détails Composite (Implicite) :**
La structure hiérarchique du domaine utilise une composition naturelle :
- `Puzzle` contient une collection de `Island`
- `Island` a des relations avec `Bridge` (BridgesFrom, BridgesTo)
- Cette structure représente une composition, mais sans implémentation explicite du pattern Composite

**Exemple de structure hiérarchique :**
```1:48:prisonbreak/prisonbreak.Server/Models/Island.cs
namespace prisonbreak.Server.Models;

/// <summary>
/// Représente une île dans le jeu Hashi
/// Une île est un nœud du puzzle qui doit être connecté à d'autres îles par des ponts
/// </summary>
public class Island
{
    /// <summary>
    /// Identifiant unique de l'île
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Position X de l'île dans la grille (colonne)
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Position Y de l'île dans la grille (ligne)
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// Nombre requis de ponts qui doivent être connectés à cette île
    /// Valeur entre 1 et 8
    /// </summary>
    public int RequiredBridges { get; set; }

    /// <summary>
    /// Identifiant du puzzle auquel appartient cette île
    /// </summary>
    public int PuzzleId { get; set; }

    /// <summary>
    /// Navigation vers le puzzle parent
    /// </summary>
    public Puzzle? Puzzle { get; set; }

    /// <summary>
    /// Collection des ponts partant de cette île
    /// </summary>
    public ICollection<Bridge> BridgesFrom { get; set; } = new List<Bridge>();

    /// <summary>
    /// Collection des ponts arrivant à cette île
    /// </summary>
    public ICollection<Bridge> BridgesTo { get; set; } = new List<Bridge>();
```

---

### 🎭 Patterns Comportementaux (Behavioral Patterns)

| Pattern | Présent | Localisation | Détails |
|---------|---------|--------------|---------|
| **Observer** | ✅ Oui | Frontend Vue.js | Système de réactivité avec `ref()`, `computed()`, `watch()` |
| **State** | ⚠️ Partiel | Backend C# | Enum `GameStatus` mais pas d'implémentation complète du pattern State |
| **Strategy** | ❌ Non | - | Pas de stratégies interchangeables pour les algorithmes |
| **Template Method** | ✅ Oui | Frontend Vue.js | Composables avec méthodes standardisées |
| **Visitor** | ❌ Non | - | Pas de pattern Visitor pour parcourir des structures |

**Détails State (Partiel) :**
```95:116:prisonbreak/prisonbreak.Server/Models/Game.cs
/// <summary>
/// Statut d'une partie de Hashi
/// </summary>
public enum GameStatus
{
    /// <summary>
    /// Partie en cours
    /// </summary>
    InProgress = 1,

    /// <summary>
    /// Partie terminée avec succès
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Partie abandonnée par le joueur
    /// </summary>
    Abandoned = 3,

    /// <summary>
    /// Partie en pause
    /// </summary>
    Paused = 4
}
```

Le projet utilise un enum pour représenter les états, mais n'implémente pas le pattern State complet avec des classes d'état séparées et des transitions gérées par des objets State.

---

## 📊 Résumé des Patterns Identifiés

| Pattern | Catégorie | Présent | Localisation | Utilisation |
|---------|-----------|---------|--------------|-------------|
| **Repository** | Structurel | ✅ | Backend C# | Abstraction de l'accès aux données |
| **Dependency Injection** | Structurel | ✅ | Backend C# | Découplage et testabilité |
| **Adapter** | Structurel | ✅ | Backend C# | Conversion Models ↔ DTOs |
| **Facade** | Structurel | ✅ | Backend C# | Simplification de l'accès aux services |
| **Observer** | Comportemental | ✅ | Frontend Vue.js | Réactivité et mise à jour automatique |
| **Singleton** | Créational | ✅ | Backend/Frontend | Gestion d'instances uniques |
| **Template Method** | Comportemental | ✅ | Frontend Vue.js | Logique réutilisable dans les composables |
| **Builder** | Créational | ⚠️ Partiel | Backend C# | Via framework ASP.NET Core |
| **Composite** | Structurel | ⚠️ Implicite | Backend C# | Structure hiérarchique Puzzle → Islands → Bridges |
| **Proxy** | Structurel | ⚠️ Partiel | Frontend | SPA Proxy (mais pas pattern Proxy GOF) |
| **State** | Comportemental | ⚠️ Partiel | Backend C# | Enum GameStatus (mais pas pattern State complet) |
| **Abstract Factory** | Créational | ❌ | - | Non utilisé |
| **Factory Method** | Créational | ❌ | - | Non utilisé |
| **Prototype** | Créational | ❌ | - | Non utilisé |
| **Decorator** | Structurel | ❌ | - | Non utilisé |
| **Strategy** | Comportemental | ❌ | - | Non utilisé |
| **Visitor** | Comportemental | ❌ | - | Non utilisé |

---

## 🔍 Patterns Non Identifiés (mais potentiellement utiles)

Les patterns suivants ne sont **pas explicitement utilisés** dans le code actuel, mais pourraient être ajoutés :

1. **Strategy Pattern** - Pour différentes stratégies de validation ou de génération de puzzles
2. **Factory Method / Abstract Factory** - Pour créer des instances de services ou de DTOs de manière centralisée
3. **Command Pattern** - Pour encapsuler les actions du jeu (placer un pont, valider, etc.)
4. **State Pattern** (complet) - Pour gérer les transitions d'états d'une partie avec des classes d'état séparées
5. **Visitor Pattern** - Pour parcourir la structure Puzzle → Islands → Bridges et appliquer des opérations
6. **Decorator Pattern** - Pour ajouter des fonctionnalités aux services (logging, caching, etc.)

---

##  Notes

- Les patterns sont principalement utilisés de manière **implicite** via les frameworks (ASP.NET Core, Vue.js)
- L'architecture suit les **meilleures pratiques** de chaque framework
- La séparation des responsabilités est respectée à travers les différentes couches

---

**Date de création :** 2024
**Dernière mise à jour :** Analyse du code actuel

