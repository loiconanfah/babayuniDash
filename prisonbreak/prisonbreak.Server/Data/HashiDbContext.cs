using Microsoft.EntityFrameworkCore;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Data
{
    /// <summary>
    /// Contexte Entity Framework Core pour le jeu Hashi / Prison Break.
    /// - Regroupe toutes les entités persistées en base.
    /// - Applique les contraintes décrites dans le PDF et l'ARCHITECTURE_BACKEND :
    ///   * User.Email unique et requis
    ///   * Session.SessionToken unique et requis
    ///   * Game lié obligatoirement à une Session et à un Puzzle
    ///   * Relations 1-N entre User → Sessions, Session → Games, Puzzle → Islands/Bridges.
    /// </summary>
    public class HashiDbContext : DbContext
    {
        /// <summary>
        /// Initialise une nouvelle instance du contexte HashiDbContext.
        /// DbContextOptions est injecté par ASP.NET Core via Program.cs.
        /// </summary>
        public HashiDbContext(DbContextOptions<HashiDbContext> options)
            : base(options)
        {
        }

        // ============================
        // DbSet = tables principales
        // ============================

        /// <summary>
        /// Table des utilisateurs (joueurs).
        /// Correspond à la gestion du joueur : Nom + Email + métadonnées.
        /// </summary>
        public DbSet<User> Users => Set<User>();

        /// <summary>
        /// Table des sessions de jeu (token, expiration, lien utilisateur).
        /// </summary>
        public DbSet<Session> Sessions => Set<Session>();

        /// <summary>
        /// Table des parties (Game) jouées par les joueurs.
        /// Utilisée plus tard pour les statistiques et le leaderboard.
        /// </summary>
        public DbSet<Game> Games => Set<Game>();

        /// <summary>
        /// Table des puzzles Hashi (grilles générées).
        /// </summary>
        public DbSet<Puzzle> Puzzles => Set<Puzzle>();

        /// <summary>
        /// Table des îles d’un puzzle (Island).
        /// </summary>
        public DbSet<Island> Islands => Set<Island>();

        /// <summary>
        /// Table des ponts de solution d'un puzzle (Bridge).
        /// </summary>
        public DbSet<Bridge> Bridges => Set<Bridge>();

        /// <summary>
        /// Table des parties de Tic-Tac-Toe (Morpion).
        /// </summary>
        public DbSet<TicTacToeGame> TicTacToeGames => Set<TicTacToeGame>();

        /// <summary>
        /// Table des parties de Connect Four (Puissance 4).
        /// </summary>
        public DbSet<ConnectFourGame> ConnectFourGames => Set<ConnectFourGame>();

        /// <summary>
        /// Table des parties de Rock Paper Scissors (Pierre-Papier-Ciseaux).
        /// </summary>
        public DbSet<RockPaperScissorsGame> RockPaperScissorsGames => Set<RockPaperScissorsGame>();

        /// <summary>
        /// Table des parties d'aventure (jeu d'énigmes et d'exploration).
        /// </summary>
        public DbSet<AdventureGame> AdventureGames => Set<AdventureGame>();

        /// <summary>
        /// Table des items disponibles dans la boutique.
        /// </summary>
        public DbSet<Item> Items => Set<Item>();

        /// <summary>
        /// Table de jonction pour les items possédés par les utilisateurs.
        /// </summary>
        public DbSet<UserItem> UserItems => Set<UserItem>();

        /// <summary>
        /// Table des relations d'amitié entre utilisateurs.
        /// </summary>
        public DbSet<Friendship> Friendships => Set<Friendship>();

        /// <summary>
        /// Table des demandes d'amitié.
        /// </summary>
        public DbSet<FriendRequest> FriendRequests => Set<FriendRequest>();

        /// <summary>
        /// Table des messages de chat.
        /// </summary>
        public DbSet<ChatMessage> ChatMessages => Set<ChatMessage>();

        /// <summary>
        /// Table des posts de la communauté.
        /// </summary>
        public DbSet<CommunityPost> CommunityPosts => Set<CommunityPost>();

        /// <summary>
        /// Table des likes sur les posts de la communauté.
        /// </summary>
        public DbSet<CommunityPostLike> CommunityPostLikes => Set<CommunityPostLike>();

        /// <summary>
        /// Table des commentaires sur les posts de la communauté.
        /// </summary>
        public DbSet<CommunityPostComment> CommunityPostComments => Set<CommunityPostComment>();

        /// <summary>
        /// Configuration fine du modèle : contraintes, index, relations.
        /// Tout ce qui touche à la structure SQL est centralisé ici.
        /// </summary>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            ConfigureUser(modelBuilder);
            ConfigureSession(modelBuilder);
            ConfigureGame(modelBuilder);
            ConfigurePuzzle(modelBuilder);
            ConfigureIsland(modelBuilder);
            ConfigureBridge(modelBuilder);
            ConfigureTicTacToeGame(modelBuilder);
            ConfigureConnectFourGame(modelBuilder);
            ConfigureRockPaperScissorsGame(modelBuilder);
            ConfigureAdventureGame(modelBuilder);
            ConfigureItem(modelBuilder);
            ConfigureUserItem(modelBuilder);
            ConfigureFriendship(modelBuilder);
            ConfigureFriendRequest(modelBuilder);
            ConfigureChatMessage(modelBuilder);
            ConfigureCommunityPost(modelBuilder);
            ConfigureCommunityPostLike(modelBuilder);
            ConfigureCommunityPostComment(modelBuilder);
        }

        // ============================
        // Configuration User
        // ============================

        /// <summary>
        /// Configure la table Users :
        /// - Email unique et requis (comme dans le PDF : joueur identifié par pseudo/email)
        /// - Nom avec taille limitée (pseudo max ~50 caractères)
        /// - Valeurs par défaut CreatedAt et IsActive.
        /// </summary>
        private static void ConfigureUser(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<User>();

            entity.ToTable("Users");

            // Clé primaire
            entity.HasKey(u => u.Id);

            // Pseudo / Nom du joueur
            entity.Property(u => u.Name)
                  .IsRequired()
                  .HasMaxLength(50); // cohérent avec un pseudo court

            // Email du joueur (identifiant unique)
            entity.Property(u => u.Email)
                  .IsRequired()
                  .HasMaxLength(255); // 255 = standard pour les emails

            // Index unique sur l'email (une seule entrée par email)
            entity.HasIndex(u => u.Email)
                  .IsUnique();

            // Dates & état
            entity.Property(u => u.CreatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP"); // SQLite : date de création auto

            entity.Property(u => u.IsActive)
                  .HasDefaultValue(true);

            // Coins par défaut
            entity.Property(u => u.Coins)
                  .HasDefaultValue(500);

            // Relation logique User (1) -> (N) Sessions
            // Note: La collection Sessions n'existe pas dans User, mais la relation est gérée via UserId dans Session
            // On configure la relation depuis Session vers User
            // Cette configuration sera complétée dans ConfigureSession
        }

        // ============================
        // Configuration Session
        // ============================

        /// <summary>
        /// Configure la table Sessions :
        /// - Token unique et requis
        /// - Date d'expiration obligatoire
        /// - Lien obligatoire vers User (UserId).
        /// </summary>
        private static void ConfigureSession(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Session>();

            entity.ToTable("Sessions");

            entity.HasKey(s => s.Id);

            // Token de session (GUID + timestamp)
            entity.Property(s => s.SessionToken)
                  .IsRequired()
                  .HasMaxLength(128);

            // Index unique sur le token
            entity.HasIndex(s => s.SessionToken)
                  .IsUnique();

            // Session active par défaut
            entity.Property(s => s.IsActive)
                  .HasDefaultValue(true);

            // Date d'expiration obligatoire
            entity.Property(s => s.ExpiresAt)
                  .IsRequired();

            // Index sur UserId pour accélérer les requêtes par utilisateur
            entity.HasIndex(s => s.UserId);

            // Relation Session -> User (N -> 1)
            entity.HasOne(s => s.User)
                  .WithMany()
                  .HasForeignKey(s => s.UserId)
                  .OnDelete(DeleteBehavior.Cascade); // suppression d'un user = supprime ses sessions + games
        }

        // ============================
        // Configuration Game (Parties)
        // ============================

        /// <summary>
        /// Configure la table Games :
        /// - Lien obligatoire vers Session (SessionId) et Puzzle (PuzzleId)
        /// - Score avec valeur par défaut 0
        /// - Statut requis (InProgress, Completed, etc.)
        /// - Index sur SessionId pour les stats / leaderboard par joueur.
        /// </summary>
        private static void ConfigureGame(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Game>();

            entity.ToTable("Games");

            entity.HasKey(g => g.Id);

            // Statut de la partie (en cours, terminée, etc.)
            entity.Property(g => g.Status)
                  .IsRequired();

            // Score de la partie (utilisé pour le leaderboard)
            entity.Property(g => g.Score)
                  .HasDefaultValue(0);

            // Date de début : par défaut maintenant
            entity.Property(g => g.StartedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Index pour les requêtes par SessionId (stats par session/joueur)
            entity.HasIndex(g => g.SessionId);

            // Relation Game -> Session (N -> 1)
            entity.HasOne(g => g.Session)
                  .WithMany(s => s.Games)
                  .HasForeignKey(g => g.SessionId)
                  .OnDelete(DeleteBehavior.Cascade); // cohérent avec "supprimer User supprime Sessions + Games"

            // Relation Game -> Puzzle (N -> 1)
            entity.HasOne(g => g.Puzzle)
                  .WithMany(p => p.Games)
                  .HasForeignKey(g => g.PuzzleId)
                  .OnDelete(DeleteBehavior.Restrict); // on évite de supprimer un puzzle utilisé par des games
        }

        // ============================
        // Configuration Puzzle
        // ============================

        /// <summary>
        /// Configure la table Puzzles :
        /// - Dimensions obligatoires (width/height)
        /// - Difficulté requise (pour le leaderboard : colonne « Difficulté »)
        /// - Index sur Difficulty pour filtrer facilement par niveau.
        /// </summary>
        private static void ConfigurePuzzle(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Puzzle>();

            entity.ToTable("Puzzles");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Name)
                  .HasMaxLength(100); // nom de la grille (optionnel mais limité)

            entity.Property(p => p.Width)
                  .IsRequired();

            entity.Property(p => p.Height)
                  .IsRequired();

            // Difficulté (Easy/Medium/Hard/Expert)
            entity.Property(p => p.Difficulty)
                  .IsRequired();

            // Thème du puzzle
            entity.Property(p => p.Theme)
                  .IsRequired()
                  .HasDefaultValue(PuzzleTheme.Classic);

            // Index pour accélérer les requêtes par difficulté
            entity.HasIndex(p => p.Difficulty);
            
            // Index pour accélérer les requêtes par thème
            entity.HasIndex(p => p.Theme);

            // Relation Puzzle -> Islands (1 -> N) - configurée dans ConfigureIsland
        }

        // ============================
        // Configuration Island
        // ============================

        /// <summary>
        /// Configure la table Islands :
        /// - Position (X, Y) et RequiredBridges obligatoires
        /// - Lien obligatoire vers Puzzle (PuzzleId).
        /// </summary>
        private static void ConfigureIsland(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Island>();

            entity.ToTable("Islands");

            entity.HasKey(i => i.Id);

            entity.Property(i => i.X)
                  .IsRequired();

            entity.Property(i => i.Y)
                  .IsRequired();

            entity.Property(i => i.RequiredBridges)
                  .IsRequired();

            // Index sur PuzzleId pour retrouver toutes les îles d'une grille
            entity.HasIndex(i => i.PuzzleId);

            // Relation Island -> Puzzle (N -> 1)
            entity.HasOne(i => i.Puzzle)
                  .WithMany(p => p.Islands)
                  .HasForeignKey(i => i.PuzzleId)
                  .OnDelete(DeleteBehavior.Cascade);
        }

        // ============================
        // Configuration Bridge
        // ============================

        /// <summary>
        /// Configure la table Bridges :
        /// - Lien vers Puzzle (PuzzleId) pour la solution
        /// - Indicateur IsDouble requis (pont simple/double).
        /// </summary>
        private static void ConfigureBridge(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Bridge>();

            entity.ToTable("Bridges");

            // Clé primaire
            entity.HasKey(b => b.Id);

            // Pont simple ou double
            entity.Property(b => b.IsDouble)
                  .IsRequired();

            // Direction (Horizontal / Vertical)
            entity.Property(b => b.Direction)
                  .IsRequired();

            // Index utiles pour les requêtes
            entity.HasIndex(b => b.PuzzleId);
            entity.HasIndex(b => b.FromIslandId);
            entity.HasIndex(b => b.ToIslandId);

            // ============================
            // Relations
            // ============================

            // Bridge -> Puzzle (N ponts pour 1 puzzle)
            entity.HasOne(b => b.Puzzle)
                  .WithMany(p => p.SolutionBridges)
                  .HasForeignKey(b => b.PuzzleId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Bridge -> Island (île de départ)
            entity.HasOne(b => b.FromIsland)
                  .WithMany(i => i.BridgesFrom)
                  .HasForeignKey(b => b.FromIslandId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Bridge -> Island (île d'arrivée)
            entity.HasOne(b => b.ToIsland)
                  .WithMany(i => i.BridgesTo)
                  .HasForeignKey(b => b.ToIslandId)
                  .OnDelete(DeleteBehavior.Restrict);
        }

        // ============================
        // Configuration TicTacToeGame
        // ============================

        /// <summary>
        /// Configure la table TicTacToeGames :
        /// - Lien obligatoire vers Session pour le joueur 1
        /// - Lien optionnel vers Session pour le joueur 2
        /// - Statut requis
        /// - Index sur les sessions pour les requêtes
        /// </summary>
        private static void ConfigureTicTacToeGame(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<TicTacToeGame>();

            entity.ToTable("TicTacToeGames");

            entity.HasKey(g => g.Id);

            // Statut de la partie
            entity.Property(g => g.Status)
                  .IsRequired();

            // Grille sérialisée en JSON
            entity.Property(g => g.BoardJson)
                  .IsRequired()
                  .HasDefaultValue("[\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\",\"\"]");

            // Joueur actuel
            entity.Property(g => g.CurrentPlayer)
                  .IsRequired()
                  .HasDefaultValue(1);

            // Mode de jeu
            entity.Property(g => g.GameMode)
                  .IsRequired()
                  .HasDefaultValue(TicTacToeGameMode.Player);

            // Date de création
            entity.Property(g => g.CreatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Index pour les requêtes
            entity.HasIndex(g => g.Player1SessionId);
            entity.HasIndex(g => g.Player2SessionId);
            entity.HasIndex(g => g.Status);

            // Relation TicTacToeGame -> Session (Joueur 1)
            entity.HasOne(g => g.Player1Session)
                  .WithMany()
                  .HasForeignKey(g => g.Player1SessionId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Relation TicTacToeGame -> Session (Joueur 2)
            entity.HasOne(g => g.Player2Session)
                  .WithMany()
                  .HasForeignKey(g => g.Player2SessionId)
                  .OnDelete(DeleteBehavior.SetNull);
        }

        // ============================
        // Configuration ConnectFourGame
        // ============================

        /// <summary>
        /// Configure la table ConnectFourGames :
        /// - Lien obligatoire vers Session pour le joueur 1
        /// - Lien optionnel vers Session pour le joueur 2
        /// - Statut requis
        /// - Index sur les sessions pour les requêtes
        /// </summary>
        private static void ConfigureConnectFourGame(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<ConnectFourGame>();

            entity.ToTable("ConnectFourGames");

            entity.HasKey(g => g.Id);

            // Statut de la partie
            entity.Property(g => g.Status)
                  .IsRequired();

            // Grille sérialisée en JSON (7 colonnes x 6 lignes)
            entity.Property(g => g.BoardJson)
                  .IsRequired()
                  .HasDefaultValue("[[0,0,0,0,0,0],[0,0,0,0,0,0],[0,0,0,0,0,0],[0,0,0,0,0,0],[0,0,0,0,0,0],[0,0,0,0,0,0],[0,0,0,0,0,0]]");

            // Joueur actuel
            entity.Property(g => g.CurrentPlayer)
                  .IsRequired()
                  .HasDefaultValue(1);

            // Mode de jeu
            entity.Property(g => g.GameMode)
                  .IsRequired()
                  .HasDefaultValue(ConnectFourGameMode.Player);

            // Date de création
            entity.Property(g => g.CreatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // Index pour les requêtes
            entity.HasIndex(g => g.Player1SessionId);
            entity.HasIndex(g => g.Player2SessionId);
            entity.HasIndex(g => g.Status);

            // Relation ConnectFourGame -> Session (Joueur 1)
            entity.HasOne(g => g.Player1Session)
                  .WithMany()
                  .HasForeignKey(g => g.Player1SessionId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Relation ConnectFourGame -> Session (Joueur 2)
            entity.HasOne(g => g.Player2Session)
                  .WithMany()
                  .HasForeignKey(g => g.Player2SessionId)
                  .OnDelete(DeleteBehavior.SetNull);
        }

        // ============================
        // Configuration RockPaperScissorsGame
        // ============================

        /// <summary>
        /// Configure la table RockPaperScissorsGames
        /// </summary>
        private static void ConfigureRockPaperScissorsGame(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<RockPaperScissorsGame>();

            entity.ToTable("RockPaperScissorsGames");

            entity.HasKey(g => g.Id);

            entity.Property(g => g.Status).IsRequired();
            entity.Property(g => g.GameMode).IsRequired().HasDefaultValue(RPSGameMode.Player);
            entity.Property(g => g.RoundNumber).IsRequired().HasDefaultValue(1);
            entity.Property(g => g.Player1Score).IsRequired().HasDefaultValue(0);
            entity.Property(g => g.Player2Score).IsRequired().HasDefaultValue(0);
            entity.Property(g => g.RoundsToWin).IsRequired().HasDefaultValue(3);
            entity.Property(g => g.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(g => g.Player1SessionId);
            entity.HasIndex(g => g.Player2SessionId);
            entity.HasIndex(g => g.Status);

            entity.HasOne(g => g.Player1Session)
                  .WithMany()
                  .HasForeignKey(g => g.Player1SessionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(g => g.Player2Session)
                  .WithMany()
                  .HasForeignKey(g => g.Player2SessionId)
                  .OnDelete(DeleteBehavior.SetNull);
        }

        // ============================
        // Configuration AdventureGame
        // ============================

        /// <summary>
        /// Configure la table AdventureGames
        /// </summary>
        private static void ConfigureAdventureGame(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<AdventureGame>();

            entity.ToTable("AdventureGames");

            entity.HasKey(g => g.Id);

            entity.Property(g => g.Status).IsRequired();
            entity.Property(g => g.CurrentRoom).IsRequired().HasDefaultValue(1);
            entity.Property(g => g.CollectedItemsJson).IsRequired().HasDefaultValue("[]");
            entity.Property(g => g.SolvedPuzzlesJson).IsRequired().HasDefaultValue("[]");
            entity.Property(g => g.Score).IsRequired().HasDefaultValue(0);
            entity.Property(g => g.PuzzlesSolved).IsRequired().HasDefaultValue(0);
            entity.Property(g => g.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(g => g.PlayerSessionId);
            entity.HasIndex(g => g.Status);

            entity.HasOne(g => g.PlayerSession)
                  .WithMany()
                  .HasForeignKey(g => g.PlayerSessionId)
                  .OnDelete(DeleteBehavior.Cascade);
        }

        // ============================
        // Configuration Item
        // ============================

        /// <summary>
        /// Configure la table Items
        /// </summary>
        private static void ConfigureItem(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Item>();

            entity.ToTable("Items");

            entity.HasKey(i => i.Id);

            entity.Property(i => i.Name)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(i => i.Description)
                  .HasMaxLength(500);

            entity.Property(i => i.Price)
                  .IsRequired();

            entity.Property(i => i.ItemType)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.Property(i => i.Rarity)
                  .HasMaxLength(20)
                  .HasDefaultValue("Common");

            entity.Property(i => i.IsAvailable)
                  .HasDefaultValue(true);

            entity.Property(i => i.CreatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(i => i.ItemType);
            entity.HasIndex(i => i.Rarity);
        }

        // ============================
        // Configuration UserItem
        // ============================

        /// <summary>
        /// Configure la table UserItems (relation plusieurs-à-plusieurs)
        /// </summary>
        private static void ConfigureUserItem(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<UserItem>();

            entity.ToTable("UserItems");

            entity.HasKey(ui => ui.Id);

            entity.Property(ui => ui.PurchasedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(ui => ui.IsEquipped)
                  .HasDefaultValue(false);

            // Index pour les requêtes
            entity.HasIndex(ui => ui.UserId);
            entity.HasIndex(ui => ui.ItemId);
            entity.HasIndex(ui => new { ui.UserId, ui.ItemId }).IsUnique(); // Un utilisateur ne peut avoir qu'un seul exemplaire d'un item

            // Relation UserItem -> User
            entity.HasOne(ui => ui.User)
                  .WithMany(u => u.UserItems)
                  .HasForeignKey(ui => ui.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Relation UserItem -> Item
            entity.HasOne(ui => ui.Item)
                  .WithMany(i => i.UserItems)
                  .HasForeignKey(ui => ui.ItemId)
                  .OnDelete(DeleteBehavior.Restrict);
        }

        // ============================
        // Configuration Friendship
        // ============================

        private static void ConfigureFriendship(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<Friendship>();

            entity.ToTable("Friendships");

            entity.HasKey(f => f.Id);

            entity.HasIndex(f => new { f.UserId, f.FriendId }).IsUnique();
            entity.HasIndex(f => f.UserId);
            entity.HasIndex(f => f.FriendId);

            entity.HasOne(f => f.User)
                  .WithMany()
                  .HasForeignKey(f => f.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(f => f.Friend)
                  .WithMany()
                  .HasForeignKey(f => f.FriendId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(f => f.CreatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(f => f.Status)
                  .HasDefaultValue(FriendshipStatus.Active);
        }

        // ============================
        // Configuration FriendRequest
        // ============================

        private static void ConfigureFriendRequest(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<FriendRequest>();

            entity.ToTable("FriendRequests");

            entity.HasKey(fr => fr.Id);

            entity.HasIndex(fr => new { fr.RequesterId, fr.ReceiverId }).IsUnique();
            entity.HasIndex(fr => fr.RequesterId);
            entity.HasIndex(fr => fr.ReceiverId);
            entity.HasIndex(fr => fr.Status);

            entity.HasOne(fr => fr.Requester)
                  .WithMany()
                  .HasForeignKey(fr => fr.RequesterId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(fr => fr.Receiver)
                  .WithMany()
                  .HasForeignKey(fr => fr.ReceiverId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(fr => fr.CreatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(fr => fr.Status)
                  .HasDefaultValue(FriendRequestStatus.Pending);
        }

        // ============================
        // Configuration ChatMessage
        // ============================

        private static void ConfigureChatMessage(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<ChatMessage>();

            entity.ToTable("ChatMessages");

            entity.HasKey(cm => cm.Id);

            entity.HasIndex(cm => new { cm.SenderId, cm.ReceiverId });
            entity.HasIndex(cm => cm.SenderId);
            entity.HasIndex(cm => cm.ReceiverId);
            entity.HasIndex(cm => cm.SentAt);

            entity.HasOne(cm => cm.Sender)
                  .WithMany()
                  .HasForeignKey(cm => cm.SenderId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cm => cm.Receiver)
                  .WithMany()
                  .HasForeignKey(cm => cm.ReceiverId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(cm => cm.SentAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(cm => cm.IsRead)
                  .HasDefaultValue(false);

            entity.Property(cm => cm.Content)
                  .IsRequired()
                  .HasMaxLength(1000);
        }

        // ============================
        // Configuration CommunityPost
        // ============================

        private static void ConfigureCommunityPost(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<CommunityPost>();

            entity.ToTable("CommunityPosts");

            entity.HasKey(cp => cp.Id);

            entity.HasIndex(cp => cp.AuthorId);
            entity.HasIndex(cp => cp.PostType);
            entity.HasIndex(cp => cp.CreatedAt);
            entity.HasIndex(cp => cp.IsActive);

            entity.HasOne(cp => cp.Author)
                  .WithMany()
                  .HasForeignKey(cp => cp.AuthorId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(cp => cp.CreatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(cp => cp.PostType)
                  .HasDefaultValue(CommunityPostType.Discussion);

            entity.Property(cp => cp.LikesCount)
                  .HasDefaultValue(0);

            entity.Property(cp => cp.CommentsCount)
                  .HasDefaultValue(0);

            entity.Property(cp => cp.IsActive)
                  .HasDefaultValue(true);

            entity.Property(cp => cp.Title)
                  .IsRequired()
                  .HasMaxLength(200);

            entity.Property(cp => cp.Content)
                  .IsRequired()
                  .HasMaxLength(5000);

            entity.Property(cp => cp.ImageUrl)
                  .HasMaxLength(500);
        }

        // ============================
        // Configuration CommunityPostLike
        // ============================

        private static void ConfigureCommunityPostLike(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<CommunityPostLike>();

            entity.ToTable("CommunityPostLikes");

            entity.HasKey(cpl => cpl.Id);

            entity.HasIndex(cpl => new { cpl.PostId, cpl.UserId }).IsUnique();
            entity.HasIndex(cpl => cpl.PostId);
            entity.HasIndex(cpl => cpl.UserId);

            entity.HasOne(cpl => cpl.Post)
                  .WithMany(cp => cp.Likes)
                  .HasForeignKey(cpl => cpl.PostId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cpl => cpl.User)
                  .WithMany()
                  .HasForeignKey(cpl => cpl.UserId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(cpl => cpl.CreatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }

        // ============================
        // Configuration CommunityPostComment
        // ============================

        private static void ConfigureCommunityPostComment(ModelBuilder modelBuilder)
        {
            var entity = modelBuilder.Entity<CommunityPostComment>();

            entity.ToTable("CommunityPostComments");

            entity.HasKey(cpc => cpc.Id);

            entity.HasIndex(cpc => cpc.PostId);
            entity.HasIndex(cpc => cpc.AuthorId);
            entity.HasIndex(cpc => cpc.CreatedAt);

            entity.HasOne(cpc => cpc.Post)
                  .WithMany(cp => cp.Comments)
                  .HasForeignKey(cpc => cpc.PostId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(cpc => cpc.Author)
                  .WithMany()
                  .HasForeignKey(cpc => cpc.AuthorId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.Property(cpc => cpc.CreatedAt)
                  .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(cpc => cpc.IsActive)
                  .HasDefaultValue(true);

            entity.Property(cpc => cpc.Content)
                  .IsRequired()
                  .HasMaxLength(1000);
        }
    }
}
