<template>
  <div class="h-screen w-full bg-zinc-950 text-zinc-50 flex overflow-hidden">
    <!-- Overlay pour mobile -->
    <div
      v-if="ui.isMobileMenuOpen"
      @click="ui.closeMobileMenu()"
      class="fixed inset-0 bg-black/60 backdrop-blur-sm z-40 lg:hidden"
    ></div>

    <!-- Sidebar verticale - Fixe sur desktop, drawer sur mobile -->
    <aside
      :class="[
        'bg-zinc-900/95 border-r border-zinc-800/50 flex flex-col py-6 px-4 lg:px-6 overflow-y-auto transition-transform duration-300 ease-in-out backdrop-blur-xl',
        'fixed lg:static inset-y-0 left-0 z-40',
        ui.isMobileMenuOpen ? 'translate-x-0' : '-translate-x-full lg:translate-x-0',
        'w-64 lg:w-56 xl:w-64'
      ]"
    >
      <!-- Logo / icône prisonnier -->
      <div class="flex items-center gap-2 sm:gap-3 mb-8 group cursor-pointer flex-shrink-0" @click="ui.goToHome()">
        <div
          class="h-12 w-12 sm:h-14 sm:w-14 flex-shrink-0 rounded-2xl bg-gradient-to-br from-cyan-500 via-purple-500 to-pink-500 flex items-center justify-center text-white font-bold text-lg shadow-lg shadow-cyan-500/40 transition-all duration-300 group-hover:scale-110 group-hover:rotate-3 group-hover:shadow-xl group-hover:shadow-cyan-500/60 ring-2 ring-cyan-500/30 overflow-hidden"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 sm:h-7 sm:w-7" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2.5">
            <path stroke-linecap="round" stroke-linejoin="round" d="M13.828 10.172a4 4 0 00-5.656 0l-4 4a4 4 0 105.656 5.656l1.102-1.101m-.758-4.899a4 4 0 005.656 0l4-4a4 4 0 00-5.656-5.656l-1.1 1.1" />
          </svg>
        </div>
        <div class="flex flex-col min-w-0 flex-1 overflow-hidden">
          <span class="text-sm sm:text-base font-semibold text-zinc-50 group-hover:text-cyan-400 transition-colors duration-300 truncate">Prison Break</span>
          <span class="text-[10px] sm:text-xs text-zinc-400 group-hover:text-zinc-300 transition-colors duration-300 truncate">Évasion de cellule</span>
        </div>
      </div>

      <!-- ========================
           MENU
      ============================ -->
      <nav class="flex flex-col gap-1.5 text-sm flex-1">

        <!-- Accueil -->
        <button
          class="nav-item group relative flex items-center gap-3 px-3 py-2.5 rounded-xl text-left transition-all duration-300 ease-out"
          :class="ui.currentScreen === 'home'
            ? 'bg-gradient-to-r from-cyan-500 via-purple-500 to-pink-500 text-white shadow-lg shadow-cyan-500/40 scale-[1.02]'
            : 'text-zinc-300 hover:bg-zinc-800/80 hover:text-zinc-50 hover:translate-x-1'"
          @click="ui.goToHome(); ui.closeMobileMenu()"
        >
          <div class="nav-icon-wrapper" :class="ui.currentScreen === 'home' ? 'nav-icon-active' : ''">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
            </svg>
          </div>
          <span class="font-medium">Accueil</span>
          <div v-if="ui.currentScreen === 'home'" class="absolute right-2 w-1.5 h-1.5 rounded-full bg-white animate-pulse"></div>
        </button>

        <!-- Niveaux -->
        <button
          class="nav-item group relative flex items-center gap-3 px-3 py-2.5 rounded-xl text-left transition-all duration-300 ease-out"
          :class="ui.currentScreen === 'levels'
            ? 'bg-gradient-to-r from-cyan-500 via-purple-500 to-pink-500 text-white shadow-lg shadow-cyan-500/40 scale-[1.02]'
            : 'text-zinc-300 hover:bg-zinc-800/80 hover:text-zinc-50 hover:translate-x-1'"
          @click="ui.goToLevels(); ui.closeMobileMenu()"
        >
          <div class="nav-icon-wrapper" :class="ui.currentScreen === 'levels' ? 'nav-icon-active' : ''">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-3 7h3m-3 4h3m-6-4h.01M9 16h.01" />
            </svg>
          </div>
          <span class="font-medium">Niveaux</span>
          <div v-if="ui.currentScreen === 'levels'" class="absolute right-2 w-1.5 h-1.5 rounded-full bg-white animate-pulse"></div>
        </button>

        <!-- Classement -->
        <button
          class="nav-item group relative flex items-center gap-3 px-3 py-2.5 rounded-xl text-left transition-all duration-300 ease-out"
          :class="ui.currentScreen === 'leaderboard'
            ? 'bg-gradient-to-r from-cyan-500 via-purple-500 to-pink-500 text-white shadow-lg shadow-cyan-500/40 scale-[1.02]'
            : 'text-zinc-300 hover:bg-zinc-800/80 hover:text-zinc-50 hover:translate-x-1'"
          @click="ui.goToStats(); ui.closeMobileMenu()"
        >
          <div class="nav-icon-wrapper" :class="ui.currentScreen === 'leaderboard' ? 'nav-icon-active' : ''">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M9 12l2 2 4-4M7.835 4.697a3.42 3.42 0 001.946-.806 3.42 3.42 0 014.438 0 3.42 3.42 0 001.946.806 3.42 3.42 0 013.138 3.138 3.42 3.42 0 00.806 1.946 3.42 3.42 0 010 4.438 3.42 3.42 0 00-.806 1.946 3.42 3.42 0 01-3.138 3.138 3.42 3.42 0 00-1.946.806 3.42 3.42 0 01-4.438 0 3.42 3.42 0 00-1.946-.806 3.42 3.42 0 01-3.138-3.138 3.42 3.42 0 00-.806-1.946 3.42 3.42 0 010-4.438 3.42 3.42 0 00.806-1.946 3.42 3.42 0 013.138-3.138z" />
            </svg>
          </div>
          <span class="font-medium">Classement</span>
          <div v-if="ui.currentScreen === 'leaderboard'" class="absolute right-2 w-1.5 h-1.5 rounded-full bg-white animate-pulse"></div>
        </button>

        <!-- Statistiques -->
        <button
          class="nav-item group relative flex items-center gap-3 px-3 py-2.5 rounded-xl text-left transition-all duration-300 ease-out"
          :class="ui.currentScreen === 'stats'
            ? 'bg-gradient-to-r from-cyan-500 via-purple-500 to-pink-500 text-white shadow-lg shadow-cyan-500/40 scale-[1.02]'
            : 'text-zinc-300 hover:bg-zinc-800/80 hover:text-zinc-50 hover:translate-x-1'"
          @click="ui.goToStats(); ui.closeMobileMenu()"
        >
          <div class="nav-icon-wrapper" :class="ui.currentScreen === 'stats' ? 'nav-icon-active' : ''">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
            </svg>
          </div>
          <span class="font-medium">Statistiques</span>
          <div v-if="ui.currentScreen === 'stats'" class="absolute right-2 w-1.5 h-1.5 rounded-full bg-white animate-pulse"></div>
        </button>

        <!-- Jeux -->
        <button
          class="nav-item group relative flex items-center gap-3 px-3 py-2.5 rounded-xl text-left transition-all duration-300 ease-out"
          :class="ui.currentScreen === 'games'
            ? 'bg-gradient-to-r from-cyan-500 to-cyan-600 text-white shadow-lg shadow-cyan-500/30 scale-[1.02]'
            : 'text-zinc-300 hover:bg-zinc-800/80 hover:text-zinc-50 hover:translate-x-1'"
          @click="ui.goToGames(); ui.closeMobileMenu()"
        >
          <div class="nav-icon-wrapper" :class="ui.currentScreen === 'games' ? 'nav-icon-active' : ''">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M14.751 9.75l3.501 3.75m0 0l3.499-3.75M18.252 13.5H21m-2.25 0v6.75m-9-9.75H5.25A2.25 2.25 0 003 12.75v6.75A2.25 2.25 0 005.25 22h13.5A2.25 2.25 0 0021 19.5v-6.75a2.25 2.25 0 00-2.25-2.25h-4.752m-9 0H3m2.25 0h4.752M9.75 3v3m0 0v3m0-3h3m-3 0H6.75" />
            </svg>
          </div>
          <span class="font-medium">Jeux</span>
          <div v-if="ui.currentScreen === 'games'" class="absolute right-2 w-1.5 h-1.5 rounded-full bg-white animate-pulse"></div>
        </button>

        <!-- Boutique -->
        <button
          class="nav-item group relative flex items-center gap-3 px-3 py-2.5 rounded-xl text-left transition-all duration-300 ease-out"
          :class="ui.currentScreen === 'shop'
            ? 'bg-gradient-to-r from-cyan-500 via-purple-500 to-pink-500 text-white shadow-lg shadow-cyan-500/40 scale-[1.02]'
            : 'text-zinc-300 hover:bg-zinc-800/80 hover:text-zinc-50 hover:translate-x-1'"
          @click="ui.goToShop(); ui.closeMobileMenu()"
        >
          <div class="nav-icon-wrapper" :class="ui.currentScreen === 'shop' ? 'nav-icon-active' : ''">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M16 11V7a4 4 0 00-8 0v4M5 9h14l1 12H4L5 9z" />
            </svg>
          </div>
          <span class="font-medium">Boutique</span>
          <div v-if="ui.currentScreen === 'shop'" class="absolute right-2 w-1.5 h-1.5 rounded-full bg-white animate-pulse"></div>
        </button>

        <!-- Collection -->
        <button
          class="nav-item group relative flex items-center gap-3 px-3 py-2.5 rounded-xl text-left transition-all duration-300 ease-out"
          :class="ui.currentScreen === 'collection'
            ? 'bg-gradient-to-r from-cyan-500 via-purple-500 to-pink-500 text-white shadow-lg shadow-cyan-500/40 scale-[1.02]'
            : 'text-zinc-300 hover:bg-zinc-800/80 hover:text-zinc-50 hover:translate-x-1'"
          @click="ui.goToCollection(); ui.closeMobileMenu()"
        >
          <div class="nav-icon-wrapper" :class="ui.currentScreen === 'collection' ? 'nav-icon-active' : ''">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M19 11H5m14 0a2 2 0 012 2v6a2 2 0 01-2 2H5a2 2 0 01-2-2v-6a2 2 0 012-2m14 0V9a2 2 0 00-2-2M5 11V9a2 2 0 012-2m0 0V5a2 2 0 012-2h6a2 2 0 012 2v2M7 7h10" />
            </svg>
          </div>
          <span class="font-medium">Collection</span>
          <div v-if="ui.currentScreen === 'collection'" class="absolute right-2 w-1.5 h-1.5 rounded-full bg-white animate-pulse"></div>
        </button>

        <!-- Matchs VS -->
        <button
          class="nav-item group relative flex items-center gap-3 px-3 py-2.5 rounded-xl text-left transition-all duration-300 ease-out"
          :class="ui.currentScreen === 'matches'
            ? 'bg-gradient-to-r from-cyan-500 via-purple-500 to-pink-500 text-white shadow-lg shadow-cyan-500/40 scale-[1.02]'
            : 'text-zinc-300 hover:bg-zinc-800/80 hover:text-zinc-50 hover:translate-x-1'"
          @click="ui.goToMatches(); ui.closeMobileMenu()"
        >
          <div class="nav-icon-wrapper" :class="ui.currentScreen === 'matches' ? 'nav-icon-active' : ''">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
            </svg>
          </div>
          <span class="font-medium">Matchs VS</span>
          <div v-if="ui.currentScreen === 'matches'" class="absolute right-2 w-1.5 h-1.5 rounded-full bg-white animate-pulse"></div>
        </button>

        <!-- Communauté -->
        <button
          class="nav-item group relative flex items-center gap-3 px-3 py-2.5 rounded-xl text-left transition-all duration-300 ease-out"
          :class="ui.currentScreen === 'community'
            ? 'bg-gradient-to-r from-cyan-500 via-purple-500 to-pink-500 text-white shadow-lg shadow-cyan-500/40 scale-[1.02]'
            : 'text-zinc-300 hover:bg-zinc-800/80 hover:text-zinc-50 hover:translate-x-1'"
          @click="ui.goToCommunity(); ui.closeMobileMenu()"
        >
          <div class="nav-icon-wrapper" :class="ui.currentScreen === 'community' ? 'nav-icon-active' : ''">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z" />
            </svg>
          </div>
          <span class="font-medium">Communauté</span>
          <div v-if="ui.currentScreen === 'community'" class="absolute right-2 w-1.5 h-1.5 rounded-full bg-white animate-pulse"></div>
        </button>

        <!-- Profil -->
        <button
          v-if="userStore.isLoggedIn"
          class="nav-item group relative flex items-center gap-3 px-3 py-2.5 rounded-xl text-left transition-all duration-300 ease-out"
          :class="ui.currentScreen === 'profile'
            ? 'bg-gradient-to-r from-cyan-500 via-purple-500 to-pink-500 text-white shadow-lg shadow-cyan-500/40 scale-[1.02]'
            : 'text-zinc-300 hover:bg-zinc-800/80 hover:text-zinc-50 hover:translate-x-1'"
          @click="ui.goToProfile(); ui.closeMobileMenu()"
        >
          <div class="nav-icon-wrapper" :class="ui.currentScreen === 'profile' ? 'nav-icon-active' : ''">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
            </svg>
          </div>
          <span class="font-medium">Mon Profil</span>
          <div v-if="ui.currentScreen === 'profile'" class="absolute right-2 w-1.5 h-1.5 rounded-full bg-white animate-pulse"></div>
        </button>

      </nav>

      <!-- Section Récemment joué -->
      <div class="mt-auto pt-6 border-t border-zinc-800/50">
        <h3 class="text-xs uppercase tracking-wider text-zinc-400 font-semibold mb-3 px-3">
          Récemment joué
        </h3>
        <div class="space-y-2">
          <div
            v-for="game in recentlyPlayedGames"
            :key="game.id"
            @click="navigateToGame(game.type)"
            class="group flex items-center gap-3 px-3 py-2 rounded-xl hover:bg-zinc-800/60 transition-all duration-200 cursor-pointer"
          >
              <div class="h-10 w-10 rounded-lg bg-gradient-to-br from-cyan-500/20 to-zinc-800 flex items-center justify-center">
              <IconBridge v-if="game.name === 'Hashi'" class="h-6 w-6 text-cyan-400" />
              <span v-else class="text-xl">{{ game.icon }}</span>
            </div>
            <div class="flex-1 min-w-0">
              <p class="text-sm font-medium text-zinc-200 truncate">{{ game.name }}</p>
              <div class="flex items-center gap-1 mt-0.5">
                <svg xmlns="http://www.w3.org/2000/svg" class="h-3 w-3 text-cyan-400" viewBox="0 0 20 20" fill="currentColor">
                  <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                </svg>
                <span class="text-[10px] text-zinc-400">{{ game.rating }}</span>
              </div>
            </div>
          </div>
        </div>
      </div>

      <!-- Pied de page -->
      <div class="mt-6 text-[11px] text-zinc-500">
        <p>Session Hashi Prison Break</p>
      </div>
    </aside>

    <!-- ========================
         CONTENU PRINCIPAL
    ============================ -->
    <div class="flex-1 flex flex-col overflow-hidden">

      <!-- Header - Fixe -->
      <header class="h-16 border-b border-zinc-800/50 bg-gradient-to-r from-zinc-950/98 via-zinc-900/95 to-zinc-950/98 backdrop-blur-xl px-3 sm:px-4 lg:px-8 flex items-center justify-between shadow-2xl flex-shrink-0 z-50 relative">
        <!-- Bouton menu hamburger pour mobile -->
        <button
          @click="ui.toggleMobileMenu()"
          class="lg:hidden p-2 rounded-xl hover:bg-zinc-800/60 transition-colors duration-200 mr-2"
        >
          <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-zinc-300" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
            <path stroke-linecap="round" stroke-linejoin="round" d="M4 6h16M4 12h16M4 18h16" />
          </svg>
        </button>

        <!-- Barre de recherche -->
        <div class="flex-1 max-w-2xl mx-2 sm:mx-4 hidden md:block">
          <div class="relative">
            <div class="absolute inset-y-0 left-0 pl-4 flex items-center pointer-events-none">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-zinc-400" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
            </div>
            <input
              type="text"
              placeholder="Rechercher des jeux..."
              class="w-full pl-12 pr-4 py-2.5 rounded-xl bg-zinc-800/80 border border-zinc-700/50 text-zinc-100 placeholder-zinc-400 focus:outline-none focus:ring-2 focus:ring-cyan-500/50 focus:border-cyan-500/50 transition-all duration-200 backdrop-blur-sm"
            />
          </div>
        </div>

        <!-- Status et actions -->
        <div class="flex items-center gap-2 sm:gap-4">
          <div class="flex items-center gap-2 px-3 sm:px-4 py-2 rounded-xl bg-zinc-800/60 border border-zinc-700/50 backdrop-blur-sm">
            <div class="h-2.5 w-2.5 rounded-full bg-gradient-to-r from-cyan-400 to-purple-500 animate-pulse shadow-lg shadow-cyan-500/50"></div>
            <span class="text-xs text-zinc-200 font-medium hidden xl:inline">En ligne</span>
          </div>
          <!-- Notifications -->
          <div class="relative z-[60]">
            <button 
              @click="notificationsStore.toggleNotificationsPanel()"
              class="relative p-2.5 rounded-xl hover:bg-zinc-800/60 transition-all duration-200 group border border-transparent hover:border-cyan-500/30"
            >
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-zinc-300 group-hover:text-cyan-400 transition-colors" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M15 17h5l-1.405-1.405A2.032 2.032 0 0118 14.158V11a6.002 6.002 0 00-4-5.659V5a2 2 0 10-4 0v.341C7.67 6.165 6 8.388 6 11v3.159c0 .538-.214 1.055-.595 1.436L4 17h5m6 0v1a3 3 0 11-6 0v-1m6 0H9" />
              </svg>
              <span 
                v-if="notificationsStore.unreadCount > 0"
                class="absolute -top-1 -right-1 h-5 w-5 bg-gradient-to-r from-cyan-500 to-pink-500 rounded-full border-2 border-zinc-900 flex items-center justify-center text-[10px] font-bold text-white shadow-lg shadow-cyan-500/50"
              >
                {{ notificationsStore.unreadCount > 9 ? '9+' : notificationsStore.unreadCount }}
              </span>
            </button>

            <!-- Panneau de notifications -->
            <NotificationsPanel />
          </div>

          <!-- Menu utilisateur si connecté -->
          <div v-if="userStore.isLoggedIn" class="relative z-[60]">
            <div class="flex items-center gap-2 sm:gap-3 px-2 sm:px-3 py-2 rounded-xl hover:bg-zinc-800/60 transition-all duration-200 cursor-pointer border border-zinc-700/30 hover:border-cyan-500/30" @click="toggleUserMenu">
              <div class="h-8 w-8 sm:h-9 sm:w-9 rounded-full bg-gradient-to-br from-cyan-500 via-purple-500 to-pink-500 flex items-center justify-center text-white font-bold text-xs sm:text-sm shadow-lg shadow-cyan-500/40 ring-2 ring-zinc-800 overflow-hidden">
                <img
                  v-if="userStore.equippedAvatarUrl"
                  :src="userStore.equippedAvatarUrl"
                  :alt="userStore.user?.name"
                  class="w-full h-full object-cover"
                />
                <span v-else>{{ userStore.user?.name?.charAt(0).toUpperCase() || 'U' }}</span>
              </div>
              <div class="hidden md:flex flex-col items-start">
                <span class="text-xs sm:text-sm text-zinc-100 font-semibold leading-tight truncate max-w-[120px]">{{ userStore.user?.name }}</span>
                <div class="flex items-center gap-2">
                  <span class="text-[10px] sm:text-xs text-cyan-400 font-medium">Premium</span>
                  <span class="text-[10px] sm:text-xs text-cyan-400 font-bold flex items-center gap-1">
                    <svg xmlns="http://www.w3.org/2000/svg" class="h-3 w-3" fill="currentColor" viewBox="0 0 20 20">
                      <path d="M8.433 7.418c.155-.103.346-.196.567-.267v1.698a2.305 2.305 0 01-.567-.267C8.07 8.34 8 8.114 8 8c0-.114.07-.34.433-.582zM11 12.849v-1.698c.22.071.412.164.567.267.364.243.433.468.433.582 0 .114-.07.34-.433.582a2.305 2.305 0 01-.567.267z" />
                      <path fill-rule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm1-13a1 1 0 10-2 0v.092a4.535 4.535 0 00-1.676.662C6.602 6.234 6 7.009 6 8c0 .99.602 1.765 1.324 2.246.48.32 1.054.545 1.676.662v1.941c-.391-.127-.68-.317-.843-.504a1 1 0 10-1.51 1.31c.562.649 1.413 1.076 2.353 1.253V15a1 1 0 102 0v-.092a4.535 4.535 0 001.676-.662C13.398 13.766 14 12.991 14 12c0-.99-.602-1.765-1.324-2.246A4.535 4.535 0 0011 9.092V7.151c.391.127.68.317.843.504a1 1 0 101.511-1.31c-.563-.649-1.413-1.076-2.354-1.253V5z" clip-rule="evenodd" />
                    </svg>
                    {{ userStore.coins || 0 }}
                  </span>
                </div>
              </div>
              <svg xmlns="http://www.w3.org/2000/svg" class="h-4 w-4 text-zinc-400 transition-transform duration-200 hidden md:block" :class="{ 'rotate-180': isUserMenuOpen }" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M19 9l-7 7-7-7" />
              </svg>
            </div>
            
            <!-- Menu déroulant amélioré -->
            <div v-if="isUserMenuOpen" class="absolute right-0 mt-2 w-[calc(100vw-4rem)] sm:w-72 max-w-[calc(100vw-4rem)] sm:max-w-none bg-gradient-to-br from-zinc-800 to-zinc-900 rounded-2xl shadow-2xl border border-zinc-700/50 overflow-hidden z-[70] animate-fadeIn backdrop-blur-xl">
              <div class="px-5 py-4 border-b border-zinc-700/50 bg-gradient-to-r from-cyan-500/10 to-purple-500/10">
                <p class="text-sm font-bold text-zinc-50">{{ userStore.user?.name }}</p>
                <p class="text-xs text-zinc-400 mt-1">{{ userStore.user?.email }}</p>
                <div class="mt-2 flex items-center gap-2">
                  <span class="px-2 py-0.5 rounded-full text-[10px] font-bold bg-gradient-to-r from-cyan-500 to-purple-500 text-white shadow-lg shadow-cyan-500/30">PREMIUM</span>
                  <span class="text-xs text-zinc-500">Membre actif</span>
                </div>
              </div>
              
              <div class="py-2">
                <button
                  @click="ui.goToProfile(); isUserMenuOpen = false"
                  class="w-full flex items-center gap-3 px-5 py-3 text-sm text-zinc-300 hover:bg-zinc-700/50 hover:text-cyan-400 transition-all duration-200 group"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
                  </svg>
                  <span class="font-medium">Mon Profil</span>
                </button>
                <button
                  @click="ui.goToStats()"
                  class="w-full flex items-center gap-3 px-5 py-3 text-sm text-zinc-300 hover:bg-zinc-700/50 hover:text-cyan-400 transition-all duration-200 group"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M9 19v-6a2 2 0 00-2-2H5a2 2 0 00-2 2v6a2 2 0 002 2h2a2 2 0 002-2zm0 0V9a2 2 0 012-2h2a2 2 0 012 2v10m-6 0a2 2 0 002 2h2a2 2 0 002-2m0 0V5a2 2 0 012-2h2a2 2 0 012 2v14a2 2 0 01-2 2h-2a2 2 0 01-2-2z" />
                  </svg>
                  <span class="font-medium">Mes Statistiques</span>
                </button>
                <button
                  @click="handleLogout"
                  class="w-full flex items-center gap-3 px-5 py-3 text-sm text-red-400 hover:bg-red-500/10 hover:text-red-300 transition-all duration-200 group"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 group-hover:rotate-12 transition-transform duration-200" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                    <path stroke-linecap="round" stroke-linejoin="round" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1" />
                  </svg>
                  <span class="font-medium">Déconnexion</span>
                </button>
              </div>
            </div>
          </div>
          
          <!-- Bouton paramètres si non connecté -->
          <button v-else class="p-2 rounded-lg hover:bg-zinc-800/60 transition-colors duration-200 group" @click="ui.openUserModal">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5 text-zinc-400 group-hover:text-zinc-200 transition-colors" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z" />
              <path stroke-linecap="round" stroke-linejoin="round" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
            </svg>
          </button>
        </div>
      </header>

      <!-- Main - Scrollable -->
      <main class="flex-1 overflow-y-auto overflow-x-hidden relative z-10 bg-gradient-to-br from-zinc-950 via-zinc-900 to-zinc-950">
        <transition name="fade" mode="out-in">
          <component 
            :is="currentComponent" 
            :key="ui.currentScreen" 
          />
        </transition>
      </main>

      <!-- Modales globales -->
      <UserRegisterModal v-if="ui.isUserModalOpen" />
      <TutorialModal v-if="ui.isTutorialModalOpen" />

      <!-- Chat Panel -->
      <ChatPanel />
    </div>
  </div>
