<template>
  <section class="w-full h-full px-4 lg:px-10 py-8 relative overflow-y-auto">
    <!-- Effet de barreaux en arrière-plan -->
    <div class="absolute inset-0 overflow-hidden pointer-events-none opacity-5">
      <div class="absolute inset-0" style="background-image: repeating-linear-gradient(90deg, transparent, transparent 60px, rgba(6, 182, 212, 0.15) 60px, rgba(6, 182, 212, 0.15) 62px);"></div>
      <div class="absolute inset-0" style="background-image: repeating-linear-gradient(0deg, transparent, transparent 60px, rgba(168, 85, 247, 0.15) 60px, rgba(168, 85, 247, 0.15) 62px);"></div>
    </div>

    <!-- Effet de particules en arrière-plan -->
    <div class="absolute inset-0 overflow-hidden pointer-events-none">
      <div 
        v-for="i in 15" 
        :key="i" 
        class="particle"
        :style="{
          left: `${Math.random() * 100}%`,
          animationDelay: `${Math.random() * 5}s`,
          animationDuration: `${3 + Math.random() * 4}s`
        }"
      ></div>
    </div>

    <!-- Icônes de barreaux décoratives -->
    <div class="absolute top-10 right-4 sm:right-10 opacity-10 pointer-events-none hidden sm:block">
      <IconBars class="w-24 sm:w-32 h-24 sm:h-32 text-cyan-500" />
    </div>
    <div class="absolute bottom-10 left-4 sm:left-10 opacity-10 pointer-events-none hidden sm:block">
      <IconBars class="w-16 sm:w-24 h-16 sm:h-24 text-purple-500" />
    </div>

    <div class="w-full max-w-7xl mx-auto relative z-10">
      <!-- Titre + intro -->
      <header class="mb-8 animate-fade-in">
        <div class="flex items-center gap-3 mb-3">
          <div class="h-1 w-12 bg-gradient-to-r from-cyan-400 via-purple-500 to-pink-500 rounded-full"></div>
          <p class="text-sm uppercase tracking-[0.3em] text-cyan-300 font-semibold">
            Prison Break
          </p>
        </div>
        <h1 class="text-4xl sm:text-5xl lg:text-6xl font-extrabold mb-4 bg-gradient-to-r from-cyan-400 via-purple-400 to-pink-400 bg-clip-text text-transparent drop-shadow-lg">
          {{ greeting }}
        </h1>
        <p class="text-base sm:text-lg text-zinc-300 max-w-2xl leading-relaxed">
          Bienvenue dans ta cellule. Relie les verrous entre eux pour t'échapper. 
          Chaque puzzle résolu te rapproche de la liberté.
        </p>
      </header>

      <!-- Featured Game Section -->
      <section class="mb-10 animate-slide-up" style="animation-delay: 0.1s">
        <div class="relative rounded-3xl overflow-hidden bg-gradient-to-br from-zinc-900/95 to-zinc-800/95 border border-zinc-700/50 shadow-2xl backdrop-blur-xl">
          <div class="grid md:grid-cols-2 gap-0">
            <!-- Image du jeu featured -->
            <div class="relative h-64 md:h-80 bg-gradient-to-br from-cyan-600/20 via-zinc-900 to-zinc-800 overflow-hidden">
              <div class="absolute inset-0 bg-gradient-to-r from-cyan-500/10 via-purple-500/10 to-transparent"></div>
              <div class="absolute inset-0 flex items-center justify-center">
                <IconBridge class="w-32 h-32 opacity-20 text-cyan-400" />
              </div>
              <div class="absolute top-4 left-4">
                <span class="px-3 py-1.5 rounded-full text-xs font-bold bg-gradient-to-r from-cyan-500 to-purple-500 text-white shadow-lg shadow-cyan-500/40">
                  POPULAIRE
                </span>
              </div>
            </div>

            <!-- Détails du jeu -->
            <div class="p-4 sm:p-6 lg:p-8 flex flex-col justify-between bg-gradient-to-br from-zinc-900/80 to-zinc-800/80">
              <div>
                <h2 class="text-xl sm:text-2xl lg:text-3xl font-bold text-zinc-50 mb-2">Hashi - Évasion de Cellule</h2>
                <p class="text-xs sm:text-sm text-zinc-400 mb-4">Puzzle Logique • Prison Break</p>
                
                <!-- Tags -->
                <div class="flex flex-wrap gap-2 mb-4">
                  <span class="px-3 py-1 rounded-full text-xs font-medium bg-cyan-500/20 text-cyan-300 border border-cyan-500/30">
                    Strategy
                  </span>
                  <span class="px-3 py-1 rounded-full text-xs font-medium bg-purple-500/20 text-purple-300 border border-purple-500/30">
                    Puzzle
                  </span>
                  <span class="px-3 py-1 rounded-full text-xs font-medium bg-pink-500/20 text-pink-300 border border-pink-500/30">
                    Solo
                  </span>
                </div>

                <!-- Rating -->
                <div class="flex items-center gap-2 mb-4">
                  <div class="flex">
                    <svg v-for="i in 5" :key="i" xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-cyan-400" viewBox="0 0 20 20" fill="currentColor">
                      <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                    </svg>
                  </div>
                  <span class="text-sm font-bold text-zinc-50">5.0</span>
                  <span class="text-xs text-zinc-400">(124 avis)</span>
                </div>

                <!-- Description -->
                <p class="text-sm text-zinc-300 leading-relaxed mb-6">
                  Relie les verrous entre eux pour débloquer ta cellule. 
                  Plus tu progresses, plus les défis deviennent intéressants.
                </p>
              </div>

              <!-- Bouton d'action -->
              <button
                @click="onPlay"
                class="w-full px-6 py-3.5 rounded-xl bg-gradient-to-r from-cyan-500 via-purple-500 to-pink-500 text-white text-sm font-bold tracking-wide hover:from-cyan-400 hover:via-purple-400 hover:to-pink-400 transition-all duration-300 shadow-lg shadow-cyan-500/40 hover:shadow-xl hover:shadow-cyan-500/60 hover:scale-[1.02] active:scale-95 flex items-center justify-center gap-2"
              >
                <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M14.752 11.168l-3.197-2.132A1 1 0 0010 9.87v4.263a1 1 0 001.555.832l3.197-2.132a1 1 0 000-1.664z" />
                  <path stroke-linecap="round" stroke-linejoin="round" d="M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                </svg>
                <span>Jouer maintenant</span>
              </button>
            </div>
          </div>
        </div>
      </section>

      <!-- Catégories de jeux -->
      <section class="mb-8 animate-slide-up" style="animation-delay: 0.2s">
        <div class="flex items-center gap-3 mb-4">
          <h3 class="text-lg font-bold text-zinc-50">Catégories de Jeux</h3>
        </div>
        <div class="flex flex-wrap gap-3">
          <button
            v-for="(category, index) in gameCategories"
            :key="category.name"
            @click="selectCategory(category.name)"
            class="px-5 py-2.5 rounded-xl text-sm font-semibold transition-all duration-300 hover:scale-105"
            :class="selectedCategory === category.name
              ? 'bg-gradient-to-r from-cyan-500 via-purple-500 to-pink-500 text-white shadow-lg shadow-cyan-500/40'
              : 'bg-zinc-800/60 text-zinc-300 hover:bg-zinc-700/60 border border-zinc-700/50'"
            :style="{ animationDelay: `${0.2 + index * 0.05}s` }"
          >
            {{ category.name }}
          </button>
        </div>
      </section>

      <!-- Statut du prisonnier -->
      <section class="mb-8 animate-slide-up">
        <div class="flex flex-col sm:flex-row items-start sm:items-center gap-3 sm:gap-4 p-4 rounded-2xl bg-zinc-900/60 backdrop-blur-sm border border-zinc-800/50 shadow-xl relative overflow-hidden">
          <!-- Effet de barreaux subtil -->
          <div class="absolute right-0 top-0 bottom-0 w-px bg-gradient-to-b from-transparent via-cyan-500/20 to-transparent hidden sm:block"></div>
          
          <div class="flex items-center gap-3 flex-1">
            <div class="relative">
              <div 
                class="h-3 w-3 rounded-full animate-pulse"
                :class="isLoggedIn ? 'bg-emerald-500 shadow-lg shadow-emerald-500/50' : 'bg-cyan-500 shadow-lg shadow-cyan-500/50'"
              ></div>
              <div 
                class="absolute inset-0 rounded-full animate-ping"
                :class="isLoggedIn ? 'bg-emerald-500' : 'bg-cyan-500'"
                style="animation-duration: 2s; opacity: 0.3;"
              ></div>
            </div>
            <span class="text-xs sm:text-sm font-medium text-zinc-200">
              {{ isLoggedIn ? 'Connecté' : 'Mode invité' }}
            </span>
          </div>
          <span
            class="px-3 sm:px-4 py-1.5 sm:py-2 rounded-full text-[10px] sm:text-xs font-bold tracking-wider transition-all duration-300 transform hover:scale-105"
            :class="
              isLoggedIn
                ? 'bg-gradient-to-r from-emerald-500 to-teal-500 text-white shadow-lg shadow-emerald-500/30'
                : 'bg-gradient-to-r from-cyan-500 to-purple-500 text-white shadow-lg shadow-cyan-500/40'
            "
          >
            {{ isLoggedIn ? 'SESSION ACTIVE' : 'SESSION INVITÉE' }}
          </span>
        </div>
      </section>

      <!-- Cartes visuelles -->
      <section
        class="grid grid-cols-1 md:grid-cols-2 gap-6 md:gap-8 mb-10"
      >
        <!-- Carte puzzle / verrous -->
        <div
          class="group rounded-3xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 shadow-2xl p-6 flex flex-col justify-between backdrop-blur-sm transition-all duration-500 hover:shadow-orange-500/20 hover:border-orange-500/30 hover:scale-[1.02] animate-slide-up"
          style="animation-delay: 0.1s"
        >
          <div class="mb-4">
            <div class="flex items-center gap-2 mb-2">
              <div class="h-1 w-8 bg-gradient-to-r from-cyan-500 to-purple-500 rounded-full"></div>
              <p class="text-xs uppercase tracking-[0.2em] text-cyan-300 font-semibold">
                Plan de la cellule
              </p>
            </div>
            <p class="text-sm text-zinc-200 leading-relaxed">
              Voici la carte de ta cellule. Relie les verrous pour t'échapper.
            </p>
          </div>

          <!-- Faux visuel de puzzle/verrous amélioré avec barreaux -->
          <div
            class="mt-4 flex-1 rounded-2xl bg-gradient-to-br from-zinc-800/80 to-zinc-900/80 border border-zinc-700/50 grid grid-cols-3 grid-rows-3 place-items-center gap-3 px-5 py-4 shadow-inner group-hover:border-cyan-500/30 transition-colors duration-300 relative overflow-hidden"
          >
            <!-- Effet de barreaux en arrière-plan -->
            <div class="absolute inset-0 opacity-5 pointer-events-none">
              <div class="absolute inset-0" style="background-image: repeating-linear-gradient(0deg, transparent, transparent 20px, rgba(6, 182, 212, 0.3) 20px, rgba(6, 182, 212, 0.3) 22px), repeating-linear-gradient(90deg, transparent, transparent 20px, rgba(168, 85, 247, 0.3) 20px, rgba(168, 85, 247, 0.3) 22px);"></div>
            </div>
            
            <!-- Verrous/îles -->
            <div class="relative z-10 h-10 w-10 rounded-full bg-gradient-to-br from-zinc-700 to-zinc-800 border-2 border-cyan-500/50 flex items-center justify-center text-xs font-bold text-cyan-300 shadow-lg shadow-cyan-500/20 group-hover:scale-110 transition-transform">
              1
            </div>
            <div class="relative z-10 h-10 w-10 rounded-full bg-gradient-to-br from-zinc-700 to-zinc-800 border-2 border-cyan-500/50 flex items-center justify-center text-xs font-bold text-cyan-300 shadow-lg shadow-cyan-500/20 group-hover:scale-110 transition-transform">
              2
            </div>
            <div class="relative z-10 h-10 w-10 rounded-full bg-gradient-to-br from-zinc-700 to-zinc-800 border-2 border-cyan-500/50 flex items-center justify-center text-xs font-bold text-cyan-300 shadow-lg shadow-cyan-500/20 group-hover:scale-110 transition-transform">
              2
            </div>
            <div class="relative z-10 col-span-3 h-1.5 w-4/5 bg-gradient-to-r from-transparent via-cyan-500/60 to-transparent rounded-full shadow-lg shadow-cyan-500/30"></div>
            <div class="relative z-10 h-10 w-10 rounded-full bg-gradient-to-br from-zinc-700 to-zinc-800 border-2 border-cyan-500/50 flex items-center justify-center text-xs font-bold text-cyan-300 shadow-lg shadow-cyan-500/20 group-hover:scale-110 transition-transform">
              3
            </div>
            <div class="relative z-10 h-10 w-10 rounded-full bg-gradient-to-br from-zinc-700 to-zinc-800 border-2 border-cyan-500/50 flex items-center justify-center text-xs font-bold text-cyan-300 shadow-lg shadow-cyan-500/20 group-hover:scale-110 transition-transform">
              4
            </div>
            <div class="relative z-10 h-10 w-10 rounded-full bg-gradient-to-br from-zinc-700 to-zinc-800 border-2 border-cyan-500/50 flex items-center justify-center text-xs font-bold text-cyan-300 shadow-lg shadow-cyan-500/20 group-hover:scale-110 transition-transform">
              1
            </div>
          </div>
        </div>

        <!-- Carte prisonnier / mugshot -->
        <div
          class="group rounded-3xl bg-gradient-to-br from-zinc-900/90 to-zinc-800/90 border border-zinc-700/50 shadow-2xl p-6 flex flex-col justify-between backdrop-blur-sm transition-all duration-500 hover:shadow-purple-500/20 hover:border-purple-500/30 hover:scale-[1.02] animate-slide-up"
          style="animation-delay: 0.2s"
        >
          <div class="mb-4">
            <div class="flex items-center gap-2 mb-2">
              <div class="h-1 w-8 bg-gradient-to-r from-purple-500 to-pink-500 rounded-full"></div>
              <p class="text-xs uppercase tracking-[0.2em] text-purple-300 font-semibold">
                Ton profil
              </p>
            </div>
            <p class="text-sm text-zinc-200 leading-relaxed">
              Ton identité et tes statistiques. Connecte-toi pour sauvegarder ta progression.
            </p>
          </div>

          <!-- Contenu différent selon l'état connexion -->
          <div
            class="mt-4 flex-1 rounded-2xl bg-gradient-to-br from-zinc-800/80 to-zinc-900/80 border border-zinc-700/50 flex items-center justify-center px-5 py-4 shadow-inner group-hover:border-purple-500/30 transition-colors duration-300"
          >
            <!-- AVANT enregistrement : prisonnier générique fâché -->
            <div
              v-if="!isLoggedIn"
              class="flex flex-col items-center gap-3 text-xs text-zinc-200"
            >
              <div class="relative">
                <IconPrisoner class="h-20 w-20 drop-shadow-xl animate-pulse" />
                <div class="absolute -top-1 -right-1">
                  <IconBars class="w-6 h-6 text-cyan-500/50" />
                </div>
              </div>
              <p class="font-bold text-sm">Prisonnier #000</p>
              <p class="text-[11px] text-zinc-400 text-center max-w-[200px] leading-relaxed">
                Pas encore de compte ? Crée ton profil pour sauvegarder ta progression.
              </p>
            </div>

            <!-- APRÈS enregistrement : mugshot -->
            <div
              v-else
              class="flex flex-col sm:flex-row items-center sm:items-start gap-3 sm:gap-5 w-full"
            >
              <!-- Silhouette du prisonnier -->
              <div class="relative">
                <IconPrisoner class="h-20 w-20 sm:h-24 sm:w-24 drop-shadow-xl transition-transform group-hover:scale-110" />
                <div class="absolute -top-1 -right-1 opacity-60">
                  <IconBars class="w-5 h-5 sm:w-6 sm:h-6 text-cyan-500" />
                </div>
              </div>
              <!-- Ardoise + infos -->
              <div class="flex-1 w-full sm:w-auto">
                <div
                  class="w-full rounded-xl bg-gradient-to-br from-zinc-950 to-zinc-900 border-2 border-cyan-500/30 px-3 sm:px-4 py-2 sm:py-3 mb-2 sm:mb-3 flex flex-col items-center shadow-lg"
                >
                  <span class="text-[10px] sm:text-xs font-semibold text-cyan-300 uppercase tracking-wider mb-1">
                    {{ userNameLabel }}
                  </span>
                  <span class="text-base sm:text-lg font-mono font-bold text-zinc-50 bg-zinc-800 px-2 sm:px-3 py-1 rounded-lg">
                    #{{ userIdLabel }}
                  </span>
                </div>
                <p class="text-[10px] sm:text-[11px] text-zinc-400 leading-relaxed text-center">
                  Identité utilisée pour suivre tes cellules résolues, tes temps d'évasion et ton classement.
                </p>
              </div>
            </div>
          </div>
        </div>
      </section>

      <!-- Jeux les plus populaires -->
      <section class="mb-10 animate-slide-up" style="animation-delay: 0.3s">
        <div class="flex items-center justify-between mb-6">
          <h3 class="text-xl font-bold text-zinc-50">Jeux les plus populaires</h3>
          <button
            @click="uiStore.goToGames()"
            class="text-sm text-cyan-400 hover:text-cyan-300 font-medium transition-colors flex items-center gap-1"
          >
            Voir tout
            <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M9 5l7 7-7 7" />
            </svg>
          </button>
        </div>
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
          <!-- Carte Hashi -->
          <div
            @click="uiStore.goToLevels()"
            class="group relative rounded-2xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 overflow-hidden shadow-xl hover:shadow-orange-500/20 hover:border-orange-500/30 transition-all duration-300 hover:scale-[1.02] cursor-pointer"
          >
            <div class="relative h-40 bg-gradient-to-br from-cyan-600/30 to-zinc-900 flex items-center justify-center">
              <IconBridge class="w-16 h-16 opacity-40 text-cyan-400" />
              <div class="absolute top-3 right-3">
                <div class="flex items-center gap-1 bg-slate-900/80 backdrop-blur-sm px-2 py-1 rounded-lg">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-yellow-400" viewBox="0 0 20 20" fill="currentColor">
                    <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                  </svg>
                  <span class="text-xs font-bold text-slate-50">5.0</span>
                </div>
              </div>
            </div>
            <div class="p-4">
              <h4 class="text-lg font-bold text-slate-50 mb-2">Hashi</h4>
              <div class="flex flex-wrap gap-1.5 mb-3">
                <span class="px-2 py-0.5 rounded text-[10px] font-medium bg-cyan-500/20 text-cyan-300">Strategy</span>
                <span class="px-2 py-0.5 rounded text-[10px] font-medium bg-purple-500/20 text-purple-300">Puzzle</span>
              </div>
              <p class="text-xs text-slate-400 line-clamp-2">Connectez les îles avec des ponts</p>
            </div>
          </div>

          <!-- Carte TicTacToe -->
          <div
            @click="uiStore.goToTicTacToe()"
            class="group relative rounded-2xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 overflow-hidden shadow-xl hover:shadow-blue-500/20 hover:border-blue-500/30 transition-all duration-300 hover:scale-[1.02] cursor-pointer"
          >
            <div class="relative h-40 bg-gradient-to-br from-cyan-600/30 to-slate-900 flex items-center justify-center">
              <IconTicTacToe class="w-16 h-16 opacity-40 text-cyan-400" />
              <div class="absolute top-3 right-3">
                <div class="flex items-center gap-1 bg-slate-900/80 backdrop-blur-sm px-2 py-1 rounded-lg">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-yellow-400" viewBox="0 0 20 20" fill="currentColor">
                    <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                  </svg>
                  <span class="text-xs font-bold text-slate-50">5.0</span>
                </div>
              </div>
            </div>
            <div class="p-4">
              <h4 class="text-lg font-bold text-slate-50 mb-2">Tic-Tac-Toe</h4>
              <div class="flex flex-wrap gap-1.5 mb-3">
                <span class="px-2 py-0.5 rounded text-[10px] font-medium bg-cyan-500/20 text-cyan-300">Strategy</span>
                <span class="px-2 py-0.5 rounded text-[10px] font-medium bg-cyan-500/20 text-cyan-300">Multijoueur</span>
              </div>
              <p class="text-xs text-slate-400 line-clamp-2">Le classique jeu de morpion</p>
            </div>
          </div>

          <!-- Carte Connect Four -->
          <div
            @click="uiStore.goToConnectFour()"
            class="group relative rounded-2xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 overflow-hidden shadow-xl hover:shadow-red-500/20 hover:border-red-500/30 transition-all duration-300 hover:scale-[1.02] cursor-pointer"
          >
            <div class="relative h-40 bg-gradient-to-br from-rose-600/30 to-slate-900 flex items-center justify-center">
              <IconConnectFour class="w-16 h-16 opacity-40 text-rose-400" />
              <div class="absolute top-3 right-3">
                <div class="flex items-center gap-1 bg-slate-900/80 backdrop-blur-sm px-2 py-1 rounded-lg">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-yellow-400" viewBox="0 0 20 20" fill="currentColor">
                    <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                  </svg>
                  <span class="text-xs font-bold text-slate-50">5.0</span>
                </div>
              </div>
            </div>
            <div class="p-4">
              <h4 class="text-lg font-bold text-slate-50 mb-2">Connect Four</h4>
              <div class="flex flex-wrap gap-1.5 mb-3">
                <span class="px-2 py-0.5 rounded text-[10px] font-medium bg-rose-500/20 text-rose-300">Strategy</span>
                <span class="px-2 py-0.5 rounded text-[10px] font-medium bg-rose-500/20 text-rose-300">Alignement</span>
              </div>
              <p class="text-xs text-slate-400 line-clamp-2">Alignez 4 pièces pour gagner</p>
            </div>
          </div>

          <!-- Carte Aventure -->
          <div
            @click="uiStore.goToAdventure()"
            class="group relative rounded-2xl bg-gradient-to-br from-slate-900/90 to-slate-800/90 border border-slate-700/50 overflow-hidden shadow-xl hover:shadow-emerald-500/20 hover:border-emerald-500/30 transition-all duration-300 hover:scale-[1.02] cursor-pointer"
          >
            <div class="relative h-40 bg-gradient-to-br from-emerald-600/30 to-slate-900 flex items-center justify-center">
              <IconKey class="w-16 h-16 opacity-40 text-emerald-400" />
              <div class="absolute top-3 right-3">
                <div class="flex items-center gap-1 bg-slate-900/80 backdrop-blur-sm px-2 py-1 rounded-lg">
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-yellow-400" viewBox="0 0 20 20" fill="currentColor">
                    <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                  </svg>
                  <span class="text-xs font-bold text-slate-50">4.8</span>
                </div>
              </div>
            </div>
            <div class="p-4">
              <h4 class="text-lg font-bold text-slate-50 mb-2">Aventure</h4>
              <div class="flex flex-wrap gap-1.5 mb-3">
                <span class="px-2 py-0.5 rounded text-[10px] font-medium bg-emerald-500/20 text-emerald-300">Aventure</span>
                <span class="px-2 py-0.5 rounded text-[10px] font-medium bg-emerald-500/20 text-emerald-300">Énigmes</span>
              </div>
              <p class="text-xs text-slate-400 line-clamp-2">Explorez et résolvez des énigmes</p>
            </div>
          </div>
        </div>
      </section>

      <!-- Boutons principaux -->
      <section class="flex flex-wrap gap-4 animate-slide-up" style="animation-delay: 0.4s">
        <button
          v-if="!isLoggedIn"
          class="group relative px-8 py-4 rounded-full bg-gradient-to-r from-orange-500 to-orange-600 text-slate-900 text-sm font-bold tracking-wide hover:from-orange-400 hover:to-orange-500 transition-all duration-300 shadow-lg shadow-orange-500/30 hover:shadow-xl hover:shadow-orange-500/40 hover:scale-105 active:scale-95 overflow-hidden"
          @click="onPlayOrRegister"
        >
          <span class="relative z-10 flex items-center gap-2">
            <span>🎮</span>
            <span>Jouer / S'inscrire</span>
            <span class="opacity-0 group-hover:opacity-100 transition-opacity">→</span>
          </span>
          <div class="absolute inset-0 bg-gradient-to-r from-orange-400 to-orange-500 opacity-0 group-hover:opacity-100 transition-opacity"></div>
        </button>

        <button
          v-else
          class="group relative px-8 py-4 rounded-full bg-gradient-to-r from-green-500 to-green-600 text-slate-900 text-sm font-bold tracking-wide hover:from-green-400 hover:to-green-500 transition-all duration-300 shadow-lg shadow-green-500/30 hover:shadow-xl hover:shadow-green-500/40 hover:scale-105 active:scale-95 overflow-hidden"
          @click="onPlay"
        >
          <span class="relative z-10 flex items-center gap-2">
            <span>🎮</span>
            <span>Jouer</span>
            <span class="opacity-0 group-hover:opacity-100 transition-opacity">→</span>
          </span>
          <div class="absolute inset-0 bg-gradient-to-r from-green-400 to-green-500 opacity-0 group-hover:opacity-100 transition-opacity"></div>
        </button>

        <button
          class="group relative px-8 py-4 rounded-full bg-gradient-to-r from-slate-800 to-slate-900 text-slate-100 text-sm font-bold tracking-wide hover:from-slate-700 hover:to-slate-800 transition-all duration-300 shadow-lg shadow-slate-900/30 hover:shadow-xl hover:shadow-slate-900/40 hover:scale-105 active:scale-95 border border-slate-700 hover:border-slate-600 overflow-hidden"
          @click="onTutorial"
        >
          <span class="relative z-10 flex items-center gap-2">
            <span>📖</span>
            <span>Tutoriel</span>
            <span class="opacity-0 group-hover:opacity-100 transition-opacity">→</span>
          </span>
          <div class="absolute inset-0 bg-gradient-to-r from-slate-700 to-slate-800 opacity-0 group-hover:opacity-100 transition-opacity"></div>
        </button>
      </section>
    </div>
  </section>
