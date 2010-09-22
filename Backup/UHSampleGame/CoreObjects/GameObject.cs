#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.ScreenManagement;
#endregion

namespace UHSampleGame.CoreObjects
{
    public class GameObject
    {
        #region Class Variables
        /// <summary>
        /// The Game the object is in
        /// </summary>
        protected Game game;
        /// <summary>
        /// The position of the object in 3D space
        /// </summary>
        protected Vector3 position;
        #endregion

        #region Initialization
        public GameObject()
        {
            game = ScreenManager.Game;
            position = Vector3.Zero;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the position of the object in 3D space
        /// </summary>
        public Vector3 Position
        {
            get { return position; }
        }
        #endregion

        #region Manipulation
        /// <summary>
        /// Sets the position of the object in 3D space
        /// </summary>
        /// <param name="position">The position to which to set the object</param>
        public void SetPosition(Vector3 position)
        {
            this.position = position;
        }
        #endregion

        #region Update and Draw
        /// <summary>
        /// Updates various components in the game
        /// </summary>
        /// <param name="gameTime">Current game time</param>
        public virtual void Update(GameTime gameTime) { }

        /// <summary>
        /// Draws various components in the game
        /// </summary>
        /// <param name="gameTime">Current game time</param>
        public virtual void Draw(GameTime gameTime) { }
        #endregion
    }
}
