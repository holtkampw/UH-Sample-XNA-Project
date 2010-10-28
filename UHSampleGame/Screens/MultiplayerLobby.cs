using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UHSampleGame.ScreenManagement;
using UHSampleGame.InputManagement;
using UHSampleGame.Players;
using UHSampleGame.LevelManagement;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace UHSampleGame.Screens
{
    public class MultiplayerLobby : Screen
    {
        #region Class Variables
        Texture2D background;

        //playerNum, teamNum
        Texture2D[][] activeIcons;
        Texture2D[] idleIcons;
        Vector2[] playerIconLocations;
        PlayerIndex[] playerIndexes;
        PlayerSetup[] playerSetup;

        char[] numtoCharMapping = {' ', 'A', 'B', 'C', 'D'};

        #endregion

        #region Initialization
        public MultiplayerLobby() : base("MultiplayerLobby")
        {

        }

        public override void LoadContent()
        {
            background = ScreenManager.Game.Content.Load<Texture2D>("MultiplayerLobby\\background");
            activeIcons = new Texture2D[5][];
            idleIcons = new Texture2D[5];
            playerIconLocations = new Vector2[5];
            Vector2 currentPosition = new Vector2(40, 230);
            Vector2 offset = new Vector2(300, 0);
            playerSetup = new PlayerSetup[5];
            for (int i = 1; i < 5; i++)
            {
                activeIcons[i] = new Texture2D[5];
                idleIcons[i] = ScreenManager.Game.Content.Load<Texture2D>("MultiplayerLobby\\mPlayer0" + i + "_idle");
                
                for (int t = 1; t < 5; t++ )
                    activeIcons[i][t] = ScreenManager.Game.Content.Load<Texture2D>("MultiplayerLobby\\mSelect_player0" + i + numtoCharMapping[t]);
                
                playerIconLocations[i] = currentPosition;
                currentPosition += offset;

                playerSetup[i] = new PlayerSetup();
                playerSetup[i].active = false;
                playerSetup[i].playerNum = i;
                playerSetup[i].type = PlayerType.Human;
                playerSetup[i].teamNum = i;
            }

            playerIndexes = new PlayerIndex[5];
            playerIndexes[1] = PlayerIndex.One;
            playerIndexes[2] = PlayerIndex.Two;
            playerIndexes[3] = PlayerIndex.Three;
            playerIndexes[4] = PlayerIndex.Four;
        }

        public override void UnloadContent()
        {
            
        }
        #endregion

        #region Input
        public override void HandleInput(InputManager input)
        {
            for(int i = 1; i < 5; i++)
            {
                if (input.CheckNewAction(InputAction.JoinGame, playerIndexes[i]))
                {
                    if (playerSetup[i].active)
                        playerSetup[i].active = false;
                    else
                        playerSetup[i].active = true;
                }

                if (input.CheckNewAction(InputAction.TeamUp, playerIndexes[i]))
                {
                    if (playerSetup[i].teamNum - 1 >= 1)
                        playerSetup[i].teamNum--;
                    else
                        playerSetup[i].teamNum = 4;
                }

                if (input.CheckNewAction(InputAction.TeamDown, playerIndexes[i]))
                {
                    if (playerSetup[i].teamNum + 1 <= 4)
                        playerSetup[i].teamNum++;
                    else
                        playerSetup[i].teamNum = 1;
                }
            }

            if (input.CheckNewAction(InputAction.BackToMainMenu))
            {
                ScreenManager.RemoveScreen(this.Name);
            }

            if (input.CheckNewAction(InputAction.StartGame))
            {
                int numActivePlayers = 0;
                for (int i = 1; i < playerSetup.Length; i++)
                {
                    if (playerSetup[i].active)
                        numActivePlayers++;
                }

                LevelType levelType = LevelType.MultiTwo;
                switch(numActivePlayers)
                {
                    case 0:
                        return;
                    case 1:
                        return;
                    case 2:
                        levelType = LevelType.MultiTwo;
                        break;
                    case 3:
                        levelType = LevelType.MultiThree;
                        break;
                    case 4:
                        levelType = LevelType.MultiFour;
                        break;
                }
                screenManager.ShowScreen(new LoadScreen(levelType, playerSetup));
            }
        }
        #endregion

        #region Update/Draw
        public override void Update(GameTime gameTime)
        {

        }

        private Texture2D ActiveTexture(int player)
        {
            if (playerSetup[player].active)
                return activeIcons[player][playerSetup[player].teamNum];
            return idleIcons[player];
        }

        public override void Draw(GameTime gameTime)
        {
            ScreenManager.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            ScreenManager.SpriteBatch.Draw(background, Vector2.Zero, Color.White);
            for (int i = 1; i < 5; i++)
            {
                ScreenManager.SpriteBatch.Draw(ActiveTexture(i), playerIconLocations[i], Color.White);
            }
            ScreenManager.SpriteBatch.End();
        }
        #endregion
    }
}
