using Sudoku.Controller.Finder;

namespace Sudoku
{
    //TODO: move this to the Cells namespace
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

        public bool IsInFirstRow()
        {
            return this.Row == 0;
        }

        public bool IsInLastRow()
        {
            return this.Row == 8;
        }

        public bool IsInFirstColumn()
        {
            return this.Col == 0;
        }

        public bool IsInLastColumn()
        {
            return this.Col == 8;
        }

        public bool IsInSameRowAs(Cell cell)
        {
            return this.Row == cell.Row;
        }

        /// <summary>
        /// Not right above, just in a higher row in the table.
        /// </summary>
        public bool IsAbove(Cell cell)
        {
            return this.Row < cell.Row;
        }

        /// <summary>
        /// Not right to the left but somewhere to the left of cell.
        /// </summary>
        public bool IsAtLeftOf(Cell cell)
        {
            return this.Col < cell.Col;
        }

        public void CopyIndecesOf(Cell cell)
        {
            this.Row = cell.Row;
            this.Col = cell.Col;
        }

        public Cell WithAlteredIndecesByDirection(Direction direction)
        {
            int row = this.Row, col = this.Col;
            switch (direction)
            {
                case Direction.LEFT:
                    --col;
                    break;
                case Direction.RIGHT:
                    ++col;
                    break;
                case Direction.UP:
                    --row;
                    break;
                case Direction.DOWN:
                    ++row;
                    break;
            }
            return new Cell(row, col);
        }

        public override bool Equals(object obj)
        {
            return (obj as Cell).Row == this.Row && (obj as Cell).Col == this.Col;
        }

        #endregion
    }
}
