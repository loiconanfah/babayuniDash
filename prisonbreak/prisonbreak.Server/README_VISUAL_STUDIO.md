# 🚀 Configuration Visual Studio - Lancement Automatique

## ✅ Configuration Actuelle

Le projet est configuré pour que Visual Studio lance **automatiquement** le backend ET le frontend simultanément.

### Comment ça fonctionne ?

1. **SPA Proxy** : Visual Studio utilise le package `Microsoft.AspNetCore.SpaProxy` pour lancer automatiquement le frontend
2. **Configuration dans .csproj** : Les propriétés `SpaRoot`, `SpaProxyServerUrl` et `SpaProxyLaunchCommand` sont configurées
3. **Launch Settings** : Le profil "https" a `ASPNETCORE_HOSTINGSTARTUPASSEMBLIES` activé

---

## 🎯 Utilisation

### Lancer avec Visual Studio

1. **Ouvrir la solution** : `prisonbreak/prisonbreak.sln`
2. **Sélectionner le profil** : Choisir **"https"** dans la liste déroulante
3. **Appuyer sur F5** ou cliquer sur ▶️

**Visual Studio va automatiquement :**
- ✅ Compiler le backend
- ✅ Lancer le serveur ASP.NET Core sur `https://localhost:5001`
- ✅ Lancer `npm run dev` dans le dossier `frontend/`
- ✅ Démarrer Vite sur `http://localhost:5173`
- ✅ Configurer le proxy pour rediriger les requêtes

### Accès à l'Application

Une fois lancé, vous pouvez accéder à :
- **Application** : `https://localhost:5001` (le proxy redirige vers le frontend)
- **Frontend direct** : `http://localhost:5173`
- **Swagger** : `https://localhost:5001/swagger`

---

## 🔧 Configuration Technique

### Fichier .csproj

```xml
<SpaRoot>..\..\frontend\</SpaRoot>
<SpaProxyServerUrl>http://localhost:5173</SpaProxyServerUrl>
<SpaProxyLaunchCommand>npm run dev</SpaProxyLaunchCommand>
<SpaProxyWorkingDirectory>..\..\frontend</SpaProxyWorkingDirectory>
```

### launchSettings.json

Le profil "https" inclut :
```json
"ASPNETCORE_HOSTINGSTARTUPASSEMBLIES": "Microsoft.AspNetCore.SpaProxy"
```

### Frontend API Configuration

Le frontend utilise une URL relative `/api` qui est automatiquement proxyfiée vers le backend.

Fichier `.env` dans `frontend/` :
```
VITE_API_URL=/api
```

---

## ✅ Vérification

### Vérifier que tout fonctionne

1. **Lancer avec F5** dans Visual Studio
2. **Vérifier les consoles** :
   - Console Visual Studio : Backend démarré
   - Fenêtre Vite : Frontend démarré sur port 5173
3. **Ouvrir le navigateur** : `https://localhost:5001`
4. **Tester l'API** : Ouvrir la console du navigateur (F12) et vérifier qu'il n'y a pas d'erreurs CORS

### Si le frontend ne démarre pas automatiquement

1. Vérifier que Node.js est installé : `node --version`
2. Vérifier que npm est installé : `npm --version`
3. Installer les dépendances : `cd frontend && npm install`
4. Vérifier que le package `Microsoft.AspNetCore.SpaProxy` est installé

---

## 🐛 Dépannage

### Le frontend ne démarre pas

**Solution 1 :** Vérifier les dépendances
```powershell
cd frontend
npm install
```

**Solution 2 :** Lancer manuellement le frontend
```powershell
cd frontend
npm run dev
```

### Erreurs CORS

**Solution :** Vérifier que le CORS est bien configuré dans `Program.cs` et que l'ordre des middlewares est correct.

### Le proxy ne fonctionne pas

**Solution :** Vérifier que :
- Le port 5173 n'est pas utilisé par un autre processus
- Le package `Microsoft.AspNetCore.SpaProxy` est bien référencé dans le .csproj
- Le profil "https" est sélectionné

---

## 📝 Notes

- Le SPA Proxy fonctionne uniquement en mode **Development**
- En production, le frontend sera compilé et servi directement par le backend
- Les requêtes `/api/*` sont automatiquement proxyfiées vers le backend
- Les autres requêtes sont proxyfiées vers Vite (pour le Hot Module Replacement)

---

**Configuration prête ! Lancez simplement avec F5 dans Visual Studio** 🚀

