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
        [ContextMenu("Generate")]
        private void Generate()
        {
            _viewModel.Generate(_palette, _gridSettings, ref _random);
        }

        #endregion
    }
}