using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sudoku.Generate;

namespace Sudoku.Controller
{
    public class CenterDotController : SimpleSudokuController
    {
        #region Constructor

        public CenterDotController() : base()
        {
            se.ExerciseType = SudokuType.CenterDot;
        }

        #endregion

        #region Methods
        #region Public

        /// <summary>Marks all the not empty cells as occupied in the all the houses of the current cell.</summary>
        /// <param name="num">The number table.</param>
        public override void MakeHousesOccupied(int num, int row, int col)
        {
            base.MakeHousesOccupied(num, row, col);

            if (CellIsAtMiddleOfAnyBlock(row, col))
                MakeCenterCellsOccupied(num);
        }

        /// <summary>Inspects whether value is present in any houses of the current cell of the exercise.</summary>
        /// <returns>True in case of inclusion, otherwise false.</returns>
        public override bool HousesContainValue(int rowOfCurrentCell, int colOfCurrentCell, int value)
        {
            if (!base.HousesContainValue(rowOfCurrentCell, colOfCurrentCell, value)
                && !CenterContainsValue(rowOfCurrentCell, colOfCurrentCell, value))
                return false;

            return true;
        }

        public static bool CellIsAtMiddleOfAnyBlock(int row, int col)
        {
            return row % 3 == 1 && col % 3 == 1;
        }

        #endregion

        #region Private

        private void MakeCenterCellsOccupied(int num)
        {
            for (int row = 1; row <= 7; row += 3)
            {
                for (int col = 1; col <= 7; col += 3)
                {
                    MakeCellOccupied(num, row, col);
                }
            }
        }

        /// <summary>Inspects whether value is present in any center-cell of blocks of the exercise.</summary>
        /// <returns>True in case of inclusion, otherwise false.</returns>
        private bool CenterContainsValue(int rowOfCurrentCell, int colOfCurrentCell, int value)
        {
            if (!CellIsAtMiddleOfAnyBlock(rowOfCurrentCell, colOfCurrentCell))
                return false;

            for (int row = 1; row <= 7; row += 3)
            {
                for (int col = 1; col <= 7; col += 3)
                {
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
