using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku
{
    abstract class CommonUtil
    {
        /// <summary>Az első paraméterben levő tömb értékeit a második paraméterben megadott tömb értékeivel írja felül</summary>
        /// <param name="target">Ennek a tömbnek lesznek új értékei</param>
        /// <param name="source">Ezen tömb értékeit adjuk értékül az első paraméterben megadott tömb értékeinek</param>
        public static void CopyJaggedThreeDimensionArray(int[][,] target, int[][,] source)
        {
            //Végigmegyek a tömbökön
            for (int t = 0; t < 10; t++)
            {
                CopyTwoDimensionArray(target[t], source[t]);
            }
        }

        /// <summary>Az első paraméterben levő tömb értékeit a második paraméterben megadott tömb értékeivel írja felül</summary>
        public static void CopyTwoDimensionArray(int[,] target, int[,] source)
        {
            //target = source.Clone(); ???????
            //Végigmegyek a tömbön
            for (int p = 0; p < 81; p++) //Átmásolom az értékeket
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

        /// <summary> Inicializálja a paraméterben megadott tömböt </summary>
        /// <param name="array"> Az inicializálandó tömb </param>
        public static void InitializeArray(out int[][,] array)
        {
            array = new int[10][,];
            for (int t = 0; t < 10; t++)
            {
                array[t] = new int[9, 9];
            }
        }
    }
}
