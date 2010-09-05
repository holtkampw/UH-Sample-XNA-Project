using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AStartTest.Vectors;

namespace AStartTest.TileSystem
{
    public enum NeighborTile
    {
        Up, Down, Left, Right, UpLeft,
        UpRight, DownLeft, DownRight
    }

    public class TileMap
    {
        List<Tile> tiles;
        Vector2 position;
        Vector2 upperLeftPos;
        Vector2 numTiles;
        Vector2 tileSize;
        List<NeighborTile> allNeighbors;

        public IList<Tile> Tiles
        {
            get { return tiles; }
        }

        public TileMap(Vector2 position, Vector2 numTiles, Vector2 tileSize, IList<Panel> panels)
        {
            this.position = position;
            this.numTiles = numTiles;
            this.tileSize = tileSize;

            upperLeftPos = new Vector2();

            tiles = new List<Tile>();
            InitializeTiles(panels);
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

        protected void InitializeTiles(IList<Panel> panels)
        {
            Vector2 upperLeftLoc = new Vector2();
            Vector2 currentCenterPos = new Vector2();

            upperLeftLoc.X = position.X - (tileSize.X * numTiles.X / 2.0f);
            upperLeftLoc.Y = position.Y - (tileSize.Y * numTiles.Y / 2.0f);

            upperLeftPos.X = upperLeftLoc.X + tileSize.X / 2.0f;
            upperLeftPos.Y = upperLeftLoc.Y + tileSize.Y / 2.0f;

            currentCenterPos.X = upperLeftPos.X;
            currentCenterPos.Y = 0;
            currentCenterPos.Y = upperLeftPos.Y;

            for (int y = 0; y < numTiles.Y; y++)
            {
                currentCenterPos.X = upperLeftPos.X;
                for (int x = 0; x < numTiles.X; x++)
                {
                    tiles.Add(new Tile(tiles.Count, Vector2.Copy(currentCenterPos), Vector2.Copy(tileSize), panels[(int)(y*numTiles.X) + x]));
                    currentCenterPos.X += tileSize.X;
                }
                currentCenterPos.Y += tileSize.Y;
            }
        }

        public Tile GetTileFromPanel(Panel panel)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].Panel == panel)
                    return tiles[i];
            }
            return null;
        }

        /// <summary>
        /// Gets a neigbhoring tile's position
        /// </summary>
        /// <param name="tile">The start tile</param>
        /// <param name="neighborTile">The neighbor to examine</param>
        /// <returns></returns>
        public Tile GetTileNeighbor(Tile tile, NeighborTile neighborTile)
        {
            int newIndex = tile.ID;
            int min, max;
            min = (tile.ID / (int)numTiles.X) * (int)numTiles.X;
            max = min + (int)numTiles.X - 1;
            switch (neighborTile)
            {
                case NeighborTile.Up: newIndex = tile.ID - (int)numTiles.X;
                    break;
                case NeighborTile.Down: newIndex = tile.ID + (int)numTiles.X;
                    break;
                case NeighborTile.Left: newIndex = tile.ID - 1;
                    if (newIndex < min)
                        newIndex = tile.ID;
                    break;
                case NeighborTile.Right: newIndex = tile.ID + 1;
                    if (newIndex > max)
                        newIndex = tile.ID;
                    break;
                case NeighborTile.UpLeft: newIndex = tile.ID - (int)numTiles.X - 1;
                    if (newIndex < min - (int)numTiles.X)
                        newIndex = tile.ID;
                    break;
                case NeighborTile.UpRight: newIndex = tile.ID - (int)numTiles.X + 1;
                    if (newIndex > max - (int)numTiles.X)
                        newIndex = tile.ID;
                    break;
                case NeighborTile.DownLeft: newIndex = tile.ID + (int)numTiles.X - 1;
                    if (newIndex < min + (int)numTiles.X)
                        newIndex = tile.ID;
                    break;
                case NeighborTile.DownRight: newIndex = tile.ID + (int)numTiles.X + 1;
                    if (newIndex > max + (int)numTiles.X)
                        newIndex = tile.ID;
                    break;
            }

            if (newIndex < 0 || newIndex >= (numTiles.X * numTiles.Y))
            {
                newIndex = tile.ID;
            }
            if (newIndex == tile.ID)
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
        public Tile GetTileFromPos(Vector2 position)
        {
            int xNum, yNum, index;
            xNum = yNum = index = 0;

            xNum = (int)(upperLeftPos.X - position.X) / (int)tileSize.X;
            yNum = (int)(((int)(upperLeftPos.X - position.X) / (int)tileSize.Y) * numTiles.X);

            index = Math.Abs(xNum) + Math.Abs(yNum);

            if (index >= 0 && index < numTiles.X * numTiles.Y)
                return tiles[index];

            return null;
        }

        public void ClearPath()
        {
            for(int i =0; i<tiles.Count; i++)
            {
                if(tiles[i].TileType == TileType.Path ||
                    tiles[i].TileType == TileType.Open ||
                    tiles[i].TileType == TileType.Closed)
                tiles[i].SetTileType(TileType.Walkable);
            }
        }

        public Tile GetTileFromType(TileType tileType)
        {
            for (int i = 0; i < tiles.Count; i++)
            {
                if (tiles[i].TileType == tileType)
                    return tiles[i];
            }
            return null;
        }

        public List<Tile> GetWalkableNeighbors(Tile tile)
        {
            List<Tile> neighbors = new List<Tile>();
            Tile currentNeighbor;
            for (int i = 0; i < allNeighbors.Count; i++)
            {
                currentNeighbor = GetTileNeighbor(tile, allNeighbors[i]);
                if (currentNeighbor.TileType != TileType.Null && currentNeighbor.IsWalkable())
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
    }
}