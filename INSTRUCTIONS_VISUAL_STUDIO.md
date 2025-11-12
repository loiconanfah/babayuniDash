# 🎯 Instructions pour Lancer le Projet dans Visual Studio

Ce document explique comment lancer complètement le projet Hashi (backend + client Vue.js) depuis Visual Studio.

---

## ✅ Prérequis

Avant de commencer, assurez-vous d'avoir :

- ✅ **Visual Studio 2022** (ou plus récent)
- ✅ **Node.js** 20.19+ ou 22.12+ installé
- ✅ **npm** installé (vient avec Node.js)

### Vérification des Prérequis

Ouvrez PowerShell et exécutez :

```powershell
node --version
npm --version
```

Si vous voyez les versions, c'est bon ! ✅

---

## 📦 Étape 1 : Installer les Dépendances npm

**IMPORTANT** : Avant de lancer dans Visual Studio, installez les dépendances npm :

```powershell
cd prisonbreak\prisonbreak.client
npm install
```

Attendez que l'installation se termine (ça peut prendre 1-2 minutes).

---

## 🚀 Étape 2 : Lancer depuis Visual Studio

### Option A : Lancement Automatique (Recommandé) 🌟

1. **Ouvrez la solution** dans Visual Studio :
   - Double-cliquez sur le fichier `.sln` à la racine
   - OU Fichier → Ouvrir → Projet/Solution

2. **Sélectionnez le profil de démarrage** :
   - En haut, à côté du bouton vert de démarrage
   - Sélectionnez **`https`** (PAS `http` ou `IIS Express`)

3. **Cliquez sur le bouton de démarrage** (▶️ ou F5)

4. **Que va-t-il se passer ?** :
   - ✅ Visual Studio va compiler le backend
   - ✅ Visual Studio va lancer automatiquement `npm run dev` dans le dossier client
   - ✅ Une fenêtre de terminal va s'ouvrir pour Vite
   - ✅ Le navigateur va s'ouvrir automatiquement sur `https://localhost:5001`

---

## 🔍 Vérifications

### Dans la Console de Visual Studio

Vous devriez voir :

```
Now listening on: https://localhost:5001
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
Hosting environment: Development
```

### Dans le Navigateur

Le navigateur devrait s'ouvrir et afficher :

- **Page principale** : Interface Vue.js avec le titre "🌉 Hashi"
- **Bouton** : "✨ Générer un Puzzle Test"

### Test de Fonctionnement

1. Cliquez sur **"Générer un Puzzle Test"**
2. Vous devriez voir une alerte : "Puzzle généré avec succès !"
3. Le puzzle apparaît dans la liste en dessous

✅ **Si ça fonctionne = TOUT EST BON !** 🎉

---

## 🐛 Problèmes Courants

### ❌ "npm n'est pas reconnu"

**Problème** : Node.js n'est pas installé ou pas dans le PATH

**Solution** :
1. Téléchargez Node.js : https://nodejs.org/
2. Installez-le
3. Redémarrez Visual Studio
4. Réessayez

---

### ❌ "Cannot find module 'vite'"

**Problème** : Les dépendances npm ne sont pas installées

**Solution** :
```powershell
cd prisonbreak\prisonbreak.client
npm install
```

---

### ❌ "Port 5173 already in use"

**Problème** : Le port est déjà utilisé

**Solution** :
```powershell
# Trouver le processus
netstat -ano | findstr :5173

# Tuer le processus (remplacez <PID> par le numéro)
taskkill /PID <PID> /F
```

---

### ❌ Le navigateur affiche "Cannot GET /"

**Problème** : Vite n'a pas démarré correctement

**Solution** :
1. Regardez la console de Visual Studio
2. Cherchez les erreurs dans le terminal Vite
3. Arrêtez (Shift+F5) et relancez (F5)

---

### ❌ Erreur de certificat HTTPS

**Problème** : Certificat de développement non approuvé

**Solution** :
```powershell
dotnet dev-certs https --trust
```

Cliquez sur "Oui" dans la boîte de dialogue.

---

## 📊 Structure du Démarrage

