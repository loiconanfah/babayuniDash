using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using prisonbreak.Server.Data;
using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;
using prisonbreak.Server.Repositories;

namespace prisonbreak.Server.Services;

/// <summary>
/// Service de gestion des parties d'aventure
/// Gère l'exploration, la collecte d'objets et la résolution d'énigmes
/// </summary>
public class AdventureService : IAdventureService
{
    private readonly HashiDbContext _context;
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<AdventureService>? _logger;

    // Configuration des salles et énigmes
    private static readonly Dictionary<int, RoomInfo> Rooms = new()
    {
        { 1, new RoomInfo { Name = "Entrée du Château", Description = "Vous êtes devant l'entrée d'un mystérieux château. La porte est verrouillée.", RequiredItems = new List<string>(), Puzzles = new List<int> { 1, 2 } } },
        { 2, new RoomInfo { Name = "Hall Principal", Description = "Un grand hall avec des portraits anciens. Une clé brille sur une table.", RequiredItems = new List<string>(), Puzzles = new List<int> { 3, 4 } } },
        { 3, new RoomInfo { Name = "Bibliothèque", Description = "Des milliers de livres. Un livre particulier attire votre attention.", RequiredItems = new List<string> { "key" }, Puzzles = new List<int> { 5, 6, 7 } } },
        { 4, new RoomInfo { Name = "Laboratoire", Description = "Un laboratoire rempli d'équipements étranges. Une formule est écrite au tableau.", RequiredItems = new List<string>(), Puzzles = new List<int> { 8, 9 } } },
        { 5, new RoomInfo { Name = "Salle du Trésor", Description = "Vous avez trouvé la salle du trésor ! Mais elle est protégée par une dernière énigme.", RequiredItems = new List<string> { "map", "torch" }, Puzzles = new List<int> { 10, 11, 12 } } },
        { 6, new RoomInfo { Name = "Salle Secrète", Description = "Une salle cachée derrière un mur de livres. Des énigmes complexes vous attendent.", RequiredItems = new List<string> { "book" }, Puzzles = new List<int> { 13, 14 } } },
        { 7, new RoomInfo { Name = "Tour du Mages", Description = "Une tour mystérieuse remplie de magie. Les énigmes ici sont les plus difficiles.", RequiredItems = new List<string> { "crystal" }, Puzzles = new List<int> { 15 } } }
    };

