using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class ThirdPersonController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;           // �����ٶ�
    public float turnSpeed = 10f;          // ת���ٶ�
    public float gravity = 9.81f;          // ����ֵ
    public float jumpHeight = 2f;          // ��Ծ�߶�
    public float groundCheckDistance = 0.1f; // ���������

    [Header("Camera Settings")]
    public Transform cameraFollowTarget;   // ��������Ŀ���
    public Vector3 cameraOffset = new Vector3(0, 1.5f, -3f); // ���ƫ��
    public float cameraDistance = 5f;      // �������
    public float cameraMinDistance = 1f;    // ����������
    public float cameraMaxDistance = 10f;   // �����Զ����
    public float cameraSensitivity = 2f;   // ���������

    [Header("Animation Settings")]
    public string speedParameter = "Speed"; // �����������е��ٶȲ���
    public float animationTransition = 0.1f; // ��������ʱ��

    // �������
    private CharacterController _controller;
    private Animator _animator;
    private Camera _mainCamera;

    // �ƶ�״̬
    private Vector3 _velocity;
    private bool _isGrounded;
    private bool _isJumping;
    private float _yVelocity;

    // ���״̬
    private float _currentX;
    private float _currentY;
    private float _desiredDistance;

    void Start()
    {
        // ��ȡ���
        _controller = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        _mainCamera = Camera.main;

        // ��ʼ���������
        _desiredDistance = cameraDistance;

        // �������
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // ������
        CheckGrounded();

        // ��������
        HandleInput();

        // ��������
        ApplyGravity();

        // ���¶���
        UpdateAnimation();
    }

    void LateUpdate()
    {
        // �������
        UpdateCamera();
    }

    // ������
    private void CheckGrounded()
    {
        // ʹ��CharacterController�Դ��ĵ�����
        _isGrounded = _controller.isGrounded;

        if (_isGrounded && _yVelocity < 0)
        {
            _yVelocity = -gravity * Time.deltaTime;
            _isJumping = false;
        }
    }

    // ���봦��
    private void HandleInput()
    {
        // ��ȡ���뷽��
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 inputDirection = new Vector3(horizontal, 0, vertical).normalized;

        // ������������ϵ��ƶ�
        if (inputDirection.magnitude >= 0.1f)
        {
            // ��ȡ�������
            Vector3 cameraForward = _mainCamera.transform.forward;
            Vector3 cameraRight = _mainCamera.transform.right;

            // �������������תӰ��
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            // �����ƶ���������������
            Vector3 moveDirection = cameraForward * inputDirection.z + cameraRight * inputDirection.x;

            // ƽ��ת��Ŀ�귽��
            if (moveDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation,
                                                      turnSpeed * Time.deltaTime);
            }

            // Ӧ���ƶ��ٶ�
            _controller.Move(moveDirection * moveSpeed * Time.deltaTime);
        }

        // ��Ծ����
        if (Input.GetButtonDown("Jump") && _isGrounded && !_isJumping)
        {
            _yVelocity = Mathf.Sqrt(jumpHeight * 2f * gravity);
            _isJumping = true;
        }

        // ���´�ֱ�ٶ�
        _velocity.y = _yVelocity;

        // Ӧ���ƶ�
        _controller.Move(_velocity * Time.deltaTime);
    }

    // ����Ӧ��
    private void ApplyGravity()
    {
        if (!_isGrounded)
        {
            _yVelocity -= gravity * Time.deltaTime;
        }
    }

    // ��������
    private void UpdateAnimation()
    {
        // ���㵱ǰ�ٶȣ��ڵ�ƽ���ϵ��ƶ��ٶȣ�
        Vector3 horizontalVelocity = new Vector3(_controller.velocity.x, 0, _controller.velocity.z);
        float speed = horizontalVelocity.magnitude;

        // ���ö����������ӵ�ǰֵƽ�����ɵ�Ŀ��ֵ��
        Debug.Log(_controller.velocity);
        _animator.SetFloat(speedParameter, speed, animationTransition, Time.deltaTime);
    }

    // �������
    private void UpdateCamera()
    {
        // ��ȡ�������
        _currentX += Input.GetAxis("Mouse X") * cameraSensitivity;
        _currentY -= Input.GetAxis("Mouse Y") * cameraSensitivity;
        _currentY = Mathf.Clamp(_currentY, -10f, 80f); // ���Ƹ����Ƕ�

        // ����������
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        _desiredDistance = Mathf.Clamp(_desiredDistance - scroll * 5f,
                                     cameraMinDistance, cameraMaxDistance);

        // ���������ת
        Quaternion rotation = Quaternion.Euler(_currentY, _currentX, 0);

        // �������λ��
        Vector3 position = rotation * (Vector3.zero - cameraOffset.normalized * _desiredDistance);

        // ����Ŀ��λ�ã�����Ŀ��λ�ã�
        if (cameraFollowTarget != null)
        {
            position += cameraFollowTarget.position;
        }
        else
        {
            position += transform.position;
        }

        // Ӧ�����λ�ú���ת
        _mainCamera.transform.position = position;
        _mainCamera.transform.rotation = rotation;
    }

    // ���Ƶ�����Ϣ
    private void OnDrawGizmos()
    {
        if (cameraFollowTarget != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(cameraFollowTarget.position, 0.2f);
        }
    }
}