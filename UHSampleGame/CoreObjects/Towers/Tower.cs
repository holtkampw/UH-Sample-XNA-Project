using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.TileSystem;
using UHSampleGame.Events;
using UHSampleGame.CoreObjects.Units;
using UHSampleGame.CameraManagement;
using UHSampleGame.ScreenManagement;

using UHSampleGame.ProjectileManagment;
using UHSampleGame.Players;


namespace UHSampleGame.CoreObjects.Towers
{
    public enum TowerType { Plasma, Cannon, Electric, SmallUnit, LargeUnit }
    public enum TowerStatus { Inactive, Active, ActiveNoShoot }
    public enum BaseTowerType { Offense, Defense }
    public enum TowerUpgradeType { Speed, Attack }

    public struct TowerUpgrades 
    {
        public TowerUpgradeType type;
        public float amount;
        public int price; //NOT USED FOR FUTURE USE
        public string priceString; //NOT USED FOR FUTURE USE
    }

    struct UnitInformation
    {
        public UnitType type;
        public Texture2D icon;
    }


    public class Tower
    {

        #region Class Variables
        public static Enum[] towerEnumType = EnumHelper.EnumToArray(new TowerType());

        public Matrix Transforms;
        public float Scale;
        public Vector3 Position;
        Matrix scaleMatrix;
        Matrix rotationMatrixX;
        Matrix rotationMatrixY;
        Matrix rotationMatrixZ;
        Matrix scaleRot;

        int attackStrength = 20;

        public UnitType UnitTypeToBuild = UnitType.SpeedBoat;
        public TowerType Type;
        public TowerStatus Status;
        public int TeamNum;
        public int PlayerNum;
        public Unit unitToAttack;
        public List<Tower> towersToAttack;
        public Tile tile;

        int currentXPToGive;

        public event TowerEvent Died;

        private TimeSpan timeToAttack;
        private TimeSpan currentTimeToAttack;

        public int ID;

        public int Health;
        public int HealthCapacity;
        public int XP;
        public int Level = 1;
        public string LevelString = "1";
        public int Cost;
        public int TotalInvestedCost;

        int unitCreationAmount;

        TimeSpan unitBuild = new TimeSpan(0, 0, 1);
        TimeSpan currentTimeSpan = new TimeSpan();
        static int moneyToGive = 0;

        static int currentID = 0;

        static Vector2[] upgradeIconOffset;
        static Vector2[] repairIconOffset;
        static Vector2[] recycleIconOffset;
        static Vector2[] upgradeStringOffset;
        static Vector2[] repairStringOffset;
        static Vector2[] recycleStringOffset;
        static SpriteFont font;
        static Texture2D recycleIcon;
        static Texture2D repairIcon;
        TowerUpgrades[] UpgradeAmounts;
        static Texture2D[] upgradeIcon;
        public static Enum[] towerUpgradeEnumType = EnumHelper.EnumToArray(new TowerUpgradeType());

        BaseTowerType BaseType;
        public int destroyCost;
        public string destroyCostString;
        public int upgradeCost;
        public string upgradeCostString;
        public int repairCost;
        public string repairCostString;


        int unitTypeSelected = 0;
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
        static Rectangle highlightIconSourceRect;

        static Vector2[][] unitIconLocation;
        static Rectangle[][] highlightUnitIconLocations;
        const int MAX_UNIT_TYPES = 4;
        static Vector2 unitIconOffset = new Vector2(44.0f, 0.0f);
        static Vector2 unitIconRowOffset = new Vector2(0.0f, 44.0f);
        static Texture2D highlightIcon;
        static UnitInformation[] unitInformation;
        static Vector2[] globalLocations = { Vector2.Zero, new Vector2(40f, 30f), new Vector2(40f, 200f), new Vector2(40f, 370f), new Vector2(40f, 540f) };

        Vector2 unitSelectionPosition = Vector2.Zero;
        #endregion

        #region Initialization
        
