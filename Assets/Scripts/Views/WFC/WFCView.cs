using System.Collections.Generic;
using Assets.Scripts.Models.WFC;
using Assets.Scripts.Models.WFC.SOs;
using Assets.Scripts.ViewModels.WFC;
using UnityEngine;
using random = Unity.Mathematics.Random;

namespace Assets.Scripts.Views.WFC
{
    /// <summary>
    /// L'interface de l'algorithme WFC
    /// </summary>
    public class WFCView : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// La logique
        /// </summary>
        [field: SerializeField]
        private WFCViewModel _viewModel { get; set; }

        /// <summary>
        /// La palette contenant les modules à instancier
        /// </summary>
        [field: SerializeField]
        private ModulePaletteSO _palette { get; set; }

        /// <summary>
        /// Les paramètres de la grille
        /// </summary>
        [field: SerializeField]
        private GridSettingsSO _gridSettings { get; set; }

        /// <summary>
        /// L'ID de génération
        /// </summary>
        [field: SerializeField]
        private uint _seed { get; set; }

        #endregion

        #region Variables d'instance

        /// <summary>
        /// Générateur d'aléatoire
        /// </summary>
        private random _random;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Appelée quand l'inspecteur change
        /// </summary>
        private void OnValidate()
        {
            uint seed = _seed != 0 ? _seed : (uint)UnityEngine.Random.Range(0, uint.MaxValue);
            _random = new random(seed);
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Génère un nouveau niveau
        /// </summary>
        [ContextMenu("Clear Meshes")]
        private void ClearMeshes()
        {
            while (transform.childCount > 0)
            {
                DestroyImmediate(transform.GetChild(0).gameObject);
            }
        }

        /// <summary>
        /// Génère un nouveau niveau
        /// </summary>
        [ContextMenu("Generate")]
        private void Generate()
        {
            try
            {
                _viewModel.Generate(_palette, _gridSettings, ref _random, out Cell[] gridCells, out List<Prototype> prototypes);
                ClearMeshes();
                Render(gridCells, prototypes, transform, _gridSettings.CellSize);
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                Generate();
            }
        }

        /// <summary>
        /// Affiche les modèles contenus dans chaque cellule
        /// </summary>
        /// <param name="gridCells"></param>
        /// <param name="parent">Le parent des instances</param>
        /// <param name="prototypes">Les prototypes de chaque module</param>
        /// <param name="cellSize">La taille d'une cellule dans la scène</param>
        private void Render(Cell[] gridCells, List<Prototype> prototypes, Transform parent, int cellSize)
        {
            foreach (Cell cell in gridCells)
            {
                Vector3 pos = new(cell.Coords.x * cellSize, cell.Coords.y * cellSize, cell.Coords.z * cellSize);
                Prototype prototype = prototypes[cell.Options[0]];

                Instantiate(prototype.Prefab, pos, Quaternion.Euler(0f, 90f * prototype.Rotation, 0f), transform);
            }
        }

        #endregion
    }
}