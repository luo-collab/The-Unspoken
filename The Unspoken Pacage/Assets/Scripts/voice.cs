using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerVoice : NetworkBehaviour
{
    public float minVolume = 0.1f;
    public float maxVolume = 1f;
    public float maxDistance = 20f;
    public AudioSource voiceSource; // 拖拽语音AudioSource进来

    // 用于同步音量
    public NetworkVariable<float> NetVolume = new NetworkVariable<float>(
        1f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    void Update()
    {
        if (IsOwner)
        {
            // 计算与中心的距离
            Vector3 pos = transform.position;
            float distance = Mathf.Sqrt(pos.x * pos.x + pos.z * pos.z);

            // 距离越近，音量越大
            float t = Mathf.Clamp01(1f - (distance / maxDistance));
            float volume = Mathf.Lerp(minVolume, maxVolume, t);

            // 同步音量
            NetVolume.Value = volume;
        }

        // 所有人都用NetVolume来设置AudioSource音量
        if (voiceSource != null)
        {
            voiceSource.volume = NetVolume.Value;
        }
    }
}
