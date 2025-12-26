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
        /// <param name="gridCells">La liste des cellules de la grille</param>
        /// <param name="tilePalette">La palette de salles et cases à utiliser</param>
        /// <param name="gridSize">Les dimensions de la grille</param>
        /// <param name="nbMaxRooms">Le nombre max de salles pouvant être instanciées</param>
        /// <param name="rand">Le générateur d'aléatoire</param>
        public static void Generate(List<Cell> gridCells, TilePalette tilePalette, int3 gridSize, int nbMaxRooms, ref Random rand)
        {
            // Au début de la génération, on place les salles préconstruites

            CreateGuaranteedFixedRooms(gridCells, tilePalette.GuaranteedFixedRooms, gridSize, ref rand);

            // Réitère le calcul d'entropie des cellules
            // tant qu'il nous reste des cellules à effondrer

            //int nbCollapsedCells = 0;

            //while (nbCollapsedCells < cells.Count)
            //{
            //    Iterate(cells, gridSize, ref rand);
            //}
        }

        #endregion

        #region Méthodes statiques privées

        /// <summary>
        /// Génère toutes les salles guaranties d'être présentes dans un niveau
        /// </summary>
        /// <param name="gridCells">La liste des cellules de la grille</param>
        /// <param name="rooms">La palette de salles à utiliser</param>
        /// <param name="gridSize">Les dimensions de la grille</param>
        /// <param name="rand">Le générateur d'aléatoire</param>
        private static void CreateGuaranteedFixedRooms(List<Cell> gridCells, FixedRoom[] rooms, int3 gridSize, ref Random rand)
        {
            List<Cell> unoccupiedCells = gridCells.Where(c => !c.Collapsed).ToList();

            foreach (FixedRoom room in rooms)
            {
                int iterations = 0;
                List<Cell> roomCells = new(room.Size.x * room.Size.y * room.Size.z);

                while (iterations < 100)
                {
                    if (TryCollapseRandomCellToFixedRoom(room, gridCells, gridSize, unoccupiedCells, roomCells, ref rand))
                    {
                        break;
                    }

                    ++iterations;
                }

                if (iterations == 100)
                {
                    // Si après tant d'itérations, la salle n'a pas pu être créée,
                    // on renvoie un message d'erreur.
                    // S'il y a bcp de salles à créer et la grille est petite,
                    // le code pourra avoir du mal à trouver de la place pour les dernières salles. 
                    // Ce n'est pas bien grave donc on le laisse comme ça pour l'instant.

                    throw new System.Exception("Erreur : Le délai d'attente pour la génération de salles garanties est dépassé.");
                }
            }
        }

        /// <summary>
        /// Détermine les cellules nécessaires à la création d'une salle
        /// et fusionne ces cellules en une seul
        /// </summary>
        /// <param name="room">La salle à créer</param>
        /// <param name="gridCells">La liste des cellules de la grille</param>
        /// <param name="gridSize">Les dimensions de la grille</param>
        /// <param name="unoccupiedCells">La liste des cellules libres</param>
        /// <param name="roomCells">La liste des cellules requises pour la création de la salle</param>
        /// <param name="rand">Le générateur d'aléatoire</param>
        /// <exception cref="System.Exception">Le délai d'attente pour la génération de salles garanties est dépassé.</exception>
        private static bool TryCollapseRandomCellToFixedRoom(FixedRoom room, List<Cell> gridCells, int3 gridSize, List<Cell> unoccupiedCells, List<Cell> roomCells, ref Random rand)
        {
            // On recherche une cellule libre

            Cell cell = unoccupiedCells[rand.NextInt(0, unoccupiedCells.Count)];
            int cellIndex = gridCells.IndexOf(cell);
            int3 start = cell.Range[0];

            // Si la salle devrait dépasser la grille, on retourne à la sélection de cellule

            bool3 outOfRange = start + room.Size > gridSize;

            if (outOfRange.x || outOfRange.y || outOfRange.z)
            {
                return false;
            }

            // Une fois la cellule libre trouvée, en regarde s'il y a assez de cellules libres voisines
            // pour créer la salle.

            roomCells.Clear();

            for (int x = start.x; x < start.x + room.Size.x; ++x)
            {
                for (int y = start.y; y < start.y + room.Size.y; ++y)
                {
                    for (int z = start.z; z < start.z + room.Size.z; ++z)
                    {
                        Cell nextCell = gridCells.First(c =>
                        {
                            bool3 coordsAreEqual = c.Range[0] == new int3(x, y, z);
                            return coordsAreEqual.x && coordsAreEqual.y && coordsAreEqual.z;
                        });

                        // Si la cellule suivante est déjà occupée, on ne peut pas créer de salle ici

                        if (nextCell.Collapsed)
                        {
                            return false;
                        }

                        roomCells.Add(nextCell);
                    }
                }
            }

            // Retire la 1è cellule vu que c'est un doublon

            roomCells.RemoveAt(0);

            // S'il y en a assez, on les fusionne

            foreach (Cell c in roomCells)
            {
                FuseCells(ref cell, in c, gridCells);
                unoccupiedCells.Remove(c);
                gridCells.Remove(c);
            }

            // Puis on lui assigne la salle comme seule option

            cell.Options = new List<ITileOption> { room };
            gridCells[cellIndex] = cell;

            return true;
        }

        /// <summary>
        /// Fusionne deux cellules
        /// </summary>
        /// <param name="cell">La cellule de départ</param>
        /// <param name="c">La cellule à assimiler</param>
        /// <param name="gridCells">Les cellules de la grille</param>
        private static void FuseCells(ref Cell cell, in Cell c, List<Cell> gridCells)
        {
            // On change la plage de cell pour englober c
            Range oldRange = cell.Range;
            int3[] newArr = new int3[cell.Range.Length + c.Range.Length];
            System.Array.Copy(cell.Range.Value, 0, newArr, 0, cell.Range.Length);
            System.Array.Copy(c.Range.Value, 0, newArr, cell.Range.Length, c.Range.Length);
            cell.Range = new Range(newArr);

            #region Pour chaque cellule pointant vers c, on les redirige vers cell

            for (int i = 0; i < gridCells.Count; ++i)
            {
                Cell neighbour = gridCells[i];

                // Droite

                if (neighbour.RightNeighbours.Contains(c.Range))
                {
                    neighbour.RightNeighbours.Remove(c.Range);

                    if (neighbour.RightNeighbours.Contains(oldRange))
                    {
                        neighbour.RightNeighbours.Remove(oldRange);
                    }

                    neighbour.RightNeighbours.Add(cell.Range);
                }

                // Gauche

                if (neighbour.LeftNeighbours.Contains(c.Range))
                {
                    neighbour.LeftNeighbours.Remove(c.Range);

                    if (neighbour.LeftNeighbours.Contains(oldRange))
                    {
                        neighbour.LeftNeighbours.Remove(oldRange);
                    }

                    neighbour.LeftNeighbours.Add(cell.Range);
                }

                // Haut

                if (neighbour.UpNeighbours.Contains(c.Range))
                {
                    neighbour.UpNeighbours.Remove(c.Range);

                    if (neighbour.UpNeighbours.Contains(oldRange))
                    {
                        neighbour.UpNeighbours.Remove(oldRange);
                    }

                    neighbour.UpNeighbours.Add(cell.Range);
                }

                // Bas

                if (neighbour.DownNeighbours.Contains(c.Range))
                {
                    neighbour.DownNeighbours.Remove(c.Range);

                    if (neighbour.DownNeighbours.Contains(oldRange))
                    {
                        neighbour.DownNeighbours.Remove(oldRange);
                    }

                    neighbour.DownNeighbours.Add(cell.Range);
                }

                // Devant

                if (neighbour.ForwardNeighbours.Contains(c.Range))
                {
                    neighbour.ForwardNeighbours.Remove(c.Range);

                    if (neighbour.ForwardNeighbours.Contains(oldRange))
                    {
                        neighbour.ForwardNeighbours.Remove(oldRange);
                    }

                    neighbour.ForwardNeighbours.Add(cell.Range);
                }

                // Derrière

                if (neighbour.BackNeighbours.Contains(c.Range))
                {
                    neighbour.BackNeighbours.Remove(c.Range);

                    if (neighbour.BackNeighbours.Contains(oldRange))
                    {
                        neighbour.BackNeighbours.Remove(oldRange);
                    }

                    neighbour.BackNeighbours.Add(cell.Range);
                }

                gridCells[i] = neighbour;
            }

            #endregion

            #region Pour chaque cellule que pointe c, on les ajoute à cell comme voisins.

            // Comme on incrémente à chaque fois, on n'a besoin de vérifier que les voisins négatifs,
            // car ils ont peut-être déjà été incorporés dans cell

            cell.RightNeighbours.AddRange(c.RightNeighbours);
            cell.UpNeighbours.AddRange(c.UpNeighbours);
            cell.ForwardNeighbours.AddRange(c.ForwardNeighbours);

            foreach (Range neighbour in c.LeftNeighbours)
            {
                if (!cell.Range.Contains(neighbour))
                {
                    cell.LeftNeighbours.Add(neighbour);
                }
            }

            foreach (Range neighbour in c.DownNeighbours)
            {
                if (!cell.Range.Contains(neighbour))
                {
                    cell.DownNeighbours.Add(neighbour);
                }
            }

            foreach (Range neighbour in c.BackNeighbours)
            {
                if (!cell.Range.Contains(neighbour))
                {
                    cell.BackNeighbours.Add(neighbour);
                }
            }

            #endregion
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

        /// <summary>
        /// Récupère dans la liste de cellules renseignée,
        /// les cellules avec le nb de possibilités le plus bas
        /// </summary>
        /// <param name="gridCells">La liste de cellules non-effondrées</param>
        /// <returns>La liste des positions des cellules avec le nb de possibilités le plus bas</returns>
        private static int[] GetCellsWithLowestEntropy(List<Cell> gridCells)
        {
            List<Cell> tempGrid = new(gridCells);
            tempGrid.RemoveAll(c => c.Collapsed);
            tempGrid.OrderBy(c => c.Entropy);
            tempGrid.RemoveAll(c => c.Entropy != tempGrid[0].Entropy);

            int[] ids = new int[tempGrid.Count];

            for (int i = 0; i < tempGrid.Count; ++i)
            {
                ids[i] = gridCells.IndexOf(tempGrid[i]);
            }

            return ids;
        }

        /// <summary>
        /// Réduit l'entropie d'une cellule au hasard de la liste
        /// et effondre chaque cellule avec une seule possibilité restante
        /// </summary>
        /// <param name="lowestEntropyCells">La liste des cellules avec le nb de possibilités le plus bas</param>
        private static void CollapseRandomPossibility(ref Cell cellToCollapse, ref Random rand)
        {
            ITileOption selectedTile = cellToCollapse.Options[rand.NextInt(0, cellToCollapse.Options.Count)];
            cellToCollapse.Options = new List<ITileOption> { selectedTile };
        }

        #endregion
    }
}