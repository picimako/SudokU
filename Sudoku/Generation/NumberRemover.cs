using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Controller;
using static Sudoku.Table.TableUtil;

namespace Sudoku.Generate
{
    class NumberRemover
    {
        #region Members

        private const int MAX_NUMBER_OF_REMEMBERED_REMOVED_CELL_INDECES = 6;
        private SudokuExercise se = SudokuExercise.get;
        private Random random = new Random();
        private GeneratorUtil util;

        #endregion

        #region Constructor

        public NumberRemover(GeneratorUtil util)
        {
            this.util = util;
        }

        #endregion

        #region Methods

        /// <summary> Számokat vesz ki.
        /// Addig teszi ezt, míg tud olyan számot kivenni, ami után a feladat még megoldható visszalépéses algoritmus
        /// használata nélkül.</summary>
        public void RemoveNumbersWithoutBackTrack()
        {
            int row = -1, col = -1, cellValue = 0;

            List<int> blockIndecesOfLastNRemovedCells = new List<int>(1);
            util.RectangularCells = new Dictionary<int, List<Cell>>();

            do
            {
                //Ha még nem vettem ki számot, akor generálok indexeket
                if (util.RemovedCellsAndValuesBeforeRemoval.Count == 0)
                    GenerateCellIndeces(ref row, ref col, ref cellValue);
                else
                {
                    RestoreToPreviousState(false);

                    //Ha 6 szám van a listában, akkor az elsőt kitöröljük, az új blokkszámot pedig a sor végére tesszük be
                    if (blockIndecesOfLastNRemovedCells.Count == MAX_NUMBER_OF_REMEMBERED_REMOVED_CELL_INDECES)
                        blockIndecesOfLastNRemovedCells.RemoveAt(0);

                    blockIndecesOfLastNRemovedCells.Add(BlockIndexByCellIndeces(row, col));

                    //Ha row és col benne van az előző 6 körben generált valamely indexekhez tartozó blokkban
                    //vagy a feladat táblában a row,col indexen lévő elem üres, akkor újragenerálom az indexeket
                    do GenerateCellIndeces(ref row, ref col, ref cellValue); 
                        while (blockIndecesOfLastNRemovedCells.Contains(BlockIndexByCellIndeces(row, col)) || cellValue == se.EMPTY);
                }

                //Kitörlöm a generált indexeken levő számot
                se.Exercise[0][row, col] = se.EMPTY;

                //Elmentem a törölt cella indexeit, és törlés előtti értékét
                util.RemovedCellsAndValuesBeforeRemoval.Add(new Cell(row, col), cellValue);

                //Cella törlése miatt a törölhető értékek beállítása a számtömbökben.
                se.Ctrl.RegenerateNumberTablesForRemovedValue(cellValue, row, col);
            } while (se.Ctrl.IsExerciseSolvableWithoutBackTrack(util.RemovedCellsAndValuesBeforeRemoval.Count));

            //Mivel az utolsó megoldás során marad üres cella, így a teljes megoldás értékeit megkapja Exercise
            Arrays.CopyJaggedThreeDimensionArray(se.Exercise, util.Solution);

            //Az utolsó törölt cella kivételével az összes többi cella értékét törlöm
            RestoreToPreviousState(true);
        }

        /// <summary> Számokat vesz ki, és mellé használja a visszalépéses algoritmust is. 
        /// Annyiszor vesz ki 2 darab számot, amekkora nehézséget a felhasználói felületen beállítottam. </summary>
        public void RemoveNumbersWithBackTrack()
        {
            /* k: számláló a 2 szám kivételéhez
             * szamlalo: számolja, hogy hányszor próbált meg 2 cellát törölni
             * uresCellakSzama: üres cellák száma a feladat korábbi állapotában*/
            int k, row = 0, col = 0, cellValue = 0, szamlalo, uresCellakSzama = 0;

            //Ebbe a tömbbe mentem el a feladatot ...
            int[][,] tombokMentes = Arrays.CreateInitializedArray();

            //Annyiszor veszek ki 2 darab számot, amekkora nehézséget beállítottam
            for (int n = 1; n <= util.Difficulty; n++)
            {
                //Ha az első 2 számot veszem ki
                if (n == 1)
                {
                    //Elmentem a feladat eddigi állapotát
                    Arrays.CopyJaggedThreeDimensionArray(tombokMentes, se.Exercise);
                    //Elmentem a feladatban levő üres cellák számát
                    uresCellakSzama = se.NumberOfEmptyCells;
                }
                else
                    RestoreExerciseToItsLastSolvableState(tombokMentes, ref uresCellakSzama);

                szamlalo = 0;

                do
                {
                    //Törlöm az eddig elmentett törölt cellákat
                    util.RemovedCellsAndValuesBeforeRemoval.Clear();

                    //Ha már legalább 2-szor próbálok meg törölni 2 cellát
                    if (++szamlalo > 1)
                    {
                        //Visszaállítom a tömbök mentett állapotát (az utoljára törölt 2 cella törlése előtti állapotot)
                        Arrays.CopyJaggedThreeDimensionArray(se.Exercise, tombokMentes);
                        //Visszaállítom az üres cellák számát
                        se.NumberOfEmptyCells = uresCellakSzama;
                    }

                    k = 0;

                    do
                    {
                        //Generálok olyan indexpárt, amely indexeken levő cella foglalt
                        do
                            GenerateCellIndeces(ref row, ref col, ref cellValue);
                        while (cellValue == se.EMPTY);

                        se.Exercise[0][row, col] = se.EMPTY;

                        //Mentem a törölt cella indexeit, és törlés előtti értékét
                        util.RemovedCellsAndValuesBeforeRemoval.Add(new Cell(row, col), cellValue);

                        //Törlöm a szükséges értékeket a számtömbökből
                        se.Ctrl.RegenerateNumberTablesForRemovedValue(cellValue, row, col);
                    } while (++k <= 1);

                    //Mivel töröltem 2 számot, az üres cellák száma nő 2-vel
                    se.NumberOfEmptyCells = uresCellakSzama + 2;

                    /* Backtrack algoritmus használatával megoldja a feladatot, és false értéket ad vissza, ha a feladatnak több megoldása van.
                     * Ekkor visszalép a do-while ciklus elejére, és megpróbál másik 2 számot kivenni.*/
                } while (!IsExerciseSolvableWithBackTrack());
            }

            RestoreExerciseToItsLastSolvableState(tombokMentes, ref uresCellakSzama);
        }

