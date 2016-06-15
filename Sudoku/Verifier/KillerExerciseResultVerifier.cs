using System.Linq;
using System.Collections.Generic;
using Sudoku.Cells;
using Sudoku.Controller;
using Sudoku.Generate;

namespace Sudoku.Verifier
{
    public class KillerExerciseResultVerifier
    {
        private static SudokuExercise se = SudokuExercise.get;

        /// <summary>A kitöltött feladványt megvizsgálja minden feltétel alapján, hogy jó-e vagy sem</summary>
        // TODO: move this to a new KillerExerciseResultVerifier for it
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
            List<int> house = new List<int>();

            //Végigmegyek minden SORon
            //TODO: check whether this should be "if (!sorEredmenyJo(haz))"
            if (sorEredmenyJo(house))
                return false;

            //Végigmegyek minden OSZLOPon
            //TODO: check whether this should be "if (!sorEredmenyJo(haz))"
            if (oszlopEredmenyJo(house))
                return false;

            //Végigmegyek minden BLOKKon
            for (int bl = 0; bl < 9; bl++)
            {
                //Minden blokk vizsgálata elején törlöm haz értékeit
                house.Clear();

                //Végigmegyek a blokk elemein
                for (int i = bl - (bl % 3); i <= (bl - (bl % 3)) + 2; i++)
                {
                    for (int j = (bl % 3) * 3; j <= ((bl % 3) * 3) + 2; j++)
                    {
                        //Ha haz még nem tartalmazza az aktuális értéket, akkor hozzáadom
                        if (HouseContainsValueBeforeAdded(house, se.Exercise[0][i, j]))
                            //Ha már tartalmazza, akkor nem jó a megoldás
                            return false;
                    }
                }
            }

            //Sudoku-X esetén az átlók
            if (se.ExerciseType == SudokuType.SudokuX)
            {
                //Az átló vizsgálata elején törlöm haz értékeit
                house.Clear();

                //Végigmegyek a főátlón
                for (int r = 0; r < 9; r++)
                {
                    //Ha haz még nem tartalmazza az aktuális értéket, akkor hozzáadom
                    if (HouseContainsValueBeforeAdded(house, se.Exercise[0][r, r]))
                        //Ha már tartalmazza, akkor nem jó a megoldás
                        return false;
                }

                //Az átló vizsgálata elején törlöm haz értékeit
                house.Clear();

                //Végigmegyek a mellékátlón
                for (int r = 8; r >= 0; r--)
                {
                    //Ha haz még nem tartalmazza az aktuális értéket, akkor hozzáadom
                    if (HouseContainsValueBeforeAdded(house, se.Exercise[0][r, 8 - r]))
                        //Ha már tartalmazza, akkor nem jó a megoldás
                        return false;
                }
            }

            //Középpont Sudoku esetén a középső cellák
            if (se.ExerciseType == SudokuType.CenterDot)
            {
                //A vizsgálat elején törlöm haz értékeit
                house.Clear();

                //Végigmegyek a blokok középső celláin
                for (int r = 1; r <= 7; r += 3)
                {
                    for (int p = 1; p <= 7; p += 3)
                    {
                        //Ha haz még nem tartalmazza az aktuális értéket, akkor hozzáadom
                        if (HouseContainsValueBeforeAdded(house, se.Exercise[0][r, p]))
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
                    house.Clear();

                    //Ha haz még nem tartalmazza az aktuális értéket, akkor hozzáadom
                    if (HouseContainsValueBeforeAdded(house, se.Exercise[0][cell.Row, cell.Col]))
                        //Ha már tartalmazza, akkor nem jó a megoldás
                        return false;
                }
            }

            foreach (KeyValuePair<int, Cage> cage in se.Killer.Cages)
            {
                if (!SumOfNumbersIsCorrectInCage(cage.Key, cage.Value.Cells))
                    return false;
            }

            return true;
        }

        /// <summary>Megvizsgálja, hogy haz tartalmazza-e tomb adott [i, j] helyen levő értékét. Ha nem, akkor hozzáadja.</summary>
        /// <param name="house">A vizsgálandó ház</param>
        /// <param name="cellValue">A vizsgálandó érték</param>
        /// <returns>Ha a ház nem tartalmazza az adott értéket, akkor false, egyébként true.</returns>
        private bool HouseContainsValueBeforeAdded(List<int> house, int cellValue)
        {
            if (!house.Contains(cellValue))
            {
                house.Add(cellValue);
                return false;
            }

            return true;
        }

        private bool sorEredmenyJo(List<int> house)
        {
            return sorOszlopEredmenyJo(house, true);
        }

        private bool oszlopEredmenyJo(List<int> house)
        {
            return sorOszlopEredmenyJo(house, false);
        }

        /// <summary>Megvizsgálja az eredményt, hogy a sorok vagy oszlopok jól vannak-e kitöltve.</summary>
        /// <param name="house">Az aktuálisan vizsgálandó sor vagy oszlop.</param>
        /// <param name="isRowCheck">Azt mondja meg, hogy sort vagy oszlopot kell vizsgálni.</param>
        private bool sorOszlopEredmenyJo(List<int> house, bool isRowCheck)
        {
            for (int i = 0; i < 9; i++)
            {
                //Minden sor/oszlop vizsgálata elején törlöm haz értékeit
                house.Clear();

                //Végigmegyek a sor/oszlop elemein
                for (int j = 0; j < 9; j++)
                {
                    //Ha haz még nem tartalmazza az aktuális értéket, akkor hozzáadom
                    if (isRowCheck
                        ? HouseContainsValueBeforeAdded(house, se.Exercise[0][i, j])
                        : HouseContainsValueBeforeAdded(house, se.Exercise[0][j, i]))
                        //Ha már tartalmazza, akkor nem jó a megoldás
                        return false;
                }
            }

            return true;
        }

        /// <summary>Megvizsgálja, hogy a cageIndex ketrecbe eddig beírt számok összeg nagyobb-e a tényleges összegnél</summary>
        /// <param name="cageIndex">A vizsgálandó ketrec száma</param>
        /// <returns>Ha a számok aktuális összege nagyobb, mint a tényleges összeg, akkor false-szal térek vissza, egyébként true-val</returns>
        public static bool SumOfNumbersIsCorrectInCage(int cageIndex, List<Cell> cellsInCage)
        {
            int sumOfCellValuesInCage = cellsInCage.Sum(cell => se.Exercise[0][cell.Row, cell.Col]);
            return sumOfCellValuesInCage <= se.Killer.Cages[cageIndex].SumOfNumbers;
        }
    }
}