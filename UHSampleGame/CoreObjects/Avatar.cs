#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.CameraManagement;
using UHSampleGame.ScreenManagement;
using UHSampleGame.InputManagement;
#endregion

namespace UHSampleGame.CoreObjects
{
    public class Avatar
    {
        #region Class Variables
        protected float scale;
        /// <summary>
        /// The Game the object is in
        /// </summary>
        protected Game game;
        /// <summary>
        /// The position of the object in 3D space
        /// </summary>
        public Vector3 Position;
        public Vector3 Velocity;

        float elapsedVelocitySlowDown = 0;
        float maxVelocitySlowDown = 50;

        CameraManager cameraManager;

        //Model Stuff
        Matrix view;
        Matrix transforms;
        Matrix[] boneTransforms;
        Matrix rotationMatrixX;
        Matrix rotationMatrixY;
        Matrix rotationMatrixZ;
        Model model;
        Matrix LocalRotationMatrix;
        bool avatarMoved = false;
        float pitch = 0;
        float yaw = 0;
        float roll = 0;
        Matrix YawPitchRollMatrix;
        #endregion

        #region Initialization
        /// <summary>
        /// Default Constructor to setup Model
        /// </summary>
        public Avatar(Vector3 position)
        {
            model = null;
            game = ScreenManager.Game;
            this.Position = position;
            SetupModel(position);
            SetupCamera();
        }

        /// <summary>
        /// Constructor consisting of a given model
        /// </summary>
        /// <param name="model">Model for use</param>
        public Avatar(Model newModel, Vector3 position)
        {
            this.model = newModel;
            game = ScreenManager.Game;
            SetupModel(position);
            SetupCamera();
        }

        /// <summary>
        /// Adds a model to the Static Model and performs setup
        /// </summary>
        /// <param name="model">Model for this instance</param>
        public void SetupModel(Model newModel, Vector3 position)
        {
            this.model = newModel;
            SetupModel(position);
            SetupCamera();
        }

        protected void SetupModel(Vector3 position)
        {
            //set scale
            scale = 1.0f;

            //save bones
            if (model != null)
            {
                boneTransforms = new Matrix[model.Bones.Count];
                model.CopyAbsoluteBoneTransformsTo(boneTransforms);
            }

            //setup transforms
            transforms = Matrix.CreateScale(scale) * Matrix.CreateTranslation(position);

            //give default rotation
            rotationMatrixX = Matrix.CreateRotationX(0.0f);
            rotationMatrixY = Matrix.CreateRotationY(0.0f);
            rotationMatrixZ = Matrix.CreateRotationZ(0.0f);
            LocalRotationMatrix = Matrix.Identity;
            YawPitchRollMatrix = Matrix.Identity;

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
        #endregion

        #region Properties
        public Matrix Transforms
        {
            get { return transforms; }
        }

        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                SetupCamera();
            }
        }
        #endregion

        #region Manipulation
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
        #endregion

        #region Update and Draw
        public void Update(GameTime gameTime)
        {   
            //Slow down velocity
            if (!avatarMoved)
            {
                elapsedVelocitySlowDown += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedVelocitySlowDown >= maxVelocitySlowDown)
                {
                    if (Velocity.X > 0)
                    {
                        Velocity.X--;
                    } else if (Velocity.X < 0)
                    {
                        Velocity.X++;
                    }

                    if (Velocity.Y > 0)
                    {
                        Velocity.Y--;
                    } else if (Velocity.Y < 0)
                    {
                        Velocity.Y++;
                    }

                    if (Velocity.Z > 0)
                    {
                        Velocity.Z--;
                    } else if (Velocity.Z < 0)
                    {
                        Velocity.Z++;
                    }

                    if (pitch > 0)
                    {
                        pitch -= 0.1f;
                        if (pitch <= 0.1f)
                            pitch = 0.0f;
                        ResetYawPitchRoll();
                    }
                    else if (pitch < 0)
                    {
                        pitch += 0.1f;
                        if (pitch >= -0.1f)
                            pitch = 0.0f;
                        ResetYawPitchRoll();
                    }

                    //ResetRoll();
                    //ResetYaw();

                    elapsedVelocitySlowDown = 0;
                }
            }
            else
            {
                elapsedVelocitySlowDown = 0;
            }

           
            if (Velocity != Vector3.Zero)
            {
                RotateToFace(Position, Position + Velocity, Vector3.Up);
            }

