using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models.WFC;
using Assets.Scripts.Models.WFC.SOs;
using Unity.Collections;
using Unity.Mathematics;

namespace Assets.Scripts.ViewModels.WFC
{
    /// <summary>
    /// La logique de l'algorithme WFC
    /// </summary>
    public class WFCViewModel : UnityEngine.MonoBehaviour
    {
        #region Constantes

        /// <summary>
        /// La liste des directions possibles
        /// </summary>
        private static readonly int3[] _directions = new int3[6]
        {
            new(1, 0, 0),
            new(-1, 0, 0),
            new(0, 1, 0),
            new(0, -1, 0),
            new(0, 0, 1),
            new(0, 0, -1)
        };

        #endregion

        #region Méthodes publiques

        /// <summary>
        /// Génère un nouveau niveau
        /// </summary>
        /// <param name="palette">La palette contenant les modules à instancier</param>
        /// <param name="gridSettings">Les paramètres de la grille</param>
        /// <param name="random">Générateur d'aléatoire</param>
        /// <param name="gridCells">La grille de cellules</param>
        /// <param name="prototypes">Les prototypes de chaque module</param>
        public void Generate(ModulePaletteSO palette, GridSettingsSO gridSettings, ref Random random, out Cell[] gridCells, out List<Prototype> prototypes)
        {
            int3 gridSize = random.NextInt3(gridSettings.MinSize, gridSettings.MaxSize);
            prototypes = CreatePrototypes(palette.Modules);
            gridCells = CreateGridCells(gridSize, prototypes);

            while (!gridCells.All(c => c.Collapsed))
            {
                Iterate(gridCells, gridSize, prototypes, ref random);
            }
        }

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

        #endregion

        #region Méthodes privées

        /// <summary>
        /// Crée les prototypes représentant chaque rotation du module renseigné.
        /// Cette méthode évite de créer des doublons.
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

            (p4.ValidNeighbours.RightNeighbours, p4.ValidNeighbours.ForwardNeighbours, p4.ValidNeighbours.LeftNeighbours, p4.ValidNeighbours.BackNeighbours) =
            (p4.ValidNeighbours.BackNeighbours, p4.ValidNeighbours.RightNeighbours, p4.ValidNeighbours.ForwardNeighbours, p4.ValidNeighbours.LeftNeighbours);

            temp.Add(p4);
        }

        /// <summary>
        /// Crée la grille de cellules
        /// </summary>
        /// <param name="gridSize">La taille de la grille</param>
        /// <param name="prototypes">Les prototypes à instancier</param>
        /// <returns>Une nouvelle grille de cellules</returns>
        private Cell[] CreateGridCells(int3 gridSize, List<Prototype> prototypes)
        {
            NativeArray<int> prototypeIDs = new(prototypes.Count, Allocator.Temp);

            for (int i = 0; i < prototypes.Count; ++i)
            {
                prototypeIDs[i] = i;
            }

            Cell[] gridCells = new Cell[gridSize.x * gridSize.y * gridSize.z];

            for (int z = 0; z < gridSize.z; ++z)
            {
                for (int y = 0; y < gridSize.y; ++y)
                {
                    for (int x = 0; x < gridSize.x; ++x)
                    {
                        // Plutôt que de copie la liste des prototypes dans chaque cellule,
                        // on va copier leurs indices pour gagner en mémoire et en performance

                        NativeList<int> prototypeIDsCopy = new(prototypes.Count, Allocator.Temp);
                        prototypeIDsCopy.CopyFrom(in prototypeIDs);
                        int index = z * gridSize.y * gridSize.x + y * gridSize.x + x;
                        gridCells[index] = new Cell(new int3(x, y, z), prototypeIDsCopy);
                    }
                }
            }

            return gridCells;
        }

        /// <summary>
        /// Lance une itération de la génération avec l'algorithme WFC
        /// </summary>
        /// <param name="gridCells">La grille de cellules</param>
        /// <param name="gridSize">La taille de la grille</param>
        /// <param name="prototypes">Les prototypes à instancier</param>
        /// <param name="random">Générateur d'aléatoire</param>
        private void Iterate(Cell[] gridCells, int3 gridSize, List<Prototype> prototypes, ref Random random)
        {
            List<Cell> cellsWithLowestEntropy = GetCellsWithLowestEntropy(gridCells);
            Cell randomCell = CollapseRandomCell(cellsWithLowestEntropy, prototypes, ref random);
            Propagate(randomCell, gridCells, prototypes, gridSize);
        }

