using Microsoft.EntityFrameworkCore;
using prisonbreak.Server.Data;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Repositories;

/// <summary>
/// Implémentation du repository pour la gestion des utilisateurs
/// Fournit l'accès aux données pour les opérations CRUD sur les utilisateurs
/// Utilise Entity Framework Core pour l'accès à la base de données
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly HashiDbContext _context;

    /// <summary>
    /// Initialise une nouvelle instance du repository utilisateur
    /// </summary>
    /// <param name="context">Contexte de base de données Entity Framework</param>
    public UserRepository(HashiDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc/>
    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Sessions)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    /// <inheritdoc/>
    public async Task<User?> GetByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("L'email ne peut pas être vide", nameof(email));

        return await _context.Users
            .Include(u => u.Sessions)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<User>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.Users
            .Include(u => u.Sessions)
            .AsQueryable();

        if (!includeInactive)
        {
            query = query.Where(u => u.IsActive);
        }

        return await query.ToListAsync();
    }

    /// <inheritdoc/>
    public async Task<bool> ExistsByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return await _context.Users
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }

    /// <inheritdoc/>
    public async Task<User> CreateAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        // Vérifier que l'email n'existe pas déjà
        if (await ExistsByEmailAsync(user.Email))
            throw new InvalidOperationException($"Un utilisateur avec l'email '{user.Email}' existe déjà");

        // S'assurer que la date de création est définie
        if (user.CreatedAt == default)
            user.CreatedAt = DateTime.UtcNow;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
    }

    /// <inheritdoc/>
    public async Task<User> UpdateAsync(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return user;
    }

    /// <inheritdoc/>
    public async Task<bool> DeleteAsync(int id)
    {
        var user = await GetByIdAsync(id);
        if (user == null)
            return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return true;
    }

    /// <inheritdoc/>
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}

