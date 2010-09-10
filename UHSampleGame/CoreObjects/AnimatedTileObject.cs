using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.TileSystem;

namespace UHSampleGame.CoreObjects
{
    public class AnimatedTileObject : AnimatedModel, ITileableObject 
    {
        public AnimatedTileObject(Model model)
            : base(model) { }

        public Tile GetTile()
        {
            throw new NotImplementedException();
        }
    }
}
