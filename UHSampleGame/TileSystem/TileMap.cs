using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace UHSampleGame.TileSystem
{
    public enum NeighborTile
    {
        Up, Down, Left, Right, UpLeft,
        UpRight, DownLeft, DownRight
    }

    class TileMap
    {
        List<Tile> tiles;
        Vector3 position;
        Vector3 upperLeftPos;
        Vector2 numTiles;
        Vector2 tileSize;

        public TileMap(Vector3 position, Vector2 numTiles, Vector2 tileSize)
        {
            this.position = position;
            this.numTiles = numTiles;
            this.tileSize = tileSize;

            tiles = new List<Tile>();
            InitializeTiles();
        }

        protected void InitializeTiles()
        {
            Vector2 upperLeftLoc = new Vector2();
            Vector3 currentCenterPos = new Vector3();

            upperLeftLoc.X = position.X - (tileSize.X * numTiles.X / 2.0f);
            upperLeftLoc.Y = position.Z - (tileSize.Y * numTiles.Y / 2.0f);

            upperLeftPos.X = upperLeftLoc.X + tileSize.X / 2.0f;
            upperLeftPos.Y = upperLeftLoc.Y + tileSize.Y / 2.0f;

            currentCenterPos.X = upperLeftPos.X;
            currentCenterPos.Y = 0;
            currentCenterPos.Z = upperLeftPos.Y;

            for (int y = 0; y < numTiles.Y; y++)
            {
                currentCenterPos.X = upperLeftPos.X;
                for (int x = 0; x < numTiles.X; x++)
                {
                    tiles.Add(new Tile(tiles.Count, currentCenterPos, tileSize));
                    currentCenterPos.X += tileSize.X;
                }
                currentCenterPos.Z += tileSize.Y;
            }
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
            max = min + (int)numTiles.X-1;
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
                    break;
                case NeighborTile.UpRight: newIndex = tile.ID - (int)numTiles.X + 1;
                    break;
                case NeighborTile.DownLeft: newIndex = tile.ID + (int)numTiles.X - 1;
                    break;
                case NeighborTile.DownRight: newIndex = tile.ID + (int)numTiles.X + 1;
                    break;
            }

            if (newIndex < 0 || newIndex >= (numTiles.X * numTiles.Y))
            {
                newIndex = tile.ID;
            }
            return tiles[newIndex];
        }

        /// <summary>
        /// Returns a tile given a Vector3 position
        /// </summary>
        /// <param name="position">The position to investigate</param>
        /// <returns>Returns the tile that encompasses the position or a 
        /// null tile if no tile exists</returns>
        public Tile GetTileFromPos(Vector3 position)
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
    }
}
