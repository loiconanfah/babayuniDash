# 🚀 Comment Lancer le Projet dans Visual Studio

Guide rapide pour lancer le projet Hashi dans Visual Studio.

---

## ⚡ Démarrage Rapide

### 1. Installer les Dépendances npm (Une seule fois)

Ouvrez PowerShell dans le dossier `prisonbreak` :

```powershell
cd prisonbreak.client
npm install
cd ..
```

### 2. Ouvrir dans Visual Studio

**Double-cliquez sur :** `prisonbreak\prisonbreak.sln`

OU

**Dans Visual Studio :**
- Fichier → Ouvrir → Projet/Solution
- Naviguez vers `prisonbreak\prisonbreak.sln`
- Cliquez sur "Ouvrir"

### 3. Choisir le Profil de Démarrage

En haut de Visual Studio, à côté du bouton vert ▶️, vous verrez une liste déroulante.

**Trois options disponibles :**

#### Option 1 : **`https`** (Recommandé pour le développement) ⭐
- Lance le backend sur https://localhost:5001
- Lance automatiquement le client Vue.js
- Meilleure expérience de développement

#### Option 2 : **`http`**
- Lance le backend sur http://localhost:5000 (sans SSL)
- Lance automatiquement le client Vue.js
- Bon pour tester sans certificat

#### Option 3 : **`IIS Express`**
- Utilise IIS Express comme serveur
- Lance automatiquement le client Vue.js
- Configuration proche de la production

### 4. Lancer (F5)

Appuyez sur **F5** ou cliquez sur le bouton vert ▶️

**Visual Studio va :**
1. ✅ Restaurer les packages NuGet
2. ✅ Compiler le backend C#
3. ✅ Lancer le serveur ASP.NET Core
4. ✅ Exécuter `npm run dev` pour le client Vue.js
5. ✅ Ouvrir le navigateur automatiquement

---

## 🌐 URLs Après le Lancement

| Profil | URL Backend | URL Client | Description |
|--------|-------------|------------|-------------|
| **https** | https://localhost:5001 | https://localhost:5001 | HTTPS avec proxy SPA |
| **http** | http://localhost:5000 | http://localhost:5000 | HTTP avec proxy SPA |
| **IIS Express** | https://localhost:5001 | https://localhost:5001 | IIS Express |

**Swagger (documentation API) :** https://localhost:5001/swagger

---

## ✅ Ce que Vous Devriez Voir

### Dans le Navigateur

1. **Page d'accueil** avec le titre : "🌉 Hashi - Jeu de Puzzle"
2. **Bouton** : "✨ Générer un Puzzle Test"
3. **Liste de vérification** : Backend fonctionnel, Client connecté, etc.

### Test de Fonctionnement

1. Cliquez sur **"Générer un Puzzle Test"**
2. Une alerte apparaît : "Puzzle généré avec succès ! ID: X"
3. Le puzzle apparaît dans la liste

✅ **Si tout cela fonctionne = Configuration parfaite !** 🎉

---

## 🔍 Console de Visual Studio

Vous devriez voir dans la console :

```
Now listening on: https://localhost:5001
Now listening on: http://localhost:5000
Application started. Press Ctrl+C to shut down.
Hosting environment: Development
```

**Et dans le terminal Vite (une fenêtre séparée) :**

```
VITE v7.x.x ready in XXX ms
➜ Local: http://localhost:5173/
➜ press h to show help
```

---

## 🛑 Arrêter l'Application

Dans Visual Studio :
- **Shift + F5** : Arrête le débogage
- OU cliquez sur le carré rouge ⏹️

Cela arrête automatiquement :
- ✅ Le backend ASP.NET Core
- ✅ Le serveur Vite (client Vue.js)

---

## 🐛 Résolution de Problèmes

### ❌ "npm n'est pas reconnu"

**Solution :** Installez Node.js
- Téléchargez : https://nodejs.org/
- Installez la version LTS
- Redémarrez Visual Studio
- Réessayez

### ❌ "Cannot find module 'vite'"

**Solution :** Installez les dépendances
```powershell
cd prisonbreak\prisonbreak.client
npm install
```

### ❌ Erreur de certificat SSL

**Solution :** Faites confiance au certificat de développement
```powershell
dotnet dev-certs https --trust
```
Cliquez sur "Oui" dans la boîte de dialogue

### ❌ "Port 5001 is already in use"

**Solution :** Tuez le processus qui utilise le port
```powershell
# Trouver le processus
netstat -ano | findstr :5001

# Tuer le processus (remplacez <PID>)
taskkill /PID <PID> /F
```

### ❌ Le projet ne se charge pas dans Visual Studio

**Vérifiez :**
1. Vous avez ouvert `prisonbreak.sln` (pas un .csproj)
2. Visual Studio 2022 est installé
3. Le workload "ASP.NET et développement web" est installé

