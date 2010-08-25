#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.ScreenManagement;
using UHSampleGame.InputManagement;
#endregion

namespace UHSampleGame.Screens
{
    public class DummyTextScreen : Screen
    {
        #region Class Variables
        Texture2D background;
        int currentColorChangeTime;
        const int maxColorChangeTime = 2000;
        Color currentColor;
        Random randomNumber;
        Vector2 center;
        SpriteFont font;
        string text;
        Vector2 textPosition;
        InputManager inputManager;
        ScreenManager screenManager;
        #endregion

        #region Initialization
        public DummyTextScreen() : base("DummyText")
        {
            background = ScreenManager.Game.Content.Load<Texture2D>("DummyText\\background");
            font = ScreenManager.Game.Content.Load<SpriteFont>("DummyText\\Font");
            currentColorChangeTime = 0;
            currentColor = Color.White;
            randomNumber = new Random((int)DateTime.Now.Ticks);

            center = new Vector2((ScreenManager.GraphicsDeviceManager.PreferredBackBufferWidth / 2),
                                 (ScreenManager.GraphicsDeviceManager.PreferredBackBufferHeight / 2)); 
            //Setup Text
            text = "Hello World! Hello World! Hellllllooooo World!";

            //Find out how long the text is using this font
            Vector2 textLength = font.MeasureString(text);

            textPosition = new Vector2(center.X - (textLength.X / 2), center.Y - (textLength.Y / 2));
            inputManager = (InputManager)ScreenManager.Game.Services.GetService(typeof(InputManager));
            screenManager = (ScreenManager)ScreenManager.Game.Services.GetService(typeof(ScreenManager));
        }
        #endregion

        #region Update and Draw
        public override void Update(GameTime gameTime)
        {
            currentColorChangeTime += gameTime.ElapsedGameTime.Milliseconds;
            if (currentColorChangeTime > maxColorChangeTime)
            {
                //get random number
                int num = randomNumber.Next(1, 5);
                switch (num)
                {
                    case 1:
                        currentColor = Color.Green;
                        break;
                    case 2:
                        currentColor = Color.Blue;
                        break;
                    case 3:
                        currentColor = Color.Pink;
                        break;
                    case 4:
                        currentColor = Color.Red;
                        break;
                    default:
                        currentColor = Color.Black;
                        break;
                }

                //reset timer
                currentColorChangeTime = 0;
            }

            if (inputManager.CheckKeyboardAction(InputAction.Selection))
            {
                ScreenManager.ShowScreen(new ModelScreen());
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            ScreenManager.SpriteBatch.DrawString(font, text, textPosition, currentColor);
            ScreenManager.SpriteBatch.End();
            
            base.Draw(gameTime);
        }
        #endregion
    }
}
