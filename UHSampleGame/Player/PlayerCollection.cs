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
using UHSampleGame.Screens;

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
        public static Player[] Players;
        static bool[] activePlayer;
        static bool[] teamsActive;
        static int numTeamsActive;
        static ScreenManager screenManager;
        static int winTeam;
        #endregion

        public static int NumPlayers
        {
            get
            {
                return Players.Length - 1;
            }
        }


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

            teamsActive = new bool[5];
            for (int i = 0; i < 5; i++)
            {
                teamsActive[i] = false;
            }

            screenManager = (ScreenManager)ScreenManager.Game.Services.GetService(typeof(ScreenManager));
        }

        public static void Update(GameTime gameTime)
        {
            for (int i = 1; i < Players.Length; i++)
            {
                if(activePlayer[i])
                    Players[i].Update(gameTime);
            }
        }

        public static void SetPlayerInactive(int playerNum)
        {
            activePlayer[playerNum] = false;

            CheckGameWin();
        }

        public static void CheckGameWin()
        {
            //Reset Teams
            for (int i = 1; i < 5; i++)
            {
                teamsActive[i] = false;
            }

            //Check players to see who's alive
            for (int p = 1; p < activePlayer.Length; p++)
            {
                if (!Players[p].IsDead)//activePlayer[p])
                    teamsActive[Players[p].TeamNum] = true;
            }

            //Check Win condition
            numTeamsActive = 0;
            for (int i = 1; i < 5; i++)
            {
                if (teamsActive[i])
                {
                    numTeamsActive++;
                    winTeam = i;
                }
            }

            if (numTeamsActive == 1)
            {
                //Show Win Screen
                screenManager.RemoveScreen("PlayScreen");
                screenManager.ShowScreen(new WinScreen(winTeam));
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
            teamsActive[player.TeamNum] = true;
        }

        public static void AttackPlayer(int playerNum)
        {
            Players[playerNum].TakeDamage();
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

        public static bool ShowHUDFor(int playerNum)
        {
            return Players[playerNum].isHUDDisplayed;
        }

        public static void UpdateTargetPlayers(int oldPlayerNum)
        {
            for (int i = 1; i <= NumPlayers; i++)
            {
                if (i == oldPlayerNum)
                    continue;

                if (Players[i].TargetPlayerNum == oldPlayerNum)
                {
                    for (int j = 1; j <= NumPlayers; j++)
                    {
                        if (i != j && j != oldPlayerNum && Players[i].TeamNum != Players[j].TeamNum &&
                           !Players[j].IsDead && activePlayer[j])
                        {
                            Players[i].TargetPlayerNum = j;
                            break;
                        }
                    }
                }
            }
        }


        public static int GetNextTargetFor(int p)
        {
            int currentTarget = Players[p].TargetPlayerNum;
            
            for (int i = 1; i <= NumPlayers; i++)
            {
                if (currentTarget + 1 <= NumPlayers)
                {
                    currentTarget++;
                }
                else
                {
                    currentTarget = 1;
                }

                if (!Players[currentTarget].IsDead && Players[currentTarget].TeamNum != Players[p].TeamNum)
                    return currentTarget;
            }
            
            return currentTarget;
        }
    }
}
