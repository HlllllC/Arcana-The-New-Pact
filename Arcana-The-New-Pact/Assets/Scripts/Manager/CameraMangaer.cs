using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

public class CameraController2D : MonoBehaviour
{
    public GridManager gridManager;


    [Header("移动设置")]
    public float moveSpeed = 15f;
    public Vector2 xBounds;
    public Vector2 yBounds;
    public float smoothTime = 0.1f;
    private float targetSize;
    /*
    [Header("缩放设置")]
    public float zoomSpeed = 1f;
    public float minSize = 3f;
    public float maxSize = 10f;
    public bool zoomToMouse = true;  // 是否以鼠标为中心缩放
    // 平滑缩放变量
    private float currentVelocity;
    */

    [Header("相机设置")]
    public Camera targetCamera;

    // 平滑移动变量
    private Vector3 moveVelocity;
    private Vector3 targetPosition;
    private void Awake()
    {
        gridManager = FindObjectOfType<GridManager>();
        var width = gridManager.GetWidth();
        var height = gridManager.GetHeight();
        xBounds = new Vector2(0, width);
        yBounds = new Vector2(0, height);
    }
    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;


        gridManager = FindObjectOfType<GridManager>();
        var width = gridManager.GetWidth();
        var height = gridManager.GetHeight();

        // 确保相机是正交模式
        targetCamera.orthographic = true;

        // 初始化目标值
        targetSize = targetCamera.orthographicSize;
        targetPosition = new Vector3(0,0,transform.position.z);
    }

    void Update()
    {
        HandleMovement();
        //HandleZoom();
    }

    void LateUpdate()
    {
        // 应用平滑移动
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref moveVelocity,
            smoothTime
        );
    }

    void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector2 moveInput = new Vector2(horizontal, vertical);

        if (moveInput.magnitude > 0.1f)
        {
            // 移动速度随缩放级别调整（远时移动更快）
            float dynamicSpeed = moveSpeed * (targetCamera.orthographicSize / 5f);

            // 计算目标位置
            targetPosition += new Vector3(
                horizontal,
                vertical,
                0
            ) * dynamicSpeed * Time.deltaTime;

            // 边界限制
            ClampTargetPosition();
        }
    }
    /*
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll != 0)
        {
            // 记录缩放前的鼠标世界位置
            Vector3 oldMouseWorldPos = Vector3.zero;
            if (zoomToMouse)
            {
                oldMouseWorldPos = targetCamera.ScreenToWorldPoint(
                    new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)
                );
            }

            // 更新目标大小
            targetSize -= scroll * zoomSpeed;
            targetSize = Mathf.Clamp(targetSize, minSize, maxSize);

            // 以鼠标为中心缩放
            if (zoomToMouse)
            {
                // 获取缩放后的鼠标世界位置
                Vector3 newMouseWorldPos = targetCamera.ScreenToWorldPoint(
                    new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)
                );

                // 计算相机应该移动的距离
                Vector3 delta = oldMouseWorldPos - newMouseWorldPos;

                // 应用移动
                targetPosition += delta;

                // 边界限制
                ClampTargetPosition();
            }
        }

        // 平滑缩放
        targetCamera.orthographicSize = Mathf.SmoothDamp(
            targetCamera.orthographicSize,
            targetSize,
            ref currentVelocity,
            smoothTime
        );
    }
    */
    void ClampTargetPosition()
    {
        // 获取当前缩放级别下的有效边界
        float verticalExtent = targetCamera.orthographicSize;
        float horizontalExtent = verticalExtent * targetCamera.aspect;

        // 调整边界以考虑相机视野范围
        float adjustedXMin = xBounds.x + horizontalExtent;
        float adjustedXMax = xBounds.y - horizontalExtent;
        float adjustedYMin = yBounds.x + verticalExtent;
        float adjustedYMax = yBounds.y - verticalExtent;

        // 确保边界不反转（当地图小于视野时）
        if (adjustedXMin > adjustedXMax)
        {
            float midX = (xBounds.x + xBounds.y) / 2;
            adjustedXMin = midX - horizontalExtent;
            adjustedXMax = midX + horizontalExtent;
        }

        if (adjustedYMin > adjustedYMax)
        {
            float midY = (yBounds.x + yBounds.y) / 2;
            adjustedYMin = midY - verticalExtent;
            adjustedYMax = midY + verticalExtent;
        }

        // 应用边界限制
        targetPosition.x = Mathf.Clamp(targetPosition.x, adjustedXMin, adjustedXMax);
        targetPosition.y = Mathf.Clamp(targetPosition.y, adjustedYMin, adjustedYMax);
        targetPosition.z = transform.position.z; // 保持Z轴不变
    }

    // 可视化边界（在Scene视图中显示）
    void OnDrawGizmosSelected()
    {
        // 绘制世界边界
        Gizmos.color = Color.yellow;
        Vector3 center = new Vector3(
            (xBounds.x + xBounds.y) / 2,
            (yBounds.x + yBounds.y) / 2,
            transform.position.z
        );
        Vector3 size = new Vector3(
            xBounds.y - xBounds.x,
            yBounds.y - yBounds.x,
            1
        );
        Gizmos.DrawWireCube(center, size);

        // 如果有相机，绘制当前视野范围
        if (targetCamera != null)
        {
            Gizmos.color = Color.green;
            float verticalExtent = targetCamera.orthographicSize;
            float horizontalExtent = verticalExtent * targetCamera.aspect;

            Vector3 viewCenter = transform.position;
            Vector3 viewSize = new Vector3(horizontalExtent * 2, verticalExtent * 2, 1);
            Gizmos.DrawWireCube(viewCenter, viewSize);
        }
    }

    // 可选：在游戏中显示当前缩放级别
    void OnGUI()
    {
        GUIStyle style = new GUIStyle();
        style.fontSize = 16;
        style.normal.textColor = Color.white;
        /*
        GUI.Label(
            new Rect(10, 10, 200, 30),
            $"Zoom: {targetCamera.orthographicSize:F1} / {maxSize:F1}",
            style
        );
        */
        GUI.Label(
            new Rect(10, 40, 300, 30),
            $"Position: ({targetPosition.x:F1}, {targetPosition.y:F1})",
            style
        );
    }
}