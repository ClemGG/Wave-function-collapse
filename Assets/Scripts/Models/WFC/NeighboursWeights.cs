using System;
using UnityEngine;

namespace Assets.Scripts.Models.WFC
{
    /// <summary>
    /// Les poids des voisins valides pour chaque face d'un module, de 0% à 100%.
    /// Si une liste est vide, cela veut dire qu'il n'y a qu'1 seul voisin
    /// ou qu'ils ont tous la même probabilité.
    /// </summary>
    [Serializable]
    public class NeighboursWeights
    {
        #region Propriétés

        /// <summary>
        ///Les poids pour la face droite
        /// </summary>
        [field: SerializeField]
        public float[] RightWeights { get; set; } = new float[0];

        /// <summary>
        ///Les poids pour la face gauche
        /// </summary>
        [field: SerializeField]
        public float[] LeftWeights { get; set; } = new float[0];

        /// <summary>
        ///Les poids pour la face au-dessus
        /// </summary>
        [field: SerializeField]
        public float[] UpWeights { get; set; } = new float[0];

        /// <summary>
        ///Les poids pour la face en-dessous
        /// </summary>
        [field: SerializeField]
        public float[] DownWeights { get; set; } = new float[0];

        /// <summary>
        ///Les poids pour la face avant
        /// </summary>
        [field: SerializeField]
        public float[] ForwardWeights { get; set; } = new float[0];

        /// <summary>
        ///Les poids pour la face arrière
        /// </summary>
        [field: SerializeField]
        public float[] BackWeights { get; set; } = new float[0];

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="weights"> Les poids des voisins valides pour chaque face d'un module, de 0% à 100%</param>
        public NeighboursWeights(NeighboursWeights weights)
        {
            if (weights.RightWeights != null)
            {
                RightWeights = new float[weights.RightWeights.Length];
                Array.Copy(weights.RightWeights, RightWeights, weights.RightWeights.Length);
            }

            if (weights.LeftWeights != null)
            {
                LeftWeights = new float[weights.LeftWeights.Length];
                Array.Copy(weights.LeftWeights, LeftWeights, weights.LeftWeights.Length);
            }

            if (weights.UpWeights != null)
            {
                UpWeights = new float[weights.UpWeights.Length];
                Array.Copy(weights.UpWeights, UpWeights, weights.UpWeights.Length);
            }

            if (weights.DownWeights != null)
            {
                DownWeights = new float[weights.DownWeights.Length];
                Array.Copy(weights.DownWeights, DownWeights, weights.DownWeights.Length);
            }

            if (weights.ForwardWeights != null)
            {
                ForwardWeights = new float[weights.ForwardWeights.Length];
                Array.Copy(weights.ForwardWeights, ForwardWeights, weights.ForwardWeights.Length);
            }

            if (weights.BackWeights != null)
            {
                BackWeights = new float[weights.BackWeights.Length];
                Array.Copy(weights.BackWeights, BackWeights, weights.BackWeights.Length);
            }
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Indique si les deux objets sont identiques
        /// </summary>
        /// <param name="other">Le comparé</param>
        /// <returns>TRUE si les deux objets sont identiques</returns>
        public bool Equals(NeighboursWeights other)
        {
            if (RightWeights.Length != other.RightWeights.Length ||
                LeftWeights.Length != other.LeftWeights.Length ||
                UpWeights.Length != other.UpWeights.Length ||
                DownWeights.Length != other.DownWeights.Length ||
                ForwardWeights.Length != other.ForwardWeights.Length ||
                BackWeights.Length != other.BackWeights.Length)
            {
                return false;
            }

            for (int i = 0; i < RightWeights.Length; ++i)
            {
                if (RightWeights[i] != other.RightWeights[i])
                {
                    return false;
                }
            }

            for (int i = 0; i < LeftWeights.Length; ++i)
            {
                if (LeftWeights[i] != other.LeftWeights[i])
                {
                    return false;
                }
            }

            for (int i = 0; i < UpWeights.Length; ++i)
            {
                if (UpWeights[i] != other.UpWeights[i])
                {
                    return false;
                }
            }

            for (int i = 0; i < DownWeights.Length; ++i)
            {
                if (DownWeights[i] != other.DownWeights[i])
                {
                    return false;
                }
            }

            for (int i = 0; i < ForwardWeights.Length; ++i)
            {
                if (ForwardWeights[i] != other.ForwardWeights[i])
                {
                    return false;
                }
            }

            for (int i = 0; i < BackWeights.Length; ++i)
            {
                if (BackWeights[i] != other.BackWeights[i])
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}