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

        // uresCellakSzama: a feladat újrakezdéséhez ebbe mentem az üres cellák számát a feladat kezdetekor
        private int uresCellakSzama;

        //Ezen osztály segítségével hozok létre illetve olvatatok be feladatot
        private ExerciseGeneratorInitializer generatorInitializer;

        private MenuHandler menuHandler;
        private UITableHandler tableHandler;
        private SudokuExercise se = SudokuExercise.get;
        private LocHandler loc = LocHandler.get;
        private ConfigHandler conf = ConfigHandler.get;

        //Tömb színek tárolására
        private Color[] colors;

        public SudokuApp()
        {
            InitializeComponent();
            //Ablak maximalizálási lehetősége letiltva
            this.MaximizeBox = false;
            //Az ablakot nem lehet átméretezni
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            GenerateColors();
        }

        private void SudokuApp_Load(object sender, EventArgs e)
        {
            BuildMenuBar();
            CreateTableOnGUI();

            tableHandler = new UITableHandler(verifyExerciseButton, colors, numberOfEmptyCellsLabel, exerciseTable);
            se.ExerciseType = SudokuType.SimpleSudoku;
            conf.ExerciseInProgress = false;

            SetLabels();
            SetFormControlDefaultValues();
            BindEventHandlers();
        }

        private void SetFormControlDefaultValues()
        {
            //A feladat újrakezdése, szüneteltetése, megállítása és ellenőrzése gombok inaktívak lesznek
            restartExerciseButton.Enabled = stopExerciseButton.Enabled = verifyExerciseButton.Enabled = false;

            //A nehézség beállítására szolgáló csúszka legkisebb értéke 1, legnagyobb értéke pedig 6 lesz
            difficultyBar.Minimum = 0;
            difficultyBar.Maximum = 6;

            sudButton.Tag = SudokuType.SimpleSudoku;
            xButton.Tag = SudokuType.SudokuX;
            centerButton.Tag = SudokuType.CenterDot;

            //A Killer feladat nehézségét állító csúszka legkisebb értéke 1, legnagyobb értéke pedig 7 lesz
            killerDifficultyBar.Minimum = 1;
            killerDifficultyBar.Maximum = 7;

            //Elrejtem a csúszkát és a hozzá tartozó címkét
            killerDifficultyBar.Visible = killerDifficultyLabel.Visible = false;
        }

        private void GenerateColors()
        {
            //szinek tömb feltöltése színekkel
            colors = new Color[] {new Color(), Color.LightCoral, Color.LightCyan, Color.LightGoldenrodYellow, Color.LightGreen, Color.LightPink, 
                                    Color.LightSalmon, Color.LightSeaGreen, Color.LightSkyBlue, Color.LightSteelBlue, Color.LightYellow, 
                                    Color.LightSlateGray, Color.LightGray, Color.Coral, Color.CornflowerBlue, Color.PaleVioletRed, Color.PeachPuff, 
                                    Color.MediumSeaGreen, Color.IndianRed};
        }

        private void BindEventHandlers()
        {
            difficultyBar.ValueChanged += delegate(object sender, EventArgs e) { difficultyLabel.Text = loc.GetLoc("difficulty") + ": " + difficultyBar.Value.ToString(); };
            killerDifficultyBar.ValueChanged += delegate(object sender, EventArgs e) { killerDifficultyLabel.Text = "Killer " + loc.GetLoc("difficulty").ToLower() + ": " + killerDifficultyBar.Value.ToString(); };
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

        /// <summary> Táblázat készítése. </summary>
        private void CreateTableOnGUI()
        {
            //Making the table not visible to speed up the drawing
            exerciseTable.Visible = false;

            //A táblázatnak ne legyen egy sora és oszlopa se
            exerciseTable.RowStyles.Clear();
            exerciseTable.ColumnStyles.Clear();

            //A tábla felosztásának kialakítása. A táblázat 9 sorból és 9 oszlopból fog állni
            exerciseTable.RowCount = exerciseTable.ColumnCount = 9;

            //Cellák kialakítása sorok és oszlopok hozzáadásával
            for (int i = 0; i < 9; i++)
            {
                //Új sor kialakítása, melynek magassága a tábla magasságának 11%-a
                exerciseTable.RowStyles.Add(new RowStyle(SizeType.Percent, 11f));
                //Új oszlop kialakítása, melynek szélessége a tábla szélességének 11%-a
                exerciseTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 11f));
            }

            exerciseTable.BackColor = SystemColors.Window;

            //Cellakeretek stílusának beállítása sima vonalra
            exerciseTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
        }

        /// <summary> Feladat beolvasása vagy generálása, illetve a táblázat feltöltését végzi el </summary>
        /// <param name="isExerciseGenerated"> Megadja, hogy generált feladatról van szó vagy sem </param>
        private void CreateExercise(bool isExerciseGenerated)
        {
            se.IsExerciseGenerated = isExerciseGenerated;

            //Ha nem generált feladatról van szó és a feladat választása ablakban Mégsét nyomok, ne történjen semmi
            if (!se.IsExerciseGenerated && !vanValasztottFeladat())
                return;

            //Inicializálom a keszit objektumot, ami a megadott fajtájú feladat generálására lesz képes
            generatorInitializer = new ExerciseGeneratorInitializer();

            /* generaltFeladat adja meg, hogy a feladatot beolvasni kell vagy generálni
             * nehezsegSav.Value adja meg a feladat nehézségét, amit majd a backtrack algoritmusban használok. 
             * killerSav.Value adja meg a Killer feladat nehézségét*/
            if (!generatorInitializer.GenerateExercise(difficultyBar.Value, killerDifficultyBar.Value))
            {
                //Ha nem generált feladatról van szó, akkor a beolvasás lehet sikertelen, ebben az esetben pedig semmi más ne történjen
                //csak lépjen ki ebből az eljárásból.
                MessageBox.Show(loc.GetLoc("failed_read_file"), loc.GetLoc("failed_read_file_caption"), MessageBoxButtons.OK, MessageBoxIcon.Error);
                
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
                MessageBox.Show(loc.GetLoc("exercise_not_solvable"), loc.GetLoc("exercise_not_solvable_caption"), 
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            
            //A felhasználói felületen kitöltöm a táblázatot a feladattal
            tableHandler.FillTableOnGUI();

            //Inicializálom az exerciseBackup tombot, melyben ezután lementem a feladatot, a feladat esetleges újrakezdéséhez
            CommonUtil.InitializeArray(out exerciseBackup);

            //Elmentem a feladatot a hozzá tartozó tömbökkel és az üres cellák számával együtt
            CommonUtil.CopyJaggedThreeDimensionArray(exerciseBackup, se.Exercise);
            uresCellakSzama = se.NumberOfEmptyCells;

            //Beállítom a gombok állapotát
            gombokBeallit(true);

            //A beolvasás gomb, valamint a Megnyitás és Generálás menü elemei inaktívak lesznek
            beolvasButton.Enabled = false;
            menuHandler.DisableOpenAndGenerateMenuItems();

            //Üres cellák számának címkéje látható
            numberOfEmptyCellsLabel.Visible = true;
            //Üres cellák számának megjelenítése
            numberOfEmptyCellsLabel.Text = loc.GetLoc("numof_empty_cells") + ": " + se.NumberOfEmptyCells;
        }

        //Feladat újrakezdéséhez
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
            numberOfEmptyCellsLabel.Text = loc.GetLoc("numof_empty_cells") + ": " + se.NumberOfEmptyCells;
        }

        /// <summary>Megnyit egy fájlválasztó dialógusablakot</summary>
        /// <returns>Ha OK-t nyomtam az ablakban, akkor true, egyébként false</returns>
        private bool vanValasztottFeladat()
        {
            //Fájlválasztó ablak létrehozása
            OpenFileDialog selectExerciseDialog = new OpenFileDialog();

            //A dialógus ablak megnyitáskor melyik könyvtár tartalmát mutassa
            selectExerciseDialog.InitialDirectory = conf.GetConfig("alapFajlUtvonal");

            //Az ablak címsorában megjelenő szöveg
            selectExerciseDialog.Title = loc.GetLoc("select_file");

            //A választható fájl-kiterjesztések
            selectExerciseDialog.Filter = loc.GetLoc("text_files") + "(*.txt)|*.txt";

            //Egy változóba beteszem a fájl útvonalát, majd a fájl beolvasásakor ezt a változót adom meg útvonalnak
            if (selectExerciseDialog.ShowDialog() == DialogResult.OK)
            {                
                //Az első "paraméter" adja meg a fájl útvonalát (a fájlnév nélkül), a második pedig a fájl nevét
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
            menuHandler.SetEventHandlers(
                new EventHandler(GenerateSimpleSudoku),
                new EventHandler(GenerateSudokuX),
                new EventHandler(GenerateCenterDot),
                new EventHandler(OpenSimpleSudoku),
                new EventHandler(OpenSudokuX),
                new EventHandler(OpenCenterDot));
            this.Controls.Add(menuHandler.BuildMainDialogMenu());
        }

        /// <summary> Elvégzi a változó feliratú címkék feliratozását. </summary>
        private void SetLabels()
        {
            //A nehezsegSav-hoz tartozó címke feliratozása
            difficultyLabel.Text = loc.GetLoc("difficulty") + ": " + difficultyBar.Value.ToString();

            //A killerSav-hoz tartozó címke feliratozása
            killerDifficultyLabel.Text = "Killer " + loc.GetLoc("difficulty").ToLower() + ": " + killerDifficultyBar.Value.ToString();

            exerciseTypeGroup.Text = loc.GetLoc("types") + ":";
            centerButton.Text = loc.GetLoc("centerdot");

            generalButton.Text = loc.GetLoc("generate");
            beolvasButton.Text = loc.GetLoc("open");
            restartExerciseButton.Text = loc.GetLoc("restart");

            verifyExerciseButton.Text = loc.GetLoc("check");

            stopExerciseButton.Text = loc.GetLoc("stop");

            difficultyLabel.Text = loc.GetLoc("difficulty") + ": " + difficultyBar.Value.ToString();

            menuHandler.SetLabels();

            this.Text = "SudokU";
            sudButton.Text = "Sudoku";
            xButton.Text = "Sudoku-X";
            killerBox.Text = "Killer Sudoku";
        }

        //A 3 fajta ellenőrzés végrehajtására
        private void ellenorizButton_Click(object sender, EventArgs e)
        {
            ExerciseResultVerifier.InitVerifier(verifyExerciseLabel, tableHandler.GUITable);
            ExerciseResultVerifier.VerifyResult();
        }

        //Feladat megállításakor fut le
        private void feladatStopButton_Click(object sender, EventArgs e)
        {
            //Beállítom a gombok állapotát
            gombokBeallit(false);

            //Az ellenőrzés gomb inaktív lesz
            verifyExerciseButton.Enabled = false;

            //Az ellenőrzéshez tartozó címkének nem lesz szövege
            verifyExerciseLabel.Text = "";

            //A beolvasás gomb inaktív lesz
            beolvasButton.Enabled = true;

            //Törlöm a felhasználói felületen levő tábla vezérlőit (azaz a TextBox-okat)
            exerciseTable.Controls.Clear();

            //Üres cellák számának címkéje nem látható
            numberOfEmptyCellsLabel.Visible = false;

            //Megnyitás és Generálás menü elemei aktívak lesznek
            menuHandler.EnableOpenAndGenerateMenuItems();
        }

        /// <summary>Beállítja a kezelőfelületen levő gombok állapotát</summary>
        /// <param name="status">A táblázat láthatósága alapján kap értéket</param>
        private void gombokBeallit(bool status)
        {
            //Feladat létének, feladat újrakezdése, szüneteltetése és megállítása gombok, valamint a táblázat láthatóságának állítása
            conf.ExerciseInProgress = restartExerciseButton.Enabled = stopExerciseButton.Enabled = exerciseTable.Visible = status;

            //A generálás, valamint a nehezsegSav csúszka, fajtacsoport és Killer checkbox engedélyezésének/tiltásának beállítása
            generalButton.Enabled = exerciseTypeGroup.Enabled = killerBox.Enabled = killerDifficultyBar.Enabled = !status;

            //Nehézségsáv engedélyezése vagy tiltása
            difficultyBar.Enabled = (killerBox.Checked || (status == true)) ? false : true;
        }
    }
}
