using System.Linq;
using System.Collections.Generic;
using Sudoku.Cells;
using Sudoku.Controller.Finder.Killer;
using Sudoku.Log;
using static Sudoku.Controller.SudokuExercise;

namespace Sudoku.Controller
{
    /// <summary>
    /// Controller class for Killer Sudoku exercises.
    /// </summary>
    public class KillerSudokuController
    {
        private SudokuExercise se = SudokuExercise.Instance;
        private Logger log = Logger.Instance;
        private PossibleNeighbourCellsFinder finder = new PossibleNeighbourCellsFinder();

        public PossibleNeighbourCellsFinder NeighbourCellFinder { get { return finder; } }

        /// <summary>
        /// Constuctor.
        /// </summary>
        public KillerSudokuController()
        {
            se.Solution = new int[9, 9];
        }

        /// <summary>
        /// Copies the given solution array to the solution of the current Sudoku exercise.
        /// This is needed when the Killer exercise is generated.
        /// The actual solution is stored where the exercise is stored but that array is needed for the game itself.
        /// </summary>
        public void CopySolutionToExercise()
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    se.Solution[i, j] = se.Exercise[0][i, j];
                }
            }
        }

        /// <summary>
        /// Checks whether the cell indicated by [row,col] is the top left cell of the provided cage.
        /// </summary>
        /// <param name="topLeftIndicatorCell">The top left (indicator) cell of the current cage.</param>
        /// <param name="row">The row index of the cell to compare the top left cell to.</param>
        /// <param name="col">The column index of the cell to compare the top left cell to.</param>
        /// <returns></returns>
        public bool IsCellAtTopLeftOfCage(Cell topLeftIndicatorCell, int row, int col)
        {
            return new Cell(row, col).Equals(topLeftIndicatorCell);
        }

        /// <summary>
        /// Searches for the first empty cell in the table that hasn't been placed in any cage.
        /// The search is performed by position from left to right, top to bottom.
        /// </summary>
        /// <returns>If it finds an empty cell then returns with it, otherwise with an out of range cell.</returns>
        public Cell FindFirstEmptyCell()
        {
            int p = 0;

            while (p < LAST_CELL && se.Killer.IsCellInAnyCage(p / 9, p % 9))
                p++;

            //If it didn't step of out the table
            return p < LAST_CELL ? new Cell(p / 9, p % 9) : Cell.OUT_OF_RANGE;
        }

        /// <summary>
        /// Collects the cage indeces which are
        ///  - the neighbour(s) of the provided cell and
        ///  - the cage of the neighbour cell doesn't contain the value of the provided cell and
        ///  - the cage of the neighbour cell contains less than 9 values
        /// </summary>
        /// <param name="cell">The cell to search the neighbour cages of.</param>
        /// <returns>The list of possible neighbour cages.</returns>
        public List<int> FindPossibleNeighbourCages(Cell cell)
        {
            List<Cell> possibleNeighbourCells = finder.FindPossibleNeighbourCells(cell, -1, false);
            List<int> possibleCageIndeces = new List<int>();

            foreach (Cell neighbourCell in possibleNeighbourCells)
            {
                int cageIndexOfNeighbourCell = se.Killer.Exercise[neighbourCell.Row, neighbourCell.Col].CageIndex;
                //If the cage of the neighbour cell consists of less than 9 cells, this cage is a candidate
                if (se.Killer.Cages[cageIndexOfNeighbourCell].Cells.Count < 9)
                    possibleCageIndeces.Add(cageIndexOfNeighbourCell);
            }

            return possibleCageIndeces;
        }

        /// <summary>
        /// Places cell in the given cage.
        /// Creates the cage if it hasn't exist.
        /// Adds the cell to the new cage.
        /// </summary>
        /// <param name="cell">The cell to put in the given cage.</param>
        /// <param name="cageIndex">The cage index to put the given cell in.</param>
        public void PutCellInCage(Cell cell, int cageIndex)
        {
            se.Killer.Exercise[cell.Row, cell.Col].CageIndex = cageIndex;

            if (!se.Killer.Cages.ContainsKey(cageIndex))
                se.Killer.Cages.Add(cageIndex, new Cage());

            se.Killer.Cages[cageIndex].Cells.Add(new Cell(cell.Row, cell.Col));
        }

        /// <summary>
        /// Collects the sum of numbers in the cages and the cells
        /// that will contain these sums when displaying the exercise on the GUI.
        /// </summary>
        public Dictionary<Cell, int> GetSumOfNumbersAndIndicatorCages()
        {
            if (se.IsExerciseGenerated)
                CalculateSumOfNumbersInAllCages();

            //This is the cell where the sum of numbers will be displayed for each cage
            Cell upperLeftCornerCellOfCage;

            Dictionary<Cell, int> cornerCellsWithSums = new Dictionary<Cell, int>();

            foreach (KeyValuePair<int, Cage> cage in se.Killer.Cages)
            {
                upperLeftCornerCellOfCage = new Cell(9, 9);
                upperLeftCornerCellOfCage = GetMostUpperCellInCage(cage, upperLeftCornerCellOfCage);
                upperLeftCornerCellOfCage = GetMostLeftCellInCage(cage, upperLeftCornerCellOfCage);

                cornerCellsWithSums.Add(upperLeftCornerCellOfCage, se.Killer.Cages[cage.Key].SumOfNumbers);
            }

            return cornerCellsWithSums;
        }

        /// <summary>
        /// Calculates the sum of cell values for each cage.
        /// Cage indeces are mapped with the sum of numbers in them.
        /// </summary>
        private void CalculateSumOfNumbersInAllCages()
        {
            foreach (KeyValuePair<int, Cage> cage in se.Killer.Cages)
            {
                int sumOfNumbersInCage = cage.Value.Cells.Sum(cell => se.Solution[cell.Row, cell.Col]);

                se.Killer.Cages[cage.Key].SumOfNumbers = sumOfNumbersInCage;
            }
        }

        private Cell GetMostUpperCellInCage(KeyValuePair<int, Cage> cage, Cell upperLeftCornerCellOfCage)
        {
            Cell mostUpperCell = upperLeftCornerCellOfCage;
            foreach (Cell cell in cage.Value.Cells)
            {
                if (cell.IsAbove(mostUpperCell))
                {
                    mostUpperCell.CopyIndecesOf(cell);
                }
            }
            return mostUpperCell;
        }

        private Cell GetMostLeftCellInCage(KeyValuePair<int, Cage> cage, Cell upperLeftCornerCellOfCage)
        {
            Cell mostLeftCell = upperLeftCornerCellOfCage;
            foreach (Cell cell in cage.Value.Cells)
            {
                //If the cell is amongst the top ones
                if (cell.IsInSameRowAs(mostLeftCell) && cell.IsAtLeftOf(mostLeftCell))
                {
                    mostLeftCell.CopyIndecesOf(cell);
                }
            }
            return mostLeftCell;
        }
    }
}
