using System;
using System.Windows.Forms;
using System.IO;
using Sudoku.Configuration;
using Sudoku.Generate;
using Sudoku.Controller;
using Sudoku.Language;
using Sudoku.Log;
using Sudoku.Verifier;
using static Sudoku.Configuration.ConfigurationKeys;
using static Sudoku.Verifier.ExerciseResultVerifier;

namespace Sudoku.Dialog
{
    public partial class SudokuApp : Form
    {
        private int[][,] exerciseBackupForRestart;
        private int numberOfEmptyCellsForRestart;

        private ExerciseResultVerifier resultVerifier;
        private MenuHandler menuHandler;
        private UITableHandler tableHandler;
        private SudokuExercise se = SudokuExercise.Instance;
        private LocHandler loc = LocHandler.get;
        private ConfigHandler conf = ConfigHandler.get;
        private Logger log = Logger.Instance;

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

            SetButtonDefaultStates();
            SetDifficultyBarDefaultStatesAndValues();
            SetSudokuTypeButtonTags();
            BindEventHandlers();
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
            difficultyBar.ValueChanged += (sender, e) => { SetDifficultyLabelText(); };
            killerDifficultyBar.ValueChanged += (sender, e) => { SetKillerDifficultyLabelText(); };
            killerBox.CheckedChanged += (sender, e) =>
            {
                /* Killer vagy nem Killer feladat a Killer Sudoku-hoz tartozó checkbox értéke alapján,
                * illetve a killerhez tartozó csúszka látható lesz vagy eltűnik*/
                killerDifficultyBar.Visible = killerDifficultyLabel.Visible = se.IsExerciseKiller = killerBox.Checked;

                //Ha Killer Sudoku-t készítek, akkor a nehézség sáv inaktív lesz, mert nincs rá szükség
                difficultyBar.Enabled = !se.IsExerciseKiller;
            };
            sudButton.CheckedChanged += new EventHandler(SetExerciseType);
            xButton.CheckedChanged += new EventHandler(SetExerciseType);
            centerButton.CheckedChanged += new EventHandler(SetExerciseType);
            this.Activated += (sender, e) => { SetLabels(); };
        }

