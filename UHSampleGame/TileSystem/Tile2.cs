﻿using System;
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
   // public enum TileType { Walkable, Blocked, Path, Null }

    public class Tile2
    {
        public Vector3 Position;
        public Vector2 Size;
        public TileType TileType;
        Tower2 tower;
        List<Unit2> units;
        public List<List<Tile2>> Paths;
        Random rand;
        public int ID;

        public static Tile2 NullTile = new Tile2();

        public event RegisterUnitWithTile2 UnitEnter;

        /// <summary>
        /// Represents a Tile2 of a Tile2 map
        /// </summary>
        /// <param name="position">The center position of the Tile2</param>
        /// <param name="size">The width and length of the Tile2</param>
        public Tile2(int id, Vector3 position, Vector2 size)
            : this(id, position, size, TileType.Walkable) { }

        public Tile2()
            : this(-1, Vector3.Zero, Vector2.Zero, TileType.Null) { }

        public Tile2(int id, Vector3 position, Vector2 size, TileType tileType)
        {
            //this.rand = new Random(DateTime.Now.Millisecond);
            this.ID = id;
            this.Position = position;
            this.Size = size;
            this.TileType = tileType;
            this.Paths = new List<List<Tile2>>();
            this.units = new List<Unit2>();

            SetTileType(tileType);

            rand = new Random(DateTime.Now.Millisecond);

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

        public void SetBlockableObject(Tower2 gameObject)
        {
            this.tower = gameObject;
            SetTileType(TileType.Blocked);
        }

        public void RemoveBlockableObject()
        {
            this.tower = null;
            SetTileType(TileType.Walkable);
        }

        public List<Tile2> GetPathTo(Tile2 baseTile)
        {
            UpdatePathTo(baseTile);
            return Paths[baseTile.ID];
        }

        public void UpdatePathTo(Tile2 baseTile)
        {
            AStar2 aStar = new AStar2(this, baseTile);

            for (int i = Paths.Count; i <= baseTile.ID; i++)
                Paths.Add(new List<Tile2>());
            Paths[baseTile.ID] = new List<Tile2>(aStar.FindPath());
        }

        public Vector3 GetRandPoint()
        {
            //rand = new Random(DateTime.Now.Millisecond);
            int sizeX = (int)(Size.X / 3);
            int sizeY = (int)(Size.Y / 3);
            return new Vector3(Position.X + rand.Next(-sizeX, sizeX), 0/*rand.Next(-10, 10)*/, Position.Z + rand.Next(-sizeY, sizeY));
        }

        public void RegisterTowerListener(ref Tower2 tower)
        {
            UnitEnter += tower.RegisterAttackUnit;
        }

        public void UnregisterTowerListener(ref Tower2 tower)
        {
            UnitEnter -= tower.RegisterAttackUnit;
        }

        public void AddUnit(UnitType type, ref Unit2 unit)
        {
            units.Add(unit);
            //unit.Died += RemoveUnit;
            //OnUnitEnter(new GameEventArgs(unit));
        }

        public void RemoveUnit(UnitType type, ref Unit2 unit)
        {
            units.Remove(unit);
            //Set new unit to attack
           // if (units.Count > 0)
              //  OnUnitEnter(new GameEventArgs(units[0]));
        }

        private void OnUnitEnter(GameEventArgs2 args)
        {
            if (UnitEnter != null)
            {
                UnitEnter(args);
            }
        }

        //public override bool Equals(object obj)
        //{
        //    return id == ((Tile2)obj).id;
        //}
    }
}
