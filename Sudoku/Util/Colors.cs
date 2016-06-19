using Sudoku.Cells;
using System.Drawing;

namespace Sudoku.Util
{
    class Colors
    {
        public static Color INCORRECT = Color.Red;

        private static Color[] colors = new Color[] {new Color(), Color.LightCoral, Color.LightCyan, Color.LightGoldenrodYellow,
                Color.LightGreen, Color.LightPink, Color.LightSalmon, Color.LightSeaGreen,
                Color.LightSkyBlue, Color.LightSteelBlue, Color.LightYellow, Color.LightSlateGray,
                Color.LightGray, Color.Coral, Color.CornflowerBlue, Color.PaleVioletRed,
                Color.PeachPuff, Color.MediumSeaGreen, Color.IndianRed };

        public static Color[] GetColors()
        {
            return colors;
        }

        public static Color GetOriginalCellColorOf(Cell cell)
        {
            return CellHandler.IsCellSpecial(cell) ? Color.LightBlue : Color.White;
        }

        public static Color GetOriginalKillerCellColorOf(int currentCellCageIndex)
        {
            return currentCellCageIndex <= 10
                ? colors[currentCellCageIndex]
                : colors[currentCellCageIndex - 10];
        }
    }
}
