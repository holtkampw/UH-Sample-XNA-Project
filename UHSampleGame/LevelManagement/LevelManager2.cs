using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UHSampleGame.TileSystem;
using UHSampleGame.Players;

namespace UHSampleGame.LevelManagement
{
    public static class LevelManager2
    {
        static List<Player2> humanPlayers = new List<Player2>();
        static List<Player2> aiPlayers = new List<Player2>();
        static List<Level2> levels;
        public static Level2 CurrentLevel;

        public static void Initialize()
        {
            levels = new List<Level2>();
            InitLevel1();
        }

        public static void AddPlayer(Player2 player)
        {
            if(player.Type == PlayerType.Human)
                humanPlayers.Add(player);
            else
                aiPlayers.Add(player);

            if(aiPlayers.Count >0)
                for (int i = 0; i < humanPlayers.Count; i++)
                    humanPlayers[i].SetTargetBase(aiPlayers[0].PlayerBase);

            if(humanPlayers.Count >0)
                for (int i = 0; i < aiPlayers.Count; i++)
                    aiPlayers[i].SetTargetBase(humanPlayers[0].PlayerBase);
        }

        public static void LoadLevel(int level)
        {
            CurrentLevel = levels[level - 1];
            CurrentLevel.Load();
            for (int i = 0; i < humanPlayers.Count; i++)
                humanPlayers[i].SetTargetBase(aiPlayers[0].PlayerBase);

            for (int i = 0; i < aiPlayers.Count; i++)
                aiPlayers[i].SetTargetBase(humanPlayers[0].PlayerBase);
        }

        private static void InitLevel1()
        {
            List<List<int>> map = new List<List<int>>();
            map.Add(new List<int> { 1100, 0000, 0000, 0000, 0000, 0000, 5510, 0000, 0000, 0000 });
            map.Add(new List<int> { 0000, 1110, 0000, 0000, 0000, 0000, 5510, 0000, 5510, 0000 });
            map.Add(new List<int> { 0000, 0000, 0000, 0000, 0000, 0000, 5510, 0000, 5510, 0000 });
            map.Add(new List<int> { 0000, 0000, 0000, 0000, 0000, 0000, 5510, 0000, 5510, 0000 });
            map.Add(new List<int> { 0000, 0000, 0000, 0000, 0000, 0000, 5510, 0000, 5510, 0000 });
            map.Add(new List<int> { 0000, 0000, 0000, 0000, 0000, 0000, 5510, 0000, 5510, 0000 });
            map.Add(new List<int> { 0000, 0000, 0000, 0000, 0000, 0000, 5510, 0000, 5510, 0000 });
            map.Add(new List<int> { 0000, 0000, 0000, 0000, 0000, 0000, 5510, 0000, 5510, 0000 });
            map.Add(new List<int> { 0000, 0000, 0000, 0000, 0000, 0000, 5510, 0000, 5510, 0000 });
            map.Add(new List<int> { 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000, 5510, 5000 });
            levels.Add(new Level2(1, map, humanPlayers, aiPlayers));
        }
    }
}
