using Microsoft.EntityFrameworkCore;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Data;

/// <summary>
/// Classe pour initialiser les items de base dans la boutique
/// </summary>
public static class SeedItems
{
    /// <summary>
    /// Ajoute les items de base à la base de données s'ils n'existent pas déjà
    /// </summary>
    public static async Task SeedAsync(HashiDbContext context)
    {
        if (await context.Items.AnyAsync())
        {
            return; // Les items existent déjà
        }

        var items = new List<Item>
        {
            // Avatars
            new Item
            {
                Name = "Avatar Classique",
                Description = "Un avatar élégant et intemporel",
                Price = 50,
                ItemType = "Avatar",
                Rarity = "Common",
                Icon = "👤",
                IsAvailable = true
            },
            new Item
            {
                Name = "Avatar Guerrier",
                Description = "Un avatar de guerrier courageux",
                Price = 150,
                ItemType = "Avatar",
                Rarity = "Rare",
                Icon = "⚔️",
                IsAvailable = true
            },
            new Item
            {
                Name = "Avatar Mage",
                Description = "Un avatar de mage puissant",
                Price = 300,
                ItemType = "Avatar",
                Rarity = "Epic",
                Icon = "🧙",
                IsAvailable = true
            },
            new Item
            {
                Name = "Avatar Légendaire",
                Description = "Un avatar légendaire très rare",
                Price = 1000,
                ItemType = "Avatar",
                Rarity = "Legendary",
                Icon = "👑",
                IsAvailable = true
            },

            // Themes
            new Item
            {
                Name = "Thème Sombre",
                Description = "Un thème sombre pour une ambiance mystérieuse",
                Price = 100,
                ItemType = "Theme",
                Rarity = "Common",
                Icon = "🌙",
                IsAvailable = true
            },
            new Item
            {
                Name = "Thème Arc-en-ciel",
                Description = "Un thème coloré et joyeux",
                Price = 200,
                ItemType = "Theme",
                Rarity = "Rare",
                Icon = "🌈",
                IsAvailable = true
            },
            new Item
            {
                Name = "Thème Néon",
                Description = "Un thème futuriste avec des effets néon",
                Price = 500,
                ItemType = "Theme",
                Rarity = "Epic",
                Icon = "💡",
                IsAvailable = true
            },

            // PowerUps
            new Item
            {
                Name = "Indice Bonus",
                Description = "Gagnez un indice gratuit pour vos puzzles",
                Price = 75,
                ItemType = "PowerUp",
                Rarity = "Common",
                Icon = "💡",
                IsAvailable = true
            },
            new Item
            {
                Name = "Temps Bonus",
                Description = "Gagnez 30 secondes supplémentaires",
                Price = 100,
                ItemType = "PowerUp",
                Rarity = "Rare",
                Icon = "⏰",
                IsAvailable = true
            },
            new Item
            {
                Name = "Récompense Double",
                Description = "Doublez vos coins gagnés pendant 1 heure",
                Price = 250,
                ItemType = "PowerUp",
                Rarity = "Epic",
                Icon = "💰",
                IsAvailable = true
            },

            // Decorations
            new Item
            {
                Name = "Cadre Or",
                Description = "Un cadre doré pour votre profil",
                Price = 150,
                ItemType = "Decoration",
                Rarity = "Rare",
                Icon = "🖼️",
                IsAvailable = true
            },
            new Item
            {
                Name = "Badge Premium",
                Description = "Affichez votre statut premium",
                Price = 500,
                ItemType = "Decoration",
                Rarity = "Epic",
                Icon = "⭐",
                IsAvailable = true
            }
        };

        context.Items.AddRange(items);
        await context.SaveChangesAsync();
    }
}

