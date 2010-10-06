using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UHSampleGame.TileSystem;

namespace UHSampleGame.PathFinding
{
    public class AStar2
    {
        class Node
        {
            public Tile2 parentTile;
            public Tile2 Tile2;
            public float overallCost;
            public float currentCost;
            public float goalCost;

            public Node()
                : this(new Tile2()) { }

            public Node(Tile2 Tile2)
                : this(null, Tile2, 0f, 0f, 0f) { }

            public Node(Tile2 parentTile, Tile2 Tile2, float overallCost, float initialCost, float goalCost)
            {
                this.parentTile = parentTile;
                this.Tile2 = Tile2;
                this.overallCost = overallCost;
                this.currentCost = initialCost;
                this.goalCost = goalCost;
            }
        };

        List<Node> openNodes;
        List<Node> closedNodes;

        Tile2 startTile;
        Tile2 goalTile;

        Dictionary<int, Tile2> closedDict;
        Dictionary<int, Tile2> openDict;

        public AStar2(Tile2 startTile, Tile2 goalTile)
        {
            openNodes = new List<Node>();
            closedNodes = new List<Node>();
            openDict = new Dictionary<int, Tile2>();
            closedDict = new Dictionary<int, Tile2>();
            this.startTile = startTile;
            this.goalTile = goalTile;
        }

        public List<Tile2> FindPath()
        {
            if (startTile == goalTile)
            {
                return new List<Tile2>();
            }
            Node startNode = new Node(startTile);
            Node currentNode = startNode;
            List<Tile2> neighborTiles;
            List<Tile2> path = new List<Tile2>();

            //Add current node to open nodes
            openNodes.Add(startNode);
            openDict.Add(startNode.Tile2.ID, startNode.Tile2);

            do
            {
                //Switch the lowest cost node to the closed list
                currentNode = GetLowestCostNodeFromOpenNodes();

                closedNodes.Add(currentNode);
                closedDict.Add(currentNode.Tile2.ID, currentNode.Tile2);
                openNodes.Remove(currentNode);
                openDict.Remove(currentNode.Tile2.ID);

                //Find walkable neighbor tiles not on the closed list

                neighborTiles = GetWalkableNeighborsNotOnClosedList(currentNode);

                //Handle if neighbor node is on the open list already
                //and add to open list
                AddNeighborNodesToOpenList(currentNode, neighborTiles);
            } while (openNodes.Count > 0 && currentNode.Tile2 != goalTile);

            if (openNodes.Count == 0 && currentNode.Tile2 != goalTile)
                return new List<Tile2>();

            path.Add(currentNode.Tile2);
            do
            {
                for (int i = 0; i < closedNodes.Count; i++)
                {
                    if (closedNodes[i].Tile2 == currentNode.parentTile)
                    {
                        currentNode = closedNodes[i];
                        break;
                    }
                }
                //currentNode = closedNodes.Find(new Predicate<Node>(delegate(Node node)
                //{
                //    return node.Tile2 == currentNode.parentTile;
                // }));
                path.Add(currentNode.Tile2);

            } while (currentNode.parentTile != null);

            path.Reverse();

            return path;

        }

        private List<Tile2> GetWalkableNeighborsNotOnClosedList(Node currentNode)
        {
            return TileMap2.GetWalkableNeighbors(currentNode.Tile2, closedDict);
        }

        private void AddNeighborNodesToOpenList(Node currentNode, List<Tile2> neighborTiles)
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
                        if (openNodes[j].Tile2 == neighborTiles[i])
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
                        openNodes[openNodeSimilarIndex] = TransformToNode(openNodes[openNodeSimilarIndex].Tile2, currentNode);
                    }
                }
                else
                {
                    openNodes.Add(neighborNode);
                    openDict.Add(neighborTiles[i].ID, neighborTiles[i]);
                }
            }
        }

        private Node GetLowestCostNodeFromOpenNodes()
        {
            Node lowestCostNode = new Node();
            lowestCostNode.overallCost = float.MaxValue;
            for (int i = 0; i < openNodes.Count; i++)
            {
                if (openNodes[i].overallCost < lowestCostNode.overallCost)
                    lowestCostNode = openNodes[i];
            }
            return lowestCostNode;
        }

        private Node TransformToNode(Tile2 Tile2, Node parentNode)
        {
            Node node = new Node();
            if (Tile2 == null)
            {
                Tile2 = new Tile2();
            }
            node.Tile2 = Tile2;
            node.parentTile = parentNode.Tile2;
            node.currentCost = GetDistanceBetweenTiles(ref Tile2, ref parentNode.Tile2) + parentNode.currentCost;
            node.goalCost = GetDistanceToGoal(ref Tile2);
            node.overallCost = node.currentCost + node.goalCost;

            return node;
        }

        private float GetDistanceToGoal(ref Tile2 Tile2)
        {
            return GetDistanceBetweenTiles(ref Tile2, ref goalTile);
        }

        private float GetDistanceBetweenTiles(ref Tile2 tile1, ref Tile2 tile2)
        {
            float first = (tile1.Position.X - tile2.Position.X);
            float second = (tile1.Position.Z - tile2.Position.Z);
            // return Math.Abs(tile1.Position.X - tile1.Position.Y) + Math.Abs(tile2.Position.X - tile2.Position.Y);
            return (float)Math.Sqrt((first * first) + (second * second));
        }
    }
}
