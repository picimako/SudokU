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

                //There is a solvable exercise
                return true;
            }
            //There is no solvable exercise
            else
                return false;
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
                    se.Exercise[num][deletedCellRow, deletedCellColumn] = 0;
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
            for (int row = se.StartRowOfBlockByRow(rowOfCurrentCell); row <= se.EndRowOfBlockByRow(rowOfCurrentCell); row++)
            {
                for (int col = se.StartColOfBlockByCol(colOfCurrentCell); col <= se.EndColOfBlockByCol(colOfCurrentCell); col++)
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
            MakeBlockOccupied(num, se.StartRowOfBlockByRow(row), se.StartColOfBlockByCol(col));
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
            Pair emptyCell = new Pair();

            //Iterating through the houses
            for (int k = 0; k < 9; k++)
            {
                //TODO: összevagyolni az iterációk közötti eredményeket, de ha már volt egy true-val visszatért eredmény, akkor kilépni belőle

                /* tombok[r] k indexű során megy végig. Ha talál egyedüli üres cellát, akkor visszaadja, hogy a k indexű sorban melyik indexű elem az üres
                 * egyébként pedig -1-et*/
                if ((col = FindOnlyEmptyCellInRow(num, row: k)) > 0)
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
                else if ((row = FindOnlyEmptyCellInColumn(num, col: k)) > 0)
                {
                    PutNumToExerciseAndMakeCellsOccupied(num, row, k);

                    col = k;
                    return true;
                }
                /* tombok[num] k index-szel jelzett blokkján megy végig, és index-be belerakja a megtalált üres cella indexeit
                 * ha egy üres cellát talált, akkor visszatér true-val, egyébként false-szal*/
                else if (FindOnlyEmptyCellInBlock(num, out emptyCell, blockIndex: k))
                {
                    PutNumToExerciseAndMakeCellsOccupied(num, emptyCell.row, emptyCell.col);

                    row = emptyCell.row;
                    col = emptyCell.col;
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

            if (!SolveExerciseWithoutBackTrack())
                se.Solution = SolveExerciseWithBackTrack();
            else
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

        /// <summary>Solves the exercise using backtrack algorithm.</summary>
        /// <returns>The solved exercise.</returns>
        protected int[,] SolveExerciseWithBackTrack()
        {
            new BackTrackSolver().SolveExerciseWithBackTrack();
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

                //van értéke van|teli lesz
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

                //van értéke van|teli lesz
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

        /// <summary>Iterates through the 'i' row or column in the 'num' numbertable and searches for an only empty cell
        /// in the row or column</summary>
        /// <param name="i">The row or column to check.</param>
        /// <returns>
        /// The found row or column index if there is only one cell found.
        /// -1 if there is more or less then 1 cell is found.
        /// </returns>
        public int FindOnlyEmptyCellInRowOrColumn(int num, int i, bool rowToCheck)
        {
            List<int> list = new List<int>(1);

            for (int k = 0; k < 9; k++)
            {
                if (rowToCheck ? se.IsCellEmpty(num, i, k) : se.IsCellEmpty(num, k, i))
                    list.Add(k);

                //Since we are searching for only one cell, if there are two, the search doesn't need to continue
                if (list.Count > 1)
                    return -1;
            }

            return list.Count == 1 ? list.First() : -1;
        }

        public int FindOnlyEmptyCellInRow(int num, int row)
        {
            return FindOnlyEmptyCellInRowOrColumn(num, row, rowToCheck: true);
        }

        public int FindOnlyEmptyCellInColumn(int num, int col)
        {
            return FindOnlyEmptyCellInRowOrColumn(num, col, rowToCheck: false);
        }

        /// <summary>Searches for the only empty cell in the given block of the passed number table.
        /// The indeces of the empty cell is stored in 'cell'.</summary>
        /// <param name="blockIndex">The index of the block to check.</param>
        /// <param name="cell">The only empty cell in the block.</param>
        /// <returns>True if there is only one empty cell in the block, otherwise false.</returns>
        public bool FindOnlyEmptyCellInBlock(int num, out Pair cell, int blockIndex)
        {
            cell = new Pair();

            int startRow = se.StartRowOfBlockByBlockIndex(blockIndex);
            int startCol = se.StartColOfBlockByBlockIndex(blockIndex);
            int endRow = se.EndRowOfBlockByBlockIndex(blockIndex);
            int endCol = se.EndColOfBlockByBlockIndex(blockIndex);
            int numberOfEmptyCellsInBlock = 0;

            for (int row = startRow; row <= endRow; row++)
            {
                for (int col = startCol; col <= endCol; col++)
                {
                    if (se.IsCellEmpty(num, row, col))
                    {
                        if (++numberOfEmptyCellsInBlock > 1)
                            return false;

                        cell.row = row;
                        cell.col = col;
                    }
                }
            }

            return numberOfEmptyCellsInBlock == 1;
        }

        /// <summary>
        /// Collect the cells (from the specified numbertable)
        /// that are placed in blocks that have 'numberOfSoughtEmptyCells' number of empty cells.
        /// <param name="numberOfSoughtEmptyCells">The number of empty cells needed.</param>
        /// TODO: make sure to check if the refactor didn't break anything
        public List<Pair> FindXNumberOfEmptyCellsInBlocks(int num, int numberOfSoughtEmptyCells)
        {
            List<Pair> emptyCellsInBlock = new List<Pair>();
            List<Pair> allEmptyCells = new List<Pair>();

            for (int b = 0; b < 9; b++)
            {
                //Haven't found empty cells
                emptyCellsInBlock.Clear();

                for (int row = se.StartRowOfBlockByBlockIndex(b); row <= se.EndRowOfBlockByBlockIndex(b) &&
                    emptyCellsInBlock.Count <= numberOfSoughtEmptyCells; row++)
                {
                    for (int col = se.StartColOfBlockByBlockIndex(b); col <= se.EndColOfBlockByBlockIndex(b) &&
                        emptyCellsInBlock.Count <= numberOfSoughtEmptyCells; col++)
                    {
                        if (se.IsCellEmpty(num, row, col))
                            emptyCellsInBlock.Add(new Pair(row, col));
                    }
                }

                if (emptyCellsInBlock.Count == numberOfSoughtEmptyCells)
                    allEmptyCells.AddRange(emptyCellsInBlock);
            }

            return allEmptyCells;
        }

        /// <summary>Collects the first maximum 4 empty cells in the 'num' numbertable.</summary>
        /// TODO: make sure to check if the refactor didn't break anything
        public List<Pair> FindEmptyCellsInNumberTable(int num)
        {
            List<Pair> emptyCells = new List<Pair>();

            int p = 0;
            while (p < se.LAST_CELL_INDEX && emptyCells.Count <= 4)
            {
                if (se.IsCellEmpty(num, p))
                    emptyCells.Add(new Pair(p / 9, p % 9));

                p++;
            }

            return emptyCells;
        }
    }
}