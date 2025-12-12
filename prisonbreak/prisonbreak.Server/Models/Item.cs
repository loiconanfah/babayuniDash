using System.ComponentModel.DataAnnotations;

namespace prisonbreak.Server.Models;

/// <summary>
/// Représente un item disponible dans la boutique
/// </summary>
public class Item
{
    /// <summary>
    /// Identifiant unique de l'item
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nom de l'item
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description de l'item
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Prix en coins
    /// </summary>
    public int Price { get; set; }

    /// <summary>
    /// Type d'item (Avatar, Theme, PowerUp, etc.)
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ItemType { get; set; } = string.Empty;

    /// <summary>
    /// Rareté de l'item (Common, Rare, Epic, Legendary)
    /// </summary>
    [MaxLength(20)]
    public string Rarity { get; set; } = "Common";

    /// <summary>
    /// URL de l'image de l'item
    /// </summary>
    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// Emoji ou icône de l'item
    /// </summary>
    [MaxLength(10)]
    public string Icon { get; set; } = "🎁";

    /// <summary>
    /// L'item est-il disponible à l'achat ?
    /// </summary>
    public bool IsAvailable { get; set; } = true;

    /// <summary>
    /// Date de création de l'item
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Collection des utilisateurs possédant cet item
    /// Relation plusieurs-à-plusieurs via UserItem
    /// </summary>
    public ICollection<UserItem> UserItems { get; set; } = new List<UserItem>();
}