</template>

<script setup lang="ts">
import { computed, onMounted, ref, onUnmounted, watch } from 'vue';
import { useUiStore } from '@/stores/ui';
import { useUserStore } from '@/stores/user';
import { useStatsStore } from '@/stores/stats';
import { useNotificationsStore } from '@/stores/notifications';
import { useChatStore } from '@/stores/chat';

import HomeScreen from '@/components/HomeScreen.vue';
import LevelSelectScreen from '@/components/LevelSelectScreen.vue';
import GameScreen from '@/components/GameScreen.vue';
import StatsScreen from '@/components/StatsScreen.vue';
import GamesScreen from '@/components/GamesScreen.vue';
import TicTacToeScreen from '@/components/TicTacToeScreen.vue';
import ConnectFourScreen from '@/components/ConnectFourScreen.vue';
import RockPaperScissorsScreen from '@/components/RockPaperScissorsScreen.vue';
import AdventureScreen from '@/components/AdventureScreen.vue';
import ShopScreen from '@/components/ShopScreen.vue';
import CollectionScreen from '@/components/CollectionScreen.vue';
import MatchesScreen from '@/components/MatchesScreen.vue';
import ProfileScreen from '@/components/ProfileScreen.vue';
import CommunityScreen from '@/components/CommunityScreen.vue';
import NotificationsPanel from '@/components/NotificationsPanel.vue';
import TournamentsScreen from '@/components/TournamentsScreen.vue';
import UserRegisterModal from '@/components/UserRegisterModal.vue';
import TutorialModal from '@/components/TutorialModal.vue';
import ChatPanel from '@/components/ChatPanel.vue';
import IconNotification from '@/components/icons/IconNotification.vue';
import IconBridge from '@/components/icons/IconBridge.vue';

