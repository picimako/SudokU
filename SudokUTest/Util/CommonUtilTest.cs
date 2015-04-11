using Sudoku;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace SudokUTest
{
    [TestClass()]
    public class CommonUtilTest
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
        ///Tests if the exercise is correct.
        ///</summary>
        [TestMethod()]
        public void IsExerciseCorrectTest()
        {
            TestExerciseGenerator generator = new TestExerciseGenerator();
            int[,] solution = generator.GenerateRandomExercise();
            
            bool expected = true;
            bool actual;
            actual = CommonUtil.IsExerciseCorrect(solution);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Tests if the exercise is not correct.
        ///</summary>
        [TestMethod()]
        public void IsExerciseNotCorrectTest()
        {
            TestExerciseGenerator generator = new TestExerciseGenerator();
            int[,] solution = generator.GenerateRandomExercise();
            Random random = new Random();
            solution[random.Next(0, 9), random.Next(0, 9)] = 0;

            bool expected = false;
            bool actual;
            actual = CommonUtil.IsExerciseCorrect(solution);
            Assert.AreEqual(expected, actual);
        }
    }
}
