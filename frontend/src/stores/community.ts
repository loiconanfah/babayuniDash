import { defineStore } from 'pinia';
import { ref } from 'vue';

export interface CommunityPost {
  id: number;
  authorId: number;
  authorName: string;
  title: string;
  content: string;
  imageUrl: string | null;
  postType: 'Discussion' | 'Achievement' | 'Question' | 'Tip' | 'Share';
  createdAt: string;
  updatedAt: string | null;
  likesCount: number;
  commentsCount: number;
  isLikedByCurrentUser: boolean;
}

export interface CommunityPostComment {
  id: number;
  postId: number;
  authorId: number;
  authorName: string;
  content: string;
  createdAt: string;
  updatedAt: string | null;
}

export const useCommunityStore = defineStore('community', () => {
  const posts = ref<CommunityPost[]>([]);
  const currentPost = ref<CommunityPost | null>(null);
  const comments = ref<CommunityPostComment[]>([]);
  const isLoading = ref(false);

  async function fetchPosts(limit?: number, postType?: string) {
    isLoading.value = true;
    try {
      let url = '/api/community/posts?';
      if (limit) url += `limit=${limit}&`;
      if (postType) url += `postType=${postType}&`;
      const response = await fetch(url);
      if (response.ok) {
        posts.value = await response.json();
      }
    } catch (error) {
      console.error('Erreur lors de la récupération des posts:', error);
    } finally {
      isLoading.value = false;
    }
  }

  async function fetchPost(postId: number) {
    isLoading.value = true;
    try {
      const response = await fetch(`/api/community/posts/${postId}`);
      if (response.ok) {
        currentPost.value = await response.json();
      }
    } catch (error) {
      console.error('Erreur lors de la récupération du post:', error);
    } finally {
      isLoading.value = false;
    }
  }

  async function createPost(authorId: number, title: string, content: string, imageUrl: string | null, postType: string) {
    try {
      const response = await fetch('/api/community/posts', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ 
          authorId, 
          title, 
          content, 
          imageUrl, 
          postTypeString: postType // Le backend attend PostTypeString
        }),
      });
      if (response.ok) {
        const post = await response.json();
        posts.value.unshift(post);
        return post;
      }
      const errorData = await response.json().catch(() => ({ message: 'Erreur lors de la création du post' }));
      throw new Error(errorData.message || 'Erreur lors de la création du post');
    } catch (error) {
      console.error('Erreur lors de la création du post:', error);
      throw error;
    }
  }

  async function likePost(postId: number, userId: number) {
    try {
      const response = await fetch(`/api/community/posts/${postId}/like`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ userId }),
      });
      if (response.ok) {
        const post = posts.value.find(p => p.id === postId);
        if (post) {
          post.likesCount++;
          post.isLikedByCurrentUser = true;
        }
        return true;
      }
      return false;
    } catch (error) {
      console.error('Erreur lors du like:', error);
      return false;
    }
  }

  async function unlikePost(postId: number, userId: number) {
    try {
      const response = await fetch(`/api/community/posts/${postId}/like`, {
        method: 'DELETE',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ userId }),
      });
      if (response.ok) {
        const post = posts.value.find(p => p.id === postId);
        if (post) {
          post.likesCount--;
          post.isLikedByCurrentUser = false;
        }
        return true;
      }
      return false;
    } catch (error) {
      console.error('Erreur lors du retrait du like:', error);
      return false;
    }
  }

  async function fetchComments(postId: number) {
    try {
      const response = await fetch(`/api/community/posts/${postId}/comments`);
      if (response.ok) {
        comments.value = await response.json();
      }
    } catch (error) {
      console.error('Erreur lors de la récupération des commentaires:', error);
    }
  }

  async function addComment(postId: number, authorId: number, content: string) {
    try {
      const response = await fetch(`/api/community/posts/${postId}/comments`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ authorId, content }),
      });
      if (response.ok) {
        const comment = await response.json();
        comments.value.push(comment);
        const post = posts.value.find(p => p.id === postId);
        if (post) {
          post.commentsCount++;
        }
        return comment;
      }
      throw new Error('Erreur lors de l\'ajout du commentaire');
    } catch (error) {
      console.error('Erreur lors de l\'ajout du commentaire:', error);
      throw error;
    }
  }

  async function deletePost(postId: number, userId: number) {
    try {
      const response = await fetch(`/api/community/posts/${postId}`, {
        method: 'DELETE',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ userId }),
      });
      if (response.ok) {
        posts.value = posts.value.filter(p => p.id !== postId);
        return true;
      }
      const errorData = await response.json().catch(() => ({ message: 'Erreur lors de la suppression du post' }));
      throw new Error(errorData.message || 'Erreur lors de la suppression du post');
    } catch (error) {
      console.error('Erreur lors de la suppression du post:', error);
      throw error;
    }
  }

  return {
    posts,
    currentPost,
    comments,
    isLoading,
    fetchPosts,
    fetchPost,
    createPost,
    deletePost,
    likePost,
    unlikePost,
    fetchComments,
    addComment,
  };
});

