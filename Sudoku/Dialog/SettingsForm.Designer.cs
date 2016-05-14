namespace Sudoku.Dialog
{
    partial class SettingsForm
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
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.filePathBox = new System.Windows.Forms.TextBox();
            this.browseButton = new System.Windows.Forms.Button();
            this.utvonalLabel = new System.Windows.Forms.Label();
            this.languageDropdown = new System.Windows.Forms.ComboBox();
            this.nyelvLabel = new System.Windows.Forms.Label();
            this.sameNumberAlreadyInHouseHintBox = new System.Windows.Forms.CheckBox();
            this.ellenorzesGroup = new System.Windows.Forms.GroupBox();
            this.showExerciseCorrectnessRadio = new System.Windows.Forms.RadioButton();
            this.showNumberOfWrongCellsRadio = new System.Windows.Forms.RadioButton();
            this.showWrongCellsRadio = new System.Windows.Forms.RadioButton();
            this.sumOfNumbersBiggerInCageHintBox = new System.Windows.Forms.CheckBox();
            this.ellenorzesGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(364, 206);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // megseButton
            // 
            this.cancelButton.Location = new System.Drawing.Point(445, 206);
            this.cancelButton.Name = "megseButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // utvonalBox
            // 
            this.filePathBox.Location = new System.Drawing.Point(12, 32);
            this.filePathBox.Name = "utvonalBox";
            this.filePathBox.ReadOnly = true;
            this.filePathBox.Size = new System.Drawing.Size(407, 20);
            this.filePathBox.TabIndex = 0;
            // 
            // tallozButton
            // 
            this.browseButton.Location = new System.Drawing.Point(425, 31);
            this.browseButton.Name = "tallozButton";
            this.browseButton.Size = new System.Drawing.Size(96, 20);
            this.browseButton.TabIndex = 1;
            this.browseButton.UseVisualStyleBackColor = true;
            // 
            // utvonalLabel
            // 
            this.utvonalLabel.AutoSize = true;
            this.utvonalLabel.Location = new System.Drawing.Point(12, 9);
            this.utvonalLabel.Name = "utvonalLabel";
            this.utvonalLabel.Size = new System.Drawing.Size(0, 13);
            this.utvonalLabel.TabIndex = 2;
            // 
            // nyelvCombo
            // 
            this.languageDropdown.FormattingEnabled = true;
            this.languageDropdown.Location = new System.Drawing.Point(364, 164);
            this.languageDropdown.Name = "nyelvCombo";
            this.languageDropdown.Size = new System.Drawing.Size(121, 21);
            this.languageDropdown.TabIndex = 5;
            // 
            // nyelvLabel
            // 
            this.nyelvLabel.AutoSize = true;
            this.nyelvLabel.Location = new System.Drawing.Point(364, 145);
            this.nyelvLabel.Name = "nyelvLabel";
            this.nyelvLabel.Size = new System.Drawing.Size(0, 13);
            this.nyelvLabel.TabIndex = 6;
            // 
            // helpRedBox
            // 
            this.sameNumberAlreadyInHouseHintBox.AutoSize = true;
            this.sameNumberAlreadyInHouseHintBox.Location = new System.Drawing.Point(12, 77);
            this.sameNumberAlreadyInHouseHintBox.Name = "helpRedBox";
            this.sameNumberAlreadyInHouseHintBox.Size = new System.Drawing.Size(15, 14);
            this.sameNumberAlreadyInHouseHintBox.TabIndex = 7;
            this.sameNumberAlreadyInHouseHintBox.UseVisualStyleBackColor = true;
            // 
            // ellenorzesGroup
            // 
            this.ellenorzesGroup.Controls.Add(this.showExerciseCorrectnessRadio);
            this.ellenorzesGroup.Controls.Add(this.showNumberOfWrongCellsRadio);
            this.ellenorzesGroup.Controls.Add(this.showWrongCellsRadio);
            this.ellenorzesGroup.Location = new System.Drawing.Point(12, 145);
            this.ellenorzesGroup.Name = "ellenorzesGroup";
            this.ellenorzesGroup.Size = new System.Drawing.Size(273, 87);
            this.ellenorzesGroup.TabIndex = 9;
            this.ellenorzesGroup.TabStop = false;
            // 
            // joVagyRosszMutatRadio
            // 
            this.showExerciseCorrectnessRadio.AutoSize = true;
            this.showExerciseCorrectnessRadio.Location = new System.Drawing.Point(6, 65);
            this.showExerciseCorrectnessRadio.Name = "joVagyRosszMutatRadio";
            this.showExerciseCorrectnessRadio.Size = new System.Drawing.Size(14, 13);
            this.showExerciseCorrectnessRadio.TabIndex = 2;
            this.showExerciseCorrectnessRadio.TabStop = true;
            this.showExerciseCorrectnessRadio.UseVisualStyleBackColor = true;
            // 
            // hanyRosszMutatRadio
            // 
            this.showNumberOfWrongCellsRadio.AutoSize = true;
            this.showNumberOfWrongCellsRadio.Location = new System.Drawing.Point(6, 42);
            this.showNumberOfWrongCellsRadio.Name = "hanyRosszMutatRadio";
            this.showNumberOfWrongCellsRadio.Size = new System.Drawing.Size(14, 13);
            this.showNumberOfWrongCellsRadio.TabIndex = 1;
            this.showNumberOfWrongCellsRadio.TabStop = true;
            this.showNumberOfWrongCellsRadio.UseVisualStyleBackColor = true;
            // 
            // rosszakMutatRadio
            // 
            this.showWrongCellsRadio.AutoSize = true;
            this.showWrongCellsRadio.Location = new System.Drawing.Point(6, 19);
            this.showWrongCellsRadio.Name = "rosszakMutatRadio";
            this.showWrongCellsRadio.Size = new System.Drawing.Size(14, 13);
            this.showWrongCellsRadio.TabIndex = 0;
            this.showWrongCellsRadio.TabStop = true;
            this.showWrongCellsRadio.UseVisualStyleBackColor = true;
            // 
            // killerBox
            // 
            this.sumOfNumbersBiggerInCageHintBox.AutoSize = true;
            this.sumOfNumbersBiggerInCageHintBox.Location = new System.Drawing.Point(12, 111);
            this.sumOfNumbersBiggerInCageHintBox.Name = "killerBox";
            this.sumOfNumbersBiggerInCageHintBox.Size = new System.Drawing.Size(15, 14);
            this.sumOfNumbersBiggerInCageHintBox.TabIndex = 10;
            this.sumOfNumbersBiggerInCageHintBox.UseVisualStyleBackColor = true;
            // 
            // BeallitasokForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(533, 243);
            this.Controls.Add(this.sumOfNumbersBiggerInCageHintBox);
            this.Controls.Add(this.ellenorzesGroup);
            this.Controls.Add(this.sameNumberAlreadyInHouseHintBox);
            this.Controls.Add(this.nyelvLabel);
            this.Controls.Add(this.languageDropdown);
            this.Controls.Add(this.browseButton);
            this.Controls.Add(this.utvonalLabel);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.filePathBox);
            this.Name = "BeallitasokForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            this.ellenorzesGroup.ResumeLayout(false);
            this.ellenorzesGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox filePathBox;
        private System.Windows.Forms.Button browseButton;
        private System.Windows.Forms.Label utvonalLabel;
        private System.Windows.Forms.ComboBox languageDropdown;
        private System.Windows.Forms.Label nyelvLabel;
        private System.Windows.Forms.CheckBox sameNumberAlreadyInHouseHintBox;
        private System.Windows.Forms.GroupBox ellenorzesGroup;
        private System.Windows.Forms.RadioButton showExerciseCorrectnessRadio;
        private System.Windows.Forms.RadioButton showNumberOfWrongCellsRadio;
        private System.Windows.Forms.RadioButton showWrongCellsRadio;
        private System.Windows.Forms.CheckBox sumOfNumbersBiggerInCageHintBox;
    }
}