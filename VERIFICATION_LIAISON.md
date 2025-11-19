# ✅ Vérification de la Liaison Frontend/Backend

## 🔧 Modifications Effectuées

### 1. Configuration CORS (Program.cs)
- ✅ Ajout des origines `http://localhost:5000` et `https://localhost:5001`
- ✅ Réorganisation de l'ordre des middlewares (CORS après HTTPS redirection)

### 2. Configuration API Frontend (api.ts)
- ✅ Utilisation d'URL relative `/api` par défaut en développement
- ✅ Compatible avec le SPA Proxy de Visual Studio
- ✅ Fallback vers URL absolue si nécessaire

### 3. Configuration Visual Studio
- ✅ `SpaProxyWorkingDirectory` ajouté dans .csproj
- ✅ `ASPNETCORE_HOSTINGSTARTUPASSEMBLIES` activé dans launchSettings.json (profil "https")

---

## 🚀 Comment Lancer avec Visual Studio

### Étapes

1. **Ouvrir la solution** : `prisonbreak/prisonbreak.sln`
2. **Sélectionner le profil** : Choisir **"https"** dans la liste déroulante (en haut)
3. **Appuyer sur F5** ou cliquer sur ▶️

### Ce qui se passe automatiquement

✅ Visual Studio compile le backend  
✅ Visual Studio lance le serveur ASP.NET Core sur `https://localhost:5001`  
✅ Visual Studio lance automatiquement `npm run dev` dans `frontend/`  
✅ Vite démarre sur `http://localhost:5173`  
✅ Le SPA Proxy redirige les requêtes `/api/*` vers le backend  
✅ Le SPA Proxy redirige les autres requêtes vers Vite  

### Accès

- **Application** : `https://localhost:5001` (tout passe par le proxy)
- **Swagger** : `https://localhost:5001/swagger`
- **Frontend direct** : `http://localhost:5173` (si besoin)

---

## ✅ Vérification de la Liaison

### Test 1 : Vérifier que les deux démarrent

1. Lancer avec F5 dans Visual Studio
2. Vérifier dans la **console Visual Studio** : "Now listening on: https://localhost:5001"
3. Vérifier dans la **fenêtre Vite** (s'ouvre automatiquement) : "Local: http://localhost:5173"
4. Si les deux sont présents → ✅ **OK**

### Test 2 : Vérifier la communication

1. Ouvrir le navigateur sur `https://localhost:5001`
2. Ouvrir la console du navigateur (F12)
3. Aller dans l'onglet **Network**
4. Utiliser l'application (charger un puzzle, etc.)
5. Vérifier que les requêtes vers `/api/*` sont bien envoyées
6. Vérifier qu'il n'y a **pas d'erreurs CORS** → ✅ **OK**

### Test 3 : Tester l'API directement

1. Ouvrir Swagger : `https://localhost:5001/swagger`
2. Tester un endpoint (ex: `GET /api/puzzles`)
3. Vérifier que la réponse est correcte → ✅ **OK**

---

## 🔍 Détails Techniques

### Comment fonctionne le SPA Proxy ?

1. **Requêtes `/api/*`** → Redirigées vers le backend ASP.NET Core
2. **Autres requêtes** → Redirigées vers Vite (pour le HMR)
3. **Hot Module Replacement** → Fonctionne normalement

### Configuration Frontend

Le frontend utilise maintenant :
```typescript
const API_BASE_URL = import.meta.env.VITE_API_URL || 
  (import.meta.env.DEV ? '/api' : 'https://localhost:5001/api')
```

Cela signifie :
- En développement : Utilise `/api` (URL relative, proxyfiée)
- En production : Utilise l'URL absolue
- Variable d'environnement : Peut être surchargée avec `VITE_API_URL`

### Configuration Backend

Le CORS autorise :
- `http://localhost:5173` (Vite direct)
- `https://localhost:5173` (Vite avec HTTPS)
- `http://localhost:5000` (Backend HTTP)
- `https://localhost:5001` (Backend HTTPS)

---

## 🐛 Dépannage

### Le frontend ne démarre pas automatiquement

**Symptômes** : Seul le backend démarre

**Solutions** :
1. Vérifier que Node.js est installé : `node --version`
2. Installer les dépendances : `cd frontend && npm install`
3. Vérifier que le package `Microsoft.AspNetCore.SpaProxy` est installé
4. Relancer Visual Studio

### Erreurs CORS dans la console

**Symptômes** : Erreurs "CORS policy" dans la console du navigateur

**Solutions** :
1. Vérifier que le CORS est bien configuré dans `Program.cs`
2. Vérifier l'ordre des middlewares (CORS après HTTPS)
3. Vérifier que l'origine est bien autorisée

### Les requêtes API échouent

**Symptômes** : Erreurs 404 ou erreurs réseau

**Solutions** :
1. Vérifier que le backend tourne sur le port 5001
2. Vérifier que le SPA Proxy est activé
3. Vérifier l'URL dans `api.ts` (doit être `/api` en développement)
4. Tester directement avec Swagger

---

## ✅ Checklist de Vérification

Avant de considérer que tout fonctionne :

- [ ] Visual Studio lance les deux (backend + frontend) avec F5
- [ ] Le backend démarre sur `https://localhost:5001`
- [ ] Le frontend démarre sur `http://localhost:5173`
- [ ] L'application est accessible sur `https://localhost:5001`
- [ ] Swagger est accessible sur `https://localhost:5001/swagger`
- [ ] Pas d'erreurs CORS dans la console du navigateur
- [ ] Les requêtes API fonctionnent (test dans l'application)
- [ ] Le Hot Module Replacement fonctionne (modifier un fichier Vue et voir le changement)

---

## 📝 Notes Importantes

1. **Ne pas modifier le visuel du frontend** : Comme demandé, aucun changement visuel n'a été fait
2. **URL relative** : Le frontend utilise maintenant `/api` par défaut, ce qui fonctionne avec le proxy
3. **Configuration flexible** : Peut fonctionner avec ou sans SPA Proxy (URL absolue en fallback)

---

**La liaison frontend/backend est maintenant correctement configurée !** ✅

**Visual Studio lancera automatiquement les deux avec F5** 🚀

