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
            SimpleSudokuController controller = null;
            switch (exerciseType)
            {
                case SudokuType.SimpleSudoku:
                    controller = new SimpleSudokuController();
                    break;
                case SudokuType.SudokuX:
                    controller = new SudokuXController();
                    break;
                case SudokuType.CenterDot:
                    controller = new CenterDotController();
                    break;
            }
            return controller;
        }
    }
}
