using Sudoku.Controller;
using Sudoku.Generate;

namespace Sudoku.Cells
{
    class CellHandler
    {
        private static SudokuExercise se = SudokuExercise.get;

        public static bool IsCellSpecial(int row, int col)
        {
            return (se.ExerciseType == SudokuType.SudokuX && SudokuXController.CellIsInAnyDiagonal(row, col))
                || (se.ExerciseType == SudokuType.CenterDot && CenterDotController.CellIsAtMiddleOfAnyBlock(row, col));
        }

        public static bool IsCellInFirstRow(Cell cell)
        {
            return cell.Row == 0;
        }

        public static bool IsCellInLastRow(Cell cell)
        {
            return cell.Row == 8;
        }

        public static bool IsCellInFirstColumn(Cell cell)
        {
            return cell.Col == 0;
        }

        public static bool IsCellInLastColumn(Cell cell)
        {
            return cell.Col == 8;
        }
    }
}
