#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.ScreenManagement;
using UHSampleGame.InputManagement;
using UHSampleGame.CoreObjects;
using UHSampleGame.CameraManagement;
#endregion

namespace UHSampleGame.Screens
{
    public class ModelScreen : Screen
    {
        #region Class Variables
        Texture2D background;
        InputManager inputManager;

        StaticModel model;
        float modelRotation;
        CameraManager cameraManager;
        #endregion

        #region Initialization
        public ModelScreen()
            : base("Model")
        {
            background = ScreenManager.Game.Content.Load<Texture2D>("Model\\background");
            inputManager = (InputManager)ScreenManager.Game.Services.GetService(typeof(InputManager));
            model = new StaticModel(ScreenManager.Game.Content.Load<Model>("Model\\box"));
            model.Scale = 200.0f;
            modelRotation = 0.0f;
            
            // Set the position of the camera in world space, for our view matrix.
            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));
            cameraManager.SetPosition(new Vector3(0.0f, 50.0f, 5000.0f));
        }
        #endregion

        #region Update and Draw 
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            cameraManager.Update();

            //rotate model
            modelRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds *
                                MathHelper.ToRadians(0.1f);
            model.RotateX(modelRotation);
            model.RotateZ(modelRotation);

            model.Update(gameTime);
        }

        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);
            if (input.CheckAction(InputAction.Selection))
            {
                screenManager.ShowScreen(new ModelAndText());
            }
            if (input.CheckAction(InputAction.RotateLeft))
            {
                cameraManager.RotateX(-0.03f);
            }
            if (input.CheckAction(InputAction.RotateRight))
            {
                cameraManager.RotateX(0.03f);
            }
            if (input.CheckAction(InputAction.RotateUp))
            {
                cameraManager.RotateY(0.01f);
            }
            if (input.CheckAction(InputAction.RotateDown))
            {
                cameraManager.RotateY(-0.01f);
            }
            if (input.CheckAction(InputAction.StrafeLeft))
            {
                cameraManager.StrafeX(-10.0f);
            }
            if (input.CheckAction(InputAction.StrafeRight))
            {
                cameraManager.StrafeX(10.0f);
            }
            if (input.CheckAction(InputAction.StrafeUp))
            {
                cameraManager.StrafeY(10.0f);
            }
            if (input.CheckAction(InputAction.StrafeDown))
            {
                cameraManager.StrafeY(-10.0f);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            ScreenManager.SpriteBatch.End();

            model.Draw(gameTime);
            base.Draw(gameTime);
        }
        #endregion
    }
}
