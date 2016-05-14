using System;
using System.IO;
using System.Windows.Forms;
using Sudoku.Language;

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

        private void SetLanguages()
        {
            if (!Directory.Exists("Languages"))
            {
                //TODO: create localization for this message
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
            //A Languages mappából beolvasom az .xml kiterjesztésű fájlok neveit kiterjesztéssel együtt (a nyelvi fájlok .xml kiterjesztésűek)
            string[] availableLanguages = Directory.GetFiles("Languages", "*.xml");

            //Minden fájlnév végéről levágom a kiterjesztést
            for (int i = 0; i < availableLanguages.Length; i++)
            {
                availableLanguages[i] = Path.GetFileNameWithoutExtension(availableLanguages[i]);
            }

            //A lenyíló lista nyelvek elemeit fogja megjeleníteni
            languageDropdown.DataSource = availableLanguages;

            //Beállítom, hogy az aktuálisan beállított nyelv legyen kijelölve a listában
            languageDropdown.SelectedItem = conf.GetConfig("alapNyelv");
        }

        private void SetFormControlDefaultValues()
        {
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
            SetLanguages();
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
                try
                {
                    LocHandler.get.ReadLocalization();
                }
                catch (IOException)
                {
                    return;
                }
                conf.SetAttributeValue("alapNyelv", (string)languageDropdown.SelectedItem);
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
