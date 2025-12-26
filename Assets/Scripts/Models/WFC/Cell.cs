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
        public List<ITileOption> Options { get; set; }

        /// <summary>
        /// La rotation de cette cellule
        /// </summary>
        public int Rotation { get; set; }

        /// <summary>
        /// Les coordonnées de cette cellule
        /// </summary>
        public Range Range { get; set; }

        /// <summary>
        /// Les IDs des voisins possibles à droite de cette case
        /// </summary>
        public List<Range> RightNeighbours { get; set; }

        /// <summary>
        /// Les IDs des voisins possibles à gauche de cette case
        /// </summary>
        public List<Range> LeftNeighbours { get; set; }

        /// <summary>
        /// Les IDs des voisins possibles au-dessus de cette case
        /// </summary>
        public List<Range> UpNeighbours { get; set; }

        /// <summary>
        /// Les IDs des voisins possibles au-dessus de cette case
        /// </summary>
        public List<Range> DownNeighbours { get; set; }

        /// <summary>
        /// Les IDs des voisins possibles devant de cette case
        /// </summary>
        public List<Range> ForwardNeighbours { get; set; }

        /// <summary>
        /// Les IDs des voisins possibles derrière de cette case
        /// </summary>
        public List<Range> BackNeighbours { get; set; }

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="options">Les possibilités de cette cellule</param>
        /// <param name="coords">Les coordonnées de cette cellule</param>
        public Cell(List<ITileOption> options, int3 coords)
        {
            this.Options = options;
            this.Range = new Range(coords);
            this.Rotation = 0;
            this.RightNeighbours = new List<Range>();
            this.LeftNeighbours = new List<Range>();
            this.UpNeighbours = new List<Range>();
            this.DownNeighbours = new List<Range>();
            this.ForwardNeighbours = new List<Range>();
            this.BackNeighbours = new List<Range>();
        }

        #endregion
    }
}