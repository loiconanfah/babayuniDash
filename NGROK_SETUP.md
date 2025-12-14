# Configuration ngrok pour l'accès en ligne

Ce guide vous explique comment rendre votre plateforme accessible en ligne depuis votre machine locale en utilisant ngrok.

## 📋 Prérequis

1. **ngrok installé** : Téléchargez depuis [https://ngrok.com/download](https://ngrok.com/download)
   - Ou installez via Chocolatey : `choco install ngrok`
   - Ou via winget : `winget install ngrok`

2. **Compte ngrok** (gratuit) : Créez un compte sur [https://dashboard.ngrok.com](https://dashboard.ngrok.com)

3. **Token d'authentification** : Récupérez votre token sur [https://dashboard.ngrok.com/get-started/your-authtoken](https://dashboard.ngrok.com/get-started/your-authtoken)

## 🚀 Configuration rapide

### Étape 1 : Configurer ngrok

```powershell
# Configurer votre token ngrok
ngrok config add-authtoken VOTRE_TOKEN_ICI
```

### Étape 2 : Démarrer les services

1. **Démarrer le backend** :
   ```powershell
   cd prisonbreak\prisonbreak.Server
   dotnet run
   ```

2. **Démarrer le frontend** (dans un autre terminal) :
   ```powershell
   cd frontend
   npm run dev:port1
   ```

   Pour le multijoueur, démarrez aussi l'instance 2 :
   ```powershell
   npm run dev:port2
   ```

### Étape 3 : Configurer votre token ngrok

Éditez le fichier `ngrok-config.yml` et remplacez `YOUR_AUTH_TOKEN` par votre token ngrok.

Ou configurez-le directement :
```powershell
ngrok config add-authtoken VOTRE_TOKEN_ICI
```

### Étape 4 : Lancer ngrok

#### Option A : Utiliser le script pour plusieurs tunnels (recommandé)

```powershell
.\start-ngrok-multiple.ps1
```

Ce script démarre une instance ngrok séparée pour chaque service (backend, frontend 1, frontend 2). **C'est la méthode recommandée pour ngrok 3.x**.

#### Option B : Utiliser le script complet

```powershell
.\start-with-ngrok.ps1
```

Ou en double-cliquant sur `start-with-ngrok.bat`

Ce script vérifie automatiquement quels services sont en cours d'exécution et démarre les tunnels correspondants.

#### Option C : Configuration manuelle

**Pour le backend :**
```powershell
ngrok http 5000
```

**Pour le frontend (instance 1) :**
```powershell
ngrok http 5173
```

**Pour le frontend (instance 2) :**
```powershell
ngrok http 5174
```

> **Note** : Avec ngrok 3.x, chaque tunnel nécessite une instance séparée. Chaque instance utilise un port API différent (4040, 4041, 4042, etc.) pour son dashboard.

**Pour le backend :**
```powershell
ngrok http 5000
```

**Pour le frontend (instance 1) :**
```powershell
ngrok http 5173
```

**Pour le frontend (instance 2) :**
```powershell
ngrok http 5174
```

### Étape 5 : Récupérer les URLs

Une fois ngrok démarré, vous verrez les URLs publiques dans la console, par exemple :
- Backend : `https://abc123.ngrok-free.app` (Dashboard: http://localhost:4040)
- Frontend 1 : `https://def456.ngrok-free.app` (Dashboard: http://localhost:4041)
- Frontend 2 : `https://ghi789.ngrok-free.app` (Dashboard: http://localhost:4042)

> **Note** : Avec ngrok 3.x, chaque tunnel a son propre dashboard sur un port différent (4040, 4041, 4042, etc.)

Vous pouvez aussi consulter les dashboards ngrok directement dans votre navigateur.

## 🔧 Configuration avancée

### URLs ngrok statiques (plan payant)

Si vous avez un plan ngrok payant, vous pouvez utiliser des URLs statiques :

```powershell
ngrok http 5000 --domain=votre-domaine.ngrok.io
ngrok http 5173 --domain=votre-domaine-frontend.ngrok.io
```

### Configuration via variables d'environnement

Vous pouvez définir les URLs ngrok dans les variables d'environnement :

```powershell
$env:NGROK_URLS = "https://abc123.ngrok-free.app;https://def456.ngrok-free.app"
```

Puis redémarrer le backend.

## 📝 Notes importantes

1. **URLs temporaires** : Avec le plan gratuit, les URLs ngrok changent à chaque redémarrage
2. **Limites** : Le plan gratuit a des limites de connexions et de bande passante
3. **Sécurité** : Les URLs ngrok sont publiques, ne partagez que si nécessaire
4. **CORS** : Le backend est configuré pour accepter automatiquement les URLs ngrok en développement

## 🛠️ Dépannage

### ngrok ne démarre pas

- Vérifiez que ngrok est dans votre PATH
- Vérifiez votre token : `ngrok config check`
- Consultez les logs : `ngrok logs`

### Erreurs CORS

- Assurez-vous que le backend accepte les URLs ngrok (configuré automatiquement en développement)
- Vérifiez que vous utilisez bien l'URL ngrok du frontend dans votre navigateur

### Le frontend ne se connecte pas au backend

- Vérifiez que le proxy Vite pointe vers `localhost:5000` (pas vers l'URL ngrok)
- Le proxy Vite gère automatiquement la redirection vers le backend local
- Les requêtes API passent par le proxy, donc pas besoin de modifier les URLs dans le code

## 🔗 Ressources

- [Documentation ngrok](https://ngrok.com/docs)
- [Dashboard ngrok](https://dashboard.ngrok.com)
- [Guide ngrok pour les développeurs](https://ngrok.com/docs/getting-started)

