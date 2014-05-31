using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Sudoku.Generate;

namespace Sudoku.Controller
{
    public class SimpleSudokuController
    {
        protected SudokuExercise se = SudokuExercise.get;

        #region Constructor

        public SimpleSudokuController()
		{
            se.ExerciseType = SudokuType.SimpleSudoku;
		}

        #endregion

        /// <summary> A megadott fájlból beolvassa az értékeket. </summary>
        /// <param name="filePath"> A beolvasandó fájl útvonala </param>
        /// <returns>false-szal tér vissza, ha nincs megoldandó feladat vagy a beolvasás sikertelen volt, egyébként true-val</returns>
        public bool ReadSudoku(string filePath)
        {
            se.InitExercise();
            return ExerciseReader.ReadSudoku(filePath);
        }

        /// <summary>Feltölti a számokhoz tartozó tömböket a feladat értékei alapján</summary>
        /// <returns>false, ha van megoldandó feladat, true, ha nincs</returns>
        protected bool GenerateValuesInNumberTables()
        {
            //Ha a kitöltendő cellák száma nem 0 és nem 81, akkor van megoldandó feladat
            if (0 < se.NumberOfEmptyCells && se.NumberOfEmptyCells < 81)
            {
                //Végigmegyek a táblán
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        //Ha olyan cellán állok, ami nem üres
                        if (!se.IsCellEmpty(0, i, j))
                        {
                            /* Az aktuális cella indexeinek megfelelő cellát feltöltöm a számhoz tartozó tömbben
                             * pl.: tombok[0][2, 3] = 1 -> tombok[1][2, 3] = 1*/
                            se.Exercise[se.Exercise[0][i, j]][i, j] = se.Exercise[0][i, j];

                            //Végigmegyek a számtömbökön
                            for (int t = 1; t <= 9; t++)
                            {
                                /* Ha nem az aktuális szám tömbjébe akarok írni, akkor a t szám tömbjébe,
                                 * az előbb kitöltött cella indexeivel megegyező cellába írok -1-et. Ez jelzi, hogy a cella foglalt. */
                                if (t != se.Exercise[0][i, j])
                                    se.MakeCellOccupied(t, i, j);
                            }

                            //Beállítom a beírt szám táblájában a foglalt helyeket (házak alapján)
                            MakeHousesOccupied(se.Exercise[0][i, j], i, j);
                        }
                    }
                }

