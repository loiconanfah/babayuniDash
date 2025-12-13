# 🚀 Lancer 2 Instances du Frontend avec Visual Studio

## ✅ Configuration Automatique

Le projet est maintenant configuré pour que Visual Studio lance **automatiquement 2 instances** du frontend quand vous utilisez le profil **"https"**.

## 📋 Comment Utiliser

### 1. Ouvrir Visual Studio

Ouvrez la solution : `prisonbreak/prisonbreak.sln`

### 2. Sélectionner le Profil "https"

En haut de Visual Studio, dans la liste déroulante à côté du bouton ▶️, sélectionnez **"https"**.

### 3. Lancer (F5)

Appuyez sur **F5** ou cliquez sur ▶️.

## 🎯 Ce qui se passe automatiquement

1. ✅ Visual Studio compile le backend
2. ✅ Visual Studio lance le serveur ASP.NET Core sur `https://localhost:5001`
3. ✅ Visual Studio exécute le script `start-multi-frontend.ps1`
4. ✅ Le script lance **2 fenêtres PowerShell** :
   - **Instance 1** : Port 5173 (fenêtre verte)
   - **Instance 2** : Port 5174 (fenêtre bleue)
5. ✅ Chaque instance démarre Vite avec son propre port

## 🌐 URLs Disponibles

Après le lancement, vous avez accès à :

- **Backend** : `https://localhost:5001`
- **Instance 1** : `http://localhost:5173`
- **Instance 2** : `http://localhost:5174`

## 🎮 Tester le Multijoueur

1. **Instance 1** (Port 5173) :
   - Ouvrez `http://localhost:5173` dans votre navigateur
   - Connectez-vous avec un compte (ex: `joueur1@test.com`)

2. **Instance 2** (Port 5174) :
   - Ouvrez `http://localhost:5174` dans un autre onglet ou navigateur
   - Connectez-vous avec un autre compte (ex: `joueur2@test.com`)

3. **Jouer ensemble** :
   - Allez dans "Jeux" > "Tic-Tac-Toe" (ou autre jeu multijoueur)
   - Créez une partie depuis l'instance 1
   - Rejoignez depuis l'instance 2

## 🔧 Configuration Technique

### Fichier Modifié : `prisonbreak.Server.csproj`

```xml
<SpaProxyLaunchCommand>powershell -ExecutionPolicy Bypass -File "$(MSBuildProjectDirectory)\start-multi-frontend.ps1"</SpaProxyLaunchCommand>
```

### Script : `start-multi-frontend.ps1`

Ce script :
- Lance deux fenêtres PowerShell séparées
- Chaque fenêtre exécute `npm run dev:port1` ou `npm run dev:port2`
- Les instances tournent indépendamment

## ⚠️ Notes Importantes

- **Les deux instances partagent le même backend** (sur `https://localhost:5001`)
- **Les sessions sont partagées** entre les deux instances
- **Vous pouvez voir les utilisateurs en ligne** depuis les deux instances
- **Pour arrêter** : Fermez Visual Studio ou arrêtez les fenêtres PowerShell

## 🐛 Dépannage

### Si une seule instance se lance

1. Vérifiez que les ports 5173 et 5174 ne sont pas déjà utilisés
2. Fermez toutes les instances Vite en cours
3. Relancez Visual Studio avec F5

### Si le script ne s'exécute pas

1. Vérifiez que PowerShell peut exécuter des scripts :
   ```powershell
   Get-ExecutionPolicy
   ```
2. Si nécessaire, changez la politique :
   ```powershell
   Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
   ```

### Si les instances ne démarrent pas

1. Vérifiez que les dépendances npm sont installées :
   ```powershell
   cd frontend
   npm install
   ```

## ✅ Vérification

Après le lancement, vous devriez voir :
- ✅ 1 fenêtre Visual Studio avec le backend
- ✅ 2 fenêtres PowerShell (une verte, une bleue) avec Vite
- ✅ 3 URLs accessibles (backend + 2 frontends)

---

**Maintenant, quand vous lancez avec F5 en mode "https", vous aurez automatiquement 2 instances du frontend !** 🎉

