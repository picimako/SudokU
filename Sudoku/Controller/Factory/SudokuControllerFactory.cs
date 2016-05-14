using Sudoku.Controller;

namespace Sudoku.Generate
{
    class SudokuControllerFactory
    {
        /// <summary>A megadott típusú feladathoz tartozó Sudoku osztálypéldányt ad vissza</summary>
        /// <param name="exerciseType">A feladat típusa</param>
        /// <param name="killer">Megadja, hogy a feladat Killer-e vagy sem</param>
        public SimpleSudokuController CreateController(SudokuType exerciseType)
        {
            SimpleSudokuController s = null;
            switch (exerciseType)
            {
                case SudokuType.SimpleSudoku:
                    s = new SimpleSudokuController();
                    break;
                case SudokuType.SudokuX:
                    s = new SudokuXController();
                    break;
                case SudokuType.CenterDot:
                    s = new CenterDotController();
                    break;
            }
            return s;
        }
    }
}
