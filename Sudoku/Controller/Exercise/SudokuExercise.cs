using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sudoku.Generate;

namespace Sudoku.Controller
{
    public class SudokuExercise
    {
        #region Members

        private static SudokuExercise ARRAY;
        private int[][,] exercise;
        private SudokuType exerciseType;
        private SimpleSudokuController controller;
        private int[,] solution;
        private int numberOfEmptyCells;
        private int firstEmptyCell;
        private bool isExerciseKiller;
        private bool isExerciseGenerated;
        private string exerciseFilePath;

        private KillerSudokuExercise killerExercise;
        
        #endregion

        #region Properties

        public int NumberOfEmptyCells
        {
            get { return numberOfEmptyCells; }
            set { numberOfEmptyCells = value; }
        }

        public int[][,] Exercise
        {
            get { return exercise; }
            set { exercise = value; }
        }

        public int FirstEmptyCell
        {
            get { return firstEmptyCell; }
            set { firstEmptyCell = value; }
        }

        public SudokuType ExerciseType
        {
            get { return exerciseType; }
            set { exerciseType = value; }
        }

        public bool IsExerciseKiller
        {
            get { return isExerciseKiller; }
            set { isExerciseKiller = value; }
        }

        public bool IsExerciseGenerated
        {
            get { return isExerciseGenerated; }
            set { isExerciseGenerated = value; }
        }

        public int[,] Solution
        {
            get { return solution; }
            set { solution = value; }
        }

        public SimpleSudokuController Ctrl
        {
            get { return controller; }
            set { controller = value; }
        }

        public KillerSudokuExercise Killer
        {
            get { return killerExercise; }
            set { killerExercise = value; }
        }

        public string ExerciseFilePath
        {
            get { return exerciseFilePath; }
            set { exerciseFilePath = value; }
        }

        public void InitKillerExercise()
        {
            killerExercise = new KillerSudokuExercise();
        }

        #endregion

        #region Constructor

        private SudokuExercise() { }

        public void InitExercise()
        {
            //Az üres cellák száma 0 lesz
            numberOfEmptyCells = 0;
            CommonUtil.InitializeArray(out exercise);
        }

        #endregion

        #region InitSingleton

        public static SudokuExercise get
        {
            get
            {
                if (ARRAY == null)
                {
                    ARRAY = new SudokuExercise();
                }
                return ARRAY;
            }
        }

        #endregion

        #region Methods
        #region Public

        public bool IsCellEmpty(int num, int row, int col)
        {
            return exercise[num][row, col] == 0;
        }

        public bool IsCellEmpty(int num, int position)
        {
            return exercise[num][position / 9, position % 9] == 0;
        }

        public void MakeCellOccupied(int num, int row, int col)
        {
            exercise[num][row, col] = -1;
        }

        public int StartRowOfBlockByRow(int row)
        {
            return (row / 3) * 3;
        }

        public int EndRowOfBlockByRow(int row)
        {
            return (row / 3) * 3 + 2;
        }

        public int StartColOfBlockByCol(int col)
        {
            return StartRowOfBlockByRow(col);
        }

        public int EndColOfBlockByCol(int col)
        {
            return EndRowOfBlockByRow(col);
        }

        public int StartRowOfBlockByBlockIndex(int blockIndex)
        {
            return blockIndex - (blockIndex % 3);
        }

        public int EndRowOfBlockByBlockIndex(int blockIndex)
        {
            return StartRowOfBlockByBlockIndex(blockIndex) + 2;
        }

        public int StartColOfBlockByBlockIndex(int blockIndex)
        {
            return (blockIndex % 3) * 3;
        }

        public int EndColOfBlockByBlockIndex(int blockIndex)
        {
            return StartColOfBlockByBlockIndex(blockIndex) + 2;
        }

        /// <summary>
        /// Numbering of blocks: from left to right, from top to bottom, starting with 0
        /// </summary>
        /// <param name="i">Row index of cell.</param>
        /// <param name="j">Column index of cell.</param>
        /// <returns>The block index determined by the row and column indeces.</returns>
        public int BlockIndexByCellIndeces(int i, int j)
        {
            return (i / 3) * 3 + (j / 3);
        }

        /// <summary>
        /// Finds the first empty cell
        /// </summary>
        /// <param name="p">The currently examined cell.</param>
        public void RecalculateFirstEmptyCell(int p)
        {
            if (p == firstEmptyCell && numberOfEmptyCells != 0)
            {
                while (firstEmptyCell < 81 && !IsCellEmpty(0, firstEmptyCell))
                    firstEmptyCell++;
            }
        }

        public bool IsExerciseFull()
        {
            return numberOfEmptyCells == 0;
        }

        public bool IsExerciseEmpty()
        {
            return numberOfEmptyCells == 81;
        }

        #endregion
        #endregion
    }
}
