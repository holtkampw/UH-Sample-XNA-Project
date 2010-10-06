using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.TileSystem;
using UHSampleGame.Events;

namespace UHSampleGame.CoreObjects.Base
{
    public class Base2 : TeamableStaticObject
    {
        protected Base2 goalBase;
        protected Tile2 tile;
        protected int health;

        public event BaseDestroyed2 baseDestroyed;

        public Base2 GoalBase
        {
            get { return goalBase; }
        }

        public Tile2 Tile
        {
            get { return tile; }
        }

        public int Health
        {
            get { return health; }
        }

        public Base2(int playerNum, int teamNum, Model model, Tile2 tile)
            : base(playerNum, teamNum, model, tile.Position)
        {
            this.tile = tile;
            this.position = tile.Position;
        }

        public void SetGoalBase(Base2 goalBase)
        {
            this.goalBase = goalBase;
        }

        public void HitBase(int damage)
        {
            health -= damage;

            if (health <= 0)
                OnBaseDestroyed();
        }

        protected void OnBaseDestroyed()
        {
            if (baseDestroyed != null)
                baseDestroyed(this);
        }


    }
}
