using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;
using prisonbreak.Server.Repositories;

namespace prisonbreak.Server.Services;

/// <summary>
/// Service de gestion des sessions
/// Implémente la logique métier pour la gestion des sessions de jeu
/// </summary>
public class SessionService : ISessionService
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IUserRepository _userRepository;

    /// <summary>
    /// Initialise une nouvelle instance du service session
    /// </summary>
    /// <param name="sessionRepository">Repository pour l'accès aux données session</param>
    /// <param name="userRepository">Repository pour l'accès aux données utilisateur</param>
    public SessionService(ISessionRepository sessionRepository, IUserRepository userRepository)
    {
        _sessionRepository = sessionRepository ?? throw new ArgumentNullException(nameof(sessionRepository));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    /// <inheritdoc/>
    public async Task<SessionDto?> GetSessionByIdAsync(int id)
    {
        var session = await _sessionRepository.GetByIdAsync(id, includeUser: true, includeGames: true);
        return session != null ? ConvertToDto(session) : null;
    }

    /// <inheritdoc/>
    public async Task<SessionDto?> GetSessionByTokenAsync(string sessionToken)
    {
        if (string.IsNullOrWhiteSpace(sessionToken))
            throw new ArgumentException("Le token de session ne peut pas être vide", nameof(sessionToken));

        var session = await _sessionRepository.GetByTokenAsync(sessionToken, includeUser: true, includeGames: true);
        return session != null ? ConvertToDto(session) : null;
    }

    /// <inheritdoc/>
    public async Task<IEnumerable<SessionDto>> GetSessionsByUserIdAsync(int userId, bool includeActiveOnly = false)
    {
        var sessions = await _sessionRepository.GetByUserIdAsync(userId, includeActiveOnly);
        return sessions.Select(ConvertToDto);
    }

    /// <inheritdoc/>
    public async Task<SessionDto?> GetActiveSessionByUserIdAsync(int userId)
    {
        var session = await _sessionRepository.GetActiveSessionByUserIdAsync(userId);
        return session != null ? ConvertToDto(session) : null;
    }

    /// <inheritdoc/>
    public async Task<SessionDto> CreateSessionAsync(CreateSessionRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        // Vérifier que l'utilisateur existe
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
            throw new ArgumentException($"L'utilisateur avec l'ID {request.UserId} n'existe pas", nameof(request));

        // Vérifier que l'utilisateur est actif
        if (!user.IsActive)
            throw new InvalidOperationException($"L'utilisateur avec l'ID {request.UserId} est inactif");

        // Désactiver les sessions actives existantes de l'utilisateur (une seule session active à la fois)
        var activeSessions = await _sessionRepository.GetByUserIdAsync(request.UserId, includeActiveOnly: true);
        foreach (var activeSession in activeSessions)
        {
            activeSession.Deactivate();
            await _sessionRepository.UpdateAsync(activeSession);
        }

        // Créer la nouvelle session
        var session = new Session
        {
            UserId = request.UserId,
            SessionToken = Session.GenerateSessionToken(),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(request.DurationDays),
            LastActivityAt = DateTime.UtcNow,
            IsActive = true,
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent
        };

        var createdSession = await _sessionRepository.CreateAsync(session);

        // Mettre à jour la dernière connexion de l'utilisateur
        user.UpdateLastLogin();
        await _userRepository.UpdateAsync(user);

        return ConvertToDto(createdSession);
    }

    /// <inheritdoc/>
    public async Task<bool> UpdateSessionActivityAsync(string sessionToken)
    {
        if (string.IsNullOrWhiteSpace(sessionToken))
            return false;

        var session = await _sessionRepository.GetByTokenAsync(sessionToken);
        if (session == null || !session.IsValid())
            return false;

        session.UpdateActivity();
        await _sessionRepository.UpdateAsync(session);

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> DeactivateSessionAsync(string sessionToken)
    {
        if (string.IsNullOrWhiteSpace(sessionToken))
            return false;

        var session = await _sessionRepository.GetByTokenAsync(sessionToken);
        if (session == null)
            return false;

        session.Deactivate();
        await _sessionRepository.UpdateAsync(session);

        return true;
    }

    /// <inheritdoc/>
    public async Task<bool> IsValidSessionAsync(string sessionToken)
    {
        return await _sessionRepository.IsValidSessionAsync(sessionToken);
    }

    /// <inheritdoc/>
    public SessionDto ConvertToDto(Session session)
    {
        if (session == null)
            throw new ArgumentNullException(nameof(session));

        return new SessionDto
        {
            Id = session.Id,
            UserId = session.UserId,
            User = session.User != null ? new UserDto
            {
                Id = session.User.Id,
                Name = session.User.Name,
                Email = session.User.Email,
                CreatedAt = session.User.CreatedAt,
                LastLoginAt = session.User.LastLoginAt,
                IsActive = session.User.IsActive,
                ActiveSessionCount = 0 // Non calculé ici pour éviter les requêtes supplémentaires
            } : null,
            SessionToken = session.SessionToken,
            CreatedAt = session.CreatedAt,
            ExpiresAt = session.ExpiresAt,
            LastActivityAt = session.LastActivityAt,
            IsActive = session.IsActive,
            IsExpired = session.IsExpired(),
            GameCount = session.Games?.Count ?? 0
        };
    }
}

