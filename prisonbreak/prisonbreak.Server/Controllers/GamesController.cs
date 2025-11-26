using Microsoft.AspNetCore.Mvc;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;
using prisonbreak.Server.Services;

namespace prisonbreak.Server.Controllers;

/// <summary>
/// Contrôleur API pour la gestion des parties de jeu
/// Gère le cycle de vie d'une partie : création, mise à jour, validation, fin
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly IValidationService _validationService;
    private readonly IPuzzleService _puzzleService;
    private readonly ILogger<GamesController> _logger;

    public GamesController(
        IGameService gameService, 
        IValidationService validationService,
        IPuzzleService puzzleService,
        ILogger<GamesController> logger)
    {
        _gameService = gameService;
        _validationService = validationService;
        _puzzleService = puzzleService;
        _logger = logger;
    }

    /// <summary>
    /// POST api/games
    /// Crée une nouvelle partie pour un puzzle
    /// </summary>
    /// <param name="request">Requête contenant l'ID du puzzle et de la session</param>
    /// <returns>La partie créée</returns>
    [HttpPost]
    public async Task<ActionResult<GameDto>> CreateGame([FromBody] CreateGameRequest request)
    {
        try
        {
            if (request == null)
            {
                return BadRequest("La requête ne peut pas être nulle");
            }

            _logger.LogInformation("Création d'une partie : PuzzleId={PuzzleId}, SessionId={SessionId}", 
                request.PuzzleId, request.SessionId);

            var game = await _gameService.CreateGameAsync(request.PuzzleId, request.SessionId);
            
            _logger.LogInformation("Partie créée avec succès : GameId={GameId}", game.Id);
            
            var gameDto = _gameService.ConvertToDto(game);
            
            _logger.LogInformation("Nouvelle partie créée : {GameId} pour le puzzle {PuzzleId} et la session {SessionId}", 
                game.Id, game.PuzzleId, game.SessionId);
            
            return CreatedAtAction(nameof(GetGameById), new { id = game.Id }, gameDto);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Erreur ArgumentException lors de la création de la partie : {Message}", ex.Message);
            return BadRequest(new { message = ex.Message, error = "ArgumentException" });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Erreur InvalidOperationException lors de la création de la partie : {Message}", ex.Message);
            return BadRequest(new { message = ex.Message, error = "InvalidOperationException" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur inattendue lors de la création de la partie : {Message}", ex.Message);
            return StatusCode(500, new { message = "Erreur interne du serveur", error = ex.GetType().Name, details = ex.Message });
        }
    }

    /// <summary>
    /// GET api/games/{id}
    /// Récupère une partie par son ID
    /// </summary>
    /// <param name="id">ID de la partie</param>
    /// <returns>La partie demandée</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<GameDto>> GetGameById(int id)
    {
        try
        {
            var game = await _gameService.GetGameByIdAsync(id);
            
            if (game == null)
            {
                return NotFound($"La partie avec l'ID {id} n'existe pas");
            }

            var gameDto = _gameService.ConvertToDto(game);
            return Ok(gameDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de la partie {GameId}", id);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// PUT api/games/{id}/bridges
    /// Met à jour les ponts placés par le joueur
    /// </summary>
    /// <param name="id">ID de la partie</param>
    /// <param name="bridges">Liste des ponts placés</param>
    /// <returns>La partie mise à jour</returns>
    [HttpPut("{id}/bridges")]
    public async Task<ActionResult<GameDto>> UpdateBridges(int id, [FromBody] List<BridgeDto> bridges)
    {
        try
        {
            var game = await _gameService.UpdateGameBridgesAsync(id, bridges);
            var gameDto = _gameService.ConvertToDto(game);
            
            return Ok(gameDto);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour des ponts pour la partie {GameId}", id);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// POST api/games/{id}/validate
    /// Valide la solution actuelle du joueur
    /// </summary>
    /// <param name="id">ID de la partie</param>
    /// <returns>Résultat de la validation</returns>
    [HttpPost("{id}/validate")]
    public async Task<ActionResult<ValidationResultDto>> ValidateSolution(int id)
    {
        try
        {
            var game = await _gameService.GetGameByIdAsync(id);
            
            if (game == null)
            {
                return NotFound($"La partie avec l'ID {id} n'existe pas");
            }

            if (game.Puzzle == null)
            {
                return BadRequest("Le puzzle associé à cette partie est introuvable");
            }

            // Désérialiser les ponts du joueur
            var playerBridges = System.Text.Json.JsonSerializer.Deserialize<List<BridgeDto>>(game.PlayerBridgesJson) ?? new();

            // Valider la solution
            var validationResult = _validationService.ValidateSolution(game.Puzzle, playerBridges);

            // Si la solution est valide, terminer la partie
            if (validationResult.IsValid)
            {
                // Calculer le score en fonction du temps, des indices et du niveau de difficulté
                int difficultyLevel = game.Puzzle != null ? (int)game.Puzzle.Difficulty : 1;
                int score = CalculateScore(game.ElapsedSeconds, game.HintsUsed, difficultyLevel);
                await _gameService.CompleteGameAsync(id, GameStatus.Completed, score);
                
                _logger.LogInformation("Partie {GameId} terminée avec succès. Score: {Score}, Niveau: {Difficulty}", 
                    id, score, difficultyLevel);
            }

            return Ok(validationResult);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la validation de la partie {GameId}", id);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// POST api/games/{id}/abandon
    /// Abandonne une partie en cours
    /// </summary>
    /// <param name="id">ID de la partie</param>
    /// <returns>La partie abandonnée</returns>
    [HttpPost("{id}/abandon")]
    public async Task<ActionResult<GameDto>> AbandonGame(int id)
    {
        try
        {
            var game = await _gameService.CompleteGameAsync(id, GameStatus.Abandoned, 0);
            var gameDto = _gameService.ConvertToDto(game);
            
            _logger.LogInformation("Partie {GameId} abandonnée", id);
            
            return Ok(gameDto);
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'abandon de la partie {GameId}", id);
            return StatusCode(500, "Erreur interne du serveur");
        }
    }

    /// <summary>
    /// Calcule le score basé sur le temps, les indices utilisés et le niveau de difficulté
    /// Formule: Score de base (selon niveau) - (temps en secondes) - (indices * 100)
    /// </summary>
    /// <param name="elapsedSeconds">Temps écoulé en secondes</param>
    /// <param name="hintsUsed">Nombre d'indices utilisés</param>
    /// <param name="difficultyLevel">Niveau de difficulté (1=Facile, 2=Moyen, 3=Difficile)</param>
    /// <returns>Score calculé</returns>
    private int CalculateScore(int elapsedSeconds, int hintsUsed, int difficultyLevel)
    {
        // Score de base selon le niveau de difficulté
        int baseScore = difficultyLevel switch
        {
            1 => 1000,  // Facile
            2 => 2000,  // Moyen
            3 => 3000,  // Difficile
            _ => 1000   // Par défaut
        };

        // Pénalité pour le temps (max 50% du score de base)
        int maxTimePenalty = baseScore / 2;
        int timePenalty = Math.Min(elapsedSeconds, maxTimePenalty);

        // Pénalité pour les indices (100 points par indice)
        int hintPenalty = hintsUsed * 100;

        // Score final (minimum 0)
        int score = Math.Max(0, baseScore - timePenalty - hintPenalty);
        return score;
    }
}

/// <summary>
/// Requête pour créer une nouvelle partie
/// </summary>
public class CreateGameRequest
{
    /// <summary>
    /// ID du puzzle à jouer
    /// Requis
    /// </summary>
    public int PuzzleId { get; set; }

    /// <summary>
    /// ID de la session de jeu
    /// Requis - Chaque partie appartient à une session utilisateur
    /// </summary>
    public int SessionId { get; set; }
}

