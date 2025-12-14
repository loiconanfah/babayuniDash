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
        // Vérifier si des items existent déjà pour éviter les erreurs de foreign key
        var existingItems = await context.Items.ToListAsync();
        if (existingItems.Any())
        {
            // Les items existent déjà, ne rien faire
            return;
        }

        var items = new List<Item>
        {
            // Avatars - Images Unsplash
            new Item
            {
                Name = "Avatar Élégant",
                Description = "Un avatar sophistiqué et raffiné pour votre profil",
                Price = 75,
                ItemType = "Avatar",
                Rarity = "Common",
                ImageUrl = "/assets/items/unsplash_OlTjeydUpQw.png",
                IsAvailable = true
            },
            new Item
            {
                Name = "Avatar Moderne",
                Description = "Un style moderne et contemporain",
                Price = 120,
                ItemType = "Avatar",
                Rarity = "Rare",
                ImageUrl = "/assets/items/unsplash_pVq6YhmDPtk.png",
                IsAvailable = true
            },
            new Item
            {
                Name = "Avatar Premium",
                Description = "Un avatar premium exclusif",
                Price = 250,
                ItemType = "Avatar",
                Rarity = "Epic",
                ImageUrl = "/assets/items/unsplash_tMbQpdguDVQ.png",
                IsAvailable = true
            },
            new Item
            {
                Name = "Avatar Légendaire",
                Description = "L'avatar le plus rare et prestigieux",
                Price = 800,
                ItemType = "Avatar",
                Rarity = "Legendary",
                ImageUrl = "/assets/items/unsplash_tZCrFpSNiIQ.png",
                IsAvailable = true
            },

            // Thèmes - Images génériques
            new Item
            {
                Name = "Thème Classique",
                Description = "Un thème intemporel et élégant",
                Price = 100,
                ItemType = "Theme",
                Rarity = "Common",
                ImageUrl = "/assets/items/image 2.png",
                IsAvailable = true
            },
            new Item
            {
                Name = "Thème Vibrant",
                Description = "Des couleurs vives pour égayer votre expérience",
                Price = 180,
                ItemType = "Theme",
                Rarity = "Rare",
                ImageUrl = "/assets/items/image 2 (1).png",
                IsAvailable = true
            },
            new Item
            {
                Name = "Thème Mystique",
                Description = "Une ambiance mystérieuse et envoûtante",
                Price = 350,
                ItemType = "Theme",
                Rarity = "Epic",
                ImageUrl = "/assets/items/image 2 (2).png",
                IsAvailable = true
            },
            new Item
            {
                Name = "Thème Futuriste",
                Description = "Plongez dans le futur avec ce thème high-tech",
                Price = 600,
                ItemType = "Theme",
                Rarity = "Legendary",
                ImageUrl = "/assets/items/image 7.png",
                IsAvailable = true
            },

            // Équipements Électroniques
            new Item
            {
                Name = "Console de Jeu",
                Description = "Une console rétro pour votre collection",
                Price = 200,
                ItemType = "Decoration",
                Rarity = "Rare",
                ImageUrl = "/assets/items/Property 1=Console.png",
                IsAvailable = true
            },
            new Item
            {
                Name = "Écran Gaming",
                Description = "Un écran gaming haute performance",
                Price = 400,
                ItemType = "Decoration",
                Rarity = "Epic",
                ImageUrl = "/assets/items/Property 1=Gaming.png",
                IsAvailable = true
            },
            new Item
            {
                Name = "iPod Vintage",
                Description = "Un iPod classique pour les nostalgiques",
                Price = 150,
                ItemType = "Decoration",
                Rarity = "Rare",
                ImageUrl = "/assets/items/Property 1=iPod.png",
                IsAvailable = true
            },
            new Item
            {
                Name = "Téléphone Rétro",
                Description = "Un téléphone vintage pour votre profil",
                Price = 120,
                ItemType = "Decoration",
                Rarity = "Common",
                ImageUrl = "/assets/items/Property 1=Landline.png",
                IsAvailable = true
            },
            new Item
            {
                Name = "Radio Antique",
                Description = "Une radio vintage pour une ambiance rétro",
                Price = 100,
                ItemType = "Decoration",
                Rarity = "Common",
                ImageUrl = "/assets/items/Property 1=Radio.png",
                IsAvailable = true
            },
            new Item
            {
                Name = "Télévision Vintage",
                Description = "Une télévision rétro pour votre collection",
                Price = 180,
                ItemType = "Decoration",
                Rarity = "Rare",
                ImageUrl = "/assets/items/Property 1=Television.png",
                IsAvailable = true
            },

            // PowerUps - Images rectangulaires
            new Item
            {
                Name = "Boost d'Énergie",
                Description = "Gagnez un boost d'énergie pour vos parties",
                Price = 80,
                ItemType = "PowerUp",
                Rarity = "Common",
                ImageUrl = "/assets/items/Rectangle 8.png",
                IsAvailable = true
            },
            new Item
            {
                Name = "Multiplicateur x2",
                Description = "Doublez vos gains pendant 30 minutes",
                Price = 200,
                ItemType = "PowerUp",
                Rarity = "Rare",
                ImageUrl = "/assets/items/Rectangle 8 (1).png",
                IsAvailable = true
            },
            new Item
            {
                Name = "Multiplicateur x3",
                Description = "Triplez vos gains pendant 15 minutes",
                Price = 450,
                ItemType = "PowerUp",
                Rarity = "Epic",
                ImageUrl = "/assets/items/Rectangle 8 (2).png",
                IsAvailable = true
            },

            // Items spéciaux
            new Item
            {
                Name = "Badge Top Player",
                Description = "Affichez votre statut de meilleur joueur",
                Price = 500,
                ItemType = "Decoration",
                Rarity = "Epic",
                ImageUrl = "/assets/items/Top.png",
                IsAvailable = true
            },
            new Item
            {
                Name = "Pack Premium",
                Description = "Un pack exclusif avec des bonus",
                Price = 1000,
                ItemType = "PowerUp",
                Rarity = "Legendary",
                ImageUrl = "/assets/items/image 7 (1).png",
                IsAvailable = true
            }
        };

        context.Items.AddRange(items);
        await context.SaveChangesAsync();
    }
}

