using System.Collections.Generic;
using System.Linq;
using Sudoku.Cells;
using static Sudoku.Table.TableUtil;

namespace Sudoku.Controller.Finder
{
    class EmptyCellFinder
    {
        SudokuExercise se = SudokuExercise.get;

        /// <summary>Iterates through the 'i' row or column in the 'num' numbertable and searches for an only empty cell
        /// in the row or column</summary>
        /// <param name="i">The row or column to check.</param>
        /// <returns>
        /// The found row or column index if there is only one cell found.
        /// -1 if there is more or less then 1 cell is found.
        /// </returns>
        public int FindOnlyEmptyCellInRowOrColumn(int num, int i, bool rowToCheck)
        {
            List<int> emptyCells = new List<int>(2);
            //Since we are searching for only one cell, if there are two, the search doesn't need to continue
            for (int k = 0; k < 9 && emptyCells.Count <= 1; k++)
            {
                if (rowToCheck ? se.IsCellEmpty(num, i, k) : se.IsCellEmpty(num, k, i))
                    emptyCells.Add(k);
            }

            return emptyCells.Count == 1 ? emptyCells.First() : -1;
        }

        public int FindOnlyEmptyCellInRow(int num, int row)
        {
            return FindOnlyEmptyCellInRowOrColumn(num, row, rowToCheck: true);
        }

        public int FindOnlyEmptyCellInColumn(int num, int col)
        {
            return FindOnlyEmptyCellInRowOrColumn(num, col, rowToCheck: false);
        }

        /// <summary>Searches for the only empty cell in the given block of the passed number table.
        /// The indeces of the empty cell is stored in 'cell'.</summary>
        /// <param name="blockIndex">The index of the block to check.</param>
        /// <param name="cell">The only empty cell in the block.</param>
        /// <returns>True if there is only one empty cell in the block, otherwise false.</returns>
        public bool FindOnlyEmptyCellInBlock(int num, out Cell cell, int blockIndex)
        {
            cell = new Cell();

            int startRow = StartRowOfBlockByBlockIndex(blockIndex);
            int startCol = StartColOfBlockByBlockIndex(blockIndex);
            int endRow = EndRowOfBlockByBlockIndex(blockIndex);
            int endCol = EndColOfBlockByBlockIndex(blockIndex);
            int numberOfEmptyCellsInBlock = 0;

            for (int row = startRow; row <= endRow; row++)
            {
                for (int col = startCol; col <= endCol; col++)
                {
                    if (se.IsCellEmpty(num, row, col))
                    {
                        if (++numberOfEmptyCellsInBlock > 1)
                            return false;

                        cell.Row = row;
                        cell.Col = col;
                    }
                }
            }

            return numberOfEmptyCellsInBlock == 1;
        }

        /// <summary>
        /// Collect the cells (from the specified numbertable)
        /// that are placed in blocks that have 'numberOfSoughtEmptyCells' number of empty cells.
        /// <param name="numberOfSoughtEmptyCells">The number of empty cells needed.</param>
        /// TODO: make sure to check if the refactor didn't break anything
        public List<Cell> FindXNumberOfEmptyCellsInBlocks(int num, int numberOfSoughtEmptyCells)
        {
            List<Cell> emptyCellsInBlock = new List<Cell>();
            List<Cell> allEmptyCells = new List<Cell>();

            for (int b = 0; b < 9; b++)
            {
                //Haven't found empty cells
                emptyCellsInBlock.Clear();

                for (int row = StartRowOfBlockByBlockIndex(b); row <= EndRowOfBlockByBlockIndex(b) &&
                    emptyCellsInBlock.Count <= numberOfSoughtEmptyCells; row++)
                {
                    for (int col = StartColOfBlockByBlockIndex(b); col <= EndColOfBlockByBlockIndex(b) &&
                        emptyCellsInBlock.Count <= numberOfSoughtEmptyCells; col++)
                    {
                        if (se.IsCellEmpty(num, row, col))
                            emptyCellsInBlock.Add(new Cell(row, col));
                    }
                }

                if (emptyCellsInBlock.Count == numberOfSoughtEmptyCells)
                    allEmptyCells.AddRange(emptyCellsInBlock);
            }

            return allEmptyCells;
        }

        /// <summary>Collects the first maximum 4 empty cells in the 'num' numbertable.</summary>
        /// TODO: make sure to check if the refactor didn't break anything
        public List<Cell> FindEmptyCellsInNumberTable(int num)
        {
            List<Cell> emptyCells = new List<Cell>();

            int p = 0;
            while (p < se.LAST_CELL_INDEX && emptyCells.Count <= 4)
            {
                if (se.IsCellEmpty(num, p))
                    emptyCells.Add(new Cell(p / 9, p % 9));

                p++;
            }

            return emptyCells;
        }
    }
}
