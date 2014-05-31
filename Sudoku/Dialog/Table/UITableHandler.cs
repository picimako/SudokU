using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Sudoku.Controller;
using Sudoku.Language;
using Sudoku.Generate;

namespace Sudoku.Dialogusok
{
    class UITableHandler
    {
        #region Members

        //A feladat értékeit az ezen tömbben levő TextBox-ok fogják tárolni
        private TextBox[,] guiTable;
        private SudokuExercise se = SudokuExercise.get;
        private LocHandler loc = LocHandler.get;
        private ConfigHandler conf = ConfigHandler.get;

        private Button verifyExerciseButton;
        private Color[] colors;
        private Label numberOfEmptyCellsLabel;
        private TableLayoutPanel exerciseTable;
        /*previousCellValue: ahhoz, hogy a feladathoz szükséges tömböket változtatni tudjam egy szám kitörlésekor, tudnom kell,
         * hogy mi volt a cella előző értéke. Ezt az értéket fogom ebben a változóban tárolni.*/
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

        public UITableHandler(Button verifyExerciseButton, Color[] colors, Label numberOfEmptyCellsLabel,
            TableLayoutPanel exerciseTable)
        {
            this.verifyExerciseButton = verifyExerciseButton;
            this.colors = colors;
            this.numberOfEmptyCellsLabel = numberOfEmptyCellsLabel;
            this.exerciseTable = exerciseTable;
        }

        #endregion

        #region Public

        //Feladat újrakezdéséhez
        public void ReloadTableForRestart(int[][,] exerciseBackup)
        {
            //Végigmegyek az összes cellán
            for (int p = 0; p < 81; p++)
            {
                //Az aktuális celláról leveszem az eseménykezelőt, hogy ha változtatom az értékét, akkor ne fusson le semmi egyéb változtatás
                guiTable[p / 9, p % 9].TextChanged -= new System.EventHandler(this.TextBox_TextChanged);

                //Átírom az aktuális cella értékét a feladat kezdetekori értékre
                guiTable[p / 9, p % 9].Text = (exerciseBackup[0][p / 9, p % 9] > 0) ? exerciseBackup[0][p / 9, p % 9].ToString() : "";

                //Ismét hozzáadom az eseménykezelőt
                guiTable[p / 9, p % 9].TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            }
        }

