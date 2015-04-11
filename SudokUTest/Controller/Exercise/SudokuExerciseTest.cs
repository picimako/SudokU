using Sudoku.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace SudokUTest
{
    
    
    /// <summary>
    ///This is a test class for SudokuExerciseTest and is intended
    ///to contain all SudokuExerciseTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SudokuExerciseTest
    {
        private TestContext testContextInstance;

        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for StartRowOfBlockByRow
        ///</summary>
        [TestMethod()]
        public void StartRowOfBlockByRowTest()
        {
            SudokuExercise_Accessor target = new SudokuExercise_Accessor();
            Dictionary<int, int> rowExpectedMap = new Dictionary<int, int>();
            rowExpectedMap.Add(0, 0);
            rowExpectedMap.Add(1, 0);
            rowExpectedMap.Add(4, 3);
            rowExpectedMap.Add(8, 6);
            foreach (KeyValuePair<int, int> item in rowExpectedMap)
            {
                int actual = target.StartRowOfBlockByRow(item.Key);
                Assert.AreEqual(item.Value, actual);
            }
        }

        /// <summary>
        ///A test for StartRowOfBlockByBlockIndex
        ///</summary>
        [TestMethod()]
        public void StartRowOfBlockByBlockIndexTest()
        {
            SudokuExercise_Accessor target = new SudokuExercise_Accessor();
            Dictionary<int, int> blockIndexExpectedRow = new Dictionary<int, int>();
            blockIndexExpectedRow.Add(0, 0);
            blockIndexExpectedRow.Add(3, 3);
            blockIndexExpectedRow.Add(8, 6);
            foreach (KeyValuePair<int, int> item in blockIndexExpectedRow)
            {
                int actualRow = target.StartRowOfBlockByBlockIndex(item.Key);
                Assert.AreEqual(item.Value, actualRow);
            }            
        }

        /// <summary>
        ///A test for StartColOfBlockByBlockIndex
        ///</summary>
        [TestMethod()]
        public void StartColOfBlockByBlockIndexTest()
        {
            SudokuExercise_Accessor target = new SudokuExercise_Accessor();
            Dictionary<int, int> blockIndexExpectedCol = new Dictionary<int, int>();
            blockIndexExpectedCol.Add(0, 0);
            blockIndexExpectedCol.Add(4, 3);
            blockIndexExpectedCol.Add(8, 6);
            foreach (KeyValuePair<int, int> item in blockIndexExpectedCol)
            {
                int actualCol = target.StartRowOfBlockByBlockIndex(item.Key);
                Assert.AreEqual(item.Value, actualCol);
            }  
        }

        /// <summary>
        ///A test for BlockIndexByCellIndeces
        ///</summary>
        [TestMethod()]
        public void BlockIndexByCellIndecesTest()
        {
            SudokuExercise_Accessor target = new SudokuExercise_Accessor();
            Dictionary<KeyValuePair<int, int>, int> cellIndecesExpectedBlockIndex = new Dictionary<KeyValuePair<int, int>, int>();
            cellIndecesExpectedBlockIndex.Add(new KeyValuePair<int, int>(1, 7), 2);
            cellIndecesExpectedBlockIndex.Add(new KeyValuePair<int, int>(3, 5), 4);
            cellIndecesExpectedBlockIndex.Add(new KeyValuePair<int, int>(6, 1), 6);
            cellIndecesExpectedBlockIndex.Add(new KeyValuePair<int, int>(8, 4), 7);
            foreach (KeyValuePair<KeyValuePair<int, int>, int> item in cellIndecesExpectedBlockIndex)
            {
                KeyValuePair<int, int> cellIndeces = item.Key;
                int actualBlock = target.BlockIndexByCellIndeces(cellIndeces.Key, cellIndeces.Value);
                Assert.AreEqual(item.Value, actualBlock);
            }
        }

        /// <summary>
        ///A test for RecalculateFirstEmptyCell
        ///</summary>
        [TestMethod()]
        public void RecalculateFirstEmptyCellTest()
        {
            SudokuExercise_Accessor target = new SudokuExercise_Accessor();
            TestExerciseGenerator generator = new TestExerciseGenerator();
            //Create random exercise with some zero values
            int[,] exercise = generator.GenerateRandomExerciseWithZeroValues();
            target.InitExercise();
            target.Exercise[0] = exercise;
            //Set firstEmptyCell and numberOfEmptyCells according to the added 0 values
            //Save the position of the second 0-value cell
            int originalFirstEmptyCell = -1;
            int originalSecondEmptyCell = -1;
            int numberOfEmptyCells = 0;
            for (int position = 0; position < 81; position++)
            {
                if (exercise[position / 9, position % 9] == 0)
                {
                    numberOfEmptyCells++;
                    if (originalFirstEmptyCell == -1)
                        originalFirstEmptyCell = position;
                    if (originalSecondEmptyCell == -1 && position > originalFirstEmptyCell)
                        originalSecondEmptyCell = position;
                }
            }
            target.FirstEmptyCell = originalFirstEmptyCell;
            target.NumberOfEmptyCells = numberOfEmptyCells;

            //Fill in the first 0-value cell with a number from 1 to 9
            Random random = new Random();
            exercise[originalFirstEmptyCell / 9, originalFirstEmptyCell % 9] = random.Next(1, 9);

            //Recalculate
            target.RecalculateFirstEmptyCell(originalFirstEmptyCell);

            //Check if the first 0-value cell is the same as the second 0-value cell in the original exercise
            Assert.AreEqual(originalSecondEmptyCell, target.FirstEmptyCell);
        }
    }
}
