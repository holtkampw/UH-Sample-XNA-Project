using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UHSampleGame.TileSystem;
using UHSampleGame.Players;
using Microsoft.Xna.Framework;

namespace UHSampleGame.LevelManagement
{
    public enum LevelType
    {
        SingleOne,
        MultiTwo,
        MultiThree,
        MultiFour,
    };

    public static class LevelManager
    {
        //static List<Player> humanPlayers = new List<Player>();
        //static List<Player> aiPlayers = new List<Player>();
        static List<Player> players = new List<Player>();
        static List<Level> levels;
        public static Level CurrentLevel;

        public static void Initialize()
        {
            levels = new List<Level>();
            InitLevel1();
        }

        public static void CreateLevel(Vector2 size, PlayerSetup[] playerSetup)
        {
            List<PlayerSetup> activePlayers = new List<PlayerSetup>();
            for (int i = 0; i < playerSetup.Length; i++)
                if (playerSetup[i].active)
                    activePlayers.Add(playerSetup[i]);

            List<List<int>> map = new List<List<int>>();
            int currentPlayerIndex = 0;
            for (int y = 0; y < (int)size.Y; y++)
            {
                map.Add(new List<int>());
                for (int x = 0; x < (int)size.X; x++)
                {
                    //Case 1: First Player
                    if (x == 0 && y == 0)
                    {
                        currentPlayerIndex = SetupPlayerOnMap(activePlayers, map, currentPlayerIndex, y);
                    }
                    else if (activePlayers.Count >= 3 && x == (int)size.X - 1 && y == 0) //Case 2nd Player if 3+ Players exist
                    {
                        currentPlayerIndex = SetupPlayerOnMap(activePlayers, map, currentPlayerIndex, y);
                    }
                    else if (activePlayers.Count >= 4 && x == 0 && y == (int)size.Y - 1) //Case 4th Player if 4 Players exist
                    {
                        currentPlayerIndex = SetupPlayerOnMap(activePlayers, map, currentPlayerIndex, y);
                    }
                    else if (activePlayers.Count >= 2 && x == (int)size.X - 1 && y == (int)size.Y - 1) //Case: Final Player if 2+ Players exist
                    {
                        currentPlayerIndex = SetupPlayerOnMap(activePlayers, map, currentPlayerIndex, y);
                    }
                    else
                    {
                        map[y].Add(00000);
                    }
                }
            }
            
            levels.Add(new Level(0, map, activePlayers));
            levels[levels.Count - 1].Load();
        }

        private static int SetupPlayerOnMap(List<PlayerSetup> activePlayers, List<List<int>> map, int currentPlayerIndex, int y)
        {
            int playerType = 2;
            if (activePlayers[currentPlayerIndex].type == PlayerType.Human)
            {
                playerType = 1;
            }
            map[y].Add((playerType * 10000)
                + (activePlayers[currentPlayerIndex].playerNum * 1000)
                + (activePlayers[currentPlayerIndex].teamNum * 100));
            currentPlayerIndex++;
            return currentPlayerIndex;
        }

        public static void AddPlayer(Player player)
        {
            players.Add(player);
 
        }

        public static void LoadLevel(int level)
        {
            CurrentLevel = levels[level - 1];
            CurrentLevel.Load();

        }

        private static void InitLevel1()
        {
            List<List<int>> map = new List<List<int>>();
            //PlayerType
            //PlayerNum
            //Team Num
            map.Add(new List<int> { 11100, 00000, 00000, 00000, 00000, 00000, 22510, 00000, 00000, 00000 });
            map.Add(new List<int> { 00000, 11110, 00000, 00000, 00000, 00000, 22510, 00000, 22510, 00000 });
            map.Add(new List<int> { 00000, 00000, 00000, 00000, 00000, 00000, 22510, 00000, 22510, 00000 });
            map.Add(new List<int> { 00000, 00000, 00000, 00000, 00000, 00000, 22510, 00000, 22510, 00000 });
            map.Add(new List<int> { 00000, 00000, 00000, 00000, 00000, 00000, 22510, 00000, 22510, 00000 });
            map.Add(new List<int> { 00000, 00000, 00000, 00000, 00000, 00000, 22510, 00000, 22510, 00000 });
            map.Add(new List<int> { 00000, 00000, 00000, 00000, 00000, 00000, 22510, 00000, 22510, 00000 });
            map.Add(new List<int> { 00000, 00000, 00000, 00000, 00000, 00000, 22510, 00000, 22510, 00000 });
            map.Add(new List<int> { 00000, 00000, 00000, 00000, 00000, 00000, 22510, 00000, 22510, 00000 });
            map.Add(new List<int> { 00000, 00000, 00000, 00000, 00000, 00000, 00000, 00000, 22510, 22000 });
            levels.Add(new Level(1, map, players /*humanPlayers, aiPlayers*/));
        }

    }
}
