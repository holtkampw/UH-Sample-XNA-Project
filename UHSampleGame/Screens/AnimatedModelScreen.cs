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
        StaticModel ground;
        InputManager inputManager;
        CameraManager cameraManager;
        
        Vector2 center;
        SpriteFont font;
        string text;
        Vector2 textPosition;
        #endregion

        #region Initialization
        public AnimatedModelScreen()
            : base("AnimatedModelScreen")
        {
            background = ScreenManager.Game.Content.Load<Texture2D>("Model\\background");
            myModel = new AnimatedModel(ScreenManager.Game.Content.Load<Model>("AnimatedModel\\dude"));
            myModel.Scale = 20.0f;
            myModel.PlayClip("Take 001");
            inputManager = (InputManager)ScreenManager.Game.Services.GetService(typeof(InputManager));
            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));
            cameraManager.SetPosition(new Vector3(0.0f, 50.0f, 5000.0f));

            ground = new StaticModel(ScreenManager.Game.Content.Load<Model>("Model\\pyramids"));
            ground.Scale = 1000.0f;
            ground.SetPosition(new Vector3(0.0f, -0.1f, 0.0f));

            #region Setup Text
            font = ScreenManager.Game.Content.Load<SpriteFont>("DummyText\\Font");

            center = new Vector2((ScreenManager.GraphicsDeviceManager.PreferredBackBufferWidth / 2),
                                 (ScreenManager.GraphicsDeviceManager.PreferredBackBufferHeight / 2));
            //Setup Text
            text = "Hello World! Hello World! Hellllllooooo World!";

            //Find out how long the text is using this font
            Vector2 textLength = font.MeasureString(text);

            textPosition = new Vector2(center.X - (textLength.X / 2), center.Y - (textLength.Y / 2));
            #endregion
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
            ground.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            ScreenManager.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            ScreenManager.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            ScreenManager.SpriteBatch.DrawString(font, text, textPosition - new Vector2(0.0f, 50.0f), Color.White);
            ScreenManager.SpriteBatch.End();

            ResetRenderStates();

            ResetRenderStates();
            ground.Draw(gameTime);
            myModel.Draw(gameTime);

            ScreenManager.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            ScreenManager.SpriteBatch.DrawString(font, text, textPosition + new Vector2(0.0f, 50.0f), Color.White);
            ScreenManager.SpriteBatch.End();

        }
        #endregion

    }
}
