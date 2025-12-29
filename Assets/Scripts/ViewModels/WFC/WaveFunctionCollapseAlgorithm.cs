using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Models.WFC;
using Assets.Scripts.Models.WFC.SOs;
using Unity.Mathematics;

namespace Assets.Scripts.ViewModels.WFC
{
    /// <summary>
    /// Algorithme du Wave Function Collapse
    /// </summary>
    public static class WaveFunctionCollapseAlgorithm
    {
        #region Constantes

        /// <summary>
        /// Le nombre max d'itérations pour la création de salles garanties
        /// avant d'abandonner
        /// </summary>
        private const int NB_GUARANTEED_ROOMS_ITERATIONS = 100;

        #endregion

        #region Méthodes statiques publiques

        /// <summary>
        /// Génère un nouveau niveau
        /// </summary>
        /// <param name="tilePalette">La palette de salles et cases à utiliser</param>
        /// <param name="gridSettings">Les paramètres de la grille</param>
        /// <param name="rand">Le générateur d'aléatoire</param>
        public static List<Cell> Generate(TilePalette tilePalette, GridSettings gridSettings, ref Random rand)
        {
            GetRandomSizeAndNbRooms(gridSettings, ref rand, out int3 gridSize, out int nbMaxRooms);
            List<Cell> gridCells = CreateCells(tilePalette.Tiles, gridSize);
            List<Tile> allOptions = new(tilePalette.Tiles);
            SetNeighbours(gridCells, gridSize);

            // Au début de la génération, on place les salles préconstruites

            List<Cell> unoccupiedCells = gridCells.Where(c => !c.Collapsed).ToList();
            CreateGuaranteedFixedRooms(gridCells, unoccupiedCells, tilePalette.GuaranteedFixedRooms, allOptions, gridSize, ref rand);
            CreateFixedRooms(gridCells, unoccupiedCells, tilePalette.FixedRooms, allOptions, gridSize, nbMaxRooms, ref rand);

            //// Réitère le calcul d'entropie des cellules
            //// tant qu'il nous reste des cellules à effondrer

            List<Cell> cellsWithLowestEntropy = new(unoccupiedCells.Count);

            while (unoccupiedCells.Count > 0)
            {
                CreateTiles(gridCells, unoccupiedCells, cellsWithLowestEntropy, allOptions, ref rand);
            }

            return gridCells;
        }

        #endregion

        #region Méthodes statiques privées

        /// <summary>
        /// Obtient une taille aléatoire ainsi qu'un nb aléatoire de salles pour le niveau
        /// </summary>
        /// <param name="gridSettings">Les paramètres de la grille</param>
        /// <param name="rand">Le générateur d'aléatoire</param>
        /// <param name="gridSize">Les dimensions du niveau à créer</param>
        /// <param name="nbMaxRooms">Le nb max de salles à créer</param>
        private static void GetRandomSizeAndNbRooms(GridSettings gridSettings, ref Random rand, out int3 gridSize, out int nbMaxRooms)
        {
            gridSize = new(rand.NextInt3(gridSettings.MinSize, gridSettings.MaxSize));
            nbMaxRooms = rand.NextInt(gridSettings.MinMaxNbRooms.x, gridSettings.MinMaxNbRooms.y);
        }

        /// <summary>
        /// Crée une nouvelle grille de cellules
        /// </summary>
        /// <param name="possibleTiles">La liste des possibilités de chaque cellule</param>
        /// <param name="gridSize">Les dimensions de la grille</param>
        /// <returns>La liste des cellules composant la grille</returns>
        private static List<Cell> CreateCells(Tile[] possibleTiles, int3 gridSize)
        {
            List<Cell> cells = new(gridSize.x * gridSize.y * gridSize.z);

            for (int x = 0; x < gridSize.x; ++x)
            {
                for (int y = 0; y < gridSize.y; ++y)
                {
                    for (int z = 0; z < gridSize.z; ++z)
                    {
                        cells.Add(new Cell(new List<Tile>(possibleTiles), new int3(x, y, z)));
                    }
                }
            }

            return cells;
        }

