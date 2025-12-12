using Microsoft.AspNetCore.Mvc;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Services;

namespace prisonbreak.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        [HttpPost("send")]
        public async Task<ActionResult<ChatMessageDto>> SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                // Le service envoie déjà via SignalR, on retourne juste le message
                var message = await _chatService.SendMessageAsync(request.SenderId, request.ReceiverId, request.Content);
                return Ok(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi du message");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("conversation/{userId1}/{userId2}")]
        public async Task<ActionResult<List<ChatMessageDto>>> GetConversation(int userId1, int userId2)
        {
            try
            {
                var messages = await _chatService.GetConversationAsync(userId1, userId2);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de la conversation");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("conversations/{userId}")]
        public async Task<ActionResult<List<ChatConversationDto>>> GetConversations(int userId)
        {
            try
            {
                var conversations = await _chatService.GetConversationsAsync(userId);
                return Ok(conversations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des conversations");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("message/{messageId}/read")]
        public async Task<ActionResult> MarkAsRead(int messageId, [FromBody] MarkAsReadRequest request)
        {
            try
            {
                var result = await _chatService.MarkAsReadAsync(messageId, request.UserId);
                if (result)
                    return Ok(new { message = "Message marqué comme lu" });
                return BadRequest(new { message = "Impossible de marquer le message comme lu" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du marquage du message comme lu");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("unread/{userId}")]
        public async Task<ActionResult<int>> GetUnreadCount(int userId)
        {
            try
            {
                var count = await _chatService.GetUnreadCountAsync(userId);
                return Ok(new { count });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération du nombre de messages non lus");
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class SendMessageRequest
    {
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    public class MarkAsReadRequest
    {
        public int UserId { get; set; }
    }
}



