namespace prisonbreak.Server.Models;

/// <summary>
/// Table de jonction pour la relation plusieurs-à-plusieurs entre User et Item
/// Représente un item possédé par un utilisateur
/// </summary>
public class UserItem
{
    /// <summary>
    /// Identifiant unique de la relation
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant de l'utilisateur
    /// </summary>
    public int UserId { get; set; }

    /// <summary>
    /// Navigation vers l'utilisateur
    /// </summary>
    public User User { get; set; } = null!;

    /// <summary>
    /// Identifiant de l'item
    /// </summary>
    public int ItemId { get; set; }

    /// <summary>
    /// Navigation vers l'item
    /// </summary>
    public Item Item { get; set; } = null!;

    /// <summary>
    /// Date d'achat de l'item
    /// </summary>
    public DateTime PurchasedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// L'item est-il actuellement équipé ?
    /// </summary>
    public bool IsEquipped { get; set; } = false;
}

