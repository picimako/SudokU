using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sudoku.Configuration;
using Sudoku.Controller;
using Sudoku.Language;
using Sudoku.Util;
using static Sudoku.Configuration.ConfigurationKeys;
using static Sudoku.Controller.SudokuExercise;

namespace Sudoku.Verifier
{
    public class ExerciseResultVerifier
    {
        #region Members

        private static ConfigHandler conf = ConfigHandler.get;

        private SudokuExercise se = SudokuExercise.Instance;
        private LocHandler loc = LocHandler.get;

        private KillerExerciseResultVerifier killerVerifier = new KillerExerciseResultVerifier();
        private Label checkLabel;
        private TextBox[,] activeTable;

        #endregion

        #region Methods

        #region Public

        public ExerciseResultVerifier(Label checkLabel, TextBox[,] activeTable)
        {
            this.checkLabel = checkLabel;
            this.activeTable = activeTable;
        }

        /// <summary>
        /// Verifies the result against various criteria based on different verification methods.
        /// Displays a result message based on whether the result is correct or not.
        /// </summary>
        public void VerifyResult()
        {
            if (se.IsExerciseKiller && !se.IsExerciseGenerated)
            {
                if (killerVerifier.IsKillerSolutionCorrect())
                    PrintSolutionIsGood();
                else
                    PrintSolutionIsWrong();
                return;
            }            

            if (conf.GetAsBool(SHOW_INCORRECT_CELLS_ENABLED))
            {
                VerifyShowIncorrectNumbers();
            }
            else if (conf.GetAsBool(SHOW_NUMBER_OF_INCORRECT_CELLS_ENABLED))
            {
                VerifyShowNumberOfIncorrectNumbers();
            }
            else if (conf.GetAsBool(SHOW_WHETHER_SOLUTION_IS_CORRECT_ENABLED))
            {
                VerifyShowWhetherSolutionIsCorrect();
            }
        }

        private void VerifyShowIncorrectNumbers()
        {
            bool isSolutionCorrect = true;
            for (int p = 0; p < LAST_CELL; p++)
            {
                if (!IsFieldEmpty(p) && !IsFieldValueMatchCurrentCellValueInSolutionInPosition(p))
                {
                    FontUtil.SetTextboxFont(activeTable[p / 9, p % 9], FontStyle.Italic);
                    isSolutionCorrect = false;
                }
                else
                    FontUtil.SetTextboxFont(activeTable[p / 9, p % 9], FontStyle.Regular);
            }
            if (isSolutionCorrect)
                PrintSolutionIsGood();
        }

        private void VerifyShowNumberOfIncorrectNumbers()
        {
            int numberOfIncorrectCells = 0;
            for (int p = 0; p < LAST_CELL; p++)
            {
                if (!IsFieldEmpty(p) && !IsFieldValueMatchCurrentCellValueInSolutionInPosition(p))
                    numberOfIncorrectCells++;
            }
            checkLabel.Text = loc.Get("show_wrong_number_label") + ": " + numberOfIncorrectCells;
        }

        private void VerifyShowWhetherSolutionIsCorrect()
        {
            for (int p = 0; p < LAST_CELL; p++)
            {
                if (!IsFieldValueMatchCurrentCellValueInSolutionInPosition(p))
                {
                    PrintSolutionIsWrong();
                    return;
                }
            }
            PrintSolutionIsGood();
        }

        #endregion

        #region Private

        private bool IsFieldEmpty(int p)
        {
            return String.IsNullOrEmpty(activeTable[p / 9, p % 9].Text);
        }

        private bool IsFieldValueMatchCurrentCellValueInSolutionInPosition(int p)
        {
            return ParseValueOfCurrentCellInPosition(p) == se.Solution[p / 9, p % 9];
        }

        private void PrintSolutionIsGood()
        {
            checkLabel.Text = loc.Get("good_wrong_solution_label") + " " + loc.Get("good");
        }

        private void PrintSolutionIsWrong()
        {
            checkLabel.Text = loc.Get("good_wrong_solution_label") + " " + loc.Get("wrong");
        }

        /// <remarks>
        /// No need for TryParse as text and special characters are not allowed in TextBox cells.
        /// </remarks>
        private int ParseValueOfCurrentCellInPosition(int pos)
        {
            return Int32.Parse(activeTable[pos / 9, pos % 9].Text);
        }

        /// <summary>
        /// Inspects whether the solution contains 0 value.
        /// </summary>
        /// <param name="solution">The solution of the exercise.</param>
        /// <returns>True if the exercise doesn't contains 0 value, meaning the solution is correct, otherwise false.</returns>
        public static bool IsExerciseCorrect(int[,] solution)
        {
            return !solution.OfType<int>().ToList().Contains(0);
        }

        /// <summary>
        /// Returns whether checking the "sum of numbers are bigger than the expected sum of numbers in a cage" is enabled.
        /// </summary>
        public static bool ToCheckSumOfNumbersBiggerInCage()
        {
            return conf.GetAsBool(SUM_OF_NUMBERS_BIGGER_IN_CAGE_CHECK_ENABLED);
        }

        /// <summary>
        /// Returns whether checking the "typed number is already in the changed house" is enabled.
        /// </summary>
        public static bool ToCheckSameNumberAlreadyInHouse()
        {
            return conf.GetAsBool(CELL_RED_BACKGROUND_ENABLED);
        }

        #endregion

        #endregion
    }
}
