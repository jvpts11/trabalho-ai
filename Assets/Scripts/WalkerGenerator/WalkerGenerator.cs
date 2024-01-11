using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WalkerGenerator : MonoBehaviour
{
    
    public enum Grid
    {
        FLOOR,
        WALL,
        EMPTY,
        YELLOWSQUARE,
        GREENSQUARE,
    }

    public Grid[,] gridHandler;

    public List<WalkerObject> walkers;

    public Tilemap tileMap;

    public Tile floor;
    public Tile wall;

    public Tile greenSquare;
    public Tile yellowSquare;

    public int maximumWalkers = 10;

    public int tileCount = default;

    public float fillPercentage = 0.4f;

    public float waitTime = 0.05f;

    public int mapWidth = 30;

    public int mapHeight = 30;

    private Vector2Int greenSquarePosition;
    private Vector2Int yellowSquarePosition;

    private float chanceToSpawnGreenSquare = 0.1f;
    private float chanceToSpawnYellowSquare = 0.1f;

    private bool isGreenSquareSpawned = false;
    private bool isYellowSquareSpawned = false;




    void Start()
    {
        InitializeGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void InitializeGrid()
    {
        gridHandler = new Grid[mapWidth,mapHeight];

        for (int x = 0; x < gridHandler.GetLength(0); x++)
        {
            for (int y = 0; y < gridHandler.GetLength(1); y++)
            {
                gridHandler[x, y] = Grid.EMPTY;
            }
        }

        walkers = new List<WalkerObject>();

        Vector3Int tileCenter = new Vector3Int(gridHandler.GetLength(0) / 2, gridHandler.GetLength(1) / 2, 0);

        WalkerObject currentWalker = new WalkerObject(new Vector2(tileCenter.x,tileCenter.y),GetDirection(),0.5f);

        gridHandler[tileCenter.x, tileCenter.y] = Grid.FLOOR;
        tileMap.SetTile(tileCenter, floor);
        walkers.Add(currentWalker);

        tileCount++;

        StartCoroutine(CreateFloors());
    }

    Vector2 GetDirection()
    {
        int choice = Mathf.FloorToInt(UnityEngine.Random.value * 3.99f);

        switch (choice)
        {
            case 0:
                return Vector2.down;
            case 1:
                return Vector2.left;
            case 2:
                return Vector2.up;
            case 3:
                return Vector2.right;
            default:
                return Vector2.zero;
        }
    }

    IEnumerator CreateFloors()
    {
        while ((float)tileCount / (float)gridHandler.Length < fillPercentage)
        {
            bool hasCreatedFloor = false;
            foreach (WalkerObject curWalker in walkers)
            {
                Vector3Int curPos = new Vector3Int((int)curWalker.position.x, (int)curWalker.position.y, 0);

                if (gridHandler[curPos.x, curPos.y] != Grid.FLOOR)
                {
                    tileMap.SetTile(curPos, floor);
                    tileCount++;
                    gridHandler[curPos.x, curPos.y] = Grid.FLOOR;
                    hasCreatedFloor = true;
                }
            }

            // Walker Methods
            ChanceToRemove();
            ChanceToRedirect();
            ChanceToCreate();
            UpdatePosition();

            if (hasCreatedFloor)
            {
                yield return new WaitForSeconds(waitTime);
            }
        }

        // Após a geração do floor, tente gerar o quadrado verde e amarelo
        TrySpawnGreenSquare();
        TrySpawnYellowSquare();

        StartCoroutine(CreateWalls());
    }



    void ChanceToRemove()
    {
        int updatedCount = walkers.Count;
        for (int i = 0; i < updatedCount; i++)
        {
            if (UnityEngine.Random.value < walkers[i].chanceToChange && walkers.Count > 1)
            {
                walkers.RemoveAt(i);
                break;
            }
        }
    }
    void ChanceToRedirect()
    {
        for (int i = 0; i < walkers.Count; i++)
        {
            if (UnityEngine.Random.value < walkers[i].chanceToChange)
            {
                WalkerObject curWalker = walkers[i];
                curWalker.direction = GetDirection();
                walkers[i] = curWalker;
            }
        }
    }

    void ChanceToCreate()
    {
        int updatedCount = walkers.Count;
        for (int i = 0; i < updatedCount; i++)
        {
            if (UnityEngine.Random.value < walkers[i].chanceToChange && walkers.Count < maximumWalkers)
            {
                Vector2 newDirection = GetDirection();
                Vector2 newposition = walkers[i].position;

                WalkerObject newWalker = new WalkerObject(newposition, newDirection, 0.5f);
                walkers.Add(newWalker);
            }
        }
    }

    void UpdatePosition()
    {
        for (int i = 0; i < walkers.Count; i++)
        {
            WalkerObject FoundWalker = walkers[i];
            FoundWalker.position += FoundWalker.direction;
            FoundWalker.position.x = Mathf.Clamp(FoundWalker.position.x, 1, gridHandler.GetLength(0) - 2);
            FoundWalker.position.y = Mathf.Clamp(FoundWalker.position.y, 1, gridHandler.GetLength(1) - 2);
            walkers[i] = FoundWalker;
        }
    }

    void TrySpawnGreenSquare()
    {
        for(int i = 0;i < gridHandler.GetLength(0); i++)
        {
            for (int j = 0; j < gridHandler.GetLength(0); j++)
            {
                if (gridHandler[i,j] == Grid.FLOOR)
                {
                    if (!isGreenSquareSpawned && UnityEngine.Random.value < chanceToSpawnGreenSquare)
                    {
                        Vector2Int randomPos = (Vector2Int)new Vector3Int(i, j, 0);
                        tileMap.SetTile(new Vector3Int(randomPos.x, randomPos.y, 0), greenSquare);
                        gridHandler[i, j] = Grid.GREENSQUARE;
                        greenSquarePosition = randomPos;
                        isGreenSquareSpawned = true;
                        Debug.Log(isGreenSquareSpawned);
                    }
                }
            }
        }
        
    }

    void TrySpawnYellowSquare()
    {
        for (int i = 0; i < gridHandler.GetLength(0); i++)
        {
            for (int j = 0; j < gridHandler.GetLength(0); j++)
            {
                if (gridHandler[i,j] == Grid.FLOOR && gridHandler[i,j] != Grid.GREENSQUARE)
                {
                    if (isGreenSquareSpawned && !isYellowSquareSpawned && UnityEngine.Random.value < chanceToSpawnYellowSquare)
                    {
                        Vector2Int randomPos = GetRandomFloorPosition();
                        tileMap.SetTile(new Vector3Int(randomPos.x, randomPos.y, 0), yellowSquare);
                        yellowSquarePosition = randomPos;
                        isYellowSquareSpawned = true;
                        Debug.Log(isGreenSquareSpawned);
                    }
                }
            }
        }
    }

    Vector2Int GetRandomFloorPosition()
    {
        Vector2Int randomPos;
        do
        {
            randomPos = new Vector2Int(UnityEngine.Random.Range(1, mapWidth - 1), UnityEngine.Random.Range(1, mapHeight - 1));
        } while (gridHandler[randomPos.x, randomPos.y] != Grid.FLOOR);

        return randomPos;
    }


    bool IsAdjacentToGreenSquare(Vector2Int position)
    {
        if (greenSquarePosition == Vector2Int.zero)
        {
            return false;
        }

        int maxDistance = 2;

        int distance = Mathf.Abs(greenSquarePosition.x - position.x) + Mathf.Abs(greenSquarePosition.y - position.y);

        return distance <= maxDistance;
    }

    IEnumerator CreateWalls()
    {
        for (int x = 0; x < gridHandler.GetLength(0) - 1; x++)
        {
            for (int y = 0; y < gridHandler.GetLength(1) - 1; y++)
            {
                if (gridHandler[x, y] == Grid.FLOOR)
                {
                    bool hasCreatedWall = false;

                    if (gridHandler[x + 1, y] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x + 1, y, 0), wall);
                        gridHandler[x + 1, y] = Grid.WALL;
                        hasCreatedWall = true;
                    }
                    if (gridHandler[x - 1, y] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x - 1, y, 0), wall);
                        gridHandler[x - 1, y] = Grid.WALL;
                        hasCreatedWall = true;
                    }
                    if (gridHandler[x, y + 1] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x, y + 1, 0), wall);
                        gridHandler[x, y + 1] = Grid.WALL;
                        hasCreatedWall = true;
                    }
                    if (gridHandler[x, y - 1] == Grid.EMPTY)
                    {
                        tileMap.SetTile(new Vector3Int(x, y - 1, 0), wall);
                        gridHandler[x, y - 1] = Grid.WALL;
                        hasCreatedWall = true;
                    }

                    if (hasCreatedWall)
                    {
                        yield return new WaitForSeconds(waitTime);
                    }
                }
            }
        }
    }
}