        public Tower(TowerType type)
        {
            this.rotationMatrixX = Matrix.Identity;
            this.rotationMatrixY = Matrix.Identity;
            this.rotationMatrixZ = Matrix.Identity;
            this.scaleRot = Matrix.Identity;
            this.scaleMatrix = Matrix.Identity;
            this.Type = type;
            this.Status = TowerStatus.Inactive;
            unitToAttack = null;
            towersToAttack = new List<Tower>();

            timeToAttack = new TimeSpan(0, 0, 1);
            currentTimeToAttack = new TimeSpan();

            //Depends on Type
            HealthCapacity = 100;
            Health = HealthCapacity;
            XP = 0;
            Level = 0;
            Level = 1;
            Cost = 100;
            TotalInvestedCost = Cost;

            ID = currentID;
            currentID++;

           
        }

        public void Setup(Vector3 position)
        {
            this.Position = position;
            switch (Type)
            {
                case TowerType.Plasma:
                    this.Scale = 4.0f;
                    this.BaseType = BaseTowerType.Defense;
                    break;
                case TowerType.Electric:
                    this.Scale = 3.0f;
                    this.BaseType = BaseTowerType.Defense;
                    break;
                case TowerType.Cannon:
                    this.Scale = 2.0f;
                    this.BaseType = BaseTowerType.Defense;
                    break;
                case TowerType.SmallUnit:
                    this.Scale = 2.0f;
                    this.BaseType = BaseTowerType.Offense;
                    break;
                case TowerType.LargeUnit:
                    this.Scale = 2.0f;
                    this.BaseType = BaseTowerType.Offense;
                    break;
            }
            UpdateScaleRotations();
            UpdateTransforms();
            this.Status = TowerStatus.Active;

            if (font == null)
            {
                font = ScreenManager.Game.Content.Load<SpriteFont>("PlayerMenu\\towerInfoFont");
                recycleIcon = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\TowerInfo\\recycle");
                repairIcon = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\TowerInfo\\repair");

                upgradeIcon = new Texture2D[towerUpgradeEnumType.Length];
                for (int i = 0; i < towerUpgradeEnumType.Length; i++)
                {
                    switch ((TowerUpgradeType)i)
                    {
                        case TowerUpgradeType.Attack:
                            upgradeIcon[i] = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\TowerInfo\\attack");
                            break;
                        case TowerUpgradeType.Speed:
                            upgradeIcon[i] = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\TowerInfo\\speed");
                            break;
                    }
                }

                unitIconLocation = new Vector2[5][];
                highlightUnitIconLocations = new Rectangle[5][];
                highlightIcon = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Icons\\icon_selector");
                highlightIconSourceRect = new Rectangle(0, 0, highlightIcon.Width, highlightIcon.Height);
                highlightOrigin = new Vector2(highlightIcon.Width / 2.0f, highlightIcon.Height / 2.0f);
                upgradeIconOffset = new Vector2[5];
                upgradeStringOffset = new Vector2[5];
                repairIconOffset = new Vector2[5];
                repairStringOffset = new Vector2[5];
                recycleIconOffset = new Vector2[5];
                recycleStringOffset = new Vector2[5];

                for (int player = 1; player < 4; player++)
                {
                    unitIconLocation[player] = new Vector2[MAX_UNIT_TYPES];
                    highlightUnitIconLocations[player] = new Rectangle[MAX_UNIT_TYPES];
                    Vector2 unitIconStartPosition = new Vector2(180, 60);
                    for (int unit = 0; unit < MAX_UNIT_TYPES; unit++)
                    {
                        unitIconLocation[player][unit] = globalLocations[player] + unitIconStartPosition;
                        highlightUnitIconLocations[player][unit] = new Rectangle((int)
                            (unitIconLocation[player][unit].X - 5.0f + (highlightIcon.Width / 2)),
                            (int)(unitIconLocation[player][unit].Y - 5.0f + (highlightIcon.Height / 2)),
                            highlightIcon.Width, highlightIcon.Height);
                        unitIconStartPosition += unitIconOffset;
                        if (unit == 1)
                        {
                            unitIconStartPosition += unitIconRowOffset;
                            unitIconStartPosition.X = 180;
                        }
                    }

                   upgradeIconOffset[player] = globalLocations[player] + new Vector2(10, 30);
                   upgradeStringOffset[player] = globalLocations[player] + new Vector2(54, 35);
                   repairIconOffset[player] = globalLocations[player] + new Vector2(10, 75);
                   repairStringOffset[player] = globalLocations[player] + new Vector2(54, 80);
                   recycleIconOffset[player] = globalLocations[player] + new Vector2(10, 120);
                   recycleStringOffset[player] = globalLocations[player] + new Vector2(54, 125);
                }

                unitInformation = new UnitInformation[MAX_UNIT_TYPES];

                unitInformation[0].type = UnitType.SpeedBoat;
                unitInformation[0].icon = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Icons\\speedBoat");

                unitInformation[2].type = UnitType.SpeederBoat;
                unitInformation[2].icon = ScreenManager.Game.Content.Load<Texture2D>("PlayerMenu\\Icons\\speederBoat");

            }

            // TowerUpgrades[nextLevel]
            UpgradeAmounts = new TowerUpgrades[5];
            UpgradeAmounts[2].amount = 0.4f;
            UpgradeAmounts[2].type = TowerUpgradeType.Speed;
            UpgradeAmounts[2].price = 1000;
            UpgradeAmounts[2].priceString = UpgradeAmounts[2].price.ToString();
            UpgradeAmounts[3].amount = 0.3f;
            UpgradeAmounts[3].type = TowerUpgradeType.Attack;
            UpgradeAmounts[3].price = 3000;
            UpgradeAmounts[3].priceString = UpgradeAmounts[3].price.ToString();
            UpgradeAmounts[4].amount = 0.6f;
            UpgradeAmounts[4].type = TowerUpgradeType.Attack;
            UpgradeAmounts[4].price = 6000;
            UpgradeAmounts[4].priceString = UpgradeAmounts[4].price.ToString();

            unitTypeSelected = 0;
            unitSelectionPosition.X = 0;
            unitSelectionPosition.Y = 0;

            DestroyCost();
            UpgradeCost();
            RepairCost();

        }

