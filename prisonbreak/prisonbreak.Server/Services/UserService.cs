using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;
using prisonbreak.Server.Repositories;

namespace prisonbreak.Server.Services
{
    /// <summary>
    /// Implémente la logique métier pour User.
    /// </summary>
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<UserDto> CreateOrLoginAsync(string name, string email)
        {
            var existing = await _userRepository.GetByEmailAsync(email);

            if (existing is null)
            {
                var newUser = new User
                {
                    Name = name,
                    Email = email,
                    CreatedAt = DateTime.UtcNow,
                    LastLoginAt = DateTime.UtcNow,
                    IsActive = true
                };

                newUser = await _userRepository.AddAsync(newUser);

                return MapToDto(newUser, activeSessionCount: 0);
            }
            else
            {
                existing.Name = name;
                existing.LastLoginAt = DateTime.UtcNow;

                await _userRepository.UpdateAsync(existing);

                // Pour l'instant, on ne calcule pas vraiment ActiveSessionCount ici.
                // On laisse 0 ou on pourra ajouter un vrai calcul plus tard
                // en utilisant un SessionRepository si besoin.
                return MapToDto(existing, activeSessionCount: 0);
            }
        }

        public async Task<List<UserDto>> SearchUsersByEmailAsync(string emailQuery, int limit = 10)
        {
            var users = await _userRepository.SearchByEmailAsync(emailQuery, limit);
            return users.Select(u => MapToDto(u, 0)).ToList();
        }

        /// <summary>
        /// Mappe un modèle User vers un UserDto complet.
        /// </summary>
        private static UserDto MapToDto(User user, int activeSessionCount)
        {
            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                IsActive = user.IsActive,
                ActiveSessionCount = activeSessionCount,
                Coins = user.Coins
            };
        }
    }
}