const ui = useUiStore();
const userStore = useUserStore();
const statsStore = useStatsStore();
const notificationsStore = useNotificationsStore();
const chatStore = useChatStore();

// État du menu utilisateur
const isUserMenuOpen = ref(false);

// Toggle du menu utilisateur
function toggleUserMenu() {
  isUserMenuOpen.value = !isUserMenuOpen.value;
}

// Fermer le menu si on clique en dehors
function handleClickOutside(event: MouseEvent) {
  const target = event.target as HTMLElement;
  if (!target.closest('.relative')) {
    isUserMenuOpen.value = false;
    notificationsStore.closeNotificationsPanel();
  }
}

// Gérer la déconnexion
function handleLogout() {
  userStore.clearUser();
  statsStore.resetStats();
  isUserMenuOpen.value = false;
  ui.goToHome();
}

// Jeux récemment joués (mock data - à remplacer par des vraies données)
const recentlyPlayedGames = [
  { id: 1, name: 'Hashi', icon: '🌉', rating: '5.0', type: 'levels' },
  { id: 2, name: 'Tic-Tac-Toe', icon: '⭕', rating: '4.8', type: 'ticTacToe' },
  { id: 3, name: 'Connect Four', icon: '🔴', rating: '4.6', type: 'connectFour' },
  { id: 4, name: 'Aventure', icon: '🗝️', rating: '4.9', type: 'adventure' }
];

