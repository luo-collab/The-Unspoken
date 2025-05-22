using UnityEngine;

public class LookAtCenterOnlyY : MonoBehaviour
{
    public Vector3 center = Vector3.zero; // 中心点，默认为 (0,0,0)

    void Update()
    {
        // 计算从当前物体到中心点的方向向量
        Vector3 directionToCenter = center - transform.position;

        // 我们只需要水平方向 (忽略Y轴的差异) 来计算旋转
        Vector3 horizontalDirection = new Vector3(directionToCenter.x, 0, directionToCenter.z);

        // 如果水平方向向量的长度大于一个很小的阈值，避免计算旋转时出现问题
        if (horizontalDirection.sqrMagnitude > 0.001f)
        {
            // 计算指向中心方向的旋转，使得物体的局部X轴朝向中心
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.right, horizontalDirection);

            // 只应用Y轴的旋转，保持X和Z轴旋转为0
            transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        }
    }
} 