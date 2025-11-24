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
    }
}

