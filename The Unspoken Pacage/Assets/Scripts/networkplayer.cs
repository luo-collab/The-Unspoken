using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    public float minScale = 0.1f;
    public float maxScale = 10f;
    public float maxDistance = 20f;

    private void Update()
    {
        // 只让本地玩家控制自己的缩放和位置
        if (IsOwner)
        {
            // 计算与中心的距离
            Vector3 pos = transform.position;
            float distance = Mathf.Sqrt(pos.x * pos.x + pos.z * pos.z);

            // 距离越近，scale越大
            float t = Mathf.Clamp01(1f - (distance / maxDistance));
            float yScale = Mathf.Lerp(minScale, maxScale, t);

            // 设置缩放
            Vector3 scale = transform.localScale;
            scale.y = yScale;
            transform.localScale = scale;

            // 同步缩放到服务器
            ScaleY.Value = yScale;
        }
        else
        {
            // 非本地玩家，跟随网络同步的缩放
            Vector3 scale = transform.localScale;
            scale.y = ScaleY.Value;
            transform.localScale = scale;
        }
    }

    // 用于同步y轴缩放
    public NetworkVariable<float> ScaleY = new NetworkVariable<float>(1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
}