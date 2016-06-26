using Sudoku.Controller.Finder;

namespace Sudoku.Cells
{
    /// <summary>
    /// Represents a cell with a row and column index.
    /// Also used for storing the cage index of a cell for Killer Sudoku exercises.
    /// </summary>
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

        /// <summary>
        /// Returns whether the cell is in the first row of a table.
        /// </summary>
        /// <returns>True if the cell is in the first row, otherwise false.</returns>
        public bool IsInFirstRow()
        {
            return this.Row == 0;
        }

        /// <summary>
        /// Returns whether the cell is in the last row of a table.
        /// </summary>
        /// <returns>True if the cell is in the last row, otherwise false.</returns>
        public bool IsInLastRow()
        {
            return this.Row == 8;
        }

        /// <summary>
        /// Returns whether the cell is in the first column of a table.
        /// </summary>
        /// <returns>True if the cell is in the first column, otherwise false.</returns>
        public bool IsInFirstColumn()
        {
            return this.Col == 0;
        }

        /// <summary>
        /// Returns whether the cell is in the last column of a table.
        /// </summary>
        /// <returns>True if the cell is in the last column, otherwise false.</returns>
        public bool IsInLastColumn()
        {
            return this.Col == 8;
        }

        /// <summary>
        /// Compares this and the cell provided as argument whether they are in the same row.
        /// </summary>
        /// <param name="cell">The cell to compare this cell to.</param>
        /// <returns>True if this cell is in the same row as the one passed as param, otherwise false.</returns>
        public bool IsInSameRowAs(Cell cell)
        {
            return this.Row == cell.Row;
        }

        /// <summary>
        /// Compares this and the cell provided as argument whether they are in the same column.
        /// </summary>
        /// <param name="cell">The cell to compare this cell to.</param>
        /// <returns>True if this cell is in the same column as the one passed as param, otherwise false.</returns>
        public bool IsInSameColumnAs(Cell cell)
        {
            return this.Col == cell.Col;
        }

        /// <summary>
        /// Compares this and the cell provided as argument whether this one is above the other one.
        /// Not right above, just in a higher row in the table.
        /// </summary>
        /// <param name="cell">The cell to compare this cell to.</param>
        /// <returns>True if this cell is above as the one passed as param, otherwise false.</returns>
        public bool IsAbove(Cell cell)
        {
            return this.Row < cell.Row;
        }

        /// <summary>
        /// Compares this and the cell provided as argument whether this one is at the left of the other one.
        /// Not right to the left but somewhere to the left of cell.
        /// </summary>
        /// <param name="cell">The cell to compare this cell to.</param>
        /// <returns>True if this cell is to the left of the one passed as param, otherwise false.</returns>
        public bool IsAtLeftOf(Cell cell)
        {
            return this.Col < cell.Col;
        }

        /// <summary>
        /// Copies the row and column indeces of the cell provided as argument to the this cell instance.
        /// </summary>
        /// <param name="cell">The cell whose indeces are copied.</param>
        public void CopyIndecesOf(Cell cell)
        {
            this.Row = cell.Row;
            this.Col = cell.Col;
        }

        /// <summary>
        /// Returns a new Cell instance with modified indeces (only a single row or column shift)
        /// according to the direction specified. 
        /// </summary>
        /// <param name="direction">The direction to alter the indeces of this cell.</param>
        /// <returns>A new Cell instance with altered indeces compared to the current Cell instance.</returns>
        public Cell WithAlteredIndecesByDirection(Direction direction)
        {
            int row = this.Row;
            int col = this.Col;
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
