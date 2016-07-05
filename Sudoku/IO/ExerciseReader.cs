using System;
using System.IO;
using System.Linq;
using Sudoku.Cells;
using Sudoku.Controller;

namespace Sudoku
{
    class ExerciseReader
    {
        private static SudokuExercise se = SudokuExercise.Instance;

        /// <summary>Reads the exercise value from the given file.</summary>
        /// <param name="filePath">The file path to read from.</param>
        /// <returns>False if there is no exercise to solve or reading was unsuccessful.</returns>
        public static bool ReadSudoku(string filePath)
        {
            string line;
            int indexOfRowRead = 0;
            string[] numbers;

            using (StreamReader reader = new StreamReader(filePath))
            {
                try
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        numbers = line.Split(' ');
                        for (int j = 0; j < 9; j++)
                        {
                            //If the conversion can't happen that is an empty cell.
                            if (!(Int32.TryParse(numbers[j], out se.Exercise[0][indexOfRowRead, j])))
                                se.NumberOfEmptyCells++;
                        }

                        indexOfRowRead++;
                    }

                    return true;
                }
                catch (Exception ex) when (ex is EndOfStreamException || ex is IndexOutOfRangeException)
                {
                    return false;
                }
            }
        }

        /// <summary>Reads the Killer exercise value from the given file.</summary>
        /// <param name="filePath">The file path to read from.</param>
        /// <returns>False if there was any problem with the exercise or the reading was unsuccessful.</returns>
        public static bool ReadKillerSudoku(string filePath)
        {
            string line;
            int indexOfRowToRead = 0;
            int numberOfCages;
            int sumOfNumbersInCage;
            string[] numbers;

            using (StreamReader reader = new StreamReader(filePath))
            {
                try
                {
                    Int32.TryParse(reader.ReadLine(), out numberOfCages);
                    for (int cage = 1; cage <= numberOfCages; cage++)
                    {
                        Int32.TryParse(reader.ReadLine(), out sumOfNumbersInCage);
                        se.Killer.Cages.Add(cage, new Cage(sumOfNumbersInCage));
                    }

                    if (!IsSumOfValuesCorrect())
                        return false;

                    while ((line = reader.ReadLine()) != null)
                    {
                        numbers = line.Split(' ');
                        for (int j = 0; j < 9; j++)
                        {
                            /* If the conversion can't happen, then the killer layout is incorrect or the cage has 0 value,
                             * then the exercise is incorrect.*/
                            int currentCageNumber = -1;
                            if (!(Int32.TryParse(numbers[j], out currentCageNumber)) || currentCageNumber == 0)
                                return false;
                            se.Killer.CageIndeces[indexOfRowToRead, j] = currentCageNumber;
                            se.Killer.Cages[currentCageNumber].Cells.Add(new Cell(indexOfRowToRead, j));
                        }

                        indexOfRowToRead++;
                    }
                    return true;
                }
                catch (Exception ex) when (ex is EndOfStreamException || ex is IndexOutOfRangeException)
                {
                    return false;
                }
            }
        }

        private static bool IsSumOfValuesCorrect()
        {
            return 405 == se.Killer.Cages.Values.Sum(cage => cage.SumOfNumbers);
        }
    }
}
