using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UHSampleGame.TileSystem;
using UHSampleGame.CoreObjects.Units;
using UHSampleGame.CoreObjects.Base;

namespace UHSampleGame.Events
{
    public delegate void RegisterUnitWithTile(ref Unit unit);
    public delegate void UnitDied(UnitType type, Unit unit);
    public delegate void BaseDestroyed(Base destroyedBase);
    public delegate void GetNewGoalBase();
    public delegate void TowerCreated();

    public class GameEventArgs
    {
        public Unit Unit;

        public GameEventArgs(Unit unit)
        {
           Unit = unit;
        }


    }
}
