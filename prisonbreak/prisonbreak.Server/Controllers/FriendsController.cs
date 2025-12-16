using Microsoft.AspNetCore.Mvc;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Services;

namespace prisonbreak.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FriendsController : ControllerBase
    {
        private readonly IFriendshipService _friendshipService;
        private readonly ILogger<FriendsController> _logger;

        public FriendsController(IFriendshipService friendshipService, ILogger<FriendsController> logger)
        {
            _friendshipService = friendshipService;
            _logger = logger;
        }

        [HttpPost("request")]
        public async Task<ActionResult<FriendRequestDto>> SendFriendRequest([FromBody] SendFriendRequestRequest request)
        {
            try
            {
                var friendRequest = await _friendshipService.SendFriendRequestAsync(request.RequesterId, request.ReceiverId);
                return Ok(friendRequest);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi de la demande d'amitié");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("request/{requestId}/accept")]
        public async Task<ActionResult> AcceptFriendRequest(int requestId, [FromBody] AcceptFriendRequestRequest request)
        {
            try
            {
                var result = await _friendshipService.AcceptFriendRequestAsync(requestId, request.UserId);
                if (result)
                    return Ok(new { message = "Demande d'amitié acceptée" });
                return BadRequest(new { message = "Impossible d'accepter la demande" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'acceptation de la demande d'amitié");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("request/{requestId}/reject")]
        public async Task<ActionResult> RejectFriendRequest(int requestId, [FromBody] RejectFriendRequestRequest request)
        {
            try
            {
                var result = await _friendshipService.RejectFriendRequestAsync(requestId, request.UserId);
                if (result)
                    return Ok(new { message = "Demande d'amitié refusée" });
                return BadRequest(new { message = "Impossible de refuser la demande" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du refus de la demande d'amitié");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<List<FriendDto>>> GetFriends(int userId)
        {
            try
            {
                var friends = await _friendshipService.GetFriendsAsync(userId);
                return Ok(friends);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des amis");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{userId}/requests/pending")]
        public async Task<ActionResult<List<FriendRequestDto>>> GetPendingRequests(int userId)
        {
            try
            {
                var requests = await _friendshipService.GetPendingRequestsAsync(userId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des demandes en attente");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("{userId}/requests/sent")]
        public async Task<ActionResult<List<FriendRequestDto>>> GetSentRequests(int userId)
        {
            try
            {
                var requests = await _friendshipService.GetSentRequestsAsync(userId);
                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des demandes envoyées");
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{userId}/{friendId}")]
        public async Task<ActionResult> RemoveFriend(int userId, int friendId)
        {
            try
            {
                var result = await _friendshipService.RemoveFriendAsync(userId, friendId);
                if (result)
                    return Ok(new { message = "Ami supprimé" });
                return BadRequest(new { message = "Impossible de supprimer l'ami" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de l'ami");
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class SendFriendRequestRequest
    {
        public int RequesterId { get; set; }
        public int ReceiverId { get; set; }
    }

    public class AcceptFriendRequestRequest
    {
        public int UserId { get; set; }
    }

    public class RejectFriendRequestRequest
    {
        public int UserId { get; set; }
    }
}

