using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.TileSystem;

namespace UHSampleGame.CoreObjects.Base
{
    public abstract class Base : TeamableStaticObject
    {
        protected Base goalBase;
        protected Tile tile;
        protected int health;

        public Base GoalBase
        {
            get { return goalBase; }
        }

        public Tile Tile
        {
            get { return tile; }
        }

        public int Health
        {
            get { return health; }
        }

        public Base(int playerNum, int teamNum, Model model, Tile tile)
            : base(playerNum, teamNum, model, tile.Position) 
        {
            this.tile = tile;
            this.position = tile.Position;
        }

        public void SetGoalBase(Base goalBase)
        {
            this.goalBase = goalBase;
        }

        public void HitBase(int damage)
        {
            health -= damage;
        }


    }
}
