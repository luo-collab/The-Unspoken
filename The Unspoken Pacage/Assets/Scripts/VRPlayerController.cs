using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRPlayerController : MonoBehaviour
{
    [Header("移动设置")]
    public float moveSpeed = 3f;         // 移动速度
    public float rotationSpeed = 60f;    // 旋转速度
    public float deadzone = 0.1f;        // 摇杆死区

    [Header("组件引用")]
    public Camera vrCamera;              // VR相机引用

    private Vector2 moveInput;           // 移动输入
    private CharacterController characterController; // 角色控制器
    private PlayerInput playerInput;     // 输入系统
    private Transform cameraTransform;   // 相机变换组件

    private void Start()
    {
        // 获取角色控制器组件
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            characterController = gameObject.AddComponent<CharacterController>();
        }

        // 设置角色控制器参数
        characterController.height = 1.8f;
        characterController.radius = 0.2f;
        characterController.stepOffset = 0.3f;

        // 获取输入系统组件
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            playerInput = gameObject.AddComponent<PlayerInput>();
        }

        // 获取相机引用
        if (vrCamera == null)
        {
            vrCamera = GetComponentInChildren<Camera>();
            if (vrCamera == null)
            {
                Debug.LogError("未找到VR相机！请确保相机是VRPlayer的子对象。");
                return;
            }
        }
        cameraTransform = vrCamera.transform;

        // 启用陀螺仪
        Input.gyro.enabled = true;

        // 调试信息
        Debug.Log("VRPlayerController 初始化完成");
    }

    private void Update()
    {
        // 检查必要组件
        if (cameraTransform == null || characterController == null)
        {
            Debug.LogError("缺少必要组件！");
            return;
        }

        // 处理移动
        HandleMovement();
        
        // 处理旋转（通过手机陀螺仪）
        HandleRotation();
    }

    // 处理移动输入
    public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
        // 调试信息
        Debug.Log($"收到移动输入: {moveInput}");
    }

    private void HandleMovement()
    {
        // 应用死区
        if (moveInput.magnitude < deadzone)
        {
            moveInput = Vector2.zero;
        }

        // 获取相机的正前方和右方向
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        // 将相机的正前方和右方向投影到水平面上
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        // 计算移动方向
        Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x);
        
        // 应用重力
        moveDirection.y = Physics.gravity.y * Time.deltaTime;
        
        // 移动角色
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);

        // 调试信息
        if (moveInput != Vector2.zero)
        {
            Debug.Log($"正在移动: {moveDirection}");
        }
    }

    private void HandleRotation()
    {
        // 获取手机陀螺仪数据
        if (Input.gyro.enabled)
        {
            // 获取陀螺仪的旋转速率
            Vector3 gyroRotation = Input.gyro.rotationRate;
            
            // 根据陀螺仪的Y轴旋转来旋转玩家
            transform.Rotate(Vector3.up, gyroRotation.y * rotationSpeed * Time.deltaTime);

            // 调试信息
            if (gyroRotation.y != 0)
            {
                Debug.Log($"正在旋转: {gyroRotation.y}");
            }
        }
    }
}