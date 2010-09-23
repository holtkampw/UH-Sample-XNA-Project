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
using UHSampleGame.CameraManagement;
using UHSampleGame.InstancedModels;

namespace UHSampleGame.Player
{
    public abstract class Player
    {
        protected Base playerBase;

        protected List<Tower> towers;
        protected Dictionary<UnitType, List<Unit>> units;
        protected Dictionary<UnitType, int> previousCount;
        protected Dictionary<UnitType, InstancedModel> instancedModels;
        protected int money;
        protected int playerNum;
        protected int teamNum;

        protected SkinnedEffect genericEffect;
        protected CameraManager cameraManager;

        //Transforms
        Dictionary<UnitType, List<Matrix>> unitTransforms;

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
            get {
                int count = 0;
                foreach (UnitType key in Enum.GetValues(typeof(UnitType)))
                {
                    count += units[key].Count;
                }
                return count; 
            }
        }

        public Player(int playerNum, int teamNum, Tile startTile)
        {
            this.playerNum = playerNum;
            this.teamNum = teamNum;
            this.playerBase = new TestBase(playerNum, teamNum, startTile);
            TileMap.SetBase(playerBase);
            this.towers = new List<Tower>();
            this.units = new Dictionary<UnitType, List<Unit>>();
            previousCount = new Dictionary<UnitType, int>();
            unitTransforms = new Dictionary<UnitType, List<Matrix>>();
            instancedModels = new Dictionary<UnitType, InstancedModel>();
            foreach(UnitType key in Enum.GetValues(typeof(UnitType)))
            {
                units[key] = new List<Unit>();
                previousCount[key] = 0;
                unitTransforms[key] = new List<Matrix>();
                instancedModels[key] = new InstancedModel(ScreenManager.Game.GraphicsDevice);
                switch(key)
                {
                    case UnitType.TestUnit:
                        instancedModels[key].Setup(TestUnit.Model);
                        break;
                }
            }
            this.money = 0;
            this.cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));

            this.genericEffect = new SkinnedEffect(ScreenManager.Game.GraphicsDevice);
            genericEffect.View = cameraManager.ViewMatrix;
            genericEffect.Projection = cameraManager.ProjectionMatrix;
            
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

        protected void AddUnit(UnitType type, Unit unit)
        {
            unit.Died += RemoveUnit;
            units[type].Add(unit);
            TileMap.TowerCreated += unit.UpdatePath;
        }

        protected void RemoveUnit(UnitType type, Unit unit)
        {
            units[type].Remove(unit);
            TileMap.TowerCreated -= unit.UpdatePath;
        }

        public virtual void Update(GameTime gameTime)
        {
            playerBase.Update(gameTime);

            for (int i = 0; i < towers.Count; i++)
                towers[i].Update(gameTime);

            foreach (UnitType key in Enum.GetValues(typeof(UnitType)))
                for (int i = 0; i < units[key].Count; i++)
                    units[key][i].Update(gameTime);
        }

        public virtual void Draw(GameTime gameTime)
        {
            playerBase.Draw(gameTime);

            for (int i = 0; i < towers.Count; i++)
                towers[i].Draw(gameTime);

            foreach (UnitType key in Enum.GetValues(typeof(UnitType)))
            {
                bool clear = false;
                if (units[key].Count != previousCount[key])
                {
                    ClearTransforms(key);
                    clear = true;
                }
                previousCount[key] = units[key].Count;

                for (int i = 0; i < units[key].Count; i++)
                {
                    if(clear)
                        unitTransforms[key].Add(units[key][i].Transforms);
                    else
                        unitTransforms[key][i] = units[key][i].Transforms;
                } 

            }

            DrawUnits(gameTime);
        }

        private void ClearTransforms(UnitType type)
        {
           unitTransforms[type].Clear();
        }

        private void DrawUnits(GameTime gameTime)
        {
            genericEffect.World = cameraManager.ViewMatrix;

            foreach (UnitType key in Enum.GetValues(typeof(UnitType)))
            {
                if (unitTransforms[key].Count > 0)
                {
                    Matrix[] tempTransforms = unitTransforms[key].ToArray();
                    instancedModels[key].Draw(ref tempTransforms, ref genericEffect);
                }
            }
        }
    }
}
