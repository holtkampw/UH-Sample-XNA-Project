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
        protected Base playerBase;

        protected List<Tower> towers;
        protected List<Unit> units;
        protected int money;
        protected int playerNum;
        protected int teamNum;

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

        public int TowerCount
        {
            get { return towers.Count; }
        }

        public int UnitCount
        {
            get { return units.Count; }
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
            

            
        }

        public void SetTargetBase(Base target)
        {
            playerBase.SetGoalBase(target);
        }

        public virtual void HandleInput(InputManager input)
        {
           
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

        public virtual void Update(GameTime gameTime)
        {
            playerBase.Update(gameTime);

            for (int i = 0; i < towers.Count; i++)
                towers[i].Update(gameTime);

            for (int i = 0; i < units.Count; i++)
                units[i].Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime)
        {
            playerBase.Draw(gameTime);

            for (int i = 0; i < towers.Count; i++)
                towers[i].Draw(gameTime);

            for (int i = 0; i < units.Count; i++)
                units[i].Draw(gameTime);
        }
    }
}
