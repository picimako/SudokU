namespace Sudoku.Table
{
    class TableUtil
    {
        public static int StartRowOfBlockByRow(int row)
        {
            return (row / 3) * 3;
        }

        public static int EndRowOfBlockByRow(int row)
        {
            return (row / 3) * 3 + 2;
        }

        public static int StartColOfBlockByCol(int col)
        {
            return StartRowOfBlockByRow(col);
        }

        public static int EndColOfBlockByCol(int col)
        {
            return EndRowOfBlockByRow(col);
        }

        public static int StartRowOfBlockByBlockIndex(int blockIndex)
        {
            return blockIndex - (blockIndex % 3);
        }

        public static int EndRowOfBlockByBlockIndex(int blockIndex)
        {
            return StartRowOfBlockByBlockIndex(blockIndex) + 2;
        }

        public static int StartColOfBlockByBlockIndex(int blockIndex)
        {
            return (blockIndex % 3) * 3;
        }

        public static int EndColOfBlockByBlockIndex(int blockIndex)
        {
            return StartColOfBlockByBlockIndex(blockIndex) + 2;
        }

        /// <summary>
        /// Numbering of blocks: from left to right, from top to bottom, starting with 0
        /// </summary>
        /// <param name="i">Row index of cell.</param>
        /// <param name="j">Column index of cell.</param>
        /// <returns>The block index determined by the row and column indeces.</returns>
        public static int BlockIndexByCellIndeces(int i, int j)
        {
            return (i / 3) * 3 + (j / 3);
        }
    }
}
