using System;
using Unity.Collections;

using UnityEngine;

namespace Assets.Scripts.Models.WFC
{
    /// <summary>
    /// Représente une liste de voisins possibles pour une case donnée
    /// à une cellule spécifique.
    /// Cela permet d'avoir des cases s'étendant sur plusieurs cellules
    /// et pouvant donc posséder plusieurs ports par côté.
    /// </summary>
    [Serializable]
    public struct Socket
    {
        /// <summary>
        /// Les IDs des voisins possibles pour ce port
        /// </summary>
        [field: SerializeField]
        public FixedString32Bytes[] PossibleNeighbours { get; private set; }

        /// <summary>
        /// Les probabilités de chaque ID d'être sélectionné pour la génération.
        /// Si le tableau est vide, les IDs DOIVENT être sélectionnés 
        /// tels qu'ils sont renseignés dans la liste des voisins possibles.
        /// </summary>
        [field: SerializeField]
        public float[] Weights { get; private set; }
    }
}