using Sudoku.Generate;
using Sudoku.Controller.Finder;
using static Sudoku.Table.TableUtil;

namespace Sudoku.Controller
{
    public class SimpleSudokuController
    {
        protected SudokuExercise se = SudokuExercise.get;
        private EmptyCellFinder finder = new EmptyCellFinder();

        #region Constructor

        public SimpleSudokuController()
		{
            se.ExerciseType = SudokuType.SimpleSudoku;
		}

        #endregion

        /// <summary>Reads the exercise from the given file path.</summary>
        /// <param name="filePath">Path of the file.</param>
        /// <returns>False: there is no solvable exercise, or the reading was unsuccessful.
        /// True: no error occured.</returns>
        public bool ReadSudoku(string filePath)
        {
            se.InitExercise();
            return ExerciseReader.ReadSudoku(filePath);
        }

        /// <summary>Fills the number tables according to the values in the exercise.</summary>
        /// <returns>True if there is a solvable exercise, otherwise false.</returns>
        protected bool GenerateValuesInNumberTables()
        {
            bool isThereSolvableExercise = false;
            if (!se.IsExerciseFull() && !se.IsExerciseEmpty())
            {
                //Iterating through the table
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < 9; j++)
                    {
                        if (!se.IsCellEmpty(0, i, j))
                        {
                            int currentCellValue = se.Exercise[0][i, j];
                            se.Exercise[currentCellValue][i, j] = currentCellValue;

                            for (int numberTable = 1; numberTable <= 9; numberTable++)
                            {
                                if (numberTable != currentCellValue)
                                    se.MakeCellOccupied(numberTable, i, j);
                            }

                            MakeHousesOccupied(currentCellValue, i, j);
                        }
                    }
                }

