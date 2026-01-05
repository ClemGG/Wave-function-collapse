using UnityEngine;

namespace Assets.Scripts.Models.WFC.SOs
{
    /// <summary>
    /// Module contenant le modèle 3D et les ports de chaque face
    /// </summary>
    [CreateAssetMenu(fileName = "New Module", menuName = "Scriptable Objects/WFC/Module")]
    public class ModuleSO : ScriptableObject
    {
        /// <summary>
        /// Le modèle à instancier
        /// </summary>
        [field: SerializeField]
        public GameObject Prefab { get; private set; }

        /// <summary>
        /// Les ports de chaque face (X, -X, Y, -Y, Z, -Z)
        /// </summary>
        [field: SerializeField]
        public string[] Sockets { get; private set; } = new string[6];

        /// <summary>
        /// Les listes des voisins valides par face (X, -X, Y, -Y, Z, -Z)
        /// </summary>
        [field: SerializeField]
        public ValidNeighbours ValidNeighbours { get; private set; }

        /// <summary>
        /// Les poids des voisins valides pour chaque face d'un module, de 0% à 100%.
        /// Si une liste est vide, cela veut dire qu'il n'y a qu'1 seul voisin
        /// ou qu'ils ont tous la même probabilité.
        /// </summary>
        [field: SerializeField]
        public NeighboursWeights Weights { get; private set; }
    }
}