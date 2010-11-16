using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UHSampleGame.ScreenManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.InputManagement;

namespace UHSampleGame.Screens
{
    public class TrainingOver : Screen
    {
        #region Class Variables
        Texture2D pause_screen;
        ScreenManager screenManager;
        #endregion

        public TrainingOver() :
            base("TrainingOver")
        {
        }


        public override void LoadContent()
        {
            pause_screen = ScreenManager.Game.Content.Load<Texture2D>("OverlayScreens\\trainingComplete");
            screenManager = (ScreenManager)ScreenManager.Game.Services.GetService(typeof(ScreenManager));
        }

        public override void UnloadContent()
        {

        }

        public override void Reload()
        {

        }

        public override void HandleInput()
        {
            if (ScreenManager.InputManager.CheckNewAction(InputAction.Selection))
            {
                screenManager.RemoveScreen(this);
                screenManager.ShowScreen(new AfterTrainingVideoScreen());
            }
        }

        public override void Update(GameTime gameTime)
        {

        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            ScreenManager.SpriteBatch.Draw(pause_screen, Vector2.Zero, Color.White);
            ScreenManager.SpriteBatch.End();
        }
    }
}
