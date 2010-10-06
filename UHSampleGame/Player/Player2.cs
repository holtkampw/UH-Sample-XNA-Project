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
using UHSampleGame.Events;

namespace UHSampleGame.Players
{
    public class Player2
    {

        
        public Base2 PlayerBase;

        protected int PlayerNum;
        protected int TeamNum;

        protected CameraManager cameraManager;

        public PlayerType Type;
        public static Enum[] playerEnumType = EnumHelper.EnumToArray(new PlayerType());

        //HumanPlayer
        TeamableAnimatedObject avatar;

        #region Properties
       
        #endregion

        public Player2(int playerNum, int teamNum, Tile2 startTile, PlayerType type)
        {
            this.PlayerNum = playerNum;
            this.TeamNum = teamNum;
            this.Type = type;
            SetBase(new TestBase2(playerNum, teamNum, startTile));
            
            this.cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));

            //HumanPlayer
            if (Type == PlayerType.Human)
            {
                avatar = new TeamableAnimatedObject(playerNum, teamNum,
                    ScreenManager.Game.Content.Load<Model>("AnimatedModel\\dude"));

                avatar.Scale = 2.0f;
                avatar.PlayClip("Take 001");
                avatar.SetPosition(PlayerBase.Position);
            }
        }

        public void SetBase(Base2 playerBase)
        {
            this.PlayerBase = playerBase;
            // TileMap2.SetBase(playerBase);
        }

        public void HandleInput(InputManager input)
        {
            //Human Player
            if (Type == PlayerType.Human)
            {
                if (input.CheckAction(InputAction.TileMoveUp))
                    avatar.SetPosition(avatar.Position + new Vector3(0, 0, -3));

                if (input.CheckAction(InputAction.TileMoveDown))
                    avatar.SetPosition(avatar.Position + new Vector3(0, 0, 3));

                if (input.CheckAction(InputAction.TileMoveLeft))
                    avatar.SetPosition(avatar.Position + new Vector3(-3, 0, 0));

                if (input.CheckAction(InputAction.TileMoveRight))
                    avatar.SetPosition(avatar.Position + new Vector3(3, 0, 0));

                if (avatar.Position.X < TileMap2.Left)
                    avatar.SetPosition(new Vector3(TileMap2.Left, avatar.Position.Y, avatar.Position.Z));

                if (avatar.Position.Z < TileMap2.Top)
                    avatar.SetPosition(new Vector3(avatar.Position.X, avatar.Position.Y, TileMap2.Top));

                if (avatar.Position.X > TileMap2.Right)
                    avatar.SetPosition(new Vector3(TileMap2.Right, avatar.Position.Y, avatar.Position.Z));

                if (avatar.Position.Z > TileMap2.Bottom)
                    avatar.SetPosition(new Vector3(avatar.Position.X, avatar.Position.Y, TileMap2.Bottom));

                if (input.CheckAction(InputAction.Selection))
                {
                    UnitCollection.Add(PlayerNum, UnitType.TestUnit);
                    UnitCollection.Add(PlayerNum, UnitType.TestUnit);
                    UnitCollection.Add(PlayerNum, UnitType.TestUnit);
                    UnitCollection.Add(PlayerNum, UnitType.TestUnit);
                    UnitCollection.Add(PlayerNum, UnitType.TestUnit);
                    UnitCollection.Add(PlayerNum, UnitType.TestUnit);
                    UnitCollection.Add(PlayerNum, UnitType.TestUnit);
                    UnitCollection.Add(PlayerNum, UnitType.TestUnit);
                    UnitCollection.Add(PlayerNum, UnitType.TestUnit);
                    UnitCollection.Add(PlayerNum, UnitType.TestUnit);
                }

                if (input.CheckNewAction(InputAction.TowerBuild))
                {
                    TowerCollection.Add(PlayerNum, TeamNum, TowerType.TowerA, this.avatar.Position);
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            PlayerBase.Update(gameTime);

            //HumanPlayer
            if (Type == PlayerType.Human)
            {
                avatar.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            PlayerBase.Draw(gameTime);

            //HumanPlayer
            if (Type == PlayerType.Human)
            {
                avatar.Draw(gameTime);
            }
        }

        public void SetTargetBase(Base2 target)
        {
            PlayerBase.SetGoalBase(target);
           // target.baseDestroyed += GetNewTargetBase;
        }

        public void SetTowerForLevelMap(Tower2 tower, Tile2 tile)
        {
            TileMap2.SetTowerForLevelMap(tower, tile);
            TowerCollection.Add(PlayerNum, TeamNum, tower.Type, tile.Position);
        }
    }
}
