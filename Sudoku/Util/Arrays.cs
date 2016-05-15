namespace Sudoku
{
    abstract class Arrays
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
            //target = (int[,])source.Clone();
            for (int p = 0; p < 81; p++)
                target[p / 9, p % 9] = source[p / 9, p % 9];
        }

        /// <summary> Initializes the passed array parameter.</summary>
        public static void Initialize(out int[][,] array)
        {
            array = new int[10][,];
            for (int num = 0; num < 10; num++)
            {
                array[num] = new int[9, 9];
            }
        }
    }
}
