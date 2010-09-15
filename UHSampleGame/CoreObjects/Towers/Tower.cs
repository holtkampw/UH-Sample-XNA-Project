using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.TileSystem;

namespace UHSampleGame.CoreObjects.Towers
{
    public abstract class Tower : StaticModel, ITileableObject//AnimatedModel, ITileableObject
    {
        public Tower(Model model)
            : base(model) { }

        public Tile GetTile()
        {
            throw new NotImplementedException();
        }
    }
}
