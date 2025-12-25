using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Models.WFC
{
    /// <summary>
    /// Représente une salle préconstruite placée avant la génération WFC,
    /// ainsi que sa liste de voisins possibles
    /// </summary>
    [CreateAssetMenu(fileName = "Fixed Room", menuName = "Scriptable Objects/WFC/Fixed Room")]
    public class FixedRoom : ScriptableObject, ITileOption
    {
        /// <summary>
        /// La prefab de cette case
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("La prefab de cette case")]
        public GameObject TilePrefab { get; set; }

        /// <summary>
        /// La taille que prend cette case dans la grille de cellules
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("La taille que prend cette case dans la grille de cellules")]
        public int3 Size { get; private set; } = new int3(1, 1, 1);

        /// <summary>
        /// Les IDs des voisins possibles à droite de cette case
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Les IDs des voisins possibles à droite de cette case")]
        public Socket[] RightNeighbours { get; private set; }

        /// <summary>
        /// Les IDs des voisins possibles à gauche de cette case
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Les IDs des voisins possibles à gauche de cette case")]
        public Socket[] LeftNeighbours { get; private set; }

        /// <summary>
        /// Les IDs des voisins possibles au-dessus de cette case
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Les IDs des voisins possibles au-dessus de cette case")]
        public Socket[] UpNeighbours { get; private set; }

        /// <summary>
        /// Les IDs des voisins possibles au-dessus de cette case
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Les IDs des voisins possibles au-dessus de cette case")]
        public Socket[] DownNeighbours { get; private set; }

        /// <summary>
        /// Les IDs des voisins possibles devant de cette case
        /// </summary>
        [field: Tooltip("Les IDs des voisins possibles devant de cette case")]
        [field: SerializeField]
        public Socket[] ForwardNeighbours { get; private set; }

        /// <summary>
        /// Les IDs des voisins possibles derrière de cette case
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Les IDs des voisins possibles derrière de cette case")]
        public Socket[] BackNeighbours { get; private set; }
    }
}