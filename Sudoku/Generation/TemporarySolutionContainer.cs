﻿using Sudoku.Controller;

namespace Sudoku.Generate
{
    sealed class TemporarySolutionContainer
    {
        private SudokuExercise se = SudokuExercise.Instance;
        private int[][,] solution;

        public int[][,] Solution
        {
            get { return solution; }
        }

        public void Initialize()
        {
            if (!se.IsExerciseKiller)
            {
                solution = Arrays.CreateInitializedArray();
            }
        }

    }
}
