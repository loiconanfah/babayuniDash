using System.ComponentModel.DataAnnotations;

namespace prisonbreak.Server.DTOs;

/// <summary>
/// Requête pour créer un nouvel utilisateur
/// Utilisé dans les endpoints POST pour la création de compte
/// </summary>
public class CreateUserRequest
{
    /// <summary>
    /// Nom complet ou pseudonyme de l'utilisateur
    /// Requis, entre 2 et 100 caractères
    /// </summary>
    [Required(ErrorMessage = "Le nom est requis")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Le nom doit contenir entre 2 et 100 caractères")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Adresse email de l'utilisateur
    /// Requis, doit être une adresse email valide
    /// </summary>
    [Required(ErrorMessage = "L'email est requis")]
    [EmailAddress(ErrorMessage = "L'email doit être une adresse email valide")]
    [StringLength(255, ErrorMessage = "L'email ne peut pas dépasser 255 caractères")]
    public string Email { get; set; } = string.Empty;
}

