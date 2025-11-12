namespace prisonbreak.Server.DTOs;

/// <summary>
/// DTO pour un puzzle complet
/// Contient toutes les informations nécessaires pour afficher et jouer un puzzle
/// </summary>
public class PuzzleDto
{
    /// <summary>
    /// Identifiant du puzzle
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nom du puzzle
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Largeur de la grille
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// Hauteur de la grille
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// Niveau de difficulté (1=Easy, 2=Medium, 3=Hard, 4=Expert)
    /// </summary>
    public int Difficulty { get; set; }

    /// <summary>
    /// Liste des îles du puzzle
    /// </summary>
    public List<IslandDto> Islands { get; set; } = new();

    /// <summary>
    /// Nombre total d'îles
    /// </summary>
    public int IslandCount { get; set; }
}

