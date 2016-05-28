using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sudoku.Controller;
using Sudoku.Language;
using Sudoku.Util;

namespace Sudoku.Verifier
{
    public class ExerciseResultVerifier
    {
        #region Members

        private static ConfigHandler conf = ConfigHandler.get;

        private SudokuExercise se = SudokuExercise.get;
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

            if (Boolean.Parse(conf.GetConfig("rosszakMutat")))
            {
                bool isSolutionCorrect = true;
                for (int p = 0; p < se.LAST_CELL_INDEX; p++)
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
            else if (Boolean.Parse(conf.GetConfig("hanyRosszMutat")))
            {
                int numberOfIncorrectCells = 0;
                for (int p = 0; p < se.LAST_CELL_INDEX; p++)
                {
                    if (!IsFieldEmpty(p) && !IsFieldValueMatchCurrentCellValueInSolutionInPosition(p))
                        numberOfIncorrectCells++;
                }
                checkLabel.Text = loc.Get("show_wrong_number_label") + ": " + numberOfIncorrectCells;
            }
            else if (Boolean.Parse(conf.GetConfig("joVagyRosszMutat")))
            {
                for (int p = 0; p < se.LAST_CELL_INDEX; p++)
                {
                    if (!IsFieldValueMatchCurrentCellValueInSolutionInPosition(p))
                    {
                        PrintSolutionIsWrong();
                        return;
                    }
                }
                PrintSolutionIsGood();
            }
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

        private int ParseValueOfCurrentCellInPosition(int pos)
        {
            //No need for TryParse as text and special characters are not allowed in TextBox cells
            return Int32.Parse(activeTable[pos / 9, pos % 9].Text);
        }

        /// <summary>Megvizsgálja, hogy a feladat megoldásában van-e 0 értékű cella.</summary>
        /// <param name="solution">The solution of the exercise.</param>
        /// <returns>A nincs 0 értékű cella a megoldásban, akkor true, egyébként false</returns>
        public static bool IsExerciseCorrect(int[,] solution)
        {
            return !solution.OfType<int>().ToList().Contains(0);
        }

        /// <summary>Returns whether checking the "sum of numbers are bigger than the expected sum of numbers in a cage" is enabled</summary>
        public static bool ToCheckSumOfNumbersBiggerInCage()
        {
            return Boolean.Parse(conf.GetConfig("cageSum"));
        }

        /// <summary>Returns whether checking the "typed number is already in the changed house" is enabled.</summary>
        public static bool ToCheckSameNumberAlreadyInHouse()
        {
            return Boolean.Parse(conf.GetConfig("helpRed"));
        }

        #endregion

        #endregion
    }
}
