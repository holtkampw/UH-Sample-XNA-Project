using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.TileSystem;
using UHSampleGame.CoreObjects.Base;
using UHSampleGame.Events;

namespace UHSampleGame.CoreObjects.Units
{
    public enum UnitType { TestUnit };

    public abstract class Unit : TeamableStaticObject
    {
        public Vector3 velocity;
        protected Tile previousTile;
        protected Tile currentTile;
        protected Tile goalTile;
        protected Tile focalTile;
        protected Vector3 focalPoint;
        protected bool isStuck;
        protected int health;
        protected int pathLength;
        protected UnitType type;

        public int Health
        {
            get { return health; }
        }

        public UnitType Type
        {
            get { return type; }
        }

        public event UnitDied Died;

        public Unit(int playerNum, int teamNum, Model model, Base.Base goalBase, Vector3 position)
            : base(playerNum, teamNum, model, position)
        {
            previousTile = new Tile();
            currentTile = new Tile();
            focalPoint = new Vector3();
            focalTile = new Tile();
            
            goalTile = goalBase.Tile;
            isStuck = false;
            health = 100;

            pathLength = 0;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            UpdatePath();
        }

        public void UpdatePath()
        {
            previousTile = currentTile;
            SetCurrentTile(GetTile());

            if (CheckIfStuck())
                return;

            if (!focalTile.IsWalkable())
            {
                SetFocalPointAndVelocity(currentTile.Paths[goalTile.ID][1]);
            }

            if (currentTile.GetTileType() == TileType.Blocked)
            {
                if (previousTile.Paths[goalTile.ID].Count > 1)
                {
                    SetFocalPointAndVelocity(previousTile.Paths[goalTile.ID][1]);
                }
                else
                {
                    List<Tile> goodNieghbors = TileMap.GetWalkableNeighbors(currentTile);
                    if (goodNieghbors.Count > 0)
                        SetFocalPointAndVelocity(goodNieghbors[0].Paths[goalTile.ID][1]);
                    else
                        throw new NotImplementedException("NO walkable neighbors... handle this!");
                }

                if (CheckIfStuck())
                    return;
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

        private void SetCurrentTile(Tile tile)
        {
            currentTile.RemoveUnit(this.type, this);
            currentTile = tile;
            pathLength = currentTile.Paths[goalTile.ID].Count;
            currentTile.AddUnit(this.type, this);
        }

        public int GetPathLength()
        {
            return pathLength; // currentTile.Paths[goalTile.ID].Count;
        }

        public void TakeDamage(int damage)
        {
            health -= damage;
            if (health <= 0)
                OnDied();
        }

        private void OnDied()
        {
            if (Died != null)
                Died(this.type, this);
        }

        public override string ToString()
        {
            return this.position.ToString();
        }

        public List<Tile> PathToGoal()
        {
            return currentTile.Paths[goalTile.ID];
        }
    }
}
