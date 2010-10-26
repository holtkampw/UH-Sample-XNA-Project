using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UHSampleGame.TileSystem;

namespace UHSampleGame.PathFinding
{
    public static class AStar2
    {
        static Tile StartTile;
        static Tile GoalTile;

        static List<bool> closed = new List<bool>();
        static List<Tile> open = new List<Tile>();
        static List<Tile> cameFrom = new List<Tile>();

        static List<float> gScore = new List<float>();
        static List<float> hScore = new List<float>();
        static List<float> fScore = new List<float>();

        static float tentativeGScore;
        static bool tentaiveIsBetter;

        static float lowestScore;
        static Tile returnTile;
        static Tile currentTile;
        static List<Tile> neighbors;
        static Tile neighborTile;

        static List<Tile> finalPath =  new List<Tile>();
        static List<Tile> emptyPath = new List<Tile>();

        public static void InitAstar(Tile startTile, Tile goalTile)
        {
            if (gScore.Count < 1)
            {
                for (int i = 0; i < TileMap.TileCount; i++)
                {
                    gScore.Add(100000f);
                    hScore.Add(100000f);
                    fScore.Add(100000f);
                    closed.Add(false);
                    cameFrom.Add(Tile.NullTile);
                }
            }
            StartTile = startTile;
            GoalTile = goalTile;

            //closed.Clear();
            open.Clear();
            finalPath.Clear();

            for (int i = 0; i < TileMap.TileCount; i++)
            {
                cameFrom[i] = Tile.NullTile;
                gScore[i] = 100000f;
                hScore[i] = 100000f;
                fScore[i] = 100000f;
                closed[i] = false;
            }

            open.Add(startTile);

            gScore[startTile.ID] = 0;
            hScore[startTile.ID] = GetDistanceBetweenTiles(ref StartTile, ref GoalTile);
            fScore[startTile.ID] = hScore[startTile.ID];
        }

        public static void FindPath(ref List<Tile> path)
        {
            path.Clear();
            if (StartTile.ID == GoalTile.ID)
            {
                //finalPath.Add(GoalTile);
                //return finalPath;
                path.Add(GoalTile);
                return;
            }
                
            while (open.Count > 0)
            {
                currentTile = GetLowestFScoreFromOpenSet();
                if (currentTile.ID == GoalTile.ID)
                {
                    ReconstructPath(ref path, GoalTile);
                
                }
                open.Remove(currentTile);
                closed[currentTile.ID] = true;
                //closed.Add(currentTile);

                neighbors = TileMap.GetWalkableNeighbors(currentTile);
                for (int i = 0; i < neighbors.Count; i++)
                {
                    //if (closed.Contains(neighbors[i]))
                    //    continue;

                    if (closed[neighbors[i].ID])
                        continue;

                    neighborTile = neighbors[i];
                    tentativeGScore = gScore[currentTile.ID] + GetDistanceBetweenTiles(ref currentTile, ref neighborTile);

                    if (!open.Contains(neighborTile))
                    {
                        open.Add(neighborTile);
                        tentaiveIsBetter = true;
                    }
                    else if (tentativeGScore < gScore[neighborTile.ID])
                    {
                        tentaiveIsBetter = true;
                    }
                    else
                        tentaiveIsBetter = false;

                    if (tentaiveIsBetter)
                    {
                        cameFrom[neighborTile.ID] = currentTile;
                        gScore[neighborTile.ID] = tentativeGScore;
                        hScore[neighborTile.ID] = GetDistanceBetweenTiles(ref neighborTile, ref GoalTile);
                        fScore[neighborTile.ID] = gScore[neighborTile.ID] + hScore[neighborTile.ID];
                    }
                }
            }

            return;
        }

        static void ReconstructPath(ref List<Tile> path, Tile curTile)
        {
            if (!cameFrom[curTile.ID].IsNull())
            {
                ReconstructPath(ref path, cameFrom[curTile.ID]);
                path.Add(curTile);
                //return finalPath;
            }
            else
            {
                //List<Tile> temp = new List<Tile>();
                path.Add(curTile);
                //return temp;
            }
        }

        static Tile GetLowestFScoreFromOpenSet()
        {
            lowestScore = float.MaxValue;
            for (int i = 0; i < open.Count; i++)
            {
                if (fScore[open[i].ID] < lowestScore)
                {
                    lowestScore = fScore[open[i].ID];
                    returnTile = open[i];
                }
            }
            return returnTile;
        }

        static float GetDistanceBetweenTiles(ref Tile tile1, ref Tile tile2)
        {
            if (tile1.IsNull() || tile2.IsNull())
                return 0;

            float first = (tile1.Position.X - tile2.Position.X);
            float second = (tile1.Position.Z - tile2.Position.Z);
            //return Math.Abs(tile1.Position.X - tile1.Position.Y) + Math.Abs(tile2.Position.X - tile2.Position.Y);
            return (float)Math.Sqrt((first * first) + (second * second));
        }
    }
}
