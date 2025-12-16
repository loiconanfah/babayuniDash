# 🔧 Guide de Dépannage - Erreur 500

## 🐛 Problème : Erreur 500 lors de l'installation

Si votre développeur rencontre une **erreur 500** lors de l'installation du projet, voici les causes possibles et leurs solutions.

---

## ❌ Erreur : `InvalidOperationException: L'île à (X, Y) n'a aucun pont`

### Symptômes

```
System.InvalidOperationException: L'île à (3, 2) n'a aucun pont dans la solution. Toutes les îles doivent être connectées.
at prisonbreak.Server.Services.PuzzleService.CalculateRequiredBridges
at prisonbreak.Server.Services.PuzzleService.GeneratePuzzleAsync
at prisonbreak.Server.Services.PuzzleService.GetPuzzlesByDifficultyAsync
```

### Cause

Cette erreur se produit quand :
- Un puzzle est généré avec des îles **non connectées** par des ponts
- La logique de génération a créé des îles isolées
- La base de données contient des puzzles corrompus

### ✅ Solution

#### Option 1 : Supprimer la base de données (Recommandé)

```powershell
cd prisonbreak\prisonbreak.Server
Remove-Item hashi.db -ErrorAction SilentlyContinue
dotnet run
```

La base de données sera recréée automatiquement avec des puzzles valides.

#### Option 2 : Vérifier les migrations

```powershell
cd prisonbreak\prisonbreak.Server
dotnet ef database drop --force
dotnet ef database update
dotnet run
```

#### Option 3 : Nettoyer et reconstruire

```powershell
cd prisonbreak\prisonbreak.Server
dotnet clean
dotnet build
Remove-Item hashi.db -ErrorAction SilentlyContinue
dotnet run
```

---

## ❌ Erreur : `SQLite Error 1: 'table "Puzzles" already exists'`

### Symptômes

```
SQLite Error 1: 'table "Puzzles" already exists'
```

### Cause

- La base de données existe déjà mais les migrations ne sont pas à jour
- Conflit entre `EnsureCreated()` et les migrations

### ✅ Solution

```powershell
cd prisonbreak\prisonbreak.Server
Remove-Item hashi.db -ErrorAction SilentlyContinue
dotnet run
```

---

## ❌ Erreur : `Cannot open database file`

### Symptômes

```
Microsoft.Data.Sqlite.SqliteException: SQLite Error 14: 'unable to open database file'
```

### Cause

- Permissions insuffisantes sur le dossier
- Chemin de fichier incorrect
- Fichier verrouillé par un autre processus

### ✅ Solution

1. **Vérifier les permissions** :
   ```powershell
   cd prisonbreak\prisonbreak.Server
   # Vérifier que vous pouvez écrire dans le dossier
   Test-Path . -PathType Container
   ```

2. **Vérifier que le fichier n'est pas verrouillé** :
   ```powershell
   # Fermer tous les processus qui utilisent la base de données
   Get-Process | Where-Object {$_.Path -like "*prisonbreak*"} | Stop-Process -Force
   ```

3. **Supprimer et recréer** :
   ```powershell
   Remove-Item hashi.db -ErrorAction SilentlyContinue
   dotnet run
   ```

---

## ❌ Erreur : `No migrations found`

### Symptômes

```
No migrations found. Ensure that the migrations have been added.
```

### Cause

- Les migrations Entity Framework n'ont pas été créées
- Le dossier `Migrations/` est manquant

### ✅ Solution

```powershell
cd prisonbreak\prisonbreak.Server

# Créer les migrations si elles n'existent pas
dotnet ef migrations add InitialCreate

# Appliquer les migrations
dotnet ef database update

# Relancer
dotnet run
```

---

## ❌ Erreur : `Connection string is null`

### Symptômes

```
System.ArgumentNullException: Connection string is null
```

### Cause

- Le fichier `appsettings.json` est manquant ou corrompu
- La chaîne de connexion n'est pas définie

### ✅ Solution

1. **Vérifier que `appsettings.json` existe** :
   ```powershell
   cd prisonbreak\prisonbreak.Server
   Test-Path appsettings.json
   ```

