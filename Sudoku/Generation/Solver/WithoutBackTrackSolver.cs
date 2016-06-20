using Sudoku.Controller;
using static Sudoku.Controller.SudokuExercise;

namespace Sudoku.Generation.Solver
{
    public class WithoutBackTrackSolver
    {
        private SudokuExercise se = SudokuExercise.get;

        /// <summary>Solves the exercise without using backtrack algorithm (as much as the difficulty of the exercise makes it possible).</summary>
        /// <returns>True if the exercise is solved completely (there is no empty cell), otherwise false</returns>
        public bool SolveExerciseWithoutBackTrack()
        {
            int numberOfEmptyCellsToFill = se.NumberOfEmptyCells;
            //the value for the only empty cell throughout number tables
            //-1 means the cell is not empty in any number tables, or it is empty in more than 1 number tables
            int valueOfOnlyEmptyCellThroughoutNumberTables = -1;
            bool cellFillingHappened;
            se.FirstEmptyCell = 0;

            for (int cell = 1; cell <= numberOfEmptyCellsToFill; cell++)
            {
                cellFillingHappened = false;

                for (int p = se.FirstEmptyCell; p < LAST_CELL; p++)
                {
                    //If the cell in not empty, then moving on to the next cell, as only empty cells are inspected.
                    if (!se.IsCellEmpty(0, p))
                        continue;

                    valueOfOnlyEmptyCellThroughoutNumberTables = -1;

                    //Getting the number tables where the current cell is empty
                    for (int num = 1; num < 10; num++)
                    {
                        if (se.IsCellEmpty(num, p))
                        {
                            if (valueOfOnlyEmptyCellThroughoutNumberTables == -1)
                                valueOfOnlyEmptyCellThroughoutNumberTables = num;
                            else
                            {
                                valueOfOnlyEmptyCellThroughoutNumberTables = -1;
                                break;
                            }
                        }
                    }

                    //If the current cell is empty in only one table (including the exercise)
                    //then the value of the found number table should be filled in
                    if (valueOfOnlyEmptyCellThroughoutNumberTables != -1)
                    {
                        se.Exercise[0][p / 9, p % 9] = valueOfOnlyEmptyCellThroughoutNumberTables;
                        se.Exercise[valueOfOnlyEmptyCellThroughoutNumberTables][p / 9, p % 9] = valueOfOnlyEmptyCellThroughoutNumberTables;
                        se.Ctrl.MakeHousesOccupied(valueOfOnlyEmptyCellThroughoutNumberTables, p / 9, p % 9);
                        cellFillingHappened = true;
                        se.Ctrl.PostProcessCellFilling(p);
                    }
                    else
                    {
                        int row;
                        int col;

                        for (int num = 1; num <= 9; num++)
                        {
                            if (se.Ctrl.FindAndFillOnlyEmptyCellInHouses(num, out row, out col))
                            {
                                cellFillingHappened = true;
                                se.Ctrl.PostProcessCellFilling((row * 9) + col);
                                break;
                            }
                        }
                    }
                }

                if (!cellFillingHappened)
                    break;
            }

            //If there is no empty cell, then the exercise could be solved without using backtrack algorithm
            return se.IsExerciseFull();
        }
    }
}
