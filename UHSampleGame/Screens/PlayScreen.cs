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
#endregion

namespace UHSampleGame.Screens
{
    public class PlayScreen : Screen
    {
        #region Class Variables
        Texture2D background;
        CameraManager cameraManager;

        TileMap tileMap;
        Tile currentTile;
        Dictionary<int, Tower> towers;
        #endregion

        #region Initialization
        public PlayScreen()
            : base("PlayScreen")
        {
            Vector2 numTiles = new Vector2(20, 10);

            TileMap.InitializeTileMap(Vector3.Zero, numTiles, new Vector2(100, 100));

            towers = new Dictionary<int, Tower>();
            for (int i = 0; i < numTiles.X * numTiles.Y; i++)
                towers.Add(i, new TowerAGood(TileMap.Tiles[i].Position));

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
        #endregion

        #region Update/Draw
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            cameraManager.Update();

            foreach (var pair in towers)
            {
                pair.Value.Update(gameTime);
            }


        }

        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);
            bool moveModel = false;
            Tile newTile = new Tile();
            //if (input.CheckNewAction(InputAction.TileMoveUp))
            //{
            //    moveModel = true;
            //    newTile = tileMap.GetTileNeighbor(currentTile, NeighborTile.Up);
            //}
            //if (input.CheckNewAction(InputAction.TileMoveDown))
            //{
            //    moveModel = true;
            //    newTile = tileMap.GetTileNeighbor(currentTile, NeighborTile.Down);
            //}
            //if (input.CheckNewAction(InputAction.TileMoveLeft))
            //{
            //    moveModel = true;
            //    newTile = tileMap.GetTileNeighbor(currentTile, NeighborTile.Left);
            //}
            //if (input.CheckNewAction(InputAction.TileMoveRight))
            //{
            //    moveModel = true;
            //    newTile = tileMap.GetTileNeighbor(currentTile, NeighborTile.Right);
            //}
            //if (!newTile.IsNull())
            //    currentTile = newTile;

            if (input.CheckNewAction(InputAction.Selection))
            {
                ScreenManager.Game.Exit();
            }


            //if (moveModel)
            //    towers.SetPosition(currentTile.Position);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            ScreenManager.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            ScreenManager.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            ScreenManager.SpriteBatch.End();

            //TileMap.Draw();
            
            ResetRenderStates();

            foreach (var pair in towers)
            {
                pair.Value.Draw(gameTime);
            }


        }
        #endregion
    }
}
