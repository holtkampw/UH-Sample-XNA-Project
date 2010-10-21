﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UHSampleGame.TileSystem;
using UHSampleGame.Players;

namespace UHSampleGame.LevelManagement
{
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

        public static void AddPlayer(Player player)
        {
            players.Add(player);
            //if(player.Type == PlayerType.Human)
            //    humanPlayers.Add(player);
            //else
            //    aiPlayers.Add(player);

            //if(aiPlayers.Count >0)
            //    for (int i = 0; i < humanPlayers.Count; i++)
            //        humanPlayers[i].SetTargetBase(aiPlayers[0].PlayerBase);

            //if(humanPlayers.Count >0)
            //    for (int i = 0; i < aiPlayers.Count; i++)
            //        aiPlayers[i].SetTargetBase(humanPlayers[0].PlayerBase);
        }

        public static void LoadLevel(int level)
        {
            CurrentLevel = levels[level - 1];
            CurrentLevel.Load();
            //for (int i = 0; i < humanPlayers.Count; i++)
            //    humanPlayers[i].SetTargetBase(aiPlayers[0].PlayerBase);

            //for (int i = 0; i < aiPlayers.Count; i++)
            //    aiPlayers[i].SetTargetBase(humanPlayers[0].PlayerBase);
        }

        private static void InitLevel1()
        {
            List<List<int>> map = new List<List<int>>();
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
