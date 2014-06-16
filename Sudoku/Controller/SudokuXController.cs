using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Sudoku.Generate;

namespace Sudoku.Controller
{
    public class SudokuXController : SimpleSudokuController
    {
        #region Constructor

        public SudokuXController() : base()
        {
            se.ExerciseType = SudokuType.SudokuX;
        }

        #endregion

        #region Methods
        #region Public

        /// <summary>Marks all the not empty cells as occupied in the all the houses of the current cell.</summary>
        /// <param name="num">The number table.</param>
        public override void MakeHousesOccupied(int num, int row, int col)
        {
            base.MakeHousesOccupied(num, row, col);

            if (CellIsInMainDiagonal(row, col))
                MakeMainDiagonalOccupied(num);

            if (CellIsInSideDiagonal(row, col))
                MakeSideDiagonalOccupied(num);
        }

        /// <summary>Inspects whether value is present in any houses of the current cell of the exercise.</summary>
        /// <returns>True in case of inclusion, otherwise false.</returns>
        public override bool HousesContainValue(int rowOfCurrentCell, int colOfCurrentCell, int value)
        {
            if (!base.HousesContainValue(rowOfCurrentCell, colOfCurrentCell, value)
                && !DiagonalContainsValue(rowOfCurrentCell, colOfCurrentCell, value))
                return false;

            return true;
        }

        public static bool CellIsInMainDiagonal(int row, int col)
        {
            return row == col;
        }

        public static bool CellIsInSideDiagonal(int row, int col)
        {
            return row + col == 8;
        }

        public static bool CellIsInAnyDiagonal(int row, int col)
        {
            return CellIsInMainDiagonal(row, col) || CellIsInSideDiagonal(row, col);
        }

        #endregion

        #region Private 

        //Main diagonal: \
        private void MakeMainDiagonalOccupied(int num)
        {
            for (int r = 0; r < 9; r++)
            {
                MakeCellOccupied(num, r, r);
            }
        }

        //Side diagonal: /
        private void MakeSideDiagonalOccupied(int num)
        {
            for (int r = 8; r >= 0; r--)
            {
                MakeCellOccupied(num, r, 8 - r);
            }
        }

        /// <summary>Inspects whether value is present in the diagonal(s) of the current cell of the exercise.</summary>
        /// <returns>True in case of inclusion, otherwise false.</returns>
        private bool DiagonalContainsValue(int rowOfCurrentCell, int colOfCurrentCell, int value)
        {
            if (CellIsInMainDiagonal(rowOfCurrentCell, colOfCurrentCell))
            {
                for (int row = 0; row < 9; row++)
                {
                    if (row != rowOfCurrentCell && se.Exercise[0][row, row] == value)
                        return true;
                }
            }
            else if (CellIsInSideDiagonal(rowOfCurrentCell, colOfCurrentCell))
            {
                for (int row = 8; row >= 0; row--)
                {
                    int col = 8 - row;
                    if (row != rowOfCurrentCell && col != colOfCurrentCell && se.Exercise[0][row, col] == value)
                        return true;
                }
            }

            return false;
        }

        #endregion
        #endregion
    }
}
