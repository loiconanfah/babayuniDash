# 📦 Installation des Dépendances - Solution au Problème SPA Proxy

## 🐛 Problème

Le SPA Proxy échoue avec l'erreur :
```
Microsoft.AspNetCore.SpaProxy.SpaProxyLaunchManager: Error: Couldn't start the SPA development server with command 'npm run dev'.
```

**Cause** : Les dépendances npm ne sont pas installées dans le dossier `frontend/`.

---

## ✅ Solution Rapide

### Étape 1 : Installer les Dépendances Frontend

Ouvrez PowerShell et exécutez :

```powershell
cd frontend
npm install
```

**Temps estimé** : 1-2 minutes

### Étape 2 : Vérifier l'Installation

```powershell
Test-Path node_modules
```

Doit retourner `True`.

### Étape 3 : Relancer Visual Studio

1. Fermez Visual Studio
2. Rouvrez la solution
3. Appuyez sur **F5**

Le SPA Proxy devrait maintenant fonctionner correctement.

---

## 🔍 Vérification Complète

### Checklist

- [ ] Node.js installé (`node --version`)
- [ ] npm installé (`npm --version`)
- [ ] Dépendances installées (`Test-Path frontend\node_modules`)
- [ ] `package.json` présent dans `frontend/`
- [ ] Visual Studio relancé

---

## 🚀 Après l'Installation

Une fois les dépendances installées, Visual Studio devrait :

1. ✅ Lancer le backend sur `https://localhost:5001`
2. ✅ Lancer automatiquement `npm run dev` dans `frontend/`
3. ✅ Démarrer Vite sur `http://localhost:5173`
4. ✅ Configurer le proxy correctement

---

## 📝 Notes

- **Première installation** : `npm install` peut prendre 1-2 minutes
- **Réinstallation** : Si vous supprimez `node_modules`, relancez `npm install`
- **Mise à jour** : `npm update` pour mettre à jour les dépendances

---

**Une fois les dépendances installées, le problème devrait être résolu !** ✅

