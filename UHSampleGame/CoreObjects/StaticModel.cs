#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.CameraManagement;
using UHSampleGame.ScreenManagement;
#endregion

namespace UHSampleGame.CoreObjects

{
    public class StaticModel : GameObject
    {
        #region Class Variables
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
        #endregion

        #region Initialization
        /// <summary>
        /// Default Constructor to setup Model
        /// </summary>
        public StaticModel(Vector3 position)
        {
             model = null;
             this.position = position;
             SetupModel(position);
             SetupCamera();
        }

        /// <summary>
        /// Constructor consisting of a given model
        /// </summary>
        /// <param name="model">Model for use</param>
        public StaticModel(Model newModel, Vector3 position)
        {
            this.model = newModel;
            SetupModel(position);
            SetupCamera();
        }

        /// <summary>
        /// Adds a model to the Static Model and performs setup
        /// </summary>
        /// <param name="model">Model for this instance</param>
        public void SetupModel(Model newModel, Vector3 position)
        {
            this.model = newModel;
            SetupModel(position);
            SetupCamera();
        }

        protected void SetupModel(Vector3 position)
        {
            //set scale
            scale = 1.0f;

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
            this.position = position;
        }

        /// <summary>
        /// Sets up default camera information
        /// </summary>
        protected void SetupCamera()
        {
            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));
            view = cameraManager.ViewMatrix;
        }
        #endregion

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
        public override void Update(GameTime gameTime)
        {
            //update view matrix
            UpdateView();
            UpdateTransforms();

            base.Update(gameTime);
        }

        public void UpdateView()
        {
            view = cameraManager.ViewMatrix;
        }

        public void UpdateTransforms()
        {
            transforms = Matrix.CreateScale(scale) *
                    Matrix.CreateTranslation(position);// *
                    //rotationMatrixX *
                    //rotationMatrixY *
                    //rotationMatrixZ;
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
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
    }
}
