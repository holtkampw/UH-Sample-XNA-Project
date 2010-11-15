using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.TileSystem;
using UHSampleGame.Events;
using UHSampleGame.Players;
using UHSampleGame.ProjectileManagment;
using UHSampleGame.PathFinding;

namespace UHSampleGame.CoreObjects.Units
{
    public enum UnitType { SpeedBoat, SpeederBoat };
    public enum UnitStatus { Active, Deployed, Inactive, Immovable };

    public class Unit
    {
        #region Class Variables
        public UnitStatus Status;
        public UnitType Type;
        bool gameOver = false;
        static int currentID = 0;
        public int ID;
        public int MoneyToGive;

        int previousTileID;
        int goalTileID;
        int focalTileID;

        public int XPToGive;

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
        public int PlayerToAttack;

        float diffX;
        float diffZ;

        public Matrix Transforms;
        public float Scale;
        public Vector3 Position;
        private Vector3 normVel;
        static Matrix scaleMatrix;
        static Matrix rotationMatrixX;
        static Matrix rotationMatrixY;
        static Matrix rotationMatrixZ;
        static Matrix LocalRotationMatrix;
        static Matrix scaleRot;
        static  Matrix translation;
        public int Damage;
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
            LocalRotationMatrix = Matrix.Identity;
            UpdateScaleRotations();
            normVel = new Vector3();
           
            //Fucking change this!!
            Position = Vector3.Zero;
            
            Health = 50;
            ID = currentID;
            currentID++;
            CurrentTileID = 0;

            switch (Type)
            {
                case UnitType.SpeedBoat:
                    Damage = 50;
                    XPToGive = 2;
                    MoneyToGive = 5;
                    break;
                case UnitType.SpeederBoat:
                    Damage = 100;
                    XPToGive = 3;
                    MoneyToGive = 10;
                    break;
                default:
                    Damage = 10;
                    XPToGive = 1;
                    break;
            }
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

        public void Deploy(Tile baseTile, Tile goalTile, int playerToAttack)
        {
            previousTileID = baseTile.ID;
            CurrentTileID = baseTile.ID;
            goalTileID = goalTile.ID;
            PlayerToAttack = playerToAttack;
           // this.previousTile = baseTile;
            //this.currentTile = baseTile;
            //this.goalTile = goalTile;
            this.Position = baseTile.Position;
            int count = 0;
            //lock (AStar2.tileInformationLock)
            lock(AStar2.locks[CurrentTileID][goalTile.ID])
            {
                while (count < 60)
                {
                    if (TileMap.Tiles[CurrentTileID].PathsInts[goalTile.ID].Count > 0)
                    {
                        SetFocalPointAndVelocity(TileMap.Tiles[CurrentTileID].PathsInts[goalTile.ID][1]);//currentTile.Paths[goalTile.ID][1]);
                        break;
                    }
                    count++;

                    if (count >= 60)
                    {
                        return;
                    }
                }

            }
            Status = UnitStatus.Deployed;
            UpdatePath();
            UpdateTransforms();
        }

        public bool IsActive()
        {
            return Status == UnitStatus.Active;
        }

        public bool IsDeployed()
        {
            return Status == UnitStatus.Deployed;
        }

        public bool Update(GameTime gameTime)
        {
            if (!PlayerCollection.CheckFreezeEnemiesFor(this.PlayerNum))
            {
                UpdatePath();
                UpdateTransforms();

                if (gameOver)
                    return true;
            }
            return false;
        }

        public void Draw(GameTime gameTime)
        {

        }