    private static readonly Dictionary<int, PuzzleInfo> Puzzles = new()
    {
        // Salle 1 - Entrée
        { 1, new PuzzleInfo { 
            Question = "Mini-jeu : Mémoire", 
            Answer = "memory", 
            Points = 15,
            MiniGameType = "memory",
            Hints = new List<string> { "Mémorisez l'ordre des cartes", "Les cartes se retournent rapidement", "Concentrez-vous sur les couleurs" }
        }},
        { 2, new PuzzleInfo { 
            Question = "Quel est le résultat de 2 + 2 ?", 
            Answer = "4", 
            Points = 10,
            Hints = new List<string> { "C'est un nombre simple", "Pensez aux bases des mathématiques", "Le résultat est inférieur à 10" }
        }},
        
        // Salle 2 - Hall Principal
        { 3, new PuzzleInfo { 
            Question = "Mini-jeu : Séquence", 
            Answer = "sequence", 
            Points = 20,
            MiniGameType = "sequence",
            Hints = new List<string> { "Suivez l'ordre des couleurs", "Répétez la séquence exactement", "La séquence s'allonge à chaque tour" }
        }},
        { 4, new PuzzleInfo { 
            Question = "Quelle est la capitale de la France ?", 
            Answer = "paris", 
            Points = 15,
            Hints = new List<string> { "C'est une ville européenne", "Elle est connue pour la Tour Eiffel", "Elle commence par la lettre P" }
        }},
        
        // Salle 3 - Bibliothèque
        { 5, new PuzzleInfo { 
            Question = "Mini-jeu : Puzzle", 
            Answer = "puzzle", 
            Points = 25,
            MiniGameType = "puzzle",
            Hints = new List<string> { "Assemblez les pièces", "Regardez les bords", "Commencez par les coins" }
        }},
        { 6, new PuzzleInfo { 
            Question = "Combien de lettres dans le mot 'AVENTURE' ?", 
            Answer = "8", 
            Points = 20,
            Hints = new List<string> { "Comptez chaque lettre", "A-V-E-N-T-U-R-E", "C'est un nombre pair" }
        }},
        { 7, new PuzzleInfo { 
            Question = "Mini-jeu : Correspondance", 
            Answer = "match", 
            Points = 30,
            MiniGameType = "match",
            Hints = new List<string> { "Trouvez les paires", "Les symboles correspondent", "Faites attention aux détails" }
        }},
        
        // Salle 4 - Laboratoire
        { 8, new PuzzleInfo { 
            Question = "Quel est le symbole chimique de l'eau ?", 
            Answer = "h2o", 
            Points = 25,
            Hints = new List<string> { "Deux éléments chimiques", "H pour hydrogène", "O pour oxygène" }
        }},
        { 9, new PuzzleInfo { 
            Question = "Mini-jeu : Logique", 
            Answer = "logic", 
            Points = 35,
            MiniGameType = "logic",
            Hints = new List<string> { "Suivez la logique", "Chaque symbole a une règle", "Observez les patterns" }
        }},
        
        // Salle 5 - Salle du Trésor
        { 10, new PuzzleInfo { 
            Question = "Quel est le nombre premier après 7 ?", 
            Answer = "11", 
            Points = 50,
            Hints = new List<string> { "Un nombre premier n'est divisible que par 1 et lui-même", "C'est entre 8 et 12", "C'est un nombre impair" }
        }},
        { 11, new PuzzleInfo { 
            Question = "Mini-jeu : Code", 
            Answer = "code", 
            Points = 60,
            MiniGameType = "code",
            Hints = new List<string> { "Déchiffrez le code", "Les couleurs ont une signification", "Suivez l'ordre des indices" }
        }},
        { 12, new PuzzleInfo { 
            Question = "Mini-jeu : Réflexe", 
            Answer = "reflex", 
            Points = 55,
            MiniGameType = "reflex",
            Hints = new List<string> { "Soyez rapide", "Cliquez au bon moment", "La vitesse est importante" }
        }},
        
        // Salle 6 - Salle Secrète
        { 13, new PuzzleInfo { 
            Question = "Mini-jeu : Mastermind", 
            Answer = "mastermind", 
            Points = 70,
            MiniGameType = "mastermind",
            Hints = new List<string> { "Trouvez la combinaison secrète", "Utilisez les indices de couleur", "Éliminez les possibilités" }
        }},
        { 14, new PuzzleInfo { 
            Question = "Quelle est la racine carrée de 144 ?", 
            Answer = "12", 
            Points = 40,
            Hints = new List<string> { "12 × 12 = ?", "C'est un nombre pair", "Entre 10 et 15" }
        }},
        
        // Salle 7 - Tour du Mages
        { 15, new PuzzleInfo { 
            Question = "Mini-jeu : Final Boss", 
            Answer = "boss", 
            Points = 100,
            MiniGameType = "boss",
            Hints = new List<string> { "Le défi ultime", "Combinez toutes vos compétences", "Concentrez-vous bien" }
        }}
    };

    private static readonly Dictionary<string, ItemInfo> Items = new()
    {
        { "key", new ItemInfo { Name = "Clé", Description = "Une clé ancienne en or", Room = 2 } },
        { "map", new ItemInfo { Name = "Carte", Description = "Une carte du château", Room = 3 } },
        { "torch", new ItemInfo { Name = "Torche", Description = "Une torche pour éclairer", Room = 4 } },
        { "book", new ItemInfo { Name = "Livre", Description = "Un livre mystérieux", Room = 3 } },
        { "crystal", new ItemInfo { Name = "Cristal", Description = "Un cristal magique brillant", Room = 6 } }
    };

    public AdventureService(
        HashiDbContext context,
        ISessionRepository sessionRepository,
        ILogger<AdventureService>? logger = null)
    {
        _context = context;
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    public async Task<AdventureGame> CreateGameAsync(int sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null || !session.IsValid())
        {
            throw new ArgumentException($"La session avec l'ID {sessionId} n'existe pas ou n'est pas valide", nameof(sessionId));
        }

        var game = new AdventureGame
        {
            PlayerSessionId = sessionId,
            CurrentRoom = 1,
            CollectedItemsJson = "[]",
            SolvedPuzzlesJson = "[]",
            Score = 0,
            Status = AdventureGameStatus.InProgress,
            CreatedAt = DateTime.UtcNow,
            StartedAt = DateTime.UtcNow
        };

        _context.AdventureGames.Add(game);
        await _context.SaveChangesAsync();

        _logger?.LogInformation("Partie d'aventure créée : GameId={GameId}, PlayerSessionId={SessionId}", game.Id, sessionId);

        return game;
    }

    public async Task<AdventureGame?> GetGameByIdAsync(int gameId)
    {
        return await _context.AdventureGames
            .Include(g => g.PlayerSession)
                .ThenInclude(s => s!.User)
            .FirstOrDefaultAsync(g => g.Id == gameId);
    }

