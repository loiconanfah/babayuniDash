using prisonbreak.Server.DTOs;

namespace prisonbreak.Server.Services;

/// <summary>
/// Interface pour le service de gestion des items et de la boutique
/// </summary>
public interface IItemService
{
    /// <summary>
    /// Récupère tous les items disponibles
    /// </summary>
    Task<List<ItemDto>> GetAllItemsAsync(int? userId = null);

    /// <summary>
    /// Récupère un item par son ID
    /// </summary>
    Task<ItemDto?> GetItemByIdAsync(int itemId, int? userId = null);

    /// <summary>
    /// Récupère les items possédés par un utilisateur
    /// </summary>
    Task<List<UserItemDto>> GetUserItemsAsync(int userId);

    /// <summary>
    /// Achète un item pour un utilisateur
    /// </summary>
    Task<UserItemDto> PurchaseItemAsync(int itemId, int userId);

    /// <summary>
    /// Équipe ou déséquipe un item
    /// </summary>
    Task<UserItemDto> EquipItemAsync(int userItemId, bool isEquipped);

    /// <summary>
    /// Récupère le nombre de coins d'un utilisateur
    /// </summary>
    Task<int> GetUserCoinsAsync(int userId);

    /// <summary>
    /// Ajoute des coins à un utilisateur
    /// </summary>
    Task<int> AddCoinsAsync(int userId, int amount);

    /// <summary>
    /// Récupère l'avatar équipé d'un utilisateur (item de type Avatar qui est équipé)
    /// </summary>
    Task<ItemDto?> GetEquippedAvatarAsync(int userId);
}

