using prisonbreak.Server.DTOs;

namespace prisonbreak.Server.Services
{
    /// <summary>
    /// Logique métier pour la gestion des joueurs.
    /// </summary>
    public interface IUserService
    {
        Task<UserDto> CreateOrLoginAsync(string name, string email);
        Task<List<UserDto>> SearchUsersByEmailAsync(string emailQuery, int limit = 10);
    }
}