    public async Task<IEnumerable<AdventureGame>> GetGamesBySessionAsync(int sessionId)
    {
        return await _context.AdventureGames
            .Include(g => g.PlayerSession)
                .ThenInclude(s => s!.User)
            .Where(g => g.PlayerSessionId == sessionId)
            .OrderByDescending(g => g.CreatedAt)
            .ToListAsync();
    }

    public async Task<AdventureGame> MoveToRoomAsync(int gameId, int sessionId, int roomNumber)
    {
        if (!Rooms.ContainsKey(roomNumber))
        {
            throw new ArgumentException($"La salle {roomNumber} n'existe pas", nameof(roomNumber));
        }

        var game = await GetGameByIdAsync(gameId);
        if (game == null)
        {
            throw new ArgumentException($"La partie avec l'ID {gameId} n'existe pas", nameof(gameId));
        }

        if (game.PlayerSessionId != sessionId)
        {
            throw new UnauthorizedAccessException("Vous n'êtes pas le joueur de cette partie");
        }

        if (game.Status != AdventureGameStatus.InProgress)
        {
            throw new InvalidOperationException("La partie n'est pas en cours");
        }

        var room = Rooms[roomNumber];
        var collectedItems = JsonSerializer.Deserialize<List<string>>(game.CollectedItemsJson) ?? new List<string>();

        // Vérifier si le joueur a les objets requis
        foreach (var requiredItem in room.RequiredItems)
        {
            if (!collectedItems.Contains(requiredItem))
            {
                throw new InvalidOperationException($"Vous avez besoin de l'objet '{Items[requiredItem].Name}' pour entrer dans cette salle");
            }
        }

        game.CurrentRoom = roomNumber;
        game.ElapsedSeconds = (int)(DateTime.UtcNow - (game.StartedAt ?? game.CreatedAt)).TotalSeconds;

        await _context.SaveChangesAsync();

        _logger?.LogInformation("Joueur déplacé : GameId={GameId}, Room={Room}", gameId, roomNumber);

        return game;
    }

    public async Task<AdventureGame> CollectItemAsync(int gameId, int sessionId, string itemName)
    {
        if (!Items.ContainsKey(itemName))
        {
            throw new ArgumentException($"L'objet '{itemName}' n'existe pas", nameof(itemName));
        }

        var game = await GetGameByIdAsync(gameId);
        if (game == null)
        {
            throw new ArgumentException($"La partie avec l'ID {gameId} n'existe pas", nameof(gameId));
        }

        if (game.PlayerSessionId != sessionId)
        {
            throw new UnauthorizedAccessException("Vous n'êtes pas le joueur de cette partie");
        }

        if (game.Status != AdventureGameStatus.InProgress)
        {
            throw new InvalidOperationException("La partie n'est pas en cours");
        }

        var item = Items[itemName];
        if (item.Room != game.CurrentRoom)
        {
            throw new InvalidOperationException($"Cet objet n'est pas disponible dans cette salle");
        }

        var collectedItems = JsonSerializer.Deserialize<List<string>>(game.CollectedItemsJson) ?? new List<string>();
        
        if (collectedItems.Contains(itemName))
        {
            throw new InvalidOperationException("Vous avez déjà collecté cet objet");
        }

        collectedItems.Add(itemName);
        game.CollectedItemsJson = JsonSerializer.Serialize(collectedItems);
        game.Score += 5; // Points pour collecter un objet
        game.ElapsedSeconds = (int)(DateTime.UtcNow - (game.StartedAt ?? game.CreatedAt)).TotalSeconds;

        await _context.SaveChangesAsync();

        _logger?.LogInformation("Objet collecté : GameId={GameId}, Item={Item}", gameId, itemName);

        return game;
    }

