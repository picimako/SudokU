using System.Collections.Generic;
using Sudoku.Cells;

namespace Sudoku.Controller
{
    /// <summary>
    /// Represents a cage in Killer Sudoku exercises by the cells and the sum of numbers in it.
    /// </summary>
    public class Cage
    {
        private List<Cell> cells = new List<Cell>();
        private int sumOfNumbers;

        public List<Cell> Cells
        {
            get { return cells; }
        }

        public int SumOfNumbers
        {
            get { return sumOfNumbers; }
            set { sumOfNumbers = value; }
        }

        public Cage()
        {
            this.sumOfNumbers = 0;
        }

        public Cage(int sumOfNumbers)
        {
            this.sumOfNumbers = sumOfNumbers;
        }
    }
}
