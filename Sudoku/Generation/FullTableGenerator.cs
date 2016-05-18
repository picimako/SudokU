using System;
using System.Collections.Generic;
using Sudoku.Controller;

namespace Sudoku.Generate
{
    class FullTableGenerator
    {
        #region Members

        /// <summary>
        /// The maximum number of retries to fill in a number
        /// </summary>
        private const int MAX_NUMBER_OF_WRONG_GENERATIONS = 3;
        private SudokuExercise se = SudokuExercise.get;
        private GeneratorUtil util;
        private Random random;

        #endregion

        #region Constructor

        public FullTableGenerator(GeneratorUtil util)
        {
            this.util = util;
            this.random = new Random();
        }

        #endregion

        #region Methods

        #region Public

        /// <summary> Generates a fully solved excercise of proper sudoku type. </summary>
        public bool GenerateFullTableFromEmptyTable()
        {
            //In case of Sudoku-X it contains the value of the cell in the middle of the table
            int cellAtMiddleOfTable = -1;
            //The current number of tries to fill in a number with different combinations
            int currentTryToFillInNumber = 0;
            int[][,] tempTable;

            //For filling the first house in the table
            List<int> sudokuNumbers = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            //1. Azokat az indexeket fogja tárolni, melyek olyan blokkokhoz tartoznak amelyekben a blokkok közül a legkevesebb üres cella található
            List<Cell> cellsInTheMostlyFilledBlock = null;
            // For storing cell indeces whiches placement is rectangular (it will be 4 cells)
            List<Cell> rectangularCells = null;

            se.InitExercise();
            switch (se.ExerciseType)
            {
                case SudokuType.SimpleSudoku:
                    GenerateFirstBlock(sudokuNumbers);
                    break;
                case SudokuType.SudokuX:
                    if (!GenerateDiagonals(sudokuNumbers, out cellAtMiddleOfTable))
                        return false;
                    break;
                case SudokuType.CenterDot:
                    GenerateCenterDots(sudokuNumbers);
                    break;
            }

            Arrays.Initialize(out tempTable);

            int numberToFillIn = 0;
            while (++numberToFillIn <= 9)
            {
                for (int instanceOfCurrentNumber = GetNextInstanceOfNumber(cellAtMiddleOfTable, numberToFillIn);
                    instanceOfCurrentNumber <= 9; instanceOfCurrentNumber++)
                {
                    // Saving the current state of the generated exercise for possible restoration later
                    Arrays.CopyJaggedThreeDimensionArray(tempTable, se.Exercise);
                    if (numberToFillIn < 9 && AreCellsPlacedAsRectangle(ref rectangularCells, numberToFillIn))
                    {
                        util.RectangularCells.Add(numberToFillIn, rectangularCells);

                        /* Megvizsgálom az üresen maradt 4 cellát, hogy az numberToFillIn-nél kisebb indexű számtömbökben esetleg szintén üresen maradt 4 cella
                         * közül valamelyik indexei egyeznek-e a most üresen maradtak közül valamelyikével.*/
                        kozosCellaKeres(numberToFillIn);

                        // Moving to the next number to fill
                        break;
                    }

                    //Ha talál olyan cellát, ahova biztosan be lehet írni az adott számot, akkor beírja, és továbblép a következő számra
                    if (!FilledInOnlyEmptyCellInHouse(numberToFillIn, true))
                    {
                        int index;
                        int hanyCella = 2;

                        /* Megkeresem azokat a cellákat melyek olyan blokkokban szerepelnek, amelyekben az összes közül a legkevesebb üres cella fordul elő.
                         * Ehhez szükséges a hanyCella nevű változó.*/
                        do
                        {
                            //Lekérem az előbb leírtaknak megfelelő cellák indexeit. Ha 1-nél több ilyen cella van (minimum 2 lesz vagy pedig egy sem)
                            if ((cellsInTheMostlyFilledBlock = se.Ctrl.FindXNumberOfEmptyCellsInBlocks(numberToFillIn, hanyCella)).Count > 1)
                            {
                                index = random.Next(0, cellsInTheMostlyFilledBlock.Count);

                                //És oda írom be a számot
                                util.SetValueOfFilledCell(numberToFillIn, cellsInTheMostlyFilledBlock[index].row, cellsInTheMostlyFilledBlock[index].col, true);

                                /* Ha a cella, ahova épp beírtam egy számot olyan, hogy szerepel egy előző tábla 4 üresen maradt cellája között,
                                 * akkor annak a táblának a kitöltését be kell fejezni.
                                 * Ha viszont nincs mivel megvizsgálni, akkor felesleges meghívni magát az eljárást is*/
                                if (util.RectangularCells.Count > 0)
                                    egyezesKereses(cellsInTheMostlyFilledBlock[index].row, cellsInTheMostlyFilledBlock[index].col);

                                break;
                            }

                        //Ha nem találtam cellákat, akkor növelem a blokkban keresendő cellák minimum számát
                        } while (++hanyCella <= (10 - numberToFillIn)); //10-numberToFillIn üres cella lehet maximálisan egy blokkban
                    }

                    if (se.Ctrl.IsThereNotFillableHouseForNumber(numberToFillIn))
                    {
                        // Restoring the exercise to the state when the filling of the current number started
                        Arrays.CopyJaggedThreeDimensionArray(se.Exercise, tempTable);

                        numberToFillIn--;

                        //És ha a listaindexekben benne van numberToFillIn, akkor törlöm belőle
                        if (util.RectangularCells.ContainsKey(numberToFillIn))
                            util.RectangularCells.Remove(numberToFillIn);

                        if (++currentTryToFillInNumber == MAX_NUMBER_OF_WRONG_GENERATIONS)
                            return false;

                        //Mivel numberToFillIn-t előzőleg csökkentettem, ezután pedig növelni fogom, így numberToFillIn értéke nem fog változni
                        break;
                    }
                }
            }

            if (!se.IsExerciseKiller)
                //Elmentem a megoldott táblát
                Arrays.CopyJaggedThreeDimensionArray(util.Solution, se.Exercise);

            return true;
        }

