using System;
using Assets.Scripts.Models.WFC.SOs;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Views.WFC
{
    /// <summary>
    /// Affiche les modules de la palette renseignée
    /// ainsi que les valeurs de ses ports
    /// </summary>
    [ExecuteAlways]
    public class ModulePaletteVisualizer : MonoBehaviour
    {
        #region Propriétés

        /// <summary>
        /// La palette contenant les modules à instancier
        /// </summary>
        [field: SerializeField]
        private ModulePaletteSO _palette { get; set; }

        /// <summary>
        /// La taille de chaque modèle
        /// </summary>
        [field: SerializeField]
        private float _moduleSize { get; set; }

        /// <summary>
        /// L'espacement entre chaque modèle
        /// </summary>
        [field: SerializeField]
        private float _spacing { get; set; }

        /// <summary>
        /// La distance des labels par rapport au modèle
        /// </summary>
        [field: SerializeField]
        private float _labelsDst { get; set; }

        /// <summary>
        /// TRUE pour afficher les gizmos
        /// </summary>
        [field: SerializeField]
        private bool _showGizmos { get; set; }

        /// <summary>
        /// La couleur des limites du mesh du module
        /// </summary>
        [field: SerializeField]
        private Color _moduleBoundsColor { get; set; }

        /// <summary>
        /// La couleur des labels des ports
        /// </summary>
        [field: SerializeField]
        private Color _socketLabelColor { get; set; }

        #endregion

        #region Variables d'instance

        /// <summary>
        /// Les instances créées
        /// </summary>
        GameObject[] _instances;

        /// <summary>
        /// Les noms des modules
        /// </summary>
        string[] _modulesNames;

        /// <summary>
        /// Les noms des ports de chaque module
        /// </summary>
        string[][] _socketsNames;

        /// <summary>
        /// La palette précédente
        /// </summary>
        private ModulePaletteSO _previousPalette;

        /// <summary>
        /// True pour màj la scène
        /// </summary>
        private bool _updateView;

        #endregion

        #region Méthodes Unity

        /// <summary>
        /// Màj à chaque frame
        /// </summary>
        private void Update()
        {
            if (_updateView)
            {
                _updateView = false;

                if (_previousPalette != _palette)
                {
                    _previousPalette = _palette;

                    if (_instances != null)
                    {
                        foreach (GameObject go in _instances)
                        {
                            DestroyImmediate(go);
                        }

                        _instances = null;
                    }

                    InstantiateModules();
                }
            }
        }

        /// <summary>
        /// Quand l'inspecteur change
        /// </summary>
        private void OnValidate()
        {
            // On ne peut pas détruire des objets dans onValidate,
            // donc on le délaie jusqu'à l'Update

            _updateView = true;
        }

        /// <summary>
        /// Affiche des gizmos dans la scène
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!_showGizmos ^ _instances == null)
            {
                return;
            }

            Gizmos.color = _moduleBoundsColor;

            for (int i = 0; i < _instances.Length; ++i)
            {
                GameObject go = _instances[i];
                string moduleName = _modulesNames[i];

                Gizmos.DrawWireCube(go.transform.position, Vector3.one * _moduleSize);
                Handles.Label(go.transform.position + _moduleSize * Vector3.up, moduleName, EditorStyles.centeredGreyMiniLabel);
            }

            Gizmos.color = _socketLabelColor;

            for (int i = 0; i < _instances.Length; ++i)
            {
                GameObject go = _instances[i];
                string[] sockets = _socketsNames[i];

                Handles.Label(go.transform.position + (_moduleSize / 2f + _labelsDst) * Vector3.right, sockets[0]);
                Handles.Label(go.transform.position + (_moduleSize / 2f + _labelsDst) * Vector3.left, sockets[1]);
                Handles.Label(go.transform.position + (_moduleSize / 2f + _labelsDst) * Vector3.up, sockets[2]);
                Handles.Label(go.transform.position + (_moduleSize / 2f + _labelsDst) * Vector3.down, sockets[3]);
                Handles.Label(go.transform.position + (_moduleSize / 2f + _labelsDst) * Vector3.forward, sockets[4]);
                Handles.Label(go.transform.position + (_moduleSize / 2f + _labelsDst) * Vector3.back, sockets[5]);
            }
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Instancie les modules
        /// </summary>
        private void InstantiateModules()
        {
            if (_palette == null)
            {
                return;
            }

            _instances = new GameObject[_palette.Modules.Length];
            _modulesNames = new string[_palette.Modules.Length];
            _socketsNames = new string[_palette.Modules.Length][];

            for (int i = 0; i < _palette.Modules.Length; ++i)
            {
                ModuleSO module = _palette.Modules[i];

                _modulesNames[i] = module.Prefab.name.Replace(" Model", string.Empty);
                _instances[i] = Instantiate(module.Prefab, new Vector3(_moduleSize * i + _spacing * i, 0f, 0f), Quaternion.identity);
                _socketsNames[i] = new string[module.Sockets.Length];
                Array.Copy(module.Sockets, _socketsNames[i], module.Sockets.Length);
            }
        }

        #endregion
    }
}