using Microsoft.AspNetCore.Mvc;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Services;

namespace prisonbreak.Server.Controllers;

/// <summary>
/// Contrôleur API pour la gestion des utilisateurs
/// Gère la création, la récupération et la mise à jour des utilisateurs
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    /// <summary>
    /// Initialise une nouvelle instance du contrôleur utilisateurs
    /// </summary>
    /// <param name="userService">Service de gestion des utilisateurs</param>
    /// <param name="logger">Logger pour l'enregistrement des événements</param>
    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// GET api/users
    /// Récupère tous les utilisateurs
    /// </summary>
    /// <param name="includeInactive">Si true, inclut les utilisateurs inactifs</param>
    /// <returns>Liste de tous les utilisateurs</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetAllUsers([FromQuery] bool includeInactive = false)
    {
        try
        {
            var users = await _userService.GetAllUsersAsync(includeInactive);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des utilisateurs");
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// GET api/users/{id}
    /// Récupère un utilisateur par son ID
    /// </summary>
    /// <param name="id">Identifiant unique de l'utilisateur</param>
    /// <returns>L'utilisateur demandé</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetUserById(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            
            if (user == null)
            {
                return NotFound($"L'utilisateur avec l'ID {id} n'existe pas");
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de l'utilisateur {UserId}", id);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// GET api/users/email/{email}
    /// Récupère un utilisateur par son adresse email
    /// </summary>
    /// <param name="email">Adresse email de l'utilisateur</param>
    /// <returns>L'utilisateur demandé</returns>
    [HttpGet("email/{email}")]
    public async Task<ActionResult<UserDto>> GetUserByEmail(string email)
    {
        try
        {
            var user = await _userService.GetUserByEmailAsync(email);
            
            if (user == null)
            {
                return NotFound($"Aucun utilisateur trouvé avec l'email '{email}'");
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de l'utilisateur par email {Email}", email);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// POST api/users
    /// Crée un nouvel utilisateur
    /// </summary>
    /// <param name="request">Requête contenant les informations de l'utilisateur à créer</param>
    /// <returns>L'utilisateur créé</returns>
    [HttpPost]
    public async Task<ActionResult<UserDto>> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.CreateUserAsync(request);
            
            _logger.LogInformation("Nouvel utilisateur créé : {UserId} ({Email})", user.Id, user.Email);
            
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Tentative de création d'un utilisateur avec un email existant");
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création de l'utilisateur");
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// PUT api/users/{id}
    /// Met à jour un utilisateur existant
    /// </summary>
    /// <param name="id">Identifiant de l'utilisateur à mettre à jour</param>
    /// <param name="request">Requête contenant les nouvelles informations</param>
    /// <returns>L'utilisateur mis à jour</returns>
    [HttpPut("{id}")]
    public async Task<ActionResult<UserDto>> UpdateUser(int id, [FromBody] CreateUserRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userService.UpdateUserAsync(id, request);
            
            _logger.LogInformation("Utilisateur mis à jour : {UserId}", id);
            
            return Ok(user);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour de l'utilisateur {UserId}", id);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// DELETE api/users/{id}
    /// Désactive un utilisateur (soft delete)
    /// </summary>
    /// <param name="id">Identifiant de l'utilisateur à désactiver</param>
    /// <returns>Résultat de l'opération</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeactivateUser(int id)
    {
        try
        {
            var result = await _userService.DeactivateUserAsync(id);
            
            if (!result)
            {
                return NotFound($"L'utilisateur avec l'ID {id} n'existe pas");
            }

            _logger.LogInformation("Utilisateur désactivé : {UserId}", id);
            
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la désactivation de l'utilisateur {UserId}", id);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }
}