        /// <summary>
        /// Lance une itération de la génération avec l'algorithme WFC
        /// </summary>
        /// <param name="gridCells">La grille de cellules</param>
        /// <returns>Les cellules avec l'entropie la plus basse</returns>
        private List<Cell> GetCellsWithLowestEntropy(Cell[] gridCells)
        {
            List<Cell> cellsWithLowestEntropy = new(gridCells);
            cellsWithLowestEntropy.RemoveAll(c => c.Collapsed);
            cellsWithLowestEntropy.Sort((a, b) => { return a.Entropy - b.Entropy; });
            cellsWithLowestEntropy.RemoveAll(x => x.Entropy != cellsWithLowestEntropy[0].Entropy);

            return cellsWithLowestEntropy;
        }

        /// <summary>
        /// Effondre une cellule au hasard
        /// en tenant compte du poids de chaque prototype
        /// </summary>
        /// <param name="cells">La liste des cellules valides</param>
        /// <param name="prototypes">La liste des prototypes</param>
        /// <param name="random">Générateur d'aléatoire</param>
        /// <returns>Une cellule au hasard, effondrée</returns>
        private Cell CollapseRandomCell(List<Cell> cells, List<Prototype> prototypes, ref Random random)
        {
            Cell randomCell = cells[random.NextInt(0, cells.Count)];

            // On récupère les prototypes associés à cette cellule

            Prototype[] cellPrototypes = new Prototype[randomCell.Options.Length];

            for (int i = 0; i < randomCell.Options.Length; ++i)
            {
                cellPrototypes[i] = prototypes[randomCell.Options[i]];
            }

            // Crée une liste comprenant les IDs de chaque prototype, dupliqués en fonction de leurs poids.
            // Les IDs avec un plus grand poids seront répétés plus souvent
            // et auront plus de chances d'être sélectionnés

            int length = cellPrototypes.Sum(p => p.Weight);
            NativeList<int> weightedOptions = new(length, Allocator.Temp);

            for (int i = 0; i < randomCell.Options.Length; ++i)
            {
                int id = randomCell.Options[i];
                Prototype p = cellPrototypes[i];

                for (byte j = 0; j < p.Weight; ++j)
                {
                    weightedOptions.Add(id);
                }
            }

            // On sélectionne un ID de prototype au hasard

            int randomOption = weightedOptions[random.NextInt(0, weightedOptions.Length)];
            randomCell.Options.Clear();
            randomCell.Options.Add(randomOption);

            return randomCell;
        }

