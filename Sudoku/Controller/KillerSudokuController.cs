using System.Linq;
using System.Collections.Generic;
using Sudoku.Generate;
using Sudoku.Controller.Finder.Killer;
using Sudoku.Log;

namespace Sudoku.Controller
{
    public class KillerSudokuController
    {
        private SudokuExercise se = SudokuExercise.get;
        private KillerSudokuExercise killer = SudokuExercise.get.Killer;
        private Logger log = Logger.Instance;
        private PossibleNeighbourCellsFinder finder = new PossibleNeighbourCellsFinder();

        public PossibleNeighbourCellsFinder NeighbourCellFinder { get { return finder; } }

        public KillerSudokuController()
        {
            se.Solution = new int[9, 9];
        }

        public void CopySolutionToExercise(int[,] solution)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    //se.Killer.Exercise[i, j].ertek = megoldasTomb[i, j];
                    se.Solution[i, j] = solution[i, j];
                }
            }
        }

        public bool IsCellAtTopLeftOfCage(KeyValuePair<Cell, int> cage, int row, int col)
        {
            return cage.Key.Row == row && cage.Key.Col == col;
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

        /// <summary>Megvizsgálja, hogy a cageIndex ketrecbe eddig beírt számok összeg nagyobb-e a tényleges összegnél</summary>
        /// <param name="cageIndex">A vizsgálandó ketrec száma</param>
        /// <param name="tomb">A feladat aktuális állapotát tartalmazó tömb</param>
        /// <returns>Ha a számok aktuális összege nagyobb, mint a tényleges összeg, akkor false-szal térek vissza, egyébként true-val</returns>
        public bool IsCurrentSumOfNumbersBiggerThanRealSum(int cageIndex)
        {
            return ketrecOsszegJo(cageIndex, se.Killer.Cages[cageIndex].Cells);
        }

        public bool ketrecOsszegJo(int cageIndex, List<Cell> cellsInCage)
        {
            int sumOfCellValuesInCage = cellsInCage.Sum(cell => se.Exercise[0][cell.Row, cell.Col]);
            return sumOfCellValuesInCage <= se.Killer.Cages[cageIndex].SumOfNumbers;
        }

        /// <summary>Megkeresi a legelső olyan üres cellát, ami még nem lett egyik ketrecben se elhelyezve</summary>
        /// <returns>Ha talált cellát, akkor a cella indexeivel tér vissza, egyébként pedig a (-1,-1) számpárral</returns>
        public Cell FindFirstEmptyCell()
        {
            int p = 0;

            //Amíg nem lépek ki a tábláról és a vizsgált cella nem üres, átlépek a következő cellára
            while (p < se.LAST_CELL_INDEX && se.Killer.IsCellInAnyCage(p / 9, p % 9))
                p++;

            //Ha nem léptem ki a tábláról, akkor van üres cella, visszatérek az indexeivel
            return p < se.LAST_CELL_INDEX ? new Cell(p / 9, p % 9) : Cell.OUT_OF_RANGE;
        }

        /// <summary>Összegyűjti azokat a ketrecszámokat, amely ketrecek az [i,j] cella szomszédja(i) és az [i,j] cella értéke még nem szerepel
        /// a szomszéd cella ketrecében, és a szomszéd cella ketrece 9-nél kevesebb értéket tartalmaz</summary>
        /// <param name="cell">A keresendő érték cellája</param>
        /// <returns>The list of possible neighbour cages.</returns>
        public List<int> FindPossibleNeighbourCages(Cell cell)
        {
            List<Cell> possibleNeighbourCells = finder.FindPossibleNeighbourCells(cell, -1, false);
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

        /// <summary>Összegyűjti a ketrecekben levő számok összegét, és minden ketrecnek azt a celláját,
        /// amelybe a megjelenítéskor a ketrecben levő értékek összegét kell írni</summary>
        public Dictionary<Cell, int> GetSumOfNumbersAndIndicatorCages()
        {
            if (se.IsExerciseGenerated)
                CalculateSumOfNumbersInAllCages();

            //This is the cell where the sum of numbers will be displayed for each cage
            Cell upperLeftCornerCellOfCage;

            Dictionary<Cell, int> cornerCellsWithSums = new Dictionary<Cell, int>();

            foreach (KeyValuePair<int, Cage> cage in se.Killer.Cages)
            {
                upperLeftCornerCellOfCage = new Cell(9, 9);
                upperLeftCornerCellOfCage = GetMostUpperCellInCage(cage, upperLeftCornerCellOfCage);
                upperLeftCornerCellOfCage = GetMostLeftCellInCage(cage, upperLeftCornerCellOfCage);

                cornerCellsWithSums.Add(upperLeftCornerCellOfCage, se.Killer.Cages[cage.Key].SumOfNumbers);
            }

            return cornerCellsWithSums;
        }

        private Cell GetMostUpperCellInCage(KeyValuePair<int, Cage> cage, Cell upperLeftCornerCellOfCage)
        {
            Cell mostUpperCell = upperLeftCornerCellOfCage;
            foreach (Cell cell in cage.Value.Cells)
            {
                if (cell.IsAbove(mostUpperCell))
                {
                    mostUpperCell.CopyIndecesOf(cell);
                }
            }
            return mostUpperCell;
        }

        private Cell GetMostLeftCellInCage(KeyValuePair<int, Cage> cage, Cell upperLeftCornerCellOfCage)
        {
            Cell mostLeftCell = upperLeftCornerCellOfCage;
            foreach (Cell cell in cage.Value.Cells)
            {
                //Ha a cella a legfelül levők között van
                if (cell.IsInSameRowAs(mostLeftCell) && cell.IsAtLeftOf(mostLeftCell))
                {
                    mostLeftCell.CopyIndecesOf(cell);
                }
            }
            return mostLeftCell;
        }

        /// <summary>A megadott cageIndex ketrec cellái közül állítja foglaltra azokat, melyek még nem voltak azok</summary>
        /// <param name="cageIndex">A kitöltött cella ketrecszáma.</param>
        public void FillInCage(int cageIndex)
        {
            foreach (Cell cell in se.Killer.Cages[cageIndex].Cells)
            {
                if (se.Solution[cell.Row, cell.Col] == se.EMPTY)
                    se.Solution[cell.Row, cell.Col] = se.OCCUPIED;
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
    }
}
