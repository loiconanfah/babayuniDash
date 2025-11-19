# 🔧 Correction du Problème SPA Proxy

## 🐛 Problème Identifié

Le SPA Proxy ne peut pas démarrer le serveur de développement frontend :
```
Microsoft.AspNetCore.SpaProxy.SpaProxyLaunchManager: Error: Couldn't start the SPA development server with command 'npm run dev'.
```

## ✅ Solutions

### Solution 1 : Installer les Dépendances Frontend (OBLIGATOIRE)

**Le problème principal** : Les dépendances npm ne sont pas installées dans le dossier `frontend/`.

**Action requise :**
```powershell
cd frontend
npm install
```

Cela installera toutes les dépendances nécessaires (Vite, Vue, etc.)

### Solution 2 : Vérifier le Chemin

Le `SpaProxyWorkingDirectory` a été mis à jour pour utiliser un chemin absolu basé sur le répertoire du projet.

### Solution 3 : Lancer Manuellement (Alternative)

Si le SPA Proxy continue à échouer, vous pouvez lancer le frontend manuellement :

**Terminal 1 - Backend (Visual Studio) :**
- Lancer avec F5 (sans le SPA Proxy)

**Terminal 2 - Frontend (PowerShell) :**
```powershell
cd frontend
npm run dev
```

Puis accéder à `http://localhost:5173` directement.

---

## 🔍 Vérification

### Vérifier que les Dépendances sont Installées

```powershell
cd frontend
Test-Path node_modules
```

Doit retourner `True`.

### Vérifier que npm run dev fonctionne

```powershell
cd frontend
npm run dev
```

Doit démarrer Vite sans erreur.

---

## 📝 Configuration Actuelle

### .csproj
```xml
<SpaRoot>..\..\frontend\</SpaRoot>
<SpaProxyServerUrl>http://localhost:5173</SpaProxyServerUrl>
<SpaProxyLaunchCommand>npm run dev</SpaProxyLaunchCommand>
<SpaProxyWorkingDirectory>$(MSBuildProjectDirectory)\..\..\frontend</SpaProxyWorkingDirectory>
```

### launchSettings.json
Le profil "https" a :
```json
"ASPNETCORE_HOSTINGSTARTUPASSEMBLIES": "Microsoft.AspNetCore.SpaProxy"
```

---

## ✅ Étapes pour Corriger

1. **Installer les dépendances** :
   ```powershell
   cd frontend
   npm install
   ```

2. **Vérifier l'installation** :
   ```powershell
   Test-Path frontend\node_modules
   ```

3. **Relancer Visual Studio** avec F5

4. **Vérifier les logs** :
   - Console Visual Studio : Backend démarré
   - Fenêtre Vite : Frontend démarré (si le proxy fonctionne)

---

## 🚨 Si le Problème Persiste

### Option A : Désactiver Temporairement le SPA Proxy

Dans `launchSettings.json`, retirer `ASPNETCORE_HOSTINGSTARTUPASSEMBLIES` du profil "https" et lancer manuellement le frontend.

### Option B : Utiliser le Script PowerShell

Utiliser le script `start-dev.ps1` qui lance les deux manuellement.

---

**La cause principale est l'absence de `node_modules` dans le dossier `frontend/`** ⚠️

