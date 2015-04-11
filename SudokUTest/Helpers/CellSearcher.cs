using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokUTest.Helpers
{
    class CellSearcher
    {
        //TODO: debug to see what happens, and if it is correct this way
        public KeyValuePair<int, int> GetRandomEmptyCellFromExercise(int[,] exercise)
        {
            List<int> cells = exercise.OfType<int>().ToList<int>();
            var cellList = cells.Select(sublist => sublist).Where(item => item == 0);
            int numberOfEmptyCells = cellList.Count<int>();
            int nthEmptyCell = new Random().Next(1, numberOfEmptyCells);
            int currentEmptyCellIndex = 0;
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    if (exercise[i, j] == 0 && ++currentEmptyCellIndex == nthEmptyCell)
                    {
                        return new KeyValuePair<int,int>(i, j);
                    }
                }
            }

            return new KeyValuePair<int,int>(-1, -1);
        }
    }
}
