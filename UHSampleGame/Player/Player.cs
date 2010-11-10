﻿using System;
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
using UHSampleGame.ProjectileManagment;

namespace UHSampleGame.Players
{
    public enum PlayerType { Human, AI };
    public enum PlayerMenuTabs { Status, UnitTower, DefenseTower, Powers }

    struct TowerInformation
    {
        public string price;
        public Vector2[] priceLocation;
        public string name;
        public Vector2[] nameLocation;
        public string description;
        public Vector2[] descriptionLocation;
        public TowerType type;
    }

    struct UnitInformation
    {
        public UnitType type;
        public Texture2D icon;
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

        public int PlayerNum;
        public int TeamNum;

        public int TargetPlayerNum;

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
        static Vector2[] computerStatusHealthLocation;
        //Local Location of Status > NumberOfUnits
        static Vector2[] statusNumberOfUnitsLocation;
        static Vector2[] computerStatusNumberOfUnitsLocation;

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
        TowerType lastBuiltTower;

        //playerNum, towerNum
        TowerInformation[] defenseTowerInfo;
        SpriteFont towerTitle;
        SpriteFont towerPrice;
        SpriteFont towerDescription;

        static Texture2D[] offenseIcons;
        const int NUM_OFFENSE_TOWERS = 2;
        int offenseTowerSelected = 0;
        TowerInformation[] offenseTowerInfo;

        //HumanPlayer
        Avatar avatar;
        bool avatarMoved = true;
        StaticModel avatarFollowingTile;
        public bool IsActive = true;
        public bool IsDead = false;

        public int Money = 10000;
        public string MoneyString;
        public int Health = 100;
        public string HealthString;
        static string AIMoneyString = "??????";

        //Current Selected Unit 
        // 0 | 1 | 2
        // 3 | - | 4
        // 5 | 6 | 7
        const int MAX_UNIT_TYPES = 8;
        public int selectedUnit = 1;

        //[playerNum][position]
        Vector2[][] unitIconLocation;
        Texture2D[] unitIcons;

        //[playerNum][position]
        Rectangle[][] highlightUnitIconLocations;
        UnitInformation[] unitInformation;
        bool unitScreenActivated = false;
        Vector2 unitSelectionPosition = Vector2.Zero;
        int queuedUnits = 0; //Amount of Units Queued
        int queuedUnitAmount = 1;  //Amount to add each time
        float percentOfUnitsQueued = 0f; //Percent of Total Units to Queue
        int unitsDeployed = 0; //Number of Units deployed from the current Queue
        int queuedUnitsToDeploy = 0;
        bool queuedDeclineMode = false;

        int queuedUnitType;
        int elapsedUnitDeployTime = 0;
        int maxUnitDeployTime = 50;
        Texture2D unitMeterBaseTexture;
        Rectangle[] unitMeterBaseLocation;

        Texture2D[] unitMeterOverlayTexture;
        Rectangle unitMeterOverlaySource;
        Rectangle[] unitMeterOverlayDestination;
        float[] unitMeterOverlayBaseY;
        float[] unitMeterOverlayMaxHeight;

        static PlayerIndex[] playerIndexes = { 0, PlayerIndex.One, PlayerIndex.Two, PlayerIndex.Three, PlayerIndex.Four };
        static char[] mapTeamNumToTeamChar = { ' ', 'A', 'B', 'C', 'D' };

        //PlayerNum, TeamNum
        static Texture2D[][] defensiveTab;
        static Texture2D[][] offensiveTab;
        static Texture2D[][] powersTab;
        static Texture2D[][] statusTab;
        static Texture2D[] computerTags;
        static Vector2[] computerTagLocations;
        static Texture2D[][] backgroundTabs;

        public bool isHUDDisplayed = false;

        static Texture2D healthicon;
        static Texture2D numUnitsIcon;
        static Vector2[] healthIconLocations;
        static Vector2[] numUnitsIconLocations;
        static Vector2 healthIconOffset = new Vector2(60, 33);
        static Vector2 numUnitsIconOffset = new Vector2(60, 85);
        static SpriteFont statusScreenFont;

        static Texture2D[] attackingPlayer;
        int elapsedPlayerToAttackChange = 0;
        int maxPlayerToAttackChange = 2000;
        bool pickEnemyTargetPressed = false;
        Color enemyTargetOpacity = new Color(255, 255, 255, 0);
        static Vector2[] attackingPlayerLocation;


        #region Properties
       
        #endregion

        public void SharedSetup(int playerNum, int teamNum, PlayerType type)
        {
            this.PlayerNum = playerNum;
            this.TeamNum = teamNum;
            this.Type = type;

            IsActive = true;
            IsDead = false;

            this.cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));

            //FIX THIS
            HealthString = Health.ToString();
            MoneyString = Money.ToString();

            SetupStatic();
            //SetupNonStatic();