</template>

<script setup lang="ts">
import { computed, ref } from 'vue';
import { useUserStore } from '@/stores/user';
import { useUiStore } from '@/stores/ui';
import IconPrisoner from '@/components/icons/IconPrisoner.vue';
import IconBars from '@/components/icons/IconBars.vue';
import IconBridge from '@/components/icons/IconBridge.vue';
import IconTicTacToe from '@/components/icons/IconTicTacToe.vue';
import IconConnectFour from '@/components/icons/IconConnectFour.vue';
import IconKey from '@/components/icons/IconKey.vue';

const userStore = useUserStore();
const uiStore = useUiStore();

const isLoggedIn = computed(() => userStore.isLoggedIn);
const selectedCategory = ref<string>('Tous les jeux');

const gameCategories = [
  { name: 'Tous les jeux', icon: '🎮' },
  { name: "Aventure's", icon: '🗝️' },
  { name: 'Stratégie', icon: '🧩' },
  { name: 'Sport', icon: '⚽' },
  { name: 'Combat', icon: '⚔️' },
  { name: 'Puzzle', icon: '🌉' }
];

function selectCategory(category: string) {
  selectedCategory.value = category;
  if (category === 'Puzzle' || category === 'Tous les jeux') {
    uiStore.goToLevels();
  } else if (category === "Aventure's") {
    uiStore.goToAdventure();
  } else {
    uiStore.goToGames();
  }
}

