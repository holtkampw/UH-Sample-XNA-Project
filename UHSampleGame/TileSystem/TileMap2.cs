using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using UHSampleGame.CoreObjects.Base;
using UHSampleGame.CoreObjects.Towers;
using UHSampleGame.CoreObjects;
using UHSampleGame.Events;

namespace UHSampleGame.TileSystem
{
    public enum NeighborTile
    {
        Up, Down, Left, Right, UpLeft,
        UpRight, DownLeft, DownRight
    }

    public class TileMap2
    {
        static List<Tile2> tiles;
        static List<Base2> bases;
        static List<int> mins;
        static List<int> maxs;
        static Vector3 position;
        static Vector3 upperLeftPos;
        static Vector3 lowerRightPos;
        static Vector2 numTiles;
        static Vector2 tileSize;
        static int numTilesX;
        static int numTilesY;
        static List<NeighborTile> allNeighbors;
        static List<Tile2> neighbors = new List<Tile2>();

        public static event TowerCreated TowerCreated;

        public static IList<Tile2> Tiles
        {
            get { return tiles; }
        }

        public static Vector2 TileSize
        {
            get { return tileSize; }
        }

        public static float Top
        {
            get { return tiles[0].Position.Z; }
        }
        public static float Left
        {
            get { return tiles[0].Position.X; }
        }
        public static float Right
        {
            get { return tiles[tiles.Count - 1].Position.X; }
        }
        public static float Bottom
        {
            get { return tiles[tiles.Count - 1].Position.Z; }
        }

        public static void InitializeTileMap(Vector3 position, Vector2 numTiles, Vector2 tileSize)
        {
            TileMap2.position = position;
            TileMap2.numTiles = numTiles;
            TileMap2.tileSize = tileSize;

            bases = new List<Base2>();

            mins = new List<int>();
            maxs = new List<int>();

            numTilesX = (int)numTiles.X;
            numTilesY = (int)numTiles.Y;

            upperLeftPos = new Vector3();
            lowerRightPos = new Vector3();

            tiles = new List<Tile2>();
            InitializeTiles();
            allNeighbors = new List<NeighborTile>();
            allNeighbors.Add(NeighborTile.Down);
            allNeighbors.Add(NeighborTile.DownLeft);
            allNeighbors.Add(NeighborTile.DownRight);
            allNeighbors.Add(NeighborTile.Left);
            allNeighbors.Add(NeighborTile.Right);
            allNeighbors.Add(NeighborTile.Up);
            allNeighbors.Add(NeighborTile.UpLeft);
            allNeighbors.Add(NeighborTile.UpRight);
        }

        public static void SetBase(Base2 setBase)
        {
            bases.Add(setBase);
        }

        protected static void InitializeTiles()
        {
            Vector3 upperLeftLoc = new Vector3();
            Vector3 currentCenterPos = new Vector3();
            int min, max, tileId;

            upperLeftLoc.X = position.X - (tileSize.X * numTiles.X / 2.0f);
            upperLeftLoc.Z = position.Z - (tileSize.Y * numTiles.Y / 2.0f);

            upperLeftPos.X = upperLeftLoc.X + tileSize.X / 2.0f;
            upperLeftPos.Z = upperLeftLoc.Z + tileSize.Y / 2.0f;

            currentCenterPos.X = upperLeftPos.X;
            currentCenterPos.Y = 0;
            currentCenterPos.Z = upperLeftPos.Z;

            for (int y = 0; y < numTiles.Y; y++)
            {
                currentCenterPos.X = upperLeftPos.X;
                for (int x = 0; x < numTiles.X; x++)
                {
                    tileId = (y * numTilesX) + x;
                    min = (tileId / numTilesX) * numTilesX;
                    max = min + numTilesX - 1;

                    mins.Add(min);
                    maxs.Add(max);
                    tiles.Add(new Tile2(tiles.Count, new Vector3(currentCenterPos.X, currentCenterPos.Y, currentCenterPos.Z),
                        new Vector2(tileSize.X, tileSize.Y)));
                    currentCenterPos.X += tileSize.X;
                }
                currentCenterPos.Z += tileSize.Y;
            }

            lowerRightPos = new Vector3(tiles[tiles.Count - 1].Position.X + (tileSize.X / 2), 0,
                tiles[tiles.Count - 1].Position.Z + (tileSize.Y / 2));
        }

