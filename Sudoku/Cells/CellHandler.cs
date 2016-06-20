using Sudoku.Controller;
using Sudoku.Generate;

namespace Sudoku.Cells
{
    class CellHandler
    {
        private static SudokuExercise se = SudokuExercise.get;

        public static bool IsCellSpecial(Cell cell)
        {
            return IsCellSpecial(cell.Row, cell.Col);
        }

        /// <summary>
        /// A cell is special in two cases:
        ///  - when the exercise is SudokuX and the cell is in a diagonal
        ///  - when the exercise is Center Dot and the cell is at the middle of a block
        /// </summary>
        /// <param name="row">The row index of the cell examined.</param>
        /// <param name="col">The column index of the cell examined.</param>
        public static bool IsCellSpecial(int row, int col)
        {
            return (se.ExerciseType == SudokuType.SudokuX && SudokuXController.CellIsInAnyDiagonal(row, col))
                || (se.ExerciseType == SudokuType.CenterDot && CenterDotController.CellIsAtMiddleOfAnyBlock(row, col));
        }
    }
}
