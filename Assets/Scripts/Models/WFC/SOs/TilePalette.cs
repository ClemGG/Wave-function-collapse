using UnityEngine;

namespace Assets.Scripts.Models.WFC.SOs
{
    /// <summary>
    /// Représente une palette de différentes cases 
    /// permettant de représenter différents environnements
    /// </summary>
    [CreateAssetMenu(fileName = "Tile Palette", menuName = "Scriptable Objects/WFC/Tile Palette")]
    public class TilePalette : ScriptableObject
    {
        /// <summary>
        /// La liste des salles préconstruites créées avant tout le reste de la génération.
        /// Elles ne sont pas incluses dans la liste des options possibles pour une cellule.
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("La liste des salles préconstruites créées avant tout le reste de la génération.\n\nElles ne sont pas incluses dans la liste des options possibles pour une cellule.")]
        public FixedRoom[] GuaranteedFixedRooms { get; private set; }

        /// <summary>
        /// La liste des salles préconstruites instantiables
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("La liste des salles préconstruites instantiables")]
        public FixedRoom[] FixedRooms { get; private set; }

        /// <summary>
        /// La liste des cases instantiables
        /// </summary>
        [field: SerializeField]
        [field: Tooltip("La liste des cases instantiables")]
        public Tile[] Tiles { get; private set; }
    }
}