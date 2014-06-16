using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

namespace Sudoku
{
    abstract class CommonUtil
    {
        /// <summary>Copies the values in the source table to the target table.</summary>
        public static void CopyJaggedThreeDimensionArray(int[][,] target, int[][,] source)
        {
            for (int num = 0; num < 10; num++)
            {
                CopyTwoDimensionArray(target[num], source[num]);
            }
        }

        /// <summary>Copies the values in the source table to the target table.</summary>
        public static void CopyTwoDimensionArray(int[,] target, int[,] source)
        {
            //target = source.Clone(); ???????
            for (int p = 0; p < 81; p++)
                target[p / 9, p % 9] = source[p / 9, p % 9];
        }

        /// <summary>Megvizsgálja, hogy a feladat megoldásában van-e 0 értékű cella.</summary>
        /// <param name="solution">A feladat megoldása.</param>
        /// <returns>A nincs 0 értékű cella a megoldásban, akkor true, egyébként false</returns>
        public static bool IsExerciseCorrect(int[,] solution)
        {
            List<int> list = solution.OfType<int>().ToList<int>();
            return !list.Contains(0);
        }

        /// <summary> Initializes the passed array parameter.</summary>
        public static void InitializeArray(out int[][,] array)
        {
            array = new int[10][,];
            for (int num = 0; num < 10; num++)
            {
                array[num] = new int[9, 9];
            }
        }

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
