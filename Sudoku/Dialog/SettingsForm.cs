using System;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using Sudoku.Language;
using System.Collections.Generic;

namespace Sudoku.Dialogusok
{
    public partial class SettingsForm : Form
    {
        #region Members

        private bool settingsChanged;
        private ConfigHandler conf = ConfigHandler.get;

        #endregion

        #region Constructor

        public SettingsForm()
        {
            InitializeComponent();
            //Az ablakot nem lehet átméretezni
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            //Nem változott egy beállítás sem
            settingsChanged = false;
        }

        #endregion

        #region Methods

        #region SetDefaultValues

        private void SetFormControlDefaultValues()
        {
            //A Languages mappából beolvasom az .xml kiterjesztésű fájlok neveit kiterjesztéssel együtt (a nyelvi fájlok .xml kiterjesztésűek)
            string[] availableLanguages = Directory.GetFiles("Languages", "*.xml");

            //Minden fájlnév végéről levágom a kiterjesztést
            for (int i = 0; i < availableLanguages.Length; i++)
                availableLanguages[i] = Path.GetFileNameWithoutExtension(availableLanguages[i]);

            //A lenyíló lista nyelvek elemeit fogja megjeleníteni
            languageDropdown.DataSource = availableLanguages;

            //Beállítom, hogy az aktuálisan beállított nyelv legyen kijelölve a listában
            languageDropdown.SelectedItem = conf.GetConfig("alapNyelv");

            //CheckBox-ok beállítása
            sameNumberAlreadyInHouseHintBox.Checked = Boolean.Parse(conf.GetConfig("helpRed"));
            sumOfNumbersBiggerInCageHintBox.Checked = Boolean.Parse(conf.GetConfig("cageSum"));

            //Rádiógombok beállítása
            showWrongCellsRadio.Checked = Boolean.Parse(conf.GetConfig("rosszakMutat"));
            showNumberOfWrongCellsRadio.Checked = Boolean.Parse(conf.GetConfig("hanyRosszMutat"));
            showExerciseCorrectnessRadio.Checked = Boolean.Parse(conf.GetConfig("joVagyRosszMutat"));

            //A segítség és óra CheckBox, valamint az ellenőrzés csoportja vanFeladat értéke szerint kapnak értéket
            sameNumberAlreadyInHouseHintBox.Enabled = sumOfNumbersBiggerInCageHintBox.Enabled = ellenorzesGroup.Enabled = !conf.ExerciseInProgress;
        }

        #endregion

        #region EventHandlers

        //Az ablak betöltődésekor fog lefutni
        private void SettingsForm_Load(object sender, EventArgs e)
        {
            SetFormControlDefaultValues();
            SetLabels();
            BindEventHandlers();
        }

        private void BindEventHandlers()
        {
            cancelButton.Click += delegate(object sender, EventArgs e) { this.Close(); };
            okButton.Click += new EventHandler(OkButton_Click);
            browseButton.Click += delegate(object sender, EventArgs e)
            {
                //Létrehozok egy mappaválasztó dialógusablakot
                FolderBrowserDialog fbd = new FolderBrowserDialog();

                //Ha kiválasztottam a mappát és ok-t nyomtam,
                if (fbd.ShowDialog() == DialogResult.OK)
                    //akkor a kijelölt mappának az útvonala lesz az útvonalat tároló TextBox értéke
                    filePathBox.Text = fbd.SelectedPath;
            };

            filePathBox.TextChanged += new EventHandler(TakeActionForSettingsChanged);
            sameNumberAlreadyInHouseHintBox.CheckedChanged += new EventHandler(TakeActionForSettingsChanged);
            sumOfNumbersBiggerInCageHintBox.CheckedChanged += new EventHandler(TakeActionForSettingsChanged);
            showWrongCellsRadio.CheckedChanged += new EventHandler(TakeActionForSettingsChanged);
            showNumberOfWrongCellsRadio.CheckedChanged += new EventHandler(TakeActionForSettingsChanged);
            showExerciseCorrectnessRadio.CheckedChanged += new EventHandler(TakeActionForSettingsChanged);
        }

        /// <summary>Ha még nem változott egyik beállítás sem, akkor megváltoztatja</summary>
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
                conf.SetAttributeValue("alapNyelv", (string)languageDropdown.SelectedItem);
                LocHandler.get.ReadLocalization();
            }

            conf.SaveConfiguration();
            this.Close();
        }

        private bool SelectedLanguageChanged()
        {
            return !((string)languageDropdown.SelectedItem).Equals(conf.GetConfig("alapNyelv"));
        }

        #endregion

        #region Labeling

        private void SetLabels()
        {
            LocHandler loc = LocHandler.get;
            this.Text = loc.GetLoc("options_menu");

            filePathBox.Text = conf.GetConfig("alapFajlUtvonal");

            utvonalLabel.Text = loc.GetLoc("open_default_folder") + ":";
            browseButton.Text = loc.GetLoc("browse");
            nyelvLabel.Text = loc.GetLoc("language") + ":";
            sameNumberAlreadyInHouseHintBox.Text = loc.GetLoc("helpRed");
            ellenorzesGroup.Text = loc.GetLoc("check_type") + ":";
            showWrongCellsRadio.Text = loc.GetLoc("show_wrong");
            showNumberOfWrongCellsRadio.Text = loc.GetLoc("show_wrong_number");
            showExerciseCorrectnessRadio.Text = loc.GetLoc("good_wrong_solution");
            cancelButton.Text = loc.GetLoc("cancel");
            sumOfNumbersBiggerInCageHintBox.Text = loc.GetLoc("cageSum");
        }

        #endregion

        #endregion
    }
}
