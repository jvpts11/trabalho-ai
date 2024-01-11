using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AStarPathFindingOne : MonoBehaviour
{
    public WalkerGenerator walkerGenerator;

    private PathNode[,] nodes;
    private List<PathNode> openSet;
    private List<PathNode> closedSet;
    public List<PathNode> path;

    /*
    void Start()
    {
        if (walkerGenerator.mapHasBeenGenerated)
        {
            InitializeNodes();
            FindPath();
        }
    }
    */

    public void StartAStarPathfinding()
    {
        InitializeNodes();
        FindPath();
    }

    void Update()
    {

        if (path != null && path.Count > 0)
        {
            foreach (PathNode node in path)
            {
            Vector3Int tilePosition = new Vector3Int(node.x, node.y, 0);
            walkerGenerator.tileMap.SetTile(tilePosition, walkerGenerator.floor);
            }
        }
    }

    public void InitializeNodes()
    {
        Debug.Log("Initializing the nodes!");
        nodes = new PathNode[walkerGenerator.mapWidth, walkerGenerator.mapHeight];

        for (int x = 0; x < walkerGenerator.mapWidth; x++)
        {
            for (int y = 0; y < walkerGenerator.mapHeight; y++)
            {
                WalkerGenerator.Grid gridType = walkerGenerator.gridHandler[x, y];
                PathNode.NodeType nodeType = walkerGenerator.ConvertGridToNodeType(gridType);

                nodes[x, y] = new PathNode(nodeType, x, y, 0, 0, 0);
            }
        }
    }

    public void FindPath()
    {
        PathNode startNode = nodes[walkerGenerator.greenSquarePosition.x, walkerGenerator.greenSquarePosition.y];
        PathNode targetNode = nodes[walkerGenerator.yellowSquarePosition.x, walkerGenerator.yellowSquarePosition.y];

        openSet = new List<PathNode> { startNode };
        closedSet = new List<PathNode>();
        path = new List<PathNode>();

        while (openSet.Count > 0)
        {
            PathNode currentNode = GetLowestFCostNode(openSet);

            if (currentNode == targetNode)
            {
                Debug.Log("Path found");

                RetracePath(startNode, targetNode);
                return;
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            foreach (PathNode neighbor in GetNeighbors(currentNode))
            {
                if (closedSet.Contains(neighbor))
                    continue;

                int tentativeGCost = currentNode.gCost + CalculateDistance(currentNode, neighbor);

                if (!openSet.Contains(neighbor) || tentativeGCost < neighbor.gCost)
                {
                    neighbor.gCost = tentativeGCost;
                    neighbor.hCost = CalculateDistance(neighbor, targetNode);
                    neighbor.fCost = neighbor.gCost + neighbor.hCost;
                    neighbor.nodeBefore = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        Debug.Log("Path did not found");
    }

    public void RetracePath(PathNode startNode, PathNode endNode)
    {
        path.Clear();

        PathNode currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.nodeBefore;
        }

        path.Reverse();
    }

    public PathNode GetLowestFCostNode(List<PathNode> nodeList)
    {
        PathNode lowestNode = nodeList[0];

        foreach (PathNode node in nodeList)
        {
            if (node.fCost < lowestNode.fCost)
                lowestNode = node;
        }

        return lowestNode;
    }

    public List<PathNode> GetNeighbors(PathNode node)
    {
        List<PathNode> neighbors = new List<PathNode>();

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;

                int checkX = node.x + x;
                int checkY = node.y + y;

                if (checkX >= 0 && checkX < walkerGenerator.mapWidth && checkY >= 0 && checkY < walkerGenerator.mapHeight)
                {
                    neighbors.Add(nodes[checkX, checkY]);
                }
            }
        }

        return neighbors;
    }

    public void MoveBlueCircle(List<PathNode> path)
    {
        float moveDelay = 0.2f;

        foreach (PathNode node in path)
        {
            Vector3Int blueCirclePosition = new Vector3Int(node.x, node.y, 0);
            walkerGenerator.tileMap.SetTile(blueCirclePosition, walkerGenerator.blueCircle);
            walkerGenerator.tileMap.SetTile((Vector3Int)walkerGenerator.blueCirclePosition, walkerGenerator.floor);
            walkerGenerator.blueCirclePosition = (Vector2Int)blueCirclePosition;

            StartCoroutine(WaitAndClearTile(blueCirclePosition, moveDelay));
        }
    }

    IEnumerator WaitAndClearTile(Vector3Int position, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        walkerGenerator.tileMap.SetTile(position, walkerGenerator.floor);
    }


    public int CalculateDistance(PathNode nodeA, PathNode nodeB)
    {
        return Mathf.Abs(nodeA.x - nodeB.x) + Mathf.Abs(nodeA.y - nodeB.y);
    }
}
