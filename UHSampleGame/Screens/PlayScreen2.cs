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

using Microsoft.Xna.Framework.Media;
#endregion

namespace UHSampleGame.Screens
{
    public class PlayScreen2 : Screen
    {
        #region Class Variables
        Texture2D background;
        CameraManager cameraManager;

        Video video;
        VideoPlayer videoPlayer;
        Vector2 dimensions;

        Player2 p1;

        #endregion

        #region Initialization
        public PlayScreen2()
            : base("PlayScreen2")
        {

            Vector2 numTiles = new Vector2(20, 10);

            TileMap.InitializeTileMap(Vector3.Zero, numTiles, new Vector2(100, 100));
            p1 =  new Player2(1, 1, TileMap.Tiles[0], PlayerType.Human);
            Viewport viewport = ScreenManager.GraphicsDeviceManager.GraphicsDevice.Viewport;
            dimensions = new Vector2(viewport.Width, viewport.Height);

            video = ScreenManager.Game.Content.Load<Video>("Video\\oceanView");
            videoPlayer = new VideoPlayer();
            videoPlayer.IsLooped = true;

            if (videoPlayer.State != MediaState.Playing)
                videoPlayer.Play(video);

            background = ScreenManager.Game.Content.Load<Texture2D>("water_tiled");

            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));

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
        }

        public override void LoadContent()
        {

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
            FPSCounter.Update(gameTime);
            
        }

        public override void HandleInput(InputManager input)
        {
            p1.HandleInput(input);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            if (videoPlayer.State == MediaState.Playing || videoPlayer.State == MediaState.Stopped)
            {
                //spriteBatch.Draw(videoPlayer.GetTexture(), new Rectangle(0, 0, video.Width, video.Height), Color.White);
                ScreenManager.SpriteBatch.Draw(videoPlayer.GetTexture(), new Rectangle(0, 0,
                    (int)dimensions.X, (int)dimensions.Y), Color.White);
            }

            ScreenManager.SpriteBatch.End();

            p1.Draw(gameTime);

            ResetRenderStates();

            ScreenManager.SpriteBatch.Begin();
            FPSCounter.Draw();
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
