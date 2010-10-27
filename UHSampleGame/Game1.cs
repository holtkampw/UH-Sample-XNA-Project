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
using UHSampleGame.CameraManagement;
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
        CameraManager cameraManager;
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
            inputManager.AddInput(InputAction.Selection, Keys.Enter);
            inputManager.AddInput(InputAction.Selection, Buttons.A);
            inputManager.AddInput(InputAction.Selection, Buttons.Start);
            
            //Menu Actions
            inputManager.AddInput(InputAction.MenuUp, Keys.Up);
            inputManager.AddInput(InputAction.MenuDown, Keys.Down);
            inputManager.AddInput(InputAction.MenuLeft, Keys.Left);
            inputManager.AddInput(InputAction.MenuRight, Keys.Right);
            inputManager.AddInput(InputAction.MenuSelect, Keys.Enter);
            inputManager.AddInput(InputAction.MenuCancel, Keys.Back);

            inputManager.AddInput(InputAction.MenuUp, Buttons.LeftThumbstickUp);
            inputManager.AddInput(InputAction.MenuDown, Buttons.LeftThumbstickDown);
            inputManager.AddInput(InputAction.MenuLeft, Buttons.LeftThumbstickLeft);
            inputManager.AddInput(InputAction.MenuRight, Buttons.LeftThumbstickRight);
            inputManager.AddInput(InputAction.MenuSelect, Buttons.A);
            inputManager.AddInput(InputAction.MenuCancel, Buttons.B);

            inputManager.AddInput(InputAction.ExitGame, Buttons.Back);
            inputManager.AddInput(InputAction.ExitGame, Keys.Z);


            this.Services.AddService(typeof(InputManager), inputManager);

            //Movement Actions
            inputManager.AddInput(InputAction.TileMoveUp, Keys.T);
            inputManager.AddInput(InputAction.TileMoveDown, Keys.G);
            inputManager.AddInput(InputAction.TileMoveLeft, Keys.F);
            inputManager.AddInput(InputAction.TileMoveRight, Keys.H);

            inputManager.AddInput(InputAction.TileMoveUp, Buttons.LeftThumbstickUp);
            inputManager.AddInput(InputAction.TileMoveDown, Buttons.LeftThumbstickDown);
            inputManager.AddInput(InputAction.TileMoveLeft, Buttons.LeftThumbstickLeft);
            inputManager.AddInput(InputAction.TileMoveRight, Buttons.LeftThumbstickRight);

            //Tower Action
            inputManager.AddInput(InputAction.TowerBuild, Keys.Space);
            inputManager.AddInput(InputAction.TowerBuild, Buttons.A);

            //PlayerMenu
            inputManager.AddInput(InputAction.PlayerMenuLeft, Buttons.DPadLeft);
            inputManager.AddInput(InputAction.PlayerMenuRight, Buttons.DPadRight);
            inputManager.AddInput(InputAction.PlayerMenuUp, Buttons.DPadUp);
            inputManager.AddInput(InputAction.PlayerMenuDown, Buttons.DPadDown);
            inputManager.AddInput(InputAction.PlayerMenuLeft, Keys.J);
            inputManager.AddInput(InputAction.PlayerMenuRight, Keys.L);
            inputManager.AddInput(InputAction.PlayerMenuUp, Keys.I);
            inputManager.AddInput(InputAction.PlayerMenuDown, Keys.K);

            //UnitAction
            inputManager.AddInput(InputAction.UnitBuild, Buttons.RightTrigger);
            inputManager.AddInput(InputAction.UnitBuild, Keys.U);
            inputManager.AddInput(InputAction.UnitLeft, Buttons.RightThumbstickLeft);
            inputManager.AddInput(InputAction.UnitRight, Buttons.RightThumbstickRight);
            inputManager.AddInput(InputAction.UnitUp, Buttons.RightThumbstickUp);
            inputManager.AddInput(InputAction.UnitDown, Buttons.RightThumbstickDown);
            inputManager.AddInput(InputAction.UnitLeft, Keys.A);
            inputManager.AddInput(InputAction.UnitRight, Keys.D);
            inputManager.AddInput(InputAction.UnitUp, Keys.W);
            inputManager.AddInput(InputAction.UnitDown, Keys.S);
            
            //Setup Screen Manager
            screenManager = new ScreenManager(this);

            //Setup Camera
            cameraManager = new CameraManager();
            this.Services.AddService(typeof(CameraManager), cameraManager);

            //Set Starting Screen
            screenManager.ShowScreen(new IntroScreen());
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

            //Update Input
            inputManager.Update();


            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();
            else if (inputManager.CheckNewAction(InputAction.ExitGame))
            {
                this.Exit();
            }

            

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
