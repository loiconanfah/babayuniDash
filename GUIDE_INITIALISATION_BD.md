# 🗄️ Guide d'Initialisation de la Base de Données - SQL Server

## 📋 Pour les Nouveaux Développeurs

Ce guide explique comment initialiser la base de données **SQL Server** sur votre machine locale.

---

## ✅ Oui, les Utilisateurs SONT Enregistrés en Base de Données

Tous les éléments sont persistés en base de données **SQL Server** :

- ✅ **Utilisateurs** (`Users`) : Nom, Email, dates de création/connexion
- ✅ **Sessions** (`Sessions`) : Tokens de session, expiration
- ✅ **Puzzles** (`Puzzles`) : Grilles avec dimensions, difficulté, thème
- ✅ **Îles** (`Islands`) : Positions (X, Y), nombre de ponts requis
- ✅ **Ponts** (`Bridges`) : Connexions entre îles (solution)
- ✅ **Parties** (`Games`) : Parties jouées par les utilisateurs

---

## 📦 Prérequis

### 1. Installer SQL Server

**Option A : SQL Server Express (Gratuit, Recommandé pour le développement)**

1. Téléchargez SQL Server Express : https://www.microsoft.com/en-us/sql-server/sql-server-downloads
2. Installez avec les options par défaut
3. Notez le nom de l'instance (généralement `localhost\SQLEXPRESS` ou `localhost`)

**Option B : SQL Server LocalDB (Léger, Inclus avec Visual Studio)**

Si vous avez Visual Studio installé, LocalDB est probablement déjà disponible.

**Option C : SQL Server en Docker (Avancé)**

```powershell
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourStrong@Passw0rd" -p 1433:1433 --name sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
```

### 2. Installer SQL Server Management Studio (SSMS) - Optionnel mais Recommandé

Téléchargez SSMS : https://aka.ms/ssmsfullsetup

Cela vous permettra de visualiser et gérer votre base de données facilement.

---

## 🔧 Configuration de la Base de Données

### Étape 1 : Créer la Base de Données

**Méthode 1 : Via SQL Server Management Studio (SSMS)**

1. Ouvrez SSMS
2. Connectez-vous à votre instance SQL Server (ex: `localhost\SQLEXPRESS`)
3. Clic droit sur "Databases" → "New Database"
4. Nommez la base : `HashiPrisonBreak` (ou le nom de votre choix)
5. Cliquez sur "OK"

**Méthode 2 : Via SQL Command Line (sqlcmd)**

```powershell
# Se connecter à SQL Server
sqlcmd -S localhost\SQLEXPRESS -E

# Créer la base de données
CREATE DATABASE HashiPrisonBreak;
GO

# Quitter
EXIT
```

**Méthode 3 : Via PowerShell (Script automatique)**

```powershell
# Créer la base de données automatiquement
$serverInstance = "localhost\SQLEXPRESS"
$databaseName = "HashiPrisonBreak"

$sql = "IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = '$databaseName') CREATE DATABASE [$databaseName]"
Invoke-Sqlcmd -ServerInstance $serverInstance -Query $sql
```

### Étape 2 : Configurer la Chaîne de Connexion

Modifiez le fichier `appsettings.json` ou `appsettings.Development.json` :

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=HashiPrisonBreak;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Explications de la chaîne de connexion :**
- `Server=localhost\\SQLEXPRESS` : Nom de votre instance SQL Server
  - Pour LocalDB : `Server=(localdb)\\mssqllocaldb`
  - Pour SQL Server par défaut : `Server=localhost`
  - Pour Docker : `Server=localhost,1433`
- `Database=HashiPrisonBreak` : Nom de votre base de données
- `Trusted_Connection=True` : Utilise l'authentification Windows
- `TrustServerCertificate=True` : Nécessaire pour les connexions locales sans certificat

**Alternative avec authentification SQL Server :**

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=HashiPrisonBreak;User Id=sa;Password=VotreMotDePasse;TrustServerCertificate=True;"
  }
}
```

### Étape 3 : Mettre à Jour Program.cs

Assurez-vous que `Program.cs` utilise SQL Server :

```csharp
builder.Services.AddDbContext<HashiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### Étape 4 : Installer le Package NuGet (Si nécessaire)