// Titre principal
const greeting = computed(() =>
  userStore.user ? `Bonsoir, ${userStore.user.name}` : 'Bonsoir, Joueur'
);

// Nom affiché sur l'ardoise après enregistrement
const userNameLabel = computed(() =>
  userStore.user ? userStore.user.name.toUpperCase() : 'PRISONNIER'
);

// Numéro de prisonnier (#000 avant enregistrement, #XYZ après)
const userIdLabel = computed(() => {
  if (!userStore.user) {
    return '000';
  }
  return userStore.user.id.toString().padStart(3, '0');
});

function onPlayOrRegister() {
  uiStore.openUserModal();
}

function onPlay() {
  uiStore.goToLevels();
}

function onTutorial() {
  uiStore.openTutorialModal();
}
</script>

<style scoped>
/* Particules animées */
.particle {
  position: absolute;
  width: 4px;
  height: 4px;
  background: rgba(251, 146, 60, 0.4);
  border-radius: 50%;
  animation: float linear infinite;
  box-shadow: 0 0 10px rgba(251, 146, 60, 0.5);
}

@keyframes float {
  0% {
    transform: translateY(100vh) translateX(0);
    opacity: 0;
  }
  10% {
    opacity: 1;
  }
  90% {
    opacity: 1;
  }
  100% {
    transform: translateY(-100px) translateX(100px);
    opacity: 0;
  }
}

/* Animations d'entrée */
@keyframes fade-in {
  from {
    opacity: 0;
  }
  to {
    opacity: 1;
  }
}

@keyframes slide-up {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.animate-fade-in {
  animation: fade-in 0.8s ease-out;
}

.animate-slide-up {
  animation: slide-up 0.8s ease-out both;
}
</style>
