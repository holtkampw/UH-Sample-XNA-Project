using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using UHSampleGame.Players;
using UHSampleGame.CoreObjects.Towers;

namespace UHSampleGame.PowerManagement
{

    public enum PowerType { FreezeEnemies, Rezoning, StrongTowers, EMP, Bombastic }

    public struct PowerInformation
    {
        public PowerType type;
        public string name;
        public string description;
        public int cost;
        public string costString;
    }

    public struct PlayerPower
    {
        public int PlayerNum;
        public PowerType Type;
        public int timeLeft;
        public int maxTime;
        public bool active;
    }

    public static class PowerManager
    {
        #region Class Variables
        public static PowerInformation[] powerInformation;
        public static Enum[] powerTypes = EnumHelper.EnumToArray(new PowerType());
        public static int NumPowers = EnumHelper.EnumToArray(new PowerType()).Length;
        static int MAX_ACTIVE_POWERS = 5;
        public static PlayerPower[][] playerPowers;
        public static List<int> actionPlayers = new List<int>(5);
        public static List<int> tempPlayers = new List<int>(5);
        static Random rand;
        #endregion

        #region Intialization
        
        public static void Initialize()
        {
            powerInformation = new PowerInformation[powerTypes.Length];
            powerInformation[0].name = "Freeze Enemies";
            powerInformation[0].type = PowerType.FreezeEnemies;
            powerInformation[0].description = "Temporarily Freezes\nEnemy Units";
            powerInformation[0].cost = 500;
            powerInformation[0].costString = powerInformation[0].cost.ToString();

            powerInformation[1].name = "No Rezoning";
            powerInformation[1].type = PowerType.Rezoning;
            powerInformation[1].description = "Temporarily Disables\nyour enemy's tower\ncreation";
            powerInformation[1].cost = 600;
            powerInformation[1].costString = powerInformation[1].cost.ToString();

            powerInformation[2].name = "Strong Towers";
            powerInformation[2].type = PowerType.StrongTowers;
            powerInformation[2].description = "Temporarily makes\nyour towers\nstrong";
            powerInformation[2].cost = 700;
            powerInformation[2].costString = powerInformation[2].cost.ToString();

            powerInformation[3].name = "EMP";
            powerInformation[3].type = PowerType.EMP;
            powerInformation[3].description = "Temporarily Disables\nEnemy Towers";
            powerInformation[3].cost = 800;
            powerInformation[3].costString = powerInformation[3].cost.ToString();

            powerInformation[4].name = "Bombastic";
            powerInformation[4].type = PowerType.Bombastic;
            powerInformation[4].description = "Explodes some enemy\ntowers";
            powerInformation[4].cost = 1000;
            powerInformation[4].costString = powerInformation[4].cost.ToString();


            playerPowers = new PlayerPower[5][];
            for (int i = 1; i < 5; i++)
                playerPowers[i] = new PlayerPower[MAX_ACTIVE_POWERS];

            rand = new Random((int)DateTime.Now.Ticks);
        }
        #endregion

        #region Update
        public static void Update(GameTime gameTime)
        {
            
            for (int i = 1; i < 5; i++)
            {
                for (int j = 0; j < MAX_ACTIVE_POWERS; j++)
                {
                    if (playerPowers[i][j].active)
                    {
                        playerPowers[i][j].timeLeft += gameTime.ElapsedGameTime.Milliseconds;
                        if (playerPowers[i][j].timeLeft >= playerPowers[i][j].maxTime)
                        {
                            switch(playerPowers[i][j].Type)
                            {
                                case PowerType.StrongTowers:
                                    RemoveStrongTowers(i);
                                    break;
                                case PowerType.Rezoning:
                                    RemoveRezone(i, j);
                                    break;
                                case PowerType.FreezeEnemies:
                                    RemoveFreezeEnemies(i, j);
                                    break;
                                case PowerType.EMP:
                                    RemoveEMP(i, j);
                                    break;
                            }
                            playerPowers[i][j].active = false;
                        }
                    }
                }
            }
        }
        #endregion

        public static void AddPower(PowerType type, int PlayerNum)
        {
            //start effect for player
            for (int i = 0; i < MAX_ACTIVE_POWERS; i++)
            {
                if (!playerPowers[PlayerNum][i].active)
                {
                    if (PlayerCollection.ChargeMoneyForPlayer(PlayerNum, powerInformation[(int)type].cost))
                    {
                        playerPowers[PlayerNum][i].active = true;
                        playerPowers[PlayerNum][i].Type = type;
                        playerPowers[PlayerNum][i].PlayerNum = PlayerNum;

                        switch (type)
                        {
                            case PowerType.StrongTowers:
                                StrongTowers(PlayerNum);
                                playerPowers[PlayerNum][i].maxTime = rand.Next(10000, 30000);
                                playerPowers[PlayerNum][i].timeLeft = 0;
                                break;
                            case PowerType.Rezoning:
                                Rezone(PlayerNum);
                                playerPowers[PlayerNum][i].maxTime = rand.Next(5000, 10000);
                                playerPowers[PlayerNum][i].timeLeft = 0;
                                break;
                            case PowerType.FreezeEnemies:
                                FreezeEnemies(PlayerNum);
                                playerPowers[PlayerNum][i].maxTime = rand.Next(5000, 10000);
                                playerPowers[PlayerNum][i].timeLeft = 0;
                                break;
                            case PowerType.EMP:
                                EMP(PlayerNum);
                                playerPowers[PlayerNum][i].maxTime = rand.Next(5000, 15000);
                                playerPowers[PlayerNum][i].timeLeft = 0;
                                break;
                            case PowerType.Bombastic:
                                Bombastic(PlayerNum);
                                playerPowers[PlayerNum][i].active = false; //instance activation
                                break;
                        }
                    }
                }
            }
        }

