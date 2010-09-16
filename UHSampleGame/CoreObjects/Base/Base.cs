using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.TileSystem;

namespace UHSampleGame.CoreObjects.Base
{
    public abstract class Base : StaticTileObject
    {
        protected Base goalBase;
        protected Tile tile;

        public Base GoalBase
        {
            get { return goalBase; }
        }

        public Tile Tile
        {
            get { return tile; }
        }

        public Base(Model model)
            : base(model) 
        {
            this.tile = GetTile();
        }

        public void SetGoalBase(Base goalBase)
        {
            this.goalBase = goalBase;
        }


    }
}
