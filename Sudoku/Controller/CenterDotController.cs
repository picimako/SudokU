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

        /// <summary> Konstruktor, amely meghívja az ősosztály konstruktorát, és beállítja a feladat fajtáját </summary>
        /// <param name="killer">Megmondja, hogy a feladat Killer Sudoku lesz-e vagy sem</param>
        public CenterDotController() : base()
        {
            se.ExerciseType = SudokuType.CenterDot;
        }

        #endregion

        #region Methods
        #region Public

        //Meghívja az egyes házakhoz tartozó azon eljárásokat, melyek a megadott cellához viszonyítva beállítják a foglalt cellákat.
        public override void MakeHousesOccupied(int t, int i, int j)
        {
            //Meghívom az ősosztály ugyanezen eljárását (sor, oszlop, blokk)
            base.MakeHousesOccupied(t, i, j);

            //Ha az újonnan kitöltött cella valamelyik blokk középső cellája, akkor kitöltöm a blokkok középső celláiból álló házat
            if (CellIsAtMiddleOfBlock(i, j))
                MakeCenterCellsOccupied(t);
        }

        /* Meghívja az egyes házakhoz tartozó érték-tartalmazást vizsgáló függvényeket 
         * és azok visszaadott értékei szerint ad vissza értéket.*/
        public override bool HousesContainValue(int i, int j, int ertek)
        {
            //Ha egyik ház se tartalmazza ertek-et
            if (!base.HousesContainValue(i, j, ertek) && !CenterContainsValue(i, j, ertek))
                return false;

            //Ha valamelyik ház tartalmazza a beírt számot
            return true;
        }

        public static bool CellIsAtMiddleOfBlock(int i, int j)
        {
            return i % 3 == 1 && j % 3 == 1;
        }

        #endregion

        #region Private

        /// <summary>A blokkok középső celláiból álló házat tölti fel</summary>
        /// <param name="t">A kitöltendő tábla</param>
        private void MakeCenterCellsOccupied(int t)
        {
            //Végigmegyek a blokkok középső celláin
            for (int r = 1; r <= 7; r += 3)
            {
                for (int p = 1; p <= 7; p += 3)
                {
                    //Ha egy cella üres, akkor foglalt lesz
                    MakeCellOccupied(t, r, p);
                }
            }
        }

        /// <summary>Megvizsgálja, hogy a megadott cella értékét tartalmazza-e már a blokkok középső celláiból álló ház.
        /// Ezt a vizsgálatot csak akkor csinálja meg, hogy ha a megadott cella a középső cellák valamelyike</summary>
        /// <param name="i">A vizsgálandó cella sorindexe</param>
        /// <param name="j">A vizsgálandó cella oszlopindexe</param>
        /// <param name="value">A keresendő érték</param>
        /// <returns>Ha megtalálta ertek-et, akkor true, egyébként false. False akkor is, ha a megadott cella nem egy blokk középső cellája.</returns>
        private bool CenterContainsValue(int i, int j, int value)
        {
            //Ha a megadott cella nem valamelyik blokk középső cellája, akkor nem vizsgálódok
            if (!CellIsAtMiddleOfBlock(i, j))
                return false;

            //Egyébként végigmegyek a blokkok középső celláin
            for (int r = 1; r <= 7; r += 3)
            {
                for (int p = 1; p <= 7; p += 3)
                {
                    //Ha nem önmagával vizsgálom meg, és a vizsgált cella üres, akkor foglalt lesz
                    if (r != i && p != j && se.Exercise[0][r, p] == value)
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
