namespace prisonbreak.Server.DTOs;

/// <summary>
/// DTO pour un item de la boutique
/// </summary>
public class ItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Price { get; set; }
    public string ItemType { get; set; } = string.Empty;
    public string Rarity { get; set; } = "Common";
    public string ImageUrl { get; set; } = string.Empty;
    public string Icon { get; set; } = "🎁";
    public bool IsAvailable { get; set; }
    public bool IsOwned { get; set; } = false; // Si l'utilisateur actuel possède cet item
}

/// <summary>
/// DTO pour un item possédé par un utilisateur
/// </summary>
public class UserItemDto
{
    public int Id { get; set; }
    public int ItemId { get; set; }
    public ItemDto Item { get; set; } = null!;
    public DateTime PurchasedAt { get; set; }
    public bool IsEquipped { get; set; }
}

/// <summary>
/// DTO pour les coins d'un utilisateur
/// </summary>
public class UserCoinsDto
{
    public int UserId { get; set; }
    public int Coins { get; set; }
}

/// <summary>
/// Requête pour acheter un item
/// </summary>
public class PurchaseItemRequest
{
    public int ItemId { get; set; }
    public int UserId { get; set; }
}

/// <summary>
/// Requête pour équiper/déséquiper un item
/// </summary>
public class EquipItemRequest
{
    public int UserItemId { get; set; }
    public bool IsEquipped { get; set; }
}

