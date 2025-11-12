# 🚀 Guide de Démarrage Rapide

Ce guide vous permettra de démarrer le projet Hashi en quelques minutes.

## ⚡ Démarrage Rapide (Windows)

### Prérequis Installés ?

Vérifiez que vous avez :
- ✅ Node.js (version 20.19+ ou 22.12+)
- ✅ .NET SDK 8.0
- ✅ Git

### Vérification des Versions

```powershell
# Vérifier Node.js
node --version

# Vérifier npm
npm --version

# Vérifier .NET
dotnet --version

# Vérifier Git
git --version
```

---

## 📦 Installation Première Fois

### 1. Installer les Dépendances Frontend

```powershell
# Aller dans le dossier frontend
cd frontend

# Installer les dépendances
npm install

# Retour à la racine
cd ..
```

### 2. Restaurer les Packages Backend

```powershell
# Aller dans le dossier backend
cd prisonbreak\prisonbreak.Server

# Restaurer les packages NuGet
dotnet restore

# Retour à la racine
cd ..\..
```

---

## 🎮 Lancement de l'Application

### Option 1 : Deux Terminaux Séparés (Recommandé)

#### Terminal 1 - Backend

```powershell
cd prisonbreak\prisonbreak.Server
dotnet run
```

Attendez de voir : `Now listening on: https://localhost:5001`

#### Terminal 2 - Frontend

```powershell
cd frontend
npm run dev
```

Attendez de voir : `Local: http://localhost:5173/`

### Option 2 : Script PowerShell (Windows)

Créez un fichier `start.ps1` à la racine :

```powershell
# start.ps1
Write-Host "🚀 Démarrage de Hashi..." -ForegroundColor Green

# Démarrer le backend en arrière-plan
Write-Host "📡 Démarrage du backend..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd prisonbreak\prisonbreak.Server; dotnet run"

# Attendre 5 secondes pour que le backend démarre
Start-Sleep -Seconds 5

# Démarrer le frontend
Write-Host "🎨 Démarrage du frontend..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd frontend; npm run dev"

Write-Host "✅ Application démarrée !" -ForegroundColor Green
Write-Host "Frontend: http://localhost:5173" -ForegroundColor Yellow
Write-Host "Backend: https://localhost:5001" -ForegroundColor Yellow
Write-Host "Swagger: https://localhost:5001/swagger" -ForegroundColor Yellow
```

Puis exécutez :

```powershell
.\start.ps1
```

---

## 🌐 Accès à l'Application

Une fois les deux serveurs lancés :

### Frontend
Ouvrez votre navigateur : **http://localhost:5173**

### Backend API
- **API** : https://localhost:5001/api
- **Swagger** : https://localhost:5001/swagger

---

## 🔧 Commandes Utiles

### Frontend

```powershell
cd frontend

# Démarrage en mode développement
npm run dev

# Build de production
npm run build

# Prévisualiser le build
npm run preview

# Linter
npm run lint

# Vérification des types
npm run type-check
```

### Backend

```powershell
cd prisonbreak\prisonbreak.Server

# Démarrage
dotnet run

# Build
dotnet build

# Watch mode (redémarre automatiquement)
dotnet watch run

# Nettoyer
dotnet clean
```

---

## ❓ Problèmes Courants

### Port 5173 déjà utilisé

```powershell
# Le frontend ne démarre pas
# Solution : Tuer le processus qui utilise le port
netstat -ano | findstr :5173
taskkill /PID <PID> /F
```

### Port 5001 déjà utilisé

```powershell
# Le backend ne démarre pas
# Solution : Tuer le processus
netstat -ano | findstr :5001
taskkill /PID <PID> /F
```

### Erreur CORS

Si vous voyez des erreurs CORS dans la console du navigateur :

1. Vérifiez que le backend tourne sur le port 5001
2. Vérifiez que le frontend accède à `https://localhost:5001/api`
3. Vérifiez le fichier `.env` du frontend

### Erreur de certificat SSL

Si vous avez une erreur de certificat HTTPS :

```powershell
# Faire confiance au certificat de développement .NET
dotnet dev-certs https --trust
```

### Base de données corrompue

Si la base de données pose problème :

```powershell
# Supprimer la base de données
cd prisonbreak\prisonbreak.Server
del hashi.db

# Relancer l'application (elle recréera la DB)
dotnet run
```

### Dépendances manquantes

```powershell
# Frontend
cd frontend
rm -r node_modules
rm package-lock.json
npm install

# Backend
cd prisonbreak\prisonbreak.Server
dotnet clean
dotnet restore
```

---

## 🐛 Debugging

### Frontend (Chrome DevTools)

1. Ouvrir les DevTools (F12)
2. Onglet "Console" pour les logs
3. Onglet "Network" pour les requêtes HTTP
4. Installer Vue DevTools : https://devtools.vuejs.org/

### Backend (Visual Studio Code)

1. Installer l'extension C# DevKit
2. F5 pour lancer en mode debug
3. Placer des breakpoints dans le code

---

## 📚 Prochaines Étapes

Une fois l'application lancée :

1. **Explorer l'interface** : Naviguez dans les différentes vues
2. **Tester l'API** : Allez sur https://localhost:5001/swagger
3. **Lire le code** : Consultez les commentaires dans le code
4. **Contribuer** : Lisez CONTRIBUTING.md

---

## 🎯 Checklist Première Utilisation

- [ ] Node.js installé et version vérifiée
- [ ] .NET SDK installé et version vérifiée
- [ ] Dépendances frontend installées (`npm install`)
- [ ] Packages backend restaurés (`dotnet restore`)
- [ ] Backend lancé (port 5001)
- [ ] Frontend lancé (port 5173)
- [ ] Application accessible dans le navigateur
- [ ] Swagger accessible
- [ ] Aucune erreur dans les consoles

---

## 📞 Besoin d'Aide ?

Si vous êtes bloqué :

1. Vérifiez les logs dans les terminaux
2. Consultez la section "Problèmes Courants" ci-dessus
3. Cherchez dans les issues GitHub
4. Créez une nouvelle issue si nécessaire
5. Contactez l'équipe

---

## 🎉 Félicitations !

Vous êtes maintenant prêt à développer sur le projet Hashi ! 🚀

**Happy coding!** 💻

