using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.TileSystem;

namespace UHSampleGame.CoreObjects.Units
{
    public abstract class Unit : AnimatedTileObject, IPathableObject
    {
        public Unit(Model model)
            : base(model) { }

        public List<Tile> GetPath()
        {
            throw new NotImplementedException();
        }
    }
}
