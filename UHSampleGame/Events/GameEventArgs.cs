using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UHSampleGame.TileSystem;
using UHSampleGame.CoreObjects.Units;
using UHSampleGame.CoreObjects.Base;
using UHSampleGame.CoreObjects.Towers;

namespace UHSampleGame.Events
{
    public delegate void UnitEvent(ref Unit unit);
    public delegate void TowerEvent(ref Tower tower);
    public delegate void BaseDestroyed(Base destroyedBase);
    public delegate void GetNewGoalBase();
    public delegate void TowerCreated();

}
