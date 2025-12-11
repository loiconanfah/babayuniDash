# 🚀 Guide Pratique de Déploiement - Étape par Étape

Ce guide vous accompagne pour déployer votre plateforme de jeux Hashi en production.

## 📋 Prérequis

- Compte GitHub/GitLab
- Compte sur une plateforme de déploiement (Heroku, Netlify, Azure, etc.)
- Accès à un terminal/command line
- Git installé

---

## 🎯 Option Recommandée : Déploiement Séparé (Backend + Frontend)

### Partie 1 : Déploiement du Backend (Heroku)

#### Étape 1 : Préparer le Backend

1. **Créer un fichier `Procfile`** dans `prisonbreak/prisonbreak.Server/` :
```
web: dotnet prisonbreak.Server.dll --urls http://0.0.0.0:$PORT
```

2. **Créer un fichier `.dockerignore`** (optionnel) :
```
bin/
obj/
*.db
*.db-shm
*.db-wal
```

3. **Modifier `Program.cs`** pour supporter les variables d'environnement :
```csharp
// Ajouter avant app.Run()
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
app.Urls.Add($"http://0.0.0.0:{port}");
```

#### Étape 2 : Créer l'Application Heroku

```bash
# Installer Heroku CLI (si pas déjà fait)
# Windows: https://devcenter.heroku.com/articles/heroku-cli

# Se connecter
heroku login

# Créer l'application
cd prisonbreak/prisonbreak.Server
heroku create hashi-backend-votre-nom

# Ajouter le buildpack .NET
heroku buildpacks:set https://github.com/jincod/dotnetcore-buildpack

# Configurer les variables d'environnement
heroku config:set ASPNETCORE_ENVIRONMENT=Production
heroku config:set ConnectionStrings__DefaultConnection="[VOTRE_CONNEXION]"

# Déployer
git push heroku main
```

#### Étape 3 : Configurer la Base de Données

```bash
# Ajouter PostgreSQL (gratuit)
heroku addons:create heroku-postgresql:mini

# La variable DATABASE_URL sera automatiquement configurée
# Modifier Program.cs pour utiliser DATABASE_URL si présent
```

#### Étape 4 : Vérifier le Déploiement

```bash
# Voir les logs
heroku logs --tail

# Ouvrir l'application
heroku open
```

---

### Partie 2 : Déploiement du Frontend (Netlify)

#### Étape 1 : Préparer le Frontend

1. **Créer un fichier `netlify.toml`** dans `frontend/` :
```toml
[build]
  command = "npm run build"
  publish = "dist"

[[redirects]]
  from = "/*"
  to = "/index.html"
  status = 200

[build.environment]
  NODE_VERSION = "20"
```

2. **Créer un fichier `.env.production`** dans `frontend/` :
```env
VITE_API_BASE_URL=https://hashi-backend-votre-nom.herokuapp.com
```

#### Étape 2 : Déployer sur Netlify

**Option A : Via l'interface web (Recommandé)**

1. Aller sur https://app.netlify.com
2. Cliquer sur "Add new site" > "Import an existing project"
3. Connecter votre repository GitHub/GitLab
4. Configurer :
   - **Base directory** : `frontend`
   - **Build command** : `npm run build`
   - **Publish directory** : `frontend/dist`
5. Ajouter les variables d'environnement :
   - `VITE_API_BASE_URL` = `https://votre-backend.herokuapp.com`
6. Cliquer sur "Deploy site"

**Option B : Via CLI**

```bash
# Installer Netlify CLI
npm install -g netlify-cli

# Se connecter
netlify login

# Dans le dossier frontend
cd frontend

# Déployer
netlify deploy --prod
```

#### Étape 3 : Configurer le Domaine Personnalisé (Optionnel)

1. Dans Netlify : Site settings > Domain management
2. Ajouter un domaine personnalisé
3. Suivre les instructions DNS

---

## 🐳 Option Alternative : Déploiement avec Docker

### Créer les Dockerfiles

#### Backend Dockerfile (`prisonbreak/prisonbreak.Server/Dockerfile`)
```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["prisonbreak.Server.csproj", "./"]
RUN dotnet restore "prisonbreak.Server.csproj"
COPY . .
WORKDIR "/src"
RUN dotnet build "prisonbreak.Server.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "prisonbreak.Server.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "prisonbreak.Server.dll"]
```

#### Frontend Dockerfile (`frontend/Dockerfile`)
```dockerfile
FROM node:20-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

#### Nginx Config (`frontend/nginx.conf`)
```nginx
server {
    listen 80;
    server_name localhost;
    root /usr/share/nginx/html;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }

    location /api {
        proxy_pass http://backend:5000;
        proxy_http_version 1.1;
        proxy_set_header Upgrade $http_upgrade;
        proxy_set_header Connection 'upgrade';
        proxy_set_header Host $host;
        proxy_cache_bypass $http_upgrade;
    }
}
```

#### Docker Compose (`docker-compose.yml`)
```yaml
version: '3.8'

