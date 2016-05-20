﻿using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using Sudoku.Cells;
using Sudoku.Controller;
using Sudoku.Language;
using Sudoku.Util;
using static Sudoku.Cells.CellHandler;
using static Sudoku.Verifier.ExerciseResultVerifier;

namespace Sudoku.Dialog
{
    class UITableHandler
    {
        #region Members

        //Each TextBox is a cell on the UI to contain the values of the exercise
        private TextBox[,] guiTable;
        private SudokuExercise se = SudokuExercise.get;
        private LocHandler loc = LocHandler.get;
        private ConfigHandler conf = ConfigHandler.get;
        private CellHandler cellHandler;

        private Button verifyExerciseButton;
        private Color[] colors = new ColorListFactory().GetColors();
        private Label numberOfEmptyCellsLabel;
        private TableLayoutPanel exerciseTable;
        /*The previous value of the cell. To be able to change the number tables, I need to know the value of the cell before
         removing its value.*/
        private int previousCellValue;

        #endregion

        #region Properties

        public TextBox[,] GUITable
        {
            get { return guiTable; }
            set { guiTable = value; }
        }

        #endregion

        #region Methods

        #region Constructor

        public UITableHandler(Button verifyExerciseButton, Label numberOfEmptyCellsLabel, TableLayoutPanel exerciseTable)
        {
            this.exerciseTable = exerciseTable;
            this.verifyExerciseButton = verifyExerciseButton;
            this.numberOfEmptyCellsLabel = numberOfEmptyCellsLabel;
            CreateTableOnGUI();
        }

        #endregion

        #region Public

        public void ReloadTableForRestart(int[][,] exerciseBackup)
        {
            for (int p = 0; p < se.LAST_CELL_INDEX; p++)
            {
                //Since changing the value of the cell, the eventhandler gets temporarily removed
                guiTable[p / 9, p % 9].TextChanged -= new EventHandler(this.TextBox_TextChanged);
                //Changing the value of the cell to the value at the beginning of the exercise
                guiTable[p / 9, p % 9].Text = (exerciseBackup[0][p / 9, p % 9] > 0) ? exerciseBackup[0][p / 9, p % 9].ToString() : "";
                guiTable[p / 9, p % 9].TextChanged += new EventHandler(this.TextBox_TextChanged);
            }
        }

