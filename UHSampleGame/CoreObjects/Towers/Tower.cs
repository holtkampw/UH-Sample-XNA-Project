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

namespace UHSampleGame.CoreObjects.Towers
{
    //public enum TowerType { TowerA }

    public class Tower
    {

        #region Class Variables    
        //Type Information
        public static Dictionary<int, Model> Models = new Dictionary<int, Model>()
        {
            {(int)TowerType.TowerA, ScreenManager.Game.Content.Load<Model>("Objects\\Towers\\towerA_red")}
        };
        public static Enum[] towerEnumType = EnumHelper.EnumToArray(new TowerType());

        TimeSpan attackTime;
        TimeSpan elapsedTime;
        Unit unitToAttack;
        public Tile Tile;
        int attackPower;

        public TowerType Type;

        //TeamStaticObject
        protected int PlayerNum;
        protected int TeamNum;

        //StaticModel
        public float Scale;

        CameraManager cameraManager;

        //Model Stuff
        Matrix view;
        public Matrix Transforms;
        Matrix[] boneTransforms;
        Matrix rotationMatrixX;
        Matrix rotationMatrixY;
        Matrix rotationMatrixZ;

        //GameObject
        public Vector3 Position;
        #endregion

        public Tower(TowerType newType, int playerNum, int teamNum, Tile tile)
        {
            this.Type = newType;
            attackTime = new TimeSpan(0, 0, 0,0,500);
            elapsedTime = new TimeSpan(0,0,1);
            unitToAttack = null;
            attackPower = 10;
            this.Tile = tile;
            this.Position = tile.Position;

            //TeamStaticObject
            this.PlayerNum = playerNum;
            this.TeamNum = teamNum;

            foreach (TowerType type in towerEnumType)
            {
                switch (type)
                {
                    case TowerType.TowerA:
                        this.Scale = 4.0f;
                        break;
                }
            }
            SetupModel(Position);
            SetupCamera();
        }

        public void RegisterAttackUnit(GameEventArgs args)
        {
            if (args.Unit.TeamNum != TeamNum)
            {
                if (unitToAttack == null || args.Unit.GetPathLength() < unitToAttack.GetPathLength())
                {
                    unitToAttack = args.Unit;
                    unitToAttack.Died += GetNewAttackUnit;
                }
            }
            
        }

        private void GetNewAttackUnit(UnitType type, Unit unit)
        {
            unitToAttack = null;
        }

        public void Update(GameTime gameTime)
        {
            if (unitToAttack != null)
            {
                elapsedTime = elapsedTime.Add(gameTime.ElapsedGameTime);
                if (elapsedTime >= attackTime)
                {
                    AttackUnit();
                    elapsedTime = TimeSpan.Zero;
                }
            }
            else
            {
                elapsedTime = attackTime;
            }
            UpdateView();
            UpdateTransforms();
        }

        public void Draw(GameTime gameTime)
        {
            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in Models[(int)Type].Meshes)
            {
                // This is where the mesh orientation is set, as well 
                // as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = boneTransforms[mesh.ParentBone.Index] * Transforms;
                    effect.View = cameraManager.ViewMatrix;
                    effect.Projection = cameraManager.ProjectionMatrix;
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }

        public void AttackUnit()
        {
            if (unitToAttack != null)
            {
                unitToAttack.TakeDamage(attackPower);
            }
        }

        #region TeamStaticObject
        public void SetPlayerNum(int newPlayerNum)
        {
            PlayerNum = newPlayerNum;
        }

        public void SetTeamNum(int newTeamNum)
        {
            TeamNum = newTeamNum;
        }
        #endregion

        #region StaticTileObject
        public Tile GetTile()
        {
            return TileMap.GetTileFromPos(Position);
        }
        #endregion

        #region StaticModel
        protected void SetupModel(Vector3 position)
        {
            //save bones
            switch (Type)
            {
                case TowerType.TowerA:
                    boneTransforms = new Matrix[Models[(int)Type].Bones.Count];
                    Models[(int)Type].CopyAbsoluteBoneTransformsTo(boneTransforms);
                    break;
            }

            //setup transforms
            Transforms = Matrix.CreateScale(Scale) * Matrix.CreateTranslation(position);

            //give default rotation
            rotationMatrixX = Matrix.CreateRotationX(0.0f);
            rotationMatrixY = Matrix.CreateRotationY(0.0f);
            rotationMatrixZ = Matrix.CreateRotationZ(0.0f);

            //give default position
            this.Position = position;
        }

        /// <summary>
        /// Sets up default camera information
        /// </summary>
        protected void SetupCamera()
        {
            cameraManager = (CameraManager)ScreenManager.Game.Services.GetService(typeof(CameraManager));
            view = cameraManager.ViewMatrix;
        }

        public void RotateX(float rotation)
        {
            rotationMatrixX = Matrix.CreateRotationX(rotation);
        }

        public void RotateY(float rotation)
        {
            rotationMatrixY = Matrix.CreateRotationY(rotation);
        }

        public void RotateZ(float rotation)
        {
            rotationMatrixZ = Matrix.CreateRotationZ(rotation);
        }

        public void UpdateView()
        {
            view = cameraManager.ViewMatrix;
        }

        public void UpdateTransforms()
        {
            Matrix scaleMatrix = Matrix.CreateScale(this.Scale);
            Matrix translation = Matrix.CreateTranslation(Position);
            Transforms = scaleMatrix *
                    rotationMatrixX *
                    rotationMatrixY *
                    rotationMatrixZ *
                    translation;
        }
        #endregion
    }
}
