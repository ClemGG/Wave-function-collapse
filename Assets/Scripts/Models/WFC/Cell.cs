using System;
using Unity.Collections;
using Unity.Mathematics;

namespace Assets.Scripts.Models.WFC
{
    /// <summary>
    /// Représente une cellule de la grille générée par l'algo WFC
    /// </summary>
    public readonly struct Cell : IDisposable
    {
        #region Propriétés

        /// <summary>
        /// TRUE si la cellule n'a plus qu'une seule option
        /// </summary>
        public readonly bool Collapsed => Options.Length == 1;

        /// <summary>
        /// Les coordonnées de cette cellule
        /// </summary>
        public readonly int3 Coords { get; }

        /// <summary>
        /// La liste des options restantes dans cette cellule
        /// </summary>
        public readonly NativeList<int> Options { get; }

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="coords">Les coordonnées de cette cellule</param>
        /// <param name="options">La liste des options restantes dans cette cellule</param>
        public Cell(int3 coords, NativeList<int> options)
        {
            Options = options;
            Coords = coords;
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Nettoyage
        /// </summary>
        public void Dispose()
        {
            Options.Dispose();
        }

        #endregion
    }
}