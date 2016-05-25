namespace Sudoku
{
    //TODO: move this to the Cells namespace
    //TODO: create a CellPositionComparator and add it to this class. Could be used in KillerSudokuController.GetSumOfNumbersAndIndicatorCages()
    public class Cell
    {
        #region Members

        private int i;
        private int j;

        #endregion

        #region Properties

        public int Row { get { return i; } set { i = value; } }
        public int Col { get { return j; } set { j = value; } }

        public int CageIndex { get { return i; } set { i = value; } }
        //public int Value { get { return j; } set { j = value; } }

        public static Cell OUT_OF_RANGE { get { return new Cell(-1, -1); } }

        #endregion

        #region Constructor

        public Cell() { }

        public Cell(int i, int j)
        {
            this.i = i;
            this.j = j;
        }

        public override bool Equals(object obj)
        {
            return (obj as Cell).Row == this.Row && (obj as Cell).Col == this.Col;
        }

        #endregion
    }
}
