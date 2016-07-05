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

        private int[,] cageIndeces;

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

        public int[,] CageIndeces
        {
            get { return cageIndeces; }
        }

        #endregion

        #region Constructor

        public KillerSudokuExercise()
        {
            cageIndeces = new int[9, 9];
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
            return cageIndeces[row, col] != 0;
        }

        #endregion
    }
}
