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

        /// <summary> Konstruktor, amely meghívja az ősosztály konstruktorát, és beállítja a feladat fajtáját </summary>
        /// <param name="killer">Megmondja, hogy a feladat Killer Sudoku lesz-e vagy sem</param>
        public SudokuXController() : base()
        {
            se.ExerciseType = SudokuType.SudokuX;
        }

        #endregion

        #region Methods
        #region Public

        //Meghívja az egyes házakhoz tartozó azon eljárásokat, melyek a megadott cellához viszonyítva beállítják a foglalt cellákat.
        public override void MakeHousesOccupied(int t, int i, int j)
        {
            //Meghívom az ősosztály ugyanezen eljárását (sor, oszlop, blokk)
            base.MakeHousesOccupied(t, i, j);

            if (CellIsInMainDiagonal(i, j))
                MakeMainDiagonalOccupied(t);

            if (CellIsInSideDiagonal(i, j))
                MakeSideDiagonalOccupied(t);
        }

        /* Meghívja az egyes házakhoz tartozó érték-tartalmazást vizsgáló függvényeket 
         * és azok visszaadott értékei szerint ad vissza értéket.*/
        public override bool HousesContainValue(int i, int j, int value)
        {
            //Ha egyik ház se tartalmazza value-t
            if (!base.HousesContainValue(i, j, value) && !DiagonalContainsValue(i, j, value))
                return false;

            //Ha valamelyik ház tartalmazza
            return true;
        }

        public static bool CellIsInMainDiagonal(int i, int j)
        {
            return i == j;
        }

        public static bool CellIsInSideDiagonal(int i, int j)
        {
            return i + j == 8;
        }

        public static bool CellIsInAnyDiagonal(int i, int j)
        {
            return CellIsInMainDiagonal(i, j) || CellIsInSideDiagonal(i, j);
        }

        #endregion

        #region Private 

        /// <summary>A megadott átlóban állítja be a foglalt cellákat</summary>
        /// <param name="t">A kitöltendő tábla</param>
        private void MakeMainDiagonalOccupied(int t)
        {
            //Végigmegyek az átlón
            for (int r = 0; r < 9; r++)
            {
                //Ha egy cella üres, akkor foglalt lesz
                MakeCellOccupied(t, r, r);
            }
        }

        private void MakeSideDiagonalOccupied(int t)
        {
            //Végigmegyek a mellékátlón
            for (int r = 8; r >= 0; r--)
            {
                //Ha egy cella üres, akkor foglalt lesz
                MakeCellOccupied(t, r, 8 - r);
            }
        }

        /// <summary>Megvizsgálja, hogy az átló (amelyben a megadott cella van) tartalmazza-e ertek értéket.</summary>
        /// <param name="i">A vizsgálandó cella sorindexe.</param>
        /// <param name="j">A vizsgálandó cella oszlopindexe.</param>
        /// <param name="value">A keresendő érték.</param>
        /// <returns>Ha a megadott [i,j] cella átlója tartalmazza az ertek értéket, akkor true, egyébként false.</returns>
        private bool DiagonalContainsValue(int i, int j, int value)
        {
            //Ha a főátlóban van a cella
            if (CellIsInMainDiagonal(i, j))
            {
                //Végigmegyek a főátlón
                for (int r = 0; r < 9; r++)
                {
                    //Ha nem önmagával vizsgálom a cellát és megtalálom ertek-et, akkor true-val térek vissza
                    if (r != i && se.Exercise[0][r, r] == value)
                        return true;
                }
            }
            //Egyébként ha a mellékátlóban van a cella
            else if (CellIsInSideDiagonal(i, j))
            {
                //Végigmegyek a mellékátlón
                for (int r = 8; r >= 0; r--)
                {
                    //Ha nem önmagával vizsgálom a cellát és megtalálom ertek-et, akkor true-val térek vissza
                    if (r != i && (8 - r) != j && se.Exercise[0][r, 8 - r] == value)
                        return true;
                }
            }

            //Nem volt ütközés, ezért false-szal térek vissza
            return false;
        }

        #endregion
        #endregion
    }
}
