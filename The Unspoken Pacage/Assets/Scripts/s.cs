using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s : MonoBehaviour
{
    public Transform cameraTransform; // 拖拽主摄像机进来
    public float minScale = 0.1f;
    public float maxScale = 10f;
    public float maxDistance = 20f; // 你可以根据场景大小调整

    void Update()
    {
        if (cameraTransform == null) return;

        // 只计算 x、z 平面上的距离
        Vector3 camPos = cameraTransform.position;
        float distance = Mathf.Sqrt(camPos.x * camPos.x + camPos.z * camPos.z);

        // 距离越近，scale 越大；距离越远，scale 越小
        float tValue = Mathf.Clamp01(1f - (distance / maxDistance));
        float yScaleValue = Mathf.Lerp(minScale, maxScale, tValue);

        // 设置 XR Origin 的 y 方向 scale
        Vector3 scale = transform.localScale;
        scale.y = yScaleValue;
        transform.localScale = scale;
    }
}