        public void Activate(int playerNum, int teamNum)
        {
            Status = TowerStatus.Active;
            PlayerNum = playerNum;
            TeamNum = teamNum;
            HealthCapacity = 100;
            Health = HealthCapacity;
            XP = 0;
            Level = 1;
            Cost = 100;
            TotalInvestedCost = Cost;
            towersToAttack.Clear();
        }

        public bool IsActive()
        {
            return Status != TowerStatus.Inactive;
        }

        public void RegisterAttackTower(ref Tower tower)
        {
            if (tower.TeamNum == this.TeamNum)
                return;

            if (towersToAttack.Count > 0 && towersToAttack[0].Health < tower.Health)
            {
                towersToAttack.Insert(0, tower);
            }
            else if (!towersToAttack.Contains(tower))
            {
                towersToAttack.Add(tower);
            }

        }

        public void UnregisterAttackTower(ref Tower tower)
        {
            towersToAttack.Remove(tower);
        }

        public void UnregisterAttackUnit(ref Unit unit)
        {
            if (unitToAttack != null && unitToAttack.ID == unit.ID)
            {
                unitToAttack = null;
            }
        }

        public void RegisterAttackUnit(ref Unit unit)
        {
            if ((unitToAttack == null && unit == null) ||
                (unitToAttack != null && unitToAttack.IsDeployed() && unit == null))
                return;

            if (unitToAttack != null && unitToAttack.Health <= 0)
                unitToAttack = null;

            if (unitToAttack != null && !unitToAttack.IsDeployed())
            {
                unitToAttack = unit;
                currentXPToGive = unitToAttack.XPToGive;
                return;
            }

            if (unit.TeamNum != TeamNum)
            {
                if (unitToAttack == null || unit.PathLength < unitToAttack.PathLength)
                {
                    unitToAttack = unit;
                    currentXPToGive = unitToAttack.XPToGive;
                    //unitToAttack.Died += GetNewAttackUnit;
                }
            }
        }

