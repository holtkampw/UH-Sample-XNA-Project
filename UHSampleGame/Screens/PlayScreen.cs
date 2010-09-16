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

        List<Unit> units;
        List<Tower> towers;

        TestBase startBase;
        TestBase goalBase;

        List<Base> bases;

        //Dictionary<int, Tower> towers;
        #endregion

        #region Initialization
        public PlayScreen()
            : base("PlayScreen")
        {
            units = new List<Unit>();
            towers = new List<Tower>();

            Vector2 numTiles = new Vector2(20, 10);

            TileMap.InitializeTileMap(Vector3.Zero, numTiles, new Vector2(100, 100));

            startBase = new TestBase(TileMap.Tiles[0]);
            goalBase = new TestBase(TileMap.Tiles[TileMap.Tiles.Count - 1]);

            startBase.SetGoalBase(goalBase);
            goalBase.SetGoalBase(startBase);

            bases = new List<Base>();
            bases.Add(startBase);
            bases.Add(goalBase);

            TileMap.SetBase(goalBase);
            TileMap.SetBase(startBase);

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

            for (int i = 0; i < units.Count; i++)
                units[i].Update(gameTime);


            for (int i = 0; i < towers.Count; i++)
                towers[i].Update(gameTime);

            myModel.Update(gameTime);
            startBase.Update(gameTime);
            goalBase.Update(gameTime);
        }

        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);
            bool moveModel = false;
            Tile newTile = new Tile();
            if (input.CheckNewAction(InputAction.TileMoveUp))
            {
                moveModel = true;
                newTile = TileMap.GetTileNeighbor(currentTile, NeighborTile.Up);
            }
            if (input.CheckNewAction(InputAction.TileMoveDown))
            {
                moveModel = true;
                newTile = TileMap.GetTileNeighbor(currentTile, NeighborTile.Down);
            }
            if (input.CheckNewAction(InputAction.TileMoveLeft))
            {
                moveModel = true;
                newTile = TileMap.GetTileNeighbor(currentTile, NeighborTile.Left);
            }
            if (input.CheckNewAction(InputAction.TileMoveRight))
            {
                moveModel = true;
                newTile = TileMap.GetTileNeighbor(currentTile, NeighborTile.Right);
            }
            if (!newTile.IsNull())
                currentTile = newTile;

            if (input.CheckNewAction(InputAction.Selection))
            {
                //units.Add(new TestUnit(startBase.Position, goalBase));
                units.Add(new TestUnit(goalBase.Position, startBase));
            }

            if (moveModel)
                myModel.SetPosition(currentTile.Position);

            if (input.CheckNewAction(InputAction.TowerBuild))
            {
                BuildTower(currentTile);
            }
        }

        public void BuildTower(Tile tile)
        {
            List<Tile> path;
            Tower tower = new TowerAGood(tile.Position);
            tile.SetTower(tower);
            bool canBuildTower = true;

            if (TileMap.UpdateTilePaths())
            {
                towers.Add(tower);
            }
            else
            {
                tile.RemoveTower(tower);
                TileMap.UpdateTilePaths();
                //throw new NotImplementedException("Cannot block path");
            }

        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            ScreenManager.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            ScreenManager.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            ScreenManager.SpriteBatch.End();

            //TileMap.Draw();

            myModel.Draw(gameTime);

            startBase.Draw(gameTime);
            goalBase.Draw(gameTime);

            for (int i = 0; i < towers.Count; i++)
                towers[i].Draw(gameTime);

            for (int i = 0; i < units.Count; i++)
                units[i].Draw(gameTime);
            
            ResetRenderStates();

            //foreach (var pair in towers)
            //{
            //    pair.Value.Draw(gameTime);
            //}


        }
        #endregion
    }
}
