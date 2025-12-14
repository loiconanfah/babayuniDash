# ⚙️ Configuration Render - Guide Rapide

## 🎯 Configuration pour le Backend

Dans l'interface Render, configurez :

### Informations de base :
- **Nom** : `babayuniDash-backend`
- **Langue** : `Docker`
- **Branche** : `puzzul`
- **Région** : `Virginia (US East)` (ou votre choix)
- **Répertoire racine** : `prisonbreak/prisonbreak.Server`
- **Chemin Dockerfile** : `Dockerfile`
- **Type d'instance** : `Free` (pour tester)

### Variables d'environnement :
```
ASPNETCORE_ENVIRONMENT=Production
ASPNETCORE_URLS=http://0.0.0.0:$PORT
PORT=10000
```

---

## 🎨 Configuration pour le Frontend

### Option 1 : Static Site (Recommandé - Gratuit)

- **Type** : `Static Site`
- **Nom** : `babayuniDash-frontend`
- **Branche** : `puzzul`
- **Répertoire racine** : `frontend`
- **Commande de build** : `npm install && npm run build`
- **Répertoire de publication** : `dist`

**Variable d'environnement :**
```
VITE_API_URL=https://babayuniDash-backend.onrender.com/api
```
⚠️ Remplacez par l'URL réelle du backend après déploiement

### Option 2 : Web Service (Node.js)

- **Type** : `Web Service`
- **Langue** : `Node`
- **Branche** : `puzzul`
- **Répertoire racine** : `frontend`
- **Commande de build** : `npm install && npm run build`
- **Commande de démarrage** : `npx serve dist -s -l $PORT`

**Variables d'environnement :**
```
VITE_API_URL=https://babayuniDash-backend.onrender.com/api
PORT=10000
```

---

## 📋 Ordre de Déploiement

1. **Déployer le backend d'abord**
   - Notez l'URL générée (ex: `https://babayuniDash-backend.onrender.com`)

2. **Déployer le frontend ensuite**
   - Utilisez l'URL du backend dans `VITE_API_URL`
   - Format : `https://babayuniDash-backend.onrender.com/api`

---

## ✅ Checklist de Configuration

- [ ] Backend configuré avec Docker
- [ ] Répertoire racine : `prisonbreak/prisonbreak.Server`
- [ ] Variables d'environnement backend ajoutées
- [ ] Frontend configuré (Static Site ou Web Service)
- [ ] Variable `VITE_API_URL` configurée avec l'URL du backend
- [ ] CORS configuré dans le backend pour autoriser le frontend

---

**🚀 Une fois configuré, cliquez sur "Create Web Service" !**



