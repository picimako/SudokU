using System;
using System.IO;
using System.Windows.Forms;
using Sudoku.Language;

namespace Sudoku.Dialog
{
    public partial class SettingsForm : Form
    {
        #region Members

        private bool settingsChanged;
        private ConfigHandler conf = ConfigHandler.get;
        private LocHandler loc = LocHandler.get;

        #endregion

        #region Constructor

        public SettingsForm()
        {
            InitializeComponent();
            //Cannot resize the dialog
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            settingsChanged = false;
        }

        #endregion

        #region Methods

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            SetLanguages();
            SetFormControlDefaultValues();
            SetLabels();
            BindEventHandlers();
        }

        #region SetDefaultValues

        private void SetLanguages()
        {
            if (!Directory.Exists(loc.LocalizationFolder))
            {
                //TODO: Move this to localization file. Don't have it hardcoded.
                languageDropdown.DataSource = new string[1] { "No language files." };
                return;
            }
            else
            {
                CreateValidLanguageDropdown();
            }
        }

        private void CreateValidLanguageDropdown()
        {
            string[] availableLanguages = Directory.GetFiles(loc.LocalizationFolder, "*.xml");

            for (int i = 0; i < availableLanguages.Length; i++)
            {
                availableLanguages[i] = Path.GetFileNameWithoutExtension(availableLanguages[i]);
            }

            languageDropdown.DataSource = availableLanguages;
            languageDropdown.SelectedItem = conf.GetConfig("alapNyelv");
        }

        private void SetFormControlDefaultValues()
        {
            sameNumberAlreadyInHouseHintBox.Checked = Boolean.Parse(conf.GetConfig("helpRed"));
            sumOfNumbersBiggerInCageHintBox.Checked = Boolean.Parse(conf.GetConfig("cageSum"));

            showWrongCellsRadio.Checked = Boolean.Parse(conf.GetConfig("rosszakMutat"));
            showNumberOfWrongCellsRadio.Checked = Boolean.Parse(conf.GetConfig("hanyRosszMutat"));
            showExerciseCorrectnessRadio.Checked = Boolean.Parse(conf.GetConfig("joVagyRosszMutat"));

            //A segítség és óra CheckBox, valamint az ellenőrzés csoportja vanFeladat értéke szerint kapnak értéket
            sameNumberAlreadyInHouseHintBox.Enabled = sumOfNumbersBiggerInCageHintBox.Enabled = ellenorzesGroup.Enabled = !conf.ExerciseInProgress;
        }

        #endregion

        #region EventHandlers

        private void BindEventHandlers()
        {
            cancelButton.Click += (sender, e) => { this.Close(); };
            okButton.Click += new EventHandler(OkButton_Click);
            browseButton.Click += (sender, e) =>
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                    filePathBox.Text = dialog.SelectedPath;
            };

            filePathBox.TextChanged += new EventHandler(TakeActionForSettingsChanged);
            sameNumberAlreadyInHouseHintBox.CheckedChanged += new EventHandler(TakeActionForSettingsChanged);
            sumOfNumbersBiggerInCageHintBox.CheckedChanged += new EventHandler(TakeActionForSettingsChanged);
            showWrongCellsRadio.CheckedChanged += new EventHandler(TakeActionForSettingsChanged);
            showNumberOfWrongCellsRadio.CheckedChanged += new EventHandler(TakeActionForSettingsChanged);
            showExerciseCorrectnessRadio.CheckedChanged += new EventHandler(TakeActionForSettingsChanged);
        }

        private void TakeActionForSettingsChanged(object sender, EventArgs e)
        {
            if (!settingsChanged)
                settingsChanged = true;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            if (settingsChanged)
            {
                conf.SetAttributeValue("alapFajlUtvonal", filePathBox.Text);
                conf.SetAttributeValue("helpRed", sameNumberAlreadyInHouseHintBox.Checked.ToString());
                conf.SetAttributeValue("rosszakMutat", showWrongCellsRadio.Checked.ToString());
                conf.SetAttributeValue("hanyRosszMutat", showNumberOfWrongCellsRadio.Checked.ToString());
                conf.SetAttributeValue("joVagyRosszMutat", showExerciseCorrectnessRadio.Checked.ToString());
                conf.SetAttributeValue("cageSum", sumOfNumbersBiggerInCageHintBox.Checked.ToString());
            }

            if (SelectedLanguageChanged())
            {
                try
                {
                    conf.SetAttributeValue("alapNyelv", SelectedLanguage());
                    LocHandler.get.ReadLocalization();
                }
                catch (IOException)
                {
                    return;
                }
            }

            conf.SaveConfiguration();
            this.Close();
        }

        private bool SelectedLanguageChanged()
        {
            return !SelectedLanguage().Equals(conf.GetConfig("alapNyelv"));
        }

        private string SelectedLanguage()
        {
            return (string)languageDropdown.SelectedItem;
        }

        #endregion

        #region Labeling

        private void SetLabels()
        {
            this.Text = loc.Get("options_menu");

            filePathBox.Text = conf.GetConfig("alapFajlUtvonal");

            utvonalLabel.Text = loc.Get("open_default_folder") + ":";
            browseButton.Text = loc.Get("browse");
            nyelvLabel.Text = loc.Get("language") + ":";
            sameNumberAlreadyInHouseHintBox.Text = loc.Get("helpRed");
            ellenorzesGroup.Text = loc.Get("check_type") + ":";
            showWrongCellsRadio.Text = loc.Get("show_wrong");
            showNumberOfWrongCellsRadio.Text = loc.Get("show_wrong_number");
            showExerciseCorrectnessRadio.Text = loc.Get("good_wrong_solution");
            cancelButton.Text = loc.Get("cancel");
            sumOfNumbersBiggerInCageHintBox.Text = loc.Get("cageSum");
        }

        #endregion

        #endregion
    }
}
