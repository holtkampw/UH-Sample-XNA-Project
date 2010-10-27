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

    struct TowerInformation
    {
        public string price;
        public Vector2[] priceLocation;
        public string name;
        public Vector2[] nameLocation;
        public string description;
        public Vector2[] descriptionLocation;
    }

    public class Player
    {
        static Texture2D menuTab;
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
        static Vector2[] statusHealthNameLocation;
        const string HealthName = "Health: ";
        static Vector2[] statusHealthLocation;
        //Local Location of Status > NumberOfUnits
        static Vector2[] statusNumberOfUnitsNameLocation;
        const string UnitsName = "Total Units: ";
        static Vector2[] statusNumberOfUnitsLocation;

        //Local Location for Icons [playerNum][iconNum]
        static Vector2[][] iconLocations;
        static Rectangle[][] highlightIconLocations;
        static Texture2D[] defenseIcons;
        static Texture2D highlightIcon;
        static Rectangle highlightIconSourceRect;
        const int NUM_DEFENSE_TOWERS = 3;
        int defenseTowerSelected = 0;
        static Vector2 highlightOrigin;
        static float[] highlightRotations = {   0.0f, 0.1f, 0.2f, 0.3f, 0.4f, 0.5f, 0.6f, 0.7f, 0.8f, 0.9f,
                                                1.0f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f, 1.8f, 1.9f,
                                                2.0f, 2.1f, 2.2f, 2.3f, 2.4f, 2.5f, 2.6f, 2.7f, 2.8f, 2.9f,
                                                3.0f, 3.1f, 3.2f, 3.3f, 3.4f, 3.5f, 3.6f, 3.7f, 3.8f, 3.9f,
                                                4.0f, 4.1f, 4.2f, 4.3f, 4.4f, 4.5f, 4.6f, 4.7f, 4.8f, 4.9f,
                                                5.0f, 5.1f, 5.2f, 5.3f, 5.4f, 5.5f, 5.6f, 5.7f, 5.8f, 5.9f,
                                                6.0f, 6.1f  };
        int currentHighlightRotation = 0;
        static int maxHighlightRotations = highlightRotations.Length;
        int elapsedHighlightUpdateTime = 0;
        int maxHighlightUpdateTime = 25;

        //playerNum, towerNum
        TowerInformation[] defenseTowerInfo;
        SpriteFont towerTitle;
        SpriteFont towerPrice;
        SpriteFont towerDescription;

        //HumanPlayer
        AnimatedModel avatar;
        bool avatarMoved = true;
        StaticModel avatarFollowingTile;

        public int Money = 1000;
        public string MoneyString;
        public int Health;
        public string HealthString;
        static string AIMoneyString = "??????";
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

            //FIX THIS
            HealthString = Health.ToString();
            MoneyString = Money.ToString();

            //HumanPlayer
            if (Type == PlayerType.Human)
            {
                avatar = new AnimatedModel(playerNum, teamNum,
                    ScreenManager.Game.Content.Load<Model>("AnimatedModel\\dude"));

                avatar.Scale = 2.0f;
                avatar.PlayClip("Take 001");
                avatar.Position = new Vector3(PlayerBase.Position.X, 200.0f, PlayerBase.Position.Z);
                avatarMoved = false;
                avatarFollowingTile = new StaticModel(ScreenManager.Game.Content.Load<Model>("Objects\\Copter\\squarePlacer_red"), avatar.Position);
                avatarFollowingTile.Scale = 4.0f;
            }

            if (menuTab == null)
            {
                menuTab = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\playerMenuTab");    

                menuTabSource = new Rectangle[2];
                menuTabSource[SELECTED] = new Rectangle(0, menuTab.Width, menuTab.Width, menuTab.Height / 2);
                menuTabSource[NORMAL] = new Rectangle(0, 0, menuTab.Width, menuTab.Height / 2);
                statusFont = ScreenManager.Game.Content.Load<SpriteFont>("PlayerMenu\\statusFont");
                highlightIcon = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Icons\\icon_selector");

                tabLocation = new Vector2[5][];
                Vector2 tabOffset = new Vector2(15, 0);
                moneyLocation = new Vector2[5];
                statusHealthNameLocation = new Vector2[5];
                statusHealthLocation = new Vector2[5];
                statusNumberOfUnitsNameLocation = new Vector2[5];
                statusNumberOfUnitsLocation = new Vector2[5];
                iconLocations = new Vector2[5][];
                highlightIconLocations = new Rectangle[5][];
                Vector2 iconOffset = new Vector2(0, 36);

                defenseTowerInfo = new TowerInformation[NUM_DEFENSE_TOWERS];
                defenseIcons = new Texture2D[3];
                defenseIcons[0] = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Icons\\plasma_tower");
                defenseIcons[1] = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Icons\\electric_tower");
                defenseIcons[2] = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Icons\\cannon_tower");
                defenseTowerInfo[0].name = "Plasma Tower";
                defenseTowerInfo[0].price = "Price: $1000";
                defenseTowerInfo[0].description = "Shoots quick, but weak blasts\nat the enemy";
                defenseTowerInfo[0].nameLocation = new Vector2[5];
                defenseTowerInfo[0].descriptionLocation = new Vector2[5];
                defenseTowerInfo[0].priceLocation = new Vector2[5];

                defenseTowerInfo[1].name = "Electric Tower";
                defenseTowerInfo[1].price = "Price: $3000";
                defenseTowerInfo[1].description = "Attacks enemies in all directions";
                defenseTowerInfo[1].nameLocation = new Vector2[5];
                defenseTowerInfo[1].descriptionLocation = new Vector2[5];
                defenseTowerInfo[1].priceLocation = new Vector2[5];

                defenseTowerInfo[2].name = "Cannon Tower";
                defenseTowerInfo[2].price = "Price: $6000";
                defenseTowerInfo[2].description = "Powerful, long range attack\nbut slower than other towers";
                defenseTowerInfo[2].nameLocation = new Vector2[5];
                defenseTowerInfo[2].descriptionLocation = new Vector2[5];
                defenseTowerInfo[2].priceLocation = new Vector2[5];

                towerTitle = ScreenManager.Game.Content.Load<SpriteFont>("PlayerMenu\\towerTitle");
                towerPrice = ScreenManager.Game.Content.Load<SpriteFont>("PlayerMenu\\towerPrice");
                towerDescription = ScreenManager.Game.Content.Load<SpriteFont>("PlayerMenu\\towerDescription");

                for (int player = 1; player < 5; player++)
                {
                    Vector2 tabStartPosition = new Vector2(8, 148);
                    tabLocation[player] = new Vector2[NUM_TABS];
                    for (int t = 0; t < NUM_TABS; t++)
                    {
                        tabLocation[player][t] = globalLocations[player] + tabStartPosition;
                        tabStartPosition += tabOffset;
                    }

                    moneyLocation[player] = globalLocations[player] + new Vector2(110, -1);
                    statusHealthNameLocation[player] = globalLocations[player] + new Vector2(10, 40);
                    statusHealthLocation[player] = globalLocations[player] + new Vector2(120, 40);
                    statusNumberOfUnitsNameLocation[player] = globalLocations[player] + new Vector2(10, 80);
                    statusNumberOfUnitsLocation[player] = globalLocations[player] + new Vector2(180, 80);

                    iconLocations[player] = new Vector2[4];
                    highlightIconLocations[player] = new Rectangle[4];
                    Vector2 iconStartPosition = new Vector2(8, 36);
                    
                    for (int icon = 0; icon < 4; icon++)
                    {
                        iconLocations[player][icon] = globalLocations[player] + iconStartPosition;
                        highlightIconLocations[player][icon] = new Rectangle((int)(iconLocations[player][icon].X - 5.0f + (highlightIcon.Width / 2)), 
                            (int)(iconLocations[player][icon].Y - 5.0f + (highlightIcon.Height / 2)),
                            highlightIcon.Width, highlightIcon.Height);
                        iconStartPosition += iconOffset;
                    }

                    for (int tower = 0; tower < NUM_DEFENSE_TOWERS; tower++)
                    {
                        defenseTowerInfo[tower].nameLocation[player] = globalLocations[player] + new Vector2(70.0f, 30);
                        defenseTowerInfo[tower].priceLocation[player] = globalLocations[player] + new Vector2(70.0f, 64);
                        defenseTowerInfo[tower].descriptionLocation[player] = globalLocations[player] + new Vector2(70.0f, 94);
                    }
                }
                
                
                highlightIconSourceRect = new Rectangle(0, 0, highlightIcon.Width, highlightIcon.Height);
                highlightOrigin = new Vector2(highlightIcon.Width / 2.0f, highlightIcon.Height / 2.0f);
            }
                
            currentlySelectedPlayerStatus = PlayerMenuTabs.Status;
            if(Type == PlayerType.Human)
                playerMenuBg = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\player0" + playerNum);
            else
                playerMenuBg = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\player0" + playerNum + "computer");
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
                {
                    avatar.Position = avatar.Position + new Vector3(0, 0, -3);
                    avatarMoved = true;
                }

                if (input.CheckAction(InputAction.TileMoveDown))
                {
                    avatar.Position = avatar.Position + new Vector3(0, 0, 3);
                    avatarMoved = true;
                }

                if (input.CheckAction(InputAction.TileMoveLeft))
                {
                    avatar.Position = avatar.Position + new Vector3(-3, 0, 0);
                    avatarMoved = true;
                }

                if (input.CheckAction(InputAction.TileMoveRight))
                {
                    avatar.Position = avatar.Position + new Vector3(3, 0, 0);
                    avatarMoved = true;
                }

                if (avatar.Position.X < TileMap.Left)
                {
                    avatar.Position = new Vector3(TileMap.Left, avatar.Position.Y, avatar.Position.Z);
                    avatarMoved = true;
                }

                if (avatar.Position.Z < TileMap.Top)
                {
                    avatar.Position = new Vector3(avatar.Position.X, avatar.Position.Y, TileMap.Top);
                    avatarMoved = true;
                }

                if (avatar.Position.X > TileMap.Right)
                {
                    avatar.Position = new Vector3(TileMap.Right, avatar.Position.Y, avatar.Position.Z);
                    avatarMoved = true;
                }
                

                if (avatar.Position.Z > TileMap.Bottom)
                {
                    avatar.Position = new Vector3(avatar.Position.X, avatar.Position.Y, TileMap.Bottom);
                    avatarMoved = true;
                }

                if (input.CheckAction(InputAction.Selection))
                {
                    for(int i =0; i<1; i++)
                        UnitCollection.Add(PlayerNum, TeamNum, TargetPlayerNum, UnitType.TestUnit);
                }

                if (input.CheckNewAction(InputAction.TowerBuild))
                {
                    Tower tower = TowerCollection.Add(PlayerNum, TeamNum, Money, TowerType.TowerA, this.avatar.Position);
                    if(tower != null)
                        Money -= tower.Cost;

                    MoneyString = Money.ToString();
                }

                if (input.CheckNewAction(InputAction.TowerDestroy))
                {
                    Money += TowerCollection.Remove(PlayerNum, ref this.avatar.Position);
                    MoneyString = Money.ToString();
                }

                if (input.CheckNewAction(InputAction.TowerRepair))
                {
                    Money -= TowerCollection.Repair(PlayerNum, Money, ref this.avatar.Position);
                    MoneyString = Money.ToString();
                }

                if (input.CheckNewAction(InputAction.TowerUpgrade))
                {
                    Money -= TowerCollection.Upgrade(PlayerNum, Money, ref this.avatar.Position);
                    MoneyString = Money.ToString();
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

                if (input.CheckNewAction(InputAction.PlayerMenuUp))
                {
                    if (currentlySelectedPlayerStatus == PlayerMenuTabs.DefenseTower)
                    {
                        if (((int)defenseTowerSelected - 1) >= 0)
                            defenseTowerSelected--;
                    }
                }

                if (input.CheckNewAction(InputAction.PlayerMenuDown))
                {
                    if (currentlySelectedPlayerStatus == PlayerMenuTabs.DefenseTower)
                    {
                        if (((int)defenseTowerSelected + 1) < NUM_DEFENSE_TOWERS)
                            defenseTowerSelected++;
                    }
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
                if (avatarMoved)
                {
                    avatarMoved = false;
                    avatarFollowingTile.Position = TileMap.GetTilePosFromPos(avatar.Position);
                    avatarFollowingTile.Update(gameTime);
                }
            }

            elapsedHighlightUpdateTime += gameTime.ElapsedGameTime.Milliseconds;
            if (elapsedHighlightUpdateTime > maxHighlightUpdateTime)
            {
                elapsedHighlightUpdateTime = 0;
                if (currentHighlightRotation + 1 >= maxHighlightRotations)
                {
                    currentHighlightRotation = 0;
                }
                else
                {
                    currentHighlightRotation++;
                }
            }
        }

        public void Draw(GameTime gameTime)
        {
            PlayerBase.Draw(gameTime);

            //HumanPlayer
            if (Type == PlayerType.Human)
            {
                avatar.Draw(gameTime);
                avatarFollowingTile.Draw(gameTime);
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
            TileMap.SetTowerForLevelMap(TowerCollection.Add(PlayerNum, TeamNum, 100000, towerType, tile.Position), tile);
        }

        public void DrawMenu(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(playerMenuBg, globalLocations[PlayerNum], Color.White);
            
            if (Type == PlayerType.Human)
            {
                ScreenManager.SpriteBatch.DrawString(statusFont, MoneyString, moneyLocation[PlayerNum], Color.White);

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
                        DrawDefenseTowers();
                        break;
                    case PlayerMenuTabs.UnitTower:
                        break;
                    case PlayerMenuTabs.Powers:
                        break;
                }
            }
            else
            {
                ScreenManager.SpriteBatch.DrawString(statusFont, AIMoneyString, moneyLocation[PlayerNum], Color.White);
            }

            ScreenManager.SpriteBatch.End();

        }

        void DrawStatus()
        {
            ScreenManager.SpriteBatch.DrawString(statusFont, HealthName, statusHealthNameLocation[PlayerNum], Color.White);
            ScreenManager.SpriteBatch.DrawString(statusFont, HealthString, statusHealthLocation[PlayerNum], Color.White);
            ScreenManager.SpriteBatch.DrawString(statusFont, UnitsName, statusNumberOfUnitsNameLocation[PlayerNum], Color.White);
            ScreenManager.SpriteBatch.DrawString(statusFont, UnitCollection.UnitCountForPlayerString(PlayerNum), 
                statusNumberOfUnitsLocation[PlayerNum], Color.White);
        }

        void DrawDefenseTowers()
        {
            for (int i = 0; i < NUM_DEFENSE_TOWERS; i++)
            {
                ScreenManager.SpriteBatch.Draw(defenseIcons[i], iconLocations[PlayerNum][i], Color.White);
                if (i == defenseTowerSelected)
                {
                    ScreenManager.SpriteBatch.Draw(highlightIcon,
                        highlightIconLocations[PlayerNum][i],
                        highlightIconSourceRect, 
                        Color.White, 
                        highlightRotations[currentHighlightRotation], 
                        highlightOrigin,
                        SpriteEffects.None, 
                        1.0f);
                    ScreenManager.SpriteBatch.DrawString(towerTitle, defenseTowerInfo[i].name, 
                        defenseTowerInfo[i].nameLocation[PlayerNum], Color.White);
                    ScreenManager.SpriteBatch.DrawString(towerPrice, defenseTowerInfo[i].price, 
                        defenseTowerInfo[i].priceLocation[PlayerNum], Color.White);
                    ScreenManager.SpriteBatch.DrawString(towerDescription, defenseTowerInfo[i].description, 
                        defenseTowerInfo[i].descriptionLocation[PlayerNum], Color.White);
                }
                
            }

        }
    }
}
