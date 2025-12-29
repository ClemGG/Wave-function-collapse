using UnityEngine;

namespace Assets.Scripts.Models.WFC.SOs
{
    /// <summary>
    /// Représente les moodèles pouvant être utilisés comme options de cellule
    /// </summary>
    public interface ITileOption
    {
        /// <summary>
        /// La prefab de cette case
        /// </summary>
        public GameObject TilePrefab { get; set; }
    }
}