        void UpdatePath()
        {
            SetCurrentTile(TileMap.GetTileFromPos(Position).ID);
            if (CurrentTileID == goalTileID)
                return;
            if (CheckIfStuck())
                return;

            if (!TileMap.Tiles[focalTileID].IsWalkable())
            {
                lock (AStar2.locks[CurrentTileID][goalTileID])
                {
                    SetFocalPointAndVelocity(TileMap.Tiles[CurrentTileID].PathsInts[goalTileID][1]);//currentTile.Paths[goalTile.ID][1]);
                }
            }

            if (TileMap.Tiles[CurrentTileID].TileType == TileType.Blocked)
            {
                if (TileMap.Tiles[previousTileID].PathsInts[goalTileID].Count > 1)
                {
                    lock (AStar2.locks[CurrentTileID][goalTileID])
                    {
                        SetFocalPointAndVelocity(TileMap.Tiles[previousTileID].PathsInts[goalTileID][1]);//previousTile.Paths[goalTile.ID][1]);
                    }
                }
                else
                {
                    List<Tile> goodNieghbors = TileMap.GetWalkableNeighbors(TileMap.Tiles[CurrentTileID]);
                    if (goodNieghbors.Count > 0)
                    {
                        for (int i = 0; i < goodNieghbors.Count; i++)
                        {
                            if (goodNieghbors[i].PathsInts[goalTileID].Count > 0)
                            {
                                lock (AStar2.locks[CurrentTileID][goalTileID])
                                {
                                    SetFocalPointAndVelocity(goodNieghbors[i].PathsInts[goalTileID][1]);
                                }
                                break;
                            }
                        }
                    }
                    else
                    {
                        Health = 0;
                        OnDied();
                        //throw new NotImplementedException("NO walkable neighbors... handle this!");
                    }
                }

                if (CheckIfStuck())
                    return;
            }

            if (CurrentTileID != previousTileID)
            {
                lock (AStar2.locks[CurrentTileID][goalTileID])
                {
                    SetFocalPointAndVelocity(TileMap.Tiles[CurrentTileID].PathsInts[goalTileID][1]);//currentTile.Paths[goalTile.ID][1]);
                }
            }

            UpdatePositionAndRotation();
        }

        void UpdateTransforms()
        {
            Matrix.CreateTranslation(ref Position, out translation);
            UpdateScaleRotations();
            Matrix.Multiply(ref scaleRot, ref translation, out Transforms); 
        }

        void UpdateScaleRotations()
        {
            scaleRot = Matrix.Multiply(scaleMatrix, LocalRotationMatrix);
           // scaleRot = Matrix.Multiply(scaleMatrix, 
            //        Matrix.Multiply(rotationMatrixX, Matrix.Multiply(rotationMatrixY, rotationMatrixZ)));
        }

        // O is your object's position
        // P is the position of the object to face
        // U is the nominal "up" vector (typically Vector3.Y)
        // Note: this does not work when O is straight below or straight above P
        void RotateToFace(Vector3 O, Vector3 P, Vector3 U)
        {
            Vector3 D = (P - O);
            Vector3 Right = Vector3.Cross(U, D);
            Vector3.Normalize(ref Right, out Right);
            Vector3 Backwards = Vector3.Cross(Right, U);
            Vector3.Normalize(ref Backwards, out Backwards);
            Vector3 Up = Vector3.Cross(Backwards, Right);


            //D.Normalize();

            //roll = (float)Math.Asin(D.Z);
            //yaw = (float)Math.Acos(D.X / Math.Cos(roll));
            ////if (D.Y < 0)
            ////    yaw = MathHelper.TwoPi - yaw;
            //ResetYawPitchRoll();

            LocalRotationMatrix.M11 = Right.X;
            LocalRotationMatrix.M12 = Right.Y;
            LocalRotationMatrix.M13 = Right.Z;
            LocalRotationMatrix.M14 = 0;
            LocalRotationMatrix.M21 = Up.X;
            LocalRotationMatrix.M22 = Up.Y;
            LocalRotationMatrix.M23 = Up.Z;
            LocalRotationMatrix.M24 = 0;
            LocalRotationMatrix.M31 = Backwards.X;
            LocalRotationMatrix.M32 = Backwards.Y;
            LocalRotationMatrix.M33 = Backwards.Z;
            LocalRotationMatrix.M34 = 0;
            LocalRotationMatrix.M41 = 0;
            LocalRotationMatrix.M42 = 0;
            LocalRotationMatrix.M43 = 0;
            LocalRotationMatrix.M44 = 1;
        }