function navigateToGame(type: string) {
  switch (type) {
    case 'levels':
      ui.goToLevels();
      break;
    case 'ticTacToe':
      ui.goToTicTacToe();
      break;
    case 'connectFour':
      ui.goToConnectFour();
      break;
    case 'adventure':
      ui.goToAdventure();
      break;
  }
}

function formatTime(date: Date): string {
  const now = new Date();
  const diff = now.getTime() - date.getTime();
  const minutes = Math.floor(diff / 60000);
  const hours = Math.floor(diff / 3600000);
  const days = Math.floor(diff / 86400000);
  
  if (minutes < 1) return 'À l\'instant';
  if (minutes < 60) return `Il y a ${minutes} min`;
  if (hours < 24) return `Il y a ${hours}h`;
  if (days < 7) return `Il y a ${days}j`;
  return date.toLocaleDateString('fr-FR');
}

function handleNotificationClick(notification: any) {
  if (notification.type === 'invitation' && notification.invitation) {
    notificationsStore.markAsRead(notification.id);
    notificationsStore.closeNotificationsPanel();
    // Naviguer vers le jeu correspondant
    // Le composant de jeu détectera l'invitation et ouvrira automatiquement le modal de mise
    const invitation = notification.invitation;
    switch (invitation.gameType) {
      case 'TicTacToe':
        ui.goToTicTacToe();
        break;
      case 'ConnectFour':
        ui.goToConnectFour();
        break;
      case 'RockPaperScissors':
        ui.goToRockPaperScissors();
        break;
    }
  } else {
    notificationsStore.markAsRead(notification.id);
  }
}

