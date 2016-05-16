using System.Windows.Forms;
using Sudoku.Controller;
using Sudoku.Generate;

namespace Sudoku.Cell
{
    class CellHandler
    {
        private static SudokuExercise se = SudokuExercise.get;
        private TextBox[,] guiTable;

        public CellHandler(TextBox[,] guiTable)
        {
            this.guiTable = guiTable;
        }

        public int FindNearestEditableCellLeft(int row, int col)
        {
            while (col > 0)
            {
                if (guiTable[row, --col].Enabled)
                    break;
            }
            return col;
        }

        public int FindNearestEditableCellRight(int row, int col)
        {
            while (col < 8)
            {
                if (guiTable[row, ++col].Enabled)
                    break;
            }
            return col;
        }

        public int FindNearestEditableCellUp(int row, int col)
        {
            while (row > 0)
            {
                if (guiTable[--row, col].Enabled)
                    break;
            }
            return row;
        }

        public int FindNearestEditableCellDown(int row, int col)
        {
            while (row < 8)
            {
                if (guiTable[++row, col].Enabled)
                    break;
            }
            return row;
        }

        public static bool IsCellSpecial(int row, int col)
        {
            return (se.ExerciseType == SudokuType.SudokuX && SudokuXController.CellIsInAnyDiagonal(row, col))
                || (se.ExerciseType == SudokuType.CenterDot && CenterDotController.CellIsAtMiddleOfAnyBlock(row, col));
        }
    }
}
