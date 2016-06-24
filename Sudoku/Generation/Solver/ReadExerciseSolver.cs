using Sudoku.Controller;

namespace Sudoku.Generation.Solver
{
    /// <summary>
    /// Solver class for exercises read form file.
    /// </summary>
    public class ReadExerciseSolver
    {
        private SudokuExercise se = SudokuExercise.get;
        private WithoutBackTrackSolver solver = new WithoutBackTrackSolver();

        /// <summary>
        /// Generates the number tables based on the read values.
        /// Then solves the exercise and sets the necessary fields.
        /// </summary>
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

        /// <summary>
        /// Solves the exercise using backtrack algorithm.
        /// </summary>
        /// <returns>The solved exercise.</returns>
        private int[,] SolveExerciseWithBackTrack()
        {
            new BackTrackSolver().SolveExerciseWithBackTrack();
            return se.Exercise[0];
        }
    }
}
