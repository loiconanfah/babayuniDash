# 🌉 Hashi - Jeu de Puzzle des Ponts

![Hashi Logo](https://img.shields.io/badge/Jeu-Hashi-blueviolet?style=for-the-badge)
![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![Vue.js](https://img.shields.io/badge/Vue.js-3.5-4FC08D?style=flat-square&logo=vue.js)
![TypeScript](https://img.shields.io/badge/TypeScript-5.9-3178C6?style=flat-square&logo=typescript)

## 📋 Table des matières

- [Description](#-description)
- [Architecture du Projet](#-architecture-du-projet)
- [Technologies Utilisées](#-technologies-utilisées)
- [Installation](#-installation)
- [Démarrage](#-démarrage)
- [Structure du Projet](#-structure-du-projet)
- [API REST](#-api-rest)
- [Fonctionnalités](#-fonctionnalités)
- [Règles du Jeu](#-règles-du-jeu)
- [Pour les Collaborateurs](#-pour-les-collaborateurs)

---

## 🎮 Description

**Hashi** (橋をかけろ, *Hashiwokakero*) est un jeu de puzzle logique japonais où le but est de relier toutes les îles avec des ponts en respectant des règles précises.

Ce projet est une implémentation complète du jeu avec :
- Un **backend ASP.NET Core** pour la logique métier et la gestion des données
- Un **frontend Vue.js 3** avec TypeScript pour l'interface utilisateur
- Une **API REST** pour la communication entre les deux
- Une **base de données SQLite** pour la persistance

---

## 🏗️ Architecture du Projet

```
projet-de-session-hashi-prisonbreak/
│
├── frontend/                          # Application Vue.js 3 (TypeScript)
│   ├── src/
│   │   ├── components/                # Composants Vue réutilisables
│   │   │   └── game/                  # Composants spécifiques au jeu
│   │   │       ├── IslandComponent.vue      # Affichage d'une île
│   │   │       ├── BridgeComponent.vue      # Affichage d'un pont
│   │   │       ├── GameGrid.vue             # Grille de jeu principale
│   │   │       └── GameControls.vue         # Contrôles du jeu
│   │   ├── views/                     # Pages/Vues de l'application
│   │   │   ├── MenuView.vue           # Menu principal
│   │   │   ├── PuzzleSelectionView.vue # Sélection de puzzle
│   │   │   ├── GeneratePuzzleView.vue  # Génération de puzzle
│   │   │   └── GameView.vue           # Vue de jeu
│   │   ├── stores/                    # Stores Pinia (gestion d'état)
│   │   │   ├── game.ts                # État du jeu
│   │   │   └── puzzle.ts              # État des puzzles
│   │   ├── services/                  # Services pour les appels API
│   │   │   └── api.ts                 # Client API REST
│   │   ├── types/                     # Types TypeScript
│   │   │   └── index.ts               # Définitions de types
│   │   ├── router/                    # Configuration Vue Router
│   │   │   └── index.ts
│   │   ├── App.vue                    # Composant racine
│   │   └── main.ts                    # Point d'entrée
│   └── package.json
│
└── prisonbreak/prisonbreak.Server/    # Backend ASP.NET Core
    ├── Models/                        # Modèles de données (entités)
    │   ├── Island.cs                  # Modèle d'île
    │   ├── Bridge.cs                  # Modèle de pont
    │   ├── Puzzle.cs                  # Modèle de puzzle
    │   └── Game.cs                    # Modèle de partie
    ├── DTOs/                          # Data Transfer Objects (pour l'API)
    │   ├── IslandDto.cs
    │   ├── BridgeDto.cs
    │   ├── PuzzleDto.cs
    │   ├── GameDto.cs
    │   └── ValidationResultDto.cs
    ├── Services/                      # Services métier
    │   ├── IPuzzleService.cs          # Interface service de puzzles
    │   ├── PuzzleService.cs           # Implémentation
    │   ├── IGameService.cs            # Interface service de parties
    │   ├── GameService.cs             # Implémentation
    │   ├── IValidationService.cs      # Interface service de validation
    │   └── ValidationService.cs       # Implémentation
    ├── Controllers/                   # Contrôleurs API REST
    │   ├── PuzzlesController.cs       # API pour les puzzles
    │   └── GamesController.cs         # API pour les parties
    ├── Data/                          # Accès aux données
    │   └── HashiDbContext.cs          # Contexte Entity Framework
    ├── Program.cs                     # Point d'entrée et configuration
    └── prisonbreak.Server.csproj      # Fichier de projet .NET
```

---

## 🛠️ Technologies Utilisées

### Backend
- **ASP.NET Core 8.0** - Framework web moderne et performant
- **Entity Framework Core** - ORM pour la gestion de la base de données
- **SQLite** - Base de données légère et portable
- **Swagger** - Documentation automatique de l'API REST

### Frontend
- **Vue.js 3.5** - Framework JavaScript progressif
- **TypeScript 5.9** - Typage statique pour JavaScript
- **Pinia** - Gestion d'état moderne pour Vue.js
- **Vue Router** - Routing côté client
- **Vite** - Build tool ultra-rapide
- **Tailwind CSS** - Framework CSS utilitaire (optionnel)

---

## 📦 Installation

### Prérequis

- **Node.js** 20.19+ ou 22.12+ (pour le frontend)
- **.NET SDK 8.0** (pour le backend)
- **Git** (pour cloner le repository)

### 1. Cloner le Repository

```bash
git clone [URL_DU_REPO]
cd projet-de-session-hashi-prisonbreak
```

### 2. Installer les Dépendances du Frontend

```bash
cd frontend
npm install
```

### 3. Configurer le Backend

```bash
cd ../prisonbreak/prisonbreak.Server
dotnet restore
```

La base de données SQLite sera créée automatiquement au premier lancement.

---

## 🚀 Démarrage

### Démarrer le Backend (Port 5001)

```bash
cd prisonbreak/prisonbreak.Server
dotnet run
```

Le backend sera accessible à :
- **API**: `https://localhost:5001/api`
- **Swagger**: `https://localhost:5001/swagger`

### Démarrer le Frontend (Port 5173)

Dans un autre terminal :

```bash
cd frontend
npm run dev
```

Le frontend sera accessible à : `http://localhost:5173`

### Accéder à l'Application

Ouvrez votre navigateur et accédez à : **http://localhost:5173**

---

## 📁 Structure du Projet

### Backend - Fichiers Importants

| Fichier | Description |
|---------|-------------|
| `Program.cs` | Configuration de l'application (services, middleware, CORS) |
| `HashiDbContext.cs` | Configuration de la base de données et des relations |
| `Models/*.cs` | Entités de la base de données |
| `DTOs/*.cs` | Objets de transfert pour l'API |
| `Services/*.cs` | Logique métier (validation, génération, gestion) |
| `Controllers/*.cs` | Endpoints de l'API REST |

### Frontend - Fichiers Importants

| Fichier | Description |
|---------|-------------|
| `main.ts` | Point d'entrée de l'application |
| `App.vue` | Composant racine |
| `router/index.ts` | Configuration des routes |
| `stores/*.ts` | Gestion d'état avec Pinia |
| `services/api.ts` | Client pour les appels API |
| `types/index.ts` | Définitions de types TypeScript |
| `components/game/*.vue` | Composants du jeu |
| `views/*.vue` | Pages de l'application |

---

## 🔌 API REST

### Endpoints Principaux

#### Puzzles

| Méthode | Endpoint | Description |
|---------|----------|-------------|
| `GET` | `/api/puzzles` | Récupère tous les puzzles |
| `GET` | `/api/puzzles/{id}` | Récupère un puzzle par ID |
| `GET` | `/api/puzzles/difficulty/{level}` | Récupère les puzzles par difficulté |
| `POST` | `/api/puzzles/generate` | Génère un nouveau puzzle |

#### Games (Parties)

| Méthode | Endpoint | Description |
|---------|----------|-------------|
| `POST` | `/api/games` | Crée une nouvelle partie |
| `GET` | `/api/games/{id}` | Récupère une partie par ID |
| `PUT` | `/api/games/{id}/bridges` | Met à jour les ponts placés |
| `POST` | `/api/games/{id}/validate` | Valide la solution |
| `POST` | `/api/games/{id}/abandon` | Abandonne une partie |

### Documentation Swagger

Accédez à la documentation interactive : **https://localhost:5001/swagger**

---

## ✨ Fonctionnalités

### Implémentées

✅ **Gestion des Puzzles**
- Récupération de puzzles existants
- Filtrage par difficulté
- Génération de nouveaux puzzles (basique)

✅ **Gameplay**
- Placement de ponts simples et doubles
- Suppression de ponts
- Sélection interactive des îles
- Affichage visuel de l'état des îles (complète, incomplète, erreur)

✅ **Validation**
- Vérification du nombre de ponts par île
- Détection des croisements de ponts
- Vérification de la connectivité du réseau
- Feedback détaillé sur les erreurs

✅ **Interface Utilisateur**
- Menu principal
- Sélection de puzzle par difficulté
- Génération de puzzle personnalisé
- Grille de jeu interactive
- Timer en temps réel
- Contrôles de partie (valider, réinitialiser, abandonner)

✅ **Backend**
- API REST complète et documentée
- Base de données avec Entity Framework Core
- Sauvegarde automatique de la progression
- Système de score

### À Améliorer

🔄 **Génération de Puzzles**
- Actuellement simplifiée
- TODO: Implémenter un générateur garantissant une solution unique
- TODO: Utiliser des algorithmes de backtracking

🔄 **Authentification**
- Structure prête pour un système d'utilisateurs
- TODO: Implémenter JWT/OAuth

🔄 **Fonctionnalités Additionnelles**
- Système d'indices
- Classement/leaderboard
- Partage de puzzles
- Mode multijoueur

---

## 📖 Règles du Jeu

### Objectif
Relier toutes les îles avec des ponts en respectant les contraintes.

### Règles

1. **Nombre de ponts** : Chaque île indique combien de ponts doivent y être connectés (1 à 8)

2. **Direction** : Les ponts peuvent être **horizontaux** ou **verticaux** uniquement (pas de diagonales)

3. **Ponts doubles** : Entre deux îles, il peut y avoir :
   - 0 pont
   - 1 pont simple
   - 2 ponts doubles (maximum)

4. **Pas de croisements** : Les ponts ne peuvent **jamais se croiser**

5. **Pas de traversée** : Un pont ne peut pas passer par-dessus une île

6. **Réseau connecté** : À la fin, toutes les îles doivent former **un seul réseau connecté** (pas de groupes isolés)

### Comment Jouer

1. **Sélectionner une île** : Cliquez sur une première île
2. **Créer un pont** : Cliquez sur une autre île alignée pour créer un pont
3. **Pont double** : Cliquez à nouveau pour créer un pont double
4. **Supprimer** : Cliquez une 3ème fois pour supprimer le pont
5. **Valider** : Quand vous pensez avoir terminé, cliquez sur "Valider"

### Codes Couleur

- 🔵 **Bleu** : Île sélectionnée
- 🟢 **Vert** : Île complète (bon nombre de ponts)
- 🟠 **Orange** : Île incomplète (pas assez de ponts)
- 🔴 **Rouge** : Île en erreur (trop de ponts)

---

## 👥 Pour les Collaborateurs

### Standards de Code

#### Backend (C#)
- Suivre les conventions C# standards
- Utiliser des commentaires XML pour la documentation
- Chaque méthode publique doit avoir une description
- Les services doivent avoir une interface

#### Frontend (TypeScript)
- Utiliser TypeScript strict
- Documenter les fonctions complexes
- Suivre les conventions Vue.js 3 (Composition API)
- Nommer les composants en PascalCase

### Git Workflow

1. Créer une branche pour chaque fonctionnalité :
   ```bash
   git checkout -b feature/nom-de-la-fonctionnalite
   ```

2. Commiter régulièrement avec des messages clairs :
   ```bash
   git commit -m "feat: ajouter validation des ponts croisés"
   ```

3. Pousser et créer une Pull Request

### Structure des Commits

- `feat:` - Nouvelle fonctionnalité
- `fix:` - Correction de bug
- `docs:` - Documentation
- `style:` - Formatage, style
- `refactor:` - Refactoring de code
- `test:` - Ajout de tests
- `chore:` - Maintenance

### Développement

#### Ajouter un Nouveau Service Backend

1. Créer l'interface dans `Services/IMonService.cs`
2. Implémenter dans `Services/MonService.cs`
3. Enregistrer dans `Program.cs` :
   ```csharp
   builder.Services.AddScoped<IMonService, MonService>();
   ```

#### Ajouter une Nouvelle Vue Frontend

1. Créer le fichier dans `views/MaVue.vue`
2. Ajouter la route dans `router/index.ts`
3. Utiliser les stores Pinia pour l'état

#### Tester l'API

1. Lancer le backend
2. Accéder à Swagger : `https://localhost:5001/swagger`
3. Tester les endpoints directement

### Base de Données

#### Créer une Migration (si besoin)

```bash
cd prisonbreak/prisonbreak.Server
dotnet ef migrations add NomDeLaMigration
dotnet ef database update
```

#### Réinitialiser la Base de Données

Supprimer le fichier `hashi.db` et relancer l'application.

---

## 🐛 Debugging

### Backend

- Les logs sont affichés dans la console
- Utiliser Visual Studio ou VS Code avec l'extension C#
- Points d'arrêt disponibles

### Frontend

- Utiliser Vue DevTools dans le navigateur
- Console du navigateur pour les logs
- Breakpoints dans les DevTools

---

## 📝 Licence

Ce projet est un projet éducatif.

---

## 🙏 Remerciements

- Inspiration du jeu original Hashiwokakero
- Framework Vue.js et ASP.NET Core
- Communauté open-source

---

## 📞 Support

Pour toute question ou problème, consultez la documentation ou contactez l'équipe de développement.

---

**Bon développement ! 🚀**

