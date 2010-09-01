#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UHSampleGame.ScreenManagement;
#endregion

namespace UHSampleGame.CameraManagement
{
    public class CameraManager
    {
        #region Class Variables
        float aspectRatio;
        Matrix viewMatrix;
        Matrix projectionMatrix;
        Matrix rotationMatrix;
        Vector3 position;
        Vector3 lookAtPoint;
        float rotationLeftRight;
        float rotationUpDown;
        #endregion

        #region Properties
        public Matrix ViewMatrix
        {
            get { return viewMatrix; }
        }

        public Matrix ProjectionMatrix
        {
            get { return projectionMatrix; }
        }

        public Vector3 Position
        {
            get { return position; }
        }

        public Vector3 LookAtPoint
        {
            get { return lookAtPoint; }
        }
        #endregion

        #region Initialization
        public CameraManager()
        {
            //tmp position
            position = Vector3.Zero;

            //tmp lookAtPoint
            lookAtPoint = Vector3.Zero;

            //set aspect ratio
            aspectRatio = (float)ScreenManager.GraphicsDeviceManager.GraphicsDevice.Viewport.Width /
                          (float)ScreenManager.GraphicsDeviceManager.GraphicsDevice.Viewport.Height;

            //setup projection
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                            MathHelper.ToRadians(40.0f),
                            this.aspectRatio,
                            1.0f,
                            10000.0f);

            viewMatrix = Matrix.CreateLookAt(position, lookAtPoint, Vector3.Up);
            rotationLeftRight = 0.0f;
            rotationUpDown = 0.0f;
        }
        #endregion

        #region Manipulation
        public void StrafeX(float amount)
        {
            position = new Vector3(position.X + amount, position.Y, position.Z);
            lookAtPoint = new Vector3(lookAtPoint.X + amount, lookAtPoint.Y, lookAtPoint.Z);
        }

        public void StrafeY(float amount)
        {
            position = new Vector3(position.X, position.Y + amount, position.Z);
            lookAtPoint = new Vector3(lookAtPoint.X, lookAtPoint.Y + amount, lookAtPoint.Z);
        }

        public void StrafeZ(float amount)
        {
            position = new Vector3(position.X, position.Y, position.Z + amount);
            lookAtPoint = new Vector3(lookAtPoint.X, lookAtPoint.Y, lookAtPoint.Z + amount);
        }

        public void RotateX(float amount)
        {
            rotationLeftRight += amount;
            //position = new Vector3(position.X + amount, position.Y, position.Z);
        }

        public void RotateY(float amount)
        {
            rotationUpDown -= amount;
            //position = new Vector3(position.X, position.Y + amount, position.Z);
        }

        public void SetPosition(Vector3 position)
        {
            this.position = position;
        }

        public void SetLookAtPoint(Vector3 lookAtPoint)
        {
            this.lookAtPoint = lookAtPoint;
        }
        #endregion

        #region Update
        public void Update()
        {
            rotationMatrix = Matrix.CreateRotationX(rotationUpDown) * Matrix.CreateRotationY(rotationLeftRight);

            //For rotating look at point around camera position
            //Vector3 cameraRotatedTarget = Vector3.Transform(new Vector3(0,0,-1), rotationMatrix);
            //Vector3 cameraFinalTarget = position + cameraRotatedTarget;
            //Vector3 cameraRotatedUpVector = Vector3.Transform(Vector3.Up, rotationMatrix);
            //viewMatrix = Matrix.CreateLookAt(position, cameraFinalTarget, cameraRotatedUpVector);

            //For rotating camera position around look at point
            Vector3 cameraRotatedPosition = Vector3.Transform(position, rotationMatrix);
            Vector3 cameraRotatedUpVector = Vector3.Transform(Vector3.Up, rotationMatrix);
            viewMatrix = Matrix.CreateLookAt(cameraRotatedPosition, lookAtPoint, cameraRotatedUpVector);

            //standard without rotation
            //viewMatrix = Matrix.CreateLookAt(position, lookAtPoint, Vector3.Up);
        }
        #endregion
    }
}
