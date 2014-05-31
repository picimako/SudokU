namespace Sudoku.Dialogusok
{
    partial class SudokuApp
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.generalButton = new System.Windows.Forms.Button();
            this.restartExerciseButton = new System.Windows.Forms.Button();
            this.exerciseTypeGroup = new System.Windows.Forms.GroupBox();
            this.centerButton = new System.Windows.Forms.RadioButton();
            this.xButton = new System.Windows.Forms.RadioButton();
            this.sudButton = new System.Windows.Forms.RadioButton();
            this.exerciseTable = new System.Windows.Forms.TableLayoutPanel();
            this.beolvasButton = new System.Windows.Forms.Button();
            this.difficultyBar = new System.Windows.Forms.TrackBar();
            this.difficultyLabel = new System.Windows.Forms.Label();
            this.verifyExerciseButton = new System.Windows.Forms.Button();
            this.verifyExerciseLabel = new System.Windows.Forms.Label();
            this.stopExerciseButton = new System.Windows.Forms.Button();
            this.killerBox = new System.Windows.Forms.CheckBox();
            this.numberOfEmptyCellsLabel = new System.Windows.Forms.Label();
            this.killerDifficultyBar = new System.Windows.Forms.TrackBar();
            this.killerDifficultyLabel = new System.Windows.Forms.Label();
            this.exerciseTypeGroup.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.difficultyBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.killerDifficultyBar)).BeginInit();
            this.SuspendLayout();
            // 
            // generalButton
            // 
            this.generalButton.Location = new System.Drawing.Point(11, 159);
            this.generalButton.Name = "generalButton";
            this.generalButton.Size = new System.Drawing.Size(69, 24);
            this.generalButton.TabIndex = 0;
            this.generalButton.UseVisualStyleBackColor = true;
            this.generalButton.Click += new System.EventHandler(this.GeneralButton_Click);
            // 
            // ujraButton
            // 
            this.restartExerciseButton.Location = new System.Drawing.Point(500, 144);
            this.restartExerciseButton.Name = "ujraButton";
            this.restartExerciseButton.Size = new System.Drawing.Size(112, 39);
            this.restartExerciseButton.TabIndex = 4;
            this.restartExerciseButton.UseVisualStyleBackColor = true;
            this.restartExerciseButton.Click += new System.EventHandler(this.RestartExerciseButton_Click);
            // 
            // exerciseTypeGroup
            // 
            this.exerciseTypeGroup.Controls.Add(this.centerButton);
            this.exerciseTypeGroup.Controls.Add(this.xButton);
            this.exerciseTypeGroup.Controls.Add(this.sudButton);
            this.exerciseTypeGroup.Location = new System.Drawing.Point(12, 42);
            this.exerciseTypeGroup.Name = "exerciseTypeGroup";
            this.exerciseTypeGroup.Size = new System.Drawing.Size(152, 88);
            this.exerciseTypeGroup.TabIndex = 6;
            this.exerciseTypeGroup.TabStop = false;
            // 
            // centerButton
            // 
            this.centerButton.AutoSize = true;
            this.centerButton.Location = new System.Drawing.Point(7, 65);
            this.centerButton.Name = "centerButton";
            this.centerButton.Size = new System.Drawing.Size(14, 13);
            this.centerButton.TabIndex = 3;
            this.centerButton.UseVisualStyleBackColor = true;
            // 
            // xButton
            // 
            this.xButton.AutoSize = true;
            this.xButton.Location = new System.Drawing.Point(7, 42);
            this.xButton.Name = "xButton";
            this.xButton.Size = new System.Drawing.Size(14, 13);
            this.xButton.TabIndex = 1;
            this.xButton.UseVisualStyleBackColor = true;
            // 
            // sudButton
            // 
            this.sudButton.AutoSize = true;
            this.sudButton.Checked = true;
            this.sudButton.Location = new System.Drawing.Point(7, 20);
            this.sudButton.Name = "sudButton";
            this.sudButton.Size = new System.Drawing.Size(14, 13);
            this.sudButton.TabIndex = 0;
            this.sudButton.TabStop = true;
            this.sudButton.UseVisualStyleBackColor = true;
            // 
            // table
            // 
            this.exerciseTable.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Inset;
            this.exerciseTable.ColumnCount = 1;
            this.exerciseTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.exerciseTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.exerciseTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.exerciseTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.exerciseTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.exerciseTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.exerciseTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.exerciseTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.exerciseTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.exerciseTable.Location = new System.Drawing.Point(183, 42);
            this.exerciseTable.Name = "table";
            this.exerciseTable.RowCount = 1;
            this.exerciseTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.exerciseTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 278F));
            this.exerciseTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 278F));
            this.exerciseTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 278F));
            this.exerciseTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 278F));
            this.exerciseTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 278F));
            this.exerciseTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 278F));
            this.exerciseTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 278F));
            this.exerciseTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 278F));
            this.exerciseTable.Size = new System.Drawing.Size(280, 280);
            this.exerciseTable.TabIndex = 0;
            // 
            // beolvasButton
            // 
            this.beolvasButton.Location = new System.Drawing.Point(86, 159);
            this.beolvasButton.Name = "beolvasButton";
            this.beolvasButton.Size = new System.Drawing.Size(77, 23);
            this.beolvasButton.TabIndex = 12;
            this.beolvasButton.UseVisualStyleBackColor = true;
            this.beolvasButton.Click += new System.EventHandler(this.BeolvasButton_Click);
            // 
            // nehezsegSav
            // 
            this.difficultyBar.Location = new System.Drawing.Point(35, 206);
            this.difficultyBar.Name = "nehezsegSav";
            this.difficultyBar.Size = new System.Drawing.Size(98, 45);
            this.difficultyBar.TabIndex = 13;
            // 
            // nehezsegLabel
            // 
            this.difficultyLabel.AutoSize = true;
            this.difficultyLabel.Location = new System.Drawing.Point(51, 238);
            this.difficultyLabel.Name = "nehezsegLabel";
            this.difficultyLabel.Size = new System.Drawing.Size(0, 13);
            this.difficultyLabel.TabIndex = 14;
            // 
            // ellenorizButton
            // 
            this.verifyExerciseButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(238)));
            this.verifyExerciseButton.Location = new System.Drawing.Point(500, 42);
            this.verifyExerciseButton.Name = "ellenorizButton";
            this.verifyExerciseButton.Size = new System.Drawing.Size(112, 23);
            this.verifyExerciseButton.TabIndex = 19;
            this.verifyExerciseButton.UseVisualStyleBackColor = true;
            this.verifyExerciseButton.Click += new System.EventHandler(this.ellenorizButton_Click);
            // 
            // ellenorzesLabel
            // 
            this.verifyExerciseLabel.AutoSize = true;
            this.verifyExerciseLabel.Location = new System.Drawing.Point(509, 68);
            this.verifyExerciseLabel.Name = "ellenorzesLabel";
            this.verifyExerciseLabel.Size = new System.Drawing.Size(0, 13);
            this.verifyExerciseLabel.TabIndex = 20;
            this.verifyExerciseLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // feladatStopButton
            // 
            this.stopExerciseButton.Location = new System.Drawing.Point(500, 189);
            this.stopExerciseButton.Name = "feladatStopButton";
            this.stopExerciseButton.Size = new System.Drawing.Size(112, 36);
            this.stopExerciseButton.TabIndex = 22;
            this.stopExerciseButton.UseVisualStyleBackColor = true;
            this.stopExerciseButton.Click += new System.EventHandler(this.feladatStopButton_Click);
            // 
            // killerBox
            // 
            this.killerBox.AutoSize = true;
            this.killerBox.Location = new System.Drawing.Point(19, 136);
            this.killerBox.Name = "killerBox";
            this.killerBox.Size = new System.Drawing.Size(15, 14);
            this.killerBox.TabIndex = 23;
            this.killerBox.UseVisualStyleBackColor = true;
            // 
            // uCSzLabel
            // 
            this.numberOfEmptyCellsLabel.AutoSize = true;
            this.numberOfEmptyCellsLabel.Location = new System.Drawing.Point(497, 247);
            this.numberOfEmptyCellsLabel.Name = "uCSzLabel";
            this.numberOfEmptyCellsLabel.Size = new System.Drawing.Size(0, 13);
            this.numberOfEmptyCellsLabel.TabIndex = 24;
            // 
            // killerSav
            // 
            this.killerDifficultyBar.Location = new System.Drawing.Point(35, 273);
            this.killerDifficultyBar.Name = "killerSav";
            this.killerDifficultyBar.Size = new System.Drawing.Size(98, 45);
            this.killerDifficultyBar.TabIndex = 25;
            // 
            // killerNehezsegLabel
            // 
            this.killerDifficultyLabel.AutoSize = true;
            this.killerDifficultyLabel.Location = new System.Drawing.Point(41, 305);
            this.killerDifficultyLabel.Name = "killerNehezsegLabel";
            this.killerDifficultyLabel.Size = new System.Drawing.Size(0, 13);
            this.killerDifficultyLabel.TabIndex = 26;
            // 
            // SudokuApp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(633, 347);
            this.Controls.Add(this.killerDifficultyLabel);
            this.Controls.Add(this.killerDifficultyBar);
            this.Controls.Add(this.numberOfEmptyCellsLabel);
            this.Controls.Add(this.killerBox);
            this.Controls.Add(this.stopExerciseButton);
            this.Controls.Add(this.verifyExerciseLabel);
            this.Controls.Add(this.verifyExerciseButton);
            this.Controls.Add(this.exerciseTable);
            this.Controls.Add(this.difficultyLabel);
            this.Controls.Add(this.difficultyBar);
            this.Controls.Add(this.beolvasButton);
            this.Controls.Add(this.exerciseTypeGroup);
            this.Controls.Add(this.restartExerciseButton);
            this.Controls.Add(this.generalButton);
            this.Name = "SudokuApp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.SudokuApp_Load);
            this.exerciseTypeGroup.ResumeLayout(false);
            this.exerciseTypeGroup.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.difficultyBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.killerDifficultyBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button generalButton;
        private System.Windows.Forms.Button restartExerciseButton;
        private System.Windows.Forms.GroupBox exerciseTypeGroup;
        private System.Windows.Forms.RadioButton xButton;
        private System.Windows.Forms.RadioButton sudButton;
        private System.Windows.Forms.RadioButton centerButton;
        private System.Windows.Forms.TableLayoutPanel exerciseTable;
        private System.Windows.Forms.Button beolvasButton;
        private System.Windows.Forms.TrackBar difficultyBar;
        private System.Windows.Forms.Label difficultyLabel;
        private System.Windows.Forms.Button verifyExerciseButton;
        private System.Windows.Forms.Label verifyExerciseLabel;
        private System.Windows.Forms.Button stopExerciseButton;
        private System.Windows.Forms.CheckBox killerBox;
        private System.Windows.Forms.Label numberOfEmptyCellsLabel;
        private System.Windows.Forms.TrackBar killerDifficultyBar;
        private System.Windows.Forms.Label killerDifficultyLabel;
    }
}

