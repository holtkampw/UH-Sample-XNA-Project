#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.ScreenManagement;
using UHSampleGame.CoreObjects;
using UHSampleGame.InputManagement;
using UHSampleGame.CameraManagement;
#endregion

namespace UHSampleGame.Screens
{
    public class AnimatedModelScreen : Screen
    {
        #region Class Variables
        Texture2D background;
        AnimatedModel myModel;
        InputManager inputManager;
        CameraManager cameraManager;
        #endregion

        #region Initialization
        public AnimatedModelScreen()
            : base("AnimatedModelScreen")
        {
            background = ScreenManager.Game.Content.Load<Texture2D>("Model\\background");
            myModel = new AnimatedModel(ScreenManager.Game.Content.Load<Model>("AnimatedModel\\dude"));
            myModel.Scale = 30.0f;
            myModel.PlayClip("Take 001");
            inputManager = (InputManager)ScreenManager.Game.Services.GetService(typeof(InputManager));
            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));
            cameraManager.SetPosition(new Vector3(0.0f, 50.0f, 5000.0f));
        }
        #endregion

        #region Update and Draw
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (inputManager.CheckKeyboardAction(InputAction.Selection))
            {
                ScreenManager.Game.Exit();
            }
            else if (inputManager.CheckKeyboardActionPressed(InputAction.RotateLeft))
            {
                cameraManager.RotateX(-0.03f);
            }
            else if (inputManager.CheckKeyboardActionPressed(InputAction.RotateRight))
            {
                cameraManager.RotateX(0.03f);
            }
            else if (inputManager.CheckKeyboardActionPressed(InputAction.RotateUp))
            {
                cameraManager.RotateY(0.01f);
            }
            else if (inputManager.CheckKeyboardActionPressed(InputAction.RotateDown))
            {
                cameraManager.RotateY(-0.01f);
            }
            else if (inputManager.CheckKeyboardActionPressed(InputAction.StrafeLeft))
            {
                cameraManager.StrafeX(-10.0f);
            }
            else if (inputManager.CheckKeyboardActionPressed(InputAction.StrafeRight))
            {
                cameraManager.StrafeX(10.0f);
            }
            else if (inputManager.CheckKeyboardActionPressed(InputAction.StrafeUp))
            {
                cameraManager.StrafeY(10.0f);
            }
            else if (inputManager.CheckKeyboardActionPressed(InputAction.StrafeDown))
            {
                cameraManager.StrafeY(-10.0f);
            }

            cameraManager.Update();
            myModel.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            ScreenManager.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            ScreenManager.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            ScreenManager.SpriteBatch.End();

            ResetRenderStates();
            myModel.Draw(gameTime);

        }
        #endregion

    }
}