        #endregion

        #region Private

        private int GetNextInstanceOfNumber(int cellAtMiddleOfTable, int numberToFillIn)
        {
            /* Ha a feladat Sudoku-X típusú és a beírandó szám nem egyezik a tábla középső cellájába írt értékkel, akkor az r számból a 3.
             * példányt kezdem beírni a tömbbe, egyébként pedig a másodikat.*/
            return (se.ExerciseType == SudokuType.SudokuX && cellAtMiddleOfTable != numberToFillIn) ? 3 : 2;
        }

        /// <summary>
        /// Decides if the provided cells are placed as rectangular, based on the method parameters.
        /// Example for 4 rectangular cells with x,y cell indeces: [1, 0], [1, 4], [5, 0], [5, 4]
        /// </summary>
        private bool AreCellsPlacedAsRectangle(ref List<Cell> rectangularCells, int numberToFillIn)
        {
            return (rectangularCells = se.Ctrl.FindEmptyCellsInNumberTable(numberToFillIn)).Count == 4 &&
                        (rectangularCells[0].row == rectangularCells[1].row
                        && rectangularCells[2].row == rectangularCells[3].row
                        && rectangularCells[0].col == rectangularCells[2].col
                        && rectangularCells[1].col == rectangularCells[3].col);
        }

        private void GenerateFirstBlock(List<int> sudokuNumbers)
        {
            /* Legelőször feltöltöm a bal felső blokkot számokkal; a számtáblákat is a beírt számoknak megfelelően töltöm ki.
             * p a blokk aktuális celláját jelenti*/
            for (int p = 0; p < 9; p++)
            {
                int index = GetRandomNumberFromRemainingNumbers(sudokuNumbers);

                util.SetValueOfFilledCell(sudokuNumbers[index], p / 3, p % 3, true);

                //Beírtam a számot, ezért kitörlöm a listából
                sudokuNumbers.Remove(sudokuNumbers[index]);
            }
        }