        /// <summary> Generates random row and col cell indeces</summary>
        /// <param name="row">Row index</param>
        /// <param name="col">Column index</param>
        /// <param name="cellValue">The value of the [row, col] cell in the actual exercise.</param>
        private void GenerateCellIndeces(ref int row, ref int col, ref int cellValue)
        {
            row = random.Next(0, 9);
            col = random.Next(0, 9);
            cellValue = se.Exercise[0][row, col];
        }

        /// <summary> A RemoveNumbersWithoutBackTrack eljárásban szükséges. A generált táblát egy előző állapotába állítja vissza.</summary>
        /// <param name="nemKellUtolso">Az mondja meg, hogy az utolsó elemet vissza kell-e állítani</param>
        private void RestoreToPreviousState(bool nemKellUtolso)
        {
            //Mivel az utolsó elemet (az utoljára kivett számot) nem kell visszaállítani, törlöm a lista utolsó elemét
            if (nemKellUtolso)
                util.RemovedCellsAndValuesBeforeRemoval.Remove(util.RemovedCellsAndValuesBeforeRemoval.Last().Key);

            //Törlöm az aktuális cella értékét
            //A törlendő cellák törlése
            RemoveCellsAndRegenerateTablesForRestoration();

            se.NumberOfEmptyCells = util.RemovedCellsAndValuesBeforeRemoval.Count;
        }

        /// <summary> A RemoveNumbersWithBackTrack eljárásban van szükség rá. Visszaállítja a feladatot az utolsó megoldható (röviden korábbi) állapotába.</summary>
        /// <param name="tombokMentes">Ebben vannak a korábbi állapot értékei(nek nagy része)</param>
        /// <param name="numberOfEmptyCells">Üres cellák száma a feladat korábbi állapotában</param>
        private void RestoreExerciseToItsLastSolvableState(int[][,] tombokMentes, ref int numberOfEmptyCells)
        {
            //Visszaállítom a "korábbi állapotot"
            Arrays.CopyJaggedThreeDimensionArray(se.Exercise, tombokMentes);

            //Végigmegyek az utolsó két törölt cellán
            //Az utoljára törölt két cellát ismét kitörlöm
            //A törölhető cellák törlése a számtömbökben
            RemoveCellsAndRegenerateTablesForRestoration();
            
            //Mivel töröltem 2 cellát, így az üres cellák száma nő 2-vel
            se.NumberOfEmptyCells = numberOfEmptyCells + 2;
        }

        private void RemoveCellsAndRegenerateTablesForRestoration()
        {
            foreach (KeyValuePair<Cell, int> cell in util.RemovedCellsAndValuesBeforeRemoval)
            {
                se.Exercise[0][cell.Key.Row, cell.Key.Col] = se.EMPTY;
                se.Ctrl.RegenerateNumberTablesForRemovedValue(cell.Value, cell.Key.Row, cell.Key.Col);
            }
        }

        /// <summary> Meghívja a feladat vizsgálatához szükséges függvényt.</summary>
        /// <returns> Ha a feladatnak egy megoldása van, akkor true-val, egyébként pedig false-szal tér vissza </returns>
        private bool IsExerciseSolvableWithBackTrack()
        {
            /* Megoldja a feladatot.
             * A megoldást a megoldottTabla[0]-ba adja vissza, és visszatér true-val ha a feladatnak egy megoldása van,
             * egyébként false-szal
             Control-ban a feladat értékei vannak.*/
            return new BackTrackSolver().SolveExerciseWithBackTrack();
        }

        #endregion
    }
}
