using System;
using UnityEngine;

namespace Assets.Scripts.Models.WFC
{
    /// <summary>
    /// Contient une référence à une case
    /// et le montant d'instances à créer
    /// </summary>
    [Serializable]
    public struct FixedTile
    {
        /// <summary>
        /// Les possibilités de cette cellule
        /// </summary>
        [field: SerializeField]
        public Tile Tile { get; private set; }

        /// <summary>
        /// La rotation de cette cellule
        /// </summary>
        [field: SerializeField]
        public int Amount { get; set; }
    }
}