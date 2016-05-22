using Sudoku.Controller;

namespace Sudoku.Generate
{
    public class ExerciseGeneratorInitializer
    {
        public ExerciseGeneratorInitializer() { }

        /// <summary> Decides whether it needs to generate the exercise automatically or the user need to read it from
        /// file, and then acts so.</summary>
        /// <param name="difficulty">The difficulty of the exercise.</param>
        /// <param name="killerDifficulty">The difficulty of the exercise if it is a Killer one.</param>
        /// <returns>False in case of an error, otherwise true.</returns>
        public bool GenerateExercise(int difficulty, int killerDifficulty)
        {
            SudokuExercise se = SudokuExercise.get;

            if (!se.IsExerciseGenerated)
            {
                se.SetControllerForCurrentExerciseType();
                if (!se.IsExerciseKiller)
                {
                    se.InitExercise();
                    if (!ExerciseReader.ReadSudoku(se.ExerciseFilePath))
                        return false;

                    se.Ctrl.SolveReadExercise();
                }
                else
                {
                    se.InitKillerExercise();
                    if (!ExerciseReader.ReadKillerSudoku(se.ExerciseFilePath))
                        return false;

                    //All cells are empty
                    se.NumberOfEmptyCells = se.LAST_CELL_INDEX;
                }
            }
            else
            {
                new ExerciseGenerator(difficulty, killerDifficulty).Generate();
            }
            return true;
        }
    }
}
