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
                    //Ebbe olvasok be egy sort a fájlból
                    string line;
                    //Sorindex az értékek eltárolásához
                    int indexOfRowToRead = 0;
                    //Ebbe a tömbbe darabolom szét a beolvasott sort
                    string[] numbers;

                    //Amíg a beolvasott sor nem üres
                    while ((line = reader.ReadLine()) != null)
                    {
                        //A sor szétdarabolása szóközök mentén
                        numbers = line.Split(' ');
                        for (int j = 0; j < 9; j++)
                        {
                            /* Ha a konvertálás nem sikerült, akkor az üres cellának számít,
                             * ezért növelni kell az üres cellák számát tároló változó értékét.*/
                            if (!(Int32.TryParse(numbers[j], out se.Exercise[0][indexOfRowToRead, j])))
                                se.NumberOfEmptyCells++;
                        }

                        //Eltároltam az adott sor értékeit, lépek a következő sorba
                        indexOfRowToRead++;
                    }

                    //Ha minden rendben volt, akkor true-val térek vissza
                    return true;
                }
                //Akkor dobódik, amikor az adatfolyam vége után próbálunk meg adatot olvasni
                catch (EndOfStreamException)
                {
                    return false;
                }
                //Ha valamelyik sorba több mindent írtunk, mint amennyi dolog megengedett, akkor nem jól adtuk meg a feladatot
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
            se.InitKillerExercise();
            using (StreamReader reader = new StreamReader(filePath))
            {
                try
                {
                    //Ebbe olvasok be egy sort a fájlból
                    string line;
                    //Sorindex az értékek eltárolásához
                    int indexOfRowToRead = 0;
                    int numberOfCages;
                    int sumOfNumbersInCage;
                    int sum = 0;
                    //Ebbe a tömbbe darabolom szét a beolvasott sort
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

                    //Összeadom a ketrecösszegeket
                    foreach (Cage cage in se.Killer.Cages.Values)
                        sum += cage.SumOfNumbers;

                    //Ha a ketrecösszeg
                    if (sum != 405)
                        return false;

                    while ((line = reader.ReadLine()) != null)
                    {
                        //A sor szétdarabolása szóközök mentén
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

                        //Eltároltam az adott sor értékeit, lépek a következő sorba
                        indexOfRowToRead++;
                    }
                    //Ha minden rendben volt, akkor true-val térek vissza
                    return true;
                }
                //Akkor dobódik, amikor az adatfolyam vége után próbálunk meg adatot olvasni
                catch (EndOfStreamException)
                {
                    return false;
                }
                //Ha valamelyik sorba több mindent írtunk, mint amennyi dolog megengedett, akkor nem jól adtuk meg a feladatot, tehát hamissal
                catch (IndexOutOfRangeException)
                {
                    return false;
                }
            }
        }
    }
}
