using Sudoku.Controller;
using Sudoku.Generation.Solver;
using static Sudoku.Controller.SudokuExercise;

namespace Sudoku.Generate
{
    public class ExerciseGeneratorInitializer
    {
        /// <summary>
        /// Decides whether it needs to generate the exercise automatically or the user need to read it from
        /// file, and then acts so.
        /// </summary>
        /// <param name="difficulty">The difficulty of the exercise.</param>
        /// <param name="killerDifficulty">The difficulty of the exercise if it is a Killer one.</param>
        /// <returns>False in case of an error during exercise read from file, otherwise true.</returns>
        public bool GenerateExercise(int difficulty, int killerDifficulty)
        {
            SudokuExercise se = SudokuExercise.get;

            se.Difficulty = difficulty;
            se.KillerDifficulty = killerDifficulty;

            if (!se.IsExerciseGenerated)
            {
                se.SetControllerForCurrentExerciseType();
                se.InitExercise();
                if (!se.IsExerciseKiller)
                {
                    if (!ExerciseReader.ReadSudoku(se.ExerciseFilePath))
                        return false;

                    new ReadExerciseSolver().Solve();
                }
                else
                {
                    se.InitKillerExercise();
                    if (!ExerciseReader.ReadKillerSudoku(se.ExerciseFilePath))
                        return false;

                    //All cells are empty
                    se.NumberOfEmptyCells = LAST_CELL;
                }
            }
            else
            {
                new ExerciseGenerator().Generate();
            }
            return true;
        }
    }
}
