using prisonbreak.Server.DTOs;
using prisonbreak.Server.Models;

namespace prisonbreak.Server.Services;

/// <summary>
/// Interface du service de gestion des utilisateurs
/// Définit les opérations métier pour la gestion des utilisateurs
/// Séparation des responsabilités : le service contient la logique métier,
/// le repository gère l'accès aux données
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Récupère un utilisateur par son identifiant
    /// </summary>
    /// <param name="id">Identifiant unique de l'utilisateur</param>
    /// <returns>Le DTO de l'utilisateur trouvé, ou null si non trouvé</returns>
    Task<UserDto?> GetUserByIdAsync(int id);

    /// <summary>
    /// Récupère un utilisateur par son adresse email
    /// </summary>
    /// <param name="email">Adresse email de l'utilisateur</param>
    /// <returns>Le DTO de l'utilisateur trouvé, ou null si non trouvé</returns>
    Task<UserDto?> GetUserByEmailAsync(string email);

    /// <summary>
    /// Récupère tous les utilisateurs
    /// </summary>
    /// <param name="includeInactive">Si true, inclut les utilisateurs inactifs</param>
    /// <returns>Liste des DTOs des utilisateurs</returns>
    Task<IEnumerable<UserDto>> GetAllUsersAsync(bool includeInactive = false);

    /// <summary>
    /// Crée un nouvel utilisateur
    /// </summary>
    /// <param name="request">Requête contenant les informations de l'utilisateur à créer</param>
    /// <returns>Le DTO de l'utilisateur créé</returns>
    /// <exception cref="InvalidOperationException">Si un utilisateur avec cet email existe déjà</exception>
    Task<UserDto> CreateUserAsync(CreateUserRequest request);

    /// <summary>
    /// Met à jour les informations d'un utilisateur
    /// </summary>
    /// <param name="id">Identifiant de l'utilisateur à mettre à jour</param>
    /// <param name="request">Requête contenant les nouvelles informations</param>
    /// <returns>Le DTO de l'utilisateur mis à jour</returns>
    /// <exception cref="ArgumentException">Si l'utilisateur n'existe pas</exception>
    Task<UserDto> UpdateUserAsync(int id, CreateUserRequest request);

    /// <summary>
    /// Désactive un utilisateur (soft delete)
    /// </summary>
    /// <param name="id">Identifiant de l'utilisateur à désactiver</param>
    /// <returns>True si l'utilisateur a été désactivé, False si non trouvé</returns>
    Task<bool> DeactivateUserAsync(int id);

    /// <summary>
    /// Met à jour la date de dernière connexion d'un utilisateur
    /// </summary>
    /// <param name="id">Identifiant de l'utilisateur</param>
    /// <returns>True si la mise à jour a réussi, False si l'utilisateur n'existe pas</returns>
    Task<bool> UpdateLastLoginAsync(int id);

    /// <summary>
    /// Convertit un modèle User en UserDto
    /// </summary>
    /// <param name="user">Le modèle User à convertir</param>
    /// <returns>Le DTO correspondant</returns>
    UserDto ConvertToDto(User user);
}

