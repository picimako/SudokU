using System.Collections.Generic;
using Sudoku.Cells;
using Sudoku.Controller;

namespace Sudoku.Generate
{
    sealed class GeneratorUtil
    {
        #region Members

        private SudokuExercise se = SudokuExercise.get;
        private int[][,] solution;

        #endregion

        #region Properties

        public int[][,] Solution
        {
            get { return solution; }
        }

        #endregion

        #region Methods

        public void InitializeGeneration()
        {
            if (!se.IsExerciseKiller)
            {
                solution = Arrays.CreateInitializedArray();
            }
        }

        //TODO: investigate moving this to SimpleSudokuController
        public void SetValueOfFilledCell(int numToFill, Cell cell, bool kellSzamtombKitolt)
        {
            SetValueOfFilledCell(numToFill, cell.Row, cell.Col, kellSzamtombKitolt);
        }

        //TODO: investigate moving this to SimpleSudokuController
        /// <summary> Fills in r into the proper tables and sets the occupied cells.</summary>
        /// <param name="numToFill">The number to fill in.</param>
        /// <param name="row">The row index of the filled in cell.</param>
        /// <param name="col">The column index of the filled in cell.</param>
        /// <param name="isNumberTableFillingNeeded">Whether occupied cells should be set in se.Exercise[numToFill]</param>
        public void SetValueOfFilledCell(int numToFill, int row, int col, bool isNumberTableFillingNeeded)
        {
            se.Exercise[0][row, col] = numToFill;
            se.Exercise[numToFill][row, col] = numToFill;

            for (int num = 1; num <= 9; num++)
            {
                //Ha nem tombok[r]-be akarok írni, akkor se.Exercise[num]-ba írok -1-et az előbb kitöltött cella indexeivel megegyező cellába
                if (num != numToFill)
                    se.Exercise[num][row, col] = se.OCCUPIED;
            }

            /* Ha el kell végezni, akkor elvégzem se.Exercise[numToFill]-ben a foglalt cellák beállítását
             * Azért adom meg ezt a paramétert, mert a kitöltés során lehet olyan eset, amikor az egész r tömbben már csak egyetlen egy
             * üres cella van, ekkor csak be kell írni a számot, de ebben a tömbben már felesleges a szükséges cellákat -1-re (foglaltra állítani)
             * mert már minden cella r vagy -1 értékű*/
            if (isNumberTableFillingNeeded)
                se.Ctrl.MakeHousesOccupied(numToFill, row, col);
        }

        #endregion
    }
}
