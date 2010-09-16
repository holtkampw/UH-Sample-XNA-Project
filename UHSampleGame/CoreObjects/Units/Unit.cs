﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.TileSystem;
using UHSampleGame.CoreObjects.Base;

namespace UHSampleGame.CoreObjects.Units
{
    public abstract class Unit : StaticTileObject
    {
        public Vector3 velocity;
        protected Tile previousTile;
        protected Tile currentTile;
        protected Tile goalTile;
        protected Tile focalTile;
        protected Vector3 focalPoint;
        protected bool isStuck;

        public Unit(Model model, Base.Base goalBase)
            : base(model)
        {
            previousTile = new Tile();
            currentTile = new Tile();
            focalPoint = new Vector3();
            focalTile = new Tile();
            goalTile = goalBase.Tile;
            isStuck = false;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            previousTile = currentTile;
            currentTile = GetTile();

            if (CheckIfStuck())
                return;

            if (!focalTile.IsWalkable())
            {
                SetFocalPointAndVelocity(currentTile.Paths[goalTile.ID][1]);
            }

            if (currentTile.GetTileType() == TileType.Blocked)
            {
                if (previousTile.IsWalkable())
                {
                    currentTile = previousTile;
                }
                else
                {
                    List<Tile> goodNieghbors = TileMap.GetWalkableNeighbors(currentTile);
                    if (goodNieghbors.Count > 0)
                        currentTile = goodNieghbors[0];
                    else
                        throw new NotImplementedException("NO walkable neighbors... handle this!");
                }
                position = currentTile.GetRandPoint();

                if (CheckIfStuck())
                    return;

                SetFocalPointAndVelocity(currentTile.Paths[goalTile.ID][1]);
            }

            if (IsNewTile())
            {
                SetFocalPointAndVelocity(currentTile.Paths[goalTile.ID][1]);
            }

            this.position += velocity;
        }

        private bool  CheckIfStuck()
        {
            if (currentTile.Paths[goalTile.ID].Count < 1)
            {
                if ((Math.Abs(position.X - focalPoint.X) < 30 && Math.Abs(position.Z - focalPoint.Z) < 30) 
                    || !TileMap.GetTileFromPos(focalPoint).IsWalkable() || !isStuck)
                {
                    List<Tile> stuckTiles = new List<Tile>(TileMap.GetWalkableNeighbors(currentTile));
                    stuckTiles.Add(currentTile);

                    if (stuckTiles.Count == 1 && !currentTile.IsWalkable())
                    {
                        throw new NotImplementedException("No walkable neighbors with blocked current tile... handle this!");
                    }

                    Random rand = new Random(DateTime.Now.Millisecond);

                    SetFocalPointAndVelocity(stuckTiles[rand.Next(stuckTiles.Count)]);
                }

                this.position += velocity;
                isStuck = true;
                //base.Update(gameTime);
                return true;
            }
            isStuck = false;
            return false;
        }

        private void SetFocalPointAndVelocity(Tile newTile)
        {
            focalTile = newTile;
            focalPoint = focalTile.GetRandPoint();

            velocity = new Vector3(focalPoint.X - position.X, focalPoint.Y-position.Y, focalPoint.Z - position.Z);
            this.RotateY(velocity.Y);
            velocity.Normalize();
            Vector3.Multiply(velocity, 3);
        }

        private bool IsNewTile()
        {
            return currentTile != previousTile;
        }
    }
}
