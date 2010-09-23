using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.TileSystem;
using UHSampleGame.CoreObjects;
using UHSampleGame.ScreenManagement;
using UHSampleGame.CoreObjects.Units;
using UHSampleGame.InputManagement;

namespace UHSampleGame.Player
{
    public class HumanPlayer : Player
    {
        TeamableAnimatedObject avatar;

        public HumanPlayer(int playerNum, int teamNum, Tile baseTile)
            : base(playerNum, teamNum, baseTile) 
        {
            avatar = new TeamableAnimatedObject(playerNum, teamNum,
                ScreenManager.Game.Content.Load<Model>("AnimatedModel\\dude"));

            avatar.Scale = 2.0f;
            avatar.PlayClip("Take 001");
            avatar.SetPosition(playerBase.Position);
        }

        public override void HandleInput(InputManager input)
        {
            base.HandleInput(input);
            if (input.CheckAction(InputAction.TileMoveUp))
                avatar.SetPosition(avatar.Position + new Vector3(0, 0, -3));

            if (input.CheckAction(InputAction.TileMoveDown))
                avatar.SetPosition(avatar.Position + new Vector3(0, 0, 3));

            if (input.CheckAction(InputAction.TileMoveLeft))
                avatar.SetPosition(avatar.Position + new Vector3(-3, 0, 0));

            if (input.CheckAction(InputAction.TileMoveRight))
                avatar.SetPosition(avatar.Position + new Vector3(3, 0, 0));

            if (avatar.Position.X < TileMap.Left)
                avatar.SetPosition(new Vector3(TileMap.Left, avatar.Position.Y, avatar.Position.Z));

            if (avatar.Position.Z < TileMap.Top)
                avatar.SetPosition(new Vector3(avatar.Position.X, avatar.Position.Y, TileMap.Top));

            if (avatar.Position.X > TileMap.Right)
                avatar.SetPosition(new Vector3(TileMap.Right, avatar.Position.Y, avatar.Position.Z));

            if (avatar.Position.Z > TileMap.Bottom)
                avatar.SetPosition(new Vector3(avatar.Position.X, avatar.Position.Y, TileMap.Bottom));

            if (input.CheckAction(InputAction.Selection))
                AddUnit(UnitType.TestUnit, new TestUnit(1, 1, playerBase.Position, playerBase.GoalBase));

            if (input.CheckNewAction(InputAction.TowerBuild))
                BuildTower(TileMap.GetTileFromPos(avatar.Position));
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            avatar.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
            avatar.Draw(gameTime);
        }
    }
}
