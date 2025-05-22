using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class VRPlayerScaleBasedOnDistance : MonoBehaviour
{
    public Vector3 center = Vector3.zero; // 中心点，默认为 (0,0,0)
    public float scaleMinDistance = 0f; // Scale 最大时的距离，改为 0米
    public float scaleMaxDistance = 10f; // Scale 变为 0 时的距离
    public float maxScale = 4.878f; // 最大 Scale 值，改为 4.878

    private CharacterController characterController; // 引用 CharacterController 组件

    void Awake()
    {
        // 获取 CharacterController 组件
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // 计算当前物体与中心的距离
        float distanceToCenter = Vector3.Distance(transform.position, center);

        // 使用 InverseLerp 将距离映射到一个 t 值 (0到1)，对应 Scale 从 maxScale 到 0
        // Clamp 距离在 scaleMinDistance 和某个最大值之间，这里为了模拟 SpiralMover 的行为，Clamp上限用20f
        // 但实际影响t值的范围是 scaleMinDistance 到 scaleMaxDistance (0m 到 10m)
        float clampedDistance = Mathf.Clamp(distanceToCenter, scaleMinDistance, 20f);
        float t = Mathf.InverseLerp(scaleMinDistance, scaleMaxDistance, clampedDistance);

        // 使用 Lerp 计算当前的 Scale 值
        float currentScaleValue = Mathf.Lerp(maxScale, 0f, t);

        // 将计算出的 Scale 值应用到物体上
        // 确保 Scale 值不会小于 0
        currentScaleValue = Mathf.Max(0f, currentScaleValue);
        transform.localScale = new Vector3(currentScaleValue, currentScaleValue, currentScaleValue);

        // 计算目标Y轴Position (Scale值的一半)
        float targetY = currentScaleValue / 2f;

        // 确保目标Y轴Position最小不小于 0.01米
        targetY = Mathf.Max(0.01f, targetY);

        // 计算需要移动的Y轴距离
        float deltaY = targetY - transform.position.y;

        // 通过 CharacterController 移动物体，只改变Y轴位置
        // 注意：如果手柄控制也使用 CharacterController.Move，这可能会叠加或冲突
        // 理想情况是手柄控制脚本计算总的 delta 向量，并包含这个 deltaY
        // 但这里只处理 Y 轴，假设手柄处理 XZ
        characterController.Move(new Vector3(0, deltaY, 0));
    }
} 