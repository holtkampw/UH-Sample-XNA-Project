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
        static List<int> cameFrom = new List<int>();

        static float tentativeGScore;
        static bool tentaiveIsBetter;

        static float lowestScore;
        static Tile returnTile;
        static Tile currentTile;
        static int currentTileID;

        static TileInformation[] tileInformation;
        static bool[] closed;
        static List<int> open = new List<int>();

        static TileInformation[] readOnlyTileInformation;
        static bool[] readOnlyClosed;
        static List<int> readOnlyCameFrom = new List<int>();
        static List<int> readOnlyOpen = new List<int>();
        static Tile readOnlyStartTile;
        static Tile readOnlyGoalTile;
        static List<int> readOnlyPath = new List<int>();
        static float readOnlyTentativeGScore;
        static bool readOnlyTentativeIsBetter;
        static float readOnlyLowestScore;
        static Tile readOnlyReturnTile;
        static Tile readOnlyCurrentTile;
        static int readOnlyCurrentTileID;
        static List<int> readOnlyWalkableInts = new List<int>();

        public static bool SetupDone = false;

        public static void Setup()
        {
            closed = new bool[TileMap.TileCount];
            readOnlyClosed = new bool[TileMap.TileCount];

            tileInformation = new TileInformation[TileMap.TileCount];
            readOnlyTileInformation = new TileInformation[TileMap.TileCount];

            for (int i = 0; i < TileMap.TileCount; i++)
            {
                TileInformation tInfo;
                tInfo.gScore = 0;
                tInfo.fScore = 0;
                tInfo.hScore = 0;
                
                tInfo.position = TileMap.Tiles[i].Position;
                tInfo.ID = TileMap.Tiles[i].ID;
                tInfo.neighbors = TileMap.GetWalkableNeighborsInts(TileMap.Tiles[i]);
                tileInformation[i] = tInfo;
                //tileInformation[i].position = TileMap.Tiles[i].Position;
                //tileInformation[i].ID = TileMap.Tiles[i].ID;
                //tileInformation[i].neighbors = TileMap.GetWalkableNeighborsInts(TileMap.Tiles[i]);
                
            }

            Array.Copy(tileInformation, readOnlyTileInformation, tileInformation.Length);
            SetupDone = true;
        }

        public static void UpdateWalkableNeighborsForTileID(int id)
        {
            int index;
            for (int i = 0; i < 8; i++)
            {
                index = TileMap.Tiles[id].tileNeighbors[i].ID;
                if(index >=0)
                    tileInformation[index].neighbors = TileMap.GetWalkableNeighborsInts(TileMap.Tiles[id].tileNeighbors[i]);
            }
        }

        public static void ReadOnlyUpdateWalkableNeighborsForTileID(int id)
        {
            int index;
            for (int i = 0; i < 8; i++)
            {
                index = TileMap.Tiles[id].tileNeighbors[i].ID;
                if (index >= 0)
                    tileInformation[index].neighbors = TileMap.GetWalkableNeighborsInts(TileMap.Tiles[id].tileNeighbors[i]);
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

        public static void FindPath(ref List<int> path)
        {
            path.Clear();
            if (StartTile.ID == GoalTile.ID)
            {      
                path.Add(GoalTile.ID);
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
                //tileInformation[currentTile].neighbors = TileMap.GetWalkableNeighborsInts(TileMap.Tiles[currentTile]);
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

        static void ReconstructPath(ref List<int> path,/* int curTile */Tile curTile)
        {
            if (cameFrom[curTile.ID]!=-1)
            {
                ReconstructPath(ref path, TileMap.Tiles[cameFrom[curTile.ID]]);
                path.Add(curTile.ID);
                //return finalPath;
            }
            else
            {
                //List<Tile> temp = new List<Tile>();
                path.Add(curTile.ID);
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

        static void ReadOnlyReconstructPath(ref List<int> path, Tile curTile)
        {
            if (readOnlyCameFrom[curTile.ID] != -1)
            {
                ReadOnlyReconstructPath(ref path, TileMap.Tiles[readOnlyCameFrom[curTile.ID]]);
                readOnlyPath.Add(curTile.ID);
            }
            else
            {
                readOnlyPath.Add(curTile.ID);
            }
          
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

        static int ReadOnlyGetLowestFScoreFromOpenSet()
        {
            readOnlyLowestScore = float.MaxValue;
            int returnItem = 0;
            for (int i = 0; i < readOnlyOpen.Count; i++)
            {
                if (readOnlyTileInformation[readOnlyOpen[i]].fScore < readOnlyLowestScore)
                {
                    readOnlyLowestScore = readOnlyTileInformation[readOnlyOpen[i]].fScore;
                    returnItem = readOnlyOpen[i];
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

        static float ReadOnlyGetDistanceBetweenTiles(int tile1, int tile2)
        {
            if (tile1 == -1 || tile2 == -1)
                return 0;

            float first = (readOnlyTileInformation[tile1].position.X - readOnlyTileInformation[tile2].position.X);
            float second = (readOnlyTileInformation[tile1].position.Z - readOnlyTileInformation[tile2].position.Z);
            return (float)Math.Sqrt((first * first) + (second * second));
        }

        #region Read Only
        public static void ReadOnlyInit(Tile startTile, Tile goalTile, int blockedTile)
        {
            Array.Copy(tileInformation, readOnlyTileInformation, tileInformation.Length);

            readOnlyWalkableInts = TileMap.GetWalkableNeighborsInts(TileMap.Tiles[blockedTile]);
            for (int i = 0; i < readOnlyWalkableInts.Count; i++)
            {
                readOnlyTileInformation[readOnlyWalkableInts[i]].neighbors.Remove(blockedTile);
            }


            for (int i = 0; i < TileMap.TileCount; i++)
            {
                readOnlyTileInformation[i].gScore = 100000f;
                readOnlyTileInformation[i].fScore = 100000f;
                readOnlyTileInformation[i].hScore = 100000f;
                readOnlyClosed[i] = false;
            }
            
            if (readOnlyCameFrom.Count < 1)
            {
                for (int i = 0; i < TileMap.TileCount; i++)
                {
                    readOnlyCameFrom.Add(-1);
                }
            }
            
            readOnlyStartTile = startTile;
            readOnlyGoalTile = goalTile;

            readOnlyOpen.Add(startTile.ID);

            readOnlyTileInformation[readOnlyStartTile.ID].gScore = 0;
            readOnlyTileInformation[readOnlyStartTile.ID].hScore = ReadOnlyGetDistanceBetweenTiles(readOnlyStartTile.ID, readOnlyGoalTile.ID);
            readOnlyTileInformation[readOnlyStartTile.ID].fScore = readOnlyTileInformation[readOnlyStartTile.ID].hScore;
        }
        #endregion

        public static int GetReadOnlyPathCount()
        {
            readOnlyPath.Clear();
            if (readOnlyStartTile.ID == readOnlyGoalTile.ID)
            {
                readOnlyPath.Add(readOnlyGoalTile.ID);
                return -1;
            }

            int readOnlyCurrentTile = 0;
            while (readOnlyOpen.Count > 0)
            {
                readOnlyCurrentTile = ReadOnlyGetLowestFScoreFromOpenSet();
                if (readOnlyCurrentTile == readOnlyGoalTile.ID)
                {
                    ReadOnlyReconstructPath(ref readOnlyPath, readOnlyGoalTile);
                }
                readOnlyOpen.Remove(readOnlyCurrentTile);
                readOnlyClosed[readOnlyCurrentTile] = true;

                for (int i = 0; i < readOnlyTileInformation[readOnlyCurrentTile].neighbors.Count; i++)
                {
                    if (readOnlyClosed[readOnlyTileInformation[readOnlyCurrentTile].neighbors[i]])
                        continue;


                    readOnlyTentativeGScore = readOnlyTileInformation[readOnlyCurrentTile].gScore + 
                        ReadOnlyGetDistanceBetweenTiles(readOnlyCurrentTile, readOnlyTileInformation[readOnlyCurrentTile].neighbors[i]); 

                    if (!readOnlyOpen.Contains(readOnlyTileInformation[readOnlyCurrentTile].neighbors[i]))
                    {
                        readOnlyOpen.Add(readOnlyTileInformation[readOnlyCurrentTile].neighbors[i]);
                        readOnlyTentativeIsBetter = true;
                    }
                    else if (readOnlyTentativeGScore < readOnlyTileInformation[readOnlyTileInformation[readOnlyCurrentTile].neighbors[i]].gScore)
                    {
                        readOnlyTentativeIsBetter = true;
                    }
                    else
                        readOnlyTentativeIsBetter = false;

                    if (readOnlyTentativeIsBetter)
                    {
                        readOnlyCameFrom[readOnlyTileInformation[readOnlyCurrentTile].neighbors[i]] = TileMap.Tiles[readOnlyCurrentTile].ID;
                        readOnlyTileInformation[readOnlyTileInformation[readOnlyCurrentTile].neighbors[i]].gScore = readOnlyTentativeGScore;
                        readOnlyTileInformation[readOnlyTileInformation[readOnlyCurrentTile].neighbors[i]].hScore =
                            ReadOnlyGetDistanceBetweenTiles(readOnlyTileInformation[readOnlyCurrentTile].neighbors[i], readOnlyGoalTile.ID);
                        readOnlyTileInformation[readOnlyTileInformation[readOnlyCurrentTile].neighbors[i]].fScore =
                            readOnlyTileInformation[readOnlyTileInformation[readOnlyCurrentTile].neighbors[i]].gScore +
                            readOnlyTileInformation[readOnlyTileInformation[readOnlyCurrentTile].neighbors[i]].hScore;
                    }
                }
            }

            return readOnlyPath.Count;
        }
    }
}
