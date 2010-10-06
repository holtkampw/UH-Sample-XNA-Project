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
        Player2 aI;

        #endregion

        #region Initialization
        public PlayScreen2()
            : base("PlayScreen2")
        {
            UnitCollection.Initialize(2);
            Vector2 numTiles = new Vector2(20, 10);

            TileMap2.InitializeTileMap(Vector3.Zero, numTiles, new Vector2(100, 100));
            p1 =  new Player2(0, 1, TileMap2.Tiles[0], PlayerType.Human);
            aI = new Player2(1, 2, TileMap2.Tiles[TileMap2.Tiles.Count-1], PlayerType.AI);

            LevelManager2.Initialize();
            LevelManager2.AddPlayer(p1);
            LevelManager2.AddPlayer(aI);
            Levelmanager2.LoadLevel(1);

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
            aI.Update(gameTime);
            UnitCollection.Update(gameTime);
            DebugInfo.Update(gameTime);
            
        }

        public override void HandleInput(InputManager input)
        {
            p1.HandleInput(input);
            aI.HandleInput(input);
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

            ResetRenderStates();
            p1.Draw(gameTime);
            aI.Draw(gameTime);
            UnitCollection.Draw(gameTime);
           

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
