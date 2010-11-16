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
    public class MoreLevelsLater : Screen
    {
        #region Class Variables
        Texture2D pause_screen;
        ScreenManager screenManager;
        #endregion

        public MoreLevelsLater() :
            base("MoreLevelsLater")
        {
        }


        public override void LoadContent()
        {
            pause_screen = ScreenManager.Game.Content.Load<Texture2D>("OverlayScreens\\round3");
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
            }
            else if (ScreenManager.InputManager.CheckNewAction(InputAction.MenuCancel))
            {
                screenManager.RemoveScreen(this);
                screenManager.RemoveScreen("PlayScreen");
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
