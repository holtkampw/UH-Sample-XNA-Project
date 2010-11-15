using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UHSampleGame.TileSystem;
using Microsoft.Xna.Framework;
using UHSampleGame.CoreObjects.Towers;

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
        static int StartTileID;
        static int GoalTileID;
        static List<int> cameFrom = new List<int>(TileMap.Tiles.Count);

        static float tentativeGScore;
        static bool tentaiveIsBetter;

        static float lowestScore;

        static TileInformation[] tileInformation;
        static bool[] closed;
        static bool[] openContains;
        static List<int> open = new List<int>(TileMap.Tiles.Count);

        static TileInformation[] readOnlyTileInformation;
        static bool[] readOnlyClosed;
        static bool[] readOnlyOpenContains;
        static List<int> readOnlyCameFrom = new List<int>(TileMap.Tiles.Count);
        static List<int> readOnlyOpen = new List<int>(TileMap.Tiles.Count);
        static int readOnlyStartTileID;
        static int readOnlyGoalTileID;
        static List<int> readOnlyPath = new List<int>(TileMap.Tiles.Count);
        static float readOnlyTentativeGScore;
        static bool readOnlyTentativeIsBetter;
        static float readOnlyLowestScore;

        static List<int> readOnlyWalkableInts = new List<int>(TileMap.Tiles.Count);
        public static bool SetupDone = false;

        public static object towerLock = new object();

        public static object[][] locks;
        public static object tileInformationLock = new object();
        public static bool UpdateNeeded = false;

        public static void Setup()
        {
            openContains = new bool[TileMap.TileCount];
            closed = new bool[TileMap.TileCount];
            readOnlyClosed = new bool[TileMap.TileCount];
            readOnlyOpenContains = new bool[TileMap.TileCount];

            tileInformation = new TileInformation[TileMap.TileCount];
            readOnlyTileInformation = new TileInformation[TileMap.TileCount];
            locks = new object[TileMap.TileCount][];

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

                TileInformation rTInfo;
                rTInfo.gScore = 0;
                rTInfo.fScore = 0;
                rTInfo.hScore = 0;

                rTInfo.position = TileMap.Tiles[i].Position;
                rTInfo.ID = TileMap.Tiles[i].ID;
                rTInfo.neighbors = new List<int>(/*TileMap.GetWalkableNeighborsInts(TileMap.Tiles[i])*/8);
                for (int j = 0; j < tInfo.neighbors.Count; j++)
                {
                    rTInfo.neighbors.Add(tInfo.neighbors[j]);
                }
                readOnlyTileInformation[i] = rTInfo;

                locks[i] = new object[TileMap.TileCount];
                for (int l = 0; l < TileMap.TileCount; l++)
                {
                    locks[i][l] = new object();
                }

            }
            SetupDone = true;
        }

        public static void UpdateWalkableNeighborsForTileID(int id) //REVISIT WHO CALLS THIS
        {
            int index;
            for (int i = 0; i < 8; i++)
            {
                index = TileMap.Tiles[id].tileNeighbors[i].ID;
                if (index >= 0)
                {
                    lock (tileInformation)
                    {
                        tileInformation[index].neighbors = TileMap.GetWalkableNeighborsInts(TileMap.Tiles[id].tileNeighbors[i]);
                    }
                }
            }
        
        }


        public static void InitAstar(int startTileID, int goalTileID)
        {
            for (int i = 0; i < TileMap.TileCount; i++)
            {
                tileInformation[i].gScore = 100000f;
                tileInformation[i].fScore = 100000f;
                tileInformation[i].hScore = 100000f;
                //open[i] = -1;
                closed[i] = false;
                openContains[i] = false;
                //cameFrom[i] = -1;
            }

            if (cameFrom.Count < 1)
            {
                for (int i = 0; i < TileMap.TileCount; i++)
                {
                    cameFrom.Add(-1);
                }
            }
            StartTileID = startTileID;
            GoalTileID = goalTileID;

            for (int i = 0; i < TileMap.TileCount; i++)
            {
                cameFrom[i] = -1;
                tileInformation[i].gScore = 100000f;
                tileInformation[i].fScore = 100000f;
                tileInformation[i].hScore = 100000f;
                //open[i] = -1;
                closed[i] = false;
                //cameFrom[i] = -1;
            }

            open.Add(startTileID);
            openContains[startTileID] = true;

            tileInformation[startTileID].gScore = 0;
            tileInformation[startTileID].hScore = GetDistanceBetweenTiles(StartTileID, GoalTileID);//GetDistanceBetweenTiles(ref StartTile, ref GoalTile);
            tileInformation[startTileID].fScore = tileInformation[startTileID].hScore;
            
        }

        public static void FindPath(ref List<int> path)
        {
            path.Clear();
            if (StartTileID == GoalTileID)
            {
                path.Add(GoalTileID);
                return;
            }

            int currentTile = 0;
            while (open.Count > 0)
            {
                currentTile = GetLowestFScoreFromOpenSet();
                if (currentTile == GoalTileID)
                {
                    ReconstructPath(ref path, GoalTileID);
                }
                open.Remove(currentTile);
                openContains[currentTile] = false;
                closed[currentTile] = true;
                //closed.Add(currentTile);

                //neighbors = TileMap.GetWalkableNeighbors(TileMap.Tiles[currentTile.ID]);

                lock (tileInformation)
                {
                    tileInformation[currentTile].neighbors = TileMap.GetWalkableNeighborsInts(TileMap.Tiles[currentTile]);
                }


                for (int i = 0; i < /*neighbors.Count*/ tileInformation[currentTile].neighbors.Count; i++)
                {
                    if (closed[/*neighbors[i].ID*/  tileInformation[currentTile].neighbors[i]])
                        continue;

                    //neighborTile = neighbors[i];
                    //tentativeGScore = gScore[currentTile.ID]  + GetDistanceBetweenTiles(ref currentTile, ref neighborTile);
                    tentativeGScore = tileInformation[currentTile].gScore + GetDistanceBetweenTiles(currentTile, /*neighborTile.ID*/ tileInformation[currentTile].neighbors[i]);

                    if (!openContains[tileInformation[currentTile].neighbors[i]])//!open.Contains(/*neighborTile*//*TileMap.Tiles[tileInformation[currentTile.ID].neighbors[i]]))*/tileInformation[currentTile].neighbors[i]))
                    //if(OpenContains(neighborTile.ID))
                    {
                        open.Add(/*neighborTile*//*TileMap.Tiles[tileInformation[currentTile.ID].neighbors[i]]);*/tileInformation[currentTile].neighbors[i]);
                        //AddOpen(neighborTile.ID);
                        openContains[tileInformation[currentTile].neighbors[i]] = true;
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
                        tileInformation[/*neighborTile.ID*/tileInformation[currentTile].neighbors[i]].hScore = GetDistanceBetweenTiles(/*neighborTile.ID*/tileInformation[currentTile].neighbors[i], GoalTileID);//GetDistanceBetweenTiles(ref neighborTile, ref GoalTile);
                        tileInformation[/*neighborTile.ID*/tileInformation[currentTile].neighbors[i]].fScore = tileInformation[/*neighborTile.ID*/tileInformation[currentTile].neighbors[i]].gScore + tileInformation[/*neighborTile.ID*/tileInformation[currentTile].neighbors[i]].hScore;
                        //gScore[neighborTile.ID] = tentativeGScore;
                        //hScore[neighborTile.ID] = GetDistanceBetweenTiles(ref neighborTile, ref GoalTile);
                        //fScore[neighborTile.ID] = gScore[neighborTile.ID] + hScore[neighborTile.ID];
                    }
                }
            }

            return;
        }

        static void ReconstructPath(ref List<int> path,/* int curTile */int curTileID)
        {
            if (cameFrom[curTileID] != -1)
            {
                ReconstructPath(ref path, TileMap.Tiles[cameFrom[curTileID]].ID);
                path.Add(curTileID);
                //return finalPath;
            }
            else
            {
                //List<Tile> temp = new List<Tile>();
                path.Add(curTileID);
                //return temp;
            }

        }

        static void ReadOnlyReconstructPath(ref List<int> path, int curTileID)
        {
            if (readOnlyCameFrom[curTileID] != -1)
            {
                ReadOnlyReconstructPath(ref path, TileMap.Tiles[readOnlyCameFrom[curTileID]].ID);
                readOnlyPath.Add(curTileID);
            }
            else
            {
                readOnlyPath.Add(curTileID);
            }

        }

        static int GetLowestFScoreFromOpenSet()
        {
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

            float result = 0;

                float first = (tileInformation[tile1].position.X - tileInformation[tile2].position.X);
                float second = (tileInformation[tile1].position.Z - tileInformation[tile2].position.Z);
                //return Math.Abs(tile1.Position.X - tile1.Position.Y) + Math.Abs(tile2.Position.X - tile2.Position.Y);
                result = (float)Math.Sqrt((first * first) + (second * second));
                return result;

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

        public static void ReadOnlyInit(int startTileID, int goalTileID, int blockedTile)
        {
            lock (tileInformationLock)
            {
                for (int i = 0; i < tileInformation.Length; i++)
                {
                    readOnlyTileInformation[i].ID = tileInformation[i].ID;
                    readOnlyTileInformation[i].position = tileInformation[i].position;
                    readOnlyTileInformation[i].neighbors.Clear();

                    for (int j = 0; j < tileInformation[i].neighbors.Count; j++)
                        readOnlyTileInformation[i].neighbors.Add(tileInformation[i].neighbors[j]);
                }
            }

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
                readOnlyOpenContains[i] = false;
            }

            if (readOnlyCameFrom.Count < 1)
            {
                for (int i = 0; i < TileMap.TileCount; i++)
                {
                    readOnlyCameFrom.Add(-1);
                }
            }

            for (int i = 0; i < TileMap.TileCount; i++)
            {
                readOnlyCameFrom[i] = -1;
            }

            readOnlyStartTileID = startTileID;
            readOnlyGoalTileID = goalTileID;

            readOnlyOpen.Add(startTileID);
            readOnlyOpenContains[startTileID] = true;

            readOnlyTileInformation[readOnlyStartTileID].gScore = 0;
            readOnlyTileInformation[readOnlyStartTileID].hScore = ReadOnlyGetDistanceBetweenTiles(readOnlyStartTileID, readOnlyGoalTileID);
            readOnlyTileInformation[readOnlyStartTileID].fScore = readOnlyTileInformation[readOnlyStartTileID].hScore;
        }
        #endregion

        public static int GetReadOnlyPathCount()
        {
            readOnlyPath.Clear();
            if (readOnlyStartTileID == readOnlyGoalTileID)
            {
                readOnlyPath.Add(readOnlyGoalTileID);
                return -1;
            }

            int readOnlyCurrentTile = 0;
            while (readOnlyOpen.Count > 0)
            {
                readOnlyCurrentTile = ReadOnlyGetLowestFScoreFromOpenSet();
                if (readOnlyCurrentTile == readOnlyGoalTileID)
                {
                    ReadOnlyReconstructPath(ref readOnlyPath, readOnlyGoalTileID);
                }
                readOnlyOpen.Remove(readOnlyCurrentTile);
                readOnlyOpenContains[readOnlyCurrentTile] = false;
                readOnlyClosed[readOnlyCurrentTile] = true;

                for (int i = 0; i < readOnlyTileInformation[readOnlyCurrentTile].neighbors.Count; i++)
                {
                    if (readOnlyClosed[readOnlyTileInformation[readOnlyCurrentTile].neighbors[i]])
                        continue;


                    readOnlyTentativeGScore = readOnlyTileInformation[readOnlyCurrentTile].gScore +
                        ReadOnlyGetDistanceBetweenTiles(readOnlyCurrentTile, readOnlyTileInformation[readOnlyCurrentTile].neighbors[i]);

                    if (!readOnlyOpenContains[readOnlyTileInformation[readOnlyCurrentTile].neighbors[i]])//!readOnlyOpen.Contains(readOnlyTileInformation[readOnlyCurrentTile].neighbors[i]))
                    {
                        readOnlyOpen.Add(readOnlyTileInformation[readOnlyCurrentTile].neighbors[i]);
                        readOnlyOpenContains[readOnlyTileInformation[readOnlyCurrentTile].neighbors[i]] = true;
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
                            ReadOnlyGetDistanceBetweenTiles(readOnlyTileInformation[readOnlyCurrentTile].neighbors[i], readOnlyGoalTileID);
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
