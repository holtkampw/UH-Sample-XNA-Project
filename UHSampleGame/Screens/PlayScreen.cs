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

using Microsoft.Xna.Framework.Media;
#endregion

namespace UHSampleGame.Screens
{

    public class PlayScreen : Screen
    {
        #region Class Variables
        Texture2D background;
        Texture2D playerMenuBg;
        CameraManager cameraManager;

        Video video;
        VideoPlayer videoPlayer;
        Vector2 dimensions;

        Player p1;
        Player aI;

        SpriteFont font;

        bool isLoaded = false;

        Vector2 numTiles;

        #endregion

        #region Initialization
        public PlayScreen()
            : base("PlayScreen2")
        {
          
        }

        public override void LoadContent()
        {
            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));

            numTiles = new Vector2(20, 10);

            if (numTiles.X == 10 && numTiles.Y == 10)
            {
                cameraManager.SetPosition(new Vector3(0.0f, 1400.0f, 500.0f));
                cameraManager.SetLookAtPoint(new Vector3(0.0f, 0.0f, 50.0f));
            }
            else if (numTiles.X == 20 && numTiles.Y == 10)
            {
                cameraManager.SetPosition(new Vector3(0.0f, 1700.0f, 500.0f));
                cameraManager.SetLookAtPoint(new Vector3(0.0f, 0.0f, 100.0f));
            }

            videoPlayer = new VideoPlayer();
            video = ScreenManager.Game.Content.Load<Video>("Video\\oceanView");
            videoPlayer.IsLooped = true;

            background = ScreenManager.Game.Content.Load<Texture2D>("water_tiled");
            playerMenuBg= ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\playerMenu");
            font = ScreenManager.Game.Content.Load<SpriteFont>("font");
            dimensions = new Vector2((float)ScreenManager.GraphicsDeviceManager.GraphicsDevice.Viewport.Width,             
                                     (float)ScreenManager.GraphicsDeviceManager.GraphicsDevice.Viewport.Height);
             
            UnitCollection.Initialize(8);
            TowerCollection.Initialize(8);
            BaseCollection.Initialize();
            TileMap.InitializeTileMap(Vector3.Zero, numTiles, new Vector2(100f, 100f));
        
            p1 = new Player(1, 1, 2, TileMap.Tiles[0], PlayerType.Human);
            aI = new Player(2, 2, 1, TileMap.Tiles[TileMap.Tiles.Count - 1], PlayerType.AI);

            LevelManager.Initialize();
            LevelManager.AddPlayer(p1);
            LevelManager.AddPlayer(aI);
            LevelManager.LoadLevel(1);

            isLoaded = true;
        }
        #endregion

        #region Update/Draw
        public override void Update(GameTime gameTime)
        {
 
            if (!IsVisible)
            {
                videoPlayer.Stop();
                return;
            }

            if (videoPlayer.State != MediaState.Playing)
                videoPlayer.Play(video);

            cameraManager.Update();

            p1.Update(gameTime);
            aI.Update(gameTime);
            UnitCollection.Update(gameTime);
            TowerCollection.Update(gameTime);
            DebugInfo.Update(gameTime);
            
        }

        public override void HandleInput(InputManager input)
        {

            if (isLoaded)
            { 
                p1.HandleInput(input);
                aI.HandleInput(input);
            }
        }

        public override void Draw(GameTime gameTime)
        {

                ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                if (videoPlayer.State == MediaState.Playing || videoPlayer.State == MediaState.Stopped)
                {
                    ScreenManager.SpriteBatch.Draw(videoPlayer.GetTexture(), new Rectangle(0, 0,
                        (int)dimensions.X, (int)dimensions.Y), Color.White);
                }
                ScreenManager.SpriteBatch.Draw(playerMenuBg, Vector2.Zero, Color.White);

                ScreenManager.SpriteBatch.End();

                ResetRenderStates();
                
                UnitCollection.Draw(gameTime);
                TowerCollection.Draw(gameTime);
                p1.Draw(gameTime);
                aI.Draw(gameTime);


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
