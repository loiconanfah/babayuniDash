using Microsoft.AspNetCore.Mvc;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Services;

namespace prisonbreak.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommunityController : ControllerBase
    {
        private readonly ICommunityService _communityService;
        private readonly ILogger<CommunityController> _logger;

        public CommunityController(ICommunityService communityService, ILogger<CommunityController> logger)
        {
            _communityService = communityService;
            _logger = logger;
        }

        [HttpPost("posts")]
        public async Task<ActionResult<CommunityPostDto>> CreatePost([FromBody] CreatePostRequest request)
        {
            try
            {
                // Convertir la string en enum si nécessaire
                CommunityPostTypeDto postType;
                if (!string.IsNullOrEmpty(request.PostTypeString) && Enum.TryParse<CommunityPostTypeDto>(request.PostTypeString, true, out var parsedType))
                {
                    postType = parsedType;
                }
                else if (request.PostType != default(CommunityPostTypeDto))
                {
                    postType = request.PostType;
                }
                else
                {
                    postType = CommunityPostTypeDto.Discussion; // Par défaut
                }

                if (request.AuthorId <= 0)
                {
                    return BadRequest(new { message = "L'ID de l'auteur est requis." });
                }

                if (string.IsNullOrWhiteSpace(request.Title))
                {
                    return BadRequest(new { message = "Le titre est requis." });
                }

                if (string.IsNullOrWhiteSpace(request.Content))
                {
                    return BadRequest(new { message = "Le contenu est requis." });
                }

                var post = await _communityService.CreatePostAsync(request.AuthorId, request.Title, request.Content, request.ImageUrl, postType);
                return Ok(post);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création du post: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("posts")]
        public async Task<ActionResult<List<CommunityPostDto>>> GetPosts([FromQuery] int? limit, [FromQuery] CommunityPostTypeDto? postType)
        {
            try
            {
                var posts = await _communityService.GetPostsAsync(limit, postType);
                return Ok(posts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des posts");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("posts/{postId}")]
        public async Task<ActionResult<CommunityPostDto>> GetPost(int postId)
        {
            try
            {
                var post = await _communityService.GetPostByIdAsync(postId);
                if (post == null)
                    return NotFound();
                return Ok(post);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du post");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("posts/{postId}/like")]
        public async Task<ActionResult> LikePost(int postId, [FromBody] LikePostRequest request)
        {
            try
            {
                var result = await _communityService.LikePostAsync(postId, request.UserId);
                if (result)
                    return Ok(new { message = "Post liké" });
                return BadRequest(new { message = "Impossible de liker le post" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du like du post");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("posts/{postId}/like")]
        public async Task<ActionResult> UnlikePost(int postId, [FromBody] UnlikePostRequest request)
        {
            try
            {
                var result = await _communityService.UnlikePostAsync(postId, request.UserId);
                if (result)
                    return Ok(new { message = "Like retiré" });
                return BadRequest(new { message = "Impossible de retirer le like" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du retrait du like");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("posts/{postId}/comments")]
        public async Task<ActionResult<CommunityPostCommentDto>> AddComment(int postId, [FromBody] AddCommentRequest request)
        {
            try
            {
                var comment = await _communityService.AddCommentAsync(postId, request.AuthorId, request.Content);
                return Ok(comment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'ajout du commentaire");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("posts/{postId}/comments")]
        public async Task<ActionResult<List<CommunityPostCommentDto>>> GetComments(int postId)
        {
            try
            {
                var comments = await _communityService.GetCommentsAsync(postId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des commentaires");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("posts/{postId}")]
        public async Task<ActionResult> DeletePost(int postId, [FromBody] DeletePostRequest request)
        {
            try
            {
                var result = await _communityService.DeletePostAsync(postId, request.UserId);
                if (result)
                    return Ok(new { message = "Post supprimé" });
                return BadRequest(new { message = "Impossible de supprimer le post" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression du post");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("upload")]
        public async Task<ActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "Aucun fichier fourni" });

                // Vérifier le type de fichier
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                    return BadRequest(new { message = "Type de fichier non autorisé. Utilisez JPG, PNG, GIF ou WEBP." });

                // Vérifier la taille (max 20MB pour permettre les images de plus de 10MB)
                if (file.Length > 20 * 1024 * 1024)
                    return BadRequest(new { message = "Le fichier est trop volumineux. Taille maximale: 20MB" });

                // Créer le dossier uploads s'il n'existe pas
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "community");
                if (!Directory.Exists(uploadsFolder))
                    Directory.CreateDirectory(uploadsFolder);

                // Générer un nom de fichier unique
                var fileName = $"{Guid.NewGuid()}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Sauvegarder le fichier
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Retourner l'URL relative
                var url = $"/uploads/community/{fileName}";
                return Ok(new { url });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'upload de l'image");
                return StatusCode(500, new { message = "Erreur lors de l'upload de l'image" });
            }
        }
    }

    public class CreatePostRequest
    {
        public int AuthorId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
        public CommunityPostTypeDto PostType { get; set; }
        public string? PostTypeString { get; set; } // Pour accepter une string depuis le frontend
    }

    public class LikePostRequest
    {
        public int UserId { get; set; }
    }

    public class UnlikePostRequest
    {
        public int UserId { get; set; }
    }

    public class AddCommentRequest
    {
        public int AuthorId { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    public class DeletePostRequest
    {
        public int UserId { get; set; }
    }
}

