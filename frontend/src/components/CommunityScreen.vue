<template>
  <section class="w-full h-full px-4 sm:px-6 lg:px-12 py-8 overflow-y-auto">
    <div class="max-w-7xl mx-auto">
      <!-- En-tête -->
      <header class="mb-8">
        <div class="flex items-center gap-3 mb-3">
          <div class="h-1 w-12 bg-gradient-to-r from-cyan-400 via-purple-500 to-pink-500 rounded-full"></div>
          <p class="text-sm uppercase tracking-[0.3em] text-cyan-300 font-semibold">
            Communauté
          </p>
        </div>
        <div class="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-4">
          <div class="flex-1">
            <h2 class="text-3xl sm:text-4xl lg:text-5xl font-extrabold text-zinc-50 bg-gradient-to-r from-cyan-400 via-purple-400 to-pink-400 bg-clip-text text-transparent mb-3">
              Communauté Prison Break
            </h2>
            <p class="text-sm sm:text-base text-zinc-300 max-w-2xl">
              Partage tes exploits, pose tes questions et découvre les astuces de la communauté.
            </p>
          </div>
          <button
            v-if="userStore.isLoggedIn"
            @click="showCreatePostModal = true"
            class="px-6 py-3 rounded-xl bg-gradient-to-r from-cyan-500 via-purple-500 to-pink-500 text-white font-semibold hover:from-cyan-400 hover:via-purple-400 hover:to-pink-400 transition-all duration-200 shadow-lg shadow-cyan-500/40"
          >
            + Créer un post
          </button>
        </div>
      </header>

      <!-- Stats de la communauté -->
      <div class="grid grid-cols-1 sm:grid-cols-3 gap-4 mb-8">
        <div class="p-6 rounded-2xl bg-gradient-to-br from-zinc-900/90 to-zinc-800/90 border border-zinc-700/50 backdrop-blur-sm">
          <div class="flex items-center gap-3 mb-2">
            <div class="h-10 w-10 rounded-xl bg-cyan-500/20 flex items-center justify-center">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-cyan-400" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
              </svg>
            </div>
            <div>
              <p class="text-2xl font-bold text-zinc-50">{{ communityStore.posts.length }}</p>
              <p class="text-xs text-zinc-400">Posts</p>
            </div>
          </div>
        </div>
        <div class="p-6 rounded-2xl bg-gradient-to-br from-zinc-900/90 to-zinc-800/90 border border-zinc-700/50 backdrop-blur-sm">
          <div class="flex items-center gap-3 mb-2">
            <div class="h-10 w-10 rounded-xl bg-purple-500/20 flex items-center justify-center">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-purple-400" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M4.318 6.318a4.5 4.5 0 000 6.364L12 20.364l7.682-7.682a4.5 4.5 0 00-6.364-6.364L12 7.636l-1.318-1.318a4.5 4.5 0 00-6.364 0z" />
              </svg>
            </div>
            <div>
              <p class="text-2xl font-bold text-zinc-50">{{ totalLikes }}</p>
              <p class="text-xs text-zinc-400">Likes</p>
            </div>
          </div>
        </div>
        <div class="p-6 rounded-2xl bg-gradient-to-br from-zinc-900/90 to-zinc-800/90 border border-zinc-700/50 backdrop-blur-sm">
          <div class="flex items-center gap-3 mb-2">
            <div class="h-10 w-10 rounded-xl bg-pink-500/20 flex items-center justify-center">
              <svg xmlns="http://www.w3.org/2000/svg" class="h-6 w-6 text-pink-400" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
              </svg>
            </div>
            <div>
              <p class="text-2xl font-bold text-zinc-50">{{ totalComments }}</p>
              <p class="text-xs text-zinc-400">Commentaires</p>
            </div>
          </div>
        </div>
      </div>

      <!-- Filtres -->
      <div class="flex gap-2 mb-6 overflow-x-auto pb-2">
        <button
          @click="selectedPostType = null"
          :class="[
            'px-4 py-2 rounded-xl text-sm font-semibold whitespace-nowrap transition-all duration-200',
            selectedPostType === null
              ? 'bg-gradient-to-r from-cyan-500 to-purple-500 text-white'
              : 'bg-zinc-800 text-zinc-300 hover:bg-zinc-700'
          ]"
        >
          Tous
        </button>
        <button
          v-for="type in postTypes"
          :key="type.value"
          @click="selectedPostType = type.value"
          :class="[
            'px-4 py-2 rounded-xl text-sm font-semibold whitespace-nowrap transition-all duration-200',
            selectedPostType === type.value
              ? 'bg-gradient-to-r from-cyan-500 to-purple-500 text-white'
              : 'bg-zinc-800 text-zinc-300 hover:bg-zinc-700'
          ]"
        >
          {{ type.label }}
        </button>
      </div>

      <!-- Liste des posts -->
      <div v-if="communityStore.isLoading" class="text-center py-12 text-zinc-400">
        <p>Chargement...</p>
      </div>

      <div v-else-if="filteredPosts.length === 0" class="text-center py-12 text-zinc-400">
        <svg xmlns="http://www.w3.org/2000/svg" class="h-16 w-16 mx-auto text-zinc-400 mb-4" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
          <path stroke-linecap="round" stroke-linejoin="round" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z" />
        </svg>
        <p class="text-zinc-300 text-lg font-semibold">Aucun post pour le moment</p>
        <p class="text-sm text-zinc-400 mt-2">Soyez le premier à partager quelque chose !</p>
      </div>

      <div v-else class="space-y-6">
        <div
          v-for="post in filteredPosts"
          :key="post.id"
          class="group relative p-6 rounded-2xl bg-gradient-to-br from-zinc-900/90 to-zinc-800/90 border border-zinc-700/50 hover:border-cyan-500/30 transition-all duration-300 shadow-lg hover:shadow-xl hover:shadow-cyan-500/10 backdrop-blur-sm overflow-hidden"
        >
          <!-- Effet de brillance au survol -->
          <div class="absolute inset-0 bg-gradient-to-r from-transparent via-cyan-500/5 to-transparent opacity-0 group-hover:opacity-100 transition-opacity duration-500 -translate-x-full group-hover:translate-x-full"></div>
          
          <!-- Header du post -->
          <div class="flex items-center gap-3 mb-4 relative z-10">
            <div class="h-12 w-12 rounded-full bg-gradient-to-br from-cyan-500 via-purple-500 to-pink-500 flex items-center justify-center text-white font-bold text-lg flex-shrink-0 shadow-lg ring-2 ring-zinc-700/50 group-hover:ring-cyan-500/50 transition-all duration-300">
              {{ post.authorName?.charAt(0)?.toUpperCase() || '?' }}
            </div>
            <div class="flex-1 min-w-0">
              <p class="text-sm font-bold text-zinc-50 truncate group-hover:text-cyan-300 transition-colors">{{ post.authorName || 'Utilisateur' }}</p>
              <p class="text-xs text-zinc-400">{{ formatTime(post.createdAt) }}</p>
            </div>
            <div class="flex items-center gap-2 flex-shrink-0">
              <span
                :class="[
                  'px-3 py-1.5 rounded-lg text-xs font-semibold whitespace-nowrap shadow-sm',
                  getPostTypeClass(post.postType)
                ]"
              >
                {{ getPostTypeLabel(post.postType) }}
              </span>
              <!-- Bouton de suppression (visible uniquement par l'auteur) -->
              <button
                v-if="userStore.isLoggedIn && post.authorId === userStore.userId"
                @click.stop="handleDeletePost(post.id)"
                :disabled="isDeletingPost === post.id"
                class="p-2 rounded-lg bg-red-500/20 text-red-400 hover:bg-red-500/30 transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed flex-shrink-0 hover:scale-110 active:scale-95"
                title="Supprimer le post"
              >
                <svg v-if="isDeletingPost !== post.id" xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                  <path stroke-linecap="round" stroke-linejoin="round" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
                </svg>
                <div v-else class="inline-block animate-spin rounded-full h-5 w-5 border-b-2 border-red-400"></div>
              </button>
            </div>
          </div>

          <!-- Contenu du post -->
          <div class="relative z-10">
            <h4 class="text-xl font-bold text-zinc-50 mb-3 group-hover:text-cyan-100 transition-colors">{{ post.title }}</h4>
            <p class="text-sm text-zinc-300 mb-4 whitespace-pre-wrap leading-relaxed">{{ post.content }}</p>

            <!-- Image si présente -->
            <div v-if="post.imageUrl" class="mb-4 rounded-xl overflow-hidden border border-zinc-700/50 group-hover:border-cyan-500/30 transition-all duration-300">
              <img
                :src="getImageUrl(post.imageUrl)"
                :alt="post.title"
                class="w-full rounded-xl object-cover max-h-96 group-hover:scale-[1.02] transition-transform duration-500"
                @error="handleImageError"
                @load="console.log('Image chargée:', getImageUrl(post.imageUrl))"
              />
            </div>
          </div>

          <!-- Actions -->
          <div class="flex items-center gap-4 pt-4 border-t border-zinc-800/50 relative z-10">
            <button
              @click="toggleLike(post.id)"
              :class="[
                'flex items-center gap-2 px-4 py-2.5 rounded-lg transition-all duration-200 hover:scale-105 active:scale-95',
                post.isLikedByCurrentUser
                  ? 'bg-pink-500/20 text-pink-400 border border-pink-500/30 shadow-lg shadow-pink-500/20'
                  : 'bg-zinc-800/50 text-zinc-400 hover:bg-zinc-700/50 border border-zinc-700/30'
              ]"
            >
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="currentColor" viewBox="0 0 20 20">
                <path fill-rule="evenodd" d="M3.172 5.172a4 4 0 015.656 0L10 6.343l1.172-1.171a4 4 0 115.656 5.656L10 17.657l-6.828-6.829a4 4 0 010-5.656z" clip-rule="evenodd" />
              </svg>
              <span class="text-sm font-semibold">{{ post.likesCount }}</span>
            </button>
            <button
              @click="toggleComments(post.id)"
              class="flex items-center gap-2 px-4 py-2.5 rounded-lg bg-zinc-800/50 text-zinc-400 hover:bg-zinc-700/50 transition-all duration-200 hover:scale-105 active:scale-95 border border-zinc-700/30 hover:border-cyan-500/30"
            >
              <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
                <path stroke-linecap="round" stroke-linejoin="round" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
              </svg>
              <span class="text-sm font-semibold">{{ post.commentsCount }}</span>
            </button>
          </div>

          <!-- Section commentaires -->
          <div v-if="expandedComments === post.id" class="mt-4 pt-4 border-t border-zinc-800/50 relative z-10">
            <div v-if="postComments[post.id]?.length === 0" class="text-center py-4 text-zinc-500 text-sm">
              Aucun commentaire pour le moment
            </div>
            <div v-else class="space-y-3 mb-4">
              <div
                v-for="comment in postComments[post.id]"
                :key="comment.id"
                class="p-3 rounded-xl bg-zinc-800/50 border border-zinc-700/30 hover:border-cyan-500/20 transition-all duration-200 hover:bg-zinc-800/70"
              >
                <div class="flex items-center gap-2 mb-2">
                  <div class="h-8 w-8 rounded-full bg-gradient-to-br from-cyan-500 to-purple-500 flex items-center justify-center text-white font-bold text-xs shadow-md">
                    {{ comment.authorName.charAt(0).toUpperCase() }}
                  </div>
                  <div>
                    <p class="text-xs font-semibold text-zinc-50">{{ comment.authorName }}</p>
                    <p class="text-[10px] text-zinc-500">{{ formatTime(comment.createdAt) }}</p>
                  </div>
                </div>
                <p class="text-sm text-zinc-300 ml-10 leading-relaxed">{{ comment.content }}</p>
              </div>
            </div>
            <!-- Formulaire de commentaire -->
            <div v-if="userStore.isLoggedIn" class="flex gap-2">
              <input
                v-model="newComments[post.id]"
                @keyup.enter="addComment(post.id)"
                type="text"
                placeholder="Écris un commentaire..."
                class="flex-1 px-4 py-2 rounded-xl bg-zinc-800 border border-zinc-700 text-zinc-50 placeholder-zinc-500 focus:outline-none focus:ring-2 focus:ring-cyan-500/50 focus:border-cyan-500/50 transition-all"
              />
              <button
                @click="addComment(post.id)"
                class="px-4 py-2 rounded-xl bg-gradient-to-r from-cyan-500 to-purple-500 text-white font-semibold hover:from-cyan-400 hover:to-purple-400 transition-all duration-200 hover:scale-105 active:scale-95 shadow-lg shadow-cyan-500/20"
              >
                Envoyer
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Modal de confirmation de suppression -->
    <div
      v-if="showDeleteConfirm"
      class="fixed inset-0 bg-black/70 backdrop-blur-sm z-[60] flex items-center justify-center p-4"
      @click.self="showDeleteConfirm = false; postToDelete = null"
    >
      <div class="bg-gradient-to-br from-zinc-900 via-zinc-800 to-zinc-900 rounded-2xl p-6 max-w-md w-full border border-red-500/30 shadow-2xl shadow-red-500/20 animate-fade-in">
        <!-- Icône d'avertissement -->
        <div class="flex justify-center mb-4">
          <div class="h-16 w-16 rounded-full bg-red-500/20 flex items-center justify-center border-2 border-red-500/50">
            <svg xmlns="http://www.w3.org/2000/svg" class="h-8 w-8 text-red-400" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
            </svg>
          </div>
        </div>
        
        <!-- Titre -->
        <h3 class="text-xl font-bold text-zinc-50 text-center mb-2">
          Confirmer la suppression
        </h3>
        
        <!-- Message -->
        <p class="text-sm text-zinc-300 text-center mb-6 leading-relaxed">
          Êtes-vous sûr de vouloir supprimer ce post ?<br>
          <span class="text-red-400 font-semibold">Cette action est irréversible.</span>
        </p>
        
        <!-- Boutons d'action -->
        <div class="flex gap-3">
          <button
            @click="showDeleteConfirm = false; postToDelete = null"
            class="flex-1 px-4 py-3 rounded-xl bg-zinc-800 text-zinc-300 font-semibold hover:bg-zinc-700 transition-all duration-200 border border-zinc-700/50"
          >
            Annuler
          </button>
          <button
            @click="confirmDelete"
            :disabled="isDeletingPost === postToDelete"
            class="flex-1 px-4 py-3 rounded-xl bg-gradient-to-r from-red-500 to-red-600 text-white font-semibold hover:from-red-400 hover:to-red-500 transition-all duration-200 disabled:opacity-50 disabled:cursor-not-allowed shadow-lg shadow-red-500/30 flex items-center justify-center gap-2"
          >
            <svg v-if="isDeletingPost !== postToDelete" xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16" />
            </svg>
            <div v-else class="inline-block animate-spin rounded-full h-5 w-5 border-b-2 border-white"></div>
            <span>{{ isDeletingPost === postToDelete ? 'Suppression...' : 'Supprimer' }}</span>
          </button>
        </div>
      </div>
    </div>

    <!-- Modal pour créer un post -->
    <div
      v-if="showCreatePostModal"
      class="fixed inset-0 bg-black/60 backdrop-blur-sm z-50 flex items-center justify-center p-4"
      @click.self="showCreatePostModal = false"
    >
      <div class="bg-zinc-900 rounded-2xl p-6 max-w-2xl w-full border border-zinc-700/50 max-h-[90vh] overflow-y-auto">
        <h3 class="text-xl font-bold text-zinc-50 mb-4">Créer un post</h3>
        <div class="space-y-4">
          <div>
            <label class="block text-sm font-medium text-zinc-300 mb-2">Type de post</label>
            <select
              v-model="newPost.postType"
              class="w-full px-4 py-2.5 rounded-xl bg-zinc-800 border border-zinc-700 text-zinc-50 focus:outline-none focus:ring-2 focus:ring-cyan-500/50"
            >
              <option v-for="type in postTypes" :key="type.value" :value="type.value">
                {{ type.label }}
              </option>
            </select>
          </div>
          <div>
            <label class="block text-sm font-medium text-zinc-300 mb-2">Titre</label>
            <input
              v-model="newPost.title"
              type="text"
              placeholder="Titre du post"
              class="w-full px-4 py-2.5 rounded-xl bg-zinc-800 border border-zinc-700 text-zinc-50 placeholder-zinc-500 focus:outline-none focus:ring-2 focus:ring-cyan-500/50"
            />
          </div>
          <div>
            <label class="block text-sm font-medium text-zinc-300 mb-2">Contenu</label>
            <textarea
              v-model="newPost.content"
              rows="6"
              placeholder="Contenu du post..."
              class="w-full px-4 py-2.5 rounded-xl bg-zinc-800 border border-zinc-700 text-zinc-50 placeholder-zinc-500 focus:outline-none focus:ring-2 focus:ring-cyan-500/50"
            ></textarea>
          </div>
          <div>
            <label class="block text-sm font-medium text-zinc-300 mb-2">Image (optionnel)</label>
            <div class="space-y-3">
              <!-- Option: Upload fichier -->
              <div>
                <label class="block text-xs text-zinc-400 mb-2">Uploader une image</label>
                <input
                  ref="fileInput"
                  type="file"
                  accept="image/*"
                  @change="handleFileSelect"
                  class="hidden"
                />
                <button
                  type="button"
                  @click="fileInput?.click()"
                  class="w-full px-4 py-2.5 rounded-xl bg-zinc-800 border border-zinc-700 text-zinc-300 hover:bg-zinc-700 transition-colors text-sm font-medium"
                >
                  {{ selectedFile ? `📷 ${selectedFile.name}` : '📁 Choisir un fichier' }}
                </button>
                <div v-if="uploading" class="mt-2 text-xs text-cyan-400">
                  ⏳ Upload en cours...
                </div>
                <div v-if="uploadError" class="mt-2 text-xs text-red-400">
                  ❌ {{ uploadError }}
                </div>
              </div>
              <!-- Option: Lien URL -->
              <div>
                <label class="block text-xs text-zinc-400 mb-2">Ou utiliser un lien</label>
                <input
                  v-model="newPost.imageUrl"
                  type="url"
                  placeholder="https://example.com/image.jpg"
                  class="w-full px-4 py-2.5 rounded-xl bg-zinc-800 border border-zinc-700 text-zinc-50 placeholder-zinc-500 focus:outline-none focus:ring-2 focus:ring-cyan-500/50"
                />
              </div>
              <!-- Aperçu de l'image -->
              <div v-if="imagePreview" class="mt-3">
                <img
                  :src="imagePreview"
                  alt="Aperçu"
                  class="w-full max-h-48 object-cover rounded-xl border border-zinc-700"
                />
                <button
                  type="button"
                  @click="clearImage"
                  class="mt-2 text-xs text-red-400 hover:text-red-300"
                >
                  ✕ Supprimer l'image
                </button>
              </div>
            </div>
          </div>
          <div class="flex gap-3">
            <button
              @click="createPost"
              class="flex-1 px-4 py-2.5 rounded-xl bg-gradient-to-r from-cyan-500 to-purple-500 text-white font-semibold hover:from-cyan-400 hover:to-purple-400 transition-all duration-200"
            >
              Publier
            </button>
            <button
              @click="showCreatePostModal = false"
              class="px-4 py-2.5 rounded-xl bg-zinc-800 text-zinc-300 font-semibold hover:bg-zinc-700 transition-colors"
            >
              Annuler
            </button>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup lang="ts">
