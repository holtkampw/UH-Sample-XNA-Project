using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UHSampleGame.CoreObjects.Towers;

namespace UHSampleGame.TileSystem
{
    class Tile
    {
        Vector3 position;
        Vector2 size;
        Tower tower;

        int id;
        
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
        /// The tower on the tile
        /// </summary>
        public Tower Tower
        {
            get { return tower; }
        }

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
        {
            this.id = id;
            this.position = position;
            this.size = size;
        }

        /// <summary>
        /// Returns whether or not the tile has a tower on it
        /// </summary>
        /// <returns>Returns true if there is a tower on the tile</returns>
        public bool HasTower()
        {
            return tower != null;
        }

        /// <summary>
        /// Sets a tower on the tile
        /// </summary>
        /// <param name="tower">The tower to put on the tile</param>
        /// <returns></returns>
        public bool SetTower(Tower tower)
        {
            if (HasTower())
                return false;

            this.tower = tower;
            return true;
        }
    }
}
