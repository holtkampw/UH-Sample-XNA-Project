using AStartTest.TileSystem;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using AStartTest.Vectors;
using System.Windows.Forms;
using System.Collections.Generic;

namespace TestProject1
{
    
    
    /// <summary>
    ///This is a test class for TileMapTest and is intended
    ///to contain all TileMapTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TileMapTest
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
        ///A test for TileMap Constructor
        ///</summary>
        [TestMethod()]
        public void TileMapConstructorTest()
        {
            Vector2 position = null; // TODO: Initialize to an appropriate value
            Vector2 numTiles = null; // TODO: Initialize to an appropriate value
            Vector2 tileSize = null; // TODO: Initialize to an appropriate value
            IList<Panel> panels = null; // TODO: Initialize to an appropriate value
            TileMap target = new TileMap(position, numTiles, tileSize, panels);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for ClearPath
        ///</summary>
        [TestMethod()]
        public void ClearPathTest()
        {
            Vector2 position = null; // TODO: Initialize to an appropriate value
            Vector2 numTiles = null; // TODO: Initialize to an appropriate value
            Vector2 tileSize = null; // TODO: Initialize to an appropriate value
            IList<Panel> panels = null; // TODO: Initialize to an appropriate value
            TileMap target = new TileMap(position, numTiles, tileSize, panels); // TODO: Initialize to an appropriate value
            target.ClearPath();
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GetTileFromPanel
        ///</summary>
        [TestMethod()]
        public void GetTileFromPanelTest()
        {
            Vector2 position = null; // TODO: Initialize to an appropriate value
            Vector2 numTiles = null; // TODO: Initialize to an appropriate value
            Vector2 tileSize = null; // TODO: Initialize to an appropriate value
            IList<Panel> panels = null; // TODO: Initialize to an appropriate value
            TileMap target = new TileMap(position, numTiles, tileSize, panels); // TODO: Initialize to an appropriate value
            Panel panel = null; // TODO: Initialize to an appropriate value
            Tile expected = null; // TODO: Initialize to an appropriate value
            Tile actual;
            actual = target.GetTileFromPanel(panel);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetTileFromPos
        ///</summary>
        [TestMethod()]
        public void GetTileFromPosTest()
        {
            Vector2 position = null; // TODO: Initialize to an appropriate value
            Vector2 numTiles = null; // TODO: Initialize to an appropriate value
            Vector2 tileSize = null; // TODO: Initialize to an appropriate value
            IList<Panel> panels = null; // TODO: Initialize to an appropriate value
            TileMap target = new TileMap(position, numTiles, tileSize, panels); // TODO: Initialize to an appropriate value
            Vector2 position1 = null; // TODO: Initialize to an appropriate value
            Tile expected = null; // TODO: Initialize to an appropriate value
            Tile actual;
            actual = target.GetTileFromPos(position1);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetTileFromType
        ///</summary>
        [TestMethod()]
        public void GetTileFromTypeTest()
        {
            Vector2 position = null; // TODO: Initialize to an appropriate value
            Vector2 numTiles = null; // TODO: Initialize to an appropriate value
            Vector2 tileSize = null; // TODO: Initialize to an appropriate value
            IList<Panel> panels = null; // TODO: Initialize to an appropriate value
            TileMap target = new TileMap(position, numTiles, tileSize, panels); // TODO: Initialize to an appropriate value
            TileType tileType = new TileType(); // TODO: Initialize to an appropriate value
            Tile expected = null; // TODO: Initialize to an appropriate value
            Tile actual;
            actual = target.GetTileFromType(tileType);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetTileNeighbor
        ///</summary>
        [TestMethod()]
        public void GetTileNeighborTest()
        {
            Vector2 position = null; // TODO: Initialize to an appropriate value
            Vector2 numTiles = null; // TODO: Initialize to an appropriate value
            Vector2 tileSize = null; // TODO: Initialize to an appropriate value
            IList<Panel> panels = null; // TODO: Initialize to an appropriate value
            TileMap target = new TileMap(position, numTiles, tileSize, panels); // TODO: Initialize to an appropriate value
            Tile tile = null; // TODO: Initialize to an appropriate value
            NeighborTile neighborTile = new NeighborTile(); // TODO: Initialize to an appropriate value
            Tile expected = null; // TODO: Initialize to an appropriate value
            Tile actual;
            actual = target.GetTileNeighbor(tile, neighborTile);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetWalkableNeighbors
        ///</summary>
        [TestMethod()]
        public void GetWalkableNeighborsTest()
        {
            Vector2 position = null; // TODO: Initialize to an appropriate value
            Vector2 numTiles = null; // TODO: Initialize to an appropriate value
            Vector2 tileSize = null; // TODO: Initialize to an appropriate value
            IList<Panel> panels = null; // TODO: Initialize to an appropriate value
            TileMap target = new TileMap(position, numTiles, tileSize, panels); // TODO: Initialize to an appropriate value
            Tile tile = null; // TODO: Initialize to an appropriate value
            Dictionary<int, Tile> exclude = null; // TODO: Initialize to an appropriate value
            List<Tile> expected = null; // TODO: Initialize to an appropriate value
            List<Tile> actual;
            actual = target.GetWalkableNeighbors(tile, exclude);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetWalkableNeighbors
        ///</summary>
        [TestMethod()]
        public void GetWalkableNeighborsTest1()
        {
            Vector2 position = null; // TODO: Initialize to an appropriate value
            Vector2 numTiles = null; // TODO: Initialize to an appropriate value
            Vector2 tileSize = null; // TODO: Initialize to an appropriate value
            IList<Panel> panels = null; // TODO: Initialize to an appropriate value
            TileMap target = new TileMap(position, numTiles, tileSize, panels); // TODO: Initialize to an appropriate value
            Tile tile = null; // TODO: Initialize to an appropriate value
            List<Tile> expected = null; // TODO: Initialize to an appropriate value
            List<Tile> actual;
            actual = target.GetWalkableNeighbors(tile);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for InitializeTiles
        ///</summary>
        [TestMethod()]
        [DeploymentItem("AStartTest.exe")]
        public void InitializeTilesTest()
        {
            PrivateObject param0 = null; // TODO: Initialize to an appropriate value
            TileMap_Accessor target = new TileMap_Accessor(param0); // TODO: Initialize to an appropriate value
            IList<Panel> panels = null; // TODO: Initialize to an appropriate value
            target.InitializeTiles(panels);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for Tiles
        ///</summary>
        [TestMethod()]
        public void TilesTest()
        {
            Vector2 position = null; // TODO: Initialize to an appropriate value
            Vector2 numTiles = null; // TODO: Initialize to an appropriate value
            Vector2 tileSize = null; // TODO: Initialize to an appropriate value
            IList<Panel> panels = null; // TODO: Initialize to an appropriate value
            TileMap target = new TileMap(position, numTiles, tileSize, panels); // TODO: Initialize to an appropriate value
            IList<Tile> actual;
            actual = target.Tiles;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
