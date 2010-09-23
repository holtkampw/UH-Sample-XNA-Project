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
using UHSampleGame.Player;
using UHSampleGame.LevelManagement;
#endregion

namespace UHSampleGame.Screens
{
    public class PlayScreen : Screen
    {
        #region Class Variables
        Texture2D background;
        CameraManager cameraManager;
        
        Tile currentTile;

        TestBase goalBase;

        HumanPlayer player;
        LevelManager levelManager;

        int frames;
        int frameRate;
        SpriteFont font;
        private TimeSpan elapsedTime;

        #endregion

        #region Initialization
        public PlayScreen()
            : base("PlayScreen")
        {
            levelManager = new LevelManager();
            levelManager.LoadLevel(1);
            //Vector2 numTiles = new Vector2(20, 10);

            //TileMap.InitializeTileMap(Vector3.Zero, numTiles, new Vector2(100, 100));

           // goalBase = new TestBase(2,2,TileMap.Tiles[TileMap.Tiles.Count - 1]);

            player = new HumanPlayer(1, 1, TileMap.Tiles[0]);
            //player.SetTargetBase(goalBase);
            //goalBase.SetGoalBase(player.Base);

            //TileMap.SetBase(goalBase);

            //TileMap.UpdateTilePaths();

            background = ScreenManager.Game.Content.Load<Texture2D>("water_tiled");

            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));

            currentTile = TileMap.GetTileFromPos(Vector3.Zero);

            if (levelManager.CurrentLevel.NumTiles.X == 10 && levelManager.CurrentLevel.NumTiles.Y == 10)
            {
                cameraManager.SetPosition(new Vector3(0.0f, 1400.0f, 500.0f));
                cameraManager.SetLookAtPoint(new Vector3(0.0f, 0.0f, 50.0f));
            }
            else if (levelManager.CurrentLevel.NumTiles.X == 20 && levelManager.CurrentLevel.NumTiles.Y == 10)
            {
                cameraManager.SetPosition(new Vector3(0.0f, 1700.0f, 500.0f));
                cameraManager.SetLookAtPoint(new Vector3(0.0f, 0.0f, 100.0f));
            }

            font = ScreenManager.Game.Content.Load<SpriteFont>("font");
            frames = 0;
            frameRate = 0;
        }
        #endregion

        #region Update/Draw
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            cameraManager.Update();

           // goalBase.Update(gameTime);
            player.Update(gameTime);

            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frames;
                frames = 0;
            }
        }

        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);
            player.HandleInput(input);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            ScreenManager.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            ScreenManager.SpriteBatch.End();

            ResetRenderStates();

            player.Draw(gameTime);
           // goalBase.Draw(gameTime);

            ScreenManager.SpriteBatch.Begin();
            string text = "FPS: " + frameRate + "\nTowers: " + player.TowerCount + "\nUnits: " + player.UnitCount;
            ScreenManager.SpriteBatch.DrawString(font, text, new Vector2(11.0f, 11.0f), Color.Black);
            ScreenManager.SpriteBatch.DrawString(font, text, new Vector2(10.0f, 10.0f), Color.White);
            ScreenManager.SpriteBatch.End();

            frames++;
        }
        #endregion
    }
}
