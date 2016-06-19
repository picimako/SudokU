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

        public static bool IsCellSpecial(int row, int col)
        {
            return (se.ExerciseType == SudokuType.SudokuX && SudokuXController.CellIsInAnyDiagonal(row, col))
                || (se.ExerciseType == SudokuType.CenterDot && CenterDotController.CellIsAtMiddleOfAnyBlock(row, col));
        }
    }
}
