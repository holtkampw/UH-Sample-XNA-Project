using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.ScreenManagement;

namespace UHSampleGame.ScenarioManagement
{
    public enum ScenarioItemType { BuildDefenseTower, BuildUnitTower, Attack, Defend, UpgradeTower, RepairTower, DestroyTower, DestroyBase }
   
    public struct ScenarioItem
    {
        public ScenarioItemType type;
        public int totalAmount;
        public int amountLeft;
        public string information;
    }

    public static class ScenarioManager
    {
        #region Class Variables
        static int playerNum;
        static int computerNum;
        static Texture2D tooltipBackground;
        static Vector2 toolTipBackgroundLocation;
        static Vector2 toolTipLocation;
        static SpriteFont font;
        #endregion

        #region Initialization
        public static void Initialize()
        {
            tooltipBackground = ScreenManager.Game.Content.Load<Texture2D>("Scenario\\objective_box");
            toolTipBackgroundLocation = new Vector2(950, 20);
            toolTipLocation = new Vector2(954, 40);
            font = ScreenManager.Game.Content.Load<SpriteFont>("Scenario\\scenarioFont");
        }
        #endregion

        public static void Update(GameTime gameTime)
        {

        }

        public static void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(tooltipBackground, toolTipBackgroundLocation, Color.White);
            ScreenManager.SpriteBatch.End();
        }

        public static void RegisterAction(ScenarioItemType action)
        {
        
        }

    }
}