        public void FillTableOnGUI()
        {
            Dictionary<Cell, int> cageCornersAndSums = new Dictionary<Cell, int>();
            if (se.IsExerciseKiller)
                cageCornersAndSums = se.Killer.Ctrl.GetSumOfNumbersAndIndicatorCages();
            guiTable = new TextBox[9, 9];

            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    if (se.IsExerciseKiller)
                    {
                        foreach (KeyValuePair<Cell, int> cage in cageCornersAndSums)
                        {
                            if (se.Killer.Ctrl.IsCellAtTopLeftOfCage(cage, row, col))
                            {
                                guiTable[row, col] = IsCellSpecial(row, col)
                                    ? new KillerTextBox(cage.Value.ToString(), true)
                                    : new KillerTextBox(cage.Value.ToString(), false);

                                //Removing the used cage key, so in the next round 
                                //the foreach won't iterate through the already processed cages
                                cageCornersAndSums.Remove(cage.Key);
                                break;
                            }
                        }
                    }

                    //If there is no TextBox placed
                    if (guiTable[row, col] == null)
                    {
                        guiTable[row, col] = se.IsExerciseKiller && IsCellSpecial(row, col) ? new KillerTextBox("", true) : new TextBox();
                        guiTable[row, col].TextAlign = HorizontalAlignment.Center;
                    }
                    guiTable[row, col].Font = new Font(guiTable[row, col].Font.FontFamily, 19.5f);

                    //No border
                    guiTable[row, col].BorderStyle = BorderStyle.None;

                    //No margin
                    guiTable[row, col].Margin = new Padding(0);

                    //Setting the height of the cell
                    guiTable[row, col].Top = exerciseTable.Height / exerciseTable.RowCount;

                    //Anchoring the cell horizontally to the center
                    guiTable[row, col].Anchor = AnchorStyles.Left | AnchorStyles.Right;

                    //Maximum 1 character allowed as value
                    guiTable[row, col].MaxLength = 1;

                    //Setting the cell indeces in the Tag property for later usage
                    guiTable[row, col].Tag = new Cell(row, col);

                    if (!se.IsExerciseKiller)
                    {
                        if (se.Exercise[0][row, col] > 0)
                        {
                            guiTable[row, col].Text = se.Exercise[0][row, col].ToString();
                            //It is not allowed to edit the cells that contain the predefined values
                            guiTable[row, col].Enabled = false;
                        }
                        SetOriginalCellBackgroundColor(guiTable[row, col]);
                    }
                    else
                        SetOriginalKillerCellBackgroundColor(guiTable[row, col]);

                    guiTable[row, col].TextChanged += new EventHandler(TextBox_TextChanged);
                    guiTable[row, col].GotFocus += delegate(object sender, EventArgs e)
                    {
                        //Storing the indeces of the TextBox in focus
                        int _i = GetSenderTag(sender).row;
                        int _j = GetSenderTag(sender).col;
                        Int32.TryParse(guiTable[_i, _j].Text, out previousCellValue);
                    };

                    guiTable[row, col].KeyDown += new KeyEventHandler(TextBox_KeyDown);

                    exerciseTable.Controls.Add(guiTable[row, col], col, row);
                }
            }
            cellHandler = new CellHandler(guiTable);
        }

        public void CreateTableOnGUI()
        {
            //Making the table not visible to speed up the drawing
            exerciseTable.Visible = false;

            //No rows and columns in the table initially
            exerciseTable.RowStyles.Clear();
            exerciseTable.ColumnStyles.Clear();

            //The table has 9 row and 9 columns
            exerciseTable.RowCount = exerciseTable.ColumnCount = 9;

            for (int i = 0; i < 9; i++)
            {
                //Creating new row. It's height is 11% of the height of the table
                exerciseTable.RowStyles.Add(new RowStyle(SizeType.Percent, 11f));
                //Creating new column. It's width is 11% of the width of the table
                exerciseTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11f));
            }

            exerciseTable.BackColor = SystemColors.Window;
            exerciseTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
        }

        #endregion

        #region Private

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            //The indeces of the cell that has changed
            int row = GetSenderTag(sender).row;
            int col = GetSenderTag(sender).col;

            switch (e.KeyCode)
            {
                case Keys.Left:
                    col = cellHandler.FindNearestEditableCellLeft(row, col);
                    break;
                case Keys.Right:
                    col = cellHandler.FindNearestEditableCellRight(row, col);
                    break;
                case Keys.Up:
                    row = cellHandler.FindNearestEditableCellUp(row, col);
                    break;
                case Keys.Down:
                    row = cellHandler.FindNearestEditableCellDown(row, col);
                    break;
            }

            //Setting the focus to the desired cell
            guiTable[row, col].Focus();
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            //Indeces of the changed TextBox cell
            int row = GetSenderTag(sender).row;
            int col = GetSenderTag(sender).col;
            TextBox changedCell = guiTable[row, col];

            //If the cell is empty, the value got removed from it
            if (String.IsNullOrEmpty(changedCell.Text))
            {
                se.Exercise[0][row, col] = se.EMPTY;

                se.Ctrl.RegenerateNumberTablesForRemovedValue(previousCellValue, row, col);

                se.NumberOfEmptyCells++;

                verifyExerciseButton.Enabled = false;

                //Setting the original background color
                if (se.IsExerciseKiller)
                    SetOriginalKillerCellBackgroundColor(changedCell);
                else
                    SetOriginalCellBackgroundColor(changedCell);

                if (se.IsExerciseKiller && ToCheckSumOfNumbersBiggerInCage())
                {
                    int cageIndex = se.Killer.Exercise[row, col].CageIndex;
                    if (se.Killer.Ctrl.IsCurrentSumOfNumbersBiggerThanRealSum(cageIndex))
                    {
                        foreach (Cell cell in se.Killer.Cages[cageIndex].Cells)
                            FontUtil.SetTextboxFont(changedCell, guiTable[cell.row, cell.col], FontStyle.Regular);
                    }
                }

                previousCellValue = 0;
            }
            //If the cell has a value, then a value got added into it
            else
            {
                int valueOfChangedCell;

                if (IsInvalidValueSet(changedCell, out valueOfChangedCell))
                {
                    ClearCellWithInvalidValue(changedCell);
                    return;
                }

                se.Exercise[0][row, col] = se.Exercise[valueOfChangedCell][row, col] = valueOfChangedCell;
                se.Ctrl.MakeHousesOccupied(valueOfChangedCell, row, col);

                if (se.IsExerciseKiller)
                    se.Killer.Ctrl.ketrecKitolt(se.Killer.Exercise[row, col].CageIndex);

                //If the cell was empty, then decrementing the number of empty cells
                if (previousCellValue == se.EMPTY)
                    se.NumberOfEmptyCells--;

                //If there is no more empty cell, then the exercise can be verified
                if (se.IsExerciseFull())
                    verifyExerciseButton.Enabled = true;

                if (ToCheckSameNumberAlreadyInHouse())
                    if (!se.IsExerciseKiller)
                    {
                        if (se.Ctrl.HousesContainValue(row, col, valueOfChangedCell))
                            SetIncorrectCellBackground(changedCell);
                        else
                            SetOriginalCellBackgroundColor(changedCell);
                    }
                    else
                    {
                        if (se.Ctrl.HousesContainValue(row, col, valueOfChangedCell) || se.Killer.Ctrl.HouseContainsValue(row, col, valueOfChangedCell, se.Exercise[0]))
                            SetIncorrectCellBackground(changedCell);
                        else
                            SetOriginalKillerCellBackgroundColor(changedCell);
                    }

                if (se.IsExerciseKiller && ToCheckSumOfNumbersBiggerInCage())
                {
                    int cageIndex = se.Killer.Exercise[row, col].CageIndex;
                    if (se.Killer.Ctrl.IsCurrentSumOfNumbersBiggerThanRealSum(cageIndex))
                    {
                        foreach (Cell cell in se.Killer.Cages[cageIndex].Cells)
                            FontUtil.SetTextboxFont(changedCell, guiTable[cell.row, cell.col], FontStyle.Bold);
                    }
                }

                //Setting the previousCellValue to the current value of the cell.
                //This is needed for the case when the value is changed via selecting the text and not removing it before adding the new value.
                Int32.TryParse(changedCell.Text, out previousCellValue);
            }

            numberOfEmptyCellsLabel.Text = loc.Get("numof_empty_cells") + ": " + se.NumberOfEmptyCells;
        }

        private void SetOriginalCellBackgroundColor(TextBox cell)
        {
            Cell aCell = cell.Tag as Cell;
            cell.BackColor = IsCellSpecial(aCell.row, aCell.col) ? Color.LightBlue : Color.White;
        }

        private void SetOriginalKillerCellBackgroundColor(TextBox cell)
        {
            int row = (cell.Tag as Cell).row;
            int col = (cell.Tag as Cell).col;
            cell.BackColor =
                se.Killer.Exercise[row, col].CageIndex <= 10
                ? colors[se.Killer.Exercise[row, col].CageIndex]
                : colors[se.Killer.Exercise[row, col].CageIndex - 10];
        }

        private void SetIncorrectCellBackground(TextBox cell)
        {
            cell.BackColor = Color.Red;
        }

        private bool IsInvalidValueSet(TextBox cell, out int value)
        {
            //Zero, text and special characters are not allowed to type into the cells
            return !Int32.TryParse(cell.Text, out value) || cell.Text.Equals("0");
        }

        private void ClearCellWithInvalidValue(TextBox cell)
        {
            cell.TextChanged -= new EventHandler(this.TextBox_TextChanged);         
            cell.Clear();
            cell.TextChanged += new EventHandler(this.TextBox_TextChanged);
        }

        private Cell GetSenderTag(object sender)
        {
            return ((TextBox)sender).Tag as Cell;
        }

        #endregion

        #endregion
    }
}
