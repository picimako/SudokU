using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SudokUTest
{
    public class TestExerciseGenerator
    {
        public int[,] GenerateRandomExercise()
        {
            Random random = new Random();
            int[,] exercise = new int[9, 9];
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    exercise[i, j] = random.Next(1, 9);
            return exercise;
        }

        public int[,] GenerateRandomExerciseWithZeroValues()
        {
            Random random = new Random();
            int[,] exercise = GenerateRandomExercise();

            for (int i = 0; i < random.Next(1, 10); i++)
                exercise[random.Next(0, 9), random.Next(0, 9)] = 0;

            return exercise;
        }

        public int[,] ConvertStringToIntArray(string sArray)
        {

        }
    }
}
