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
        const string MODEL_PATH = "Objects\\Units\\boat";
        static Model model;

        public static Model Model
        {
            get { 
                if(model == null)
                    model = ScreenManager.Game.Content.Load<Model>(MODEL_PATH);
                return model; 
            }
        }
        public TestUnit(int playerNum, int teamNum, Vector3 position, Base.Base goalBase)
            : base(UnitType.TestUnit, playerNum, teamNum, position, goalBase) 
        {
            
            if(model == null)
                model = ScreenManager.Game.Content.Load<Model>(MODEL_PATH);
        }

    }
}
