using System.Collections.Generic;

/// <summary>
/// Représente une cellule et ses possibilités dans la grille
/// </summary>
public struct Cell
{
    #region Propriétés

    /// <summary>
    /// TRUE s'il ne reste qu'une seule possibilité à cette case
    /// </summary>
    public readonly bool Collapsed => this.Entropy == 1;

    /// <summary>
    /// Les possibilités restantes de cette cellule
    /// </summary>
    public readonly int Entropy => this.Options.Count;

    /// <summary>
    /// Les possibilités de cette cellule
    /// </summary>
    public List<Tile> Options { get; private set; }

    /// <summary>
    /// La rotation de cette cellule
    /// </summary>
    public int Rotation { get; set; }

    #endregion

    #region Constructeur

    /// <summary>
    /// Constructeur
    /// </summary>
    /// <param name="options">Les possibilités de cette cellule</param>
    public Cell(List<Tile> options)
    {
        this.Options = options;
        this.Rotation = 0;
    }

    #endregion
}
