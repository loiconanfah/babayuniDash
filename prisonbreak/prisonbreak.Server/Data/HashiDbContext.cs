using Microsoft.EntityFrameworkCore;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Data;

/// <summary>
/// Contexte de base de données pour le jeu Hashi
/// Gère toutes les entités et leurs relations
/// </summary>
public class HashiDbContext : DbContext
{
    public HashiDbContext(DbContextOptions<HashiDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Table des puzzles
    /// </summary>
    public DbSet<Puzzle> Puzzles { get; set; }

    /// <summary>
    /// Table des îles
    /// </summary>
    public DbSet<Island> Islands { get; set; }

    /// <summary>
    /// Table des ponts
    /// </summary>
    public DbSet<Bridge> Bridges { get; set; }

    /// <summary>
    /// Table des parties
    /// </summary>
    public DbSet<Game> Games { get; set; }

    /// <summary>
    /// Table des utilisateurs
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Table des sessions
    /// </summary>
    public DbSet<Session> Sessions { get; set; }

    /// <summary>
    /// Configuration du modèle de données
    /// Définit les relations entre les entités et les contraintes
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuration du Puzzle
        modelBuilder.Entity<Puzzle>(entity =>
        {
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Name).HasMaxLength(200);
            entity.Property(p => p.Width).IsRequired();
            entity.Property(p => p.Height).IsRequired();
            entity.Property(p => p.Difficulty).IsRequired();
            entity.Property(p => p.CreatedAt).IsRequired();

            // Relation Puzzle -> Islands (un puzzle a plusieurs îles)
            entity.HasMany(p => p.Islands)
                .WithOne(i => i.Puzzle)
                .HasForeignKey(i => i.PuzzleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation Puzzle -> Bridges (un puzzle a plusieurs ponts dans sa solution)
            entity.HasMany(p => p.SolutionBridges)
                .WithOne(b => b.Puzzle)
                .HasForeignKey(b => b.PuzzleId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relation Puzzle -> Games (un puzzle peut avoir plusieurs parties)
            entity.HasMany(p => p.Games)
                .WithOne(g => g.Puzzle)
                .HasForeignKey(g => g.PuzzleId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration de l'Island
        modelBuilder.Entity<Island>(entity =>
        {
            entity.HasKey(i => i.Id);
            entity.Property(i => i.X).IsRequired();
            entity.Property(i => i.Y).IsRequired();
            entity.Property(i => i.RequiredBridges).IsRequired();

            // Index pour améliorer les performances de recherche par position
            entity.HasIndex(i => new { i.PuzzleId, i.X, i.Y });
        });

        // Configuration du Bridge
        modelBuilder.Entity<Bridge>(entity =>
        {
            entity.HasKey(b => b.Id);
            entity.Property(b => b.IsDouble).IsRequired();
            entity.Property(b => b.Direction).IsRequired();

            // Relation Bridge -> FromIsland (pont depuis une île)
            entity.HasOne(b => b.FromIsland)
                .WithMany(i => i.BridgesFrom)
                .HasForeignKey(b => b.FromIslandId)
                .OnDelete(DeleteBehavior.Restrict); // Empêche la suppression en cascade

            // Relation Bridge -> ToIsland (pont vers une île)
            entity.HasOne(b => b.ToIsland)
                .WithMany(i => i.BridgesTo)
                .HasForeignKey(b => b.ToIslandId)
                .OnDelete(DeleteBehavior.Restrict); // Empêche la suppression en cascade

            // Index pour améliorer les performances
            entity.HasIndex(b => new { b.FromIslandId, b.ToIslandId });
        });

        // Configuration du User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);
            entity.Property(u => u.Name).IsRequired().HasMaxLength(100);
            entity.Property(u => u.Email).IsRequired().HasMaxLength(255);
            entity.Property(u => u.CreatedAt).IsRequired();
            entity.Property(u => u.IsActive).IsRequired().HasDefaultValue(true);

            // Index unique sur l'email pour garantir l'unicité
            entity.HasIndex(u => u.Email).IsUnique();

            // Index pour améliorer les recherches
            entity.HasIndex(u => u.IsActive);
            entity.HasIndex(u => u.Email);

            // Relation User -> Sessions (un utilisateur a plusieurs sessions)
            entity.HasMany(u => u.Sessions)
                .WithOne(s => s.User)
                .HasForeignKey(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration du Session
        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(s => s.Id);
            entity.Property(s => s.SessionToken).IsRequired().HasMaxLength(255);
            entity.Property(s => s.CreatedAt).IsRequired();
            entity.Property(s => s.ExpiresAt).IsRequired();
            entity.Property(s => s.LastActivityAt).IsRequired();
            entity.Property(s => s.IsActive).IsRequired().HasDefaultValue(true);
            entity.Property(s => s.IpAddress).HasMaxLength(45); // IPv6 max length
            entity.Property(s => s.UserAgent).HasMaxLength(500);

            // Index unique sur le token pour garantir l'unicité
            entity.HasIndex(s => s.SessionToken).IsUnique();

            // Index pour améliorer les recherches
            entity.HasIndex(s => s.UserId);
            entity.HasIndex(s => new { s.IsActive, s.ExpiresAt });
            entity.HasIndex(s => s.CreatedAt);

            // Relation Session -> Games (une session a plusieurs parties)
            entity.HasMany(s => s.Games)
                .WithOne(g => g.Session)
                .HasForeignKey(g => g.SessionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuration du Game
        modelBuilder.Entity<Game>(entity =>
        {
            entity.HasKey(g => g.Id);
            entity.Property(g => g.SessionId).IsRequired();
            entity.Property(g => g.StartedAt).IsRequired();
            entity.Property(g => g.Status).IsRequired();
            entity.Property(g => g.PlayerBridgesJson).IsRequired().HasDefaultValue("[]");
            entity.Property(g => g.Score).HasDefaultValue(0);
            entity.Property(g => g.HintsUsed).HasDefaultValue(0);

            // Index pour rechercher les parties d'une session
            entity.HasIndex(g => g.SessionId);
            
            // Index pour rechercher les parties par statut
            entity.HasIndex(g => g.Status);
        });
    }
}

