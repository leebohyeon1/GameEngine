using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputManager _inputManager;
    private CharacterController _characterController;
    private PlayerMovement _playerMovement;
    private FSMBase _fsm;

    private Transform _cameraTransform;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    public Animator Animator { get; private set; }  

    #region Stat
    [Space(20f)]
    [SerializeField] private float _moveSpeed; //�̵� �ӵ�
    [SerializeField] private float _jumpHeight = 2f; // ���� ����
    [SerializeField] private float _gravity = -9.81f; // �߷�

    [SerializeField] private float _xRotationSpeed = 5f; // ȸ�� �ӵ�
    [SerializeField] private float _yRotationSpeed = 5f; // ȸ�� �ӵ�

    private float minOffsetY = 0.1f;  // ī�޶��� �ּ� Y ������
    private float maxOffsetY = 10f;  // ī�޶��� �ִ� Y ������
    #endregion

    private float _rotationY;
    private float _currentOffsetY;
    private Vector3 _velocity;
    private bool _isGrounded;

    private void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _characterController = GetComponent<CharacterController>();
        Animator = GetComponent<Animator>();

        SetFSM();
    }

    private void Start()
    {
        _inputManager = InputManager.Instance;
        _cameraTransform = Camera.main.transform;

        // �ʱ� ī�޶� offset Y ����
        _currentOffsetY = virtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset.y;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        CheckGround();

        _fsm.Update(this);

       
    }

    #region InitialFunc

    private void SetFSM()
    {
        _fsm = new FSMBase();
        _fsm.AddState("Idle", new IdleState(_fsm));
        _fsm.AddState("Move", new MoveState(_fsm));
        _fsm.AddState("JumpUp", new JumpUpState(_fsm));
        _fsm.AddState("Fall", new FallState(_fsm));
        _fsm.AddState("JumpDown", new JumpDownState(_fsm));


        _fsm.SetState("Idle", this);
    }

    #endregion

    #region MovementFunc

    public void Move()
    {
        // InputManager���� �Է� ���� �޾ƿ���
        Vector2 moveInput = _inputManager.MoveInput;

        // ī�޶��� ������ �������� �̵� ���� ����
        Vector3 forward = _cameraTransform.forward;
        Vector3 right = _cameraTransform.right;

        // ���� ���ʹ� �����Ͽ� ��鿡�� �̵��ϵ��� ��
        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        // ī�޶� ������ �������� �̵� ���� ���
        Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

        // �÷��̾� �̵�
        _characterController.Move(moveDirection * _moveSpeed * Time.deltaTime);

        // ���� Ʈ���� �Ķ���Ͱ� �Է�
        Animator.SetFloat("XDir", moveInput.x);
        Animator.SetFloat("YDir", moveInput.y);

        SetGravity();
    }

    public void Rotate()
    {
        Vector2 lookInput = _inputManager.LookInput;

        // �¿� ȸ�� ����
        _rotationY += lookInput.x * _xRotationSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, _rotationY, 0);

        // ī�޶��� Y ������ ���� (���Ʒ� ȸ�� ȿ��)
        _currentOffsetY -= lookInput.y * _yRotationSpeed * Time.deltaTime;
        _currentOffsetY = Mathf.Clamp(_currentOffsetY, minOffsetY, maxOffsetY);

        // CinemachineTransposer�� followOffset Y �� ������Ʈ
        var transposer = virtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        Vector3 offset = transposer.m_FollowOffset;
        offset.y = _currentOffsetY;
        transposer.m_FollowOffset = offset;
    }

    public void Jump()
    {
        if (_isGrounded)
        {
            // ���� �̵� ������ ���� ���Ϳ� �߰�
            Vector2 moveInput = _inputManager.MoveInput;
            Vector3 forward = _cameraTransform.forward;
            Vector3 right = _cameraTransform.right;

            forward.y = 0;
            right.y = 0;
            forward.Normalize();
            right.Normalize();

            // ���� �̵� ���� ����
            Vector3 moveDirection = (forward * moveInput.y + right * moveInput.x).normalized;

            // �̵� ����� ���� ���̸� �Բ� �ݿ�
            _velocity = moveDirection * _moveSpeed + Vector3.up * Mathf.Sqrt(_jumpHeight * -2f * _gravity);

            SetGravity();
            SetState("JumpUp");
        }
    }

    private void CheckGround()
    {
        // ���鿡 ��� �ִ��� Ȯ��
        _isGrounded = _characterController.isGrounded;
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.x = 0f;
            _velocity.z = 0f;
            _velocity.y = -1f;
        }
        else if(!_isGrounded)
        {
            SetGravity();
            if (_velocity.y < -1.5f && !_fsm.IsSameState("Fall"))
            {
                SetState("Fall");
            }
        }
        
    }
    #endregion

    #region SetFunc

    public void SetState(string key)
    {
        _fsm.SetState(key, this);
    }

    private void SetGravity()
    {
        // �߷� ����
        _velocity.y += _gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
    }
    #endregion

    #region GetFunc
    public bool GetIsGround()
    {
        return _isGrounded;
    }

    #endregion

}
