using UnityEngine;

public class ProceduralGridGenerator : MonoBehaviour
{
    public int gridSizeX = 10;
    public int gridSizeY = 10;
    public float obstacleProbability = 0.2f;

    private Vector2Int lastGreenSquarePosition;
    private Vector2Int lastYellowSquarePosition;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        // Crie um array 2D para armazenar os objetos do grid
        GameObject[,] gridObjects = new GameObject[gridSizeX, gridSizeY];

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 spawnPosition = new Vector3(x, y, 0);

                // Adicione bordas com quadrados pretos
                if (x == 0 || x == gridSizeX - 1 || y == 0 || y == gridSizeY - 1)
                {
                    GameObject obstacle = CreateObstacle(spawnPosition);
                    gridObjects[x, y] = obstacle;
                }
                else
                {
                    bool isObstacle = Random.Range(0f, 1f) < obstacleProbability;

                    if (isObstacle)
                    {
                        GameObject obstacle = CreateObstacle(spawnPosition);
                        gridObjects[x, y] = obstacle;
                    }
                    else
                    {
                        GameObject walkableSpace = CreateWalkableSpace(spawnPosition);
                        gridObjects[x, y] = walkableSpace;
                    }
                }
            }
        }

        // Adicione o quadrado verde (posição inicial da IA)
        Vector2Int greenSquarePosition = GetRandomWalkableSpace(gridObjects);
        GameObject greenSquare = CreateGreenSquare(new Vector3(greenSquarePosition.x, greenSquarePosition.y, 0));

        // Adicione o quadrado amarelo (objetivo da IA)
        Vector2Int yellowSquarePosition = GetAdjacentWhiteWalkableSpace(gridObjects);
        GameObject yellowSquare = CreateYellowSquare(new Vector3(yellowSquarePosition.x, yellowSquarePosition.y, 0));
    }

    GameObject CreateObstacle(Vector3 position)
    {
        GameObject obstacle = GameObject.CreatePrimitive(PrimitiveType.Cube);
        obstacle.gameObject.name = "Wall";
        obstacle.transform.position = position;
        obstacle.GetComponent<Renderer>().material.color = Color.black;
        obstacle.transform.parent = transform;
        return obstacle;
    }

    GameObject CreateWalkableSpace(Vector3 position)
    {
        GameObject walkableSpace = GameObject.CreatePrimitive(PrimitiveType.Cube);
        walkableSpace.transform.position = position;
        walkableSpace.gameObject.name = "Floor";
        Renderer renderer = walkableSpace.GetComponent<Renderer>();
        renderer.material = new Material(Shader.Find("Standard"));
        renderer.material.color = Color.white;
        walkableSpace.transform.parent = transform;
        return walkableSpace;
    }

    GameObject CreateGreenSquare(Vector3 position)
    {
        GameObject greenSquare = GameObject.CreatePrimitive(PrimitiveType.Cube);
        greenSquare.gameObject.name =  "GreenSquare";
        greenSquare.transform.position = position;
        greenSquare.GetComponent<Renderer>().material.color = Color.green;
        return greenSquare;
    }

    GameObject CreateYellowSquare(Vector3 position)
    {
        GameObject yellowSquare = GameObject.CreatePrimitive(PrimitiveType.Cube);
        yellowSquare.transform.position = position;

        yellowSquare.gameObject.name = "YellowSquare";

        // Crie um novo material para garantir que a cor seja aplicada corretamente
        Material yellowMaterial = new Material(Shader.Find("Standard"));
        yellowMaterial.color = Color.yellow;

        Renderer renderer = yellowSquare.GetComponent<Renderer>();
        renderer.material = yellowMaterial;
        Debug.Log("Yellow Square Color: " + yellowSquare.GetComponent<Renderer>().material.color);

        return yellowSquare;
    }

    Vector2Int GetRandomWalkableSpace(GameObject[,] gridObjects)
    {
        int maxAttempts = gridSizeX * gridSizeY; // Evitar um loop infinito
        int attempts = 0;
        float minDistance = 2.0f; // Distância mínima entre quadrados verde e amarelo

        while (attempts < maxAttempts)
        {
            Vector2Int randomPosition = new Vector2Int(Random.Range(1, gridSizeX - 1), Random.Range(1, gridSizeY - 1));

            if (gridObjects[randomPosition.x, randomPosition.y].GetComponent<Renderer>().material.color == Color.white &&
                randomPosition != lastGreenSquarePosition &&
                HasAdjacentWhiteWalkableSpace(gridObjects, randomPosition) &&
                Vector2.Distance(new Vector2(randomPosition.x, randomPosition.y), new Vector2(lastYellowSquarePosition.x, lastYellowSquarePosition.y)) > minDistance)
            {
                RemoveSquare(gridObjects, randomPosition);
                lastGreenSquarePosition = randomPosition;
                return randomPosition;
            }

            attempts++;
        }

        Debug.LogError("Não foi possível encontrar espaço válido para o quadrado verde após " + maxAttempts + " tentativas.");
        return Vector2Int.zero;
    }

    Vector2Int GetAdjacentWhiteWalkableSpace(GameObject[,] gridObjects)
    {
        int maxAttempts = gridSizeX * gridSizeY; // Evitar um loop infinito
        int attempts = 0;
        float minDistance = 2.0f; // Distância mínima entre quadrados verde e amarelo

        while (attempts < maxAttempts)
        {
            Vector2Int adjacentPosition = new Vector2Int(Random.Range(1, gridSizeX - 1), Random.Range(1, gridSizeY - 1));

            if (gridObjects[adjacentPosition.x, adjacentPosition.y].GetComponent<Renderer>().material.color == Color.black &&
                adjacentPosition != lastYellowSquarePosition &&
                HasAdjacentWhiteWalkableSpace(gridObjects, adjacentPosition) &&
                Vector2.Distance(new Vector2(adjacentPosition.x, adjacentPosition.y), new Vector2(lastGreenSquarePosition.x, lastGreenSquarePosition.y)) > minDistance)
            {
                RemoveSquare(gridObjects, adjacentPosition);
                lastYellowSquarePosition = adjacentPosition;
                return adjacentPosition;
            }

            attempts++;
        }

        Debug.LogError("Não foi possível encontrar espaço válido para o quadrado amarelo após " + maxAttempts + " tentativas.");
        return Vector2Int.zero;
    }


    void RemoveSquare(GameObject[,] gridObjects, Vector2Int position)
    {
        Destroy(gridObjects[position.x, position.y]);
        gridObjects[position.x, position.y] = null;
    }


    bool HasAdjacentWhiteWalkableSpace(GameObject[,] gridObjects, Vector2Int position)
    {
        // Verifique se há um quadrado branco adjacente
        return gridObjects[position.x - 1, position.y].GetComponent<Renderer>().material.color == Color.white ||
               gridObjects[position.x + 1, position.y].GetComponent<Renderer>().material.color == Color.white ||
               gridObjects[position.x, position.y - 1].GetComponent<Renderer>().material.color == Color.white ||
               gridObjects[position.x, position.y + 1].GetComponent<Renderer>().material.color == Color.white;
    }
}
