﻿using System;
using System.Collections.Generic;
using Sudoku.Controller;

namespace Sudoku.Generate
{
    class ExerciseGenerator
    {
        private SudokuExercise se = SudokuExercise.get;
        private int maxNumberOfEmptiedCells = 30;
        private GeneratorUtil util;
        private NumberRemover remover;
        private Random random;

        /// <param name="difficulty"> The difficulty of the simple exercise. </param>
        /// <param name="killerDifficulty"> The difficulty of the optional Killer exercise. </param>
        public ExerciseGenerator(int difficulty, int killerDifficulty)
        {
            util = new GeneratorUtil(difficulty, killerDifficulty);
            remover = new NumberRemover(util);
            random = new Random();
        }

        /// <summary> Ez egy összefoglaló eljárás, ami meghívja a feladat generálásához szükséges függvényeket és eljárásokat. </summary>
        /// <param name="megoldottFeladat"> Ebbe a tömbbe adja vissza a feladat megoldását</param>
        /// <returns> A kitöltést kezelő osztály objektumát fogja visszadni. Ebben vannak a feladat értékei is tárolva. </returns>
        public void Generate()
        {
            //true, ha a jó feladatot sikerült generálni, egyébként false
            bool joFeladat;

            do
            {
                if (!se.IsExerciseKiller)
                    util.InitializeSolutionContainer();

                //Törlöm a generálásnál használt szótárakat
                util.InitializeGeneration();

                /* Controller osztály példányosítása, melynek megadom a feladat fajtáját
                 * Ezzel a feladat eddig generált értékeit (ha voltak), törli*/
                se.SetControllerForCurrentExerciseType();

                /* Létrehozok egy teljesen kitöltött sudoku feladatot.
                 * Ha ütközés jött létre a táblában, akkor a feladat generálásának újrakezdése.*/
                FullTableGenerator tableGenerator = new FullTableGenerator(util);
                if (!(joFeladat = tableGenerator.GenerateFullTableFromEmptyTable()))
                    continue;

                if (!se.IsExerciseKiller)
                    //akkor veszek ki számot
                    remover.RemoveNumbersWithoutBackTrack();

                /* Addig generál, míg:
                 * nem Killer esetén: az üres cellák száma kisebb, mint 20
                 * Killer esetén: amíg a feladat nem jól generált*/
            } while (!se.IsExerciseKiller ? util.RemovedCellsAndValuesBeforeRemoval.Count < maxNumberOfEmptiedCells : !joFeladat);

            if (se.IsExerciseKiller)
            {
                do
                {
                    se.InitKillerExercise();

                    se.Killer.Ctrl.CopySolutionToExercise(se.Exercise[0]);

                    /* Legenerálok egy felosztást.
                     * Ha van nem elhelyezhető cella, akkor új felosztást készítek*/
                } while (!GenerateKiller());

                //Kitörlök minden értéket a feladatból, mert üres táblát kell megadni a feladatban
                int[][,] tombok = Arrays.CreateInitializedArray();
                se.Exercise = tombok;

                //81 lesz az üres cellák száma, mert nem kell megadni egyetlen egy számot sem a feladatban
                se.NumberOfEmptyCells = se.LAST_CELL_INDEX;
            }
            else
            {
                if (util.Difficulty != 0)
                    remover.RemoveNumbersWithBackTrack(); //További számok kivétele nehezítés gyanánt
            }

            if (!se.IsExerciseKiller)
            {
                se.Solution = util.Solution[0];
            }
        }

        /// <summary>Generates a Killer layout on an already generated Sudoku solution.</summary>
        public bool GenerateKiller()
        {
            int cageIndex = 1;
            int currentCageSize;
            int numberOfCellsPutInCurrentCage;
            
            List<Cell> neighbourCells = new List<Cell>();
            //Egy egyedül maradt cella lehetséges szomszéd ketreceinek tárolására
            List<int> neighbourCages = new List<int>();
            //Az aktuálisan feldolgozandó cella indexeit tárolja
            Cell currentCell = new Cell(0, 0);

            do
            {
                numberOfCellsPutInCurrentCage = 0;

                /* Az aktuális ketrec mérete (benne levő cellák száma) a [2, killerNehezseg + 3) halmazból egy szám
                 * killerNehezseg + 3 - 1 az a szám, amekkora legnagyobb méretű ketreceket megengedek
                 * Persze ettől függetlenül létrejöhetnek olyan ketrecek, amelyek ennél több cellából állnak, mert általában van olyan cella,
                 * ami önmaga alkotna egy ketrecet, de ekkor hozzá kell venni egy szomszéd cella ketrecéhez.*/
                currentCageSize = random.Next(2, util.KillerDifficulty + 3);

                se.Killer.Ctrl.PutCellInCage(currentCell, cageIndex);

                do
                {
                    /* Lekérem az aktuális cella minden szabad szomszédját.
                     * (ami még nem szerepel egyik ketrecben sem és a ketrec nem tartalmazza az aktuális cella értékét.*/
                    neighbourCells = se.Killer.Ctrl.FindPossibleNeighbourCells(currentCell, cageIndex, true);
                    if (neighbourCells.Count != 0)
                    {
                        //Ha van lehetséges szomszéd, akkor véletlenszerűen választok egyet közülük
                        currentCell = neighbourCells[RandomFrom(neighbourCells.Count)];

                        //A kiválasztott szomszédot elhelyezem a ketrecben
                        se.Killer.Ctrl.PutCellInCage(currentCell, cageIndex);
                    }
                    else
                    {
                        /* Ha pedig nincs lehetséges szomszédja, akkor ezt a cellát belle kell raknom valamelyik olyan szomszéd ketrecébe, ahol 9-nél
                         * kevesebb elem van, és az aktuális cella értéke még nem szerepel abban a ketrecben.
                         * Ha az aktuális ketrecben meg nincs cella*/
                        if (numberOfCellsPutInCurrentCage == 0)
                        {
                            //Ha nincs lehetséges választható szomszéd ketrec, 
                            if ((neighbourCages = se.Killer.Ctrl.FindPossibleNeighbourCages(currentCell)).Count == 0)
                                return false; //visszatérek false-szal, és a felosztás készítése elölről kezdődik

                            /* Ha van választható szomszéd ketrec, akkor választok közülük
                             * Az aktuális (egyedül maradt) cellát elhelyezem a kiválasztott szomszéd ketrecben*/
                            se.Killer.Ctrl.PutCellInCage(currentCell, neighbourCages[RandomFrom(neighbourCages.Count)]);

                            //Kitörlöm a cella előző ketrecszámához tartozó értéket a ketreceket tároló szótárból
                            se.Killer.Cages.Remove(cageIndex);
                        }

                        //Elkezdek egy új ketrecet létrehozni
                        break;
                    }
                    //Addig megy, amíg a ketrec elemszámának megfelelő cellát el nem helyezek a ketrecben
                } while (++numberOfCellsPutInCurrentCage < currentCageSize);
                cageIndex++;
                /* Megkeresem a legelső 0 ketrecértékű cellát, és a megtalált cella indexeit kapja értékül aktCella.
                 * Ha az indexek értéke -1, akkor teli van a tábla, tehát kész a felosztás.*/
            } while ((currentCell = se.Killer.Ctrl.FindFirstEmptyCell()).Row != -1);

            return true;
        }

        private int RandomFrom(int number)
        {
            return random.Next(0, number);
        }
    }
}