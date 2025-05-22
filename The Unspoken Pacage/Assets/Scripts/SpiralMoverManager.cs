using System.Collections.Generic;
using UnityEngine;

public class SpiralMoverManager : MonoBehaviour
{
    public static SpiralMoverManager Instance { get; private set; }
    private List<SpiralMover> movers = new List<SpiralMover>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void Register(SpiralMover mover)
    {
        if (!movers.Contains(mover)) movers.Add(mover);
    }

    public void Unregister(SpiralMover mover)
    {
        if (movers.Contains(mover)) movers.Remove(mover);
    }

    public bool IsPositionValid(SpiralMover self, Vector3 targetPos, float selfScaleX)
    {
        foreach (var mover in movers)
        {
            if (mover == self) continue;
            float otherScaleX = mover.CurrentScaleX;
            float minDist = (selfScaleX + otherScaleX) * 0.2f;
            if (Vector3.Distance(targetPos, mover.transform.position) < minDist)
                return false;
        }
        return true;
    }

    // 新增方法：统计在给定半径范围内的 SpiralMover 物体数量
    public int CountMoversInRadius(float radius)
    {
        int count = 0;
        Vector3 center = Vector3.zero; // 中心点为 (0,0,0)
        foreach (var mover in movers)
        {
            // 检查 mover 是否有效 (未被销毁)
            if (mover != null)
            {
                float distanceToCenter = Vector3.Distance(mover.transform.position, center);
                if (distanceToCenter < radius)
                {
                    count++;
                }
            }
        }
        return count;
    }
}