    public async Task<AdventureGame> SolvePuzzleAsync(int gameId, int sessionId, int puzzleId, string answer)
    {
        if (!Puzzles.ContainsKey(puzzleId))
        {
            throw new ArgumentException($"L'énigme {puzzleId} n'existe pas", nameof(puzzleId));
        }

        var game = await GetGameByIdAsync(gameId);
        if (game == null)
        {
            throw new ArgumentException($"La partie avec l'ID {gameId} n'existe pas", nameof(gameId));
        }

        if (game.PlayerSessionId != sessionId)
        {
            throw new UnauthorizedAccessException("Vous n'êtes pas le joueur de cette partie");
        }

        if (game.Status != AdventureGameStatus.InProgress)
        {
            throw new InvalidOperationException("La partie n'est pas en cours");
        }

        var puzzle = Puzzles[puzzleId];
        var solvedPuzzles = JsonSerializer.Deserialize<List<int>>(game.SolvedPuzzlesJson) ?? new List<int>();

        if (solvedPuzzles.Contains(puzzleId))
        {
            throw new InvalidOperationException("Vous avez déjà résolu cette énigme");
        }

        // Vérifier si l'énigme est disponible dans la salle actuelle
        if (!Rooms.ContainsKey(game.CurrentRoom) || !Rooms[game.CurrentRoom].Puzzles.Contains(puzzleId))
        {
            throw new InvalidOperationException("Cette énigme n'est pas disponible dans cette salle");
        }

        // Vérifier la réponse (insensible à la casse)
        bool isCorrect = string.Equals(puzzle.Answer, answer.Trim(), StringComparison.OrdinalIgnoreCase);

        if (isCorrect)
        {
            solvedPuzzles.Add(puzzleId);
            game.SolvedPuzzlesJson = JsonSerializer.Serialize(solvedPuzzles);
            game.Score += puzzle.Points;
            game.PuzzlesSolved++;
            game.ElapsedSeconds = (int)(DateTime.UtcNow - (game.StartedAt ?? game.CreatedAt)).TotalSeconds;

            // Vérifier si toutes les énigmes sont résolues (victoire) - au moins 12 énigmes sur 15
            if (solvedPuzzles.Count >= 12)
            {
                game.Status = AdventureGameStatus.Completed;
                game.CompletedAt = DateTime.UtcNow;
            }
        }
        else
        {
            throw new InvalidOperationException("Mauvaise réponse ! Essayez encore.");
        }

        await _context.SaveChangesAsync();

        _logger?.LogInformation("Énigme résolue : GameId={GameId}, PuzzleId={PuzzleId}, Correct={Correct}", gameId, puzzleId, isCorrect);

        return game;
    }

    public async Task<AdventureGame> AbandonGameAsync(int gameId, int sessionId)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null)
        {
            throw new ArgumentException($"La partie avec l'ID {gameId} n'existe pas", nameof(gameId));
        }

        if (game.PlayerSessionId != sessionId)
        {
            throw new UnauthorizedAccessException("Vous n'êtes pas le joueur de cette partie");
        }

        game.Status = AdventureGameStatus.Abandoned;
        game.CompletedAt = DateTime.UtcNow;
        game.ElapsedSeconds = (int)(DateTime.UtcNow - (game.StartedAt ?? game.CreatedAt)).TotalSeconds;

        await _context.SaveChangesAsync();

        _logger?.LogInformation("Partie abandonnée : GameId={GameId}, PlayerSessionId={SessionId}", gameId, sessionId);

        return game;
    }

    public async Task<PuzzleInfoDto> GetPuzzleInfoAsync(int puzzleId)
    {
        if (!Puzzles.ContainsKey(puzzleId))
        {
            throw new ArgumentException($"L'énigme {puzzleId} n'existe pas", nameof(puzzleId));
        }

        var puzzle = Puzzles[puzzleId];
        return new PuzzleInfoDto
        {
            Id = puzzleId,
            Question = puzzle.Question,
            MiniGameType = puzzle.MiniGameType,
            Hints = puzzle.Hints,
            Points = puzzle.Points
        };
    }

    public AdventureGameDto ConvertToDto(AdventureGame game)
    {
        return new AdventureGameDto
        {
            Id = game.Id,
            PlayerSessionId = game.PlayerSessionId,
            PlayerName = game.PlayerSession?.User?.Name,
            CurrentRoom = game.CurrentRoom,
            CollectedItems = JsonSerializer.Deserialize<List<string>>(game.CollectedItemsJson) ?? new List<string>(),
            SolvedPuzzles = JsonSerializer.Deserialize<List<int>>(game.SolvedPuzzlesJson) ?? new List<int>(),
            Score = game.Score,
            ElapsedSeconds = game.ElapsedSeconds,
            PuzzlesSolved = game.PuzzlesSolved,
            Status = (int)game.Status,
            CreatedAt = game.CreatedAt,
            StartedAt = game.StartedAt,
            CompletedAt = game.CompletedAt
        };
    }

    // Classes internes pour la configuration
    private class RoomInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<string> RequiredItems { get; set; } = new();
        public List<int> Puzzles { get; set; } = new();
    }

    private class PuzzleInfo
    {
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public int Points { get; set; }
        public string? MiniGameType { get; set; }
        public List<string> Hints { get; set; } = new();
    }

    private class ItemInfo
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Room { get; set; }
    }
}

