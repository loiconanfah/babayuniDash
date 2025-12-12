using Microsoft.AspNetCore.Mvc;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Services;

namespace prisonbreak.Server.Controllers
{
    /// <summary>
    /// Contrôleur API exposant les endpoints utilisateur.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<ActionResult<UserDto>> CreateOrLogin([FromBody] CreateUserRequest request)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            _logger.LogInformation("CreateOrLogin: {Email}", request.Email);

            var result = await _userService.CreateOrLoginAsync(request.Name, request.Email);

            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<ActionResult<List<UserDto>>> SearchUsers([FromQuery] string email, [FromQuery] int limit = 10)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return Ok(new List<UserDto>());

                var users = await _userService.SearchUsersByEmailAsync(email, limit);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la recherche d'utilisateurs");
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

