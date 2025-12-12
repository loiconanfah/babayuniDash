using Microsoft.EntityFrameworkCore;
using prisonbreak.Server.Data;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Services;

/// <summary>
/// Service pour la gestion des items et de la boutique
/// </summary>
public class ItemService : IItemService
{
    private readonly HashiDbContext _context;
    private readonly IUserService _userService;

    public ItemService(HashiDbContext context, IUserService userService)
    {
        _context = context;
        _userService = userService;
    }

    public async Task<List<ItemDto>> GetAllItemsAsync(int? userId = null)
    {
        var items = await _context.Items
            .Where(i => i.IsAvailable)
            .OrderBy(i => i.Price)
            .ToListAsync();

        var userItems = userId.HasValue
            ? await _context.UserItems
                .Where(ui => ui.UserId == userId.Value)
                .Select(ui => ui.ItemId)
                .ToListAsync()
            : new List<int>();

        return items.Select(item => new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            ItemType = item.ItemType,
            Rarity = item.Rarity,
            ImageUrl = item.ImageUrl,
            Icon = item.Icon,
            IsAvailable = item.IsAvailable,
            IsOwned = userItems.Contains(item.Id)
        }).ToList();
    }

    public async Task<ItemDto?> GetItemByIdAsync(int itemId, int? userId = null)
    {
        var item = await _context.Items.FindAsync(itemId);
        if (item == null) return null;

        var isOwned = userId.HasValue && await _context.UserItems
            .AnyAsync(ui => ui.UserId == userId.Value && ui.ItemId == itemId);

        return new ItemDto
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description,
            Price = item.Price,
            ItemType = item.ItemType,
            Rarity = item.Rarity,
            ImageUrl = item.ImageUrl,
            Icon = item.Icon,
            IsAvailable = item.IsAvailable,
            IsOwned = isOwned
        };
    }

    public async Task<List<UserItemDto>> GetUserItemsAsync(int userId)
    {
        var userItems = await _context.UserItems
            .Include(ui => ui.Item)
            .Where(ui => ui.UserId == userId)
            .OrderByDescending(ui => ui.PurchasedAt)
            .ToListAsync();

        return userItems.Select(ui => new UserItemDto
        {
            Id = ui.Id,
            ItemId = ui.ItemId,
            Item = new ItemDto
            {
                Id = ui.Item.Id,
                Name = ui.Item.Name,
                Description = ui.Item.Description,
                Price = ui.Item.Price,
                ItemType = ui.Item.ItemType,
                Rarity = ui.Item.Rarity,
                ImageUrl = ui.Item.ImageUrl,
                Icon = ui.Item.Icon,
                IsAvailable = ui.Item.IsAvailable,
                IsOwned = true
            },
            PurchasedAt = ui.PurchasedAt,
            IsEquipped = ui.IsEquipped
        }).ToList();
    }

    public async Task<UserItemDto> PurchaseItemAsync(int itemId, int userId)
    {
        // Vérifier que l'item existe et est disponible
        var item = await _context.Items.FindAsync(itemId);
        if (item == null)
            throw new ArgumentException("L'item n'existe pas", nameof(itemId));

        if (!item.IsAvailable)
            throw new InvalidOperationException("Cet item n'est pas disponible à l'achat");

        // Vérifier que l'utilisateur n'a pas déjà cet item
        var existingUserItem = await _context.UserItems
            .FirstOrDefaultAsync(ui => ui.UserId == userId && ui.ItemId == itemId);

        if (existingUserItem != null)
            throw new InvalidOperationException("Vous possédez déjà cet item");

        // Vérifier que l'utilisateur a assez de coins
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new ArgumentException("L'utilisateur n'existe pas", nameof(userId));

        if (user.Coins < item.Price)
            throw new InvalidOperationException("Vous n'avez pas assez de coins");

        // Débiter les coins
        user.RemoveCoins(item.Price);
        await _context.SaveChangesAsync();

        // Créer le UserItem
        var userItem = new UserItem
        {
            UserId = userId,
            ItemId = itemId,
            PurchasedAt = DateTime.UtcNow,
            IsEquipped = false
        };

        _context.UserItems.Add(userItem);
        await _context.SaveChangesAsync();

        // Charger l'item pour le DTO
        await _context.Entry(userItem).Reference(ui => ui.Item).LoadAsync();

        return new UserItemDto
        {
            Id = userItem.Id,
            ItemId = userItem.ItemId,
            Item = new ItemDto
            {
                Id = userItem.Item.Id,
                Name = userItem.Item.Name,
                Description = userItem.Item.Description,
                Price = userItem.Item.Price,
                ItemType = userItem.Item.ItemType,
                Rarity = userItem.Item.Rarity,
                ImageUrl = userItem.Item.ImageUrl,
                Icon = userItem.Item.Icon,
                IsAvailable = userItem.Item.IsAvailable,
                IsOwned = true
            },
            PurchasedAt = userItem.PurchasedAt,
            IsEquipped = userItem.IsEquipped
        };
    }

    public async Task<UserItemDto> EquipItemAsync(int userItemId, bool isEquipped)
    {
        var userItem = await _context.UserItems
            .Include(ui => ui.Item)
            .FirstOrDefaultAsync(ui => ui.Id == userItemId);

        if (userItem == null)
            throw new ArgumentException("L'item utilisateur n'existe pas", nameof(userItemId));

        // Si on équipe un item, déséquiper les autres items du même type
        if (isEquipped)
        {
            var otherItems = await _context.UserItems
                .Where(ui => ui.UserId == userItem.UserId 
                    && ui.Item.ItemType == userItem.Item.ItemType 
                    && ui.Id != userItemId 
                    && ui.IsEquipped)
                .ToListAsync();

            foreach (var otherItem in otherItems)
            {
                otherItem.IsEquipped = false;
            }
        }

        userItem.IsEquipped = isEquipped;
        await _context.SaveChangesAsync();

        return new UserItemDto
        {
            Id = userItem.Id,
            ItemId = userItem.ItemId,
            Item = new ItemDto
            {
                Id = userItem.Item.Id,
                Name = userItem.Item.Name,
                Description = userItem.Item.Description,
                Price = userItem.Item.Price,
                ItemType = userItem.Item.ItemType,
                Rarity = userItem.Item.Rarity,
                ImageUrl = userItem.Item.ImageUrl,
                Icon = userItem.Item.Icon,
                IsAvailable = userItem.Item.IsAvailable,
                IsOwned = true
            },
            PurchasedAt = userItem.PurchasedAt,
            IsEquipped = userItem.IsEquipped
        };
    }

    public async Task<int> GetUserCoinsAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new ArgumentException("L'utilisateur n'existe pas", nameof(userId));

        return user.Coins;
    }

    public async Task<int> AddCoinsAsync(int userId, int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Le montant doit être positif", nameof(amount));

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            throw new ArgumentException("L'utilisateur n'existe pas", nameof(userId));

        user.AddCoins(amount);
        await _context.SaveChangesAsync();

        return user.Coins;
    }
}

