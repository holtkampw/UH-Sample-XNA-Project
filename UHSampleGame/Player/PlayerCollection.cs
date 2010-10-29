using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UHSampleGame.InputManagement;
using UHSampleGame.TileSystem;
using UHSampleGame.ScreenManagement;
using UHSampleGame.LevelManagement;
using UHSampleGame.CoreObjects.Base;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UHSampleGame.Players
{
    public struct PlayerSetup
    {
        public PlayerType type;
        public int playerNum;
        public int teamNum;
        public bool active;
    }

    public static class PlayerCollection
    {
        #region Class Variables
        static Player[] Players;
        static bool[] activePlayer;
        #endregion
        public static void Initialize()
        {
            
            Players = new Player[5];
            Players[0] = new Player();
            Players[1] = new Player();
            Players[2] = new Player();
            Players[3] = new Player();
            Players[4] = new Player();

            activePlayer = new bool[5];
            activePlayer[0] = false;
            activePlayer[1] = false;
            activePlayer[2] = false;
            activePlayer[3] = false;
            activePlayer[4] = false;
        }

        public static void Update(GameTime gameTime)
        {
            for (int i = 1; i < Players.Length; i++)
            {
                if(activePlayer[i])
                    Players[i].Update(gameTime);
            }
        }

        public static void Draw(GameTime gameTime)
        {
            for (int i = 1; i < Players.Length; i++)
            {
                if (activePlayer[i])
                {
                    Players[i].Draw(gameTime);
                    ResetRenderStates();
                }
            }
        }

        public static void HandleInput(InputManager input)
        {
            for (int i = 1; i < Players.Length; i++)
            {
                if (activePlayer[i])
                    Players[i].HandleInput(input);
            }
        }

        public static void AddPlayer(Player player)
        {
            Players[player.PlayerNum] = player;
            activePlayer[player.PlayerNum] = true;
        }

        public static void SetBaseFor(int playerNum, Base playerBase )
        {
            Players[playerNum].SetBase(playerBase);
        }

        public static void SetTargetFor(int playerNum, int targetNum)
        {
            Players[playerNum].SetTargetBase(Players[targetNum].PlayerBase);
        }

        public static void ResetRenderStates()
        {
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.BlendState = BlendState.AlphaBlend;
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
            ScreenManager.GraphicsDeviceManager.GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
        }

    }
}
