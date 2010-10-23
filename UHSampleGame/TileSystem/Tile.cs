using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UHSampleGame.CoreObjects.Towers;
using UHSampleGame.CoreObjects.Units;
using UHSampleGame.CoreObjects;
using UHSampleGame.Events;
using UHSampleGame.PathFinding;

namespace UHSampleGame.TileSystem
{
    public enum TileType { Walkable, Blocked, Path, Null }

    public class Tile
    {
        public Vector3 Position;
        public Vector2 Size;
        public TileType TileType;
        Tower tower;
        List<Unit> units;
        public List<List<Tile>> Paths;
        static Random rand = new Random(DateTime.Now.Millisecond);
        public int ID;

        public static Tile NullTile = new Tile();

        public event UnitEvent UnitEnter;
        public event UnitEvent UnitExit;

        /// <summary>
        /// Represents a Tile2 of a Tile2 map
        /// </summary>
        /// <param name="position">The center position of the Tile2</param>
        /// <param name="size">The width and length of the Tile2</param>
        public Tile(int id, Vector3 position, Vector2 size)
            : this(id, position, size, TileType.Walkable) { }

        public Tile()
            : this(-1, Vector3.Zero, Vector2.Zero, TileType.Null) { }

        public Tile(int id, Vector3 position, Vector2 size, TileType tileType)
        {
            //this.rand = new Random(DateTime.Now.Millisecond);
            this.ID = id;
            this.Position = position;
            this.Size = size;
            this.TileType = tileType;
            this.Paths = new List<List<Tile>>();
            this.units = new List<Unit>();

            SetTileType(tileType);

        }

        public bool IsWalkable()
        {
            return TileType == TileType.Walkable;
        }


        public bool IsNull()
        {
            return TileType == TileType.Null;
        }

        public TileType GetTileType()
        {
            return TileType;
        }

        public void SetTileType(TileType tileType)
        {
            this.TileType = tileType;
        }

        public override string ToString()
        {
            return this.ID.ToString() + " " + TileType.ToString();
        }

        public void SetBlockableObject(Tower gameObject)
        {
            this.tower = gameObject;
            SetTileType(TileType.Blocked);
        }

        public void RemoveBlockableObject()
        {
            this.tower = null;
            SetTileType(TileType.Walkable);
        }

        public List<Tile> GetPathTo(Tile baseTile)
        {
            UpdatePathTo(baseTile);
            return Paths[baseTile.ID];
        }

        public void UpdatePathTo(Tile baseTile)
        {
            AStar.InitAstar(this, baseTile);

            for (int i = Paths.Count; i < TileMap.Tiles.Count; i++)
                Paths.Add(new List<Tile>());

            Paths[baseTile.ID] = AStar.FindPath(); //new List<Tile>(AStar.FindPath());
        }

        public Vector3 GetRandPoint()
        {
            //rand = new Random(DateTime.Now.Millisecond);
            int sizeX = (int)(Size.X / 3);
            int sizeY = (int)(Size.Y / 3);
            return new Vector3(Position.X + rand.Next(-sizeX, sizeX), 0/*rand.Next(-10, 10)*/, Position.Z + rand.Next(-sizeY, sizeY));
        }

        public void RegisterTowerListener(ref Tower tower)
        {
            UnitEnter += tower.RegisterAttackUnit;
            UnitExit += tower.UnregisterAttackUnit;
        }

        public void UnregisterTowerListener(ref Tower tower)
        {
            UnitEnter -= tower.RegisterAttackUnit;
            UnitExit -= tower.UnregisterAttackUnit;
        }

        public void AddUnit(ref Unit unit)
        {
            units.Add(unit);
            //unit.Died += RemoveUnit;
            OnUnitEnter(ref unit);
        }

        public void RemoveUnit(ref Unit unit)
        {
            units.Remove(unit);
            //Set new unit to attack
            OnUnitExit(ref unit);
            //unit.Died -= RemoveUnit;
            if (units.Count > 0)
            {
                Unit u = units[0];
                OnUnitEnter(ref u);
            }
            //else
              //  OnUnitEnter(null);

        }

        private void OnUnitExit(ref Unit unit)
        {
            if (UnitExit != null)
            {
                UnitExit(ref unit);
            }
        }

        private void OnUnitEnter(ref Unit unit)
        {
            if (UnitEnter != null)
            {
                UnitEnter(ref unit);
            }
        }

        public override bool Equals(object obj)
        {
            return ID == ((Tile)obj).ID;
        }
    }
}
