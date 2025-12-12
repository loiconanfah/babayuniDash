using Microsoft.EntityFrameworkCore;
using prisonbreak.Server.Data;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Services
{
    /// <summary>
    /// Service pour gérer la communauté.
    /// </summary>
    public class CommunityService : ICommunityService
    {
        private readonly HashiDbContext _context;

        public CommunityService(HashiDbContext context)
        {
            _context = context;
        }

        public async Task<CommunityPostDto> CreatePostAsync(int authorId, string title, string content, string? imageUrl, CommunityPostTypeDto postType)
        {
            if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Le titre et le contenu sont requis.");

            var author = await _context.Users.FindAsync(authorId);
            if (author == null)
                throw new ArgumentException("Utilisateur introuvable.");

            var post = new CommunityPost
            {
                AuthorId = authorId,
                Title = title,
                Content = content,
                ImageUrl = imageUrl,
                PostType = (CommunityPostType)postType,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.CommunityPosts.Add(post);
            await _context.SaveChangesAsync();

            return new CommunityPostDto
            {
                Id = post.Id,
                AuthorId = authorId,
                AuthorName = author.Name,
                Title = title,
                Content = content,
                ImageUrl = imageUrl,
                PostType = postType,
                CreatedAt = post.CreatedAt,
                LikesCount = 0,
                CommentsCount = 0,
                IsLikedByCurrentUser = false
            };
        }

        public async Task<List<CommunityPostDto>> GetPostsAsync(int? limit = null, CommunityPostTypeDto? postType = null)
        {
            var query = _context.CommunityPosts
                .Include(cp => cp.Author)
                .Where(cp => cp.IsActive)
                .AsQueryable();

            if (postType.HasValue)
            {
                query = query.Where(cp => cp.PostType == (CommunityPostType)postType.Value);
            }

            query = query.OrderByDescending(cp => cp.CreatedAt);

            if (limit.HasValue)
            {
                query = query.Take(limit.Value);
            }

            var posts = await query.ToListAsync();

            return posts.Select(p => new CommunityPostDto
            {
                Id = p.Id,
                AuthorId = p.AuthorId,
                AuthorName = p.Author.Name,
                Title = p.Title,
                Content = p.Content,
                ImageUrl = p.ImageUrl,
                PostType = (CommunityPostTypeDto)p.PostType,
                CreatedAt = p.CreatedAt,
                UpdatedAt = p.UpdatedAt,
                LikesCount = p.LikesCount,
                CommentsCount = p.CommentsCount,
                IsLikedByCurrentUser = false // Sera mis à jour si on passe un userId
            }).ToList();
        }

        public async Task<CommunityPostDto?> GetPostByIdAsync(int postId)
        {
            var post = await _context.CommunityPosts
                .Include(cp => cp.Author)
                .FirstOrDefaultAsync(cp => cp.Id == postId && cp.IsActive);

            if (post == null)
                return null;

            return new CommunityPostDto
            {
                Id = post.Id,
                AuthorId = post.AuthorId,
                AuthorName = post.Author.Name,
                Title = post.Title,
                Content = post.Content,
                ImageUrl = post.ImageUrl,
                PostType = (CommunityPostTypeDto)post.PostType,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                LikesCount = post.LikesCount,
                CommentsCount = post.CommentsCount,
                IsLikedByCurrentUser = false
            };
        }

        public async Task<bool> LikePostAsync(int postId, int userId)
        {
            var existingLike = await _context.CommunityPostLikes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (existingLike != null)
                return false; // Déjà liké

            var post = await _context.CommunityPosts.FindAsync(postId);
            if (post == null)
                return false;

            var like = new CommunityPostLike
            {
                PostId = postId,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            _context.CommunityPostLikes.Add(like);
            post.LikesCount++;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UnlikePostAsync(int postId, int userId)
        {
            var like = await _context.CommunityPostLikes
                .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId);

            if (like == null)
                return false;

            var post = await _context.CommunityPosts.FindAsync(postId);
            if (post != null)
            {
                post.LikesCount--;
            }

            _context.CommunityPostLikes.Remove(like);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<CommunityPostCommentDto> AddCommentAsync(int postId, int authorId, string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Le contenu du commentaire ne peut pas être vide.");

            var post = await _context.CommunityPosts.FindAsync(postId);
            if (post == null)
                throw new ArgumentException("Post introuvable.");

            var author = await _context.Users.FindAsync(authorId);
            if (author == null)
                throw new ArgumentException("Utilisateur introuvable.");

            var comment = new CommunityPostComment
            {
                PostId = postId,
                AuthorId = authorId,
                Content = content,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.CommunityPostComments.Add(comment);
            post.CommentsCount++;
            await _context.SaveChangesAsync();

            return new CommunityPostCommentDto
            {
                Id = comment.Id,
                PostId = postId,
                AuthorId = authorId,
                AuthorName = author.Name,
                Content = content,
                CreatedAt = comment.CreatedAt
            };
        }

        public async Task<List<CommunityPostCommentDto>> GetCommentsAsync(int postId)
        {
            var comments = await _context.CommunityPostComments
                .Include(c => c.Author)
                .Where(c => c.PostId == postId && c.IsActive)
                .OrderBy(c => c.CreatedAt)
                .ToListAsync();

            return comments.Select(c => new CommunityPostCommentDto
            {
                Id = c.Id,
                PostId = c.PostId,
                AuthorId = c.AuthorId,
                AuthorName = c.Author.Name,
                Content = c.Content,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            }).ToList();
        }

        public async Task<bool> DeletePostAsync(int postId, int userId)
        {
            var post = await _context.CommunityPosts.FindAsync(postId);
            if (post == null || post.AuthorId != userId)
                return false;

            post.IsActive = false;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}



