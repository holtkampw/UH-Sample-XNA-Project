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
#endregion

namespace UHSampleGame.Screens
{
    public class PlayScreen : Screen
    {
        #region Class Variables
        Texture2D background;
        CameraManager cameraManager;
        AnimatedModel myModel;
        
        Tile currentTile;

        TestBase goalBase;

        List<Base> bases;

        HumanPlayer player;

        #endregion

        #region Initialization
        public PlayScreen()
            : base("PlayScreen")
        {

            Vector2 numTiles = new Vector2(20, 10);

            TileMap.InitializeTileMap(Vector3.Zero, numTiles, new Vector2(100, 100));

            goalBase = new TestBase(2,2,TileMap.Tiles[TileMap.Tiles.Count - 1]);

            player = new HumanPlayer(1, 1, TileMap.Tiles[0]);
            player.SetTargetBase(goalBase);
            goalBase.SetGoalBase(player.Base);

            bases = new List<Base>();
            bases.Add(goalBase);

            TileMap.SetBase(goalBase);

            TileMap.UpdateTilePaths();

            //towers = new Dictionary<int, Tower>();
            //for (int i = 0; i < numTiles.X * numTiles.Y; i++)
            //   towers.Add(i, new TowerAGood(TileMap.Tiles[i].Position));

            background = ScreenManager.Game.Content.Load<Texture2D>("water_tiled");

            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));

            myModel = new AnimatedModel(ScreenManager.Game.Content.Load<Model>("AnimatedModel\\dude"));
            myModel.Scale = 2.0f;
            myModel.PlayClip("Take 001");

            currentTile = TileMap.GetTileFromPos(Vector3.Zero);
            myModel.SetPosition(currentTile.Position);

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
        #endregion

        #region Update/Draw
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            cameraManager.Update();

            goalBase.Update(gameTime);
            player.Update(gameTime);
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

            //TileMap.Draw();
            ResetRenderStates();
            //myModel.Draw(gameTime);

            player.Draw(gameTime);
            goalBase.Draw(gameTime);

           // ResetRenderStates();

            //foreach (var pair in towers)
            //{
            //    pair.Value.Draw(gameTime);
            //}


        }
        #endregion
    }
}
