#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using UHSampleGame.ScreenManagement;
using UHSampleGame.Screens;
using UHSampleGame.InputManagement;
#endregion

namespace UHSampleGame
{


    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region Class Variables
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;
        InputManager inputManager;
        #endregion

        #region Initialization
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            //Set Graphics Card Resolution
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;

            //Set Game to full screen or windowed screen
            graphics.IsFullScreen = false;

            //Should Mouse be visible?
            this.IsMouseVisible = false;

            //Various Default Configuration
            Content.RootDirectory = "Content";
            this.Components.Add(new GamerServicesComponent(this));
            this.Services.AddService(typeof(GraphicsDeviceManager), graphics);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            //Setup some basic input
            inputManager = new InputManager();
            inputManager.AddKey(InputAction.Selection, Keys.Enter);
            inputManager.AddKey(InputAction.Rotation, Keys.Space);
            this.Services.AddService(typeof(InputManager), inputManager);

            //Setup Screen Manager
            screenManager = new ScreenManager(this);

            //Set Starting Screen
            ScreenManager.ShowScreen(new DummyTextScreen());

            
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        #endregion

        #region Update
        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            //Update Input
            inputManager.Update();

            //Update our screens
            screenManager.Update(gameTime);

            base.Update(gameTime);
        }
        #endregion

        #region Draw
        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            //Reset the screen
            GraphicsDevice.Clear(Color.CornflowerBlue);

            //Draw our current screen
            screenManager.Draw(gameTime);
            base.Draw(gameTime);
        }
        #endregion
    }
}
