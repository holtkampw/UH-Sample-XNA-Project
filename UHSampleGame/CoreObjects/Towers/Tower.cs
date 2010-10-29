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


namespace UHSampleGame.CoreObjects.Towers
{
    public enum TowerType { Plasma, Cannon, Electric, Unit }
    public enum TowerStatus { Inactive, Active }

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

        private TimeSpan timeToAttack;
        private TimeSpan currentTimeToAttack;

        public int ID;

        public int Health;
        public int HealthCapacity;
        public int XP;
        public int Level;
        public int Cost;
        public int TotalInvestedCost;

        TimeSpan unitBuild = new TimeSpan(0, 0, 1);
        TimeSpan currentTimeSpan = new TimeSpan();

        static int currentID = 0;
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

            timeToAttack = new TimeSpan(0, 0, 0, 0, 200);
            currentTimeToAttack = new TimeSpan();

            //Depends on Type
            HealthCapacity = 100;
            Health = HealthCapacity;
            XP = 0;
            Level = 0;
            Cost = 100;
            TotalInvestedCost = Cost;

            ID = currentID;
            currentID++;
        }

        public void Setup(Vector3 position)
        {
            this.Position = position;
            //this.Position = TileMap.GetTilePosFromPos(position);
            switch (Type)
            {
                case TowerType.Plasma:
                    this.Scale = 4.0f;
                    break;
                case TowerType.Electric:
                    this.Scale = 3.0f;
                    break;
                case TowerType.Cannon:
                    this.Scale = 2.0f;
                    break;
                case TowerType.Unit:
                    this.Scale = 2.0f;
                    break;
            }
            UpdateScaleRotations();
            UpdateTransforms();
            this.Status = TowerStatus.Active;
        }

        public void Activate(int playerNum, int teamNum)
        {
            Status = TowerStatus.Active;
            PlayerNum = playerNum;
            TeamNum = teamNum;
        }

        public bool IsActive()
        {
            return Status != TowerStatus.Inactive;
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
                (unitToAttack != null && unitToAttack.IsActive() && unit == null))
                return;

            if (unitToAttack != null && !unitToAttack.IsActive())
            {
                unitToAttack = unit;
                return;
            }

            if (unit.TeamNum != TeamNum)
            {
                if (unitToAttack == null || unit.PathLength < unitToAttack.PathLength)
                {
                    unitToAttack = unit;
                    //unitToAttack.Died += GetNewAttackUnit;
                }
            }
        }

        private void Attack(GameTime gameTime)
        {
            currentTimeToAttack += gameTime.ElapsedGameTime;
            if (currentTimeToAttack > timeToAttack)
            {
                if (unitToAttack != null && unitToAttack.Health > 0)
                {
                    ProjectileManager.AddParticle(this.Position, unitToAttack.Position);
                    unitToAttack.TakeDamage(attackStrength);
                    //DO XP GIVING HERE                    
                }
                currentTimeToAttack = TimeSpan.Zero;
            }
        }

        public int Repair(int money)
        {
            float perc = 1.0f-(Health / (float)HealthCapacity);
            Health = HealthCapacity;
            int costToRepair = (int)(perc * Cost);

            if (money >= costToRepair)
                return costToRepair;

            return 0;
        }

        public int Upgrade(int money)
        {
            int upgradeCost = Cost + (int)((Level / 4.0f) * Cost);

            if (money >= upgradeCost)
            {
                TotalInvestedCost += upgradeCost;
                Level++;
                XP = 0;
                return upgradeCost;
            }

            return 0;
        }

        public int DestroyCost()
        {
            float perc = (Health / (float)HealthCapacity);
            return (int)(TotalInvestedCost * perc*.75f);
        }

        private void BuildUnit(GameTime gameTime)
        {
            currentTimeSpan = currentTimeSpan.Add(gameTime.ElapsedGameTime);
            if (currentTimeSpan > unitBuild)
            {
                UnitCollection.Build(PlayerNum, TeamNum, UnitTypeToBuild);
                currentTimeSpan = TimeSpan.Zero;
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
            if (Type != TowerType.Unit)
                Attack(gameTime);
            else
            {
                BuildUnit(gameTime);
            }
        }
        #endregion

        
    }
}