        /// <summary>
        /// Assigne les voisins de chaque cellule
        /// </summary>
        /// <param name="cells">La liste des cellules de la grille</param>
        /// <param name="gridSize">Les dimensions de la grille</param>
        private static void SetNeighbours(List<Cell> cells, int3 gridSize)
        {
            for (int i = 0; i < cells.Count; ++i)
            {
                Cell c = cells[i];
                int3 coords = c.Range[0];

                // Droite

                if (coords.x + 1 < gridSize.x)
                {
                    c.RightNeighbours.Add(new Range(coords + new int3(1, 0, 0)));
                }

                // Gauche

                if (coords.x - 1 > 0)
                {
                    c.LeftNeighbours.Add(new Range(coords + new int3(-1, 0, 0)));
                }

                // Haut

                if (coords.y + 1 < gridSize.y)
                {
                    c.UpNeighbours.Add(new Range(coords + new int3(0, 1, 0)));
                }

                // Bas

                if (coords.y - 1 > 0)
                {
                    c.DownNeighbours.Add(new Range(coords + new int3(0, -1, 0)));
                }

                // Devant

                if (coords.z + 1 < gridSize.z)
                {
                    c.ForwardNeighbours.Add(new Range(coords + new int3(0, 0, 1)));
                }

                if (coords.z - 1 > 0)
                {
                    c.BackNeighbours.Add(new Range(coords + new int3(0, 0, -1)));
                }

                cells[i] = c;
            }
        }

