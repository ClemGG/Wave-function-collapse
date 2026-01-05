using System;
using Assets.Scripts.Models.WFC.SOs;
using UnityEngine;

namespace Assets.Scripts.Models.WFC
{
    /// <summary>
    /// Les listes des voisins valides pour chaque face d'un module
    /// </summary>
    [Serializable]
    public struct ValidNeighbours
    {
        /// <summary>
        ///Les voisins valides pour la face droite
        /// </summary>
        [field: SerializeField]
        public ModuleSO[] RightNeighbours { get; private set; }

        /// <summary>
        ///Les voisins valides pour la face gauche
        /// </summary>
        [field: SerializeField]
        public ModuleSO[] LeftNeighbours { get; private set; }

        /// <summary>
        ///Les voisins valides pour la face au-dessus
        /// </summary>
        [field: SerializeField]
        public ModuleSO[] UpNeighbours { get; private set; }

        /// <summary>
        ///Les voisins valides pour la face en-dessous
        /// </summary>
        [field: SerializeField]
        public ModuleSO[] DownNeighbours { get; private set; }

        /// <summary>
        ///Les voisins valides pour la face avant
        /// </summary>
        [field: SerializeField]
        public ModuleSO[] ForwardNeighbours { get; private set; }

        /// <summary>
        ///Les voisins valides pour la face arrière
        /// </summary>
        [field: SerializeField]
        public ModuleSO[] BackNeighbours { get; private set; }
    }
}