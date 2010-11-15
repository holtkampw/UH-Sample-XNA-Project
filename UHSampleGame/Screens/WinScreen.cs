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
    public class WinScreen : Screen
    {
        #region Class Variables
        Texture2D win_screen;
        int teamNum;
        ScreenManager screenManager;
        char[] mapTeamNumToLetter = { ' ', 'A', 'B', 'C', 'D' };
        #endregion

        public WinScreen(int teamNum) :
            base("WinScreen")
        {
            this.teamNum = teamNum;
        }


        public override void LoadContent()
        {
            win_screen = ScreenManager.Game.Content.Load<Texture2D>("OverlayScreens\\mWinner_Team" + mapTeamNumToLetter[teamNum]);
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
        }

        public override void Update(GameTime gameTime)
        {
   
        }

        public override void Draw(GameTime gameTime)
        {
           

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            ScreenManager.SpriteBatch.Draw(win_screen, Vector2.Zero, Color.White);
            ScreenManager.SpriteBatch.End();
        }
    }
}