2. **Vérifier le contenu** :
   Le fichier doit contenir :
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=hashi.db"
     }
   }
   ```

3. **Recréer si nécessaire** :
   ```powershell
   # Copier depuis appsettings.Development.json si disponible
   Copy-Item appsettings.Development.json appsettings.json
   ```

---

## ❌ Erreur : `Service not registered`

### Symptômes

```
System.InvalidOperationException: Unable to resolve service for type 'prisonbreak.Server.Services.IPuzzleService'
```

### Cause

- Un service n'est pas enregistré dans `Program.cs`
- Erreur de configuration des dépendances

### ✅ Solution

1. **Vérifier `Program.cs`** :
   Assurez-vous que tous les services sont enregistrés :
   ```csharp
   builder.Services.AddScoped<IPuzzleService, PuzzleService>();
   builder.Services.AddScoped<IGameService, GameService>();
   // etc.
   ```

2. **Nettoyer et reconstruire** :
   ```powershell
   cd prisonbreak\prisonbreak.Server
   dotnet clean
   dotnet restore
   dotnet build
   dotnet run
   ```

---

## 🔍 Diagnostic Général

### Checklist de Vérification

Avant de signaler une erreur, vérifiez :

- [ ] **Base de données** : Le fichier `hashi.db` existe-t-il ?
- [ ] **Migrations** : Les migrations sont-elles appliquées ?
- [ ] **Configuration** : `appsettings.json` est-il présent et valide ?
- [ ] **Dépendances** : `dotnet restore` a-t-il été exécuté ?
- [ ] **Build** : `dotnet build` réussit-il sans erreur ?
- [ ] **Logs** : Quels sont les messages d'erreur exacts dans la console ?

### Commandes de Diagnostic

```powershell
cd prisonbreak\prisonbreak.Server

# 1. Vérifier la base de données
Test-Path hashi.db

# 2. Vérifier les migrations
dotnet ef migrations list

# 3. Vérifier la configuration
Get-Content appsettings.json

# 4. Nettoyer et reconstruire
dotnet clean
dotnet restore
dotnet build

# 5. Vérifier les logs au démarrage
dotnet run
```

---

## 🚀 Solution Rapide (Recommandée)

Si vous ne savez pas quelle est la cause exacte, utilisez cette solution complète :

```powershell
# 1. Aller dans le dossier du serveur
cd prisonbreak\prisonbreak.Server

# 2. Arrêter tous les processus
Get-Process | Where-Object {$_.Path -like "*prisonbreak*"} | Stop-Process -Force -ErrorAction SilentlyContinue

# 3. Supprimer la base de données
Remove-Item hashi.db -ErrorAction SilentlyContinue
Remove-Item hashi.db-shm -ErrorAction SilentlyContinue
Remove-Item hashi.db-wal -ErrorAction SilentlyContinue

# 4. Nettoyer le projet
dotnet clean

# 5. Restaurer les dépendances
dotnet restore

# 6. Reconstruire
dotnet build

# 7. Relancer (la base de données sera recréée automatiquement)
dotnet run
```

---

## 📝 Logs Utiles

### Activer les logs détaillés

Dans `appsettings.Development.json`, ajoutez :

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information",
      "prisonbreak.Server": "Debug"
    }
  }
}
```

Cela affichera plus de détails sur les erreurs dans la console.

---

## 🆘 Si Rien Ne Fonctionne

1. **Vérifier la version de .NET** :
   ```powershell
   dotnet --version
   # Doit être 8.0 ou supérieur
   ```

2. **Vérifier les dépendances** :
   ```powershell
   dotnet restore
   dotnet list package
   ```

3. **Créer un nouveau projet de test** :
   ```powershell
   dotnet new webapi -n TestProject
   cd TestProject
   dotnet run
   # Si ça fonctionne, le problème est spécifique au projet
   ```

4. **Contacter l'équipe** avec :
   - Les logs complets de l'erreur
   - La version de .NET (`dotnet --version`)
   - Le système d'exploitation
   - Les étapes exactes pour reproduire l'erreur

---

## ✅ Vérification Finale

Après avoir appliqué une solution, vérifiez que tout fonctionne :

1. **Backend démarre** :
   ```powershell
   dotnet run
   # Doit afficher : "Now listening on: https://localhost:5001"
   ```

2. **Swagger accessible** :
   Ouvrez : https://localhost:5001/swagger

3. **API fonctionne** :
   Testez : `GET /api/puzzles/difficulty/1`
   - Doit retourner une liste de puzzles (pas d'erreur 500)

4. **Base de données créée** :
   ```powershell
   Test-Path hashi.db
   # Doit retourner True
   ```

---

## 📚 Ressources

- **Guide d'initialisation** : `GUIDE_INITIALISATION_BD.md`
- **Guide de l'équipe** : `GUIDE_EQUIPE.md`
- **Documentation Entity Framework** : https://learn.microsoft.com/en-us/ef/core/

---

## 🎯 Résumé

**Erreur 500 la plus courante** : Îles non connectées dans les puzzles générés

**Solution la plus rapide** :
```powershell
cd prisonbreak\prisonbreak.Server
Remove-Item hashi.db -ErrorAction SilentlyContinue
dotnet run
```

La base de données sera recréée automatiquement avec des puzzles valides ! 🚀

