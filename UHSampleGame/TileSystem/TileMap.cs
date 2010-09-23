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

    public class TileMap
    {
        static List<Tile> tiles;
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
        static List<NeighborTile> allNeighbors;

        public static event TowerCreated TowerCreated;

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

        public static void InitializeTileMap(Vector3 position, Vector2 numTiles, Vector2 tileSize)
        {
            TileMap.position = position;
            TileMap.numTiles = numTiles;
            TileMap.tileSize = tileSize;

            bases = new List<Base>();

            mins = new List<int>();
            maxs = new List<int>();

            numTilesX = (int)numTiles.X;
            numTilesY = (int)numTiles.Y;

            upperLeftPos = new Vector3();
            lowerRightPos = new Vector3();

            tiles = new List<Tile>();
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

        public static void SetBase(Base setBase)
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
        /// Gets a neigbhoring tile's position
        /// </summary>
        /// <param name="tile">The start tile</param>
        /// <param name="neighborTile">The neighbor to examine</param>
        /// <returns>Returns the tile neighbor or a null tile if neighbor is not found</returns>
        public static Tile GetTileNeighbor(Tile tile, NeighborTile neighborTile)
        {
            int newIndex = 0;// tile.ID;
            int min, max, tileId;
            tileId = tile.ID;
            min = mins[tileId];// (tile.ID / numTilesX) * numTilesX;
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
                return new Tile();
            }

            return tiles[newIndex];
        }

        /// <summary>
        /// Returns a tile given a Vector3 position
        /// </summary>
        /// <param name="position">The position to investigate</param>
        /// <returns>Returns the tile that encompasses the position or a 
        /// null tile if no tile exists</returns>
        public static Tile GetTileFromPos(Vector3 position)
        {
            int xNum, yNum, index;
            xNum = yNum = index = 0;

            //xNum = (int)((upperLeftPos.X - position.X) / (int)tileSize.X);
            //yNum = (int)(((upperLeftPos.Z - position.Z) / (int)tileSize.Y) * numTiles.X);

            xNum = (int)Math.Round((upperLeftPos.X - position.X) / (int)tileSize.X);
            yNum = (int)(Math.Round((upperLeftPos.Z - position.Z) / (int)tileSize.Y) * numTiles.X);

            index = Math.Abs(xNum) + Math.Abs(yNum);

            if (index >= 0 && index < numTiles.X * numTiles.Y)
                return tiles[index];

            return new Tile();
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
        public static List<Tile> GetWalkableNeighbors(Tile tile)
        {
            return GetWalkableNeighbors(tile, new Dictionary<int, Tile>());
        }

        public static List<Tile> GetWalkableNeighbors(Tile tile, Dictionary<int, Tile> exclude)
        {
            List<Tile> neighbors = new List<Tile>();
            Tile currentNeighbor;
            for (int i = 0; i < allNeighbors.Count; i++)
            {
                currentNeighbor = GetTileNeighbor(tile, allNeighbors[i]);
                if (exclude.ContainsKey(currentNeighbor.ID))
                    continue;
                if (currentNeighbor.IsWalkable())
                {
                    if (allNeighbors[i] == NeighborTile.DownLeft)
                    {
                        if (GetTileNeighbor(tile, NeighborTile.Down).IsWalkable() &&
                            GetTileNeighbor(tile, NeighborTile.Left).IsWalkable())
                            neighbors.Add(currentNeighbor);
                    }
                    else if (allNeighbors[i] == NeighborTile.DownRight)
                    {
                        if (GetTileNeighbor(tile, NeighborTile.Down).IsWalkable() &&
                            GetTileNeighbor(tile, NeighborTile.Right).IsWalkable())
                            neighbors.Add(currentNeighbor);
                    }
                    else if (allNeighbors[i] == NeighborTile.UpLeft)
                    {
                        if (GetTileNeighbor(tile, NeighborTile.Up).IsWalkable() &&
                            GetTileNeighbor(tile, NeighborTile.Left).IsWalkable())
                            neighbors.Add(currentNeighbor);
                    }
                    else if (allNeighbors[i] == NeighborTile.UpRight)
                    {
                        if (GetTileNeighbor(tile, NeighborTile.Up).IsWalkable() &&
                            GetTileNeighbor(tile, NeighborTile.Right).IsWalkable())
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

        public static void SetObject(GameObject gameObject, Tile tile)
        {
            if (gameObject is Tower)
                SetTower((Tower)gameObject, tile);
            else if (gameObject is Base)
                SetBase((Base)gameObject);

        }

        public static bool SetTower(Tower tower, Tile tile)
        {
            tile.SetBlockableObject(tower);
            if (IsTilePathsValid())
            {
                UpdateTilePaths();

                List<Tile> walkableNeighbors = GetWalkableNeighbors(tile);

                for (int i = 0; i < walkableNeighbors.Count; i++)
                {
                    walkableNeighbors[i].RegisterTowerListener(tower);
                }
                OnTowerCreated();
                return true;
            }

            RemoveTower(tile);
            return false;

        }

        public static void SetTowerForLevelMap(Tower tower, Tile tile)
        {
            tile.SetBlockableObject(tower);

            //UpdateTilePaths();

            List<Tile> walkableNeighbors = GetWalkableNeighbors(tile);

            for (int i = 0; i < walkableNeighbors.Count; i++)
            {
                walkableNeighbors[i].RegisterTowerListener(tower);
            }
            //OnTowerCreated();


        }

        private static void OnTowerCreated()
        {
            if (TowerCreated != null)
                TowerCreated();
        }

        public static void RemoveTower(Tile tile)
        {
            tile.RemoveBlockableObject();
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
