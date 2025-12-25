using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models.WFC;
using Unity.Mathematics;

namespace Assets.Scripts.ViewModels.WFC
{
    /// <summary>
    /// Algorithme du Wave Function Collapse
    /// </summary>
    public static class WaveFunctionCollapseAlgorithm
    {
        #region Méthodes statiques publiques

        /// <summary>
        /// Génère un nouveau niveau
        /// </summary>
        /// <param name="cells">La liste des cellules de la grille</param>
        /// <param name="tilePalette">La palette de salles et cases à utiliser</param>
        /// <param name="gridSize">Les dimensions de la grille</param>
        /// <param name="nbMaxRooms">Le nombre max de salles pouvant être instanciées</param>
        /// <param name="rand">Le générateur d'aléatoire</param>
        public static void Generate(List<Cell> cells, TilePalette tilePalette, int3 gridSize, int nbMaxRooms, ref Random rand)
        {
            Range gridRange = new(int3.zero, gridSize);

            // Au début de la génération, on place les salles préconstruites

            CreateGuaranteedFixedRooms(cells, tilePalette.GuaranteedFixedRooms, gridRange, ref rand);

            // Réitère le calcul d'entropie des cellules
            // tant qu'il nous reste des cellules à effondrer

            //int nbCollapsedCells = 0;

            //while (nbCollapsedCells < cells.Count)
            //{
            //    Iterate(cells, gridSize, ref rand);
            //}
        }

        /// <summary>
        /// Récupère dans la liste de cellules renseignée,
        /// les cellules avec le nb de possibilités le plus bas
        /// </summary>
        /// <param name="cells">La liste de cellules non-effondrées</param>
        /// <returns>La liste des positions des cellules avec le nb de possibilités le plus bas</returns>
        public static int[] GetCellsWithLowestEntropy(List<Cell> cells)
        {
            List<Cell> tempGrid = new(cells);
            tempGrid.RemoveAll(c => c.Collapsed);
            tempGrid.OrderBy(c => c.Entropy);
            tempGrid.RemoveAll(c => c.Entropy != tempGrid[0].Entropy);

            int[] ids = new int[tempGrid.Count];

            for (int i = 0; i < tempGrid.Count; ++i)
            {
                ids[i] = cells.IndexOf(tempGrid[i]);
            }

            return ids;
        }

        /// <summary>
        /// Réduit l'entropie d'une cellule au hasard de la liste
        /// et effondre chaque cellule avec une seule possibilité restante
        /// </summary>
        /// <param name="lowestEntropyCells">La liste des cellules avec le nb de possibilités le plus bas</param>
        public static void CollapseRandomPossibility(ref Cell cellToCollapse, ref Random rand)
        {
            ITileOption selectedTile = cellToCollapse.Options[rand.NextInt(0, cellToCollapse.Options.Count)];
            cellToCollapse.Options = new List<ITileOption> { selectedTile };
        }

        #endregion

        #region Méthodes statiques privées

        /// <summary>
        /// Génère toutes les salles guaranties d'être présentes dans un niveau
        /// </summary>
        /// <param name="cells">La liste des cellules de la grille</param>
        /// <param name="tilePalette">La palette de salles et cases à utiliser</param>
        /// <param name="gridRange">La plage de cellules de la grille</param>
        /// <param name="rand">Le générateur d'aléatoire</param>
        private static void CreateGuaranteedFixedRooms(List<Cell> cells, FixedRoom[] rooms, Range gridRange, ref Random rand)
        {
            List<Cell> unoccupiedCells = cells.Where(c => !c.Collapsed).ToList();

            foreach (FixedRoom room in rooms)
            {
                CollapseRandomCellToFixedRoom(cells, gridRange, unoccupiedCells, room, ref rand);
            }
        }

        /// <summary>
        /// Détermine les cellules nécessaires à la création d'une salle
        /// et fusionne ces cellules en une seul
        /// </summary>
        /// <param name="cells">La liste des cellules de la grille</param>
        /// <param name="gridRange">La plage de cellules de la grille</param>
        /// <param name="unoccupiedCells">La liste des cellules libres</param>
        /// <param name="room">La salle à créer</param>
        /// <param name="rand">Le générateur d'aléatoire</param>
        /// <exception cref="System.Exception">Le délai d'attente pour la génération de salles garanties est dépassé.</exception>
        private static void CollapseRandomCellToFixedRoom(List<Cell> cells, Range gridRange, List<Cell> unoccupiedCells, FixedRoom room, ref Random rand)
        {
            bool done = false;
            int iterations = 0;

            while (!done)
            {
                // On recherche une cellule libre

                Cell cell = unoccupiedCells[rand.NextInt(0, unoccupiedCells.Count)];
                int cellIndex = cells.IndexOf(cell);

                // Si la salle devrait dépasser la grille, on retourne à la sélection de cellule

                if (!gridRange.Contains(cell.Range.Start + room.Size))
                {
                    goto Fail;
                }

                // Une fois la cellule libre trouvée, en regarde s'il y a assez de cellules libres voisines
                // pour créer la salle.

                List<Cell> roomCells = new(room.Size.x * room.Size.y * room.Size.z) { cell };

                for (int x = 0; x < room.Size.x; ++x)
                {
                    for (int y = 0; y < room.Size.y; ++y)
                    {
                        for (int z = 1; z < room.Size.z; ++z)   // z = 1 pour sauter la 1è cellule, càd "cell"
                        {
                            Cell nextCell = cells.First(c => c.Range.Contains(new int3(x, y, z)));

                            // Si la cellule suivant est déjà occupée, on ne peut pas créer de salle ici

                            if (nextCell.Collapsed)
                            {
                                goto Fail;
                            }

                            roomCells.Add(nextCell);
                        }
                    }
                }

                // S'il y en a assez, on les fusionne

                foreach (Cell c in roomCells)
                {
                    cell.FuseWith(c);
                    unoccupiedCells.Remove(c);
                    cells.Remove(c);
                }

                // Puis on lui assigne la salle comme seule option

                cell.Options = new List<ITileOption> { room };
                cells[cellIndex] = cell;

                Fail:
                {
                    ++iterations;

                    // Si après tant d'itérations, la salle n'a pas pu être créée,
                    // on renvoie un message d'erreur.
                    // S'il y a bcp de salles à créer et la grille est petite,
                    // le code pourra avoir du mal à trouver de la place pour les dernières salles. 
                    // Ce n'est pas bien grave donc on le laisse comme ça pour l'instant.

                    if (iterations == 100)
                    {
                        throw new System.Exception("Erreur : Le délai d'attente pour la génération de salles garanties est dépassé.");
                    }
                }
            }
        }

        /// <summary>
        /// Itère sur les cellules à effondrer
        /// </summary>
        /// <param name="cells">La liste des cellules de la grille</param>
        /// <param name="gridSize">Les dimensions de la grille</param>
        /// <param name="rand">Le générateur d'aléatoire</param>
        private static void Iterate(List<Cell> cells, int3 gridSize, ref Random rand)
        {
            // On récupère les cellules avec l'entropie la plus basse (le moins de possibilités restantes)

            int[] lowestEntropyIndexes = GetCellsWithLowestEntropy(cells);

            // Effondre une cellule au hasard

            int randIndex = lowestEntropyIndexes[rand.NextInt(0, lowestEntropyIndexes.Length)];
            Cell cellToCollapse = cells[randIndex];
            CollapseRandomPossibility(ref cellToCollapse, ref rand);
            cells[randIndex] = cellToCollapse;

            // Màj l'entropie des voisins

            //UpdateNeighbouringCells();
        }

        #endregion
    }
}