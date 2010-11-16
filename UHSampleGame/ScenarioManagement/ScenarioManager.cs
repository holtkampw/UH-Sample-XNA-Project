using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using UHSampleGame.ScreenManagement;

namespace UHSampleGame.ScenarioManagement
{
    public enum ScenarioItemType
    {
        AvatarMove, BuildDefenseTower, DestroyEnemyTower, ShowHUD, RepairTower, UpgradeTower, DestroyTower, BuildUnitTower, Deploy, DestroyBase, Power
    }
   
    public class ScenarioItem
    {
        public ScenarioItemType type;
        public int amountLeft;
        public string information;

        public ScenarioItem()
        {
        }

        public ScenarioItem(ScenarioItemType type, int amountLeft, string information)
        {
            this.type = type;
            this.amountLeft = amountLeft;
            this.information = information;
        }
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
        static List<ScenarioItem> scenarioItems;
        #endregion

        #region Initialization
        public static void Initialize()
        {
            tooltipBackground = ScreenManager.Game.Content.Load<Texture2D>("Scenario\\objective_box");
            toolTipBackgroundLocation = new Vector2(40, 370);
            toolTipLocation = new Vector2(44, 400);
            font = ScreenManager.Game.Content.Load<SpriteFont>("Scenario\\scenarioFont");
            scenarioItems = new List<ScenarioItem>();

            //start scripting
            scenarioItems.Add(
                 new ScenarioItem(ScenarioItemType.AvatarMove, 1,
                     "Move your avatar around.  You can move the avatar by using the left thumbstick."
                 ));

            scenarioItems.Add(
                new ScenarioItem(ScenarioItemType.BuildDefenseTower, 10, 
                    "Build 10 defensive towers.  To build a tower, navigate to the third UI tab using the DPad left/right buttons. In that tab, " + 
                    "you can select which tower to build using the DPad Up/Down buttons.  To build the towers, click the A button and it will build " +
                    "the currently selected tower.  The tower will be built under the avatar's position."
                ));

            scenarioItems.Add(
                new ScenarioItem(ScenarioItemType.DestroyEnemyTower, 1,
                    "Your towers can also attack enemy towers.  Move your avatar near an enemy tower.  If you build towers surrounding the enemy's tower, " +
                    "then your towers will attack until the enemy until it is destroyed.  Destroy 1 enemy towers."
                ));

            scenarioItems.Add(
               new ScenarioItem(ScenarioItemType.ShowHUD, 1,
                   "Your tower took damage from the enemy.  Press in the left thumbstick to quickly see your tower's health and upgrade progress."
               ));

            scenarioItems.Add(
                new ScenarioItem(ScenarioItemType.RepairTower, 1,
                    "You can repair the tower to recover health.  Place your avatar over the tower that was attacked  " +
                    "and press the Right Shoulder button.  This allows you to see information about your tower, including the cost to repair it.  " +
                    "Now, repair the tower using the Y button."
                ));

            scenarioItems.Add(
                new ScenarioItem(ScenarioItemType.UpgradeTower, 3,
                    "Towers can also be upgraded to become stronger and faster.  You can upgrade a tower by hovering over it with your avatar and  " +
                    "pressing the X button.  Upgrade 3 towers now."
                ));

            scenarioItems.Add(
                new ScenarioItem(ScenarioItemType.DestroyTower, 1,
                    "From time to time, you might want to replace an existing tower or get rid of it all together. " +
                    "You can recycle existing by hovering over a tower and pressing the B button.  Recycling towers earns you a percentage of its costs back."
                ));

            scenarioItems.Add(
                new ScenarioItem(ScenarioItemType.Power, 1,
                    "Your avatar has special powers that can help you throughout the game.  Navigate to the powers tab using the DPad.  " +
                    "Select the 'Freeze Enemies' power and hit the Left Trigger to invoke.  All the enemy's units will freeze allowing you time to kill them."
                ));

            scenarioItems.Add(
                new ScenarioItem(ScenarioItemType.BuildUnitTower, 3,
                    "Now its time to start building your offense.  You can build a unit tower by moving to the second UI tab (using the DPad buttons).  " +
                    "Again, building a tower is done by pressing the A button.  Build 3 Unit towers."
                ));

            scenarioItems.Add(
                new ScenarioItem(ScenarioItemType.Deploy, 3,
                    "You are constantly building units now.  You can select which unit to deploy by using the right thumbstick button.  " +
                    "After selecting a unit type, hold down the right trigger to deploy units.  You will see a bar rise.  That bar indicates the " +
                    "percentage of units are being deployed.  The longer you hold, the more units that will be deployed."
                ));

            scenarioItems.Add(
                new ScenarioItem(ScenarioItemType.DestroyBase, 3,
                    "Now its time to destroy the enemy base.  Deploy enough units to destroy his base."
                ));

            for (int i = 0; i < scenarioItems.Count; i++)
            {
                scenarioItems[i].information = WordWrap(scenarioItems[i].information, font, 280);
            }
        }
        #endregion

        public static void Update(GameTime gameTime)
        {

        }

        public static void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin();
            ScreenManager.SpriteBatch.Draw(tooltipBackground, toolTipBackgroundLocation, Color.White);
            ScreenManager.SpriteBatch.DrawString(font, scenarioItems[0].information, toolTipLocation, Color.White);
            ScreenManager.SpriteBatch.End();
        }

        public static void RegisterAction(ScenarioItemType action)
        {
            for (int i = 0; i < scenarioItems.Count; i++)
            {
                if (scenarioItems[i].type == action)
                {
                    switch (scenarioItems[i].type)
                    {
                        case ScenarioItemType.BuildDefenseTower:
                        case ScenarioItemType.BuildUnitTower:
                        case ScenarioItemType.RepairTower:
                        case ScenarioItemType.UpgradeTower:
                        case ScenarioItemType.DestroyTower:
                        case ScenarioItemType.AvatarMove:
                            scenarioItems[i].amountLeft--;
                            if (scenarioItems[i].amountLeft <= 0)
                            {
                                scenarioItems.Remove(scenarioItems[i]);
                            }
                            break;
                        case ScenarioItemType.ShowHUD:
                            if (i == 0)
                                scenarioItems.Remove(scenarioItems[0]);
                            break;
                        case ScenarioItemType.DestroyBase:
                            break;
                        default:
                            int enterThisIdiot = 0;
                            break;
                    }
                }
            }
        }

        static string WordWrap(string text, SpriteFont font, float width)
        {
            string finalText = "";
            string lineText = "";
            if (font.MeasureString(text).X < width)
            {
                return text;
            }

            string[] words = text.Split(' ');
            finalText = "";
            lineText = words[0];
            for (int i = 1; i < words.Length; i++)
            {
                string tempText = lineText + " " + words[i];
                if (font.MeasureString(tempText).X < width)
                {
                    if (lineText == "")
                        lineText = words[i];
                    else
                        lineText += " " + words[i];
                }
                else
                {
                    if (finalText == "")
                        finalText = lineText + "\n";
                    else
                        finalText += lineText + "\n";
                    lineText = words[i];
                }
            }
            if (lineText != "")
                finalText += lineText;

            return finalText;

        }

    }
}
