#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
#endregion

namespace UHSampleGame.ScreenManagement
{
    public enum ScreenStatus { Visible, Disabled, Overlay }

    public class Screen
    {
        #region Class Variables
        string name;
        ScreenStatus status;
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
        #endregion


        #region Update and Draw
        /// <summary>
        /// Function that contains code that will update the screen
        /// </summary>
        /// <param name="gameTime">Contains timer information</param>
        public virtual void Update(GameTime gameTime) {}

        /// <summary>
        /// Function that contains code to draw the current screen state
        /// </summary>
        /// <param name="gameTime">Contains timer information</param>
        public virtual void Draw(GameTime gameTime) {}
        #endregion

    }
}
