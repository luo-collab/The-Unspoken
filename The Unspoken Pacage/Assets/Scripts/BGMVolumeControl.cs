using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class BGMVolumeControl : MonoBehaviour
{
    public GameObject audioListenerObject; // 拖拽您的玩家或监听器物体到这里
    public float minDistance = 5f; // 在这个距离内音量最大
    public float maxDistance = 25f; // 超过这个距离音量为零

    private AudioSource audioSource; // 背景音乐的AudioSource组件

    void Awake()
    {
        // 获取AudioSource组件
        audioSource = GetComponent<AudioSource>();

        // 检查是否指定了监听器物体
        if (audioListenerObject == null)
        {
            Debug.LogError("BGMVolumeControl: 未指定 Audio Listener 物体！请在 Inspector 中将您的玩家或带有 AudioListener 的物体拖拽到 audioListenerObject 字段。");
            enabled = false; // 禁用脚本，避免报错
            return;
        }

        // 确保AudioSource的Spatial Blend设置为2D或Hybrid
        // 如果Spatial Blend设置为3D，会与我们脚本控制的音量衰减叠加，可能导致意外效果。
        // 设为0 (2D) 或小于1的值（Hybrid）让脚本主导距离衰减。
        audioSource.spatialBlend = 0.0f; // 设为2D，完全由脚本控制音量
        audioSource.playOnAwake = true; // 确保音乐自动播放
        audioSource.loop = true; // 确保音乐循环播放
        audioSource.volume = 1.0f; // 初始音量设为最大，由脚本控制衰减
    }

    void Update()
    {
        // 再次检查引用是否有效
        if (audioSource == null || audioListenerObject == null)
        {
            enabled = false;
            return;
        }

        // 计算当前物体 (音乐源) 与监听器物体之间的距离
        float distance = Vector3.Distance(transform.position, audioListenerObject.transform.position);

        // 使用 InverseLerp 将距离映射到 t 值 (0到1)，反转 minDistance 和 maxDistance 的顺序
        // 距离 <= minDistance 对应 t=1 (最大音量)
        // 距离 >= maxDistance 对应 t=0 (最小音量)
        float clampedDistance = Mathf.Clamp(distance, minDistance, maxDistance);
        float t = Mathf.InverseLerp(maxDistance, minDistance, clampedDistance);

        // 使用 Lerp 将 t 值映射到音量范围 (0到1)
        // t=1 对应音量最大 (1.0f)
        // t=0 对应音量最小 (0.0f)
        float targetVolume = Mathf.Lerp(0.0f, 1.0f, t);

        // 将计算出的音量应用到AudioSource
        audioSource.volume = targetVolume;
    }
} 