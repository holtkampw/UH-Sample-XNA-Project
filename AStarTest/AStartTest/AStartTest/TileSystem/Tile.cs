using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Windows.Forms;

using AStartTest.Vectors;

namespace AStartTest.TileSystem
{
    public enum TileType { Walkable, Blocked, Start, Goal, Path, Current, Open, Closed, Null }

    public class Tile
    {
        Panel panel;
        Vector2 position;
        Vector2 size;
        TileType tileType;
        Dictionary<TileType, Color> colorDictionary;

        int id;

        public TileType TileType
        {
            get { return tileType; }
        }

        /// <summary>
        /// The 3D coordinate of the CENTER of the tile
        /// </summary>
        public Vector2 Position
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

        public int ID
        {
            get { return id; }
        }

        public Panel Panel
        {
            get{return panel;}
        }

        /// <summary>
        /// Represents a tile of a tile map
        /// </summary>
        /// <param name="position">The center position of the tile</param>
        /// <param name="size">The width and length of the tile</param>
        public Tile(int id, Vector2 position, Vector2 size, Panel panel)
            : this(id, position, size, panel, TileType.Walkable) { }

        public Tile()
        {
            this.tileType = TileType.Null;
        }

        public Tile(int id, Vector2 position, Vector2 size, Panel panel, TileType tileType)
        {
            this.id = id;
            this.position = position;
            this.size = size;
            this.panel = panel;
            this.tileType = tileType;
            this.colorDictionary = new Dictionary<TileType, Color>();

            panel.Location = new System.Drawing.Point((int)position.X, (int)position.Y);
            panel.Size = new System.Drawing.Size((int)size.X, (int)size.Y);

            colorDictionary.Add(TileType.Blocked, Color.Red);
            colorDictionary.Add(TileType.Goal, Color.Blue);
            colorDictionary.Add(TileType.Path, Color.Yellow);
            colorDictionary.Add(TileType.Start, Color.Green);
            colorDictionary.Add(TileType.Walkable, Color.Black);
            colorDictionary.Add(TileType.Current, Color.Pink);
            colorDictionary.Add(TileType.Open, Color.Purple);
            colorDictionary.Add(TileType.Closed, Color.Orange);

            SetTileType(tileType);

        }

        public bool IsWalkable()
        {
            return tileType != TileType.Blocked && tileType != TileType.Null;
        }

        public bool IsStart()
        {
            return tileType == TileType.Start;
        }

        public bool IsGoal()
        {
            return tileType == TileType.Goal;
        }

        public TileType GetTileType()
        {
            return tileType;
        }

        public void SetTileType(TileType tileType)
        {
            this.tileType = tileType;
            panel.BackColor = colorDictionary[tileType];
        }

        public override string ToString()
        {
            return tileType.ToString();
        }

        public override bool Equals(object obj)
        {
            return id == ((Tile)obj).id;
        }

    }
}
