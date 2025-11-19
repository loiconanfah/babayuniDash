# 🔄 Migration du Frontend - Résumé des Changements

## ✅ Changements Effectués

Le projet a été restructuré pour utiliser uniquement le dossier `frontend/` à la racine comme client Vue.js, au lieu de `prisonbreak/prisonbreak.client/`.

### Fichiers Modifiés

1. **`prisonbreak/prisonbreak.Server/prisonbreak.Server.csproj`**
   - ✅ `SpaRoot` mis à jour : `..\prisonbreak.client\` → `..\..\frontend\`
   - ✅ Référence au projet `prisonbreak.client.esproj` supprimée

2. **`prisonbreak/prisonbreak.sln`**
   - ✅ Projet `prisonbreak.client` retiré de la solution
   - ✅ Seul le projet `prisonbreak.Server` reste dans la solution

3. **`prisonbreak/start-dev.ps1`**
   - ✅ Chemin vers `node_modules` mis à jour : `..\frontend\node_modules`
   - ✅ Commande de démarrage mise à jour : `cd '$PWD\..\frontend'`

4. **`prisonbreak/start-dev.bat`**
   - ✅ Commande de démarrage mise à jour : `cd ..\frontend`

5. **`prisonbreak/LANCER_VISUAL_STUDIO.md`**
   - ✅ Toutes les références à `prisonbreak.client` mises à jour vers `frontend`

6. **`prisonbreak/prisonbreak.Server/wwwroot/index.html`**
   - ✅ Instructions mises à jour pour pointer vers `frontend`

---

## ⚠️ Action Requise

### Supprimer le Dossier `prisonbreak.client`

Le dossier `prisonbreak/prisonbreak.client/` doit être supprimé manuellement car il est actuellement verrouillé par un processus.

**Pour le supprimer :**

1. **Fermez tous les processus qui pourraient utiliser ce dossier :**
   - Visual Studio
   - Terminaux PowerShell/CMD qui tournent dans ce dossier
   - Node.js (processus npm/node)
   - Explorateur de fichiers si ouvert dans ce dossier

2. **Supprimez le dossier :**
   ```powershell
   cd prisonbreak
   Remove-Item -Recurse -Force prisonbreak.client
   ```

   OU manuellement via l'Explorateur de fichiers.

---

## 📁 Nouvelle Structure

```
projet-de-session-hashi-prisonbreak2/
│
├── frontend/                    ← Client Vue.js (à la racine)
│   ├── src/
│   ├── package.json
│   └── vite.config.ts
│
└── prisonbreak/
    ├── prisonbreak.Server/     ← Backend ASP.NET Core
    │   ├── Controllers/
    │   ├── Services/
    │   └── Program.cs
    │
    ├── prisonbreak.sln         ← Solution Visual Studio
    ├── start-dev.ps1           ← Script de démarrage
    └── start-dev.bat           ← Script de démarrage (Windows)
```

---

## 🚀 Démarrage du Projet

### Option 1 : Script PowerShell (Recommandé)

```powershell
cd prisonbreak
.\start-dev.ps1
```

### Option 2 : Manuellement

**Terminal 1 - Backend :**
```powershell
cd prisonbreak\prisonbreak.Server
dotnet run
```

**Terminal 2 - Frontend :**
```powershell
cd frontend
npm run dev
```

---

## ✅ Vérification

Après les changements, vérifiez que :

- [ ] Le dossier `prisonbreak.client` est supprimé
- [ ] Le backend démarre correctement
- [ ] Le frontend démarre sur `http://localhost:5173`
- [ ] Le backend pointe vers `https://localhost:5001`
- [ ] Les deux communiquent correctement (pas d'erreurs CORS)

---

## 📝 Notes

- Le dossier `frontend/` est maintenant le seul client Vue.js du projet
- Tous les scripts et configurations pointent vers `frontend/`
- La solution Visual Studio ne contient plus le projet client (normal, il est externe)
- Le SPA Proxy dans Visual Studio fonctionnera toujours car `SpaRoot` pointe vers `frontend/`

---

**Migration terminée ! 🎉**

