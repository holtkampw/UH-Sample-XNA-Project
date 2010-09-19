#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.ScreenManagement;
using UHSampleGame.CoreObjects;
using UHSampleGame.InputManagement;
using UHSampleGame.CameraManagement;
using UHSampleGame.TileSystem;
using UHSampleGame.CoreObjects.Units;
using UHSampleGame.CoreObjects.Towers;
using UHSampleGame.CoreObjects.Base;
#endregion

namespace UHSampleGame.Screens
{
    public class TileTestScreen : Screen
    {
        #region Class Variables
        Texture2D background;
        AnimatedModel myModel;
        StaticModel ground;
        CameraManager cameraManager;

        Vector2 center;
        SpriteFont font;
        string text;
        Vector2 textPosition;

        List<Unit> units;
        List<Tower> towers;

        TestBase startBase;
        TestBase goalBase;

        Random rand;
        Tile currentTile;

        int currentINterval = 0;
        #endregion

        #region Initialization
        public TileTestScreen()
            : base("TileTestScreen")
        {
            units = new List<Unit>();
            towers = new List<Tower>();
            rand = new Random();

            TileMap.InitializeTileMap(Vector3.Zero, new Vector2(10, 10), new Vector2(50, 50));

            startBase = new TestBase(TileMap.Tiles[0]);
            goalBase = new TestBase(TileMap.Tiles[TileMap.Tiles.Count - 1]);

            for (int i = 0; i < TileMap.Tiles.Count; i++)
            {
                TileMap.Tiles[i].UpdatePathTo(goalBase.GetTile());
            }

            background = ScreenManager.Game.Content.Load<Texture2D>("Model\\background");
            myModel = new AnimatedModel(ScreenManager.Game.Content.Load<Model>("AnimatedModel\\dude"));
            myModel.Scale = 5.0f;
            myModel.PlayClip("Take 001");

            currentTile = TileMap.GetTileFromPos(Vector3.Zero);
            myModel.SetPosition(currentTile.Position);
            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));
            cameraManager.SetPosition(new Vector3(0.0f, 500.0f, 5000.0f));

            ground = new StaticModel(ScreenManager.Game.Content.Load<Model>("Model\\pyramids"));
            ground.Scale = 1000.0f;
            ground.SetPosition(new Vector3(0.0f, -0.1f, 0.0f));

            #region Setup Text
            font = ScreenManager.Game.Content.Load<SpriteFont>("DummyText\\Font");

            center = new Vector2((ScreenManager.GraphicsDeviceManager.PreferredBackBufferWidth / 2),
                                 (ScreenManager.GraphicsDeviceManager.PreferredBackBufferHeight / 2));
            //Setup Text
            text = "Hello World! Hello World! Hellllllooooo World!";

            //Find out how long the text is using this font
            Vector2 textLength = font.MeasureString(text);

            textPosition = new Vector2(center.X - (textLength.X / 2), center.Y - (textLength.Y / 2));
            #endregion
        }
        #endregion

        #region Update and Draw
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            currentINterval++;

            for (int i = 0; i < units.Count; i++)
                units[i].Update(gameTime);

            if (currentINterval > 200)
            {
                //units.Add(new TestUnit(startBase.GetTile().Position));
                currentINterval = 0;
            }


            for (int i = 0; i < towers.Count; i++)
                towers[i].Update(gameTime);



            cameraManager.Update();
            myModel.Update(gameTime);
            ground.Update(gameTime);
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

            if(input.CheckNewAction(InputAction.Selection))
            {
                //units.Add(new TestUnit(startBase.GetTile().Position));
            }

            //if (input.CheckNewAction(InputAction.Selection))
            //{
            //    for (int i = 0; i < units.Count; i++)
            //    {
            //        units[i].SetPosition(new Vector3(rand.Next(-99, 99), 0, rand.Next(-99, 99)));
            //        units[i].GetTile();
            //    }
            //    //ScreenManager.Game.Exit();
            //}
            if (input.CheckAction(InputAction.RotateLeft))
            {
                cameraManager.RotateX(-0.03f);
            }
            if (input.CheckAction(InputAction.RotateRight))
            {
                cameraManager.RotateX(0.03f);
            }
            if (input.CheckAction(InputAction.RotateUp))
            {
                cameraManager.RotateY(0.01f);
            }
            if (input.CheckAction(InputAction.RotateDown))
            {
                cameraManager.RotateY(-0.01f);
            }
            if (input.CheckAction(InputAction.StrafeLeft))
            {
                cameraManager.StrafeX(-10.0f);
            }
            if (input.CheckAction(InputAction.StrafeRight))
            {
                cameraManager.StrafeX(10.0f);
            }
            if (input.CheckAction(InputAction.StrafeUp))
            {
                cameraManager.StrafeY(10.0f);
            }
            if (input.CheckAction(InputAction.StrafeDown))
            {
                cameraManager.StrafeY(-10.0f);
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
            Tower tower = new TowerTest(tile);
            tile.SetTower(tower);
            bool canBuildTower = true;
            for (int i = 0; i < TileMap.Tiles.Count; i++)
            {
                path = TileMap.Tiles[i].GetPathTo(goalBase.GetTile());
                if (path.Count == 0 && TileMap.Tiles[i] != goalBase.GetTile())
                {
                    canBuildTower = false;
                }
            }
            if (canBuildTower)
            {
                towers.Add(tower);
            }
            else
            {
                tile.RemoveTower();
            }

        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            ScreenManager.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            ScreenManager.SpriteBatch.DrawString(font, text, textPosition - new Vector2(0.0f, 50.0f), Color.White);
            ScreenManager.SpriteBatch.End();

            ResetRenderStates();

            ResetRenderStates();
            ground.Draw(gameTime);
            myModel.Draw(gameTime);

            startBase.Draw(gameTime);
            goalBase.Draw(gameTime);

            for (int i = 0; i < towers.Count; i++)
                towers[i].Draw(gameTime);

            for (int i = 0; i < units.Count; i++)
                units[i].Draw(gameTime);

            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            ScreenManager.SpriteBatch.DrawString(font, myModel.Position.ToString(), textPosition + new Vector2(0.0f, 50.0f), Color.White);
            ScreenManager.SpriteBatch.End();

        }
        #endregion

    }
}
