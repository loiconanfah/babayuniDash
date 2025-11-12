using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using prisonbreak.Server.Data;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Services;

/// <summary>
/// Service de gestion des parties de jeu
/// Gère la création, la mise à jour et la fin des parties
/// </summary>
public class GameService : IGameService
{
    private readonly HashiDbContext _context;
    private readonly IPuzzleService _puzzleService;

    public GameService(HashiDbContext context, IPuzzleService puzzleService)
    {
        _context = context;
        _puzzleService = puzzleService;
    }

    /// <summary>
    /// Crée une nouvelle partie pour un puzzle
    /// </summary>
    public async Task<Game> CreateGameAsync(int puzzleId, string? playerId = null)
    {
        // Vérifier que le puzzle existe
        var puzzle = await _puzzleService.GetPuzzleByIdAsync(puzzleId);
        if (puzzle == null)
        {
            throw new ArgumentException($"Le puzzle avec l'ID {puzzleId} n'existe pas");
        }

        var game = new Game
        {
            PuzzleId = puzzleId,
            PlayerId = playerId,
            StartedAt = DateTime.UtcNow,
            Status = GameStatus.InProgress,
            PlayerBridgesJson = "[]",
            Score = 0,
            HintsUsed = 0,
            ElapsedSeconds = 0
        };

        _context.Games.Add(game);
        await _context.SaveChangesAsync();

        return game;
    }

    /// <summary>
    /// Récupère une partie par son ID
    /// </summary>
    public async Task<Game?> GetGameByIdAsync(int gameId)
    {
        return await _context.Games
            .Include(g => g.Puzzle)
                .ThenInclude(p => p!.Islands)
            .FirstOrDefaultAsync(g => g.Id == gameId);
    }

    /// <summary>
    /// Met à jour les ponts placés par le joueur dans une partie
    /// </summary>
    public async Task<Game> UpdateGameBridgesAsync(int gameId, List<BridgeDto> bridges)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null)
        {
            throw new ArgumentException($"La partie avec l'ID {gameId} n'existe pas");
        }

        if (game.Status != GameStatus.InProgress)
        {
            throw new InvalidOperationException("Cette partie n'est plus en cours");
        }

        // Sérialiser les ponts en JSON
        game.PlayerBridgesJson = JsonSerializer.Serialize(bridges);
        
        // Mettre à jour le temps écoulé
        game.ElapsedSeconds = (int)(DateTime.UtcNow - game.StartedAt).TotalSeconds;

        await _context.SaveChangesAsync();

        return game;
    }

    /// <summary>
    /// Termine une partie avec un statut et un score
    /// </summary>
    public async Task<Game> CompleteGameAsync(int gameId, GameStatus status, int score)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null)
        {
            throw new ArgumentException($"La partie avec l'ID {gameId} n'existe pas");
        }

        game.Status = status;
        game.CompletedAt = DateTime.UtcNow;
        game.Score = score;
        game.ElapsedSeconds = (int)(DateTime.UtcNow - game.StartedAt).TotalSeconds;

        await _context.SaveChangesAsync();

        return game;
    }

    /// <summary>
    /// Convertit une Game en GameDto pour l'envoyer au frontend
    /// </summary>
    public GameDto ConvertToDto(Game game)
    {
        List<BridgeDto> playerBridges;
        try
        {
            playerBridges = JsonSerializer.Deserialize<List<BridgeDto>>(game.PlayerBridgesJson) ?? new();
        }
        catch
        {
            playerBridges = new();
        }

        return new GameDto
        {
            Id = game.Id,
            PuzzleId = game.PuzzleId,
            Puzzle = game.Puzzle != null ? _puzzleService.ConvertToDto(game.Puzzle) : null,
            StartedAt = game.StartedAt,
            CompletedAt = game.CompletedAt,
            ElapsedSeconds = game.ElapsedSeconds,
            Status = (int)game.Status,
            PlayerBridges = playerBridges,
            Score = game.Score,
            HintsUsed = game.HintsUsed
        };
    }
}

