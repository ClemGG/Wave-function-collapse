using System;
using Assets.Scripts.Models.WFC.SOs;
using UnityEngine;

namespace Assets.Scripts.Models.WFC
{
    /// <summary>
    /// Les listes des voisins valides pour chaque face d'un module
    /// </summary>
    [Serializable]
    public class ValidNeighbours
    {
        #region Propriétés

        /// <summary>
        ///Les voisins valides pour la face droite
        /// </summary>
        [field: SerializeField]
        public ModuleSO[] RightNeighbours { get; set; }

        /// <summary>
        ///Les voisins valides pour la face gauche
        /// </summary>
        [field: SerializeField]
        public ModuleSO[] LeftNeighbours { get; set; }

        /// <summary>
        ///Les voisins valides pour la face au-dessus
        /// </summary>
        [field: SerializeField]
        public ModuleSO[] UpNeighbours { get; set; }

        /// <summary>
        ///Les voisins valides pour la face en-dessous
        /// </summary>
        [field: SerializeField]
        public ModuleSO[] DownNeighbours { get; set; }

        /// <summary>
        ///Les voisins valides pour la face avant
        /// </summary>
        [field: SerializeField]
        public ModuleSO[] ForwardNeighbours { get; set; }

        /// <summary>
        ///Les voisins valides pour la face arrière
        /// </summary>
        [field: SerializeField]
        public ModuleSO[] BackNeighbours { get; set; }

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="validNeighbours">La liste des voisins valides</param>
        public ValidNeighbours(ValidNeighbours validNeighbours)
        {
            RightNeighbours = new ModuleSO[validNeighbours.RightNeighbours.Length];
            LeftNeighbours = new ModuleSO[validNeighbours.LeftNeighbours.Length];
            UpNeighbours = new ModuleSO[validNeighbours.UpNeighbours.Length];
            DownNeighbours = new ModuleSO[validNeighbours.DownNeighbours.Length];
            ForwardNeighbours = new ModuleSO[validNeighbours.ForwardNeighbours.Length];
            BackNeighbours = new ModuleSO[validNeighbours.BackNeighbours.Length];

            Array.Copy(validNeighbours.RightNeighbours, RightNeighbours, validNeighbours.RightNeighbours.Length);
            Array.Copy(validNeighbours.LeftNeighbours, LeftNeighbours, validNeighbours.LeftNeighbours.Length);
            Array.Copy(validNeighbours.UpNeighbours, UpNeighbours, validNeighbours.UpNeighbours.Length);
            Array.Copy(validNeighbours.DownNeighbours, DownNeighbours, validNeighbours.DownNeighbours.Length);
            Array.Copy(validNeighbours.ForwardNeighbours, ForwardNeighbours, validNeighbours.ForwardNeighbours.Length);
            Array.Copy(validNeighbours.BackNeighbours, BackNeighbours, validNeighbours.BackNeighbours.Length);
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Indique si les deux objets sont identiques
        /// </summary>
        /// <param name="other">Le comparé</param>
        /// <returns>TRUE si les deux objets sont identiques</returns>
        public bool Equals(ValidNeighbours other)
        {
            if (RightNeighbours.Length != other.RightNeighbours.Length ||
                LeftNeighbours.Length != other.LeftNeighbours.Length ||
                UpNeighbours.Length != other.UpNeighbours.Length ||
                DownNeighbours.Length != other.DownNeighbours.Length ||
                ForwardNeighbours.Length != other.ForwardNeighbours.Length ||
                BackNeighbours.Length != other.BackNeighbours.Length)
            {
                return false;
            }

            for (int i = 0; i < RightNeighbours.Length; ++i)
            {
                if (RightNeighbours[i].name != other.RightNeighbours[i].name)
                {
                    return false;
                }
            }

            for (int i = 0; i < LeftNeighbours.Length; ++i)
            {
                if (LeftNeighbours[i].name != other.LeftNeighbours[i].name)
                {
                    return false;
                }
            }

            for (int i = 0; i < UpNeighbours.Length; ++i)
            {
                if (UpNeighbours[i].name != other.UpNeighbours[i].name)
                {
                    return false;
                }
            }

            for (int i = 0; i < DownNeighbours.Length; ++i)
            {
                if (DownNeighbours[i].name != other.DownNeighbours[i].name)
                {
                    return false;
                }
            }

            for (int i = 0; i < ForwardNeighbours.Length; ++i)
            {
                if (ForwardNeighbours[i].name != other.ForwardNeighbours[i].name)
                {
                    return false;
                }
            }

            for (int i = 0; i < BackNeighbours.Length; ++i)
            {
                if (BackNeighbours[i].name != other.BackNeighbours[i].name)
                {
                    return false;
                }
            }

            return true;
        }

        #endregion
    }
}