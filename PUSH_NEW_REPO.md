# 📤 Envoyer le Projet vers un Nouveau Dépôt Git

## 📋 Étapes

### 1. Créer un nouveau dépôt sur GitHub/GitLab/Bitbucket

Créez un nouveau dépôt vide sur votre plateforme Git préférée (sans README, .gitignore, ou licence).

### 2. Ajouter le nouveau remote

```bash
# Ajouter le nouveau remote (remplacez par votre URL)
git remote add new-origin https://github.com/VOTRE_USERNAME/VOTRE_REPO.git

# Ou pour SSH
git remote add new-origin git@github.com:VOTRE_USERNAME/VOTRE_REPO.git
```

### 3. Vérifier les remotes

```bash
git remote -v
```

Vous devriez voir :
- `origin` : votre dépôt actuel
- `new-origin` : votre nouveau dépôt

### 4. Pousser vers le nouveau dépôt

```bash
# Pousser toutes les branches
git push new-origin --all

# Pousser aussi les tags (optionnel)
git push new-origin --tags
```

### 5. (Optionnel) Remplacer l'ancien remote

Si vous voulez que `origin` pointe vers le nouveau dépôt :

```bash
# Supprimer l'ancien remote
git remote remove origin

# Renommer le nouveau remote
git remote rename new-origin origin
```

## 🔄 Alternative : Changer l'URL de l'origin existant

Si vous voulez simplement changer l'URL du remote `origin` :

```bash
# Changer l'URL de origin
git remote set-url origin https://github.com/VOTRE_USERNAME/VOTRE_REPO.git

# Pousser vers le nouveau dépôt
git push -u origin --all
```

## ⚠️ Notes Importantes

1. **Fichiers sensibles** : Vérifiez que vous n'avez pas de fichiers sensibles (tokens, mots de passe) dans le dépôt
2. **.gitignore** : Assurez-vous que votre `.gitignore` exclut les fichiers temporaires et les secrets
3. **Branches** : La commande `--all` pousse toutes les branches locales
4. **Historique** : Tout l'historique Git sera copié vers le nouveau dépôt

## 🚀 Commandes Rapides

```bash
# 1. Ajouter le nouveau remote
git remote add new-origin https://github.com/VOTRE_USERNAME/VOTRE_REPO.git

# 2. Pousser tout
git push new-origin --all

# 3. (Optionnel) Remplacer origin
git remote remove origin
git remote rename new-origin origin
```

