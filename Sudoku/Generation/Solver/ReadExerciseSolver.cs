using Sudoku.Controller;

namespace Sudoku.Generation.Solver
{
    public class ReadExerciseSolver
    {
        SudokuExercise se = SudokuExercise.get;
        WithoutBackTrackSolver solver = new WithoutBackTrackSolver();

        public void Solve()
        {
            se.Ctrl.GenerateValuesInNumberTables();

            int[][,] exerciseInitialState = Arrays.CreateInitializedArray();
            Arrays.CopyJaggedThreeDimensionArray(exerciseInitialState, se.Exercise);
            int originalNumberOfEmptyCells = se.NumberOfEmptyCells;

            if (!solver.SolveExerciseWithoutBackTrack())
                //WARN: investigate this part as there may be a problem generating exercises that need solving
                //with backtrack
                se.Solution = SolveExerciseWithBackTrack();
            else
            {
                se.Solution = new int[9, 9];
                Arrays.CopyTwoDimensionArray(se.Solution, se.Exercise[0]);
            }

            Arrays.CopyJaggedThreeDimensionArray(se.Exercise, exerciseInitialState);
            se.NumberOfEmptyCells = originalNumberOfEmptyCells;
        }

        /// <summary>Solves the exercise using backtrack algorithm.</summary>
        /// <returns>The solved exercise.</returns>
        private int[,] SolveExerciseWithBackTrack()
        {
            new BackTrackSolver().SolveExerciseWithBackTrack();
            return se.Exercise[0];
        }
    }
}
