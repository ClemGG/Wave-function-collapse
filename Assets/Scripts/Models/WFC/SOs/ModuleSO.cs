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
        /// La probabilité du prototype d'être sélectioné lors de l'effondrement d'une cellule
        /// </summary>
        [field: SerializeField]
        public byte Weight { get; private set; } = 1;

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
    }
}