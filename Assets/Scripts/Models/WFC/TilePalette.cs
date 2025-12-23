using UnityEngine;

/// <summary>
/// Représente une palette de différentes cases 
/// permettant de représenter différents environnements
/// </summary>
[CreateAssetMenu(fileName = "Tile Palette", menuName = "Scriptable Objects/WFC/Tile Palette")]
public class TilePalette : ScriptableObject
{
    /// <summary>
    /// La case de départ. 
    /// On ne la met pas avec les autres
    /// car on ne veut pas l'instancier par accident.
    /// </summary>
    [field: SerializeField]
    public Tile StartTile { get; private set; }

    /// <summary>
    /// La liste des cases instantiables
    /// </summary>
    [field: SerializeField]
    public Tile[] Tiles { get; private set; }
}
