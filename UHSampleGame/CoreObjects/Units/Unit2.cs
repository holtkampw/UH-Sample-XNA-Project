using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace UHSampleGame.CoreObjects.Units
{
    public class Unit2
    {
        #region Class Variables
        UnitStatus Status;
        public Matrix Transforms;
        #endregion

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

        public void Update(GameTime gameTime)
        {

        }

        public void Draw(GameTime gameTime)
        {

        }
    }
}
