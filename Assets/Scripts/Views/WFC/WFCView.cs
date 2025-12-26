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

        private void OnDrawGizmosSelected()
        {
            if (_cells == null)
            {
                return;
            }

            foreach (Cell cell in _cells)
            {
                if (cell.Options.Count > 1)
                {
                    Gizmos.color = Color.yellow;
                    int3 v = cell.Range[0];
                    Gizmos.DrawWireCube(new Vector3(v.x, v.y, v.z), Vector3.one);
                }
                else
                {
                    Gizmos.color = Color.red;
                    int3 size = (cell.Options[0] as FixedRoom).Size;
                    int3 avg = 0;
                    for (int i = 0; i < cell.Range.Length; ++i)
                    {
                        avg += cell.Range[i];
                    }

                    avg = avg / cell.Range.Length;
                    Gizmos.DrawCube(new Vector3(avg.x, avg.y, avg.z), new Vector3(size.x, size.y, size.z));
                }
            }
        }

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Génère un nouveau niveau
        /// </summary>
        [ContextMenu("Generate")]
        public void Generate()
        {
            GetRandomSizeAndNbRooms(ref rand, out int3 gridSize, out int nbMaxRooms);
            _cells = CreateCells(_tilePalette.Tiles, gridSize);
            SetNeighbours(_cells, gridSize);

            try
            {
                WFCAlg.Generate(_cells, _tilePalette, gridSize, nbMaxRooms, ref rand);
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
        /// <param name="gridSize">Les dimensions du niveau à créer</param>
        /// <param name="nbMaxRooms">Le nb max de salles à créer</param>
        private void GetRandomSizeAndNbRooms(ref random rand, out int3 gridSize, out int nbMaxRooms)
        {
            gridSize = new(rand.NextInt3(_gridSettings.MinSize, _gridSettings.MaxSize));
            nbMaxRooms = rand.NextInt(_gridSettings.MinMaxNbRooms.x, _gridSettings.MinMaxNbRooms.y);
        }

        /// <summary>
        /// Crée une nouvelle grille de cellules
        /// </summary>
        /// <param name="possibleTiles">La liste des possibilités de chaque cellule</param>
        /// <param name="gridSize">Les dimensions de la grille</param>
        /// <returns>La liste des cellules composant la grille</returns>
        private List<Cell> CreateCells(Tile[] possibleTiles, int3 gridSize)
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

        /// <summary>
        /// Assigne les voisins de chaque cellule
        /// </summary>
        /// <param name="cells">La liste des cellules de la grille</param>
        /// <param name="gridSize">Les dimensions de la grille</param>
        private void SetNeighbours(List<Cell> cells, int3 gridSize)
        {
            for (int i = 0; i < cells.Count; ++i)
            {
                Cell c = cells[i];
                int3 coords = c.Range[0];

                // Droite

                if (coords.x + 1 < gridSize.x)
                {
                    c.RightNeighbours.Add(new Range(coords + new int3(1, 0, 0)));
                }

                // Gauche

                if (coords.x - 1 > 0)
                {
                    c.LeftNeighbours.Add(new Range(coords + new int3(-1, 0, 0)));
                }

                // Haut

                if (coords.y + 1 < gridSize.y)
                {
                    c.UpNeighbours.Add(new Range(coords + new int3(0, 1, 0)));
                }

                // Bas

                if (coords.y - 1 > 0)
                {
                    c.DownNeighbours.Add(new Range(coords + new int3(0, -1, 0)));
                }

                // Devant

                if (coords.z + 1 < gridSize.z)
                {
                    c.ForwardNeighbours.Add(new Range(coords + new int3(0, 0, 1)));
                }

                if (coords.z - 1 > 0)
                {
                    c.BackNeighbours.Add(new Range(coords + new int3(0, 0, -1)));
                }

                cells[i] = c;
            }


        }

        #endregion
    }
}