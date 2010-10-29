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
    public enum UnitType { SpeedBoat, SpeederBoat };
    public enum UnitStatus { Active, Deployed, Inactive };

    public class Unit
    {
        #region Class Variables
        public UnitStatus Status;
        public UnitType Type;

        static int currentID = 0;
        public int ID;

        int previousTileID;
        int goalTileID;
        int focalTileID;

        //Tile previousTile;
       // Tile currentTile;
        //Tile goalTile;
       // Tile focalTile;

        public int CurrentTileID;

        Vector3 focalPoint;
        Vector3 velocity;

        bool isStuck;
        static Random rand = new Random(DateTime.Now.Millisecond);
        public event UnitEvent Died;

        public int Health;
        public int TeamNum;
        public int PlayerNum;
        public int PathLength;

        public Matrix Transforms;
        public float Scale;
        public Vector3 Position;
        private Vector3 normVel;
        static Matrix scaleMatrix;
        static Matrix rotationMatrixX;
        static Matrix rotationMatrixY;
        static Matrix rotationMatrixZ;
        static Matrix scaleRot;
        static  Matrix translation;
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
            normVel = new Vector3();
           //Fucking change this!!
            Position = Vector3.Zero;
            Health = 50;
            ID = currentID;
            currentID++;
            CurrentTileID = 0;
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
            previousTileID = baseTile.ID;
            CurrentTileID = baseTile.ID;
            goalTileID = goalTile.ID;
            
           // this.previousTile = baseTile;
            //this.currentTile = baseTile;
            //this.goalTile = goalTile;
            this.Position = baseTile.Position;

            SetFocalPointAndVelocity(TileMap.Tiles[CurrentTileID].Paths[goalTile.ID][1]);//currentTile.Paths[goalTile.ID][1]);

            Status = UnitStatus.Deployed;

            UpdatePath();
            UpdateTransforms();
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
            SetCurrentTile(TileMap.GetTileFromPos(Position));
            if (CurrentTileID == goalTileID)
                return;
            if (CheckIfStuck())
                return;

            if (!TileMap.Tiles[focalTileID].IsWalkable())
            {
                SetFocalPointAndVelocity(TileMap.Tiles[CurrentTileID].Paths[goalTileID][1]);//currentTile.Paths[goalTile.ID][1]);
            }

            if (TileMap.Tiles[CurrentTileID].TileType == TileType.Blocked)
            {
                if (TileMap.Tiles[previousTileID].Paths[goalTileID].Count > 1)
                {
                    SetFocalPointAndVelocity(TileMap.Tiles[previousTileID].Paths[goalTileID][1]);//previousTile.Paths[goalTile.ID][1]);
                }
                else
                {
                    List<Tile> goodNieghbors = TileMap.GetWalkableNeighbors(TileMap.Tiles[CurrentTileID]);
                    if (goodNieghbors.Count > 0)
                        SetFocalPointAndVelocity(goodNieghbors[0].Paths[goalTileID][1]);
                    else
                        throw new NotImplementedException("NO walkable neighbors... handle this!");
                }

                if (CheckIfStuck())
                    return;
            }

            if (CurrentTileID != previousTileID)
            {
                SetFocalPointAndVelocity(TileMap.Tiles[CurrentTileID].Paths[goalTileID][1]);//currentTile.Paths[goalTile.ID][1]);
            }

            UpdatePositionAndRotation();
        }

        void UpdateTransforms()
        {
            Matrix.CreateTranslation(ref Position, out translation);
            Matrix.Multiply(ref scaleRot, ref translation, out Transforms); 
        }

        void UpdateScaleRotations()
        {
            scaleRot = Matrix.Multiply(scaleMatrix, 
                    Matrix.Multiply(rotationMatrixX, Matrix.Multiply(rotationMatrixY, rotationMatrixZ)));
        }

        void SetCurrentTile(Tile Tile2)
        {
            previousTileID = CurrentTileID;
            CurrentTileID = Tile2.ID;
            
            Unit unit = this;

            if (CurrentTileID != previousTileID)
            {
                TileMap.Tiles[previousTileID].RemoveUnit(ref unit);
                if (CurrentTileID == goalTileID)
                {
                    //Register Hit
                    OnDied();
                }
                else
                {
                    PathLength = TileMap.Tiles[CurrentTileID].Paths[goalTileID].Count;
                    TileMap.Tiles[CurrentTileID].AddUnit(ref unit);
                    
                }
                //CurrentTileID = currentTile.ID;
            }
        }

        Tile GetTile()
        {
            return TileMap.GetTileFromPos(Position);
        }

        bool IsNewTile()
        {
            return CurrentTileID != previousTileID;
        }

        bool CheckIfStuck()
        {
            if (TileMap.Tiles[CurrentTileID].Paths[goalTileID].Count < 1)
            {
                //FIX MATH.ABS CRAP!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                if ((Math.Abs(Position.X - focalPoint.X) < 30 && Math.Abs(Position.Z - focalPoint.Z) < 30)
                    || !TileMap.GetTileFromPos(focalPoint).IsWalkable() || !isStuck)
                {
                    List<Tile> stuckTiles = TileMap.GetWalkableNeighbors(TileMap.Tiles[CurrentTileID]);
                    stuckTiles.Add(TileMap.Tiles[CurrentTileID]);

                    if (stuckTiles.Count == 1 && !TileMap.Tiles[CurrentTileID].IsWalkable())
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
            focalTileID = newTile.ID;
           // focalTile = newTile;
            focalPoint = TileMap.Tiles[focalTileID].GetRandPoint();

            velocity = focalPoint - Position;
            //normVel.X = velocity.X;
            //normVel.Y = velocity.Y;
            //normVel.Z = velocity.Z;
            //normVel.Normalize();

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
                //this.currentTile.RemoveUnit(ref unit);
                OnDied();
            }
        }

        private void OnDied()
        {
            Unit u = this;

            TileMap.Tiles[u.CurrentTileID].RemoveUnit(ref u);

            UnitCollection.Remove(ref u);

            if (Died != null)
                Died(ref u);

            
        }

        public override bool Equals(object obj)
        {
            return ID == ((Unit)obj).ID;
        }
    }
}
