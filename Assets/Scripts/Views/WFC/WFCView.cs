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
        /// La liste de toutes les options possibles pour une case
        /// </summary>
        private List<ITileOption> _possibleTiles;

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
            _possibleTiles = new List<ITileOption>();
            _possibleTiles.AddRange(_tilePalette.FixedRooms);
            _possibleTiles.AddRange(_tilePalette.Tiles);
            GetRandomSizeAndNbRooms(ref rand, out int3 randDimensions, out int randNbMaxRooms);
            _cells = CreateCells(_possibleTiles, randDimensions);

            try
            {
                WFCAlg.Generate(_cells, _tilePalette, randDimensions, randNbMaxRooms, ref rand);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
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

        /// <summary>
        /// Crée une nouvelle grille de cellules
        /// </summary>
        /// <param name="possibleTiles">La liste des possibilités de chaque cellule</param>
        /// <param name="gridSize">Les dimensions de la grille</param>
        /// <returns>La liste des cellules composant la grille</returns>
        private List<Cell> CreateCells(List<ITileOption> possibleTiles, int3 gridSize)
        {
            List<Cell> cells = new(gridSize.x * gridSize.y * gridSize.z);

            for (int x = 0; x < gridSize.x; ++x)
            {
                for (int y = 0; y < gridSize.y; ++y)
                {
                    for (int z = 0; z < gridSize.z; ++z)
                    {
                        cells.Add(new Cell(new List<ITileOption>(possibleTiles), new int3(x, y, z)));
                    }
                }
            }

            return cells;
        }

        #endregion
    }
}