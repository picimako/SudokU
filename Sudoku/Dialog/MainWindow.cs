using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using Sudoku.Generate;
using Sudoku.Controller;
using Sudoku.Verifier;
using Sudoku.Language;

namespace Sudoku.Dialogusok
{
    public partial class SudokuApp : Form
    {
        //Ide mentem le a feladathoz tartozó táblákat a feladat esetleges újrakezdése miatt
        private int[][,] exerciseBackup;

        //a feladat újrakezdéséhez ebbe mentem az üres cellák számát a feladat kezdetekor
        private int uresCellakSzama;
        private ExerciseGeneratorInitializer generatorInitializer;

        private MenuHandler menuHandler;
        private UITableHandler tableHandler;
        private SudokuExercise se = SudokuExercise.get;
        private LocHandler loc = LocHandler.get;
        private ConfigHandler conf = ConfigHandler.get;

        public SudokuApp()
        {
            InitializeComponent();
            //Window maximalization is disabled
            this.MaximizeBox = false;
            //Window resizing is disabled
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
        }

        private void SudokuApp_Load(object sender, EventArgs e)
        {
            BuildMenuBar();
            tableHandler = new UITableHandler(verifyExerciseButton, numberOfEmptyCellsLabel, exerciseTable);
            se.ExerciseType = SudokuType.SimpleSudoku;
            conf.ExerciseInProgress = false;

            SetLabels();
            SetFormControlDefaultValues();
            BindEventHandlers();
        }

        private void SetFormControlDefaultValues()
        {
            SetButtonDefaultStates();
            SetDifficultyBarDefaultStatesAndValues();
            SetSudokuTypeButtonTags();
        }

        private void SetButtonDefaultStates()
        {
            restartExerciseButton.Enabled = stopExerciseButton.Enabled = verifyExerciseButton.Enabled = false;
        }

        private void SetDifficultyBarDefaultStatesAndValues()
        {
            difficultyBar.Minimum = 0;
            difficultyBar.Maximum = 6;
            killerDifficultyBar.Minimum = 1;
            killerDifficultyBar.Maximum = 7;
            killerDifficultyBar.Visible = killerDifficultyLabel.Visible = false;
        }

        private void SetSudokuTypeButtonTags()
        {
            sudButton.Tag = SudokuType.SimpleSudoku;
            xButton.Tag = SudokuType.SudokuX;
            centerButton.Tag = SudokuType.CenterDot;
        }

        private void BindEventHandlers()
        {
            difficultyBar.ValueChanged += delegate(object sender, EventArgs e)
            {
                difficultyLabel.Text = loc.Get("difficulty") + ": " + difficultyBar.Value.ToString();
            };
            killerDifficultyBar.ValueChanged += delegate(object sender, EventArgs e)
            {
                killerDifficultyLabel.Text = "Killer " + loc.Get("difficulty").ToLower() + ": " + killerDifficultyBar.Value.ToString();
            };
            killerBox.CheckedChanged += delegate(object sender, EventArgs e)
            {
                /* Killer vagy nem Killer feladat a Killer Sudoku-hoz tartozó checkbox értéke alapján,
                * illetve a killerhez tartozó csúszka látható lesz vagy eltűnik*/
                killerDifficultyBar.Visible = killerDifficultyLabel.Visible = se.IsExerciseKiller = killerBox.Checked;

                //Ha Killer Sudoku-t készítek, akkor a nehézség sáv inaktív lesz, mert nincs rá szükség
                difficultyBar.Enabled = !se.IsExerciseKiller;
            };
            sudButton.CheckedChanged += new EventHandler(checkBoxCheckedChanged);
            xButton.CheckedChanged += new EventHandler(checkBoxCheckedChanged);
            centerButton.CheckedChanged += new EventHandler(checkBoxCheckedChanged);
        }

        private void checkBoxCheckedChanged(object sender, EventArgs e)
        {
            se.ExerciseType = (SudokuType)((RadioButton)sender).Tag;
        }

        private void BeolvasButton_Click(object sender, EventArgs e)
        {
            CreateExercise(false);
        }

        private void GeneralButton_Click(object sender, EventArgs e)
        {
            CreateExercise(true);
        }

