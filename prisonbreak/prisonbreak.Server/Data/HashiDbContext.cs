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
        /// Table des ponts de solution d’un puzzle (Bridge).
        /// </summary>
        public DbSet<Bridge> Bridges => Set<Bridge>();

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
    }
}
