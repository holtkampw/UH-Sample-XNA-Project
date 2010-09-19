using AStartTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AStartTest.TileSystem;
using System.Collections.Generic;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for AStarTest and is intended
    ///to contain all AStarTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AStarTest
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
        ///A test for AStar Constructor
        ///</summary>
        [TestMethod()]
        public void AStarConstructorTest()
        {
            TileMap tileMap = null; // TODO: Initialize to an appropriate value
            AStar target = new AStar(tileMap);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for AddNeighborNodesToOpenList
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AStartTest.exe")]
        public void AddNeighborNodesToOpenListTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            AStar_Accessor target = new AStar_Accessor(param0); // TODO: Initialize to an appropriate value
            AStar_Accessor.Node currentNode = null; // TODO: Initialize to an appropriate value
            List<Tile> neighborTiles = null; // TODO: Initialize to an appropriate value
            target.AddNeighborNodesToOpenList(currentNode, neighborTiles);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for FindPath
        ///</summary>
        [TestMethod()]
        public void FindPathTest()
        {
            TileMap tileMap = null; // TODO: Initialize to an appropriate value
            AStar target = new AStar(tileMap); // TODO: Initialize to an appropriate value
            List<Tile> expected = null; // TODO: Initialize to an appropriate value
            List<Tile> actual;
            actual = target.FindPath();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDistanceBetweenTiles
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AStartTest.exe")]
        public void GetDistanceBetweenTilesTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            AStar_Accessor target = new AStar_Accessor(param0); // TODO: Initialize to an appropriate value
            Tile tile1 = null; // TODO: Initialize to an appropriate value
            Tile tile2 = null; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = target.GetDistanceBetweenTiles(tile1, tile2);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetDistanceToGoal
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AStartTest.exe")]
        public void GetDistanceToGoalTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            AStar_Accessor target = new AStar_Accessor(param0); // TODO: Initialize to an appropriate value
            Tile tile = null; // TODO: Initialize to an appropriate value
            float expected = 0F; // TODO: Initialize to an appropriate value
            float actual;
            actual = target.GetDistanceToGoal(tile);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetLowestCostNodeFromOpenNodes
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AStartTest.exe")]
        public void GetLowestCostNodeFromOpenNodesTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            AStar_Accessor target = new AStar_Accessor(param0); // TODO: Initialize to an appropriate value
            AStar_Accessor.Node expected = null; // TODO: Initialize to an appropriate value
            AStar_Accessor.Node actual;
            actual = target.GetLowestCostNodeFromOpenNodes();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetWalkableNeighborsNotOnClosedList
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AStartTest.exe")]
        public void GetWalkableNeighborsNotOnClosedListTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            AStar_Accessor target = new AStar_Accessor(param0); // TODO: Initialize to an appropriate value
            AStar_Accessor.Node currentNode = null; // TODO: Initialize to an appropriate value
            List<Tile> expected = null; // TODO: Initialize to an appropriate value
            List<Tile> actual;
            actual = target.GetWalkableNeighborsNotOnClosedList(currentNode);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Iterate
        ///</summary>
        [TestMethod()]
        public void IterateTest()
        {
            TileMap tileMap = null; // TODO: Initialize to an appropriate value
            AStar target = new AStar(tileMap); // TODO: Initialize to an appropriate value
            Tile currentTile = null; // TODO: Initialize to an appropriate value
            Tile currentTileExpected = null; // TODO: Initialize to an appropriate value
            List<Tile> open = null; // TODO: Initialize to an appropriate value
            List<Tile> openExpected = null; // TODO: Initialize to an appropriate value
            List<Tile> closed = null; // TODO: Initialize to an appropriate value
            List<Tile> closedExpected = null; // TODO: Initialize to an appropriate value
            target.Iterate(ref currentTile, ref open, ref closed);
            Assert.AreEqual(currentTileExpected, currentTile);
            Assert.AreEqual(openExpected, open);
            Assert.AreEqual(closedExpected, closed);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for TransformToNode
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AStartTest.exe")]
        public void TransformToNodeTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            AStar_Accessor target = new AStar_Accessor(param0); // TODO: Initialize to an appropriate value
            Tile tile = null; // TODO: Initialize to an appropriate value
            AStar_Accessor.Node parentNode = null; // TODO: Initialize to an appropriate value
            AStar_Accessor.Node expected = null; // TODO: Initialize to an appropriate value
            AStar_Accessor.Node actual;
            actual = target.TransformToNode(tile, parentNode);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
