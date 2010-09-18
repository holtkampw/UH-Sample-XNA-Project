using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.TileSystem;
using UHSampleGame.ScreenManagement;

namespace UHSampleGame.CoreObjects.Towers
{
    public class TowerTest : Tower
    {
        public TowerTest(Tile tile)
            : base(ScreenManager.Game.Content.Load<Model>("Model\\pyramids"), tile) 
        {
            
            this.position = position;
            this.Scale = 5;
        }
    }
}
