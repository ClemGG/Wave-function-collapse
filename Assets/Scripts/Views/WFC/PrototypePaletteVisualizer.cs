using System;
using System.Collections.Generic;
using Assets.Scripts.Models.WFC;
using Assets.Scripts.Models.WFC.SOs;
using Assets.Scripts.ViewModels.WFC;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Views.WFC
{
    /// <summary>
    /// Affiche les prototypes de la palette renseignée
    /// ainsi que les valeurs de leurs ports
    /// </summary>
    [ExecuteAlways]
    public class PrototypeVisualizer : MonoBehaviour
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
        /// Les positions des instances créées
        /// </summary>
        Vector3[] _instancesPositions;

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

                    if (_palette != null)
                    {
                        CreatePrototypes();
                    }
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
            if (!_showGizmos || _instances == null || _palette == null)
            {
                return;
            }

            Gizmos.color = _moduleBoundsColor;

            for (int i = 0; i < _instancesPositions.Length; ++i)
            {
                string moduleName = _modulesNames[i];

                Gizmos.DrawWireCube(_instancesPositions[i], Vector3.one * _moduleSize);
                Handles.Label(_instancesPositions[i] + _moduleSize * Vector3.up, moduleName, EditorStyles.centeredGreyMiniLabel);
            }

            Gizmos.color = _socketLabelColor;

            for (int i = 0; i < _instancesPositions.Length; ++i)
            {
                string[] sockets = _socketsNames[i];

                Handles.Label(_instancesPositions[i] + (_moduleSize / 2f + _labelsDst) * Vector3.right, sockets[0]);
                Handles.Label(_instancesPositions[i] + (_moduleSize / 2f + _labelsDst) * Vector3.left, sockets[1]);
                Handles.Label(_instancesPositions[i] + (_moduleSize / 2f + _labelsDst) * Vector3.up, sockets[2]);
                Handles.Label(_instancesPositions[i] + (_moduleSize / 2f + _labelsDst) * Vector3.down, sockets[3]);
                Handles.Label(_instancesPositions[i] + (_moduleSize / 2f + _labelsDst) * Vector3.forward, sockets[4]);
                Handles.Label(_instancesPositions[i] + (_moduleSize / 2f + _labelsDst) * Vector3.back, sockets[5]);
            }
        }

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Instancie les modules
        /// </summary>
        private void CreatePrototypes()
        {
            if (_palette == null)
            {
                return;
            }

            List<Prototype> prototypes = _viewModel.CreatePrototypes(_palette.Modules);

            _instances = new GameObject[prototypes.Count];
            _instancesPositions = new Vector3[prototypes.Count];
            _modulesNames = new string[prototypes.Count];
            _socketsNames = new string[prototypes.Count][];

            int count = 0;
            int z = 0;
            GameObject previousGameObject = prototypes.Count > 0 ? prototypes[0].Prefab : null;

            while (count < prototypes.Count)
            {
                for (int i = 0; i < 4; ++i)
                {
                    if (count == prototypes.Count)
                    {
                        break;
                    }

                    if (previousGameObject != prototypes[count].Prefab)
                    {
                        previousGameObject = prototypes[count].Prefab;
                        break;
                    }

                    _instancesPositions[count] = new Vector3(_moduleSize * i + _spacing * i, 0f, _moduleSize * z + _spacing * z);

                    ++count;
                }

                ++z;
            }

            // Idéalement pour que les instances ne persistent pas quand on quitte la scène,
            // il faudrait les marquer pour indiquer à l'éditeur de ne pas les sauvegarder avec la scène.
            // j'ai la flemme de le faire, donc il faudra penser à retirer la palette quand on a terminé.

            for (int i = 0; i < prototypes.Count; ++i)
            {
                Prototype prototype = prototypes[i];

                _modulesNames[i] = prototype.Prefab.name.Replace(" Model", string.Empty);
                _instances[i] = Instantiate(prototype.Prefab, _instancesPositions[i], Quaternion.Euler(0f, 90f * prototype.Rotation, 0f), transform);
                _instances[i].name = _modulesNames[i];
                _socketsNames[i] = new string[prototype.Sockets.Length];
                Array.Copy(prototype.Sockets, _socketsNames[i], prototype.Sockets.Length);
            }
        }

        #endregion
    }
}