// Écouter les clics pour fermer le menu
onMounted(() => {
  document.addEventListener('click', handleClickOutside);
});

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside);
});

// Charger l'utilisateur si stocké en local et récupérer ses statistiques
onMounted(async () => {
  // S'assurer que l'écran par défaut est 'home'
  if (!ui.currentScreen) {
    ui.goToHome();
  }
  
  userStore.loadFromLocalStorage();
  
  // Si l'utilisateur n'est pas connecté, ouvrir automatiquement le modal de création de compte
  if (!userStore.isLoggedIn) {
    ui.openUserModal();
  }
  
  // Si un utilisateur est déjà connecté, charger ses statistiques automatiquement
  if (userStore.user?.email) {
    try {
      await statsStore.loadUserStatsByEmail(userStore.user.email);
    } catch (err) {
      // Ignorer les erreurs si l'utilisateur n'a pas encore de statistiques
      console.log('Aucune statistique disponible pour cet utilisateur');
    }
    
    // Charger les coins dès le début (pas besoin d'entrer dans le magasin)
    await userStore.loadCoins();
    
    // Charger l'avatar équipé
    await userStore.loadEquippedAvatar();
    
    // Charger les notifications
    await notificationsStore.loadNotifications();
    
    // Initialiser SignalR pour le chat en temps réel
    await chatStore.initializeSignalR(userStore.user.id);
    
    // Actualiser automatiquement les données toutes les 30 secondes
    const refreshInterval = setInterval(async () => {
      try {
        await userStore.loadCoins();
        await notificationsStore.loadNotifications();
        if (userStore.user?.id) {
          await statsStore.loadUserStatsByEmail(userStore.user.email || '');
        }
      } catch (err) {
        console.warn('Erreur lors de l\'actualisation automatique:', err);
      }
    }, 30000);
    
    // Nettoyer l'intervalle au démontage
    onUnmounted(() => {
      clearInterval(refreshInterval);
    });
  }
  
  // Écouter les erreurs 404 de l'API pour ouvrir automatiquement le modal de création de compte
  const handle404Error = () => {
    // Si l'utilisateur n'est pas connecté, ouvrir le modal de création de compte
    if (!userStore.isLoggedIn && !ui.isUserModalOpen) {
      ui.openUserModal();
    }
  };
  
  window.addEventListener('api-404-error', handle404Error);
  
  // Nettoyer l'écouteur d'événement au démontage
  onUnmounted(() => {
    window.removeEventListener('api-404-error', handle404Error);
  });
});

