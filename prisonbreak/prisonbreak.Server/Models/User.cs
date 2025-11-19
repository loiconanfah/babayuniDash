namespace prisonbreak.Server.Models;

/// <summary>
/// Représente un utilisateur du jeu Hashi
/// Chaque utilisateur possède un nom et une adresse email unique
/// Un utilisateur peut avoir plusieurs sessions de jeu
/// </summary>
public class User
{
    /// <summary>
    /// Identifiant unique de l'utilisateur
    /// Clé primaire auto-générée
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nom complet ou pseudonyme de l'utilisateur
    /// Requis, maximum 100 caractères
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Adresse email de l'utilisateur
    /// Requis, unique, maximum 255 caractères
    /// Utilisé pour l'identification et la communication
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Date et heure de création du compte utilisateur
    /// Initialisé automatiquement à la création
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Date et heure de la dernière connexion
    /// Mis à jour à chaque authentification
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Indique si le compte utilisateur est actif
    /// Permet de désactiver un compte sans le supprimer
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Collection des sessions de jeu de l'utilisateur
    /// Relation un-à-plusieurs : un utilisateur peut avoir plusieurs sessions
    /// </summary>
    public ICollection<Session> Sessions { get; set; } = new List<Session>();

    /// <summary>
    /// Vérifie si l'utilisateur a une session active
    /// </summary>
    /// <returns>True si l'utilisateur a au moins une session active, sinon False</returns>
    public bool HasActiveSession()
    {
        return Sessions.Any(s => s.IsActive);
    }

    /// <summary>
    /// Récupère la session active de l'utilisateur
    /// </summary>
    /// <returns>La première session active trouvée, ou null si aucune session active</returns>
    public Session? GetActiveSession()
    {
        return Sessions.FirstOrDefault(s => s.IsActive);
    }

    /// <summary>
    /// Met à jour la date de dernière connexion
    /// </summary>
    public void UpdateLastLogin()
    {
        LastLoginAt = DateTime.UtcNow;
    }
}