        void SetCurrentTile(int tileID)
        {
            if (tileID == -1)
            {
                Health = 0;
                OnDied();
                return;
                //FIX THIS...........................
            }
            previousTileID = CurrentTileID;
            CurrentTileID = tileID;
            
            Unit unit = this;

            if (CurrentTileID != previousTileID)
            {
                TileMap.Tiles[previousTileID].RemoveUnit(ref unit);
                if (CurrentTileID == goalTileID)
                {
                    //Register Hit
                    if (PlayerCollection.AttackPlayer(PlayerToAttack))
                    {
                        gameOver = true;
                        return;
                    }
                    Vector3 nv = new Vector3();
                    nv.X = unit.Position.X;
                    nv.Y = unit.Position.Y + 1;
                    nv.Z = unit.Position.Z;
                    ProjectileManager.AddParticle(unit.Position, nv);
                    OnDied();
                }
                else
                {
                    PathLength = TileMap.Tiles[CurrentTileID].PathsInts[goalTileID].Count;
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
            if (TileMap.Tiles[CurrentTileID].PathsInts[goalTileID].Count < 1)
            {
                //FIX MATH.ABS CRAP!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                diffX = Position.X - focalPoint.X;
                diffZ = Position.Z - focalPoint.Z;
                diffX = diffX >= 0 ? diffX : diffX * -1;
                diffZ = diffZ >= 0 ? diffZ : diffZ * -1;
                if ((diffX < 30 && diffZ < 30)
                    || !TileMap.GetTileFromPos(focalPoint).IsWalkable() || !isStuck)
                {
                    List<Tile> stuckTiles = TileMap.GetWalkableNeighbors(TileMap.Tiles[CurrentTileID]);
                    stuckTiles.Add(TileMap.Tiles[CurrentTileID]);

                    if (stuckTiles.Count == 1 && !TileMap.Tiles[CurrentTileID].IsWalkable())
                    {
                        Health = 0;
                        OnDied();
                        //throw new NotImplementedException("No walkable neighbors with blocked current Tile2... handle this!");
                    }

                    SetFocalPointAndVelocity(stuckTiles[rand.Next(stuckTiles.Count)].ID);
                }

                UpdatePositionAndRotation();
                isStuck = true;
                return true;
            }
            isStuck = false;
            return false;
        }

        void SetFocalPointAndVelocity(int newTileID)
        {
            focalTileID = newTileID;
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
            RotateToFace(Position, focalPoint, Vector3.Up);
            Position += velocity;
            
        }

        public bool TakeDamage(int damage)
        {
            this.Health -= damage;

            if (Health <= 0)
            {
                OnDied();
                return true;
            }
            return false;
        }

        private void OnDied()
        {
            Unit u = this;
            
            TileMap.Tiles[u.CurrentTileID].RemoveUnit(ref u);
            this.Status = UnitStatus.Inactive;
            UnitCollection.Remove(ref u);

            if (Died != null)
                Died(ref u);

            
        }

        public void UpdateTargetPlayer(ref Tile newGoalTile, int newTargetPlayer)
        {
            goalTileID = newGoalTile.ID;
            PlayerToAttack = newTargetPlayer;

            SetFocalPointAndVelocity(TileMap.Tiles[CurrentTileID].PathsInts[newGoalTile.ID][1]);//currentTile.Paths[goalTile.ID][1]);

            Status = UnitStatus.Deployed;

            UpdatePath();
            UpdateTransforms();
        }

        public override bool Equals(object obj)
        {
            return ID == ((Unit)obj).ID;
        }
    }
}