        /// <summary>
        /// Génère toutes les salles guaranties d'être présentes dans un niveau
        /// </summary>
        /// <param name="gridCells">La liste des cellules de la grille</param>
        /// <param name="unoccupiedCells">La liste des cellules libres</param>
        /// <param name="rooms">La palette de salles à utiliser</param>
        /// <param name="allOptions">La liste de toutes les options de case</param>
        /// <param name="gridSize">Les dimensions de la grille</param>
        /// <param name="rand">Le générateur d'aléatoire</param>
        private static void CreateGuaranteedFixedRooms(List<Cell> gridCells, List<Cell> unoccupiedCells, FixedRoom[] rooms, List<Tile> allOptions, int3 gridSize, ref Random rand)
        {
            foreach (FixedRoom room in rooms)
            {
                int iterations = 0;
                List<Cell> roomCells = new(room.Size.x * room.Size.y * room.Size.z);

                while (iterations < NB_GUARANTEED_ROOMS_ITERATIONS)
                {
                    if (TryCollapseRandomCellToFixedRoom(room, gridCells, gridSize, unoccupiedCells, roomCells, ref rand))
                    {
                        // Màj l'entropie des voisins

                        UpdateNeighbouringCells(gridCells, unoccupiedCells, allOptions);
                        break;
                    }

                    ++iterations;
                }

                if (iterations == NB_GUARANTEED_ROOMS_ITERATIONS)
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
        /// Génère autant de salles que possibles dans le niveau
        /// </summary>
        /// <param name="gridCells">La liste des cellules de la grille</param>
        /// <param name="unoccupiedCells">La liste des cellules libres</param>
        /// <param name="rooms">La palette de salles à utiliser</param>
        /// <param name="allOptions">La liste de toutes les options de case</param>
        /// <param name="gridSize">Les dimensions de la grille</param>
        /// <param name="nbMaxRooms">Le nombre max d'itérations avant d'abandonner</param>
        /// <param name="rand">Le générateur d'aléatoire</param>
        private static void CreateFixedRooms(List<Cell> gridCells, List<Cell> unoccupiedCells, FixedRoom[] rooms, List<Tile> allOptions, int3 gridSize, int nbMaxRooms, ref Random rand)
        {
            int iterations = 0;

            while (iterations < nbMaxRooms)
            {
                FixedRoom room = rooms[rand.NextInt(0, rooms.Length)];
                List<Cell> roomCells = new(room.Size.x * room.Size.y * room.Size.z);

                if (TryCollapseRandomCellToFixedRoom(room, gridCells, gridSize, unoccupiedCells, roomCells, ref rand))
                {
                    // Màj l'entropie des voisins

                    UpdateNeighbouringCells(gridCells, unoccupiedCells, allOptions);
                }

                ++iterations;
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
                            bool3 coordsAreEqual = c.Range.Contains(new int3(x, y, z));
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
            unoccupiedCells.Remove(cell);

            // S'il y en a assez, on les fusionne

            foreach (Cell c in roomCells)
            {
                FuseCells(ref cell, in c, gridCells);
                unoccupiedCells.Remove(c);
                gridCells.Remove(c);
            }

            // Puis on lui assigne la salle comme seule option

            cell.Options.Clear();
            cell.SelectedOption = room;
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
        /// <param name="gridCells">La liste des cellules de la grille</param>
        /// <param name="unoccupiedCells">a liste des cellules libres</param>
        /// <param name="cellsWithLowestEntropy">La liste des cellules avec la plus faible entropie</param>
        /// <param name="allOptions">La liste de toutes les options de case</param>
        /// <param name="rand">Le générateur d'aléatoire</param>
        private static void CreateTiles(List<Cell> gridCells, List<Cell> unoccupiedCells, List<Cell> cellsWithLowestEntropy, List<Tile> allOptions, ref Random rand)
        {
            // On récupère les cellules avec l'entropie la plus basse (le moins de possibilités restantes)

            GetCellsWithLowestEntropy(unoccupiedCells, cellsWithLowestEntropy);

            // Effondre une cellule au hasard

            Cell cellToCollapse = cellsWithLowestEntropy[rand.NextInt(0, cellsWithLowestEntropy.Count)];
            int index = gridCells.IndexOf(cellToCollapse);
            unoccupiedCells.Remove(cellToCollapse);
            CollapseRandomPossibility(ref cellToCollapse, ref rand);
            gridCells[index] = cellToCollapse;

            // Màj l'entropie des voisins

            UpdateNeighbouringCells(gridCells, unoccupiedCells, allOptions);
        }

        /// <summary>
        /// Récupère dans la liste de cellules renseignée,
        /// les cellules avec le nb de possibilités le plus bas
        /// </summary>
        /// <param name="unoccupiedCells">La liste de cellules libres</param>
        /// <param name="cellsWithLowestEntropy">La liste des cellules avec la plus faible entropie</param>
        /// <returns>La liste des positions des cellules avec le nb de possibilités le plus bas</returns>
        private static void GetCellsWithLowestEntropy(List<Cell> unoccupiedCells, List<Cell> cellsWithLowestEntropy)
        {
            cellsWithLowestEntropy.Clear();
            cellsWithLowestEntropy.AddRange(unoccupiedCells);
            cellsWithLowestEntropy.OrderBy(c => c.Entropy);
            cellsWithLowestEntropy.RemoveAll(c => c.Entropy != cellsWithLowestEntropy[0].Entropy);
        }

        /// <summary>
        /// Réduit l'entropie d'une cellule au hasard de la liste
        /// et effondre chaque cellule avec une seule possibilité restante
        /// </summary>
        /// <param name="lowestEntropyCells">La liste des cellules avec le nb de possibilités le plus bas</param>
        private static void CollapseRandomPossibility(ref Cell cellToCollapse, ref Random rand)
        {
            ITileOption selectedTile = cellToCollapse.Options[rand.NextInt(0, cellToCollapse.Options.Count)];
            cellToCollapse.Options.Clear();
            cellToCollapse.SelectedOption = selectedTile;
        }

        /// <summary>
        /// Màj l'entropie des cellules voisines
        /// </summary>
        /// <param name="gridCells">La liste des cellules de la grille</param>
        /// <param name="unoccupiedCells">a liste des cellules libres</param>
        /// <param name="allOptions">La liste de toutes les options de case</param>
        private static void UpdateNeighbouringCells(List<Cell> gridCells, List<Cell> unoccupiedCells, List<Tile> allOptions)
        {
            foreach (Cell cell in unoccupiedCells)
            {
                int index = gridCells.IndexOf(cell);
                unoccupiedCells.Remove(cell);

            }
        }

        #endregion
    }
}