        /// <summary>
        /// Gets a neigbhoring Tile2's position
        /// </summary>
        /// <param name="Tile2">The start Tile2</param>
        /// <param name="neighborTile">The neighbor to examine</param>
        /// <returns>Returns the Tile2 neighbor or a null Tile2 if neighbor is not found</returns>
        public static Tile2 GetTileNeighbor(Tile2 Tile2, NeighborTile neighborTile)
        {
            int newIndex = 0;// Tile2.ID;
            int min, max, tileId;
            tileId = Tile2.ID;
            min = mins[tileId];// (Tile2.ID / numTilesX) * numTilesX;
            max = maxs[tileId];// min + numTilesX - 1;
            switch (neighborTile)
            {
                case NeighborTile.Up: newIndex = tileId - (int)numTiles.X;
                    break;
                case NeighborTile.Down: newIndex = tileId + (int)numTiles.X;
                    break;
                case NeighborTile.Left: newIndex = tileId - 1;
                    if (newIndex < min)
                        newIndex = tileId;
                    break;
                case NeighborTile.Right: newIndex = tileId + 1;
                    if (newIndex > max)
                        newIndex = tileId;
                    break;
                case NeighborTile.UpLeft: newIndex = tileId - numTilesX - 1;
                    if (newIndex < min - (int)numTiles.X)
                        newIndex = tileId;
                    break;
                case NeighborTile.UpRight: newIndex = tileId - numTilesX + 1;
                    if (newIndex > max - (int)numTiles.X)
                        newIndex = tileId;
                    break;
                case NeighborTile.DownLeft: newIndex = tileId + numTilesX - 1;
                    if (newIndex < min + (int)numTiles.X)
                        newIndex = tileId;
                    break;
                case NeighborTile.DownRight: newIndex = tileId + numTilesX + 1;
                    if (newIndex > max + (int)numTiles.X)
                        newIndex = tileId;
                    break;
            }

            if (newIndex < 0 || newIndex >= (numTilesX * numTilesY))
            {
                newIndex = tileId;
            }
            if (newIndex == tileId)
            {
                return Tile2.NullTile;
            }

            return tiles[newIndex];
        }

        /// <summary>
        /// Returns a Tile2 given a Vector3 position
        /// </summary>
        /// <param name="position">The position to investigate</param>
        /// <returns>Returns the Tile2 that encompasses the position or a 
        /// null Tile2 if no Tile2 exists</returns>
        public static Tile2 GetTileFromPos(Vector3 position)
        {
            int xNum, yNum, index;
            xNum = yNum = index = 0;

            //xNum = (int)((upperLeftPos.X - position.X) / (int)tileSize.X);
            //yNum = (int)(((upperLeftPos.Z - position.Z) / (int)tileSize.Y) * numTiles.X);

            // xNum = (int)Math.Round((upperLeftPos.X - position.X) / (int)tileSize.X);
            // yNum = (int)(Math.Round((upperLeftPos.Z - position.Z) / (int)tileSize.Y) * numTiles.X);

            index = Math.Abs(xNum) + Math.Abs(yNum);

            if (index >= 0 && index < numTiles.X * numTiles.Y)
                return tiles[index];

            return Tile2.NullTile;
        }

        public static Vector3 GetTilePosFromPos(Vector3 position)
        {
            int xNum, yNum, index;
            xNum = yNum = index = 0;

            //xNum = (int)((upperLeftPos.X - position.X) / (int)tileSize.X);
            //yNum = (int)(((upperLeftPos.Z - position.Z) / (int)tileSize.Y) * numTiles.X);

            xNum = (int)Math.Round((upperLeftPos.X - position.X) / (int)tileSize.X);
            yNum = (int)(Math.Round((upperLeftPos.Z - position.Z) / (int)tileSize.Y) * numTiles.X);

            index = Math.Abs(xNum) + Math.Abs(yNum);

            if (index >= 0 && index < numTiles.X * numTiles.Y)
                return tiles[index].Position;

            return Vector3.Zero;
        }

