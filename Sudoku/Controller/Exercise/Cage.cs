using System.Collections.Generic;

namespace Sudoku.Controller
{
    public class Cage
    {
        //List of cells in the cage - the sum of numbers in the cage
        private List<Pair> cells;
        private int sumOfNumbers;

        public List<Pair> Cells
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
            this.cells = new List<Pair>();
            this.sumOfNumbers = 0;
        }

        public Cage(int sumOfNumbers)
        {
            cells = new List<Pair>();
            this.sumOfNumbers = sumOfNumbers;
        }
    }
}
