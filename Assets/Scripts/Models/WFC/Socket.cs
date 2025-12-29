using System;

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
    public class Socket
    {
        /// <summary>
        /// Les IDs des options possibles pour ce port
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Les IDs des options possibles pour ce port")]
        public SocketOption[] Options { get; private set; }
    }
}