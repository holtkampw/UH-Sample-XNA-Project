using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UHSampleGame.CoreObjects.Units
{
    public class Unit2
    {
        UnitStatus Status;
        public Unit2(UnitType unitType)
        {
            Status = UnitStatus.Inactive;
        }

        public void Activate()
        {
            Status = UnitStatus.Active;
        }

        public bool IsActive()
        {
            return Status != UnitStatus.Inactive;
        }
    }
}