import { ref, computed, onMounted, watch } from 'vue';
import { useCommunityStore } from '@/stores/community';
import { useUserStore } from '@/stores/user';

const communityStore = useCommunityStore();
const userStore = useUserStore();

const showCreatePostModal = ref(false);
const selectedPostType = ref<string | null>(null);
const expandedComments = ref<number | null>(null);
const isDeletingPost = ref<number | null>(null);
const postComments = ref<Record<number, any[]>>({});
const newComments = ref<Record<number, string>>({});
const showDeleteConfirm = ref(false);
const postToDelete = ref<number | null>(null);

const fileInput = ref<HTMLInputElement | null>(null);
const selectedFile = ref<File | null>(null);
const uploading = ref(false);
const uploadError = ref<string | null>(null);
const imagePreview = ref<string | null>(null);

const newPost = ref({
  title: '',
  content: '',
  imageUrl: null as string | null,
  postType: 'Discussion',
});

const postTypes = [
  { value: 'Discussion', label: '💬 Discussion' },
  { value: 'Achievement', label: '🏆 Exploit' },
  { value: 'Question', label: '❓ Question' },
  { value: 'Tip', label: '💡 Astuce' },
  { value: 'Share', label: '📤 Partage' },
];

const filteredPosts = computed(() => {
  if (!selectedPostType.value) {
    return communityStore.posts;
  }
  return communityStore.posts.filter(p => p.postType === selectedPostType.value);
});

