namespace Sudoku
{
    public class Pair
    {
        #region Members

        private int i;
        private int j;

        #endregion

        #region Properties

        //For table indexing
        public int row { get { return i; } set { i = value; } }
        public int col { get { return j; } set { j = value; } }

        public int CageIndex { get { return i; } set { i = value; } }
        //public int Value { get { return j; } set { j = value; } }

        #endregion

        #region Constructor

        public Pair() { }

        public Pair(int i, int j)
        {
            this.i = i;
            this.j = j;
        }

        #endregion
    }
}
