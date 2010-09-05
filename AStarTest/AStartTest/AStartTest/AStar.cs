using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AStartTest.TileSystem;
using AStartTest.Vectors;

namespace AStartTest
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
        TileMap tileMap;

        Tile startTile;
        Tile goalTile;

        public AStar(TileMap tileMap)
        {
            this.tileMap = tileMap;
            openNodes = new List<Node>();
            closedNodes = new List<Node>();
            startTile = tileMap.GetTileFromType(TileType.Start);
            goalTile = tileMap.GetTileFromType(TileType.Goal);
        }

        public void Iterate(ref Tile currentTile, ref List<Tile> open, ref List<Tile> closed)
        {
            Node startNode = new Node(tileMap.GetTileFromType(TileType.Start));
            Node currentNode;
            List<Tile> neighborTiles;
            List<Tile> path = new List<Tile>();
            if (currentTile == null)
                openNodes.Add(startNode);

            //Switch the lowest cost node to the closed list
            currentNode = GetLowestCostNodeFromOpenNodes();
            closedNodes.Add(currentNode);
            openNodes.Remove(currentNode);

            //Find walkable neighbor tiles not on the closed list
            neighborTiles = GetWalkableNeighborsNotOnClosedList(currentNode);

            //Handle if neighbor node is on the open list already
            //and add to open list
            AddNeighborNodesToOpenList(currentNode, neighborTiles);

            currentTile = currentNode.tile;
            List<Tile> newOpen = new List<Tile>();
            List<Tile> newClosed = new List<Tile>();

            for (int i = 0; i < openNodes.Count; i++)
            {
                newOpen.Add(openNodes[i].tile);
            }

            for (int i = 0; i < closedNodes.Count; i++)
            {
                newClosed.Add(closedNodes[i].tile);
            }
            open = newOpen;
            closed = newClosed;
        }

        public List<Tile> FindPath()
        {
            Node startNode = new Node(tileMap.GetTileFromType(TileType.Start));
            Node currentNode = startNode;
            List<Tile> neighborTiles;
            List<Tile> path = new List<Tile>();

            //Add current node to open nodes
            openNodes.Add(startNode);

            do
            {
                //Switch the lowest cost node to the closed list
                currentNode = GetLowestCostNodeFromOpenNodes();
                closedNodes.Add(currentNode);
                openNodes.Remove(currentNode);

                //Find walkable neighbor tiles not on the closed list
                neighborTiles = GetWalkableNeighborsNotOnClosedList(currentNode);

                //Handle if neighbor node is on the open list already
                //and add to open list
                AddNeighborNodesToOpenList(currentNode, neighborTiles);
            } while (openNodes.Count > 0 && currentNode.tile != goalTile);

            path.Add(currentNode.tile);
            do
            {
                currentNode = closedNodes.Find(new Predicate<Node>(delegate(Node node)
                {
                    return node.tile == currentNode.parentTile;
                }));
                path.Add(currentNode.tile);

            } while (currentNode.parentTile != null);

            return path;

        }

        private List<Tile> GetWalkableNeighborsNotOnClosedList(Node currentNode)
        {
            List<Tile> neighborTiles;
            neighborTiles = tileMap.GetWalkableNeighbors(currentNode.tile);
            for (int i = 0; i < closedNodes.Count; i++)
            {
                neighborTiles.Remove(closedNodes[i].tile);
            }
            return neighborTiles;
        }

        private void AddNeighborNodesToOpenList(Node currentNode, List<Tile> neighborTiles)
        {
            bool openHasNeighbor = false;
            Node neighborNode;
            int openNodeSimilarIndex = 0;
            for (int i = 0; i < neighborTiles.Count; i++)
            {
                openHasNeighbor = false;
                for (int j = 0; j < openNodes.Count; j++)
                {
                    if (openNodes[j].tile == neighborTiles[i])
                    {
                        openHasNeighbor = true;
                        openNodeSimilarIndex = j;
                        break;
                    }
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
                    openNodes.Add(TransformToNode(neighborTiles[i], currentNode));
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
            node.tile = tile;
            node.parentTile = parentNode.tile;
            node.currentCost = GetDistanceBetweenTiles(tile, parentNode.tile) + parentNode.currentCost;
            node.goalCost = GetDistanceToGoal(tile);
            node.overallCost = node.currentCost + node.goalCost;

            return node;
        }

        private float GetDistanceToGoal(Tile tile)
        {
            return GetDistanceBetweenTiles(tile, goalTile);
        }

        private float GetDistanceBetweenTiles(Tile tile1, Tile tile2)
        {
            //return Math.Abs(tile1.Position.X - tile1.Position.Y) + Math.Abs(tile2.Position.X - tile2.Position.Y);
            return (float)Math.Sqrt(Math.Pow((double)(tile1.Position.X - tile2.Position.X), 2.0) +
                Math.Pow((double)(tile1.Position.Y - tile2.Position.Y), 2.0));
        }
    }
}