const totalLikes = computed(() => {
  return communityStore.posts.reduce((sum, p) => sum + p.likesCount, 0);
});

const totalComments = computed(() => {
  return communityStore.posts.reduce((sum, p) => sum + p.commentsCount, 0);
});

watch(selectedPostType, (newType) => {
  communityStore.fetchPosts(50, newType || undefined);
});

onMounted(async () => {
  console.log('CommunityScreen monté, chargement des posts...');
  try {
    await communityStore.fetchPosts(50);
    console.log('Posts chargés:', communityStore.posts.length);
  } catch (error) {
    console.error('Erreur lors du chargement des posts:', error);
  }
});

function formatTime(dateString: string) {
  const date = new Date(dateString);
  const now = new Date();
  const diff = now.getTime() - date.getTime();
  const minutes = Math.floor(diff / 60000);
  const hours = Math.floor(minutes / 60);
  const days = Math.floor(hours / 24);
  
  // Afficher la date et l'heure réelles
  if (days > 7) {
    // Si plus de 7 jours, afficher la date complète
    return date.toLocaleDateString('fr-FR', { 
      day: 'numeric', 
      month: 'short', 
      year: date.getFullYear() !== now.getFullYear() ? 'numeric' : undefined,
      hour: '2-digit',
      minute: '2-digit'
    });
  } else if (days > 0) {
    return `Il y a ${days} jour${days > 1 ? 's' : ''}`;
  } else if (hours > 0) {
    return `Il y a ${hours} heure${hours > 1 ? 's' : ''}`;
  } else if (minutes > 0) {
    return `Il y a ${minutes} minute${minutes > 1 ? 's' : ''}`;
  } else {
    // Moins d'une minute : afficher l'heure exacte
    return date.toLocaleTimeString('fr-FR', { hour: '2-digit', minute: '2-digit' });
  }
}

