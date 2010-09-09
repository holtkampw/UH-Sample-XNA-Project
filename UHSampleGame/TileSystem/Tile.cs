using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UHSampleGame.CoreObjects.Towers;

namespace UHSampleGame.TileSystem
{
    public enum TileType { Walkable, Blocked, Start, Goal, Path, Current, Open, Closed, Null }

    public class Tile
    {
        Vector3 position;
        Vector2 size;
        TileType tileType;

        int id;

        public TileType TileType
        {
            get { return tileType; }
        }

        /// <summary>
        /// The 3D coordinate of the CENTER of the tile
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
        }

        /// <summary>
        /// The width and length of the tile
        /// </summary>
        public Vector2 Size
        {
            get { return size; }
        }


        /// <summary>
        /// The unique id of the tile
        /// </summary>
        public int ID
        {
            get { return id; }
        }

        /// <summary>
        /// Represents a tile of a tile map
        /// </summary>
        /// <param name="position">The center position of the tile</param>
        /// <param name="size">The width and length of the tile</param>
        public Tile(int id, Vector3 position, Vector2 size)
            : this(id, position, size, TileType.Walkable) { }

        public Tile()
        {
            this.tileType = TileType.Null;
        }

        public Tile(int id, Vector3 position, Vector2 size, TileType tileType)
        {
            this.id = id;
            this.position = position;
            this.size = size;
            this.tileType = tileType;

            SetTileType(tileType);

        }

        public bool IsWalkable()
        {
            return tileType != TileType.Blocked && !IsNull();
        }

        public bool IsStart()
        {
            return tileType == TileType.Start;
        }

        public bool IsGoal()
        {
            return tileType == TileType.Goal;
        }

        public bool IsNull()
        {
            return tileType == TileType.Null;
        }

        public TileType GetTileType()
        {
            return tileType;
        }

        public void SetTileType(TileType tileType)
        {
            this.tileType = tileType;
        }

        public override string ToString()
        {
            return tileType.ToString();
        }

        //public override bool Equals(object obj)
        //{
        //    return id == ((Tile)obj).id;
        //}
    }
}
