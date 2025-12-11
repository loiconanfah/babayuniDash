# Configuration Visual Studio pour lancer automatiquement 2 instances du frontend

## Configuration automatique

Le projet est maintenant configuré pour lancer automatiquement les deux instances du frontend (ports 5173 et 5174) lorsque vous démarrez le projet dans Visual Studio.

## Comment ça fonctionne

1. **Script npm** : `npm run dev:multi` lance les deux instances en parallèle
2. **Script PowerShell** : `start-frontend-multi.ps1` est exécuté automatiquement avant le build
3. **Deux fenêtres PowerShell** s'ouvrent automatiquement avec les deux instances

## Utilisation

### Démarrage normal
1. Ouvrez le projet dans Visual Studio
2. Appuyez sur **F5** ou cliquez sur **Démarrer**
3. Les deux instances du frontend se lancent automatiquement :
   - **Instance 1** : http://localhost:5173
   - **Instance 2** : http://localhost:5174

### Si les instances ne se lancent pas automatiquement

Si les instances ne se lancent pas, vous pouvez les lancer manuellement :

**Option 1 : Script PowerShell**
```powershell
.\lancer-multijoueur.ps1
```

**Option 2 : Script npm**
```powershell
cd frontend
npm run dev:multi
```

**Option 3 : Deux terminaux séparés**
```powershell
# Terminal 1
cd frontend
npm run dev:port1

# Terminal 2
cd frontend
npm run dev:port2
```

## Désactiver le lancement automatique

Si vous ne voulez pas que les deux instances se lancent automatiquement, commentez ou supprimez la section `<Target Name="StartFrontendMulti">` dans `prisonbreak.Server.csproj`.

## Notes

- Les deux instances partagent la même base de données
- Le backend écoute sur http://localhost:5000 et https://localhost:5001
- Les deux instances du frontend utilisent le même proxy vers le backend
- Vous pouvez vous connecter avec deux comptes différents pour tester le multijoueur

