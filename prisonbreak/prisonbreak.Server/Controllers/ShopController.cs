using Microsoft.AspNetCore.Mvc;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Services;

namespace prisonbreak.Server.Controllers;

/// <summary>
/// Contrôleur API pour la boutique et les items
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ShopController : ControllerBase
{
    private readonly IItemService _itemService;
    private readonly ILogger<ShopController> _logger;

    public ShopController(IItemService itemService, ILogger<ShopController> logger)
    {
        _itemService = itemService;
        _logger = logger;
    }

    /// <summary>
    /// Récupère tous les items disponibles
    /// </summary>
    [HttpGet("items")]
    public async Task<ActionResult<List<ItemDto>>> GetItems([FromQuery] int? userId = null)
    {
        try
        {
            var items = await _itemService.GetAllItemsAsync(userId);
            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des items");
            return StatusCode(500, "Erreur serveur");
        }
    }

    /// <summary>
    /// Récupère un item par son ID
    /// </summary>
    [HttpGet("items/{id}")]
    public async Task<ActionResult<ItemDto>> GetItem(int id, [FromQuery] int? userId = null)
    {
        try
        {
            var item = await _itemService.GetItemByIdAsync(id, userId);
            if (item == null)
                return NotFound();

            return Ok(item);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de l'item {ItemId}", id);
            return StatusCode(500, "Erreur serveur");
        }
    }

    /// <summary>
    /// Récupère les items possédés par un utilisateur
    /// </summary>
    [HttpGet("users/{userId}/items")]
    public async Task<ActionResult<List<UserItemDto>>> GetUserItems(int userId)
    {
        try
        {
            var items = await _itemService.GetUserItemsAsync(userId);
            return Ok(items);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des items de l'utilisateur {UserId}", userId);
            return StatusCode(500, "Erreur serveur");
        }
    }

    /// <summary>
    /// Achète un item
    /// </summary>
    [HttpPost("purchase")]
    public async Task<ActionResult<UserItemDto>> PurchaseItem([FromBody] PurchaseItemRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var userItem = await _itemService.PurchaseItemAsync(request.ItemId, request.UserId);
            return Ok(userItem);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'achat de l'item {ItemId} par l'utilisateur {UserId}", request.ItemId, request.UserId);
            return StatusCode(500, "Erreur serveur");
        }
    }

    /// <summary>
    /// Équipe ou déséquipe un item
    /// </summary>
    [HttpPut("items/{userItemId}/equip")]
    public async Task<ActionResult<UserItemDto>> EquipItem(int userItemId, [FromBody] EquipItemRequest request)
    {
        if (!ModelState.IsValid)
            return ValidationProblem(ModelState);

        try
        {
            var userItem = await _itemService.EquipItemAsync(userItemId, request.IsEquipped);
            return Ok(userItem);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'équipement de l'item {UserItemId}", userItemId);
            return StatusCode(500, "Erreur serveur");
        }
    }

    /// <summary>
    /// Récupère le nombre de coins d'un utilisateur
    /// </summary>
    [HttpGet("users/{userId}/coins")]
    public async Task<ActionResult<UserCoinsDto>> GetUserCoins(int userId)
    {
        try
        {
            var coins = await _itemService.GetUserCoinsAsync(userId);
            return Ok(new UserCoinsDto { UserId = userId, Coins = coins });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des coins de l'utilisateur {UserId}", userId);
            return StatusCode(500, "Erreur serveur");
        }
    }

    /// <summary>
    /// Ajoute des coins à un utilisateur (pour les récompenses, etc.)
    /// </summary>
    [HttpPost("users/{userId}/coins")]
    public async Task<ActionResult<UserCoinsDto>> AddCoins(int userId, [FromBody] int amount)
    {
        try
        {
            var coins = await _itemService.AddCoinsAsync(userId, amount);
            return Ok(new UserCoinsDto { UserId = userId, Coins = coins });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'ajout de coins à l'utilisateur {UserId}", userId);
            return StatusCode(500, "Erreur serveur");
        }
    }
}

