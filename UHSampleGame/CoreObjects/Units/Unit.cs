using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.TileSystem;

namespace UHSampleGame.CoreObjects.Units
{
    public abstract class Unit : StaticModel, ITileableObject //Change back to animated model?
    {
        public Vector3 velocity;
        private Tile previousTile;
        private Tile currentTile;

        public Unit(Model model)
            : base(model)
        {
            previousTile = new Tile();
            currentTile = new Tile();
        }

        public override void Update(GameTime gameTime)
        {
            previousTile = currentTile;
            currentTile = GetTile();
            if (currentTile.Path.Count < 1)
                return;

            if (currentTile.GetTileType() == TileType.Blocked)
            {
                if(previousTile.IsWalkable())
                {
                    currentTile = previousTile;
                }
                else
                {
                    List<Tile> goodNieghbors = TileMap.GetWalkableNeighbors(currentTile);
                    if (goodNieghbors.Count > 0)
                        currentTile = goodNieghbors[0];
                    else
                        throw new NotImplementedException("NO walkable neighbors... handle this!");
                }
                position = currentTile.Position;
            }

            Tile nextTile = currentTile.Path[1];
            
            velocity = new Vector3(nextTile.Position.X-currentTile.Position.X, 0, nextTile.Position.Z - currentTile.Position.Z);
            velocity.Normalize();
            this.position += velocity;
            base.Update(gameTime);
        }

        public Tile GetTile()
        {
            return TileMap.GetTileFromPos(position);
        }
    }
}
