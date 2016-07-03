using static Sudoku.Controller.SudokuExercise;
using static Sudoku.Table.TableUtil;

namespace Sudoku.Controller.Finder
{
    class NotFillableItemFinder
    {
        private SudokuExercise se = SudokuExercise.Instance;

        //True if all the houses examined so far contain only occupied cells
        //TODO: this may not needed
        private bool isThereFullHouseAmongAll;
        //True if the currently examined house contains only occupied cells
        private bool isCurrentHouseFull;

        /// <summary>Megvizsgálja, hogy van-e olyan ház, ahol csak -1 értékek szerepelnek, a t szám viszont nem</summary>
        /// <param name="t">The index of the table to be examined</param>
        /// <returns>Ha van egyetlen egy olyan ház is, ahol a vizsgálat igaz értéket ad, akkor true, egyébként false</returns>
        public bool IsThereNotFillableHouseForNumber(int t)
        {
            isThereFullHouseAmongAll = false;
            isCurrentHouseFull = false;

            return IsThereNotFillable(HouseType.ROW, t)
                || IsThereNotFillable(HouseType.COLUMN, t)
                || IsThereNotFillableBlock(t);
        }

        private bool IsThereNotFillable(HouseType houseType, int t)
        {
            for (int i = 0; i < 9; i++)
            {
                isCurrentHouseFull = true;

                for (int j = 0; j < 9; j++)
                {
                    if (houseType == HouseType.ROW
                        ? IsThereNotOccupiedCell(t, i, j)
                        : IsThereNotOccupiedCell(t, j, i))
                        break;
                }

                isThereFullHouseAmongAll |= isCurrentHouseFull;

                if (isThereFullHouseAmongAll)
                    return true;
            }

            return false;
        }

        private bool IsThereNotFillableBlock(int num)
        {
            for (int bl = 0; bl < 9; bl++)
            {
                isCurrentHouseFull = true;

                for (int row = StartRowOfBlockByBlockIndex(bl); row <= EndRowOfBlockByBlockIndex(bl); row++)
                {
                    for (int col = StartColOfBlockByBlockIndex(bl); col <= EndColOfBlockByBlockIndex(bl); col++)
                    {
                        if (IsThereNotOccupiedCell(num, row, col))
                            break;
                    }

                    if (!isCurrentHouseFull)
                        break;
                }

                isThereFullHouseAmongAll |= isCurrentHouseFull;

                if (isThereFullHouseAmongAll)
                    return true;
            }
            return false;
        }

        /// <summary>Returns whether the given cell is not occupied.</summary>
        private bool IsThereNotOccupiedCell(int num, int row, int col)
        {
            if (se.Exercise[num][row, col] != OCCUPIED)
            {
                isCurrentHouseFull = false;
                return true;
            }

            return false;
        }

        private enum HouseType
        {
            ROW,
            COLUMN
        }
    }
}
