using Microsoft.EntityFrameworkCore;
using prisonbreak.Server.Data;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Services;

/// <summary>
/// Service de gestion des puzzles Hashi
/// Gère la génération, la récupération et la conversion des puzzles
/// </summary>
public class PuzzleService : IPuzzleService
{
    private readonly HashiDbContext _context;
    private readonly Random _random = new();

    public PuzzleService(HashiDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Génère un nouveau puzzle Hashi aléatoire
    /// IMPORTANT: Cette implémentation est simplifiée pour la démonstration
    /// TODO: Implémenter un générateur de puzzle plus sophistiqué qui garantit une solution unique
    /// </summary>
    public async Task<Puzzle> GeneratePuzzleAsync(int width, int height, DifficultyLevel difficulty)
    {
        // Créer le puzzle
        var puzzle = new Puzzle
        {
            Name = $"Puzzle {difficulty} ({width}x{height})",
            Width = width,
            Height = height,
            Difficulty = difficulty,
            CreatedAt = DateTime.UtcNow
        };

        // Déterminer le nombre d'îles selon la difficulté
        int islandCount = difficulty switch
        {
            DifficultyLevel.Easy => Math.Min(width * height / 4, 8),
            DifficultyLevel.Medium => Math.Min(width * height / 3, 12),
            DifficultyLevel.Hard => Math.Min(width * height / 2, 16),
            DifficultyLevel.Expert => Math.Min(width * height / 2, 20),
            _ => 8
        };

        // Générer des positions aléatoires pour les îles
        var usedPositions = new HashSet<(int, int)>();
        var islands = new List<Island>();

        for (int i = 0; i < islandCount; i++)
        {
            int x, y;
            int attempts = 0;
            
            // Trouver une position libre
            do
            {
                x = _random.Next(0, width);
                y = _random.Next(0, height);
                attempts++;
            } while (usedPositions.Contains((x, y)) && attempts < 100);

            if (attempts >= 100)
                break; // Impossible de placer plus d'îles

            usedPositions.Add((x, y));

            // Créer l'île avec un nombre de ponts requis aléatoire (pour l'instant)
            // TODO: Calculer le nombre exact basé sur la solution
            var island = new Island
            {
                X = x,
                Y = y,
                RequiredBridges = _random.Next(1, 9), // Temporaire
                Puzzle = puzzle
            };

            islands.Add(island);
        }

        puzzle.Islands = islands;

        // TODO: Générer une solution valide et calculer les RequiredBridges corrects
        // Pour l'instant, on crée un puzzle basique pour tester l'architecture

        // Sauvegarder dans la base de données
        _context.Puzzles.Add(puzzle);
        await _context.SaveChangesAsync();

        return puzzle;
    }

    /// <summary>
    /// Récupère un puzzle par son ID avec toutes ses îles
    /// </summary>
    public async Task<Puzzle?> GetPuzzleByIdAsync(int puzzleId)
    {
        return await _context.Puzzles
            .Include(p => p.Islands)
            .Include(p => p.SolutionBridges)
            .FirstOrDefaultAsync(p => p.Id == puzzleId);
    }

    /// <summary>
    /// Récupère tous les puzzles
    /// </summary>
    public async Task<List<Puzzle>> GetAllPuzzlesAsync()
    {
        return await _context.Puzzles
            .Include(p => p.Islands)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Récupère les puzzles par difficulté
    /// </summary>
    public async Task<List<Puzzle>> GetPuzzlesByDifficultyAsync(DifficultyLevel difficulty)
    {
        return await _context.Puzzles
            .Include(p => p.Islands)
            .Where(p => p.Difficulty == difficulty)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Convertit un Puzzle en PuzzleDto pour l'envoyer au frontend
    /// </summary>
    public PuzzleDto ConvertToDto(Puzzle puzzle)
    {
        return new PuzzleDto
        {
            Id = puzzle.Id,
            Name = puzzle.Name,
            Width = puzzle.Width,
            Height = puzzle.Height,
            Difficulty = (int)puzzle.Difficulty,
            IslandCount = puzzle.Islands.Count,
            Islands = puzzle.Islands.Select(i => new IslandDto
            {
                Id = i.Id,
                X = i.X,
                Y = i.Y,
                RequiredBridges = i.RequiredBridges,
                CurrentBridges = 0 // Le joueur commence avec 0 ponts placés
            }).ToList()
        };
    }
}

