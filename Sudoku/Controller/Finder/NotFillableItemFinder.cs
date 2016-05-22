using static Sudoku.Table.TableUtil;

namespace Sudoku.Controller.Finder
{
    class NotFillableItemFinder
    {
        SudokuExercise se = SudokuExercise.get;

        /// <summary>Megvizsgálja, hogy van-e olyan ház, ahol csak -1 értékek szerepelnek, a t szám viszont nem</summary>
        /// <param name="t">A vizsgálandó tábla indexe</param>
        /// <returns>Ha van egyetlen egy olyan ház is, ahol a vizsgálat igaz értéket ad, akkor true, egyébként false</returns>
        public bool IsThereNotFillableHouseForNumber(int t)
        {
            /* van: az eddig vizsgált összes cella teliségét tárolja
             * teli: true az éppen vizsgált ház csak -1-es értéket tartalmaz, egyébként false*/
            bool van = false, teli = false;

            return IsThereNotFillableRow(t, ref van, ref teli) || IsThereNotFillableColumn(t, ref van, ref teli)
                || IsThereNotFillableBlock(t, ref van, ref teli);
        }

        private bool IsThereNotFillableBlock(int num, ref bool van, ref bool teli)
        {
            for (int bl = 0; bl < 9; bl++)
            {
                //Csak -1-esek vannak
                teli = true;

                for (int row = StartRowOfBlockByBlockIndex(bl); row <= EndRowOfBlockByBlockIndex(bl); row++)
                {
                    for (int col = StartColOfBlockByBlockIndex(bl); col <= EndColOfBlockByBlockIndex(bl); col++)
                    {
                        //Ha nem csak -1-es van, akkor teli értéke false lesz, és befejezem a blokk vizsgálatát
                        if (IsThereOccupiedCell(num, row, col, ref teli))
                            break;
                    }

                    if (!teli)
                        break;
                }

                //van értéke van|teli lesz
                van |= teli;

                //Ha van olyan blokk, ami csak -1-eseket tartalmaz, akkor visszatérek true-val
                if (van)
                    return true;
            }
            return false;
        }

        public bool IsThereNotFillableRow(int num, ref bool van, ref bool teli)
        {
            return IsThereNotFillableRowOrColumn(num, ref van, ref teli, true);
        }

        public bool IsThereNotFillableColumn(int num, ref bool van, ref bool teli)
        {
            return IsThereNotFillableRowOrColumn(num, ref van, ref teli, false);
        }

        private bool IsThereNotFillableRowOrColumn(int t, ref bool van, ref bool teli, bool row)
        {
            //Végigmegyek a soron/oszlopon
            for (int i = 0; i < 9; i++)
            {
                //Csak -1-esek vannak
                teli = true;

                //Végigmegyek az oszlopon/soron
                for (int j = 0; j < 9; j++)
                {
                    //Ha nem csak -1-es van, akkor teli értéke false lesz, és befejezem a sor vizsgálatát
                    if (row ? IsThereOccupiedCell(t, i, j, ref teli) : IsThereOccupiedCell(t, j, i, ref teli))
                        break;
                }

                //van értéke van|teli lesz
                van |= teli;

                //Ha van olyan sor, ami csak -1-eseket tartalmaz, akkor visszatérek true-val
                if (van)
                    return true;
            }

            return false;
        }

        private bool IsThereOccupiedCell(int num, int row, int col, ref bool teli)
        {
            //Ha nem csak -1-es van, akkor teli értéke false lesz, és befejezem az oszlop vizsgálatát
            if (se.Exercise[num][row, col] != se.OCCUPIED)
            {
                teli = false;
                return true;
            }

            return false;
        }
    }
}
