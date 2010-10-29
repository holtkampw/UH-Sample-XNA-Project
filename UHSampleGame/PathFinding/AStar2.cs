using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UHSampleGame.TileSystem;
using Microsoft.Xna.Framework;

namespace UHSampleGame.PathFinding
{
    public struct TileInformation
    {
        public float gScore;
        public float hScore;
        public float fScore;
        public Vector3 position;
        public int ID;
        public List<int> neighbors;
    }

    public static class AStar2
    {
        static Tile StartTile;
        static Tile GoalTile;

        //static List<bool> closed = new List<bool>();
        //static List<Tile> open = new List<Tile>();
        static List<int> cameFrom = new List<int>();

        //static List<float> gScore = new List<float>();
        //static List<float> hScore = new List<float>();
        //static List<float> fScore = new List<float>();

        static float tentativeGScore;
        static bool tentaiveIsBetter;

        static float lowestScore;
        static Tile returnTile;
        static Tile currentTile;
        static int currentTileID;
        //static List<Tile> neighbors;
        //static Tile neighborTile;

        static TileInformation[] tileInformation;
        static bool[] closed;
        static List<int> open = new List<int>();

        public static void Setup()
        {
            closed = new bool[TileMap.TileCount];

            tileInformation = new TileInformation[TileMap.TileCount];

            for (int i = 0; i < TileMap.TileCount; i++)
            {
                tileInformation[i].position = TileMap.Tiles[i].Position;
                tileInformation[i].ID = TileMap.Tiles[i].ID;
                tileInformation[i].neighbors = TileMap.GetWalkableNeighborsInts(TileMap.Tiles[i]);
                
            }
            
        }

        public static void InitAstar(Tile startTile, Tile goalTile)
        {
            //if (closed.Count < 1)
            //{
            //    for (int i = 0; i < TileMap.TileCount; i++)
            //    {
            //        closed.Add(false);
            //        cameFrom.Add(Tile.NullTile);
            //    }
            //}

            for (int i = 0; i < TileMap.TileCount; i++)
            {
                tileInformation[i].gScore = 100000f;
                tileInformation[i].fScore = 100000f;
                tileInformation[i].hScore = 100000f;
                //open[i] = -1;
                closed[i] = false;
                //cameFrom[i] = -1;
            }
            if (cameFrom.Count < 1)
            {
                for (int i = 0; i < TileMap.TileCount; i++)
                {
                    //gScore.Add(100000f);
                    //hScore.Add(100000f);
                    //fScore.Add(100000f);
                    //closed.Add(false);
                    cameFrom.Add(-1);
                    //cameFrom.Add(Tile.NullTile);
                }
            }
            StartTile = startTile;
            GoalTile = goalTile;

            //closed.Clear();
            //open.Clear();


            for (int i = 0; i < TileMap.TileCount; i++)
            {
                cameFrom[i] = -1;
                //cameFrom[i] = Tile.NullTile;
                //gScore[i] = 100000f;
                //hScore[i] = 100000f;
                //fScore[i] = 100000f;
                //closed[i] = false;
            }

            open.Add(startTile.ID);

            //gScore[startTile.ID] = 0;
            //hScore[startTile.ID] = GetDistanceBetweenTiles(ref StartTile, ref GoalTile);
            //fScore[startTile.ID] = hScore[startTile.ID];

            tileInformation[startTile.ID].gScore = 0;
            tileInformation[startTile.ID].hScore = GetDistanceBetweenTiles(StartTile.ID, GoalTile.ID);//GetDistanceBetweenTiles(ref StartTile, ref GoalTile);
            tileInformation[startTile.ID].fScore = tileInformation[startTile.ID].hScore;
        }

