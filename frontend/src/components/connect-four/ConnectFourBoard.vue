<template>
  <div class="connect-four-board">
    <div class="board-container">
      <!-- Colonnes cliquables pour laisser tomber les pièces -->
      <div
        v-for="(column, colIndex) in game.board"
        :key="colIndex"
        class="column"
        :class="{
          'column-disabled': !canPlay(colIndex) || isGameOver,
          'column-hover': canPlay(colIndex) && !isGameOver
        }"
        @click="handleColumnClick(colIndex)"
      >
        <!-- Cases de la colonne (de haut en bas) -->
        <div
          v-for="(cell, rowIndex) in column"
          :key="rowIndex"
          class="cell"
          :class="{
            'cell-player1': cell === 1,
            'cell-player2': cell === 2,
            'cell-empty': cell === 0
          }"
        >
          <div v-if="cell === 1" class="piece piece-player1"></div>
          <div v-else-if="cell === 2" class="piece piece-player2"></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useConnectFourStore } from '@/stores/connectFour'
import type { ConnectFourGame } from '@/types'

const props = defineProps<{
  game: ConnectFourGame
}>()

const emit = defineEmits<{
  columnClick: [column: number]
}>()

const connectFourStore = useConnectFourStore()

const isGameOver = computed(() => connectFourStore.isGameOver)
const isMyTurn = computed(() => connectFourStore.isMyTurn)

/**
 * Vérifie si on peut jouer dans cette colonne
 */
function canPlay(column: number): boolean {
  if (isGameOver.value) return false
  if (!isMyTurn.value) return false
  
  // Vérifier si la colonne a de la place (la case du haut est vide)
  const columnData = props.game.board[column]
  if (!columnData || columnData[0] !== 0) return false
  
  return true
}

/**
 * Gère le clic sur une colonne
 */
function handleColumnClick(column: number): void {
  if (canPlay(column)) {
    emit('columnClick', column)
  }
}
</script>

<style scoped>
.connect-four-board {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 20px;
}

.board-container {
  display: grid;
  grid-template-columns: repeat(7, 1fr);
  gap: 8px;
  background: linear-gradient(135deg, #1e3a8a 0%, #3b82f6 100%);
  padding: 12px;
  border-radius: 12px;
  box-shadow: 0 8px 16px rgba(0, 0, 0, 0.3);
  max-width: 600px;
  width: 100%;
}

.column {
  display: flex;
  flex-direction: column-reverse; /* De bas en haut pour que les pièces "tombent" */
  gap: 6px;
  cursor: pointer;
  transition: all 0.2s ease;
  padding: 4px;
  border-radius: 8px;
}

.column-hover:hover {
  background-color: rgba(255, 255, 255, 0.1);
  transform: translateY(-4px);
}

.column-disabled {
  cursor: not-allowed;
  opacity: 0.6;
}

.cell {
  aspect-ratio: 1;
  background-color: #1e293b;
  border-radius: 50%;
  display: flex;
  justify-content: center;
  align-items: center;
  border: 3px solid #0f172a;
  min-height: 60px;
  transition: all 0.2s ease;
}

.cell-empty {
  background-color: #1e293b;
}

.cell-player1 {
  background-color: #1e293b;
}

.cell-player2 {
  background-color: #1e293b;
}

.piece {
  width: 90%;
  height: 90%;
  border-radius: 50%;
  box-shadow: inset 0 4px 8px rgba(0, 0, 0, 0.3);
  animation: drop 0.3s ease-out;
}

.piece-player1 {
  background: radial-gradient(circle at 30% 30%, #ef4444, #dc2626);
  border: 2px solid #991b1b;
}

.piece-player2 {
  background: radial-gradient(circle at 30% 30%, #fbbf24, #f59e0b);
  border: 2px solid #92400e;
}

@keyframes drop {
  from {
    transform: translateY(-100%);
    opacity: 0;
  }
  to {
    transform: translateY(0);
    opacity: 1;
  }
}

@media (max-width: 600px) {
  .board-container {
    gap: 4px;
    padding: 8px;
  }
  
  .cell {
    min-height: 40px;
  }
  
  .piece {
    width: 85%;
    height: 85%;
  }
}
</style>

