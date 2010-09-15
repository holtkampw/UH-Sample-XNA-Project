using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UHSampleGame.CoreObjects.Towers;
using UHSampleGame.CoreObjects;
using UHSampleGame.PathFinding;

namespace UHSampleGame.TileSystem
{
    public enum TileType { Walkable, Blocked, Path, Null }

    public class Tile
    {
        Vector3 position;
        Vector2 size;
        TileType tileType;
        GameObject gameObject;
        List<Tile> path;

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

        public List<Tile> Path
        {
            get { return path; }
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
            this.id = -1;
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

        public void SetTower(Tower tower)
        {
            gameObject = tower;
            SetTileType(TileType.Blocked);
        }

        public void RemoveTower(Tower tower)
        {
            gameObject = null;
            SetTileType(TileType.Walkable);
        }

        public List<Tile> GetPathTo(Tile tile)
        {
            AStar aStar = new AStar(this, tile);
            this.path = new List<Tile>(aStar.FindPath());
            return path;
        }

        public void UpdatePathTo(Tile tile)
        {
            AStar aStar = new AStar(this, tile);
            this.path = new List<Tile>(aStar.FindPath());
        }

        //public override bool Equals(object obj)
        //{
        //    return id == ((Tile)obj).id;
        //}
    }
}
