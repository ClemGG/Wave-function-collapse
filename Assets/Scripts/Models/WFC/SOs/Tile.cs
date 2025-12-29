using UnityEngine;

namespace Assets.Scripts.Models.WFC.SOs
{
    /// <summary>
    /// Représente le modèle ainsi que sa liste d'options possibles
    /// </summary>
    [CreateAssetMenu(fileName = "Tile", menuName = "Scriptable Objects/WFC/Tile")]
    public class Tile : ScriptableObject, ITileOption
    {
        /// <summary>
        /// La prefab de cette case
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("La prefab de cette case")]
        public GameObject TilePrefab { get; set; }

        /// <summary>
        /// Les IDs des options possibles à droite de cette case
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Les IDs des options possibles à droite de cette case")]
        public Socket RightOptions { get; private set; }

        /// <summary>
        /// Les IDs des options possibles à gauche de cette case
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Les IDs des options possibles à gauche de cette case")]
        public Socket LeftOptions { get; private set; }

        /// <summary>
        /// Les IDs des options possibles au-dessus de cette case
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Les IDs des options possibles au-dessus de cette case")]
        public Socket UpOptions { get; private set; }

        /// <summary>
        /// Les IDs des options possibles au-dessus de cette case
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Les IDs des options possibles au-dessus de cette case")]
        public Socket DownOptions { get; private set; }

        /// <summary>
        /// Les IDs des options possibles devant de cette case
        /// </summary>
        [field: Tooltip("Les IDs des options possibles devant de cette case")]
        [field: SerializeField]
        public Socket ForwardOptions { get; private set; }

        /// <summary>
        /// Les IDs des options possibles derrière de cette case
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("Les IDs des options possibles derrière de cette case")]
        public Socket BackOptions { get; private set; }
    }
}