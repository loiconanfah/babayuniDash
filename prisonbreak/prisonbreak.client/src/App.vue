<script setup>
import { ref, onMounted } from 'vue'
import { puzzleApi } from './services/api.js'

const puzzles = ref([])
const loading = ref(false)
const error = ref(null)

// Test de l'API au chargement
onMounted(async () => {
  try {
    loading.value = true
    puzzles.value = await puzzleApi.getAll()
  } catch (err) {
    error.value = err.message
  } finally {
    loading.value = false
  }
})

// Générer un puzzle de test
async function generateTestPuzzle() {
  try {
    loading.value = true
    error.value = null
    const puzzle = await puzzleApi.generate({
      width: 10,
      height: 10,
      difficulty: 1
    })
    puzzles.value = [puzzle, ...puzzles.value]
    alert('Puzzle généré avec succès ! ID: ' + puzzle.id)
  } catch (err) {
    error.value = err.message
    alert('Erreur: ' + err.message)
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div id="app">
    <header class="header">
      <h1>🌉 Hashi - Jeu de Puzzle</h1>
      <p class="subtitle">Application Vue.js + ASP.NET Core</p>
    </header>

    <main class="main">
      <div class="card">
        <h2>Test de Connexion API</h2>
        
        <button 
          @click="generateTestPuzzle" 
          :disabled="loading"
          class="btn btn-primary"
        >
          {{ loading ? 'Génération...' : '✨ Générer un Puzzle Test' }}
        </button>

        <div v-if="error" class="error">
          ❌ Erreur: {{ error }}
        </div>

        <div v-if="loading" class="loading">
          Chargement...
        </div>

        <div v-else-if="puzzles.length > 0" class="puzzles">
          <h3>Puzzles disponibles ({{ puzzles.length }})</h3>
          <div class="puzzle-list">
            <div v-for="puzzle in puzzles" :key="puzzle.id" class="puzzle-item">
              <div class="puzzle-info">
                <strong>Puzzle #{{ puzzle.id }}</strong>
                <span class="puzzle-detail">
                  {{ puzzle.width }}x{{ puzzle.height }} - 
                  {{ puzzle.islandCount }} îles
                </span>
              </div>
              <span class="puzzle-difficulty">
                {{ puzzle.difficulty === 1 ? 'Facile' : 
                   puzzle.difficulty === 2 ? 'Moyen' : 
                   puzzle.difficulty === 3 ? 'Difficile' : 'Expert' }}
              </span>
            </div>
          </div>
        </div>

        <div v-else class="empty">
          Aucun puzzle disponible. Cliquez sur "Générer" pour en créer un !
        </div>
      </div>

      <div class="card">
        <h3>✅ Configuration Réussie !</h3>
        <ul class="success-list">
          <li>✓ Backend ASP.NET Core fonctionnel</li>
          <li>✓ Client Vue.js connecté</li>
          <li>✓ API REST accessible</li>
          <li>✓ Base de données SQLite créée</li>
        </ul>
        <p class="note">
          Pour voir la documentation API complète, visitez : 
          <a href="https://localhost:5001/swagger" target="_blank">
            https://localhost:5001/swagger
          </a>
        </p>
      </div>
    </main>
  </div>
</template>

<style scoped>
#app {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  padding: 2rem;
}

.header {
  text-align: center;
  color: white;
  margin-bottom: 3rem;
}

.header h1 {
  font-size: 3rem;
  margin: 0;
  text-shadow: 0 4px 8px rgba(0, 0, 0, 0.2);
}

.subtitle {
  font-size: 1.25rem;
  opacity: 0.9;
  margin-top: 0.5rem;
}

.main {
  max-width: 800px;
  margin: 0 auto;
  display: flex;
  flex-direction: column;
  gap: 2rem;
}

.card {
  background: white;
  border-radius: 1rem;
  padding: 2rem;
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
}

.card h2 {
  margin-top: 0;
  color: #2d3748;
}

.card h3 {
  color: #4a5568;
}

.btn {
  padding: 1rem 2rem;
  border: none;
  border-radius: 0.5rem;
  font-size: 1.125rem;
  font-weight: 600;
  cursor: pointer;
  transition: all 0.2s ease;
  width: 100%;
  margin-bottom: 1rem;
}

.btn-primary {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
}

.btn-primary:hover:not(:disabled) {
  transform: translateY(-2px);
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.2);
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.error {
  padding: 1rem;
  background: #fed7d7;
  border: 1px solid #fc8181;
  border-radius: 0.5rem;
  color: #c53030;
  margin-bottom: 1rem;
}

.loading {
  text-align: center;
  padding: 2rem;
  color: #718096;
  font-size: 1.125rem;
}

.empty {
  text-align: center;
  padding: 2rem;
  color: #a0aec0;
  font-style: italic;
}

.puzzles {
  margin-top: 1rem;
}

.puzzle-list {
  display: flex;
  flex-direction: column;
  gap: 0.75rem;
  margin-top: 1rem;
}

.puzzle-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem;
  background: #f7fafc;
  border-radius: 0.5rem;
  border: 1px solid #e2e8f0;
}

.puzzle-info {
  display: flex;
  flex-direction: column;
  gap: 0.25rem;
}

.puzzle-detail {
  font-size: 0.875rem;
  color: #718096;
}

.puzzle-difficulty {
  padding: 0.25rem 0.75rem;
  background: #667eea;
  color: white;
  border-radius: 0.25rem;
  font-size: 0.875rem;
  font-weight: 600;
}

.success-list {
  list-style: none;
  padding: 0;
  margin: 1rem 0;
}

.success-list li {
  padding: 0.5rem 0;
  color: #2d3748;
  font-size: 1.125rem;
}

.note {
  margin-top: 1rem;
  padding: 1rem;
  background: #fef5e7;
  border: 1px solid #f9e79f;
  border-radius: 0.5rem;
  color: #7d6608;
}

.note a {
  color: #667eea;
  font-weight: 600;
  text-decoration: none;
}

.note a:hover {
  text-decoration: underline;
}
</style>
