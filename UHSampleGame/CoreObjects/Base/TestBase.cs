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
    public class TestBase : Base
    {
        public TestBase(Tile tile)
            :base(ScreenManager.Game.Content.Load<Model>("Model\\pyramids"))
        {
            this.scale = 5;
            this.position = tile.Position;
            this.tile = tile;
        }
    }
}
