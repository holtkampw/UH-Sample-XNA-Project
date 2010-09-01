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
        protected Game game;
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
        public Vector3 Position
        {
            get { return position; }
        }
        #endregion

        #region Manipulation
        public void SetPosition(Vector3 position)
        {
            this.position = position;
        }
        #endregion

        #region Update and Draw
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }
        #endregion
    }
}
