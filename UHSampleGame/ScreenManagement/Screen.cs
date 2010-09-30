#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.InputManagement;
#endregion

namespace UHSampleGame.ScreenManagement
{
    public enum ScreenStatus { Visible, Disabled, Overlay }

    public abstract class Screen
    {
        #region Class Variables
        protected string name;
        protected ScreenStatus status;
        protected ScreenManager screenManager;
        #endregion

        #region Properties
        /// <summary>
        /// Returns the name of the screen
        /// GET only
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        public ScreenStatus Status
        {
            get { return status; }
        }

        public ScreenManager ScreenManager
        {
            get { return screenManager; }
            set { screenManager = value; }
        }

        public bool IsVisible
        {
            get { return status == ScreenStatus.Visible; }
        }
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor that creates a screen base class
        /// </summary>
        /// <param name="name">This should be a unique way to identify each screen</param>
        public Screen(string name)
        {
            this.name = name;
            this.status = ScreenStatus.Disabled;
        }

        /// <summary>
        /// Sets the current screen display status
        /// </summary>
        /// <param name="status">Current screen status</param>
        public void SetStatus(ScreenStatus status)
        {
            this.status = status;
        }
        #endregion

        #region Load and Unload Content
        public abstract void LoadContent();
        public abstract void UnloadContent();
        #endregion Load and Unload Content

        #region Update and Draw
        /// <summary>
        /// Function that contains code that will update the screen
        /// </summary>
        /// <param name="gameTime">Contains timer information</param>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Handles input logic
        /// </summary>
        /// <param name="input">The input manager for the game</param>
        public abstract void HandleInput(InputManager input);

        /// <summary>
        /// Function that contains code to draw the current screen state
        /// </summary>
        /// <param name="gameTime">Contains timer information</param>
        public abstract void Draw(GameTime gameTime);
        #endregion

        #region Helpers
        public void ExitScreen()
        {
            ScreenManager.RemoveScreen(this);
        }

        /// <summary>
        /// Reset the render states so spritebatch and models render correctly
        /// </summary>
        public void ResetRenderStates()
        {
            //ScreenManager.GraphicsDeviceManager.GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
            //ScreenManager.GraphicsDeviceManager.GraphicsDevice.RenderState.DepthBufferEnable = true;

            //ScreenManager.GraphicsDeviceManager.GraphicsDevice.RenderState.AlphaBlendEnable = true;
            //ScreenManager.GraphicsDeviceManager.GraphicsDevice.RenderState.AlphaBlendOperation = BlendFunction.Add; //Now AlphaBlendFunction
            //ScreenManager.GraphicsDeviceManager.GraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha; // Now ColorSourceBlend
            //ScreenManager.GraphicsDeviceManager.GraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha; // Now ColorDestinationBlend
            //ScreenManager.GraphicsDeviceManager.GraphicsDevice.RenderState.SeparateAlphaBlendEnabled = false;

            //ScreenManager.GraphicsDeviceManager.GraphicsDevice.RenderState.AlphaTestEnable = true;
            //ScreenManager.GraphicsDeviceManager.GraphicsDevice.RenderState.AlphaFunction = CompareFunction.Greater;
            //ScreenManager.GraphicsDeviceManager.GraphicsDevice.RenderState.ReferenceAlpha = 0;

            ScreenManager.GraphicsDeviceManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;

            //ScreenManager.GraphicsDeviceManager.GraphicsDevice.SamplerStates[0] = new SamplerState()
            //{
            //    AddressU = TextureAddressMode.Clamp,
            //    AddressV = TextureAddressMode.Clamp,
            //    Filter = TextureFilter.MinLinearMagPointMipLinear,
            //    MipMapLevelOfDetailBias = 0.0f,
            //    MaxMipLevel = 0,
            //};

            //ScreenManager.GraphicsDeviceManager.GraphicsDevice.SamplerStates[0].AddressU = TextureAddressMode.Clamp;
            //ScreenManager.GraphicsDeviceManager.GraphicsDevice.SamplerStates[0].AddressV = TextureAddressMode.Clamp;

            //ScreenManager.GraphicsDeviceManager.GraphicsDevice.SamplerStates[0].MinFilter = TextureFilter.Linear;
            //ScreenManager.GraphicsDeviceManager.GraphicsDevice.SamplerStates[0].MipFilter = TextureFilter.Linear;

            //ScreenManager.GraphicsDeviceManager.GraphicsDevice.SamplerStates[0].MipMapLevelOfDetailBias = 0.0f;
            //ScreenManager.GraphicsDeviceManager.GraphicsDevice.SamplerStates[0].MaxMipLevel = 0;
        }
        #endregion

    }
}
