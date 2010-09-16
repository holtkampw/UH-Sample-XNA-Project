#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.ScreenManagement;
using UHSampleGame.CameraManagement;
using SkinnedModel;
#endregion

namespace UHSampleGame.CoreObjects
{
    public class AnimatedModel : GameObject
    {
        #region Class Variables
        protected AnimationPlayer animationPlayer;
        protected Model model;
        SkinningData skinningData;
        Matrix[] bones;
        protected float scale;
        CameraManager cameraManager;

        //animation stuff
        Matrix view;
        #endregion

        #region Properties
        public float Scale
        {
            get { return scale; }
            set { 
                scale = value;
                SetupCamera();
            }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Default Constructor to setup Model
        /// </summary>
        public AnimatedModel() : base()
        {
            model = null;
        }

        /// <summary>
        /// Constructor consisting of a given model
        /// </summary>
        /// <param name="model">Model for use</param>
        public AnimatedModel(Model model) : base()
        {
            this.model = model;
            SetupModel();
            SetupCamera();
        }

        /// <summary>
        /// Adds a model to the Animated Model and performs setup
        /// </summary>
        /// <param name="model">Model for this instance</param>
        public void SetupModel(Model model)
        {
            this.model = model;
            SetupModel();
            SetupCamera();
        }

        /// <summary>
        /// Performs required setup sequence for model
        /// </summary>
        protected void SetupModel()
        {
            //Get texture animation data
            skinningData = model.Tag as SkinningData;

            //Double check the model
            if (skinningData == null)
                throw new InvalidOperationException
                    ("This model does not contain a SkinningData tag.");

            // Create an animation player, and start decoding an animation clip.
            animationPlayer = new AnimationPlayer(skinningData);

            //get location of bones
            bones = animationPlayer.GetSkinTransforms();

            scale = 1.0f;
        }

        /// <summary>
        /// Sets up default camera information
        /// </summary>
        protected void SetupCamera()
        {
            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));
            view = Matrix.CreateTranslation(position) * Matrix.CreateScale(scale) * cameraManager.ViewMatrix;
        }
        #endregion

        #region Animation
        /// <summary>
        /// Play a particular animation
        /// </summary>
        /// <param name="take">Animation to play</param>
        public void PlayClip(string take)
        {
            AnimationClip clip = skinningData.AnimationClips[take];
            animationPlayer.StartClip(clip);
        }
        #endregion

        #region Update and Draw
        public override void Update(GameTime gameTime)
        {
            //update view matrix
            UpdateView();

            //update animation
            animationPlayer.Update(gameTime.ElapsedGameTime, true, Matrix.Identity);

            //update bones
            bones = animationPlayer.GetSkinTransforms();

            base.Update(gameTime);
        }

        public void UpdateView()
        {
            view = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position) *
                    
                    cameraManager.ViewMatrix;
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.Parameters["Bones"].SetValue(bones);
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(cameraManager.ProjectionMatrix);
                }
                mesh.Draw();
            }

            base.Draw(gameTime);
        }
        #endregion
    }
}
