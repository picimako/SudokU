﻿using Sudoku.Controller;

namespace Sudoku.Generate
{
    class SudokuControllerFactory
    {
        /// <summary>Creates a SudokuController based on the given exercise type.</summary>
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
