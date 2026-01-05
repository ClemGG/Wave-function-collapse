using Assets.Scripts.Models.WFC.SOs;
using Assets.Scripts.ViewModels.WFC;
using UnityEngine;

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

        #endregion

        #region Variables d'instance

        #endregion

        #region Méthodes Unity

        #endregion

        #region Méthodes privées

        #endregion
    }
}