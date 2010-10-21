using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.TileSystem;
using UHSampleGame.Events;

namespace UHSampleGame.CoreObjects.Units
{
    public enum UnitType { TestUnit };
    public enum UnitStatus { Active, Deployed, Inactive };

    public class Unit
    {
        #region Class Variables
        public UnitStatus Status;
        UnitType Type;

        Tile previousTile;
        Tile currentTile;
        Tile goalTile;
        Tile focalTile;

        Vector3 focalPoint;
        Vector3 velocity;

        bool isStuck;
        Random rand;
        public event UnitDied Died;

        public int Health;
        public int TeamNum;
        public int PlayerNum;
        public int PathLength;

        public Matrix Transforms;
        public float Scale;
        public Vector3 Position;
        Matrix scaleMatrix;
        Matrix rotationMatrixX;
        Matrix rotationMatrixY;
        Matrix rotationMatrixZ;
        Matrix scaleRot;
        #endregion

        public Unit(UnitType unitType)
        {
            Status = UnitStatus.Inactive;
            Type = unitType;
            Scale = 1.5f;
            scaleMatrix = Matrix.CreateScale(this.Scale);
            rotationMatrixX = Matrix.Identity;
            rotationMatrixY = Matrix.Identity;
            rotationMatrixZ = Matrix.Identity;
            UpdateScaleRotations();
           //Fucking change this!!
            Position = Vector3.Zero;
            Health = 50;

            rand = new Random(DateTime.Now.Millisecond);
        }

        #region Matrix Setters
        public void SetScale(float newScale)
        {
            this.Scale = newScale;
            scaleMatrix = Matrix.CreateScale(this.Scale);
            UpdateScaleRotations();
        }

        public void RotateX(float rotation)
        {
            rotationMatrixX = Matrix.CreateRotationX(rotation);
            UpdateScaleRotations();
        }

        public void RotateY(float rotation)
        {
            rotationMatrixY = Matrix.CreateRotationY(rotation);
            UpdateScaleRotations();
        }

        public void RotateZ(float rotation)
        {
            rotationMatrixZ = Matrix.CreateRotationZ(rotation);
            UpdateScaleRotations();
        }
        #endregion

        public void Activate()
        {
            Status = UnitStatus.Active; 
        }

        public void Deploy(Tile baseTile, Tile goalTile)
        {
            this.previousTile = baseTile;
            this.currentTile = baseTile;
            this.goalTile = goalTile;
            this.Position = baseTile.Position;

            SetFocalPointAndVelocity(currentTile.Paths[goalTile.ID][1]);

            Status = UnitStatus.Deployed;
        }

        public bool IsActive()
        {
            return Status != UnitStatus.Inactive;
        }

        public void Update(GameTime gameTime)
        {
            UpdatePath();
            UpdateTransforms();
        }

        public void Draw(GameTime gameTime)
        {

        }

        void UpdatePath()
        {
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

            UpdatePositionAndRotation();
        }

        void UpdateTransforms()
        {
            Matrix translation = Matrix.CreateTranslation(Position);
            Transforms = Matrix.Multiply(scaleRot, translation); 
        }

        void UpdateScaleRotations()
        {
            scaleRot = Matrix.Multiply(scaleMatrix, 
                    Matrix.Multiply(rotationMatrixX, Matrix.Multiply(rotationMatrixY, rotationMatrixZ)));
        }

        void SetCurrentTile(Tile Tile2)
        {
            previousTile = currentTile;
            currentTile = Tile2;

            if (IsNewTile())
            {
                previousTile.RemoveUnit(Type, this);
                if (currentTile == goalTile)
                {
                    //Register Hit
                   // OnDied();
                }
                else
                {
                    PathLength = currentTile.Paths[goalTile.ID].Count;
                    currentTile.AddUnit(Type, this);
                }
            }
        }

        Tile GetTile()
        {
            return TileMap.GetTileFromPos(Position);
        }

        bool IsNewTile()
        {
            return currentTile != previousTile;
        }

        bool CheckIfStuck()
        {
            if (currentTile.Paths[goalTile.ID].Count < 1)
            {
                if ((Math.Abs(Position.X - focalPoint.X) < 30 && Math.Abs(Position.Z - focalPoint.Z) < 30)
                    || !TileMap.GetTileFromPos(focalPoint).IsWalkable() || !isStuck)
                {
                    List<Tile> stuckTiles = TileMap.GetWalkableNeighbors(currentTile);
                    stuckTiles.Add(currentTile);

                    if (stuckTiles.Count == 1 && !currentTile.IsWalkable())
                    {
                        throw new NotImplementedException("No walkable neighbors with blocked current Tile2... handle this!");
                    }

                    SetFocalPointAndVelocity(stuckTiles[rand.Next(stuckTiles.Count)]);
                }

                UpdatePositionAndRotation();
                isStuck = true;
                return true;
            }
            isStuck = false;
            return false;
        }

        void SetFocalPointAndVelocity(Tile newTile)
        {
            focalTile = newTile;
            focalPoint = focalTile.GetRandPoint();

            velocity = focalPoint - Position;
            Vector3 normVel = new Vector3(velocity.X, velocity.Y, velocity.Z);
            normVel.Normalize();

            //this.RotateY((float)Math.Atan2(velocity.X, velocity.Z));
            velocity.Normalize();

        }

        void UpdatePositionAndRotation()
        {
            Position += velocity;
        }

        public void TakeDamage(int damage)
        {
            this.Health -= damage;
            if (Health <= 0)
            {
                this.currentTile.RemoveUnit(Type, this);
                OnDied();
            }
        }

        private void OnDied()
        {
            if (Died != null)
                Died(this.Type, this);

            this.Status = UnitStatus.Inactive;
        }
    }
}
