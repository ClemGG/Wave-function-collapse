using System.Collections.Generic;
using Assets.Scripts.Models.WFC;
using Assets.Scripts.Models.WFC.SOs;
using UnityEngine;

namespace Assets.Scripts.ViewModels.WFC
{
    /// <summary>
    /// La logique de l'algorithme WFC
    /// </summary>
    public class WFCViewModel : MonoBehaviour
    {
        /// <summary>
        /// Crée une liste de prototypes à partir des modules renseignés
        /// </summary>
        /// <param name="modules">Les modules à instancier</param>
        /// <returns>Une liste de prototypes représentant les modules sous chaque rotation</returns>
        public List<Prototype> CreatePrototypes(ModuleSO[] modules)
        {
            List<Prototype> prototypes = new(modules.Length * 4);
            List<Prototype> temp = new(4);

            foreach (ModuleSO module in modules)
            {
                temp.Clear();
                CreatePrototypes(module, temp);
                prototypes.AddRange(temp);
            }

            return prototypes;
        }

        /// <summary>
        /// Crée les prototypes représentant chaque rotation du module renseigné
        /// </summary>
        /// <param name="module">Le module</param>
        /// <param name="temp">La liste des prototypes</param>
        /// <returns>Les prototypes représentant chaque rotation du module renseigné</returns>
        private void CreatePrototypes(ModuleSO module, List<Prototype> temp)
        {
            // Rotation 0

            Prototype p1 = new(module);
            p1.Rotation = 0;
            temp.Add(p1);

            // Rotation 1

            Prototype p2 = new(module);
            p2.Rotation = 1;

            // Right, Forward, Left, Back => Forward, Left, Back, Right
            (p2.Sockets[0], p2.Sockets[4], p2.Sockets[1], p2.Sockets[5]) = (p2.Sockets[4], p2.Sockets[1], p2.Sockets[5], p2.Sockets[0]);

            (p2.Weights.RightWeights, p2.Weights.ForwardWeights, p2.Weights.LeftWeights, p2.Weights.BackWeights) =
            (p2.Weights.ForwardWeights, p2.Weights.LeftWeights, p2.Weights.BackWeights, p2.Weights.RightWeights);

            (p2.ValidNeighbours.RightNeighbours, p2.ValidNeighbours.ForwardNeighbours, p2.ValidNeighbours.LeftNeighbours, p2.ValidNeighbours.BackNeighbours) =
            (p2.ValidNeighbours.ForwardNeighbours, p2.ValidNeighbours.LeftNeighbours, p2.ValidNeighbours.BackNeighbours, p2.ValidNeighbours.RightNeighbours);

            // Si les prototypes sont identiques après 1 rotation,
            // alors les 4 faces sont les mêmes ; pas besoin de rotation.
            // Si le voisin du dessus requiert une rotation,
            // on continue quand même

            if (module.Sockets[2][0] != 'v' && p1.Equals(p2))
            {
                return;
            }

            temp.Add(p2);

            // Rotation 2

            Prototype p3 = new(module);
            p3.Rotation = 2;

            // Right, Forward, Left, Back => Left, Back, Right, Forward 
            (p3.Sockets[0], p3.Sockets[4], p3.Sockets[1], p3.Sockets[5]) = (p3.Sockets[1], p3.Sockets[5], p3.Sockets[0], p3.Sockets[4]);

            (p3.Weights.RightWeights, p3.Weights.ForwardWeights, p3.Weights.LeftWeights, p3.Weights.BackWeights) =
            (p3.Weights.LeftWeights, p3.Weights.BackWeights, p3.Weights.RightWeights, p3.Weights.ForwardWeights);

            (p3.ValidNeighbours.RightNeighbours, p3.ValidNeighbours.ForwardNeighbours, p3.ValidNeighbours.LeftNeighbours, p3.ValidNeighbours.BackNeighbours) =
            (p3.ValidNeighbours.LeftNeighbours, p3.ValidNeighbours.BackNeighbours, p3.ValidNeighbours.RightNeighbours, p3.ValidNeighbours.ForwardNeighbours);

            // Si les 2 faces opposées sont identiques,
            // alors on n'a besoin que d'une seule rotation.
            // Si le voisin du dessus requiert une rotation (le 1er caractère du port commence par 'v'),
            // on continue quand même

            if (module.Sockets[2][0] != 'v' && p1.Equals(p3))
            {
                return;
            }

            temp.Add(p3);


            // Rotation 3

            Prototype p4 = new(module);
            p4.Rotation = 3;

            // Right, Forward, Left, Back => Back, Right, Forward, Left 
            (p4.Sockets[0], p4.Sockets[4], p4.Sockets[1], p4.Sockets[5]) = (p4.Sockets[5], p4.Sockets[0], p4.Sockets[4], p4.Sockets[1]);

            (p4.Weights.RightWeights, p4.Weights.ForwardWeights, p4.Weights.LeftWeights, p4.Weights.BackWeights) =
            (p4.Weights.BackWeights, p4.Weights.RightWeights, p4.Weights.ForwardWeights, p4.Weights.LeftWeights);

            (p4.ValidNeighbours.RightNeighbours, p4.ValidNeighbours.ForwardNeighbours, p4.ValidNeighbours.LeftNeighbours, p4.ValidNeighbours.BackNeighbours) =
            (p4.ValidNeighbours.BackNeighbours, p4.ValidNeighbours.RightNeighbours, p4.ValidNeighbours.ForwardNeighbours, p4.ValidNeighbours.LeftNeighbours);

            temp.Add(p4);
        }
    }
}