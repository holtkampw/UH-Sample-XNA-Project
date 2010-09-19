using AStartTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AStartTest.TileSystem;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for AStar_NodeTest and is intended
    ///to contain all AStar_NodeTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AStar_NodeTest
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
        ///A test for Node Constructor
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AStartTest.exe")]
        public void AStar_NodeConstructorTest()
        {
            Tile parentTile = null; // TODO: Initialize to an appropriate value
            Tile tile = null; // TODO: Initialize to an appropriate value
            float overallCost = 0F; // TODO: Initialize to an appropriate value
            float initialCost = 0F; // TODO: Initialize to an appropriate value
            float goalCost = 0F; // TODO: Initialize to an appropriate value
            AStar_Accessor.Node target = new AStar_Accessor.Node(parentTile, tile, overallCost, initialCost, goalCost);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Node Constructor
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AStartTest.exe")]
        public void AStar_NodeConstructorTest1()
        {
            Tile tile = null; // TODO: Initialize to an appropriate value
            AStar_Accessor.Node target = new AStar_Accessor.Node(tile);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Node Constructor
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AStartTest.exe")]
        public void AStar_NodeConstructorTest2()
        {
            AStar_Accessor.Node target = new AStar_Accessor.Node();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
