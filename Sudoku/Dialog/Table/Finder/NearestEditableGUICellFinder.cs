using System.Windows.Forms;

namespace Sudoku.Dialog.Table.Finder
{
    class NearestEditableGUICellFinder
    {
        private TextBox[,] guiTable;

        public NearestEditableGUICellFinder(TextBox[,] guiTable)
        {
            this.guiTable = guiTable;
        }

        /// <summary> Finds the nearest editable cell on the UI.</summary>
        /// <param name="cell">The cell the search is done correlated to.</param>
        /// <param name="keyCode">The direction it searches towards.</param>
        /// <returns></returns>
        public Cell FindNearestEditableCellComparedTo(Cell cell, Keys keyCode)
        {
            int row = cell.Row, col = cell.Col;
            switch (keyCode)
            {
                case Keys.Left:
                    col = FindNearestEditableCellLeftTo(row, col);
                    break;
                case Keys.Right:
                    col = FindNearestEditableCellRightTo(row, col);
                    break;
                case Keys.Up:
                    row = FindNearestEditableCellUpFrom(row, col);
                    break;
                case Keys.Down:
                    row = FindNearestEditableCellDownFrom(row, col);
                    break;
            }
            return new Cell(row, col);
        }

        private int FindNearestEditableCellLeftTo(int row, int col)
        {
            while (col > 0)
            {
                if (guiTable[row, --col].Enabled)
                    break;
            }
            return col;
        }

        private int FindNearestEditableCellRightTo(int row, int col)
        {
            while (col < 8)
            {
                if (guiTable[row, ++col].Enabled)
                    break;
            }
            return col;
        }

        private int FindNearestEditableCellUpFrom(int row, int col)
        {
            while (row > 0)
            {
                if (guiTable[--row, col].Enabled)
                    break;
            }
            return row;
        }

        private int FindNearestEditableCellDownFrom(int row, int col)
        {
            while (row < 8)
            {
                if (guiTable[++row, col].Enabled)
                    break;
            }
            return row;
        }
    }
}
