using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.CoreObjects;
using UHSampleGame.CoreObjects.Base;
using UHSampleGame.CoreObjects.Towers;
using UHSampleGame.CoreObjects.Units;
using UHSampleGame.TileSystem;
using UHSampleGame.InputManagement;
using UHSampleGame.ScreenManagement;

namespace UHSampleGame.Player
{
    public abstract class Player
    {
        Base playerBase;
        TeamableAnimatedObject avatar;
        List<Tower> towers;
        List<Unit> units;
        int money;
        int playerNum;
        int teamNum;

        public int PlayerNum
        {
            get { return playerNum; }
        }

        public int TeamNum
        {
            get { return teamNum; }
        }

        public Base Base
        {
            get { return playerBase; }
        }

        public Player(int playerNum, int teamNum, Tile startTile)
        {
            this.playerNum = playerNum;
            this.teamNum = teamNum;
            this.playerBase = new TestBase(playerNum, teamNum, startTile);
            TileMap.SetBase(playerBase);
            this.towers = new List<Tower>();
            this.units = new List<Unit>();
            this.money = 0;
            this.avatar = new TeamableAnimatedObject(playerNum, teamNum,
                ScreenManager.Game.Content.Load<Model>("AnimatedModel\\dude"));

            avatar.Scale = 2.0f;
            avatar.PlayClip("Take 001");
            avatar.SetPosition(playerBase.Position);
        }

        public void SetTargetBase(Base target)
        {
            playerBase.SetGoalBase(target);
        }

        public void HandleInput(InputManager input)
        {
            if (input.CheckAction(InputAction.TileMoveUp))
            {
                avatar.SetPosition(avatar.Position + new Vector3(0, 0, -3));
            }
            if (input.CheckAction(InputAction.TileMoveDown))
            {
                avatar.SetPosition(avatar.Position + new Vector3(0, 0, 3));
            }
            if (input.CheckAction(InputAction.TileMoveLeft))
            {
                avatar.SetPosition(avatar.Position + new Vector3(-3, 0, 0));
            }
            if (input.CheckAction(InputAction.TileMoveRight))
            {
                avatar.SetPosition(avatar.Position + new Vector3(3, 0, 0));
            }

            if (input.CheckAction(InputAction.Selection))
            {
                AddUnit(new TestUnit(1, 1, playerBase.Position, playerBase.GoalBase));
            }

            if (avatar.Position.X < TileMap.Left)
                avatar.SetPosition(new Vector3(TileMap.Left, avatar.Position.Y, avatar.Position.Z));

            if (avatar.Position.Z < TileMap.Top)
                avatar.SetPosition(new Vector3(avatar.Position.X, avatar.Position.Y, TileMap.Top));

            if (avatar.Position.X > TileMap.Right)
                avatar.SetPosition(new Vector3(TileMap.Right, avatar.Position.Y, avatar.Position.Z));

            if (avatar.Position.Z > TileMap.Bottom)
                avatar.SetPosition(new Vector3(avatar.Position.X, avatar.Position.Y, TileMap.Bottom));

            if (input.CheckNewAction(InputAction.TowerBuild))
            {
                BuildTower(TileMap.GetTileFromPos(avatar.Position));
            }
        }

        protected void BuildTower(Tile tile)
        {
            Tower tower = new TowerAGood(1, 1, tile);
            if (TileMap.SetTower(tower, tile))
                towers.Add(tower);
        }

        protected void AddUnit(Unit unit)
        {
            unit.Died += RemoveUnit;
            units.Add(unit);
            TileMap.TowerCreated += unit.UpdatePath;
        }

        protected void RemoveUnit(Unit unit)
        {
            units.Remove(unit);
            TileMap.TowerCreated -= unit.UpdatePath;
        }

        public void Update(GameTime gameTime)
        {
            playerBase.Update(gameTime);

            for (int i = 0; i < towers.Count; i++)
                towers[i].Update(gameTime);

            for (int i = 0; i < units.Count; i++)
                units[i].Update(gameTime);

            avatar.Update(gameTime);

        }

        public void Draw(GameTime gameTime)
        {
            playerBase.Draw(gameTime);

            for (int i = 0; i < towers.Count; i++)
                towers[i].Draw(gameTime);

            for (int i = 0; i < units.Count; i++)
                units[i].Draw(gameTime);

            avatar.Draw(gameTime);
        }
    }
}
