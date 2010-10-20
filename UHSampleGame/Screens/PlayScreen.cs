﻿#region Using Statements
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
        CameraManager cameraManager;

        Video video;
        VideoPlayer videoPlayer;
        Vector2 dimensions;

        Player p1;
        Player aI;

        SpriteFont font;

        Vector2 numTiles;
        bool contentLoaded = false;

        #endregion

        #region Initialization
        public PlayScreen()
            : base("PlayScreen2")
        {
            AssetHelper.LoadOne(ScreenManager.Game);
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

            //UnitCollection.Initialize(2);
            //TowerCollection.Initialize(2);
            //Vector2 numTiles = new Vector2(20, 10);

            //TileMap2.InitializeTileMap(Vector3.Zero, numTiles, new Vector2(100, 100));
            //p1 = new Player2(0, 1, TileMap2.Tiles[0], PlayerType.Human);
            //aI = new Player2(1, 2, TileMap2.Tiles[TileMap2.Tiles.Count - 1], PlayerType.AI);

            //LevelManager2.Initialize();
            //LevelManager2.AddPlayer(p1);
            //LevelManager2.AddPlayer(aI);
            //LevelManager2.LoadLevel(1);

            //Viewport viewport = ScreenManager.GraphicsDeviceManager.GraphicsDevice.Viewport;
            //dimensions = new Vector2(viewport.Width, viewport.Height);

            //video = ScreenManager.Game.Content.Load<Video>("Video\\oceanView");
            //videoPlayer = new VideoPlayer();
            //videoPlayer.IsLooped = true;

            //if (videoPlayer.State != MediaState.Playing)
            //    videoPlayer.Play(video);

            //background = ScreenManager.Game.Content.Load<Texture2D>("water_tiled");

            //cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));

            //if (numTiles.X == 10 && numTiles.Y == 10)
            //{
            //    cameraManager.SetPosition(new Vector3(0.0f, 1400.0f, 500.0f));
            //    cameraManager.SetLookAtPoint(new Vector3(0.0f, 0.0f, 50.0f));
            //}
            //else if (numTiles.X == 20 && numTiles.Y == 10)
            //{
            //    cameraManager.SetPosition(new Vector3(0.0f, 1700.0f, 500.0f));
            //    cameraManager.SetLookAtPoint(new Vector3(0.0f, 0.0f, 100.0f));
            //}

            //font = ScreenManager.Game.Content.Load<SpriteFont>("font");
        }
        #endregion

        #region Update/Draw
        public override void Update(GameTime gameTime)
        {
            //Call into the Asset helper to load your assets
            AssetHelper.LoadOne(ScreenManager.Game);

            if (AssetHelper.Loaded)
            {
                videoPlayer = AssetHelper.Get<VideoPlayer>("videoPlayer");
                video = AssetHelper.Get<Video>("video");
                p1 = AssetHelper.Get<Player>("p1");
                aI = AssetHelper.Get<Player>("aI");
                dimensions = AssetHelper.Get<Vector2>("dimensions");
                contentLoaded = true;

            }
            // Show a simple example fade effect
            if (AssetHelper.Loaded)
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
            
        }

        public override void HandleInput(InputManager input)
        {
            if (AssetHelper.Loaded && contentLoaded)
            {
                p1.HandleInput(input);
                aI.HandleInput(input);
            }
        }

        public override void Draw(GameTime gameTime)
        {
            
            if (!AssetHelper.Loaded && !contentLoaded)
            {
                //Currently loading, display progress
                ScreenManager.SpriteBatch.Begin();
                //ScreenManager.SpriteBatch.Draw(loading_scree, Vector2.Zero, Color.White);
                ScreenManager.SpriteBatch.Draw(AssetHelper.Get<Texture2D>("loading_screen"), Vector2.Zero, Color.White);

                ScreenManager.SpriteBatch.DrawString(AssetHelper.Get<SpriteFont>("font"),
                    "Loading Progress: " + AssetHelper.PercentLoaded + "%", new Vector2(50, 50), Color.DarkRed);
                ScreenManager.SpriteBatch.End();
            }
            else
            {
                ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                if (videoPlayer.State == MediaState.Playing || videoPlayer.State == MediaState.Stopped)
                {
                    //spriteBatch.Draw(videoPlayer.GetTexture(), new Rectangle(0, 0, video.Width, video.Height), Color.White);
                    ScreenManager.SpriteBatch.Draw(videoPlayer.GetTexture(), new Rectangle(0, 0,
                        (int)dimensions.X, (int)dimensions.Y), Color.White);
                }

                ScreenManager.SpriteBatch.End();

                ResetRenderStates();
                p1.Draw(gameTime);
                aI.Draw(gameTime);
                UnitCollection.Draw(gameTime);
                TowerCollection.Draw(gameTime);


                ScreenManager.SpriteBatch.Begin();
                DebugInfo.Draw();
                ScreenManager.SpriteBatch.End();
            }
        }
        #endregion

        #region Unload
        public override void UnloadContent()
        {

        }
        #endregion
    }
}
