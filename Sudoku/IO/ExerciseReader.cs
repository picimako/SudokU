using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Sudoku.Controller;

namespace Sudoku
{
    class ExerciseReader
    {
        /// <summary> A megadott fájlból beolvassa az értékeket. </summary>
        /// <param name="filePath"> A beolvasandó fájl útvonala </param>
        /// <returns>false-szal tér vissza, ha nincs megoldandó feladat vagy a beolvasás sikertelen volt, egyébként true-val</returns>
        public static bool ReadSudoku(string filePath)
        {
            SudokuExercise se = SudokuExercise.get;
            using (StreamReader reader = new StreamReader(filePath))
            {
                try
                {
                    string line;
                    int indexOfRowToRead = 0;
                    string[] numbers;

                    while ((line = reader.ReadLine()) != null)
                    {
                        numbers = line.Split(' ');
                        for (int j = 0; j < 9; j++)
                        {
                            /* Ha a konvertálás nem sikerült, akkor az üres cellának számít,
                             * ezért növelni kell az üres cellák számát tároló változó értékét.*/
                            if (!(Int32.TryParse(numbers[j], out se.Exercise[0][indexOfRowToRead, j])))
                                se.NumberOfEmptyCells++;
                        }

                        indexOfRowToRead++;
                    }

                    return true;
                }
                catch (EndOfStreamException)
                {
                    return false;
                }
                catch (IndexOutOfRangeException)
                {
                    return false;
                }
            }
        }

        /// <summary>Beolvassa a megadott fájlból a killer típusú feladatot</summary>
        /// <param name="filePath">A beolvasandó fájl</param>
        /// <returns>Ha hiba volt, akkor false, ha minden rendben volt a beolvasás során, akkor true</returns>
        public static bool ReadKillerSudoku(string filePath)
        {
            SudokuExercise se = SudokuExercise.get;
            using (StreamReader reader = new StreamReader(filePath))
            {
                try
                {
                    string line;
                    int indexOfRowToRead = 0;
                    int numberOfCages;
                    int sumOfNumbersInCage;
                    string[] numbers;

                    //Beolvasom, hogy mennyi ketrecem van
                    Int32.TryParse(reader.ReadLine(), out numberOfCages);
                    //Ennyi ketrecösszeget olvasok be
                    for (int k = 1; k <= numberOfCages; k++)
                    {
                        //A ketrecben levő számok összegét beolvasom, és számmá alakítom
                        Int32.TryParse(reader.ReadLine(), out sumOfNumbersInCage);
                        //Elmentem az aktuális ketrecszám, ketrecösszeg párost
                        se.Killer.Cages.Add(k, new Cage(sumOfNumbersInCage));
                    }

                    if (!IsSumOfValuesCorrect())
                        return false;

                    while ((line = reader.ReadLine()) != null)
                    {
                        numbers = line.Split(' ');
                        for (int j = 0; j < 9; j++)
                        {
                            /* Ha a konvertálás nem sikerült, akkor hibás a felosztás vagy 0 értékű a ketrec,
                             * akkor nem jó a feladat, visszatérek false-szal.*/
                            int currentCageNumber = -1;
                            if (!(Int32.TryParse(numbers[j], out currentCageNumber)) || currentCageNumber == 0)
                                return false;
                            se.Killer.Exercise[indexOfRowToRead, j].CageIndex = currentCageNumber;
                            se.Killer.Cages[currentCageNumber].Cells.Add(new Pair(indexOfRowToRead, j));
                        }

                        indexOfRowToRead++;
                    }
                    return true;
                }
                catch (EndOfStreamException)
                {
                    return false;
                }
                catch (IndexOutOfRangeException)
                {
                    return false;
                }
            }
        }

        private static bool IsSumOfValuesCorrect()
        {
            SudokuExercise se = SudokuExercise.get;
            int sumOfNumbers = 0;
            foreach (Cage cage in se.Killer.Cages.Values)
                sumOfNumbers += cage.SumOfNumbers;

            return sumOfNumbers == 405;
        }
    }
}
