using Sudoku.Controller;

namespace Sudoku.Generate
{
    class BackTrackSolver
    {
        #region Members

        private int numberOfSolutionsFound = 0;
        private SudokuExercise se = SudokuExercise.get;

        #endregion

        #region Methods
        #region Constructor

        /// <summary>Solves the exercise, first without backtrack (while it is possible), then with backtrack.</summary>
        /// <returns>True if there is only one solution found.</returns>
        public bool SolveExerciseWithBackTrack()
        {
            se.Ctrl.IsExerciseSolvableWithoutBackTrack(se.NumberOfEmptyCells);
            Solve(0);
            return numberOfSolutionsFound == 1;
        }

        #endregion

        #region Private

        /// <summary>Checks if the n number can be put into the [i,j] cell</summary>
        /// <returns>True if n can be placed.</returns>
        private bool CanNBePutIntoCell(int i, int j, int n)
        {
            //If none of the houses contains n, then it can be placed.
            return !se.Ctrl.HousesContainValue(i, j, n);
        }

        /// <param name="p">Start position</param>
        private void Solve(int p)
        {
            p = FindFirstEmptyCellAfterP(p);

            //I've already filled this with a number
            if (se.IsCellTheLastInTable(p))
            {
                numberOfSolutionsFound++;
            }
            else
            {
                //Looping through the numbers
                for (int n = 1; n <= 9; n++)
                {
                    //Filling the current cell with the current number
                    se.Exercise[0][p / 9, p % 9] = n;

                    //If n can be placed, then proceeding to the next cell
                    if (CanNBePutIntoCell(p / 9, p % 9, n))
                        Solve(p + 1);
                }

                se.Exercise[0][p / 9, p % 9] = se.EMPTY;
            }
        }

        private int FindFirstEmptyCellAfterP(int pos)
        {
            int p = pos;
            while (p < se.LAST_CELL_INDEX)
            {
                if (se.Exercise[0][p / 9, p % 9] == se.EMPTY)
                    break;

                p++;
            }
            return p;
        }

        #endregion
        #endregion
    }
}
