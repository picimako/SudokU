using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Sudoku.Controller;
using Sudoku.Language;
using Sudoku.Util;


namespace Sudoku.Verifier
{
    //TODO: make this non-static
    public class ExerciseResultVerifier
    {
        #region Members

        private SudokuExercise se = SudokuExercise.get;
        private LocHandler loc = LocHandler.get;
        private ConfigHandler conf = ConfigHandler.get;
        private Label checkLabel;
        private TextBox[,] activeTable;
        private int currentCellValue;

        #endregion

        #region Methods

        #region Public

        public ExerciseResultVerifier(Label checkLabelParam, TextBox[,] activeTableParam)
        {
            this.checkLabel = checkLabelParam;
            this.activeTable = activeTableParam;
        }

        public void VerifyResult()
        {
            if (se.IsExerciseKiller && !se.IsExerciseGenerated)
            {
                if (se.Killer.Ctrl.IsKillerSolutionCorrect())
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
                    ParseValueOfCurrentCell(p);
                    if (!IsFieldEmpty(p) && !IsFieldValueMatchValueInSolution(currentCellValue, p))
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
                    ParseValueOfCurrentCell(p);
                    if (!IsFieldEmpty(p) && !IsFieldValueMatchValueInSolution(currentCellValue, p))
                        numberOfIncorrectCells++;
                }
                checkLabel.Text = loc.Get("show_wrong_number_label") + ": " + numberOfIncorrectCells;
            }
            else if (Boolean.Parse(conf.GetConfig("joVagyRosszMutat")))
            {
                for (int p = 0; p < se.LAST_CELL_INDEX; p++)
                {
                    ParseValueOfCurrentCell(p);
                    if (!IsFieldValueMatchValueInSolution(currentCellValue, p))
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
            return activeTable[p / 9, p % 9].Text == "";
        }

        private bool IsFieldValueMatchValueInSolution(int currentCellValue, int p)
        {
            return currentCellValue == se.Solution[p / 9, p % 9];
        }

        private void PrintSolutionIsGood()
        {
            checkLabel.Text = loc.Get("good_wrong_solution_label") + " " + loc.Get("good");
        }

        private void PrintSolutionIsWrong()
        {
            checkLabel.Text = loc.Get("good_wrong_solution_label") + " " + loc.Get("wrong");
        }

        private void ParseValueOfCurrentCell(int pos)
        {
            Int32.TryParse(activeTable[pos / 9, pos % 9].Text, out currentCellValue);
        }

        /// <summary>Megvizsgálja, hogy a feladat megoldásában van-e 0 értékű cella.</summary>
        /// <param name="solution">A feladat megoldása.</param>
        /// <returns>A nincs 0 értékű cella a megoldásban, akkor true, egyébként false</returns>
        public static bool IsExerciseCorrect(int[,] solution)
        {
            List<int> list = solution.OfType<int>().ToList<int>();
            return !list.Contains(0);
        }

        #endregion

        #endregion
    }
}