        private bool GenerateDiagonals(List<int> sudokuNumbers, out int cellAtMiddleOfTableParam)
        {
            int r;
            int cellAtMiddleOfTable = -1;
            for (int index = 0; index < 9; index++)
            {
                r = GetRandomNumberFromRemainingNumbers(sudokuNumbers);

                //Ha a tábla középső cellájába írtam be egy számot, akkor elmentem a beírt számot
                if (index == 4)
                    cellAtMiddleOfTable = sudokuNumbers[r];

                //Beállítom az értékeket
                util.SetValueOfFilledCell(sudokuNumbers[r], index, index, true);

                //Beírtam a számot, ezért kitörlöm a listából
                sudokuNumbers.Remove(sudokuNumbers[r]);
            }

            //Végigmegyek a mellékátlón
            for (r = 1; r <= 9; r += (r == cellAtMiddleOfTable - 1) ? 2 : 1)
            {
                //Az r szám tömbjének mellékátlójában összegyűjtöm azokat a cellákat, amelyek üresek, és elmentem őket listaint-be
                for (int i = 8; i >= 0; i--)
                {
                    if (se.Exercise[r][i, 8 - i] == 0)
                        sudokuNumbers.Add(i);
                }

                //Ha nincs üres cella, akkor kilépek
                if (sudokuNumbers.Count == 0)
                {
                    cellAtMiddleOfTableParam = cellAtMiddleOfTable;
                    return false;
                }

                //Ha van üres cella, akkor választok egyet közülük
                int index = GetRandomNumberFromRemainingNumbers(sudokuNumbers);

                //Beállítom a megfelelő értékeket
                util.SetValueOfFilledCell(r, sudokuNumbers[index], 8 - sudokuNumbers[index], true);

                //Törlöm a listát, mert ezután a következő számtömb mellékátlójának vizsgálatához kell
                sudokuNumbers.Clear();
            }
            cellAtMiddleOfTableParam = cellAtMiddleOfTable;
            return true;
        }

        private void GenerateCenterDots(List<int> sudokuNumbers)
        {
            //Végigmegyek a blokkok középső celláján
            for (int i = 1; i <= 7; i += 3)
            {
                for (int j = 1; j <= 7; j += 3)
                {
                    //Generálok egy számot, ez lesz beírva a blokk aktuális cellájába
                    int index = GetRandomNumberFromRemainingNumbers(sudokuNumbers);

                    util.SetValueOfFilledCell(sudokuNumbers[index], i, j, true);

                    //Beírtam a számot, ezért kitörlöm a listából
                    sudokuNumbers.Remove(sudokuNumbers[index]);
                }
            }
        }

        private int GetRandomNumberFromRemainingNumbers(List<int> numbers)
        {
            return new Random().Next(0, numbers.Count);
        }

