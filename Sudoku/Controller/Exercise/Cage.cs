using System.Collections.Generic;
using Sudoku.Cells;

namespace Sudoku.Controller
{
    public class Cage
    {
        private List<Cell> cells = new List<Cell>();
        private int sumOfNumbers;

        public List<Cell> Cells
        {
            get { return cells; }
            set { cells = value; }
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