        public static Tile2 GetTileFromType(TileType tileType)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].TileType == tileType)
                    return tiles[i];
            }
            return null;
        }
        public static List<Tile2> GetWalkableNeighbors(Tile2 Tile2)
        {
            return GetWalkableNeighbors(Tile2, null);
        }

        public static List<Tile2> GetWalkableNeighbors(Tile2 Tile2, Dictionary<int, Tile2> exclude)
        {
            //List<Tile2> neighbors = new List<Tile2>();
            neighbors.Clear();
            Tile2 currentNeighbor;
            for (int i = 0; i < allNeighbors.Count; i++)
            {
                currentNeighbor = GetTileNeighbor(Tile2, allNeighbors[i]);
                if (exclude != null && exclude.ContainsKey(currentNeighbor.ID))
                    continue;
                if (currentNeighbor.IsWalkable())
                {
                    if (allNeighbors[i] == NeighborTile.DownLeft)
                    {
                        if (GetTileNeighbor(Tile2, NeighborTile.Down).IsWalkable() &&
                            GetTileNeighbor(Tile2, NeighborTile.Left).IsWalkable())
                            neighbors.Add(currentNeighbor);
                    }
                    else if (allNeighbors[i] == NeighborTile.DownRight)
                    {
                        if (GetTileNeighbor(Tile2, NeighborTile.Down).IsWalkable() &&
                            GetTileNeighbor(Tile2, NeighborTile.Right).IsWalkable())
                            neighbors.Add(currentNeighbor);
                    }
                    else if (allNeighbors[i] == NeighborTile.UpLeft)
                    {
                        if (GetTileNeighbor(Tile2, NeighborTile.Up).IsWalkable() &&
                            GetTileNeighbor(Tile2, NeighborTile.Left).IsWalkable())
                            neighbors.Add(currentNeighbor);
                    }
                    else if (allNeighbors[i] == NeighborTile.UpRight)
                    {
                        if (GetTileNeighbor(Tile2, NeighborTile.Up).IsWalkable() &&
                            GetTileNeighbor(Tile2, NeighborTile.Right).IsWalkable())
                            neighbors.Add(currentNeighbor);
                    }
                    else
                        neighbors.Add(currentNeighbor);
                }

            }
            return neighbors;
        }

        public static void UpdateTilePaths()
        {
            for (int j = 0; j < bases.Count; j++)
            {
                for (int i = 0; i < tiles.Count; i++)
                {
                    if (tiles[i].IsWalkable())
                        tiles[i].UpdatePathTo(bases[j].Tile);
                }
            }
        }

        public static bool IsTilePathsValid()
        {
            for (int j = 0; j < bases.Count; j++)
            {
                for (int i = 0; i < bases.Count; i++)
                {
                    if (i != j)
                    {
                        bases[i].Tile.UpdatePathTo(bases[j].Tile);
                        if (bases[j].Tile.Paths[bases[i].Tile.ID].Count == 0)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public static void SetObject(Tower2 gameObject, Tile2 Tile2)
        {
            SetTower(gameObject, Tile2);
        }

        public static void SetObject(Base2 gameObject, Tile2 Tile2)
        {
            SetBase(gameObject);
        }

        public static bool SetTower(Tower2 tower, Tile2 Tile2)
        {
            Tile2.SetBlockableObject(tower);
            if (IsTilePathsValid())
            {
                UpdateTilePaths();

                List<Tile2> walkableNeighbors = GetWalkableNeighbors(Tile2);

                for (int i = 0; i < walkableNeighbors.Count; i++)
                {
                    walkableNeighbors[i].RegisterTowerListener(ref tower);
                }
                OnTowerCreated();
                return true;
            }

            RemoveTower(ref Tile2);
            return false;

        }

        public static void SetTowerForLevelMap(Tower2 tower, Tile2 tile)
        {
            
            tile.SetBlockableObject(tower);
            //tile2.SetBlockableObject(tower);

            //UpdateTilePaths();

            List<Tile2> walkableNeighbors = GetWalkableNeighbors(tile);

            for (int i = 0; i < walkableNeighbors.Count; i++)
            {
                walkableNeighbors[i].RegisterTowerListener(ref tower);
            }
            //OnTowerCreated();
        }

        private static void OnTowerCreated()
        {
            if (TowerCreated != null)
                TowerCreated();
        }

        public static void RemoveTower(ref Tile2 Tile2)
        {
            Tile2.RemoveBlockableObject();
            UpdateTilePaths();
        }

        public static void Draw()
        {
            Microsoft.Xna.Framework.Graphics.Texture2D first = ScreenManagement.ScreenManager.Game.Content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Tiles\\1");
            Microsoft.Xna.Framework.Graphics.Texture2D second = ScreenManagement.ScreenManager.Game.Content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Tiles\\2");
            Microsoft.Xna.Framework.Graphics.Texture2D third = ScreenManagement.ScreenManager.Game.Content.Load<Microsoft.Xna.Framework.Graphics.Texture2D>("Tiles\\3");

            List<Microsoft.Xna.Framework.Graphics.Texture2D> graphics = new List<Microsoft.Xna.Framework.Graphics.Texture2D>();
            graphics.Add(first);
            graphics.Add(second);
            graphics.Add(third);

            Vector2 left_position = new Vector2(position.X - ((tileSize.X * numTiles.X) / 2.0f),
                position.Z - ((tileSize.Y * numTiles.Y) / 2.0f));

            for (int i = 0; i < tiles.Count; i++)
            {
                ScreenManagement.ScreenManager.SpriteBatch.Begin();
                ScreenManagement.ScreenManager.SpriteBatch.Draw(graphics[i % 3],
                    new Vector2(-left_position.X + (tiles[i].Position.X - (first.Width / 2)),
                        -left_position.Y + (tiles[i].Position.Z - (first.Height / 2))),
                        Color.White);
                ScreenManagement.ScreenManager.SpriteBatch.End();
            }
        }

    }
}
