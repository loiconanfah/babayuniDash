using Microsoft.EntityFrameworkCore;
using prisonbreak.Server.Data;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Repositories
{
    /// <summary>
    /// Implémentation de l'accès aux données pour User via EF Core.
    /// </summary>
    public class UserRepository : IUserRepository
    {
        private readonly HashiDbContext _context;

        public UserRepository(HashiDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc />
        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <inheritdoc />
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        /// <inheritdoc />
        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        /// <inheritdoc />
        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        /// <inheritdoc />
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
