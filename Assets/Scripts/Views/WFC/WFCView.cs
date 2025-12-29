using System.Collections.Generic;
using Assets.Scripts.Models.WFC;
using Assets.Scripts.Models.WFC.SOs;
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

        [field: Header("Paramètres :")]
        [field: Space(10)]

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

        [field: Space(10)]
        [field: Header("Gizmos :")]
        [field: Space(10)]

        /// <summary>
        /// TRUE pour afficher les gizmos
        /// </summary>
        [field: SerializeField]
        private bool _drawGizmos { get; set; } = true;

        /// <summary>
        /// TRUE pour afficher les cellules
        /// </summary>
        [field: SerializeField]
        private bool _drawCells { get; set; } = true;

        /// <summary>
        /// TRUE pour afficher les salles
        /// </summary>
        [field: SerializeField]
        private bool _drawRooms { get; set; } = true;

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

        /// <summary>
        /// Affiche des indicateurs dans la scène
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (!_drawGizmos || _cells == null)
            {
                return;
            }

            //Gizmos.color = Color.yellow;

            if (_drawCells)
            {
                foreach (Cell cell in _cells)
                {
                    Gizmos.color = Color.Lerp(Color.yellow, Color.white, (float)cell.Options.Count / _tilePalette.Tiles.Length);
                    int3 v = cell.Range[0];
                    Gizmos.DrawWireCube(new Vector3(v.x, v.y, v.z), Vector3.one);
                }
            }

            if (_drawRooms)
            {
                foreach (Cell cell in _cells)
                {
                    switch (cell.SelectedOption)
                    {
                        case Tile:
                            Gizmos.color = Color.green;
                            int3 v = cell.Range[0];
                            Gizmos.DrawCube(new Vector3(v.x, v.y, v.z), Vector3.one);
                            break;

                        case FixedRoom fr:
                            Gizmos.color = Color.red;
                            int3 size = fr.Size;
                            int3 avg = cell.Range.Centroid;
                            Gizmos.DrawCube(new Vector3(avg.x, avg.y, avg.z), new Vector3(size.x, size.y, size.z));
                            break;

                        default:
                            break;

                    }
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
            try
            {
                _cells = WFCAlg.Generate(_tilePalette, _gridSettings, ref rand);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);

                Clear();
            }
        }

        /// <summary>
        /// Génère un nouveau niveau
        /// </summary>
        [ContextMenu("Clear Previous Results")]
        public void Clear()
        {
            _cells?.Clear();
        }

        #endregion
    }
}