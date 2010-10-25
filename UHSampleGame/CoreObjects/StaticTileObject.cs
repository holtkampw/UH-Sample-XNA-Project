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
        public StaticTileObject(Model model, Vector3 position)
            : base(model, position) { }

        public Tile GetTile()
        {
            return TileMap.GetTileFromPos(Position);
        }
    }
}
