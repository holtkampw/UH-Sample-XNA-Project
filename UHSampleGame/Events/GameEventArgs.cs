using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UHSampleGame.TileSystem;
using UHSampleGame.CoreObjects.Units;
using UHSampleGame.CoreObjects.Base;

namespace UHSampleGame.Events
{
    public delegate void RegisterUnitWithTile2(GameEventArgs args);
    public delegate void UnitDied2(UnitType type, Unit unit);
    public delegate void BaseDestroyed2(Base destroyedBase);
    public delegate void GetNewGoalBase2();
    public delegate void TowerCreated2();

    public class GameEventArgs
    {
        int tilesToGoal;
        Unit unit;

        public GameEventArgs(Unit unit)
        {
            this.unit = unit;
            this.tilesToGoal = unit.PathLength;
        }

        public int TilesToGoal
        {
            get { return tilesToGoal; }
        }

        public Unit Unit
        {
            get { return unit; }
        }
    }
}
