using UnityEngine;

[RequireComponent(typeof(Camera))]
public class VRCameraFOVBasedOnPlayerScale : MonoBehaviour
{
    public GameObject vrPlayer; // 拖拽您的 VRPlayer 物体到这里
    public float minFOV = 10f; // Scale 最小 (0) 时的 FOV 值
    public float maxFOV = 100f; // Scale 最大时 (取决于 VRPlayerScaleBasedOnDistance 脚本的 maxScale) 的 FOV 值

    private Camera vrCamera; // 引用 VR Camera 组件
    private VRPlayerScaleBasedOnDistance playerScaleScript; // 引用 VRPlayer 上的 Scale 脚本

    void Awake()
    {
        // 获取 VR Camera 组件
        vrCamera = GetComponent<Camera>();

        // 检查是否指定了 VRPlayer 物体
        if (vrPlayer == null)
        {
            Debug.LogError("VRCameraFOVBasedOnPlayerScale: 未指定 VRPlayer 物体！请在 Inspector 中将 VRPlayer 物体拖拽到 vrPlayer 字段。");
            enabled = false; // 禁用脚本，避免报错
            return;
        }

        // 获取 VRPlayer 上的 VRPlayerScaleBasedOnDistance 脚本
        playerScaleScript = vrPlayer.GetComponent<VRPlayerScaleBasedOnDistance>();
        if (playerScaleScript == null)
        {
            Debug.LogError("VRCameraFOVBasedOnPlayerScale: 在指定的 VRPlayer 物体上找不到 VRPlayerScaleBasedOnDistance 脚本。");
            enabled = false; // 禁用脚本，避免报错
            return;
        }

        // 确保 FOV 的范围是有效的
        if (minFOV >= maxFOV)
        {
             Debug.LogWarning("VRCameraFOVBasedOnPlayerScale: minFOV 应该小于 maxFOV，已自动调整。");
             minFOV = 10f; // 默认值
             maxFOV = 100f; // 默认值
        }
    }

    void Update()
    {
        // 再次检查引用是否有效，以防运行时丢失
        if (vrPlayer == null || vrCamera == null || playerScaleScript == null)
        {
            enabled = false; // 禁用脚本
            return;
        }

        // 获取 VRPlayer 当前的 Y 轴 Scale 值
        // 由于 VRPlayerScaleBasedOnDistance 脚本将所有轴的 Scale 设为同一个值，这里取 Y 轴 Scale 即可
        float currentScale = vrPlayer.transform.localScale.y;

        // 获取 VRPlayer 的最大 Scale 值 (从其 Scale 脚本中获取)
        float maxPlayerScale = playerScaleScript.maxScale;

        // 避免除以零的情况，特别是当 maxPlayerScale 可能为 0 时
        if (maxPlayerScale <= 0.001f)
        {
             vrCamera.fieldOfView = minFOV; // 如果最大 Scale 接近 0，FOV 设为最小值
             return;
        }

        // 使用 InverseLerp 将 VRPlayer 当前 Scale (0到maxPlayerScale) 映射到 t 值 (0到1)
        // Scale 0 对应 t=0， Scale maxPlayerScale 对应 t=1
        float t = Mathf.InverseLerp(0f, maxPlayerScale, currentScale);

        // 使用 Lerp 将 t 值映射到 FOV 范围 (minFOV到maxFOV)
        // t=0 对应 minFOV， t=1 对应 maxFOV
        float targetFOV = Mathf.Lerp(minFOV, maxFOV, t);

        // 将计算出的 FOV 值应用到 VR Camera
        vrCamera.fieldOfView = targetFOV;
    }
} 