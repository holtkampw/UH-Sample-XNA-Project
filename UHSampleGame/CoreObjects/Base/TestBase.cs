using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using UHSampleGame.ScreenManagement;


namespace UHSampleGame.CoreObjects.Base
{
    public class TestBase : Base
    {
        public TestBase(Vector3 position)
            :base(ScreenManager.Game.Content.Load<Model>("Model\\pyramids"))
        {
            this.scale = 5;
            this.position = position;
        }
    }
}
