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

        /// <summary> </summary>
        /// <param name="difficulty">A csúszkán megadott érték</param>
        /// <returns>Ha valami hiba adódik, akkor false, egyébként true</returns>
        public bool GenerateExercise(int difficulty, int killerDifficulty)
        {
            SudokuExercise se = SudokuExercise.get;
            se.Ctrl = new SudokuControllerFactory().CreateController(se.ExerciseType);
            //Ha nem generálom a feladatot
            if (!se.IsExerciseGenerated)
            {
                //se.Ctrl = new Generate.Factory().CreateSudokuController(se.ExerciseType, killer);
                //Ha nem killer feladatról van szó
                if (!se.IsExerciseKiller)
                {
                    //Beolvastatom a feladatot a megadott fájlból
                    if (!se.Ctrl.ReadSudoku(se.ExerciseFilePath))
                        return false;

                    /* megoldottFeladat-ba bekerül a megoldott feladat, illetve a sud-ban pedig a táblák beolvasás utáni állapota lesz
                     * plusz a szám tömbök is megfelelő értékekkel rendelkeznek*/
                    se.Ctrl.SolveExercise();
                }
                //Ha killer feladatról van szó
                else
                {
                    //Beolvasom a feladatot a fájlból
                    if (!se.Killer.Ctrl.ReadKillerSudoku(se.ExerciseFilePath))
                        return false;

                    //Üres cellák száma 81 lesz, tehát minden cella üres
                    se.NumberOfEmptyCells = 81;
                }
            }
            //Feladat generálása, majd a gen-ben levő Sudoku példány átadása ezen osztály Sudoku példányának (sud)
            else
            {
                ExerciseGenerator generator = new ExerciseGenerator(difficulty, killerDifficulty);

                /* A gen.generalas visszatér a benne levő control osztály sud kitöltőjével (amiben a kitöltendő feladat van a hozzá tartozó
                 * szám tömbökkel), illetve kimeneti (out) paraméterben pedig megkapom a megoldott feladatot*/
                generator.Generate();
            }

            return true;
        }
    }
}
