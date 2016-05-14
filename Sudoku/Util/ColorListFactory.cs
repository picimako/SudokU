using System.Drawing;

namespace Sudoku.Util
{
    class ColorListFactory
    {
        public Color[] GetColors()
        {
            return new Color[] {new Color(), Color.LightCoral, Color.LightCyan, Color.LightGoldenrodYellow,
                Color.LightGreen, Color.LightPink, Color.LightSalmon, Color.LightSeaGreen,
                Color.LightSkyBlue, Color.LightSteelBlue, Color.LightYellow, Color.LightSlateGray,
                Color.LightGray, Color.Coral, Color.CornflowerBlue, Color.PaleVioletRed,
                Color.PeachPuff, Color.MediumSeaGreen, Color.IndianRed};
        }
    }
}