        private void Attack(GameTime gameTime)
        {

            if (currentTimeToAttack > timeToAttack)
            {

                if (unitToAttack != null && unitToAttack.Health > 0)
                {
                    currentTimeToAttack = TimeSpan.Zero;
                    ProjectileManager.AddParticle(this.Position, unitToAttack.Position);
                    moneyToGive = unitToAttack.MoneyToGive;
                    bool kill = unitToAttack.TakeDamage(attackStrength);

                    if (kill)
                    {
                        PlayerCollection.EarnedMoneyForPlayer(PlayerNum, moneyToGive);
                    }
                    if (Level < 4 && kill)
                    {
                        XP += currentXPToGive + (int)((Level / 4.0f) * currentXPToGive);
                        if (XP >= 100 )
                        {
                            XPUpgrade();
                        }
                    }

                }
                else if (towersToAttack.Count > 0)
                {
                    currentTimeToAttack = TimeSpan.Zero;
                    ProjectileManager.AddParticle(this.Position, towersToAttack[0].Position);
                    bool kill = towersToAttack[0].TakeDamage(attackStrength);
                    //if (kill)
                       // towersToAttack.Remove(towersToAttack[0]);
                }

                if (towersToAttack.Count > 0 && towersToAttack[0].Health <= 0)
                {
                    towersToAttack.RemoveAt(0);
                }

                if (unitToAttack != null && unitToAttack.Health <= 0)
                {
                    unitToAttack = null;
                }
                return;
            }
            currentTimeToAttack += gameTime.ElapsedGameTime;
        }

        public int Repair(int money)
        {
            int costToRepair = RepairCost();
            
            if (money >= costToRepair)
            {
                Health = HealthCapacity;
                RepairCost();
                return costToRepair;
            }

            return 0;
        }

        public int RepairCost()
        {
            float perc = 1.0f - (Health / (float)HealthCapacity);
            int amount = (int)(perc * Cost);
            repairCost = amount;
            repairCostString = "$ " + repairCost.ToString();
            return amount;
        }

        public int Upgrade(int money)
        {
            if (CanUpgrade())
            {
                int cost = upgradeCost;
                if (money >= upgradeCost)
                {
                    TotalInvestedCost += upgradeCost;
                    Level++;
                    XP = 0;
                    UpgradeCost();
                    return cost;
                }
            }

            return 0;
        }

        public int UpgradeCost()
        {
            upgradeCost = Cost + (int)((Level / 4.0f) * Cost);
            upgradeCostString = "$ " + upgradeCost.ToString();
            return upgradeCost;
        }

        public void XPUpgrade()
        {
            Level++;
            XP = 0;
        }

        public bool CanUpgrade()
        {
            if (Level + 1 <= 4)
            {
                return true;
            }
            return false;
        }

        public int DestroyCost()
        {
            float perc = (Health / (float)HealthCapacity);
            destroyCost = (int)(TotalInvestedCost * perc * .75f);
            destroyCostString = "$ +" + destroyCost.ToString();
            return destroyCost;
        }

        private void BuildUnit(GameTime gameTime)
        {
            currentTimeSpan = currentTimeSpan.Add(gameTime.ElapsedGameTime);
            if (currentTimeSpan > unitBuild)
            {
                if (Type == TowerType.SmallUnit)
                    unitCreationAmount = 2;
                else
                    unitCreationAmount = 4;
                for (int i = 0; i < unitCreationAmount; i++)
                    UnitCollection.Build(PlayerNum, TeamNum, UnitTypeToBuild);

                currentTimeSpan = TimeSpan.Zero;
            }
        }

        public bool TakeDamage(int damage)
        {
            Health -= damage;

            if (Health <= 0)
            {
                OnDied();
                return true;
            }
            else
            {
                RepairCost();
                DestroyCost();
            }
            return false;
        }

        public void OnDied()
        {
            Tower t = this;
            t.towersToAttack.Clear();
            this.Status = TowerStatus.Inactive;
            tile.RemoveBlockableObject();
            for (int i = 0; i < tile.tileNeighbors.Count; i++)
            {
                tile.tileNeighbors[i].UnregisterTowerListenerForTower(ref t);
                tile.tileNeighbors[i].UnregisterTowerListenerForUnit(ref t);
            }
        }
        #endregion

        #region Matrix Setters
        public void SetScale(float newScale)
        {
            this.Scale = newScale;
            scaleMatrix = Matrix.CreateScale(this.Scale);
            UpdateScaleRotations();
        }

        public void RotateX(float rotation)
        {
            rotationMatrixX = Matrix.CreateRotationX(rotation);
            UpdateScaleRotations();
        }

