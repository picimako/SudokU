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
        //számolja, hogy a backtrack algoritmus hány megoldást talált a feladatra
        private int numberOfSolutions = 0;
        private SudokuExercise se = SudokuExercise.get;

        /// <summary>Megoldja a feladatot, és visszaadja, hogy 1 megoldása volt vagy nem</summary>
        /// <param name="megoldasTabla">Ebben a paraméterben adja vissza a feladat megoldását</param>
        /// <returns>Ha feladatnak egy megoldása van, akkor true, egyébként false</returns>
        public bool SolveExerciseWithBackTrack()
        {
            //Először backtrack használata nélkül (ameddig lehetséges)
            se.Ctrl.IsExerciseSolvableWithoutBackTrack(se.NumberOfEmptyCells);

            //majd backtrack algoritmus hasznátával oldom meg a feladatot
            megold(0);

            /* Ha 1 megoldása van a feladatnak, akkor visszatérek true-val
             * ha nem 1 megoldása volt, akkor false-szal*/
            return numberOfSolutions == 1;
        }

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

        /// <summary>Megoldja a feladatot</summary>
        /// <param name="p">Az indulási pozíció</param>
        private void megold(int p)
        {
            //Megkeresem a p pozíció utáni legelső üres cellát
            while (p < 81)
            {
                if (se.Exercise[0][p / 9, p % 9] == 0)
                    break;

                p++;
            }

            //Ha az utolsó cellán vagyok (tehát műr ide is beírtam egy számot)
            if (p == 81)
                //Találtam egy megoldást
                numberOfSolutions++;

            //Ha nem az utolsó cellán vagyok
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
                        megold(p + 1);
                }

                //Törlöm a beírt értéket
                se.Exercise[0][p / 9, p % 9] = 0;
            }
        }
    }
}