Quand vous lancez depuis Visual Studio :

```
Visual Studio (F5)
    │
    ├─► Backend (prisonbreak.Server)
    │   ├─ Compile C#
    │   ├─ Démarre sur port 5001 (HTTPS)
    │   └─ Démarre sur port 5000 (HTTP)
    │
    └─► Client (prisonbreak.client)
        ├─ Exécute: npm run dev
        ├─ Démarre Vite sur port 5173
        └─ Proxy les appels /api vers le backend
```

---

## 🌐 URLs Importantes

Après le démarrage :

| Service | URL | Description |
|---------|-----|-------------|
| **Application** | https://localhost:5001 | Page principale (Vue.js) |
| **API Swagger** | https://localhost:5001/swagger | Documentation API interactive |
| **Vite Dev Server** | http://localhost:5173 | Serveur de développement Vue.js |
| **API Directe** | https://localhost:5001/api | Endpoints de l'API |

---

## 🎯 Workflow de Développement

### Modifier le Code Backend (C#)

1. Modifiez les fichiers `.cs`
2. Visual Studio recompile automatiquement
3. Rechargez la page pour voir les changements

### Modifier le Code Frontend (Vue.js)

1. Modifiez les fichiers `.vue` ou `.js` dans `prisonbreak.client/src/`
2. Vite recharge automatiquement (Hot Module Replacement)
3. La page se rafraîchit automatiquement ! ⚡

---

## 🛑 Arrêter l'Application

Dans Visual Studio :

- **Shift + F5** : Arrête le débogage
- OU cliquez sur le bouton rouge ⏹️ "Arrêter le débogage"

Cela va arrêter :
- Le backend ASP.NET Core
- Le serveur Vite (automatiquement)

---

## 📝 Notes Importantes

### Base de Données

- La base de données SQLite (`hashi.db`) est créée automatiquement
- Elle se trouve dans : `prisonbreak\prisonbreak.Server\bin\Debug\net8.0\`
- Pour réinitialiser : supprimez le fichier `hashi.db` et relancez

### Dossier `frontend/`

Le dossier `frontend/` à la racine **N'EST PAS UTILISÉ** dans cette configuration.
Seul `prisonbreak/prisonbreak.client/` est utilisé par Visual Studio.

### Hot Reload

- **Backend** : Rechargement manuel (F5)
- **Frontend** : Hot reload automatique ⚡ (aucune action nécessaire)

---

## 🔧 Configuration Avancée

### Changer le Port de Vite

Modifiez dans `prisonbreak.client/vite.config.js` :

```javascript
server: {
    port: 5173, // Changez ici
    // ...
}
```

### Changer le Port du Backend

Modifiez dans `prisonbreak.Server/Properties/launchSettings.json` :

```json
"applicationUrl": "https://localhost:5001;http://localhost:5000"
```

---

## ✅ Checklist de Vérification

Avant de commencer à développer :

- [ ] Node.js et npm installés et fonctionnels
- [ ] Dépendances npm installées (`npm install`)
- [ ] Projet ouvert dans Visual Studio
- [ ] Profil de démarrage "https" sélectionné
- [ ] Backend démarre sans erreur (port 5001)
- [ ] Client démarre sans erreur (port 5173)
- [ ] Page s'affiche dans le navigateur
- [ ] Test de génération de puzzle fonctionne
- [ ] Swagger accessible sur /swagger

---

## 🆘 Besoin d'Aide ?

Si rien ne fonctionne :

1. **Vérifiez les logs** dans la console de Visual Studio
2. **Vérifiez le terminal Vite** qui s'ouvre
3. **Consultez** `START.md` pour d'autres solutions
4. **Ouvrez une issue** sur GitHub

---

## 🎉 Prêt à Développer !

Une fois que tout fonctionne, vous êtes prêt à développer !

**Prochaines étapes** :
- Explorez le code dans `prisonbreak.Server/` (backend)
- Explorez le code dans `prisonbreak.client/src/` (frontend)
- Testez l'API avec Swagger
- Créez de nouvelles fonctionnalités !

**Bon développement ! 🚀**