**Pour installer le workload :**
- Outils → Obtenir des outils et des fonctionnalités
- Cochez "ASP.NET et développement web"
- Cliquez sur "Modifier"

### ❌ Le client Vue.js ne démarre pas

**Vérifiez dans la console :**
- Cherchez les erreurs npm
- Vérifiez que Node.js est installé : `node --version`
- Vérifiez que npm est installé : `npm --version`

**Si nécessaire, lancez manuellement :**
```powershell
cd prisonbreak\prisonbreak.client
npm run dev
```

---

## 📊 Structure des Projets dans Visual Studio

Dans l'**Explorateur de solutions**, vous verrez :

```
Solution 'prisonbreak' (2 projets)
│
├─ prisonbreak.Server (ASP.NET Core)
│  ├─ Controllers/
│  ├─ Data/
│  ├─ DTOs/
│  ├─ Models/
│  ├─ Services/
│  ├─ Properties/
│  └─ Program.cs
│
└─ prisonbreak.client (Vue.js)
   ├─ src/
   │  ├─ components/
   │  ├─ services/
   │  ├─ types/
   │  └─ App.vue
   ├─ package.json
   └─ vite.config.js
```

---

## 🔧 Configuration du Débogage

### Déboguer le Backend (C#)

1. Placez des **points d'arrêt** (cliquez à gauche d'une ligne de code)
2. Lancez avec **F5**
3. Quand le code atteint le point d'arrêt, l'exécution s'arrête
4. Inspectez les variables, naviguez dans le code, etc.

### Déboguer le Frontend (Vue.js)

1. Ouvrez les **DevTools du navigateur** (F12)
2. Onglet "Sources" → Trouvez votre fichier Vue
3. Placez des points d'arrêt dans le navigateur
4. Les breakpoints fonctionnent grâce au source mapping

---

## 💻 Workflow de Développement

### Modifier le Backend

1. Modifiez un fichier `.cs`
2. Visual Studio recompile automatiquement
3. **Rechargez la page** du navigateur pour voir les changements

### Modifier le Frontend

1. Modifiez un fichier `.vue`, `.js` dans `prisonbreak.client/src/`
2. Vite **recharge automatiquement** (Hot Module Replacement)
3. La page se met à jour instantanément ! ⚡

---

## 📝 Fichiers Importants

| Fichier | Description |
|---------|-------------|
| `prisonbreak.sln` | Solution Visual Studio (fichier à ouvrir) |
| `prisonbreak.Server.csproj` | Projet backend |
| `prisonbreak.client.esproj` | Projet client (JavaScript) |
| `launchSettings.json` | Configuration des profils de démarrage |
| `vite.config.js` | Configuration du serveur de développement Vue.js |
| `Program.cs` | Point d'entrée du backend |
| `App.vue` | Composant racine Vue.js |

---

## ✨ Fonctionnalités de Visual Studio

### IntelliSense

- **C#** : IntelliSense complet dans le backend
- **Vue.js** : IntelliSense basique (installez Volar pour plus)

### Explorateur de solutions

- Clic droit sur un projet → **Définir comme projet de démarrage**
- Clic droit → **Propriétés** pour configurer le projet

### Console de sortie

- Affiche les logs du backend
- Menu : Affichage → Sortie

### Gestionnaire de packages NuGet

- Clic droit sur le projet → Gérer les packages NuGet
- Pour ajouter/mettre à jour des packages C#

---

## 🎯 Conseils Pro

1. **Utilisez deux écrans** : Visual Studio sur l'un, navigateur sur l'autre
2. **Gardez la console ouverte** : Pour voir les logs en temps réel
3. **Utilisez Git** : Visual Studio a un excellent support Git intégré
4. **Extensions recommandées** :
   - Volar (pour Vue.js)
   - C# Dev Kit (déjà inclus)

---

## 📚 Documentation

- **Backend** : Consultez `README.md` à la racine
- **API** : https://localhost:5001/swagger
- **Architecture** : `ARCHITECTURE.md`
- **Contribution** : `CONTRIBUTING.md`

---

## ✅ Checklist Complète

Avant de développer :

- [ ] Node.js et npm installés
- [ ] Visual Studio 2022 installé
- [ ] Workload "ASP.NET et développement web" installé
- [ ] `npm install` exécuté dans `prisonbreak.client`
- [ ] Certificat HTTPS approuvé (`dotnet dev-certs https --trust`)
- [ ] `prisonbreak.sln` ouvert dans Visual Studio
- [ ] Profil "https" sélectionné
- [ ] F5 lance sans erreur
- [ ] Page s'affiche dans le navigateur
- [ ] Bouton "Générer un Puzzle" fonctionne
- [ ] Swagger accessible

---

## 🎉 Vous êtes Prêt !

Si tout fonctionne, vous êtes maintenant prêt à développer !

**Bon codage ! 🚀**

---

**En cas de problème, consultez `INSTRUCTIONS_VISUAL_STUDIO.md` pour plus de détails.**

