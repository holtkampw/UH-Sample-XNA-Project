using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.ScreenManagement;

namespace UHSampleGame.CoreObjects.Towers
{
    public class TowerTest : Tower
    {
        public TowerTest(Vector3 position)
            : base(ScreenManager.Game.Content.Load<Model>("Model\\pyramids")) 
        {
            
            this.position = position;
            this.Scale = 5;
        }
    }
}
