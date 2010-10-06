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

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.TileSystem;


namespace UHSampleGame.CoreObjects.Towers
{
    public enum TowerType { TowerA }
    public enum TowerStatus { Inactive, Active }

    public class Tower2
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

        public TowerType Type;
        public TowerStatus Status;
        public int TeamNum;
        public int PlayerNum;
        public Unit2 unitToAttack;
        #endregion

        #region Initialization
        public Tower2(TowerType type)
        {
            this.rotationMatrixX = Matrix.Identity;
            this.rotationMatrixY = Matrix.Identity;
            this.rotationMatrixZ = Matrix.Identity;
            this.scaleRot = Matrix.Identity;
            this.scaleMatrix = Matrix.Identity;
            this.Type = type;
            this.Status = TowerStatus.Inactive;
            unitToAttack = null;
        }

        public void Setup(Vector3 position)
        {
            this.Position = TileMap2.GetTilePosFromPos(position);
            switch(Type)
            {
                case TowerType.TowerA:
                    this.Scale = 4.0f;
                    break;
            }
            UpdateScaleRotations();
            UpdateTransforms();
            this.Status = TowerStatus.Active;
        }

        public void Activate()
        {
            Status = TowerStatus.Active;
        }

        public bool IsActive()
        {
            return Status != TowerStatus.Inactive;
        }

        public void RegisterAttackUnit(GameEventArgs2 args)
        {
            if (args.Unit.TeamNum != TeamNum)
            {
                if (unitToAttack == null || args.Unit.PathLength < unitToAttack.PathLength)
                {
                    unitToAttack = args.Unit;
                    //unitToAttack.Died += GetNewAttackUnit;
                }
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
        }
        #endregion
    }
}
