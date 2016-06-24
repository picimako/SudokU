using Sudoku.Controller;
using Sudoku.Generate;

namespace Sudoku.Cells
{
    /// <summary>
    /// Helper class for cells.
    /// </summary>
    class CellHandler
    {
        private static SudokuExercise se = SudokuExercise.get;

        /// <summary>
        /// Checks if the cell is special which it is in two cases:
        ///  - when the exercise is SudokuX and the cell is in a diagonal
        ///  - when the exercise is Center Dot and the cell is at the middle of a block
        /// </summary>
        /// <param name="cell">The examined cell.</param>
        /// <returns>True if the cell is special, false otherwise.</returns>
        public static bool IsCellSpecial(Cell cell)
        {
            return IsCellSpecial(cell.Row, cell.Col);
        }

        /// <summary>
        /// Checks if the cell is special.
        /// </summary>
        /// <param name="row">The row index of the cell examined.</param>
        /// <param name="col">The column index of the cell examined.</param>
        /// <returns>True if the cell is special, false otherwise.</returns>
        public static bool IsCellSpecial(int row, int col)
        {
            return (se.ExerciseType == SudokuType.SudokuX && SudokuXController.CellIsInAnyDiagonal(row, col))
                || (se.ExerciseType == SudokuType.CenterDot && CenterDotController.CellIsAtMiddleOfAnyBlock(row, col));
        }
    }
}
