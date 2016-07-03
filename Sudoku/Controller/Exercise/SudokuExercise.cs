using Sudoku.Generate;
using Sudoku.Generation.Solver;

namespace Sudoku.Controller
{
    /// <summary>
    /// Represents a non-Killer Sudoku exercise but has access to the Killer one as well.
    /// This is a singleton class since only one exercise can be present at a time.
    /// </summary>
    public class SudokuExercise
    {
        #region Members

        public static int LAST_CELL = 81;
        public static int EMPTY = 0;
        public static int OCCUPIED = -1;

        private static SudokuExercise sudokuExercise;
        private SimpleSudokuController controller;
        private KillerSudokuExercise killerExercise;

        private int difficulty;
        private int killerDifficulty;

        private int[][,] exercise;
        private int[,] solution;

        #endregion

        #region Properties

        public int Difficulty
        {
            get { return difficulty; }
            set { this.difficulty = value; }
        }

        public int KillerDifficulty
        {
            get { return killerDifficulty; }
            set { this.killerDifficulty = value; }
        }

        public int[][,] Exercise
        {
            get { return exercise; }
            set { exercise = value; }
        }

        public int[,] Solution
        {
            get { return solution; }
            set { solution = value; }
        }

        public SimpleSudokuController Ctrl
        {
            get { return controller; }
        }

        public KillerSudokuExercise Killer
        {
            get { return killerExercise; }
        }

        public int NumberOfEmptyCells { get; set; }

        public int FirstEmptyCell { get; set; }

        public SudokuType ExerciseType { get; set; }

        public bool IsExerciseKiller { get; set; }

        public bool IsExerciseGenerated { get; set; }

        public string ExerciseFilePath { get; set; }

        #endregion

        #region Constructor

        public static SudokuExercise Instance
        {
            get
            {
                if (sudokuExercise == null)
                {
                    sudokuExercise = new SudokuExercise();
                }
                return sudokuExercise;
            }
        }

        private SudokuExercise() { }

        public void InitExercise()
        {
            NumberOfEmptyCells = 0;
            exercise = Arrays.CreateInitializedArray();
        }

        public void InitKillerExercise()
        {
            killerExercise = new KillerSudokuExercise();
        }

        #endregion

        #region Methods
        #region Public

        /// <summary>
        /// Set the sudoku controller based on the current exercise type.
        /// </summary>
        public void SetControllerForCurrentExerciseType()
        {
            controller = new SudokuControllerFactory().CreateController(ExerciseType);
        }

        /// <summary>
        /// Checks if a cell, represented by its position in the exercise table, is empty.
        /// </summary>
        /// <param name="num">The number table. Can be from 0 to 9.</param>
        /// <param name="position">The position of the cell in the exercise.</param>
        /// <returns>True if the cell is empty, false otherwise.</returns>
        public bool IsCellEmpty(int num, int position)
        {
            return IsCellEmpty(num, position / 9, position % 9);
        }

        /// <summary>
        /// Checks if a cell, represented by its row and column indeces, is empty.
        /// </summary>
        /// <param name="num">The number table. Can be from 0 to 9.</param>
        /// <param name="row">The row index of the cell examined.</param>
        /// <param name="col">The column index of the cell examined.</param>
        /// <returns>True if the cell is empty, false otherwise.</returns>
        public bool IsCellEmpty(int num, int row, int col)
        {
            return exercise[num][row, col] == EMPTY;
        }

        /// <summary>
        /// Set the given cell to occupied.
        /// </summary>
        /// <param name="num">The number table. Can be from 0 to 9.</param>
        /// <param name="row">The row index of the cell.</param>
        /// <param name="col">The column index of the cell.</param>
        public void MakeCellOccupied(int num, int row, int col)
        {
            exercise[num][row, col] = OCCUPIED;
        }

        /// <summary>
        /// Searches for the first empty cell after the given position.
        /// </summary>
        /// <param name="p">The start position of the cell from where the search starts.</param>
        public void RecalculateFirstEmptyCell(int p)
        {
            if (p == FirstEmptyCell && NumberOfEmptyCells != 0)
            {
                while (FirstEmptyCell < LAST_CELL && !IsCellEmpty(0, FirstEmptyCell))
                    FirstEmptyCell++;
            }
        }

        /// <summary>
        /// Checks if the exercise is full.
        /// </summary>
        /// <returns>True if the exercise is full, false otherwise.</returns>
        public bool IsExerciseFull()
        {
            return NumberOfEmptyCells == 0;
        }

        /// <summary>
        /// Checks if the exercise is empty.
        /// </summary>
        /// <returns>True if the exercise is empty, false otherwise.</returns>
        public bool IsExerciseEmpty()
        {
            return NumberOfEmptyCells == LAST_CELL;
        }

        /// <summary>
        /// Check if the given position represents the last cell (bottom right) in the table.
        /// </summary>
        /// <param name="p">The position.</param>
        /// <returns>True if the given position is the last cell, false otherwise.</returns>
        public bool IsCellTheLastInTable(int p)
        {
            return p == LAST_CELL;
        }

        /// <summary>
        /// Megoldja a feladatot visszalépéses algoritmus használata nélkül.
        /// Akkor oldható meg backtrack nélkül egy feladat, ha csak a sima kitöltést elvégezve nem marad üres cella a táblában.
        /// Ez a függvény eldönti, hogy a feladat megoldaható-e backtrack nélkül, sima kitöltéssel vagy sem.
        /// </summary>
        /// <param name="numberOfEmptyCells">Az üres cellák száma</param>
        /// <returns>Ha megoldható backtrack használata nélkül, akkor true, egyébként false</returns>
        public bool IsExerciseSolvableWithoutBackTrack(int numberOfEmptyCells)
        {
            //az üres cellák száma a generáláskori üres cellák száma lesz
            NumberOfEmptyCells = numberOfEmptyCells;

            return new WithoutBackTrackSolver().SolveExerciseWithoutBackTrack();
        }

        #endregion
        #endregion
    }
}
