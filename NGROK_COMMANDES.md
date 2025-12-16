# 🚀 Commandes ngrok pour Exposer la Plateforme

## 📋 Prérequis

1. **ngrok installé** : Téléchargé depuis https://ngrok.com/download
2. **Backend lancé** : Sur `http://localhost:5000`
3. **Frontend lancé** : Sur `http://localhost:5173`

---

## 🔧 Commandes ngrok

### Option 1 : Tunnel pour le Frontend (Recommandé)

Le frontend utilise le proxy Vite qui redirige `/api/*` vers le backend, donc un seul tunnel suffit :

```bash
# Exposer le frontend (port 5173)
ngrok http 5173
```

**URLs générées :**
- URL publique : `https://xxxxx.ngrok-free.app` (à utiliser)
- URL locale : `http://localhost:5173`

**Accès :**
- Ouvrez l'URL ngrok dans votre navigateur
- Le frontend redirigera automatiquement les requêtes `/api/*` vers le backend local

---

### Option 2 : Tunnels Séparés (Frontend + Backend)

Si vous voulez exposer le backend séparément :

#### Terminal 1 : Tunnel Frontend
```bash
ngrok http 5173
```

#### Terminal 2 : Tunnel Backend
```bash
ngrok http 5000
```

**Configuration :**
1. Notez l'URL du backend ngrok (ex: `https://yyyyy.ngrok-free.app`)
2. Créez un fichier `.env` dans `frontend/` :
```env
VITE_API_URL=https://yyyyy.ngrok-free.app/api
```
3. Redémarrez le frontend

---

## 🎯 Configuration Recommandée (Option 1)

### Étape 1 : Lancer le Backend
```bash
cd prisonbreak/prisonbreak.Server
dotnet run
```
Le backend doit être accessible sur `http://localhost:5000`

### Étape 2 : Lancer le Frontend
```bash
cd frontend
npm run dev
```
Le frontend doit être accessible sur `http://localhost:5173`

### Étape 3 : Lancer ngrok
```bash
ngrok http 5173
```

### Étape 4 : Utiliser l'URL ngrok
- Copiez l'URL HTTPS générée (ex: `https://xxxxx.ngrok-free.app`)
- Ouvrez-la dans votre navigateur
- **C'est tout !** Le proxy Vite redirige automatiquement `/api/*` vers le backend local

---

## 🔍 Vérification

### Tester l'API via ngrok
```bash
# Tester l'endpoint API
curl https://xxxxx.ngrok-free.app/api/puzzles

# Ou dans le navigateur
https://xxxxx.ngrok-free.app/api/puzzles
```

### Vérifier les logs ngrok
- Ouvrez http://localhost:4040 dans votre navigateur
- Vous verrez toutes les requêtes passant par ngrok

---

## ⚠️ Notes Importantes

1. **URL ngrok change à chaque redémarrage** : L'URL change si vous relancez ngrok
2. **Version gratuite** : ngrok affiche un avertissement la première fois (cliquez sur "Visit Site")
3. **CORS** : Le backend est déjà configuré pour accepter les domaines ngrok
4. **WebSockets (SignalR)** : Fonctionnent automatiquement avec ngrok

---

## 🚀 Commandes Rapides

### Lancer tout en une fois (PowerShell)
```powershell
# Terminal 1 : Backend
cd prisonbreak/prisonbreak.Server
dotnet run

# Terminal 2 : Frontend
cd frontend
npm run dev

# Terminal 3 : ngrok
ngrok http 5173
```

---

## 📱 Accès Mobile

Une fois ngrok lancé :
1. Notez l'URL ngrok (ex: `https://xxxxx.ngrok-free.app`)
2. Ouvrez cette URL sur votre téléphone (même réseau WiFi ou données)
3. L'application devrait fonctionner normalement

---

## 🔐 Sécurité

⚠️ **Attention** : ngrok expose votre application localement sur Internet. 
- Ne partagez l'URL qu'avec des personnes de confiance
- Pour la production, utilisez un domaine personnalisé avec authentification ngrok

---

## 🆘 Dépannage

### Erreur "Blocked request"
- Vérifiez que `vite.config.ts` a `host: true` et `allowedHosts` configurés (déjà fait)

### Erreur CORS
- Vérifiez que le backend accepte les domaines ngrok (déjà configuré dans `Program.cs`)

### Les données ne se chargent pas
- Vérifiez que le backend est bien lancé sur `http://localhost:5000`
- Vérifiez que le frontend utilise des URLs relatives `/api/*` (déjà corrigé)

---

## 📝 Exemple Complet

```bash
# 1. Lancer le backend
cd prisonbreak/prisonbreak.Server
dotnet run
# ✅ Backend sur http://localhost:5000

# 2. Lancer le frontend (nouveau terminal)
cd frontend
npm run dev
# ✅ Frontend sur http://localhost:5173

# 3. Lancer ngrok (nouveau terminal)
ngrok http 5173
# ✅ URL publique : https://xxxxx.ngrok-free.app

# 4. Ouvrir dans le navigateur
# https://xxxxx.ngrok-free.app
```

---

**🎉 C'est tout ! Votre plateforme est maintenant accessible en ligne !**

