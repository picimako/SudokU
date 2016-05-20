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
    }
}
