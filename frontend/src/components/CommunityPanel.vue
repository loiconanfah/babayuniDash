<template>
  <div class="community-panel w-full">
    <div class="mb-6">
      <h3 class="text-2xl font-bold text-zinc-50 mb-2">Communauté</h3>
      <p class="text-sm text-zinc-400">Partage tes exploits et découvre ceux des autres</p>
    </div>

    <!-- Bouton pour créer un post -->
    <button
      @click="showCreatePostModal = true"
      class="w-full mb-6 px-4 py-3 rounded-xl bg-gradient-to-r from-cyan-500 via-purple-500 to-pink-500 text-white font-semibold hover:from-cyan-400 hover:via-purple-400 hover:to-pink-400 transition-all duration-200"
    >
      + Créer un post
    </button>

    <!-- Filtres -->
    <div class="flex gap-2 mb-6 overflow-x-auto pb-2">
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

    <div v-else-if="communityStore.posts.length === 0" class="text-center py-12 text-zinc-400">
      <p>Aucun post pour le moment</p>
    </div>

    <div v-else class="space-y-4">
      <div
        v-for="post in communityStore.posts"
        :key="post.id"
        class="p-6 rounded-2xl bg-zinc-900/60 border border-zinc-700/50 hover:border-cyan-500/30 transition-all duration-200"
      >
        <!-- Header du post -->
        <div class="flex items-center gap-3 mb-4">
          <div class="h-10 w-10 rounded-full bg-gradient-to-br from-cyan-500 to-purple-500 flex items-center justify-center text-white font-bold text-sm">
            {{ post.authorName.charAt(0).toUpperCase() }}
          </div>
          <div class="flex-1">
            <p class="text-sm font-bold text-zinc-50">{{ post.authorName }}</p>
            <p class="text-xs text-zinc-400">{{ formatTime(post.createdAt) }}</p>
          </div>
          <span
            :class="[
              'px-2 py-1 rounded-lg text-xs font-semibold',
              getPostTypeClass(post.postType)
            ]"
          >
            {{ getPostTypeLabel(post.postType) }}
          </span>
        </div>

        <!-- Contenu du post -->
        <h4 class="text-lg font-bold text-zinc-50 mb-2">{{ post.title }}</h4>
        <p class="text-sm text-zinc-300 mb-4 whitespace-pre-wrap">{{ post.content }}</p>

        <!-- Image si présente -->
        <img
          v-if="post.imageUrl"
          :src="post.imageUrl"
          :alt="post.title"
          class="w-full rounded-xl mb-4 object-cover max-h-96"
        />

        <!-- Actions -->
        <div class="flex items-center gap-4 pt-4 border-t border-zinc-800">
          <button
            @click="toggleLike(post.id)"
            :class="[
              'flex items-center gap-2 px-3 py-1.5 rounded-lg transition-colors',
              post.isLikedByCurrentUser
                ? 'bg-pink-500/20 text-pink-400'
                : 'bg-zinc-800 text-zinc-400 hover:bg-zinc-700'
            ]"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="currentColor" viewBox="0 0 20 20">
              <path fill-rule="evenodd" d="M3.172 5.172a4 4 0 015.656 0L10 6.343l1.172-1.171a4 4 0 115.656 5.656L10 17.657l-6.828-6.829a4 4 0 010-5.656z" clip-rule="evenodd" />
            </svg>
            <span class="text-sm font-semibold">{{ post.likesCount }}</span>
          </button>
          <button
            @click="showComments(post.id)"
            class="flex items-center gap-2 px-3 py-1.5 rounded-lg bg-zinc-800 text-zinc-400 hover:bg-zinc-700 transition-colors"
          >
            <svg xmlns="http://www.w3.org/2000/svg" class="h-5 w-5" fill="none" viewBox="0 0 24 24" stroke="currentColor" stroke-width="2">
              <path stroke-linecap="round" stroke-linejoin="round" d="M8 12h.01M12 12h.01M16 12h.01M21 12c0 4.418-4.03 8-9 8a9.863 9.863 0 01-4.255-.949L3 20l1.395-3.72C3.512 15.042 3 13.574 3 12c0-4.418 4.03-8 9-8s9 3.582 9 8z" />
            </svg>
            <span class="text-sm font-semibold">{{ post.commentsCount }}</span>
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
            <label class="block text-sm font-medium text-zinc-300 mb-2">URL de l'image (optionnel)</label>
            <input
              v-model="newPost.imageUrl"
              type="url"
              placeholder="https://example.com/image.jpg"
              class="w-full px-4 py-2.5 rounded-xl bg-zinc-800 border border-zinc-700 text-zinc-50 placeholder-zinc-500 focus:outline-none focus:ring-2 focus:ring-cyan-500/50"
            />
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
  </div>
</template>

<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import { useCommunityStore } from '@/stores/community';
import { useUserStore } from '@/stores/user';

const communityStore = useCommunityStore();
const userStore = useUserStore();

const showCreatePostModal = ref(false);
const selectedPostType = ref<string | undefined>(undefined);

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

watch(selectedPostType, (newType) => {
  communityStore.fetchPosts(20, newType);
});

onMounted(async () => {
  await communityStore.fetchPosts(20);
});

function formatTime(dateString: string) {
  const date = new Date(dateString);
  const now = new Date();
  const diff = now.getTime() - date.getTime();
  const minutes = Math.floor(diff / 60000);
  
  if (minutes < 1) return 'À l\'instant';
  if (minutes < 60) return `Il y a ${minutes} min`;
  if (minutes < 1440) return `Il y a ${Math.floor(minutes / 60)} h`;
  return date.toLocaleDateString('fr-FR');
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

function showComments(postId: number) {
  // TODO: Implémenter l'affichage des commentaires
  console.log('Afficher les commentaires pour le post', postId);
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
  } catch (error) {
    console.error('Erreur lors de la création du post:', error);
  }
}
</script>