function getPostTypeLabel(type: string) {
  const typeMap: Record<string, string> = {
    Discussion: '💬 Discussion',
    Achievement: '🏆 Exploit',
    Question: '❓ Question',
    Tip: '💡 Astuce',
    Share: '📤 Partage',
  };
  return typeMap[type] || type;
}

function getPostTypeClass(type: string) {
  const classMap: Record<string, string> = {
    Discussion: 'bg-cyan-500/20 text-cyan-400',
    Achievement: 'bg-yellow-500/20 text-yellow-400',
    Question: 'bg-purple-500/20 text-purple-400',
    Tip: 'bg-emerald-500/20 text-emerald-400',
    Share: 'bg-pink-500/20 text-pink-400',
  };
  return classMap[type] || 'bg-zinc-800 text-zinc-400';
}

function handleDeletePost(postId: number) {
  if (!userStore.userId) return;
  postToDelete.value = postId;
  showDeleteConfirm.value = true;
}

async function confirmDelete() {
  if (!postToDelete.value || !userStore.userId) return;
  
  isDeletingPost.value = postToDelete.value;
  try {
    await communityStore.deletePost(postToDelete.value, userStore.userId);
    // Le post sera automatiquement retiré de la liste par le store
    showDeleteConfirm.value = false;
    postToDelete.value = null;
  } catch (error) {
    console.error('Erreur lors de la suppression:', error);
    alert(error instanceof Error ? error.message : 'Erreur lors de la suppression du post');
  } finally {
    isDeletingPost.value = null;
  }
}

