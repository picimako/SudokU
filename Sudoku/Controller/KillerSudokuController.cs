using System.Linq;
using System.Collections.Generic;
using Sudoku.Generate;
using Sudoku.Controller.Finder;

namespace Sudoku.Controller
{
    public class KillerSudokuController
    {
        SudokuExercise se = SudokuExercise.get;

        public KillerSudokuController()
        {
            se.Solution = new int[9, 9];
        }

        public void CopySolutionToExercise(int[,] megoldasTomb)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    //se.Killer.Exercise[i, j].ertek = megoldasTomb[i, j];
                    se.Solution[i, j] = megoldasTomb[i, j];
                }
            }
        }

        /// <summary>Kiszámolja, hogy melyik ketrechez milyen ketrecösszeg tartozik</summary>
        private void CalculateSumOfNumbersInAllCages()
        {
            foreach (KeyValuePair<int, Cage> cage in se.Killer.Cages)
            {
                int sumOfNumbersInCage = cage.Value.Cells.Sum(cell => se.Solution[cell.Row, cell.Col]);

                //Elmentem az adott ketrecszámot és a hozzá tartozó összeget
                se.Killer.Cages[cage.Key].SumOfNumbers = sumOfNumbersInCage;
            }
        }

        public bool IsCellAtTopLeftOfCage(KeyValuePair<Cell, int> cage, int row, int col)
        {
            return cage.Key.Row == row && cage.Key.Col == col;
        }

        /// <summary>Beolvassa a megadott fájlból a killer típusú feladatot</summary>
        /// <param name="feladatFajl">A beolvasandó fájl</param>
        /// <returns>Ha hiba volt, akkor false, ha minden rendben volt a beolvasás során, akkor true</returns>
        public bool ReadKillerSudoku(string feladatFajl)
        {
            return ExerciseReader.ReadKillerSudoku(feladatFajl);
        }

        /// <summary>A kitöltött feladványt megvizsgálja minden feltétel alapján, hogy jó-e vagy sem</summary>
        /// <param name="tomb">A vizsgálandó tömb, vagyis a megoldott feladat</param>
        /// <returns>Ha jó a megoldás, akkor true, egyébként false</returns>
        public bool IsKillerSolutionCorrect()
        {
            /*Amit ellenőrizni kell:
             * sorok
             * oszlopok
             * blokkok
             * x esetén az x, center esetén a középső cellák
             * ketrecek
             * ketrecösszegek*/

            //Lista, amely az aktuális házban levő számokat tárolja
            List<int> haz = new List<int>();

            //Végigmegyek minden SORon
            //TODO: check whether this should be "if (!sorEredmenyJo(haz))"
            if (sorEredmenyJo(haz))
                return false;

            //Végigmegyek minden OSZLOPon
            //TODO: check whether this should be "if (!sorEredmenyJo(haz))"
            if (oszlopEredmenyJo(haz))
                return false;

            //Végigmegyek minden BLOKKon
            for (int bl = 0; bl < 9; bl++)
            {
                //Minden blokk vizsgálata elején törlöm haz értékeit
                haz.Clear();

                //Végigmegyek a blokk elemein
                for (int i = bl - (bl % 3); i <= (bl - (bl % 3)) + 2; i++)
                {
                    for (int j = (bl % 3) * 3; j <= ((bl % 3) * 3) + 2; j++)
                    {
                        //Ha haz még nem tartalmazza az aktuális értéket, akkor hozzáadom
                        if (hazTartalmazErtek(haz, se.Exercise[0][i, j]))
                            //Ha már tartalmazza, akkor nem jó a megoldás
                            return false;
                    }
                }
            }

            //Sudoku-X esetén az átlók
            if (se.ExerciseType == SudokuType.SudokuX)
            {
                //Az átló vizsgálata elején törlöm haz értékeit
                haz.Clear();

                //Végigmegyek a főátlón
                for (int r = 0; r < 9; r++)
                {
                    //Ha haz még nem tartalmazza az aktuális értéket, akkor hozzáadom
                    if (hazTartalmazErtek(haz, se.Exercise[0][r, r]))
                        //Ha már tartalmazza, akkor nem jó a megoldás
                        return false;
                }

                //Az átló vizsgálata elején törlöm haz értékeit
                haz.Clear();

                //Végigmegyek a mellékátlón
                for (int r = 8; r >= 0; r--)
                {
                    //Ha haz még nem tartalmazza az aktuális értéket, akkor hozzáadom
                    if (hazTartalmazErtek(haz, se.Exercise[0][r, 8 - r]))
                        //Ha már tartalmazza, akkor nem jó a megoldás
                        return false;
                }
            }

            //Középpont Sudoku esetén a középső cellák
            if (se.ExerciseType == SudokuType.CenterDot)
            {
                //A vizsgálat elején törlöm haz értékeit
                haz.Clear();

                //Végigmegyek a blokok középső celláin
                for (int r = 1; r <= 7; r += 3)
                {
                    for (int p = 1; p <= 7; p += 3)
                    {
                        //Ha haz még nem tartalmazza az aktuális értéket, akkor hozzáadom
                        if (hazTartalmazErtek(haz, se.Exercise[0][r, p]))
                            //Ha már tartalmazza, akkor nem jó a megoldás
                            return false;
                    }
                }
            }

            //Végigmegyek minden KETRECen
            foreach (KeyValuePair<int, Cage> cage in se.Killer.Cages)
            {
                foreach (Cell cell in cage.Value.Cells)
                {
                    //Minden ketrec vizsgálata elején törlöm haz értékeit
                    haz.Clear();

                    //Ha haz még nem tartalmazza az aktuális értéket, akkor hozzáadom
                    if (hazTartalmazErtek(haz, se.Exercise[0][cell.Row, cell.Col]))
                        //Ha már tartalmazza, akkor nem jó a megoldás
                        return false;
                }
            }

            foreach (KeyValuePair<int, Cage> cage in se.Killer.Cages)
            {
                if (!ketrecOsszegJo(cage.Key, cage.Value.Cells))
                    return false;
            }

            return true;
        }

        /// <summary>Megvizsgálja, hogy haz tartalmazza-e tomb adott [i, j] helyen levő értékét. Ha nem, akkor hozzáadja.</summary>
        /// <param name="haz">A vizsgálandó ház</param>
        /// <param name="cellValue">A vizsgálandó érték</param>
        /// <returns>Ha a ház nem tartalmazza az adott értéket, akkor false, egyébként true.</returns>
        private bool hazTartalmazErtek(List<int> haz, int cellValue)
        {
            if (!haz.Contains(cellValue))
            {
                haz.Add(cellValue);
                return false;
            }

            return true;
        }

        private bool sorEredmenyJo(List<int> haz)
        {
            return sorOszlopEredmenyJo(haz, true);
        }

        private bool oszlopEredmenyJo(List<int> haz)
        {
            return sorOszlopEredmenyJo(haz, false);
        }

        /// <summary>Megvizsgálja az eredményt, hogy a sorok vagy oszlopok jól vannak-e kitöltve.</summary>
        /// <param name="haz">Az aktuálisan vizsgálandó sor vagy oszlop.</param>
        /// <param name="tomb">A feladat megoldása.</param>
        /// <param name="sor">Azt mondja meg, hogy sort vagy oszlopot kell vizsgálni.</param>
        private bool sorOszlopEredmenyJo(List<int> haz, bool sor)
        {
            for (int i = 0; i < 9; i++)
            {
                //Minden sor/oszlop vizsgálata elején törlöm haz értékeit
                haz.Clear();

                //Végigmegyek a sor/oszlop elemein
                for (int j = 0; j < 9; j++)
                {
                    //Ha haz még nem tartalmazza az aktuális értéket, akkor hozzáadom
                    if (sor ? hazTartalmazErtek(haz, se.Exercise[0][i, j]) : hazTartalmazErtek(haz, se.Exercise[0][j, i]))
                        //Ha már tartalmazza, akkor nem jó a megoldás
                        return false;
                }
            }

            return true;
        }

        public bool ketrecOsszegJo(int cageIndex, List<Cell> cellsInCage)
        {
            int sumOfCellValuesInCage = cellsInCage.Sum(cell => se.Exercise[0][cell.Row, cell.Col]);
            return sumOfCellValuesInCage <= se.Killer.Cages[cageIndex].SumOfNumbers;
        }

        /// <summary>Megvizsgálja, hogy a cageIndex ketrecbe eddig beírt számok összeg nagyobb-e a tényleges összegnél</summary>
        /// <param name="cageIndex">A vizsgálandó ketrec száma</param>
        /// <param name="tomb">A feladat aktuális állapotát tartalmazó tömb</param>
        /// <returns>Ha a számok aktuális összege nagyobb, mint a tényleges összeg, akkor false-szal térek vissza, egyébként true-val</returns>
        public bool IsCurrentSumOfNumbersBiggerThanRealSum(int cageIndex)
        {
            return ketrecOsszegJo(cageIndex, se.Killer.Cages[cageIndex].Cells);
        }

        /// <summary>Megvizsgálja, hogy a megadott érték benne van-e a megadott (cageIndex számú) ketrecben</summary>
        /// <param name="value">A keresendő érték</param>
        /// <param name="cageIndex">A vizsgálandó ketrec száma</param>
        /// <param name="tomb">A vizsgálandó tömb: feladat vagy megoldás tömbje</param>
        /// <returns>Ha benne van az érték, akkor true, egyébként false</returns>
        private bool ketrecTartalmazErtek(int value, int cageIndex, int[,] tomb)
        {
            List<Cell> cellsOfValueFoundInCage = se.Killer.Cages[cageIndex].Cells
                .Where(cell => tomb[cell.Row, cell.Col] == value)
                .ToList();
            return cellsOfValueFoundInCage.Count > 0;
        }

        /// <summary>Megkeresi a legelső olyan üres cellát, ami még nem lett egyik ketrecben se elhelyezve</summary>
        /// <returns>Ha talált cellát, akkor a cella indexeivel tér vissza, egyébként pedig a (-1,-1) számpárral</returns>
        public Cell FindFirstEmptyCell()
        {
            int p = 0;

            //Amíg nem lépek ki a tábláról és a vizsgált cella nem üres, átlépek a következő cellára
            while (p < se.LAST_CELL_INDEX && se.Killer.Exercise[p / 9, p % 9].CageIndex != 0)
                p++;

            //Ha nem léptem ki a tábláról, akkor van üres cella, visszatérek az indexeivel
            if (p < se.LAST_CELL_INDEX)
                return new Cell(p / 9, p % 9);
            else
                return new Cell(-1, -1);
        }

        /// <summary>Összegyűjti azokat a ketrecszámokat, amely ketrecek az [i,j] cella szomszédja(i) és az [i,j] cella értéke még nem szerepel
        /// a szomszéd cella ketrecében, és a szomszéd cella ketrece 9-nél kevesebb értéket tartalmaz</summary>
        /// <param name="cell">A keresendő érték cellája</param>
        /// <returns>The list of possible neighbour cages.</returns>
        public List<int> FindPossibleNeighbourCages(Cell cell)
        {
            List<Cell> possibleNeighbourCells = FindPossibleNeighbourCells(cell, -1, false);
            List<int> possibleCageIndeces = new List<int>();

            foreach (Cell neighbourCell in possibleNeighbourCells)
            {
                int cageIndexOfNeighbourCell = se.Killer.Exercise[neighbourCell.Row, neighbourCell.Col].CageIndex;
                //Ha a szomszéd cella ketrece kevesebb, mint 9 cellából áll, akkor ez a ketrec szóba jöhet
                if (se.Killer.Cages[cageIndexOfNeighbourCell].Cells.Count < 9)
                    possibleCageIndeces.Add(cageIndexOfNeighbourCell);
            }

            return possibleCageIndeces;
        }

        /// <summary>ketrecTomb-ben és ketrecSzotarban beállítja a megfelelő értékeket</summary>
        /// <param name="cell">A beállítandó cella</param>
        /// <param name="cageIndex">Ehhez a ketrechez adom hozzá az [row,col] indexű cellát</param>
        public void PutCellInCage(Cell cell, int cageIndex)
        {
            //A [row,col] cellát elhelyezem a cageIndex számú ketrecben
            se.Killer.Exercise[cell.Row, cell.Col].CageIndex = cageIndex;

            //Ha létre kell hozni egy új ketrecet, akkor megteszem
            if (!se.Killer.Cages.ContainsKey(cageIndex))
                se.Killer.Cages.Add(cageIndex, new Cage());

            //Hozzáadom a ketrechez a cellát
            se.Killer.Cages[cageIndex].Cells.Add(new Cell(cell.Row, cell.Col));
        }

        /// <summary>Megvizsgálja az [i,j] indexű cella 4 szomszéd celláját, 
        /// hogy melyik szomszéd cella értéke van benne az [i,j] indexű cella ketrecében</summary>
        /// <param name="direction">The direction to search towards</param>
        /// <param name="cell">A viszonyítást képező cella</param>
        /// <param name="cageIndex">Az [i,j] indexű cella ketrecszáma</param>
        /// <param name="egyenlo">Két fajta vizsgálat megkülönböztetésére szolgál</param>
        /// <param name="possibleNeighbourCells">Ebben a listában tárolja el a lehetséges szomszédokat</param>
        private void iranyMegad(Direction direction, Cell cell, int cageIndex, bool egyenlo, List<Cell> possibleNeighbourCells)
        {
            int row = cell.Row, col = cell.Col;

            switch (direction)
            {
                case Direction.LEFT:
                    --col;
                    break;
                case Direction.RIGHT:
                    ++col;
                    break;
                case Direction.UP:
                    --row;
                    break;
                case Direction.DOWN:
                    ++row;
                    break;
            }

            if (egyenlo
                /* Ha az [i, j] indexű cella ketrecéhez szeretném hozzávenni valamelyik szomszéd cellát.
                 * A szomszéd szerepel-e már valamelyik ketrecben, és a szomszéd cella értéke benne van-e az [i,j] indexű cella ketrecében*/
                ? se.Killer.Exercise[row, col].CageIndex == 0 && !ketrecTartalmazErtek(se.Solution[row, col], cageIndex, se.Solution)

                /* Ha az [i,j] indexű cellát szeretném valamelyik szomszéd cella ketrecében elhelyezni.
                * Ez akkor jöhet elő, ha az [i,j] indexű cella üresen marad, és a körülötte levő cellák már mind benne vannak egy ketrecben.
                * Ha a szomszéd már benne van egy ketrecben, és a szomszéd cella ketrece nem tartalmazza az [i,j] indexű cella értékét*/
                : se.Killer.Exercise[row, col].CageIndex != 0 && !ketrecTartalmazErtek(se.Solution[cell.Row, cell.Col], se.Killer.Exercise[row, col].CageIndex, se.Solution))
                possibleNeighbourCells.Add(new Cell(row, col));
        }

        private void balraJobbraViszgalat(Cell cell, int cageIndex, bool egyenlo, List<Cell> possibleNeighbourCells)
        {
            iranyMegad(Direction.LEFT, cell, cageIndex, egyenlo, possibleNeighbourCells);

            iranyMegad(Direction.RIGHT, cell, cageIndex, egyenlo, possibleNeighbourCells);
        }

        private void felLeVizsgalat(Cell cell, int cageIndex, bool egyenlo, List<Cell> possibleNeighbourCells)
        {
            iranyMegad(Direction.UP, cell, cageIndex, egyenlo, possibleNeighbourCells);

            iranyMegad(Direction.DOWN, cell, cageIndex, egyenlo, possibleNeighbourCells);
        }

        private void sarokEsBenneSorVizsgalat(Cell cell, int cageIndex, bool egyenlo, List<Cell> possibleNeighbourCells)
        {
            //i=0: Ha a bal felső sarokban van, i=8: Ha a bal alsó sarokban van
            if (cell.Col == 0)
            {
                iranyMegad(Direction.RIGHT, cell, cageIndex, egyenlo, possibleNeighbourCells);
            }
            //i=0: Ha a jobb felső sarokban van, i=8: Ha a jobb alsó sarokban van
            else if (cell.Col == 8)
            {
                iranyMegad(Direction.LEFT, cell, cageIndex, egyenlo, possibleNeighbourCells);
            }
            //Ha az előző 2 kivételével valahol a sorban
            else
            {
                balraJobbraViszgalat(cell, cageIndex, egyenlo, possibleNeighbourCells);
            }
        }

        /// <summary>A megadott cella elhelyezkedésétől függően megkeresi a cella lehetséges szomszédait</summary>
        /// <param name="cell">A viszonyítást képző cella</param>
        /// <param name="cageIndex">A vizsgálandó ketrec száma</param>
        /// <param name="egyenlo">Két fajta vizsgálat megkülönböztetésére szolgál</param>
        /// <returns>The list of possible neighbour cells.</returns>
        public List<Cell> FindPossibleNeighbourCells(Cell cell, int cageIndex, bool egyenlo)
        {
            List<Cell> possibleNeighbourCells = new List<Cell>();

            //Ha a cella az első sorban van
            if (cell.Row == 0)
            {
                iranyMegad(Direction.DOWN, cell, cageIndex, egyenlo, possibleNeighbourCells);

                sarokEsBenneSorVizsgalat(cell, cageIndex, egyenlo, possibleNeighbourCells);

                return possibleNeighbourCells;
            }

            //Ha a cella az utolsó sorban van
            if (cell.Row == 8)
            {
                iranyMegad(Direction.UP, cell, cageIndex, egyenlo, possibleNeighbourCells);

                sarokEsBenneSorVizsgalat(cell, cageIndex, egyenlo, possibleNeighbourCells);

                return possibleNeighbourCells;
            }

            //Ha a cella a bal szélső oszlopban van
            if (cell.Col == 0)
            {
                iranyMegad(Direction.RIGHT, cell, cageIndex, egyenlo, possibleNeighbourCells);

                felLeVizsgalat(cell, cageIndex, egyenlo, possibleNeighbourCells);

                return possibleNeighbourCells;
            }

            //Ha a cella a jobb szélső oszlopban van
            if (cell.Col == 8)
            {
                iranyMegad(Direction.LEFT, cell, cageIndex, egyenlo, possibleNeighbourCells);

                felLeVizsgalat(cell, cageIndex, egyenlo, possibleNeighbourCells);

                return possibleNeighbourCells;
            }

            /* Ha a cella indexei egyik előző esetnek sem felelnek meg, akkor mind a 4 szomszédot megvizsgálhatom*/
            balraJobbraViszgalat(cell, cageIndex, egyenlo, possibleNeighbourCells);

            felLeVizsgalat(cell, cageIndex, egyenlo, possibleNeighbourCells);

            return possibleNeighbourCells;
        }

        /// <summary>Összegyűjti a ketrecekben levő számok összegét, és minden ketrecnek azt a celláját,
        /// amelybe a megjelenítéskor a ketrecben levő értékek összegét kell írni</summary>
        public Dictionary<Cell, int> GetSumOfNumbersAndIndicatorCages()
        {
            //Csak akkor kell kiszámolni a ketrecösszegeket, ha a feladat generált. Beolvasáskor az összegek beolvasásre kerülnek.
            if (se.IsExerciseGenerated)
                CalculateSumOfNumbersInAllCages();

            //Változó a ketrec azon cellájának tárolására, ahova majd írni kell (megjelenítéskor) a ketrec összegét
            Cell upperLeftCornerCellOfCage;

            //Szótár a visszaadandó értékek tárolására
            Dictionary<Cell, int> sarokEsOsszeg = new Dictionary<Cell, int>();

            foreach (KeyValuePair<int, Cage> cage in se.Killer.Cages)
            {
                upperLeftCornerCellOfCage = new Cell(9, 9);

                /* Kiszámolom a ketrecben levő értékek összegét 
                 * és meghatározom azt a cellát (pontosabban a cella sorindexét) a ketrecben, ami a legfelül helyezkedik el*/
                foreach (Cell cell in cage.Value.Cells)
                {
                    /* Ha találok olyan cellát, ami még feljebb van az eddigi legfelül lévőnél,
                     * akkor ez az új cella lesz a legfelül levő*/
                    if (cell.Row < upperLeftCornerCellOfCage.Row)
                    {
                        upperLeftCornerCellOfCage.Row = cell.Row;
                        upperLeftCornerCellOfCage.Col = cell.Col;
                    }
                }

                //A legfelül levő cellák közül megkeresem azt a cellát, ami a legbalrább helyezkedik el
                foreach (Cell cell in cage.Value.Cells)
                {
                    //Ha a cella a legfelül levők között van
                    if (cell.Row == upperLeftCornerCellOfCage.Row)
                    {
                        /* Ha találok olyan cellát, ami még balrább van az eddigi legbalrább lévőnél,
                         * akkor ez az új cella lesz a legbalrább levő*/
                        if (cell.Col < upperLeftCornerCellOfCage.Col)
                        {
                            upperLeftCornerCellOfCage.Col = cell.Col;
                            upperLeftCornerCellOfCage.Row = cell.Row;
                        }
                    }
                }

                //Elmentem a ketrec bal felső celláját és a ketrecben levő számok összegét
                sarokEsOsszeg.Add(upperLeftCornerCellOfCage, se.Killer.Cages[cage.Key].SumOfNumbers);
            }

            //Visszadom a kiszámolt értékeket
            return sarokEsOsszeg;
        }

        /// <summary>Meghívja az egyes házakhoz tartozó érték-tartalmazást vizsgáló függvényeket 
        /// és azok visszaadott értékei szerint ad vissza értéket.</summary>
        /// <param name="row">A vizsgálandó cella sorindexe.</param>
        /// <param name="col">A vizsgálandó cella oszlopindexe.</param>
        /// <param name="value">A keresendő érték.</param>
        /// <param name="tomb"></param>
        /// <returns>Ha egyik ház sem tartalmazza ertek-et, akkor false, egyébként true</returns>
        public bool HouseContainsValue(int row, int col, int value, int[,] tomb)
        {
            return !ketrecTartalmazErtek(value, se.Killer.Exercise[row, col].CageIndex, tomb);
        }

        /// <summary>A megadott cageIndex ketrec cellái közül állítja foglaltra azokat, melyek még nem voltak azok</summary>
        /// <param name="cageIndex">A kitöltött cella ketrecszáma.</param>
        public void ketrecKitolt(int cageIndex)
        {
            foreach (Cell cell in se.Killer.Cages[cageIndex].Cells)
            {
                //Foglalt cellák beállítása
                if (se.Solution[cell.Row, cell.Col] == se.EMPTY)
                    se.Solution[cell.Row, cell.Col] = se.OCCUPIED;
            }
        }
    }
}