            currentlySelectedPlayerStatus = PlayerMenuTabs.Status;
            if (Type == PlayerType.Human)
                playerMenuBg = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\player0" + playerNum);
            else
                playerMenuBg = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\player0" + playerNum + "computer");
        }

        public Player()
        {
            IsActive = false;
            IsDead = true;
        }

        public void PlayerSetup(int playerNum, int teamNum, Base gameObject, PlayerType playerType)
        {
            SharedSetup(playerNum, teamNum, playerType);
            SetBase(gameObject);
        }

        public Player(int playerNum, int teamNum, Base gameObject, PlayerType playerType)
        {
            SharedSetup(playerNum, teamNum, playerType);
            SetBase(gameObject);
        }

        public Player(int playerNum, int teamNum, int targetPlayerNum, Tile startTile, PlayerType type)
        {
            SharedSetup(playerNum, teamNum, type);
            this.TargetPlayerNum = targetPlayerNum;
            SetBase(new Base(playerNum, teamNum, BaseType.type1, startTile));

            SetupAvatar(); 
            
        }

        public void SetupStatic()
        {
            if (/*menuTab == null*/ true)
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
                statusHealthLocation = new Vector2[5];
                computerStatusHealthLocation = new Vector2[5];
                statusNumberOfUnitsLocation = new Vector2[5];
                computerStatusNumberOfUnitsLocation = new Vector2[5];
                iconLocations = new Vector2[5][];
                highlightIconLocations = new Rectangle[5][];
                Vector2 iconOffset = new Vector2(0, 36);

                defenseTowerInfo = new TowerInformation[NUM_DEFENSE_TOWERS];
                defenseIcons = new Texture2D[3];
                defenseIcons[0] = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Icons\\plasma_tower");
                defenseIcons[1] = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Icons\\electric_tower");
                defenseIcons[2] = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Icons\\plasma360_tower");
                defenseTowerInfo[0].name = "Plasma Tower";
                defenseTowerInfo[0].price = "Price: $1000";
                defenseTowerInfo[0].description = "Shoots quick, but weak\nblasts at the enemy";
                defenseTowerInfo[0].nameLocation = new Vector2[5];
                defenseTowerInfo[0].descriptionLocation = new Vector2[5];
                defenseTowerInfo[0].priceLocation = new Vector2[5];
                defenseTowerInfo[0].type = TowerType.Plasma;

                defenseTowerInfo[1].name = "Electric Tower";
                defenseTowerInfo[1].price = "Price: $3000";
                defenseTowerInfo[1].description = "Attacks enemies in\nall directions";
                defenseTowerInfo[1].nameLocation = new Vector2[5];
                defenseTowerInfo[1].descriptionLocation = new Vector2[5];
                defenseTowerInfo[1].priceLocation = new Vector2[5];
                defenseTowerInfo[1].type = TowerType.Electric;

                defenseTowerInfo[2].name = "Plasma360 Tower";
                defenseTowerInfo[2].price = "Price: $6000";
                defenseTowerInfo[2].description = "A plasma tower that\nshoots in multiple\ndirections at once.";
                defenseTowerInfo[2].nameLocation = new Vector2[5];
                defenseTowerInfo[2].descriptionLocation = new Vector2[5];

                defenseTowerInfo[2].priceLocation = new Vector2[5];
                defenseTowerInfo[2].type = TowerType.Cannon;

                lastBuiltTower = TowerType.Plasma;
                defenseTowerInfo[2].priceLocation = new Vector2[5];

                towerTitle = ScreenManager.Game.Content.Load<SpriteFont>("PlayerMenu\\towerTitle");
                towerPrice = ScreenManager.Game.Content.Load<SpriteFont>("PlayerMenu\\towerPrice");
                towerDescription = ScreenManager.Game.Content.Load<SpriteFont>("PlayerMenu\\towerDescription");

                offenseTowerInfo = new TowerInformation[NUM_OFFENSE_TOWERS];
                offenseIcons = new Texture2D[NUM_OFFENSE_TOWERS];
                offenseIcons[0] = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Icons\\plasma_tower");
                offenseIcons[1] = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Icons\\electric_tower");

                offenseTowerInfo[0].name = "Small Training Area";
                offenseTowerInfo[0].price = "Price: $3000";
                offenseTowerInfo[0].description = "Trains units\nfor use";
                offenseTowerInfo[0].nameLocation = new Vector2[5];
                offenseTowerInfo[0].descriptionLocation = new Vector2[5];
                offenseTowerInfo[0].priceLocation = new Vector2[5];
                offenseTowerInfo[0].type = TowerType.SmallUnit;

                offenseTowerInfo[1].name = "Large Training Area";
                offenseTowerInfo[1].price = "Price: $6000";
                offenseTowerInfo[1].description = "Trains units\nfor use much\nmore quickly";
                offenseTowerInfo[1].nameLocation = new Vector2[5];
                offenseTowerInfo[1].descriptionLocation = new Vector2[5];
                offenseTowerInfo[1].priceLocation = new Vector2[5];
                offenseTowerInfo[1].type = TowerType.LargeUnit;

                offenseTowerInfo = new TowerInformation[NUM_OFFENSE_TOWERS];
                offenseIcons = new Texture2D[NUM_OFFENSE_TOWERS];
                offenseIcons[0] = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Icons\\plasma_tower");
                offenseIcons[1] = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Icons\\electric_tower");

                offenseTowerInfo[0].name = "Small Training Area";
                offenseTowerInfo[0].price = "Price: $3000";
                offenseTowerInfo[0].description = "Trains units\nfor use";
                offenseTowerInfo[0].nameLocation = new Vector2[5];
                offenseTowerInfo[0].descriptionLocation = new Vector2[5];
                offenseTowerInfo[0].priceLocation = new Vector2[5];
                offenseTowerInfo[0].type = TowerType.SmallUnit;

                offenseTowerInfo[1].name = "Large Training Area";
                offenseTowerInfo[1].price = "Price: $6000";
                offenseTowerInfo[1].description = "Trains units\nfor use much\nmore quickly";
                offenseTowerInfo[1].nameLocation = new Vector2[5];
                offenseTowerInfo[1].descriptionLocation = new Vector2[5];
                offenseTowerInfo[1].priceLocation = new Vector2[5];
                offenseTowerInfo[1].type = TowerType.LargeUnit;

                unitIconLocation = new Vector2[5][];
                highlightUnitIconLocations = new Rectangle[5][];
                unitIcons = new Texture2D[MAX_UNIT_TYPES];
                Vector2 unitIconOffset = new Vector2(44.0f, 0.0f);
                Vector2 unitIconRowOffset = new Vector2(0.0f, 44.0f);
                unitInformation = new UnitInformation[MAX_UNIT_TYPES];

                unitInformation[1].type = UnitType.SpeedBoat;
                unitInformation[1].icon = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Icons\\speedBoat");

                unitInformation[6].type = UnitType.SpeederBoat;
                unitInformation[6].icon = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Icons\\speederBoat");

                unitMeterBaseTexture = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\unit_meter_base");
                unitMeterBaseLocation = new Rectangle[5];
                unitMeterOverlayMaxHeight = new float[5];
                
                //unitMeterHighlightTexture = new Texture2D[5];
                unitMeterOverlayTexture = new Texture2D[5];
                for (int i = 1; i < 5; i++ )
                    unitMeterOverlayTexture[i] = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\unit_meter_overlay_0" + i);

                unitMeterOverlaySource = new Rectangle(0, 10, 50, 100);
                unitMeterOverlayDestination = new Rectangle[5];
                //unitMeterHighlightTexture = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\unit_meter_animated");
                //unitMeterHightlightSource = new Rectangle(0, 0, 30, 0);
                unitMeterOverlayBaseY = new float[5];

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
                    statusHealthLocation[player] = globalLocations[player] + new Vector2(164, 38);
                    computerStatusHealthLocation[player] = globalLocations[player] + new Vector2(178, 62);
                    statusNumberOfUnitsLocation[player] = globalLocations[player] + new Vector2(164, 60);
                    computerStatusNumberOfUnitsLocation[player] = globalLocations[player] + new Vector2(178, 82);
                    statusHealthLocation[player] = globalLocations[player] + new Vector2(125, 36);
                    computerStatusHealthLocation[player] = globalLocations[player] + new Vector2(125, 36);
                    statusNumberOfUnitsLocation[player] = globalLocations[player] + new Vector2(125, 85);
                    computerStatusNumberOfUnitsLocation[player] = globalLocations[player] + new Vector2(125, 85);

                    iconLocations[player] = new Vector2[4];
                    highlightIconLocations[player] = new Rectangle[4];
                    Vector2 iconStartPosition = new Vector2(30, 36);
                    Vector2 iconrowOffset = new Vector2(36, 0);
                    for (int icon = 0; icon < 4; icon++)
                    {
                        iconLocations[player][icon] = globalLocations[player] + iconStartPosition;
                        highlightIconLocations[player][icon] = new Rectangle((int)(iconLocations[player][icon].X - 5.0f + (highlightIcon.Width / 2)),
                            (int)(iconLocations[player][icon].Y - 5.0f + (highlightIcon.Height / 2)),
                            highlightIcon.Width, highlightIcon.Height);
                        
                        if (icon % 2 == 1)
                        {
                            iconStartPosition += iconrowOffset;
                            iconStartPosition.Y = iconrowOffset.X;
                        }
                        else
                            iconStartPosition += iconOffset;
                    }

                    for (int tower = 0; tower < NUM_DEFENSE_TOWERS; tower++)
                    {
                        defenseTowerInfo[tower].nameLocation[player] = globalLocations[player] + new Vector2(120.0f, 30);
                        defenseTowerInfo[tower].priceLocation[player] = globalLocations[player] + new Vector2(120.0f, 64);
                        defenseTowerInfo[tower].descriptionLocation[player] = globalLocations[player] + new Vector2(120.0f, 94);
                    }

                    for (int tower = 0; tower < NUM_OFFENSE_TOWERS; tower++)
                    {
                        offenseTowerInfo[tower].nameLocation[player] = globalLocations[player] + new Vector2(120.0f, 30);
                        offenseTowerInfo[tower].priceLocation[player] = globalLocations[player] + new Vector2(120.0f, 64);
                        offenseTowerInfo[tower].descriptionLocation[player] = globalLocations[player] + new Vector2(120.0f, 94);
                    }


                    unitIconLocation[player] = new Vector2[MAX_UNIT_TYPES];
                    highlightUnitIconLocations[player] = new Rectangle[MAX_UNIT_TYPES];
                    Vector2 unitIconStartPosition = new Vector2(48, 36);
                    for (int unit = 0; unit < MAX_UNIT_TYPES; unit++)
                    {
                        if (unit == 4)
                        {
                            //move over invalid middle
                            unitIconStartPosition += unitIconOffset;
                        }
                        unitIconLocation[player][unit] = globalLocations[player] + unitIconStartPosition;
                        highlightUnitIconLocations[player][unit] = new Rectangle((int)(unitIconLocation[player][unit].X - 5.0f + (highlightIcon.Width / 2)),
                            (int)(unitIconLocation[player][unit].Y - 5.0f + (highlightIcon.Height / 2)),
                            highlightIcon.Width, highlightIcon.Height);
                        unitIconStartPosition += unitIconOffset;
                        if (unit == 2 || unit == 4)
                        {
                            unitIconStartPosition += unitIconRowOffset;
                            unitIconStartPosition.X = 48;
                        }
                    }

                    unitMeterOverlayDestination[player] = new Rectangle((int)globalLocations[player].X + 8,
                        (int)globalLocations[player].Y + 44 + 110,
                        50,
                        0);
                    unitMeterOverlayBaseY[player] = unitMeterOverlayDestination[player].Y;
                    

                    unitMeterBaseLocation[player] = new Rectangle((int)globalLocations[player].X + 8,
                        (int)globalLocations[player].Y + 44,
                        50,
                        120);

                    unitMeterOverlayMaxHeight[player] = unitMeterBaseLocation[player].Height - 20.0f;

                }


                highlightIconSourceRect = new Rectangle(0, 0, highlightIcon.Width, highlightIcon.Height);
                highlightOrigin = new Vector2(highlightIcon.Width / 2.0f, highlightIcon.Height / 2.0f);


                defensiveTab = new Texture2D[5][];
                offensiveTab = new Texture2D[5][];
                statusTab = new Texture2D[5][];
                powersTab = new Texture2D[5][];
                backgroundTabs = new Texture2D[5][];

                computerTags = new Texture2D[5];
                computerTagLocations = new Vector2[5];

                for (int player = 1; player < 5; player++)
                {
                    defensiveTab[player] = new Texture2D[5];
                    offensiveTab[player] = new Texture2D[5];
                    statusTab[player] = new Texture2D[5];
                    powersTab[player] = new Texture2D[5];
                    backgroundTabs[player] = new Texture2D[5];

                    computerTags[player] = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Tabs\\computerMode" + mapTeamNumToTeamChar[player]);
                    computerTagLocations[player] = globalLocations[player] + new Vector2(0, 105);

                    for (int team = 1; team < 5; team++)
                    {
                        defensiveTab[player][team] = 
                            ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Tabs\\player0" + player + "DefensiveUnits" + mapTeamNumToTeamChar[team]);
                            ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Tabs\\player0" + player + 
                            "DefensiveUnits" + mapTeamNumToTeamChar[team]);
                        offensiveTab[player][team] = 
                            ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Tabs\\player0" + player + "OffensiveUnits" + mapTeamNumToTeamChar[team]);
                            ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Tabs\\player0" + player + 
                            "OffensiveUnits" + mapTeamNumToTeamChar[team]);
                        statusTab[player][team] = 
                            ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Tabs\\player0" + player + "Status" + mapTeamNumToTeamChar[team]);
                            ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Tabs\\player0" + player + 
                            "Status" + mapTeamNumToTeamChar[team]);
                        powersTab[player][team] = 
                            ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Tabs\\player0" + player + "Power" + mapTeamNumToTeamChar[team]);
                            ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Tabs\\player0" + player + 
                            "Power" + mapTeamNumToTeamChar[team]);
                        backgroundTabs[player][team] =
                            ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Tabs\\player0" + player + "BACKGROUND_" + mapTeamNumToTeamChar[team]);
                            ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Tabs\\player0" + player + 
                            "BACKGROUND_" + mapTeamNumToTeamChar[team]);

                    }
                }

                healthicon = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Icons\\icon_health");
                numUnitsIcon = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Icons\\icon_num_units");
                healthIconLocations = new Vector2[5];
                numUnitsIconLocations = new Vector2[5];
                for (int i = 1; i < 5; i++)
                {
                    healthIconLocations[i] = globalLocations[i] + healthIconOffset;
                    numUnitsIconLocations[i] = globalLocations[i] + numUnitsIconOffset;
                }
                statusScreenFont = ScreenManager.Game.Content.Load<SpriteFont>("PlayerMenu\\statusScreenFont");

                healthicon = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Icons\\icon_health");
                numUnitsIcon = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Icons\\icon_num_units");
                healthIconLocations = new Vector2[5];
                numUnitsIconLocations = new Vector2[5];
                for (int i = 1; i < 5; i++)
                {
                    healthIconLocations[i] = globalLocations[i] + healthIconOffset;
                    numUnitsIconLocations[i] = globalLocations[i] + numUnitsIconOffset;
                }
                statusScreenFont = ScreenManager.Game.Content.Load<SpriteFont>("PlayerMenu\\statusScreenFont");

                attackingPlayer = new Texture2D[5];
                attackingPlayerLocation = new Vector2[5];
                for (int p = 1; p < 5; p++)
                {
                    attackingPlayer[p] = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Attacking\\" + p);
                    attackingPlayerLocation[p] = globalLocations[p] + new Vector2(260.0f, 126.0f);
                }

            }
        }

        public void TakeDamage()
        {
            Health--;
            HealthString = Health.ToString();
            if (Health <= 0)
            {
                //Event if died??
                
                IsDead = true;
                PlayerCollection.CheckGameWin();
                PlayerCollection.UpdateTargetPlayers(PlayerNum);
                UnitCollection.SetOtherUnitsToNewTarget(PlayerNum);
                UnitCollection.SetAllUnitsImmovable(PlayerNum);
                TowerCollection.SetAllNoShoot(PlayerNum);
            }
            
        }

        public void SetBase(Base playerBase)
        {
            this.PlayerBase = playerBase;

            if (Type == PlayerType.Human && avatar == null)
            {
                SetupAvatar();  
            }
            // TileMap2.SetBase(playerBase);
        }

        public void SetupAvatar()
        {
            //avatar = new AnimatedModel(PlayerNum, TeamNum,
            //        ScreenManager.Game.Content.Load<Model>("AnimatedModel\\dude"));

            //avatar.Scale = 2.0f;
            //avatar.PlayClip("Take 001");
            //avatar.Position = new Vector3(PlayerBase.Position.X, 200.0f, PlayerBase.Position.Z);
            //avatarMoved = false;
            //avatarFollowingTile = new StaticModel(ScreenManager.Game.Content.Load<Model>("Objects\\Copter\\squarePlacer_red"), avatar.Position);
            //avatarFollowingTile.Scale = 4.0f;

            avatar = new Avatar(
                ScreenManager.Game.Content.Load<Model>("Objects\\Copter\\player0" + PlayerNum + "Ship01"),
                new Vector3(PlayerBase.Position.X, 300.0f, PlayerBase.Position.Z));
            avatar.Scale = 6.0f;

            avatarMoved = false;
            avatarFollowingTile = new StaticModel(ScreenManager.Game.Content.Load<Model>("Objects\\Copter\\Boxes\\box_0" + PlayerNum), 
                    TileMap.GetTilePosFromPos(avatar.Position));

            avatarFollowingTile.glow = true;
            avatarFollowingTile.Scale = 4.0f;
            
        }

        public void HandleInput(InputManager input)
        {
            //Human Player
            if (Type == PlayerType.Human)
            {
                avatarMoved = avatar.HandleInput(input, playerIndexes[PlayerNum]);

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

                if (input.CheckNewAction(InputAction.TowerBuild, playerIndexes[PlayerNum]))
                {
                    
                    Tower tower = TowerCollection.Add(PlayerNum, TeamNum, Money, lastBuiltTower, this.avatarFollowingTile.Position);
                    if(tower != null)
                        Money -= tower.Cost;

                    MoneyString = Money.ToString();
                }

                if (input.CheckNewAction(InputAction.TowerDestroy, playerIndexes[PlayerNum]))
                {
                    Money += TowerCollection.Remove(PlayerNum, ref this.avatar.Position);
                    MoneyString = Money.ToString();
                }

                if (input.CheckNewAction(InputAction.TowerRepair, playerIndexes[PlayerNum]))
                {
                    Money -= TowerCollection.Repair(PlayerNum, Money, ref this.avatar.Position);
                    MoneyString = Money.ToString();
                }

                if (input.CheckNewAction(InputAction.TowerUpgrade, playerIndexes[PlayerNum]))
                {
                    Money -= TowerCollection.Upgrade(PlayerNum, Money, ref this.avatar.Position);
                    MoneyString = Money.ToString();
                }

                if (input.CheckNewAction(InputAction.PlayerMenuLeft, playerIndexes[PlayerNum]))
                {
                    if (((int)currentlySelectedPlayerStatus - 1) >= 0)
                        currentlySelectedPlayerStatus--;
                    unitScreenActivated = false;

                    if (currentlySelectedPlayerStatus == PlayerMenuTabs.DefenseTower)
                    {
                        //if (((int)defenseTowerSelected - 1) >= 0)
                        //    defenseTowerSelected--;
                        //else
                        //    defenseTowerSelected = NUM_DEFENSE_TOWERS - 1;
                        lastBuiltTower = defenseTowerInfo[defenseTowerSelected].type;
                    }
                    else if (currentlySelectedPlayerStatus == PlayerMenuTabs.UnitTower)
                    {
                        //if (((int)defenseTowerSelected - 1) >= 0)
                        //    defenseTowerSelected--;
                        //else
                        //    defenseTowerSelected = NUM_DEFENSE_TOWERS - 1;
                        lastBuiltTower = offenseTowerInfo[offenseTowerSelected].type;
                    }
                    else if (currentlySelectedPlayerStatus == PlayerMenuTabs.UnitTower)
                    {
                        //if (((int)defenseTowerSelected - 1) >= 0)
                        //    defenseTowerSelected--;
                        //else
                        //    defenseTowerSelected = NUM_DEFENSE_TOWERS - 1;
                        lastBuiltTower = offenseTowerInfo[offenseTowerSelected].type;
                    }
                }

                if (input.CheckNewAction(InputAction.PlayerMenuRight, playerIndexes[PlayerNum]))
                {
                    if (((int)currentlySelectedPlayerStatus + 1) < playerMenuTabsEnumType.Length)
                        currentlySelectedPlayerStatus++;
                    unitScreenActivated = false;

                    if (currentlySelectedPlayerStatus == PlayerMenuTabs.DefenseTower)
                    {
                        //if (((int)defenseTowerSelected - 1) >= 0)
                        //    defenseTowerSelected--;
                        //else
                        //    defenseTowerSelected = NUM_DEFENSE_TOWERS - 1;
                        lastBuiltTower = defenseTowerInfo[defenseTowerSelected].type;
                    }
                    else if (currentlySelectedPlayerStatus == PlayerMenuTabs.UnitTower)
                    {
                        //if (((int)defenseTowerSelected - 1) >= 0)
                        //    defenseTowerSelected--;
                        //else
                        //    defenseTowerSelected = NUM_DEFENSE_TOWERS - 1;
                        lastBuiltTower = offenseTowerInfo[offenseTowerSelected].type;
                    }
                    else if (currentlySelectedPlayerStatus == PlayerMenuTabs.UnitTower)
                    {
                        //if (((int)defenseTowerSelected - 1) >= 0)
                        //    defenseTowerSelected--;
                        //else
                        //    defenseTowerSelected = NUM_DEFENSE_TOWERS - 1;
                        lastBuiltTower = offenseTowerInfo[offenseTowerSelected].type;
                    }
                }

                if (input.CheckNewAction(InputAction.PlayerMenuUp, playerIndexes[PlayerNum]))
                {
                    if (currentlySelectedPlayerStatus == PlayerMenuTabs.DefenseTower)
                    {
                        if (((int)defenseTowerSelected - 1) >= 0)
                            defenseTowerSelected--;
                        else
                            defenseTowerSelected = NUM_DEFENSE_TOWERS - 1;
                        lastBuiltTower = defenseTowerInfo[defenseTowerSelected].type;
                    }
                    else if (currentlySelectedPlayerStatus == PlayerMenuTabs.UnitTower)
                    {
                        if (((int)offenseTowerSelected - 1) >= 0)
                            offenseTowerSelected--;
                        else
                            offenseTowerSelected = NUM_OFFENSE_TOWERS - 1;
                        lastBuiltTower = offenseTowerInfo[offenseTowerSelected].type;
                    }

                    else if (currentlySelectedPlayerStatus == PlayerMenuTabs.UnitTower)
                    {
                        if (((int)offenseTowerSelected - 1) >= 0)
                            offenseTowerSelected--;
                        else
                            offenseTowerSelected = NUM_OFFENSE_TOWERS - 1;
                        lastBuiltTower = offenseTowerInfo[offenseTowerSelected].type;
                    }


                    unitScreenActivated = false;
                }

                if (input.CheckNewAction(InputAction.PlayerMenuDown, playerIndexes[PlayerNum]))
                {
                    if (currentlySelectedPlayerStatus == PlayerMenuTabs.DefenseTower)
                    {
                        if (((int)defenseTowerSelected + 1) < NUM_DEFENSE_TOWERS)
                            defenseTowerSelected++;
                        else
                            defenseTowerSelected = 0;
                        lastBuiltTower = defenseTowerInfo[defenseTowerSelected].type;
                    }
                    else if (currentlySelectedPlayerStatus == PlayerMenuTabs.UnitTower)
                    {
                        if (((int)offenseTowerSelected + 1) < NUM_OFFENSE_TOWERS)
                            offenseTowerSelected++;
                        else
                            offenseTowerSelected =0;
                        lastBuiltTower = offenseTowerInfo[offenseTowerSelected].type;
                    }

                    else if (currentlySelectedPlayerStatus == PlayerMenuTabs.UnitTower)
                    {
                        if (((int)offenseTowerSelected + 1) < NUM_OFFENSE_TOWERS)
                            offenseTowerSelected++;
                        else
                            offenseTowerSelected =0;
                        lastBuiltTower = offenseTowerInfo[offenseTowerSelected].type;
                    }

                    unitScreenActivated = false;
                }

                if (input.CheckAction(InputAction.UnitUp, playerIndexes[PlayerNum]))
                {
                    if (unitSelectionPosition.Y < 1)
                        unitSelectionPosition.Y += 1;

                    unitScreenActivated = true;
                }

                if (input.CheckAction(InputAction.UnitLeft, playerIndexes[PlayerNum]))
                {
                    if (unitSelectionPosition.X > -1)
                        unitSelectionPosition.X -= 1;
                    unitScreenActivated = true;
                       
                }

                if (input.CheckAction(InputAction.UnitDown, playerIndexes[PlayerNum]))
                {
                    if (unitSelectionPosition.Y > -1)
                        unitSelectionPosition.Y -= 1;
                    unitScreenActivated = true;
                }

                if (input.CheckAction(InputAction.UnitRight, playerIndexes[PlayerNum]))
                {
                    if (unitSelectionPosition.X < 1)
                        unitSelectionPosition.X += 1;
                    unitScreenActivated = true;
                }


                // 0 | 1 | 2
                // 3 | - | 4
                // 5 | 6 | 7
                //case 1
                if (unitSelectionPosition.X == 0 && unitSelectionPosition.Y == 1)
                {
                    selectedUnit = 1;
                }
                else if (unitSelectionPosition.X == 0 && unitSelectionPosition.Y == -1)
                {
                    selectedUnit = 6;
                }


                if (input.CheckAction(InputAction.UnitBuild, playerIndexes[PlayerNum]))
                {

                    if (selectedUnit != queuedUnitType)
                    {
                        queuedUnits = 0;
                        queuedDeclineMode = false;
                        unitsDeployed = 0;
                        queuedUnitsToDeploy = 0;
                        percentOfUnitsQueued = 0;
                    }

                    if (queuedDeclineMode && input.CheckNewAction(InputAction.UnitBuild, playerIndexes[PlayerNum]))
                    {
                        queuedUnits = 0;
                        queuedDeclineMode = false;
                        unitsDeployed = 0;
                        queuedUnitsToDeploy = 0;
                        percentOfUnitsQueued = 0;
                    }

                    if (!queuedDeclineMode || queuedUnitsToDeploy == 0)
                    {
                        int amountLeft =  UnitCollection.MaxUnitsToDeployFor(PlayerNum, unitInformation[selectedUnit].type);
                        if (queuedUnits + queuedUnitAmount <= amountLeft)
                        {
                            queuedUnits += 100;// queuedUnitAmount;

                            if (queuedUnitsToDeploy != 0 && queuedDeclineMode)
                            {
                                percentOfUnitsQueued = (float)queuedUnits / (float)queuedUnitsToDeploy;
                            }
                            queuedDeclineMode = false;

                        }

                        if (!queuedDeclineMode)
                        {
                            percentOfUnitsQueued += 1.4f;
                        }
                        queuedUnitType = selectedUnit;
                    }
                }

                if (input.CheckNewReleaseAction(InputAction.UnitBuild, playerIndexes[PlayerNum]) || (percentOfUnitsQueued >= 100.0f))
                {
                    if (!queuedDeclineMode)
                    { 
                        queuedDeclineMode = true;
                        int totalUnits = UnitCollection.MaxUnitsToDeployFor(PlayerNum, unitInformation[selectedUnit].type) + unitsDeployed;
                        queuedUnits = (int)(totalUnits * (percentOfUnitsQueued / 100.0f)) - unitsDeployed;
                        queuedUnitsToDeploy = totalUnits - unitsDeployed;
                        percentOfUnitsQueued = 0;
                    }
                }

                if (input.CheckNewAction(InputAction.HUD, playerIndexes[PlayerNum]))
                {
                    isHUDDisplayed = true;
                }
                
                if(input.CheckNewReleaseAction(InputAction.HUD, playerIndexes[PlayerNum]))
                {
                    isHUDDisplayed = false;
                }

                if (input.CheckNewAction(InputAction.PickEnemyTarget, playerIndexes[PlayerNum]))
                {
                    if (pickEnemyTargetPressed)
                    {
                        this.TargetPlayerNum = PlayerCollection.GetNextTargetFor(this.PlayerNum);
                        PlayerCollection.SetTargetFor(PlayerNum, TargetPlayerNum);
                    }

                    //get opacity to display draw
                    enemyTargetOpacity.A = 255;
                    pickEnemyTargetPressed = true;
                }


            }
        }

        public void Update(GameTime gameTime)
        {
            PlayerBase.Update(gameTime);

            if (IsDead)
            {
                if (UnitCollection.Destroy(PlayerNum) && TowerCollection.Destroy(PlayerNum))
                {
                    PlayerBase.Destroy();
                    Vector3 nv = new Vector3();
                    if (this.Type != PlayerType.AI)
                    {
                        nv.X = avatar.Position.X;
                        nv.Y = avatar.Position.Y + 5;
                        nv.Z = avatar.Position.Z;
                        ProjectileManager.AddParticle(avatar.Position, nv);
                    }
                    PlayerCollection.SetPlayerInactive(PlayerNum);
                }
                return;
            }

            //HumanPlayer
            if (Type == PlayerType.Human)
            {
                avatar.Update(gameTime);
                if (avatarMoved)
                {
                    avatarMoved = false;
                    avatarFollowingTile.Position = TileMap.GetTilePosFromPos(avatar.Position);
                }

                avatarFollowingTile.Update(gameTime);
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

            if (queuedUnits > 0)
            {
                elapsedUnitDeployTime += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedUnitDeployTime >= maxUnitDeployTime)
                {
                    elapsedUnitDeployTime = 0;

                    UnitCollection.Deploy(PlayerNum, TeamNum, TargetPlayerNum, unitInformation[queuedUnitType].type);
                    queuedUnits--;
                    unitsDeployed++;
                }

                //elapsedUnitMeterUpdateTime += gameTime.ElapsedGameTime.Milliseconds;
                //if (elapsedUnitMeterUpdateTime >= maxUnitMeterUpdateDate)
                //{
                //    elapsedUnitMeterUpdateTime = 0;
                //    unitMeterHightlightSource.Y += 2;
                //    if (unitMeterHightlightSource.Y >= unitMeterHighlightTexture.Height)
                //        unitMeterHightlightSource.Y = 0;
                //}
            }
            else
            {
                queuedUnitsToDeploy = 0;
            }

            if (pickEnemyTargetPressed)
            {
                elapsedPlayerToAttackChange += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedPlayerToAttackChange >= maxPlayerToAttackChange)
                {
                    pickEnemyTargetPressed = false;
                    elapsedPlayerToAttackChange = 0;
                }
            }

            if (enemyTargetOpacity.A > 0)
            {
                if (enemyTargetOpacity.A <= 4)
                    enemyTargetOpacity.A = 0;
                else
                    enemyTargetOpacity.A -= 4;
                
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
            this.TargetPlayerNum = target.PlayerNum;
           // target.baseDestroyed += GetNewTargetBase;
        }

        public void SetTowerForLevelMap(TowerType towerType, Tile tile)
        {
            TileMap.SetTowerForLevelMap(TowerCollection.Add(PlayerNum, TeamNum, 100000, towerType, tile.Position), tile);
        }

        public void DrawMenu(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            if (!unitScreenActivated)
            {
                switch (currentlySelectedPlayerStatus)
                {
                    case PlayerMenuTabs.Status:
                        DrawStatus();
                        break;
                    case PlayerMenuTabs.DefenseTower:
                        DrawDefenseTowers();
                        break;
                    case PlayerMenuTabs.UnitTower:
                        DrawOffenseTowers();
                        break;
                    case PlayerMenuTabs.Powers:
                        DrawPowers();
                        break;
                }
            }
            else
            {
                DrawDeployTab();
            }


            if (Type == PlayerType.Human)
            {
                ScreenManager.SpriteBatch.DrawString(statusFont, MoneyString, moneyLocation[PlayerNum], Color.White);
                if(enemyTargetOpacity.A > 0)
                {
                    ScreenManager.SpriteBatch.Draw(attackingPlayer[TargetPlayerNum], 
                        attackingPlayerLocation[PlayerNum], enemyTargetOpacity);
                }
            }

            ScreenManager.SpriteBatch.End();

        }

        void DrawStatus()
        {
            if (Type == PlayerType.Human)
            {
                ScreenManager.SpriteBatch.Draw(statusTab[PlayerNum][TeamNum], globalLocations[PlayerNum], Color.White);
                ScreenManager.SpriteBatch.Draw(healthicon, healthIconLocations[PlayerNum], Color.White);
                ScreenManager.SpriteBatch.Draw(numUnitsIcon, numUnitsIconLocations[PlayerNum], Color.White);
                ScreenManager.SpriteBatch.DrawString(statusScreenFont, HealthString, statusHealthLocation[PlayerNum], Color.White);
                ScreenManager.SpriteBatch.DrawString(statusScreenFont, UnitCollection.UnitCountForPlayerString(PlayerNum),
                    statusNumberOfUnitsLocation[PlayerNum], Color.White);
            }
            else
            {
                ScreenManager.SpriteBatch.Draw(computerTags[TeamNum], globalLocations[PlayerNum], Color.White);
                ScreenManager.SpriteBatch.Draw(healthicon, healthIconLocations[PlayerNum], Color.White);
                ScreenManager.SpriteBatch.Draw(numUnitsIcon, numUnitsIconLocations[PlayerNum], Color.White);
                ScreenManager.SpriteBatch.DrawString(statusScreenFont, HealthString,
                    statusHealthLocation[PlayerNum], Color.White);
                ScreenManager.SpriteBatch.DrawString(statusScreenFont, UnitCollection.UnitCountForPlayerString(PlayerNum),

                    statusNumberOfUnitsLocation[PlayerNum], Color.White);
            }
        }

        void DrawDefenseTowers()
        {
            ScreenManager.SpriteBatch.Draw(defensiveTab[PlayerNum][TeamNum], globalLocations[PlayerNum], Color.White);
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

        void DrawOffenseTowers()
        {
            ScreenManager.SpriteBatch.Draw(offensiveTab[PlayerNum][TeamNum], globalLocations[PlayerNum], Color.White);

            for (int i = 0; i < NUM_OFFENSE_TOWERS; i++)
            {
                ScreenManager.SpriteBatch.Draw(offenseIcons[i], iconLocations[PlayerNum][i], Color.White);
                if (i == offenseTowerSelected)
                {
                    ScreenManager.SpriteBatch.Draw(highlightIcon,
                        highlightIconLocations[PlayerNum][i],
                        highlightIconSourceRect,
                        Color.White,
                        highlightRotations[currentHighlightRotation],
                        highlightOrigin,
                        SpriteEffects.None,
                        1.0f);
                    ScreenManager.SpriteBatch.DrawString(towerTitle, offenseTowerInfo[i].name,
                        offenseTowerInfo[i].nameLocation[PlayerNum], Color.White);
                    ScreenManager.SpriteBatch.DrawString(towerPrice, offenseTowerInfo[i].price,
                        offenseTowerInfo[i].priceLocation[PlayerNum], Color.White);
                    ScreenManager.SpriteBatch.DrawString(towerDescription, offenseTowerInfo[i].description,
                        offenseTowerInfo[i].descriptionLocation[PlayerNum], Color.White);
                }

            }
        }

        void DrawPowers()
        {
            ScreenManager.SpriteBatch.Draw(powersTab[PlayerNum][TeamNum], globalLocations[PlayerNum], Color.White);
        }

        void DrawDeployTab()
        {
            ScreenManager.SpriteBatch.Draw(backgroundTabs[PlayerNum][TeamNum], globalLocations[PlayerNum], Color.White);
            for (int i = 0; i < MAX_UNIT_TYPES; i++)
            {
                if (unitInformation[i].icon != null)
                {
                    ScreenManager.SpriteBatch.Draw(unitInformation[i].icon,
                        unitIconLocation[PlayerNum][i], Color.White);

                    if (selectedUnit == i)
                    {
                        ScreenManager.SpriteBatch.Draw(highlightIcon,
                            highlightUnitIconLocations[PlayerNum][i],
                            highlightIconSourceRect,
                            Color.White,
                            highlightRotations[currentHighlightRotation],
                            highlightOrigin,
                            SpriteEffects.None,
                            1.0f);
                    }
                }
            }

            ScreenManager.SpriteBatch.Draw(unitMeterBaseTexture, unitMeterBaseLocation[PlayerNum], Color.White);
            if (queuedUnits > 0)
            {
                if (queuedDeclineMode)
                {
                    unitMeterOverlayDestination[PlayerNum].Height = (int)(
                        ((float) queuedUnits / (float)queuedUnitsToDeploy) * 
                        unitMeterOverlayMaxHeight[PlayerNum]);
                    //((float)queuedUnits / (float)UnitCollection.MaxUnitsToDeployFor(PlayerNum, unitInformation[queuedUnitType].type)) *
                    //unitMeterBaseLocation[PlayerNum].Height);

                   unitMeterOverlayDestination[PlayerNum].Y = (int)(unitMeterOverlayBaseY[PlayerNum] - unitMeterOverlayDestination[PlayerNum].Height);
                }
                else
                {
                    unitMeterOverlayDestination[PlayerNum].Height = (int)((percentOfUnitsQueued / 100.0f) * 
                        unitMeterOverlayMaxHeight[PlayerNum]);
                    // unitMeterBaseLocation[PlayerNum].Height);
                    unitMeterOverlayDestination[PlayerNum].Y = (int)(unitMeterOverlayBaseY[PlayerNum] - unitMeterOverlayDestination[PlayerNum].Height);
                }

                //unitMeterHightlightSource.Height = unitMeterOverlayDestination[PlayerNum].Height;

                ScreenManager.SpriteBatch.Draw(unitMeterOverlayTexture[TeamNum], unitMeterOverlayDestination[PlayerNum],
                    unitMeterOverlaySource, Color.White);
                //ScreenManager.SpriteBatch.Draw(unitMeterHighlightTexture, unitMeterOverlayDestination[PlayerNum],
                //    unitMeterHightlightSource, new Color(255, 255, 255, 50));

            }

        }
    }
}
