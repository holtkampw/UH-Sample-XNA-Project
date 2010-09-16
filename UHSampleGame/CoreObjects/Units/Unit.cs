using System;
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
        private Tile previousTile;
        private Tile currentTile;
        private Tile goalTile;
        private Vector3 focalPoint;

        public Unit(Model model, Base.Base goalBase)
            : base(model)
        {
            previousTile = new Tile();
            currentTile = new Tile();
            focalPoint = new Vector3();
            goalTile = goalBase.Tile;
        }

        public override void Update(GameTime gameTime)
        {
            previousTile = currentTile;
            currentTile = GetTile();

            if (currentTile.Paths[goalTile.ID].Count < 1)
                return;

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
                SetFocalPointAndVelocity();
            }

            if (IsNewTile())
            {
                SetFocalPointAndVelocity();
            }

            this.position += velocity;
            base.Update(gameTime);
        }

        private void SetFocalPointAndVelocity()
        {
            focalPoint = currentTile.Paths[goalTile.ID][1].GetRandPoint();
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