        /// <summary> Feladat beolvasása vagy generálása, illetve a táblázat feltöltését végzi el </summary>
        /// <param name="isExerciseGenerated"> Megadja, hogy generált feladatról van szó vagy sem </param>
        private void CreateExercise(bool isExerciseGenerated)
        {
            se.IsExerciseGenerated = isExerciseGenerated;

            //Ha nem generált feladatról van szó és a feladat választása ablakban Mégsét nyomok, ne történjen semmi
            if (!se.IsExerciseGenerated && !HasSelectedFileToOpen())
                return;

            generatorInitializer = new ExerciseGeneratorInitializer();

            if (!generatorInitializer.GenerateExercise(difficultyBar.Value, killerDifficultyBar.Value))
            {
                //Ha nem generált feladatról van szó, akkor a beolvasás lehet sikertelen, ebben az esetben pedig semmi más ne történjen
                //csak lépjen ki ebből az eljárásból.
                MessageBox.Show(loc.Get("failed_read_file"), loc.Get("failed_read_file_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                return;
            }

            //Ha a Killer Sudoku feladat fájlból lesz beolvasva, akkor csak a feladat jó voltát lehet ellenőrizni
            if (se.IsExerciseKiller && !se.IsExerciseGenerated)
            {
                conf.SetAttributeValue("rosszakMutat", "false");
                conf.SetAttributeValue("hanyRosszMutat", "false");
                conf.SetAttributeValue("joVagyRosszMutat", "true");
            }

            //Ha fájlból olvastattam be feladatot, és a megoldása tartalmaz 0 értéke(ke)t, akkor a feladat nem megoldható a kiválasztott típus alapján
            if (!se.IsExerciseGenerated && (se.IsExerciseKiller ? false : !CommonUtil.IsExerciseCorrect(se.Solution)))
            {
                MessageBox.Show(loc.Get("exercise_not_solvable"), loc.Get("exercise_not_solvable_caption"), 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            tableHandler.FillTableOnGUI();

            //Inicializálom az exerciseBackup tombot, melyben ezután lementem a feladatot, a feladat esetleges újrakezdéséhez
            CommonUtil.InitializeArray(out exerciseBackup);

            //Elmentem a feladatot a hozzá tartozó tömbökkel és az üres cellák számával együtt
            CommonUtil.CopyJaggedThreeDimensionArray(exerciseBackup, se.Exercise);
            uresCellakSzama = se.NumberOfEmptyCells;

            SetButtonStates(true);

            //A beolvasás gomb, valamint a Megnyitás és Generálás menü elemei inaktívak lesznek
            beolvasButton.Enabled = false;
            menuHandler.DisableOpenAndGenerateMenuItems();

            //Üres cellák számának címkéje látható
            numberOfEmptyCellsLabel.Visible = true;
            //Üres cellák számának megjelenítése
            numberOfEmptyCellsLabel.Text = loc.Get("numof_empty_cells") + ": " + se.NumberOfEmptyCells;
        }

        private void RestartExerciseButton_Click(object sender, EventArgs e)
        {
            //Visszaállítom a feladat kezdeti értékeit, illetve az üres cellák számát
            CommonUtil.CopyJaggedThreeDimensionArray(se.Exercise, exerciseBackup);
            se.NumberOfEmptyCells = uresCellakSzama;

            tableHandler.ReloadTableForRestart(exerciseBackup);

            //Az ellenőrzés gomb megint inaktív lesz, mivel újrakezdjük a feladatot és lesz üres cella
            verifyExerciseButton.Enabled = false;

            //Az ellenőrzéshez tartozó címkének nem lesz szövege
            verifyExerciseLabel.Text = "";

            //Üres cellák kijelzése
            numberOfEmptyCellsLabel.Text = loc.Get("numof_empty_cells") + ": " + se.NumberOfEmptyCells;
        }

        private bool HasSelectedFileToOpen()
        {
            OpenFileDialog selectExerciseDialog = new OpenFileDialog();
            selectExerciseDialog.InitialDirectory = conf.GetConfig("alapFajlUtvonal");
            selectExerciseDialog.Title = loc.Get("select_file");
            selectExerciseDialog.Filter = loc.Get("text_files") + "(*.txt)|*.txt";
            if (selectExerciseDialog.ShowDialog() == DialogResult.OK)
            {                
                se.ExerciseFilePath = Path.GetDirectoryName(selectExerciseDialog.FileName) + 
                    "\\" + Path.GetFileName(selectExerciseDialog.FileName);
                
                return true;
            }

            return false;     
        }

        /// <summary></summary>
        /// <param name="exerciseType"></param>
        /// <param name="generalButton"></param>
        private void ButtonClick(SudokuType exerciseType, bool generalButton, object sender, EventArgs e)
        {
            se.ExerciseType = exerciseType;

            if (generalButton)
                GeneralButton_Click(sender, e);
            else
                BeolvasButton_Click(sender, e);
        }

        private void GenerateSimpleSudoku(object sender, EventArgs e)
        {
            ButtonClick(SudokuType.SimpleSudoku, true, sender, e);
        }

        private void GenerateSudokuX(object sender, EventArgs e)
        {
            ButtonClick(SudokuType.SudokuX, true, sender, e);
        }

        private void GenerateCenterDot(object sender, EventArgs e)
        {
            ButtonClick(SudokuType.CenterDot, true, sender, e);
        }

        private void OpenSimpleSudoku(object sender, EventArgs e)
        {
            ButtonClick(SudokuType.SimpleSudoku, false, sender, e);
        }

        private void OpenSudokuX(object sender, EventArgs e)
        {
            ButtonClick(SudokuType.SudokuX, false, sender, e);
        }

        private void OpenCenterDot(object sender, EventArgs e)
        {
            ButtonClick(SudokuType.CenterDot, false, sender, e);
        }

        private void BuildMenuBar()
        {
            menuHandler = new MenuHandler();
            menuHandler.AddEventHandler(SudokuCreationType.GEN_SUD, GenerateSimpleSudoku);
            menuHandler.AddEventHandler(SudokuCreationType.GEN_SUDX, GenerateSudokuX);
            menuHandler.AddEventHandler(SudokuCreationType.GEN_CENT, GenerateCenterDot);
            menuHandler.AddEventHandler(SudokuCreationType.OPEN_SUD, OpenSimpleSudoku);
            menuHandler.AddEventHandler(SudokuCreationType.OPEN_SUDX, OpenSudokuX);
            menuHandler.AddEventHandler(SudokuCreationType.OPEN_CENT, OpenCenterDot);
            this.Controls.Add(menuHandler.BuildMainDialogMenu());
        }

        private void SetLabels()
        {
            difficultyLabel.Text = loc.Get("difficulty") + ": " + difficultyBar.Value.ToString();

            killerDifficultyLabel.Text = "Killer " + loc.Get("difficulty").ToLower() + ": " + killerDifficultyBar.Value.ToString();

            exerciseTypeGroup.Text = loc.Get("types") + ":";
            centerButton.Text = loc.Get("centerdot");

            generalButton.Text = loc.Get("generate");
            beolvasButton.Text = loc.Get("open");
            restartExerciseButton.Text = loc.Get("restart");

            verifyExerciseButton.Text = loc.Get("check");

            stopExerciseButton.Text = loc.Get("stop");

            difficultyLabel.Text = loc.Get("difficulty") + ": " + difficultyBar.Value.ToString();

            menuHandler.SetLabels();

            this.Text = "SudokU";
            sudButton.Text = "Sudoku";
            xButton.Text = "Sudoku-X";
            killerBox.Text = "Killer Sudoku";
        }

        //A 3 fajta ellenőrzés végrehajtására
        private void VerifyButton_Click(object sender, EventArgs e)
        {
            ExerciseResultVerifier.InitVerifier(verifyExerciseLabel, tableHandler.GUITable);
            ExerciseResultVerifier.VerifyResult();
        }

        private void ExerciseStopButton_Click(object sender, EventArgs e)
        {
            SetButtonStates(false);
            verifyExerciseButton.Enabled = false;
            verifyExerciseLabel.Text = "";
            beolvasButton.Enabled = true;
            exerciseTable.Controls.Clear();
            numberOfEmptyCellsLabel.Visible = false;
            menuHandler.EnableOpenAndGenerateMenuItems();
        }

        /// <summary>Beállítja a kezelőfelületen levő gombok állapotát</summary>
        /// <param name="state">A táblázat láthatósága alapján kap értéket</param>
        private void SetButtonStates(bool state)
        {
            //Feladat létének, feladat újrakezdése, szüneteltetése és megállítása gombok, valamint a táblázat láthatóságának állítása
            conf.ExerciseInProgress = restartExerciseButton.Enabled = stopExerciseButton.Enabled = exerciseTable.Visible = state;

            //A generálás, valamint a nehezsegSav csúszka, fajtacsoport és Killer checkbox engedélyezésének/tiltásának beállítása
            generalButton.Enabled = exerciseTypeGroup.Enabled = killerBox.Enabled = killerDifficultyBar.Enabled = !state;

            //Nehézségsáv engedélyezése vagy tiltása
            difficultyBar.Enabled = killerBox.Checked || state ? false : true;
        }
    }
}
