using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MoverAudio : MonoBehaviour
{
    public AudioClip[] audioClips; // 可供选择的音频片段数组
    private AudioSource audioSource; // 物体的AudioSource组件
    private SpiralMover spiralMover; // 获取SpiralMover脚本以便访问Scale值

    public float minDistanceScaleMultiplier = 0.2f; // Min Distance 等于 Scale.x 的这个倍数
    public float maxDistanceScaleMultiplier = 5f; // Max Distance 等于 Scale.x 的这个倍数 (可调)

    void Awake()
    {
        // 获取或添加AudioSource组件
        audioSource = GetComponent<AudioSource>();
        spiralMover = GetComponent<SpiralMover>();
    }

    void Start()
    {
        // 检查是否有音频片段可用
        if (audioClips == null || audioClips.Length == 0)
        {
            Debug.LogError("MoverAudio: 没有指定音频片段！请在Inspector中为 audioClips 数组添加音频文件。");
            return;
        }

        // 随机选择一个音频片段并分配给AudioSource
        int randomIndex = Random.Range(0, audioClips.Length);
        audioSource.clip = audioClips[randomIndex];

        // 配置AudioSource为3D声音
        audioSource.spatialBlend = 1.0f; //  fully 3D sound
        audioSource.loop = true; // 循环播放背景音
        audioSource.playOnAwake = true; // 物体生成时自动播放
        audioSource.volume = 1.0f; // 初始音量设为最大，让距离衰减控制音量

        // 设置音量衰减模式，Linear或Logarithmic都可以使变化明显
        audioSource.rolloffMode = AudioRolloffMode.Linear; // 线性衰减

        // 播放音频
        audioSource.Play();

        // 设置初始的最小和最大距离
        UpdateAudioDistances();
    }

    void Update()
    {
        // 由于Scale会变化，持续更新最小和最大距离
        UpdateAudioDistances();
    }

    void UpdateAudioDistances()
    {
        if (spiralMover != null)
        {
            // 获取当前X轴的Scale值
            float currentScaleX = spiralMover.CurrentScaleX; // 使用SpiralMover中的CurrentScaleX属性
            // 或者如果SpiralMover中没有暴露CurrentScaleX，可以使用 transform.localScale.x (但这可能不是期望的ScaleValue)
            // float currentScaleX = transform.localScale.x;

            // 根据Scale值设置最小和最大距离
            audioSource.minDistance = currentScaleX * minDistanceScaleMultiplier;
            audioSource.maxDistance = currentScaleX * maxDistanceScaleMultiplier; // 越大的物体，声音传播距离越远

            // 确保最小距离不会是0或负数，以免导致问题
            if (audioSource.minDistance < 0.01f) audioSource.minDistance = 0.01f;
            // 确保最大距离大于最小距离
            if (audioSource.maxDistance <= audioSource.minDistance) audioSource.maxDistance = audioSource.minDistance + 0.1f;
        }
    }
} 