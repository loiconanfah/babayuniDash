<template>
  <div class="tic-tac-toe-board">
    <div class="board-container">
      <div
        v-for="(cell, index) in game.board"
        :key="index"
        class="cell"
        :class="{
          'cell-x': cell === 'X',
          'cell-o': cell === 'O',
          'cell-empty': cell === '',
          'cell-disabled': !canPlay(index) || isGameOver
        }"
        @click="handleCellClick(index)"
      >
        <span v-if="cell === 'X'" class="symbol symbol-x">X</span>
        <span v-else-if="cell === 'O'" class="symbol symbol-o">O</span>
        <span v-else class="symbol symbol-empty"></span>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed } from 'vue'
import { useTicTacToeStore } from '@/stores/ticTacToe'
import type { TicTacToeGame } from '@/types'

const props = defineProps<{
  game: TicTacToeGame
}>()

const emit = defineEmits<{
  cellClick: [position: number]
}>()

const ticTacToeStore = useTicTacToeStore()

const isGameOver = computed(() => ticTacToeStore.isGameOver)
const isMyTurn = computed(() => ticTacToeStore.isMyTurn)

/**
 * Vérifie si on peut jouer sur cette case
 */
function canPlay(position: number): boolean {
  if (isGameOver.value) return false
  if (!isMyTurn.value) return false
  if (props.game.board[position] !== '') return false
  return true
}

/**
 * Gère le clic sur une case
 */
function handleCellClick(position: number): void {
  if (canPlay(position)) {
    emit('cellClick', position)
  }
}
</script>

<style scoped>
.tic-tac-toe-board {
  display: flex;
  justify-content: center;
  align-items: center;
  padding: 20px;
}

.board-container {
  display: grid;
  grid-template-columns: repeat(3, 1fr);
  grid-template-rows: repeat(3, 1fr);
  gap: 4px;
  background-color: #333;
  padding: 4px;
  border-radius: 8px;
  box-shadow: 0 4px 6px rgba(0, 0, 0, 0.3);
  max-width: 400px;
  width: 100%;
  aspect-ratio: 1;
}

.cell {
  background-color: #fff;
  border-radius: 4px;
  display: flex;
  justify-content: center;
  align-items: center;
  cursor: pointer;
  transition: all 0.2s ease;
  min-height: 80px;
}

.cell:hover:not(.cell-disabled):not(.cell-empty) {
  background-color: #f0f0f0;
}

.cell-empty:hover:not(.cell-disabled) {
  background-color: #e8f5e9;
  transform: scale(0.95);
}

.cell-disabled {
  cursor: not-allowed;
  opacity: 0.6;
}

.cell-x {
  background-color: #fff3e0;
}

.cell-o {
  background-color: #e3f2fd;
}

.symbol {
  font-size: 3rem;
  font-weight: bold;
  user-select: none;
}

.symbol-x {
  color: #ff6b35;
}

.symbol-o {
  color: #4a90e2;
}

.symbol-empty {
  display: none;
}

@media (max-width: 600px) {
  .symbol {
    font-size: 2rem;
  }
  
  .cell {
    min-height: 60px;
  }
}
</style>

