using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    public static MazeGenerator Instance;

    [Header("�̷� ����")]
    public int width = 10;
    public int height = 10;
    public GameObject cellPrefab;
    public float cellSize = 2f;

    [Header("�ð�ȭ ����")]
    public bool visualizeGeneration = false;                    // ���� ���� ����
    public float visualizationSpeed = 0.05f;                    // �ӵ�
    public Color visitedColor = Color.cyan;                     // �湮�� ĭ ����
    public Color currentColor = Color.yellow;                   // ���� ĭ ����
    public Color backtrackColor = Color.magenta;                // �ڷ� ���� ����

    private MazeCell[,] maze;
    private Stack<MazeCell> cellStack;

    void Start()
    {
        GenerateMaze();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GenerateMaze()
    {
        maze = new MazeCell[width, height];
        cellStack = new Stack<MazeCell>();

        CreateCells();                                  // ��� �� ����

        if(visualizeGeneration)
        {
            StartCoroutine(GenerateWithDFSVisualized());
        }
        else
        {
            GenerateWithDFS();
        }
    }

    void GenerateWithDFS()                              // DFS �˰������� �̷� ����
    {
        MazeCell current = maze[0, 0];      
        current.visited = true;
        cellStack.Push(current);                        // ù ��° ĭ�� ���ÿ� �ִ´�.

        while(cellStack.Count > 0)
        {
            current = cellStack.Peek();

            // �湮���� ���� �̿� ã��
            List<MazeCell> unvisitedNeighbors = GetUnvisitedNeighbors(current); 

            if(unvisitedNeighbors.Count > 0 )
            {
                // �����ϰ� �̿� ����
                MazeCell next = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];  // �����ϰ� �̿� ����
                RemoveWallBetween(current, next);           // �� ����
                next.visited = true;
                cellStack.Push(next);
            }
            else
            {
                cellStack.Pop();                            // �� Ʈ��ŷ
            }
        }
    }

    void CreateCells()              // �� ���� �Լ�
    {
        if(cellPrefab == null)
        {
            Debug.LogError("�� �������� ����");
            return;
        }

        for(int x = 0; x < width; x++)
        {
            for(int z  = 0; z < height; z++)
            {
                Vector3 pos = new Vector3(x * cellSize, 0, z * cellSize);
                GameObject cellObj = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                cellObj.name = $"Cell_{x}_{z}";

                MazeCell cell = cellObj.GetComponent<MazeCell>();
                if(cell == null)
                {
                    Debug.LogError("MazeCell ��ũ��Ʈ ����");
                    return;
                }

                cell.Initialize(x, z);
                maze[x, z] = cell;
            }
        }
    }

    List<MazeCell> GetUnvisitedNeighbors(MazeCell cell)                     // �湮���� ���� �̿� ã��
    {
        List<MazeCell> neighbors = new List<MazeCell>();

        // �����¿� üũ
        if(cell.x > 0 && !maze[cell.x - 1, cell.z].visited)
            neighbors.Add(maze[cell.x - 1, cell.z]);

        if (cell.x < width - 1 && !maze[cell.x + 1, cell.z].visited)
            neighbors.Add(maze[cell.x + 1, cell.z]);

        if (cell.z > 0 && !maze[cell.x, cell.z - 1].visited)
            neighbors.Add(maze[cell.x, cell.z - 1]);

        if (cell.z < height - 1 && !maze[cell.x, cell.z + 1].visited)
            neighbors.Add(maze[cell.x, cell.z + 1]);

        return neighbors;
    }

    void RemoveWallBetween(MazeCell current, MazeCell next)                 // �� �� ������ �� ����
    {
        if(current.x < next.x)              // ������
        {
            current.RemoveWall("right");
            next.RemoveWall("left");
        }
        else if (current.x > next.x)        // ����
        {
            current.RemoveWall("left");
            next.RemoveWall("right");
        }
        else if (current.z < next.z)        // ��
        {
            current.RemoveWall("top");
            next.RemoveWall("bottom");
        }
        else if (current.z > next.z)        // �Ʒ�
        {
            current.RemoveWall("bottom");
            next.RemoveWall("top");
        }
    }

    // Ư�� ��ġ�� �� ��������
    public MazeCell GetCell(int x, int z)
    {
        if (x >= 0 && x < width && z >= 0 && z < height)
            return maze[x, z];

        return null;
    }

    IEnumerator GenerateWithDFSVisualized()
    {
        MazeCell current = maze[0, 0];
        current.visited = true;

        current.SetColor(currentColor);                                     // �ð�ȭ �߰�
        cellStack.Clear();                                                  // �ð�ȭ �߰�

        cellStack.Push(current);                        // ù ��° ĭ�� ���ÿ� �ִ´�.

        yield return new WaitForSeconds(visualizationSpeed);                // �ð�ȭ �߰�

        int totalCells = width * height;                                    // �ð�ȭ �߰�
        int visitedCount = 1;                                               // �ð�ȭ �߰�

        while (cellStack.Count > 0)
        {
            current = cellStack.Peek();
            current.SetColor(currentColor);                                 // �ð�ȭ �߰� (���� ĭ ����)
            yield return new WaitForSeconds(visualizationSpeed);            // �ð�ȭ �߰�

            // �湮���� ���� �̿� ã��
            List<MazeCell> unvisitedNeighbors = GetUnvisitedNeighbors(current);

            if (unvisitedNeighbors.Count > 0)
            {
                // �����ϰ� �̿� ����
                MazeCell next = unvisitedNeighbors[Random.Range(0, unvisitedNeighbors.Count)];  // �����ϰ� �̿� ����
                RemoveWallBetween(current, next);           // �� ����

                current.SetColor(visitedColor);                             // �ð�ȭ �߰� (���� �湮 �Ϸ� ������)
                next.visited = true;
                visitedCount++;                                             // �ð�ȭ �߰�
                cellStack.Push(next);

                next.SetColor(currentColor);                                // �ð�ȭ �߰�
                yield return new WaitForSeconds(visualizationSpeed);        // �ð�ȭ �߰�
            }
            else
            {
                current.SetColor(backtrackColor);                           // �ð�ȭ �߰�
                yield return new WaitForSeconds(visualizationSpeed);        // �ð�ȭ �߰�

                current.SetColor(visitedColor);                             // �ð�ȭ �߰�
                cellStack.Pop();                            // �� Ʈ��ŷ
            }
        }

        yield return new WaitForSeconds(visualizationSpeed);            // �ð�ȭ �߰�
        ReserAllColors();
        Debug.Log($"�̷� ���� �Ϸ�! �� ({visitedCount} / {totalCells}) ĭ");
    }

    void ReserAllColors()                                                   // ��� ĭ���� �ʱ�ȭ
    {
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < height; z++)
            {
                maze[x, z].SetColor(Color.white);
            }
        }
    }
}
