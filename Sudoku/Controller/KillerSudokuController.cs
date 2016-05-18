using System.Collections.Generic;
using Sudoku.Generate;

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
            //Változó a számok összegének tárolására
            int sum;

            //Végigmegyek a ketreceken
            foreach (KeyValuePair<int, Cage> cage in se.Killer.Cages)
            {
                //Kezdetben az összeg 0
                sum = 0;

                //Kiszámolom a ketrecben levő értékek összegét
                foreach (Pair cell in cage.Value.Cells)
                    sum += se.Solution[cell.row, cell.col]; //összeadom a ketrecben levő számokat

                //Elmentem az adott ketrecszámot és a hozzá tartozó összeget
                se.Killer.Cages[cage.Key].SumOfNumbers = sum;
            }
        }

        public bool IsCellAtTopLeftOfCage(KeyValuePair<Pair, int> cage, int row, int col)
        {
            return cage.Key.row == row && cage.Key.col == col;
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

            //Sorok
            //Végigmegyek minden soron
            if (sorEredmenyJo(haz))
                return false;

            //Oszlopok
            //Végigmegyek minden oszlopon
            if (oszlopEredmenyJo(haz))
                return false;

            //Blokkok
            //Végigmegyek minden blokkon
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

            //Ketrecek
            //Végigmegyek minden ketrecen
            foreach (KeyValuePair<int, Cage> cage in se.Killer.Cages)
            {
                foreach (Pair cell in cage.Value.Cells)
                {
                    //Minden ketrec vizsgálata elején törlöm haz értékeit
                    haz.Clear();

                    //Ha haz még nem tartalmazza az aktuális értéket, akkor hozzáadom
                    if (hazTartalmazErtek(haz, se.Exercise[0][cell.row, cell.col]))
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

        public bool ketrecOsszegJo(int cageIndex, List<Pair> cells)
        {
            int osszeg = 0;

            //Végigmegyek a ketrec celláin,
            foreach (Pair cell in cells)
            {
                //ha a számok aktuális összege már nagyobb, mint a tényleges összeg, akkor false-szal térek vissza
                if ((osszeg += se.Exercise[0][cell.row, cell.col]) > se.Killer.Cages[cageIndex].SumOfNumbers)
                    return false;
            }

            return true;
        }

        /// <summary>Megvizsgálja, hogy a kszam ketrecbe eddig beírt számok összeg nagyobb-e a tényleges összegnél</summary>
        /// <param name="cageIndex">A vizsgálandó ketrec száma</param>
        /// <param name="tomb">A feladat aktuális állapotát tartalmazó tömb</param>
        /// <returns>Ha a számok aktuális összege nagyobb, mint a tényleges összeg, akkor false-szal térek vissza, egyébként true-val</returns>
        public bool IsCurrentSumOfNumbersBiggerThanRealSum(int cageIndex)
        {
            return ketrecOsszegJo(cageIndex, se.Killer.Cages[cageIndex].Cells);
        }

        /// <summary>Megvizsgálja, hogy a megadott érték benne van-e a megadott (kszam számú) ketrecben</summary>
        /// <param name="value">A keresendő érték</param>
        /// <param name="cageIndex">A vizsgálandó ketrec száma</param>
        /// <param name="tomb">A vizsgálandó tömb: feladat vagy megoldás tömbje</param>
        /// <returns>Ha benne van az érték, akkor true, egyébként false</returns>
        private bool ketrecTartalmazErtek(int value, int cageIndex, int[,] tomb)
        {
            //Végigmegyek a megadott ketrec celláin
            foreach (Pair cell in se.Killer.Cages[cageIndex].Cells)
            {
                //Ha megtalálom, visszatérek true-val
                if (tomb[cell.row, cell.col] == value)
                    return true;
            }

            //Ha nem találtam meg, akkor false-szal térek vissza
            return false;
        }

        /// <summary>Megkeresi a legelső olyan üres cellát, ami még nem lett egyik ketrecben se elhelyezve</summary>
        /// <returns>Ha talált cellát, akkor a cella indexeivel tér vissza, egyébként pedig a (-1,-1) számpárral</returns>
        public Pair FindFirstEmptyCell()
        {
            //Ciklusváltozó
            int p = 0;

            //Amíg nem lépek ki a tábláról és a vizsgált cella nem üres, átlépek a következő cellára
            while (p < se.LAST_CELL_INDEX && se.Killer.Exercise[p / 9, p % 9].CageIndex != 0)
                p++;

            //Ha nem léptem ki a tábláról, akkor van üres cella, visszatérek az indexeivel
            if (p < se.LAST_CELL_INDEX)
                return new Pair(p / 9, p % 9);
            //Egyébként a (-1,-1) számpárral
            else
                return new Pair(-1, -1);
        }

        /// <summary>Összegyűjti azokat a ketrecszámokat, amely ketrecek az [i,j] cella szomszédja(i) és az [i,j] cella értéke még nem szerepel
        /// a szomszéd cella ketrecében, és a szomszéd cella ketrece 9-nél kevesebb értéket tartalmaz</summary>
        /// <param name="cella">A keresendő érték cellája</param>
        /// <returns>A lehetséges ketreceket tároló listával tér vissza</returns>
        public List<int> FindPossibleNeighbourCages(Pair cella)
        {
            //Lekérem a lehetséges szomszédokat
            List<Pair> cellaLista = FindPossibleNeighbourCells(cella, -1, false);
            //Lista a lehetséges ketrecszámok tárolására
            List<int> listaInt = new List<int>();

            //Végigmegyek a lehetséges szomszédokon
            foreach (Pair _cella in cellaLista)
            {
                //Ha a szomszéd cella ketrece kevesebb, mint 9 cellából áll, akkor ez a ketrec szóba jöhet
                if (se.Killer.Cages[se.Killer.Exercise[_cella.row, _cella.col].CageIndex].Cells.Count < 9)
                    listaInt.Add(se.Killer.Exercise[_cella.row, _cella.col].CageIndex);
            }

            //Visszatérek a lehetséges ketrecszámokkal
            return listaInt;
        }

        /// <summary>ketrecTomb-ben és ketrecSzotarban beállítja a megfelelő értékeket</summary>
        /// <param name="cell">A beállítandó cella</param>
        /// <param name="kszam">Ehhez a ketrechez adom hozzá az [i,j] indexű cellát</param>
        public void PutCellInCage(Pair cell, int cageIndex)
        {
            //Az [i,j] cellát elhelyezem a kszam számú ketrecben
            se.Killer.Exercise[cell.row, cell.col].CageIndex = cageIndex;

            //Ha létre kell hozni egy új ketrecet, akkor megteszem
            if (!se.Killer.Cages.ContainsKey(cageIndex))
                se.Killer.Cages.Add(cageIndex, new Cage());

            //Hozzáadom a ketrechez a cellát
            se.Killer.Cages[cageIndex].Cells.Add(new Pair(cell.row, cell.col));
        }

        /// <summary>Megvizsgálja az [i,j] indexű cella 4 szomszéd celláját, 
        /// hogy melyik szomszéd cella értéke van benne az [i,j] indexű cella ketrecében</summary>
        /// <param name="irany">1: balra: [i, j - 1], 2: jobbra: [i, j + 1], 3: fel: [i - 1, j], 4: le: [i + 1, j]</param>
        /// <param name="i">A viszonyítást képező cella sorindexe</param>
        /// <param name="j">A viszonyítást képező cella oszlopindexe</param>
        /// <param name="kSzam">Az [i,j] indexű cella ketrecszáma</param>
        /// <param name="egyenlo">Két fajta vizsgálat megkülönböztetésére szolgál</param>
        /// <param name="lista">Ebben a listában tárolja el a lehetséges szomszédokat</param>
        private void iranyMegad(int irany, int i, int j, int kSzam, bool egyenlo, List<Pair> lista)
        {
            int ii = i, jj = j;

            switch (irany)
            {
                case 1:
                    --jj;
                    break;
                case 2:
                    ++jj;
                    break;
                case 3:
                    --ii;
                    break;
                case 4:
                    ++ii;
                    break;
            }

            if (egyenlo
                /* Ha az [i, j] indexű cella ketrecéhez szeretném hozzávenni valamelyik szomszéd cellát.
                 * A szomszéd szerepel-e már valamelyik ketrecben, és a szomszéd cella értéke benne van-e az [i,j] indexű cella ketrecében*/
                ? se.Killer.Exercise[ii, jj].CageIndex == 0 && !ketrecTartalmazErtek(se.Solution[ii, jj], kSzam, se.Solution)

                /* Ha az [i,j] indexű cellát szeretném valamelyik szomszéd cella ketrecében elhelyezni.
                * Ez akkor jöhet elő, ha az [i,j] indexű cella üresen marad, és a körülötte levő cellák már mind benne vannak egy ketrecben.
                * Ha a szomszéd már benne van egy ketrecben, és a szomszéd cella ketrece nem tartalmazza az [i,j] indexű cella értékét*/
                : se.Killer.Exercise[ii, jj].CageIndex != 0 && !ketrecTartalmazErtek(se.Solution[i, j], se.Killer.Exercise[ii, jj].CageIndex, se.Solution))
                lista.Add(new Pair(ii, jj));
        }

        private void balraJobbraViszgalat(int i, int j, int kSzam, bool egyenlo, List<Pair> lista)
        {
            //Balra
            iranyMegad(1, i, j, kSzam, egyenlo, lista);

            //Jobbra
            iranyMegad(2, i, j, kSzam, egyenlo, lista);
        }

        private void felLeVizsgalat(int i, int j, int kSzam, bool egyenlo, List<Pair> lista)
        {
            //Fel
            iranyMegad(3, i, j, kSzam, egyenlo, lista);

            //Le
            iranyMegad(4, i, j, kSzam, egyenlo, lista);
        }

        private void sarokEsBenneSorVizsgalat(int i, int j, int kSzam, bool egyenlo, List<Pair> lista)
        {
            //i=0: Ha a bal felső sarokban van, i=8: Ha a bal alsó sarokban van
            if (j == 0)
                //Jobbra
                iranyMegad(2, i, j, kSzam, egyenlo, lista);

            //i=0: Ha a jobb felső sarokban van, i=8: Ha a jobb alsó sarokban van
            else if (j == 8)
                //Balra
                iranyMegad(1, i, j, kSzam, egyenlo, lista);

            //Ha az előző 2 kivételével valahol a sorban
            else
                //Balra, jobbra
                balraJobbraViszgalat(i, j, kSzam, egyenlo, lista);
        }

        /// <summary>A megadott cella elhelyezkedésétől függően megkeresi a cella lehetséges szomszédait</summary>
        /// <param name="cella">A viszonyítást képző cella</param>
        /// <param name="kSzam">A vizsgálandó ketrec száma</param>
        /// <param name="egyenlo">Két fajta vizsgálat megkülönböztetésére szolgál</param>
        /// <returns>A lehetséges szomszédokat tartalmazó listával tér vissza</returns>
        public List<Pair> FindPossibleNeighbourCells(Pair cella, int kSzam, bool egyenlo)
        {
            List<Pair> lista = new List<Pair>();

            //Ha a cella az első sorban van
            if (cella.row == 0)
            {
                //Le
                iranyMegad(4, cella.row, cella.col, kSzam, egyenlo, lista);

                sarokEsBenneSorVizsgalat(cella.row, cella.col, kSzam, egyenlo, lista);

                return lista;
            }

            //Ha a cella az utolsó sorban van
            if (cella.row == 8)
            {
                //Fel
                iranyMegad(3, cella.row, cella.col, kSzam, egyenlo, lista);

                sarokEsBenneSorVizsgalat(cella.row, cella.col, kSzam, egyenlo, lista);

                return lista;
            }

            //Ha a cella a bal szélső oszlopban van
            if (cella.col == 0)
            {
                //Jobbra
                iranyMegad(2, cella.row, cella.col, kSzam, egyenlo, lista);

                //Fel, le
                felLeVizsgalat(cella.row, cella.col, kSzam, egyenlo, lista);

                return lista;
            }

            //Ha a cella a jobb szélső oszlopban van
            if (cella.col == 8)
            {
                //Balra
                iranyMegad(1, cella.row, cella.col, kSzam, egyenlo, lista);

                //Fel, le
                felLeVizsgalat(cella.row, cella.col, kSzam, egyenlo, lista);

                return lista;
            }

            /* Ha a cella indexei egyik előző esetnek sem felelnek meg, akkor mind a 4 szomszédot megvizsgálhatom*/
            //Balra, jobbra
            balraJobbraViszgalat(cella.row, cella.col, kSzam, egyenlo, lista);

            //Fel, le
            felLeVizsgalat(cella.row, cella.col, kSzam, egyenlo, lista);

            return lista;
        }

        /// <summary>Összegyűjti a ketrecekben levő számok összegét, és minden ketrecnek azt a celláját,
        /// amelybe a megjelenítéskor a ketrecben levő értékek összegét kell írni</summary>
        /// <param name="generaltFeladat">Azt mondja meg, hogy a feladat generált vagy fájlból van beolvasva.</param>
        public Dictionary<Pair, int> GetSumOfNumbersAndIndicatorCages()
        {
            //Csak akkor kell kiszámolni a ketrecösszegeket, ha a feladat generált. Beolvasáskor az összegek beolvasásre kerülnek.
            if (se.IsExerciseGenerated)
                CalculateSumOfNumbersInAllCages();

            //Változó a ketrec azon cellájának tárolására, ahova majd írni kell (megjelenítéskor) a ketrec összegét
            Pair balFelsoCella;

            //Szótár a visszaadandó értékek tárolására
            Dictionary<Pair, int> sarokEsOsszeg = new Dictionary<Pair, int>();

            //végigmegyek a ketreceken
            foreach (KeyValuePair<int, Cage> cage in se.Killer.Cages)
            {
                //(9,9) lesz a cella értéke
                balFelsoCella = new Pair(9, 9);

                /* Kiszámolom a ketrecben levő értékek összegét 
                 * és meghatározom azt a cellát (pontosabban a cella sorindexét) a ketrecben, ami a legfelül helyezkedik el*/
                foreach (Pair cell in cage.Value.Cells)
                {
                    /* Ha találok olyan cellát, ami még feljebb van az eddigi legfelül lévőnél,
                     * akkor ez az új cella lesz a legfelül levő*/
                    if (cell.row < balFelsoCella.row)
                    {
                        balFelsoCella.row = cell.row;
                        balFelsoCella.col = cell.col;
                    }
                }

                //A legfelül levő cellák közül megkeresem azt a cellát, ami a legbalrább helyezkedik el
                foreach (Pair cell in cage.Value.Cells)
                {
                    //Ha a cella a legfelül levők között van
                    if (cell.row == balFelsoCella.row)
                    {
                        /* Ha találok olyan cellát, ami még balrább van az eddigi legbalrább lévőnél,
                         * akkor ez az új cella lesz a legbalrább levő*/
                        if (cell.col < balFelsoCella.col)
                        {
                            balFelsoCella.col = cell.col;
                            balFelsoCella.row = cell.row;
                        }
                    }
                }

                //Elmentem a ketrec bal felső celláját és a ketrecben levő számok összegét
                sarokEsOsszeg.Add(balFelsoCella, se.Killer.Cages[cage.Key].SumOfNumbers);
            }

            //Visszadom a kiszámolt értékeket
            return sarokEsOsszeg;
        }

        /// <summary>Meghívja az egyes házakhoz tartozó érték-tartalmazást vizsgáló függvényeket 
        /// és azok visszaadott értékei szerint ad vissza értéket.</summary>
        /// <param name="i">A vizsgálandó cella sorindexe.</param>
        /// <param name="j">A vizsgálandó cella oszlopindexe.</param>
        /// <param name="ertek">A keresendő érték.</param>
        /// <param name="tomb"></param>
        /// <returns>Ha egyik ház sem tartalmazza ertek-et, akkor false, egyébként true</returns>
        public bool HouseContainsValue(int i, int j, int ertek, int[,] tomb)
        {
            return !ketrecTartalmazErtek(ertek, se.Killer.Exercise[i, j].CageIndex, tomb);
        }

        /// <summary>A megadott - [i,j] - cella ketrecében állítja foglaltra azokat a cellákat, melyek még nem voltak azok</summary>
        /// <param name="cageIndex">A kitöltött cella ketrecszáma.</param>
        public void ketrecKitolt(int cageIndex)
        {
            //Végigmegyek az [i,j] cella ketrecén
            foreach (Pair cell in se.Killer.Cages[cageIndex].Cells)
            {
                //Foglalt cellák beállítása
                if (se.Solution[cell.row, cell.col] == 0)
                    se.Solution[cell.row, cell.col] = se.OCCUPIED;
            }
        }
    }
}
