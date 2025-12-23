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
        /// <param name="dimensions">Les dimensions de la grille</param>
        /// <param name="rand">Le générateur d'aléatoire</param>
        public static void Generate(List<Cell> cells, int3 dimensions, ref Random rand)
        {
            // Réitère le calcul d'entropie des cellules
            // tant qu'il nous reste des cellules à effondrer

            int nbCollapsedCells = 0;

            while (nbCollapsedCells < cells.Count)
            {
                Iterate(cells, dimensions, ref rand);
            }
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
            Tile selectedTile = cellToCollapse.Options[rand.NextInt(0, cellToCollapse.Options.Count)];
            cellToCollapse.Options = new List<Tile> { selectedTile };
        }

        #endregion

        #region Méthodes statiques privées

        /// <summary>
        /// Itère sur les cellules à effondrer
        /// </summary>
        /// <param name="cells">La liste des cellules de la grille</param>
        /// <param name="dimensions">Les dimensions de la grille</param>
        /// <param name="rand">Le générateur d'aléatoire</param>
        private static void Iterate(List<Cell> cells, int3 dimensions, ref Random rand)
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