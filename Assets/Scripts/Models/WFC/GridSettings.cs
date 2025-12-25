using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Models.WFC
{
    /// <summary>
    /// Les paramètres de la grille
    /// </summary>
    [CreateAssetMenu(fileName = "Grid Settings", menuName = "Scriptable Objects/WFC/Grid Settings")]
    public class GridSettings : ScriptableObject
    {
        /// <summary>
        /// La taille minimale de la grille
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("La taille minimale de la grille")]
        public int3 MinSize { get; private set; }

        /// <summary>
        /// La taille maximale de la grille
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("La taille maximale de la grille")]
        public int3 MaxSize { get; private set; }

        /// <summary>
        /// L'intervalle du nombre de salles pouvant être créées
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("L'intervalle du nombre de salles pouvant être créées")]
        public int2 MinMaxNbRooms { get; private set; }
    }
}