using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Settings")]
    public int width = 8;
    public int height = 8;
    public float cellSize = 1f;
    public GameObject cellPrefab;

    private GridCell[,] cells;
    public GridCell[,] Cells => cells;
    void Awake()
    {
        // 单例模式
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("Multiple GridManager instances found! Destroying duplicate.");
            Destroy(gameObject);
            return;
        }
    }
    void Start()
    {
        GenerateGrid();
        GenerateInternalWalls(); // 随机内部墙体
    }

    void GenerateGrid()
    {
        cells = new GridCell[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 position = new Vector3(x * cellSize, y * cellSize, 0);
                GameObject go = Instantiate(cellPrefab, position, Quaternion.identity, transform);
                go.name = $"Cell({x},{y})";
                GridCell cell = go.GetComponent<GridCell>();
                cell.coordinate = new Vector2Int(x, y);

                // 设置外围墙体为Normal
                if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                {
                    cell.SetWallType(WallType.Normal);
                }
                else
                {
                    cell.SetWallType(WallType.None);
                }

                // 订阅点击事件（可选）
                cell.OnCellClicked.AddListener(OnCellClicked);

                cells[x, y] = cell;
            }
        }
    }

    void OnCellClicked(GridCell cell)
    {
        Debug.Log($"Clicked on {cell.coordinate}");
        // 后续可交由游戏管理器处理
    }

    public GridCell GetCell(Vector2Int coord)
    {
        if (coord.x < 0 || coord.x >= width || coord.y < 0 || coord.y >= height)
            return null;
        return cells[coord.x, coord.y];
    }

    public bool IsCellPassable(Vector2Int coord, Unit unit = null)
    {
        GridCell cell = GetCell(coord);
        if (cell == null) return false;
        if (cell.IsBlocked) return false; // 普通墙体阻挡
        if (cell.currentUnit != null && cell.currentUnit != unit) return false; // 其他单位阻挡
        return true;
    }

    // 临时：随机生成内部墙体（后续步骤7会替换为连通性算法）
    void GenerateInternalWalls()
    {
        // 步骤1：初始化所有内部格子为墙体
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                cells[x, y].SetWallType(WallType.Normal);
            }
        }

        // 步骤2：随机选择一个起点作为通路
        List<Vector2Int> walls = new List<Vector2Int>();
        Vector2Int start = new Vector2Int(Random.Range(1, width - 1), Random.Range(1, height - 1));
        SetPassage(start);
        AddAdjacentWalls(start, walls);

        // 步骤3：随机Prim主循环
        while (walls.Count > 0)
        {
            int index = Random.Range(0, walls.Count);
            Vector2Int wall = walls[index];
            walls.RemoveAt(index);

            // 检查这个墙是否能连接两个区域（只有一侧是通路）
            List<Vector2Int> neighbors = GetNeighbors(wall);
            int passageCount = 0;
            Vector2Int passageNeighbor = Vector2Int.zero;
            foreach (var n in neighbors)
            {
                if (IsInBounds(n) && cells[n.x, n.y].wallType == WallType.None)
                {
                    passageCount++;
                    passageNeighbor = n;
                }
            }
            if (passageCount == 1) // 只有一侧是通路，则打通这面墙
            {
                SetPassage(wall);
                // 添加新墙
                foreach (var n in neighbors)
                {
                    if (IsInBounds(n) && cells[n.x, n.y].wallType == WallType.Normal && !walls.Contains(n))
                        walls.Add(n);
                }
            }
            // 如果两侧都是通路，保留墙体（形成房间间隔）
        }

        // 步骤4：将部分墙体改为玻璃（可选）
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++)
            {
                if (cells[x, y].wallType == WallType.Normal && Random.value < 0.1f) // 10%概率改为玻璃
                {
                    cells[x, y].SetWallType(WallType.Glass);
                }
            }
        }
    }

    // 辅助方法
    void SetPassage(Vector2Int pos)
    {
        cells[pos.x, pos.y].SetWallType(WallType.None);
    }

    void AddAdjacentWalls(Vector2Int pos, List<Vector2Int> walls)
    {
        foreach (var dir in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
        {
            Vector2Int neighbor = pos + dir;
            if (IsInBounds(neighbor) && cells[neighbor.x, neighbor.y].wallType == WallType.Normal && !walls.Contains(neighbor))
                walls.Add(neighbor);
        }
    }

    List<Vector2Int> GetNeighbors(Vector2Int pos)
    {
        List<Vector2Int> list = new List<Vector2Int>();
        foreach (var dir in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
        {
            Vector2Int neighbor = pos + dir;
            if (IsInBounds(neighbor))
                list.Add(neighbor);
        }
        return list;
    }

    bool IsInBounds(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
    }
    public List<Vector2Int> FindPath(Vector2Int start, Vector2Int end, Unit movingUnit = null)
    {
        // 简单起见，使用BFS（网格小的情况足够）
        Queue<Vector2Int> frontier = new Queue<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> cameFrom = new Dictionary<Vector2Int, Vector2Int>();
        frontier.Enqueue(start);
        cameFrom[start] = start;

        while (frontier.Count > 0)
        {
            Vector2Int current = frontier.Dequeue();
            if (current == end)
                break;

            foreach (var dir in new Vector2Int[] { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right })
            {
                Vector2Int next = current + dir;
                if (!IsInBounds(next)) continue;
                if (!IsCellPassable(next, movingUnit)) continue; // 检查是否可通行
                if (cameFrom.ContainsKey(next)) continue; // 已访问

                frontier.Enqueue(next);
                cameFrom[next] = current;
            }
        }

        // 重建路径
        if (!cameFrom.ContainsKey(end))
            return null; // 不可达

        List<Vector2Int> path = new List<Vector2Int>();
        Vector2Int step = end;
        while (step != start)
        {
            path.Add(step);
            step = cameFrom[step];
        }
        path.Add(start);
        path.Reverse();
        return path;
    }
    public void UpdateCellColor(GridCell cell)
    {
        if (cell == null) return;
        SpriteRenderer sr = cell.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        switch (cell.wallType)
        {
            case WallType.None:
                sr.color = Color.white;
                break;
            case WallType.Normal:
                sr.color = Color.gray;
                break;
            case WallType.Glass:
                sr.color = Color.cyan;
                break;
        }
    }
    void OnDrawGizmos()
    {
        if (cells == null) return;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridCell cell = cells[x, y];
                if (cell == null) continue;
                Vector3 pos = new Vector3(x * cellSize, y * cellSize, 0) + Vector3.one * cellSize * 0.5f;
                if (cell.wallType == WallType.Normal)
                    Gizmos.color = Color.red;
                else if (cell.wallType == WallType.Glass)
                    Gizmos.color = Color.cyan;
                else
                    Gizmos.color = Color.green;
                Gizmos.DrawWireCube(pos, Vector3.one * cellSize * 0.9f);
            }
        }
    }
}