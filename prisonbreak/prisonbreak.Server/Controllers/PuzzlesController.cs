using Microsoft.AspNetCore.Mvc;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;
using prisonbreak.Server.Services;

namespace prisonbreak.Server.Controllers;

/// <summary>
/// Contrôleur API pour la gestion des puzzles Hashi
/// Expose les endpoints pour récupérer et générer des puzzles
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PuzzlesController : ControllerBase
{
    private readonly IPuzzleService _puzzleService;
    private readonly ILogger<PuzzlesController> _logger;

    public PuzzlesController(IPuzzleService puzzleService, ILogger<PuzzlesController> logger)
    {
        _puzzleService = puzzleService;
        _logger = logger;
    }

    /// <summary>
    /// GET api/puzzles
    /// Récupère tous les puzzles disponibles
    /// </summary>
    /// <returns>Liste de tous les puzzles</returns>
    [HttpGet]
    public async Task<ActionResult<List<PuzzleDto>>> GetAllPuzzles()
    {
        try
        {
            var puzzles = await _puzzleService.GetAllPuzzlesAsync();
            var puzzleDtos = puzzles.Select(p => _puzzleService.ConvertToDto(p)).ToList();
            return Ok(puzzleDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des puzzles");
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// GET api/puzzles/{id}
    /// Récupère un puzzle spécifique par son ID
    /// </summary>
    /// <param name="id">ID du puzzle</param>
    /// <returns>Le puzzle demandé</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<PuzzleDto>> GetPuzzleById(int id)
    {
        try
        {
            var puzzle = await _puzzleService.GetPuzzleByIdAsync(id);
            
            if (puzzle == null)
            {
                return NotFound($"Le puzzle avec l'ID {id} n'existe pas");
            }

            var puzzleDto = _puzzleService.ConvertToDto(puzzle);
            return Ok(puzzleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du puzzle {PuzzleId}", id);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// GET api/puzzles/difficulty/{level}
    /// Récupère les puzzles d'un niveau de difficulté spécifique
    /// </summary>
    /// <param name="level">Niveau de difficulté (1=Easy, 2=Medium, 3=Hard, 4=Expert)</param>
    /// <returns>Liste des puzzles de cette difficulté</returns>
    [HttpGet("difficulty/{level}")]
    public async Task<ActionResult<List<PuzzleDto>>> GetPuzzlesByDifficulty(int level)
    {
        try
        {
            if (!Enum.IsDefined(typeof(DifficultyLevel), level))
            {
                return BadRequest("Niveau de difficulté invalide. Utilisez 1 (Easy), 2 (Medium), 3 (Hard) ou 4 (Expert)");
            }

            var difficulty = (DifficultyLevel)level;
            var puzzles = await _puzzleService.GetPuzzlesByDifficultyAsync(difficulty);
            var puzzleDtos = puzzles.Select(p => _puzzleService.ConvertToDto(p)).ToList();
            
            return Ok(puzzleDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des puzzles de difficulté {Level}", level);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// GET api/puzzles/{id}/solution
    /// Récupère la solution d'un puzzle (les ponts de la solution)
    /// </summary>
    /// <param name="id">ID du puzzle</param>
    /// <returns>Liste des ponts de la solution</returns>
    [HttpGet("{id}/solution")]
    public async Task<ActionResult<List<BridgeDto>>> GetPuzzleSolution(int id)
    {
        try
        {
            var puzzle = await _puzzleService.GetPuzzleByIdAsync(id);
            
            if (puzzle == null)
            {
                return NotFound($"Le puzzle avec l'ID {id} n'existe pas");
            }

            if (puzzle.SolutionBridges == null || !puzzle.SolutionBridges.Any())
            {
                return NotFound("Ce puzzle n'a pas de solution enregistrée");
            }

            var solutionBridges = puzzle.SolutionBridges.Select(b => new BridgeDto
            {
                FromIslandId = b.FromIslandId,
                ToIslandId = b.ToIslandId,
                IsDouble = b.IsDouble
            }).ToList();

            return Ok(solutionBridges);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de la solution du puzzle {PuzzleId}", id);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// POST api/puzzles/generate
    /// Génère un nouveau puzzle aléatoire
    /// </summary>
    /// <param name="request">Paramètres de génération (largeur, hauteur, difficulté)</param>
    /// <returns>Le puzzle généré</returns>
    [HttpPost("generate")]
    public async Task<ActionResult<PuzzleDto>> GeneratePuzzle([FromBody] GeneratePuzzleRequest request)
    {
        try
        {
            // Validation des paramètres
            if (request.Width < 5 || request.Width > 20)
            {
                return BadRequest("La largeur doit être entre 5 et 20");
            }

            if (request.Height < 5 || request.Height > 20)
            {
                return BadRequest("La hauteur doit être entre 5 et 20");
            }

            if (!Enum.IsDefined(typeof(DifficultyLevel), request.Difficulty))
            {
                return BadRequest("Niveau de difficulté invalide");
            }

            var puzzle = await _puzzleService.GeneratePuzzleAsync(
                request.Width, 
                request.Height, 
                (DifficultyLevel)request.Difficulty
            );

            var puzzleDto = _puzzleService.ConvertToDto(puzzle);
            
            _logger.LogInformation("Nouveau puzzle généré : {PuzzleId}", puzzle.Id);
            
            return CreatedAtAction(nameof(GetPuzzleById), new { id = puzzle.Id }, puzzleDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la génération du puzzle");
            return StatusCode(500, "Erreur interne du serveur");
        }
    }
}

/// <summary>
/// Requête pour générer un nouveau puzzle
/// </summary>
public class GeneratePuzzleRequest
{
    /// <summary>
    /// Largeur de la grille (5-20)
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Hauteur de la grille (5-20)
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Niveau de difficulté (1=Easy, 2=Medium, 3=Hard, 4=Expert)
    /// </summary>
    public int Difficulty { get; set; }
}