// Déconnecter SignalR quand l'utilisateur se déconnecte
watch(() => userStore.isLoggedIn, async (isLoggedIn) => {
  if (!isLoggedIn) {
    await chatStore.disconnectSignalR();
  } else if (userStore.user?.id) {
    await chatStore.initializeSignalR(userStore.user.id);
  }
});

// Sélection dynamique de l'écran actif
const currentComponent = computed(() => {
  switch (ui.currentScreen) {
    case 'home': return HomeScreen;
    case 'levels': return LevelSelectScreen;
    case 'game': return GameScreen;
    case 'stats': return StatsScreen;
    case 'games': return GamesScreen;
    case 'ticTacToe': return TicTacToeScreen;
    case 'connectFour': return ConnectFourScreen;
    case 'rockPaperScissors': return RockPaperScissorsScreen;
    case 'adventure': return AdventureScreen;
    case 'shop': return ShopScreen;
    case 'collection': return CollectionScreen;
    case 'matches': return MatchesScreen;
    case 'profile': return ProfileScreen;
    case 'community': return CommunityScreen;
    case 'tournaments': return TournamentsScreen;
    case 'leaderboard': return HomeScreen; // temporaire
    default: return HomeScreen;
  }
});
</script>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 180ms ease;
}
.fade-enter-from,
.fade-leave-to {
  opacity: 0;
}

