using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AStartTest.Vectors;
using AStartTest.TileSystem;

namespace AStartTest
{
    public partial class Form1 : Form
    {
        List<Panel> nodes;
        Action<Panel> ColorToBlack;
        TileMap tileMap;
        List<TileType> tileTypes;
        AStar aStar = null;
        Random r = new Random();

        List<Tile> open = new List<Tile>();
        List<Tile> closed = new List<Tile>();
        List<Tile> blocked = new List<Tile>();
        Tile current = null;

        double avgTime;// =new TimeSpan();
        int iters = 0;

        public Form1()
        {
            InitializeComponent();
            Vector2 numTiles = new Vector2(24, 24);
            Vector2 tileSize = new Vector2(20, 20);
            nodes = new List<Panel>();
            tileTypes = new List<TileType>();
            tileTypes.Add(TileType.Walkable);
            tileTypes.Add(TileType.Blocked);
            tileTypes.Add(TileType.Start);
            tileTypes.Add(TileType.Goal);

            Panel panel;
            for (int y = 0; y < numTiles.Y; y++)
            {
                for (int x = 0; x < numTiles.X; x++)
                {
                    panel = new Panel();
                    panel.MouseDown += panel33_MouseDown;
                    panel.BorderStyle = BorderStyle.FixedSingle;
                    this.Controls.Add(panel);
                    nodes.Add(panel);
                }
            }
            tileMap = new TileMap(new Vector2(250, 250), numTiles, tileSize, nodes);

            findPathBtn.Enabled = false;
        }

        private void ChangePanelColorToBlack(Panel p)
        {
            p.BackColor = Color.Black;
        }

        private void findPathBtn_Click(object sender, EventArgs e)
        {
            ExecuteAStar();
        }

        private void ExecuteAStar()
        {
            tileMap.ClearPath();
            DateTime time1;
            DateTime time2;

            TimeSpan tSpan = new TimeSpan();
            aStar = new AStar(tileMap);

            time1 = DateTime.Now;
            List<Tile> path = aStar.FindPath();
            time2 = DateTime.Now;

            tSpan = time2.Subtract(time1);
            for (int i = 0; i < path.Count; i++)
            {
                if (path[i].TileType != TileType.Start &&
                    path[i].TileType != TileType.Goal)
                    path[i].SetTileType(TileType.Path);
            }

            avgTime = ((avgTime * iters) + tSpan.TotalMilliseconds) / (iters + 1);
            iters++;
            StringBuilder s = new StringBuilder();
            s.AppendLine("Time: " + tSpan.ToString());
            s.AppendLine("Avg: " + avgTime);
            s.AppendLine("Iters: " + iters);
            timelbl.Text = s.ToString();
        }

        private void panel33_MouseDown(object sender, MouseEventArgs e)
        {
            Tile tile = tileMap.GetTileFromPanel((Panel)sender);

            bool startTile = false;
            bool goalTile = false;

            for (int i = 0; i < tileMap.Tiles.Count; i++)
            {
                if (!startTile && tileMap.Tiles[i].IsStart())
                    startTile = true;
                if (!goalTile && tileMap.Tiles[i].IsGoal())
                    goalTile = true;

                if (startTile && goalTile)
                {
                    findPathBtn.Enabled = true;
                    break;
                }
            }

            if (tile.TileType == TileType.Walkable)
                tile.SetTileType(TileType.Blocked);
            else if (tile.TileType == TileType.Blocked)
            {
                if (startTile)
                {
                    if (goalTile)
                    {
                        tile.SetTileType(TileType.Walkable);
                    }
                    else
                    {
                        tile.SetTileType(TileType.Goal);
                        goalTile = true;
                    }
                }
                else
                {
                    tile.SetTileType(TileType.Start);
                    startTile = true;
                }
            }
            else if (tile.TileType == TileType.Start)
            {
                if (goalTile)
                {
                    tile.SetTileType(TileType.Walkable);
                    goalTile = false;
                }
                else
                {
                    tile.SetTileType(TileType.Goal);
                    goalTile = true;
                }
                startTile = false;
            }
            else if (tile.TileType == TileType.Goal)
            {
                tile.SetTileType(TileType.Walkable);
                goalTile = false;
            }
            else
                tile.SetTileType(TileType.Blocked);


            findPathBtn.Enabled = (startTile && goalTile);
            if (findPathBtn.Enabled)
            {
                //ExecuteAStar();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

            tileMap.ClearPath();
            DateTime time1 = DateTime.Now;
            DateTime time2;
            if (aStar == null)
                aStar = new AStar(tileMap);

            aStar.Iterate(ref current, ref open, ref closed);

            current.SetTileType(TileType.Current);

            for (int i = 0; i < open.Count; i++)
            {
                if (open[i].TileType != TileType.Start &&
                    open[i].TileType != TileType.Goal)
                    open[i].SetTileType(TileType.Open);
            }

            for (int i = 0; i < closed.Count; i++)
            {
                if (closed[i].TileType != TileType.Start &&
                    closed[i].TileType != TileType.Goal)
                    closed[i].SetTileType(TileType.Closed);
            }
            time2 = DateTime.Now;
            timelbl.Text = "Time: " + time2.Subtract(time1).ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            timer1.Enabled = !timer1.Enabled;
            timer1.Interval = 20;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            aStar = new AStar(tileMap);
            List<Tile> path = aStar.FindPath();

            int index = r.Next(path.Count);

            if (path.Count > 0)
            {
                while (path[index].TileType == TileType.Goal || path[index].TileType == TileType.Start ||
                    index == path.Count - 2 || index == 1)
                {
                    index = r.Next(path.Count);
                }
                blocked.Add(path[index]);
                panel33_MouseDown(path[index].Panel, null);
                ExecuteAStar();
            }
            else
            {
                int count = 0;
                int index2;
                int minRemove = blocked.Count / 3;
                while (path.Count == 0 || count < minRemove)
                {
                    index2 = r.Next(blocked.Count);
                    panel33_MouseDown(blocked[index2].Panel, null);
                    blocked.RemoveAt(index2);

                    ExecuteAStar();
                    aStar = new AStar(tileMap);
                    path = aStar.FindPath();

                    count++;

                }

            }

        }
    }
}
