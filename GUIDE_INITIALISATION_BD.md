# 🗄️ Guide d'Initialisation de la Base de Données

## 📋 Pour les Nouveaux Développeurs

Ce guide explique comment initialiser la base de données sur votre machine locale.

---

## ✅ Oui, les Utilisateurs SONT Enregistrés en Base de Données

Tous les éléments sont persistés en base de données SQLite :

- ✅ **Utilisateurs** (`Users`) : Nom, Email, dates de création/connexion
- ✅ **Sessions** (`Sessions`) : Tokens de session, expiration
- ✅ **Puzzles** (`Puzzles`) : Grilles avec dimensions, difficulté, thème
- ✅ **Îles** (`Islands`) : Positions (X, Y), nombre de ponts requis
- ✅ **Ponts** (`Bridges`) : Connexions entre îles (solution)
- ✅ **Parties** (`Games`) : Parties jouées par les utilisateurs

---

## 🚀 Initialisation Automatique (Recommandé)

### Option 1 : Lancement Normal (Automatique)

La base de données est **créée automatiquement** au premier lancement de l'application :

```powershell
cd prisonbreak/prisonbreak.Server
dotnet run
```

**Ce qui se passe :**
1. Entity Framework Core détecte qu'il n'y a pas de base de données
2. Les migrations sont appliquées automatiquement (`context.Database.Migrate()` dans `Program.cs`)
3. Le fichier `hashi.db` est créé dans `prisonbreak/prisonbreak.Server/`
4. Les tables sont créées avec la structure correcte

**✅ C'est tout !** Vous pouvez maintenant utiliser l'application.

---

## 🔧 Initialisation Manuelle (Si Besoin)

### Option 2 : Créer les Migrations Manuellement

Si vous avez modifié les modèles et besoin de créer une nouvelle migration :

```powershell
cd prisonbreak/prisonbreak.Server

# Créer une nouvelle migration
dotnet ef migrations add NomDeLaMigration

# Appliquer les migrations à la base de données
dotnet ef database update
```

### Option 3 : Réinitialiser la Base de Données

Si vous voulez repartir de zéro (⚠️ **supprime toutes les données**) :

```powershell
cd prisonbreak/prisonbreak.Server

# Supprimer l'ancienne base de données
Remove-Item hashi.db -ErrorAction SilentlyContinue

# Relancer l'application (recréera la DB automatiquement)
dotnet run
```

---

## 📍 Emplacement de la Base de Données

La base de données SQLite est stockée dans :

```
prisonbreak/prisonbreak.Server/hashi.db
```

**Note :** Ce fichier est **local à chaque développeur**. Chaque machine a sa propre base de données.

---

## 🔍 Vérifier que la Base de Données est Créée

### Méthode 1 : Vérifier le Fichier

```powershell
cd prisonbreak/prisonbreak.Server
Test-Path hashi.db
```

Si `True`, la base de données existe.

### Méthode 2 : Utiliser Swagger

1. Lancez l'application : `dotnet run`
2. Ouvrez : https://localhost:5001/swagger
3. Testez `GET /api/puzzles` - si ça fonctionne, la DB est créée

### Méthode 3 : Vérifier les Logs

Au démarrage, vous devriez voir dans les logs :

```
Application des migrations de base de données...
Migrations appliquées avec succès.
```

---

## 🆕 Premier Utilisateur

Quand un utilisateur se connecte pour la première fois via le frontend :

1. Le frontend appelle `POST /api/users` avec nom et email
2. Le backend crée l'utilisateur dans la table `Users`
3. Une session est créée automatiquement
4. Tout est sauvegardé en base de données

**Aucune action manuelle requise !**

---

## 🔄 Synchronisation entre Développeurs

⚠️ **Important :** La base de données SQLite est **locale** à chaque machine.

- Chaque développeur a sa propre base de données
- Les données ne sont **pas partagées** entre développeurs
- C'est normal et attendu pour le développement

**Pour la production :** Utilisez une base de données partagée (PostgreSQL, SQL Server, etc.)

---

## 🐛 Problèmes Courants

### Erreur : "SQLite Error 1: 'table "Puzzles" already exists'"

**Solution :**
```powershell
cd prisonbreak/prisonbreak.Server
Remove-Item hashi.db
dotnet run
```

### Erreur : "No migrations found"

**Solution :**
```powershell
cd prisonbreak/prisonbreak.Server
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### La base de données ne se crée pas

**Vérifications :**
1. Vérifiez que vous êtes dans le bon répertoire : `prisonbreak/prisonbreak.Server`
2. Vérifiez les permissions d'écriture dans le dossier
3. Vérifiez les logs pour les erreurs

---

## 📊 Structure de la Base de Données

### Tables Principales

| Table | Description | Relations |
|-------|-------------|-----------|
| `Users` | Utilisateurs (nom, email) | 1 → N Sessions |
| `Sessions` | Sessions de jeu | N → 1 User, 1 → N Games |
| `Puzzles` | Puzzles Hashi | 1 → N Islands, 1 → N Bridges |
| `Islands` | Îles d'un puzzle | N → 1 Puzzle |
| `Bridges` | Ponts de la solution | N → 1 Puzzle, N → 1 Island (from/to) |
| `Games` | Parties jouées | N → 1 Session, N → 1 Puzzle |

### Schéma Visuel

```
Users (1) ──→ (N) Sessions (1) ──→ (N) Games
                                    │
                                    └──→ (N) Puzzles (1) ──→ (N) Islands
                                                          └──→ (N) Bridges
```

---

## ✅ Checklist pour Nouveau Développeur

Avant de commencer à développer :

- [ ] Repository cloné
- [ ] .NET SDK 8.0 installé (`dotnet --version`)
- [ ] Node.js installé (`node --version`)
- [ ] Dépendances frontend installées (`npm install` dans `frontend/`)
- [ ] Backend lancé au moins une fois (`dotnet run` dans `prisonbreak.Server/`)
- [ ] Fichier `hashi.db` créé (vérifier avec `Test-Path hashi.db`)
- [ ] Swagger accessible (https://localhost:5001/swagger)
- [ ] Frontend accessible (http://localhost:5173)
- [ ] Premier utilisateur créé via l'interface

---

## 📚 Ressources

- **Documentation Entity Framework** : https://learn.microsoft.com/en-us/ef/core/
- **SQLite Documentation** : https://www.sqlite.org/docs.html
- **Guide de l'équipe** : `GUIDE_EQUIPE.md`

---

## 🎯 Résumé

1. **Lancez simplement** `dotnet run` dans `prisonbreak.Server/`
2. **La base de données est créée automatiquement**
3. **Les utilisateurs sont sauvegardés** quand ils se connectent
4. **Tout est persistant** entre les redémarrages

**C'est tout ! Pas besoin de configuration supplémentaire.** 🚀