        static void StrongTowers(int PlayerNum)
        {
            TowerCollection.SetStrongTowersFor(PlayerNum);
        }

        static void Rezone(int PlayerNum)
        {
            PlayerCollection.SetRezoneFor(PlayerNum);
        }

        static void EMP(int PlayerNum)
        {
            for (int i = 1; i < 5; i++)
            {
                if (i != PlayerNum)
                {
                    PlayerCollection.SetEMPFor(i);
                }
            }
        }

        static void Bombastic(int PlayerNum)
        {
            TowerCollection.UseBombasticOn(PlayerCollection.CheckAttackingPlayerFor(PlayerNum), rand.Next(1, 3));
        }

        static void FreezeEnemies(int PlayerNum)
        {
            PlayerCollection.SetFreezeEnemiesFor(PlayerNum);
        }

        static void RemoveStrongTowers(int PlayerNum)
        {
            TowerCollection.RemoveStrongTowersFor(PlayerNum);
        }

        static void RemoveRezone(int PlayerNum, int index)
        {
            actionPlayers.Clear();
            //Figure out who is currently rezoning
            for (int i = 1; i < 5; i++)
            {
                for (int j = 0; j < MAX_ACTIVE_POWERS; j++)
                {
                    if (playerPowers[i][j].active && (i != PlayerNum && j != index))
                    {
                        if (playerPowers[i][j].Type == PowerType.Rezoning)
                        {
                            actionPlayers.Add(i);
                        }
                    }
                }
            }

            //Get all players who need to stay rezoned
            tempPlayers.Clear();
            for (int i = 0; i < actionPlayers.Count; i++)
            {
                for (int j = 1; j < PlayerNum; j++)
                {
                    if (j != i && !tempPlayers.Contains(j))
                    {
                        tempPlayers.Add(j);
                    }
                }
            }

            //if a player is not listed, then un-rezone them
            for (int i = 1; i < 5; i++)
            {
                if (!tempPlayers.Contains(i))
                {
                    PlayerCollection.RemoveRezoneFor(i);
                }
            }

        }

        static void RemoveEMP(int PlayerNum, int index)
        {
            actionPlayers.Clear();
            //Figure out who is currently EMP'ing
            for (int i = 1; i < 5; i++)
            {
                for (int j = 0; j < MAX_ACTIVE_POWERS; j++)
                {
                    if (playerPowers[i][j].active && (i != PlayerNum && j != index))
                    {
                        if (playerPowers[i][j].Type == PowerType.EMP)
                        {
                            actionPlayers.Add(i);
                        }
                    }
                }
            }

            //Get all players who need to stay EMP'd
            tempPlayers.Clear();
            for (int i = 0; i < actionPlayers.Count; i++)
            {
                for (int j = 1; j < PlayerNum; j++)
                {
                    if (j != i && !tempPlayers.Contains(j))
                    {
                        tempPlayers.Add(j);
                    }
                }
            }

            //if a player is not listed, then un-rezone them
            for (int i = 1; i < 5; i++)
            {
                if (!tempPlayers.Contains(i))
                {
                    PlayerCollection.RemoveEMPFor(i);
                }
            }
        }

        static void RemoveFreezeEnemies(int PlayerNum, int index)
        {
            actionPlayers.Clear();
            //Figure out who is currently freezing
            for (int i = 1; i < 5; i++)
            {
                for (int j = 0; j < MAX_ACTIVE_POWERS; j++)
                {
                    if (playerPowers[i][j].active && (i != PlayerNum && j != index))
                    {
                        if (playerPowers[i][j].Type == PowerType.FreezeEnemies)
                        {
                            actionPlayers.Add(i);
                        }
                    }
                }
            }

            //Get all players who need to stay rezoned
            tempPlayers.Clear();
            for (int i = 0; i < actionPlayers.Count; i++)
            {
                for (int j = 1; j < PlayerNum; j++)
                {
                    if (j != i && !tempPlayers.Contains(j))
                    {
                        tempPlayers.Add(j);
                    }
                }
            }

            //if a player is not listed, then un-rezone them
            for (int i = 1; i < 5; i++)
            {
                if (!tempPlayers.Contains(i))
                {
                    PlayerCollection.RemoveFreezeEnemiesFor(i);
                }
            }
        }
    }
}
