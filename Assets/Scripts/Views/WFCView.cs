using System.Collections.Generic;
using Assets.Scripts.Models.WFC;
using Assets.Scripts.Models.WFC.SOs;
using UnityEngine;

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

    #endregion

    #region Variables d'instance

    /// <summary>
    /// Les celles comprenant la grille
    /// </summary>
    private List<Cell> _cells;

    #endregion

    #region Méthodes publiques

    /// <summary>
    /// Génère un nouveau niveau
    /// </summary>
    [ContextMenu("Generate")]
    public void Generate()
    {

    }

    #endregion
}
