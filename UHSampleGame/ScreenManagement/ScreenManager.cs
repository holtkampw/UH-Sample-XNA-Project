#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.InputManagement;
#endregion

namespace UHSampleGame.ScreenManagement
{
    public class ScreenManager
    {
        #region Class Variables
        public static Game Game;
        public static SpriteBatch SpriteBatch;
        public static GraphicsDeviceManager GraphicsDeviceManager;
        public static InputManager InputManager;
        static List<Screen> screens;
        #endregion

        #region Initialization
        public ScreenManager(Game game)
        {
            //Setup the game for use in all other screens
            ScreenManager.Game = game;

            //Setup the graphics device for later use
            ScreenManager.GraphicsDeviceManager = (GraphicsDeviceManager)game.Services.GetService(typeof(GraphicsDeviceManager));

            //Create a sprite for use in all other screens
            ScreenManager.SpriteBatch = new SpriteBatch(game.GraphicsDevice);

            ScreenManager.InputManager = (InputManager)game.Services.GetService(typeof(InputManager));

            screens = new List<Screen>();
        }
        #endregion

        #region Screen Manipulation
        /// <summary>
        /// Add as screen to the screen list
        /// </summary>
        /// <param name="screen">Screen to add</param>
        /// <returns>If operation was successful</returns>
        public bool AddScreen(Screen screen)
        {
            if (screen == null)
                return false;
            screen.ScreenManager = this;
            screen.LoadContent();
            screens.Add(screen);
            return true;
        }

        /// <summary>
        /// Remove an existing screen
        /// </summary>
        /// <param name="screen">Screen object to remove</param>
        /// <returns>If operation was successful</returns>
        public bool RemoveScreen(Screen screen)
        {
            if (screen != null && screens.Count > 1)
            {
                screen.UnloadContent();
                screens.Remove(screen);
                screens[screens.Count - 1].Reload();
                screens[screens.Count - 1].SetStatus(ScreenStatus.Visible);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Remove an existing screen
        /// </summary>
        /// <param name="screenName">The Unique Screen Identifier</param>
        /// <returns>If operation was successful</returns>
        public bool RemoveScreen(string screenName)
        {
            for (int i = 0; i < screens.Count; i++)
                if (screens[i].Name == screenName)
                    return RemoveScreen(screens[i]);
            return false;
        }

        /// <summary>
        /// Show a screen
        /// </summary>
        /// <param name="screen">Screen to show</param>
        /// <returns>If operation was successful</returns>
        public bool ShowScreen(Screen screen)
        {
            //Find if screen exists.  If not, add it
            if (!screens.Contains(screen))
                AddScreen(screen);

            //Find out where the screen exists in the array
            int foundAtIndex = -1;
            for (int i = 0; i < screens.Count; i++)
            {
                if (screens[i] == screen)
                {
                    screens[i].Reload();
                    foundAtIndex = i;
                    break;
                }
            }

            //if screen isn't found, error out
            if (foundAtIndex == -1)
                return false;

            //disable all non-relevant screens
            for (int i = 0; i < foundAtIndex; i++)
            {
                if (screens[foundAtIndex].Status == ScreenStatus.Overlay && i == foundAtIndex - 1)
                    //overlay screens can have 1 screen visible underneath them
                    screens[i].SetStatus(ScreenStatus.Visible);
                else
                    screens[i].SetStatus(ScreenStatus.Disabled);
            }

            if (screens[foundAtIndex].Status != ScreenStatus.Overlay)
                screens[foundAtIndex].SetStatus(ScreenStatus.Visible);

            return true;
        }
        #endregion

        #region Update and Draw
        public void Update(GameTime gameTime)
        {
            //Update current screen that is available for interaction
            //TEMPORARY FIX
            for (int i = 0; i < screens.Count; i++)
                if(screens[i].Status != ScreenStatus.Disabled)
                    screens[i].Update(gameTime);
            //screens[screens.Count - 1].Update(gameTime);
            screens[screens.Count - 1].HandleInput(InputManager);
            //Yes this order is on purpose... if we add a screen in HandleInput then update the
            //inputManager is not updated... Perhaps move inputManager.Update into here as well?
        }

        public void Draw(GameTime gameTime)
        {
            //Draw any visible screens
            for (int i = 0; i < screens.Count; i++)
                if (screens[i].Status != ScreenStatus.Disabled)
                    screens[i].Draw(gameTime);
        }
        #endregion
    }
}
