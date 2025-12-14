# 🔧 Dépannage ngrok - Limite de Sessions

## ❌ Erreur : "Your account is limited to 1 simultaneous ngrok agent sessions"

Cette erreur signifie qu'une session ngrok est déjà active. Avec le plan gratuit, vous ne pouvez avoir qu'**une seule session** à la fois.

---

## ✅ Solution 1 : Arrêter les Sessions Existantes

### Méthode A : Via l'Interface Web ngrok

1. Ouvrez http://localhost:4040 dans votre navigateur
2. Cliquez sur **"Stop"** pour arrêter la session active
3. Relancez ngrok

### Méthode B : Via le Terminal

#### Windows (PowerShell)
```powershell
# Trouver et arrêter les processus ngrok
Get-Process ngrok | Stop-Process -Force
```

#### Windows (CMD)
```cmd
taskkill /F /IM ngrok.exe
```

#### Vérifier qu'aucun processus ngrok n'est actif
```powershell
Get-Process ngrok -ErrorAction SilentlyContinue
# Si rien ne s'affiche, c'est bon !
```

---

## ✅ Solution 2 : Utiliser un Seul Tunnel (Recommandé)

Comme le frontend redirige automatiquement les requêtes `/api/*` vers le backend, **un seul tunnel suffit** :

```bash
# Un seul tunnel pour le frontend (port 5173)
ngrok http 5173
```

Le proxy Vite redirige automatiquement :
- `/api/*` → `http://localhost:5000/api/*`
- `/hubs/*` → `http://localhost:5000/hubs/*`
- `/uploads/*` → `http://localhost:5000/uploads/*`

**Pas besoin de tunnel séparé pour le backend !**

---

## ✅ Solution 3 : Configuration d'Agent (Plusieurs Tunnels)

Si vous avez vraiment besoin de plusieurs tunnels, configurez un fichier de configuration :

### Étape 1 : Créer le fichier de configuration

Créez un fichier `ngrok.yml` dans votre dossier utilisateur (`C:\Users\VotreNom\AppData\Local\ngrok\ngrok.yml`) :

```yaml
version: "2"
authtoken: VOTRE_TOKEN_NGROK  # Récupérez-le depuis https://dashboard.ngrok.com/get-started/your-authtoken

tunnels:
  frontend:
    addr: 5173
    proto: http
    
  backend:
    addr: 5000
    proto: http
```

### Étape 2 : Lancer tous les tunnels

```bash
ngrok start --all
```

### Étape 3 : Configurer le frontend

Si vous utilisez des tunnels séparés, créez un fichier `.env` dans `frontend/` :

```env
VITE_API_URL=https://URL_BACKEND_NGROK/api
```

---

## 🎯 Solution Recommandée (Simple)

**Utilisez un seul tunnel pour le frontend :**

```bash
# 1. Arrêter toutes les sessions ngrok existantes
taskkill /F /IM ngrok.exe

# 2. Lancer le backend (Terminal 1)
cd prisonbreak/prisonbreak.Server
dotnet run

# 3. Lancer le frontend (Terminal 2)
cd frontend
npm run dev

# 4. Lancer ngrok (Terminal 3)
ngrok http 5173
```

**C'est tout !** Le proxy Vite gère automatiquement la redirection vers le backend.

---

## 🔍 Vérifier les Sessions Actives

### Via le Dashboard ngrok
1. Allez sur https://dashboard.ngrok.com/agents
2. Voir toutes les sessions actives
3. Cliquez sur "Stop" pour arrêter une session

### Via l'Interface Web Locale
1. Ouvrez http://localhost:4040
2. Voir les tunnels actifs
3. Cliquez sur "Stop" pour arrêter

---

## 📝 Script PowerShell pour Arrêter ngrok

Créez un fichier `stop-ngrok.ps1` :

```powershell
# Arrêter tous les processus ngrok
Write-Host "🛑 Arrêt de toutes les sessions ngrok..." -ForegroundColor Yellow

$processes = Get-Process ngrok -ErrorAction SilentlyContinue
if ($processes) {
    $processes | Stop-Process -Force
    Write-Host "✅ Sessions ngrok arrêtées" -ForegroundColor Green
} else {
    Write-Host "ℹ️  Aucune session ngrok active" -ForegroundColor Gray
}
```

Utilisation :
```powershell
.\stop-ngrok.ps1
```

---

## 🚀 Commandes Rapides

### Arrêter et Relancer ngrok
```powershell
# Arrêter
taskkill /F /IM ngrok.exe

# Attendre 2 secondes
Start-Sleep -Seconds 2

# Relancer
ngrok http 5173
```

---

## ⚠️ Notes Importantes

1. **Plan Gratuit** : Limite de 1 session simultanée
2. **Plan Payant** : Permet plusieurs sessions simultanées
3. **Un seul tunnel suffit** : Le proxy Vite gère la redirection vers le backend

---

## 🆘 Si le Problème Persiste

1. Vérifiez qu'aucun processus ngrok n'est actif :
   ```powershell
   Get-Process ngrok -ErrorAction SilentlyContinue
   ```

2. Vérifiez les sessions dans le dashboard :
   https://dashboard.ngrok.com/agents

3. Redémarrez votre ordinateur si nécessaire (solution de dernier recours)

---

## ✅ Vérification Finale

Après avoir arrêté les sessions existantes, lancez :

```bash
ngrok http 5173
```

Vous devriez voir :
```
Session Status                online
Account                       [votre compte]
Version                       [version]
Region                        [région]
Forwarding                    https://xxxxx.ngrok-free.app -> http://localhost:5173
```

**Si vous voyez cela, c'est bon ! 🎉**



