using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;
using prisonbreak.Server.Repositories;

namespace prisonbreak.Server.Services;

/// <summary>
/// Service de gestion des utilisateurs
/// Implémente la logique métier pour la gestion des utilisateurs
/// Utilise le pattern Repository pour l'accès aux données
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;

    /// <summary>
    /// Initialise une nouvelle instance du service utilisateur
    /// </summary>
    /// <param name="userRepository">Repository pour l'accès aux données utilisateur</param>
    /// <param name="sessionRepository">Repository pour l'accès aux données session</param>
    public UserService(IUserRepository userRepository, ISessionRepository sessionRepository)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
    }

    /// <inheritdoc/>
    public async Task<UserDto?> GetUserByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null ? ConvertToDto(user) : null;
    }

    /// <inheritdoc/>
    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("L'email ne peut pas être vide", nameof(email));

        var user = await _userRepository.GetByEmailAsync(email);
        return user != null ? ConvertToDto(user) : null;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync(bool includeInactive = false)
    {
        var users = await _userRepository.GetAllAsync(includeInactive);
        return users.Select(ConvertToDto);
    }

    /// <inheritdoc/>
    public async Task<UserDto> CreateUserAsync(CreateUserRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // Vérifier si un utilisateur avec cet email existe déjà
        if (await _userRepository.ExistsByEmailAsync(request.Email))
        {
            throw new InvalidOperationException($"Un utilisateur avec l'email '{request.Email}' existe déjà");
        }

        // Créer le nouvel utilisateur
        var user = new User
        {
            Name = request.Name.Trim(),
            Email = request.Email.Trim().ToLower(),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        var createdUser = await _userRepository.CreateAsync(user);
        return ConvertToDto(createdUser);
    }

    /// <inheritdoc/>
    public async Task<UserDto> UpdateUserAsync(int id, CreateUserRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new ArgumentException($"L'utilisateur avec l'ID {id} n'existe pas", nameof(id));

        // Vérifier si l'email change et s'il existe déjà
        if (user.Email.ToLower() != request.Email.Trim().ToLower())
        {
            if (await _userRepository.ExistsByEmailAsync(request.Email))
            {
                throw new InvalidOperationException($"Un utilisateur avec l'email '{request.Email}' existe déjà");
            }
        }

        // Mettre à jour les propriétés
        user.Name = request.Name.Trim();
        user.Email = request.Email.Trim().ToLower();

        var updatedUser = await _userRepository.UpdateAsync(user);
        return ConvertToDto(updatedUser);
    }

    /// <inheritdoc/>
    public async Task<bool> DeactivateUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return false;

        user.IsActive = false;
        await _userRepository.UpdateAsync(user);

        // Désactiver toutes les sessions actives de l'utilisateur
        var activeSessions = await _sessionRepository.GetByUserIdAsync(user.Id, includeActiveOnly: true);
        foreach (var session in activeSessions)
        {
            session.Deactivate();
            await _sessionRepository.UpdateAsync(session);
        }

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateLastLoginAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return false;

        user.UpdateLastLogin();
        await _userRepository.UpdateAsync(user);

        return true;
    }

    /// <inheritdoc/>
    public UserDto ConvertToDto(User user)
    {
        if (user == null)
            throw new ArgumentNullException(nameof(user));

        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            LastLoginAt = user.LastLoginAt,
            IsActive = user.IsActive,
            ActiveSessionCount = user.Sessions?.Count(s => s.IsActive && !s.IsExpired()) ?? 0
        };
    }
}