                isThereSolvableExercise = true;
            }
            return isThereSolvableExercise;
        }

        /// <summary> During the generation, when a number is removed from a cell, then:
        /// 1. the number table of the removed value gets cleared
        /// 2. the number table gets regenerated based on the values in the exercise
        /// 3. the changed cell gets empty in the number tables that are not the value of the removed number. An empty cell means 0 value.
        public void RegenerateNumberTablesForRemovedValue(int removedValue, int deletedCellRow, int deletedCellColumn)
        {
            //1.
            se.Exercise[removedValue] = new int[9, 9];

            //2.
            for (int p = 0; p < se.LAST_CELL_INDEX; p++)
            {
                if (se.Exercise[0][p / 9, p % 9] == removedValue)
                {
                    se.Exercise[removedValue][p / 9, p % 9] = removedValue;
                    MakeHousesOccupied(removedValue, p / 9, p % 9);
                }
                else
                {
                    if (!se.IsCellEmpty(0, p / 9, p % 9))
                        se.MakeCellOccupied(removedValue, p / 9, p % 9);
                }
            }

            //3.
            for (int num = 1; num < 10; num++)
            {
                if (num != removedValue && !HousesContainValue(deletedCellRow, deletedCellColumn, num))
                {
                    se.Exercise[num][deletedCellRow, deletedCellColumn] = se.EMPTY;
                }
            }
        }

        /// <summary>Inspects whether value is present in the colOfCurrentCell column of the exercise.</summary>
        /// <returns>True in case of inclusion, otherwise false.</returns>
        protected bool ColumnContainsValue(int rowOfCurrentCell, int colOfCurrentCell, int value)
        {
            for (int row = 0; row < 9; row++)
            {
                if (row != rowOfCurrentCell && se.Exercise[0][row, colOfCurrentCell] == value)
                    return true;
            }

            return false;
        }

        /// <summary>Inspects whether value is present in the rowOfCurrentCell row of the exercise.</summary>
        /// <returns>True in case of inclusion, otherwise false.</returns>
        protected bool RowContainsValue(int rowOfCurrentCell, int colOfCurrentCell, int value)
        {
            for (int col = 0; col < 9; col++)
            {
                if (col != colOfCurrentCell && se.Exercise[0][rowOfCurrentCell, col] == value)
                    return true;
            }

            return false;
        }

        /// <summary>Inspects whether value is present in the block of the current cell of the exercise.</summary>
        /// <returns>True in case of inclusion, otherwise false.</returns>
        protected bool BlockContainsValue(int rowOfCurrentCell, int colOfCurrentCell, int value)
        {
            for (int row = StartRowOfBlockByRow(rowOfCurrentCell); row <= EndRowOfBlockByRow(rowOfCurrentCell); row++)
            {
                for (int col = StartColOfBlockByCol(colOfCurrentCell); col <= EndColOfBlockByCol(colOfCurrentCell); col++)
                {
                    if (row != rowOfCurrentCell && col != colOfCurrentCell && se.Exercise[0][row, col] == value)
                        return true;
                }
            }

            return false;
        }

        /// <summary>Inspects whether value is present in any houses of the current cell of the exercise.</summary>
        /// <returns>True in case of inclusion, otherwise false.</returns>
        public virtual bool HousesContainValue(int rowOfCurrentCell, int colOfCurrentCell, int value)
        {
            if (!RowContainsValue(rowOfCurrentCell, colOfCurrentCell, value)
                && !ColumnContainsValue(rowOfCurrentCell, colOfCurrentCell, value)
                && !BlockContainsValue(rowOfCurrentCell, colOfCurrentCell, value))
                    return false;

            return true;
        }
		
        /// <summary>Marks all the not empty cells as occupied in the block of the current cell.</summary>
        // __ __ __		// __ __ __		// __ __ __		// __ __ __		// __ __ __		// __ __ __		// __ __ __		// __ __ __		// __ __ __
        //|00|__|__|	//|__|01|__|	//|__|__|02|	//|__|__|__|	//|__|__|__|	//|__|__|__|	//|__|__|__|	//|__|__|__|	//|__|__|__|
        //|__|__|__|	//|__|__|__|	//|__|__|__|	//|10|__|__|	//|__|11|__|	//|__|__|12|	//|__|__|__|	//|__|__|__|	//|__|__|__|
        //|__|__|__|	//|__|__|__|	//|__|__|__|	//|__|__|__|	//|__|__|__|	//|__|__|__|	//|20|__|__|	//|__|21|__|	//|__|__|22|
		protected virtual void MakeBlockOccupied(int num, int row, int col)
        {
            for (int r = row; r <= row + 2; r++)
            {
                for (int p = col; p <= col + 2; p++)
                {
                    MakeCellOccupied(num, r, p);
                }
            }
        }

        /// <summary>Marks all the not empty cells as occupied in the row of the current cell.</summary>
		protected void MakeRowOccupied(int num, int row)
        {
            for (int col = 0; col < 9; col++)
            {
                MakeCellOccupied(num, row, col);
            }
        }

        /// <summary>Marks all the not empty cells as occupied in the column of the current cell.</summary>
        protected void MakeColumnOccupied(int num, int col)
        {
            for (int row = 0; row < 9; row++)
            {
                MakeCellOccupied(num, row, col);
            }
        }

        /// <summary>Marks the cell as occupied if the cell is empty.</summary>
        protected void MakeCellOccupied(int num, int row, int col)
        {
            if (se.IsCellEmpty(num, row, col))
                se.MakeCellOccupied(num, row, col);
        }

        /// <summary>Marks all the not empty cells as occupied in the all the houses of the current cell.</summary>
        /// <param name="num">The number table.</param>
        public virtual void MakeHousesOccupied(int num, int row, int col)
        {
            MakeRowOccupied(num, row);
            MakeColumnOccupied(num, col);
            MakeBlockOccupied(num, StartRowOfBlockByRow(row), StartColOfBlockByCol(col));
        }

        /// <summary>Solves the exercise without using backtrack algorithm (as much as the difficulty of the exercise makes it possible).</summary>
        /// <returns>True if the exercise is solved completely (there is no empty cell), otherwise false</returns>
        protected bool SolveExerciseWithoutBackTrack()
        {
            int numberOfEmptyCellsToFill = se.NumberOfEmptyCells;
            //the value for the only empty cell throughout number tables
            //-1 means the cell is not empty in any number tables, or it is empty in more than 1 number tables
            int valueOfOnlyEmptyCellThroughoutNumberTables = -1;
            se.FirstEmptyCell = 0;
            bool cellFillingHappened;

            for (int cell = 1; cell <= numberOfEmptyCellsToFill; cell++)
            {
                cellFillingHappened = false;

                for (int p = se.FirstEmptyCell; p < se.LAST_CELL_INDEX; p++)
                {
                    //If the cell in not empty, then moving on to the next cell, as only empty cells are inspected.
                    if (!se.IsCellEmpty(0, p))
                        continue;

                    valueOfOnlyEmptyCellThroughoutNumberTables = -1;

                    //Getting the number tables where the current cell is empty
                    for (int num = 1; num < 10; num++)
                    {
                        if (se.IsCellEmpty(num, p))
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

                    //If the current cell is empty in only one table (including the exercise)
                    //then the value of the found number table should be filled in
                    if (valueOfOnlyEmptyCellThroughoutNumberTables != -1)
                    {
                        se.Exercise[0][p / 9, p % 9] = valueOfOnlyEmptyCellThroughoutNumberTables;
                        se.Exercise[valueOfOnlyEmptyCellThroughoutNumberTables][p / 9, p % 9] = valueOfOnlyEmptyCellThroughoutNumberTables;
                        MakeHousesOccupied(valueOfOnlyEmptyCellThroughoutNumberTables, p / 9, p % 9);
                        cellFillingHappened = true;
                        PostProcessCellFilling(p);
                    }
                    else
                    {
                        int row, col;

                        for (int num = 1; num <= 9; num++)
                        {
                            if (FindAndFillOnlyEmptyCellInHouses(num, out row, out col))
                            {
                                cellFillingHappened = true;
                                PostProcessCellFilling((row * 9) + col);
                                break;
                            }
                        }
                    }
                }

                if (!cellFillingHappened)
                    break;
            }

            //If there is no empty cell, then the exercise could be solved without using backtrack algorithm
            return se.IsExerciseFull();
        }

        private void PostProcessCellFilling(int position)
        {
            se.NumberOfEmptyCells--;

            //The next round of finding an empty cell can be started only from the first EMPTY cell in the table
            //and not from the first cell in the table (0,0).
            se.RecalculateFirstEmptyCell(position);
        }

        /// <summary> Ha van olyan ház, amelyben egyetlen egy üres cella van (oda biztos beírható az adott szám), akkor azt kitölti. </summary>
        /// <returns>True if an empty cell was found and could be filled in the house of the given cell, otherwise false.</returns>
        private bool FindAndFillOnlyEmptyCellInHouses(int num, out int row, out int col)
        {
            Cell emptyCell = new Cell();

            //Iterating through the houses
            for (int k = 0; k < 9; k++)
            {
                //TODO: összevagyolni az iterációk közötti eredményeket, de ha már volt egy true-val visszatért eredmény, akkor kilépni belőle

                /* tombok[r] k indexű során megy végig. Ha talál egyedüli üres cellát, akkor visszaadja, hogy a k indexű sorban melyik indexű elem az üres
                 * egyébként pedig -1-et*/
                if ((col = finder.FindOnlyEmptyCellInRow(num, row: k)) > 0)
                {
                    //beírom a megfelelő tömbökbe az r számot, és minden számtömbben beállítom a foglalt cellákat
                    PutNumToExerciseAndMakeCellsOccupied(num, k, col);
                    /* megnézem, hogy a az az indexű cella, ahova most beírtam r-t, szerepel-e egy olyan tömbben, amiben még van 4 üres cella
                     * ha van ilyen, akkor elvégzi a megfelelő lépéseket*/

                    row = k;
                    return true;
                }
                /* tombok[r] k indexű oszlopán megy végig. Ha talál egyedüli üres cellát, akkor visszaadja, hogy a k indexű oszlopban melyik indexű elem
                 * az üres, egyébként pedig -1-et*/
                else if ((row = finder.FindOnlyEmptyCellInColumn(num, col: k)) > 0)
                {
                    PutNumToExerciseAndMakeCellsOccupied(num, row, k);

                    col = k;
                    return true;
                }
                /* tombok[num] k index-szel jelzett blokkján megy végig, és index-be belerakja a megtalált üres cella indexeit
                 * ha egy üres cellát talált, akkor visszatér true-val, egyébként false-szal*/
                else if (finder.FindOnlyEmptyCellInBlock(num, out emptyCell, blockIndex: k))
                {
                    PutNumToExerciseAndMakeCellsOccupied(num, emptyCell.Row, emptyCell.Col);

                    row = emptyCell.Row;
                    col = emptyCell.Col;
                    return true;
                }
            }

            row = col = -1;

            return false;
        }

        /// <summary>Fills in the passed cell and makes all the necessary houses occupied.</summary>
        private void PutNumToExerciseAndMakeCellsOccupied(int num, int row, int col)
        {
            se.Exercise[0][row, col] = se.Exercise[num][row, col] = num;

            for (int numberTable = 1; numberTable <= 9; numberTable++)
            {
                if (numberTable != num)
                    se.MakeCellOccupied(numberTable, row, col);
            }

            MakeHousesOccupied(num, row, col);
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

            return SolveExerciseWithoutBackTrack();
        }

        public void SolveReadExercise()
        {
            GenerateValuesInNumberTables();

            int[][,] exerciseInitialState;
            Arrays.Initialize(out exerciseInitialState);
            Arrays.CopyJaggedThreeDimensionArray(exerciseInitialState, se.Exercise);
            int originalNumberOfEmptyCells = se.NumberOfEmptyCells;

            if (!SolveExerciseWithoutBackTrack())
                //WARN: investigate this part as there may be a problem generating exercises that need solving
                //with backtrack
                se.Solution = SolveExerciseWithBackTrack();
            else
            {
                se.Solution = new int[9, 9];
                Arrays.CopyTwoDimensionArray(se.Solution, se.Exercise[0]);
            }

            Arrays.CopyJaggedThreeDimensionArray(se.Exercise, exerciseInitialState);
            se.NumberOfEmptyCells = originalNumberOfEmptyCells;
        }

        /// <summary>Solves the exercise using backtrack algorithm.</summary>
        /// <returns>The solved exercise.</returns>
        protected int[,] SolveExerciseWithBackTrack()
        {
            new BackTrackSolver().SolveExerciseWithBackTrack();
            return se.Exercise[0];
        }
    }
}