function getImageUrl(imageUrl: string | null): string {
  if (!imageUrl) return '';
  
  // Si c'est déjà une URL complète (http/https), la retourner telle quelle
  if (imageUrl.startsWith('http://') || imageUrl.startsWith('https://')) {
    return imageUrl;
  }
  
  // Si c'est un chemin relatif (commence par /), l'utiliser tel quel
  // Le backend sert les fichiers statiques depuis wwwroot
  if (imageUrl.startsWith('/')) {
    return imageUrl;
  }
  
  // Sinon, ajouter le préfixe
  return imageUrl.startsWith('/') ? imageUrl : `/${imageUrl}`;
}

function handleImageError(event: Event) {
  const img = event.target as HTMLImageElement;
  console.error('Erreur de chargement de l\'image:', img.src);
  // Optionnel: masquer l'image en cas d'erreur
  img.style.display = 'none';
}

async function toggleLike(postId: number) {
  if (!userStore.user) return;
  
  const post = communityStore.posts.find(p => p.id === postId);
  if (!post) return;

  if (post.isLikedByCurrentUser) {
    await communityStore.unlikePost(postId, userStore.user.id);
  } else {
    await communityStore.likePost(postId, userStore.user.id);
  }
}

async function toggleComments(postId: number) {
  if (expandedComments.value === postId) {
    expandedComments.value = null;
  } else {
    expandedComments.value = postId;
    if (!postComments.value[postId]) {
      await communityStore.fetchComments(postId);
      postComments.value[postId] = communityStore.comments;
    }
  }
}