        public void RotateY(float rotation)
        {
            rotationMatrixY = Matrix.CreateRotationY(rotation);
            UpdateScaleRotations();
        }

        public void RotateZ(float rotation)
        {
            rotationMatrixZ = Matrix.CreateRotationZ(rotation);
            UpdateScaleRotations();
        }

        public void UpdateTransforms()
        {
            Matrix translation = Matrix.CreateTranslation(Position);
            Transforms = Matrix.Multiply(scaleRot, translation);
        }

        void UpdateScaleRotations()
        {
            scaleMatrix = Matrix.CreateScale(this.Scale);
            scaleRot = Matrix.Multiply(scaleMatrix,
                    Matrix.Multiply(rotationMatrixX, Matrix.Multiply(rotationMatrixY, rotationMatrixZ)));
        }
        #endregion

        #region Update/Draw
        public void Update(GameTime gameTime)
        {
            if (Type != TowerType.SmallUnit && Type != TowerType.LargeUnit)
                Attack(gameTime);
            else
            {
                BuildUnit(gameTime);
            }
        }

        public void InputLeft()
        {
            if (unitTypeSelected == 1)
            {
                if (unitInformation[0].icon != null)
                    unitTypeSelected = 0;
            }
            else if (unitTypeSelected == 3)
            {
                if (unitInformation[2].icon != null)
                    unitTypeSelected = 2;
            }
        }

        public void InputRight()
        {
            if (unitTypeSelected == 0)
            {
                if (unitInformation[1].icon != null)
                    unitTypeSelected = 1;
            }
            else if (unitTypeSelected == 2)
            {
                if (unitInformation[3].icon != null)
                    unitTypeSelected = 3;
            }
        }

        public void InputUp()
        {
            if (unitTypeSelected == 2)
            {
                if (unitInformation[0].icon != null)
                    unitTypeSelected = 0;
            }
            else if (unitTypeSelected == 3)
            {
                if (unitInformation[1].icon != null)
                    unitTypeSelected = 1;
            }
        }

        public void InputDown()
        {
            if (unitTypeSelected == 0)
            {
                if (unitInformation[2].icon != null)
                    unitTypeSelected = 2;
            }
            else if (unitTypeSelected == 1)
            {
                if (unitInformation[3].icon != null)
                    unitTypeSelected = 3;
            }
        }

        public void ValidateUnitInput()
        {
            // 0 | 1 | 2
            // 3 | - | 4
            // 5 | 6 | 7
            //case 1
            if (unitSelectionPosition.X == 0 && unitSelectionPosition.Y == 0)
            {
                unitTypeSelected = 0;         
            }
            else if (unitSelectionPosition.X == 0 && unitSelectionPosition.Y == 1)
            {
                unitTypeSelected = 2;
            }
            UnitTypeToBuild = unitInformation[unitTypeSelected].type;
        }


        public void DrawHud(GameTime gameTime)
        {  
            if (Level < 4)
            {
                ScreenManager.SpriteBatch.Draw(upgradeIcon[(int)UpgradeAmounts[Level + 1].type],
                    upgradeIconOffset[PlayerNum], Color.White);
                ScreenManager.SpriteBatch.DrawString(font, upgradeCostString,
                    upgradeStringOffset[PlayerNum], Color.White);
            }

            ScreenManager.SpriteBatch.Draw(recycleIcon, recycleIconOffset[PlayerNum], Color.White);
            ScreenManager.SpriteBatch.DrawString(font, destroyCostString, recycleStringOffset[PlayerNum], Color.White);
            ScreenManager.SpriteBatch.Draw(repairIcon, repairIconOffset[PlayerNum], Color.White);
            ScreenManager.SpriteBatch.DrawString(font, repairCostString, repairStringOffset[PlayerNum], Color.White);

            if (BaseType == BaseTowerType.Offense)
            {
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

                for (int i = 0; i < MAX_UNIT_TYPES; i++)
                {
                    if (unitInformation[i].icon != null)
                    {
                        ScreenManager.SpriteBatch.Draw(unitInformation[i].icon,
                            unitIconLocation[PlayerNum][i], Color.White);

                        if (unitTypeSelected == i)
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

            }
            
        }

        #endregion
    }
}
