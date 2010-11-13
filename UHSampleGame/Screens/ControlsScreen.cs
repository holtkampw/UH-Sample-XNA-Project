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
    public class ControlsScreen : Screen
    {
        #region Class Variables
        Texture2D controls_screen;
        Texture2D front;
        Texture2D back;
        Vector2 frontPosition = new Vector2(0.0f, 130.0f);
        Vector2 backPosition = new Vector2(0.0f, 138.0f);
        bool showFront = true;
        #endregion

        public ControlsScreen() :
            base("ControlsScreen")
        {
        }


        public override void LoadContent()
        {
            controls_screen = ScreenManager.Game.Content.Load<Texture2D>("OverlayScreens\\menuControllerBackground");
            front = ScreenManager.Game.Content.Load<Texture2D>("OverlayScreens\\menuControllers_FrontView");
            back = ScreenManager.Game.Content.Load<Texture2D>("OverlayScreens\\menuControllers_topFrontView"); 
            screenManager = (ScreenManager)ScreenManager.Game.Services.GetService(typeof(ScreenManager));
        }

        public override void UnloadContent()
        {

        }

        public override void Reload()
        {

        }

        public override void HandleInput(InputManager input)
        {
            if (input.CheckNewAction(InputAction.Selection))
            {
                showFront = !showFront;
            }

            if (input.CheckNewAction(InputAction.MenuCancel))
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
            ScreenManager.SpriteBatch.Draw(controls_screen, Vector2.Zero, Color.White);
            if (showFront)
            {
                ScreenManager.SpriteBatch.Draw(front, frontPosition, Color.White);
            }
            else
            {
                ScreenManager.SpriteBatch.Draw(back, backPosition, Color.White);
            }
            ScreenManager.SpriteBatch.End();
        }
    }
}
