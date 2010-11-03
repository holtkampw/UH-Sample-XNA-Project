using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.TileSystem;
using UHSampleGame.ScreenManagement;
using UHSampleGame.Events;
using UHSampleGame.CameraManagement;
using UHSampleGame.ProjectileManagment;

namespace UHSampleGame.CoreObjects.Base
{
    public enum BaseType { type1 };

    public class Base
    {
        protected Base goalBase;
        protected Tile tile;
        protected int health;
        public int PlayerNum;
        public int TeamNum;
        protected float scale;

        CameraManager cameraManager;

        //Model Stuff
        Matrix view;
        Matrix transforms;
        Matrix[] boneTransforms;
        Matrix rotationMatrixX;
        Matrix rotationMatrixY;
        Matrix rotationMatrixZ;
        Model model;

        public event BaseDestroyed baseDestroyed;

        public Base GoalBase
        {
            get { return goalBase; }
        }  

        public Vector3 Position;

        public Tile Tile
        {
            get { return tile; }
        }

        public int Health
        {
            get { return health; }
        }

        public Base(int playerNum, int teamNum, BaseType baseType, Tile tile)
        {
            PlayerNum = playerNum;
            TeamNum = teamNum;
            this.Scale = 2.75f;
            this.tile = tile;
            this.Position = tile.Position;

            this.model = ScreenManager.Game.Content.Load<Model>("Objects\\Base\\oilRig");
            SetupModel(Position);
            SetupCamera();

        }

        public void SetGoalBase(Base goalBase)
        {
            this.goalBase = goalBase;
        }

        public void HitBase(int damage)
        {
            health -= damage;

            if (health <= 0)
                OnBaseDestroyed();
        }

        protected void OnBaseDestroyed()
        {
            if (baseDestroyed != null)
                baseDestroyed(this);
        }

        public void SetPlayerNum(int newPlayerNum)
        {
            PlayerNum = newPlayerNum;
        }

        public void SetTeamNum(int newTeamNum)
        {
            TeamNum = newTeamNum;
        }

        public Tile GetTile()
        {
            return TileMap.GetTileFromPos(Position);
        }

        public void SetupModel(Model newModel, Vector3 position)
        {
            this.model = newModel;
            SetupModel(position);
            SetupCamera();
        }

        protected void SetupModel(Vector3 position)
        {
            //set scale
            scale = 2.75f;

            //save bones
            if (model != null)
            {
                 boneTransforms = new Matrix[model.Bones.Count];
                 model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            }
            
            //setup transforms
            transforms =  Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);

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

        #region Properties
        public Matrix Transforms
        {
            get { return transforms; }
        }

        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                SetupCamera();
            }
        }
        #endregion

        #region Manipulation
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
        #endregion

        #region Update and Draw
        public void Update(GameTime gameTime)
        {
            //update view matrix
            UpdateView();
            UpdateTransforms();
        }

        public void UpdateView()
        {
            view = cameraManager.ViewMatrix;
        }

        public void UpdateTransforms()
        {
            transforms = Matrix.CreateScale(scale) *
                    //rotationMatrixX *
                    rotationMatrixY *
                    //rotationMatrixZ * 
                    Matrix.CreateTranslation(Position);
        }

        public void Draw(GameTime gameTime)
        {
            if (model != null)
            {
                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in model.Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = boneTransforms[mesh.ParentBone.Index] * transforms;
                        effect.View = cameraManager.ViewMatrix;
                        effect.Projection = cameraManager.ProjectionMatrix;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }


            }
        }
        #endregion


        internal void Destroy()
        {
            Vector3 nv = new Vector3();
            nv.X = Position.X;
            nv.Y = Position.Y+5;
            nv.Z = Position.Z;
            ProjectileManager.AddParticle(Position, Position);
        }
    }
}
