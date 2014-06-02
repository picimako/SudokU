using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
            se.Ctrl = new SudokuControllerFactory().CreateController(se.ExerciseType);
            if (!se.IsExerciseGenerated)
            {
                //se.Ctrl = new Generate.Factory().CreateSudokuController(se.ExerciseType, killer);
                if (!se.IsExerciseKiller)
                {
                    if (!se.Ctrl.ReadSudoku(se.ExerciseFilePath))
                        return false;

                    se.Ctrl.SolveExercise();
                }
                else
                {
                    if (!se.Killer.Ctrl.ReadKillerSudoku(se.ExerciseFilePath))
                        return false;

                    //All cells are empty
                    se.NumberOfEmptyCells = 81;
                }
            }
            else
            {
                ExerciseGenerator generator = new ExerciseGenerator(difficulty, killerDifficulty);
                generator.Generate();
            }
            return true;
        }
    }
}