        private void SetExerciseType(object sender, EventArgs e)
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
        /// <param name="isExerciseGenerated"> Megadja, hogy generált vagy beolvasott feladatról van szó vagy sem </param>
        private void CreateExercise(bool isExerciseGenerated)
        {
            se.IsExerciseGenerated = isExerciseGenerated;

            //Ha nem generált feladatról van szó és a feladat választása ablakban Mégsét nyomok, ne történjen semmi
            if (!se.IsExerciseGenerated && !HasSelectedFileToOpen())
                return;

            if (!new ExerciseGeneratorInitializer().GenerateExercise(difficultyBar.Value, killerDifficultyBar.Value))
            {
                //Ha nem generált feladatról van szó, akkor a beolvasás lehet sikertelen, ebben az esetben pedig semmi más ne történjen
                //csak lépjen ki ebből az eljárásból.
                MessageBox.Show(loc.Get("failed_read_file"), loc.Get("failed_read_file_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                
                return;
            }

            //Ha a Killer Sudoku feladat fájlból lesz beolvasva, akkor csak a feladat jó voltát lehet ellenőrizni
            if (se.IsExerciseKiller && !se.IsExerciseGenerated)
            {
                conf.SetAttributeValue(SHOW_INCORRECT_CELLS_ENABLED, "false");
                conf.SetAttributeValue(SHOW_NUMBER_OF_INCORRECT_CELLS_ENABLED, "false");
                conf.SetAttributeValue(SHOW_WHETHER_SOLUTION_IS_CORRECT_ENABLED, "true");
            }

            //Ha fájlból olvastattam be feladatot, és a megoldása tartalmaz 0 értéke(ke)t, akkor a feladat nem megoldható a kiválasztott típus alapján
            if (!se.IsExerciseGenerated && (se.IsExerciseKiller ? false : !IsExerciseCorrect(se.Solution)))
            {
                MessageBox.Show(loc.Get("exercise_not_solvable"), loc.Get("exercise_not_solvable_caption"), 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            tableHandler.FillTableOnGUI();

            exerciseBackupForRestart = Arrays.CreateInitializedArray();

            //Elmentem a feladatot a hozzá tartozó tömbökkel és az üres cellák számával együtt
            Arrays.CopyJaggedThreeDimensionArray(exerciseBackupForRestart, se.Exercise);
            numberOfEmptyCellsForRestart = se.NumberOfEmptyCells;

            SetButtonStates(true);

            readExerciseButton.Enabled = false;
            menuHandler.DisableOpenAndGenerateMenuItems();

            numberOfEmptyCellsLabel.Visible = true;
            SetNumberOfEmptyCellsLabelText();
        }

        private void RestartExerciseButton_Click(object sender, EventArgs e)
        {
            //Visszaállítom a feladat kezdeti értékeit, illetve az üres cellák számát
            Arrays.CopyJaggedThreeDimensionArray(se.Exercise, exerciseBackupForRestart);
            se.NumberOfEmptyCells = numberOfEmptyCellsForRestart;

            tableHandler.ReloadTableForRestart(exerciseBackupForRestart);

            //Az ellenőrzés gomb megint inaktív lesz, mivel újrakezdjük a feladatot és lesz üres cella
            SetVerifyExerciseLabelAndButtonToDefault();

            SetNumberOfEmptyCellsLabelText();
        }

        private void SetNumberOfEmptyCellsLabelText()
        {
            numberOfEmptyCellsLabel.Text = loc.Get("numof_empty_cells") + ": " + se.NumberOfEmptyCells;
        }

        private bool HasSelectedFileToOpen()
        {
            OpenFileDialog selectExerciseDialog = new SelectExerciseDialogFactory().CreateDialog();
            if (selectExerciseDialog.ShowDialog() == DialogResult.OK)
            {                
                se.ExerciseFilePath = Path.GetDirectoryName(selectExerciseDialog.FileName) + 
                    "\\" + Path.GetFileName(selectExerciseDialog.FileName);
                
                return true;
            }

            return false;     
        }

        private void ReadForType(SudokuType exerciseType, object sender, EventArgs e)
        {
            se.ExerciseType = exerciseType;
            BeolvasButton_Click(sender, e);
        }

        private void GenerateForType(SudokuType exerciseType, object sender, EventArgs e)
        {
            se.ExerciseType = exerciseType;
            GeneralButton_Click(sender, e);
        }

        private void BuildMenuBar()
        {
            menuHandler = new MenuHandler();
            menuHandler.AddEventHandler(SudokuCreationType.GEN_SUD, (sender, e) => GenerateForType(SudokuType.SimpleSudoku, sender, e));
            menuHandler.AddEventHandler(SudokuCreationType.GEN_SUDX, (sender, e) => GenerateForType(SudokuType.SudokuX, sender, e));
            menuHandler.AddEventHandler(SudokuCreationType.GEN_CENT, (sender, e) => GenerateForType(SudokuType.CenterDot, sender, e));
            menuHandler.AddEventHandler(SudokuCreationType.OPEN_SUD, (sender, e) => ReadForType(SudokuType.SimpleSudoku, sender, e));
            menuHandler.AddEventHandler(SudokuCreationType.OPEN_SUDX, (sender, e) => ReadForType(SudokuType.SudokuX, sender, e));
            menuHandler.AddEventHandler(SudokuCreationType.OPEN_CENT, (sender, e) => ReadForType(SudokuType.CenterDot, sender, e));
            this.Controls.Add(menuHandler.BuildMainDialogMenu());
        }

        private void SetLabels()
        {
            SetDifficultyLabelText();
            SetKillerDifficultyLabelText();

            exerciseTypeGroup.Text = loc.Get("types") + ":";
            centerButton.Text = loc.Get("centerdot");

            generalButton.Text = loc.Get("generate");
            readExerciseButton.Text = loc.Get("open");
            restartExerciseButton.Text = loc.Get("restart");

            verifyExerciseButton.Text = loc.Get("check");

            stopExerciseButton.Text = loc.Get("stop");

            menuHandler.SetLabels();

            this.Text = "SudokU";
            sudButton.Text = "Sudoku";
            xButton.Text = "Sudoku-X";
            killerBox.Text = "Killer Sudoku";
        }

        private void SetDifficultyLabelText()
        {
            difficultyLabel.Text = loc.Get("difficulty") + ": " + difficultyBar.Value.ToString();
        }

        private void SetKillerDifficultyLabelText()
        {
            killerDifficultyLabel.Text = "Killer " + loc.Get("difficulty").ToLower() + ": " + killerDifficultyBar.Value.ToString();
        }

        private void VerifyButton_Click(object sender, EventArgs e)
        {
            resultVerifier = new ExerciseResultVerifier(verifyExerciseLabel, tableHandler.GUITable);
            resultVerifier.VerifyResult();
        }

        private void ExerciseStopButton_Click(object sender, EventArgs e)
        {
            SetButtonStates(false);
            SetVerifyExerciseLabelAndButtonToDefault();
            readExerciseButton.Enabled = true;
            exerciseTable.Controls.Clear();
            numberOfEmptyCellsLabel.Visible = false;
            menuHandler.EnableOpenAndGenerateMenuItems();
        }

        private void SetVerifyExerciseLabelAndButtonToDefault()
        {
            verifyExerciseButton.Enabled = false;
            verifyExerciseLabel.Text = "";
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

        private void SudokuApp_FormClosing(object sender, FormClosingEventArgs e)
        {
            log.Close("Closing the application.");
        }
    }
}
