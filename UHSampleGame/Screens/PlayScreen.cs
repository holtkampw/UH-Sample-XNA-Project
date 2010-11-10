#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.CameraManagement;
using UHSampleGame.TileSystem;
using UHSampleGame.ScreenManagement;
using UHSampleGame.CoreObjects;
using UHSampleGame.InputManagement;
using UHSampleGame.CoreObjects.Towers;
using UHSampleGame.CoreObjects.Units;
using UHSampleGame.CoreObjects.Base;
using UHSampleGame.Players;
using UHSampleGame.LevelManagement;
using UHSampleGame.Debug;
using UHSampleGame;
using UHSampleGame.ProjectileManagment;
using UHSampleGame.PathFinding;

using Microsoft.Xna.Framework.Media;
#endregion

namespace UHSampleGame.Screens
{

    public class PlayScreen : Screen
    {
        #region Class Variables
        Texture2D background;
        Texture2D playerBackground;
        Color playerBackgroundColor;
        Vector2 playerBackgroundLocation;
        CameraManager cameraManager;

        Vector2 dimensions;

        //Player p1;
        //Player aI;

        SpriteFont font;

        bool isLoaded = false;

        Vector2 numTiles;

        PlayerSetup[] playerSetup;
        #endregion

        #region Initialization
        public PlayScreen(PlayerSetup[] playerSetup)
            : base("PlayScreen")
        {
            this.playerSetup = playerSetup;
        }

        public override void LoadContent()
        {
            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));

            numTiles = new Vector2(10, 10);

            if (numTiles.X == 10 && numTiles.Y == 10)
            {
                //cameraManager.SetPosition(new Vector3(0.0f, 1400.0f, 500.0f));
                //cameraManager.SetLookAtPoint(new Vector3(0.0f, 0.0f, 50.0f));
                cameraManager.SetPosition(new Vector3(0.0f, 1700.0f, 500.0f));
                cameraManager.SetLookAtPoint(new Vector3(0.0f, 0.0f, 100.0f));
                cameraManager.SetLookAtPoint(new Vector3(0.0f, 0.0f, -500.0f));
            }
            else if (numTiles.X == 20 && numTiles.Y == 10)
            {
                cameraManager.SetPosition(new Vector3(0.0f, 1700.0f, 500.0f));
                cameraManager.SetLookAtPoint(new Vector3(0.0f, 0.0f, 100.0f));
            }

            background = ScreenManager.Game.Content.Load<Texture2D>("water_tiled");
            background = ScreenManager.Game.Content.Load<Texture2D>("PlayScreen\\oceanWaveBackground");
            playerBackground = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\player_bg");
            playerBackgroundColor = new Color(255, 255, 255, 200);
            playerBackgroundLocation = new Vector2(40.0f, 0.0f);

            font = ScreenManager.Game.Content.Load<SpriteFont>("font");
            dimensions = new Vector2((float)ScreenManager.GraphicsDeviceManager.GraphicsDevice.Viewport.Width,             
                                     (float)ScreenManager.GraphicsDeviceManager.GraphicsDevice.Viewport.Height);


            UnitCollection.Initialize(8);
            TowerCollection.Initialize(8);
            BaseCollection.Initialize();
            PlayerCollection.Initialize();
            ProjectileManager.Initialize();
            //TileMap.InitializeTileMap(Vector3.Zero, numTiles, new Vector2(100f, 100f));
            //PlayerCollection.Initialize(playerTypes);
        
            //FIX THIS!!! assign players their tiles AFTER level manager is loaded... 
            //Right now TileMap is being initialized TWICE!!!
            //p1 = new Player(1, 1, 2, TileMap.Tiles[0], PlayerType.Human);
            //aI = new Player(2, 2, 1, TileMap.Tiles[TileMap.Tiles.Count - 1], PlayerType.AI);

            LevelManager.Initialize();
            LevelManager.CreateLevel(numTiles, playerSetup);
            /*LevelManager.AddPlayer(p1);
            LevelManager.AddPlayer(aI);*/
            //LevelManager.LoadLevel(1);

            GC.Collect();//force garbage collection
            isLoaded = true;
        }
        #endregion

        #region Update/Draw
        public override void Update(GameTime gameTime)
        {
 
            if (!IsVisible)
            {
                //videoPlayer.Stop();
                return;
            }

            //if (videoPlayer.State != MediaState.Playing)
            //    videoPlayer.Play(video);

            cameraManager.Update();


 ///           p1.Update(gameTime);
 //           aI.Update(gameTime);
            UnitCollection.Update(gameTime);
            TowerCollection.Update(gameTime);
            PlayerCollection.Update(gameTime);
            ProjectileManager.Update(gameTime);
            DebugInfo.Update(gameTime);
            
        }

        public override void HandleInput(InputManager input)
        {

            if (isLoaded)
            {
                PlayerCollection.HandleInput(input);
 //               p1.HandleInput(input);
 //               aI.HandleInput(input);
            }
        }

        public override void Draw(GameTime gameTime)
        {
                ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                ScreenManager.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
                //if (videoPlayer.State == MediaState.Playing || videoPlayer.State == MediaState.Stopped)
                //{
                //    ScreenManager.SpriteBatch.Draw(videoPlayer.GetTexture(), new Rectangle(0, 0,
                //        (int)dimensions.X, (int)dimensions.Y), Color.White);
                //}

                ScreenManager.SpriteBatch.Draw(playerBackground, playerBackgroundLocation, playerBackgroundColor);

                ScreenManager.SpriteBatch.End();

                ResetRenderStates();
                
                UnitCollection.Draw(gameTime);
                TowerCollection.Draw(gameTime);
                ResetRenderStates();
                PlayerCollection.Draw(gameTime);
                ProjectileManager.Draw(gameTime);
//                p1.Draw(gameTime);
//                aI.Draw(gameTime);


                ScreenManager.SpriteBatch.Begin();
                DebugInfo.Draw();
                ScreenManager.SpriteBatch.End();
        }
        #endregion

        #region Unload
        public override void UnloadContent()
        {

        }
        #endregion

       
    }
}
