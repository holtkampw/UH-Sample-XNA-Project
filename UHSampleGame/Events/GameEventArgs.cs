using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UHSampleGame.TileSystem;
using UHSampleGame.CoreObjects.Units;
using UHSampleGame.CoreObjects.Base;

namespace UHSampleGame.Events
{
    public delegate void RegisterUnitWithTile(GameEventArgs args);
    public delegate void UnitDied(UnitType type, Unit unit);
    public delegate void BaseDestroyed(Base destroyedBase);
    public delegate void TowerCreated();

    public class GameEventArgs
    {
        int tilesToGoal;
        Unit unit;

        public GameEventArgs(Unit unit)
        {
            this.unit = unit;
            this.tilesToGoal = unit.GetPathLength();
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
