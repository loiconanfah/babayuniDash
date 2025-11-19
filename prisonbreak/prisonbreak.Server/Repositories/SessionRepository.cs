using Microsoft.EntityFrameworkCore;
using prisonbreak.Server.Data;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Repositories;

/// <summary>
/// Implémentation du repository pour la gestion des sessions
/// Fournit l'accès aux données pour les opérations CRUD sur les sessions
/// </summary>
public class SessionRepository : ISessionRepository
{
    private readonly HashiDbContext _context;

    /// <summary>
    /// Initialise une nouvelle instance du repository session
    /// </summary>
    /// <param name="context">Contexte de base de données Entity Framework</param>
    public SessionRepository(HashiDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task<Session?> GetByIdAsync(int id, bool includeUser = false, bool includeGames = false)
    {
        var query = _context.Sessions.AsQueryable();

        if (includeUser)
            query = query.Include(s => s.User);

        if (includeGames)
            query = query.Include(s => s.Games);

        return await query.FirstOrDefaultAsync(s => s.Id == id);
    }

    /// <inheritdoc/>
    public async Task<Session?> GetByTokenAsync(string sessionToken, bool includeUser = false, bool includeGames = false)
    {
        if (string.IsNullOrWhiteSpace(sessionToken))
            throw new ArgumentException("Le token de session ne peut pas être vide", nameof(sessionToken));

        var query = _context.Sessions.AsQueryable();

        if (includeUser)
            query = query.Include(s => s.User);

        if (includeGames)
            query = query.Include(s => s.Games);

        return await query.FirstOrDefaultAsync(s => s.SessionToken == sessionToken);
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<Session>> GetByUserIdAsync(int userId, bool includeActiveOnly = false)
    {
        var query = _context.Sessions
            .Where(s => s.UserId == userId)
            .AsQueryable();

        if (includeActiveOnly)
        {
            query = query.Where(s => s.IsActive && s.ExpiresAt > DateTime.UtcNow);
        }

        return await query.ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<Session?> GetActiveSessionByUserIdAsync(int userId)
    {
        return await _context.Sessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.UserId == userId && s.IsActive && s.ExpiresAt > DateTime.UtcNow);
    }

    /// <inheritdoc/>
    public async Task<bool> IsValidSessionAsync(string sessionToken)
    {
        if (string.IsNullOrWhiteSpace(sessionToken))
            return false;

        var session = await _context.Sessions
            .FirstOrDefaultAsync(s => s.SessionToken == sessionToken);

        return session != null && session.IsValid();
    }

    /// <inheritdoc/>
    public async Task<Session> CreateAsync(Session session)
    {
        if (session == null)
            throw new ArgumentNullException(nameof(session));

        // Générer un token si non fourni
        if (string.IsNullOrWhiteSpace(session.SessionToken))
            session.SessionToken = Session.GenerateSessionToken();

        // Définir la date d'expiration par défaut (30 jours)
        if (session.ExpiresAt == default)
            session.ExpiresAt = DateTime.UtcNow.AddDays(30);

        // Initialiser les dates
        if (session.CreatedAt == default)
            session.CreatedAt = DateTime.UtcNow;

        if (session.LastActivityAt == default)
            session.LastActivityAt = DateTime.UtcNow;

        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();

        return session;
    }

    /// <inheritdoc/>
    public async Task<Session> UpdateAsync(Session session)
    {
        if (session == null)
            throw new ArgumentNullException(nameof(session));

        _context.Sessions.Update(session);
        await _context.SaveChangesAsync();

        return session;
    }

    /// <inheritdoc/>
    public async Task<int> DeactivateExpiredSessionsAsync()
    {
        var expiredSessions = await _context.Sessions
            .Where(s => s.IsActive && s.ExpiresAt <= DateTime.UtcNow)
            .ToListAsync();

        foreach (var session in expiredSessions)
        {
            session.Deactivate();
        }

        return await _context.SaveChangesAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        var session = await GetByIdAsync(id);
        if (session == null)
            return false;

        _context.Sessions.Remove(session);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc/>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}

