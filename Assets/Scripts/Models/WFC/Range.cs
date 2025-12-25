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
        public readonly int3 Count
        {
            get
            {
                int count = 0;

                for (int x = Start.x; x < End.x; ++x)
                {
                    for (int y = Start.y; y < End.y; ++y)
                    {
                        for (int z = Start.z; z < End.z; ++z)
                        {
                            ++count;
                        }
                    }
                }

                return count;
            }
        }

        /// <summary>
        /// Les coordonnées de la 1è cellule
        /// </summary>
        public readonly int3 Start { get; }

        /// <summary>
        /// Les coordonnées de la dernière cellule
        /// </summary>
        public readonly int3 End { get; }

        #endregion

        #region Constructeur

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="coords">Les coordonnées de la cellule</param>
        public Range(int3 coords) : this(coords, coords) { }

        /// <summary>
        /// Constructeur
        /// </summary>
        /// <param name="start">Les coordonnées de la 1è cellule</param>
        /// <param name="end">Les coordonnées de la dernière cellule</param>
        public Range(int3 start, int3 end)
        {
            Start = start;
            End = end;
        }

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
                if (index < 0)
                {
                    throw new System.ArgumentOutOfRangeException($"Erreur : L'index renseigné dépasse la plage de coordonnées ({index}).");
                }

                int count = 0;

                for (int x = Start.x; x < End.x; ++x)
                {
                    for (int y = Start.y; y < End.y; ++y)
                    {
                        for (int z = Start.z; z < End.z; ++z)
                        {
                            if (count == index)
                            {
                                return new int3(x, y, z);
                            }

                            ++count;
                        }
                    }
                }

                throw new System.ArgumentOutOfRangeException($"Erreur : L'index renseigné dépasse la plage de coordonnées (Index : {index} ; Count : {count}).");
            }
        }

        /// <summary>
        /// Indique si les coordonnées renseignées sont comprises dans la plage
        /// </summary>
        /// <param name="coords">Les coordonnées renseignées</param>
        /// <returns>TRUE si les coordonnées renseignées sont comprises dans la plage</returns>
        public readonly bool Contains(int3 coords)
        {
            bool3 s = Start <= coords;
            bool3 e = coords <= End;
            bool biggerThanStart = s.x && s.y && s.z;
            bool smallerThanEnd = e.x && e.y && e.z;

            return biggerThanStart && smallerThanEnd;
        }

        /// <summary>
        /// Indique si les coordonnées renseignées sont comprises dans la plage
        /// </summary>
        /// <param name="other">Les coordonnées renseignées</param>
        /// <returns>TRUE si les coordonnées renseignées sont comprises dans la plage</returns>
        public readonly bool Contains(Range other)
        {
            return Contains(other.Start) && Contains(other.End);
        }

        #endregion
    }
}