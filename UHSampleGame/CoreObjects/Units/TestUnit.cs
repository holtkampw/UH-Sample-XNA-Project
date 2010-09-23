using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using UHSampleGame.ScreenManagement;
using UHSampleGame.TileSystem;
using UHSampleGame.CoreObjects.Base;

namespace UHSampleGame.CoreObjects.Units
{
    public class TestUnit : Unit
    {
        public static Model model;

        public static Model Model
        {
            get { 
                if(model == null)
                    model = ScreenManager.Game.Content.Load<Model>("Objects\\Towers\\towerA_red");
                return model; 
            }
        }
        public TestUnit(int playerNum, int teamNum, Vector3 position, Base.Base goalBase)
            : base(playerNum, teamNum, ScreenManager.Game.Content.Load<Model>("Objects\\Towers\\towerA_red"), goalBase,
            position) 
        {
            this.position = position;
            this.Scale = 0.5f;
            if(model == null)
                model = ScreenManager.Game.Content.Load<Model>("Objects\\Towers\\towerA_red");
        }

    }
}
