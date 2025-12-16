# Guide pour tester le multijoueur en local

## Méthode 1 : Deux terminaux PowerShell (Recommandé)

### Terminal 1 - Instance 1 (Port 5173)
```powershell
cd frontend
npm run dev:port1
```
Ouvrez : http://localhost:5173

### Terminal 2 - Instance 2 (Port 5174)
```powershell
cd frontend
npm run dev:port2
```
Ouvrez : http://localhost:5174

## Méthode 2 : Deux fenêtres de navigateur en mode incognito

1. Lancez le frontend normalement :
```powershell
cd frontend
npm run dev
```

2. Ouvrez deux fenêtres de navigateur :
   - Fenêtre 1 : http://localhost:5173 (mode normal)
   - Fenêtre 2 : http://localhost:5173 (mode incognito/privé)

3. Connectez-vous avec deux comptes différents dans chaque fenêtre

## Méthode 3 : Script PowerShell automatique

Créez un fichier `lancer-multijoueur.ps1` :

```powershell
# Lancer deux instances du frontend
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD\frontend'; npm run dev:port1"
Start-Sleep -Seconds 2
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD\frontend'; npm run dev:port2"
```

Puis exécutez :
```powershell
.\lancer-multijoueur.ps1
```

## Backend

Le backend doit être lancé une seule fois (il écoute sur http://localhost:5000 et https://localhost:5001) :

```powershell
cd prisonbreak/prisonbreak.Server
dotnet run
```

## Test du multijoueur

1. **Instance 1** (Port 5173) :
   - Connectez-vous avec un compte (ex: loico@test.com)
   - Allez dans "Jeux" > "Tic-Tac-Toe"
   - Cliquez sur "Jouer contre un joueur"
   - Sélectionnez un joueur en ligne ou créez une partie publique

2. **Instance 2** (Port 5174) :
   - Connectez-vous avec un autre compte (ex: joueur2@test.com)
   - Allez dans "Jeux" > "Tic-Tac-Toe"
   - Cliquez sur "Jouer contre un joueur"
   - Rejoignez la partie créée par l'instance 1 ou invitez directement l'instance 1

## Notes importantes

- Les deux instances partagent la même base de données (SQLite)
- Les sessions sont partagées entre les deux instances
- Vous pouvez voir les utilisateurs en ligne depuis les deux instances
- Le rafraîchissement automatique fonctionne toutes les 2 secondes en mode multijoueur

