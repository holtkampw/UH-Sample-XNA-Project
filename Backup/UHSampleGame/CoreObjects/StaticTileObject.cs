using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.TileSystem;

namespace UHSampleGame.CoreObjects
{
    public class StaticTileObject : StaticModel
    {
        public StaticTileObject(Model model)
            : base(model) { }

        public Tile GetTile()
        {
            return TileMap.GetTileFromPos(position);
        }
    }
}
