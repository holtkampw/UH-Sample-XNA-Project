using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UHSampleGame.ScreenManagement;
using UHSampleGame.InputManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace UHSampleGame.Screens
{
    public class MultiplayerLobby : Screen
    {
        #region Class Variables
        Texture2D background;

        const int ACTIVE = 1;
        const int IDLE = 0;
        //playerNum, ACTIVE/IDLE
        Texture2D[][] playerIcons;
        bool[] activePlayers;
        Vector2[] playerIconLocations;
        PlayerIndex[] playerIndexes;
        #endregion

        #region Initialization
        public MultiplayerLobby() : base("MultiplayerLobby")
        {

        }

        public override void LoadContent()
        {
            background = ScreenManager.Game.Content.Load<Texture2D>("MultiplayerLobby\\background");
            playerIcons = new Texture2D[5][];
            activePlayers = new bool[5];
            playerIconLocations = new Vector2[5];
            Vector2 currentPosition = new Vector2(40, 230);
            Vector2 offset = new Vector2(300, 0);
            for (int i = 1; i < 5; i++)
            {
                playerIcons[i] = new Texture2D[2];
                playerIcons[i][IDLE] = ScreenManager.Game.Content.Load<Texture2D>("MultiplayerLobby\\mPlayer0" + i + "_idle");
                playerIcons[i][ACTIVE] = ScreenManager.Game.Content.Load<Texture2D>("MultiplayerLobby\\mPlayer0" + i + "_ready");
                activePlayers[i] = false;
                playerIconLocations[i] = currentPosition;
                currentPosition += offset;
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
                    if (activePlayers[i])
                        activePlayers[i] = false;
                    else
                        activePlayers[i] = true;
                }
            }

            if (input.CheckNewAction(InputAction.StartGame))
            {
                //Start Game//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            }

            if (input.CheckNewAction(InputAction.BackToMainMenu))
            {
                ScreenManager.RemoveScreen(this.Name);
            }
        }
        #endregion

        #region Update/Draw
        public override void Update(GameTime gameTime)
        {

        }

        private Texture2D ActiveTexture(int player)
        {
            if (activePlayers[player])
                return playerIcons[player][ACTIVE];
            return playerIcons[player][IDLE];
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
