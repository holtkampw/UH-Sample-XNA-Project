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

        TileMap tileMap;
        Tile currentTile;
        #endregion

        #region Initialization
        public TileTestScreen()
            : base("TileTestScreen")
        {
            tileMap = new TileMap(Vector3.Zero, new Vector2(10, 10), new Vector2(20, 20));

            background = ScreenManager.Game.Content.Load<Texture2D>("Model\\background");
            myModel = new AnimatedModel(ScreenManager.Game.Content.Load<Model>("AnimatedModel\\dude"));
            myModel.Scale = 20.0f;
            myModel.PlayClip("Take 001");
            currentTile = tileMap.GetTileFromPos(Vector3.Zero);
            myModel.SetPosition(currentTile.Position);
            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));
            cameraManager.SetPosition(new Vector3(0.0f, 50.0f, 5000.0f));

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

            cameraManager.Update();
            myModel.Update(gameTime);
            ground.Update(gameTime);
        }

        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);
            bool moveModel = false;
            if(input.CheckNewAction(InputAction.TileMoveUp))
            {
                moveModel = true;
                currentTile = tileMap.GetTileNeighbor(currentTile, NeighborTile.Up);
            }
            if (input.CheckNewAction(InputAction.TileMoveDown))
            {
                moveModel = true;
                currentTile = tileMap.GetTileNeighbor(currentTile, NeighborTile.Down);
            }
            if (input.CheckNewAction(InputAction.TileMoveLeft))
            {
                moveModel = true;
                currentTile = tileMap.GetTileNeighbor(currentTile, NeighborTile.Left);
            }
            if (input.CheckNewAction(InputAction.TileMoveRight))
            {
                moveModel = true;
                currentTile = tileMap.GetTileNeighbor(currentTile, NeighborTile.Right);
            }
            if (input.CheckNewAction(InputAction.Selection))
            {
                ScreenManager.Game.Exit();
            }
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
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            ScreenManager.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            ScreenManager.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            ScreenManager.SpriteBatch.DrawString(font, text, textPosition - new Vector2(0.0f, 50.0f), Color.White);
            ScreenManager.SpriteBatch.End();

            ResetRenderStates();

            ResetRenderStates();
            ground.Draw(gameTime);
            myModel.Draw(gameTime);

            ScreenManager.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            ScreenManager.SpriteBatch.DrawString(font, myModel.Position.ToString(), textPosition + new Vector2(0.0f, 50.0f), Color.White);
            ScreenManager.SpriteBatch.End();

            

        }
        #endregion

    }
}
