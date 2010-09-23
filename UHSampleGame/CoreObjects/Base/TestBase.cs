﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using UHSampleGame.ScreenManagement;
using UHSampleGame.TileSystem;

namespace UHSampleGame.CoreObjects.Base
{
    public class TestBase : Base
    {
        public TestBase(int playerNum, int teamNum, Tile tile)
            :base(playerNum, teamNum, ScreenManager.Game.Content.Load<Model>("Objects\\Towers\\towerA_red"),
            tile)
        {
            this.Scale = 5;
            
            this.tile = tile;
        }
    }
}