```powershell
cd prisonbreak\prisonbreak.Server
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

---

## 🚀 Initialisation Automatique (Recommandé)

### Option 1 : Lancement Normal (Automatique)

Une fois la base de données créée et la chaîne de connexion configurée, lancez l'application :

```powershell
cd prisonbreak/prisonbreak.Server
dotnet run
```

**Ce qui se passe :**
1. Entity Framework Core se connecte à SQL Server
2. Les migrations sont appliquées automatiquement (`context.Database.Migrate()` dans `Program.cs`)
3. Les tables sont créées dans la base de données `HashiPrisonBreak`
4. La structure est prête à l'emploi

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

**Méthode 1 : Via SSMS**
1. Ouvrez SSMS
2. Clic droit sur la base de données `HashiPrisonBreak`
3. "Tasks" → "Delete"
4. Cochez "Close existing connections"
5. Cliquez sur "OK"
6. Recréez la base de données (voir Étape 1 ci-dessus)
7. Relancez l'application : `dotnet run`

**Méthode 2 : Via SQL Command**

```powershell
sqlcmd -S localhost\SQLEXPRESS -E -Q "DROP DATABASE IF EXISTS HashiPrisonBreak; CREATE DATABASE HashiPrisonBreak;"
```

**Méthode 3 : Via PowerShell**

```powershell
$serverInstance = "localhost\SQLEXPRESS"
$databaseName = "HashiPrisonBreak"

# Supprimer la base de données
Invoke-Sqlcmd -ServerInstance $serverInstance -Query "DROP DATABASE IF EXISTS [$databaseName]"

# Recréer la base de données
Invoke-Sqlcmd -ServerInstance $serverInstance -Query "CREATE DATABASE [$databaseName]"
```

Puis relancez l'application :
```powershell
cd prisonbreak/prisonbreak.Server
dotnet run
```

---

## 📍 Emplacement de la Base de Données

La base de données SQL Server est stockée sur votre instance SQL Server :

- **SQL Server Express** : Généralement dans `C:\Program Files\Microsoft SQL Server\MSSQLXX.SQLEXPRESS\MSSQL\DATA\`
- **LocalDB** : Dans le dossier utilisateur : `C:\Users\VotreNom\`
- **Nom de la base** : `HashiPrisonBreak` (ou celui que vous avez choisi)

**Note :** La base de données est **partagée** entre toutes les applications qui utilisent la même instance SQL Server. Pour isoler chaque développeur, utilisez des noms de base de données différents ou des instances SQL Server séparées.

---

## 🔍 Vérifier que la Base de Données est Créée

### Méthode 1 : Via SQL Server Management Studio (SSMS)

1. Ouvrez SSMS
2. Connectez-vous à votre instance SQL Server
3. Développez "Databases"
4. Vérifiez que `HashiPrisonBreak` apparaît dans la liste
5. Développez la base de données et vérifiez que les tables existent :
   - `Users`
   - `Sessions`
   - `Puzzles`
   - `Islands`
   - `Bridges`
   - `Games`

### Méthode 2 : Via SQL Command

```powershell
sqlcmd -S localhost\SQLEXPRESS -E -Q "SELECT name FROM sys.databases WHERE name = 'HashiPrisonBreak'"
```

Si la base existe, son nom sera affiché.

### Méthode 3 : Utiliser Swagger

1. Lancez l'application : `dotnet run`
2. Ouvrez : https://localhost:5001/swagger
3. Testez `GET /api/puzzles` - si ça fonctionne, la DB est créée et accessible

### Méthode 4 : Vérifier les Logs

Au démarrage, vous devriez voir dans les logs :

```
Application des migrations de base de données...
Migrations appliquées avec succès.
```

Si vous voyez une erreur de connexion, vérifiez votre chaîne de connexion dans `appsettings.json`.

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

⚠️ **Important :** La base de données SQL Server peut être **partagée** ou **isolée** selon votre configuration.

### Option A : Base de Données Partagée (Équipe)

- Tous les développeurs se connectent à la même base de données
- Les données sont partagées entre tous
- Utile pour tester ensemble, mais attention aux conflits

**Configuration :**
- Utilisez un serveur SQL Server centralisé
- Tous les développeurs utilisent la même chaîne de connexion

### Option B : Base de Données Isolée (Recommandé pour le développement)

- Chaque développeur a sa propre base de données
- Les données ne sont **pas partagées** entre développeurs
- Évite les conflits et permet de tester librement

**Configuration :**
- Chaque développeur crée sa propre base de données (ex: `HashiPrisonBreak_Jean`, `HashiPrisonBreak_Marie`)
- Chaque développeur configure sa propre chaîne de connexion dans `appsettings.Development.json`

**Exemple pour un développeur :**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=HashiPrisonBreak_Jean;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Note :** `appsettings.Development.json` n'est généralement pas commité dans Git, donc chaque développeur peut avoir sa propre configuration.

---

## 🐛 Problèmes Courants

### Erreur : "Cannot open database" ou "Login failed"

**Causes possibles :**
- Instance SQL Server non démarrée
- Nom d'instance incorrect dans la chaîne de connexion
- Authentification échouée

**Solutions :**

1. **Vérifier que SQL Server est démarré :**
   ```powershell
   # Vérifier les services SQL Server
   Get-Service | Where-Object {$_.Name -like "*SQL*"}
   
   # Démarrer SQL Server si nécessaire
   Start-Service MSSQLSERVER
   # Ou pour SQL Express
   Start-Service MSSQL$SQLEXPRESS
   ```

2. **Vérifier le nom de l'instance :**
   ```powershell
   # Lister les instances SQL Server installées
   Get-ItemProperty 'HKLM:\SOFTWARE\Microsoft\Microsoft SQL Server\Instance Names\SQL'
   ```

3. **Tester la connexion :**
   ```powershell
   sqlcmd -S localhost\SQLEXPRESS -E -Q "SELECT @@VERSION"
   ```

4. **Vérifier la chaîne de connexion** dans `appsettings.json`

### Erreur : "A network-related or instance-specific error occurred"

**Solution :**
1. Vérifiez que SQL Server Browser est démarré :
   ```powershell
   Start-Service SQLBrowser
   ```

2. Vérifiez que le port 1433 est accessible (pour les connexions réseau)

3. Pour LocalDB, utilisez : `Server=(localdb)\\mssqllocaldb`

### Erreur : "No migrations found"

**Solution :**
```powershell
cd prisonbreak/prisonbreak.Server
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Erreur : "The database already exists"

