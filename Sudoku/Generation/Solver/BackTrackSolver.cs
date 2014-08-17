using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Sudoku.Controller;

namespace Sudoku.Generate
{
    class BackTrackSolver
    {
        #region Members

        private int numberOfSolutionsFound = 0;
        private SudokuExercise se = SudokuExercise.get;

        #endregion

        #region Methods
        #region Constructor

        /// <summary>Solves the exercise, first without backtrack (while it is possible), then with backtrack.</summary>
        /// <returns>True if there is only one solution found.</returns>
        public bool SolveExerciseWithBackTrack()
        {
            se.Ctrl.IsExerciseSolvableWithoutBackTrack(se.NumberOfEmptyCells);
            Solve(0);
            return numberOfSolutionsFound == 1;
        }

        #endregion

        #region Private

        /// <summary>Leellenőrzi, hogy a feladat [i,j] indexű cellájába beírható-e az n szám</summary>
        /// <param name="i">A vizsgálandó cella sorindexe</param>
        /// <param name="j">A vizsgálandó cella oszlopindexe</param>
        /// <param name="n">A vizsgálandó érték</param>
        /// <returns>Ha beírható a szám a megadott helyre, akkor true, egyébként false</returns>
        private bool ellenorzes(int i, int j, int n)
        {
            /* Ha nem tartalmazza egyik ház sem, tehát beírható n, akkor true
             * Ha nem írható be n (tehát tartalmazza), akkor false*/
            return !se.Ctrl.HousesContainValue(i, j, n);
        }

        /// <param name="p">Start position</param>
        private void Solve(int p)
        {
            p = FindFirstEmptyCellAfterP(p);

            //I've already filled this with a number
            if (se.IsCellTheLastInTable(p))
            {
                numberOfSolutionsFound++;
            }
            else
            {
                //Végigmegyek a számokon
                for (int i = 1; i <= 9; i++)
                {
                    //Beírom az aktuális cellába a számot
                    se.Exercise[0][p / 9, p % 9] = i;

                    /* Leellenőrzöm, hogy beírható-e, és ha igen, 
                     * akkor nézem a következő üres cellát, hogy mit írhatok oda be*/
                    if (ellenorzes(p / 9, p % 9, i))
                        Solve(p + 1);
                }

                //Törlöm a beírt értéket
                se.Exercise[0][p / 9, p % 9] = 0;
            }
        }

        private int FindFirstEmptyCellAfterP(int pos)
        {
            int p = pos;
            while (p < 81)
            {
                if (se.Exercise[0][p / 9, p % 9] == 0)
                    break;

                p++;
            }
            return p;
        }

        #endregion
        #endregion
    }
}
