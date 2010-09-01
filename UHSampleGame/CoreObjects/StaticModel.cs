#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.CameraManagement;
using UHSampleGame.ScreenManagement;
#endregion

namespace UHSampleGame.CoreObjects
{
    public class StaticModel : GameObject
    {
        #region Class Variables
        Model model;
        protected float scale;
        CameraManager cameraManager;

        //Model Stuff
        Matrix view;
        Matrix[] transforms;
        Matrix rotationMatrixX;
        Matrix rotationMatrixY;
        Matrix rotationMatrixZ;
        #endregion

        #region Initialization
        /// <summary>
        /// Default Constructor to setup Model
        /// </summary>
        public StaticModel()
        {
            model = null;
        }

        /// <summary>
        /// Constructor consisting of a given model
        /// </summary>
        /// <param name="model">Model for use</param>
        public StaticModel(Model model)
        {
            this.model = model;
            SetupModel();
            SetupCamera();
        }

        /// <summary>
        /// Adds a model to the Static Model and performs setup
        /// </summary>
        /// <param name="model">Model for this instance</param>
        public void SetupModel(Model model)
        {
            this.model = model;
            SetupModel();
            SetupCamera();
        }

        protected void SetupModel()
        {
            //set scale
            scale = 1.0f;

            //save bones
            transforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(transforms);

            //give default rotation
            rotationMatrixX = Matrix.CreateRotationX(0.0f);
            rotationMatrixY = Matrix.CreateRotationY(0.0f);
            rotationMatrixZ = Matrix.CreateRotationZ(0.0f);

            //give default position
            position = Vector3.Zero;
        }

        /// <summary>
        /// Sets up default camera information
        /// </summary>
        protected void SetupCamera()
        {
            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));
            view = Matrix.CreateTranslation(0, 0, 0) * Matrix.CreateScale(scale) * cameraManager.ViewMatrix;
        }
        #endregion

        #region Properties
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

            base.Update(gameTime);
        }

        public void UpdateView()
        {
            view = Matrix.CreateTranslation(position) *
                    rotationMatrixX *
                    rotationMatrixY *
                    rotationMatrixZ *
                    Matrix.CreateScale(scale) *
                    cameraManager.ViewMatrix;
        }

        public override void Draw(GameTime gameTime)
        {
            // Draw all of the meshes for a model
            foreach (ModelMesh mesh in model.Meshes)
            {
                //each mesh has an effect
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    //Where will the model be in the world?
                    effect.World = transforms[mesh.ParentBone.Index];

                    //How are we viewing it?
                    effect.View = view;

                    //Projection information
                    effect.Projection = cameraManager.ProjectionMatrix;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }

            base.Draw(gameTime);
        }
        #endregion
    }
}