async function addComment(postId: number) {
  if (!userStore.user || !newComments.value[postId]?.trim()) return;

  try {
    await communityStore.addComment(postId, userStore.user.id, newComments.value[postId].trim());
    newComments.value[postId] = '';
    await communityStore.fetchComments(postId);
    postComments.value[postId] = communityStore.comments;
  } catch (error) {
    console.error('Erreur lors de l\'ajout du commentaire:', error);
  }
}

async function handleFileSelect(event: Event) {
  const input = event.target as HTMLInputElement;
  if (input.files && input.files[0]) {
    const file = input.files[0];
    
    // Vérifier la taille du fichier (max 5MB)
    const maxSize = 5 * 1024 * 1024; // 5MB
    if (file.size > maxSize) {
      uploadError.value = `Le fichier est trop volumineux (${(file.size / 1024 / 1024).toFixed(2)}MB). Taille maximale: 5MB`;
      selectedFile.value = null;
      imagePreview.value = null;
      return;
    }
    
    // Vérifier le type de fichier
    const allowedTypes = ['image/jpeg', 'image/jpg', 'image/png', 'image/gif', 'image/webp'];
    if (!allowedTypes.includes(file.type)) {
      uploadError.value = 'Type de fichier non autorisé. Utilisez JPG, PNG, GIF ou WEBP.';
      selectedFile.value = null;
      imagePreview.value = null;
      return;
    }
    
    selectedFile.value = file;
    uploadError.value = null;
    
    // Créer un aperçu
    const reader = new FileReader();
    reader.onload = (e) => {
      imagePreview.value = e.target?.result as string;
    };
    reader.readAsDataURL(selectedFile.value);
    
    // Uploader le fichier
    await uploadImage();
  }
}