                //Van megoldható feladat, vissztérek true-val
                return true;
            }
            //Nincs megoldható feladat, ezért false-szal térek vissza
            else
                return false;
        }

        /// <summary>Amikor a generálás során kivettem egy számot egy cellából, akkor a kivett értéknek megfelelő szám tömbjét törlöm,
        /// a feladatban szereplő értékek alapján újragenerálom, majd a törlés miatt a nem t értékű számok tömbjeiben üresre (0-ra) állítom
        /// a megfelelő cellákat.</summary>
        public void RegenerateNumberTablesForRemovedValue(int removedValue, int deletedCellRow, int deletedCellColumn)
        {
            //A kivett értéknek megfelelő szám tömbjének értékeit törlöm
            se.Exercise[removedValue] = new int[9, 9];

            //Végigmegyek a feladaton
            for (int p = 0; p < 81; p++)
            {
                //Ha az aktuálisan vizsgált cella értéke a törölt cella törlés előtti értéke
                if (se.Exercise[0][p / 9, p % 9] == removedValue)
                {
                    //Akkor a removedValue szám tömbjében az aktuális cella removedValue értékű lesz
                    se.Exercise[removedValue][p / 9, p % 9] = removedValue;
                    //és az éppen kitöltött cellához viszonyítva beállítom a tömbben a foglalt helyeket
                    MakeHousesOccupied(removedValue, p / 9, p % 9);
                }
                //Ha azonban nem removedValue értékű
                else
                {
                    //és nem üres cella
                    if (!se.IsCellEmpty(0, p / 9, p % 9))
                        se.MakeCellOccupied(removedValue, p / 9, p % 9); //akkor a cella foglalt lesz
                }
            }

            /*Az összes számtömbből (kivéve abból a tömbből, melynek a száma a kivett értékkel egyezik meg) kitörlöm az [i, j] cella értékét akkor,
            * ha az num számtömb [i, j] cellájának egyik házában se szerepel az num szám*/
            for (int num = 1; num < 10; num++)
            {
                //Ha nem a törölt értéknek megfelelő számtömbről van szó, és ha egyik ház sem tartalmazza num-ot
                if (num != removedValue && !HousesContainValue(deletedCellRow, deletedCellColumn, num))
                {
                    se.Exercise[num][deletedCellRow, deletedCellColumn] = 0; //akkor a törlöm a megfelelő értéket
                }
            }
        }

        /// <summary>Megvizsgálja, hogy az value érték szerepel-e a feladat tömbjének col oszlopában. Ezt használom közvetetten a szamTablaKitolt()
        /// eljárásban is.</summary>
        /// <returns>Ha a megadott [_i,col] cella oszlopa tartalmazza az value értéket, akkor true, egyébként false.</returns>
        protected bool ColumnContainsValue(int _i, int col, int value)
        {
            //Végigmegyek az oszlopon
            for (int i = 0; i < 9; i++)
            {
                //Ha nem önmagával vizsgálom a cellát és megtalálom ertek-et az [i,j] cella oszlopában
                if (i != _i && se.Exercise[0][i, col] == value)
                    return true; //akkor visszatérek true-val
            }

            //Ha nem találtam meg a keresett értéket, akkor false-szal térek vissza
            return false;
        }

        /// <summary>Megvizsgálja, hogy az value érték szerepel-e a feladat tömbjének i sorában. Ezt használom közvetetten a szamTablaKitolt()
        /// eljárásban is.</summary>
        /// <returns>Ha a megadott [row,_j] cella sora tartalmazza az value értéket, akkor true, egyébként false.</returns>
        protected bool RowContainsValue(int row, int _j, int value)
        {
            //Végigmegyek a soron
            for (int j = 0; j < 9; j++)
            {
                //Ha nem önmagával vizsgálom a cellát és megtalálom ertek-et az [i,j] cella sorában
                if (j != _j && se.Exercise[0][row, j] == value)
                    return true; //akkor visszatérek true-val
            }

            //Ha nem találtam meg a keresett értéket, akkor false-szal térek vissza
            return false;
        }

        /// <summary>Megvizsgálja, hogy az value érték szerepel-e a feladat tömbjének [i,j] cellát tartalmazó blokkjában. Ezt használom közvetetten a szamTablaKitolt()
        /// eljárásban is.</summary>
        /// <returns>Ha a megadott [i,j] celláához tartozó blokk tartalmazza az value értéket, akkor true, egyébként false.</returns>
        protected bool BlockContainsValue(int i, int j, int value)
        {
            for (int r = se.StartRowOfBlockByRow(i); r <= se.EndRowOfBlockByRow(i); r++)
            {
                for (int p = se.StartColOfBlockByCol(j); p <= se.EndColOfBlockByCol(j); p++)
                {
                    //Ha nem önmagával vizsgálom a cellát és megtalálom ertek-et a [i,j] cella blokkjában
                    if (r != i && p != j && se.Exercise[0][r, p] == value)
                        return true;
                }
            }

            //Ha nem találtam meg a keresett értéket, akkor false-szal térek vissza
            return false;
        }

        /// <summary>Meghívja az egyes házakhoz tartozó érték-tartalmazást vizsgáló függvényeket 
        /// és azok visszaadott értékei szerint ad vissza értéket.</summary>
        /// <param name="i">A vizsgálandó cella sorindexe.</param>
        /// <param name="j">A vizsgálandó cella oszlopindexe.</param>
        /// <param name="value">A keresendő érték.</param>
        /// <returns>Ha egyik ház sem tartalmazza ertek-et, akkor false, egyébként true</returns>
        public virtual bool HousesContainValue(int i, int j, int value)
        {
            if (!RowContainsValue(i, j, value) && !ColumnContainsValue(i, j, value) && !BlockContainsValue(i, j, value))
                return false; //Egyik ház se tartalmazza

            //Valamelyik ház tartalmazza
            return true;
        }
		
        /// <summary>A megadott - [i,j] - cella blokkjában állítja foglaltra azokat a cellákat, melyek még nem voltak azok</summary>
		protected virtual void MakeBlockOccupied(int t, int i, int j)
        {
            //Végigmegyek a blokkon
            for (int r = i; r <= i + 2; r++)
            {
                for (int p = j; p <= j + 2; p++)
                {
                    //Ha a t szám tömbjének [r ,p] indexen lévő eleme szabad, akkor foglaltra állítom
                    MakeCellOccupied(t, r, p);
                }
            }
		
            //1. eset		//2. eset		//3. eset		//4. eset		//5. eset		//6. eset		//7. eset		//8. eset		//9. eset
            // __ __ __		// __ __ __		// __ __ __		// __ __ __		// __ __ __		// __ __ __		// __ __ __		// __ __ __		// __ __ __
            //|00|__|__|	//|__|01|__|	//|__|__|02|	//|__|__|__|	//|__|__|__|	//|__|__|__|	//|__|__|__|	//|__|__|__|	//|__|__|__|
            //|__|__|__|	//|__|__|__|	//|__|__|__|	//|10|__|__|	//|__|11|__|	//|__|__|12|	//|__|__|__|	//|__|__|__|	//|__|__|__|
            //|__|__|__|	//|__|__|__|	//|__|__|__|	//|__|__|__|	//|__|__|__|	//|__|__|__|	//|20|__|__|	//|__|21|__|	//|__|__|22|
        }

        /// <summary>A megadott - [i,j] - cella sorában és oszlopában állítja foglaltra azokat a cellákat, melyek még nem voltak azok</summary>
		protected void MakeRowAndColumnOccupied(int t, int row, int col)
        {
            //Végigmegyek a soron és az oszlopon
            for (int k = 0; k < 9; k++)
            {
                //Foglalt cellák beállítása a sorban,
                MakeCellOccupied(t, row, k);
                //oszlopban
                MakeCellOccupied(t, k, col);
            }
        }

        /// <summary>Kitölti a cellát -1-es értékkel (foglaltra állítja), ha a cella üres/szabad.</summary>
        /// <param name="t">A szám tömbje.</param>
        /// <param name="i">Sorindex.</param>
        /// <param name="j">Oszlopindex.</param>
        protected void MakeCellOccupied(int t, int i, int j)
        {
            if (se.IsCellEmpty(t, i, j))
                se.MakeCellOccupied(t, i, j);
        }

        /// <summary>Meghívja az egyes házakhoz tartozó azon eljárásokat, melyek a megadott cellához viszonyítva beállítják a foglalt cellákat.</summary>
        /// <param name="t">A tömb száma, amelyben állítani kell a foglalt cellákat.</param>
        /// <param name="i">A cella sorindexe.</param>
        /// <param name="j">A cella oszlopindexe.</param>
        public virtual void MakeHousesOccupied(int t, int i, int j)
        {
            //A megadott cella sorában és oszlopában,
            MakeRowAndColumnOccupied(t, i, j);
            //illetve blokkjában állítja be a foglalt cellákat
            MakeBlockOccupied(t, se.StartRowOfBlockByRow(i), se.StartColOfBlockByCol(j));
        }

        /// <summary>Megoldja a feladatot (amennyire tudja) visszalépéses algoritmus használata nélkül</summary>
        /// <returns>A sikerült teljesen megoldani (nem maradt üres cella), akkor true, egyébként false</returns>
        protected bool SolveExerciseWithoutBackTrack()
        {
            /* initialNumberOfEmptyCells: lementem az üres cellák számát, hogy le tudjam futtatni a kitöltést, ugyanis uresCellakSzama értéke
             * változni fog a kitöltés során
             * ures: az egyedüli üres cellába írható értéket tárolja majd*/
            int numberOfEmptyCellsToFill = se.NumberOfEmptyCells;
            int valueOfOnlyEmptyCellThroughoutNumberTables = -1;
            se.FirstEmptyCell = 0;

            //Változó annak tárolására, hogy volt-e értékbeírás
            bool wasCellFilling;
            for (int k = 1; k <= numberOfEmptyCellsToFill; k++)
            {
                wasCellFilling = false;

                //Mindig az első üres cellától kezdve vizsgálom a táblát
                for (int p = se.FirstEmptyCell; p < 81; p++)
                {
                    /* Ha a feladatban az aktuális cella nem üres, akkor lépek tovább a következő cellára,
                     * mert csak az üres cellákat kell vizsgálni.*/
                    if (!se.IsCellEmpty(0, p / 9, p % 9))
                        continue;

                    valueOfOnlyEmptyCellThroughoutNumberTables = -1;

                    //Összegyűjtöm azokat a számtömböket, amelyekben az aktuális cella üres
                    for (int num = 1; num < 10; num++)
                    {
                        //Ha üres, akkor elmentem
                        if (se.IsCellEmpty(num, p / 9, p % 9))
                        {
                            if (valueOfOnlyEmptyCellThroughoutNumberTables == -1)
                                valueOfOnlyEmptyCellThroughoutNumberTables = num;
                            else
                            {
                                valueOfOnlyEmptyCellThroughoutNumberTables = -1;
                                break;
                            }
                        }
                    }

                    /* Ha csak egy olyan táblát találtam, ahol az éppen vizsgált cella üres, akkor ebbe a cellába (a feladatba is)
                     * a megtalált tömb számát kell beírni.*/
                    if (valueOfOnlyEmptyCellThroughoutNumberTables != -1)
                    {
                        /* Ha írok az egyik táblába, akkor beírom a számot a feladat tömbjébe is
                         * uresek.First() tárolja a beírandó számot*/
                        se.Exercise[0][p / 9, p % 9] = se.Exercise[valueOfOnlyEmptyCellThroughoutNumberTables][p / 9, p % 9] = valueOfOnlyEmptyCellThroughoutNumberTables;
                        MakeHousesOccupied(valueOfOnlyEmptyCellThroughoutNumberTables, p / 9, p % 9);
                        wasCellFilling = true;
                        PostProcessCellFilling(p);
                    }
                    //Ha nem csak egy lehetséges beírható értéket találtam
                    else
                    {
                        /*i-ben és j-ben fogom tárolni a kitöltött cella sor-, 
                        * illetve oszlopindexét*/
                        int i, j;

                        //Végigmegyek a számtömbökön
                        for (int num = 1; num <= 9; num++)
                        {
                            //Kitölthető cellát keresek. Ha találtam,
                            if (FindOnlyEmptyCellInHouses(num, out i, out j))
                            {
                                wasCellFilling = true;
                                PostProcessCellFilling((i * 9) + j);
                                break;
                            }
                        }
                    }
                }

                //Ha nem lehetett számot beírni, akkor a vizsgálat befejezése
                if (!wasCellFilling)
                    break;
            }

            //Ha a feladat megoldható backtrack használata nélkül, tehát nem marad üres cella, akkor true, különben false a visszatérési érték
            return se.NumberOfEmptyCells == 0;
        }

        private void PostProcessCellFilling(int position)
        {
            se.NumberOfEmptyCells--;

            /* Ha kitöltöttem a cellát, akkor lefut egy rövid vizsgálat, ami megvizsgálja, hogy
             * az éppen kitöltött cella az első üres cella volt-e a feladatban.
             * Ha igen, akkor megkeresi az ezen cella után következő üres cellát, mert a keresést
             * elég csak attól a cellától elkezdeni.*/
            se.RecalculateFirstEmptyCell(position);
        }

        /// <summary> Ha van olyan ház, amelyben egyetlen egy üres cella van (oda biztos beírható az adott szám), akkor azt kitölti. </summary>
        /// <param name="num"> A tömb indexe, ahol keresni kell. (a beírandó szám) </param>
        /// <param name="i"> A kitöltött cella sorindexe vagy -1</param>
        /// <param name="j"> A kitöltött cella oszlopindexe vagy -1</param>
        /// <returns> Ha talált egyedüli üres cellát egy házban, akkor true-val, egyébként false-szal. </returns>
        private bool FindOnlyEmptyCellInHouses(int num, out int i, out int j)
        {
            //Blokkban való kereséshez
            Pair emptyCell = new Pair();

            //Végigmegyek az összes házon
            for (int k = 0; k < 9; k++)
            {
                /* tombok[r] k indexű során megy végig. Ha talál egyedüli üres cellát, akkor visszaadja, hogy a k indexű sorban melyik indexű elem az üres
                 * egyébként pedig -1-et*/
                if ((j = FindOnlyEmptyCellInRow(num, k)) > 0)
                {
                    //beírom a megfelelő tömbökbe az r számot, és minden számtömbben beállítom a foglalt cellákat
                    PutNumToExerciseAndMakeCellsOccupied(num, k, j);
                    /* megnézem, hogy a az az indexű cella, ahova most beírtam r-t, szerepel-e egy olyan tömbben, amiben még van 4 üres cella
                     * ha van ilyen, akkor elvégzi a megfelelő lépéseket*/

                    i = k;
                    return true;
                }
                /* tombok[r] k indexű oszlopán megy végig. Ha talál egyedüli üres cellát, akkor visszaadja, hogy a k indexű oszlopban melyik indexű elem
                 * az üres, egyébként pedig -1-et*/
                else if ((i = FindOnlyEmptyCellInColumn(num, k)) > 0)
                {
                    PutNumToExerciseAndMakeCellsOccupied(num, i, k);

                    j = k;
                    return true;
                }
                /* tombok[num] k index-szel jelzett blokkján megy végig, és index-be belerakja a megtalált üres cella indexeit
                 * ha egy üres cellát talált, akkor visszatér true-val, egyébként false-szal*/
                else if (FindOnlyEmptyCellInBlock(num, k, out emptyCell))
                {
                    PutNumToExerciseAndMakeCellsOccupied(num, emptyCell.i, emptyCell.j);

                    i = emptyCell.i;
                    j = emptyCell.j;
                    return true;
                }
            }

            i = j = -1;

            return false;
        }

        /// <summary> Beírja num-ot a megfelelő táblákba, és beállítja a foglalt cellákat </summary>
        /// <param name="num"> A beírandó szám </param>
        /// <param name="i"> A kitöltött cella sorindexe </param>
        /// <param name="j"> A kitöltött cella oszlopindexe </param>
        private void PutNumToExerciseAndMakeCellsOccupied(int num, int i, int j)
        {
            //A feladat, majd num saját számtömbjébe beírom num-ot
            se.Exercise[0][i, j] = se.Exercise[num][i, j] = num;

            //Végigmegyek a számtömbökön
            for (int t = 1; t <= 9; t++)
            {
                //Ha nem tombok[r]-be akarok írni, akkor tombok[k]-ba írok -1-et az előbb kitöltött cella indexeivel megegyező cellába
                if (t != num)
                    se.MakeCellOccupied(t, i, j);
            }

            MakeHousesOccupied(num, i, j);
        }

        /// <summary>Megoldja a feladatot visszalépéses algoritmus használata nélkül.
        /// Akkor oldható meg backtrack nélkül egy feladat, ha csak a sima kitöltést elvégezve nem marad üres cella a táblában.
        /// Ez a függvény eldönti, hogy a feladat megoldaható-e backtrack nélkül, sima kitöltéssel vagy sem.
        /// </summary>
        /// <param name="numberOfEmptyCells">Az üres cellák száma</param>
        /// <returns>Ha megoldható backtrack használata nélkül, akkor true, egyébként false</returns>
        public bool IsExerciseSolvableWithoutBackTrack(int numberOfEmptyCells)
        {
            //az üres cellák száma a generáláskori üres cellák száma lesz
            se.NumberOfEmptyCells = numberOfEmptyCells;

            //majd a megold() függvény true-val tér vissza, ha a feladat megoldható, különben false-szal 
            return SolveExerciseWithoutBackTrack();
        }

        /// <summary>Megoldja a beolvasott feladatot és visszaadja a feladat megoldását</summary>
        public void SolveExercise()
        {
            GenerateValuesInNumberTables();

            //Létrehozok egy tömböt a beolvasott feladat elmentésére
            int[][,] exerciseBackup;
            CommonUtil.InitializeArray(out exerciseBackup);

            //Elmentek minden táblát (a beolvasott feladatot)
            CommonUtil.CopyJaggedThreeDimensionArray(exerciseBackup, se.Exercise);
            int originalNumberOfEmptyCells = se.NumberOfEmptyCells;

            //Megoldom a feladatot. Ha maradt üres cella, akkor backtrack-et is használni kell,
            if (!SolveExerciseWithoutBackTrack())
                se.Solution = SolveExerciseWithBackTrack();
            else //ha pedig nem maradt üres cella
            {
                se.Solution = new int[9, 9];
                //majd belemásolom a megoldott feladat értékeit
                CommonUtil.CopyTwoDimensionArray(se.Solution, se.Exercise[0]);
            }

            /* tombok-be visszaállítom a feladat beolvasás és számtömb feltöltés utáni állapotát, mivel ezekkel a táblaállapotokkal fog dolgozni
             * a program a feladat kitöltésekor. Az üres cellák számát is visszaállítom*/
            CommonUtil.CopyJaggedThreeDimensionArray(se.Exercise, exerciseBackup);
            se.NumberOfEmptyCells = originalNumberOfEmptyCells;
        }

        /// <summary>Visszalépéses algoritmus használatával megoldja a feladatot</summary>
        protected int[,] SolveExerciseWithBackTrack()
        {
            new BackTrackSolver().SolveExerciseWithBackTrack();
            //Visszatérek a megoldott feladattal
            return se.Exercise[0];
        }

        private bool IsThereNotFillableRow(int num, ref bool van, ref bool teli)
        {
            return IsThereNotFillableRowOrColumn(num, ref van, ref teli, true);
        }

        private bool IsThereNotFillableColumn(int num, ref bool van, ref bool teli)
        {
            return IsThereNotFillableRowOrColumn(num, ref van, ref teli, false);
        }

        private bool IsThereNotFillableRowOrColumn(int t, ref bool van, ref bool teli, bool row)
        {
            //Végigmegyek a soron/oszlopon
            for (int i = 0; i < 9; i++)
            {
                //Csak -1-esek vannak
                teli = true;

                //Végigmegyek az oszlopon/soron
                for (int j = 0; j < 9; j++)
                {
                    //Ha nem csak -1-es van, akkor teli értéke false lesz, és befejezem a sor vizsgálatát
                    if (row ? IsThereOccupiedCell(t, i, j, ref teli) : IsThereOccupiedCell(t, j, i, ref teli))
                        break;
                }

                //van értéke van||teli lesz
                van |= teli;

                //Ha van olyan sor, ami csak -1-eseket tartalmaz, akkor visszatérek true-val
                if (van)
                    return true;
            }

            return false;
        }

        /// <summary>Megvizsgálja, hogy van-e olyan ház, ahol csak -1 értékek szerepelnek, a t szám viszont nem</summary>
        /// <param name="t">A vizsgálandó tábla indexe</param>
        /// <returns>Ha van egyetlen egy olyan ház is, ahol a vizsgálat igaz értéket ad, akkor true, egyébként false</returns>
        public bool IsThereNotFillableHouseForNumber(int t)
        {
            /* van: az eddig vizsgált összes cella teliségét tárolja
             * teli: true az éppen vizsgált ház csak -1-es értéket tartalmaz, egyébként false*/
            bool van = false, teli = false;

            if (IsThereNotFillableRow(t, ref van, ref teli) || IsThereNotFillableColumn(t, ref van, ref teli)
                || IsThereNotFillableBlock(t, ref van, ref teli))
            {
                return true;
            }

            return false;
        }

        private bool IsThereNotFillableBlock(int num, ref bool van, ref bool teli)
        {
            //Végigvizsgálom az összes blokkot
            for (int bl = 0; bl < 9; bl++)
            {
                //Csak -1-esek vannak
                teli = true;

                //Végigmegyek a blokkon
                for (int i = se.StartRowOfBlockByBlockIndex(bl); i <= se.EndRowOfBlockByBlockIndex(bl); i++)
                {
                    for (int j = se.StartColOfBlockByBlockIndex(bl); j <= se.EndColOfBlockByBlockIndex(bl); j++)
                    {
                        //Ha nem csak -1-es van, akkor teli értéke false lesz, és befejezem a blokk vizsgálatát
                        if (IsThereOccupiedCell(num, i, j, ref teli))
                            break;
                    }

                    if (!teli)
                        break;
                }

                //van értéke van||teli lesz
                van |= teli;

                //Ha van olyan blokk, ami csak -1-eseket tartalmaz, akkor visszatérek true-val
                if (van)
                    return true;
            }
            return false;
        }

        private bool IsThereOccupiedCell(int num, int i, int j, ref bool teli)
        {
            //Ha nem csak -1-es van, akkor teli értéke false lesz, és befejezem az oszlop vizsgálatát
            if (se.Exercise[num][i, j] != -1)
            {
                teli = false;
                return true;
            }

            return false;
        }

        /// <summary>tombok[t] i indexű során megy végig és megvizsgálja, hogy van-e egyedüli üres cella ebben a házban</summary>
        /// <param name="t">A beírt szám</param>
        /// <param name="i">A vizsgálandó sor</param>
        /// <returns>Ha talál egyedüli üres cellát, akkor visszaadja, hogy az i indexű sorban melyik indexű elem az üres,
        /// egyébként pedig -1-et</returns>
        public int FindOnlyEmptyCellInRowOrColumn(int t, int i, bool sor)
        {
            //Lista az üresnek talált cellák oszlopindexének
            List<int> list = new List<int>(1);
            //Végigmegyek a soron/oszlopon
            for (int k = 0; k < 9; k++)
            {
                //Ha az aktuális cella üres, akkor elmentem az oszlop/sorindexet
                if (sor ? se.IsCellEmpty(t, i, k) : se.IsCellEmpty(t, k, i))
                    list.Add(k);

                //Ha már 2 cella is el van mentve, akkor nem kell tovább vizsgálni
                if (list.Count == 2)
                    return -1;
            }

            /*Ha egyetlen üres cellát találtam, akkor visszatérek a cella oszlop/sorindexével
             * Ha nem találtam egy cellát se, akkor -1-gyel térek vissza*/
            return list.Count == 1 ? list.First() : -1;
        }

        public int FindOnlyEmptyCellInRow(int t, int i)
        {
            return FindOnlyEmptyCellInRowOrColumn(t, i, true);
        }

        public int FindOnlyEmptyCellInColumn(int t, int j)
        {
            return FindOnlyEmptyCellInRowOrColumn(t, j, false);
        }

        /// <summary>tombok[num] blockIndex-szel jelzett blokkján megy végig, és ind-be belerakja a megtalált üres cella indexeit</summary>
        /// <param name="num">A beírt szám</param>
        /// <param name="blockIndex">A vizsgálandó blokk száma</param>
        /// <param name="indeces">Ebbe adom vissza a megtalált üres cella indexeit</param>
        /// <returns>Ha egy üres cellát talált, akkor visszatér true-val, egyébként false-szal</returns>
        public bool FindOnlyEmptyCellInBlock(int num, int blockIndex, out Pair indeces)
        {
            //A kimenő paraméterben lesznek az üresen talált cella indexei
            indeces = new Pair();

            /* i: a bszam sorszámú blokk bal felső cellájának sorindexe
             * j: a bszam sorszámú blokk bal felső cellájának oszlopindexeindexe
             * elemszam: hány üres cellát találtam eddig a blokkban*/
            int i = blockIndex - (blockIndex % 3);
            int j = (blockIndex % 3) * 3;
            int numberOfEmptyCellsInBlock = 0;

            //Végigmegyek a blokkon
            for (int r = i; r <= i + 2; r++)
            {
                for (int p = j; p <= j + 2; p++)
                {
                    if (se.IsCellEmpty(num, r, p))
                    {
                        //Növelem az numberOfEmptyCellsInBlock-ot, és megnézem, hogy ha már 2 üres cellát találtam, akkor visszatérek false-szal
                        if (++numberOfEmptyCellsInBlock == 2)
                            return false;

                        //Egyébként elmentem az üres cella indexeit
                        indeces.i = r;
                        indeces.j = p;
                    }
                }
            }

            /* Ha egyetlen egy üres cellát találtam, akkor true-val térek vissza
             * Ha nem találtam egyetlen egy üres cellát se, akkor pedig false-szal*/
            return numberOfEmptyCellsInBlock == 1;
        }

        /// <summary>Azokat a cellákat gyűjti össze, melyek olyan blokkokban vannak, amelyekben csak hanyCella darab üres cella van.</summary>
        /// <param name="num">A tábla, ahol keresni kell</param>
        /// <param name="numberOfSoughtEmptyCells">Ennyi üres cellát keresek egy blokkban</param>
        /// <returns>A megtalált üres cellákat tároló lista</returns>
        public List<Pair> FindXNumberOfEmptyCellsInBlocks(int num, int numberOfSoughtEmptyCells)
        {
            /* tempList: ez tárolja az adott blokkban talált üres cellákat
             * finalList: ez tárolja majd az összes eredményül kapott üres cellákat*/
            List<Pair> emptyCellsInBlock = new List<Pair>();
            List<Pair> allEmptyCells = new List<Pair>();

            //Végigmegyek az egyes blokkokon
            for (int b = 0; b < 9; b++)
            {
                //Még nem találtam üres cellát
                emptyCellsInBlock.Clear();

                //Végigmegyek az adott blokkon
                for (int i = b - (b % 3); i <= b - (b % 3) + 2; i++)
                {
                    for (int j = (b % 3) * 3; j <= (b % 3) * 3 + 2; j++)
                    {
                        //Ha találok egy üres cellát, akkor elmentem az emptyCellsInBlock-ba
                        if (se.IsCellEmpty(num, i, j))
                            emptyCellsInBlock.Add(new Pair(i, j));

                        //Ha már több cellát találtam üresen, mint amennyit keresek, akkor befejezem a vizsgálatot
                        if (emptyCellsInBlock.Count == numberOfSoughtEmptyCells + 1)
                            break;
                    }

                    if (emptyCellsInBlock.Count == numberOfSoughtEmptyCells + 1)
                        break;
                }

                //Ha az emptyCellsInBlock numberOfSoughtEmptyCells elemet tartalmaz (ennyi üres cellát találtam az aktuális blokkban), akkor az mehet az allEmptyCells-be
                if (emptyCellsInBlock.Count == numberOfSoughtEmptyCells)
                    allEmptyCells.AddRange(emptyCellsInBlock);
            }

            //Visszaadom az eredményül kapott cellákat
            return allEmptyCells;
        }

        /// <summary>tombok[r]-ből gyűjti össze az üres cellákat</summary>
        /// <param name="num">A vizsgálandó tömb indexe</param>
        /// <returns>Az üres cellákat tároló listával tér vissza.</returns>
        public List<Pair> FindEmptyCellsInNumberTable(int num)
        {
            //Lista az üres cellák tárolására
            List<Pair> list = new List<Pair>();

            //Végigmegyek a tömbön
            for (int p = 0; p < 81; p++)
            {
                //Ha találok üres cellát,
                if (se.IsCellEmpty(num, p / 9, p % 9))
                {
                    //akkor azt elmentem lista-ba
                    list.Add(new Pair(p / 9, p % 9));

                    //Ha 4-nél több üres cellát találtam, akkor befejezem a keresést
                    if (list.Count == 5)
                        break;
                }
            }

            //Az üres cellák visszaadása
            return list;
        }
    }
}