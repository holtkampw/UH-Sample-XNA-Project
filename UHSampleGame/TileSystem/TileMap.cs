﻿using System;
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

    public class TileMap
    {
        static List<Tile> tiles;
        static List<Tile> tileNeighbors = new List<Tile>();
        static List<Base> bases;
        static List<int> mins;
        static List<int> maxs;
        static Vector3 position;
        static Vector3 upperLeftPos;
        static Vector3 lowerRightPos;
        static Vector2 numTiles;
        static Vector2 tileSize;
        static int numTilesX;
        static int numTilesY;
        public static int TileCount;
        static List<NeighborTile> allNeighbors;
        static List<Tile> neighbors = new List<Tile>();
        static int totalTiles;

        // public static event TowerCreated TowerCreated;

        public static IList<Tile> Tiles
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
        static List<Tile> walkableNeighbors;

        static Dictionary<int, Tile> emptyExclude = new Dictionary<int,Tile>();

        public static void InitializeTileMap(Vector3 position, Vector2 numTiles, Vector2 tileSize)
        {
            TileMap.position = position;
            TileMap.numTiles = numTiles;
            TileMap.tileSize = tileSize;

            for (int i = 0; i < 8; i++)
                tileNeighbors.Add(Tile.NullTile);

            bases = new List<Base>();

            mins = new List<int>();
            maxs = new List<int>();

            numTilesX = (int)numTiles.X;
            numTilesY = (int)numTiles.Y;

            totalTiles = numTilesX * numTilesY;

            upperLeftPos = new Vector3();
            lowerRightPos = new Vector3();

            tiles = new List<Tile>();
            TileCount = numTilesX * numTilesY;
            InitializeTiles();
            allNeighbors = new List<NeighborTile>();
            allNeighbors.Add(NeighborTile.UpLeft);
            allNeighbors.Add(NeighborTile.Up);
            allNeighbors.Add(NeighborTile.UpRight);
            allNeighbors.Add(NeighborTile.Right);
            allNeighbors.Add(NeighborTile.DownRight);
            allNeighbors.Add(NeighborTile.Down);
            allNeighbors.Add(NeighborTile.DownLeft);
            allNeighbors.Add(NeighborTile.Left);

        }

        public static void SetBase(Base setBase, Tile tile)
        {
            bases.Add(setBase);
            tile.TileType = TileType.Base;
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
                    tiles.Add(new Tile(tiles.Count, new Vector3(currentCenterPos.X, currentCenterPos.Y, currentCenterPos.Z),
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
        public static Tile GetTileNeighbor(ref Tile Tile2, NeighborTile neighborTile)
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

            if (newIndex < 0 || newIndex >= totalTiles)
            {
                newIndex = tileId;
            }
            if (newIndex == tileId)
            {
                return Tile.NullTile;
            }

            return tiles[newIndex];
        }

        /// <summary>
        /// Returns a Tile2 given a Vector3 position
        /// </summary>
        /// <param name="position">The position to investigate</param>
        /// <returns>Returns the Tile2 that encompasses the position or a 
        /// null Tile2 if no Tile2 exists</returns>
        public static Tile GetTileFromPos(Vector3 position)
        {
            int xNum, yNum, index;
            float xN, yN;
            //xNum = yNum = index = 0;

            //xNum = (int)Math.Round((upperLeftPos.X - position.X) / (int)tileSize.X);
            //yNum = (int)(Math.Round((upperLeftPos.Z - position.Z) / (int)tileSize.Y) * numTilesX);

            xN = (upperLeftPos.X - position.X) / tileSize.X;
            yN = (upperLeftPos.Z - position.Z) / tileSize.Y;

            //FIX THIS!!!
           

            //if (xNum < 0)
            //    xNum *= -1;

            //if (yNum < 0)
            //    yNum *= -1;

            //Good
            //if (xN < 0)
            //    xN *= -1;

            //if (yN < 0)
            //    yN *= -1;

            xNum = xN < 0 ? (int)(-xN + 0.5f) : (int)(xN + 0.5f);
            yNum = yN < 0 ? (int)(-yN + 0.5f) * numTilesX : (int)(yN + 0.5f) * numTilesX;

            index = xNum + yNum;

            if (index >= 0 && index < totalTiles)
                return tiles[index];

            return Tile.NullTile;
        }

        public static Vector3 GetTilePosFromPos(Vector3 position)
        {
            int xNum, yNum, index;
            float xN, yN;
            xNum = yNum = index = 0;

            //xNum = (int)Math.Round((upperLeftPos.X - position.X) / (int)tileSize.X);
            //yNum = (int)(Math.Round((upperLeftPos.Z - position.Z) / (int)tileSize.Y) * numTilesX);

            xN = (upperLeftPos.X - position.X) / tileSize.X;
            yN = (upperLeftPos.Z - position.Z) / tileSize.Y;

            //FIX THIS!!!


            //if (xNum < 0)
            //    xNum *= -1;

            //if (yNum < 0)
            //    yNum *= -1;

            if (xN < 0)
                xN *= -1;

            if (yN < 0)
                yN *= -1;

            xNum = (int)(xN + 0.5f);
            yNum = (int)(yN + 0.5f) * numTilesX;

            index = xNum + yNum;

            if (index >= 0 && index < totalTiles)
                return tiles[index].Position;

            return Vector3.Zero;
        }

        public static Tile GetTileFromType(TileType tileType)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].TileType == tileType)
                    return tiles[i];
            }
            return null;
        }

        public static List<int> GetWalkableNeighborsAsArray(Tile Tile2)
        {
            List<int> intNeighbors = new List<int>();
            for (int i = 0; i < allNeighbors.Count; i++)
                tileNeighbors[i] = GetTileNeighbor(ref Tile2, allNeighbors[i]);

            for (int i = 0; i < allNeighbors.Count; i++)
            {
                //currentNeighbor = GetTileNeighbor(ref Tile2, allNeighbors[i]);

                if (tileNeighbors[i].IsWalkable())
                {
                    if (i == 6)//allNeighbors[i] == NeighborTile.DownLeft)
                    {
                        if (tileNeighbors[5].IsWalkable() &&
                            tileNeighbors[7].IsWalkable())
                            intNeighbors.Add(tileNeighbors[i].ID);
                    }
                    else if (i == 4)//allNeighbors[i] == NeighborTile.DownRight)
                    {
                        if (tileNeighbors[5].IsWalkable() &&
                            tileNeighbors[3].IsWalkable())
                            intNeighbors.Add(tileNeighbors[i].ID);
                    }
                    else if (i == 0)//allNeighbors[i] == NeighborTile.UpLeft)
                    {
                        if (tileNeighbors[1].IsWalkable() &&
                            tileNeighbors[7].IsWalkable())
                            intNeighbors.Add(tileNeighbors[i].ID);
                    }
                    else if (i == 2)//allNeighbors[i] == NeighborTile.UpRight)
                    {
                        if (tileNeighbors[1].IsWalkable() &&
                            tileNeighbors[3].IsWalkable())
                            intNeighbors.Add(tileNeighbors[i].ID);
                    }
                    else
                        intNeighbors.Add(tileNeighbors[i].ID);
                }

            }
            return intNeighbors;
        }

        public static List<Tile> GetWalkableNeighbors(Tile Tile2)
        {
            //REFACTOR with static neighbor list
            //List<Tile2> neighbors = new List<Tile2>();
            neighbors.Clear();
            //Tile currentNeighbor;

            for(int i =0; i< allNeighbors.Count; i++)
                tileNeighbors[i] = GetTileNeighbor(ref Tile2, allNeighbors[i]);

            for (int i = 0; i < allNeighbors.Count; i++)
            {
                //currentNeighbor = GetTileNeighbor(ref Tile2, allNeighbors[i]);

                if (tileNeighbors[i].IsWalkable())
                {
                    if (i==6)//allNeighbors[i] == NeighborTile.DownLeft)
                    {
                        if (tileNeighbors[5].IsWalkable() &&
                            tileNeighbors[7].IsWalkable())
                            neighbors.Add(tileNeighbors[i]);
                    }
                    else if (i==4)//allNeighbors[i] == NeighborTile.DownRight)
                    {
                        if (tileNeighbors[5].IsWalkable() &&
                            tileNeighbors[3].IsWalkable())
                            neighbors.Add(tileNeighbors[i]);
                    }
                    else if (i==0)//allNeighbors[i] == NeighborTile.UpLeft)
                    {
                        if (tileNeighbors[1].IsWalkable() &&
                            tileNeighbors[7].IsWalkable())
                            neighbors.Add(tileNeighbors[i]);
                    }
                    else if (i==2)//allNeighbors[i] == NeighborTile.UpRight)
                    {
                        if (tileNeighbors[1].IsWalkable() &&
                            tileNeighbors[3].IsWalkable())
                            neighbors.Add(tileNeighbors[i]);
                    }
                    else
                        neighbors.Add(tileNeighbors[i]);
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

        public static void SetObject(Tower gameObject, Tile Tile2)
        {
            SetTower(ref gameObject, ref Tile2);
        }

        public static void SetObject(Base gameObject, Tile Tile2)
        {
            SetBase(gameObject, Tile2);
        }

        public static bool SetTower(ref Tower tower, ref Tile Tile2)
        {
            Tile2.SetBlockableObject(tower);
            if (IsTilePathsValid())
            {
                UpdateTilePaths();

                walkableNeighbors = GetWalkableNeighbors(Tile2);

                for (int i = 0; i < walkableNeighbors.Count; i++)
                {
                    walkableNeighbors[i].RegisterTowerListener(ref tower);
                }
                OnTowerCreated();
                return true;
            }

           // RemoveTower(ref Tile2);

            Tile2.RemoveBlockableObject();
            for (int j = 0; j < bases.Count; j++)
            {
                for (int i = 0; i < bases.Count; i++)
                {
                    if (i != j)
                    {
                        bases[i].Tile.UpdatePathTo(bases[j].Tile);
                    }
                }
            }
            return false;

        }

        public static void SetTowerForLevelMap(Tower tower, Tile tile)
        {

            tile.SetBlockableObject(tower);
            //tile2.SetBlockableObject(tower);

            //UpdateTilePaths();

            List<Tile> walkableNeighbors = GetWalkableNeighbors(tile);

            for (int i = 0; i < walkableNeighbors.Count; i++)
            {
                walkableNeighbors[i].RegisterTowerListener(ref tower);
            }
            //OnTowerCreated();
        }

        private static void OnTowerCreated()
        {
            // if (TowerCreated != null)
            //    TowerCreated();
        }

        public static void RemoveTower(ref Tile Tile2)
        {
            Tile2.RemoveBlockableObject();
            UpdateTilePaths();
        }

        public static Tile GetTileForPlayer(int playerNum)
        {
            for(int i = 0; i < bases.Count; i++)
            {
                if (bases[i].PlayerNum == playerNum)
                    return bases[i].Tile;
            }
            return Tile.NullTile;
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
