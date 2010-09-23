using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UHSampleGame.TileSystem;

namespace UHSampleGame.LevelManagement
{
    public class LevelManager
    {
        List<Level> levels;
        Level currentLevel;

        public Level CurrentLevel
        {
            get { return currentLevel; }
        }

        public LevelManager()
        {
            levels = new List<Level>();
            InitLevel1();
        }

        public void LoadLevel(int level)
        {
            currentLevel = levels[level - 1];
            currentLevel.Load();

        }

        private void InitLevel1()
        {
            List<List<int>> map = new List<List<int>>();
            map.Add(new List<int>  {1100, 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000 });
            map.Add(new List<int> { 0000, 1110, 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000 });
            map.Add(new List<int> { 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000 });
            map.Add(new List<int> { 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000 });
            map.Add(new List<int> { 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000 });
            map.Add(new List<int> { 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000 });
            map.Add(new List<int> { 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000 });
            map.Add(new List<int> { 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000 });
            map.Add(new List<int> { 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000 });
            map.Add(new List<int> { 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000, 0000, 9000 });
            levels.Add(new Level(1, map));
        }
    }
}
