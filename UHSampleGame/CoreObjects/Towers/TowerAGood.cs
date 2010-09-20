#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.ScreenManagement;
using UHSampleGame.TileSystem;
#endregion

namespace UHSampleGame.CoreObjects.Towers
{
    public class TowerAGood : Tower
    {
        #region Class Variables

        #endregion

        #region Initialization
        public TowerAGood(int playerNum, int teamNum, Tile tile)
            : base(playerNum, teamNum, ScreenManager.Game.Content.Load<Model>("Objects\\Towers\\tower1_player"), tile)
        {
            this.Scale = 4.0f;
        }
        #endregion
    }
}