        public static void FindPath(ref List<Tile> path)
        {
            path.Clear();
            if (StartTile.ID == GoalTile.ID)
            {      
                path.Add(GoalTile);
                return;
            }

            int currentTile = 0;
            while (open.Count > 0)
            {
                currentTile = GetLowestFScoreFromOpenSet();
                if (currentTile == GoalTile.ID)
                {
                    ReconstructPath(ref path, GoalTile);
                }
                open.Remove(currentTile);
                closed[currentTile] = true;
                //closed.Add(currentTile);

                //neighbors = TileMap.GetWalkableNeighbors(TileMap.Tiles[currentTile.ID]);
                tileInformation[currentTile].neighbors = TileMap.GetWalkableNeighborsInts(TileMap.Tiles[currentTile]);
                for (int i = 0; i < /*neighbors.Count*/ tileInformation[currentTile].neighbors.Count; i++)
                {
                    if (closed[/*neighbors[i].ID*/  tileInformation[currentTile].neighbors[i]])
                        continue;

                    //neighborTile = neighbors[i];
                    //tentativeGScore = gScore[currentTile.ID]  + GetDistanceBetweenTiles(ref currentTile, ref neighborTile);
                    tentativeGScore = tileInformation[currentTile].gScore + GetDistanceBetweenTiles(currentTile, /*neighborTile.ID*/ tileInformation[currentTile].neighbors[i]);

                    if (!open.Contains(/*neighborTile*//*TileMap.Tiles[tileInformation[currentTile.ID].neighbors[i]]))*/tileInformation[currentTile].neighbors[i]))
                    //if(OpenContains(neighborTile.ID))
                    {
                        open.Add(/*neighborTile*//*TileMap.Tiles[tileInformation[currentTile.ID].neighbors[i]]);*/tileInformation[currentTile].neighbors[i]);
                        //AddOpen(neighborTile.ID);
                        tentaiveIsBetter = true;
                    }
                    else if (tentativeGScore < /*gScore[neighborTile.ID])*/tileInformation[/*neighborTile.ID*/tileInformation[currentTile].neighbors[i]].gScore)
                    {
                        tentaiveIsBetter = true;
                    }
                    else
                        tentaiveIsBetter = false;

                    if (tentaiveIsBetter)
                    {
                       // cameFrom[/*neighborTile.ID*/tileInformation[currentTile].neighbors[i]] = TileMap.Tiles[currentTile];
                        cameFrom[tileInformation[currentTile].neighbors[i]] = TileMap.Tiles[currentTile].ID;
                        tileInformation[/*neighborTile.ID*/tileInformation[currentTile].neighbors[i]].gScore = tentativeGScore;
                        tileInformation[/*neighborTile.ID*/tileInformation[currentTile].neighbors[i]].hScore = GetDistanceBetweenTiles(/*neighborTile.ID*/tileInformation[currentTile].neighbors[i], GoalTile.ID);//GetDistanceBetweenTiles(ref neighborTile, ref GoalTile);
                        tileInformation[/*neighborTile.ID*/tileInformation[currentTile].neighbors[i]].fScore = tileInformation[/*neighborTile.ID*/tileInformation[currentTile].neighbors[i]].gScore + tileInformation[/*neighborTile.ID*/tileInformation[currentTile].neighbors[i]].hScore;
                        //gScore[neighborTile.ID] = tentativeGScore;
                        //hScore[neighborTile.ID] = GetDistanceBetweenTiles(ref neighborTile, ref GoalTile);
                        //fScore[neighborTile.ID] = gScore[neighborTile.ID] + hScore[neighborTile.ID];
                    }
                }
            }

            return;
        }

        static void ReconstructPath(ref List<Tile> path,/* int curTile */Tile curTile)
        {
            if (cameFrom[curTile.ID]!=-1)
            {
                ReconstructPath(ref path, TileMap.Tiles[cameFrom[curTile.ID]]);
                path.Add(curTile);
                //return finalPath;
            }
            else
            {
                //List<Tile> temp = new List<Tile>();
                path.Add(curTile);
                //return temp;
            }
            //if (cameFrom[curTile] != -1)
            //{
            //    ReconstructPath(ref path, cameFrom[curTile]);
            //    path.Add(TileMap.Tiles[curTile]);
            //    //return finalPath;
            //}
            //else
            //{
            //    //List<Tile> temp = new List<Tile>();
            //    path.Add(TileMap.Tiles[curTile]);
            //    //return temp;
            //}
        }

        static /*Tile*/ int GetLowestFScoreFromOpenSet()
        {
            //lowestScore = float.MaxValue;
            //for (int i = 0; i < open.Count; i++)
            //{
            //    if (/*fScore[open[i].ID]*/tileInformation[open[i].ID].fScore < lowestScore)
            //    {
            //        lowestScore = tileInformation[open[i].ID].fScore;//fScore[open[i].ID];
            //        returnTile = open[i];
            //    }
            //}
            //return returnTile;

            //lowestScore = float.MaxValue;
            //for (int i = 0; i < open.Count; i++)
            //{
            //    if (tileInformation[open[i].ID].fScore < lowestScore)
            //    {
            //        lowestScore = tileInformation[open[i].ID].fScore;
            //        returnTile = open[i];//TileMap.Tiles[open[i]];
            //    }
            //}
            //return returnTile;


            lowestScore = float.MaxValue;
            int returnItem = 0;
            for (int i = 0; i < open.Count; i++)
            {
                if (tileInformation[open[i]].fScore < lowestScore)
                {
                    lowestScore = tileInformation[open[i]].fScore;
                    returnItem = open[i];
                }
            }
            return returnItem;
        }

        //static float GetDistanceBetweenTiles(ref Tile tile1, ref Tile tile2)
        static float GetDistanceBetweenTiles(int tile1, int tile2)
        {
            //if (tile1.IsNull() || tile2.IsNull())
            //    return 0;

            //float first = (tile1.Position.X - tile2.Position.X);
            //float second = (tile1.Position.Z - tile2.Position.Z);
            ////return Math.Abs(tile1.Position.X - tile1.Position.Y) + Math.Abs(tile2.Position.X - tile2.Position.Y);
            //return (float)Math.Sqrt((first * first) + (second * second));
            if (tile1 == -1 || tile2 == -1)
                return 0;

            float first = (tileInformation[tile1].position.X - tileInformation[tile2].position.X);
            float second = (tileInformation[tile1].position.Z - tileInformation[tile2].position.Z);
            //return Math.Abs(tile1.Position.X - tile1.Position.Y) + Math.Abs(tile2.Position.X - tile2.Position.Y);
            return (float)Math.Sqrt((first * first) + (second * second));
        }
    }
}
