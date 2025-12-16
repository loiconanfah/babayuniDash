using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Services;

/// <summary>
/// Interface pour le service de gestion des puzzles
/// Gère la création, la récupération et la manipulation des puzzles
/// </summary>
public interface IPuzzleService
{
    /// <summary>
    /// Génère un nouveau puzzle aléatoire
    /// </summary>
    /// <param name="width">Largeur de la grille</param>
    /// <param name="height">Hauteur de la grille</param>
    /// <param name="difficulty">Niveau de difficulté</param>
    /// <param name="theme">Thème visuel du puzzle (par défaut: Classic)</param>
    /// <returns>Le puzzle généré</returns>
    Task<Puzzle> GeneratePuzzleAsync(int width, int height, DifficultyLevel difficulty, PuzzleTheme theme = PuzzleTheme.Classic);

    /// <summary>
    /// Récupère un puzzle par son ID
    /// </summary>
    /// <param name="puzzleId">ID du puzzle</param>
    /// <returns>Le puzzle ou null s'il n'existe pas</returns>
    Task<Puzzle?> GetPuzzleByIdAsync(int puzzleId);

    /// <summary>
    /// Récupère tous les puzzles
    /// </summary>
    /// <returns>Liste de tous les puzzles</returns>
    Task<List<Puzzle>> GetAllPuzzlesAsync();

    /// <summary>
    /// Récupère les puzzles par difficulté
    /// </summary>
    /// <param name="difficulty">Niveau de difficulté</param>
    /// <returns>Liste des puzzles de cette difficulté</returns>
    Task<List<Puzzle>> GetPuzzlesByDifficultyAsync(DifficultyLevel difficulty);

    /// <summary>
    /// Convertit un Puzzle en PuzzleDto
    /// </summary>
    /// <param name="puzzle">Le puzzle à convertir</param>
    /// <returns>Le DTO du puzzle</returns>
    PuzzleDto ConvertToDto(Puzzle puzzle);
}