async function uploadImage() {
  if (!selectedFile.value) return;
  
  uploading.value = true;
  uploadError.value = null;
  
  try {
    const formData = new FormData();
    formData.append('file', selectedFile.value);
    
    // Utiliser l'URL relative pour le proxy en développement
    // Le proxy Vite redirige /api vers http://localhost:5000
    const uploadUrl = '/api/community/upload';
    console.log('Upload URL:', uploadUrl);
    console.log('File:', selectedFile.value.name, 'Size:', selectedFile.value.size);
    
    const response = await fetch(uploadUrl, {
      method: 'POST',
      body: formData,
      credentials: 'include',
      // Ne pas définir Content-Type pour FormData, le navigateur le fait automatiquement avec la boundary
    });
    
    console.log('Response status:', response.status, response.statusText);
    
    if (!response.ok) {
      const errorText = await response.text();
      console.error('Error response:', errorText);
      let errorData;
      try {
        errorData = JSON.parse(errorText);
      } catch {
        errorData = { message: errorText || `Erreur HTTP ${response.status}` };
      }
      throw new Error(errorData.message || `Erreur HTTP ${response.status}`);
    }
    
    const result = await response.json();
    newPost.value.imageUrl = result.url;
    console.log('Upload réussi, URL:', result.url);
  } catch (error) {
    console.error('Erreur upload:', error);
    uploadError.value = error instanceof Error ? error.message : 'Erreur lors de l\'upload. Vérifiez que le serveur backend est démarré.';
    selectedFile.value = null;
    imagePreview.value = null;
  } finally {
    uploading.value = false;
  }
}

