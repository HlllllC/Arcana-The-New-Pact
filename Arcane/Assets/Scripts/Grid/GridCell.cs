using UnityEngine;
using UnityEngine.Events;

public enum WallType
{
    None,       // 空地
    Normal,     // 不可破坏墙体
    Glass       // 可破坏墙体
}

public class GridCell : MonoBehaviour
{
    public Vector2Int coordinate;           // 网格坐标
    public WallType wallType = WallType.None;
    public Unit currentUnit;                 // 当前站立的单位
    public SpecialCell specialCell;           // 特殊网格（如分数点）

    [Header("Events")]
    public UnityEvent<GridCell> OnCellClicked; // 点击事件，可由GridManager监听

    // 是否为空地（无单位且无墙体阻挡）
    public bool IsEmpty => currentUnit == null && (wallType == WallType.None || wallType == WallType.Glass);
    // 是否完全不可通行（普通墙体阻挡）
    public bool IsBlocked => wallType == WallType.Normal;

    private void OnMouseDown()
    {
        OnCellClicked?.Invoke(this);
    }

    // 设置墙体类型并更新显示（可扩展改变颜色或精灵）
    public void SetWallType(WallType type)
    {
        wallType = type;
        // 根据墙体类型修改颜色或精灵（示例）
        var sr = GetComponent<SpriteRenderer>();
        switch (type)
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
}