        /// <summary> Az utoljára elmentett 4 cella indexeit megvizsgálja minden elmentett cella indexével, és ha talál egyezést,
        /// akkor kitölti a szükséges cellákat.</summary>
        /// <param name="r">A generálásnál éppen soron levő szám</param>
        private void kozosCellaKeres(int r)
        {
            //Végigmegyek a szótár elemein
            foreach (KeyValuePair<int, List<Cell>> _4cella in util.RectangularCells)
            {
                //Ha nem ugyanazt a 4 cellát akarom vizsgálni
                if (util.RectangularCells[r] != _4cella.Value)
                {
                    //Végigmegyek az r indexű tömbhöz tartozó 4 cellán (az épp félbehagyott tábla)
                    for (int i = 0; i < util.RectangularCells[r].Count; i++)
                    {
                        //Végiglépkedek az aktuálisan vizsgált 4 cellán
                        for (int j = 0; j < _4cella.Value.Count; j++)
                        {
                            //Ha találtam olyan cellát, amely mindkét vizsgált táblában közös
                            if (util.RectangularCells[r][i].row == _4cella.Value[j].row && util.RectangularCells[r][i].col == _4cella.Value[j].col)
                            {
                                //Beírom az aktuálisan vizsgált tábla egyik elemét
                                util.SetValueOfFilledCell(_4cella.Key, _4cella.Value[j].row, _4cella.Value[j].col, true);

                                //Beírom az aktuálisan vizsgált tábla egyik elemét
                                util.SetValueOfFilledCell(_4cella.Key, _4cella.Value[3 - j].row, _4cella.Value[3 - j].col, false);

                                //Törlöm a vizsgált 4 cellát listaindexek-ből
                                util.RectangularCells.Remove(_4cella.Key);

                                //Beírom a maradék 2 számot az r táblába
                                FilledInOnlyEmptyCellInHouse(r, true);
                                FilledInOnlyEmptyCellInHouse(r, false);

                                //Törlöm az r indexű táblához tartozó bejegyzést is
                                util.RectangularCells.Remove(r);

                                //Kilépek az eljárásból
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary> Ha van olyan ház, amelyben egyetlen egy üres cella van (oda biztos beírható az adott szám), akkor azt kitölti. </summary>
        /// <param name="r"> A tömb indexe, ahol keresni kell. (a beírandó szám) </param>
        /// <param name="kellSzamTombKitolt"> Azt mondja meg, hogy az ertekekBeallit eljárásban végre kell-e hajtani a foglalt helyeket beállító
        /// eljárást. </param>
        /// <returns> Ha talált egyedüli üres cellát egy házban, akkor true-val, egyébként false-szal. </returns>
        private bool FilledInOnlyEmptyCellInHouse(int r, bool kellSzamTombKitolt)
        {
            //Oszlopban és sorban való kereséshez
            int _i, _j;

            //Blokkban való kereséshez
            Cell index = new Cell();

            //Végigmegyek az összes házon
            for (int k = 0; k < 9; k++)
            {
                /* tombok[r] k indexű során megy végig. Ha talál egyedüli üres cellát, akkor visszaadja, hogy a k indexű sorban melyik indexű elem az üres
                 * egyébként pedig -1-et*/
                if ((_j = se.Ctrl.FindOnlyEmptyCellInRow(r, row: k)) > 0)
                {
                    //beírom a megfelelő tömbökbe az r számot, és minden számtömbben beállítom a foglalt cellákat
                    util.SetValueOfFilledCell(r, k, _j, kellSzamTombKitolt);
                    /* megnézem, hogy a az az indexű cella, ahova most beírtam r-t, szerepel-e egy olyan tömbben, amiben még van 4 üres cella
                     * ha van ilyen, akkor elvégzi a megfelelő lépéseket*/
                    egyezesKereses(k, _j);
                    return true;
                }
                /* tombok[r] k indexű oszlopán megy végig. Ha talál egyedüli üres cellát, akkor visszaadja, hogy a k indexű oszlopban melyik indexű elem
                 * az üres, egyébként pedig -1-et*/
                else if ((_i = se.Ctrl.FindOnlyEmptyCellInColumn(r, col: k)) > 0)
                {
                    util.SetValueOfFilledCell(r, _i, k, kellSzamTombKitolt);
                    egyezesKereses(_i, k);
                    return true;
                }
                /* tombok[r] k index-szel jelzett blokkján megy végig, és index-be belerakja a megtalált üres cella indexeit
                 * ha egy üres cellát talált, akkor visszatér true-val, egyébként false-szal*/
                else if (se.Ctrl.FindOnlyEmptyCellInBlock(r, out index, blockIndex: k))
                {
                    util.SetValueOfFilledCell(r, index.row, index.col, kellSzamTombKitolt);
                    egyezesKereses(index.row, index.col);
                    return true;
                }
            }

            return false;
        }

        /// <summary> Megkeresi, hogy az a cella, ahova beírta a soron levő számot, szerepel-e valamelyik előző tábla üresen maradt 4 cellája között.
        /// Ha igen, akkor a megtalált tábla kitöltését befejezi. </summary>
        /// <param name="i">A cella sorindexe</param>
        /// <param name="j">A cella oszlopindexe</param>
        private void egyezesKereses(int i, int j)
        {
            //Ha egyezést találok, akkor ebbe a változóba mentem a számtömb indexét
            int egyezoTabla = -1;

            //Elkezdek a szótár elemein végighaladni
            foreach (KeyValuePair<int, List<Cell>> _4cella in util.RectangularCells)
            {
                //l azt jelenti, hogy a 4-elemű lista hanyadik elemén állok éppen
                for (int l = 0; l < _4cella.Value.Count; l++)
                {
                    //Ha a 4 cella közül valamelyik indexeivel egyezést találok,  és befejezem a keresést
                    if (_4cella.Value[l].row == i && _4cella.Value[l].col == j)
                    {
                        /* Akkor elmentem a tábla indexét (ahol egyezést találtam)
                         * _4cella.Key: melyik táblában van a megtalált cella*/
                        egyezoTabla = _4cella.Key;
                        //Mivel találtam egyezést, nem vizsgálódok tovább
                        break;
                    }
                }

                if (egyezoTabla != -1)
                    break;
            }

            if (egyezoTabla != -1)
            {
                //Kitöltöm a maradék 2 szükséges cellát
                FilledInOnlyEmptyCellInHouse(egyezoTabla, true);
                FilledInOnlyEmptyCellInHouse(egyezoTabla, false);

                //Törlöm a szótárból a most kitöltött tábla sorszámát, illetve a hozzá tartozó 4 cella listáját
                util.RectangularCells.Remove(egyezoTabla);
            }
        }

        #endregion

        #endregion
    }
}
