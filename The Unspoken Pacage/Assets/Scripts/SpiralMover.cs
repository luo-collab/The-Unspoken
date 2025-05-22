using UnityEngine;

public class SpiralMover : MonoBehaviour
{
    // 螺旋参数
    private float radius;
    private float angleSpeed;
    private float heightSpeed;
    private float startTime;
    private float angleOffset;
    private float spiralTightness;
    private Vector3 center = Vector3.zero;
    private float elapsed;
    private bool started = false;
    public float CurrentScaleX { get; private set; }
    private bool scalingUp = true;
    private float scaleUpTime = 2.5f;
    private float scaleUpElapsed = 0f;
    private Vector3 targetScale;

    // 靠近/远离中心参数
    private bool movingAway = true; // true为远离，false为靠近
    private float directionChangeInterval;
    private float directionChangeTimer;

    void OnEnable()
    {
        SpiralMoverManager.Instance?.Register(this);
    }

    void OnDisable()
    {
        SpiralMoverManager.Instance?.Unregister(this);
    }

    void Start()
    {
        // 随机化参数
        radius = Random.Range(1.0f, 5.0f); // 初始半径
        angleSpeed = Random.Range(0.1f, 0.3f); // 角速度，调整范围以将速度提高到目标范围
        heightSpeed = Random.Range(0.1f, 0.8f); // 螺旋上升速度 (保持不变)
        startTime = Random.Range(0f, 3f); // 随机延迟启动 (保持不变)
        angleOffset = Random.Range(0f, Mathf.PI * 2f); // 起始角度 (保持不变)
        spiralTightness = Random.Range(0.02f, 0.05f); // 螺距紧密度，调整范围以将速度提高到目标范围
        elapsed = 0f;
        // 初始scale为0
        transform.localScale = Vector3.zero;
        scalingUp = true;
        scaleUpElapsed = 0f;

        // 随机确定初始运动方向和切换时间
        movingAway = Random.value > 0.5f; // 50%概率远离，50%概率靠近
        directionChangeInterval = Random.Range(6f, 180f); // 恢复时间间隔上限为 180秒
        directionChangeTimer = 0f;
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        directionChangeTimer += Time.deltaTime;

        // 检查是否需要改变运动方向
        if (directionChangeTimer >= directionChangeInterval)
        {
            movingAway = !movingAway;
            directionChangeInterval = Random.Range(6f, 180f); // 恢复时间间隔上限为 180秒
            directionChangeTimer = 0f;
        }

        if (!started && elapsed >= startTime)
        {
            started = true;
            elapsed = 0f;
        }
        if (!started) return;

        float angle = angleSpeed * elapsed + angleOffset;
        float currentSpiralTightness = movingAway ? spiralTightness : -spiralTightness; // 根据方向调整螺距紧密度
        float spiralRadius = radius + currentSpiralTightness * elapsed;

        // 确保半径不会变为负数，如果靠近中心半径减小到接近0时，可以考虑销毁或反向运动
        if (spiralRadius < 0.1f) // 设置一个很小的阈值，避免因浮点数误差导致问题
        {
             if (!movingAway) // 如果是靠近中心导致半径过小，切换方向或销毁
             {
                 movingAway = true; // 切换为远离
                 directionChangeInterval = Random.Range(6f, 180f); // 恢复重置切换时间上限为 180秒
                 directionChangeTimer = 0f;
                 spiralRadius = 0.1f; // 重置半径到阈值，避免NaN或Inf
             }
        }

        float clampedRadius = Mathf.Clamp(spiralRadius, 1.8f, 20f);
        float t = Mathf.InverseLerp(1.8f, 10f, clampedRadius);
        float scaleValue = Mathf.Lerp(4f, 0f, t);
        targetScale = new Vector3(scaleValue, scaleValue, scaleValue);
        CurrentScaleX = scaleValue;
        float yPos = scaleValue * 0.5f;
        Vector3 targetPos = center + new Vector3(Mathf.Cos(angle) * spiralRadius, yPos, Mathf.Sin(angle) * spiralRadius);

        // 计算目标位置与中心的距离
        float distToCenter = Vector3.Distance(targetPos, center);
        // 计算最小允许距离
        float minAllowedDistance = (scaleValue / 2.0f) + 1.8f;

        // 如果物体正在靠近中心且目标位置距离中心小于最小允许距离，则切换为远离模式
        if (!movingAway && distToCenter < minAllowedDistance)
        {
            movingAway = true; // 切换为远离
            directionChangeInterval = Random.Range(6f, 180f); // 恢复重置切换时间上限为 180秒
            directionChangeTimer = 0f;
             // 可以选择在这里调整 spiralRadius 或 targetPos，避免物体瞬间移动到不满足条件的位置
             // 一个简单的做法是保持当前位置或者根据最小距离重新计算一个位置，这里先选择切换方向并让下一次Update重新计算位置
        }

        // 2.5秒scale渐变动画
        if (scalingUp)
        {
            scaleUpElapsed += Time.deltaTime;
            float lerpT = Mathf.Clamp01(scaleUpElapsed / scaleUpTime);
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, lerpT);
            if (lerpT >= 1f) scalingUp = false;
        }
        else
        {
            transform.localScale = targetScale;
        }

        // 检查与其他物体的距离
        if (SpiralMoverManager.Instance == null || SpiralMoverManager.Instance.IsPositionValid(this, targetPos, scaleValue))
        {
            transform.position = targetPos;
        }
        // 超过最大距离时销毁物体
        if (spiralRadius >= 10f && movingAway) // 将销毁距离改为 10米
        {
            Destroy(gameObject);
        }
         // 如果靠近中心且距离小于最小距离（1.8米），可以考虑切换方向或停止，这里选择切换方向
        if (spiralRadius <= 1.8f && !movingAway)
        {
             movingAway = true;
             directionChangeInterval = Random.Range(6f, 180f); // 恢复重置切换时间上限为 180秒
             directionChangeTimer = 0f;
        }
    }
} 