using System.ComponentModel.DataAnnotations.Schema;

namespace prisonbreak.Server.Models;

/// <summary>
/// Représente un pont entre deux îles dans le jeu Hashi
/// Un pont peut être simple (1 connexion) ou double (2 connexions)
/// </summary>
public class Bridge
{
    /// <summary>
    /// Identifiant unique du pont
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Identifiant de l'île de départ
    /// </summary>
    public int FromIslandId { get; set; }

    /// <summary>
    /// Identifiant de l'île d'arrivée
    /// </summary>
    public int ToIslandId { get; set; }

    /// <summary>
    /// Navigation vers l'île de départ
    /// </summary>
    public Island? FromIsland { get; set; }

    /// <summary>
    /// Navigation vers l'île d'arrivée
    /// </summary>
    public Island? ToIsland { get; set; }

    /// <summary>
    /// Indique si le pont est double (compte pour 2 connexions)
    /// False = pont simple (1 connexion)
    /// True = pont double (2 connexions)
    /// </summary>
    public bool IsDouble { get; set; }

    /// <summary>
    /// Direction du pont (Horizontal ou Vertical)
    /// </summary>
    public BridgeDirection Direction { get; set; }

    /// <summary>
    /// Identifiant du puzzle auquel appartient ce pont
    /// </summary>
    public int PuzzleId { get; set; }

    /// <summary>
    /// Navigation vers le puzzle parent
    /// </summary>
    public Puzzle? Puzzle { get; set; }

    /// <summary>
    /// Vérifie si ce pont croise un autre pont
    /// </summary>
    /// <param name="other">L'autre pont à vérifier</param>
    /// <returns>True si les ponts se croisent</returns>
    public bool IntersectsWith(Bridge other)
    {
        // Deux ponts parallèles ne peuvent pas se croiser
        if (Direction == other.Direction)
            return false;

        if (FromIsland == null || ToIsland == null || other.FromIsland == null || other.ToIsland == null)
            return false;

        // Si ce pont est horizontal et l'autre vertical
        if (Direction == BridgeDirection.Horizontal && other.Direction == BridgeDirection.Vertical)
        {
            // Vérifier si le pont vertical passe à travers ce pont horizontal
            int thisMinX = Math.Min(FromIsland.X, ToIsland.X);
            int thisMaxX = Math.Max(FromIsland.X, ToIsland.X);
            int thisY = FromIsland.Y;

            int otherMinY = Math.Min(other.FromIsland.Y, other.ToIsland.Y);
            int otherMaxY = Math.Max(other.FromIsland.Y, other.ToIsland.Y);
            int otherX = other.FromIsland.X;

            return otherX > thisMinX && otherX < thisMaxX && thisY > otherMinY && thisY < otherMaxY;
        }

        // Si ce pont est vertical et l'autre horizontal
        if (Direction == BridgeDirection.Vertical && other.Direction == BridgeDirection.Horizontal)
        {
            return other.IntersectsWith(this);
        }

        return false;
    }
}

/// <summary>
/// Direction d'un pont dans le jeu Hashi
/// Les ponts ne peuvent être que horizontaux ou verticaux (pas de diagonales)
/// </summary>
public enum BridgeDirection
{
    /// <summary>
    /// Pont horizontal (gauche-droite)
    /// </summary>
    Horizontal,

    /// <summary>
    /// Pont vertical (haut-bas)
    /// </summary>
    Vertical
}

