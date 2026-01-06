using System;
using Assets.Scripts.Models.WFC.SOs;
using UnityEngine;

namespace Assets.Scripts.Models.WFC
{
    /// <summary>
    /// Prototype représentant une rotation d'un module
    /// </summary>
    public struct Prototype
    {
        #region Propriétés

        /// <summary>
        /// Le modèle à instancier
        /// </summary>
        [field: SerializeField]
        public GameObject Prefab { get; set; }

        /// <summary>
        /// La rotation du prototype
        /// </summary>
        [field: SerializeField]
        public byte Rotation { get; set; }

        /// <summary>
        /// La probabilité du prototype d'être sélectioné lors de l'effondrement d'une cellule
        /// </summary>
        [field: SerializeField]
        public byte Weight { get; set; }

        /// <summary>
        /// Les ports de chaque face (X, -X, Y, -Y, Z, -Z)
        /// </summary>
        [field: SerializeField]
        public string[] Sockets { get; set; }

        /// <summary>
        /// Les listes des voisins valides par face (X, -X, Y, -Y, Z, -Z)
        /// </summary>
        [field: SerializeField]
        public ValidNeighbours ValidNeighbours { get; set; }

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="module">Le module source</param>
        public Prototype(ModuleSO module) : this()
        {
            Rotation = 0;
            Weight = module.Weight;
            Prefab = module.Prefab;
            Sockets = new string[6];
            ValidNeighbours = new ValidNeighbours(module.ValidNeighbours);
            Array.Copy(module.Sockets, Sockets, module.Sockets.Length);
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Indique si les deux objets sont identiques
        /// </summary>
        /// <param name="other">Le comparé</param>
        /// <returns>TRUE si les deux objets sont identiques</returns>
        public readonly bool Equals(Prototype other)
        {
            if (Prefab != other.Prefab ||
                Sockets.Length != other.Sockets.Length ||
                !ValidNeighbours.Equals(other.ValidNeighbours))
            {
                return false;
            }

            for (int i = 0; i < Sockets.Length; ++i)
            {
                if (Sockets[i] != other.Sockets[i])
                {
                    return false;
                }
            }

            if (!ValidNeighbours.Equals(other.ValidNeighbours))
            {
                return false;
            }

            return true;
        }

        #endregion
    }
}