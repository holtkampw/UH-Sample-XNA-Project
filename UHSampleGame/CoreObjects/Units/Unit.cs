#region Using Statements

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.TileSystem;
using UHSampleGame.CoreObjects.Base;
using UHSampleGame.Events;

using UHSampleGame.CameraManagement;
using UHSampleGame.ScreenManagement;
#endregion

namespace UHSampleGame.CoreObjects.Units
{
    public enum UnitType { TestUnit };
    public enum UnitStatus { Active, Deployed, Inactive };

    public class Unit
    {
        //Type Information
        public static Dictionary<int, Model> Models = new Dictionary<int, Model>()
        {
            {(int)UnitType.TestUnit, ScreenManager.Game.Content.Load<Model>("Objects\\Units\\boat")}
        };

        public static Enum[] unitEnumType = EnumHelper.EnumToArray(new UnitType());

        //TeamableStaticObject
        public int PlayerNum;
        public int TeamNum;

        //StaticModel
        protected float scale;
        CameraManager cameraManager;

        //StaticModel
        Matrix view;
        public Matrix Transforms;
        Matrix[] boneTransforms;
        Matrix rotationMatrixX;
        Matrix rotationMatrixY;
        Matrix rotationMatrixZ;

        //GameObject
        protected Vector3 Position;

        //Unit
        public UnitType[] UnitTypes = { UnitType.TestUnit };
        public Vector3 velocity;
        protected Tile previousTile;
        protected Tile currentTile;
        protected Tile goalTile;
        protected Tile focalTile;
        protected Vector3 focalPoint;
        protected bool isStuck;
        public int Health;
        protected int pathLength;
        public UnitType Type;
        protected float rotation;
        protected float currentRotation = 0;
        public bool Alive;
        public int Index;
        public UnitStatus Status;
        public event UnitDied Died;

        public Unit(int playerNum, UnitType unitType)
        {
            this.Alive = false;
            Status = UnitStatus.Inactive;

            previousTile = new Tile();
            currentTile = new Tile();
            focalPoint = new Vector3();
            focalTile = new Tile();

            goalTile = null;
            isStuck = false;
            Health = 100;

            pathLength = 0;

            this.PlayerNum = 0;
            this.TeamNum = 0;
        }

        public Unit(UnitType newType, int playerNum, int teamNum, Vector3 position, Base.Base goalBase)
        {
            Status = UnitStatus.Inactive;
            previousTile = new Tile();
            currentTile = new Tile();
            focalPoint = new Vector3();
            focalTile = new Tile();
            
            goalTile = goalBase.Tile;
            isStuck = false;
            Health = 100;

            pathLength = 0;

            this.PlayerNum = playerNum;
            this.TeamNum = teamNum;

            this.Type = newType;
                
            foreach(UnitType type in unitEnumType)
            {
                switch(type)
                {
                    case UnitType.TestUnit:
                        this.Position = position;
                        this.scale = 5.0f;
                        break;
                }
            }

            SetupModel(position);
            SetupCamera();
            Alive = true;
        }

        public void Setup(UnitType newType, int index, int playerNum, int teamNum, Vector3 position, Base.Base goalBase)
        {
            goalTile = goalBase.Tile;
            isStuck = false;
            Health = 100;
            this.Index = index;

            pathLength = 0;

            this.PlayerNum = playerNum;
            this.TeamNum = teamNum;

            this.Type = newType;

            foreach (UnitType type in unitEnumType)
            {
                switch (type)
                {
                    case UnitType.TestUnit:
                        this.Position = position;
                        this.scale = 5.0f;
                        break;
                }
            }

            SetupModel(position);
            SetupCamera();
            Alive = true;

        }

        public void Update(GameTime gameTime)
        {
            UpdatePath();
            UpdateView();
            UpdateTransforms();
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

            UpdatePositionAndRotation();
        }