**Solution :**
La base de données existe déjà. Vous pouvez :
1. La supprimer et la recréer (voir Option 3 ci-dessus)
2. Ou simplement lancer l'application - les migrations seront appliquées automatiquement

### La base de données ne se crée pas

**Vérifications :**
1. Vérifiez que SQL Server est installé et démarré
2. Vérifiez la chaîne de connexion dans `appsettings.json`
3. Vérifiez que vous avez les permissions pour créer des bases de données
4. Vérifiez les logs de l'application pour les erreurs détaillées
5. Testez la connexion manuellement avec `sqlcmd`

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
- [ ] SQL Server installé et démarré
- [ ] Base de données `HashiPrisonBreak` créée
- [ ] Chaîne de connexion configurée dans `appsettings.json` ou `appsettings.Development.json`
- [ ] Package `Microsoft.EntityFrameworkCore.SqlServer` installé
- [ ] Backend lancé au moins une fois (`dotnet run` dans `prisonbreak.Server/`)
- [ ] Tables créées (vérifier avec SSMS ou `sqlcmd`)
- [ ] Swagger accessible (https://localhost:5001/swagger)
- [ ] Frontend accessible (http://localhost:5173)
- [ ] Premier utilisateur créé via l'interface

---

## 📚 Ressources

- **Documentation Entity Framework** : https://learn.microsoft.com/en-us/ef/core/
- **SQL Server Documentation** : https://learn.microsoft.com/en-us/sql/sql-server/
- **SQL Server Express** : https://www.microsoft.com/en-us/sql-server/sql-server-downloads
- **SQL Server Management Studio** : https://aka.ms/ssmsfullsetup
- **Guide de l'équipe** : `GUIDE_EQUIPE.md`

---

## 🎯 Résumé

### Configuration Initiale (Une seule fois)

1. **Installer SQL Server** (Express, LocalDB, ou Docker)
2. **Créer la base de données** `HashiPrisonBreak` (via SSMS ou SQL command)
3. **Configurer la chaîne de connexion** dans `appsettings.json` ou `appsettings.Development.json`
4. **Installer le package** `Microsoft.EntityFrameworkCore.SqlServer` (si nécessaire)

### Utilisation Quotidienne

1. **Vérifier que SQL Server est démarré**
2. **Lancer l'application** : `dotnet run` dans `prisonbreak.Server/`
3. **Les migrations sont appliquées automatiquement**
4. **Les utilisateurs sont sauvegardés** quand ils se connectent
5. **Tout est persistant** entre les redémarrages

**Une fois configuré, c'est aussi simple que SQLite !** 🚀

---

## 🔐 Sécurité et Bonnes Pratiques

### Pour le Développement

- Utilisez `Trusted_Connection=True` (authentification Windows)
- Utilisez `TrustServerCertificate=True` pour éviter les problèmes de certificat
- Créez une base de données par développeur pour éviter les conflits

### Pour la Production

- Utilisez un utilisateur SQL Server dédié avec des permissions limitées
- Ne commitez **jamais** les mots de passe dans `appsettings.json`
- Utilisez des **User Secrets** ou **Azure Key Vault** pour les secrets
- Configurez des sauvegardes régulières
- Utilisez des certificats SSL pour les connexions sécurisées

