using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using Sudoku.Controller;
using Sudoku;
using Sudoku.Generate;
using Sudoku.Language;

namespace Sudoku.Verifier
{
    public class ExerciseResultVerifier
    {
        #region Members

        private static SudokuExercise se = SudokuExercise.get;
        private static LocHandler loc = LocHandler.get;
        private static ConfigHandler conf = ConfigHandler.get;
        private static Label checkLabel;
        private static TextBox[,] activeTable;
        private static int currentCellValue;

        #endregion

        #region Methods
        #region Public

        public static void InitVerifier(Label checkLabelParam, TextBox[,] activeTableParam)
        {
            checkLabel = checkLabelParam;
            activeTable = activeTableParam;
        }

        public static void VerifyResult()
        {
            //Ha beolvasott Killer feladatom van
            if (se.IsExerciseKiller && !se.IsExerciseGenerated)
            {
                if (se.Killer.Ctrl.IsKillerSolutionCorrect())
                {
                    PrintSolutionIsGood();
                }
                PrintSolutionIsWrong();
                return;
            }            

            if (Boolean.Parse(conf.GetConfig("rosszakMutat")))
            {
                bool isSolutionCorrect = true;
                for (int p = 0; p < 81; p++)
                {
                    ParseValueOfCurrentCell(p);
                    //Ha a tábla adott cellája nem üres, és a beírt érték nem egyezik a megoldásbeli értékkel,
                    if (!IsFieldEmpty(p) && !IsFieldValueMatchValueInSolution(currentCellValue, p))
                    {
                        //akkor a cellában lévő írás dőlt típusú lesz.
                        SetTextboxFont(p, FontStyle.Italic);
                        isSolutionCorrect = false;
                    }
                    //egyéb esetben
                    else
                        SetTextboxFont(p, FontStyle.Regular);
                }
                if (isSolutionCorrect)
                    PrintSolutionIsGood();
            }
            else if (Boolean.Parse(conf.GetConfig("hanyRosszMutat")))
            {
                int numberOfIncorrectCells = 0;
                for (int p = 0; p < 81; p++)
                {
                    ParseValueOfCurrentCell(p);
                    //ha a tábla adott cellája nem üres, és a beírt érték nem egyezik a megoldásbeli értékkel,
                    if (!IsFieldEmpty(p) && !IsFieldValueMatchValueInSolution(currentCellValue, p))
                        numberOfIncorrectCells++;
                }
                checkLabel.Text = loc.GetLoc("show_wrong_number_label") + ": " + numberOfIncorrectCells;
            }
            else if (Boolean.Parse(conf.GetConfig("joVagyRosszMutat")))
            {
                for (int p = 0; p < 81; p++)
                {
                    ParseValueOfCurrentCell(p);
                    //Ha a beírt érték nem egyezik meg a megoldásbeli értékkel,
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

        private static void SetTextboxFont(int p, FontStyle fontStyle)
        {
            activeTable[p / 9, p % 9].Font = new Font(activeTable[p / 9, p % 9].Font.FontFamily, 19.5f, fontStyle);
        }

        private static bool IsFieldEmpty(int p)
        {
            return activeTable[p / 9, p % 9].Text == "";
        }

        private static bool IsFieldValueMatchValueInSolution(int tabla_i_j, int p)
        {
            return tabla_i_j == se.Solution[p / 9, p % 9];
        }

        private static void PrintSolutionIsGood()
        {
            checkLabel.Text = loc.GetLoc("good_wrong_solution_label") + " " + loc.GetLoc("good");
        }

        private static void PrintSolutionIsWrong()
        {
            checkLabel.Text = loc.GetLoc("good_wrong_solution_label") + " " + loc.GetLoc("wrong");
        }

        private static void ParseValueOfCurrentCell(int pos)
        {
            Int32.TryParse(activeTable[pos / 9, pos % 9].Text, out currentCellValue);
        }

        #endregion
        #endregion
    }
}