        private bool  CheckIfStuck()
        {
            if (currentTile.Paths[goalTile.ID].Count < 1)
            {
                if ((Math.Abs(Position.X - focalPoint.X) < 30 && Math.Abs(Position.Z - focalPoint.Z) < 30) 
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

                UpdatePositionAndRotation();
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

            velocity = focalPoint - Position;// new Vector3(focalPoint.X - position.X, focalPoint.Y - position.Y, focalPoint.Z - position.Z);
            Vector3 normVel = new Vector3(velocity.X, velocity.Y, velocity.Z);
            normVel.Normalize();

            //currentRotation = 0;
            //rotation = (float)Math.Atan2(velocity.X, velocity.Z);
            
            this.RotateY((float)Math.Atan2(velocity.X, velocity.Z));
            velocity.Normalize();
            
        }

        private void UpdatePositionAndRotation()
        {
            this.Position += velocity;
            //currentRotation += rotation;
           
            //this.RotateY( MathHelper.Lerp(currentRotation, rotation, 0.6f));
        }

        private bool IsNewTile()
        {
            return currentTile != previousTile;
        }

        private void SetCurrentTile(Tile tile)
        {
            currentTile.RemoveUnit(this.Type, this);
            currentTile = tile;
            if (currentTile == goalTile)
            {
                //Register Hit
                OnDied();
            }
            else
            {
                pathLength = currentTile.Paths[goalTile.ID].Count;
                currentTile.AddUnit(this.Type, this);
            }
        }

        public int GetPathLength()
        {
            return pathLength; // currentTile.Paths[goalTile.ID].Count;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0)
                OnDied();
        }

        private void OnDied()
        {
            if (Died != null)
                Died(this.Type, this);
        }

        public override string ToString()
        {
            return this.Position.ToString();
        }

        public List<Tile> PathToGoal()
        {
            return currentTile.Paths[goalTile.ID];
        }

        #region TeamableStaticObject
        public void SetPlayerNum(int newPlayerNum)
        {
            PlayerNum = newPlayerNum;
        }

        public void SetTeamNum(int newTeamNum)
        {
            TeamNum = newTeamNum;
        }
        #endregion

        #region StaticTileObject
        public Tile GetTile()
        {
            return TileMap.GetTileFromPos(Position);
        }
        #endregion

        #region StaticModel
        protected void SetupModel(Vector3 position)
        {
            //save bones
            switch(Type)
            {
                case UnitType.TestUnit:
                 boneTransforms = new Matrix[Models[(int)Type].Bones.Count];
                 Models[(int)Type].CopyAbsoluteBoneTransformsTo(boneTransforms);
                 break;
            }
            
            //setup transforms
            Transforms =  Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);

            //give default rotation
            rotationMatrixX = Matrix.CreateRotationX(0.0f);
            rotationMatrixY = Matrix.CreateRotationY(0.0f);
            rotationMatrixZ = Matrix.CreateRotationZ(0.0f);

            //give default position
            this.Position = position;
        }

        /// <summary>
        /// Sets up default camera information
        /// </summary>
        protected void SetupCamera()
        {
            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));
            view = cameraManager.ViewMatrix;
        }

        public void RotateX(float rotation)
        {
            rotationMatrixX = Matrix.CreateRotationX(rotation);
        }

        public void RotateY(float rotation)
        {
            rotationMatrixY = Matrix.CreateRotationY(rotation);
        }

        public void RotateZ(float rotation)
        {
            rotationMatrixZ = Matrix.CreateRotationZ(rotation);
        }

        public void UpdateView()
        {
            view = cameraManager.ViewMatrix;
        }

        public void UpdateTransforms()
        {
            Matrix scaleMatrix = Matrix.CreateScale(this.scale);
            Matrix translation = Matrix.CreateTranslation(Position);
            Transforms = scaleMatrix *
                    rotationMatrixX *
                    rotationMatrixY *
                    rotationMatrixZ * 
                    translation;
        }

        public void Draw(GameTime time)
        {
            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in Models[(int)Type].Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = boneTransforms[mesh.ParentBone.Index] * Transforms;
                    effect.View = cameraManager.ViewMatrix;
                    effect.Projection = cameraManager.ProjectionMatrix;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

        }


        #endregion

        #region GameObject
        public void SetPosition(Vector3 position)
        {
            this.Position = position;
        }
        #endregion
    }
}