            UpdateTransforms();
        }

        // O is your object's position
        // P is the position of the object to face
        // U is the nominal "up" vector (typically Vector3.Y)
        // Note: this does not work when O is straight below or straight above P
        void RotateToFace(Vector3 O, Vector3 P, Vector3 U)
        {
            Vector3 D = (P - O);
            Vector3 Right = Vector3.Cross(U, D);
            Vector3.Normalize(ref Right, out Right);
            Vector3 Backwards = Vector3.Cross(Right, U);
            Vector3.Normalize(ref Backwards, out Backwards);
            Vector3 Up = Vector3.Cross(Backwards, Right);
            
            
            //D.Normalize();

            //roll = (float)Math.Asin(D.Z);
            //yaw = (float)Math.Acos(D.X / Math.Cos(roll));
            ////if (D.Y < 0)
            ////    yaw = MathHelper.TwoPi - yaw;
            //ResetYawPitchRoll();


            Matrix rot = new Matrix(Right.X, Right.Y, Right.Z, 0, Up.X, Up.Y, 
                Up.Z, 0, Backwards.X, Backwards.Y, Backwards.Z, 0, 0, 0, 0, 1);
            LocalRotationMatrix = rot;
        }

        public void ResetRoll()
        {
            if (roll > 0)
            {
                roll -= 0.2f;
                if (roll <= 0.2f)
                    roll = 0.0f;
                ResetYawPitchRoll();
            }
            else if (roll < 0)
            {
                roll += 0.2f;
                if (roll >= -0.2f)
                    roll = 0.0f;
                ResetYawPitchRoll();
            }
        }

        public void ResetYaw()
        {
            if (yaw > 0)
            {
                yaw -= 0.2f;
                if (yaw <= 0.2f)
                    yaw = 0.0f;
                ResetYawPitchRoll();
            }
            else if (yaw < 0)
            {
                yaw += 0.2f;
                if (yaw >= -0.2f)
                    yaw = 0.0f;
                ResetYawPitchRoll();
            }
        }

        public void ResetYawPitchRoll()
        {
            pitch = MathHelper.Clamp(pitch, -0.40f, 0.40f);
            roll = MathHelper.Clamp(roll, -0.50f, 0.50f);
            yaw = MathHelper.Clamp(yaw, -0.50f, 0.50f);
            Matrix.CreateFromYawPitchRoll(yaw, pitch, roll, out YawPitchRollMatrix);
        }

        public void UpdateView()
        {
            view = cameraManager.ViewMatrix;
        }

        public bool HandleInput(InputManager input, PlayerIndex index)
        {
            avatarMoved = false;
           
            if (input.CheckAction(InputAction.TileMoveUp, index))
            {
                Velocity = Velocity + new Vector3(0, 0, -1);
                avatarMoved = true;
            }

            if (input.CheckAction(InputAction.TileMoveDown, index))
            {
                Velocity = Velocity + new Vector3(0, 0, 1);
                avatarMoved = true;
            }

            if (input.CheckAction(InputAction.TileMoveLeft, index))
            {
                Velocity = Velocity + new Vector3(-1, 0, 0);
                avatarMoved = true;
            }

            if (input.CheckAction(InputAction.TileMoveRight, index))
            {
                Velocity = Velocity + new Vector3(1, 0, 0);
                avatarMoved = true;
            }

            Velocity.X = MathHelper.Clamp(Velocity.X, -7f, 7f);
            Velocity.Y = MathHelper.Clamp(Velocity.Y, -7f, 7f);
            Velocity.Z = MathHelper.Clamp(Velocity.Z, -7f, 7f);

            if (avatarMoved)
            {
                pitch += 0.05f;
                ResetYawPitchRoll();
            }

            if (Velocity != Vector3.Zero)
            {
                Position += Velocity;
                return true;
            }
            return avatarMoved;

        }

        public void UpdateTransforms()
        {
            transforms = Matrix.CreateScale(scale) *
                    YawPitchRollMatrix *
                    LocalRotationMatrix *
                    Matrix.CreateTranslation(Position);
        }

        public void Draw(GameTime gameTime)
        {
            if (model != null)
            {
                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in model.Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = boneTransforms[mesh.ParentBone.Index] * transforms;
                        effect.View = cameraManager.ViewMatrix;
                        effect.Projection = cameraManager.ProjectionMatrix;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }


            }
        }
        #endregion
    }
}
