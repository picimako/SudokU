namespace Sudoku.Controller.Finder
{
    /// <summary>
    /// Enum for storing directions.
    /// They are used when searching for neighbour cells in Killer Sudoku exercises.
    /// </summary>
    public enum Direction
    {
        LEFT,   //[i, j - 1]
        RIGHT,  //[i, j + 1]
        UP,     //[i - 1, j]
        DOWN    //[i + 1, j]
    }
}
