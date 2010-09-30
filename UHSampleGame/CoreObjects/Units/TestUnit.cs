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
        private const string MODEL_PATH = "Objects\\Units\\boat";

        public static Model Model
        {
            get { 
                if(model == null)
                    model = ScreenManager.Game.Content.Load<Model>(MODEL_PATH);
                return model; 
            }
        }
        public TestUnit(int playerNum, int teamNum, Vector3 position, Base.Base goalBase)
            : base(playerNum, teamNum, ScreenManager.Game.Content.Load<Model>(MODEL_PATH), goalBase,
            position) 
        {
            this.position = position;
            this.Scale = 5.0f;
            if(model == null)
                model = ScreenManager.Game.Content.Load<Model>(MODEL_PATH);
        }

    }
}
