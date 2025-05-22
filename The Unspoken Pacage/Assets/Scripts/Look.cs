using UnityEngine;

public class Look : MonoBehaviour // 修改类名为 Look
{
    void Update()
    {
        GameObject closestObject = FindClosestObject();

        // 如果找到了最近的物体
        if (closestObject != null)
        {
            // 计算从当前物体到最近物体的方向向量
            Vector3 directionToTarget = closestObject.transform.position - transform.position;

            // 我们只需要水平方向 (忽略Y轴的差异) 来计算旋转
            Vector3 horizontalDirection = new Vector3(directionToTarget.x, 0, directionToTarget.z);

            // 如果水平方向向量的长度大于一个很小的阈值，避免计算旋转时出现问题
            if (horizontalDirection.sqrMagnitude > 0.001f)
            {
                // 计算指向目标方向的旋转，使得物体的局部X轴朝向目标
                Quaternion targetRotation = Quaternion.FromToRotation(Vector3.right, horizontalDirection);

                // 只应用Y轴的旋转
                transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0); // 只保留Y轴旋转
            }
        }
    }

    GameObject FindClosestObject()
    {
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        GameObject closest = null;
        float minDistance = Mathf.Infinity;
        Vector3 position = transform.position;

        // 遍历场景中所有物体
        foreach (GameObject obj in allObjects)
        {
            // 排除自身和标记为 "plane" 的物体，以及没有Renderer的物体 (简单判断是否为3D物体)
            // 注意：这种简单的判断可能不完全准确，但对于常见情况应该够用
            if (obj != this.gameObject && !obj.CompareTag("plane") && obj.GetComponent<Renderer>() != null)
            {
                float distance = Vector3.Distance(position, obj.transform.position);

                // 如果找到更近的物体
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closest = obj;
                }
            }
        }
        return closest;
    }
} 