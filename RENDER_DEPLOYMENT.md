# 🚀 Guide de Déploiement sur Render

## 📋 Configuration pour Render

Votre projet a besoin de **2 services** sur Render :
1. **Backend** (ASP.NET Core) - Service Web
2. **Frontend** (Vue.js) - Service Web Statique ou Service Web

---

## 🔧 Service 1 : Backend (ASP.NET Core)

### Configuration dans Render :

**Source Code :**
- Repository : `loiconanfah/babayuniDash`
- Branche : `puzzul` (ou `main`)

**Configuration :**
- **Nom** : `babayuniDash-backend`
- **Langue** : `Docker`
- **Branche** : `puzzul`
- **Région** : `Virginia (US East)` ou votre région préférée
- **Répertoire racine** : `prisonbreak/prisonbreak.Server`
- **Chemin Dockerfile** : `Dockerfile` (relatif au répertoire racine)
- **Type d'instance** : `Free` (pour commencer) ou `Starter` ($7/mois)

**Variables d'environnement :**
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:10000
PORT=10000
```

**Commande de build (optionnel) :**
```
dotnet restore && dotnet build
```

**Commande de démarrage (optionnel) :**
```
dotnet run --urls http://0.0.0.0:$PORT
```

---

## 🎨 Service 2 : Frontend (Vue.js)

### Option A : Service Web Statique (Recommandé)

**Configuration :**
- **Type de service** : `Static Site`
- **Nom** : `babayuniDash-frontend`
- **Repository** : `loiconanfah/babayuniDash`
- **Branche** : `puzzul`
- **Répertoire racine** : `frontend`
- **Commande de build** : `npm install && npm run build`
- **Répertoire de publication** : `dist`

**Variables d'environnement :**
```
VITE_API_URL=https://babayuniDash-backend.onrender.com/api
```

### Option B : Service Web (Node.js)

**Configuration :**
- **Nom** : `babayuniDash-frontend`
- **Langue** : `Node`
- **Branche** : `puzzul`
- **Répertoire racine** : `frontend`
- **Type d'instance** : `Free`

**Commande de build :**
```
npm install && npm run build
```

**Commande de démarrage :**
```
npm run preview -- --host 0.0.0.0 --port $PORT
```

**Variables d'environnement :**
```
VITE_API_URL=https://babayuniDash-backend.onrender.com/api
PORT=10000
```

---

## 📝 Étapes de Configuration sur Render

### 1. Créer le Service Backend

1. Cliquez sur **"New"** → **"Web Service"**
2. Connectez votre dépôt GitHub : `loiconanfah/babayuniDash`
3. Configurez :
   - **Nom** : `babayuniDash-backend`
   - **Langue** : `Docker`
   - **Branche** : `puzzul`
   - **Répertoire racine** : `prisonbreak/prisonbreak.Server`
   - **Chemin Dockerfile** : `Dockerfile`
4. Ajoutez les variables d'environnement
5. Cliquez sur **"Create Web Service"**

### 2. Créer le Service Frontend

1. Cliquez sur **"New"** → **"Static Site"** (ou **"Web Service"**)
2. Sélectionnez le même dépôt
3. Configurez :
   - **Nom** : `babayuniDash-frontend`
   - **Branche** : `puzzul`
   - **Répertoire racine** : `frontend`
   - **Commande de build** : `npm install && npm run build`
   - **Répertoire de publication** : `dist`
4. Ajoutez la variable d'environnement `VITE_API_URL` avec l'URL du backend
5. Cliquez sur **"Create Static Site"**

---

## ⚙️ Variables d'Environnement Importantes

### Backend :
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:10000
PORT=10000
```

### Frontend :
```
VITE_API_URL=https://VOTRE_BACKEND_URL.onrender.com/api
```

**⚠️ Important :** Remplacez `VOTRE_BACKEND_URL` par l'URL réelle du backend une fois déployé.

---

## 🔗 Configuration CORS

Le backend doit autoriser l'URL du frontend Render. Vérifiez que `Program.cs` autorise les domaines Render :

```csharp
// Dans Program.cs, ajoutez les URLs Render aux origines autorisées
var allowedOrigins = new List<string>
{
    "https://babayuniDash-frontend.onrender.com",
    // ... autres origines
};
```

---

## 📦 Vérification des Dockerfiles

Assurez-vous que les Dockerfiles sont correctement configurés :

### Backend Dockerfile (`prisonbreak/prisonbreak.Server/Dockerfile`)
- Doit exposer le port configuré (généralement 10000 pour Render)
- Doit utiliser `0.0.0.0` comme host

### Frontend (si Service Web)
- Doit servir les fichiers statiques depuis `dist/`
- Doit écouter sur `0.0.0.0:$PORT`

---

## 🚀 Après le Déploiement

1. **Notez l'URL du backend** (ex: `https://babayuniDash-backend.onrender.com`)
2. **Mettez à jour la variable d'environnement** du frontend :
   ```
   VITE_API_URL=https://babayuniDash-backend.onrender.com/api
   ```
3. **Redéployez le frontend** pour appliquer la nouvelle URL

---

## 🆘 Dépannage

### Le backend ne démarre pas
- Vérifiez les logs dans Render
- Assurez-vous que le port est configuré correctement
- Vérifiez que la base de données SQLite est accessible

### Le frontend ne peut pas accéder au backend
- Vérifiez la variable `VITE_API_URL`
- Vérifiez la configuration CORS dans le backend
- Vérifiez que le backend est bien démarré

### Erreur de build
- Vérifiez que toutes les dépendances sont dans les fichiers de projet
- Vérifiez les logs de build dans Render

---

## 💡 Recommandations

1. **Commencer avec Free** : Testez avec les instances gratuites
2. **Upgrade progressif** : Passez à `Starter` ($7/mois) si nécessaire
3. **Base de données** : Pour la production, considérez PostgreSQL au lieu de SQLite
4. **Variables d'environnement** : Ne commitez jamais les secrets dans le code

---

**🎉 Une fois configuré, votre application sera accessible en ligne !**



