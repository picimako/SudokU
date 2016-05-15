using System.Drawing;
using System.Windows.Forms;

namespace Sudoku.Util
{
    class FontUtil
    {
        public static void SetTextboxFont(TextBox cell, FontStyle fontStyle)
        {
            SetTextboxFont(cell, cell, fontStyle);
        }

        public static void SetTextboxFont(TextBox targetCell, TextBox sourceCell, FontStyle fontStyle)
        {
            targetCell.Font = new Font(sourceCell.Font.FontFamily, 19.5f, fontStyle);
        }
    }
}
