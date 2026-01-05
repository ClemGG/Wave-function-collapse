using Unity.Mathematics;
using UnityEngine;

namespace Assets.Scripts.Models.WFC.SOs
{
    /// <summary>
    /// Les paramètres de la grille
    /// </summary>
    [CreateAssetMenu(fileName = "New Grid Settings", menuName = "Scriptable Objects/WFC/Grid Settings")]
    public class GridSettingsSO : ScriptableObject
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
        /// La taille d'une cellule dans la scène
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("La taille d'une cellule dans la scène")]
        public int CellSize { get; private set; }
    }
}