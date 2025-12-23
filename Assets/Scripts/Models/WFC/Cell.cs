using System.Collections.Generic;
using Unity.Mathematics;

namespace Assets.Scripts.Models.WFC
{
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
        public List<Tile> Options { get; set; }

        /// <summary>
        /// La rotation de cette cellule
        /// </summary>
        public int Rotation { get; set; }

        /// <summary>
        /// Les coordonnées de cette cellule
        /// </summary>
        public int3 Coords { get; set; }

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="options">Les possibilités de cette cellule</param>
        /// <param name="coords">Les coordonnées de cette cellule</param>
        public Cell(List<Tile> options, int3 coords)
        {
            this.Options = options;
            this.Coords = coords;
            this.Rotation = 0;
        }

        #endregion
    }
}