        /// <summary> A felhasználói felületen lévő táblázatot tölti fel </summary>
        public void FillTableOnGUI()
        {
            Dictionary<Pair, int> cageCornersAndSums = new Dictionary<Pair, int>();
            if (se.IsExerciseKiller)
                cageCornersAndSums = se.Killer.Ctrl.GetSumOfNumbersAndIndicatorCages();
            guiTable = new TextBox[9, 9];

            //Végigmegyek az összes cellán, és mindegyikben elhelyezek egy megfelelő tulajdonságokkal rendelkező TextBox-ot
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (se.IsExerciseKiller)
                    {
                        //Végigmegyek a lekért értékeken
                        foreach (KeyValuePair<Pair, int> cage in cageCornersAndSums)
                        {
                            if (se.Killer.Ctrl.IsCellAtTopLeftOfCage(cage, i, j))
                            {
                                //A táblázat aktuális cellája a Killer Sudoku megjelenítéséhez alkalmazkodó TextBox lesz a kiírandó szöveggel
                                guiTable[i, j] = IsCellSpecial(i, j)
                                    ? new KillerTextBox(cage.Value.ToString(), true)
                                    : new KillerTextBox(cage.Value.ToString(), false);

                                //Törlöm a felhasznált értéket
                                cageCornersAndSums.Remove(cage.Key);

                                //Nem vizsgálom tovább a szótárat
                                break;
                            }
                        }
                    }

                    //Ha az aktuális cellába még nem lett elhelyezve TextBox,
                    if (guiTable[i, j] == null)
                    {
                        /* A tabla TextBox tömb [i, j] indexű eleme egy új KillerTextBox lesz, ha a cella valamelyik blokk középső cellája,
                         * egyébként pedig csak egy sima TextBox.*/
                        guiTable[i, j] = se.IsExerciseKiller && IsCellSpecial(i, j)
                            ? new KillerTextBox("", true) : new TextBox();

                        //A szöveg igazítása a TextBox-on belül vízszintesen középre
                        guiTable[i, j].TextAlign = HorizontalAlignment.Center;
                    }

                    //Betűtípus megadása mérettel együtt
                    guiTable[i, j].Font = new Font(guiTable[i, j].Font.FontFamily, 19.5f);

                    //A TextBox-nak ne legyen szegélye
                    guiTable[i, j].BorderStyle = BorderStyle.None;

                    //Ne legyen margó
                    guiTable[i, j].Margin = new Padding(0);

                    //A magassága legyen a tábla magassága osztva a cellák számával
                    guiTable[i, j].Top = exerciseTable.Height / exerciseTable.RowCount;

                    //Akkor van függőlegesen középen, ha csak balra és jobbra van rögzítve
                    guiTable[i, j].Anchor = AnchorStyles.Left | AnchorStyles.Right;

                    //Maximum 1 karakter írható a TextBox-ba
                    guiTable[i, j].MaxLength = 1;

                    //Beállítok a Tag tulajdonságnak (ami object típusú) egy új Indexek példányt, amiben a TextBox-nak az i és j indexét tárolom
                    guiTable[i, j].Tag = new Pair(i, j);

                    if (!se.IsExerciseKiller)
                    {
                        if (se.Exercise[0][i, j] > 0)
                        {
                            guiTable[i, j].Text = se.Exercise[0][i, j].ToString();
                            //Nem lehet szerkeszteni (felesleges lenne, mert az előre megadott számokat nem érdemes kitörölni)
                            guiTable[i, j].Enabled = false;
                        }
                        SetCellBackgroundColor(guiTable[i, j]);
                    }
                    else
                        SetKillerCellBackgroundColor(guiTable[i, j]);

                    //Eseménykezelő, amely a textBox_TextChanged eljárást hívja meg, ha valamelyik cella tartalma megváltozott
                    guiTable[i, j].TextChanged += new System.EventHandler(TextBox_TextChanged);

                    //Eseménykezelő arra az esetre, ha valamelyik cella fókuszt kap
                    guiTable[i, j].GotFocus += delegate(object sender, EventArgs e)
                    {
                        //Létrehozok 2 változót a fókuszt kapott TextBox indexeinek tárolására
                        int _i = GetSenderTag(sender).i, _j = GetSenderTag(sender).j;

                        //A cellaElozoErtek változó a fókuszt kapott TextBox értékét kapja értékül (int-re átalakítva)
                        Int32.TryParse(guiTable[_i, _j].Text, out previousCellValue);
                    };

                    guiTable[i, j].KeyDown += new System.Windows.Forms.KeyEventHandler(TextBox_KeyDown);

                    //Hozzáadom a TextBox-ot a táblához (table)
                    exerciseTable.Controls.Add(guiTable[i, j], j, i);
                }
            }
        }

        #region Private

        //Akkor fut le, ha egy billentyűt (bármelyiket) lenyomom a billentyűzeten
        private void TextBox_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            /* Megkapjuk az i és j indexét a megváltozott tartalmú TextBox-nak.
             * Mivel sender object típusú ezért cast-olni kell TextBox-szá: (TextBox)sender
             * Ezután pedig a TextBox Tag tulajdonságát (ami szintén object típusú), át kell konvertálni Indexek típusúra, és csak ekkor tudom
             * lekérdezni az i és j indexeket. */
            int i = GetSenderTag(sender).i, j = GetSenderTag(sender).j;

            //Megvizsgálom, hogy milyen billentyű lett lenyomva
            switch (e.KeyCode)
            {
                //Ha a balra nyíl,
                case Keys.Left:
                    //megkeressük a legközelebbi olyan TextBox-ot balra, ami szerkeszthető, és arra helyezzük a fókuszt
                    while (j > 0)
                    {
                        if (guiTable[i, --j].Enabled)
                            break;
                    }
                    break;

                //Ha a jobbra nyíl,
                case Keys.Right:
                    //megkeressük a legközelebbi olyan TextBox-ot jobbra, ami szerkeszthető, és arra helyezzük a fókuszt
                    while (j < 8)
                    {
                        if (guiTable[i, ++j].Enabled)
                            break;
                    }
                    break;

                //Ha a fel nyíl,
                case Keys.Up:
                    //megkeressük a legközelebbi olyan TextBox-ot felfele, ami szerkeszthető, és arra helyezzük a fókuszt
                    while (i > 0)
                    {
                        if (guiTable[--i, j].Enabled)
                            break;
                    }
                    break;

                //Ha a le nyíl,
                case Keys.Down:
                    //megkeressük a legközelebbi olyan TextBox-ot lefele, ami szerkeszthető, és arra helyezzük a fókuszt
                    while (i < 8)
                    {
                        if (guiTable[++i, j].Enabled)
                            break;
                    }
                    break;
            }

            //Ráállítom a fókuszt arra a cellára, ahova léptem
            guiTable[i, j].Focus();
        }

        //Egy TextBox tartalmának megváltozásakor végrehajtódó eljárás
        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            //Létrehozok 2 változót a megváltozott tartalmú TextBox indexeinek tárolására
            int _i = GetSenderTag(sender).i, _j = GetSenderTag(sender).j;

            //Ha a cella üres, tehát kitöröltünk egy számot
            if ("".Equals(guiTable[_i, _j].Text))
            {
                //cellaElozoErtek értéke jelenleg a cella tartalmának kitörlés előtti értéke
                se.Exercise[0][_i, _j] = 0;

                //A számkivételnek megfelelően változtatom a táblák értékeit
                se.Ctrl.RegenerateNumberTablesForRemovedValue(previousCellValue, _i, _j);

                //Növelem az üres cellák számát, mivel kitöröltem egy számot
                se.NumberOfEmptyCells++;

                //Akármilyen állapotú is volt az Ellenőrzés gomb, most inaktív lesz
                verifyExerciseButton.Enabled = false;

                //A cella eredeti háttérszíne lesz a háttérszín
                if (se.IsExerciseKiller)
                    SetKillerCellBackgroundColor(guiTable[_i, _j]);
                else
                    SetCellBackgroundColor(guiTable[_i, _j]);

                //Ha ellenőrizni kell a ketrecben levő számok összegét, és
                //ha a számok összege nagyobb, mint a tényleges összeg, akkor félkövér betűkkel jelzem azt
                if (se.IsExerciseKiller && Boolean.Parse(conf.GetConfig("cageSum"))
                    && se.Killer.Ctrl.IsCurrentSumOfNumbersBiggerThenRealSum(se.Killer.Exercise[_i, _j].i))
                {
                    foreach (Pair cell in se.Killer.Cages[se.Killer.Exercise[_i, _j].i].Cells)
                        guiTable[cell.i, cell.j].Font = new Font(guiTable[_i, _j].Font.FontFamily, 19.5f, FontStyle.Regular);
                }

                //A cella előző értéke 0 lesz
                previousCellValue = 0;
            }
            //Ha a cellának van értéke, tehát beírtam egy számot
            else
            {
                //value a megváltozott TextBox jelenlegi értéke lesz
                int value;

                //Ha a beírt karaktert nem sikerül számmá konvertálni (betű vagy bármilyen más - nem szám - karakter) vagy a beírt szám 0,
                if (!Int32.TryParse(guiTable[_i, _j].Text, out value) || guiTable[_i, _j].Text == "0")
                {
                    guiTable[_i, _j].Clear(); //akkor törlöm a beírt karaktert. Ezzel érem el, hogy számon kívül semmi más nem írható a cellába.
                    return;
                }

                //Ha a beírt karakter szám, akkor beírom a táblákba
                se.Exercise[0][_i, _j] = se.Exercise[value][_i, _j] = value;

                //A számbeírásnak megfelelően változtatom a táblák értékeit
                se.Ctrl.MakeHousesOccupied(value, _i, _j);
                if (se.IsExerciseKiller)
                    se.Killer.Ctrl.ketrecKitolt(se.Killer.Exercise[_i, _j].i);

                //Csökkentem az üres cellák számát, ha a cella üres volt
                if (previousCellValue == 0)
                    se.NumberOfEmptyCells--;

                //Ha már nincs több üres cella, akkor le lehet ellenőrizni a feladatot, ezért az ellenőrzés gomb aktívvá válik
                if (se.NumberOfEmptyCells == 0)
                    verifyExerciseButton.Enabled = true;

                //Ha a piros segítség engedélyezve van, piros lesz a TextBox háttérszíne, ha a beírt szám szerepel valamelyik, a cellához tartozó házban
                if (Boolean.Parse(conf.GetConfig("helpRed")))
                    //Ha nem Killer feladat
                    if (!se.IsExerciseKiller)
                    {
                        //Ha van ütközés, piros lesz a háttérszín
                        if (se.Ctrl.HousesContainValue(_i, _j, value))
                            guiTable[_i, _j].BackColor = Color.Red;
                        //egyébként pedig a feladat fajtájától és a cellától függően fehér vagy világoskék
                        else
                            SetCellBackgroundColor(guiTable[_i, _j]);
                    }
                    //Ha Killer feladat
                    else
                    {
                        //Ha van ütközés, piros lesz a háttérszín
                        if (se.Ctrl.HousesContainValue(_i, _j, value) || se.Killer.Ctrl.HouseContainsValue(_i, _j, value, se.Exercise[0]))
                            guiTable[_i, _j].BackColor = Color.Red;
                        //A ketrec az eredeti háttérszínét kapja
                        else
                            SetKillerCellBackgroundColor(guiTable[_i, _j]);
                    }

                //Ha ellenőrizni kell a ketrecben levő számok összegét
                if (se.IsExerciseKiller && Boolean.Parse(conf.GetConfig("cageSum"))
                    && se.Killer.Ctrl.IsCurrentSumOfNumbersBiggerThenRealSum(se.Killer.Exercise[_i, _j].i))
                {
                    foreach (Pair cell in se.Killer.Cages[se.Killer.Exercise[_i, _j].i].Cells)
                        guiTable[cell.i, cell.j].Font = new Font(guiTable[_i, _j].Font.FontFamily, 19.5f, FontStyle.Bold);
                }

                /*previousCellValue az éppen a cellába beírt érték lesz. Ez azért kell, mert ha ezen a cellán maradok és kitörlöm a beírt értéket, akkor
                 * nem lesz meg az az érték, ami szükséges lenne a táblák számkivétel szerinti átrendezéséhez. */
                Int32.TryParse(guiTable[GetSenderTag(sender).i, GetSenderTag(sender).j].Text, out previousCellValue);
            }

            //Üres cellák számának frissítése
            numberOfEmptyCellsLabel.Text = loc.GetLoc("numof_empty_cells") + ": " + se.NumberOfEmptyCells;
        }

        #endregion

        private bool IsCellSpecial(int i, int j)
        {
            return (se.ExerciseType == SudokuType.SudokuX && SudokuXController.CellIsInAnyDiagonal(i, j))
                || (se.ExerciseType == SudokuType.CenterDot && CenterDotController.CellIsAtMiddleOfBlock(i, j));
        }

        private void SetCellBackgroundColor(TextBox cell)
        {
            cell.BackColor = IsCellSpecial((cell.Tag as Pair).i, (cell.Tag as Pair).j) ? Color.LightBlue : Color.White;
        }

        private void SetKillerCellBackgroundColor(TextBox cell)
        {
            int i = (cell.Tag as Pair).i;
            int j = (cell.Tag as Pair).j;
            cell.BackColor =
                se.Killer.Exercise[i, j].i <= 10
                ? colors[se.Killer.Exercise[i, j].i]
                : colors[se.Killer.Exercise[i, j].i - 10];
        }

        private Pair GetSenderTag(object sender)
        {
            return ((TextBox)sender).Tag as Pair;
        }

        #endregion
        #endregion
    }
}
