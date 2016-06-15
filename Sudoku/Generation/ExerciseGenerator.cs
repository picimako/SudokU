using System;
using System.Collections.Generic;
using Sudoku.Cells;
using Sudoku.Controller;

namespace Sudoku.Generate
{
    class ExerciseGenerator
    {
        private const int maxNumberOfEmptiedCells = 30;
        private SudokuExercise se = SudokuExercise.get;
        private GeneratorUtil util;
        private NumberRemover remover;
        private FullTableGenerator tableGenerator;
        private Random random = new Random();

        public ExerciseGenerator()
        {
            util = new GeneratorUtil();
            remover = new NumberRemover(util);
        }

        /// <summary>Initalizes the generation and calls the necessary generation related logic.</summary>
        public void Generate()
        {
            bool correctFullTableGenerated;

            do
            {
                util.InitializeGeneration();

                //A feladat eddig generált értékeit (ha voltak), törli
                se.SetControllerForCurrentExerciseType();

                /* Létrehozok egy teljesen kitöltött Sudoku feladatot.
                 * Ha ütközés jött létre a táblában, akkor a feladat generálásának újrakezdése.*/
                tableGenerator = new FullTableGenerator(util);
                correctFullTableGenerated = tableGenerator.GenerateFullTableFromEmptyTable();
                if (!correctFullTableGenerated)
                    continue;

                if (!se.IsExerciseKiller)
                    remover.RemoveNumbersWithoutBackTrack();

                /* Addig generál, míg:
                 * nem Killer esetén: az üres cellák száma kisebb, mint 30
                 * Killer esetén: amíg a feladat nem jól generált*/
            } while (!se.IsExerciseKiller
                   ? remover.RemovedCellsAndValuesBeforeRemoval.Count < maxNumberOfEmptiedCells
                   : !correctFullTableGenerated);

            if (se.IsExerciseKiller)
            {
                do
                {
                    se.InitKillerExercise();
                    se.Killer.Ctrl.CopySolutionToExercise(se.Exercise[0]);
                } while (!GenerateKiller());

                //Kitörlök minden értéket a feladatból, mert üres táblát kell megadni a feladatban
                se.Exercise = Arrays.CreateInitializedArray(); ;

                //81 lesz az üres cellák száma, mert nem kell megadni egyetlen egy számot sem a feladatban
                se.NumberOfEmptyCells = se.LAST_CELL_INDEX;
            }
            else
            {
                if (se.Difficulty != 0)
                    remover.RemoveNumbersWithBackTrack(); //További számok kivétele nehezítés gyanánt

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
            Cell currentCell = new Cell(0, 0);

            do
            {
                numberOfCellsPutInCurrentCage = 0;

                /* Az aktuális ketrec mérete (benne levő cellák száma) a [2, KillerDifficulty + 3) halmazból egy szám.
                 * KillerDifficulty + 3 - 1 az a szám, amekkora legnagyobb méretű ketreceket megengedek.
                 * Persze ettől függetlenül létrejöhetnek olyan ketrecek, amelyek ennél több cellából állnak, mert általában van olyan cella,
                 * ami önmaga alkotna egy ketrecet, de ekkor hozzá kell venni egy szomszéd cella ketrecéhez.*/
                currentCageSize = random.Next(2, se.KillerDifficulty + 3);

                se.Killer.Ctrl.PutCellInCage(currentCell, cageIndex);

                do
                {
                    // Amelyik cella még nem szerepel egyik ketrecben sem és a ketrec nem tartalmazza az aktuális cella értékét.
                    neighbourCells = se.Killer.Ctrl.NeighbourCellFinder.FindPossibleNeighbourCells(currentCell, cageIndex, true);
                    if (neighbourCells.Count != 0)
                    {
                        //Ha van lehetséges szomszéd, akkor véletlenszerűen választok egyet közülük
                        currentCell = neighbourCells[RandomFrom(neighbourCells.Count)];

                        //A kiválasztott szomszédot elhelyezem a ketrecben
                        se.Killer.Ctrl.PutCellInCage(currentCell, cageIndex);
                    }
                    else
                    {
                        /* Ha pedig nincs lehetséges szomszédja, akkor ezt a cellát bele kell raknom valamelyik olyan szomszéd ketrecébe, ahol 9-nél
                         * kevesebb elem van, és az aktuális cella értéke még nem szerepel abban a ketrecben.
                         * Ha az aktuális ketrecben meg nincs cella*/
                        if (numberOfCellsPutInCurrentCage == 0)
                        {
                            neighbourCages = se.Killer.Ctrl.FindPossibleNeighbourCages(currentCell);
                            //Ha nincs lehetséges választható szomszéd ketrec, 
                            if (neighbourCages.Count == 0)
                                return false; //A felosztás készítése elölről kezdődik

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
                /* Megkeresem a legelső 0 ketrecértékű cellát, és a megtalált cella indexeit kapja értékül currentCell.
                 * Ha az indexek értéke -1, akkor teli van a tábla, tehát kész a felosztás.*/
            } while (!(currentCell = se.Killer.Ctrl.FindFirstEmptyCell()).Equals(Cell.OUT_OF_RANGE));

            return true;
        }

        private int RandomFrom(int number)
        {
            return random.Next(0, number);
        }
    }
}