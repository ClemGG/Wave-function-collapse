using System.Collections.Generic;
using Assets.Scripts.Models.WFC;
using Unity.Mathematics;
using UnityEngine;
using random = Unity.Mathematics.Random;
using WFCAlg = Assets.Scripts.ViewModels.WFC.WaveFunctionCollapseAlgorithm;

namespace Assets.Scripts.Views.WFC
{
    /// <summary>
    /// Interface pour utiliser l'algorithme du Wave Function Collapse
    /// </summary>
    public class WFCView : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// La palette de cases à instancier
        /// </summary>
        [field: SerializeField]
        private TilePalette _tilePalette { get; set; }

        /// <summary>
        /// Les paramètres de la grille
        /// </summary>
        [field: SerializeField]
        private GridSettings _gridSettings { get; set; }

        /// <summary>
        /// La graine de génération
        /// </summary>
        [field: SerializeField]
        private uint _seed { get; set; }

        #endregion

        #region Variables d'instance

        /// <summary>
        /// Les celles comprenant la grille
        /// </summary>
        private List<Cell> _cells;

        /// <summary>
        /// Les cases possibles dans une cellule donnée
        /// au début de la génération
        /// </summary>
        private List<Tile> _possibleTiles;

        /// <summary>
        /// Les cases possibles dans une cellule donnée
        /// au début de la génération
        /// </summary>
        private random rand;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Appelée quand l'inspecteur change
        /// </summary>
        private void OnValidate()
        {
            uint seed = _seed;

            if (_seed == 0)
            {
                seed = (uint)UnityEngine.Random.Range(1, uint.MaxValue);
            }

            rand = new(seed);
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Génère un nouveau niveau
        /// </summary>
        [ContextMenu("Generate")]
        public void Generate()
        {
            _possibleTiles = new List<Tile>();
            _possibleTiles.AddRange(_tilePalette.RoomTiles);
            _possibleTiles.AddRange(_tilePalette.ConnectionTiles);
            GetRandomSizeAndNbRooms(ref rand, out int3 randDimensions, out int randNbMaxRooms);

            _cells = new List<Cell>(randDimensions.x * randDimensions.y * randDimensions.z);

            for (int x = 0; x < randDimensions.x; ++x)
            {
                for (int y = 0; y < randDimensions.y; ++y)
                {
                    for (int z = 0; z < randDimensions.z; ++z)
                    {
                        _cells.Add(new Cell(_possibleTiles, new int3(x, y, z)));
                    }
                }
            }

            WFCAlg.Generate(_cells, randDimensions, ref rand);
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Obtient une taille aléatoire ainsi qu'un nb aléatoire de salles pour le niveau
        /// </summary>
        /// <param name="rand">Le générateur d'aléatoire</param>
        /// <param name="randDimensions">Les dimensions du niveau à créer</param>
        /// <param name="randNbMaxRooms">Le nb max de salles à créer</param>
        private void GetRandomSizeAndNbRooms(ref random rand, out int3 randDimensions, out int randNbMaxRooms)
        {
            randDimensions = new(rand.NextInt3(_gridSettings.MinSize, _gridSettings.MaxSize));
            randNbMaxRooms = rand.NextInt(_gridSettings.MinMaxNbRooms.x, _gridSettings.MinMaxNbRooms.y);
        }

        #endregion
    }
}