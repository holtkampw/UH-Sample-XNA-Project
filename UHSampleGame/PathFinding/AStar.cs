﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UHSampleGame.TileSystem;

namespace UHSampleGame.PathFinding
{
    public static class AStar
    {
        class Node
        {
            public Tile parentTile;
            public Tile currentTile;
            public float overallCost;
            public float currentCost;
            public float goalCost;

            public Node()
                : this(Tile.NullTile) { }

            public Node(Tile Tile2)
                : this(Tile.NullTile, Tile2, 0f, 0f, 0f) { }

            public Node(Tile parentTile, Tile Tile2, float overallCost, float initialCost, float goalCost)
            {
                this.parentTile = parentTile;
                this.currentTile = Tile2;
                this.overallCost = overallCost;
                this.currentCost = initialCost;
                this.goalCost = goalCost;
            }

            public static Node NullNode = new Node();
        };

        static Node startNode;
        static Node currentNode;
        static List<Tile> neighborTiles;

        static List<Node> openNodes = new List<Node>();
        static List<Node> closedNodes = new List<Node>();

        static Tile StartTile = new Tile();
        static Tile GoalTile = new Tile();

        static Dictionary<int, Tile> closedDict = new Dictionary<int,Tile>();
        static Dictionary<int, Tile> openDict = new Dictionary<int,Tile>();
        static Dictionary<int, Node> tileNodeDict = new Dictionary<int,Node>();
        
        static Node lowestCostNode = new Node();

        public static void InitAstar(Tile startTile, Tile goalTile)
        {
            openNodes.Clear();
            closedNodes.Clear();
            openDict.Clear();
            closedDict.Clear();
            if (tileNodeDict.Count == 0)
            {
                for (int i = 0; i < TileMap.Tiles.Count; i++)
                {
                    tileNodeDict.Add(TileMap.Tiles[i].ID, new Node(TileMap.Tiles[i]));
                }
            }

            StartTile = startTile;
            GoalTile = goalTile;
        }

        public static void FindPath(ref List<Tile> path)
        {
            path.Clear();
            if (StartTile == GoalTile)
            {
                return;
                //return new List<Tile>();
            }
            startNode = TransformToNode(StartTile, Node.NullNode);// new Node(StartTile);
            currentNode = startNode;
            //List<Tile> neighborTiles;
            //List<Tile> path = new List<Tile>();

            //Add current node to open nodes
            openNodes.Add(startNode);
            openDict.Add(startNode.currentTile.ID, startNode.currentTile);

            do
            {
                //Switch the lowest cost node to the closed list
                currentNode = GetLowestCostNodeFromOpenNodes();

                closedNodes.Add(currentNode);
                closedDict.Add(currentNode.currentTile.ID, currentNode.currentTile);
                openNodes.Remove(currentNode);
                openDict.Remove(currentNode.currentTile.ID);

                //Find walkable neighbor tiles not on the closed list

                neighborTiles = GetWalkableNeighborsNotOnClosedList(currentNode);

                //Handle if neighbor node is on the open list already
                //and add to open list
                AddNeighborNodesToOpenList(currentNode, neighborTiles);
            } while (openNodes.Count > 0 && currentNode.currentTile != GoalTile);


            
            if (openNodes.Count == 0 && currentNode.currentTile != GoalTile)
            {
                return;
                //return new List<Tile>();
            }
                

            path.Add(currentNode.currentTile);
            do
            {
                for (int i = 0; i < closedNodes.Count; i++)
                {
                    if (closedNodes[i].currentTile == currentNode.parentTile)
                    {
                        currentNode = closedNodes[i];
                        break;
                    }
                }
                //currentNode = closedNodes.Find(new Predicate<Node>(delegate(Node node)
                //{
                //    return node.Tile2 == currentNode.parentTile;
                // }));
                path.Add(currentNode.currentTile);

            } while (currentNode.parentTile != Tile.NullTile);

            path.Reverse();

           // return path;

        }

        static List<Tile> GetWalkableNeighborsNotOnClosedList(Node currentNode)
        {
            return TileMap.GetWalkableNeighbors(currentNode.currentTile, closedDict);
        }

        static void AddNeighborNodesToOpenList(Node currentNode, List<Tile> neighborTiles)
        {
            bool openHasNeighbor = false;
            Node neighborNode;
            int openCount = openNodes.Count;
            int openNodeSimilarIndex = 0;
            for (int i = 0; i < neighborTiles.Count; i++)
            {
                openHasNeighbor = false;

                if (openDict.ContainsKey(neighborTiles[i].ID))
                {
                    openHasNeighbor = true;
                    for (int j = 0; j < openCount; j++)
                    {
                        if (openNodes[j].currentTile == neighborTiles[i])
                        {
                            //openHasNeighbor = true;
                            openNodeSimilarIndex = j;
                            break;
                        }
                    }
                    //openNodeSimilarIndex = neighborTiles[i].ID;
                }


                neighborNode = TransformToNode(neighborTiles[i], currentNode);
                if (openHasNeighbor)
                {
                    if (neighborNode.currentCost < openNodes[openNodeSimilarIndex].currentCost)
                    {
                        openNodes[openNodeSimilarIndex] = TransformToNode(openNodes[openNodeSimilarIndex].currentTile, currentNode);
                    }
                }
                else
                {
                    openNodes.Add(neighborNode);
                    openDict.Add(neighborTiles[i].ID, neighborTiles[i]);
                }
            }
        }

        static Node GetLowestCostNodeFromOpenNodes()
        {

            lowestCostNode.overallCost = float.MaxValue;
            for (int i = 0; i < openNodes.Count; i++)
            {
                if (openNodes[i].overallCost < lowestCostNode.overallCost)
                    lowestCostNode = openNodes[i];
            }
            return lowestCostNode;
        }

        static Node TransformToNode(Tile Tile2, Node parentNode)
        {
            Node node = tileNodeDict[Tile2.ID];
            node.currentTile = Tile2;
            node.parentTile = parentNode.currentTile;
            node.currentCost = GetDistanceBetweenTiles(ref Tile2, ref parentNode.currentTile) + parentNode.currentCost;
            node.goalCost = GetDistanceToGoal(ref Tile2);
            node.overallCost = node.currentCost + node.goalCost;

            return node;
        }

        static float GetDistanceToGoal(ref Tile Tile2)
        {
            return GetDistanceBetweenTiles(ref Tile2, ref GoalTile);
        }

        static  float GetDistanceBetweenTiles(ref Tile tile1, ref Tile tile2)
        {
            float first = (tile1.Position.X - tile2.Position.X);
            float second = (tile1.Position.Z - tile2.Position.Z);
            // return Math.Abs(tile1.Position.X - tile1.Position.Y) + Math.Abs(tile2.Position.X - tile2.Position.Y);
            return (float)Math.Sqrt((first * first) + (second * second));
        }
    }
}