/* Navigation item animations */
.nav-item {
  position: relative;
  overflow: hidden;
}

.nav-item::before {
  content: '';
  position: absolute;
  left: 0;
  top: 0;
  height: 100%;
  width: 3px;
  background: linear-gradient(to bottom, transparent, currentColor, transparent);
  transform: translateX(-100%);
  transition: transform 0.3s ease;
}

.nav-item:hover::before,
.nav-item[class*='bg-gradient']::before {
  transform: translateX(0);
}

.nav-icon-wrapper {
  transition: all 0.3s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.nav-item:hover .nav-icon-wrapper {
  transform: scale(1.1) rotate(5deg);
}

.nav-icon-active {
  transform: scale(1.15);
  animation: iconPulse 2s ease-in-out infinite;
}

@keyframes iconPulse {
  0%, 100% {
    transform: scale(1.15);
  }
  50% {
    transform: scale(1.2);
  }
}

/* Smooth transitions for active states */
.nav-item[class*='bg-gradient'] {
  animation: slideIn 0.3s ease-out;
}

@keyframes slideIn {
  from {
    opacity: 0.8;
    transform: translateX(-5px);
  }
  to {
    opacity: 1;
    transform: translateX(0);
  }
}

/* Menu utilisateur animation */
@keyframes fadeIn {
  from {
    opacity: 0;
    transform: translateY(-10px) scale(0.95);
  }
  to {
    opacity: 1;
    transform: translateY(0) scale(1);
  }
}

.animate-fadeIn {
  animation: fadeIn 0.2s ease-out;
}
</style>
