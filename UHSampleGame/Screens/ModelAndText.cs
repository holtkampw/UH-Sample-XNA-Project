﻿#region Using Statements
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
    public class ModelAndText : Screen
    {
        #region Class Variables
        Texture2D background;
        InputManager inputManager;
        Model myModel;
        float aspectRatio;
        Vector3 modelPosition;
        float modelRotation;
        Vector3 cameraPosition;

        Vector2 center;
        SpriteFont font;
        string text;
        Vector2 textPosition;
        #endregion

        #region Initialization
        public ModelAndText()
            : base("ModelAndText")
        {
            background = ScreenManager.Game.Content.Load<Texture2D>("Model\\background");
            inputManager = (InputManager)ScreenManager.Game.Services.GetService(typeof(InputManager));

            #region Setup Model
            myModel = ScreenManager.Game.Content.Load<Model>("Model\\box");

            aspectRatio = ScreenManager.GraphicsDeviceManager.GraphicsDevice.Viewport.AspectRatio;
            modelPosition = Vector3.Zero;
            modelRotation = 0.0f;

            // Set the position of the camera in world space, for our view matrix.
            cameraPosition = new Vector3(0.0f, 50.0f, 5000.0f);
            #endregion

            #region Setup Text
            font = ScreenManager.Game.Content.Load<SpriteFont>("DummyText\\Font");

            center = new Vector2((ScreenManager.GraphicsDeviceManager.PreferredBackBufferWidth / 2),
                                 (ScreenManager.GraphicsDeviceManager.PreferredBackBufferHeight / 2));
            //Setup Text
            text = "Hello World! Hello World! Hellllllooooo World!";

            //Find out how long the text is using this font
            Vector2 textLength = font.MeasureString(text);

            textPosition = new Vector2(center.X - (textLength.X / 2), center.Y - (textLength.Y / 2));
            #endregion
        }
        #endregion

        #region Update and Draw
        public override void Update(GameTime gameTime)
        {
            //rotate model
            modelRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds *
                                MathHelper.ToRadians(0.1f);

            if (inputManager.CheckKeyboardAction(InputAction.Selection))
            {
                ScreenManager.Game.Exit();
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            ScreenManager.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            ScreenManager.SpriteBatch.DrawString(font, text, textPosition - new Vector2(0.0f, 50.0f), Color.White);
            ScreenManager.SpriteBatch.End();

            ResetRenderStates();
            
            DrawModel();

            ScreenManager.SpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            ScreenManager.SpriteBatch.DrawString(font, text, textPosition + new Vector2(0.0f, 50.0f), Color.White);
            ScreenManager.SpriteBatch.End();

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
                        * Matrix.CreateScale(400.0f);

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

        public void ResetRenderStates()
        {
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.RenderState.DepthBufferEnable = false;

            ScreenManager.GraphicsDeviceManager.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.RenderState.AlphaBlendOperation = BlendFunction.Add;
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = false;

            ScreenManager.GraphicsDeviceManager.GraphicsDevice.RenderState.AlphaTestEnable = true;
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.Greater;
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.RenderState.ReferenceAlpha = 0;

            ScreenManager.GraphicsDeviceManager.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Clamp;
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Clamp;

            ScreenManager.GraphicsDeviceManager.GraphicsDevice.SamplerStates[0].MagFilter = TextureFilter.Linear;
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.Linear;
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.Linear;

            ScreenManager.GraphicsDeviceManager.GraphicsDevice.SamplerStates[0].MipMapLevelOfDetailBias = 0.0f;
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.SamplerStates[0].MaxMipLevel = 0;
        }
        #endregion
    }
}
