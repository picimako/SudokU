using System.Collections.Generic;
using System.Linq;
using Sudoku.Log;

namespace Sudoku.Controller.Finder.Killer
{
    public class PossibleNeighbourCellsFinder
    {
        private List<Cell> possibleNeighbourCells;
        private SudokuExercise se = SudokuExercise.get;
        private Logger log = Logger.Instance;

        /// <summary>A megadott cella elhelyezkedésétől függően megkeresi a cella lehetséges szomszédait</summary>
        /// <param name="cell">A viszonyítást képző cella</param>
        /// <param name="cageIndex">A vizsgálandó ketrec száma</param>
        /// <param name="egyenlo">Két fajta vizsgálat megkülönböztetésére szolgál</param>
        /// <returns>The list of possible neighbour cells.</returns>
        public List<Cell> FindPossibleNeighbourCells(Cell cell, int cageIndex, bool egyenlo)
        {
            possibleNeighbourCells = new List<Cell>();

            if (cell.IsInFirstRow())
            {
                log.Info("Cell is in first row.");
                CollectCell(Direction.DOWN, cell, cageIndex, egyenlo);

                sarokEsBenneSorVizsgalat(cell, cageIndex, egyenlo);

                return possibleNeighbourCells;
            }

            if (cell.IsInLastRow())
            {
                log.Info("Cell is in last row.");
                CollectCell(Direction.UP, cell, cageIndex, egyenlo);

                sarokEsBenneSorVizsgalat(cell, cageIndex, egyenlo);

                return possibleNeighbourCells;
            }

            if (cell.IsInFirstColumn())
            {
                log.Info("Cell is in first column.");
                CollectCell(Direction.RIGHT, cell, cageIndex, egyenlo);

                CheckUpAndDown(cell, cageIndex, egyenlo);

                return possibleNeighbourCells;
            }

            if (cell.IsInLastColumn())
            {
                log.Info("Cell is in last column.");
                CollectCell(Direction.LEFT, cell, cageIndex, egyenlo);

                CheckUpAndDown(cell, cageIndex, egyenlo);

                return possibleNeighbourCells;
            }

            /* Ha a cella indexei egyik előző esetnek sem felelnek meg, akkor mind a 4 szomszédot megvizsgálhatom*/
            CheckToLeftAndRight(cell, cageIndex, egyenlo);

            CheckUpAndDown(cell, cageIndex, egyenlo);

            return possibleNeighbourCells;
        }

        /// <summary>Megvizsgálja az [i,j] indexű cella 4 szomszéd celláját, 
        /// hogy melyik szomszéd cella értéke van benne az [i,j] indexű cella ketrecében</summary>
        /// <param name="direction">The direction to search towards</param>
        /// <param name="cell">A viszonyítást képező cella</param>
        /// <param name="cageIndex">Az [i,j] indexű cella ketrecszáma</param>
        /// <param name="egyenlo">Két fajta vizsgálat megkülönböztetésére szolgál</param>
        private void CollectCell(Direction direction, Cell cell, int cageIndex, bool egyenlo)
        {
            Cell alteredCell = cell.WithAlteredIndecesByDirection(direction);
            int row = alteredCell.Row, col = alteredCell.Col;

            if (egyenlo
                /* Ha az [i, j] indexű cella ketrecéhez szeretném hozzávenni valamelyik szomszéd cellát.
                 * A szomszéd szerepel-e már valamelyik ketrecben, és a szomszéd cella értéke benne van-e az [i,j] indexű cella ketrecében*/
                ? !se.Killer.IsCellInAnyCage(row, col) && !IsCageContainValue(se.Solution[row, col], cageIndex, se.Solution)

                /* Ha az [i,j] indexű cellát szeretném valamelyik szomszéd cella ketrecében elhelyezni.
                * Ez akkor jöhet elő, ha az [i,j] indexű cella üresen marad, és a körülötte levő cellák már mind benne vannak egy ketrecben.
                * Ha a szomszéd már benne van egy ketrecben, és a szomszéd cella ketrece nem tartalmazza az [i,j] indexű cella értékét*/
                : se.Killer.IsCellInAnyCage(row, col) && !IsCageContainValue(se.Solution[cell.Row, cell.Col], se.Killer.Exercise[row, col].CageIndex, se.Solution))
                possibleNeighbourCells.Add(new Cell(row, col));
        }

        private void CheckToLeftAndRight(Cell cell, int cageIndex, bool egyenlo)
        {
            CollectCell(Direction.LEFT, cell, cageIndex, egyenlo);

            CollectCell(Direction.RIGHT, cell, cageIndex, egyenlo);
        }

        private void CheckUpAndDown(Cell cell, int cageIndex, bool egyenlo)
        {
            CollectCell(Direction.UP, cell, cageIndex, egyenlo);

            CollectCell(Direction.DOWN, cell, cageIndex, egyenlo);
        }

        private void sarokEsBenneSorVizsgalat(Cell cell, int cageIndex, bool egyenlo)
        {
            //i=0: Ha a bal felső sarokban van, i=8: Ha a bal alsó sarokban van
            if (cell.IsInFirstColumn())
            {
                CollectCell(Direction.RIGHT, cell, cageIndex, egyenlo);
            }
            //i=0: Ha a jobb felső sarokban van, i=8: Ha a jobb alsó sarokban van
            else if (cell.IsInLastColumn())
            {
                CollectCell(Direction.LEFT, cell, cageIndex, egyenlo);
            }
            //Ha az előző 2 kivételével valahol a sorban
            else
            {
                CheckToLeftAndRight(cell, cageIndex, egyenlo);
            }
        }

        /// <summary>Megvizsgálja, hogy a megadott érték benne van-e a megadott (cageIndex számú) ketrecben</summary>
        /// <param name="value">A keresendő érték</param>
        /// <param name="cageIndex">A vizsgálandó ketrec száma</param>
        /// <param name="tomb">A vizsgálandó tömb: feladat vagy megoldás tömbje</param>
        /// <returns>Ha benne van az érték, akkor true, egyébként false</returns>
        /// TODO: should find a proper place for this!!
        public bool IsCageContainValue(int value, int cageIndex, int[,] tomb)
        {
            List<Cell> cellsOfValueFoundInCage = se.Killer.Cages[cageIndex].Cells
                .Where(cell => tomb[cell.Row, cell.Col] == value)
                .ToList();
            return cellsOfValueFoundInCage.Count > 0;
        }
    }
}
