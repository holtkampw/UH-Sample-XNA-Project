using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UHSampleGame.TileSystem;

namespace UHSampleGame.PathFinding
{
    public class AStar
    {
        class Node
        {
            public Tile parentTile;
            public Tile tile;
            public float overallCost;
            public float currentCost;
            public float goalCost;

            public Node()
                : this(new Tile()) { }

            public Node(Tile tile)
                : this(null, tile, 0f, 0f, 0f) { }

            public Node(Tile parentTile, Tile tile, float overallCost, float initialCost, float goalCost)
            {
                this.parentTile = parentTile;
                this.tile = tile;
                this.overallCost = overallCost;
                this.currentCost = initialCost;
                this.goalCost = goalCost;
            }
        };

        List<Node> openNodes;
        List<Node> closedNodes;

        Tile startTile;
        Tile goalTile;

        Dictionary<int, Tile> closedDict;
        Dictionary<int, Tile> openDict;

        public AStar(Tile startTile, Tile goalTile)
        {
            openNodes = new List<Node>();
            closedNodes = new List<Node>();
            openDict = new Dictionary<int, Tile>();
            closedDict = new Dictionary<int, Tile>();
            this.startTile = startTile;
            this.goalTile = goalTile;
        }

        public List<Tile> FindPath()
        {
            if (startTile == goalTile)
            {
                return new List<Tile>();
            }
            Node startNode = new Node(startTile);
            Node currentNode = startNode;
            List<Tile> neighborTiles;
            List<Tile> path = new List<Tile>();

            //Add current node to open nodes
            openNodes.Add(startNode);
            openDict.Add(startNode.tile.ID, startNode.tile);

            do
            {
                //Switch the lowest cost node to the closed list
                currentNode = GetLowestCostNodeFromOpenNodes();

                closedNodes.Add(currentNode);
                closedDict.Add(currentNode.tile.ID, currentNode.tile);
                openNodes.Remove(currentNode);
                openDict.Remove(currentNode.tile.ID);

                //Find walkable neighbor tiles not on the closed list

                neighborTiles = GetWalkableNeighborsNotOnClosedList(currentNode);

                //Handle if neighbor node is on the open list already
                //and add to open list
                AddNeighborNodesToOpenList(currentNode, neighborTiles);
            } while (openNodes.Count > 0 && currentNode.tile != goalTile);

            if (openNodes.Count == 0 && currentNode.tile != goalTile)
                return new List<Tile>();

            path.Add(currentNode.tile);
            do
            {
                for (int i = 0; i < closedNodes.Count; i++)
                {
                    if (closedNodes[i].tile == currentNode.parentTile)
                    {
                        currentNode = closedNodes[i];
                        break;
                    }
                }
                //currentNode = closedNodes.Find(new Predicate<Node>(delegate(Node node)
                //{
                //    return node.tile == currentNode.parentTile;
               // }));
                path.Add(currentNode.tile);

            } while (currentNode.parentTile != null);

            path.Reverse();

            return path;

        }

        private List<Tile> GetWalkableNeighborsNotOnClosedList(Node currentNode)
        {
            return TileMap.GetWalkableNeighbors(currentNode.tile, closedDict);
        }

        private void AddNeighborNodesToOpenList(Node currentNode, List<Tile> neighborTiles)
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
                        if (openNodes[j].tile == neighborTiles[i])
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
                        openNodes[openNodeSimilarIndex] = TransformToNode(openNodes[openNodeSimilarIndex].tile, currentNode);
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

        private Node TransformToNode(Tile tile, Node parentNode)
        {
            Node node = new Node();
            if (tile == null)
            {
                tile = new Tile();
            }
            node.tile = tile;
            node.parentTile = parentNode.tile;
            node.currentCost = GetDistanceBetweenTiles(ref tile, ref parentNode.tile) + parentNode.currentCost;
            node.goalCost = GetDistanceToGoal(ref tile);
            node.overallCost = node.currentCost + node.goalCost;

            return node;
        }

        private float GetDistanceToGoal(ref Tile tile)
        {
            return GetDistanceBetweenTiles(ref tile, ref goalTile);
        }

        private float GetDistanceBetweenTiles(ref Tile tile1, ref Tile tile2)
        {
            float first = (tile1.Position.X - tile2.Position.X);
            float second = (tile1.Position.Z - tile2.Position.Z);
            // return Math.Abs(tile1.Position.X - tile1.Position.Y) + Math.Abs(tile2.Position.X - tile2.Position.Y);
            return (float)Math.Sqrt((first * first) + (second * second));
        }
    }
}