function clearImage() {
  selectedFile.value = null;
  imagePreview.value = null;
  newPost.value.imageUrl = null;
  if (fileInput.value) {
    fileInput.value.value = '';
  }
}

async function createPost() {
  if (!userStore.user || !newPost.value.title || !newPost.value.content) return;

  try {
    await communityStore.createPost(
      userStore.user.id,
      newPost.value.title,
      newPost.value.content,
      newPost.value.imageUrl,
      newPost.value.postType
    );
    showCreatePostModal.value = false;
    newPost.value = {
      title: '',
      content: '',
      imageUrl: null,
      postType: 'Discussion',
    };
    clearImage();
  } catch (error) {
    console.error('Erreur lors de la création du post:', error);
    const errorMessage = error instanceof Error ? error.message : 'Erreur lors de la création du post';
    alert(errorMessage);
  }
}
</script>

<style scoped>
.scrollbar-thin {
  scrollbar-width: thin;
}

.scrollbar-thumb-zinc-700::-webkit-scrollbar-thumb {
  background-color: rgb(63 63 70);
  border-radius: 9999px;
}

.scrollbar-track-zinc-900::-webkit-scrollbar-track {
  background-color: rgb(24 24 27);
}

.scrollbar-thin::-webkit-scrollbar {
  height: 8px;
}

.scrollbar-thin::-webkit-scrollbar-thumb {
  background-color: rgb(63 63 70);
  border-radius: 9999px;
}

.scrollbar-thin::-webkit-scrollbar-track {
  background-color: rgb(24 24 27);
}

@keyframes fade-in {
  from {
    opacity: 0;
    transform: scale(0.95) translateY(-10px);
  }
  to {
    opacity: 1;
    transform: scale(1) translateY(0);
  }
}

.animate-fade-in {
  animation: fade-in 0.2s ease-out;
}
</style>

