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
    public enum PlayerType { Human, AI };
    public enum PlayerMenuTabs { Status, DefenseTower, UnitTower, Powers}
    public class Player
    {
        static Texture2D menuTab;
        static List<List<Rectangle>> menuTabStartPositions;
        static Vector2 menuTabOffset = new Vector2(15.0f, 0.0f);
        Texture2D playerMenuBg;

        const int NORMAL = 0;
        const int SELECTED = 1;
        static Rectangle[] menuTabSource;
        PlayerMenuTabs currentlySelectedPlayerStatus;

        public static Enum[] playerMenuTabsEnumType = EnumHelper.EnumToArray(new PlayerMenuTabs());
        static int NUM_TABS = playerMenuTabsEnumType.Length;
        static SpriteFont statusFont;

        public Base PlayerBase;

        protected int PlayerNum;
        protected int TeamNum;

        protected int TargetPlayerNum;

        protected CameraManager cameraManager;

        public PlayerType Type;
        public static Enum[] playerEnumType = EnumHelper.EnumToArray(new PlayerType());

        //Global Location of Player (array of 4)
        static Vector2[] globalLocations = { Vector2.Zero, new Vector2(40f, 30f), new Vector2(40f, 200f), new Vector2(40f, 370f), new Vector2(40f, 540f) };
        //Local Location of Tab [playerNum][tabNum]
        static Vector2[][] tabLocation;

        //Local Location of money
        static Vector2[] moneyLocation;
        //Local Location of Status > Health
        static Vector2[] statusHealthLocation;
        //Local Location of Status > NumberOfUnits
        static Vector2[] statusNumberOfUnitsLocation;

        //Local Location of Icons
        //Local Location of Icon Offset
        
        //Local Location of 

        //HumanPlayer
        TeamableAnimatedObject avatar;
        public int Money;
        public int Health;
        #region Properties
       
        #endregion

        public Player(int playerNum, int teamNum, int targetPlayerNum, Tile startTile, PlayerType type)
        {
            this.PlayerNum = playerNum;
            this.TargetPlayerNum = targetPlayerNum;
            this.TeamNum = teamNum;
            this.Type = type;
            SetBase(new Base(playerNum, teamNum, BaseType.type1, startTile));
            
            this.cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));

            //HumanPlayer
            if (Type == PlayerType.Human)
            {
                //avatar = new TeamableAnimatedObject(playerNum, teamNum,
                //    ScreenManager.Game.Content.Load<Model>("AnimatedModel\\dude"));

                //avatar.Scale = 2.0f;
                //avatar.PlayClip("Take 001");
                //avatar.SetPosition(PlayerBase.Position);
            }

            if (menuTab == null)
            {
                menuTab = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\playerMenuTab");    

                menuTabSource = new Rectangle[2];
                menuTabSource[SELECTED] = new Rectangle(0, menuTab.Width, menuTab.Width, menuTab.Height / 2);
                menuTabSource[NORMAL] = new Rectangle(0, 0, menuTab.Width, menuTab.Height / 2);
                statusFont = ScreenManager.Game.Content.Load<SpriteFont>("PlayerMenu\\statusFont");
               
            }
            if (tabLocation == null)
            {
                tabLocation = new Vector2[5][];
                Vector2 tabOffset = new Vector2(15, 0);
                moneyLocation = new Vector2[5];
                statusHealthLocation = new Vector2[5];
                statusNumberOfUnitsLocation = new Vector2[5];
                for (int i = 1; i < 5; i++)
                {
                    Vector2 tabStartPosition = new Vector2(8, 148);
                    tabLocation[i] = new Vector2[NUM_TABS];
                    for (int t = 0; t < NUM_TABS; t++)
                    {
                        tabLocation[i][t] = globalLocations[i] + tabStartPosition;
                        tabStartPosition += tabOffset;
                    }

                    moneyLocation[i] = globalLocations[i] + new Vector2(110, -1);
                    statusHealthLocation[i] = globalLocations[i] + new Vector2(10, 40);
                    statusNumberOfUnitsLocation[i] = globalLocations[i] + new Vector2(10, 80);
                }
            }
                currentlySelectedPlayerStatus = PlayerMenuTabs.Status;
            playerMenuBg = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\player0" + playerNum);
        }

        public void SetBase(Base playerBase)
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

                //if (avatar.Position.X < TileMap.Left)
                //    avatar.SetPosition(new Vector3(TileMap.Left, avatar.Position.Y, avatar.Position.Z));

                //if (avatar.Position.Z < TileMap.Top)
                //    avatar.SetPosition(new Vector3(avatar.Position.X, avatar.Position.Y, TileMap.Top));

                //if (avatar.Position.X > TileMap.Right)
                //    avatar.SetPosition(new Vector3(TileMap.Right, avatar.Position.Y, avatar.Position.Z));

                //if (avatar.Position.Z > TileMap.Bottom)
                //    avatar.SetPosition(new Vector3(avatar.Position.X, avatar.Position.Y, TileMap.Bottom));

                if (input.CheckAction(InputAction.Selection))
                {
                    for(int i =0; i<10; i++)
                        UnitCollection.Add(PlayerNum, TeamNum, TargetPlayerNum, UnitType.TestUnit);
                }

                if (input.CheckNewAction(InputAction.TowerBuild))
                {
                    TowerCollection.Add(PlayerNum, TeamNum, TowerType.TowerA, this.avatar.Position);
                }

                if (input.CheckNewAction(InputAction.PlayerMenuLeft))
                {
                    if (((int)currentlySelectedPlayerStatus - 1) >= 0)
                        currentlySelectedPlayerStatus--;
                }

                if (input.CheckNewAction(InputAction.PlayerMenuRight))
                {
                    if (((int)currentlySelectedPlayerStatus + 1) < playerMenuTabsEnumType.Length)
                        currentlySelectedPlayerStatus++;
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            PlayerBase.Update(gameTime);

            //HumanPlayer
            if (Type == PlayerType.Human)
            {
               // avatar.Update(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            PlayerBase.Draw(gameTime);

            //HumanPlayer
            if (Type == PlayerType.Human)
            {
               // avatar.Draw(gameTime);
            }

            DrawMenu(gameTime);
        }

        public void SetTargetBase(Base target)
        {
            PlayerBase.SetGoalBase(target);
           // target.baseDestroyed += GetNewTargetBase;
        }

        public void SetTowerForLevelMap(TowerType towerType, Tile tile)
        {
            TileMap.SetTowerForLevelMap(TowerCollection.Add(PlayerNum, TeamNum, towerType, tile.Position), tile);
        }

        public void DrawMenu(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(playerMenuBg, globalLocations[PlayerNum], Color.White);
            
            if (Type == PlayerType.Human)
            {
                ScreenManager.SpriteBatch.DrawString(statusFont, Money.ToString(), moneyLocation[PlayerNum], Color.White);

                for (int i = 0; i < NUM_TABS; i++)
                {
                    if ((int)currentlySelectedPlayerStatus == i)
                    {
                        ScreenManager.SpriteBatch.Draw(menuTab, tabLocation[PlayerNum][i],
                            menuTabSource[SELECTED], Color.White);
                    }
                    else
                    {
                        ScreenManager.SpriteBatch.Draw(menuTab, tabLocation[PlayerNum][i],
                            menuTabSource[NORMAL], Color.White);
                    }
                }

                switch (currentlySelectedPlayerStatus)
                {
                    case PlayerMenuTabs.Status:
                        DrawStatus();
                        break;
                    case PlayerMenuTabs.DefenseTower:
                        break;
                    case PlayerMenuTabs.UnitTower:
                        break;
                    case PlayerMenuTabs.Powers:
                        break;
                }
            }
            else
            {
                ScreenManager.SpriteBatch.DrawString(statusFont, "??????", moneyLocation[PlayerNum], Color.White);
            }

            ScreenManager.SpriteBatch.End();

        }

        void DrawStatus()
        {
            ScreenManager.SpriteBatch.DrawString(statusFont, Health.ToString(), statusHealthLocation[PlayerNum], Color.White);
            ScreenManager.SpriteBatch.DrawString(statusFont, UnitCollection.UnitCountForPlayer(PlayerNum).ToString(), 
                statusNumberOfUnitsLocation[PlayerNum], Color.White);

        }
    }
}
