using System.Collections.Generic;
using Sudoku.Cells;

namespace Sudoku.Controller
{
    public class KillerSudokuExercise
    {
        #region Members

        //i: cage index
        //j: value in the exercise
        private Cell[,] killerExercise;

        //Id of cage, the cage itself
        private Dictionary<int, Cage> cages;
        private KillerSudokuController controller;

        #endregion

        #region Properties

        public Dictionary<int, Cage> Cages
        {
            get { return cages; }
            set { cages = value; }
        }

        public KillerSudokuController Ctrl
        {
            get { return controller; }
            set { controller = value; }
        }

        public Cell[,] Exercise
        {
            get { return killerExercise; }
            set { killerExercise = value; }
        }

        #endregion

        #region Constructor

        public KillerSudokuExercise()
        {
            InitExercise();
            cages = new Dictionary<int, Cage>();
            controller = new KillerSudokuController();
        }

        #endregion

        #region Methods

        private void InitExercise()
        {
            killerExercise = new Cell[9, 9];
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    killerExercise[i, j] = new Cell();
                }
            }
        }

        public bool IsCellInAnyCage(int row, int col)
        {
            return killerExercise[row, col].CageIndex != 0;
        }

        #endregion
    }
}
