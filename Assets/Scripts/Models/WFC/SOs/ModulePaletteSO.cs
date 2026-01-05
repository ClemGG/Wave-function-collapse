using UnityEngine;

namespace Assets.Scripts.Models.WFC.SOs
{
    /// <summary>
    /// Représente une liste de modules pour un niveau donné.
    /// Permet d'avoir des modèles d'une esthétique précise pour chaque niveau.
    /// </summary>
    [CreateAssetMenu(fileName = "New Module Palette", menuName = "Scriptable Objects/WFC/Module Palette")]
    public class ModulePaletteSO : ScriptableObject
    {
        /// <summary>
        /// La liste des modules instanciables
        /// </summary>
        [field: SerializeField]
        public ModuleSO[] Modules { get; private set; }
    }
}