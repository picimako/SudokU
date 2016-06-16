using System;
using System.Collections.Generic;
using System.Linq;
using Sudoku.Cells;
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
        private TemporarySolutionContainer container;
        //Key: a removed cell. Value: the value of the cell before the removal.
        private Dictionary<Cell, int> removedCellsAndValuesBeforeRemoval = new Dictionary<Cell, int>();

        #endregion

        #region Constructor

        public NumberRemover(TemporarySolutionContainer container)
        {
            this.container = container;
        }

        #endregion

        #region Methods

        public Dictionary<Cell, int> RemovedCellsAndValuesBeforeRemoval
        {
            get { return removedCellsAndValuesBeforeRemoval; }
        }

        /// <summary> 
        /// Removes numbers while it can remove a number that still makes the exercise solvable without backtrack algorithm.
        /// </summary>
        public void RemoveNumbersWithoutBackTrack()
        {
            List<int> blockIndecesOfLastNRemovedCells = new List<int>(1);

            int row = -1;
            int col = -1;
            int cellValue = 0;

            do
            {
                //Ha még nem vettem ki számot, akor generálok indexeket
                if (removedCellsAndValuesBeforeRemoval.Count == 0)
                {
                    GenerateCellIndeces(ref row, ref col, ref cellValue);
                }
                else
                {
                    RestoreToPreviousState(false);

                    //Ha 6 szám van a listában, akkor az elsőt kitöröljük, az új blokkszámot pedig a sor végére tesszük be
                    if (blockIndecesOfLastNRemovedCells.Count == MAX_NUMBER_OF_REMEMBERED_REMOVED_CELL_INDECES)
                    {
                        blockIndecesOfLastNRemovedCells.RemoveAt(0);
                    }

                    blockIndecesOfLastNRemovedCells.Add(BlockIndexByCellIndeces(row, col));

                    //Ha row és col benne van az előző 6 körben generált valamely indexekhez tartozó blokkban
                    //vagy a feladat táblában a row,col indexen lévő elem üres, akkor újragenerálom az indexeket
                    do
                    {
                        GenerateCellIndeces(ref row, ref col, ref cellValue);
                    } while (blockIndecesOfLastNRemovedCells.Contains(BlockIndexByCellIndeces(row, col)) || cellValue == se.EMPTY);
                }

                //Kitörlöm a generált indexeken levő számot
                se.Exercise[0][row, col] = se.EMPTY;

                //Elmentem a törölt cella indexeit, és törlés előtti értékét
                removedCellsAndValuesBeforeRemoval.Add(new Cell(row, col), cellValue);

                //Cella törlése miatt a törölhető értékek beállítása a számtömbökben.
                se.Ctrl.RegenerateNumberTablesForRemovedValue(cellValue, row, col);

            } while (se.Ctrl.IsExerciseSolvableWithoutBackTrack(removedCellsAndValuesBeforeRemoval.Count));

            //Mivel az utolsó megoldás során marad üres cella, így a teljes megoldás értékeit megkapja Exercise
            Arrays.CopyJaggedThreeDimensionArray(se.Exercise, container.Solution);

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
            int k;
            int row = 0;
            int col = 0;
            int cellValue = 0;
            int numberOfTriesToRemoveTwoCells;
            int numberOfEmptyCells = 0;

            int[][,] tempExercise = Arrays.CreateInitializedArray();

            //Removing 2 cells n times where n is the difficulty of the exercise set on the UI
            for (int n = 1; n <= se.Difficulty; n++)
            {
                //If removing the first 2 numbers
                if (n == 1)
                {
                    //Saving the current state of the exercise
                    Arrays.CopyJaggedThreeDimensionArray(tempExercise, se.Exercise);
                    //Saving the number of empty cells of the exercise
                    numberOfEmptyCells = se.NumberOfEmptyCells;
                }
                else
                {
                    RestoreExerciseToItsLastSolvableStateFrom(tempExercise, ref numberOfEmptyCells);
                }

                numberOfTriesToRemoveTwoCells = 0;

                do
                {
                    //Clearing the saved removed cells
                    removedCellsAndValuesBeforeRemoval.Clear();

                    //Ha már legalább 2-szor próbálok meg törölni 2 cellát
                    if (++numberOfTriesToRemoveTwoCells > 1)
                    {
                        //Restoring the saved state of tables (the state before removing the last 2 cells)
                        Arrays.CopyJaggedThreeDimensionArray(se.Exercise, tempExercise);
                        //Restoring the number of empty cells
                        se.NumberOfEmptyCells = numberOfEmptyCells;
                    }

                    k = 0;

                    do
                    {
                        GenerateCellIndicesUntilOccupied(ref row, ref col, ref cellValue);
                        
                        se.Exercise[0][row, col] = se.EMPTY;

                        //Saving the removed cell and its value before removal
                        removedCellsAndValuesBeforeRemoval.Add(new Cell(row, col), cellValue);

                        //Removing the necessary values from the number tables
                        se.Ctrl.RegenerateNumberTablesForRemovedValue(cellValue, row, col);

                    } while (++k <= 1);

                    //Since 2 numbers were removed, the number of empty cells increases by 2
                    se.NumberOfEmptyCells = numberOfEmptyCells + 2;

                    /* Backtrack algoritmus használatával megoldja a feladatot, és false értéket ad vissza, ha a feladatnak több megoldása van.
                     * Ekkor visszalép a do-while ciklus elejére, és megpróbál másik 2 számot kivenni.*/
                } while (!IsExerciseSolvableWithBackTrack());
            }

            RestoreExerciseToItsLastSolvableStateFrom(tempExercise, ref numberOfEmptyCells);
        }

        private void GenerateCellIndicesUntilOccupied(ref int row, ref int col, ref int cellValue)
        {
            do
            {
                GenerateCellIndeces(ref row, ref col, ref cellValue);
            } while (cellValue == se.EMPTY);
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
            {
                removedCellsAndValuesBeforeRemoval.Remove(removedCellsAndValuesBeforeRemoval.Last().Key);
            }

            //Törlöm az aktuális cella értékét
            //A törlendő cellák törlése
            RemoveCellsAndRegenerateTablesForRestoration();

            se.NumberOfEmptyCells = removedCellsAndValuesBeforeRemoval.Count;
        }

        /// <summary>A RemoveNumbersWithBackTrack eljárásban van szükség rá. Visszaállítja a feladatot az utolsó megoldható (röviden korábbi) állapotába.</summary>
        /// <param name="tempExercise">Ebben vannak a korábbi állapot értékei(nek nagy része)</param>
        /// <param name="numberOfEmptyCells">Üres cellák száma a feladat korábbi állapotában</param>
        private void RestoreExerciseToItsLastSolvableStateFrom(int[][,] tempExercise, ref int numberOfEmptyCells)
        {
            //Restroring the "previous state"
            Arrays.CopyJaggedThreeDimensionArray(se.Exercise, tempExercise);

            //Végigmegyek az utolsó két törölt cellán
            //Az utoljára törölt két cellát ismét kitörlöm
            //A törölhető cellák törlése a számtömbökben
            RemoveCellsAndRegenerateTablesForRestoration();

            //Since 2 numbers were removed, the number of empty cells increases by 2
            se.NumberOfEmptyCells = numberOfEmptyCells + 2;
        }

        private void RemoveCellsAndRegenerateTablesForRestoration()
        {
            foreach (KeyValuePair<Cell, int> cell in removedCellsAndValuesBeforeRemoval)
            {
                se.Exercise[0][cell.Key.Row, cell.Key.Col] = se.EMPTY;
                se.Ctrl.RegenerateNumberTablesForRemovedValue(cell.Value, cell.Key.Row, cell.Key.Col);
            }
        }

        /// <summary>
        /// Solves the exercise with backtrack.
        /// The solution is stored in se.Exercise[0].
        /// </summary>
        /// <returns>True if the exercise has only one solution, otherwise false.</returns>
        private bool IsExerciseSolvableWithBackTrack()
        {
            return new BackTrackSolver().SolveExerciseWithBackTrack();
        }

        #endregion
    }
}