        /// <summary>
        /// Resreint les options des cellules voisins
        /// </summary>
        /// <param name="cell">La cellule précédemment effondrée</param>
        /// <param name="gridCells">La grille de cellules</param>
        /// <param name="prototypes">Les prototypes</param>
        /// <param name="gridSize">La taille de la grille</param>
        private void Propagate(Cell cell, Cell[] gridCells, List<Prototype> prototypes, int3 gridSize)
        {
            Stack<Cell> cellsToPropagate = new(1);
            cellsToPropagate.Push(cell);

            NativeList<int> neighbouringCellsIDs = new(6, Allocator.Temp);
            NativeList<int> protoypeValidArraysIDs = new(6, Allocator.Temp);

            while (cellsToPropagate.TryPop(out Cell curCell))
            {
                Prototype curPrototype = prototypes[curCell.Options[0]];
                GetNeighboursIndices(curCell, gridCells, gridSize, neighbouringCellsIDs, protoypeValidArraysIDs);

                for (int i = 0; i < neighbouringCellsIDs.Length; ++i)
                {
                    int cellID = neighbouringCellsIDs[i];
                    int prototypeValidArrayID = protoypeValidArraysIDs[i];
                    Cell neighbour = gridCells[cellID];
                    NativeList<int> curPossibilities = new(1, Allocator.Temp);

                    GetCurPossibilities(curPrototype, prototypeValidArrayID, prototypes, curPossibilities);

                    for (int j = neighbour.Options.Length - 1; j == 0; --j)
                    {
                        int option = neighbour.Options[j];

                        if (curPossibilities.Length > 0 && !curPossibilities.Contains(option))
                        {
                            neighbour.Options.RemoveAt(j);

                            if (!cellsToPropagate.Contains(neighbour))
                            {
                                cellsToPropagate.Push(neighbour);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Obtient la position des voisins de la cellule renseignée
        /// </summary>
        /// <param name="curCell">La cellule en cours d'itération</param>
        /// <param name="gridCells">La grille de cellules</param>
        /// <param name="gridSize">La taille de la grille</param>
        /// <param name="neighbouringCellsIDs">Les indices des voisins valides à obtenir</param>
        /// <param name="protoypeValidArraysIDs">Les IDs des collections de voisins valides de chaque prototype</param>
        private void GetNeighboursIndices(Cell curCell, Cell[] gridCells, int3 gridSize, NativeList<int> neighbouringCellsIDs, NativeList<int> protoypeValidArraysIDs)
        {
            neighbouringCellsIDs.Clear();
            protoypeValidArraysIDs.Clear();

            // Pour chaque direction, on récupère la cellule voisine.
            // On évite les coordonnées qui pointent en dehors de la grille
            // ou vers une cellule déjà effondrée.

            for (int i = 0; i < _directions.Length; ++i)
            {
                int3 direction = _directions[i];
                int3 v = curCell.Coords + direction;

                if (v.x < 0 || v.x == gridSize.x || v.y < 0 || v.y == gridSize.y || v.z < 0 || v.z == gridSize.z)
                {
                    continue;
                }

                int index = v.z * gridSize.y * gridSize.x + v.y * gridSize.x + v.x;

                if (!gridCells[index].Collapsed)
                {
                    neighbouringCellsIDs.Add(index);

                    // On ajoute aussi le n° de la collection de voisins du prototype dans cette direction
                    // (voisins de droite, gauche, haut, etc).
                    // Ca nous permettra de retrouver les modules valides dans cette direction

                    protoypeValidArraysIDs.Add(i);
                }
            }
        }

        /// <summary>
        /// Récupère les voisins possibles du prototype actuel
        /// </summary>
        /// <param name="curPrototype">Le prototype actuel</param>
        /// <param name="prototypeValidArrayID">L'ID du tableau correspondant à la cellule voisine</param>
        /// <param name="prototypes">La liste des prototypes</param>
        /// <param name="curPossibilities">Les IDs des prototypes possibles à obtenir</param>
        private void GetCurPossibilities(Prototype curPrototype, int prototypeValidArrayID, List<Prototype> prototypes, NativeList<int> curPossibilities)
        {
            curPossibilities.Clear();
            ModuleSO[] modules;

            switch (prototypeValidArrayID)
            {
                // Voisin de droite

                case 0:
                    modules = curPrototype.ValidNeighbours.RightNeighbours;

                    if (modules.Length > 0)
                    {
                        foreach (ModuleSO module in modules)
                        {
                            Prototype prototype = prototypes.First(p => p.Prefab == module.Prefab && p.Sockets[1] == curPrototype.Sockets[0]);
                            curPossibilities.Add(prototypes.IndexOf(prototype));
                        }
                    }
                    break;

                // Voisin de gauche

                case 1:
                    modules = curPrototype.ValidNeighbours.LeftNeighbours;

                    if (modules.Length > 0)
                    {
                        foreach (ModuleSO module in modules)
                        {
                            Prototype prototype = prototypes.First(p => p.Prefab == module.Prefab && p.Sockets[0] == curPrototype.Sockets[1]);
                            curPossibilities.Add(prototypes.IndexOf(prototype));
                        }
                    }

                    break;

                // Voisin du haut

                case 2:
                    modules = curPrototype.ValidNeighbours.UpNeighbours;

                    if (modules.Length > 0)
                    {
                        foreach (ModuleSO module in modules)
                        {
                            Prototype prototype = prototypes.First(p => p.Prefab == module.Prefab && p.Sockets[3] == curPrototype.Sockets[2]);
                            curPossibilities.Add(prototypes.IndexOf(prototype));
                        }
                    }

                    break;

                // Voisin du bas

                case 3:
                    modules = curPrototype.ValidNeighbours.DownNeighbours;

                    if (modules.Length > 0)
                    {
                        foreach (ModuleSO module in modules)
                        {
                            Prototype prototype = prototypes.First(p => p.Prefab == module.Prefab && p.Sockets[2] == curPrototype.Sockets[3]);
                            curPossibilities.Add(prototypes.IndexOf(prototype));
                        }
                    }

                    break;

                // Voisin avant

                case 4:
                    modules = curPrototype.ValidNeighbours.ForwardNeighbours;

                    if (modules.Length > 0)
                    {
                        foreach (ModuleSO module in modules)
                        {
                            Prototype prototype = prototypes.First(p => p.Prefab == module.Prefab && p.Sockets[5] == curPrototype.Sockets[4]);
                            curPossibilities.Add(prototypes.IndexOf(prototype));
                        }
                    }

                    break;

                // Voisin arrière

                case 5:
                    modules = curPrototype.ValidNeighbours.BackNeighbours;

                    if (modules.Length > 0)
                    {
                        foreach (ModuleSO module in modules)
                        {
                            Prototype prototype = prototypes.First(p => p.Prefab == module.Prefab && p.Sockets[4] == curPrototype.Sockets[5]);
                            curPossibilities.Add(prototypes.IndexOf(prototype));
                        }
                    }

                    break;
            }
        }

        #endregion
    }
}