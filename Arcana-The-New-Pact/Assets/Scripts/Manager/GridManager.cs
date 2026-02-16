using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    //unity中声明网格长宽边个数
    [SerializeField]private int width, height;
    //声明网格类型
    [SerializeField]private Tile wallTile,floorTile;
    //相机
    [SerializeField]private Camera camera;

    //管理网格的字典
    private Dictionary<Vector2, Tile> tiles;


    private void Start()
    {
        GenerateGrid();
    }

    /// <summary>
    /// 网格生成
    /// </summary>
    void GenerateGrid()
    {
        tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //TODO 
                #region 地图生成逻辑
                var randomNumber = Random.Range(0, 10);
                var randomTile = new Tile();
                var isWall = randomNumber > 8 || x == 0 || x == width - 1 || y == 0 || y == height - 1;
                if (isWall)
                {
                    randomTile = wallTile;
                }
                else
                {
                    randomTile = floorTile;
                }
                #endregion
                var spawnedTile = Instantiate(randomTile, new Vector3(x, y), Quaternion.identity);
                spawnedTile.transform.SetParent(transform);

                spawnedTile.name = $"Tile {x} {y}";

                
                spawnedTile.Init();

                tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        camera.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 0.5f, -10);

    }


    /// <summary>
    /// 返回鼠标所指网格
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public Tile GetTile(Vector2 pos)
    {
        if (tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }

        return null;
    }

    //屎山来喽
    public int GetWidth()
    {
        return width;
    }
    public int GetHeight()
    {
        return height;
    }
}
