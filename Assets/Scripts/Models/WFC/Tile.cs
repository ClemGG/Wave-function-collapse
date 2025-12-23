using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Models.WFC
{
    /// <summary>
    /// Représente le modèle et sa rotation
    /// ainsi que sa liste de voisins possibles
    /// </summary>
    [CreateAssetMenu(fileName = "Tile", menuName = "Scriptable Objects/WFC/Tile")]
    public class Tile : ScriptableObject
    {
        /// <summary>
        /// La prefab de cette case
        /// </summary>
        [field: SerializeField]
        public GameObject TilePrefab { get; private set; }

        /// <summary>
        /// La taille que prend cette case sur la grille
        /// </summary>
        [field: SerializeField]
        public int3 Size { get; private set; }

        /// <summary>
        /// Les IDs des voisins possibles à droite de cette case
        /// </summary>
        [field: SerializeField]
        public Socket[] RightNeighbours { get; private set; }

        /// <summary>
        /// Les IDs des voisins possibles à gauche de cette case
        /// </summary>
        [field: SerializeField]
        public Socket[] LeftNeighbours { get; private set; }

        /// <summary>
        /// Les IDs des voisins possibles au-dessus de cette case
        /// </summary>
        [field: SerializeField]
        public Socket[] UpNeighbours { get; private set; }

        /// <summary>
        /// Les IDs des voisins possibles au-dessus de cette case
        /// </summary>
        [field: SerializeField]
        public Socket[] DownNeighbours { get; private set; }

        /// <summary>
        /// Les IDs des voisins possibles devant de cette case
        /// </summary>
        [field: SerializeField]
        public Socket[] ForwardNeighbours { get; private set; }

        /// <summary>
        /// Les IDs des voisins possibles derrière de cette case
        /// </summary>
        [field: SerializeField]
        public Socket[] BackNeighbours { get; private set; }
    }
}