services:
  backend:
    build:
      context: ./prisonbreak/prisonbreak.Server
      dockerfile: Dockerfile
    ports:
      - "5000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Data Source=/app/data/hashi.db
    volumes:
      - ./data:/app/data

  frontend:
    build:
      context: ./frontend
      dockerfile: Dockerfile
    ports:
      - "80:80"
    depends_on:
      - backend
    environment:
      - VITE_API_BASE_URL=http://localhost:5000
```

### Déployer avec Docker

```bash
# Construire et lancer
docker-compose up -d

# Voir les logs
docker-compose logs -f

# Arrêter
docker-compose down
```

---

## 🔧 Configuration CORS pour Production

### Modifier `Program.cs` du Backend

```csharp
// Dans Program.cs, remplacer la configuration CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("Production", policy =>
    {
        policy.WithOrigins(
            "https://votre-site.netlify.app",
            "https://votre-domaine.com"
        )
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
    });
});

// Plus bas dans le code
if (app.Environment.IsProduction())
{
    app.UseCors("Production");
}
```

---

## 🔐 Configuration de la Sécurité

### 1. Variables d'Environnement à Configurer

**Backend (Heroku)** :
```bash
heroku config:set JWT_SECRET="[GENERER_UNE_CLE_SECRETE_LONGUE]"
heroku config:set JWT_ISSUER="https://votre-backend.herokuapp.com"
heroku config:set ALLOWED_ORIGINS="https://votre-frontend.netlify.app"
```

**Frontend (Netlify)** :
- `VITE_API_BASE_URL` = URL de votre backend
- `VITE_ENABLE_ANALYTICS` = `true` ou `false`

### 2. Générer une Clé Secrète JWT

```bash
# Sur Linux/Mac
openssl rand -base64 32

# Sur Windows (PowerShell)
[Convert]::ToBase64String((1..32 | ForEach-Object { Get-Random -Minimum 0 -Maximum 256 }))
```

---

## ✅ Vérifications Post-Déploiement

### Checklist de Vérification

- [ ] Backend accessible et répond aux requêtes
- [ ] Frontend accessible et charge correctement
- [ ] API fonctionne (tester avec Swagger)
- [ ] CORS configuré correctement
- [ ] Base de données accessible
- [ ] Authentification fonctionne
- [ ] Jeux fonctionnent (Hashi, TicTacToe, etc.)
- [ ] Multijoueur fonctionne
- [ ] HTTPS activé (certificat SSL valide)
- [ ] Logs accessibles et sans erreurs critiques

### Tests à Effectuer

1. **Test d'Authentification** :
   - Créer un compte
   - Se connecter
   - Vérifier la session

2. **Test des Jeux** :
   - Lancer une partie Hashi
   - Lancer une partie TicTacToe
   - Tester le multijoueur

3. **Test de Performance** :
   - Temps de chargement < 3 secondes
   - Pas d'erreurs dans la console
   - Responsive sur mobile

---

## 🐛 Résolution de Problèmes Courants

### Problème : CORS Error
**Solution** : Vérifier que l'URL du frontend est dans `AllowedOrigins` du backend

### Problème : Base de Données Non Accessible
**Solution** : Vérifier la chaîne de connexion et les permissions

### Problème : Frontend Ne Charge Pas
**Solution** : Vérifier que `VITE_API_BASE_URL` est correctement configuré

### Problème : Build Échoue
**Solution** : Vérifier les logs de build, souvent un problème de dépendances

---

## 📊 Monitoring et Maintenance

### Outils Recommandés

1. **Sentry** (Gestion d'erreurs) : https://sentry.io
2. **Uptime Robot** (Monitoring uptime) : https://uptimerobot.com
3. **Google Analytics** (Analytics) : https://analytics.google.com

### Commandes Utiles

```bash
# Voir les logs Heroku
heroku logs --tail

# Voir les logs Netlify
netlify logs

# Redémarrer le backend
heroku restart

# Voir les variables d'environnement
heroku config
```

---

## 🎉 Félicitations !

Votre plateforme est maintenant déployée et accessible au public !

**Prochaines Étapes** :
1. Partager l'URL avec vos utilisateurs
2. Monitorer les performances
3. Collecter les retours utilisateurs
4. Itérer et améliorer

---

**Besoin d'aide ?** Consultez la documentation officielle des plateformes ou créez une issue sur votre repository.

