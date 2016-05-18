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
        private Random random;
        private GeneratorUtil util;

        #endregion

        #region Constructor

        public NumberRemover(GeneratorUtil util)
        {
            this.util = util;
            random = new Random();
        }

        #endregion

        #region Methods

        /// <summary> i-be és j-be generál véletlenszámokat. Ezek lesznek a cellaindexek.</summary>
        /// <param name="i">Sorindex</param>
        /// <param name="j">Oszlopindex</param>
        /// <param name="cellValue">Az [i,j] indexű cella értéke a feladatban</param>
        private void GenerateCellIndeces(ref int i, ref int j, ref int cellValue)
        {
            i = random.Next(0, 9);
            j = random.Next(0, 9);
            cellValue = se.Exercise[0][i, j];
        }

        /// <summary> Számokat vesz ki. Addig teszi ezt míg tud olyan számot kivenni, ami után a feladat még megoldható visszalépéses algoritmus
        /// használata nélkül.</summary>
        public void RemoveNumbersWithoutBackTrack()
        {
            int i = -1, j = -1, cellValue = 0;

            //Ez a lista szolgál az utolsó 6 törölt cella blokkszámának tárolására.
            List<int> blockIndecesOfRemovedCells = new List<int>(1);
            util.RectangularCells = new Dictionary<int, List<Cell>>();

            do
            {
                //Ha még nem vettem ki számot, akor generálok indexeket
                if (util.RemovedCellsAndValuesBeforeRemoval.Count == 0)
                    GenerateCellIndeces(ref i, ref j, ref cellValue);
                else
                {
                    visszaallit(false);

                    //Ha 6 szám van a listában, akkor az elsőt kitöröljük, az új blokkszámot pedig a sor végére tesszük be
                    if (blockIndecesOfRemovedCells.Count == MAX_NUMBER_OF_REMEMBERED_REMOVED_CELL_INDECES)
                        blockIndecesOfRemovedCells.RemoveAt(0);

                    blockIndecesOfRemovedCells.Add(BlockIndexByCellIndeces(i, j));

                    //Ha i és j benne van az előző 6 körben generált valamely indexekhez tartozó blokkban
                    //vagy a feladat táblában az i,j indexen lévő elem 0, akkor újragenerálom az indexeket
                    do GenerateCellIndeces(ref i, ref j, ref cellValue); 
                        while (blockIndecesOfRemovedCells.Contains(BlockIndexByCellIndeces(i, j)) || cellValue == 0);
                }

                //Kitörlöm a generált indexeken levő számot
                se.Exercise[0][i, j] = 0;

                //Elmentem a törölt cella indexeit, és törlés előtti értékét
                util.RemovedCellsAndValuesBeforeRemoval.Add(new Cell(i, j), cellValue);

                //Cella törlése miatt a törölhető értékek beállítása a számtömbökben.
                se.Ctrl.RegenerateNumberTablesForRemovedValue(cellValue, i, j);
            } while (se.Ctrl.IsExerciseSolvableWithoutBackTrack(util.RemovedCellsAndValuesBeforeRemoval.Count));

            //Mivel az utolsó megoldás során marad üres cella, így a teljes megoldás értékeit megkapja Exercise
            Arrays.CopyJaggedThreeDimensionArray(se.Exercise, util.Solution);

            //Az utolsó törölt cella kivételével az összes többi cella értékét törlöm
            visszaallit(true);
        }

        /// <summary> A RemoveNumbersWithoutBackTrack eljárásban szükséges. A generált táblát egy előző állapotába állítja vissza.</summary>
        /// <param name="nemKellUtolso">Az mondja meg, hogy az utolsó elemet vissza kell-e állítani</param>
        private void visszaallit(bool nemKellUtolso)
        {
            //Mivel az utolsó elemet (az utoljára kivett számot) nem kell visszaállítani, törlöm a lista utolsó elemét
            if (nemKellUtolso)
                util.RemovedCellsAndValuesBeforeRemoval.Remove(util.RemovedCellsAndValuesBeforeRemoval.Last().Key);

            foreach (KeyValuePair<Cell, int> cell in util.RemovedCellsAndValuesBeforeRemoval)
            {
                //Törlöm az aktuális cella értékét
                se.Exercise[0][cell.Key.row, cell.Key.col] = 0;
                //A törlendő cellák törlése
                se.Ctrl.RegenerateNumberTablesForRemovedValue(cell.Value, cell.Key.row, cell.Key.col);
            }

            se.NumberOfEmptyCells = util.RemovedCellsAndValuesBeforeRemoval.Count;
        }

        /// <summary> Számokat vesz ki, és mellé használja a visszalépéses algoritmust is. 
        /// Annyiszor vesz ki 2 darab számot, amekkora nehézséget a felhasználói felületen beállítottam. </summary>
        public void RemoveNumbersWithBackTrack()
        {
            /* k: számláló a 2 szám kivételéhez
             * i: cella sorindexe
             * j: cella oszlopindexe
             * t: az [i, j] indexű cella értéke a feladatban
             * szamlalo: számolja, hogy hányszor próbált meg 2 cellát törölni
             * uresCellakSzama: üres cellák száma a feladat korábbi állapotában*/
            int k, i = 0, j = 0, t = 0, szamlalo, uresCellakSzama = 0;

            //Ebbe a tömbbe mentem el a feladatot ...
            int[][,] tombokMentes;

            Arrays.Initialize(out tombokMentes);

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
                //Egyébként
                else
                    visszaallitBT(tombokMentes, ref uresCellakSzama); //Az utolsó megoldható állapotába állítom vissza a feladatot

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
                            GenerateCellIndeces(ref i, ref j, ref t);
                        while (t == 0);

                        //Törlöm a generált indexű cella értékét
                        se.Exercise[0][i, j] = 0;

                        //Mentem a törölt cella indexeit, és törlés előtti értékét
                        util.RemovedCellsAndValuesBeforeRemoval.Add(new Cell(i, j), t);

                        //Törlöm a szükséges értékeket a számtömbökből
                        se.Ctrl.RegenerateNumberTablesForRemovedValue(t, i, j);
                    } while (++k <= 1);

                    //Mivel töröltem 2 számot, az üres cellák száma nő 2-vel
                    se.NumberOfEmptyCells = uresCellakSzama + 2;

                    /* Backtrack algoritmus használatával megoldja a feladatot, és false értéket ad vissza, ha a feladatnak több megoldása van.
                     * Ekkor visszalép a do-while ciklus elejére, és megpróbál másik 2 számot kivenni.*/
                } while (!megoldhatoBackTrack());
            }

            //Az utolsó megoldható állapotába állítom vissza a feladatot
            visszaallitBT(tombokMentes, ref uresCellakSzama);
        }

        /// <summary> A RemoveNumbersWithBackTrack eljárásban van szükség rá. Visszaállítja a feladatot az utolsó megoldható (röviden korábbi) állapotába.</summary>
        /// <param name="tombokMentes">Ebben vannak a korábbi állapot értékei(nek nagy része)</param>
        /// <param name="uresCellakSzama">Üres cellák száma a feladat korábbi állapotában</param>
        private void visszaallitBT(int[][,] tombokMentes, ref int uresCellakSzama)
        {
            //Visszaállítom a "korábbi állapotot"
            Arrays.CopyJaggedThreeDimensionArray(se.Exercise, tombokMentes);

            //Végigmegyek az utolsó két törölt cellán
            foreach (KeyValuePair<Cell, int> cella in util.RemovedCellsAndValuesBeforeRemoval)
            {
                //Az utoljára törölt két cellát ismét kitörlöm
                se.Exercise[0][cella.Key.row, cella.Key.col] = 0;
                //A törölhető cellák törlése a számtömbökben
                se.Ctrl.RegenerateNumberTablesForRemovedValue(cella.Value, cella.Key.row, cella.Key.col);
            }

            //Mivel töröltem 2 cellát, így az üres cellák száma nő 2-vel
            se.NumberOfEmptyCells = uresCellakSzama + 2;
        }

        /// <summary> Meghívja a feladat vizsgálatához szükséges függvényt. </summary>
        /// <returns> Ha a feladatnak egy megoldása van, akkor true-val, egyébként pedig false-szal tér vissza </returns>
        private bool megoldhatoBackTrack()
        {
            //Létrehozok egy BackTrack algoritmus futtatását végző objektumot. control-ban a feladat értékei vannak.
            BackTrackSolver bt = new BackTrackSolver();

            /* Megoldja a feladatot. A megoldást a megoldottTabla[0]-ba adja vissza, és visszatér true-val ha a feladatnak egy megoldása van,
             * egyébként false-szal*/
            return bt.SolveExerciseWithBackTrack();
        }

        #endregion
    }
}
