namespace prisonbreak.Server.Models;

/// <summary>
/// Représente une île dans le jeu Hashi
/// Une île est un nœud du puzzle qui doit être connecté à d'autres îles par des ponts
/// </summary>
public class Island
{
    /// <summary>
    /// Identifiant unique de l'île
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Position X de l'île dans la grille (colonne)
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// Position Y de l'île dans la grille (ligne)
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// Nombre requis de ponts qui doivent être connectés à cette île
    /// Valeur entre 1 et 8
    /// </summary>
    public int RequiredBridges { get; set; }

    /// <summary>
    /// Identifiant du puzzle auquel appartient cette île
    /// </summary>
    public int PuzzleId { get; set; }

    /// <summary>
    /// Navigation vers le puzzle parent
    /// </summary>
    public Puzzle? Puzzle { get; set; }

    /// <summary>
    /// Collection des ponts partant de cette île
    /// </summary>
    public ICollection<Bridge> BridgesFrom { get; set; } = new List<Bridge>();

    /// <summary>
    /// Collection des ponts arrivant à cette île
    /// </summary>
    public ICollection<Bridge> BridgesTo { get; set; } = new List<Bridge>();

    /// <summary>
    /// Calcule le nombre actuel de ponts connectés à cette île
    /// </summary>
    /// <returns>Nombre total de ponts (en comptant les ponts doubles comme 2)</returns>
    public int GetCurrentBridgeCount()
    {
        int count = 0;
        
        // Compter les ponts sortants
        foreach (var bridge in BridgesFrom)
        {
            count += bridge.IsDouble ? 2 : 1;
        }
        
        // Compter les ponts entrants
        foreach (var bridge in BridgesTo)
        {
            count += bridge.IsDouble ? 2 : 1;
        }
        
        return count;
    }

    /// <summary>
    /// Vérifie si l'île a le bon nombre de ponts connectés
    /// </summary>
    /// <returns>True si le nombre de ponts correspond au nombre requis</returns>
    public bool IsComplete()
    {
        return GetCurrentBridgeCount() == RequiredBridges;
    }

    /// <summary>
    /// Vérifie si on peut encore ajouter des ponts à cette île
    /// </summary>
    /// <returns>True si on peut ajouter au moins un pont de plus</returns>
    public bool CanAddBridge()
    {
        return GetCurrentBridgeCount() < RequiredBridges;
    }
}

