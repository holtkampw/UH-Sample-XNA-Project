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
    public abstract class Tower : TeamableStaticObject
    {
        TimeSpan attackTime;
        TimeSpan elapsedTime;
        Unit unitToAttack;
        Tile tile;
        int attackPower;

        public Tower(int playerNum, int teamNum, Model model, Tile tile)
            : base(playerNum, teamNum, model) 
        {
            attackTime = new TimeSpan(0, 0, 0,0,500);
            elapsedTime = new TimeSpan(0,0,1);
            unitToAttack = null;
            attackPower = 10;
            this.tile = tile;
            this.position = tile.Position;
        }

        public void RegisterAttackUnit(GameEventArgs args)
        {
            if (args.Unit.TeamNum != teamNum)
            {
                if (unitToAttack == null || args.Unit.GetPathLength() < unitToAttack.GetPathLength())
                {
                    unitToAttack = args.Unit;
                    unitToAttack.Died += GetNewAttackUnit;
                }
            }
            
        }

        private void GetNewAttackUnit(Unit unit)
        {
            unitToAttack = null;
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
