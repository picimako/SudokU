using Sudoku.Generate;

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

        private int[][,] exercise;
        private int[,] solution;
        
        #endregion

        #region Properties

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

        #endregion
        #endregion
    }
}
