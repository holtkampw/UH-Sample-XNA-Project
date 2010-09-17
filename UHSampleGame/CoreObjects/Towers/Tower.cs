using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.TileSystem;
using UHSampleGame.Events;
using UHSampleGame.CoreObjects.Units;

namespace UHSampleGame.CoreObjects.Towers
{
    public abstract class Tower : StaticTileObject
    {
        TimeSpan attackTime;
        TimeSpan elapsedTime;
        Unit unitToAttack;
        int attackPower;

        public Tower(Model model)
            : base(model) 
        {
            attackTime = new TimeSpan(0, 0, 0,0,500);
            elapsedTime = new TimeSpan(0,0,1);
            unitToAttack = null;
            attackPower = 10;
        }

        public void RegisterAttackUnit(GameEventArgs args)
        {
            //TODO: check for unit closer to goal
            unitToAttack = args.Unit;
            //Register Unit Died event here
            //Check for neighbor tile units and remove dead unit
            //Might need to create another event and register a listener in tile
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (unitToAttack != null)
            {
                elapsedTime = elapsedTime.Add(gameTime.ElapsedGameTime);
                if (elapsedTime >= attackTime)
                {
                    AttackUnit();
                    elapsedTime = TimeSpan.Zero;
                }
            
            }
            else
            {
                elapsedTime = attackTime;
            }
        }

        public void AttackUnit()
        {
            if (unitToAttack != null)
            {
                unitToAttack.TakeDamage(attackPower);
            }
        }
    }
}
