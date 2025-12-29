using System;
using UnityEngine;

/// <summary>
/// Représente une option pour un port d'une case
/// </summary>
[Serializable]
public class SocketOption
{
    /// <summary>
    /// L'ID de l'option
    /// </summary>
    [field: SerializeField]
    [field: Tooltip("L'ID de l'option")]
    public string ID { get; private set; }

    /// <summary>
    /// La probabilité de l'ID d'être sélectionné pour la génération, de 0 à 100.
    /// Laisser ce nombre à 0 pour toutes les options si toutes ont la même chance d'être sélectionné.
    /// </summary>
    [field: SerializeField]
    [field: Tooltip("La probabilité de l'ID d'être sélectionné pour la génération, de 0 à 100. Laisser ce nombre à 0 pour toutes les options si toutes ont la même chance d'être sélectionné.")]
    public float Weight { get; private set; }
}
