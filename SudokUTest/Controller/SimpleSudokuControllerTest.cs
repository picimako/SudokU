using Sudoku.Controller;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using SudokUTest.Helpers;
using System.Collections.Generic;

namespace SudokUTest
{
    
    
    /// <summary>
    ///This is a test class for SimpleSudokuControllerTest and is intended
    ///to contain all SimpleSudokuControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SimpleSudokuControllerTest
    {
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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
        ///A test for ColumnContainsValue
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Sudoku.exe")]
        public void ColumnContainsValueTest()
        {
            SimpleSudokuController_Accessor target = new SimpleSudokuController_Accessor();
            TestExerciseGenerator generator = new TestExerciseGenerator();
            CellSearcher searcher = new CellSearcher();
            List<int[,]> exercises = new List<int[,]>();
            exercises.Add(generator.ConvertStringToIntArray("598314762764295801321786495179538246453162987286479513635847129817923654942651378"));
            exercises.Add(generator.ConvertStringToIntArray("598314762764295801321786495109538246453162987286479513635847129817903654942651378"));
            foreach (int[,] exercise in exercises)
            {
                target.se.InitExercise();
                target.se.Exercise[0] = exercise;
                //TODO: get an explicit empty cell
                KeyValuePair<int, int> emptyCell = searcher.GetRandomEmptyCellFromExercise(exercise);
                //TODO: define numbers based on the empty cell selected
                List<int> values = new List<int>() { 3, 5, 6 };
                bool expected = true;
                bool actual;
                foreach (int value in values)
                {
                    actual = target.ColumnContainsValue(emptyCell.Key, emptyCell.Value, value);
                    Assert.AreEqual(expected, actual);
                }
            }
        }

        /// <summary>
        ///A test for ColumnContainsValue
        ///</summary>
        //[TestMethod()]
        //[DeploymentItem("Sudoku.exe")]
        //public void ColumnDoesntContainValueTest()
        //{
        //    //Generate an exercise with some empty cells, or give an explicit exercise to test
        //    //Run the ColumnContainsValue method for one of the empty cell's indeces
        //    SimpleSudokuController_Accessor target = new SimpleSudokuController_Accessor();
        //    int rowOfCurrentCell = 0;
        //    int colOfCurrentCell = 0;
        //    int value = 0;
        //    bool expected = false;
        //    bool actual;
        //    actual = target.ColumnContainsValue(rowOfCurrentCell, colOfCurrentCell, value);
        //    Assert.AreEqual(expected, actual);
        //}

        ///// <summary>
        /////A test for BlockContainsValue
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("Sudoku.exe")]
        //public void BlockContainsValueTest()
        //{
        //    SimpleSudokuController_Accessor target = new SimpleSudokuController_Accessor();
        //    int rowOfCurrentCell = 0;
        //    int colOfCurrentCell = 0;
        //    int value = 0;
        //    bool expected = false;
        //    bool actual;
        //    actual = target.BlockContainsValue(rowOfCurrentCell, colOfCurrentCell, value);
        //    Assert.AreEqual(expected, actual);
        //}

        ///// <summary>
        /////A test for RowContainsValue
        /////</summary>
        //[TestMethod()]
        //[DeploymentItem("Sudoku.exe")]
        //public void RowContainsValueTest()
        //{
        //    SimpleSudokuController_Accessor target = new SimpleSudokuController_Accessor();
        //    int rowOfCurrentCell = 0;
        //    int colOfCurrentCell = 0;
        //    int value = 0;
        //    bool expected = false;
        //    bool actual;
        //    actual = target.RowContainsValue(rowOfCurrentCell, colOfCurrentCell, value);
        //    Assert.AreEqual(expected, actual);
        //}
    }
}
