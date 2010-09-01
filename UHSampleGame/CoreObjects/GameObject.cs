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
        Game game;
        #endregion

        #region Initialization
        public GameObject()
        {
            game = ScreenManager.Game;
        }
        #endregion

        #region Update and Draw 
        public virtual void Update(GameTime gameTime) { }
        public virtual void Draw(GameTime gameTime) { }
        #endregion
    }
}
