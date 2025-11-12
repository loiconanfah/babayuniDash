# 🤝 Guide de Contribution

Merci de votre intérêt pour contribuer au projet Hashi ! Ce document contient les lignes directrices pour contribuer efficacement.

## 📋 Table des matières

- [Code de Conduite](#code-de-conduite)
- [Comment Contribuer](#comment-contribuer)
- [Standards de Code](#standards-de-code)
- [Processus de Pull Request](#processus-de-pull-request)
- [Structure des Commits](#structure-des-commits)
- [Tests](#tests)

---

## 👮 Code de Conduite

En participant à ce projet, vous vous engagez à maintenir un environnement respectueux et inclusif pour tous les contributeurs.

### Nos Standards

✅ **À faire :**
- Utiliser un langage accueillant et inclusif
- Respecter les différents points de vue et expériences
- Accepter les critiques constructives avec grâce
- Se concentrer sur ce qui est le mieux pour la communauté
- Montrer de l'empathie envers les autres membres

❌ **À éviter :**
- Commentaires trolls, insultants ou désobligeants
- Harcèlement public ou privé
- Publication d'informations privées sans permission
- Comportement non professionnel

---

## 🚀 Comment Contribuer

### 1. Fork et Clone

```bash
# Fork le repository sur GitHub, puis clonez votre fork
git clone https://github.com/VOTRE_USERNAME/projet-de-session-hashi-prisonbreak.git
cd projet-de-session-hashi-prisonbreak

# Ajouter le remote upstream
git remote add upstream https://github.com/ORIGINAL_OWNER/projet-de-session-hashi-prisonbreak.git
```

### 2. Créer une Branche

```bash
# Créer une branche pour votre fonctionnalité
git checkout -b feature/nom-de-la-fonctionnalite

# Ou pour un bug fix
git checkout -b fix/description-du-bug
```

### 3. Faire vos Modifications

- Écrivez du code propre et bien documenté
- Ajoutez des commentaires pour les parties complexes
- Testez vos modifications

### 4. Commiter

```bash
# Ajouter vos fichiers
git add .

# Commiter avec un message clair
git commit -m "feat: ajouter système d'indices pour les puzzles"
```

### 5. Pousser

```bash
# Pousser vers votre fork
git push origin feature/nom-de-la-fonctionnalite
```

### 6. Créer une Pull Request

- Allez sur GitHub
- Cliquez sur "New Pull Request"
- Décrivez vos modifications clairement
- Attendez la revue de code

---

## 💻 Standards de Code

### Backend (C#)

#### Conventions de Nommage

```csharp
// Classes : PascalCase
public class PuzzleService { }

// Interfaces : I + PascalCase
public interface IPuzzleService { }

// Méthodes publiques : PascalCase
public async Task<Puzzle> GetPuzzleByIdAsync(int id) { }

// Variables privées : _camelCase
private readonly HashiDbContext _context;

// Paramètres et variables locales : camelCase
public void DoSomething(int puzzleId) 
{
    var result = puzzleId * 2;
}
```

#### Documentation XML

```csharp
/// <summary>
/// Description claire de ce que fait la méthode
/// </summary>
/// <param name="id">Description du paramètre</param>
/// <returns>Description de ce qui est retourné</returns>
/// <exception cref="ArgumentException">Quand lancer cette exception</exception>
public async Task<Puzzle> GetPuzzleByIdAsync(int id)
{
    // Implémentation
}
```

#### Bonnes Pratiques

- ✅ Utiliser `async/await` pour les opérations I/O
- ✅ Gérer les erreurs avec des try/catch appropriés
- ✅ Logger les erreurs importantes
- ✅ Valider les entrées utilisateur
- ✅ Utiliser l'injection de dépendances
- ❌ Pas de code mort (commenté ou non utilisé)
- ❌ Pas de variables magiques (utiliser des constantes)

### Frontend (Vue.js + TypeScript)

#### Conventions de Nommage

```typescript
// Composants : PascalCase
// Fichiers : PascalCase.vue
export default defineComponent({
  name: 'GameGrid'
})

// Types/Interfaces : PascalCase
export interface Island {
  id: number
}

// Fonctions : camelCase
function handleIslandClick(island: Island) { }

// Variables : camelCase
const currentPuzzle = ref<Puzzle | null>(null)

// Constantes : UPPER_SNAKE_CASE
const MAX_BRIDGES_PER_ISLAND = 8
```

#### Structure des Composants Vue

```vue
<script setup lang="ts">
/**
 * Description du composant
 * Expliquer son rôle et son utilisation
 */

// 1. Imports
import { ref, computed } from 'vue'
import type { Island } from '@/types'

// 2. Props
interface Props {
  island: Island
  isSelected?: boolean
}

const props = defineProps<Props>()

// 3. Emits
const emit = defineEmits<{
  click: [island: Island]
}>()

// 4. State réactif
const isHovered = ref(false)

// 5. Computed
const classes = computed(() => ({
  'island--selected': props.isSelected
}))

// 6. Methods
function handleClick() {
  emit('click', props.island)
}
</script>

<template>
  <!-- Template clair et bien indenté -->
</template>

<style scoped>
/* Styles du composant */
</style>
```

#### Types TypeScript

```typescript
// Toujours typer explicitement
const puzzles: Puzzle[] = []

// Utiliser des types stricts
interface Island {
  id: number          // Pas "any"
  x: number
  y: number
  requiredBridges: number
}

// Éviter any
❌ const data: any = fetchData()
✅ const data: Puzzle = await fetchPuzzle()
```

#### Bonnes Pratiques

- ✅ Utiliser Composition API (pas Options API)
- ✅ Extraire la logique complexe dans des composables
- ✅ Typer toutes les props et emits
- ✅ Utiliser computed pour les valeurs dérivées
- ✅ Nommer les événements en kebab-case dans le template
- ❌ Pas de logique métier dans les composants (utiliser stores)
- ❌ Pas de manipulation directe du DOM

---

## 🔄 Processus de Pull Request

### Avant de Soumettre

- [ ] Le code compile sans erreur
- [ ] Tous les tests passent
- [ ] Le code respecte les standards de codage
- [ ] La documentation est à jour
- [ ] Les commentaires sont clairs et en français
- [ ] Pas de console.log() ou de code de debug

### Description de la PR

Utilisez ce template :

```markdown
## Description
Brève description de vos modifications

## Type de Changement
- [ ] Bug fix
- [ ] Nouvelle fonctionnalité
- [ ] Breaking change
- [ ] Documentation

## Comment Tester
1. Étape 1
2. Étape 2
3. Résultat attendu

## Checklist
- [ ] Mon code respecte les standards du projet
- [ ] J'ai commenté les parties complexes
- [ ] J'ai mis à jour la documentation
- [ ] Mes modifications ne génèrent pas de nouveaux warnings
- [ ] J'ai testé mes modifications
```

### Processus de Revue

1. **Soumission** : Vous créez une PR
2. **Revue automatique** : Les tests automatisés s'exécutent
3. **Revue par les pairs** : Un autre développeur examine le code
4. **Modifications** : Vous apportez les corrections demandées
5. **Approbation** : La PR est approuvée
6. **Merge** : Le code est fusionné dans la branche principale

---

## 📝 Structure des Commits

Nous suivons la convention [Conventional Commits](https://www.conventionalcommits.org/).

### Format

```
<type>(<scope>): <description>

[corps optionnel]

[footer optionnel]
```

### Types

| Type | Description | Exemple |
|------|-------------|---------|
| `feat` | Nouvelle fonctionnalité | `feat(game): ajouter système d'indices` |
| `fix` | Correction de bug | `fix(validation): corriger détection de ponts croisés` |
| `docs` | Documentation | `docs(readme): ajouter section installation` |
| `style` | Formatage, style | `style(frontend): uniformiser indentation` |
| `refactor` | Refactoring | `refactor(services): simplifier ValidationService` |
| `test` | Ajout de tests | `test(puzzle): ajouter tests unitaires` |
| `chore` | Maintenance | `chore(deps): mettre à jour dépendances` |
| `perf` | Performance | `perf(db): optimiser requêtes SQL` |

### Exemples

```bash
# Fonctionnalité simple
git commit -m "feat: ajouter bouton de pause"

# Avec scope
git commit -m "feat(backend): ajouter endpoint pour classement"

# Avec description longue
git commit -m "fix(game): corriger bug de suppression de pont

Le clic sur un pont double ne le supprimait pas correctement.
Ajout de vérification de l'état avant suppression.

Fixes #123"

# Breaking change
git commit -m "feat(api)!: changer format de réponse des puzzles

BREAKING CHANGE: Le format de réponse a changé de { islands: [] }
à { data: { islands: [] } }"
```

---

## 🧪 Tests

### Backend

```bash
cd prisonbreak/prisonbreak.Server
dotnet test
```

### Frontend

```bash
cd frontend
npm run test
```

### Avant de Commiter

```bash
# Linter
npm run lint

# Type checking
npm run type-check
```

---

## 🎯 Domaines de Contribution

Voici les domaines où vous pouvez contribuer :

### 🔴 Priorité Haute

- [ ] Améliorer le générateur de puzzles
- [ ] Ajouter des tests unitaires
- [ ] Optimiser les performances
- [ ] Corriger les bugs connus

### 🟠 Priorité Moyenne

- [ ] Système d'authentification
- [ ] Classement/leaderboard
- [ ] Système d'indices
- [ ] Mode sombre

### 🟢 Nice to Have

- [ ] Animations améliorées
- [ ] Sons et musique
- [ ] Partage de puzzles
- [ ] Mode multijoueur
- [ ] Application mobile

---

## 📞 Questions ?

Si vous avez des questions :

1. Consultez la documentation
2. Cherchez dans les issues existantes
3. Créez une nouvelle issue
4. Contactez l'équipe

---

## 🙏 Merci !

Merci de prendre le temps de contribuer à ce projet ! Chaque contribution, petite ou grande, est appréciée. 🎉

**Happy coding! 🚀**

