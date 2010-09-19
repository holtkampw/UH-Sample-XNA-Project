using AStartTest.TileSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AStartTest.Vectors;
using System.Windows.Forms;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for TileTest and is intended
    ///to contain all TileTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TileTest
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
        ///A test for Tile Constructor
        ///</summary>
        [TestMethod()]
        public void TileConstructorTest()
        {
            int id = 0; // TODO: Initialize to an appropriate value
            Vector2 position = null; // TODO: Initialize to an appropriate value
            Vector2 size = null; // TODO: Initialize to an appropriate value
            Panel panel = null; // TODO: Initialize to an appropriate value
            TileType tileType = new TileType(); // TODO: Initialize to an appropriate value
            Tile target = new Tile(id, position, size, panel, tileType);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Tile Constructor
        ///</summary>
        [TestMethod()]
        public void TileConstructorTest1()
        {
            Tile target = new Tile();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Tile Constructor
        ///</summary>
        [TestMethod()]
        public void TileConstructorTest2()
        {
            int id = 0; // TODO: Initialize to an appropriate value
            Vector2 position = null; // TODO: Initialize to an appropriate value
            Vector2 size = null; // TODO: Initialize to an appropriate value
            Panel panel = null; // TODO: Initialize to an appropriate value
            Tile target = new Tile(id, position, size, panel);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest()
        {
            Tile target = new Tile(); // TODO: Initialize to an appropriate value
            object obj = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Equals(obj);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetTileType
        ///</summary>
        [TestMethod()]
        public void GetTileTypeTest()
        {
            Tile target = new Tile(); // TODO: Initialize to an appropriate value
            TileType expected = new TileType(); // TODO: Initialize to an appropriate value
            TileType actual;
            actual = target.GetTileType();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsGoal
        ///</summary>
        [TestMethod()]
        public void IsGoalTest()
        {
            Tile target = new Tile(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsGoal();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsStart
        ///</summary>
        [TestMethod()]
        public void IsStartTest()
        {
            Tile target = new Tile(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsStart();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for IsWalkable
        ///</summary>
        [TestMethod()]
        public void IsWalkableTest()
        {
            Tile target = new Tile(); // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.IsWalkable();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for SetTileType
        ///</summary>
        [TestMethod()]
        public void SetTileTypeTest()
        {
            Tile target = new Tile(); // TODO: Initialize to an appropriate value
            TileType tileType = new TileType(); // TODO: Initialize to an appropriate value
            target.SetTileType(tileType);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ToString
        ///</summary>
        [TestMethod()]
        public void ToStringTest()
        {
            Tile target = new Tile(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            actual = target.ToString();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for ID
        ///</summary>
        [TestMethod()]
        public void IDTest()
        {
            Tile target = new Tile(); // TODO: Initialize to an appropriate value
            int actual;
            actual = target.ID;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Panel
        ///</summary>
        [TestMethod()]
        public void PanelTest()
        {
            Tile target = new Tile(); // TODO: Initialize to an appropriate value
            Panel actual;
            actual = target.Panel;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Position
        ///</summary>
        [TestMethod()]
        public void PositionTest()
        {
            Tile target = new Tile(); // TODO: Initialize to an appropriate value
            Vector2 actual;
            actual = target.Position;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Size
        ///</summary>
        [TestMethod()]
        public void SizeTest()
        {
            Tile target = new Tile(); // TODO: Initialize to an appropriate value
            Vector2 actual;
            actual = target.Size;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for TileType
        ///</summary>
        [TestMethod()]
        public void TileTypeTest()
        {
            Tile target = new Tile(); // TODO: Initialize to an appropriate value
            TileType actual;
            actual = target.TileType;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
