using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UHSampleGame.TileSystem;
using UHSampleGame.CoreObjects.Units;
using UHSampleGame.CoreObjects.Base;

namespace UHSampleGame.Events
{
    public delegate void RegisterUnitWithTile2(GameEventArgs2 args);
    public delegate void UnitDied2(UnitType type, Unit2 unit);
    public delegate void BaseDestroyed2(Base2 destroyedBase);
    public delegate void GetNewGoalBase2();
    public delegate void TowerCreated2();

    public class GameEventArgs2
    {
        int tilesToGoal;
        Unit2 unit;

        public GameEventArgs2(Unit2 unit)
        {
            this.unit = unit;
            this.tilesToGoal = unit.PathLength;
        }

        public int TilesToGoal
        {
            get { return tilesToGoal; }
        }

        public Unit2 Unit
        {
            get { return unit; }
        }
    }
}
