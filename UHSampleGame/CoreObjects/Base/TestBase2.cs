using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using UHSampleGame.ScreenManagement;
using UHSampleGame.TileSystem;

namespace UHSampleGame.CoreObjects.Base
{
    public class TestBase2 : Base2
    {
        public TestBase2(int playerNum, int teamNum, Tile2 tile)
            : base(playerNum, teamNum, ScreenManager.Game.Content.Load<Model>("Objects\\Base\\enemyBaseShip01"),
            tile)
        {
            this.Scale = 2.75f;

            this.tile = tile;
        }
    }
}
