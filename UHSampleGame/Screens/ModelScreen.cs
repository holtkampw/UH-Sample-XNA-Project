#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.ScreenManagement;
using UHSampleGame.InputManagement;
#endregion

namespace UHSampleGame.Screens
{
    public class ModelScreen : Screen
    {
        #region Class Variables
        Texture2D background;
        InputManager inputManager;
        Model myModel;
        float aspectRatio;
        Vector3 modelPosition;
        float modelRotation;
        Vector3 cameraPosition;
        #endregion

        #region Initialization
        public ModelScreen()
            : base("Model")
        {
            background = ScreenManager.Game.Content.Load<Texture2D>("Model\\background");
            inputManager = (InputManager)ScreenManager.Game.Services.GetService(typeof(InputManager));
            myModel = ScreenManager.Game.Content.Load<Model>("Model\\box");

            aspectRatio = ScreenManager.GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio;
            modelPosition = Vector3.Zero;
            modelRotation = 0.0f;
            
            // Set the position of the camera in world space, for our view matrix.
            cameraPosition = new Vector3(0.0f, 50.0f, 5000.0f);
        }
        #endregion

        #region Update and Draw 
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //rotate model
            modelRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds *
                                MathHelper.ToRadians(0.1f);

            if (inputManager.CheckKeyboardAction(InputAction.Selection))
            {
                ScreenManager.ShowScreen(new ModelAndText());
            }
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            ScreenManager.SpriteBatch.End();
            DrawModel();
            base.Draw(gameTime);
        }

        public void DrawModel()
        {
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw all of the meshes for a model
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                //each mesh has an effect
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    //Where will the model be in the world?
                    effect.World = transforms[mesh.ParentBone.Index]
                        * Matrix.CreateRotationY(modelRotation)
                        * Matrix.CreateRotationZ(modelRotation)
                        * Matrix.CreateTranslation(modelPosition)
                        * Matrix.CreateScale(200.0f);

                    //How are we viewing it?
                    effect.View = Matrix.CreateLookAt(cameraPosition,
                        Vector3.Zero, Vector3.Up);

                    //Projection information
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(45.0f), aspectRatio,
                        1.0f, 10000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
        #endregion
    }
}
