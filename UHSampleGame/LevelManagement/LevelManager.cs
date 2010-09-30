using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UHSampleGame.TileSystem;
using UHSampleGame.Players;

namespace UHSampleGame.LevelManagement
{
    public class LevelManager
    {
        List<Player> humanPlayers;
        List<Player> aiPlayers;
        List<Level> levels;
        Level currentLevel;

        public Level CurrentLevel
        {
            get { return currentLevel; }
        }

        public LevelManager(List<Player> humanPlayers, List<Player> aiPlayers)
        {
            this.humanPlayers = humanPlayers;
            this.aiPlayers = aiPlayers;

            for (int i = 0; i < humanPlayers.Count; i++)
                humanPlayers[i].SetTargetBase(aiPlayers[0].PlayerBase);

            for (int i = 0; i < aiPlayers.Count; i++)
                aiPlayers[i].SetTargetBase(humanPlayers[0].PlayerBase);

            levels = new List<Level>();
            InitLevel1();
        }

        public void LoadLevel(int level)
        {
            currentLevel = levels[level - 1];
            currentLevel.Load();
            for (int i = 0; i < humanPlayers.Count; i++)
                humanPlayers[i].SetTargetBase(aiPlayers[0].PlayerBase);

            for (int i = 0; i < aiPlayers.Count; i++)
                aiPlayers[i].SetTargetBase(humanPlayers[0].PlayerBase);
        }

        private void InitLevel1()
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
            levels.Add(new Level(1, map, humanPlayers, aiPlayers));
        }
    }
}
