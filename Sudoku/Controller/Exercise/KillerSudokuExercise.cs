using System.Collections.Generic;
using Sudoku.Cells;

namespace Sudoku.Controller
{
    /// <summary>
    /// Represents a Killer Sudoku Exercise.
    /// It is a layout on top of a sudoku exercise.
    /// </summary>
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
        }

        public KillerSudokuController Ctrl
        {
            get { return controller; }
        }

        public Cell[,] Exercise
        {
            get { return killerExercise; }
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

        /// <summary>
        /// Returns whether the cell in parameter is placed in any cage.
        /// </summary>
        /// <param name="row">The row index of the cell.</param>
        /// <param name="col">The column index of the cell.</param>
        /// <returns>True if the cell is placed in any of the cages, otherwise false.</returns>
        public bool IsCellInAnyCage(int row, int col)
        {
            return killerExercise[row, col].CageIndex != 0;
        }

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

        #endregion
    }
}
