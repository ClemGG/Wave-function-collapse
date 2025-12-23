using UnityEngine;

namespace Assets.Scripts.Models.WFC
{
    /// <summary>
    /// Représente une palette de différentes cases 
    /// permettant de représenter différents environnements
    /// </summary>
    [CreateAssetMenu(fileName = "Tile Palette", menuName = "Scriptable Objects/WFC/Tile Palette")]
    public class TilePalette : ScriptableObject
    {
        /// <summary>
        /// La liste des cases instantiables fixes
        /// qui servent de point d'ancrage pour la suite de la génération
        /// et doivent tjs être instanciées
        /// </summary>
        [field: SerializeField]
        public FixedTile[] FixedTiles { get; private set; }

        /// <summary>
        /// La liste des cases instantiables servant de salles préconstruites
        /// </summary>
        [field: SerializeField]
        public Tile[] RoomTiles { get; private set; }

        /// <summary>
        /// La liste des cases instantiables servant de connexions entre salles
        /// </summary>
        [field: SerializeField]
        public Tile[] ConnectionTiles { get; private set; }
    }
}