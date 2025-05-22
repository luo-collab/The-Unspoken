using UnityEngine;
using System.Collections.Generic;

public class SpiralMoverSpawner : MonoBehaviour
{
    public GameObject moverPrefab;
    public int maxMovers = 20;
    private List<GameObject> movers = new List<GameObject>();
    private float startTime;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        // 移除已被销毁的物体
        movers.RemoveAll(m => m == null);

        // 检查场景运行时间是否超过10秒
        bool timeElapsed = Time.time - startTime >= 10f;

        // 获取当前物体数量
        int currentMoverCount = movers.Count;

        // 如果场景运行时间超过10秒且当前物体数量少于最低数量 (18)
        // 或者当前物体数量少于最大数量 (maxMovers = 20) 并且半径6米内物体数量少于2
        // 则尝试生成新物体
        if ((timeElapsed && currentMoverCount < 18) || (currentMoverCount < maxMovers && SpiralMoverManager.Instance != null && SpiralMoverManager.Instance.CountMoversInRadius(6f) < 2))
        {
            // 如果场景运行时间超过10秒且当前物体数量少于18个，强制生成以达到最低数量 (忽略半径限制)
            if (timeElapsed && currentMoverCount < 18)
            {
                // 循环生成直到达到最低数量18个，或达到最大数量maxMovers
                while (movers.Count < 18 && movers.Count < maxMovers)
                {
                    if (moverPrefab != null)
                    {
                        Vector3 spawnPos = GetRandomSpawnPosition();
                        GameObject obj = Instantiate(moverPrefab, spawnPos, Quaternion.identity);
                        movers.Add(obj);
                    }
                }
            }
            // 如果已经达到最低数量或场景未运行10秒，并且当前物体数量小于最大数量且半径限制允许，则生成物体 (考虑半径限制)
            else if (currentMoverCount < maxMovers && SpiralMoverManager.Instance != null && SpiralMoverManager.Instance.CountMoversInRadius(6f) < 2)
            {
                 if (moverPrefab != null)
                 {
                     Vector3 spawnPos = GetRandomSpawnPosition();
                     GameObject obj = Instantiate(moverPrefab, spawnPos, Quaternion.identity);
                     movers.Add(obj);
                 }
            }
        }
    }

    Vector3 GetRandomSpawnPosition()
    {
        // 生成在以(0,0,0)为中心，边长20米的正方形内（X和Z坐标在-10到10之间）
        float x = Random.Range(-10f, 10f);
        float z = Random.Range(-10f, 10f);
        return new Vector3(x, 0, z);
    }
} 