using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;

namespace Assets.Scripts.Models.WFC
{
    /// <summary>
    /// Les coordonnées d'une cellule.
    /// Puisque les cellules peuvent être fusionnées,
    /// on enregistre une plage de coordonnées au lieu d'une seule cellule
    /// </summary>
    public readonly struct Range
    {
        #region Propriétés

        /// <summary>
        /// Le nombre de coordonnés comprises dans cette plage
        /// </summary>
        public readonly int Length => Value.Length;

        /// <summary>
        /// La moyenne des points enregistrés, formant le centre de la plage
        /// </summary>
        public readonly int3 Centroid
        {
            get
            {
                int3 avg = 0;

                for (int i = 0; i < Length; ++i)
                {
                    avg += Value[i];
                }

                avg /= Length;

                return avg;
            }
        }

        /// <summary>
        /// Les coordonnées de la 1è cellule
        /// </summary>
        public readonly int3[] Value { get; }

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="coords">Les coordonnées de la cellule</param>
        public Range(int3 coords)
        {
            Value = new int3[1] { coords };
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="range">La plage de cellules</param>
        public Range(List<int3> range)
        {
            Value = new int3[range.Count];
            range.CopyTo(Value);
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="range">La plage de cellules</param>
        public Range(params int3[] range)
        {
            Value = new int3[range.Length];
            Array.Copy(range, Value, range.Length);
        }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="range">La plage de cellules</param>
        public Range(Range range) : this(range.Value) { }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Récupère la coordonnée à l'index renseigné dans la plage
        /// </summary>
        /// <param name="index">L'index renseigné</param>
        /// <returns>La coordonnée à l'index renseigné dans la plage</returns>
        public readonly int3 this[int index]
        {
            get
            {
                return Value[index];
            }
            set
            {
                Value[index] = value;
            }
        }

        /// <summary>
        /// Indique si les coordonnées renseignées sont comprises dans la plage
        /// </summary>
        /// <param name="coords">Les coordonnées renseignées</param>
        /// <returns>TRUE si les coordonnées renseignées sont comprises dans la plage</returns>
        public readonly bool Contains(int3 coords)
        {
            return Value.Contains(coords);
        }

        /// <summary>
        /// Indique si les coordonnées renseignées sont comprises dans la plage
        /// </summary>
        /// <param name="other">Les coordonnées renseignées</param>
        /// <returns>TRUE si les coordonnées renseignées sont comprises dans la plage</returns>
        public readonly bool Contains(Range other)
        {
            Func<int3, bool> contains = Contains;
            return Array.TrueForAll(other.Value, coord => contains(coord));
        }

        #endregion
    }
}