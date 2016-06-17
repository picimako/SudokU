using Sudoku.Generate;
using Sudoku.Generation.Solver;

namespace Sudoku.Controller
{
    public class SudokuExercise
    {
        #region Members

        private static SudokuExercise ARRAY;
        private const int LAST_CELL = 81;
        private const int EMPTY_CELL = 0;
        private const int OCCUPIED_CELL = -1;

        private SimpleSudokuController controller;
        private KillerSudokuExercise killerExercise;
        //TODO: it might not be a good idea to have an ExerciseSolver in SudokuExercise
        //and then in ExerciseSolver make changes on SudokuExercise, but will do the work for now
        private ExerciseSolver solver;

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

        public ExerciseSolver Solver
        {
            get { return solver; }
        }

        public int LAST_CELL_INDEX
        {
            get { return LAST_CELL; }
        }

        public int OCCUPIED
        {
            get { return OCCUPIED_CELL; }
        }

        public int EMPTY
        {
            get { return EMPTY_CELL; }
        }

        public int NumberOfEmptyCells { get; set; }

        public int FirstEmptyCell { get; set; }

        public SudokuType ExerciseType { get; set; }

        public bool IsExerciseKiller { get; set; }

        public bool IsExerciseGenerated { get; set; }

        public string ExerciseFilePath { get; set; }

        #endregion

        #region Constructor

        private SudokuExercise() { }

        public void InitExercise()
        {
            NumberOfEmptyCells = 0;
            exercise = Arrays.CreateInitializedArray();
            solver = new ExerciseSolver();
        }

        public void InitKillerExercise()
        {
            killerExercise = new KillerSudokuExercise();
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

        public void SetControllerForCurrentExerciseType()
        {
            controller = new SudokuControllerFactory().CreateController(ExerciseType);
        }

        public bool IsCellEmpty(int num, int row, int col)
        {
            return exercise[num][row, col] == EMPTY;
        }

        public bool IsCellEmpty(int num, int position)
        {
            return exercise[num][position / 9, position % 9] == EMPTY;
        }

        public void MakeCellOccupied(int num, int row, int col)
        {
            exercise[num][row, col] = OCCUPIED;
        }

        /// <summary>Finds the first empty cell</summary>
        /// <param name="p">The currently examined cell.</param>
        public void RecalculateFirstEmptyCell(int p)
        {
            if (p == FirstEmptyCell && NumberOfEmptyCells != 0)
            {
                while (FirstEmptyCell < LAST_CELL && !IsCellEmpty(0, FirstEmptyCell))
                    FirstEmptyCell++;
            }
        }

        public bool IsExerciseFull()
        {
            return NumberOfEmptyCells == 0;
        }

        public bool IsExerciseEmpty()
        {
            return NumberOfEmptyCells == LAST_CELL;
        }

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

            return solver.SolveExerciseWithoutBackTrack();
        }

        #endregion
        #endregion
    }
}
