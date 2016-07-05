using System.Collections.Generic;
using System.Linq;
using Sudoku.Cells;
using Sudoku.Log;

namespace Sudoku.Controller.Finder.Killer
{
    /// <summary>
    /// Helper class for Killer Sudoku to find and collect possible neighbour cells.
    /// </summary>
    public class PossibleNeighbourCellsFinder
    {
        private List<Cell> possibleNeighbourCells;
        private SudokuExercise se = SudokuExercise.Instance;
        private Logger log = Logger.Instance;

        /// <summary>
        /// Based on the position of the given cell it collects its possible neighbour cells
        /// which are candidates to be put in the given cage.
        /// </summary>
        /// <param name="cell">The cell which it searches for the neighbours of.</param>
        /// <param name="cageIndex">The cage index to examine.</param>
        /// <param name="addingNeighbourToCageOfCell">
        /// Whether we want to add a neighbourcell to the cage of the current cell, or
        /// we want to add the current cell into the neighbour's cage.
        /// </param>
        /// <returns>The list of possible neighbour cells.</returns>
        public List<Cell> FindPossibleNeighbourCells(Cell cell, int cageIndex, bool addingNeighbourToCageOfCell)
        {
            possibleNeighbourCells = new List<Cell>();

            if (cell.IsInFirstRow())
            {
                log.Info("Cell is in first row.");
                CollectCell(Direction.DOWN, cell, cageIndex, addingNeighbourToCageOfCell);

                CornerAndThenRowCheck(cell, cageIndex, addingNeighbourToCageOfCell);

                return possibleNeighbourCells;
            }

            if (cell.IsInLastRow())
            {
                log.Info("Cell is in last row.");
                CollectCell(Direction.UP, cell, cageIndex, addingNeighbourToCageOfCell);

                CornerAndThenRowCheck(cell, cageIndex, addingNeighbourToCageOfCell);

                return possibleNeighbourCells;
            }

            if (cell.IsInFirstColumn())
            {
                log.Info("Cell is in first column.");
                CollectCell(Direction.RIGHT, cell, cageIndex, addingNeighbourToCageOfCell);

                CheckUpAndDown(cell, cageIndex, addingNeighbourToCageOfCell);

                return possibleNeighbourCells;
            }

            if (cell.IsInLastColumn())
            {
                log.Info("Cell is in last column.");
                CollectCell(Direction.LEFT, cell, cageIndex, addingNeighbourToCageOfCell);

                CheckUpAndDown(cell, cageIndex, addingNeighbourToCageOfCell);

                return possibleNeighbourCells;
            }

            //If the cell indeces doesn't fit to any previous case, all 4 neighbours of it can be checked.
            CheckToLeftAndRight(cell, cageIndex, addingNeighbourToCageOfCell);

            CheckUpAndDown(cell, cageIndex, addingNeighbourToCageOfCell);

            return possibleNeighbourCells;
        }

        /// <summary>
        /// Checks if the given value is in the given cage (by its cage index).
        /// </summary>
        /// <param name="value">The value to find.</param>
        /// <param name="cageIndex">The cage index to search in.</param>
        /// <param name="table">The table to examine: either the exercise or the solution.</param>
        /// <returns>True if value is in the given cage, otherwise false.</returns>
        /// TODO: should find a proper place for this!!
        public bool IsCageContainValue(int value, int cageIndex, int[,] table)
        {
            List<Cell> cellsOfValueFoundInCage = se.Killer.Cages[cageIndex].Cells
                .Where(cell => table[cell.Row, cell.Col] == value)
                .ToList();
            return cellsOfValueFoundInCage.Count > 0;
        }

        /// <summary>
        /// Checks the 4 neighbours of the provided cell
        /// and adds the neighbour to the list of possible neighbour cells
        /// </summary>
        /// <param name="direction">The direction to search towards</param>
        /// <param name="cell">The cell to search among the neighbours of.</param>
        /// <param name="cageIndex">The cage index to search in.</param>
        /// <param name="addingNeighbourToCageOfCell">
        /// Whether we want to add a neighbourcell to the cage of the current cell, or
        /// we want to add the current cell into the neighbour's cage.
        /// </param>
        private void CollectCell(Direction direction, Cell cell, int cageIndex, bool addingNeighbourToCageOfCell)
        {
            Cell alteredCell = cell.WithAlteredIndecesByDirection(direction);
            int row = alteredCell.Row;
            int col = alteredCell.Col;

            if (addingNeighbourToCageOfCell
                /* If we want to join one of the neighbour cells to the cage of "cell".
                 * If the neighbour is in any of the cages, and the neighbour cell is in the cage of "cell".*/
                ? !se.Killer.IsCellInAnyCage(row, col) && !IsCageContainValue(se.Solution[row, col], cageIndex, se.Solution)

                /* If we want to join "cell" to the cage of one of the neighbour cells.
                 * This can happen when "cell" is left empty, and the cells around it are already in a cage.
                 * If the neighbour is in a cage, and the cage of the neighbour cell doesn't contain the value of "cell".*/
                : se.Killer.IsCellInAnyCage(row, col) && !IsCageContainValue(se.Solution[cell.Row, cell.Col], se.Killer.CageIndeces[row, col], se.Solution))
                possibleNeighbourCells.Add(new Cell(row, col));
        }

        private void CheckToLeftAndRight(Cell cell, int cageIndex, bool addingNeighbourToCageOfCell)
        {
            CollectCell(Direction.LEFT, cell, cageIndex, addingNeighbourToCageOfCell);
            CollectCell(Direction.RIGHT, cell, cageIndex, addingNeighbourToCageOfCell);
        }

        private void CheckUpAndDown(Cell cell, int cageIndex, bool addingNeighbourToCageOfCell)
        {
            CollectCell(Direction.UP, cell, cageIndex, addingNeighbourToCageOfCell);
            CollectCell(Direction.DOWN, cell, cageIndex, addingNeighbourToCageOfCell);
        }

        private void CornerAndThenRowCheck(Cell cell, int cageIndex, bool addingNeighbourToCageOfCell)
        {
            //i=0: if cell is in the top left corner, i=8: if cell is in the bottom left corner
            if (cell.IsInFirstColumn())
            {
                CollectCell(Direction.RIGHT, cell, cageIndex, addingNeighbourToCageOfCell);
            }
            //i=0: if cell is in the top right corner, i=8: if cell is in the bottom right corner
            else if (cell.IsInLastColumn())
            {
                CollectCell(Direction.LEFT, cell, cageIndex, addingNeighbourToCageOfCell);
            }
            //If cell is in somewhere in the row except the corners
            else
            {
                CheckToLeftAndRight(cell, cageIndex, addingNeighbourToCageOfCell);
            }
        }
    }
}
