using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;           // 行走速度
    public float turnSpeed = 10f;          // 转向速度
    public float gravity = 9.81f;          // 重力值
    public float jumpHeight = 2f;          // 跳跃高度
    public float groundCheckDistance = 0.1f; // 地面检测距离

    [Header("Camera Settings")]
    public Transform cameraFollowTarget;   // 相机跟随的目标点
    public Vector3 cameraOffset = new Vector3(0, 1.5f, -3f); // 相机偏移
    public float cameraDistance = 5f;      // 相机距离
    public float cameraMinDistance = 1f;    // 相机最近距离
    public float cameraMaxDistance = 10f;   // 相机最远距离
    public float cameraSensitivity = 2f;   // 相机灵敏度

    [Header("Animation Settings")]
    public string speedParameter = "Speed"; // 动画控制器中的速度参数
    public float animationTransition = 0.1f; // 动画过渡时间

    // 组件引用
    private CharacterController _controller;
    private Animator _animator;
    private Camera _mainCamera;

    // 移动状态
    private Vector3 _velocity;
    private bool _isGrounded;
    private bool _isJumping;
    private float _yVelocity;

    // 相机状态
    private float _currentX;
    private float _currentY;
    private float _desiredDistance;

    void Start()
    {
        // 获取组件
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _mainCamera = Camera.main;

        // 初始化相机距离
        _desiredDistance = cameraDistance;

        // 锁定鼠标
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // 检测地面
        CheckGrounded();

        // 处理输入
        HandleInput();

        // 处理重力
        ApplyGravity();

        // 更新动画
        UpdateAnimation();
    }

    void LateUpdate()
    {
        // 更新相机
        UpdateCamera();
    }

    // 地面检测
    private void CheckGrounded()
    {
        // 使用CharacterController自带的地面检测
        _isGrounded = _controller.isGrounded;

        if (_isGrounded && _yVelocity < 0)
        {
            _yVelocity = -gravity * Time.deltaTime;
            _isJumping = false;
        }
    }

    // 输入处理
    private void HandleInput()
    {
        // 获取输入方向
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;

        // 计算相机方向上的移动
        if (inputDirection.magnitude >= 0.1f)
        {
            // 获取相机方向
            Vector3 cameraForward = _mainCamera.transform.forward;
            Vector3 cameraRight = _mainCamera.transform.right;

            // 忽略相机上下旋转影响
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // 计算移动方向（相对于相机）
            Vector3 moveDirection = cameraForward * inputDirection.z + cameraRight * inputDirection.x;

            // 平滑转向目标方向
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                                                      turnSpeed * Time.deltaTime);
            }

            // 应用移动速度
            _controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

        // 跳跃处理
        if (Input.GetButtonDown("Jump") && _isGrounded && !_isJumping)
        {
            _yVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
            _isJumping = true;
        }

        // 更新垂直速度
        _velocity.y = _yVelocity;

        // 应用移动
        _controller.Move(_velocity * Time.deltaTime);
    }

    // 重力应用
    private void ApplyGravity()
    {
        if (!_isGrounded)
        {
            _yVelocity -= gravity * Time.deltaTime;
        }
    }

    // 动画更新
    private void UpdateAnimation()
    {
        // 计算当前速度（在地平面上的移动速度）
        Vector3 horizontalVelocity = new Vector3(_controller.velocity.x, 0, _controller.velocity.z);
        float speed = horizontalVelocity.magnitude;

        // 设置动画参数（从当前值平滑过渡到目标值）
        Debug.Log(_controller.velocity);
        _animator.SetFloat(speedParameter, speed, animationTransition, Time.deltaTime);
    }

    // 相机更新
    private void UpdateCamera()
    {
        // 获取鼠标输入
        _currentX += Input.GetAxis("Mouse X") * cameraSensitivity;
        _currentY -= Input.GetAxis("Mouse Y") * cameraSensitivity;
        _currentY = Mathf.Clamp(_currentY, -10f, 80f); // 限制俯仰角度

        // 鼠标滚轮缩放
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        _desiredDistance = Mathf.Clamp(_desiredDistance - scroll * 5f,
                                     cameraMinDistance, cameraMaxDistance);

        // 计算相机旋转
        Quaternion rotation = Quaternion.Euler(_currentY, _currentX, 0);

        // 计算相机位置
        Vector3 position = rotation * (Vector3.zero - cameraOffset.normalized * _desiredDistance);

        // 计算目标位置（跟随目标位置）
        if (cameraFollowTarget != null)
        {
            position += cameraFollowTarget.position;
        }
        else
        {
            position += transform.position;
        }

        // 应用相机位置和旋转
        _mainCamera.transform.position = position;
        _mainCamera.transform.rotation = rotation;
    }

    // 绘制调试信息
    private void OnDrawGizmos()
    {
        if (cameraFollowTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(cameraFollowTarget.position, 0.2f);
        }
    }
}